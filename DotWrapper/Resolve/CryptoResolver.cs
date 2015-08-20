using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Encryption/decryption resolver.
    /// </summary>
    public class CryptoResolver : DataResolver
    {
        #region Static "properties"

        public enum CryptoAlgorithm
        {
            AES,
            DES,
            TripleDES,
            RC2
        }

        /// <summary>
        ///     The cryptography algorithm used for encrypting and decrypting.
        ///     Default is AES.
        /// </summary>
        public static CryptoAlgorithm Crypto = CryptoAlgorithm.AES;

        /// <summary>
        ///     Byte array used with password to generate the key.
        /// </summary>
        public static byte[] Salt = Encoding.ASCII.GetBytes("sAl7I$n0Lie");

        /// <summary>
        ///     The specific set of bytes that defines a wrap.
        /// </summary>
        public static byte[] Signature = {0, (byte) '=', 0};

        #endregion Static "properties"

        /// <summary>
        ///     Gets or sets the password used for encryption.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets whether a new password is generated.
        /// </summary>
        public bool RenewPassword { [Pure] get; set; }

        internal override IdResolver ClosingResolver
        {
            get { return ChildResolver.ClosingResolver; }
        }

        public CryptoResolver(byte[] data, string password, bool renewPassword = false)
            : this(new IdResolver(data), password, renewPassword)
        {
        }

        public CryptoResolver(byte[] data, bool renewPassword = true)
            : this(new IdResolver(data), CryptoUtils.GeneratePassword(16), renewPassword)
        {
        }

        public CryptoResolver(DataResolver childResolver, bool renewPassword = true)
            : this(childResolver, CryptoUtils.GeneratePassword(16), renewPassword)
        {
        }

        public CryptoResolver(DataResolver childResolver, string password, bool renewPassword = false)
            : base(childResolver)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");
            if (childResolver == null)
                throw new ArgumentNullException("childResolver");
            RenewPassword = renewPassword;
            Password = password;
        }

        [Pure]
        internal override byte[] Pack()
        {
            byte[] iv;
            byte[] childData = ChildResolver.Pack();
            byte[] array = childData.Encrypt(Password, out iv);
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.CryptoResolver);
                stream.Write(Password);
                stream.Write(RenewPassword ? 1 : 0);
                stream.Write(iv, false);
                stream.Write(array);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static CryptoResolver Unpack(byte[] data)
        {
            string password;
            bool renewPassword;
            byte[] childData;
            using (MemoryStream stream = new MemoryStream(data))
            {
                password = stream.ReadString();
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Empty password field");
                renewPassword = stream.ReadInt32() == 1;
                byte[] iv = stream.ReadByteArray();
                if (ArrayUtils.IsNullOrEmpty(iv))
                    throw new ArgumentException("Empty IV field");
                childData = stream.ReadToEnd().Decrypt(password, iv);
            }
            DataResolver child = DataResolver.Unpack(childData);
            return renewPassword
                ? new CryptoResolver(child)
                : new CryptoResolver(child, password, false);
        }
    }
}
