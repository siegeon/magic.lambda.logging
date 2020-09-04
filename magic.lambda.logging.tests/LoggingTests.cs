/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using magic.node;
using magic.node.extensions;

namespace magic.lambda.logging.tests
{
    public class LoggingTests
    {
        [Fact]
        public void LogInfo()
        {
            Common.Evaluate(@"log.info:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   mysql.connect:magic\r\n      mysql.create\r\n         table:log_entries\r\n         values\r\n            type:info\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogError()
        {
            Common.Evaluate(@"log.error:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   mysql.connect:magic\r\n      mysql.create\r\n         table:log_entries\r\n         values\r\n            type:error\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogDebug()
        {
            Common.Evaluate(@"log.debug:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   mysql.connect:magic\r\n      mysql.create\r\n         table:log_entries\r\n         values\r\n            type:debug\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public void LogFatal()
        {
            Common.Evaluate(@"log.fatal:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   mysql.connect:magic\r\n      mysql.create\r\n         table:log_entries\r\n         values\r\n            type:fatal\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogInfoAsync()
        {
            await Common.EvaluateAsync(@"wait.log.info:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   wait.mysql.connect:magic\r\n      wait.mysql.create\r\n         table:log_entries\r\n         values\r\n            type:info\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogErrorAsync()
        {
            await Common.EvaluateAsync(@"wait.log.error:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   wait.mysql.connect:magic\r\n      wait.mysql.create\r\n         table:log_entries\r\n         values\r\n            type:error\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogDebugAsync()
        {
            await Common.EvaluateAsync(@"wait.log.debug:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   wait.mysql.connect:magic\r\n      wait.mysql.create\r\n         table:log_entries\r\n         values\r\n            type:debug\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }

        [Fact]
        public async Task LogFatalAsync()
        {
            await Common.EvaluateAsync(@"wait.log.fatal:foo", (node) =>
            {
                System.Console.WriteLine(node.ToHyperlambda());
                Assert.Equal("\"\"\r\n   wait.mysql.connect:magic\r\n      wait.mysql.create\r\n         table:log_entries\r\n         values\r\n            type:fatal\r\n            content:foo\r\n", node.ToHyperlambda());
            });
        }
    }
}
