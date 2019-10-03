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
    /// [log.error] slot for logging error log entries.
    /// </summary>
    [Slot(Name = "log.fatal")]
    public class LogFatal : ISlot
    {
        readonly static ILog _logger = LogManager.GetLogger(typeof(LogFatal));

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var entry = input.GetEx<string>();
            _logger.Fatal(entry);
        }
    }
}
