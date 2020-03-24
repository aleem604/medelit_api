using Medelit.Common;
using Medelit.Common.Models;
using Medelit.Domain;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Infra.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        {
        }

        public void FindServices(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
                var query = from s in Db.Service

                            where s.Status != eRecordStatus.Deleted
                            select new
                            {
                                s.Id,
                                s.Name,
                                PtFees = string.Join(", ", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.PtFee.FeeName).Distinct().ToList()),
                                ProFees = string.Join(", ", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.ProFee.FeeName).Distinct().ToList()),
                                Professionals = string.Join(", ", s.ServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.Professional.Name).Distinct().ToList()),
                                s.Covermap,
                                s.Status,
                                s.CreateDate,
                                s.CreatedById
                            };


                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Professionals) && x.Professionals.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtFees) && x.PtFees.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProFees) && x.ProFees.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Covermap) && x.Covermap.CLower().Contains(viewModel.Filter.Search.CLower()))
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

                    case "professional":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;

                    case "ptfees":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PtFees);
                        else
                            query = query.OrderByDescending(x => x.PtFees);
                        break;
                    case "profees":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.ProFees);
                        else
                            query = query.OrderByDescending(x => x.ProFees);
                        break;
                    case "covermap":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Covermap);
                        else
                            query = query.OrderByDescending(x => x.Covermap);
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
                                  spfs.ServiceId,
                                  spfs.ProfessionalId,
                                  professional = spfs.Professional.Name,
                                  phone = spfs.Professional.Telephone,
                                  spfs.Professional.Email,
                                  status = ((eCollaborationCodes)spfs.Professional.ActiveCollaborationId).ToString()

                                  //PtFeeRowId = spfs.Id,
                                  //PtFeeId = spfs.PtFeeId,
                                  //PtFeeName = spfs.PtFee == null ? default : spfs.PtFee.FeeName,
                                  //PtFeeA1 = spfs.PtFee == null ? default : spfs.PtFee.A1,
                                  //PtFeeA2 = spfs.PtFee == null ? default : spfs.PtFee.A2,

                                  //ProFeeRowId = spfs.Id,
                                  //ProFeeId = spfs.ProFeeId,
                                  //ProFeeName = spfs.ProFee == null ? default : spfs.ProFee.FeeName,
                                  //ProFeeA1 = spfs.ProFee == null ? default : spfs.ProFee.A1,
                                  //ProFeeA2 = spfs.ProFee == null ? default : spfs.ProFee.A2,
                              }).ToList();

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

                var result = (from spfs in Db.ServiceProfessionalFees
                              where !connectedProfessionals.Contains(spfs.ProfessionalId)
                              select new
                              {
                                  spfs.ServiceId,
                                  spfs.ProfessionalId,
                                  pName = spfs.Professional.Name,

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
                              }).DistinctBy(x => x.ProfessionalId).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<EditProfessionalServiceFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    if (model.PtFeeId.HasValue && serviceId > 0 && model.ProfessionalId.HasValue)
                    {
                        var ex = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == model.ProfessionalId);
                        if (ex is null)
                        {
                            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = model.ProfessionalId.Value, PtFeeId = model.PtFeeId.Value });
                        }
                        else
                        {
                            ex.PtFeeId = model.PtFeeId;
                            Db.ServiceProfessionalFees.Update(ex);
                        }
                        Db.SaveChanges();
                    }

                    if (model.ProFeeId.HasValue && serviceId > 0 && model.ProfessionalId.HasValue)
                    {
                        var ex = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == model.ProfessionalId);
                        if (ex is null)
                        {
                            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = model.ProfessionalId.Value, ProFeeId = model.ProFeeId.Value });
                        }
                        else
                        {
                            ex.ProFeeId = model.ProFeeId;
                            Db.ServiceProfessionalFees.Update(ex);
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

        public void RemoveProfessionalsFromService(IEnumerable<EditProfessionalServiceFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    if (model.ProfessionalId > 0 && serviceId > 0)
                    {
                        var row = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == model.ProfessionalId && x.ServiceId == model.ServiceId).ToList();
                        if (row != null && row.Count() > 0)
                        {
                            Db.ServiceProfessionalFees.RemoveRange(row);
                            Db.SaveChanges();
                        }
                    }
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        #region service connect pt fees
        public void GetServiceConnectedPtFees(long serviceId)
        {
            try
            {
                var result = (from spt in Db.ServiceProfessionalFees
                              where spt.ServiceId == serviceId && spt.PtFeeId > 0
                              select new
                              {
                                  spt.Id,
                                  PtFeeId = spt.PtFeeId,
                                  PtFeeName = spt.PtFee.FeeName,
                                  PtFeeA1 = spt.PtFee.A1,
                                  PtFeeA2 = spt.PtFee.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == spt.PtFeeId && x.ServiceId == serviceId).Select(x => x.Professional.Name).Distinct().ToList())
                              }).DistinctBy(x => x.PtFeeId).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }

        }
        public void GetServiceConnectedPtFeesToConnect(long serviceId)
        {
            try
            {
                var connectedPtFee = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.PtFeeId).ToList();
                var connectedProfs = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).ToList();
                var result = (from spt in Db.ServiceProfessionalFees
                              where !connectedPtFee.Contains(spt.PtFeeId) && spt.PtFeeId > 0
                              && !connectedProfs.Contains(spt.ProfessionalId)
                              select new
                              {
                                  spt.Id,
                                  uid = $"{spt.ServiceId}-{spt.ProfessionalId}-{spt.PtFeeId}",
                                  PtFeeId = spt.PtFeeId,
                                  PtFeeName = spt.PtFee.FeeName,
                                  PtFeeA1 = spt.PtFee.A1,
                                  PtFeeA2 = spt.PtFee.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == spt.PtFeeId).Select(x => x.Professional.Name).Distinct().ToList()),
                                  Services = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == spt.PtFeeId).Select(x => x.Service.Name).Distinct().ToList()),
                                  Tags = string.Join(", ", Db.ServiceProfessionalFees.Where(x => x.PtFeeId == spt.PtFeeId).Select(x => x.Service.Tags).Distinct().ToList()),
                              })
                              .DistinctBy(x => x.PtFeeId, x => x.ProfessionalId)
                              .ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SavePtFeesForService(IEnumerable<ServiceConnectedPtFeesModel> models, long serviceId)
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
                                Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ProfessionalId = row.ProfessionalId, ServiceId = serviceId, PtFeeId = model.PtFeeId });
                                Db.SaveChanges();
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
        public void DetachPtFeeFromService(IEnumerable<ServiceConnectedPtFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    var rows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == serviceId && x.PtFeeId == model.PtFeeId).ToList();
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
                                if(!(exists.ProFeeId > 0))
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
            return (from c in Db.Customer
                    join
                    cs in Db.CustomerServiceRelation on c.Id equals cs.CustomerId
                    where cs.ServiceId == serviceId
                    select new
                    {
                        Customer = c.Name,
                        InvoiceEntity = c.InvoiceEntityId.HasValue ? c.InvoiceEntity.Name : string.Empty,
                        Phone = c.MainPhone,
                        c.Email
                    }).DistinctBy(x => x.Customer).ToList();
        }

        public dynamic GetConnectedBookings(long serviceId)
        {
            return (from ps in Db.ServiceProfessionalFees
                    join
                    b in Db.Booking on ps.ServiceId equals b.ServiceId
                    where ps.ServiceId == serviceId
                    select new
                    {
                        bookingName = b.Name,
                        ServiceName = ps.Service.Name,
                        //PtFee = ps.Service.PtFee.FeeName,
                        //PtFeeA1 = ps.Service.PtFee.A1,
                        //ptFeeA2 = ps.Service.PtFee.A2,

                        //ProFeeName = ps.Service.ProFee.FeeName,
                        //ProFeeA1 = ps.Service.ProFee.A1,
                        //proFeeA2 = ps.Service.ProFee.A2,

                        CustomerName = b.Customer.Name,
                        InvoiceEntity = b.InvoiceEntityId.HasValue ? b.InvoiceEntity.Name : string.Empty,
                        VisitDate = b.VisitStartDate
                    }).ToList();
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

    }
}
