/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using magic.node;
using magic.signals.services;
using magic.signals.contracts;
using magic.lambda.logging.helpers;
using magic.node.extensions.hyperlambda;

namespace magic.lambda.logging.tests
{
    public static class Common
    {
        static public Node Evaluate(string hl, Action<Node> functor)
        {
            var signaler = Initialize(functor);
            var lambda = new Parser(hl).Lambda();
            signaler.Signal("eval", lambda);
            return lambda;
        }

        static async public Task<Node> EvaluateAsync(string hl, Action<Node> functor)
        {
            var signaler = Initialize(functor);
            var lambda = new Parser(hl).Lambda();
            await signaler.SignalAsync("eval", lambda);
            return lambda;
        }

        public static ISignaler Initialize(Action<Node> functor)
        {
            var services = new ServiceCollection();
            services.AddTransient<ISignaler, Signaler>();

            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(services));
            services.AddTransient<ISignalsProvider>((svc) => types);
            services.AddTransient<ILogger>((svc) =>
            {
                return new TestLogger(
                    svc.GetService<ISignaler>(),
                    functor);
            });
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .SetupGet(x => x[It.Is<string>(x => x == "magic:databases:default")])
                .Returns(() => "mysql");
            mockConfiguration
                .SetupGet(x => x[It.Is<string>(x => x == "magic:logging:database")])
                .Returns(() => "magic");
            services.AddTransient((svc) => mockConfiguration.Object);
            var provider = services.BuildServiceProvider();
            return provider.GetService<ISignaler>();
        }

        public static IServiceProvider InitializeServices(Action<Node> functor)
        {
            var services = new ServiceCollection();
            services.AddTransient<ISignaler, Signaler>();

            var types = new SignalsProvider(InstantiateAllTypes<ISlot>(services));
            services.AddTransient<ISignalsProvider>((svc) => types);
            services.AddTransient<ILogger>((svc) =>
            {
                return new TestLogger(
                    svc.GetService<ISignaler>(),
                    functor);
            });
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .SetupGet(x => x[It.Is<string>(x => x == "magic:databases:default")])
                .Returns(() => "mysql");
            mockConfiguration
                .SetupGet(x => x[It.Is<string>(x => x == "magic:logging:database")])
                .Returns(() => "magic");
            services.AddTransient((svc) => mockConfiguration.Object);
            return services.BuildServiceProvider();
        }

        #region [ -- Private helper methods -- ]

        class TestLogger : Logger
        {
            readonly Action<Node> _functor;

            public TestLogger(ISignaler signaler, Action<Node> functor)
                : base(signaler)
            {
                _functor = functor;
            }

            protected override void Signal(Node node)
            {
                _functor(node);
            }

            protected override Task SignalAsync(Node node)
            {
                _functor(node);
                return Task.CompletedTask;
            }
        }

        static IEnumerable<Type> InstantiateAllTypes<T>(ServiceCollection services) where T : class
        {
            var type = typeof(T);
            var result = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !x.FullName.StartsWith("Microsoft", StringComparison.InvariantCulture))
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var idx in result)
            {
                services.AddTransient(idx);
            }
            return result;
        }

        #endregion
    }
}
