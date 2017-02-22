using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopcornExport.Services.Import
{
    public sealed class ImportMoviesService : IImportService
    {
        public async Task Import(IEnumerable<BsonDocument> documents)
        {

        }
    }
}
