using System;
using System.IO;
using System.Linq;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

public class FileStorageAdapterTests
{
    [TestFixture, TestOf(typeof(FileStorageAdapter))]
    public class FileStorageAdapterConstructorTests : FileStorageAdapterTestBuilder
    {
        [Test]
        public void Should_Construct_With_Valid_Options()
        {
            Assert.That(Build(), Is.Not.Null);
        }
        
        [Test]
        public void Should_Fail_Construct_With_Options_Null()
        {
            Assert.That(() => WithOptions(null).Build(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_Construct_With_Logger_Null()
        {
            Assert.That(() => WithLogger(null).Build(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_Construct_With_Storage_Path_Empty()
        {
            Options.StoragePath = string.Empty;
            Assert.That(() => Build(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_Construct_With_Storage_Path_Null()
        {
            Options.StoragePath = null;
            Assert.That(() => Build(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Construct_With_Storage_FileExtension_Null()
        {
            Options.FileExtension = null;
            Assert.That(() => Build(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Construct_With_Storage_FileExtension_Empty()
        {
            Options.FileExtension = string.Empty;
            Assert.That(() => Build(), Throws.ArgumentNullException);
        }
    }

    [TestFixture, TestOf(typeof(FileStorageAdapter))]
    public class FileStorageAdapterReadWriteTests : FileStorageAdapterTestBuilder
    {
        [Test]
        public void Should_Read_Non_Existent_Code_File()
        {
            var target = Build();
            var result = target.Read<TestSubject>();
            Assert.That(result, Is.EqualTo(new StoredSubjectActions<TestSubject>()));
        }
        
        [Test]
        public void Should_Read_Non_Existent_Code_File_With_Key()
        {
            var target = Build();
            var result = target.Read<TestSubject>();
            Assert.That(result, Is.EqualTo(new StoredSubjectActions<TestSubject>()));
        }
        
        /// <summary>
        /// THis is ridiculous - 
        /// </summary>
        [Test]
        public void Should_Write_And_Read_Code_File()
        {
            var target = WithStoredSubjectActions(StoredSubjectActions).Build();
            var result = target.Read<TestSubject>();
            // Assert.That(result, Is.EqualTo(StoredSubjectActions)); // doesn't work
            Assert.That(result.Count() == StoredSubjectActions.Count());
            Assert.That(result.First().Name == StoredSubjectActions.First().Name);
            Assert.That(result.First().Code == StoredSubjectActions.First().Code);
            Assert.That(result.First().Order == StoredSubjectActions.First().Order);
            Assert.That(result.First().Revisions.Count() == StoredSubjectActions.First().Revisions.Count());
            // TODO: find a better way to test equality
        }
        
        [Test]
        public void Should_Write_And_Read_Code_File_With_Key()
        {
            var target = WithStoredSubjectActions(StoredSubjectActions, "test-key").Build();
            var result = target.Read<TestSubject>("test-key");
            Assert.That(result.Count() == StoredSubjectActions.Count());
            Assert.That(result.First().Name == StoredSubjectActions.First().Name);
            Assert.That(result.First().Code == StoredSubjectActions.First().Code);
            Assert.That(result.First().Order == StoredSubjectActions.First().Order);
            Assert.That(result.First().Revisions.Count() == StoredSubjectActions.First().Revisions.Count());
            // TODO: find a better way to test equality
        }
    }
}

public class FileStorageAdapterTestBuilder
{
    protected FileStorageOptions Options {get; private set;}
    protected ILogger Logger {get; private set;}
    
    protected IStoredSubjectActions<TestSubject> StoredSubjectActions {get; private set;}

    [SetUp]
    public void Init()
    {
        Options = new FileStorageOptions
        {
            StoragePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()),
            FileExtension = ".txt"
        };
        
        Logger = new Mock<ILogger>().Object;
        
        StoredSubjectActions = new StoredSubjectActions<TestSubject>();
        var action = StoredSubjectActions.Create("TestAction");
        action.Update("Subject.Data = 10.ToString();", "ykazarov", "test action comment");
    }

    public FileStorageAdapterTestBuilder WithOptions(FileStorageOptions options)
    {
        Options = options;
        return this;
    }

    public FileStorageAdapterTestBuilder WithLogger(ILogger logger)
    {
        Logger = logger;
        return this;
    }
    
    public FileStorageAdapterTestBuilder WithStoredSubjectActions(IStoredSubjectActions<TestSubject> storedSubjectActions, string key = "")
    {
        var adapter = new FileStorageAdapter(Options, Logger);
        adapter.Write(storedSubjectActions, key);
        return this;
    }

    public IStorageAdapter Build()
    {
        var adapter = new FileStorageAdapter(Options, Logger);
        return adapter;
    }
    
    
}