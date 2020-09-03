/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using Microsoft.Extensions.Configuration;
using magic.node;
using magic.signals.contracts;
using System.Threading.Tasks;

namespace magic.lambda.logging.helpers
{
    /// <inheritdoc/>
    public class Logger : ILogger
    {
        readonly string _databaseType;
        readonly string _databaseName;
        readonly ISignaler _signaler;

        /// <summary>
        /// Constructs a new instance of the default ILogger implementation.
        /// </summary>
        /// <param name="services">IoC container</param>
        /// <param name="configuration">Configuration instance</param>
        public Logger(IServiceProvider services, IConfiguration configuration)
        {
            _databaseType = configuration.GetSection("magic:databases:default").Value;
            _databaseName = configuration.GetSection("magic:logging:database").Value;
            _signaler = services.GetService(typeof(ISignaler)) as ISignaler;;
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

        #region [ -- Private helper methods and properties -- ]

        void InsertLogEntry(
            string type,
            string content,
            Exception error = null)
        {
            Node lambda = BuildLambda(type, content, error, false);
            _signaler.Signal("eval", new Node("", null, new Node[] { lambda }));
        }

        async Task InsertLogEntryAsync(
            string type,
            string content,
            Exception error = null)
        {
            Node lambda = BuildLambda(type, content, error, true);
            await _signaler.SignalAsync("wait.eval", new Node("", null, new Node[] { lambda }));
        }

        Node BuildLambda(string type, string content, Exception error, bool isAsync)
        {
            var lambda = new Node((isAsync ? "wait." : "") + $"{_databaseType}.connect", _databaseName);
            var createNode = new Node((isAsync ? "wait." : "") + $"{_databaseType}.create");
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
