using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Storage.File;

public record StoredActionRevision<T>
    where T : class
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Revision { get; set; }
    
    [Required]
    public bool Active { get; set; }
    
    [Required]
    [MinLength(10, ErrorMessage = "Make sure your code is at least 10 characters long")]
    public string Code { get; set; }
    
    public string Hash { get; set; }
    
    [Required]
    [MinLength(5, ErrorMessage = "Make sure your author name is at least 5 characters long")]
    [MaxLength(20, ErrorMessage = "Make sure your author name is at most 20 characters long")]
    public string Author { get; set; }
    
    [Required]
    [MinLength(10, ErrorMessage = "Make sure your comment is at least 10 characters long")]
    public string Comment { get; set; }
    
    [Required]
    public DateTimeOffset Created { get; set; }
}