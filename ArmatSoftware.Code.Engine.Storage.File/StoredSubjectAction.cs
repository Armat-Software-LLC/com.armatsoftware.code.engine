using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Storage.File;

public class StoredSubjectAction<T> : ISubjectAction<T>
    where T : class
{
    
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    
    public string Code  => Revisions.FirstOrDefault(r => r.Active)?.Code ?? string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Order { get; set; }
    
    [Required]
    public StoredRevisionList<T> Revisions { get; set; }
    
    public void Update(string code, string author, string comment)
    {
        var newRevision = new StoredActionRevision<T>
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
