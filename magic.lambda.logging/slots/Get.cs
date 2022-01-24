/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.logging.contracts;

namespace magic.lambda.logging.slots
{
    /// <summary>
    /// [log.get] slot for retrieving a single log item given a specified ID.
    /// </summary>
    [Slot(Name = "log.get")]
    public class Get : ISlotAsync, ISlot
    {
        readonly ILogQuery _query;

        /// <summary>
        /// Creates an instance of your type.
        /// </summary>
        /// <param name="query">Actual implementation.</param>
        public Get(ILogQuery query)
        {
            _query = query;
        }

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            SignalAsync(signaler, input).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var id = input.GetEx<object>() ?? throw new HyperlambdaException("No id specified to [log.get]");
            input.Clear();
            input.Value = null;
            var item = await _query.Get(id);
            input.Add(new Node("id", item.Id));
            input.Add(new Node("type", item.Type));
            input.Add(new Node("created", item.Created));
            input.Add(new Node("content", item.Content));
            input.Add(new Node("exception", item.Exception));
        }
    }
}
