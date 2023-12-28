using System;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests
{
    [TestFixture]
    public class CodeEngineFileStorageStoreTests : CodeEngineFileStorageTestsBuilder
    {
        [Test]
        public void Should_Store_With_Valid_Parameters()
        {
            Assert.That(() =>
            {
                var target = Build();
                target.Store(typeof(CustomTestType), Guid.NewGuid(), "dd");
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Fail_With_Empty_Code()
        {
            Assert.That(() =>
            {
                var target = Build();
                target.Store(typeof(CustomTestType), Guid.NewGuid(), string.Empty);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Code()
        {
            Assert.That(() =>
            {
                var target = Build();
                target.Store(typeof(CustomTestType), Guid.NewGuid(), null);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Replace_Same_File()
        {
            Assert.That(() =>
            {
                var executorId = Guid.NewGuid();
                var target = Build();
                target.Store(typeof(CustomTestType), executorId, "one");
                target.Store(typeof(CustomTestType), executorId, "two");
                var code = target.Retrieve(typeof(CustomTestType), executorId);
                Assert.AreEqual(code, "two");
            }, Throws.Nothing);
        }
        
        // using for test only
        public class CustomTestType
        { }
    }
}