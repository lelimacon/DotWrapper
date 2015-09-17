using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using DotWrapper.Utils;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Encryption/decryption resolver.
    /// </summary>
    public class CryptoResolver : DataResolver
    {
        /// <summary>
        ///     Byte array used with password to generate the key.
        /// </summary>
        public static byte[] Salt = Encoding.ASCII.GetBytes("sAl7I$n0Lie");

        public enum CryptoAlgorithm
        {
            AES,
            DES,
            TripleDES,
            RC2
        }

        /// <summary>
        ///     The cryptographic algorithm used for encrypting and decrypting.
        ///     Default is AES.
        /// </summary>
        public CryptoAlgorithm Crypto { get; set; } = CryptoAlgorithm.AES;

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

        public CryptoResolver(byte[] data, CryptoAlgorithm crypto, string password, bool renewPassword = false)
            : this(new IdResolver(data), crypto, password, renewPassword)
        {
        }

        public CryptoResolver(byte[] data, bool renewPassword = true)
            : this(new IdResolver(data), CryptoAlgorithm.AES, CryptoUtils.GeneratePassword(16), renewPassword)
        {
        }

        public CryptoResolver(DataResolver childResolver, CryptoAlgorithm crypto = CryptoAlgorithm.AES,
            bool renewPassword = true)
            : this(childResolver, crypto, CryptoUtils.GeneratePassword(16), renewPassword)
        {
        }

        public CryptoResolver(DataResolver childResolver, CryptoAlgorithm crypto, string password, bool renewPassword = false)
            : base(childResolver)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (childResolver == null)
                throw new ArgumentNullException(nameof(childResolver));
            RenewPassword = renewPassword;
            Password = password;
        }

        [Pure]
        internal override byte[] Pack()
        {
            byte[] iv;
            byte[] childData = ChildResolver.Pack();
            byte[] array = childData.Encrypt(Crypto, Password, out iv);
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.CryptoResolver);
                stream.Write((int) Crypto);
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
            CryptoAlgorithm crypto;
            string password;
            bool renewPassword;
            byte[] childData;
            using (MemoryStream stream = new MemoryStream(data))
            {
                crypto = (CryptoAlgorithm) stream.ReadInt32();
                password = stream.ReadString();
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Empty password field");
                renewPassword = stream.ReadInt32() == 1;
                byte[] iv = stream.ReadByteArray();
                if (ArrayUtils.IsNullOrEmpty(iv))
                    throw new ArgumentException("Empty IV field");
                childData = stream.ReadToEnd().Decrypt(crypto, password, iv);
            }
            DataResolver child = DataResolver.Unpack(childData);
            return renewPassword
                ? new CryptoResolver(child)
                : new CryptoResolver(child, crypto, password, false);
        }
    }
}
