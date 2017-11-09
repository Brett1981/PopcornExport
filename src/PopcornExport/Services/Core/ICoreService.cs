using System.Threading.Tasks;

namespace PopcornExport.Services.Core
{
    /// <summary>
    /// Export service
    /// </summary>
    public interface ICoreService
    {
        /// <summary>
        /// Process the exportation
        /// </summary>
        /// <param name="consolidate">Consolidate existing assets</param>
        /// <returns><see cref="Task"/></returns>
        Task Export();
    }
}
