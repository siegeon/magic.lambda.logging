/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using log4net;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.json
{
    /// <summary>
    /// [log.info] slot for logging informational pieces of log entries.
    /// </summary>
    [Slot(Name = "log.info")]
    public class LogInfo : ISlot
    {
        readonly static ILog _logger = LogManager.GetLogger(typeof(LogInfo));

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var entry = input.GetEx<string>();
            _logger.Info(entry);
        }
    }
}
