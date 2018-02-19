using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PopcornExport.Database;
using PopcornExport.Services.Subtitle;

namespace PopcornExport.Services.Language
{
    public class LanguageService : ILanguageService
    {
        private readonly ISubtitleService _subtitleService;

        public LanguageService(ISubtitleService subtitleService)
        {
            _subtitleService = subtitleService;
        }

        public async Task UpdateLanguages()
        {
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                var languages = await _subtitleService.GetSubLanguages();
                foreach (var language in languages)
                {
                    if (!await context.LanguageSet.AnyAsync(a => a.LanguageName == language.LanguageName))
                    {
                        context.LanguageSet.Add(new Database.Language
                        {
                            LanguageName = language.LanguageName,
                            Iso639 = language.ISO639,
                            SubLanguageId = language.SubLanguageID
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Database.Language>> GetLanguages()
        {
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                return await context.LanguageSet.ToListAsync();
            }
        }

        public async Task<bool> IsOpusArchivedDownloadedForLang(string lang)
        {
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                return await context.LanguageSet.AnyAsync(a => a.Iso639 == lang && a.OpusArchiveDownloaded);
            }
        }

        public async Task SetOpusArchivedDownloadedForLang(string lang)
        {
            using (var context = new PopcornContextFactory().CreateDbContext(new string[0]))
            {
                var languageSet = await context.LanguageSet.FirstAsync(a => a.Iso639 == lang);
                languageSet.OpusArchiveDownloaded = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
