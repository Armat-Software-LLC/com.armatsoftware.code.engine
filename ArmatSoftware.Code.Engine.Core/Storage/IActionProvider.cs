using System.Collections.Generic;

namespace ArmatSoftware.Code.Engine.Core.Storage
{
    /// <summary>
    /// Interface is a simple provider for actions
    /// to be used by the compiler
    /// </summary>
    public interface IActionProvider
    {
        public IEnumerable<ISubjectAction<TSubject>> Retrieve<TSubject>(string key = "")
            where TSubject : class;
    }
}
