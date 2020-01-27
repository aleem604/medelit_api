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
            : base(context, contextAccessor)
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

        public void GetConnectedServices(long feeId, eFeeType feeType = eFeeType.PTFee)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var fee = Db.PtFee.Find(feeId);
                    var serviceIds = Db.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ServiceId).Distinct().ToArray();

                    var result = (from s in Db.Service
                                  where s.ServiceProfessionalPtFees.All(x => serviceIds.Contains(x.ServiceId))
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
                    var fee = Db.ProFee.Find(feeId);
                    var serviceIds = Db.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ServiceId).Distinct().ToArray();

                    var result = (from s in Db.Service
                                  where s.ServiceProfessionalProFees.All(x => serviceIds.Contains(x.ServiceId))
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

        public dynamic GetConnectedProfessionalsCustomers(long feeId)
        {
            var fee = Db.PtFee.Find(feeId);

            if (fee.FeeTypeId == eFeeType.PTFee)
            {
                return (from p in Db.Professional
                        join
                        sp in Db.ServiceProfessionalPtFees on p.Id equals sp.ProfessionalId
                        //where sp.Service.PTFeeId == feeId
                        select new
                        {
                            p.Id,
                            pName = p.Name,
                            pCity = p.CityId > 0 ? p.City.Value : string.Empty,
                            service = sp.Service.Name,
                            field = sp.Service.Field.Code,
                            subCategory = sp.Service.SubCategory.SubCategory
                        }).Distinct().ToList();
            }
            else
            {
                return (from p in Db.Professional
                        join
                        sp in Db.ServiceProfessionalPtFees on p.Id equals sp.ProfessionalId
                        //where sp.Service.PROFeeId == feeId
                        select new
                        {
                            p.Id,
                            pName = p.Name,
                            pCity = p.CityId > 0 ? p.City.Value : string.Empty,
                            service = sp.Service.Name,
                            field = sp.Service.Field.Code,
                            subCategory = sp.Service.SubCategory.SubCategory
                        }).Distinct().ToList();
            }

        }

        public void DeleteConnectedProfessionals(IEnumerable<long> prosIds, long feeId, eFeeType feeType)
        {
            try
            {
                if (prosIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys found to delete."));
                    return;
                }

                if (feeType == eFeeType.PTFee)
                {
                    var fee = Db.PtFee.Find(feeId);
                    var servicesIds = Db.ServiceProfessionalPtFees
                                    .Where(x => prosIds.Contains(x.ProfessionalId) && x.PtFeeId == feeId)
                                    .ToList();

                    Db.ServiceProfessionalPtFees.RemoveRange(servicesIds);
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                }
                else
                {
                    var fee = Db.ProFee.Find(feeId);
                    var servicesIds = Db.ServiceProfessionalProFees
                                    .Where(x => prosIds.Contains(x.ProfessionalId) && x.ProFeeId == feeId)
                                    .ToList();

                    Db.ServiceProfessionalProFees.RemoveRange(servicesIds);
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
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
                            professionals = string.Join(", ", s.ServiceProfessionalPtFees.Select(x => x.Professional.Name).ToArray()),
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
                            professionals = string.Join(", ", s.ServiceProfessionalPtFees.Select(x => x.Professional.Name).ToArray()),
                        }).ToList();
            }
        }

        public void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId)
        {
            var fee = Db.PtFee.Find(feeId);

            if (serviceIds.Count() > 0)
            {
                var services = Db.Service.Where(x => serviceIds.Contains(x.Id)).Include(x => x.ServiceProfessionalPtFees).ToList();


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
                    newService.ServiceProfessionalPtFees = CloneServiceProfessionals(service.ServiceProfessionalPtFees);

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

        private List<ServiceProfessionalPtFees> CloneServiceProfessionals(IEnumerable<ServiceProfessionalPtFees> serviceProfessionals)
        {
            var result = new List<ServiceProfessionalPtFees>();
            foreach (var sr in serviceProfessionals)
            {
                var obj = new ServiceProfessionalPtFees
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
                    var pfeeIds = Db.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToArray();
                    var result = (from p in Db.Professional
                                  where pfeeIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.Service.Name).ToArray()),
                                      cField = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.Service.Field.Field).Distinct().ToArray()),
                                      cSubcategory = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.Service.SubCategory.SubCategory).Distinct().ToArray())
                                  }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                    return;
                }
                else
                {
                    var pfeeIds = Db.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ProfessionalId).Distinct().ToArray();
                    var result = (from p in Db.Professional
                                  where pfeeIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.Service.Name).ToArray()),
                                      cField = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.Service.Field.Field).Distinct().ToArray()),
                                      cSubcategory = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.Service.SubCategory.SubCategory).Distinct().ToArray())
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
                    var pfIds = Db.ServiceProfessionalPtFees.Where(x => x.PtFeeId != feeId).Select(x => x.ProfessionalId).Distinct().ToList();

                    var result = (from p in Db.Professional
                                  where pfIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId != feeId).Select(x => x.Service.Name).ToArray()),
                                      cField = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId != feeId).Select(x => x.Service.Field.Field).Distinct().ToArray()),
                                      cSubcategory = string.Join(", ", p.ServiceProfessionalPtFees.Where(x => x.PtFeeId != feeId).Select(x => x.Service.SubCategory.SubCategory).Distinct().ToArray()),
                                  })
                              //.Where(x => !string.IsNullOrEmpty(x.cService))
                              .ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var pfIds = Db.ServiceProfessionalProFees.Where(x => x.ProFeeId != feeId).Select(x => x.ProfessionalId).Distinct().ToList();

                    var result = (from p in Db.Professional
                                  where pfIds.Contains(p.Id)
                                  select new
                                  {
                                      p.Id,
                                      pName = p.Name,
                                      pCity = p.City.Value,
                                      cService = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId != feeId).Select(x => x.Service.Field.Field).Distinct().ToArray()),
                                      cField = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId != feeId).Select(x => x.Service.Field.Field).Distinct().ToArray()),
                                      cSubcategory = string.Join(", ", p.ServiceProfessionalProFees.Where(x => x.ProFeeId != feeId).Select(x => x.Service.SubCategory.SubCategory).Distinct().ToArray()),
                                  })
                              //.Where(x => !string.IsNullOrEmpty(x.cService))
                              .ToList();

                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
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
                    var fee = Db.PtFee.Find(feeId);
                    foreach (var pid in proIds)
                    {
                        var services = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == pid && x.PtFeeId != feeId).Select(x => x.ServiceId).ToArray();
                        foreach (var sid in services)
                        {
                            var pf = new ServiceProfessionalPtFees
                            {
                                ServiceId = sid,
                                ProfessionalId = pid,
                                PtFeeId = feeId
                            };
                            if (Db.ServiceProfessionalPtFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == pid && x.PtFeeId == feeId) is null)
                            {
                                Db.ServiceProfessionalPtFees.Add(pf);
                                Db.SaveChanges();
                            }
                        }
                    }
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
                else
                {
                    var fee = Db.ProFee.Find(feeId);
                    foreach (var pid in proIds)
                    {
                        var services = Db.ServiceProfessionalProFees.Where(x => x.ProfessionalId == pid && x.ProFeeId != feeId).Select(x => x.ServiceId).ToArray();
                        foreach (var sid in services)
                        {
                            var pf = new ServiceProfessionalProFees
                            {
                                ServiceId = sid,
                                ProfessionalId = pid,
                                ProFeeId = feeId
                            };
                            if (Db.ServiceProfessionalProFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == pid && x.ProFeeId == feeId) is null)
                            {
                                Db.ServiceProfessionalProFees.Add(pf);
                                Db.SaveChanges();
                            }
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
                if (feeType == eFeeType.PTFee)
                {
                    var existing = Db.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId).Select(x => x.ServiceId);
                    var result = Db.Service.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var existing = Db.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId).Select(x => x.ServiceId);
                    var result = Db.Service.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetProfessionalToConnect(long serviceId, long feeId, eFeeType feeType)
        {
            try
            {
                if (feeType == eFeeType.PTFee)
                {
                    var existing = Db.ServiceProfessionalPtFees.Where(x => x.PtFeeId == feeId && x.ServiceId == serviceId).Select(x => x.ProfessionalId);
                    var result = Db.Professional.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
                }
                else
                {
                    var existing = Db.ServiceProfessionalProFees.Where(x => x.ProFeeId == feeId && x.ServiceId == serviceId).Select(x => x.ProfessionalId);
                    var result = Db.Professional.Where(x => !existing.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
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
                var exists = Db.ServiceProfessionalPtFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId && x.PtFeeId == feeId);
                if (exists is null)
                {
                    Db.ServiceProfessionalPtFees.Add(new ServiceProfessionalPtFees { ServiceId = serviceId, ProfessionalId = professionalId, PtFeeId = feeId });
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                    return;
                }
            }
            else
            {
                var exists = Db.ServiceProfessionalProFees.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId && x.ProFeeId == feeId);
                if (exists is null)
                {
                    Db.ServiceProfessionalProFees.Add(new ServiceProfessionalProFees { ServiceId = serviceId, ProfessionalId = professionalId, ProFeeId = feeId });
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
                }
            }

        }

    }
}
