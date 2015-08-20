using System.Security.Cryptography;
using System.Text;
using DotWrapper.Resolve;
using DotWrapper.Utils;
using NUnit.Framework;

namespace DotWrapper.Test.Utils
{
    [TestFixture]
    public class CryptoUtilsTest
    {
        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void GeneratePasswordTest()
        {
            const int size = 8;
            string p1 = CryptoUtils.GeneratePassword(size);
            Assert.AreEqual(p1.Length, size);
            string p2 = CryptoUtils.GeneratePassword(size);
            Assert.AreEqual(p2.Length, size);
            Assert.AreNotEqual(p1, p2);
        }

        [Test(Description = "Encrypt an empty array.")]
        public void EncryptTest()
        {
            const string password = "@S1mPlePa$$w0rd";
            byte[] iv;
            byte[] encrypted = new byte[] { }.Encrypt(password, out iv);
            Assert.False(ArrayUtils.IsNullOrEmpty(iv));
            Assert.True(encrypted.Length == 0);
        }

        [Test(Description = "Decrypt an empty array.")]
        public void DecryptTest()
        {
            const string password = "@S1mPlePa$$w0rd";
            byte[] iv = new RijndaelManaged().IV; // Will generate IV.
            byte[] decrypted = new byte[] {}.Decrypt(password, iv);
            Assert.True(decrypted.Length == 0);
        }

        [Test(Description = "Encrypt then Decrypt in AES.")]
        public void EncryptDecryptAesTest()
        {
            CryptoResolver.Crypto = CryptoResolver.CryptoAlgorithm.AES;
            const string txt = "7hîs *s #a_$érioü$_0ne.";
            const string password = "@S1mPlePa$$w0rd";
            byte[] original = Encoding.Default.GetBytes(txt);
            byte[] iv;
            byte[] encrypted = original.Encrypt(password, out iv);
            Assert.False(ArrayUtils.IsNullOrEmpty(iv));
            byte[] decrypted = encrypted.Decrypt(password, iv);
            Assert.True(decrypted.ShallowEquals(original));
        }

        [Test(Description = "Encrypt then Decrypt in DES.")]
        public void EncryptDecryptDesTest()
        {
            CryptoResolver.Crypto = CryptoResolver.CryptoAlgorithm.DES;
            const string txt = "7hîs *s #a_$érioü$_0ne.";
            const string password = "@S1mPlePa$$w0rd";
            byte[] original = Encoding.Default.GetBytes(txt);
            byte[] iv;
            byte[] encrypted = original.Encrypt(password, out iv);
            Assert.False(ArrayUtils.IsNullOrEmpty(iv));
            byte[] decrypted = encrypted.Decrypt(password, iv);
            Assert.True(decrypted.ShallowEquals(original));
        }

        [Test(Description = "Encrypt then Decrypt in Triple DES.")]
        public void EncryptDecryptTripleDesTest()
        {
            CryptoResolver.Crypto = CryptoResolver.CryptoAlgorithm.TripleDES;
            const string txt = "7hîs *s #a_$érioü$_0ne.";
            const string password = "@S1mPlePa$$w0rd";
            byte[] original = Encoding.Default.GetBytes(txt);
            byte[] iv;
            byte[] encrypted = original.Encrypt(password, out iv);
            Assert.False(ArrayUtils.IsNullOrEmpty(iv));
            byte[] decrypted = encrypted.Decrypt(password, iv);
            Assert.True(decrypted.ShallowEquals(original));
        }

        [Test(Description = "Encrypt then Decrypt in RC2.")]
        public void EncryptDecryptRc2Test()
        {
            CryptoResolver.Crypto = CryptoResolver.CryptoAlgorithm.RC2;
            const string txt = "7hîs *s #a_$érioü$_0ne.";
            const string password = "@S1mPlePa$$w0rd";
            byte[] original = Encoding.Default.GetBytes(txt);
            byte[] iv;
            byte[] encrypted = original.Encrypt(password, out iv);
            Assert.False(ArrayUtils.IsNullOrEmpty(iv));
            byte[] decrypted = encrypted.Decrypt(password, iv);
            Assert.True(decrypted.ShallowEquals(original));
        }

    }
}
