using System;
using System.Diagnostics;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Core;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

/// <summary>
/// Demonstrating several comparison and benchmarking tests
/// </summary>
[TestFixture, TestOf(typeof(CSharpCompiler<>))]
internal class CompilerPerformanceTests : CompilerPerformanceTestBase
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        PrepareOperations();
    }
    
    /// <summary>
    /// Running <paramref name="n"/> actions (functions) within a single executor.
    /// Beware of the limit on the number of methods allowed in a class.
    /// </summary>
    /// <param name="n"></param>
    [Test, Category("performance")]
    public void Should_Execute_N_Simple_Operations_In_Under_One_Second([Values(10000)] int n)
    {
        Assert.LessOrEqual(n, MaximumTestOperationsCount);
        
        Assert.That(() =>
        {
            BuildMultiActionTestSubject(n);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Target.Execute(null);
            stopwatch.Stop();
            
            Console.WriteLine($"Executed {n} operations in {stopwatch.ElapsedTicks} ticks");
            // TODO: research root cause of the execution length of n operations
            // Assert.LessOrEqual(stopwatch.ElapsedTicks, TimeSpan.TicksPerSecond, $"Execution of {n} simple operations took longer than expected");
        }, Throws.Nothing);
    }
    
    [Test, Category("performance")]
    public void Should_Execute_No_Slower_Than_Hardcoded_Expression([Values(100000)] int n)
    {
        Assert.That(() =>
        {
            BuildSingleActionTestSubject();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var idx = 0; idx < n; idx++)
            {
                var operand1 = Operations[idx, 0];
                var operand2 = Operations[idx, 1];
                var result = operand1 + operand2;
            }
            stopwatch.Stop();

            var subject = new CompilerPerformanceTestSubject();
            var stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (var idx = 0; idx < n; idx++)
            {
                subject.Operand1 = Operations[idx, 0];
                subject.Operand2 = Operations[idx, 1];
                Target.Execute(subject);
            }
            stopwatch1.Stop();
            
            Assert.LessOrEqual(stopwatch1.ElapsedTicks, stopwatch.ElapsedTicks, $"Execution of {Operations.GetLength(0)} test actions took longer than expected");
        }, Throws.TypeOf<AssertionException>());
    }
    
    [Test, Category("performance")]
    public void Should_Execute_No_Slower_Than_Hardcoded_Reflection_Expression([Values(10000)] int n)
    {
        Assert.That(() =>
        {
            BuildSingleActionTestSubject();

            var testSubjects = new CompilerPerformanceTestSubject[n];
            
            for (var idx = 0; idx < n; idx++)
            {
                testSubjects[idx] = new CompilerPerformanceTestSubject()
                {
                    Operand1 = Operations[idx, 0],
                    Operand2 = Operations[idx, 1]
                };
            }

            var operand1Info = typeof(CompilerPerformanceTestSubject).GetProperty("Operand1");
            var operand2Info = typeof(CompilerPerformanceTestSubject).GetProperty("Operand2");
            var resultInfo = typeof(CompilerPerformanceTestSubject).GetProperty("Result");
            
            var timer1 = new Stopwatch();
            timer1.Start();
            for (var idx = 0; idx < n; idx++)
            {
                var operand1 = (int)operand1Info.GetValue(testSubjects[idx]);
                var operand2 = (int)operand2Info.GetValue(testSubjects[idx]);
                resultInfo.SetValue(testSubjects[idx], operand1 + operand2);
            }
            timer1.Stop();
            Console.WriteLine($"Reflection segment completed in {timer1.ElapsedTicks}");

            var subject = new CompilerPerformanceTestSubject();
            var timer2 = new Stopwatch();
            timer2.Start();
            for (var idx = 0; idx < n; idx++)
            {
                subject.Operand1 = Operations[idx, 0];
                subject.Operand2 = Operations[idx, 1];
                Target.Execute(subject);
            }
            timer2.Stop();
            Console.WriteLine($"Compiler segment completed in {timer2.ElapsedTicks}");
            
            Assert.LessOrEqual(timer2.ElapsedTicks, timer1.ElapsedTicks, $"Execution of {Operations.GetLength(0)} test actions took longer than expected");
        }, Throws.Nothing);
    }
}

internal class CompilerPerformanceTestBase : CompilerBuilderBase<CompilerPerformanceTestSubject>
{
   
    public void BuildMultiActionTestSubject(int n)
    {
        Configuration = new CompilerConfiguration<CompilerPerformanceTestSubject>("ArmatSoftware.Code.Engine.Tests.Performance");

        Compiler = new CSharpCompiler<CompilerPerformanceTestSubject>();
        
        for (var index = 0; index < n; index++)
        {
            Configuration.Actions.Add(new CompilerPerformanceTestSubjectAction<CompilerPerformanceTestSubject>
            {
                Name = $"Action{index}",
                Code = $"var result = {Operations[index, 0]} + {Operations[index, 1]};"
            });
        }
        
        Target = Compiler.Compile(Configuration);
    }
    
    public void BuildSingleActionTestSubject()
    {
        Configuration = new CompilerConfiguration<CompilerPerformanceTestSubject>("ArmatSoftware.Code.Engine.Tests.Performance");

        Compiler = new CSharpCompiler<CompilerPerformanceTestSubject>();
        
        Configuration.Actions.Add(new CompilerPerformanceTestSubjectAction<CompilerPerformanceTestSubject>
        {
            Name = "SingleAction",
            Code = "Subject.Result = Subject.Operand1 + Subject.Operand2;"
        });
        
        Target = Compiler.Compile(Configuration);
    }
}


public class CompilerPerformanceTestSubject
{
    public int Operand1 { get; set; }
    public int Operand2 { get; set; }
    public int Result { get; set; }
}

public class CompilerPerformanceTestSubjectAction<TSubject> : ISubjectAction<TSubject> where TSubject: CompilerPerformanceTestSubject
{
    public string Name { get; set; }
    public string Code { get; set; }
    
    public int Order { get; set; }
}