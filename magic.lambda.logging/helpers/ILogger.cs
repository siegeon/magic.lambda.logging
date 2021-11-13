/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Threading.Tasks;

namespace magic.lambda.logging.helpers
{
    /// <summary>
    /// Implementation interface to allow usage of any actual logging
    /// implementation. Notice, must be provided by user of library.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug entry.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        void Debug(string value);

        /// <summary>
        /// Logs a debug entry.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        Task DebugAsync(string value);

        /// <summary>
        /// Logs an info entry.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        void Info(string value);

        /// <summary>
        /// Logs an info entry.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        Task InfoAsync(string value);

        /// <summary>
        /// Logs an error, optionally associated with an exception.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        /// <param name="error">Exception to log.</param>
        void Error(string value, Exception error = null);

        /// <summary>
        /// Logs an error, optionally associated with an exception.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        /// <param name="error">Exception to log.</param>
        Task ErrorAsync(string value, Exception error = null);

        /// <summary>
        /// Logs a fatal error, optionally associated with an exception.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        /// <param name="error">Exception to log.</param>
        void Fatal(string value, Exception error = null);

        /// <summary>
        /// Logs a fatal error, optionally associated with an exception.
        /// </summary>
        /// <param name="value">Entry to log.</param>
        /// <param name="error">Exception to log.</param>
        Task FatalAsync(string value, Exception error = null);
    }
}
