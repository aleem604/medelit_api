using Medelit.Domain.Core.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Medelit.Domain.Commands;
using Medelit.Domain.CommandHandlers;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Core.Bus;
using Medelit.Infra.CrossCutting.Bus;
using Medelit.Application;
using Medelit.Infra.Data.UoW;
using Medelit.Infra.Data.Repository;
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
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IFieldSubcategoryRepository, FieldSubCategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
            services.AddScoped<IFeeRepository, FeeRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceEntityRepository, InvoiceEntityRepository>();

            services.AddScoped<IStaticDataRepository, StaticDataRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();

        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IHangfireJobsService, HangfireJobsService>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IBookingService, BookingService>();
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


            //lead commmands
            services.AddScoped<IRequestHandler<SaveLeadCommand, bool>, LeadCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateLeadsStatusCommand, bool>, LeadCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteLeadsCommand, bool>, LeadCommandHandler>();
            services.AddScoped<IRequestHandler<ConvertLeadToBookingCommand, bool>, LeadCommandHandler>();
            services.AddScoped<IRequestHandler<LeadsBulkUploadCommand, bool>, LeadCommandHandler>();

            //customer commmands
            services.AddScoped<IRequestHandler<SaveCustomerCommand, bool>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCustomersStatusCommand, bool>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteCustomersCommand, bool>, CustomerCommandHandler>();
            services.AddScoped<IRequestHandler<ConvertCustomerToBookingCommand, bool>, CustomerCommandHandler>();

            //booking commmands
            services.AddScoped<IRequestHandler<SaveBookingCommand, bool>, BookingCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateBookingsStatusCommand, bool>, BookingCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteBookingsCommand, bool>, BookingCommandHandler>();
            services.AddScoped<IRequestHandler<BookingFromCustomerCommand, bool>, BookingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCloneCommand, bool>, BookingCommandHandler>();
            services.AddScoped<IRequestHandler<CreateCycleCommand, bool>, BookingCommandHandler>();

            //invoice
            services.AddScoped<IRequestHandler<SaveInvoiceCommand, bool>, InvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateInvoicesStatusCommand, bool>, InvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteInvoicesCommand, bool>, InvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<AddBookingToInvoiceCommand, bool>, InvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<CreateInvoiceFromBookingCommand, bool>, InvoiceCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteInvoiceBookingCommand, bool>, InvoiceCommandHandler>();

            //invoice entity commmands
            services.AddScoped<IRequestHandler<SaveInvoiceEntityCommand, bool>, InvoiceEntityCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateInvoiceEntitiesStatusCommand, bool>, InvoiceEntityCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteInvoiceEntitiesCommand, bool>, InvoiceEntityCommandHandler>();


            //fee commmands
            services.AddScoped<IRequestHandler<SaveFeeCommand, bool>, FeeCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateFeesStatusCommand, bool>, FeeCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteFeesCommand, bool>, FeeCommandHandler>();

            //professional commmands
            services.AddScoped<IRequestHandler<SaveProfessionalCommand, bool>, ProfessionalCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateProfessionalsStatusCommand, bool>, ProfessionalCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteProfessionalsCommand, bool>, ProfessionalCommandHandler>();

            //service commmands
            services.AddScoped<IRequestHandler<SaveServiceCommand, bool>, ServiceCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateServicesStatusCommand, bool>, ServiceCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteServicesCommand, bool>, ServiceCommandHandler>();
            services.AddScoped<IRequestHandler<AddProfessionalToServicesCommand, bool>, ServiceCommandHandler>();
            services.AddScoped<IRequestHandler<DetachProfessionalCommand, bool>, ServiceCommandHandler>();


        }

        private static void RegisterCommandEvents(IServiceCollection services)
        {
        }
    }
}