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
        /// <returns><see cref="Task"/></returns>
        Task Process();
    }
}
