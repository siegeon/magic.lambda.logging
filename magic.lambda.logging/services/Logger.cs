/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.logging.contracts;

namespace magic.lambda.logging.services
{
    /// <inheritdoc/>
    public class Logger : ILogger
    {
        readonly ISignaler _signaler;
        readonly IMagicConfiguration _magicConfiguration;

        /// <summary>
        /// Constructs a new instance of the default ILogger implementation.
        /// </summary>
        /// <param name="signaler">ISignaler implementation.</param>
        /// <param name="magicConfiguration">Configuration object.</param>
        public Logger(ISignaler signaler, IMagicConfiguration magicConfiguration)
        {
            _signaler = signaler;
            _magicConfiguration = magicConfiguration;
        }

        #region [ -- Interface implementations -- ]

        /// <inheritdoc/>
        public void Debug(string value)
        {
            InsertLogEntryAsync("debug", value)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public void Error(string value, Exception error = null)
        {
            InsertLogEntryAsync("error", value, error)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public void Error(string value, string stackTrace)
        {
            InsertLogEntryAsync("error", value, null, stackTrace)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public void Fatal(string value, Exception error = null)
        {
            InsertLogEntryAsync("fatal", value, error)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public void Info(string value)
        {
            InsertLogEntryAsync("info", value)
                .GetAwaiter()
                .GetResult();
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
        public Task ErrorAsync(string value, string stackTrace)
        {
            return InsertLogEntryAsync("error", value, null, stackTrace);
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

        async Task InsertLogEntryAsync(
            string type,
            string content,
            Exception error = null, 
            string stackTrace = null)
        {
            // Retrieving IDbConnection to use.
            var dbType = _magicConfiguration["magic:databases:default"];
            var level = _magicConfiguration["magic:logging:level"] ?? "debug";
            var shouldLog = false;
            switch (type)
            {
                case "debug":
                    shouldLog = level == "debug";
                    break;

                case "info":
                    shouldLog = level == "info" || level == "debug";
                    break;

                case "error":
                    shouldLog = level == "error" || level == "info" || level == "debug";
                    break;

                case "fatal":
                    shouldLog = level == "fatal" || level == "error" || level == "info" || level == "debug";
                    break;
            }

            // Verifying we're supposed to log.
            if (shouldLog)
            {
                var dbNode = new Node();
                await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
                using (var connection = dbNode.Get<DbConnection>())
                {
                    // Opening database connection.
                    connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                    await connection.OpenAsync();

                    // Creating our insert commend.
                    using (var cmd = CreateCommand(connection, dbType, type, content, error, stackTrace))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /*
         * Creates an IDbCommand that inserts into log database, and returns the command to caller.
         */
        static DbCommand CreateCommand(
            DbConnection connection,
            string dbType,
            string type,
            string content,
            Exception error,
            string stackTrace)
        {
            // Creating our SQL command.
            var command = connection.CreateCommand();
            var builder = new StringBuilder();
            if (dbType == "mysql")
                builder.Append("set time_zone = '+00:00'; ");
            builder.Append("insert into log_entries (type, content");
            if (error != null || stackTrace != null)
                builder.Append(", exception");
            builder.Append(") values (@type, @content");
            if (error != null)
                builder.Append(", @exception");
            else if (stackTrace != null)
                builder.Append(", @exception");
            builder.Append(")");
            command.CommandText = builder.ToString();

            // Adding arguments to invocation.
            var typeArg = command.CreateParameter();
            typeArg.ParameterName = "@type";
            typeArg.Value = type;
            command.Parameters.Add(typeArg);

            var contentArg = command.CreateParameter();
            contentArg.ParameterName = "@content";
            contentArg.Value = content;
            command.Parameters.Add(contentArg);

            if (error != null || stackTrace != null)
            {
                var exceptionArg = command.CreateParameter();
                exceptionArg.ParameterName = "@exception";
                exceptionArg.Value = stackTrace ?? error.StackTrace;
                command.Parameters.Add(exceptionArg);
            }

            // Returning command to caller.
            return command;
        }

        #endregion
    }
}
