using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using DotWrapper.Resolve;

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
        ///     In order to decrypt the data, one needs the password, salt and IV.
        /// </summary>
        /// <param name="data">The data that will be encrypted.</param>
        /// <param name="password">The password for encrypting.</param>
        /// <param name="iv">Output for generated Initialization Vector.</param>
        /// <returns></returns>
        [Pure]
        public static byte[] Encrypt(this byte[] data, string password, out byte[] iv)
        {
            Debug.Assert(data != null);
            Debug.Assert(!string.IsNullOrEmpty(password));
            byte[] res;
            // AesEncryptor sets the key and iv values.
            ICryptoTransform encryptor = AesEncryptor(password, out iv);
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

        [Pure]
        public static byte[] Decrypt(this byte[] data, string password, byte[] iv)
        {
            Debug.Assert(data != null);
            Debug.Assert(!string.IsNullOrEmpty(password));
            Debug.Assert(!ArrayUtils.IsNullOrEmpty(iv));
            byte[] res;
            ICryptoTransform decryptor = AesDecryptor(password, iv);
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
        private static ICryptoTransform AesEncryptor(string password, out byte[] iv)
        {
            Debug.Assert(!string.IsNullOrEmpty(password));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, CryptoResolver.Salt);
            SymmetricAlgorithm algo = GetCryptoAlgorithm();
            algo.Key = key.GetBytes(algo.KeySize / 8);
            iv = algo.IV.Replace((byte) 0, (byte) 1); // Generates IV.
            algo.IV = iv;
            algo.IV = iv;
            Debug.Print("ENCRYPT IV=" + string.Join("-", iv));
            algo.Padding = PaddingMode.PKCS7;
            return algo.CreateEncryptor(algo.Key, algo.IV);
        }

        [Pure]
        private static ICryptoTransform AesDecryptor(string password, byte[] iv)
        {
            Debug.Assert(!string.IsNullOrEmpty(password));
            Debug.Assert(!ArrayUtils.IsNullOrEmpty(iv));
            Debug.Print("DECRYPT IV=" + string.Join("-", iv));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, CryptoResolver.Salt);
            SymmetricAlgorithm algo = GetCryptoAlgorithm();
            algo.IV = iv;
            algo.Key = key.GetBytes(algo.KeySize / 8);
            algo.Padding = PaddingMode.PKCS7;
            return algo.CreateDecryptor(algo.Key, algo.IV);
        }

        private static SymmetricAlgorithm GetCryptoAlgorithm()
        {
            switch (CryptoResolver.Crypto)
            {
                case CryptoResolver.CryptoAlgorithm.AES:
                    return new RijndaelManaged();

                case CryptoResolver.CryptoAlgorithm.DES:
                    return new DESCryptoServiceProvider();

                case CryptoResolver.CryptoAlgorithm.TripleDES:
                    return new TripleDESCryptoServiceProvider();

                case CryptoResolver.CryptoAlgorithm.RC2:
                    return new RC2CryptoServiceProvider();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion CryptoTransform (encryptors/decryptors)
    }
}
