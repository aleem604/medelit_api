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
    public class ProfessionalService : IProfessionalService
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public ProfessionalService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IProfessionalRepository professionalRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _professionalRepository = professionalRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
        }
       
        public dynamic GetProfessionals()
        {
            return _professionalRepository.GetAll().Count();
        }

        public dynamic FindProfessionals(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();


            var query = _professionalRepository.GetAll();


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