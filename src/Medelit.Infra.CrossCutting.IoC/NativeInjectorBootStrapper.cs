using Medelit.Domain.Core.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Medelit.Domain.Events;
using Medelit.Domain.EventHandlers;
using Medelit.Domain.Commands;
using Medelit.Domain.CommandHandlers;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Core.Bus;
using Medelit.Infra.CrossCutting.Bus;
using Medelit.Application;
using Medelit.Infra.Data.UoW;
using Medelit.Infra.Data.Repository;
using Equinox.Infra.Data.Repository;
using Medelit.Infra.Data.Context;

namespace Medelit.Infra.CrossCutting.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // ASP.NET HttpContext dependency
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            services.AddScoped<MedelitContext>();

            RegisterApplicationServices(services);

            RegisterCommands(services);

            RegisterRepos(services);
            RegisterCommandEvents(services);

            // Infra - Data

            services.AddScoped<IUnitOfWork, UnitOfWork>();
 
        }

        private static void RegisterRepos(IServiceCollection services)
        {
            // auth0
            services.AddScoped<ITinUserRepo, UserRepository>();
            //services.AddScoped<ITinPermissionRepo, TinPermissionRepo>();
            //services.AddScoped<ITinUserRoleRepo, TinUserRoleRepo>();
            //services.AddScoped<ITinRolePermissionRepo, TinRolePermissionRepo>();

            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IFieldSubcategoryRepository, FieldSubCategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
            services.AddScoped<IFeeRepository, FeeRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceEntityRepository, InvoiceEntityRepository>();

            services.AddScoped<IAccountingCodeRepository, AccountingCodeRepository>();
            services.AddScoped<IBookingStatusRepository, BookingStatusRepository>();
            services.AddScoped<IBookingTypeRepository, BookingTypeRepository>();
            services.AddScoped<IBuildingTypeRepository, BuildingTypeRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IContactMethodRepository, ContactMethodRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IDiscountNetworkRepository, DiscountNetworkRepository>();
            services.AddScoped<IDurationRepository, DurationRepository>();
            services.AddScoped<IIERatingRepository, IERatingRepository>();
            services.AddScoped<IIETypeRepository, IETypeRepository>();
            services.AddScoped<IInvoiceStatusRepository, InvoiceStatusRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILeadCategoryRepository, LeadCategoryRepository>();
            services.AddScoped<ILeadSourceRepository, LeadSourceRepository>();
            services.AddScoped<ILeadStatusRepository, LeadStatusRepository>();
            services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();
            services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();
            services.AddScoped<IRelationshipRepository, RelationshipRepository>();
            services.AddScoped<ITitleRepository, TitleRepository>();
            services.AddScoped<IVatRepository, VatRepository>();
            services.AddScoped<IVisitVenueRepository, VisitVenueRepository>();

        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IFieldSubcategoryService, FieldSubcategoryService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IProfessionalService, ProfessionalService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IInvoiceEntityService, InvoiceEntityService>();

            services.AddScoped<IStaticDataService, StaticDataService>();
          
            // Upload
            services.AddScoped<IUploadService, UploadService>();

        }

        private static void RegisterCommands(IServiceCollection services)
        {
            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // auth0- commands
            services.AddScoped<IRequestHandler<LoginCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<SignupCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<UserCreateCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateUserCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateUserPasswordCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<GetAllUsersCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<GetUserCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<UserAssignRolesCommand, bool>, AuthCommandHandler>();
            services.AddScoped<IRequestHandler<AccessTokenFromRefreshTokenCommand, bool>, AuthCommandHandler>();

        }

        private static void RegisterCommandEvents(IServiceCollection services)
        {
            //Auth0 - Events
            services.AddScoped<INotificationHandler<UserRegisteredEvent>, Auth0EventHandler>();
        }
    }
}