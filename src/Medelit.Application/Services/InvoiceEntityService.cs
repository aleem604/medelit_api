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
    public class InvoiceEntityService : IInvoiceEntityService
    {
        private readonly IInvoiceEntityRepository _invoiceEntityRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public InvoiceEntityService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IInvoiceEntityRepository invoiceEntityRepository,
                            ILanguageRepository langRepository,
                            IStaticDataRepository staticRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _invoiceEntityRepository = invoiceEntityRepository;
            _langRepository = langRepository;
            _staticRepository = staticRepository;
        }
       
        public dynamic GetInvoiceEntities()
        {
            return _invoiceEntityRepository.GetAll().Select(x=> new {x.Id, x.Name }).ToList();
        }

        public dynamic FindInvoiceEntities(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var ratings = _staticRepository.GetIERatings();
            var ieTypes = _staticRepository.GetIETypes();


            var query = _invoiceEntityRepository.GetAll().Select((s)=> new {
                s.Id,
                s.Name,
                s.Email,
                rating = ratings.FirstOrDefault(x=>x.Id == s.RatingId).Value,
                ieType = ieTypes.FirstOrDefault(x=>x.Id == s.IETypeId).Value,
                s.BillingAddress,
                s.VatNumber,
                s.Bank,
                s.Status,
                s.CreateDate, 
                s.CreatedById,
                s.UpdateDate,
                s.UpdatedById
            });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));

            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

            switch (viewModel.SortField)
            {
                case "invoicingentityname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
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

        public InvoiceEntityViewModel GetInvoiceEntityById(long ieId)
        {
            return _mapper.Map<InvoiceEntityViewModel>(_invoiceEntityRepository.GetById(ieId));
        }

        public void SaveInvoiceEntity(InvoiceEntityViewModel viewModel)
        {
            var ieModel = _mapper.Map<InvoiceEntity>(viewModel);
            _bus.SendCommand(new SaveInvoiceEntityCommand { Entity = ieModel });
        }

        public void UpdateStatus(IEnumerable<InvoiceEntityViewModel> entityIds, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateInvoiceEntitiesStatusCommand { Entities = _mapper.Map<IEnumerable<InvoiceEntity>>(entityIds), Status = status });
        }

        public void DeleteInvoiceEntities(IEnumerable<long> ids)
        {
            _bus.SendCommand(new DeleteInvoiceEntitiesCommand { InvoieEntityIds = ids });
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}