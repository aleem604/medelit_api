using System;
using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class FeeRepository : Repository<VFees>, IFeeRepository
    {

        public FeeRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        {
        }

        public void FindFees(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
                var services = Db.Service;
                var serviceProFees = Db.ServiceProfessionalFees.Select(s => new { s.PtFeeId, s.ProFeeId, s.Service, s.Professional });

                var query = Db.VFees.Where(x => x.Status != eRecordStatus.Deleted).Select((x) => new
                {
                    x.Id,
                    x.FeeName,
                    x.FeeTypeId,
                    FeeType = x.FeeTypeId.GetDescription(),
                    services = string.Join("<br/>", x.FeeTypeId == eFeeType.PTFee ? serviceProFees.Where(w => w.PtFeeId == x.Id).Select(s => s.Service.Name).Distinct().ToList() : serviceProFees.Where(w => w.ProFeeId == x.Id).Select(s => s.Service.Name).Distinct().ToList()),
                    field = string.Join("<br/>", x.FeeTypeId == eFeeType.PTFee ? serviceProFees.Where(w => w.PtFeeId == x.Id).Select(s => s.Service.Field.Field).Distinct().ToList() : serviceProFees.Where(w => w.ProFeeId == x.Id).Select(s => s.Service.Field.Field).Distinct().ToList()),
                    subCategory = string.Join("<br/>", x.FeeTypeId == eFeeType.PTFee ? serviceProFees.Where(w => w.PtFeeId == x.Id).Select(s => s.Service.SubCategory.SubCategory).Distinct().ToList() : serviceProFees.Where(w => w.ProFeeId == x.Id).Select(s => s.Service.SubCategory.SubCategory).Distinct().ToList()),
                    professionals = string.Join("<br/>", x.FeeTypeId == eFeeType.PTFee ? serviceProFees.Where(w => w.PtFeeId == x.Id).Select(s => s.Professional.Name).Distinct().ToList() : serviceProFees.Where(w => w.ProFeeId == x.Id).Select(s => s.Professional.Name).Distinct().ToList()),

                    x.FeeCode,
                    x.Tags,
                    x.A1,
                    x.A2,
                    x.CreateDate,
                    x.UpdateDate
                }); ;

                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.FeeName) && x.FeeName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.FeeCode) && x.FeeCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.FeeType) && x.FeeType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.A1.ToString()) && x.A1.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CreateDate.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.services) && x.services.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.field) && x.field.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.professionals) && x.professionals.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Tags) && x.Tags.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Id.ToString().Contains(viewModel.Filter.Search))

                    ));
                }

                if (viewModel.Filter.FeeType != eFeeType.All)
                {
                    query = query.Where(x => x.FeeTypeId == viewModel.Filter.FeeType);
                }

                switch (viewModel.SortField)
                {
                    case "feeName":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.FeeName);
                        else
                            query = query.OrderByDescending(x => x.FeeName);
                        break;

                    case "feeType":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.FeeType);
                        else
                            query = query.OrderByDescending(x => x.FeeType);
                        break;

                    case "feeCode":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.FeeCode);
                        else
                            query = query.OrderByDescending(x => x.FeeCode);
                        break;
                    case "services":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.services);
                        else
                            query = query.OrderByDescending(x => x.services);
                        break;
                    case "professionals":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.professionals);
                        else
                            query = query.OrderByDescending(x => x.professionals);
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

                    case "tags":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Tags);
                        else
                            query = query.OrderByDescending(x => x.Tags);
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

                    default:
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.FeeCode);
                        else
                            query = query.OrderByDescending(x => x.FeeCode);

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

        public IQueryable<PtFee> GetPtFees()
        {
            return Db.PtFee;
        }

        public IQueryable<ProFee> GetProFees()
        {
            return Db.ProFee;
        }

        public PtFee GetPtFee(long feeId)
        {
            return Db.PtFee.Find(feeId);
        }

        public ProFee GetProFee(long feeId)
        {
            return Db.ProFee.Find(feeId);
        }


        public PtFee AddPtFee(PtFee feeModel)
        {
            Db.PtFee.Add(feeModel);
            Db.SaveChanges();
            return feeModel;
        }

        public ProFee AddProFee(ProFee feeModel)
        {
            Db.ProFee.Add(feeModel);
            Db.SaveChanges();
            return feeModel;
        }

        public PtFee UpdatePtFee(PtFee feeModel)
        {
            Db.PtFee.Update(feeModel);
            Db.SaveChanges();
            return feeModel;
        }

        public ProFee UpdateProFee(ProFee feeModel)
        {
            Db.ProFee.Update(feeModel);
            Db.SaveChanges();
            return feeModel;
        }

        public void RemovePtFee(PtFee feeModel)
        {
            Db.PtFee.Remove(feeModel);
            Db.SaveChanges();
        }
        public void RemoveProFee(ProFee feeModel)
        {
            Db.ProFee.Remove(feeModel);
            Db.SaveChanges();
        }
        public void GetFeeByIdAndType(long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var fee = Db.PtFee.Find(feeId);
                    fee.FeeTypeId = feeType;
                    fee.FeeType = feeType.GetDescription();
                    fee.AssignedTo = CurrentUser.FullName;
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, fee));
                }
                else
                {
                    var fee = Db.ProFee.Find(feeId);
                    fee.FeeTypeId = feeType;
                    fee.FeeType = feeType.GetDescription();
                    fee.AssignedTo = CurrentUser.FullName;
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, fee));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }

        }

        public void ConnectFeesToServiceProfessional(IEnumerable<PtFee> ptFees, IEnumerable<ProFee> proFees, long serviceId, long professionalId)
        {
            try
            {
                if (serviceId == 0 && professionalId == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "Invalid api data."));
                    return;
                }

                foreach (var ptFee in ptFees)
                {
                    var exist = Db.ServiceProfessionalFees.FirstOrDefault(x => x.PtFeeId == ptFee.Id && x.ProfessionalId == professionalId);
                    if (exist is null)
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { PtFeeId = ptFee.Id, ProfessionalId = professionalId });
                        Db.SaveChanges();
                    }
                }

                foreach (var proFee in proFees)
                {
                    var exist = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ProFeeId == proFee.Id && x.ProfessionalId == professionalId);
                    if (exist is null)
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ProFeeId = proFee.Id, ProfessionalId = professionalId });
                        Db.SaveChanges();
                    }
                }

                var professional = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (professional is null)
                {
                    Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = professionalId });
                    Db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetFeeConnectedServices(long feeId, eFeeType feeType = eFeeType.PTFee)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var serviceIds = Db.ServiceProfessionalFees.Where(x => x.PtFeeId == feeId).Select(x => x.ServiceId).Distinct().ToList();
                    var result = (from s in Db.Service
                                  where serviceIds.Contains(s.Id)
                                  select new
                                  {
                                      s.Id,
                                      cService = s.Name,
                                      cField = s.Field.Field,
                                      cSubcategory = s.SubCategory.SubCategory
                                  }).DistinctBy(x => x.Id).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var serviceIds = Db.ServiceProfessionalFees.Where(x => x.ProFeeId == feeId).Select(x => x.ServiceId).Distinct().ToList();
                    var result = (from s in Db.Service
                                  where serviceIds.Contains(s.Id)
                                  select new
                                  {
                                      s.Id,
                                      cService = s.Name,
                                      cField = s.Field.Field,
                                      cSubcategory = s.SubCategory.SubCategory
                                  }).DistinctBy(x => x.Id).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
                return;
            }
        }

        public void DeleteConnectedProfessionals(IEnumerable<FeeConnectedProfessionalsViewModel> feeConnectedProfessionals, long feeId, eFeeType feeType)
        {

            try
            {
                if (feeConnectedProfessionals.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys found to delete."));
                    return;
                }

                foreach (var row in feeConnectedProfessionals)
                {
                    var ptRow = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == row.ServiceId && x.ProfessionalId == row.ProfessionalId);
                    Db.ServiceProfessionalFees.Remove(ptRow);
                    Db.SaveChanges();
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public dynamic GetServicesToConnectWithFee(long feeId, eFeeType feeType)
        {

            if (feeType == eFeeType.PTFee)
            {
                var serviceIds = Db.ServiceProfessionalFees.Where(x => x.PtFeeId == feeId).Select((s) => s.ServiceId).ToList();

                var result = (from s in Db.Service
                              where !serviceIds.Contains(s.Id)
                              select new
                              {
                                  s.Id,
                                  serviceName = s.Name,
                                  cField = s.Field.Field,
                                  cSubcategory = s.SubCategory.SubCategory,
                              })
                              .DistinctBy(x => x.Id)
                              .ToList();
                return result;
            }
            else
            {
                var serviceIds = Db.ServiceProfessionalFees.Where(x => x.ProFeeId == feeId).Select((s) => s.ServiceId).ToList();

                var result = (from s in Db.Service
                              where !serviceIds.Contains(s.Id)
                              select new
                              {
                                  s.Id,
                                  serviceName = s.Name,
                                  cField = s.Field.Field,
                                  cSubcategory = s.SubCategory.SubCategory,
                              })
                              .DistinctBy(x => x.Id)
                              .ToList();
                return result;
            }
        }

        public void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId, eFeeType feeType)
        {
            try
            {
                if (serviceIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys were passed to api."));
                    return;
                }
                serviceIds = serviceIds.Distinct().ToList();

                if (feeType == eFeeType.PTFee)
                {
                    foreach (var sid in serviceIds)
                    {
                        var rowObj = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == sid && x.PtFeeId == feeId);
                        if (rowObj is null)
                        {
                            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = sid, PtFeeId = feeId });
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
                else
                {
                    foreach (var sid in serviceIds)
                    {
                        var rowObj = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == sid && x.ProFeeId == feeId);
                        if (rowObj is null)
                        {
                            Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = sid, ProFeeId = feeId });
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void DeleteConnectedServices(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feedType)
        {
            try
            {
                if (serviceProFeeIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys found to delete."));
                    return;
                }

                if (feedType == eFeeType.PTFee)
                {

                    foreach (var row in serviceProFeeIds)
                    {
                        var ptRows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row && x.PtFeeId == feeId).ToList();
                        foreach (var r in ptRows)
                        {
                            if (r.ProFeeId > 0)
                            {
                                r.PtFeeId = null;
                                Db.ServiceProfessionalFees.Update(r);
                            }
                            else
                            {
                                Db.ServiceProfessionalFees.RemoveRange(r);
                            }
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
                else
                {
                    foreach (var row in serviceProFeeIds)
                    {
                        var ptRows = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row && x.ProFeeId == feeId).ToList();
                        foreach (var r in ptRows)
                        {
                            if (r.PtFeeId > 0)
                            {
                                r.PtFeeId = null;
                                Db.ServiceProfessionalFees.Update(r);
                            }
                            else
                            {
                                Db.ServiceProfessionalFees.RemoveRange(r);
                            }
                            Db.SaveChanges();
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



        public void GetConnectedProfessionals(long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {

                    var result = (from sp in Db.ServiceProfessionalFees

                                  where sp.PtFeeId == feeId
                                  select new
                                  {
                                      rowId = sp.Id,
                                      professionalId = sp.ProfessionalId,
                                      sp.ServiceId,
                                      pName = sp.Professional.Name,
                                      pCity = sp.Professional.City,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var result = (from sp in Db.ServiceProfessionalFees

                                  where sp.ProFeeId == feeId
                                  select new
                                  {
                                      rowId = sp.Id,
                                      professionalId = sp.ProfessionalId,
                                      sp.ServiceId,
                                      pName = sp.Professional.Name,
                                      pCity = sp.Professional.City,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
                return;
            }
        }

        public void GetProfessionalToConnectWithFee(long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var profs = Db.ServiceProfessionalFees.Where(x => x.PtFeeId == feeId).Select((s) => $"{s.ServiceId}--{s.ProfessionalId}").ToList();

                    var result = (from sp in Db.ServiceProfessionalFees
                                  where !profs.Contains($"{sp.ServiceId}--{sp.ProfessionalId}")
                                  select new
                                  {
                                      sp.Id,
                                      sp.ProfessionalId,
                                      sp.ServiceId,
                                      uid = $"{sp.ServiceId}--{sp.ProfessionalId}",
                                      pName = sp.Professional.Name,
                                      pCity = sp.Professional.City,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).DistinctBy(x => x.uid).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var profs = Db.ServiceProfessionalFees.Where(x => x.ProFeeId == feeId).Select((s) => $"{s.ServiceId}--{s.ProfessionalId}").ToList();

                    var result = (from sp in Db.ServiceProfessionalFees
                                  where !profs.Contains($"{sp.ServiceId}--{sp.ProfessionalId}")
                                  select new
                                  {
                                      sp.Id,
                                      sp.ProfessionalId,
                                      sp.ServiceId,
                                      uid = $"{sp.ServiceId}--{sp.ProfessionalId}",
                                      pName = sp.Professional.Name,
                                      pCity = sp.Professional.City,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).DistinctBy(x => x.uid).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void SaveProfessionlToConnectWithFee(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feeType)
        {
            try
            {
                if (serviceProFeeIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys were passed to api."));
                    return;
                }
                serviceProFeeIds = serviceProFeeIds.Distinct().ToList();

                if (feeType == eFeeType.PTFee)
                {
                    foreach (var spids in serviceProFeeIds)
                    {
                        var rowObj = Db.ServiceProfessionalFees.Find(spids);
                        if (rowObj != null)
                        {
                            var obj = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == rowObj.ServiceId && x.ProfessionalId == rowObj.ProfessionalId);
                            if (obj != null)
                            {
                                obj.PtFeeId = feeId;
                                Db.ServiceProfessionalFees.Update(obj);
                            }
                            else
                            {
                                var exists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == rowObj.ServiceId && x.ProfessionalId == rowObj.ProfessionalId && x.PtFeeId == feeId);
                                if (exists is null)
                                    Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = rowObj.ServiceId, ProfessionalId = rowObj.ProfessionalId, PtFeeId = feeId });
                            }
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
                else
                {
                    foreach (var spids in serviceProFeeIds)
                    {
                        var rowObj = Db.ServiceProfessionalFees.Find(spids);
                        if (rowObj != null)
                        {
                            var obj = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == rowObj.ServiceId && x.ProfessionalId == rowObj.ProfessionalId);
                            if (obj != null)
                            {
                                obj.ProFeeId = feeId;
                                Db.ServiceProfessionalFees.Update(obj);
                            }
                            else
                            {
                                var exists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == rowObj.ServiceId && x.ProfessionalId == rowObj.ProfessionalId && x.ProFeeId == feeId);
                                if (exists is null)
                                    Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = rowObj.ServiceId, ProfessionalId = rowObj.ProfessionalId, ProFeeId = feeId });
                            }
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServicesToConnect(long feeId, eFeeType feeType)
        {
            try
            {
                var result = Db.Service.Select(x => new { x.Id, Value = x.Name }).ToList();
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                //if (feeType == eFeeType.PTFee)
                //{
                //    var existing = (from sp in Db.ServiceProfessionals
                //                    join
                //                    fs in Db.ProfessionalPtFees on sp.ProfessionalId equals fs.ProfessionalId
                //                    where fs.PtFeeId == feeId
                //                    select sp.ServiceId).Distinct().ToList();

                //    var result = Db.Service.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                //    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                //}
                //else
                //{
                //    var existing = (from sp in Db.ServiceProfessionals
                //                    join
                //                    fs in Db.ProfessionalPtFees on sp.ProfessionalId equals fs.ProfessionalId
                //                    where fs.PtFeeId == feeId
                //                    select sp.ServiceId).Distinct().ToList();

                //    var result = Db.Service.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                //    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                //}
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetServiceProfessionalsForFilter(long serviceId, long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var existingProfessionals = (from sp in Db.ServiceProfessionalFees
                                                 join
                                                  pp in Db.ServiceProfessionalFees on sp.ProfessionalId equals pp.ProfessionalId
                                                 where pp.PtFeeId == feeId && sp.ServiceId == serviceId
                                                 select sp.ProfessionalId).ToList();

                    var result = Db.Professional.Where(x => !existingProfessionals.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var existingProfessionals = (from sp in Db.ServiceProfessionalFees
                                                 join
                                                  pp in Db.ServiceProfessionalFees on sp.ProfessionalId equals pp.ProfessionalId
                                                 where pp.ProFeeId == feeId && sp.ServiceId == serviceId
                                                 select sp.ProfessionalId).ToList();

                    var result = Db.Professional.Where(x => !existingProfessionals.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void AttachNewServiceProfessionalToFee(long serviceId, long professionalId, long feeId, eFeeType feeType)
        {
            if (feeType == eFeeType.PTFee)
            {
                var exists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (exists is null)
                {
                    Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = professionalId, PtFeeId = feeId });
                }
                else
                {
                    exists.PtFeeId = feeId;
                    Db.ServiceProfessionalFees.Update(exists);
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                return;
            }
            else
            {
                var exists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (exists is null)
                {
                    Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = serviceId, ProfessionalId = professionalId, ProFeeId = feeId });
                }
                else
                {
                    exists.ProFeeId = feeId;
                    Db.ServiceProfessionalFees.Update(exists);
                }

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                return;
            }

        }



    }
}
