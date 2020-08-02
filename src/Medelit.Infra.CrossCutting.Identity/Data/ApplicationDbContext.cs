using Medelit.Common.Models;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Medelit.Infra.CrossCutting.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<MedelitUser, MedelitRole, string>
    {
        private readonly IHostingEnvironment _env;

        public ApplicationDbContext(
                    DbContextOptions<ApplicationDbContext> options, 
                    IHostingEnvironment env) : base(options)
        {
            _env = env;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // any guid
            string ADMIN_ID = Guid.NewGuid().ToString();
            // any guid, but nothing is against to use the same one
            string ROLE_ID = ADMIN_ID;
            builder.Entity<MedelitRole>().HasData(new MedelitRole
            {
                Id = ROLE_ID,
                Name = "admin",
                NormalizedName = "admin"
            });

            var hasher = new PasswordHasher<MedelitUser>();
            builder.Entity<MedelitUser>().HasData(new MedelitUser
            {
                Id = ADMIN_ID,
                UserName = "admin",
                FirstName = "Medelit",
                LastName = "Admin",
                NormalizedUserName = "admin",
                Email = "admin@medelit.com",
                NormalizedEmail = "admin@medelit.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin@Medelit"),
                SecurityStamp = string.Empty
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });


        }


    }
}
