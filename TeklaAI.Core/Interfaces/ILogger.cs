using System;

namespace TeklaAI.Core.Interfaces
{
    /// <summary>
    /// Interface for logging operations
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log informational message
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Log warning message
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// Log error message with optional exception
        /// </summary>
        void Error(string message, Exception ex = null);

        /// <summary>
        /// Log success message
        /// </summary>
        void Success(string message);

        /// <summary>
        /// Log debug message
        /// </summary>
        void Debug(string message);
    }
}