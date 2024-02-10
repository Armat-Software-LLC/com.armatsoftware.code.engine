using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

[TestFixture]
public class StoredSubjectActionTests
{
    public StoredSubjectAction<TestSubject> Target { get; set; }

    [SetUp]
    public void Initialize()
    {
        Target = new StoredSubjectAction<TestSubject>();
        Target.Name = "Test";
        Target.Order = 1;
        Target.Revisions = new StoredRevisionList<TestSubject>();
    }
    
    [Test]
    public void Should_Add_First_Revision()
    {
        Assert.That(() => 
        {
            Target.Revisions.Add(new StoredActionRevision<TestSubject>
            {
                Active = false,
                Author = "Author 1",
                Code = "sample code 1",
                Comment = "sample comment 1",
                Created = DateTimeOffset.UtcNow,
                Revision = 1
            });
        }, Throws.Nothing);
    }
    
    [Test]
    public void Should_Add_Two_Revisions()
    {
        Assert.That(() => 
        {
            Target.Revisions.Add(new StoredActionRevision<TestSubject>
            {
                Active = false,
                Author = "Author 1",
                Code = "sample code 1",
                Comment = "sample comment 1",
                Created = DateTimeOffset.UtcNow,
                Revision = 1
            });
            Target.Revisions.Add(new StoredActionRevision<TestSubject>
            {
                Active = false,
                Author = "Author 2",
                Code = "sample code 2",
                Comment = "sample comment 2",
                Created = DateTimeOffset.UtcNow,
                Revision = 2
            });
        }, Throws.Nothing);
    }
    
    [Test]
    public void Should_Fail_To_Add_Duplicate_Revisions()
    {
        Assert.That(() => 
        {
            Target.Revisions.Add(new StoredActionRevision<TestSubject>
            {
                Active = false,
                Author = "Author 1",
                Code = "sample code 1",
                Comment = "sample comment 1",
                Created = DateTimeOffset.UtcNow,
                Revision = 1
            });
            Target.Revisions.Add(new StoredActionRevision<TestSubject>
            {
                Active = false,
                Author = "Author 2",
                Code = "sample code 2",
                Comment = "sample comment 2",
                Created = DateTimeOffset.UtcNow,
                Revision = 1
            });
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Revision()
    {
        Assert.That(() => 
        {
            Target.Revisions.Add(null);
        }, Throws.ArgumentNullException);
    }
    
    [Test]
    public void Should_Fail_To_Remove_Revision()
    {
        Assert.That(() =>
        {
            Target.Revisions.Remove(null);
        }, Throws.InvalidOperationException);
    }
        
    [Test]
    public void Should_Fail_To_Clear_Revisions()
    {
        Assert.That(() =>
        {
            Target.Revisions.Clear();
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Code()
    {
        Assert.That(() =>
        {
            Target.Update(null, "valid author", "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Code()
    {
        Assert.That(() =>
        {
            Target.Update(string.Empty, "valid author", "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Code()
    {
        Assert.That(() =>
        {
            Target.Update(" ", "valid author", "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", null, "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", string.Empty, "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", " ", "valid comment");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", null);
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", string.Empty);
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", " ");
        }, Throws.TypeOf<ValidationException>());
    }
    
    [Test]
    public void Should_Update_Revisions()
    {
        Target.Update("valid code", "valid author", "valid comment");
        Assert.That(Target.Revisions.Count, Is.EqualTo(1));
    }
}