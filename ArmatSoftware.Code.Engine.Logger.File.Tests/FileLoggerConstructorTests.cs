namespace ArmatSoftware.Code.Engine.Logger.File.Tests
{
    [TestFixture]
    public class FileLoggerConstructorTests : FileLoggerTestsBase
    {
        [Test]
        public void Should_Construct_With_TextWriter()
        {
            Assert.That(() =>
            {
                Build();
                TestSubject = new CodeEngineFileLogger(LogWriter);
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Construct_With_TextWriter_Null()
        {
            Assert.That(() =>
            {
                Build();
                LogWriter = null;
                TestSubject = new CodeEngineFileLogger(LogWriter);
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Construct_With_FileStream()
        {
            Assert.That(() =>
            {
                Build();
                TestSubject = new CodeEngineFileLogger(LogStream);
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Construct_With_Configuration()
        {
            Assert.That(() =>
            {
                Build();
                TestSubject = new CodeEngineFileLogger(Configuration);
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Not_Construct_With_Configuration_Null()
        {
            Assert.That(() =>
            {
                Configuration = null;
                LogWriter = null;
                TestSubject = new CodeEngineFileLogger(Configuration);
            }, Throws.ArgumentNullException);
        }
    }
}