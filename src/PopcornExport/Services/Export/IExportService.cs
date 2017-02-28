using MongoDB.Bson;
using PopcornExport.Models.Export;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <returns>Bson documents</returns>
        Task<IEnumerable<BsonDocument>> LoadExport(ExportType exportType);
    }
}