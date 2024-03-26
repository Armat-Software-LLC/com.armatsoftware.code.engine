using System.Collections.Generic;

namespace ArmatSoftware.Code.Engine.Compiler;

/// <summary>
/// Configuration for generating teamplate
/// </summary>
public interface ITemplateConfiguration
{
	/// <summary>
	/// Gets list of imports for the supplied actions
	/// </summary>
	/// <returns>List of import namespaces</returns>
	IEnumerable<string> GetImports();

	/// <summary>
	/// Gets list of actions to compile
	/// </summary>
	/// <returns>Action names as keys and code as values</returns>
	IDictionary<string, string> GetActions();

	/// <summary>
	/// Compiled class namespace
	/// </summary>
	/// <returns></returns>
	string GetNamespace();

	/// <summary>
	/// Compiled class name
	/// </summary>
	/// <returns></returns>
	string GetClassName();

	/// <summary>
	/// Type name of the subject
	/// </summary>
	/// <returns></returns>
	string GetSubjectType();

	/// <summary>
	/// Should the compiler validate the resulting models after execution
	/// </summary>
	/// <returns></returns>
	bool EnableModelValidation();
}