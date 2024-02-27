using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Storage.Contracts;

public interface IStoredSubjectAction<TSubject> : ISubjectAction<TSubject>
    where TSubject : class
{
    public void Update(string code, string author, string comment);
    
    public void Activate(int revision);
}