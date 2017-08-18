using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PopcornExport.Database
{
    public class PopcornContextFactory : IDesignTimeDbContextFactory<PopcornContext>
    {
        public PopcornContext CreateDbContext(string[] args)
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
