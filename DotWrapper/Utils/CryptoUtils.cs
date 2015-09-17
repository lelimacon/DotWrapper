using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using DotWrapper.Resolve;
using CryptoAlg = DotWrapper.Resolve.CryptoResolver.CryptoAlgorithm;

namespace DotWrapper.Utils
{
    /// <summary>
    ///     Wrapper cryptography class.
    /// </summary>
    internal static class CryptoUtils
    {
        /// <summary>
        ///     Generates a password using the Security.Cryptography random generator.
        /// </summary>
        /// <param name="bytes">Password length specification.</param>
        /// <returns>The resulting password.</returns>
        [Pure]
        public static string GeneratePassword(int bytes)
        {
            Debug.Assert(bytes > 0);
            RandomNumberGenerator generator = new RNGCryptoServiceProvider();
            byte[] res = new byte[bytes];
            generator.GetBytes(res);
            return new string(Array.ConvertAll(res, b => (char) b));
        }

        /// <summary>
        ///     Will encrypt a byte array.
        /// </summary>
        /// <param name="data">The data that will be encrypted.</param>
        /// <param name="crypto">The cryptography algorigthm.</param>
        /// <param name="password">The password for encrypting.</param>
        /// <param name="iv">Output for generated Initialization Vector.</param>
        /// <returns>The encrypted byte array.</returns>
        [Pure]
        public static byte[] Encrypt(this byte[] data, CryptoAlg crypto, string password, out byte[] iv)
        {
            Debug.Assert(data != null);
            Debug.Assert(!string.IsNullOrEmpty(password));
            byte[] res;
            // AesEncryptor sets the key and iv values.
            ICryptoTransform encryptor = AesEncryptor(password, crypto, out iv);
            if (data.Length == 0)
                return new byte[] {};
            using (MemoryStream memoryStream = new MemoryStream())
            {
                CryptoStreamMode mode = CryptoStreamMode.Write;
                using (Stream encryptStream = new CryptoStream(memoryStream, encryptor, mode))
                    encryptStream.Write(data);
                res = memoryStream.ToArray();
            }
            return res;
        }

        /// <summary>
        ///     Will decrypt a byte array.
        /// </summary>
        /// <param name="data">The data that will be encrypted.</param>
        /// <param name="crypto">The cryptography algorigthm.</param>
        /// <param name="password">The password for encrypting.</param>
        /// <param name="iv">Output for generated Initialization Vector.</param>
        /// <returns>The decrypted byte array.</returns>
        [Pure]
        public static byte[] Decrypt(this byte[] data, CryptoAlg crypto, string password, byte[] iv)
        {
            Debug.Assert(data != null);
            Debug.Assert(!string.IsNullOrEmpty(password));
            Debug.Assert(!ArrayUtils.IsNullOrEmpty(iv));
            byte[] res;
            ICryptoTransform decryptor = AesDecryptor(password, crypto, iv);
            if (data.Length == 0)
                return new byte[] {};
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                CryptoStreamMode mode = CryptoStreamMode.Read;
                using (Stream decryptStream = new CryptoStream(memoryStream, decryptor, mode))
                {
                    res = decryptStream.ReadByteArray(data.Length);
                }
            }
            return res;
        }

        #region CryptoTransform (encryptors/decryptors)

        [Pure]
        private static ICryptoTransform AesEncryptor(string password, CryptoAlg crypto, out byte[] iv)
        {
            Debug.Assert(!string.IsNullOrEmpty(password));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, CryptoResolver.Salt);
            SymmetricAlgorithm algo = GetCryptoAlgorithm(crypto);
            algo.Key = key.GetBytes(algo.KeySize/8);
            iv = algo.IV.Replace((byte) 0, (byte) 1); // Generates IV.
            algo.IV = iv;
            algo.IV = iv;
            Debug.Print("ENCRYPT IV=" + string.Join("-", iv));
            algo.Padding = PaddingMode.PKCS7;
            return algo.CreateEncryptor(algo.Key, algo.IV);
        }

        [Pure]
        private static ICryptoTransform AesDecryptor(string password, CryptoAlg crypto, byte[] iv)
        {
            Debug.Assert(!string.IsNullOrEmpty(password));
            Debug.Assert(!ArrayUtils.IsNullOrEmpty(iv));
            Debug.Print("DECRYPT IV=" + string.Join("-", iv));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, CryptoResolver.Salt);
            SymmetricAlgorithm algo = GetCryptoAlgorithm(crypto);
            algo.IV = iv;
            algo.Key = key.GetBytes(algo.KeySize/8);
            algo.Padding = PaddingMode.PKCS7;
            return algo.CreateDecryptor(algo.Key, algo.IV);
        }

        private static SymmetricAlgorithm GetCryptoAlgorithm(CryptoAlg crypto)
        {
            switch (crypto)
            {
                case CryptoAlg.AES:
                    return new RijndaelManaged();
                case CryptoAlg.DES:
                    return new DESCryptoServiceProvider();
                case CryptoAlg.TripleDES:
                    return new TripleDESCryptoServiceProvider();
                case CryptoAlg.RC2:
                    return new RC2CryptoServiceProvider();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion CryptoTransform (encryptors/decryptors)
    }
}
