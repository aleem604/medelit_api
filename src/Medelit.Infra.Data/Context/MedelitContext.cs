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
        public DbSet<Lead> Lead { get; set; }
        public DbSet<LeadServices> LeadServiceRelation { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerServices> CustomerServiceRelation { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<FieldSubCategory> FieldSubCategory { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Professional> Professional { get; set; }

        public DbSet<ProfessionalLanguages> ProfessionalLanguages { get; set; }
        public DbSet<ProfessionalFields> ProfessionalFields { get; set; }
        public DbSet<ProfessionalSubCategories> ProfessionalSubCategories { get; set; }
        public DbSet<PtFee> PtFee { get; set; }
        public DbSet<ProFee> ProFee { get; set; }
        public DbSet<VFees> VFees { get; set; }
        
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceBookings> InvoiceBookings { get; set; }
        public DbSet<InvoiceEntity> InvoiceEntity { get; set; }

        #region views
        public DbSet<ServiceProfessionalFees> ServiceProfessionalFees { get; set; }

        #endregion views

        #region static data models
        public DbSet<Lab> Lab { get; set; }
        public DbSet<StaticData> StaticData { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Language> Languages { get; set; }

        #endregion static data models

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new CustomerMap());
                        
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
