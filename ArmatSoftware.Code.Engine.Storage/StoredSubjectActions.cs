using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ArmatSoftware.Code.Engine.Storage.Contracts;

namespace ArmatSoftware.Code.Engine.Storage;

public class StoredSubjectActions<TSubject> : IStoredSubjectActions<TSubject>
    where TSubject : class
{
    private readonly ICollection<StoredSubjectAction<TSubject>> _storedActions =
        new List<StoredSubjectAction<TSubject>>();
    
    public IStoredSubjectAction<TSubject> Create(string name)
    {
        var order = this.OrderByDescending(a => a.Order).FirstOrDefault()?.Order + 1 ?? 1;
            
        var newAction = new StoredSubjectAction<TSubject>
        {
            Name = name,
            Order = order,
            Revisions = new StoredRevisionList<TSubject>()
        };
        
        Add(newAction); // calling overridden Add method to validate the action
        return newAction;
    }

    public void Reorder(string name, int order)
    {
        var action = this.FirstOrDefault(a => a.Name == name) ?? 
                     throw new ArgumentException($"Action with name {name} not found");
        
        if (action.Order == order) return;
        
        var directionOfReorder = Math.Sign(order - action.Order);

        var shouldBeMoved = directionOfReorder switch
        {
            -1 => new Func<IStoredSubjectAction<TSubject>, bool>(a =>
                a.Order >= order && a.Order < action.Order), // action order in decreased (moved up the list)
            1 => new Func<IStoredSubjectAction<TSubject>, bool>(a =>
                a.Order <= order && a.Order > action.Order), // action order in increased (moved down the list)
        };
            
        var actionsBetween = this.Where(a => shouldBeMoved(a)).ToList();
        
        foreach (var actionBetween in actionsBetween)
        {
            // have to move the items in the direction opposite to the reorder, thus negation
            actionBetween.Order += -directionOfReorder;
        }
        
        action.Order = order;
    }
    
    // Validation

    private void Validate(IStoredSubjectAction<TSubject> newAction)
    {
        _ = newAction ?? throw new ArgumentNullException(nameof(newAction));

        var validationResults = new Collection<ValidationResult>();
        if (!Validator.TryValidateObject(newAction, new ValidationContext(newAction), validationResults, true))
        {
            throw new ValidationException($"Revision failed validation: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        }
        
        if(this.Any(i => i.Name == newAction.Name)) throw new ArgumentException($"Action with name {newAction.Name} already exists");
    }
    
    // List<> overrides

    public void Add(StoredSubjectAction<TSubject> action)
    {
        Validate(action);
        _storedActions.Add((StoredSubjectAction<TSubject>)action);
    }

    public void Clear()
    {
        _storedActions.Clear();
    }

    public bool Contains(StoredSubjectAction<TSubject> item)
    {
        return _storedActions.Contains(item);
    }

    public void CopyTo(StoredSubjectAction<TSubject>[] array, int arrayIndex)
    {
        _storedActions.CopyTo((StoredSubjectAction<TSubject>[])array, arrayIndex);
    }

    public bool Remove(StoredSubjectAction<TSubject> item)
    {
        return _storedActions.Remove((StoredSubjectAction<TSubject>)item);
    }

    public int Count => _storedActions.Count;

    public bool IsReadOnly => _storedActions.IsReadOnly;

    public IEnumerator<StoredSubjectAction<TSubject>> GetEnumerator()
    {
        return _storedActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_storedActions).GetEnumerator();
    }
}
