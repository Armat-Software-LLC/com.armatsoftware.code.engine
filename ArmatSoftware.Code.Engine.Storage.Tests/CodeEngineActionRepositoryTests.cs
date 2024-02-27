using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using ArmatSoftware.Code.Engine.Core.Logging;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.Contracts;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.Tests;

public class CodeEngineActionRepositoryTests : CodeEngineActionRepositoryTestBuilder
{
    [TestFixture, TestOf(typeof(CodeEngineActionRepository))]
    public class CodeEngineActionRepositoryConstructorTests : CodeEngineActionRepositoryTestBuilder
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
                Configuration = null;
                Build();
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Logger()
        {
            Assert.That(() =>
            {
                Logger = null;
                Build();
            }, Throws.ArgumentNullException);
        }
        
        [Test]
        public void Should_Fail_With_Null_Storage()
        {
            Assert.That(() =>
            {
                StorageAdapter = null;
                Build();
            }, Throws.ArgumentNullException);
        }
    }
    
    [TestFixture, TestOf(typeof(CodeEngineActionRepository))]
    public class CodeEngineActionRepositoryGetActionsTests : CodeEngineActionRepositoryTestBuilder
    {
        [Test]
        public void Should_Return_Actions_Without_Key()
        {
            var repository = Build();
            var result = repository.GetActions<TestSubject>();
            
            Assert.That(result, Is.EqualTo(StoredActions));
        }
        
        [Test]
        public void Should_Return_Actions_With_Key()
        {
            var repository = Build();
            var result = repository.GetActions<TestSubject>("test-key");
            
            Assert.That(result, Is.EqualTo(StoredActions));
        }
        
        [Test]
        public void Should_Fail_Return_Actions_With_Null_Key()
        {
            Assert.That(() =>
            {
                 var repository = Build();
                 var result = repository.GetActions<TestSubject>(null);
                 
                 Assert.That(result, Is.EqualTo(StoredActions));
            }, Throws.Nothing);
        }
    }

    [TestFixture, TestOf(typeof(CodeEngineActionRepository))]
    public class CodeEngineActionRepositoryAddActionTests : CodeEngineActionRepositoryTestBuilder
    {
        [Test]
        public void Should_Fail_Add_Action_With_Name_Duplicate()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("DuplicateAction", "Subject.Data = 10;", "ykazarov", "Very simple action");
                repository.AddAction<TestSubject>("DuplicateAction", "Subject.Data = 10;", "ykazarov", "Very simple action");
            }, Throws.TypeOf<ArgumentException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Name_Empty()
        {
            Assert.That(() =>
            {
                 var repository = Build();
                 repository.AddAction<TestSubject>(string.Empty, "Subject.Data = 10;", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Name_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>(null, "Subject.Data = 10;", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Code_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", string.Empty, "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Code_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", null, "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Code_Short()
        {
            Assert.That(() =>
            {
                BuildActions();
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "short", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Author_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", string.Empty, "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Author_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", null, "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Author_Short()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", "<5", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Author_Long()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", "very long author name, longer than 20 chars", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Comment_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", "ykazarov", string.Empty);
               
            }, Throws.TypeOf<ValidationException>());
        }
                
        [Test]
        public void Should_Fail_Add_Action_With_Comment_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", "ykazarov", null);
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Comment_Short()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.AddAction<TestSubject>("AnotherAction", "Subject.Data = 10;", "ykazarov", "short");
               
            }, Throws.TypeOf<ValidationException>());
        }
    }
    
    public class CodeEngineActionRepositoryUpdateActionTests : CodeEngineActionRepositoryTestBuilder
    {
        [Test]
        public void Should_Update_Action()
        {
            var repository = Build();

            Assert.That(() =>
            {
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "ykazarov",
                    "Very simple action");
            }, Throws.Nothing);
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Name_Empty()
        {
            Assert.That(() =>
            {
                 var repository = Build();
                 repository.UpdateAction<TestSubject>(string.Empty, "Subject.Data = 10;", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Name_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(null, "Subject.Data = 10;", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Code_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, string.Empty, "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Code_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, null, "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Code_Short()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "short", "ykazarov", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Author_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", string.Empty, "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Author_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", null, "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Author_Short()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "<5", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Author_Long()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "very long author name, longer than 20 chars", "Very simple action");
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Add_Action_With_Comment_Empty()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "ykazarov", string.Empty);
               
            }, Throws.TypeOf<ValidationException>());
        }
                
        [Test]
        public void Should_Fail_Update_Action_With_Comment_Null()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "ykazarov", null);
               
            }, Throws.TypeOf<ValidationException>());
        }
        
        [Test]
        public void Should_Fail_Update_Action_With_Comment_Short()
        {
            Assert.That(() =>
            {
                var repository = Build();
                repository.UpdateAction<TestSubject>(repository.GetActions<TestSubject>().First().Name, "Subject.Data = 10;", "ykazarov", "short");
               
            }, Throws.TypeOf<ValidationException>());
        }
    }
    
    
    [TestFixture, TestOf(typeof(CodeEngineActionRepository))]
    public class CodeEngineActionRepositoryReorderTests : CodeEngineActionRepositoryTestBuilder
    {
    }
    
    [TestFixture, TestOf(typeof(CodeEngineActionRepository))]
    public class CodeEngineActionRepositoryRemoveTests : CodeEngineActionRepositoryTestBuilder
    {
    }
}

public abstract class CodeEngineActionRepositoryTestBuilder
{
    protected Mock<IConfigurationRoot> ConfigurationMock { get; private set; }
    protected IConfigurationRoot Configuration { get; set; }
    
    protected Mock<ICodeEngineLogger> LoggerMock { get; private set; }
    protected ICodeEngineLogger Logger { get; set; }
    
    protected Mock<IStorageAdapter> StorageAdapterMock { get; private set; }
    protected IStorageAdapter StorageAdapter { get; set; }
    
    protected IStoredSubjectActions<TestSubject> StoredActions { get; private set; }
    
    [SetUp]
    public void Setup()
    {
        ConfigurationMock = new Mock<IConfigurationRoot>();
        LoggerMock = new Mock<ICodeEngineLogger>();
        StorageAdapterMock = new Mock<IStorageAdapter>();
        
        BuildActions();

        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStoragePath)])
            .Returns(Path.GetTempPath());
    
        ConfigurationMock.Setup(cm => cm[It.Is<string>(s => s == CodeEngineActionProvider.FileStorageExtension)])
            .Returns("test");
        
        StorageAdapterMock.Setup(sa => sa.Read<TestSubject>(It.IsAny<string>()))
            .Returns(StoredActions);
        
        Configuration = ConfigurationMock.Object;
        Logger = LoggerMock.Object;
        StorageAdapter = StorageAdapterMock.Object;
    }

    protected IActionRepository Build()
    {
        return new CodeEngineActionRepository(Configuration, Logger, StorageAdapter);
    }
    
    protected void BuildActions()
    {
        StoredActions = new StoredSubjectActions<TestSubject>();
        var action = StoredActions.Create("SimpleAction");
        action.Update("Subject.Data = 1;", "ykazarov", "Very simple action");
        action.Activate(1);
    }
}