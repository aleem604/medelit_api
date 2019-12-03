using System.IO;
using Medelit.Infra.Data.Mappings;
using Medelit.Domain.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace Medelit.Infra.Data.Context
{
    public class EventStoreSQLContext : DbContext
    {
        public DbSet<StoredEvent> StoredEvent { get; set; }
        private readonly IHostingEnvironment _env;

        public EventStoreSQLContext(IHostingEnvironment env)
        {
            _env = env;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StoredEventMap());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // get the configuration from the app settings
            var config = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            // define the database to use
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }
}