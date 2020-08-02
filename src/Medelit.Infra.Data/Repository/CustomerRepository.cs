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
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IStaticDataRepository _static;

        public CustomerRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, IStaticDataRepository @static, ApplicationDbContext appContext)
            : base(context, contextAccessor, bus)
        {
            _static = @static;
            _appContext = appContext;
        }


        public void FindCustomer(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
                if (viewModel.SearchOnly && string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    var res =  new
                    {
                        items = new List<dynamic>(),
                        totalCount = 0
                    };
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, res));
                    return;
                }

                var langs = Db.Languages.ToList();
                var titles = _static.GetTitles();
                var relations = _static.GetRelationships();
                var leadSource = _static.GetLeadSources();
                var leadCategory = _static.GetLeadCategories();
                var contactMethod = _static.GetContactMethods();
                var paymentMethods = _static.GetPaymentMethods();
                var discountNetworks = _static.GetDiscountNewtorks();
                var buildingTypes = _static.GetBuildingTypes();
                var visitVenues = _static.GetVisitVenues();

                var invoiceEntities = Db.InvoiceEntity.ToList();

                var query = (from lead in Db.Customer
                             select lead)
                            .Select((x) => new
                            {
                                x.Id,
                                x.TitleId,
                                Title = (titles.FirstOrDefault(f => f.Id == x.TitleId) ?? new FilterModel()).Value,
                                x.Name,
                                x.SurName,
                                x.InvoiceEntityId,
                                InvoiceEntity = (invoiceEntities.FirstOrDefault(f => f.Id == x.InvoiceEntityId) ?? new InvoiceEntity()).Name,
                                x.BlacklistId,
                                BlackList = x.BlacklistId.HasValue && x.BlacklistId.Value == 1 ? "Yes" : "No",
                                x.Phone2,
                                x.MainPhone,
                                x.Phone2Owner,
                                x.MainPhoneOwner,
                                x.Email,
                                Age = x.DateOfBirth.HasValue ?  CalculateYourAge(x.DateOfBirth.Value) : string.Empty,
                                x.Phone3,
                                x.Email2,
                                x.Phone3Owner,
                                x.Fax,
                                x.LanguageId,
                                Language = (langs.FirstOrDefault(s => s.Id == x.LanguageId) ?? new Language()).Name,
                                x.LeadSourceId,
                                LeadSource = (leadSource.FirstOrDefault(f => f.Id == x.LeadSourceId) ?? new FilterModel()).Value,
                                x.DateOfBirth,
                                x.CountryOfBirthId,
                                CountryOfBirth = x.CountryOfBirthId.HasValue ? x.CountryOfBirth.Value : string.Empty,
                                x.VisitRequestingPersonRelationId,
                                VisitRequestingPersonRelation = (relations.FirstOrDefault(f => f.Id == x.VisitRequestingPersonRelationId) ?? new FilterModel()).Value,
                                x.VisitRequestingPerson,
                                x.ContactPhone,
                                x.HomeStreetName,
                                x.VisitStreetName,
                                x.HomeCity,
                                x.VisitCity,
                                x.HomePostCode,
                                x.VisitPostCode,
                                x.HomeCountryId,
                                HomeCountry = x.HomeCountryId.HasValue ? x.HomeCountry.Value : string.Empty,
                                x.VisitCountryId,
                                VisitCountry = x.VisitCountryId.HasValue ? x.VisitCountry.Value : string.Empty,
                                x.VisitVenueDetail,
                                x.InsuranceCoverId,
                                InsuranceCover = x.InsuranceCoverId.HasValue && x.InsuranceCoverId.Value == 1 ? "Yes" : "No",
                                x.ListedDiscountNetworkId,
                                ListedDiscountNetwork = (discountNetworks.FirstOrDefault(f => f.Id == x.ListedDiscountNetworkId) ?? new FilterModel()).Value,
                                x.Discount,
                                x.GPCode,
                                x.Buzzer,
                                x.FlatNumber,
                                x.Floor,
                                x.BuildingTypeId,
                                BuildingType = (buildingTypes.FirstOrDefault(f => f.Id == x.BuildingTypeId) ?? new FilterModel()).Value,
                                x.VisitVenueId,
                                VisitVenue = (visitVenues.FirstOrDefault(f => f.Id == x.VisitVenueId) ?? new FilterModel()).Value,
                                x.ContactMethodId,
                                ContactMethod = (contactMethod.FirstOrDefault(f => f.Id == x.ContactMethodId) ?? new FilterModel()).Value,
                                x.AddressNotes,
                                x.PaymentMethodId,
                                PaymentMethod = (paymentMethods.FirstOrDefault(f => f.Id == x.PaymentMethodId) ?? new FilterModel()).Value,
                                x.InvoicingNotes,
                                x.LeadId,

                                x.BankName,
                                x.AccountNumber,
                                x.SortCode,
                                x.IBAN,
                                x.CreateDate,
                                x.UpdateDate,
                                AssignedTo = GetAssignedUser(x.AssignedToId),
                                Services = string.Join(", ", x.Services.Select(s => s.Service.Name)),
                            }) ;

                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                    (!string.IsNullOrEmpty(x.SurName) && x.SurName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Title) && x.Title.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoiceEntity) && x.InvoiceEntity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BlackList) && x.BlackList.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2) && x.Phone2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhone) && x.MainPhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2Owner) && x.Phone2Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhoneOwner) && x.MainPhoneOwner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3) && x.Phone3.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email2) && x.Email2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3Owner) && x.Phone3Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Fax) && x.Fax.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Language) && x.Language.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.LeadSource) && x.LeadSource.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Age) && x.Age.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.CountryOfBirth) && x.CountryOfBirth.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPersonRelation) && x.VisitRequestingPersonRelation.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPerson) && x.VisitRequestingPerson.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ContactPhone) && x.ContactPhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeStreetName) && x.HomeStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitStreetName) && x.VisitStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeCity) && x.HomeCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitCity) && x.VisitCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomePostCode) && x.HomePostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitPostCode) && x.VisitPostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeCountry) && x.HomeCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitCountry) && x.VisitCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenueDetail) && x.VisitVenueDetail.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BankName) && x.BankName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountNumber) && x.AccountNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.SortCode) && x.SortCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.IBAN) && x.IBAN.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InsuranceCover) && x.InsuranceCover.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ListedDiscountNetwork) && x.ListedDiscountNetwork.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Discount.HasValue && x.Discount.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.GPCode) && x.GPCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Buzzer) && x.Buzzer.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.FlatNumber.HasValue && x.FlatNumber.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Floor.HasValue && x.Floor.ToString().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BuildingType) && x.BuildingType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenue) && x.VisitVenue.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ContactMethod) && x.ContactMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddressNotes) && x.AddressNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PaymentMethod) && x.PaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BlackList) && x.BlackList.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CreateDate.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (x.Id.ToString().Contains(viewModel.Filter.Search))
                    || (!string.IsNullOrEmpty(x.Services) && x.Services.CLower().Contains(viewModel.Filter.Search.CLower()))

                    ));
                }

                switch (viewModel.SortField)
                {
                    case "name":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;

                    case "surName":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.SurName);
                        else
                            query = query.OrderByDescending(x => x.SurName);
                        break;
                    case "invoiceEntity":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.InvoiceEntity);
                        else
                            query = query.OrderByDescending(x => x.InvoiceEntity);
                        break;
                    case "mainPhone":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.MainPhone);
                        else
                            query = query.OrderByDescending(x => x.MainPhone);
                        break;

                    case "email":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Email);
                        else
                            query = query.OrderByDescending(x => x.Email);
                        break;

                    case "age":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Age);
                        else
                            query = query.OrderByDescending(x => x.Age);
                        break;
                    case "city":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.HomeCity);
                        else
                            query = query.OrderByDescending(x => x.HomeCity);
                        break;
                    case "country":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.HomeCountry);
                        else
                            query = query.OrderByDescending(x => x.HomeCountry);
                        break;
                    case "updateDate":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.UpdateDate);
                        else
                            query = query.OrderByDescending(x => x.UpdateDate);
                        break;
                    case "assignedTo":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.AssignedTo);
                        else
                            query = query.OrderByDescending(x => x.AssignedTo);
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

        public string CalculateYourAge(DateTime Dob)
        {
            DateTime Now = DateTime.Now;
            int _Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime _DOBDateNow = Dob.AddYears(_Years);
            int _Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (_DOBDateNow.AddMonths(i) == Now)
                {
                    _Months = i;
                    break;
                }
                else if (_DOBDateNow.AddMonths(i) >= Now)
                {
                    _Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(_DOBDateNow.AddMonths(_Months)).Days;

            return $"{_Years} Years and {_Months} Months";
        }


        public IQueryable<Customer> GetAllWithService()
        {
            return Db.Customer.Include(x => x.Services);
        }
        public Customer GetByIdWithInclude(long customerId)
        {
            return Db.Customer.Include(x => x.Services).FirstOrDefault(x => x.Id == customerId);
        }

        public void RemoveCustomerServices(long id)
        {
            var services = Db.CustomerServiceRelation.Where(x => x.CustomerId == id).ToList();
            Db.RemoveRange(services);
            Db.SaveChanges();
        }

        public void SaveCustomerRelation(List<CustomerServices> newServices)
        {
            Db.CustomerServiceRelation.AddRange(newServices);
            Db.SaveChanges();
        }



        public dynamic GetCustomerConnectedCustomers(long customerId)
        {
            var titles = Db.StaticData.Select((s) => new { s.Id, Value = s.Titles }).Where(x => x.Value != null).ToList();
            return (from l in Db.Lead
                    where l.CustomerId == customerId
                    select new
                    {
                        Title = $"{l.Customer.SurName} {titles.FirstOrDefault(x => x.Id == l.Customer.TitleId).Value}",
                        l.Customer.Name,
                        l.Customer.Email,
                        phone = l.Customer.MainPhone
                    }).ToList();
        }

        public dynamic GetCustomerConnectedServices(long customerId)
        {

            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    select new
                    {
                        b.ServiceId,
                        serviceName = b.Service.Name,
                        PtFeeName = b.PtFees.FeeName,
                        PtFee = b.PtFee,
                        ProFeeName = b.ProFees.FeeName,
                        ProFee = b.ProFee,
                        Professional = b.Professional.Name,
                        Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.PtFees.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                                    <span class='font-500'>Pro. Fee Name :</span> {b.ProFees.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).DistinctBy(d =>d.ServiceId).ToList();

        }

        public dynamic GetCustomerConnectedProfessionals(long customerId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();
            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    select new
                    {
                        b.ProfessionalId,
                        proName = b.Professional.Name,
                        phone = b.Professional.Telephone,
                        email = b.Professional.Email,
                        b.VisitStartDate,
                        b.Professional.ActiveCollaborationId,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).DistinctBy(x => x.ProfessionalId).ToList();

        }

        public dynamic GetCustomerConnectedBookings(long customerId)
        {
            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    orderby b.SrNo ascending
                    select new
                    {
                        bookingName = $"{b.Name} {b.SrNo}",
                        serviceName = b.Service.Name,
                        PtFee = b.PtFees.FeeName,
                        PtFeeA1 = b.PtFees.A1,
                        PtFeeA2 = b.PtFees.A2,
                        ProFee = b.ProFees.FeeName,
                        ProFeeA1 = b.ProFees.A1,
                        ProFeeA2 = b.ProFees.A2,
                        professional = b.Professional.Name,
                        visitDate = b.VisitStartDate,

                        Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.PtFees.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                                    <span class='font-500'>Pro. Fee Name :</span> {b.ProFees.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).ToList();
        }

        public dynamic GetCustomerConnectedInvoices(long customerId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.Booking.CustomerId == customerId
                    select new
                    {
                        ib.InvoiceId,
                        ib.Invoice.Subject,
                        ib.Invoice.InvoiceNumber,
                        ieName = ib.Invoice.InvoiceEntityId.HasValue ? ib.Invoice.InvoiceEntity.Name : string.Empty,
                        ib.Invoice.InvoiceDate,
                        ib.Invoice.TotalInvoice
                    }).DistinctBy(d => d.InvoiceId).ToList();
        }

        public dynamic GetCustomerConnectedLeads(long customerId)
        {
            return (from ls in Db.LeadServiceRelation
                    where ls.Lead.CustomerId == customerId
                    select new
                    {
                        LeadName = $"{ls.Lead.SurName} {ls.Lead.Name}",
                        ls.Lead.CreateDate,
                        ls.Lead.UpdateDate,
                        Professional = string.Join(" <br/>", ls.Service.ServiceProfessionalFees.Select(x => x.Professional.Name).ToArray()),
                        ls.Lead.LeadStatusId
                    }).ToList();
        }
        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _appContext.Users.Find(assignedToId).FullName;
        }

    }
}
