using NUnit.Framework;
using PopcornExport.Services.Export;
using PopcornExport.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopcornExport.Test.Services.Export
{
    [TestFixture]
    public class ExportTest
    {
        /// <summary>
        /// The export service
        /// </summary>
        private IExportService _exportService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _exportService = new ExportService(new LoggingService());
        }

        [Test]
        public void Export_Anime_ShouldNotBeNull()
        {
            var anime = _exportService.LoadExport(Models.Export.ExportType.Anime).GetAwaiter().GetResult();
            Assert.IsNotNull(anime);
            Assert.IsNotEmpty(anime);
        }

        [Test]
        public void Export_Shows_ShouldNotBeNull()
        {
            var shows = _exportService.LoadExport(Models.Export.ExportType.Shows).GetAwaiter().GetResult();
            Assert.IsNotNull(shows);
            Assert.IsNotEmpty(shows);
        }

        [Test]
        public void Export_Movies_ShouldNotBeNull()
        {
            var movies = _exportService.LoadExport(Models.Export.ExportType.Movies).GetAwaiter().GetResult();
            Assert.IsNotNull(movies);
            Assert.IsNotEmpty(movies);
        }
    }
}
