using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Core;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

/// <summary>
/// Demonstrating several comparison and benchmarking tests
/// </summary>
[TestFixture, TestOf(typeof(CSharpCompiler<>))]
internal class ExecutorRecursionTests : CompilerBuilderBase<RecursionSubject>
{
    [SetUp]
    public void Setup()
    {
        Configuration = new CompilerConfiguration<RecursionSubject>("ArmatSoftware.Code.Engine.Tests.Recursion");

        Compiler = new CSharpCompiler<RecursionSubject>();
        
        Configuration.Actions.Add(new RecursionSubjectAction
        {
            Name = "DoubleUntilGreaterThan1000",
            Code = "Subject.Counter = Subject.Counter * 2; if (Subject.Counter < 1000) Execute(Subject);",
            Order = 1
        });
        
        Target = Compiler.Compile(Configuration);
    }
    
    
    /// <summary>
    /// Simple recursion scenario
    /// </summary>
    [Test]
    public void Should_Execute_Recursively()
    {
        Assert.That(() =>
        {
            var subject = new RecursionSubject { Counter = 1 };
            Target.Execute(subject);
            Assert.AreEqual(1024, subject.Counter);
        }, Throws.Nothing);
    }
    
    // TODO: try to isolate the overflow exception
    /// <summary>
    /// Damn it, this test will fail the entire test class
    /// </summary>
    // [Test]
    // public void Should_Fail_Execute_Recursion_Infinitely()
    // {
    //     try
    //     {
    //         var subject = new RecursionSubject { Counter = 0 };
    //         Target.Execute(subject);
    //         Assert.Fail();
    //     }
    //     catch (StackOverflowException ex)
    //     {
    //         Assert.Pass(ex.Message, ex);
    //     }
    //     catch (LoggerException ex)
    //     {
    //         Assert.Pass(ex.Message, ex);
    //     }
    // }
}

public class RecursionSubject
{
    public int Counter { get; set; }
}

public class RecursionSubjectAction : ISubjectAction<RecursionSubject>
{
    public string Name { get; set; }
    public string Code { get; set; }

    public int Order { get; set; }
}
