/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using magic.lambda.logging.helpers;

namespace magic.lambda.logging.tests
{
    public class LoggingTests
    {
        [Fact]
        public void LogInfo()
        {
            ConnectionFactory.Arguments.Clear();
            Common.Evaluate(@"log.info:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "info"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public void LogError()
        {
            ConnectionFactory.Arguments.Clear();
            Common.Evaluate(@"log.error:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "error"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public void LogException()
        {
            ConnectionFactory.Arguments.Clear();
            var services = Common.InitializeServices();
            var logger = services.GetService(typeof(ILogger)) as ILogger;
            try
            {
                throw new ArgumentException("fooERROR");
            }
            catch (Exception err)
            {
                logger.Error("foo", err);
            }
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content, exception) values (@arg1, @arg2, @arg3)", ConnectionFactory.CommandText);
            Assert.Equal(3, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "error"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg3" && x.Item2.Contains("magic.lambda.logging.tests.LoggingTests.LogException")));
        }

        [Fact]
        public void LogDebug()
        {
            ConnectionFactory.Arguments.Clear();
            Common.Evaluate(@"log.debug:foo-bar");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "debug"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo-bar"));
        }

        [Fact]
        public void LogFatal()
        {
            ConnectionFactory.Arguments.Clear();
            Common.Evaluate(@"log.fatal:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "fatal"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public async Task LogInfoAsync()
        {
            ConnectionFactory.Arguments.Clear();
            await Common.EvaluateAsync(@"log.info:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "info"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public async Task LogErrorAsync()
        {
            ConnectionFactory.Arguments.Clear();
            await Common.EvaluateAsync(@"log.error:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "error"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public async Task LogDebugAsync()
        {
            ConnectionFactory.Arguments.Clear();
            await Common.EvaluateAsync(@"log.debug:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "debug"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public async Task LogFatalAsync()
        {
            ConnectionFactory.Arguments.Clear();
            await Common.EvaluateAsync(@"log.fatal:foo");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "fatal"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo"));
        }

        [Fact]
        public void LogInfoChildren()
        {
            ConnectionFactory.Arguments.Clear();
            Common.Evaluate(@"log.info
   .:foo
   .:-
   .:bar");
            Assert.Equal("CONNECTION-STRING-magic", ConnectionFactory.ConnectionString);
            Assert.Equal("set time_zone = '+00:00'; insert into log_entries (type, content) values (@arg1, @arg2)", ConnectionFactory.CommandText);
            Assert.Equal(2, ConnectionFactory.Arguments.Count);
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg1" && x.Item2 == "info"));
            Assert.Single(ConnectionFactory.Arguments.Where(x => x.Item1 == "@arg2" && x.Item2 == "foo-bar"));
        }
    }
}
