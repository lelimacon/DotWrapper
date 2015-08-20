using DotWrapper.Resolve;
using DotWrapper.Utils;
using NUnit.Framework;
using System;
using System.Text;

namespace DotWrapper.Test.Resolve
{
    [TestFixture]
    public class Base64ResolverTest
    {
        [Test(Description = "Default constructor.")]
        public void Constructor1Test1()
        {
            var resolver = new Base64Resolver();
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.ChildResolver.ChildResolver == null);
        }

        [Test(Description = "Data constructor with data.")]
        public void Constructor1Test2()
        {
            var data = new byte[] { 1, 2, 3 };
            var resolver = new Base64Resolver(data);
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.ChildResolver.ChildResolver == null);
            Assert.True(resolver.ClosingResolver.Data == data);
        }

        [Test(Description = "Resolve chain constructor with IdResolver.")]
        public void Constructor2Test1()
        {
            var data = new byte[] { 1, 2, 3 };
            var resolver = new Base64Resolver(new IdResolver(data));
            Assert.True(resolver.ChildResolver is IdResolver);
            Assert.True(resolver.ChildResolver.ChildResolver == null);
            Assert.True(resolver.ClosingResolver.Data == data);
        }

        [Test(Description = "Resolve chain constructor with empty chain.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor2Test2()
        {
            new Base64Resolver(null as DataResolver);
        }

        [Test(Description = "Packing string to base64.")]
        public void PackTest()
        {
            var data = Encoding.ASCII.GetBytes("DotWrapped!");
            var resolver = new Base64Resolver(data);
            var actual = resolver.Pack();
            var expected = Encoding.ASCII.GetBytes("~4fjBEb3RXcmFwcGVkIQ==");
            Assert.True(actual.ShallowEquals(expected));
        }

        [Test(Description = "Unpacking base64 to string.")]
        public void UnpackTest()
        {
            var data = Encoding.ASCII.GetBytes("~4fjBEb3RXcmFwcGVkIQ==");
            var actual = DataResolver.Unpack(data).ClosingResolver.Data;
            var expected = Encoding.ASCII.GetBytes("DotWrapped!");
            Assert.True(actual.ShallowEquals(expected));
        }
    }
}
