/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using System.Text;
using System.Collections.Generic;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.logging.helpers
{
    internal static class Utilities
    {
        public static (string Content, Dictionary<string, string> Meta) GetLogContent(Node node, ISignaler signaler)
        {
            if (node.Value != null)
                return (node.GetEx<string>(), node.Children.Where(x => x.Name != "exception").ToDictionary(x => x.Name, x => x.GetEx<string>()));

            signaler.Signal("eval", node);
            var builder = new StringBuilder();
            foreach (var idx in node.Children)
            {
                builder.Append(idx.GetEx<string>());
            }
            return (builder.ToString(), null);
        }
    }
}
