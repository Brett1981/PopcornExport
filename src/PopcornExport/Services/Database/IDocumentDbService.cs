using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace PopcornExport.Services.Database
{
    /// <summary>
    /// Database service
    /// </summary>
    public interface IDocumentDbService
    {
        /// <summary>
        /// Initialize database
        /// </summary>
        /// <returns></returns>
        Task Init();

        /// <summary>
        /// Retrieve a <see cref="DocumentClient"/>
        /// </summary>
        /// <returns><see cref="DocumentClient"/></returns>
        DocumentClient Client { get; }
    }
}
