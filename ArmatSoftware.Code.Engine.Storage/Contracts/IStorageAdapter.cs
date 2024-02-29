namespace ArmatSoftware.Code.Engine.Storage.Contracts;

public interface IStorageAdapter
{
    IStoredSubjectActions<TSubject> Read<TSubject>(string key = "")
        where TSubject : class;

    void Write<TSubject>(IStoredSubjectActions<TSubject> actions, string key = "")
        where TSubject : class;
}