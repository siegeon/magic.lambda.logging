/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using magic.node;
using magic.node.contracts;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.logging.contracts;

namespace magic.lambda.logging.services
{
    /// <inheritdoc/>
    public class Logger : ILogger, ILogQuery
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
        public Task DebugAsync(string content)
        {
            return InsertLogEntryAsync(_signaler, "debug", content, null, null);
        }

        /// <inheritdoc/>
        public Task DebugAsync(string content, Dictionary<string, string> meta)
        {
            return InsertLogEntryAsync(_signaler, "debug", content, meta, null);
        }

        /// <inheritdoc/>
        public Task InfoAsync(string content)
        {
            return InsertLogEntryAsync(_signaler, "info", content, null, null);
        }

        /// <inheritdoc/>
        public Task InfoAsync(string content, Dictionary<string, string> meta)
        {
            return InsertLogEntryAsync(_signaler, "info", content, meta, null);
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string content)
        {
            return InsertLogEntryAsync(_signaler, "error", content, null, null);
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string content, Dictionary<string, string> meta)
        {
            return InsertLogEntryAsync(_signaler, "error", content, meta, null);
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string content, string stackTrace)
        {
            return InsertLogEntryAsync(_signaler, "error", content, null, stackTrace);
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string content, Dictionary<string, string> meta, string stackTrace)
        {
            return InsertLogEntryAsync(_signaler, "error", content, meta, stackTrace);
        }

        /// <inheritdoc/>
        public Task FatalAsync(string content)
        {
            return InsertLogEntryAsync(_signaler, "fatal", content, null, null);
        }

        /// <inheritdoc/>
        public Task FatalAsync(string content, Dictionary<string, string> meta)
        {
            return InsertLogEntryAsync(_signaler, "fatal", content, meta, null);
        }

        /// <inheritdoc/>
        public Task FatalAsync(string content, string stackTrace)
        {
            return InsertLogEntryAsync(_signaler, "fatal", content, null, stackTrace);
        }

