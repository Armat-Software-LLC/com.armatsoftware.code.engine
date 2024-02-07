using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

[TestFixture]
public class StoredSubjectActionTests
{
    public IStoredSubjectAction<TestSubject> Target { get; set; }

    [SetUp]
    public void Initialize()
    {
        Target = new StoredSubjectAction<TestSubject>();
        Target.Name = "Test";
        Target.Order = 1;
    }
    
    [Test]
    public void Should_Fail_To_Add_Revision_Object()
    {
        Assert.That(() => 
        {
            Target.Add(new StoredActionRevision<TestSubject>
            {
                Active = true,
                Author = "Test",
                Code = "Test",
                Comment = "Test",
                Created = DateTimeOffset.UtcNow,
                Revision = 1,
                Hash = "Test"
            });
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Revision()
    {
        Assert.That(() => 
        {
            Target.Add(null);
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_To_Remove_Revision()
    {
        Assert.That(() =>
        {
            Target.Remove(null);
        }, Throws.InvalidOperationException);
    }
        
    [Test]
    public void Should_Fail_To_Clear_Revisions()
    {
        Assert.That(() =>
        {
            Target.Clear();
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Code()
    {
        Assert.That(() =>
        {
            Target.Update(null, "valid author", "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Code()
    {
        Assert.That(() =>
        {
            Target.Update(string.Empty, "valid author", "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Code()
    {
        Assert.That(() =>
        {
            Target.Update(" ", "valid author", "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", null, "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", string.Empty, "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Author()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", " ", "valid comment");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Null_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", null);
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Empty_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", string.Empty);
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Fail_To_Add_Whitespace_Comment()
    {
        Assert.That(() =>
        {
            Target.Update("valid code", "valid author", " ");
        }, Throws.ArgumentException);
    }
    
    [Test]
    public void Should_Update_Revisions()
    {
        Target.Update("valid code", "valid author", "valid comment");
        Assert.That(Target.Count, Is.EqualTo(1));
    }
}