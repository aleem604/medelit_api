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
using Medelit.Infra.CrossCutting.Identity.Data;

namespace Medelit.Application
{
    public class HangfireJobsService : BaseService, IHangfireJobsService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public HangfireJobsService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ILeadRepository leadRepository

            ) : base(context, httpContext, configuration)
        {
            _mapper = mapper;
            _bus = bus;
            _leadRepository = leadRepository;

        }

        public void SetLeadStatus()
        {
            try
            {
                var leads = _leadRepository.GetAll().ToList();
                foreach (var lead in leads)
                {
                    var days = (DateTime.Now - lead.CreateDate).Days;
                    if (days > 6)
                    {
                        lead.LeadStatusId = (short?)eLeadsStatus.Cold;
                    }
                    else if (days > 1)
                    {
                        lead.LeadStatusId = (short?)eLeadsStatus.Warm;
                    }
                    else
                    {
                        lead.LeadStatusId = (short?)eLeadsStatus.Hot;
                    }
                    _leadRepository.Update(lead);
                }
                _leadRepository.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public void RemoveConvertedLeads()
        {
            try
            {
                var leadIds = _leadRepository.GetAll().Where(x => x.ConvertDate.HasValue).Select(x => x.Id).ToList();
                if (leadIds.Count() > 0)
                    _leadRepository.RemoveAll(leadIds);
            }
            catch (Exception)
            {

            }
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}