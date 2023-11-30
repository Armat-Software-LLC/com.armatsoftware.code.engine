namespace ArmatSoftware.Code.Engine.Core
{
	/// <summary>
	/// Execution context offers tools necessary at run-time during execution of the actions
	/// </summary>
	public interface IExecutionContext
	{
		//TODO: find how to fix error Error CS0656: Missing compiler required member 'Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create' at : (23,3)-(23,21)
		// when using dynamic type

		/// <summary>
		/// Sets run-time value
		/// </summary>
		/// <param name="key">Unique key</param>
		/// <param name="value">Value stored at run-time</param>
		void Save(string key, object value);

		/// <summary>
		/// Gets the run-time value by its unique key
		/// </summary>
		/// <param name="key">Unique key</param>
		/// <returns>Dynamic run-time value</returns>
		object Read(string key);
	}
}
