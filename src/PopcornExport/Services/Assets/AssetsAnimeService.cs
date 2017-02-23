using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PopcornExport.Helpers;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace PopcornExport.Services.Assets
{
    public class AssetsAnimeService : IAssetsService
    {
        public async Task<string> UploadFile(string fileName, string fileUrl)
        {
            using (var client = new RestClient(Constants.AzurePopcornApi))
            {
                var request = new RestRequest("{segment}", Method.POST);
                request.AddUrlSegment("segment", "animes/assets");
                request.AddQueryParameter("fileName", fileName);
                request.AddQueryParameter("fileUrl", fileUrl);
                var response = await client.Execute<string>(request);
                return response.Data;
            }
        }
    }
}
