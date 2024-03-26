using System;
using ArmatSoftware.Code.Engine.Compiler;
using ArmatSoftware.Code.Engine.Core;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

public class CompilerBuilderBase<TSubject>
    where TSubject : class
{
    public const int MaximumTestOperationsCount = 1000000;

    protected IExecutor<TSubject> Target { get; set; }
    protected ICompilerConfiguration<TSubject> Configuration { get; set; }
    protected ICompiler<TSubject> Compiler { get; set; }
    protected static int[,] Operations { get; set; }

    public void PrepareOperations()
    {
        Console.WriteLine("Preparing list of operations for multiple tests");
            
        // beware of the error "contains more methods than the current implementation allows"
        Operations = new int[MaximumTestOperationsCount, 2];
            
        for (var index = 0; index < Operations.GetLength(0); index++)
        {
            Operations[index, 0] = new Random().Next(0, 1000);
            Operations[index, 1] = new Random().Next(0, 1000);
        }
        Console.WriteLine($"Generated {Operations.GetLength(0)} expressions");
    }
}