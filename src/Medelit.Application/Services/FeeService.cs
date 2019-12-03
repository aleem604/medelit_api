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
    public class FeeService : IFeeService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public FeeService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IFeeRepository feeRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _feeRepository = feeRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
        }
       
        public dynamic GetFees()
        {
            return _feeRepository.GetAll().Select(x=> new {x.Id, x.FeeCode }).ToList();
        }

        public dynamic FindFees(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var langs = _langRepository.GetAll().ToList();
            var titles = _titleRepository.GetAll().ToList();

            var query = _feeRepository.GetAll();


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.FeeName) && x.FeeName.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.FeeName.Equals(viewModel.Filter.Search))
                //|| (!string.IsNullOrEmpty(x.currency) && x.currency.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.FeeCode) && x.FeeCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));

            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

            switch (viewModel.SortField)
            {
                case "feename":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeName);
                    else
                        query = query.OrderByDescending(x => x.FeeName);
                    break;

                case "feecode":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeCode);
                    else
                        query = query.OrderByDescending(x => x.FeeCode);
                    break;

                case "connectedservices":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.ConnectedServices);
                    else
                        query = query.OrderByDescending(x => x.ConnectedServices);
                    break;
                case "a1":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.A1);
                    else
                        query = query.OrderByDescending(x => x.A1);
                    break;
                case "a2":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.A2);
                    else
                        query = query.OrderByDescending(x => x.A2);
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