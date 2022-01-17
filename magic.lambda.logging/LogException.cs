/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.logging.helpers;

namespace magic.lambda.logging
{
    /// <summary>
    /// [log.exception] slot for logging exception entries.
    /// </summary>
    [Slot(Name = "log.exception")]
    public class LogException : ISlotAsync, ISlot
    {
        readonly ILogger _logger;

        /// <summary>
        /// Creates an instance of your type.
        /// </summary>
        /// <param name="logger">Actual implementation.</param>
        public LogException(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var exceptionNode = input.Children.FirstOrDefault(x => x.Name == "exception");
            var stack = exceptionNode?.GetEx<string>();
            exceptionNode?.UnTie();
            _logger.Error(Utilities.GetLogContent(input, signaler), stack);
            input.Clear(); // House cleaning.
        }

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var exceptionNode = input.Children.FirstOrDefault(x => x.Name == "exception");
            var stack = exceptionNode?.GetEx<string>();
            exceptionNode?.UnTie();
            await _logger.ErrorAsync(Utilities.GetLogContent(input, signaler), stack);
            input.Clear(); // House cleaning.
        }
    }
}
