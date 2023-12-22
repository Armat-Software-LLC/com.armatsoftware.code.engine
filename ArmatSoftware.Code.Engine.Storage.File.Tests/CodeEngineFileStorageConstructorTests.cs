using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests
{
    [TestFixture]
    public class CodeEngineFileStorageConstructorTests : CodeEngineFileStorageTestsBuilder
    {
        [Test]
        public void Should_Construct_With_Valid_Parameters()
        {
            Assert.That(() => 
            {
                Build(); 
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Fail_With_Null_Configuration()
        {
            Assert.That(() =>
            {
                new CodeEngineFileStorage(null, Logger);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Logger()
        {
            Assert.That(() =>
            {
                new CodeEngineFileStorage(Configuration, null);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Parameters()
        {
            Assert.That(() =>
            {
                new CodeEngineFileStorage(null, null);
            }, Throws.ArgumentNullException);
        }
    }
}