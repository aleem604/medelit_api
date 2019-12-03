using Medelit.Domain.Models;
using Medelit.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Medelit.Infra.Data.Context
{
    public class MedelitContext : DbContext
    {
        private readonly IHostingEnvironment _env;

        public MedelitContext(IHostingEnvironment env)
        {
            _env = env;
        }

        public DbSet<TinUser> User { get; set; }
        public DbSet<Lead> Lead { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<FieldSubCategory> FieldSubCategory { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Professional> Professional { get; set; }
        public DbSet<Fee> Fee { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceEntity> InvoiceEntity { get; set; }

        #region static data models
        public DbSet<AccountingCode> AccountingCode { get; set; }
        public DbSet<BookingStatus> GetBookingStatus { get; set; }
        public DbSet<BookingType> BookingType { get; set; }
        public DbSet<BuildingType> BuildingType { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<ContactMethod> ContactMethods { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<DiscountNetwork> DiscountNetworks { get; set; }
        public DbSet<Duration> Durations { get; set; }
        public DbSet<IERating> IERatings { get; set; }
        public DbSet<IEType> IETypes { get; set; }
        public DbSet<InvoiceStatus> InvoiceStatus { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LeadCategory> LeadCategories { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<LeadStatus> LeadStatus { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<PaymentStatus> PaymentStatus { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Vat> Vats { get; set; }
        public DbSet<VisitVenue> VisitVenues { get; set; }

        #endregion static data models

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMap());
                        
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
