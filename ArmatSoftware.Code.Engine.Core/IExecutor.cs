using System;
using ArmatSoftware.Code.Engine.Core.Logging;

namespace ArmatSoftware.Code.Engine.Core
{
	/// <summary>
	/// Executes one or more actions against the subject
	/// </summary>
	/// <typeparam name="TSubject">Subject of type S</typeparam>
	public interface IExecutor<TSubject> : IExecutionContext where TSubject : class
	{
		/// <summary>
		/// Subject of the action execution.
		/// Check and use after the execution.
		/// </summary>
		TSubject Subject { get; }
		
		/// <summary>
		/// Simplified execution of the actions
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		TSubject Execute(TSubject subject);

		/// <summary>
		/// Get logger instance
		/// </summary>
		ICodeEngineLogger Log { get; }
	}
}
