using System;
using System.Diagnostics;

namespace ArmatSoftware.Code.Engine.Core.Tracing;

public static class CodeEngineActivity
{
    public const string SourceName = "ArmatSoftware.Code.Engine";
    public const string SpanName = "codeengine.execute";

    public static ActivitySource Source { get; } = new(SourceName);

    public static Activity StartExecution(Type subjectType, string key, string compiler, int actionCount)
    {
        var activity = Source.StartActivity(SpanName, ActivityKind.Internal);
        if (activity == null)
        {
            return null;
        }

        activity.SetTag("codeengine.subject.type", subjectType?.FullName);
        activity.SetTag("codeengine.executor.key", key);
        activity.SetTag("codeengine.compiler", compiler);
        activity.SetTag("codeengine.action.count", actionCount);

        return activity;
    }

    public static void RecordException(Activity activity, Exception exception)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.AddEvent(new ActivityEvent(
            "exception",
            tags: new ActivityTagsCollection
            {
                ["exception.type"] = exception.GetType().FullName,
                ["exception.message"] = exception.Message,
                ["exception.stacktrace"] = exception.StackTrace
            }));
    }
}
