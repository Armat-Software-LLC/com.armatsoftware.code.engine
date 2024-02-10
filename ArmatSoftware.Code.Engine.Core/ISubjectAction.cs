namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Action that is taken against the subject
/// </summary>
/// <typeparam name="S">Subject of type S</typeparam>
public interface ISubjectAction<S> : IExecutableAction where S : class
{ }

