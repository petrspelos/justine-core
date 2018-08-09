using JustineCore.Storage;
using Xunit;

namespace JustineCore.Tests
{
    public class StorageGroupsTests
    {
        [Fact]
        public void StorageGroupsTest()
        {
            const string expectedA = "Hey";
            const string expectedB = "Hey2";

            var ds = TestUnity.Resolve<IDataStorage>();
            ds.Store(expectedA, "MyFirstGroup", "Message");
            ds.Store(expectedB, "MySecondGroup", "Message");
            Assert.Equal(expectedA, ds.Get<string>("MyFirstGroup", "Message"));
            Assert.Equal(expectedB, ds.Get<string>("MySecondGroup", "Message"));
        }
    }
}
