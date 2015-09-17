using DotWrapper.Utils;
using NUnit.Framework;
using System;
using DotWrapper.Resolve;

namespace DotWrapper.Test
{
    [TestFixture]
    public class ChunkTest
    {
        [SetUp]
        public void Init()
        {
        }

        [Test(Description = "Data constructor.")]
        public void Constructor1Test()
        {
            Chunk chunk = new Chunk("test");
            Assert.True(chunk.Name == "test");
            Assert.True(chunk.Data.ShallowEquals(new byte[] {}));
            Assert.True(chunk.ResolveChain != null);
        }

        [Test(Description = "Data constructor with empty name.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor1Test2()
        {
            new Chunk(null);
        }

        [Test(Description = "Password constructor with default resolve chain.")]
        public void Constructor2Test1()
        {
            const string password = "@S1mPlePa$$w0rd";
            Chunk chunk = new Chunk("test", password);
            Assert.True(chunk.Name == "test");
            Assert.True(chunk.Data.ShallowEquals(new byte[] {}));
            Assert.True(chunk.ResolveChain != null);
            Assert.True(chunk.ResolveChain is CompressionResolver); 
            Assert.True(chunk.ResolveChain.ChildResolver is CryptoResolver);
            Assert.True(chunk.ResolveChain.ChildResolver.ChildResolver is IdResolver);
            Assert.True(((CryptoResolver) chunk.ResolveChain.ChildResolver).Password == password);
        }

        [Test(Description = "Password constructor with empty password.")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Constructor2Test2()
        {
            new Chunk("test", null as string);
        }

        [Test(Description = "Resolve chain constructor with empty chain.")]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Constructor3Test1()
        {
            new Chunk("test", null as DataResolver);
        }
    }
}
