using System;
using Medelit.Common;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Application
{
    public class DashboardService : IDashboardService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFieldSubcategoryRepository _fieldRepository;
        public DashboardService(
            ILeadRepository leadRepository,
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,
            IInvoiceRepository invoiceRepository,
            IProfessionalRepository professionalRepository,
            IServiceRepository serviceRepository,
            IFieldSubcategoryRepository fieldRepository
            )
        {
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _invoiceRepository = invoiceRepository;
            _professionalRepository = professionalRepository;
            _serviceRepository = serviceRepository;
            _fieldRepository = fieldRepository;

        }

        public IDictionary<string, int> GetDashboardStats()
        {
            var dict = new Dictionary<string, int>();
            var leads = _leadRepository.GetAll().Where(x => !x.ConvertDate.HasValue).Count();
            var customers = _customerRepository.GetAll().Count();
            var bookings = _bookingRepository.GetAll().Count();
            var invoices = _invoiceRepository.GetAll().Count();
            var services = _serviceRepository.GetAll().Count();
            var professionals = _professionalRepository.GetAll().Count();

            dict.Add(nameof(leads), leads);
            dict.Add(nameof(customers), customers);
            dict.Add(nameof(bookings), bookings);
            dict.Add(nameof(invoices), invoices);
            dict.Add(nameof(services), services);
            dict.Add(nameof(professionals), professionals);

            return dict;
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}