namespace ArmatSoftware.Code.Engine.Logger.File.Tests;

[TestFixture]
public class FileLoggerWriteToFileTests : FileLoggerTestsBase
{
    private const string TestLogEntry = "some log entry";
    
    [Test]
    public void Should_Log_Info()
    {
        Assert.That(() =>
        {
            Build();

            TestSubject = new CodeEngineFileLogger(LogWriter);
            
            TestSubject.Info(TestLogEntry);
            
            Assert.IsTrue(LogSink.Count == 1);
            Assert.IsTrue(LogSink[0].Contains("INFO"));
            Assert.IsTrue(LogSink[0].Contains(TestLogEntry));

        }, Throws.Nothing);
    }
    
    
    [Test]
    public void Should_Log_Warning()
    {
        Assert.That(() =>
        {
            Build();

            TestSubject = new CodeEngineFileLogger(LogWriter);
            
            TestSubject.Warning(TestLogEntry);
            
            Assert.IsTrue(LogSink.Count == 1);
            Assert.IsTrue(LogSink[0].Contains("WARN"));
            Assert.IsTrue(LogSink[0].Contains(TestLogEntry));

        }, Throws.Nothing);
    }
    
    [Test]
    public void Should_Log_Error()
    {
        Assert.That(() =>
        {
            Build();

            TestSubject = new CodeEngineFileLogger(LogWriter);
            var e = new Exception(TestLogEntry);
            
            TestSubject.Error(TestLogEntry);
            
            Assert.IsTrue(LogSink.Count == 2);
            Assert.IsTrue(LogSink[0].Contains("ERROR"));
            Assert.IsTrue(LogSink[0].Contains(TestLogEntry));
        }, Throws.Nothing);
    }
}