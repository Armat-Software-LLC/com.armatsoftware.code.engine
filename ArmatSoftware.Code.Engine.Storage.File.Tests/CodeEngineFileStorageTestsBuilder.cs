using System.IO;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests
{
    public abstract class CodeEngineFileStorageTestsBuilder
    {
        protected Mock<IConfigurationRoot> ConfigurationMock { get; private set; }
        protected IConfigurationRoot Configuration { get; private set; }
        protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
        protected ICodeEngineLogger Logger { get; private set; }
            
        [SetUp]
        public void Setup()
        {
            ConfigurationMock = new Mock<IConfigurationRoot>();
            LoggerMock = new Mock<ICodeEngineLogger>();
        }

        protected ICodeEngineStorage Build()
        {
            Configuration = ConfigurationMock.Object;

            ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineFileStorage.FileStoragePath)])
                .Returns(Path.GetTempPath());
            
            ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineFileStorage.FileStorageExtension)])
                .Returns("log");

            Logger = LoggerMock.Object;

            return new CodeEngineFileStorage(Configuration, Logger);
        }
    }
}