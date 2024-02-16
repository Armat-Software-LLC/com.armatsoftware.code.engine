﻿using System;

namespace ArmatSoftware.Code.Engine.Core
{
	/// <summary>
	/// Executes one or more actions against the subject
	/// </summary>
	/// <typeparam name="TSubject">Subject of type S</typeparam>
	public interface IExecutor<TSubject> : IExecutionContext where TSubject : class
	{
		/// <summary>
		/// Subject of the action execution
		/// </summary>
		TSubject Subject { get; set; }

		/// <summary>
		/// Execute the actions
		/// </summary>
		void Execute();
		
		/// <summary>
		/// Simplified execution of the actions
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		TSubject Execute(TSubject subject);
		
		/// <summary>
		/// Efficient cloning of the executors allows effective thread safe execution
		/// without using singletons or activating new instances
		/// </summary>
		/// <returns></returns>
		IExecutor<TSubject> Clone();
	}
}
