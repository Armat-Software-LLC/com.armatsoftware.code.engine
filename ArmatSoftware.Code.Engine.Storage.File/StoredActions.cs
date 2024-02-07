using System.Collections;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class StoredActions<T> : IStoredActions<T>
    where T : class
{
    private readonly List<IStoredSubjectAction<T>> _actions = new();
    
    public IStoredSubjectAction<T> Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or empty");
        
        if(_actions.Any(i => i.Name == name)) throw new ArgumentException($"Action with name {name} already exists");
        
        var order = _actions.OrderByDescending(a => a.Order).FirstOrDefault()?.Order + 1 ?? 1;
            
        var newAction = new StoredSubjectAction<T>
        {
            Name = name,
            Order = order
        };
        
        _actions.Add(newAction);
        return newAction;
    }

    public void Reorder(string name, int order)
    {
        var action = _actions.FirstOrDefault(a => a.Name == name) ?? 
                     throw new ArgumentException($"Action with name {name} not found");
        
        var directionOfReorder = Math.Sign(order - action.Order);

        var shouldBeMoved = directionOfReorder switch
        {
            -1 => new Func<IStoredSubjectAction<T>, bool>(a =>
                action.Order > a.Order && a.Order >= order), // action order in decreased (moved up the list)
            1 => new Func<IStoredSubjectAction<T>, bool>(a =>
                a.Order == order), // action order in increased (moved down the list)
        };
            
        var actionsBetween = _actions.Where(a => shouldBeMoved(a)).ToList();
        
        foreach (var actionBetween in actionsBetween)
        {
            actionBetween.Order += directionOfReorder;
        }
        
        action.Order = order;
    }

    // clumsy way to prevent ICollection methods from being used
    public IEnumerator<IStoredSubjectAction<T>> GetEnumerator() => _actions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _actions.GetEnumerator();
    public void Add(IStoredSubjectAction<T> item) => throw new InvalidOperationException("Use Add(string name, int order) to add actions to a StoredActions<T> object.");
    public void Clear() => throw new InvalidOperationException("If you think you can easily clear a StoredActions<T> object, you're wrong.");
    public bool Contains(IStoredSubjectAction<T> item) => _actions.Contains(item);
    public void CopyTo(IStoredSubjectAction<T>[] array, int arrayIndex) => _actions.CopyTo(array, arrayIndex);
    public bool Remove(IStoredSubjectAction<T> item) =>
        throw new InvalidOperationException("Nice try! You can't remove actions from a StoredActions<T> object.");
    public int Count => _actions.Count;
    public bool IsReadOnly => ((ICollection<IStoredSubjectAction<T>>)_actions).IsReadOnly;
    
}