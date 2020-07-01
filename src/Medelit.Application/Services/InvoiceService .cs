using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using System.Linq;
using Medelit.Domain.Models;
using System.Collections.Generic;
using Medelit.Domain.Commands;
using Microsoft.EntityFrameworkCore;
using Medelit.Infra.CrossCutting.Identity.Data;
using Microsoft.AspNetCore.Hosting;

namespace Medelit.Application
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;

        public InvoiceService(IMapper mapper,
                            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IInvoiceRepository invoiceRepository,
                            IInvoiceEntityRepository ieRepository,
                            ICustomerRepository customerRepository,
                            IStaticDataRepository staticRepository,
                            IHostingEnvironment env
           
            ) : base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
            _bus = bus;
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _staticRepository = staticRepository;
        }

        public dynamic GetInvoices()
        {
            return _invoiceRepository.GetAll().Select(x => new { x.Id, x.InvoiceNumber, x.Subject }).ToList();
        }

        public void FindInvoices(SearchViewModel viewModel)
        {
            _invoiceRepository.FindInvoices(viewModel);
        }


        public void GetInvoiceById(long invoiceId)
        {
            _invoiceRepository.GetInvoiceById(invoiceId, GetUsers());


            //try
            //{
            //    var invoice = _mapper.Map<InvoiceViewModel>(_invoiceRepository.GetById(invoiceId));
            //    invoice.AssignedTo = GetAssignedUser(invoice.AssignedToId);

            //    invoice.InvoiceBookings = (from ib in _invoiceRepository.GetInvoiceBookings()
            //                               join b in _bookingRepository.GetAll() on ib.BookingId equals b.Id

            //                               join c in _customerRepository.GetAll() on ib.Booking.CustomerId equals c.Id
            //                               where ib.InvoiceId == invoiceId
            //                               select new
            //                               {
            //                                   ib.Id,
            //                                   ib.InvoiceId,
            //                                   ib.BookingId,
            //                                   Booking = new
            //                                   {
            //                                       b.Id,
            //                                       b.Name,
            //                                       b.CustomerId,
            //                                       CustomerName = c.Name,
            //                                       Service = b.Service.Name,
            //                                       b.PtFees.FeeName,
            //                                       b.TaxAmount,
            //                                       b.PatientDiscount,
            //                                       subTotal = b.SubTotal,
            //                                       b.GrossTotal
            //                                   }
            //                               }).ToList();
            //    return invoice;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(MessageCodes.API_DATA_INVALID, ex.InnerException);
            //}
        }

        public void SaveInvoice(InvoiceViewModel viewModel)
        {
            var invoiceModel = _mapper.Map<Invoice>(viewModel);
            _bus.SendCommand(new SaveInvoiceCommand { Invoice = invoiceModel });
        }

        public void UpdateStatus(IEnumerable<InvoiceViewModel> invoices, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateInvoicesStatusCommand { Invoices = _mapper.Map<IEnumerable<Invoice>>(invoices), Status = status });
        }

        public void DeleteInvoices(IEnumerable<long> invoiceIds)
        {
            _bus.SendCommand(new DeleteInvoicesCommand { InvoiceIds = invoiceIds });
        }

        public void ProcessInvoiceEmission(long invoiceId)
        {
            _invoiceRepository.ProcessInvoiceEmission(invoiceId);
        }

        public void GetBookingToAddToInvoice(long invoiceId)
        {
            _invoiceRepository.GetBookingToAddToInvoice(invoiceId);
        }

        public void AddBookingsToInvoice(IEnumerable<long> bookingIds, long invoiceId)
        {
            _invoiceRepository.AddBookingsToInvoice(bookingIds, invoiceId);
        }

        public void AddBookingToInvoice(long bookingId, long invoiceId)
        {
            if (invoiceId > 0)
            {
                _bus.SendCommand(new AddBookingToInvoiceCommand { BookingId = bookingId, InvoiceId = invoiceId });
            }
            else
            {
                _bus.SendCommand(new CreateInvoiceFromBookingCommand { BookingId = bookingId });
            }
        }

        public void DeleteInvoiceBooking(long invoiceId, long bookingId)
        {
            _bus.SendCommand(new DeleteInvoiceBookingCommand { InvoiceId = invoiceId, BookingId = bookingId });
        }

        public dynamic GetInvoiceView(long invoiceId)
        {
            return _invoiceRepository.GetInvoiceView(invoiceId);
        }

        public dynamic InvoiceConnectedProfessional(long invoiceId)
        {
            return _invoiceRepository.InvoiceConnectedProfessional(invoiceId);
        }

        public dynamic InvoiceConnectedCustomers(long invoiceId)
        {
            return _invoiceRepository.InvoiceConnectedCustomers(invoiceId);
        }
        public dynamic InvoiceConnectedInvoiceEntity(long invoiceId)
        {
            return _invoiceRepository.InvoiceConnectedInvoiceEntity(invoiceId);
        }
        public dynamic InvoiceConnectedBookings(long invoiceId)
        {
            return _invoiceRepository.InvoiceConnectedBookings(invoiceId);
        }

        public void InvocieBookingsForCrud(long invoiceId)
        {
            _invoiceRepository.InvocieBookingsForCrud(invoiceId);
        }

        public void SaveInvocieBookingsForCrud(IEnumerable<FilterModel> model, long invoiceId)
        {
            _invoiceRepository.SaveInvocieBookingsForCrud(model, invoiceId);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}