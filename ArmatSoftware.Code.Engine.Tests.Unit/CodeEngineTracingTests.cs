using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArmatSoftware.Code.Engine.Compiler.Base;
using ArmatSoftware.Code.Engine.Compiler.CSharp;
using ArmatSoftware.Code.Engine.Compiler.Vb;
using ArmatSoftware.Code.Engine.Core.Tracing;
using NUnit.Framework;

namespace ArmatSoftware.Code.Engine.Tests.Unit;

[TestFixture, NonParallelizable]
public class CodeEngineTracingTests
{
    private readonly object _activitiesLock = new();
    private ActivityListener _listener;
    private List<Activity> _activities;

    [SetUp]
    public void Setup()
    {
        _activities = new List<Activity>();
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == CodeEngineActivity.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                lock (_activitiesLock)
                {
                    _activities.Add(activity);
                }
            }
        };

        ActivitySource.AddActivityListener(_listener);
    }

    [TearDown]
    public void TearDown()
    {
        _listener.Dispose();
    }

    [Test]
    public void Should_Create_CSharp_Execution_Span()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        configuration.Actions.Add(new TestSubjectAction<TestSubject>
        {
            Name = "SetData",
            Code = "Subject.Data = \"changed\";"
        });

        new CSharpCompiler<TestSubject>().Compile(configuration).Execute(new TestSubject());

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].OperationName, Is.EqualTo(CodeEngineActivity.SpanName));
        Assert.That(_activities[0].GetTagItem("codeengine.subject.type"), Is.EqualTo(typeof(TestSubject).FullName));
        Assert.That(_activities[0].GetTagItem("codeengine.compiler"), Is.EqualTo("CSharp"));
        Assert.That(_activities[0].GetTagItem("codeengine.action.count"), Is.EqualTo(1));
        Assert.That(_activities[0].Status, Is.EqualTo(ActivityStatusCode.Unset));
    }

    [Test]
    public void Should_Include_Executor_Metadata()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        var executor = new CSharpCompiler<TestSubject>().Compile(configuration);
        ((IExecutorMetadata)executor).SetMetadata("customer-key", "CSharp");

        executor.Execute(new TestSubject());

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].GetTagItem("codeengine.executor.key"), Is.EqualTo("customer-key"));
    }

    [Test]
    public void Should_Create_Vb_Execution_Span()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        configuration.Actions.Add(new TestSubjectAction<TestSubject>
        {
            Name = "SetData",
            Code = "Subject.Data = \"changed\""
        });

        new VbCompiler<TestSubject>().Compile(configuration).Execute(new TestSubject());

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].GetTagItem("codeengine.compiler"), Is.EqualTo("Vb"));
    }

    [Test]
    public void Should_Record_Execution_Exception_And_Rethrow()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        configuration.Actions.Add(new TestSubjectAction<TestSubject>
        {
            Name = "ThrowError",
            Code = "throw new InvalidOperationException(\"boom\");"
        });

        Assert.That(
            () => new CSharpCompiler<TestSubject>().Compile(configuration).Execute(new TestSubject()),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("boom"));

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].Status, Is.EqualTo(ActivityStatusCode.Error));
        var exceptionEvent = _activities[0].Events.Single(@event => @event.Name == "exception");
        Assert.That(exceptionEvent.Tags.Single(tag => tag.Key == "exception.type").Value, Is.EqualTo(typeof(InvalidOperationException).FullName));
        Assert.That(exceptionEvent.Tags.Single(tag => tag.Key == "exception.message").Value, Is.EqualTo("boom"));
    }

    [TestCase(null, null, int.MinValue)]
    [TestCase("", "", 0)]
    [TestCase(" ", "compiler with spaces", int.MaxValue)]
    public void Should_Preserve_Extreme_Activity_Metadata(string key, string compiler, int actionCount)
    {
        var activity = CodeEngineActivity.StartExecution(typeof(TestSubject), key, compiler, actionCount);
        activity.Stop();

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].GetTagItem("codeengine.executor.key"), Is.EqualTo(key));
        Assert.That(_activities[0].GetTagItem("codeengine.compiler"), Is.EqualTo(compiler));
        Assert.That(_activities[0].GetTagItem("codeengine.action.count"), Is.EqualTo(actionCount));
    }

    [Test]
    public void Should_Handle_Very_Long_Metadata()
    {
        var key = string.Concat(Enumerable.Repeat("key-", 2048));
        var compiler = string.Concat(Enumerable.Repeat("compiler-", 2048));
        var activity = CodeEngineActivity.StartExecution(typeof(TestSubject), key, compiler, 1);
        activity.Stop();

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].GetTagItem("codeengine.executor.key"), Is.EqualTo(key));
        Assert.That(_activities[0].GetTagItem("codeengine.compiler"), Is.EqualTo(compiler));
    }

    [Test]
    public void Should_Create_A_Span_For_Every_Execution()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        var executor = new CSharpCompiler<TestSubject>().Compile(configuration);
        const int executionCount = 128;

        for (var index = 0; index < executionCount; index++)
        {
            executor.Execute(new TestSubject());
        }

        Assert.That(_activities, Has.Count.EqualTo(executionCount));
        Assert.That(_activities.All(activity => (int)activity.GetTagItem("codeengine.action.count") == 0), Is.True);
    }

    [Test]
    public void Should_Create_Nested_Spans_For_Recursive_Execution()
    {
        var configuration = new CompilerConfiguration<RecursionSubject>("ArmatSoftware.Code.Engine.Tests.Recursion");
        configuration.Actions.Add(new RecursionSubjectAction
        {
            Name = "DoubleUntilGreaterThan1000",
            Code = "Subject.Counter = Subject.Counter * 2; if (Subject.Counter < 1000) Execute(Subject);"
        });

        var executor = new CSharpCompiler<RecursionSubject>().Compile(configuration);
        executor.Execute(new RecursionSubject { Counter = 1 });

        Assert.That(_activities, Has.Count.EqualTo(10));
    }

    [Test]
    public void Should_Trace_A_Large_Action_Set()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        const int actionCount = 256;
        for (var index = 0; index < actionCount; index++)
        {
            configuration.Actions.Add(new TestSubjectAction<TestSubject>
            {
                Name = $"Action{index}",
                Code = $"var value = {index};"
            });
        }

        new CSharpCompiler<TestSubject>().Compile(configuration).Execute(new TestSubject());

        Assert.That(_activities, Has.Count.EqualTo(1));
        Assert.That(_activities[0].GetTagItem("codeengine.action.count"), Is.EqualTo(actionCount));
    }

    [Test]
    public void Should_Record_A_Very_Long_Exception_Message()
    {
        var message = string.Concat(Enumerable.Repeat("failure-", 1024));
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        configuration.Actions.Add(new TestSubjectAction<TestSubject>
        {
            Name = "ThrowError",
            Code = $"throw new InvalidOperationException(\"{message}\");"
        });

        Assert.That(
            () => new CSharpCompiler<TestSubject>().Compile(configuration).Execute(new TestSubject()),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(message));

        var exceptionEvent = _activities.Single().Events.Single(@event => @event.Name == "exception");
        Assert.That(exceptionEvent.Tags.Single(tag => tag.Key == "exception.message").Value, Is.EqualTo(message));
    }

    [Test]
    public async Task Should_Trace_Concurrent_Executions_Independently()
    {
        var configuration = new CompilerConfiguration<TestSubject>("ArmatSoftware.Code.Engine.Tests.Executors");
        configuration.Actions.Add(new TestSubjectAction<TestSubject>
        {
            Name = "SetData",
            Code = "Subject.Data = \"changed\";"
        });
        var executor = new CSharpCompiler<TestSubject>().Compile(configuration);
        var tasks = Enumerable.Range(0, 32)
            .Select(_ => Task.Run(() => executor.Clone().Execute(new TestSubject())))
            .ToArray();

        await Task.WhenAll(tasks);

        Assert.That(_activities, Has.Count.EqualTo(tasks.Length));
        Assert.That(_activities.All(activity => activity.Status == ActivityStatusCode.Unset), Is.True);
    }
}
