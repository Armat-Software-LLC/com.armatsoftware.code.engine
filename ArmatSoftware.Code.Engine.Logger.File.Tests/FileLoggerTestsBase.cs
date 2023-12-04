using ArmatSoftware.Code.Engine.Core.Logging;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ArmatSoftware.Code.Engine.Logger.File.Tests;

public class FileLoggerTestsBase
{
    protected readonly string LogFilePath = $"{Path.GetTempPath()}/test.log";
    
    protected ICodeEngineLogger TestSubject { get; set; }
    protected TextWriter? LogWriter { get; set; }
    protected Mock<TextWriter> LogWriterMock { get; private set; }
    protected FileStream? LogStream { get; set; }
    protected Mock<FileStream> LogStreamMock { get; set; }
    protected List<string> LogSink { get; private set; }
    protected IConfiguration? Configuration { get; set; }
    protected Mock<IConfiguration> ConfigurationMock { get; private set; }
    
    protected Mock<IConfigurationSection> ConfigurationSectionMock { get; set; }
    
    [SetUp]
    public void Setup()
    {
        LogSink = new List<string>();
        
        LogWriterMock = new Mock<TextWriter>();
        LogWriterMock.Setup(fsm => fsm.WriteLine(It.IsAny<string>()))
            .Callback((string message) => ReceiveLog(message));

        LogStreamMock = new Mock<FileStream>(LogFilePath, FileMode.OpenOrCreate);
        LogStreamMock.Setup(lsm => lsm.CanWrite).Returns(true);
        
        ConfigurationSectionMock = new Mock<IConfigurationSection>();
        ConfigurationSectionMock.Setup(csm => csm.Value).Returns(LogFilePath);
        
        ConfigurationMock = new Mock<IConfiguration>();
    }

    private void ReceiveLog(string message)
    {
        LogSink.Add(message);
    }

    protected void Build()
    {
        LogWriter = LogWriterMock.Object;
        LogStream = LogStreamMock.Object;
        ConfigurationMock.Setup(cm => cm.GetSection(It.Is<string>( key => key == CodeEngineFileLogger.LogFilePathKey)))
            .Returns(ConfigurationSectionMock.Object);
        Configuration = ConfigurationMock.Object;
    }
}