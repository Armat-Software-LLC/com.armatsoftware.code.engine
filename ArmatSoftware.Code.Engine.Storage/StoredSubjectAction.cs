using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ArmatSoftware.Code.Engine.Storage.Contracts;

namespace ArmatSoftware.Code.Engine.Storage;

public class StoredSubjectAction<TSubject> : IStoredSubjectAction<TSubject>
    where TSubject : class
{
    
    [Required]
    [MinLength(3)]
    public string Name { get; set; }

    public string Code
    {
        get
        {
            var revision = Revisions.FirstOrDefault(r => r.Active);

            if (revision == null)
            {
                return string.Empty;
            }
            
            revision.CheckTamperProof();
            return revision.Code;
        }
    }

    [Required]
    [Range(1, int.MaxValue)]
    public int Order { get; set; }
    
    [Required]
    public StoredRevisionList<TSubject> Revisions { get; set; }
    
    public void Update(string code, string author, string comment)
    {
        var newRevision = new StoredActionRevision
        {
            Revision = Revisions.OrderByDescending(r => r.Revision).FirstOrDefault()?.Revision + 1 ?? 1,
            Active = false,
            Code = code,
            Author = author,
            Comment = comment,
            Created = DateTimeOffset.UtcNow
        };
        
        Revisions.Add(newRevision); // calling overridden Add method to validate the revision
    }
    
    public void Activate(int revision)
    {
        Revisions.ForEach(r => r.Active = false);
        Revisions.First(r => r.Revision == revision).Active = true;
    }
}

