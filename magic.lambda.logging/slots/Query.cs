/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.logging.contracts;

namespace magic.lambda.logging.slots
{
    /// <summary>
    /// [log.query] slot for querying log items.
    /// </summary>
    [Slot(Name = "log.query")]
    public class Query : ISlotAsync, ISlot
    {
        readonly ILogQuery _query;

        /// <summary>
        /// Creates an instance of your type.
        /// </summary>
        /// <param name="query">Actual implementation.</param>
        public Query(ILogQuery query)
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
            var max = input.Children.FirstOrDefault(x => x.Name == "max")?.GetEx<int>() ?? 10;
            var from = input.Children.FirstOrDefault(x => x.Name == "from")?.GetEx<object>();
            input.Clear();
            foreach (var idx in await _query.QueryAsync(max, from))
            {
                var tmp = new Node(".");
                tmp.Add(new Node("id", idx.Id));
                tmp.Add(new Node("type", idx.Type));
                tmp.Add(new Node("created", idx.Created));
                tmp.Add(new Node("content", idx.Content));
                tmp.Add(new Node("exception", idx.Exception));
                input.Add(tmp);
            }
        }
    }
}
