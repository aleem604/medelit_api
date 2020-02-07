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
                                PtFees = string.Join(", ", s.VServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.PtFee.FeeName).Distinct().ToList()),
                                ProFees = string.Join(", ", s.VServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.ProFee.FeeName).Distinct().ToList()),
                                Professionals = string.Join(", ", s.VServiceProfessionalFees.Where(x => x.ServiceId == s.Id).Select(x => x.ServiceProfessionals.Professional.Name).Distinct().ToList()),
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

        public IQueryable<ServiceProfessionals> GetServiceProfessionals()
        {
            return Db.ServiceProfessionals;
        }

        public IEnumerable<Service> GetAllWithProfessionals()
        {
            return Db.Service.Include(x => x.ServiceProfessionals);
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
            return Db.Service.Where(x => x.Id == serviceId).Include(x => x.ServiceProfessionals).FirstOrDefault();
        }

        public void RemoveProfessionals(long serviceId)
        {
            var professionals = Db.ServiceProfessionals.Where(x => x.ServiceId == serviceId).ToList();
            Db.RemoveRange(professionals);
            Db.SaveChanges();

        }

        public dynamic GetProfessionalServices(long proId, long? fieldId, long? categoryId, string tag)
        {
            var proServices = Db.ServiceProfessionals.Where(x => x.ProfessionalId == proId).Select(x => x.Id).ToList();
            var existingServiceIds = Db.ServiceProfessionals.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();

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
            Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = professionalId });
        }

        public void DetachProfessional(long serviceId, long professionalId)
        {
            var relation = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
            Db.ServiceProfessionals.Remove(relation);
        }

        public void GetServiceConnectedProfessionals(long serviceId)
        {
            try
            {

                var connectedProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).Distinct().ToList();

                var result = (from v in Db.VServiceProfessionalFees
                              where connectedProfessionals.Contains(v.ProfessionalId)
                              select new
                              {
                                  v.ServiceId,
                                  v.ProfessionalId,
                                  professional = v.ServiceProfessionals.Professional.Name,

                                  PtFeeRowId = v.ProfessionalPtFeesId,
                                  PtFeeId = v.PtFeeId,
                                  PtFeeName = v.PtFee == null ? default : v.PtFee.FeeName,
                                  PtFeeA1 = v.PtFee == null ? default : v.PtFee.A1,
                                  PtFeeA2 = v.PtFee == null ? default : v.PtFee.A2,

                                  ProFeeRowId = v.ProfessionalProFeeId,
                                  ProFeeId = v.ProFeeId,
                                  ProFeeName = v.ProFee == null ? default : v.ProFee.FeeName,
                                  ProFeeA1 = v.ProFee == null ? default : v.ProFee.A1,
                                  ProFeeA2 = v.ProFee == null ? default : v.ProFee.A2,
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
                var connectedProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).Distinct().ToList();

                var result = (from v in Db.VServiceProfessionalFees
                              where !connectedProfessionals.Contains(v.ProfessionalId)
                              select new
                              {
                                  v.ServiceId,
                                  v.ProfessionalId,
                                  pName = v.ServiceProfessionals.Professional.Name,

                                  PtFeeRowId = v.ProfessionalPtFeesId,
                                  PtFeeId = v.PtFeeId,
                                  PtFeeName = v.PtFee == null ? default : v.PtFee.FeeName,
                                  PtFeeA1 = v.PtFee == null ? default : v.PtFee.A1,
                                  PtFeeA2 = v.PtFee == null ? default : v.PtFee.A2,

                                  ProFeeRowId = v.ProfessionalProFeeId,
                                  ProFeeId = v.ProFeeId,
                                  ProFeeName = v.ProFee == null ? default : v.ProFee.FeeName,
                                  ProFeeA1 = v.ProFee == null ? default : v.ProFee.A1,
                                  ProFeeA2 = v.ProFee == null ? default : v.ProFee.A2,
                              }).ToList();

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

                        var ex = Db.ProfessionalPtFees.FirstOrDefault(x => x.ProfessionalId == model.ProfessionalId && x.PtFeeId == model.PtFeeId);
                        if (ex is null)
                        {
                            Db.ProfessionalPtFees.Add(new ProfessionalPtFees { PtFeeId = model.PtFeeId.Value, ProfessionalId = model.ProfessionalId.Value });

                        }
                        var sexists = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == model.ProfessionalId.Value);
                        if (sexists is null)
                        {
                            Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = model.ProfessionalId.Value });
                        }
                        Db.SaveChanges();
                    }

                    if (model.ProFeeId.HasValue && serviceId > 0 && model.ProfessionalId.HasValue)
                    {

                        var ex = Db.ProfessionalProFees.FirstOrDefault(x => x.ProfessionalId == model.ProfessionalId && x.ProFeeId == model.ProFeeId);
                        if (ex is null)
                        {
                            Db.ProfessionalProFees.Add(new ProfessionalProFees { ProFeeId = model.ProFeeId.Value, ProfessionalId = model.ProfessionalId.Value });
                        }

                        var sexists = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == model.ProfessionalId.Value);
                        if (sexists is null)
                        {
                            Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = model.ProfessionalId.Value });
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
                        var row = Db.ServiceProfessionals.Where(x => x.ProfessionalId == model.ProfessionalId && x.ServiceId == model.ServiceId).ToList();
                        if (row != null && row.Count() >0 )
                        {
                            Db.ServiceProfessionals.RemoveRange(row);
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
                var connectedPtFees = Db.VServiceProfessionalPtFees.Where(x => x.ServiceId == serviceId).Select(x => x.PtFeeId).ToList();

                var result = (from f in Db.PtFee
                              join
                                spt in Db.VServiceProfessionalPtFees on f.Id equals spt.PtFeeId
                              where connectedPtFees.Contains(f.Id)
                              select new
                              {
                                  PtFeeId = f.Id,
                                  PtFeeName = f.FeeName,
                                  PtFeeA1 = f.A1,
                                  PtFeeA2 = f.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", f.VServiceProfessionalPtFees.Where(x => x.ServiceId == serviceId && x.PtFeeId == f.Id).Select(x => x.ServiceProfessionals.Professional.Name).Distinct().ToList()),
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
                var connectedPtFees = Db.VServiceProfessionalPtFees.Where(x => x.ServiceId == serviceId).Select(x => x.PtFeeId).ToList();

                var result = (from f in Db.PtFee
                              join
                                spt in Db.VServiceProfessionalPtFees on f.Id equals spt.PtFeeId
                              where !connectedPtFees.Contains(f.Id)
                              select new
                              {
                                  PtFeeId = f.Id,
                                  PtFeeName = f.FeeName,
                                  PtFeeA1 = f.A1,
                                  PtFeeA2 = f.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", f.VServiceProfessionalPtFees.Where(x => x.ServiceId != serviceId && x.PtFeeId == f.Id).Select(x => x.ServiceProfessionals.Professional.Name).Distinct().ToList()),
                                  Services = string.Join(", ", f.VServiceProfessionalPtFees.Where(x => x.ServiceId != serviceId && x.PtFeeId == f.Id).Select(x => x.ServiceProfessionals.Service.Name).Distinct().ToList()),
                                  Tags = string.Join(", ", f.VServiceProfessionalPtFees.Where(x => x.ServiceId != serviceId && x.PtFeeId == f.Id).Select(x => x.ServiceProfessionals.Service.Tags).Distinct().ToList()),
                              }).DistinctBy(x => x.PtFeeId).ToList();

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
                    if (model.PtFeeId > 0)
                    {
                        var serviceProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).ToList();
                        foreach (var pro in serviceProfessionals)
                        {
                            var exists = Db.ProfessionalPtFees.FirstOrDefault(x => x.PtFeeId == model.PtFeeId && x.ProfessionalId == pro);
                            if (exists is null)
                            {
                                Db.ProfessionalPtFees.Add(new ProfessionalPtFees { ProfessionalId = pro, PtFeeId = model.PtFeeId });
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
                    if (model.PtFeeId > 0)
                    {
                        var serviceProfessionals = Db.VServiceProfessionalPtFees.Where(x => x.ServiceId == serviceId && x.PtFeeId == model.PtFeeId).Select(x => x.ProfessionalId).ToList();
                        foreach (var pro in serviceProfessionals)
                        {
                            var feeObject = Db.ProfessionalPtFees.FirstOrDefault(x => x.PtFeeId == model.PtFeeId && x.ProfessionalId == pro);
                            if (feeObject != null)
                            {
                                Db.ProfessionalPtFees.Remove(feeObject);
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
        #endregion service connect pt fees

        #region service connect pro fees
        public void GetServiceConnectedProFees(long serviceId)
        {
            try
            {
                var connectedPtFees = Db.VServiceProfessionalProFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProFeeId).ToList();

                var result = (from f in Db.ProFee
                              join
                                spro in Db.VServiceProfessionalProFees on f.Id equals spro.ProFeeId
                              where connectedPtFees.Contains(f.Id)
                              select new
                              {
                                  ProFeeId = f.Id,
                                  ProFeeName = f.FeeName,
                                  ProFeeA1 = f.A1,
                                  ProFeeA2 = f.A2,
                                  spro.ServiceId,
                                  spro.ProfessionalId,
                                  Professionals = string.Join(", ", f.VServiceProfessionalProFees.Where(x => x.ServiceId == serviceId && x.ProFeeId == f.Id).Select(x => x.ServiceProfessionals.Professional.Name).Distinct().ToList()),
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
                var connectedPtFees = Db.VServiceProfessionalProFees.Where(x => x.ServiceId == serviceId).Select(x => x.ProFeeId).ToList();

                var result = (from f in Db.ProFee
                              join
                                spt in Db.VServiceProfessionalProFees on f.Id equals spt.ProFeeId
                              where !connectedPtFees.Contains(f.Id)
                              select new
                              {
                                  ProFeeId = f.Id,
                                  ProFeeName = f.FeeName,
                                  ProFeeA1 = f.A1,
                                  ProFeeA2 = f.A2,
                                  spt.ServiceId,
                                  spt.ProfessionalId,
                                  Professionals = string.Join(", ", f.VServiceProfessionalProFees.Where(x => x.ServiceId != serviceId && x.ProFeeId == f.Id).Select(x => x.ServiceProfessionals.Professional.Name).Distinct().ToList()),
                                  Services = string.Join(", ", f.VServiceProfessionalProFees.Where(x => x.ServiceId != serviceId && x.ProFeeId == f.Id).Select(x => x.ServiceProfessionals.Service.Name).Distinct().ToList()),
                                  Tags = string.Join(", ", f.VServiceProfessionalProFees.Where(x => x.ServiceId != serviceId && x.ProFeeId == f.Id).Select(x => x.ServiceProfessionals.Service.Tags).Distinct().ToList()),
                              }).DistinctBy(x => x.ProFeeId).ToList();

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
                    if (model.ProFeeId > 0)
                    {
                        var serviceProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == serviceId).Select(x => x.ProfessionalId).ToList();
                        foreach (var pro in serviceProfessionals)
                        {
                            var exists = Db.ProfessionalProFees.FirstOrDefault(x => x.ProFeeId == model.ProFeeId && x.ProfessionalId == pro);
                            if (exists is null)
                            {
                                Db.ProfessionalProFees.Add(new ProfessionalProFees { ProfessionalId = pro, ProFeeId = model.ProFeeId });
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
        public void DetachProFeeFromService(IEnumerable<ServiceConnectedProFeesModel> models, long serviceId)
        {
            try
            {
                foreach (var model in models)
                {
                    if (model.ProFeeId > 0)
                    {
                        var serviceProfessionals = Db.VServiceProfessionalProFees.Where(x => x.ServiceId == serviceId && x.ProFeeId == model.ProFeeId).Select(x => x.ProfessionalId).ToList();
                        foreach (var pro in serviceProfessionals)
                        {
                            var feeObject = Db.ProfessionalProFees.FirstOrDefault(x => x.ProFeeId == model.ProFeeId && x.ProfessionalId == pro);
                            if (feeObject != null)
                            {
                                Db.ProfessionalProFees.Remove(feeObject);
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
        #endregion service connect pt fees



        public dynamic GetProfessionalServicesWithInclude(long professionalId)
        {
            return Db.ServiceProfessionals.Where(x => x.ProfessionalId == professionalId)
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
                    }).DistinctBy(x=> x.Customer).ToList();
        }

        public dynamic GetConnectedBookings(long serviceId)
        {
            return (from ps in Db.ServiceProfessionals
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
                        Professional = string.Join(", ", ls.Service.ServiceProfessionals.Select(x => x.Professional.Name).ToArray())
                    }
                ).ToList();
        }

    }
}
