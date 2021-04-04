/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Threading.Tasks;
using Xunit;
using magic.node.extensions;
using magic.lambda.logging.helpers;

namespace magic.lambda.logging.tests
{
    public class LoggingTests
    {
        [Fact]
        public void LogInfo()
        {
            Common.Evaluate(@"log.info:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:info\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogError()
        {
            Common.Evaluate(@"log.error:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:error\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogException()
        {
            var services = Common.InitializeServices((node) =>
            {
                Assert.Contains("exception:@\"System.ArgumentException", node.ToHyperlambda());
            });
            var logger = services.GetService(typeof(ILogger)) as ILogger;
            try
            {
                throw new ArgumentException("foo");
            }
            catch (Exception err)
            {
                logger.Error("foo", err);
            }
        }

        [Fact]
        public void LogDebug()
        {
            Common.Evaluate(@"log.debug:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:debug\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogFatal()
        {
            Common.Evaluate(@"log.fatal:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:fatal\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogInfoAsync()
        {
            await Common.EvaluateAsync(@"log.info:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:info\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogErrorAsync()
        {
            await Common.EvaluateAsync(@"log.error:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:error\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogDebugAsync()
        {
            await Common.EvaluateAsync(@"log.debug:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:debug\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogFatalAsync()
        {
            await Common.EvaluateAsync(@"log.fatal:foo", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:fatal\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogInfoChildren()
        {
            Common.Evaluate(@"log.info
   .:foo
   .:-
   .:bar", (node) =>
            {
                Assert.Equal("\"\"\r\n   data.connect:magic\r\n      data.create\r\n         table:log_entries\r\n         values\r\n            type:info\r\n            content:foo-bar\r\n", node.ToHyperlambda());
            });
        }
    }
}
