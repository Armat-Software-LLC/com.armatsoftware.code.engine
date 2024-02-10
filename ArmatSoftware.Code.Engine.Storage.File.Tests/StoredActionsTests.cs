using System.Linq;
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
    public void Should_Reorder_Up_End_To_Start()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action5", 1);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action4").Order);
    }
    
    [Test]
    public void Should_Reorder_Up_End_To_Middle()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action5", 3);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action4").Order);
    }
    
    [Test]
    public void Should_Reorder_Up_Middle_To_Start()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action3", 1);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action5").Order);
    }
    
    [Test]
    public void Should_Reorder_Up_Middle_To_Before_Start()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action3", -1);

        Assert.AreEqual(-1, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action5").Order);
    }
    
    [Test]
    public void Should_Reorder_Up_End_To_Before_Start()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action5", -1);

        Assert.AreEqual(-1, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action4").Order);
    }
    
    [Test]
    public void Should_Reorder_Down_Start_To_End()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action1", 5);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action1").Order);
    }
    
    [Test]
    public void Should_Reorder_Down_Start_To_Middle()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action1", 3);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action5").Order);
    }
    
    [Test]
    public void Should_Reorder_Down_Middle_To_End()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action3", 5);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(5, Target.First(a => a.Name == "Action3").Order);
    }
    
    [Test]
    public void Should_Reorder_Down_Middle_To_Outside_End()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action3", 7);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action1").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(7, Target.First(a => a.Name == "Action3").Order);
    }
    
    [Test]
    public void Should_Reorder_Down_Start_To_Outside_End()
    {
        Target.Add("Action1");
        Target.Add("Action2");
        Target.Add("Action3");
        Target.Add("Action4");
        Target.Add("Action5");

        Target.Reorder("Action1", 7);

        Assert.AreEqual(1, Target.First(a => a.Name == "Action2").Order);
        Assert.AreEqual(2, Target.First(a => a.Name == "Action3").Order);
        Assert.AreEqual(3, Target.First(a => a.Name == "Action4").Order);
        Assert.AreEqual(4, Target.First(a => a.Name == "Action5").Order);
        Assert.AreEqual(7, Target.First(a => a.Name == "Action1").Order);
    }
}