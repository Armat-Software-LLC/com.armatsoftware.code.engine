using System;
using System.Collections.Generic;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler;

/// <summary>
/// Compiler configuration
/// </summary>
/// <typeparam name="TSubject">Subject type</typeparam>
public interface ICompilerConfiguration<TSubject> : ITemplateConfiguration where TSubject: class
{
	/// <summary>
	/// List of actions to compile
	/// </summary>
	IList<ISubjectAction<TSubject>> Actions { get; set; }

	/// <summary>
	/// List of imports to add to the template
	/// </summary>
	IList<Type> References { get; set; }
		
	/// <summary>
	/// Should compiler validate models resulting from execution?
	/// </summary>
	bool ValidateModelsAfterExecution { get; set; }
}