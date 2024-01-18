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
        return "Subject.Data = \"hello world!\";";
    }
}