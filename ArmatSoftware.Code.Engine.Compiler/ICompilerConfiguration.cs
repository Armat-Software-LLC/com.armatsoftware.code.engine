using System;
using System.Collections.Generic;
using ArmatSoftware.Code.Engine.Core;

namespace ArmatSoftware.Code.Engine.Compiler
{
	/// <summary>
	/// Compiler configuration
	/// </summary>
	/// <typeparam name="S">Subject type</typeparam>
	public interface ICompilerConfiguration<S> : ITemplateConfiguration where S : class
	{
		/// <summary>
		/// List of actions to compile
		/// </summary>
		IList<ISubjectAction<S>> Actions { get; set; }

		/// <summary>
		/// List of imports to add to the template
		/// </summary>
		IList<Type> References { get; set; }
	}
}