/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.logging.helpers
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
        public Logger(ISignaler signaler, IMagicConfiguration magicConfiguration)
        {
            _signaler = signaler;
            _magicConfiguration = magicConfiguration;
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
            // Retrieving IDbConnection to use.
            var dbType = _magicConfiguration["magic:databases:default"];
            var dbNode = new Node();
            _signaler.Signal($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<IDbConnection>())
            {
                // Opening database connection.
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                connection.Open();

                // Creating our insert commend.
                using (var cmd = CreateCommand(connection, type, content, error))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        async Task InsertLogEntryAsync(
            string type,
            string content,
            Exception error = null)
        {
            // Retrieving IDbConnection to use.
            var dbType = _magicConfiguration["magic:databases:default"];
            var dbNode = new Node();
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<IDbConnection>())
            {
                // Opening database connection.
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                connection.Open();

                // Creating our insert commend.
                using (var cmd = CreateCommand(connection, type, content, error))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /*
         * Creates an IDbCommand that inserts into log database, and returns the command to caller.
         */
        static IDbCommand CreateCommand(IDbConnection connection, string type, string content, Exception error)
        {
            // Creating our SQL command.
            var command = connection.CreateCommand();
            var builder = new StringBuilder();
            builder.Append("insert into log_entries (type, content");
            if (error != null)
                builder.Append(", exception");
            builder.Append(") values (@arg1, @arg2");
            if (error != null)
                builder.Append(", @arg3");
            builder.Append(")");
            command.CommandText = builder.ToString();

            // Adding arguments to invocation.
            var typeArg = command.CreateParameter();
            typeArg.ParameterName = "@arg1";
            typeArg.Value = type;
            command.Parameters.Add(typeArg);

            var contentArg = command.CreateParameter();
            contentArg.ParameterName = "@arg2";
            contentArg.Value = content;
            command.Parameters.Add(contentArg);

            if (error != null)
            {
                var exceptionArg = command.CreateParameter();
                exceptionArg.ParameterName = "@arg3";
                exceptionArg.Value = error.StackTrace;
                command.Parameters.Add(exceptionArg);
            }

            // Returning command to caller.
            return command;
        }

        #endregion
    }
}
