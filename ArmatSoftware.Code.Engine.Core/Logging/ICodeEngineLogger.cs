using System;

namespace ArmatSoftware.Code.Engine.Core.Logging
{
    public interface ICodeEngineLogger
    {
        /// <summary>
        /// Write out informational message
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Write out warning
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);

        /// <summary>
        /// Write out error with an optional exception parameter
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Error(string message, Exception ex = null);
    }
}