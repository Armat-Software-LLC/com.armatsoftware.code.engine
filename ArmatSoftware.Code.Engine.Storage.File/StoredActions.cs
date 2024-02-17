using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class StoredActions<TSubject> : List<StoredSubjectAction<TSubject>>
    where TSubject : class
{
    
    public StoredSubjectAction<TSubject> Add(string name)
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
            -1 => new Func<StoredSubjectAction<TSubject>, bool>(a =>
                a.Order >= order && a.Order < action.Order), // action order in decreased (moved up the list)
            1 => new Func<StoredSubjectAction<TSubject>, bool>(a =>
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

    private void Validate(StoredSubjectAction<TSubject> newAction)
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
        base.Add(action);
    }
    
    public void AddRange(IEnumerable<StoredSubjectAction<TSubject>> collection)
    {
        foreach (var action in collection)
        {
            Validate(action);
            base.Add(action);
        }
    }

    public void Insert(int index, StoredSubjectAction<TSubject> action)
    {
        Validate(action);
        base.Insert(index, action);
    }
    
    public void InsertRange(int index, IEnumerable<StoredSubjectAction<TSubject>> collection)
    {
        foreach (var action in collection)
        {
            Validate(action);
            base.Insert(index, action);
        }
    }
    
    public void Remove(StoredSubjectAction<TSubject> action) => throw new InvalidOperationException("Cannot remove an action directly from a stored actions");
    
    public void Clear() => throw new InvalidOperationException("Cannot clear actions directly from a stored actions");
}