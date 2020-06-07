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

        public dynamic FindInvoices(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var customers = _customerRepository.GetAll().Select((s) => new { s.Id, s.Name }).ToList();
            var invoiceStatus = _staticRepository.GetStaticData().Select(x => new FilterModel { Id = x.Id, Value = x.InvoiceStatus }).Where(x => x.Value != null).ToList();

            var query = _invoiceRepository.GetAll().Where(x => x.Status != eRecordStatus.Deleted).Select((x) => new
            {
                x.Id,
                x.InvoiceNumber,
                x.Subject,
                x.InvoiceDate,
                x.DueDate,
                Customer = customers.FirstOrDefault(c => c.Id == x.CustomerId).Name,
                InvoiceEntity = x.InvoiceEntityId.HasValue ? x.InvoiceEntity.Name : string.Empty,
                Status = x.IsProforma ? "Proforma" : "Emitted",
                x.StatusId,
                InvoiceStatus = string.Empty, // phase II item
                PaymentMethod = x.PaymentMethodId.HasValue ? ((ePaymentMethods)x.PaymentMethodId.Value).ToString() : string.Empty,
                x.PaymentMethodId,
                x.DateOfVisit,
                
                Amount = x.TotalInvoice,
                x.UpdateDate,
                x.CreateDate,
                assignedTo = GetAssignedUser(x.AssignedToId)
                
            }) ;

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.InvoiceNumber) && x.InvoiceNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.InvoiceNumber.Equals(viewModel.Filter.Search))
                || (!string.IsNullOrEmpty(x.Subject) && x.Subject.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.InvoiceDate.HasValue && x.InvoiceDate.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.DueDate.HasValue && x.DueDate.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Customer) && x.Customer.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.InvoiceEntity) && x.InvoiceEntity.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Status) && x.Status.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.StatusId.HasValue && x.StatusId.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.PaymentMethod) && x.PaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.InvoiceStatus) && x.InvoiceStatus.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.DateOfVisit.HasValue && x.DateOfVisit.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Amount.HasValue && x.Amount.Value.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.assignedTo) && x.assignedTo.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))
                ));
            }

            if (viewModel.Filter.InvoiceFilter != eInvoiceFilter.ToBeSent)
            {
                query = query.Where(x => x.StatusId != (short?)eInvoiceStatus.Sent);
            }
            else if (viewModel.Filter.InvoiceFilter != eInvoiceFilter.InsurancePending)
            {
                query = query.Where(x => x.StatusId != (short?)eInvoiceStatus.Pending && x.PaymentMethodId == (short)ePaymentMethods.Insurance);
            }
            //else if (viewModel.Filter.InvoiceFilter != eInvoiceFilter.Refunded)
            //{
            //    query = query.Where(x => x.);
            //}

            switch (viewModel.SortField)
            {
                case "subject":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Subject);
                    else
                        query = query.OrderByDescending(x => x.Subject);
                    break;

                case "invoicenumber":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.InvoiceNumber);
                    else
                        query = query.OrderByDescending(x => x.InvoiceNumber);
                    break;

                case "customer":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Customer);
                    else
                        query = query.OrderByDescending(x => x.Customer);
                    break;

                case "createDate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreateDate);
                    else
                        query = query.OrderByDescending(x => x.CreateDate);
                    break;

                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Id);
                    else
                        query = query.OrderByDescending(x => x.Id);
                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
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