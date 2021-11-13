/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Text;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.logging.helpers
{
    internal static class Utilities
    {
        public static string GetLogContent(Node node, ISignaler signaler)
        {
            if (node.Value != null)
                return node.GetEx<string>();
            signaler.Signal("eval", node);
            var builder = new StringBuilder();
            foreach (var idx in node.Children)
            {
                builder.Append(idx.GetEx<string>());
            }
            return builder.ToString();
        }
    }
}
