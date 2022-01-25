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
    /// [log.list] slot for listing log items sequentially according to most recent items first.
    /// </summary>
    [Slot(Name = "log.list")]
    public class List : ISlotAsync, ISlot
    {
        readonly ILogQuery _query;

        /// <summary>
        /// Creates an instance of your type.
        /// </summary>
        /// <param name="query">Actual implementation.</param>
        public List(ILogQuery query)
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
                if (!string.IsNullOrEmpty(idx.Exception))
                    tmp.Add(new Node("exception", idx.Exception));

                // Retrieving meta.
                if (idx.Meta != null && idx.Meta.Count() > 0)
                {
                    var metaNode = new Node("meta");
                    metaNode.AddRange(idx.Meta.Select(x => new Node(x.Key, x.Value)));
                    tmp.Add(metaNode);
                }

                // Returning node to caller.
                input.Add(tmp);
            }
        }
    }
}
