/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.lambda.logging.contracts
{
    /// <summary>
    /// Interface to query log entries.
    /// </summary>
    public interface ILogQuery
    {
        /// <summary>
        /// Returns all log items matching specified arguments.
        /// </summary>
        /// <param name="max">Maximum number of items to return.</param>
        /// <param name="fromId">Offset of item where to start fetching items.</param>
        /// <param name="query">Optional filtering query to filter records to return.</param>
        /// <returns>Log items matching specified filter conditions.</returns>
        Task<IEnumerable<LogItem>> QueryAsync(int max, object fromId, string query = null);

        /// <summary>
        /// Returns the log item with the specified ID.
        /// </summary>
        /// <param name="id">ID of item to return</param>
        /// <returns>Item with specified ID.</returns>
        Task<LogItem> Get(object id);

        /// <summary>
        /// Returns total number of log items.
        /// </summary>
        /// <param name="query">Optional filtering parameter to restrict what records to count.</param>
        /// <returns>Number of items matching criteria.</returns>
        Task<long> CountAsync(string query = null);
    }
}
