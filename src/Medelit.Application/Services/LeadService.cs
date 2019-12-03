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
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public LeadService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ILeadRepository leadRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _leadRepository = leadRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
        }
       
        public dynamic GetLeads()
        {
            return _leadRepository.GetAll().Select(x=> new {x.Id, x.TitleId, x.SurName }).ToList();
        }

        public dynamic FindLeads(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var langs = _langRepository.GetAll().ToList();
            var titles = _titleRepository.GetAll().ToList();

            var query = (from lead in _leadRepository.GetAll()
                         join title in _titleRepository.GetAll() on lead.TitleId equals title.Id
                         join lang in _langRepository.GetAll() on lead.LanguageId equals lang.Id
                         select new { lead, title, lang })
                        .Select((x) => new
                        {
                            Id = x.lead.Id,
                            Title = x.title.Value,
                            Language = x.lang.Name,
                            SurName = x.lead.SurName,
                            Name = x.lead.Name,
                            Email = x.lead.Email,
                            MainPhone = x.lead.MainPhone,
                            UpdateDate = x.lead.UpdateDate,
                            Status = x.lead.Status
                        });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.SurName.Equals(viewModel.Filter.Search))
                //|| (!string.IsNullOrEmpty(x.currency) && x.currency.CLower().Contains(viewModel.Filter.Search.CLower()))
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
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "surname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.SurName);
                    else
                        query = query.OrderByDescending(x => x.SurName);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
                    break;
                case "language":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Language);
                    else
                        query = query.OrderByDescending(x => x.Language);
                    break;

                case "status":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
                    break;
                //case "createDate":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.CreateDate);
                //    else
                //        query = query.OrderByDescending(x => x.CreateDate);
                //    break;
                //case "createdBy":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.CreatedBy);
                //    else
                //        query = query.OrderByDescending(x => x.CreatedBy);
                //    break;

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

        public LeadViewModel GetLeadById(long leadId)
        {
            return _mapper.Map<LeadViewModel>(_leadRepository.GetAll().FirstOrDefault(x => x.Id == leadId));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}