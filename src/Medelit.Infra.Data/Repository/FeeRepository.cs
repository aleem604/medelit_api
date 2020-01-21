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

            return Db.Service.Where(x => x.PTFeeId == feeId || x.PROFeeId == feeId).Select(x => new
            {
                service = x.Name,
                x.PTFeeId,
                x.FieldId,
                Field = x.Field.Field,
                SubCategory = x.SubCategory.SubCategory

            }).ToList();
        }

        public dynamic GetConnectedProfessionalsCustomers(long feeId)
        {
            var fee = Db.Fee.Find(feeId);

            if (fee.FeeTypeId == eFeeType.PTFee)
            {
                return (from s in Db.Service
                        join
                        ps in Db.ServiceProfessionalRelation on s.Id equals ps.ServiceId
                        join
                        p in Db.Professional on ps.ProfessionalId equals p.Id
                        join
                        f in Db.Fee on s.PTFeeId equals f.Id
                        where f.Id == feeId
                        select new
                        {
                            f.Id,
                            f.FeeTypeId,
                            feeType = f.FeeTypeId.GetDescription(),
                            f.FeeName,
                            SName = s.Name,
                            ProName = p.Name,
                            feeA1 = s.PtFee.A1,
                            feeA2 = s.PtFee.A2
                        }).ToList();
            }
            else
            {
                return (from s in Db.Service
                        join
                        ps in Db.ServiceProfessionalRelation on s.Id equals ps.ServiceId
                        join
                        p in Db.Professional on ps.ProfessionalId equals p.Id
                        join
                        f in Db.Fee on s.PROFeeId equals f.Id
                        where f.Id == feeId
                        select new
                        {
                            feeId = f.Id,
                            f.FeeTypeId,
                            feeType = f.FeeTypeId.GetDescription(),
                            f.FeeName,
                            SName = s.Name,
                            ProName = p.Name,
                            feeA1 = s.ProFee.A1,
                            feeA2 = s.ProFee.A2
                        }).ToList();
            }

        }

        public dynamic GetServicesToConnectWithFee(long feeId)
        {
            var fee = Db.Fee.Find(feeId);

            if (fee.FeeTypeId == eFeeType.PTFee)
            {
                return (from s in Db.Service
                        where s.PTFeeId != feeId
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
                        where s.PROFeeId != feeId
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
                        newService.PTFeeId = feeId;
                    }
                    else
                    {
                        newService.PROFeeId = feeId;
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
    }
}
