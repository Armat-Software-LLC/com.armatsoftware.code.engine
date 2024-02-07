using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Storage.File.Tests;

[TestFixture]
public class StoredActionsTests
{
    public StoredActions<TestSubject> Target { get; set; }

    [SetUp]
    public void Initialize()
    {
        Target = new StoredActions<TestSubject>();
    }
    
    [Test]
    public void Should_Fail_Use_Add()
    {
        Assert.That(() =>
        {
            Target.Add(new StoredSubjectAction<TestSubject>
            {
                Name = "Test",
                Order = 1
            });
        }, Throws.InvalidOperationException);
    }
        
    [Test]
    public void Should_Fail_Use_Insert()
    {
        Assert.That(() =>
        {
            Target.Remove(new StoredSubjectAction<TestSubject>
            {
                Name = "Test",
                Order = 1
            });
        }, Throws.InvalidOperationException);
    }
    
    [Test]
    public void Should_Fail_Use_Clear()
    {
        Assert.That(() =>
        {
            Target.Clear();
        }, Throws.InvalidOperationException);
    }
}