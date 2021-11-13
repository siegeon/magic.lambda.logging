/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using magic.node;
using magic.signals.contracts;
using System.Threading.Tasks;

namespace magic.lambda.logging.helpers
{
    /// <inheritdoc/>
    public class Logger : ILogger
    {
        readonly ISignaler _signaler;

        /// <summary>
        /// Constructs a new instance of the default ILogger implementation.
        /// </summary>
        /// <param name="signaler">ISignaler implementation</param>
        public Logger(ISignaler signaler)
        {
            _signaler = signaler;
        }

        #region [ -- Interface implementations -- ]

        /// <inheritdoc/>
        public void Debug(string value)
        {
            InsertLogEntry("debug", value);
        }

        /// <inheritdoc/>
        public void Error(string value, Exception error = null)
        {
            InsertLogEntry("error", value, error);
        }

        /// <inheritdoc/>
        public void Fatal(string value, Exception error = null)
        {
            InsertLogEntry("fatal", value, error);
        }

        /// <inheritdoc/>
        public void Info(string value)
        {
            InsertLogEntry("info", value);
        }

        /// <inheritdoc/>
        public Task DebugAsync(string value)
        {
            return InsertLogEntryAsync("debug", value);
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string value, Exception error = null)
        {
            return InsertLogEntryAsync("error", value, error);
        }

        /// <inheritdoc/>
        public Task FatalAsync(string value, Exception error = null)
        {
            return InsertLogEntryAsync("fatal", value, error);
        }

        /// <inheritdoc/>
        public Task InfoAsync(string value)
        {
            return InsertLogEntryAsync("info", value);
        }

        #endregion

        #region [ -- Protected virtual methods to intercept parts of implementation -- ]

        /// <summary>
        /// Override this method to intercept invocation towards [eval].
        /// </summary>
        /// <param name="node">Lambda object that actually inserts item into log.</param>
        protected virtual void Signal(Node node)
        {
            _signaler.Signal("eval", node); // NOSONAR
        }

        /// <summary>
        /// Override this method to intercept invocation towards [eval].
        /// </summary>
        /// <param name="node">Lambda object that actually inserts item into log.</param>
        protected virtual Task SignalAsync(Node node)
        {
            return _signaler.SignalAsync("eval", node); // NOSONAR
        }

        #endregion

        #region [ -- Private helper methods and properties -- ]

        void InsertLogEntry(
            string type,
            string content,
            Exception error = null)
        {
            Node lambda = BuildLambda(type, content, error);
            Signal(new Node("", null, new Node[] { lambda }));
        }

        async Task InsertLogEntryAsync(
            string type,
            string content,
            Exception error = null)
        {
            Node lambda = BuildLambda(type, content, error);
            await SignalAsync(new Node("", null, new Node[] { lambda }));
        }

        Node BuildLambda(string type, string content, Exception error)
        {
            var lambda = new Node("data.connect", "magic");
            var createNode = new Node("data.create");
            createNode.Add(new Node("table", "log_entries"));
            var valuesNode = new Node("values");
            valuesNode.Add(new Node("type", type));
            valuesNode.Add(new Node("content", content));
            if (error != null)
                valuesNode.Add(new Node("exception", error.GetType() + "\r\n" + error.StackTrace));
            createNode.Add(valuesNode);
            lambda.Add(createNode);
            return lambda;
        }

        #endregion
    }
}
