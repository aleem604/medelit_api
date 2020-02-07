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
        private readonly IMediatorHandler _bus;
        public FeeRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        {
            _bus = bus;
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
                    var exist = Db.ProfessionalPtFees.FirstOrDefault(x => x.PtFeeId == ptFee.Id && x.ProfessionalId == professionalId);
                    if (exist is null)
                    {
                        Db.ProfessionalPtFees.Add(new ProfessionalPtFees { PtFeeId = ptFee.Id, ProfessionalId = professionalId });
                        Db.SaveChanges();
                    }
                }

                foreach (var proFee in proFees)
                {
                    var exist = Db.ProfessionalProFees.FirstOrDefault(x => x.ProFeeId == proFee.Id && x.ProfessionalId == professionalId);
                    if (exist is null)
                    {
                        Db.ProfessionalProFees.Add(new ProfessionalProFees { ProFeeId = proFee.Id, ProfessionalId = professionalId });
                        Db.SaveChanges();
                    }
                }

                var professional = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (professional is null)
                {
                    Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = professionalId });
                    Db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetConnectedServices(long feeId, eFeeType feeType = eFeeType.PTFee)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var proIds = Db.ProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToList();
                    var serviceIds = Db.ServiceProfessionals.Where(x => proIds.Contains(x.ProfessionalId)).Select(x => x.ServiceId).ToList();

                    var result = (from s in Db.Service
                                  where serviceIds.Contains(s.Id)
                                  select new
                                  {
                                      s.Id,
                                      cService = s.Name,
                                      cField = s.Field.Field,
                                      cSubcategory = s.SubCategory.SubCategory
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var proIds = Db.ProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToList();
                    var serviceIds = Db.ServiceProfessionals.Where(x => proIds.Contains(x.ProfessionalId)).Select(x => x.ServiceId).ToList();

                    var result = (from s in Db.Service
                                  where serviceIds.Contains(s.Id)
                                  select new
                                  {
                                      s.Id,
                                      cService = s.Name,
                                      cField = s.Field.Field,
                                      cSubcategory = s.SubCategory.SubCategory
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

        public void DeleteConnectedProfessionals(IEnumerable<FeeConnectedProfessionalsViewModel> feeConnectedProfessionals, long feeId, eFeeType feeType)
        {
            
            try
            {
                if (feeConnectedProfessionals.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys found to delete."));
                    return;
                }

                if (feeType == eFeeType.PTFee)
                {
                    foreach (var row in feeConnectedProfessionals)
                    {
                        var ptRows = Db.ProfessionalPtFees.Where(x => x.ProfessionalId == row.ProfessionalId && x.PtFeeId == feeId).ToList();
                        Db.ProfessionalPtFees.RemoveRange(ptRows);
                        Db.SaveChanges();

                        var ptProfRow = Db.ServiceProfessionals.FirstOrDefault(x => x.ProfessionalId == row.ProfessionalId);
                        if(ptProfRow is null)
                        {
                            var serveicProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == row.ServiceId && x.ProfessionalId == row.ProfessionalId).ToList();
                            Db.ServiceProfessionals.RemoveRange(serveicProfessionals);
                            Db.SaveChanges();
                        }
                    }                 
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
                }
                else
                {

                    foreach (var row in feeConnectedProfessionals)
                    {
                        var proRows = Db.ProfessionalProFees.Where(x => x.ProfessionalId == row.ProfessionalId && x.ProFeeId == feeId).ToList();
                        Db.ProfessionalProFees.RemoveRange(proRows);
                        Db.SaveChanges();

                        var proProfRow = Db.ServiceProfessionals.FirstOrDefault(x => x.ProfessionalId == row.ProfessionalId);
                        if (proProfRow is null)
                        {
                            var serveicProfessionals = Db.ServiceProfessionals.Where(x => x.ServiceId == row.ServiceId && x.ProfessionalId == row.ProfessionalId).ToList();
                            Db.ServiceProfessionals.RemoveRange(serveicProfessionals);
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

        public dynamic GetServicesToConnectWithFee(long feeId)
        {
            var fee = Db.PtFee.Find(feeId);

            if (fee.FeeTypeId == eFeeType.PTFee)
            {
                return (from s in Db.Service
                            //where s.PTFeeId != feeId
                        select new
                        {
                            s.Id,
                            serviceName = s.Name,
                            professionals = string.Join(", ", s.ServiceProfessionals.Select(x => x.Professional.Name).ToArray()),
                        }).ToList();
            }
            else
            {
                return (from s in Db.Service
                            //where s.PROFeeId != feeId
                        select new
                        {
                            s.Id,

                            serviceName = s.Name,
                            professionals = string.Join(", ", s.ServiceProfessionals.Select(x => x.Professional.Name).ToArray()),
                        }).ToList();
            }
        }

        public void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId)
        {
            var fee = Db.PtFee.Find(feeId);

            if (serviceIds.Count() > 0)
            {
                var services = Db.Service.Where(x => serviceIds.Contains(x.Id)).Include(x => x.ServiceProfessionals).ToList();


                foreach (var service in services)
                {
                    var newService = service.Clone();
                    if (fee.FeeTypeId == eFeeType.PTFee)
                    {
                        //newService.PTFeeId = feeId;
                    }
                    else
                    {
                        //newService.PROFeeId = feeId;
                    }
                    newService.ServiceProfessionals = CloneServiceProfessionals(service.ServiceProfessionals);

                    newService.CreateDate = DateTime.UtcNow;
                    newService.CreatedById = CurrentUser.Id;
                    newService.AssignedToId = CurrentUser.Id;

                    Db.Service.Add(newService);
                }
                var ids = Db.SaveChanges();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, ids));
            }
            else
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, "No services ids found to save"));
            }
        }

        private List<ServiceProfessionals> CloneServiceProfessionals(IEnumerable<ServiceProfessionals> serviceProfessionals)
        {
            var result = new List<ServiceProfessionals>();
            foreach (var sr in serviceProfessionals)
            {
                var obj = new ServiceProfessionals
                {
                    ProfessionalId = sr.ProfessionalId
                };
                result.Add(obj);
            }
            return result;
        }

        public void GetConnectedProfessionals(long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var proIds = Db.ProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToArray();
                    var services = Db.ServiceProfessionals.Where(x => proIds.Contains(x.ProfessionalId)).Select(x => x.ServiceId).Distinct().ToArray();

                    var result = (from sp in Db.ServiceProfessionals
                                  join
                                    p in Db.Professional on sp.ProfessionalId equals p.Id
                                  where proIds.Contains(p.Id)
                                  select new
                                  {
                                      rowId = sp.Id,
                                      p.Id,
                                      professionalId = p.Id,
                                      sp.ServiceId,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var proIds = Db.ProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToArray();
                    var services = Db.ServiceProfessionals.Where(x => proIds.Contains(x.ProfessionalId)).Select(x => x.ServiceId).Distinct().ToArray();

                    var result = (from sp in Db.ServiceProfessionals
                                  join
                                    p in Db.Professional on sp.ProfessionalId equals p.Id
                                  where proIds.Contains(p.Id)
                                  select new
                                  {
                                      rowId = sp.Id,
                                      p.Id,
                                      professionalId = p.Id,
                                      sp.ServiceId,
                                      pName = p.Name,
                                      pCity = p.City.Value,
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
                    var proIds = Db.ProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToList();

                    var result = (from sp in Db.ServiceProfessionals
                                  join
                                    p in Db.Professional on sp.ProfessionalId equals p.Id
                                  where !proIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = sp.Service.Name,
                                      cField = sp.Service.Field.Field,
                                      cSubcategory = sp.Service.SubCategory.SubCategory
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var proIds = Db.ProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToList();

                    var result = (from sp in Db.ServiceProfessionals
                                  join
                                    p in Db.Professional on sp.ProfessionalId equals p.Id
                                  where !proIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
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
            }
        }

        public void SaveProfessionlToConnectWithFee(IEnumerable<long> proIds, long feeId, eFeeType feeType)
        {
            try
            {
                if (proIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys were passed to api."));
                    return;
                }
                proIds = proIds.Distinct().ToList();

                if (feeType == eFeeType.PTFee)
                {
                    foreach (var pid in proIds)
                    {
                        var exists = Db.ProfessionalPtFees.FirstOrDefault(x => x.PtFeeId == feeId && x.ProfessionalId == pid);
                        if (exists is null)
                        {
                            Db.ProfessionalPtFees.Add(new ProfessionalPtFees { PtFeeId = feeId, ProfessionalId = pid });
                            Db.SaveChanges();
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
                else
                {
                    foreach (var pid in proIds)
                    {
                        var exists = Db.ProfessionalProFees.FirstOrDefault(x => x.ProFeeId == feeId && x.ProfessionalId == pid);
                        if (exists is null)
                        {
                            Db.ProfessionalProFees.Add(new ProfessionalProFees { ProFeeId = feeId, ProfessionalId = pid });
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
                    var existingProfessionals = (from sp in Db.ServiceProfessionals
                                                 join
                                                  pp in Db.ProfessionalPtFees on sp.ProfessionalId equals pp.ProfessionalId
                                                 where pp.PtFeeId == feeId && sp.ServiceId == serviceId
                                                 select sp.ProfessionalId).ToList();

                    var result = Db.Professional.Where(x => !existingProfessionals.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var existingProfessionals = (from sp in Db.ServiceProfessionals
                                                 join
                                                  pp in Db.ProfessionalProFees on sp.ProfessionalId equals pp.ProfessionalId
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
                var exists = Db.ProfessionalPtFees.FirstOrDefault(x => x.ProfessionalId == professionalId && x.PtFeeId == feeId);
                if (exists is null)
                {
                    Db.ProfessionalPtFees.Add(new ProfessionalPtFees { ProfessionalId = professionalId, PtFeeId = feeId });
                }

                var relation = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (relation is null)
                {
                    Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = professionalId });
                }

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                return;
            }
            else
            {
                var exists = Db.ProfessionalProFees.FirstOrDefault(x => x.ProfessionalId == professionalId && x.ProFeeId == feeId);
                if (exists is null)
                {
                    Db.ProfessionalProFees.Add(new ProfessionalProFees { ProfessionalId = professionalId, ProFeeId = feeId });
                }

                var relation = Db.ServiceProfessionals.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
                if (relation is null)
                {
                    Db.ServiceProfessionals.Add(new ServiceProfessionals { ServiceId = serviceId, ProfessionalId = professionalId });
                }

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                return;
            }

        }

    }
}
