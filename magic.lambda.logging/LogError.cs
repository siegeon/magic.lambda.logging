/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using log4net;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.logging
{
    /// <summary>
    /// [log.error] slot for logging error log entries.
    /// </summary>
    [Slot(Name = "log.error")]
    public class LogError : ISlot
    {
        readonly static ILog _logger = LogManager.GetLogger(typeof(LogError));

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var entry = input.GetEx<string>();
            _logger.Error(entry);
        }
    }
}
