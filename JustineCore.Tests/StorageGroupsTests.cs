using JustineCore.Storage;
using NUnit.Framework;

namespace JustineCore.Tests
{
    internal class StorageGroupsTests
    {
        [Test]
        public void StorageGroupsTest()
        {
            const string expectedA = "Hey";
            const string expectedB = "Hey2";

            var ds = TestUnity.Resolve<IDataStorage>();
            ds.Store(expectedA, "MyFirstGroup", "Message");
            ds.Store(expectedB, "MySecondGroup", "Message");
            Assert.AreEqual(expectedA, ds.Get<string>("MyFirstGroup", "Message"));
            Assert.AreEqual(expectedB, ds.Get<string>("MySecondGroup", "Message"));
        }
    }
}
