using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class StoredSubjectAction<T> : IStoredSubjectAction<T>
    where T : class
{
    private const string UtcDateSerializationFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private readonly List<IStoredActionRevision<T>> _revisions = new();
    
    public string Name { get; set; }
    
    public int Order { get; set; }
    
    public string Code => _revisions
        .OrderByDescending(r => r.Revision)
        .FirstOrDefault(r => r.Active)?.Code ?? string.Empty;
    
    // ICollection implementation
    public IEnumerator<IStoredActionRevision<T>> GetEnumerator() => _revisions.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => _revisions.GetEnumerator();
    
    void ICollection<IStoredActionRevision<T>>.Add(IStoredActionRevision<T> item) =>
        throw new InvalidOperationException("Use Update() to add revisions to a StoredSubjectAction<T> object.");

    void ICollection<IStoredActionRevision<T>>.Clear() =>
        throw new InvalidOperationException(
            "If you think you can easily clear a StoredSubjectAction<T> object, you're wrong.");

    public bool Contains(IStoredActionRevision<T> item) => _revisions.Contains(item);

    public void CopyTo(IStoredActionRevision<T>[] array, int arrayIndex) => _revisions.CopyTo(array, arrayIndex);

    bool ICollection<IStoredActionRevision<T>>.Remove(IStoredActionRevision<T> item) =>
        throw new InvalidOperationException("Nice try! You can't remove revisions from a StoredSubjectAction<T> object.");

    public int Count => _revisions.Count;

    public bool IsReadOnly => ((ICollection<IStoredActionRevision<T>>)_revisions).IsReadOnly;


    // IStoredAction implementation
    
    public void Update(string code, string author, string comment)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code cannot be null or empty");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author cannot be null or empty");
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Comment cannot be null or empty");
        
        var newRevision = new StoredActionRevision<T>
        {
            Revision = _revisions.OrderByDescending(r => r.Revision).FirstOrDefault()?.Revision + 1 ?? 1,
            Active = false,
            Code = code,
            Author = author,
            Comment = comment,
            Created = DateTimeOffset.UtcNow
        };
            
        newRevision.Hash = Base64Hash(newRevision);

        // validate the new revision object against data annotations
        var validationResults = new Collection<ValidationResult>();
        if (Validator.TryValidateObject(newRevision, new ValidationContext(newRevision), validationResults, true))
        {
            _revisions.Add(newRevision);
        }
        else
        {
            throw new ValidationException($"Revision failed validation: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        }
    }
    
    public void Activate(int revision)
    {
        _revisions.ForEach(r => r.Active = false);
        _revisions.First(r => r.Revision == revision).Active = true;
    }
    
    // utility
    
    string Base64Hash(StoredActionRevision<T> revision)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Name)))
        {
            byte[] hashBytes =
                hmac.ComputeHash(Encoding.UTF8.GetBytes($"{revision.Code}.{revision.Author}.{revision.Comment}.{revision.Created.ToString(UtcDateSerializationFormat)}"));
            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }
    }
}
