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
    public class FeeRepository : Repository<Fee>, IFeeRepository
    {
        private readonly IMediatorHandler _bus;
        public FeeRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor)
        {
            _bus = bus;
        }

        public dynamic GetConnectedServices(long feeId)
        {
            var fee = Db.Fee.Find(feeId);
            var feeProIds = Db.ProfessionalFees.Where(x => x.FeeId == feeId && x.FeeType == fee.FeeTypeId).Select(x => x.ProfessionalId).ToArray();

            return (from s in Db.Service
                    where s.ServiceProfessionals.Any(x => feeProIds.Contains(x.ProfessionalId))
                    select new
                    {
                        s.Id,
                        cService = s.Name,
                        cField = s.Field.Field,
                        cSubcategory = s.SubCategory.SubCategory
                    }).ToList();

        }

        public dynamic GetConnectedProfessionals(long feeId)
        {
            var fee = Db.Fee.Find(feeId);
            var pfeeIds = Db.ProfessionalFees.Where(x => x.FeeId == feeId && x.FeeType == fee.FeeTypeId).Select(x => x.ProfessionalId).Distinct().ToArray();
            return (from p in Db.Professional
                    where pfeeIds.Contains(p.Id)
                    select new
                    {
                        p.Id,
                        pName = p.Name,
                        pCity = p.City.Value,
                        cService = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.Name).ToArray()),
                        cField = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.Field.Field).ToArray()),
                        cSubcategory = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.SubCategory.SubCategory).ToArray())
                    }).ToList();

        }

        public dynamic GetConnectedProfessionalsCustomers(long feeId)
        {
            var fee = Db.Fee.Find(feeId);

            if (fee.FeeTypeId == eFeeType.PTFee)
            {
                return (from p in Db.Professional
                        join
                        sp in Db.ServiceProfessionalRelation on p.Id equals sp.ProfessionalId
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
                        sp in Db.ServiceProfessionalRelation on p.Id equals sp.ProfessionalId
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

        public void DeleteConnectedProfessionals(IEnumerable<long> prosIds, long feeId)
        {
            try
            {
                if (prosIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys found to delete."));
                    return;
                }
                var fee = Db.Fee.Find(feeId);
               
                var servicesIds = Db.ProfessionalFees
                                .Where(x => prosIds.Contains(x.ProfessionalId) && x.FeeId == feeId && x.FeeType == fee.FeeTypeId)
                                .ToList();

                Db.ProfessionalFees.RemoveRange(servicesIds);
                var changes = Db.SaveChanges();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, changes));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public dynamic GetServicesToConnectWithFee(long feeId)
        {
            var fee = Db.Fee.Find(feeId);

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
            var fee = Db.Fee.Find(feeId);

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

        private List<ServiceProfessionalRelation> CloneServiceProfessionals(IEnumerable<ServiceProfessionalRelation> serviceProfessionals)
        {
            var result = new List<ServiceProfessionalRelation>();
            foreach (var sr in serviceProfessionals)
            {
                var obj = new ServiceProfessionalRelation
                {
                    ProfessionalId = sr.ProfessionalId
                };
                result.Add(obj);
            }
            return result;
        }

        public void GetProfessionalToConnectWithFee(long feeId)
        {
            try
            {
                var fee = Db.Fee.Find(feeId);
                var result = new object();
                var pfIds = Db.ProfessionalFees.Where(x => x.FeeId == feeId && x.FeeType == fee.FeeTypeId).Select(x => x.ProfessionalId).ToList();

                result = (from p in Db.Professional
                          where !pfIds.Contains(p.Id)
                          select new
                          {
                              p.Id,
                              pName = p.Name,
                              pCity = p.City.Value,
                              cService = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.Name).ToArray()),
                              cField = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.Field.Field).ToArray()),
                              cSubcategory = string.Join(", ", p.ProfessionalServices.Select(x => x.Service.SubCategory.SubCategory).ToArray())
                          }).ToList();


                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }


        }
        public void SaveProfessionlToConnectWithFee(IEnumerable<long> proIds, long feeId)
        {
            try
            {
                if (proIds.Count() == 0)
                {
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, "No keys were passed to api."));
                    return;
                }
                proIds = proIds.Distinct().ToList();

                var fee = Db.Fee.Find(feeId);


                foreach (var pid in proIds)
                {
                    var pf = new ProfessionalFees
                    {
                        ProfessionalId = pid,
                        FeeId = feeId,
                        FeeType = fee.FeeTypeId
                    };
                    Db.ProfessionalFees.Add(pf);
                }
                var ids = Db.SaveChanges();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, ids));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));

            }
        }
    }
}
