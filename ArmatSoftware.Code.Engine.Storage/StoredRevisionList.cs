using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ArmatSoftware.Code.Engine.Storage;

public class StoredRevisionList<TSubject> : List<StoredActionRevision>
    where TSubject : class
{
    void Validate(StoredActionRevision revision)
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

    public void Add(StoredActionRevision revision)
    {
        Validate(revision);
        revision.TamperProof();
        base.Add(revision);
    }
    
    public void Remove(StoredActionRevision revision) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");
    
    public void Clear() => throw new InvalidOperationException("Cannot clear revisions directly from a stored subject action");

    public new void AddRange(IEnumerable<StoredActionRevision> collection)
    {
        foreach (var revision in collection)
        {
            Validate(revision);
            revision.TamperProof();
            Add(revision);
        }
    }

    public void Insert(int index, StoredActionRevision revision)
    {
        Validate(revision);
        revision.TamperProof();
        base.Insert(index, revision);
    }

    public void InsertRange(int index, IEnumerable<StoredActionRevision> collection)
    {
        foreach (var revision in collection)
        {
            Validate(revision);
            revision.TamperProof();
            Insert(index++, revision);
        } 
    }
    
    public void RemoveAt(int index) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");
    
    public void RemoveAll(Predicate<StoredActionRevision> match) => throw new InvalidOperationException("Cannot remove a revision directly from a stored subject action");
}