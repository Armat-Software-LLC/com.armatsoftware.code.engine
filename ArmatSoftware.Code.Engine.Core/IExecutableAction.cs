namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// An action (function) that is given a name, order of execution, and the logic as text values
/// </summary>
public interface IExecutableAction
{
    /// <summary>
    /// Meaningful name of the action
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Source code of the action logic. Length is limited to 10000 characters
    /// to promote healthy coding practices.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// In a series of actions to be applied to a subject, this property
    /// dictates the order of execution.
    /// </summary>
    int Order { get; set; }
}