using System;

namespace ArmatSoftware.Code.Engine.Core.Storage
{
    /// <summary>
    /// Interface for code engine storage adapters.
    /// An implementation of this interface will be responsible for storing and retrieving
    /// individual custom logic files for specific IExecutors.
    /// </summary>
    public interface ICodeEngineStorage
    {
        /// <summary>
        /// Store custom logic code for a specific subject type and executor id.
        /// </summary>
        /// <param name="subjectType">Type of the subject handled in <code>ArmatSoftware.Code.Engine.Core.IExecutor</code></param>
        /// <param name="executorId">Key or index of the executor for the subject</param>
        /// <param name="code">Text payload of the custom logic in one of handed languages</param>
        void Store(Type subjectType, Guid executorId, string code);

        /// <summary>
        /// Retrieve custom logic code for a specific subject type and executor id.
        /// </summary>
        /// <param name="subjectType">Subject type</param>
        /// <param name="executorId">Executor identifier from the storage</param>
        /// <returns>text payload - code to be compiled</returns>
        public string Retrieve(Type subjectType, Guid executorId);
    }
}
