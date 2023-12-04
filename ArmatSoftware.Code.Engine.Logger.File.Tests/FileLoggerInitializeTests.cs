namespace ArmatSoftware.Code.Engine.Logger.File.Tests;

[TestFixture]
public class FileLoggerInitializeTests : FileLoggerTestsBase
{
    [Test]
    public void Should_Initialize_File()
    {
        Assert.That(() =>
        {
            string logFilePath = CodeEngineFileLogger.LogFilePathKey;
            new CodeEngineFileLogger(logFilePath);
        }, Throws.Nothing);        
    }

    [Test] public void Should_Fail_Initialize_File_Null()
    {
        Assert.That(() =>
        {
            string logFilePath = null;
            new CodeEngineFileLogger(logFilePath);
        }, Throws.ArgumentNullException);        
    }
    
    [Test] public void Should_Fail_Initialize_File_Empty()
    {
        Assert.That(() =>
        {
            string logFilePath = string.Empty;
            new CodeEngineFileLogger(logFilePath);
        }, Throws.ArgumentException);        
    }
}