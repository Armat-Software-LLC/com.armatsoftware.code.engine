using ArmatSoftware.Code.Engine.Core.Storage;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

public class CustomCodeStorage : ICodeEngineStorage
{
    public void Store(Type subjectType, Guid executorId, string code)
    {
        throw new NotImplementedException();
    }

    public string Retrieve(Type subjectType, Guid executorId)
    {
        switch (subjectType.Name)
        {
            case nameof(StringOnlySubject):
                return "Subject.Data = \"hello world!\";";
            case nameof(NumericAndStringSubject):
                return "Subject.StringData = \"hello world 2!\";" +
                       "Subject.NumericData = 42;";
            default:
                throw new ArgumentOutOfRangeException(nameof(subjectType), subjectType, null);
        }
    }
}