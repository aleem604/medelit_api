using Medelit.Common;
using Medelit.Common.Models;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Infra.Data.Repository
{
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IStaticDataRepository _static;

        public ProfessionalRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, ApplicationDbContext appContext, IStaticDataRepository @static)
            : base(context, contextAccessor, bus)
        {
            _appContext = appContext;
            _static = @static;
        }

        public void FindProfessionals(SearchViewModel viewModel)
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

                var titles = _static.GetTitles();
                var accountingCodes = _static.GetAccountingCodes();
                var langs = Db.Languages.ToList();
                var services = Db.Service.ToList();
                var collaborations = _static.GetCollaborationCodes();
                var applicationMeans = _static.GetApplicationMeans();
                var applicationMethods = _static.GetApplicationMethods();
                var contractStatus = _static.GetContractStatus();
                var documnetList = _static.GetDocumentListSents();
                var proTaxCodes = _static.GetProTaxCodes();

                var query = Db.Professional.Select((s) => new
                {
                    s.Id,
                    s.Code,
                    s.TitleId,
                    Title = (titles.FirstOrDefault(f => f.Id == s.TitleId) ?? new FilterModel()).Value,
                    s.Name,
                    s.Email,
                    s.Telephone,
                    s.AccountingCodeId,
                    AccountingCode = (accountingCodes.FirstOrDefault(f => f.Id == s.AccountingCodeId) ?? new FilterModel()).Value,
                    s.Website,
                    s.MobilePhone,
                    s.HomePhone,
                    s.Email2,
                    s.Fax,
                    s.CoverMap,
                    s.StreetName,
                    s.City,
                    s.PostCode,
                    s.CountryId,
                    Country = s.CountryId > 0 ? s.Country.Value : string.Empty,
                    s.Description,
                    s.ClinicStreetName,
                    s.ClinicPostCode,
                    s.ClinicCity,
                    s.ClinicPhoneNumber,
                    s.DateOfBirth,
                    s.CompanyName,
                    s.CompanyNumber,
                    s.InvoicingNotes,
                    s.Bank,
                    s.Branch,
                    s.AccountName,
                    s.AccountNumber,
                    s.SortCode,
                    s.ContractDate,
                    s.ContractEndDate,
                    s.WorkPlace,
                    s.ColleagueReferring,
                    s.InsuranceExpiryDate,
                    s.ActiveCollaborationId,
                    ActiveCollaboration = (collaborations.FirstOrDefault(f => f.Id == s.ActiveCollaborationId) ?? new FilterModel()).Value,
                    s.ClinicAgreement,
                    s.ApplicationMeansId,
                    ApplicationMeans = (applicationMeans.FirstOrDefault(f => f.Id == s.ApplicationMeansId) ?? new FilterModel()).Value,
                    s.ApplicationMethodId,
                    ApplicationMethod = (applicationMethods.FirstOrDefault(f => f.Id == s.ApplicationMethodId) ?? new FilterModel()).Value,
                    s.FirstContactDate,
                    s.LastContactDate,
                    s.ContractStatusId,
                    ContractStatus = (contractStatus.FirstOrDefault(f => f.Id == s.ContractStatusId) ?? new FilterModel()).Value,
                    s.DocumentListSentId,
                    DocumentListSent = (documnetList.FirstOrDefault(f => f.Id == s.DocumentListSentId) ?? new FilterModel()).Value,
                    CalendarActivation = s.CalendarActivation == 1 ? "Yes" : "No",
                    s.ProOnlineCV,
                    s.ProtaxCodeId,
                    ProTaxCode = (proTaxCodes.FirstOrDefault(f => f.Id == s.ProtaxCodeId) ?? new FilterModel()).Value,

                    Field = string.Join("<br/>", s.ProfessionalFields.Select(x => x.Field.Field).Distinct().ToList()),
                    SubCategory = string.Join("<br/>", s.ProfessionalSubCategories.Select(x => x.SubCategory.SubCategory).Distinct().ToList()),
                    Services = string.Join("<br/>", s.ServiceProfessionalFees.Select(x => x.Service.Name).Distinct().ToList()),
                    assignedTo = GetAssignedUser(s.AssignedToId)
                });

                if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.CurrentProfessionals)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active);
                }
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.ClosedProfessionals)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Closed);
                }
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.ClosedProfessionals)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Closed);
                }
                // 4- Doctors
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Doctors)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("MEDICAL", StringComparison.CurrentCultureIgnoreCase));
                }

                // 5- PHYSIOTHERAPISTS 
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Physiotherapists)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PHYSIOTHERAPY", StringComparison.CurrentCultureIgnoreCase));
                }

                //6- NURSES (FIELD “M” = NURSING” + CONTRACT STATUS “AW” = ACTIVE)
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Nurses)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("NURSING", StringComparison.CurrentCultureIgnoreCase));
                }
                //7- SPEECH & LANGUAGE(FIELD “M” = SPEECH & LANGUAGE + CONTRACT STATUS “AW” = ACTIVE)
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.SpeechAndLanguage)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("SPEECH & LANGUAGE", StringComparison.CurrentCultureIgnoreCase));
                }
                //8- PSYCHOLOGISTS(FIELD “M” = PSYCHOLOGY + CONTRACT STATUS “AW” = ACTIVE)
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Psychologists)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PSYCHOLOGY", StringComparison.CurrentCultureIgnoreCase));
                }
                //9- PODIATRISTS (FIELD “M” = PODIATRY + CONTRACT STATUS “AW” = ACTIVE)
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Podiatrists)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PODIATRY", StringComparison.CurrentCultureIgnoreCase));
                }
                //10- ALTERNATIVE MEDICINE (FIELD “M” = ALTERNATIVE MEDICINE + CONTRACT STATUS “AW” = ACTIVE)
                else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Alternativemedicine)
                {
                    query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("ALTERNATIVE MEDICINE", StringComparison.CurrentCultureIgnoreCase));
                }

                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Code) && x.Code.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Title) && x.Title.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Telephone) && x.Telephone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountingCode) && x.AccountingCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Website) && x.Website.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MobilePhone) && x.MobilePhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomePhone) && x.HomePhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email2) && x.Email2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Fax) && x.Fax.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CoverMap) && x.CoverMap.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.StreetName) && x.StreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.City) && x.City.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PostCode) && x.PostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Country) && x.Country.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Description) && x.Description.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ClinicStreetName) && x.ClinicStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ClinicPostCode) && x.ClinicPostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ClinicCity) && x.ClinicCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ClinicPhoneNumber) && x.ClinicPhoneNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOfBirth.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CompanyName) && x.CompanyName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CompanyNumber) && x.CompanyNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Bank) && x.Bank.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Branch) && x.Branch.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountName) && x.AccountName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AccountNumber) && x.AccountNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.SortCode) && x.SortCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.ContractDate.HasValue) && x.ContractDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower()))
                    || (x.ContractEndDate.HasValue) && x.ContractEndDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower())
                    || (!string.IsNullOrEmpty(x.WorkPlace) && x.WorkPlace.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ColleagueReferring) && x.ColleagueReferring.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.InsuranceExpiryDate.HasValue) && x.InsuranceExpiryDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower())
                    || (!string.IsNullOrEmpty(x.ActiveCollaboration) && x.ActiveCollaboration.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ApplicationMethod) && x.ApplicationMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ApplicationMeans) && x.ApplicationMeans.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.FirstContactDate.HasValue) && x.FirstContactDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower())
                    || (x.LastContactDate.HasValue) && x.LastContactDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower())
                    || (!string.IsNullOrEmpty(x.ContractStatus) && x.ContractStatus.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.DocumentListSent) && x.DocumentListSent.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CalendarActivation) && x.CalendarActivation.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProOnlineCV) && x.ProOnlineCV.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProTaxCode) && x.ProTaxCode.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.Field) && x.Field.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.SubCategory) && x.SubCategory.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Services) && x.Services.CLower().Contains(viewModel.Filter.Search.CLower()))
                    );
                }

                switch (viewModel.SortField.ToLower())
                {
                    case "name":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;

                    case "email":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Email);
                        else
                            query = query.OrderByDescending(x => x.Email);
                        break;

                    case "fields":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Field);
                        else
                            query = query.OrderByDescending(x => x.Field);
                        break;
                    case "subcategories":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.SubCategory);
                        else
                            query = query.OrderByDescending(x => x.SubCategory);
                        break;
                    case "services":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Services);
                        else
                            query = query.OrderByDescending(x => x.Services);
                        break;
                    case "city":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.City);
                        else
                            query = query.OrderByDescending(x => x.City);
                        break;
                    case "contractDate":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.ContractDate);
                        else
                            query = query.OrderByDescending(x => x.ContractDate);
                        break;
                    case "contractEndDate":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.ContractEndDate);
                        else
                            query = query.OrderByDescending(x => x.ContractEndDate);
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


        public IEnumerable<ServiceProfessionalFees> GetProfessionalServices(long id)
        {
            return Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == id)
                    .Include(i => i.Service)
                    .Include(i => i.Professional)
                    .AsNoTracking().ToList();
        }

        public IEnumerable<ServiceProfessionalFees> GetProfessionalProFees(long id)
        {
            return Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == id)
                    //.Include(x => x.Service).ThenInclude(i => i.PtFee)
                    //.Include(x => x.Service).ThenInclude(i => i.ProFee)
                    .AsNoTracking().ToList();
        }

        public IQueryable<ProfessionalLanguages> GetAllLangs()
        {
            return Db.ProfessionalLanguages;
        }

        public IQueryable<Professional> GetByIdWithIncludes(long professionalId)
        {
            return Db.Professional.Include(x => x.ProfessionalLanguages)
                .Include(x => x.ProfessionalFields)
                .Include(x => x.ProfessionalSubCategories)
                .Where(x => x.Id == professionalId).AsNoTracking();
        }

        public void DeleteProfessionalRelations(long id)
        {
            var langs = Db.ProfessionalLanguages.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            var fields = Db.ProfessionalFields.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            var subCats = Db.ProfessionalSubCategories.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            Db.ProfessionalLanguages.RemoveRange(langs);
            Db.ProfessionalFields.RemoveRange(fields);
            Db.ProfessionalSubCategories.RemoveRange(subCats);
            Db.SaveChanges();
        }

        public dynamic GetConnectedCustomers(long proId)
        {
            var proBookings = Db.Booking.Where(x => x.ProfessionalId == proId);
            var query = from b in Db.Booking
                        join
                        s in Db.Service on b.ServiceId equals s.Id
                        join
                        p in Db.Professional on b.ProfessionalId equals p.Id
                        join
                        c in Db.Customer on b.CustomerId equals c.Id
                        where p.Id == proId
                        select new
                        {
                            CustomerId = c.Id,
                            Customer = c.Name,
                            PhoneNumber = c.MainPhone,
                            c.Email,
                            LastVisitDate = b.VisitStartDate,
                            Service = new
                            {
                                serviceName = s.Name,
                                //PtFee = s.PtFee.FeeName,
                                //PtFeeA1 = s.PtFee.A1,
                                //PtFeeA2 = s.PtFee.A2,
                                //ProFee = s.ProFee.FeeName,
                                //ProFeeA1 = s.ProFee.A1,
                                //ProFeeA2 = s.ProFee.A2
                            }
                        };
            return query.ToList();
        }

        public dynamic GetConnectedBookings(long proId)
        {
            var invoiceEntities = Db.InvoiceEntity.ToList();

            var query = from b in Db.Booking
                        join
                        s in Db.Service on b.ServiceId equals s.Id
                        join
                        p in Db.Professional on b.ProfessionalId equals p.Id
                        join
                        c in Db.Customer on b.CustomerId equals c.Id
                        where p.Id == proId
                        select new
                        {
                            BookingId = b.Id,
                            BookingName = b.Name,
                            CustomerName = c.Name,
                            InvoiceEntity = b.InvoiceEntityId.HasValue ? invoiceEntities.FirstOrDefault(x => x.Id == b.InvoiceEntityId).Name : string.Empty,
                            visitDate = b.VisitStartDate,
                            Service = new
                            {
                                serviceName = s.Name,
                                //PtFee = s.PtFee.FeeName,
                                //PtFeeA1 = s.PtFee.A1,
                                //PtFeeA2 = s.PtFee.A2,
                                //ProFee = s.ProFee.FeeName,
                                //ProFeeA1 = s.ProFee.A1,
                                //ProFeeA2 = s.ProFee.A2
                            }
                        };
            return query.ToList();
        }

        public dynamic GetConnectedInvoices(long proId)
        {

            var invoiceEntities = Db.InvoiceEntity.ToList();

            var query = from i in Db.Invoice
                        join
                        ib in Db.InvoiceBookings on i.Id equals ib.InvoiceId
                        join
                        p in Db.Professional on ib.Booking.ProfessionalId equals p.Id
                        where p.Id == proId
                        select new
                        {
                            InviceId = i.Id,
                            InvoiceName = i.Subject,
                            InvoiceNumber = i.InvoiceNumber,
                            InvoiceEntity = ib.Booking.InvoiceEntityId.HasValue ? invoiceEntities.FirstOrDefault(x => x.Id == ib.Booking.InvoiceEntityId).Name : string.Empty,
                            invoiceDate = i.InvoiceDate,
                            totalInvoice = i.TotalInvoice
                        };
            return query.ToList();
        }

        public dynamic GetConnectedLeads(long proId)
        {
            var leadStatus = Db.StaticData.Select((s) => new { Id = s.Id, Value = s.LeadStatus }).Where(x => x.Value != null).ToList();
            var invoiceEntities = Db.InvoiceEntity.ToList();

            var query = from l in Db.Lead
                        join
                        ls in Db.LeadServiceRelation on l.Id equals ls.ServiceId
                        where ls.ProfessionalId == proId
                        select new
                        {
                            Service = ls.Service.Name,
                            l.CreateDate,
                            l.UpdateDate,
                            l.LeadStatusId,
                            LeadStatus = leadStatus.FirstOrDefault(x => x.Id == l.LeadStatusId).Value,
                            Professional = ls.Professional.Name
                        };
            return query.ToList();
        }


        public void GetProfessionalConnectedServices(long proId)
        {
            try
            {
                var result = (from ps in Db.ServiceProfessionalFees
                              where ps.Professional.Id == proId

                              select new ProfessionalConnectedServicesModel
                              {
                                  ProfessionalId = ps.ProfessionalId,
                                  ServiceId = ps.ServiceId,
                                  CService = ps.Service.Name,
                                  CField = ps.Service.Field.Field,
                                  CSubcategory = ps.Service.SubCategory.SubCategory,
                                  PtFeeRowId = ps.Id,
                                  PtFeeId = ps.PtFeeId,
                                  PtFeeName = ps.PtFeeId == null ? default : (ps.PtFee == null ? default : ps.PtFee.FeeName),
                                  PtFeeA1 = ps == null ? default : (ps.PtFee == null ? default : ps.PtFee.A1),
                                  PtFeeA2 = ps == null ? default : (ps.PtFee == null ? default : ps.PtFee.A2),

                                  ProFeeRowId = ps.Id,
                                  ProFeeId = ps.ProFeeId,
                                  ProFeeName = ps.ProFeeId == null ? default : (ps.ProFee == null ? default : ps.ProFee.FeeName),
                                  ProFeeA1 = ps == null ? default : (ps.ProFee == null ? default : ps.ProFee.A1),
                                  ProFeeA2 = ps == null ? default : (ps.ProFee == null ? default : ps.ProFee.A2),

                              }).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void DetachProfessionalConnectedService(IEnumerable<EditProfessionalServiceFeesModel> serviceObjs, long proId)
        {
            try
            {
                foreach (var row in serviceObjs)
                {
                    var rowObj = Db.ServiceProfessionalFees.Find(row.PtFeeRowId);
                    Db.ServiceProfessionalFees.Remove(rowObj);
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public dynamic GetProfessionalServiceDetail(long professionalPtFeeRowId, long professionalProFeeRowId)
        {
            return (from sp in Db.ServiceProfessionalFees
                    join
                    ps in Db.ServiceProfessionalFees on sp.ProfessionalId equals ps.ProfessionalId
                    join
                    pro in Db.ServiceProfessionalFees on sp.ProfessionalId equals pro.ProfessionalId
                    where ps.Id == professionalPtFeeRowId && pro.Id == professionalProFeeRowId

                    select new
                    {
                        sp.Service.Id,
                        sName = sp.Service.Name,
                        //ptFeeId = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PTFee).Item1,
                        //ptFeeName = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PTFee).Item2,
                        //ptFeeA1 = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PTFee).Item3,
                        //ptFeeA2 = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PTFee).Item4,

                        //proFeeId = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PROFee).Item1,
                        //proFeeName = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PROFee).Item2,
                        //proFeeA1 = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PROFee).Item3,
                        //proFeeA2 = GetFeeDetail(fees, sp.Professional.ProfessionalFees.FirstOrDefault().PtFeeId, eFeeType.PROFee).Item4,

                    }).FirstOrDefault();
        }

        public void SaveProfessionalServiceDetail(EditProfessionalServiceFeesModel model)
        {
            try
            {
                if (model.ProfessionalId == 0 || model.ServiceId == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "Professional and Service Ids are required."));
                }

                if (model.PtFeeId > 0)
                {
                    var ptFeeExists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                    if (!(ptFeeExists is null))
                    {
                        ptFeeExists.PtFeeId = model.PtFeeId.Value;
                        Db.ServiceProfessionalFees.Update(ptFeeExists);
                    }
                    else
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { PtFeeId = model.PtFeeId.Value, ProfessionalId = model.ProfessionalId.Value });
                    }

                    Db.SaveChanges();
                }
                else
                {
                    var ptFee = new PtFee
                    {
                        FeeName = model.PtFeeName,
                        A1 = model.PtFeeA1,
                        A2 = model.PtFeeA2,
                        Tags = model.PtFeeTags,
                        CreateDate = DateTime.UtcNow,
                        CreatedById = CurrentUser.Id,
                        AssignedToId = CurrentUser.Id
                    };

                    Db.PtFee.Add(ptFee);
                    Db.SaveChanges();
                    if (ptFee.Id > 0)
                    {
                        var ptFeeRowId = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                        if (ptFeeRowId != null)
                        {
                            ptFeeRowId.PtFeeId = ptFee.Id;
                            Db.ServiceProfessionalFees.Update(ptFeeRowId);
                        }
                    }
                    Db.SaveChanges();
                }

                if (model.ProFeeId > 0)
                {
                    var proFeeExists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                    if (!(proFeeExists is null))
                    {
                        proFeeExists.ProFeeId = model.ProFeeId.Value;
                        Db.ServiceProfessionalFees.Update(proFeeExists);

                    }
                    else
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ProFeeId = model.ProFeeId.Value, ProfessionalId = model.ProfessionalId.Value });
                    }
                    Db.SaveChanges();
                }
                else
                {
                    var proFee = new ProFee
                    {
                        FeeName = model.ProFeeName,
                        A1 = model.ProFeeA1,
                        A2 = model.ProFeeA2,
                        Tags = model.ProFeeTags,
                        CreateDate = DateTime.UtcNow,
                        CreatedById = CurrentUser.Id,
                        AssignedToId = CurrentUser.Id
                    };

                    Db.ProFee.Add(proFee);
                    Db.SaveChanges();
                    if (proFee.Id > 0)
                    {
                        var proFeeRowId = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                        proFeeRowId.ProFeeId = proFee.Id;
                        Db.ServiceProfessionalFees.Update(proFeeRowId);
                    }
                    Db.SaveChanges();
                }

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServicesToAttachWithProfessional(long proId)
        {
            try
            {
                var existingServices = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();

                var result = Db.Service
                    .Where(x => !existingServices.Contains(x.Id))
                    .Select((s) => new
                    {
                        s.Id,
                        s.Name,
                        s.FieldId,
                        s.Field.Field,
                        s.SubcategoryId,
                        s.SubCategory.SubCategory,
                        s.Tags
                    }).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServicesForConnectFilter(long proId)
        {
            try
            {
                var existingProIds = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).Distinct().ToList();

                var services = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
                                select new FilterModel
                                {
                                    Id = s.Id,
                                    Value = s.Name
                                }).ToList();

                var fields = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
                              select new FilterModel
                              {
                                  Id = s.Id,
                                  Value = s.Field.Field
                              }).ToList();
                var subCats = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
                               select new FilterModel
                               {
                                   Id = s.Id,
                                   Value = s.SubCategory.SubCategory
                               }).ToList();
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { services, fields, subCats }));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void AttachServicesToProfessional(IEnumerable<long> serviceIds, long proId)
        {
            try
            {
                foreach (var sid in serviceIds)
                {
                    if (Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == proId) is null)
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = sid, ProfessionalId = proId });
                        Db.SaveChanges();
                    }
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        //public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        //{
        //    try
        //    {
        //        var ptFeesForFilter = new List<FilterModel>();
        //        var proFeesForFilter = new List<FilterModel>();

        //        var row = Db.ServiceProfessionalFees.Find(ptRelationRowId);

        //        if (row != null)
        //        {
        //            var ptFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId).Select(x => x.PtFeeId).Distinct().ToList();
        //            ptFeesForFilter = Db.PtFee.Where(x => ptFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();

        //            var proFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId).Select(x => x.ProFeeId).Distinct().ToList();
        //            proFeesForFilter = Db.ProFee.Where(x => proFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
        //        }


        //        _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));

        //    }
        //    catch (Exception ex)
        //    {
        //        _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
        //    }
        //}

        public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        {
            try
            {
                var ptFeesForFilter = new List<FilterModel>();
                var proFeesForFilter = new List<FilterModel>();

                var ptRow = Db.ServiceProfessionalFees.Find(ptRelationRowId);
                var proRow = Db.ServiceProfessionalFees.Find(proRelationRowId);

                if (ptRow != null)
                {
                    var ptFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == ptRow.ServiceId && x.ProfessionalId == ptRow.ProfessionalId && x.PtFeeId != ptRow.PtFeeId).Select(x => x.PtFeeId).Distinct().ToList();
                    ptFeesForFilter = Db.PtFee.Where(x => !ptFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }

                if (proRow != null)
                {
                    var proFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == ptRow.ServiceId && x.ProfessionalId == ptRow.ProfessionalId && x.ProFeeId != proRow.ProFeeId).Select(x => x.ProFeeId).Distinct().ToList();
                    proFeesForFilter = Db.ProFee.Where(x => !proFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }


                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _appContext.Users.Find(assignedToId).FullName;
        }

    }
}
