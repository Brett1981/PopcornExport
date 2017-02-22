using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="documents">Documents to import</param>
        /// <returns><see cref="Task"/></returns>
        Task Import(IEnumerable<BsonDocument> documents);
    }
}
