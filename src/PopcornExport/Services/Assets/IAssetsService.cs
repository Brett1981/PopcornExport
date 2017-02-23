using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Assets
{
    public interface IAssetsService
    {
        Task<string> UploadFile(string fileName, string fileUrl);
    }
}
