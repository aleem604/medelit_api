using Medelit.Common;
using Medelit.Common.Models;
using Medelit.Domain;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medelit.Infra.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IStaticDataRepository _static;
        public ServiceRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, ApplicationDbContext appContext, IStaticDataRepository @static)
            : base(context, contextAccessor, bus)
        {
            _appContext = appContext;
            _static = @static;
        }

        public void FindServices(SearchViewModel viewModel)
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

                var durations = _static.GetDurations();
                var vats = _static.GetVats();
                var fieldSubcats = Db.FieldSubCategory.ToList();
                var serviceProFees = Db.ServiceProfessionalFees.ToList();
                var professionals = Db.Professional.ToList();
                var ptFees = Db.PtFee.ToList();
                var proFees = Db.ProFee.ToList();

                var query = from s in Db.Service

                            where s.Status != eRecordStatus.Deleted
                            select new
                            {
                                s.Id,
                                s.ServiceCode,
                                s.Name,
                                s.CycleId,
                                Cycle = s.CycleId.HasValue && s.CycleId.Value == 1 ? "Yes" : "No",
                                s.ActiveServiceId,
                                ActiveService = s.ActiveServiceId.HasValue && s.ActiveServiceId.Value == 1 ? "Yes" : "No",
                                s.FieldId,
                                field = (fieldSubcats.FirstOrDefault(f => f.Id == s.FieldId) ?? new FieldSubCategory()).Field,
                                s.SubcategoryId,
                                subCategory = (fieldSubcats.FirstOrDefault(f => f.Id == s.SubcategoryId) ?? new FieldSubCategory()).SubCategory,
                                s.DurationId,
                                Duration = (durations.FirstOrDefault(f => f.Id == s.DurationId) ?? new FilterModel()).Value,
                                s.TimedServiceId,
                                TimedService = s.TimedServiceId == 1 ? "Yes" : "No",
                                s.VatId,
                                Vat = (vats.FirstOrDefault(f => f.Id == s.VatId) ?? new FilterModel()).Value,
                                s.Description,
                                s.Covermap,
                                s.InvoicingNotes,
                                s.ContractedServiceId,
                                ContractedService = s.ContractedServiceId == 1 ? "Yes" : "No",
                                s.RefundNotes,
                                s.InformedConsentId,
                                InformedConsent = s.InformedConsentId == 1 ? "Yes" : "No",
                                Tags = s.Tags != null && s.Tags.Length > 0 ? s.Tags.Replace(",", "<br/>") : s.Tags,

                                PtFees = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.PtFee.FeeName).Distinct().ToList()),
                                PtFeesA1 = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => string.Format("{0:F2}", x.PtFee.A1)).Distinct().ToList()),
                                PtFeesA2 = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => string.Format("{0:F2}", x.PtFee.A2)).Distinct().ToList()),
                                ProFees = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.ProFee.FeeName).Distinct().ToList()),
                                ProFeesA1 = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => string.Format("{0:F2}", x.ProFee.A1)).Distinct().ToList()),
                                ProFeesA2 = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => string.Format("{0:F2}", x.ProFee.A2)).Distinct().ToList()),
                                Professionals = string.Join("<br/>", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.Professional.Name).Distinct().ToList()),
                                Pros = GetProfessions(s, serviceProFees.Where(sf => sf.ServiceId == s.Id).ToList(), professionals, ptFees, proFees),

                                s.CreateDate,
                                AssignedTo = GetAssignedUser(s.AssignedToId),
                            };

                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ServiceCode) && x.ServiceCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Cycle) && x.Cycle.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ActiveService) && x.ActiveService.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.field) && x.field.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.subCategory) && x.subCategory.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Duration) && x.Duration.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.TimedService) && x.TimedService.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Vat) && x.Vat.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Description) && x.Description.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Covermap) && x.Covermap.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ContractedService) && x.ContractedService.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.RefundNotes) && x.RefundNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InformedConsent) && x.InformedConsent.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Tags) && x.Tags.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.Professionals) && x.Professionals.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.PtFees) && x.PtFees.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtFeesA1) && x.PtFeesA1.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtFeesA2) && x.PtFeesA2.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.ProFees) && x.ProFees.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProFeesA1) && x.ProFeesA1.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProFeesA2) && x.ProFeesA2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Pros) && x.Pros.CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (x.CreateDate.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Id.ToString().Contains(viewModel.Filter.Search))

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

                    case "field":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.field);
                        else
                            query = query.OrderByDescending(x => x.field);
                        break;

                    case "subCategory":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.subCategory);
                        else
                            query = query.OrderByDescending(x => x.subCategory);
                        break;



                    case "professional":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;

                    case "ptFees":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PtFees);
                        else
                            query = query.OrderByDescending(x => x.PtFees);
                        break;
                    case "ptFeesA1":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PtFeesA1);
                        else
                            query = query.OrderByDescending(x => x.PtFeesA1);
                        break;
                    case "ptFeesA2":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PtFeesA2);
                        else
                            query = query.OrderByDescending(x => x.PtFeesA2);
                        break;

                    case "proFees":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.ProFees);
                        else
                            query = query.OrderByDescending(x => x.ProFees);
                        break;
                    case "proFeesA1":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.ProFeesA1);
                        else
                            query = query.OrderByDescending(x => x.ProFeesA2);
                        break;


                    case "covermap":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Covermap);
                        else
                            query = query.OrderByDescending(x => x.Covermap);
                        break;

                    case "pros":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Pros);
                        else
                            query = query.OrderByDescending(x => x.Pros);
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


        private string GetProfessions(Service service, List<ServiceProfessionalFees> fees, List<Professional> professionals, List<PtFee> ptFees, List<ProFee> proFees)
        {
            var result = new StringBuilder($"<table class='table table-bordered table-sm table-striped custom-table mt-3'>");
            fees.ForEach(f => {
                var professional = (professionals.FirstOrDefault(p => p.Id == f.ProfessionalId) ?? new Professional()).Name;
                var ptFee = (ptFees.FirstOrDefault(p => p.Id == f.PtFeeId) ?? new PtFee());
                var proFee = (proFees.FirstOrDefault(p => p.Id == f.ProFeeId) ?? new ProFee());

                result.Append($"<thead><tr>");
                result.Append($"<td> {professional}</td>");
                result.Append($"<td> {string.Format("{0:F2}", ptFee.A1)}</td>");
                result.Append($"<td> {string.Format("{0:F2}", ptFee.A2)}</td>");
                result.Append($"<td> {string.Format("{0:F2}", proFee.A1)}</td>");
                result.Append($"<td> {string.Format("{0:F2}", proFee.A2)} </td>");
                result.Append($"</tr></thead>");
            });
            
            return result.ToString();
        }


        public void AddUpdateFeeForService(AddUpdateFeeToService model)
        {
            try
            {
                var service = Db.Service.FirstOrDefault(x => x.Id == model.ServiceId);
                if (!(service is null))
                {
                    if (model.PtFeeId > 0 && model.ProFeeId > 0)
                    {
                        if (!(service is null))
                        {
                            //service.PTFeeId = model.PtFeeId;
                            //service.PROFeeId = model.ProFeeId;
                            Db.Service.Update(service);
                            Db.SaveChanges();
                            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, "service updated successfully"));
                            return;
                        }
                    }
                    else
                    {
                        var ptFeeModel = new PtFee
                        {
                            FeeName = model.PtFeeName,
                            A1 = model.PtFeeA1,
                            A2 = model.PtFeeA2,
                            Tags = model.PtFeeTags,
                            AssignedToId = CurrentUser.Id,
                            CreateDate = DateTime.UtcNow,
                            CreatedById = CurrentUser.Id
                        };

                        var proFeeModel = new PtFee
                        {
                            FeeName = model.ProFeeName,
                            A1 = model.ProFeeA1,
                            A2 = model.ProFeeA2,
                            Tags = model.ProFeeTags,
                            AssignedToId = CurrentUser.Id,
                            CreateDate = DateTime.UtcNow,
                            CreatedById = CurrentUser.Id
                        };

                        Db.PtFee.Add(ptFeeModel);
                        Db.PtFee.Add(proFeeModel);
                        Db.SaveChanges();
                        if (ptFeeModel.Id > 0 && proFeeModel.Id > 0)
                        {
                            //service.PTFeeId = ptFeeModel.Id;
                            //service.PROFeeId = proFeeModel.Id;

                            ptFeeModel.FeeCode = ptFeeModel.FeeTypeId == eFeeType.PTFee ? $"FP{ptFeeModel.Id.ToString().PadLeft(6, '0')}" : $"FS{ptFeeModel.Id.ToString().PadLeft(6, '0')}";
                            proFeeModel.FeeCode = proFeeModel.FeeTypeId == eFeeType.PTFee ? $"FP{proFeeModel.Id.ToString().PadLeft(6, '0')}" : $"FS{proFeeModel.Id.ToString().PadLeft(6, '0')}";

                            ptFeeModel.UpdateDate = DateTime.UtcNow;
                            ptFeeModel.UpdatedById = CurrentUser.Id;

                            proFeeModel.UpdateDate = DateTime.UtcNow;
                            proFeeModel.UpdatedById = CurrentUser.Id;

                            Db.PtFee.Update(ptFeeModel);
                            Db.PtFee.Update(proFeeModel);
                        }
                        service.UpdateDate = DateTime.UtcNow;
                        service.UpdatedById = CurrentUser.Id;

                        Db.Service.Update(service);
                        Db.SaveChanges();
                        _bus.RaiseEvent(new DomainNotification(GetType().Name, null, "service updated successfully"));
                        return;
                    }
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "Service key was not found"));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public Service GetByIdWithIncludes(long serviceId)
        {
            return Db.Service.Where(x => x.Id == serviceId).Include(x => x.ServiceProfessionalFees).FirstOrDefault();
        }

        public void RemoveProfessionals(long serviceId)
        {
            var professionals = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).ToList();
            Db.RemoveRange(professionals);
            Db.SaveChanges();

        }

        public dynamic GetServiceProfessionals(long proId, long? fieldId, long? categoryId, string tag)
        {
            var proServices = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.Id).ToList();
            var existingServiceIds = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();

            var query = Db.Service
                .Where(x => !existingServiceIds.Contains(x.Id))
                .Select((s) => new
                {
                    s.Id,
                    s.Name,

                    s.FieldId,
                    s.Field.Field,
                    s.SubcategoryId,
                    s.SubCategory.SubCategory,
                    s.Tags
                });
            if (fieldId.HasValue)
                query = query.Where(x => x.FieldId == fieldId);
            if (categoryId.HasValue)
                query = query.Where(x => x.SubcategoryId == fieldId);

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => x.Tags.Contains(tag));
            }

            var services = query.ToList();
            return new
            {
                serviceIds = proServices,
                services
            };
        }

        public void AddProfessionalToService(long serviceId, long professionalId)
        {
            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = professionalId });
        }

        public void DetachProfessional(long serviceId, long professionalId)
        {
            var relation = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
            Db.ServiceProfessionalFees.Remove(relation);
        }

        public void GetServiceConnectedProfessionals(long serviceId)
        {
            try
            {
                var result = (from spfs in Db.ServiceProfessionalFees
                              where spfs.ServiceId == serviceId
                              select new
                              {
                                  spfs.Id,
                                  spfs.ServiceId,
                                  spfs.ProfessionalId,
                                  professional = spfs.Professional.Name,
                                  phone = spfs.Professional.Telephone,
                                  spfs.Professional.Email,
                                  status = ((eCollaborationCodes)spfs.Professional.ActiveCollaborationId).ToString(),

                                  PtFeeRowId = spfs.Id,
                                  PtFeeId = spfs.PtFeeId,
                                  PtFeeName = spfs.PtFee == null ? default : spfs.PtFee.FeeName,
                                  PtFeeA1 = spfs.PtFee == null ? default : spfs.PtFee.A1,
                                  PtFeeA2 = spfs.PtFee == null ? default : spfs.PtFee.A2,

                                  ProFeeRowId = spfs.Id,
                                  ProFeeId = spfs.ProFeeId,
                                  ProFeeName = spfs.ProFee == null ? default : spfs.ProFee.FeeName,
                                  ProFeeA1 = spfs.ProFee == null ? default : spfs.ProFee.A1,
                                  ProFeeA2 = spfs.ProFee == null ? default : spfs.ProFee.A2,
                              }).DistinctBy(d => d.ProfessionalId).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetProfessionalsWithFeesToConnectWithService(long serviceId)
        {
            try
            {
                var connectedProfessionals = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).Distinct().ToList();

                var result = (from p in Db.Professional
                              where !connectedProfessionals.Contains(p.Id)
                              select new
                              {
                                  p.Id,
                                  name = p.Name,
                                  email = p.Email,
                                  telephone = p.Telephone,
                                  p.City,
                                  country = p.Country.Value,

                                  ////PtFeeRowId = p.Id,
                                  //PtFeeId = p.ServiceProfessionalFees.FirstOrDefault().PtFeeId,
                                  //PtFeeName = p.ServiceProfessionalFees.FirstOrDefault().PtFee.FeeName,
                                  //PtFeeA1 = p.ServiceProfessionalFees.FirstOrDefault().PtFee.A1,
                                  //PtFeeA2 = p.ServiceProfessionalFees.FirstOrDefault().PtFee.A2,

                                  //ProFeeId = p.ServiceProfessionalFees.FirstOrDefault().ProFeeId,
                                  //ProFeeName = p.ServiceProfessionalFees.FirstOrDefault().ProFee.FeeName,
                                  //ProFeeA1 = p.ServiceProfessionalFees.FirstOrDefault().ProFee.A1,
                                  //ProFeeA2 = p.ServiceProfessionalFees.FirstOrDefault().ProFee.A2,
                              }).DistinctBy(x => x.Id).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<long> proIds, long serviceId)
        {
            try
            {
                foreach (var proId in proIds)
                {
                    if (serviceId > 0 && proId > 0)
                    {
                        var profees = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ProfessionalId == proId) ?? new ServiceProfessionalFees();
                        var ex = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == proId);
                        if (ex is null)
                        {
                            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = proId });
                        }
                        Db.SaveChanges();
                    }
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void RemoveProfessionalsFromService(IEnumerable<long> rowIds, long serviceId)
        {
            try
            {
                var rows = Db.ServiceProfessionalFees.Where(x => rowIds.Contains(x.Id)).ToList();
                Db.ServiceProfessionalFees.RemoveRange(rows);
                Db.SaveChanges();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServiceProfessionalFeeRowDetail(long rowId)
        {
            try
            {
                var result = (from p in Db.ServiceProfessionalFees
                              where p.Id == rowId
                              select new
                              {
                                  p.Id,
                                  p.ServiceId,
                                  p.ProfessionalId,
                                  p.PtFeeId,
                                  p.ProFeeId
                              }).FirstOrDefault();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServiceProfessionalFeesForFilter(long rowId)
        {
            try
            {
                var ptFeesForFilter = new List<FilterModel>();
                var proFeesForFilter = new List<FilterModel>();

                var row = Db.ServiceProfessionalFees.Find(rowId);

                if (row != null)
                {
                    var ptFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId && x.ProfessionalId == row.ProfessionalId && x.PtFeeId != row.PtFeeId).Select(x => x.PtFeeId).Distinct().ToList();
                    ptFeesForFilter = Db.PtFee.Where(x => !ptFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }

                if (row != null)
                {
                    var proFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId && x.ProfessionalId == row.ProfessionalId && x.ProFeeId != row.ProFeeId).Select(x => x.ProFeeId).Distinct().ToList();
                    proFeesForFilter = Db.ProFee.Where(x => !proFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }


                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveProfessionalServicesFees(ProfessionalConnectedServicesModel model, long rowId)
        {
            try
            {
                if (model.PtFeeId > 0 && model.ProFeeId > 0)
                {
                    var row = Db.ServiceProfessionalFees.Find(rowId);
                    if (row != null)
                    {
                        row.PtFeeId = model.PtFeeId;
                        row.ProFeeId = model.ProFeeId;
                        Db.ServiceProfessionalFees.Update(row);
                        Db.SaveChanges();
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, row));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
                return;
            }

            _bus.RaiseEvent(new DomainNotification(GetType().Name, "Invalid data"));
        }

        #region service connect fees

        public void GetServiceConnectedFees(long serviceId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var result = (from spt in Db.ServiceProfessionalFees
                                  where spt.ServiceId == serviceId && spt.PtFeeId > 0
                                  select new
                                  {
                                      spt.Id,
                                      spt.PtFeeId,
                                      PtFeeName = spt.PtFee.FeeName,
                                      PtFeeA1 = spt.PtFee.A1,
                                      PtFeeA2 = spt.PtFee.A2,
                                      spt.ServiceId,
                                      spt.ProfessionalId,
                                      Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == spt.PtFeeId && x.ServiceId == serviceId).Select(x => x.Professional.Name).Distinct().ToList())
                                  }).DistinctBy(x => x.PtFeeId).ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var result = (from spt in Db.ServiceProfessionalFees
                                  where spt.ServiceId == serviceId && spt.ProFeeId > 0
                                  select new
                                  {
                                      spt.Id,
                                      spt.ProFeeId,
                                      ProFeeName = spt.ProFee.FeeName,
                                      ProFeeA1 = spt.ProFee.A1,
                                      ProFeeA2 = spt.ProFee.A2,
                                      spt.ServiceId,
                                      spt.ProfessionalId,
                                      Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == spt.ProFeeId && x.ServiceId == serviceId).Select(x => x.Professional.Name).Distinct().ToList())
                                  }).DistinctBy(x => x.ProFeeId).ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }

        }

        public void GetServiceConnectedFeesToConnect(long serviceId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var connectedPtFee = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.PtFeeId).ToList();
                    var result = (from fee in Db.PtFee
                                  where !connectedPtFee.Contains(fee.Id)
                                  select new
                                  {
                                      fee.Id,
                                      PtFeeId = fee.Id,
                                      PtFeeName = fee.FeeName,
                                      PtFeeA1 = fee.A1,
                                      PtFeeA2 = fee.A2,
                                      //Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == fee.Id).Select(x => x.Professional.Name).Distinct().ToList()),
                                      //Services = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == fee.Id).Select(x => x.Service.Name).Distinct().ToList()),
                                      //Tags = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == fee.Id).Select(x => x.Service.Tags).Distinct().ToList()),
                                  })
                                  .DistinctBy(x => x.Id)
                                  .ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var connectedProFee = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProFeeId).ToList();
                    var result = (from fee in Db.ProFee
                                  where !connectedProFee.Contains(fee.Id)
                                  select new
                                  {
                                      fee.Id,
                                      ProFeeId = fee.Id,
                                      ProFeeName = fee.FeeName,
                                      ProFeeA1 = fee.A1,
                                      ProFeeA2 = fee.A2,
                                      //Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == fee.Id).Select(x => x.Professional.Name).Distinct().ToList()),
                                      //Services = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == fee.Id).Select(x => x.Service.Name).Distinct().ToList()),
                                      //Tags = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == fee.Id).Select(x => x.Service.Tags).Distinct().ToList()),
                                  })
                                  .DistinctBy(x => x.Id)
                                  .ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveFeesForService(IEnumerable<long> feeIds, long serviceId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    foreach (var feeId in feeIds)
                    {
                        if (feeId > 0)
                        {
                            var row = Db.ServiceProfessionalFees.FirstOrDefault(x => x.PtFeeId == feeId && x.ServiceId == serviceId);
                            if (row is null)
                            {
                                Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, PtFeeId = feeId });
                                Db.SaveChanges();
                            }
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
                else
                {
                    foreach (var feeId in feeIds)
                    {
                        if (feeId > 0)
                        {
                            var row = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ProFeeId == feeId && x.ServiceId == serviceId);
                            if (row is null)
                            {
                                Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProFeeId = feeId });
                                Db.SaveChanges();
                            }
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void DetachFeeFromService(IEnumerable<long> feeIds, long serviceId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    foreach (var feeId in feeIds)
                    {
                        var rows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId && x.PtFeeId == feeId).ToList();
                        foreach (var row in rows)
                        {
                            if (row.ProFeeId > 0)
                            {
                                row.PtFeeId = null;
                                Db.ServiceProfessionalFees.Update(row);
                                Db.SaveChanges();
                            }
                            else
                            {
                                Db.ServiceProfessionalFees.Remove(row);
                                Db.SaveChanges();
                            }
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
                else
                {
                    foreach (var feeId in feeIds)
                    {
                        var rows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId && x.ProFeeId == feeId).ToList();
                        foreach (var row in rows)
                        {
                            if (row.PtFeeId > 0)
                            {
                                row.PtFeeId = null;
                                Db.ServiceProfessionalFees.Update(row);
                                Db.SaveChanges();
                            }
                            else
                            {
                                Db.ServiceProfessionalFees.Remove(row);
                                Db.SaveChanges();
                            }
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        #endregion service connect pt fees

        #region service connect pro fees

        public void GetServiceConnectedProFees(long serviceId)
        {
            try
            {
                var result = (from spt in Db.ServiceProfessionalFees
                              where spt.ServiceId == serviceId && spt.ProFeeId > 0
                              select new
                              {
                                  spt.Id,
                                  ProFeeId = spt.ProFeeId,
                                  ProFeeName = spt.ProFee.FeeName,
                                  ProFeeA1 = spt.ProFee.A1,
                                  ProFeeA2 = spt.ProFee.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == spt.ProFeeId && x.ServiceId == serviceId).Select(x => x.Professional.Name).Distinct().ToList())
                              }).DistinctBy(x => x.ProFeeId).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }

        }
        public void GetServiceConnectedProFeesToConnect(long serviceId)
        {
            try
            {
                var connectedProFee = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProFeeId).ToList();
                var connectedProfs = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).ToList();
                var result = (from spt in Db.ServiceProfessionalFees
                              where !connectedProFee.Contains(spt.ProFeeId) && spt.ProFeeId > 0
                              && !connectedProfs.Contains(spt.ProfessionalId)
                              select new
                              {
                                  spt.Id,
                                  uid = $"{spt.ServiceId}-{spt.ProfessionalId}-{spt.ProFeeId}",
                                  ProFeeId = spt.ProFeeId,
                                  ProFeeName = spt.ProFee.FeeName,
                                  ProFeeA1 = spt.ProFee.A1,
                                  ProFeeA2 = spt.ProFee.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == spt.ProFeeId).Select(x => x.Professional.Name).Distinct().ToList()),
                                  Services = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == spt.ProFeeId).Select(x => x.Service.Name).Distinct().ToList()),
                                  Tags = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.ProFeeId == spt.ProFeeId).Select(x => x.Service.Tags).Distinct().ToList()),
                              })
                              .DistinctBy(x => x.ProFeeId, x => x.ProfessionalId)
                              .ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveProFeesForService(IEnumerable<ServiceConnectedProFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    if (model.Id > 0)
                    {
                        var row = Db.ServiceProfessionalFees.Find(model.Id);
                        if (row != null)
                        {
                            var exists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == row.ProfessionalId);
                            if (exists is null)
                            {
                                Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ProfessionalId = row.ProfessionalId, ServiceId = serviceId, ProFeeId = model.ProFeeId });
                                Db.SaveChanges();
                            }
                            else
                            {
                                if (!(exists.ProFeeId > 0))
                                {
                                    exists.ProFeeId = model.ProFeeId;
                                    Db.ServiceProfessionalFees.Update(exists);
                                    Db.SaveChanges();
                                }
                            }

                        }
                    }
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }
        public void DetachProFeeFromService(IEnumerable<ServiceConnectedProFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    var rows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId && x.ProFeeId == model.ProFeeId).ToList();
                    foreach (var row in rows)
                    {
                        if (row.PtFeeId > 0)
                        {
                            row.ProFeeId = null;
                            Db.ServiceProfessionalFees.Update(row);
                            Db.SaveChanges();
                        }
                        else
                        {
                            Db.ServiceProfessionalFees.Remove(row);
                            Db.SaveChanges();
                        }
                    }
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        #endregion service connect pro fees

        #region Tabs Data
        public dynamic GetProfessionalServicesWithInclude(long professionalId)
        {
            return Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == professionalId)
                .Include(x => x.Service).ThenInclude(x => x.Field)
                .Include(x => x.Service).ThenInclude(x => x.SubCategory)
                .ToList();
        }

        public dynamic GetConnectedCustomersInvoicingEntities(long serviceId)
        {
            return (from cs in Db.CustomerServiceRelation
                    where cs.ServiceId == serviceId
                    select new
                    {
                        cs.CustomerId,
                        Customer = cs.Customer.Name,
                        InvoiceEntity = cs.Customer.InvoiceEntityId.HasValue ? cs.Customer.InvoiceEntity.Name : string.Empty,
                        Phone = cs.Customer.MainPhone,
                        cs.Customer.Email
                    }).DistinctBy(x => x.CustomerId).ToList();
        }

        public dynamic GetConnectedBookings(long serviceId)
        {
            return (from ps in Db.ServiceProfessionalFees
                    join
                    b in Db.Booking on ps.ServiceId equals b.ServiceId
                    where ps.ServiceId == serviceId
                    select new
                    {
                        b.Id,
                        bookingName = b.Name,
                        ServiceName = ps.Service.Name,
                        PtFee = ps.PtFee.FeeName,
                        PtFeeA1 = string.Format("{0:F2}", ps.PtFee.A1),
                        ptFeeA2 = string.Format("{0:F2}", ps.PtFee.A2),

                        ProFeeName = ps.ProFee.FeeName,
                        ProFeeA1 = string.Format("{0:F2}", ps.ProFee.A1),
                        proFeeA2 = string.Format("{0:F2}", ps.ProFee.A2),

                        CustomerName = b.Customer.Name,
                        InvoiceEntity = b.InvoiceEntityId.HasValue ? b.InvoiceEntity.Name : string.Empty,
                        VisitDate = b.VisitStartDate
                    }).DistinctBy(x => x.Id).ToList();
        }

        public dynamic GetConnectedCustomerInvoices(long serviceId)
        {
            return (from ib in Db.InvoiceBookings
                    join
                    s in Db.Service on ib.Booking.ServiceId equals s.Id
                    where s.Id == serviceId
                    select new
                    {
                        InvoiceName = ib.Invoice.Subject,
                        InvoiceNumber = ib.Invoice.InvoiceNumber,
                        InvoiceEntity = ib.Invoice.InvoiceEntityId.HasValue ? ib.Invoice.InvoiceEntity.Name : string.Empty,
                        ib.Invoice.InvoiceDate,
                        ib.Invoice.TotalInvoice
                    }
                ).ToList();
        }

        public dynamic GetConnectedLeads(long serviceId)
        {
            var leadStatus = Db.StaticData.Select((s) => new { Id = s.Id, Value = s.LeadStatus }).Where(x => x.Value != null).ToList();

            return (from ls in Db.LeadServiceRelation
                    join
                    l in Db.Lead on ls.LeadId equals l.Id
                    where ls.ServiceId == serviceId
                    select new
                    {
                        l.Id,
                        service = ls.Service.Name,
                        l.CreateDate,
                        l.UpdateDate,
                        l.LeadStatusId,
                        Status = l.LeadStatusId.HasValue ? leadStatus.FirstOrDefault(x => x.Id == l.LeadStatusId).Value : string.Empty,
                        Professional = string.Join(", ", ls.Service.ServiceProfessionalFees.Select(x => x.Professional.Name).ToArray())
                    }
                ).ToList();
        }
        #endregion Tabs Data

        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _appContext.Users.Find(assignedToId).FullName;
        }

    }
}
