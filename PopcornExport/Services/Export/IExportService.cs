using PopcornExport.Models.Export;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShellProgressBar;

namespace PopcornExport.Services.Export
{
    /// <summary>
    /// Export service contract
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Load an export
        /// </summary>
        /// <param name="exportType">Export to load</param>
        /// <param name="pbar"><see cref="IProgressBar"/></param>
        /// <returns>Bson documents</returns>
        Task<IEnumerable<string>> LoadExport(ExportType exportType, IProgressBar pbar);
    }
}