using System.Collections.Generic;

namespace ArmatSoftware.Code.Engine.Storage.Contracts;

public interface IStoredSubjectActions<TSubject> : ICollection<StoredSubjectAction<TSubject>>
    where TSubject : class
{
    IStoredSubjectAction<TSubject> Create(string name);
    
    void Reorder(string name, int order);
}