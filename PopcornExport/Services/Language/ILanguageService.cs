using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Services.Language
{
    public interface ILanguageService
    {
        Task UpdateLanguages();

        Task<IEnumerable<Database.Language>> GetLanguages();
    }
}
