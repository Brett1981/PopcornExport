using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PopcornExport.Helpers;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace PopcornExport.Services.Assets
{
    public class AssetsMovieService : IAssetsService
    {
        public async Task<string> UploadFile(string fileName, string fileUrl)
        {
            try
            {
                Uri result;
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out result))
                {
                    using (var client = new RestClient(Constants.AzurePopcornApi))
                    {
                        var request = new RestRequest("{segment}", Method.POST);
                        request.AddUrlSegment("segment", "movies/assets");
                        request.AddQueryParameter("fileName", fileName);
                        request.AddQueryParameter("fileUrl", fileUrl);
                        var response = await client.Execute<string>(request);
                        return response.Data;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
