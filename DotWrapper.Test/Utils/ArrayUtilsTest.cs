using DotWrapper.Utils;
using NUnit.Framework;

namespace DotWrapper.Test.Utils
{
    [TestFixture]
    public class ArrayUtilsTest
    {
        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void ShallowEqualsTest()
        {
            Assert.True((null as int[]).ShallowEquals(null));
            Assert.False(new int[] {}.ShallowEquals(null));
            Assert.True(new int[] {}.ShallowEquals(new int[] {}));
            Assert.True(new[] {0, 2, 3}.ShallowEquals(new[] {0, 2, 3}));
            Assert.False(new[] {0, 2, 3}.ShallowEquals(new[] {1, 2, 3}));
        }

        [Test]
        public void ReplaceTest()
        {
            int[] original = {0, 2, 3, 0, 2};
            int[] originalSave = {0, 2, 3, 0, 2};
            int[] result = original.Replace(0, 1);
            int[] expected = {1, 2, 3, 1, 2};
            // Check that original array was not altered.
            Assert.True(original.ShallowEquals(originalSave));
            // Check that the result is as expected.
            Assert.True(result.ShallowEquals(expected));
        }
    }
}
