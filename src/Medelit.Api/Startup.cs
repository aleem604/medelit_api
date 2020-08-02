using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Medelit.Api.Configurations;
using Medelit.Infra.CrossCutting.IoC;
using Medelit.Infra.CrossCutting.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Medelit.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Medelit.Auth;
using Medelit.Api.Configurations.Auth;
using Hangfire;
using Hangfire.SqlServer;
using Medelit.Application;
using Wkhtmltopdf.NetCore;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Api
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public static IConfiguration _configuration { get; private set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(_configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        UsePageLocksOnDequeue = true,
                        DisableGlobalLocks = true
                    }));
            services.AddHangfireServer();
            services.AddWkhtmltopdf();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<MedelitUser, MedelitRole>(
                opts =>
                {
                    opts.Password.RequireDigit = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequiredLength = 5;
                })
                .AddRoleManager<RoleManager<MedelitRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc(options =>
            {
                options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                options.UseCentralRoutePrefix(new RouteAttribute("api/v{version}"));
            })
           .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
           .AddJsonOptions(options =>
           {
               options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
               options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
           });


            services.AddAutoMapperSetup();
            JWTConfiguration(services, _configuration);

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Medelit",
                    Description = "Medelit API Swagger surface"
                });
                s.AddSecurityDefinition("Bearer",
        new ApiKeyScheme
        {
            In = "header",
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = "apiKey"
        });
                s.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
        { "Bearer", Enumerable.Empty<string>() },
    });
            });


            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            // .NET Native DI Abstraction
            services.AddSingleton<IJwtFactory, JwtFactory>();
            RegisterServices(services);
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RoleManager<MedelitRole> roleManager, IHangfireJobsService hangfireJobs)
        {
            app.UseCors("CorsPolicy");

            app.UseHangfireDashboard();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Medelit API v1.1");
            });

            app.UseAuthentication(); // this one first
            app.UseMiddleware<TokenMiddleware>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Common}/{action=Get}/{id?}");
            });

            RecurringJob.AddOrUpdate(() => hangfireJobs.SetLeadStatus(), Cron.Daily());
            if (env.IsProduction())
                RecurringJob.AddOrUpdate(() => hangfireJobs.RemoveConvertedLeads(), Cron.MinuteInterval(5));

            var task = RolesExtensions.InitializeAsync(roleManager);
            task.Wait();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            NativeInjectorBootStrapper.RegisterServices(services);
        }

        public void JWTConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("readonlypolicy", builder => builder.RequireRole("Admin", "Manager", "Member"));
                options.AddPolicy("writepolicy", builder => builder.RequireRole("Admin", "Manager"));
            });

        }

    }
}
