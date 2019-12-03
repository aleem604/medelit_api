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

namespace Medelit.Application
{
    public class FieldSubcategoryService : IFieldSubcategoryService
    {
        private readonly IFieldSubcategoryRepository _fieldRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public FieldSubcategoryService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IFieldSubcategoryRepository fieldRepository)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _fieldRepository = fieldRepository;
        }
       
        public dynamic FindFields(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();

            var query = _fieldRepository.GetAll();


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                (!string.IsNullOrEmpty(x.Code) && x.Code.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Code.Equals(viewModel.Filter.Search))
                || (!string.IsNullOrEmpty(x.Field) && x.Field.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.SubCategory) && x.SubCategory.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));

            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

            switch (viewModel.SortField)
            {
                case "code":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Code);
                    else
                        query = query.OrderByDescending(x => x.Code);
                    break;

                case "subcategory":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.SubCategory);
                    else
                        query = query.OrderByDescending(x => x.SubCategory);
                    break;

                case "field":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Field);
                    else
                        query = query.OrderByDescending(x => x.Field);
                    break;
               
                case "status":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
                    break;
                case "createdate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreateDate);
                    else
                        query = query.OrderByDescending(x => x.CreateDate);
                    break;
                case "createdby":
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

        public dynamic GetFields()
        {
            return _fieldRepository.GetAll().ToList();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}