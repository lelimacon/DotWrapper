using DotWrapper.Utils;
using NUnit.Framework;

namespace DotWrapper.Test.Utils
{
    [TestFixture]
    public class ByteTransformUtilsTest
    {
        [Test]
        public void NoneTest()
        {
            byte original = 42;
            byte expected = 42;
            byte result = original.Transform(ByteTransform.None);
            Assert.True(result == expected);
        }

        [Test]
        public void XorTest()
        {
            byte original = 42; // 00101010
            byte expected = 213; // 11010101
            byte result = original.Transform(ByteTransform.Xor);
            Assert.True(result == expected);
        }

        [Test]
        public void EvenXorTest()
        {
            byte original = 42;  // 00101010
            byte expected = 128; // 10000000
            byte result = original.Transform(ByteTransform.EvenXor);
            Assert.True(result == expected);
        }

        [Test]
        public void OddXorTest()
        {
            byte original = 42;  // 00101010
            byte expected = 127; // 01111111
            byte result = original.Transform(ByteTransform.OddXor);
            Assert.True(result == expected);
        }

        [Test]
        public void ReverseTest()
        {
            byte original = 42; // 00101010
            byte expected = 84; // 01010100
            byte result = original.Transform(ByteTransform.Reverse);
            Assert.True(result == expected);
        }
    }
}
