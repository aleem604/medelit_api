using System;
using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class LeadRepository : Repository<Lead>, ILeadRepository
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IStaticDataRepository _static;

        public LeadRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, ApplicationDbContext appContext, IStaticDataRepository @static)
            : base(context, contextAccessor, bus)
        {
            _appContext = appContext;
            _static = @static;
        }


        public void SearchLeads(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
                if (viewModel.SearchOnly && string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    var res = new
                    {
                        items = new List<dynamic>(),
                        totalCount = 0
                    };
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, res));
                    return;
                }

                var langs = Db.Languages.ToList();
                var titles = _static.GetTitles().ToList();
                var relations = _static.GetRelationships().ToList();
                var leadSource = _static.GetLeadSources().ToList();
                var leadCategory = _static.GetLeadCategories().ToList();
                var contactMethod = _static.GetContactMethods().ToList();
                var paymentMethods = _static.GetPaymentMethods().ToList();
                var discountNetworks = _static.GetDiscountNewtorks().ToList();
                var buildingTypes = _static.GetBuildingTypes().ToList();
                var visitVenues = _static.GetVisitVenues().ToList();
                var leadStatus = _static.GetLeadStatuses().ToList();

                var invoiceEntities = Db.InvoiceEntity.ToList();
   


                var query = (from lead in Db.Lead
                             select lead)
                            .Select((x) => new
                            {
                                x.Id,
                                x.SurName,
                                x.TitleId,
                                Title = (titles.FirstOrDefault(f => f.Id == x.TitleId) ?? new FilterModel()).Value,
                                x.Name,
                                x.MainPhone,
                                x.MainPhoneOwner,
                                x.InvoiceEntityId,
                                InvoiceEntity = (invoiceEntities.FirstOrDefault(f => f.Id == x.InvoiceEntityId) ?? new InvoiceEntity()).Name,
                                x.Phone2,
                                x.Phone2Owner,
                                x.Phone3,
                                x.Phone3Owner,
                                x.ContactPhone,
                                x.VisitRequestingPerson,
                                x.VisitRequestingPersonRelationId,
                                VisitRequestingPersonRelation = (relations.FirstOrDefault(f => f.Id == x.VisitRequestingPersonRelationId) ?? new FilterModel()).Value,
                                x.Fax,
                                x.Email,
                                x.Email2,
                                x.LeadSourceId,
                                LeadSource = (leadSource.FirstOrDefault(f => f.Id == x.LeadSourceId) ?? new FilterModel()).Value,
                                x.LeadStatusId,
                                LeadStatus = (leadStatus.FirstOrDefault(f => f.Id == x.LeadStatusId) ?? new FilterModel()).Value,
                                Language = (langs.FirstOrDefault(s => s.Id == x.LanguageId) ?? new Language()).Name,
                                x.LeadCategoryId,
                                LeadCategory = (leadCategory.FirstOrDefault(f => f.Id == x.LeadCategoryId) ?? new FilterModel()).Value,
                                x.ContactMethodId,
                                ContactMethod = (contactMethod.FirstOrDefault(f => f.Id == x.ContactMethodId) ?? new FilterModel()).Value,
                                x.DateOfBirth,
                                x.CountryOfBirthId,
                                CountryOfBirth = x.CountryOfBirthId.HasValue ? x.CountryOfBirth.Value : string.Empty,
                                x.PreferredPaymentMethodId,
                                PreferredPaymentMethod = (paymentMethods.FirstOrDefault(f => f.Id == x.PreferredPaymentMethodId) ?? new FilterModel()).Value,
                                x.InvoicingNotes,
                                x.InsuranceCoverId,
                                InsuranceCover = x.InsuranceCoverId.HasValue && x.InsuranceCoverId.Value == 1 ? "Yes" : "No",
                                x.ListedDiscountNetworkId,
                                ListedDiscountNetwork = (discountNetworks.FirstOrDefault(f => f.Id == x.ListedDiscountNetworkId) ?? new FilterModel()).Value,
                                x.Discount,
                                x.GPCode,
                                x.AddressStreetName,
                                x.PostalCode,
                                x.City,
                                Country = x.CountryId > 0 ? x.Country.Value : string.Empty,
                                x.BuildingTypeId,
                                BuildingType = x.BuildingTypeId.HasValue ? (buildingTypes.FirstOrDefault(f => f.Id == x.BuildingTypeId) ?? new FilterModel()).Value : string.Empty,
                                x.FlatNumber,
                                x.Buzzer,
                                x.Floor,
                                x.VisitVenueId,
                                VisitVenue = x.VisitVenueId.HasValue ? (visitVenues.FirstOrDefault(f => f.Id == x.VisitVenueId) ?? new FilterModel()).Value : string.Empty,
                                x.AddressNotes,
                                x.VisitVenueDetail,
                                x.LeadDescription,
                                x.BlacklistId,
                                BlackList = x.BlacklistId.HasValue && x.BlacklistId.Value == 1 ? "Yes" : "No",
                                x.BankName,
                                x.AccountNumber,
                                x.SortCode,
                                x.IBAN,
                                x.CustomerId,
                                Customer = x.CustomerId.HasValue ? x.Customer.Name : string.Empty,
                                x.CreateDate,
                                x.UpdateDate,
                                AssignedTo = GetAssignedUser(x.AssignedToId),
                                Services = string.Join(", ", x.Services.Select(s => s.Service.Name)),
                                Professionals = string.Join(", ", x.Services.Select(s => s.Professional.Name)),

                            });

                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                    (!string.IsNullOrEmpty(x.SurName) && x.SurName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Title) && x.Title.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhone) && x.MainPhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhoneOwner) && x.MainPhoneOwner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoiceEntity) && x.InvoiceEntity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2) && x.Phone2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2Owner) && x.Phone2Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3) && x.Phone3.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3Owner) && x.Phone3Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ContactPhone) && x.ContactPhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPerson) && x.VisitRequestingPerson.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPersonRelation) && x.VisitRequestingPersonRelation.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Fax) && x.Fax.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email2) && x.Email2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LeadSource) && x.LeadSource.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LeadStatus) && x.LeadStatus.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Language) && x.Language.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LeadCategory) && x.LeadCategory.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ContactMethod) && x.ContactMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PreferredPaymentMethod) && x.PreferredPaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InsuranceCover) && x.InsuranceCover.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ListedDiscountNetwork) && x.ListedDiscountNetwork.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Discount.HasValue && x.Discount.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.GPCode) && x.GPCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddressStreetName) && x.AddressStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PostalCode) && x.PostalCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.City) && x.City.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Country) && x.Country.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BuildingType) && x.BuildingType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.FlatNumber.HasValue && x.FlatNumber.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Buzzer) && x.Buzzer.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Floor.HasValue && x.Floor.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenue) && x.VisitVenue.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddressNotes) && x.AddressNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenueDetail) && x.VisitVenueDetail.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LeadDescription) && x.LeadDescription.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BlackList) && x.BlackList.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BankName) && x.BankName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountNumber) && x.AccountNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.SortCode) && x.SortCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.IBAN) && x.IBAN.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CreateDate.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (x.Id.ToString().Contains(viewModel.Filter.Search))
                    || (!string.IsNullOrEmpty(x.Services) && x.Services.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Professionals) && x.Professionals.CLower().Contains(viewModel.Filter.Search.CLower()))

                    ));
                }

                if (viewModel.Filter.Filter == eLeadsFilter.HotLeads)
                {
                    query = query.Where(x => x.LeadStatusId == (int)eLeadsStatus.Hot);
                }

                 if (viewModel.Filter.Filter == eLeadsFilter.ReturningLeads)
                {
                    query = query.Where(x => x.CustomerId.HasValue);
                }

                switch (viewModel.SortField.ToLower())
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
                    case "city":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.City);
                        else
                            query = query.OrderByDescending(x => x.City);
                        break;
                    case "country":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Country);
                        else
                            query = query.OrderByDescending(x => x.Country);
                        break;
                    case "mainphone":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.MainPhone);
                        else
                            query = query.OrderByDescending(x => x.MainPhone);
                        break;

                    case "language":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Language);
                        else
                            query = query.OrderByDescending(x => x.Language);
                        break;

                    case "createdate":
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
                var result = new
                {
                    items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                    totalCount
                };

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void RemoveAll(List<long> leadIds)
        {
            var leads = Db.Lead.Where(l => leadIds.Contains(l.Id)).ToList();
            Db.RemoveRange(leads);
            Db.SaveChanges();
        }

        public IQueryable<Lead> GetAllWithService()
        {
            return Db.Lead.Include(x => x.Services);
        }
        public Lead GetWithInclude(long leadId)
        {
            return Db.Lead.Include(x => x.Services).FirstOrDefault(x => x.Id == leadId);
        }

        public void RemoveLeadServices(long leadId)
        {
            var services = Db.LeadServiceRelation.Where(x => x.LeadId == leadId).ToList();
            Db.LeadServiceRelation.RemoveRange(services);
            Db.SaveChanges();
        }

        public Customer GetCustomerId(long? fromCustomerId)
        {
            var customer = Db.Customer.FirstOrDefault(x => x.Id == fromCustomerId);
            return customer;
        }

        public IQueryable<LeadServices> GetLeadServiceRelations()
        {
            return Db.LeadServiceRelation;
        }

        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _appContext.Users.Find(assignedToId).FullName;
        }

    }
}
