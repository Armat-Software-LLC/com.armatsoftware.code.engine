using System;
using System.Collections.Generic;
using System.Linq;
using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Compiler;

namespace ArmatSoftware.Code.Engine.Compiler.Base
{
	/// <summary>
	/// Default implementation of the configuration contract
	/// </summary>
	/// <typeparam name="S">Type of subject</typeparam>
	public class CompilerConfiguration<S> : ICompilerConfiguration<S> where S : class
	{
		private readonly string _nameSpace;

		private readonly string _className;

		/// <summary>
		/// List of actions added in the order of execution
		/// </summary>
		public IList<IAction<S>> Actions { get; set; } = new List<IAction<S>>();

		/// <summary>
		/// List of referenced types used in the action logic
		/// </summary>
		public IList<Type> References { get; set; } = new List<Type>();

		/// <summary>
		/// Hidden constructor
		/// </summary>
		private CompilerConfiguration() { }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="nameSpace">Namespace to use for the comiled class</param>
		public CompilerConfiguration(string nameSpace)
		{
			this._nameSpace = nameSpace;
			this._className = $"Executor_{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
		}

		public IDictionary<string, string> GetActions()
		{
			var actionDictionary = new Dictionary<string, string>();
			foreach(var action in Actions)
			{
				actionDictionary.Add(action.Name, action.Code);
			}

			return actionDictionary;
		}

		public string GetClassName()
		{
			return this._className;
		}

		public IEnumerable<string> GetImports()
		{
			return References.Select(import => import.Namespace).Distinct();
		}

		public string GetNamespace()
		{
			return this._nameSpace;
		}

		public string GetSubjectType()
		{
			return typeof(S).Name;
		}
	}
}
