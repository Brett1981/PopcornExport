using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopcornExport.Services.Import
{
    public sealed class ImportAnimeService : IImportService
    {
        public async Task Import(IEnumerable<BsonDocument> documents)
        {

        }
    }
}
