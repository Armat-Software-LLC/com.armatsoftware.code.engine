using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Storage;

/// <summary>
/// Action with its appropriate metadata stored in the database
/// </summary>
/// <typeparam name="T">Subject type</typeparam>
public interface IStoredActionRevision<T> : IActionRevision<T>
    where T : class
{
    /// <summary>
    /// Incremental revision number to maintain track and uniqueness
    /// of the action updates
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Revision cannot be less than 1")]
    int Revision { get; set; }
    
    /// <summary>
    /// Marks the action as active or inactive. Inactive actions are not executed.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    bool Active { get; set; }
    
    /// <summary>
    /// Hash of the action code used for tampering detection.
    /// Generated when a new revision is constructed.
    /// Checked when value is set, when object is retrieved from database.
    /// </summary>
    string Hash { get; set; }
    
    /// <summary>
    /// Revision create date time
    /// </summary>
    [Required]
    DateTimeOffset Created { get; set; }
}
