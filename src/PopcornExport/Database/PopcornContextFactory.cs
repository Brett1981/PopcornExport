using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace PopcornExport.Database
{
    public class PopcornContextFactory : IDbContextFactory<PopcornContext>
    {
        public PopcornContext Create(DbContextFactoryOptions options)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<PopcornContext>();
            optionsBuilder.UseSqlServer(configuration["SQL:ConnectionString"]);

            return new PopcornContext(optionsBuilder.Options);
        }
    }
}
