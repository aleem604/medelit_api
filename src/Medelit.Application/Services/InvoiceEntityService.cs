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

        public void FindInvoiceEntities(SearchViewModel viewModel)
        {
            _invoiceEntityRepository.FindInvoiceEntities(viewModel);
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