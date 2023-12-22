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

        public class CustomTestType
        {
            // using for test only
        }
    }
}