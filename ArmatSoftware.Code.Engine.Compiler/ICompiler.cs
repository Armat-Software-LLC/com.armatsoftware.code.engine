using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler;

/// <summary>
/// Compiler of actions into an executor
/// </summary>
/// <typeparam name="TSubject">Subject type</typeparam>
public interface ICompiler<TSubject> where TSubject: class
{
	/// <summary>
	/// Compiles configuration containing actions and imports into an IExecutor object
	/// </summary>
	/// <param name="configuration">Compiler configuration</param>
	/// <returns>IExecutor object</returns>
	IFactoryExecutor<TSubject> Compile(ICompilerConfiguration<TSubject> configuration);
}