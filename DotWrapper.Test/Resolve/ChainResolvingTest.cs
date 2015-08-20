using DotWrapper.Resolve;
using DotWrapper.Utils;
using NUnit.Framework;
using System.Text;

namespace DotWrapper.Test.Resolve
{
    [TestFixture]
    public class ChainResolvingTest
    {
        public const string TestFolder = "../../TestFiles/";

        [Test(Description = "Resolve File then Base64.")]
        public void FileBase64Test()
        {
            const string path = TestFolder + "myBase64message.txt";
            var data = Encoding.ASCII.GetBytes("~5" + path);
            var actual = DataResolver.Unpack(data).ClosingResolver.Data;
            var expected = Encoding.ASCII.GetBytes("DotWrapped!");
            Assert.True(actual.ShallowEquals(expected));
        }
    }
}
