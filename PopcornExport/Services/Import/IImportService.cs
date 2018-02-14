using System.Collections.Generic;
using System.Threading.Tasks;
using ShellProgressBar;

namespace PopcornExport.Services.Import
{
    /// <summary>
    /// Import service
    /// </summary>
    public interface IImportService
    {
        /// <summary>
        /// Import documents to database
        /// </summary>
        /// <param name="rawImports">Documents to import</param>
        /// <param name="pbar"><see cref="IProgressBar"/></param>
        /// <returns><see cref="Task"/></returns>
        Task Import(IEnumerable<string> rawImports, IProgressBar pbar);
    }
}
