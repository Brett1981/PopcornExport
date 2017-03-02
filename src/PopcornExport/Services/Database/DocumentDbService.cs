using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using PopcornExport.Helpers;

namespace PopcornExport.Services.Database
{
    /// <summary>
    /// MongoDb service
    /// </summary>
    public sealed class DocumentDbService : IDocumentDbService
    {
        /// <summary>
        /// DocumentDB endpoint
        /// </summary>
        private readonly string _endpointUri;

        /// <summary>
        /// DocumentDB primary key
        /// </summary>
        private readonly string _primaryKey;

        /// <summary>
        /// Establish a connection to database
        /// </summary>
        /// <param name="endpointUri">DocumentDB endpoint</param>
        /// <param name="primaryKey">DocumentDB primary key</param>
        public DocumentDbService(string endpointUri, string primaryKey)
        {
            _endpointUri = endpointUri;
            _primaryKey = primaryKey;
        }

        /// <summary>
        /// Retrieve a <see cref="IDocumentClient"/>
        /// </summary>
        /// <returns><see cref="IDocumentClient"/></returns>
        public DocumentClient Client => new DocumentClient(new Uri(_endpointUri), _primaryKey);

        /// <summary>
        /// Initialize database
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            using (var client = new DocumentClient(new Uri(_endpointUri), _primaryKey))
            {
                await CreateDatabaseIfNotExistsAsync(client, Constants.DatabaseName);
                await CreateCollectionIfNotExistsAsync(client, Constants.DatabaseName, Constants.MoviesCollectionName);
                await CreateCollectionIfNotExistsAsync(client, Constants.DatabaseName, Constants.ShowsCollectionName);
                await CreateCollectionIfNotExistsAsync(client, Constants.DatabaseName, Constants.AnimeCollectionName);
            }
        }

        /// <summary>
        /// Create database if not exists
        /// </summary>
        /// <param name="client">DocumentDb client</param>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        private async Task CreateDatabaseIfNotExistsAsync(IDocumentClient client, string databaseName)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database
                    {
                        Id = databaseName
                    });
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Create collection if not exists
        /// </summary>
        /// <param name="client">DocumentDb client</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="collectionName">Collection name</param>
        /// <returns><see cref="Task"/></returns>
        private async Task CreateCollectionIfNotExistsAsync(IDocumentClient client, string databaseName,
            string collectionName)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection {Id = collectionName},
                        new RequestOptions {OfferThroughput = 1000});
                }
                else
                {
                    throw;
                }
            }
        }
    }
}