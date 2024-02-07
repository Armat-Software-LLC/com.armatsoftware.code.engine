using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Base action revision class used to update the action
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IActionRevision<T>
    where T : class
{
    /// <summary>
    /// Author of the revision
    /// </summary>
    [Required]
    [MinLength(5, ErrorMessage = "Author name cannot be less than 5 characters")]
    string Author { get; set; }
    
    /// <summary>
    /// Author's comment on revision
    /// </summary>
    string Comment { get; set; }
    
    /// <summary>
    /// Code of the action
    /// </summary>
    string Code { get; set; }
}