        /// <inheritdoc/>
        public Task FatalAsync(string content, Dictionary<string, string> meta, string stackTrace)
        {
            return InsertLogEntryAsync(_signaler, "fatal", content, meta, stackTrace);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LogItem>> QueryAsync(int max, object fromId, string content = null)
        {
            var dbNode = new Node();
            var dbType = _magicConfiguration["magic:databases:default"];
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<DbConnection>())
            {
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.Append("select id, created, type, content, exception, meta from log_entries");
                    if (fromId != null)
                    {
                        builder.Append(" where ");
                        builder.Append($"id < {Convert.ToInt64(fromId)}");
                    }
                    if (!string.IsNullOrEmpty(content))
                    {
                        if (fromId == null)
                            builder.Append(" where ");
                        else
                            builder.Append(" and ");
                        builder.Append("content = @content");
                        var contentArg = command.CreateParameter();
                        contentArg.ParameterName = "@content";
                        contentArg.Value = content;
                        command.Parameters.Add(contentArg);
                    }
                    builder.Append($" order by id desc limit {max}");
                    command.CommandText = builder.ToString();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<LogItem>();
                        while (await reader.ReadAsync())
                        {
                            var dt = (DateTime)reader["created"];
                            var jString = reader["meta"] as string;
                            var metaObj = jString == null ? null : JObject.Parse(jString);
                            result.Add(new LogItem
                            {
                                Id = Convert.ToString(reader["id"]),
                                Created = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc),
                                Type = reader["type"] as string,
                                Content = reader["content"] as string,
                                Exception = reader["exception"] as string,
                                Meta = metaObj?.ToObject<Dictionary<string, string>>(),
                            });
                        }
                        return result;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<long> CountAsync(string content = null)
        {
            var dbNode = new Node();
            var dbType = _magicConfiguration["magic:databases:default"];
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<DbConnection>())
            {
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.Append("select count(id) from log_entries");
                    if (!string.IsNullOrEmpty(content))
                    {
                        builder.Append(" where content = @content");
                        var contentArg = command.CreateParameter();
                        contentArg.ParameterName = "@content";
                        contentArg.Value = content;
                        command.Parameters.Add(contentArg);
                    }
                    command.CommandText = builder.ToString();
                    return Convert.ToInt64(await command.ExecuteScalarAsync());
                }
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<(string Type, long Count)>> Types()
        {
            var dbNode = new Node();
            var dbType = _magicConfiguration["magic:databases:default"];
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<DbConnection>())
            {
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select count(*) as count, type from log_entries group by type";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<(string Type, long Count)>();
                        while (await reader.ReadAsync())
                        {
                            var type = reader["type"] as string;
                            var count = Convert.ToInt64(reader["count"]);
                            result.Add((type, count));
                        }
                        return result;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<(string When, long Count)>> Timeshift(string content)
        {
            var dbNode = new Node();
            var dbType = _magicConfiguration["magic:databases:default"];
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<DbConnection>())
            {
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    var date = "";
                    var tail = "";
                    switch (dbType)
                    {
                        case "mysql":
                            date = "date_format(created, '%Y-%m-%d')";
                            tail = " limit 14";
                            break;

                        case "mssql":
                            date = "convert(char(10), created, 126)";
                            tail = " offset 0 rows fetch first 14 rows only";
                            break;

                        case "pgsql":
                            date = "to_char(created, 'YYYY-MM-dd')";
                            tail = " limit 14";
                            break;
                    }
                    var contentArg = command.CreateParameter();
                    contentArg.ParameterName = "@content";
                    contentArg.Value = content;
                    command.Parameters.Add(contentArg);
                    command.CommandText = $"select {date} as date, count(*) as count from log_entries where content = @content group by {date} order by {date} desc{tail}";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<(string When, long Count)>();
                        while (await reader.ReadAsync())
                        {
                            var when = reader["date"] as string;
                            var count = Convert.ToInt64(reader["count"]);
                            result.Add((when, count));
                        }
                        return result;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<LogItem> Get(object id)
        {
            var dbNode = new Node();
            var dbType = _magicConfiguration["magic:databases:default"];
            await _signaler.SignalAsync($".db-factory.connection.{dbType}", dbNode);
            using (var connection = dbNode.Get<DbConnection>())
            {
                connection.ConnectionString = _magicConfiguration[$"magic:databases:{dbType}:generic"].Replace("{database}", "magic");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.Append("select id, created, type, content, exception from log_entries where id = @id");
                    var filter = command.CreateParameter();
                    filter.ParameterName = "@id";
                    filter.Value = id;
                    command.Parameters.Add(filter);
                    command.CommandText = builder.ToString();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var result = new List<LogItem>();
                        if (await reader.ReadAsync())
                        {
                            var dt = (DateTime)reader["created"];
                            var jString = reader["meta"] as string;
                            var metaObj = jString == null ? null : JObject.Parse(jString);
                            return new LogItem
                            {
                                Id = Convert.ToString(reader["id"]),
                                Created = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc),
                                Type = reader["type"] as string,
                                Content = reader["content"] as string,
                                Exception = reader["exception"] as string,
                                Meta = metaObj?.ToObject<Dictionary<string, string>>(),
                            };
                        }
                        throw new HyperlambdaException($"Couldn't find log item with ID '{id}'");
                    }
                }
            }
        }

        #endregion

        #region [ -- Private helper methods and properties -- ]

        async Task InsertLogEntryAsync(
            ISignaler signaler,
            string type,
            string content,
            Dictionary<string, string> meta,
            string stackTrace)
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
                    using (var cmd = CreateCommand(signaler, connection, dbType, type, content, meta, stackTrace))
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
            ISignaler signaler,
            DbConnection connection,
            string dbType,
            string type,
            string content,
            Dictionary<string, string> meta,
            string stackTrace)
        {
            // Creating our SQL command.
            var command = connection.CreateCommand();
            var builder = new StringBuilder();
            if (dbType == "mysql")
                builder.Append("set time_zone = '+00:00'; ");
            builder.Append("insert into log_entries (type, content");
            if (stackTrace != null)
                builder.Append(", exception");
            if (meta != null && meta.Count > 0)
                builder.Append(", meta");
            builder.Append(") values (@type, @content");
            if (stackTrace != null)
                builder.Append(", @exception");
            if (meta != null && meta.Count > 0)
                builder.Append(", @meta");
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

            if (stackTrace != null)
            {
                var exceptionArg = command.CreateParameter();
                exceptionArg.ParameterName = "@exception";
                exceptionArg.Value = stackTrace;
                command.Parameters.Add(exceptionArg);
            }

            if (meta != null && meta.Count > 0)
            {
                var node = new Node();
                node.AddRange(meta.Select(x => new Node(x.Key, x.Value)));
                signaler.Signal("lambda2json", node);

                var metaArg = command.CreateParameter();
                metaArg.ParameterName = "@meta";
                metaArg.Value = node.Value;
                command.Parameters.Add(metaArg);
            }

            // Returning command to caller.
            return command;
        }

        #endregion
    }
}
