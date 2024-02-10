using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class StoredRevisionList<T> : List<StoredActionRevision<T>>
    where T : class
{
    private const string UtcDateSerializationFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    
    void Validate(StoredActionRevision<T> revision)
    {
        _ = revision ?? throw new ArgumentNullException(nameof(revision));
        
        var validationResults = new Collection<ValidationResult>();
        if (!Validator.TryValidateObject(revision, new ValidationContext(revision), validationResults, true))
        {
            throw new ValidationException($"Revision failed validation: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        }
        
        if (revision.Revision < 1) throw new ArgumentOutOfRangeException(nameof(revision.Revision), "Revision must be greater than 0");
        
        if (this.Any(r => r.Revision == revision.Revision)) throw new InvalidOperationException("Revision already exists");
        
        if (string.IsNullOrWhiteSpace(revision.Code)) throw new ArgumentException("Code cannot be null or empty");
        
        if (string.IsNullOrWhiteSpace(revision.Author)) throw new ArgumentException("Author cannot be null or empty");
        
        if (string.IsNullOrWhiteSpace(revision.Comment)) throw new ArgumentException("Comment cannot be null or empty");
        
        if (revision.Created == default) throw new ArgumentException("Created date cannot be default");
        
        if (revision.Created > DateTimeOffset.UtcNow) throw new ArgumentException("Created date cannot be in the future");
        
        if (revision.Active) throw new ArgumentException("Revision cannot be active when created");
    }
    
    // hidden methods

    public void Add(StoredActionRevision<T> revision)
    {
        Validate(revision);
        TamperProof(revision);
        base.Add(revision);
    }
    
    public void Remove(StoredActionRevision<T> revision) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");
    
    public void Clear() => throw new InvalidOperationException("Cannot clear revisions directly from a stored subject action");

    public new void AddRange(IEnumerable<StoredActionRevision<T>> collection)
    {
        foreach (var revision in collection)
        {
            Validate(revision);
            TamperProof(revision);
            Add(revision);
        }
    }

    public void Insert(int index, StoredActionRevision<T> revision)
    {
        Validate(revision);
        TamperProof(revision);
        base.Insert(index, revision);
    }

    public void InsertRange(int index, IEnumerable<StoredActionRevision<T>> collection)
    {
        foreach (var revision in collection)
        {
            Validate(revision);
            TamperProof(revision);
            Insert(index++, revision);
        } 
    }
    
    public void RemoveAt(int index) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");
    
    public void RemoveAll(Predicate<StoredActionRevision<T>> match) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");

    
    // utility
    
    string Base64Hash(StoredActionRevision<T> revision)
    {
        var salt = revision.Created.ToString(UtcDateSerializationFormat);
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt)))
        {
            byte[] hashBytes =
                hmac.ComputeHash(Encoding.UTF8.GetBytes($"{revision.Code}.{revision.Author}.{revision.Comment}"));
            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }
    }
    
    void TamperProof(StoredActionRevision<T> revision)
    {
        if (string.IsNullOrWhiteSpace(revision.Hash)) revision.Hash = Base64Hash(revision);
        if (revision.Hash != Base64Hash(revision)) throw new InvalidOperationException("Revision has been tampered with");
    }
}