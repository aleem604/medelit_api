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

namespace Medelit.Application
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public InvoiceService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IInvoiceRepository invoiceRepository,
                            ICustomerRepository customerRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository,
                            IStaticDataRepository staticRepository

            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
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
            var invoiceStatus = _staticRepository.GetInvoiceStatuses().ToList();


            var query = _invoiceRepository.GetAll().Select((x) => new
            {
                x.Id,
                x.Subject,
                x.InvoiceNumber,
                Amount = x.TotalInvoice,
                x.InvoiceDate,
                x.DueDate,
                x.CustomerId,
                Customer = customers.FirstOrDefault(c => c.Id == x.CustomerId).Name,
                x.StatusId,
                InvoiceStatus = invoiceStatus.FirstOrDefault(i => i.Id == x.StatusId).Value,
                x.AssignedToId,
                AssignedTo = "Admin",
                x.Status,
                x.CreateDate,
                x.UpdateDate,
                
            });



            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.InvoiceNumber) && x.InvoiceNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.InvoiceNumber.Equals(viewModel.Filter.Search))
                || (!string.IsNullOrEmpty(x.Subject) && x.Subject.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.CreateDate.ToString("yyyy-MM-dd").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))
                ));
            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

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

                case "customerid":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CustomerId);
                    else
                        query = query.OrderByDescending(x => x.CustomerId);
                    break;


                case "status":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
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


        public InvoiceViewModel GetInvoiceById(long invoiceId)
        {
            return _mapper.Map<InvoiceViewModel>(_invoiceRepository.GetWithInclude(invoiceId));
        }

        public void SaveInvoice(InvoiceViewModel viewModel)
        {
            var invoiceModel = _mapper.Map<Invoice>(viewModel);
            invoiceModel.Services = _mapper.Map<ICollection<InvoiceServiceRelation>>(viewModel.Services);
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


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}