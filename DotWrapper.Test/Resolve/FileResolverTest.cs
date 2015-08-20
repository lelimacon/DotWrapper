using System;
using System.IO;
using System.Text;
using DotWrapper.Resolve;
using DotWrapper.Utils;
using NUnit.Framework;

namespace DotWrapper.Test.Resolve
{
    [TestFixture]
    public class FileResolverTest
    {
        public const string TestFolder = "../../TestFiles/";

        [Test(Description = "Path constructor with null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor1Test1()
        {
            var resolver = new FileResolver(null);
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.ChildResolver.ChildResolver == null);
        }

        [Test(Description = "Path constructor with real path.")]
        public void Constructor1Test2()
        {
            const string path = "mypath";
            var resolver = new FileResolver(path);
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.Path == path);
            Assert.True(resolver.ClosingResolver.Data.Length == 0);
        }

        [Test(Description = "Resolve chain constructor with empty chain.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor2Test1()
        {
            new Base64Resolver(null as DataResolver);
        }

        [Test(Description = "Resolve chain constructor with IdResolver.")]
        public void Constructor2Test2()
        {
            const string path = "mypath";
            var resolver = new FileResolver(path, new IdResolver());
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.Path == path);
            Assert.True(resolver.ClosingResolver.Data.Length == 0);
        }

        [Test(Description = "Packing text file.")]
        public void PackTest()
        {
            const string path = TestFolder + "plaintext.txt";
            var data = Encoding.ASCII.GetBytes("DotWrapped!");
            var resolver = new FileResolver(path, data);
            var actual = resolver.Pack();
            var expected = Encoding.ASCII.GetBytes("~5" + path);
            Assert.True(actual.ShallowEquals(expected));
        }

        [Test(Description = "Unacking text file.")]
        public void UnpackTest1()
        {
            const string path = TestFolder + "plaintext.txt";
            var data = Encoding.ASCII.GetBytes("~5" + path);
            var actual = DataResolver.Unpack(data).ClosingResolver.Data;
            var expected = Encoding.ASCII.GetBytes("DotWrapped!");
            Assert.True(actual.ShallowEquals(expected));
        }

        [Test(Description = "Unacking unexisting text file.")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void UnpackTest2()
        {
            const string path = TestFolder + "myghostfile.null";
            var data = Encoding.ASCII.GetBytes("~5" + path);
            DataResolver.Unpack(data);
        }
    }
}
