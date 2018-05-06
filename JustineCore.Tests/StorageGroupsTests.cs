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
            ds.StoreObject(expectedA, "MyFirstGroup", "Message");
            ds.StoreObject(expectedB, "MySecondGroup", "Message");
            Assert.AreEqual(expectedA, ds.RestoreObject<string>("MyFirstGroup", "Message"));
            Assert.AreEqual(expectedB, ds.RestoreObject<string>("MySecondGroup", "Message"));
        }
    }
}
