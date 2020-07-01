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
    public class InvoiceEntityRepository : Repository<InvoiceEntity>, IInvoiceEntityRepository
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IStaticDataRepository _static;

        public InvoiceEntityRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, ApplicationDbContext appContext, IStaticDataRepository @static)
            : base(context, contextAccessor, bus)
        {
            _appContext = appContext;
            _static = @static;
        }


        public void FindInvoiceEntities(SearchViewModel viewModel)
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

                var relations = _static.GetRelationships();
                var contactMethod = _static.GetContactMethods();
                var paymentMethods = _static.GetPaymentMethods();
                var discountNetworks = _static.GetDiscountNewtorks();
                var ieTypes = _static.GetIETypes();
                var ratings = _static.GetIERatings();
                var contracts = _static.GetContractStatus();

                var query = Db.InvoiceEntity.Select((s) => new
                {
                    s.Id,
                    s.IENumber,
                    s.Name,
                    s.MainPhoneNumber,
                    phone = s.MainPhoneNumber,
                    s.MainPhoneNumberOwner,
                    s.Phone2,
                    s.Phone2Owner,
                    s.Phone3,
                    s.Phone3Owner,
                    s.Email,
                    s.Email2,
                    s.RatingId,
                    rating = s.RatingId.HasValue ? ratings.FirstOrDefault(x => x.Id == s.RatingId).Value : string.Empty,
                    s.RelationshipWithCustomerId,
                    relation = s.RelationshipWithCustomerId.HasValue ? (relations.FirstOrDefault(x => x.Id == s.RelationshipWithCustomerId) ?? new FilterModel()).Value : string.Empty,
                    s.IETypeId,
                    ieType = s.IETypeId.HasValue ? (ieTypes.FirstOrDefault(x => x.Id == s.IETypeId) ?? new FilterModel()).Value : string.Empty,
                    s.Fax,
                    s.DateOfBirth,
                    s.CountryOfBirthId,
                    CountryOfBirth = s.CountryOfBirthId.HasValue ? s.CountryOfBirth.Value : string.Empty,
                    s.BillingAddress,
                    s.MailingAddress,
                    s.BillingPostCode,
                    s.MailingPostCode,
                    s.BillingCity,
                    city = s.BillingCity,
                    country = s.BillingCountryId.HasValue ? s.BillingCountry.Value : string.Empty,
                    s.BillingCountryId,
                    BillingCountry = s.BillingCountryId.HasValue ? s.BillingCountry.Value : string.Empty,

                    s.MailingCity,
                    s.MailingCountryId,
                    MailingCountry = s.MailingCountryId.HasValue ? s.MailingCountry.Value : string.Empty,
                    s.Description,
                    s.VatNumber,
                    s.PaymentMethodId,
                    PaymentMethod = s.PaymentMethodId.HasValue ? (paymentMethods.FirstOrDefault(x => x.Id == s.PaymentMethodId) ?? new FilterModel()).Value : string.Empty,
                    s.Bank,
                    s.AccountNumber,
                    s.SortCode,
                    s.IBAN,
                    s.InsuranceCoverId,
                    InsuranceCover = s.InsuranceCoverId.HasValue && s.InsuranceCoverId.Value == 1 ? "Yes" : "No",
                    s.InvoicingNotes,
                    s.DiscountNetworkId,
                    DiscountNetwork = s.DiscountNetworkId.HasValue ? (discountNetworks.FirstOrDefault(x => x.Id == s.DiscountNetworkId) ?? new FilterModel()).Value : string.Empty,
                    s.PersonOfReference,
                    s.PersonOfReferenceEmail,
                    s.PersonOfReferencePhone,
                    s.BlackListId,
                    BlackList = s.BlackListId.HasValue && s.BlackListId.Value == 1 ? "Yes" : "No",
                    s.ContractedId,
                    Contracted = s.ContractedId.HasValue ? (contracts.FirstOrDefault(x => x.Id == s.ContractedId) ?? new FilterModel()).Value : string.Empty,
                    s.CreateDate,
                    s.UpdateDate,
                    address = s.BillingAddress,
                    AssignedTo = GetAssignedUser(s.AssignedToId)
                });


                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.IENumber) && x.IENumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhoneNumber) && x.MainPhoneNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MainPhoneNumberOwner) && x.MainPhoneNumberOwner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.phone) && x.phone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2) && x.Phone2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2Owner) && x.Phone2Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3) && x.Phone3.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone3Owner) && x.Phone3Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email2) && x.Email2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.rating) && x.rating.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.relation) && x.relation.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ieType) && x.ieType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Fax) && x.Fax.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CountryOfBirth) && x.CountryOfBirth.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BillingAddress) && x.BillingAddress.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MailingAddress) && x.MailingAddress.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BillingPostCode) && x.BillingPostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MailingPostCode) && x.MailingPostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BillingCity) && x.BillingCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MailingCity) && x.MailingCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BillingCountry) && x.BillingCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MailingCountry) && x.MailingCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Description) && x.Description.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VatNumber) && x.VatNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PaymentMethod) && x.PaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Bank) && x.Bank.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountNumber) && x.AccountNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.SortCode) && x.SortCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.IBAN) && x.IBAN.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InsuranceCover) && x.InsuranceCover.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.DiscountNetwork) && x.DiscountNetwork.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PersonOfReference) && x.PersonOfReference.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PersonOfReferenceEmail) && x.PersonOfReferenceEmail.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PersonOfReferencePhone) && x.PersonOfReferencePhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BlackList) && x.BlackList.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Contracted) && x.Contracted.CLower().Contains(viewModel.Filter.Search.CLower()))

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
                    case "ieType":
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


        public dynamic InvoiceEntityConnectedServices(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        Id = b.ServiceId,
                        serviceName = b.Service.Name,
                        b.PtFee,
                        b.ProFee,
                        Professional = b.Professional.Name,
                        Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.PtFees.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                                    <span class='font-500'>Pro. Fee Name :</span> {b.ProFees.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).DistinctBy(x => x.Id).ToList();
        }
        public dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        b.Id,
                        title = $"{b.Customer.SurName} {b.Customer.Name}",
                        phone = b.Customer.MainPhone,
                        b.Customer.Email,
                        //services = b.Customer.Services.Select(x => new { x.Service.Name, x.PtFeeId, x.PTFeeA1, x.PTFeeA2, x.PROFeeId, x.PROFeeA1, x.PROFeeA2 }).ToList(),
                        services = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.PtFees.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                                    <span class='font-500'>Pro. Fee Name :</span> {b.ProFees.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}",
                        visitDate = b.VisitStartDate,
                        professional = b.Professional.Name
                    }
                ).DistinctBy(d => d.Id).ToList();
        }

        public dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        b.Id,
                        b.ProfessionalId,
                        professional = b.Professional.Name,
                        phoneNumber = b.Professional.Telephone,
                        b.Professional.Email,
                        lastVisitDate = b.VisitStartDate,
                        b.Professional.ActiveCollaborationId,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).DistinctBy(d => d.Id).ToList();
        }

        public dynamic InvoiceEntityConnectedBookings(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        bookingName = b.Name,
                        services = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.PtFees.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                                    <span class='font-500'>Pro. Fee Name :</span> {b.ProFees.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}",
                        professional = b.Professional.Name,
                        visitDate = b.VisitStartDate,

                    }).ToList();

        }
        public dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId)
        {
            return (from i in Db.Invoice
                    where i.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        subject = i.Subject,
                        invoiceNumber = i.InvoiceNumber,
                        ieName = i.InvoiceEntityId.HasValue ? i.InvoiceEntity.Name : string.Empty,
                        invoiceDate = i.InvoiceDate,
                        totalInvoice = i.TotalInvoice
                    }).ToList();
        }

        public dynamic InvoiceEntityConnectedLeads(long invoiceEntityId)
        {
            var statuses = Db.StaticData.Select((s) => new { s.Id, Value = s.LeadStatus }).Where(x => x.Value != null).ToList();
            var serviceProfessions = Db.ServiceProfessionalFees.Include(s => s.Service).Include(s => s.Professional).ToList();

            return (from l in Db.Lead
                    where l.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        l.Id,
                        l.InvoiceEntityId,
                        serviceRequested = string.Join("<br/>", l.Services.Select(s =>s.Service.Name).ToArray()),
                        created = l.CreateDate,
                        lastChanged = l.UpdateDate,
                        professional = string.Join("<br/>", l.Services.Select(x => x.Professional.Name).ToArray()),
                        l.LeadStatusId,
                        Status = l.LeadStatusId.HasValue ? statuses.FirstOrDefault(x => x.Id == l.LeadStatusId).Value : string.Empty
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
