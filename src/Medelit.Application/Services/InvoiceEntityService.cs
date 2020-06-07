using System;
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
using Medelit.Infra.CrossCutting.Identity.Data;
using Microsoft.AspNetCore.Hosting;

namespace Medelit.Application
{
    public class InvoiceEntityService : BaseService, IInvoiceEntityService
    {

        private readonly IInvoiceEntityRepository _invoiceEntityRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;

        public InvoiceEntityService(IMapper mapper,
                            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IInvoiceEntityRepository invoiceEntityRepository,
                            IStaticDataRepository staticRepository,
                            IHostingEnvironment env

            ) : base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
            _bus = bus;
            _invoiceEntityRepository = invoiceEntityRepository;
            _staticRepository = staticRepository;
        }

        public dynamic GetInvoiceEntities()
        {
            return _invoiceEntityRepository.GetAll().Select(x => new { x.Id, x.Name }).ToList();
        }

        public dynamic FindInvoiceEntities(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var ratings = _staticRepository.GetIERatings();
            var ieTypes = _staticRepository.GetIETypes();


            var query = _invoiceEntityRepository.GetAll().Select((s) => new
            {
                s.Id,
                s.Name,
                ieType = s.IETypeId.HasValue ? ieTypes.FirstOrDefault(x => x.Id == s.IETypeId).Value : string.Empty,
                rating = s.RatingId.HasValue ? ratings.FirstOrDefault(x => x.Id == s.RatingId).Value : string.Empty,
                phone = s.MainPhoneNumber,
                s.Email,
                city = s.BillingCity,
                country = s.BillingCountryId.HasValue ? s.BillingCountry.Value : string.Empty,
                s.UpdateDate,
                address = s.BillingAddress,
                s.ContractedId,
                AssignedTo = GetAssignedUser(s.AssignedToId)
            });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.ieType) && x.ieType.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.rating) && x.rating.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.phone) && x.phone.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.city) && x.city.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.country) && x.country.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.address) && x.address.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))
                ));
            }

            if (viewModel.Filter.IEFilter == eIEFilter.Contracted)
            {
                query = query.Where(x => x.ContractedId == (short)eIEFilter.Contracted);
            }

            switch (viewModel.SortField)
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;
                case "ietype":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.ieType);
                    else
                        query = query.OrderByDescending(x => x.ieType);
                    break;
                case "rating":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.rating);
                    else
                        query = query.OrderByDescending(x => x.rating);
                    break;
                case "phone":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.phone);
                    else
                        query = query.OrderByDescending(x => x.phone);
                    break;
                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
                    break;
                case "city":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.city);
                    else
                        query = query.OrderByDescending(x => x.city);
                    break;
                case "country":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.country);
                    else
                        query = query.OrderByDescending(x => x.country);
                    break;
                case "address":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.address);
                    else
                        query = query.OrderByDescending(x => x.address);
                    break;
                case "updatedate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.UpdateDate);
                    else
                        query = query.OrderByDescending(x => x.UpdateDate);
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
            var vm = _mapper.Map<InvoiceEntityViewModel>(_invoiceEntityRepository.GetById(ieId));
            vm.AssignedTo = GetAssignedUser(vm.AssignedToId);
            return vm;
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

        public dynamic InvoiceEntityConnectedServices(long invoiceEntityId)
        {
            return _invoiceEntityRepository.InvoiceEntityConnectedServices(invoiceEntityId);
        }

        public dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId)
        {
            return _invoiceEntityRepository.InvoiceEntityConnectedInvoices(invoiceEntityId);
        }
        public dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId)
        {
            return _invoiceEntityRepository.InvoiceEntityConnectedProfessionals(invoiceEntityId);
        }
        public dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId)
        {
            return _invoiceEntityRepository.InvoiceEntityConnectedCustomers(invoiceEntityId);
        }
        public dynamic InvoiceEntityConnectedBookings(long invoiceEntityId)
        {

            return _invoiceEntityRepository.InvoiceEntityConnectedBookings(invoiceEntityId);
        }

        public dynamic InvoiceEntityConnectedLeads(long invoiceEntityId)
        {
            return _invoiceEntityRepository.InvoiceEntityConnectedLeads(invoiceEntityId);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}