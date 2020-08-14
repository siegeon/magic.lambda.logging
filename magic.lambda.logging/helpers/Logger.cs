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
    public class Logger : ILogger
    {
        readonly IServiceProvider _services;
        readonly IConfiguration _configuration;

        public Logger(IServiceProvider services, IConfiguration configuration)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region [ -- Interface implementations -- ]

        public void Debug(string value)
        {
            InsertLogEntry("debug", value);
        }

        public void Error(string value, Exception error = null)
        {
            InsertLogEntry("error", value, error);
        }

        public void Fatal(string value, Exception error = null)
        {
            InsertLogEntry("fatal", value, error);
        }

        public void Info(string value)
        {
            InsertLogEntry("info", value);
        }

        public Task DebugAsync(string value)
        {
            return InsertLogEntryAsync("debug", value);
        }

        public Task ErrorAsync(string value, Exception error = null)
        {
            return InsertLogEntryAsync("error", value);
        }

        public Task FatalAsync(string value, Exception error = null)
        {
            return InsertLogEntryAsync("fatal", value);
        }

        public Task InfoAsync(string value)
        {
            return InsertLogEntryAsync("info", value);
        }

        #endregion

        #region [ -- Private helper methods and properties -- ]

        string DatabaseType
        {
            get => _configuration.GetSection("magic:databases:default").Value;
        }

        string DatabaseName
        {
            get => _configuration.GetSection("magic:logging:database").Value;
        }

        ISignaler Signaler
        {
            get => _services.GetService(typeof(ISignaler)) as ISignaler;
        }

        void InsertLogEntry(
            string type,
            string content,
            Exception error = null)
        {
            var lambda = new Node($"{DatabaseType}.connect", DatabaseName);
            var createNode = new Node($"{DatabaseType}.create");
            createNode.Add(new Node("table", "log_entries"));
            var valuesNode = new Node("values");
            valuesNode.Add(new Node("type", type));
            valuesNode.Add(new Node("content", content));
            if (error != null)
                valuesNode.Add(new Node("exception", error.Message + "\r\n" + error.StackTrace));
            createNode.Add(valuesNode);
            lambda.Add(createNode);
            Signaler.Signal("eval", new Node("", null, new Node[] { lambda }));
        }

        async Task InsertLogEntryAsync(
            string type,
            string content,
            Exception error = null)
        {
            var lambda = new Node($"wait.{DatabaseType}.connect", DatabaseName);
            var createNode = new Node($"wait.{DatabaseType}.create");
            createNode.Add(new Node("table", "log_entries"));
            var valuesNode = new Node("values");
            valuesNode.Add(new Node("type", type));
            valuesNode.Add(new Node("content", content));
            if (error != null)
                valuesNode.Add(new Node("exception", error.Message + "\r\n" + error.StackTrace));
            createNode.Add(valuesNode);
            lambda.Add(createNode);
            await Signaler.SignalAsync("wait.eval", new Node("", null, new Node[] { lambda }));
        }

        #endregion
    }
}
