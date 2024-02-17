namespace ArmatSoftware.Code.Engine.Core;

/// <summary>
/// Action that is taken against the subject
/// </summary>
/// <typeparam name="TSubject">Subject of type S</typeparam>
public interface ISubjectAction<TSubject> : IExecutableAction where TSubject: class
{ }

