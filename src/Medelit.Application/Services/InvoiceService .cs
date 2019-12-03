using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Medelit.Domain.Models;

namespace Medelit.Application
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public InvoiceService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IInvoiceRepository invoiceRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _invoiceRepository = invoiceRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
        }
       
        public dynamic GetInvoices()
        {
            return _invoiceRepository.GetAll().Select(x=> new {x.Id, x.InvoiceNumber, x.Subject }).ToList();
        }

        public dynamic FindInvoices(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();

            var query = _invoiceRepository.GetAll();


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
                case "createdBy":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreatedById);
                    else
                        query = query.OrderByDescending(x => x.CreatedById);
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


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}