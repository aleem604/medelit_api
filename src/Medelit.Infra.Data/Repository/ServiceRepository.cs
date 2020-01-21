using Medelit.Common;
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
    public class ServiceRepository :  Repository<Service>, IServiceRepository
    {
        private readonly IMediatorHandler _bus;
        public ServiceRepository(MedelitContext context, IMediatorHandler bus, IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {
            _bus = bus;
        }

        public IQueryable<ServiceProfessionalRelation> GetServiceProfessionals()
        {
            return Db.ServiceProfessionalRelation;
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
                            service.PTFeeId = model.PtFeeId;
                            service.PROFeeId = model.ProFeeId;
                            Db.Service.Update(service);
                            Db.SaveChanges();
                            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, "service updated successfully"));
                            return;
                        }
                    }
                    else
                    {
                        var ptFeeModel = new Fee
                        {
                            FeeName = model.PtFeeName,
                            A1 = model.PtFeeA1,
                            A2 = model.PtFeeA2,
                            Tags = model.PtFeeTags,
                            AssignedToId = CurrentUser.Id,
                            CreateDate = DateTime.UtcNow,
                            CreatedById = CurrentUser.Id
                        };

                        var proFeeModel = new Fee
                        {
                            FeeName = model.ProFeeName,
                            A1 = model.ProFeeA1,
                            A2 = model.ProFeeA2,
                            Tags = model.ProFeeTags,
                            AssignedToId = CurrentUser.Id,
                            CreateDate = DateTime.UtcNow,
                            CreatedById = CurrentUser.Id
                        };

                        Db.Fee.Add(ptFeeModel);
                        Db.Fee.Add(proFeeModel);
                        Db.SaveChanges();
                        if(ptFeeModel.Id > 0 && proFeeModel.Id >0)
                        {
                            service.PTFeeId = ptFeeModel.Id;
                            service.PROFeeId = proFeeModel.Id;

                            ptFeeModel.FeeCode = ptFeeModel.FeeTypeId == eFeeType.PTFee ? $"FP{ptFeeModel.Id.ToString().PadLeft(6, '0')}" : $"FS{ptFeeModel.Id.ToString().PadLeft(6, '0')}";
                            proFeeModel.FeeCode = proFeeModel.FeeTypeId == eFeeType.PTFee ? $"FP{proFeeModel.Id.ToString().PadLeft(6, '0')}" : $"FS{proFeeModel.Id.ToString().PadLeft(6, '0')}";

                            ptFeeModel.UpdateDate = DateTime.UtcNow;
                            ptFeeModel.UpdatedById = CurrentUser.Id;

                            proFeeModel.UpdateDate = DateTime.UtcNow;
                            proFeeModel.UpdatedById = CurrentUser.Id;

                            Db.Fee.Update(ptFeeModel);
                            Db.Fee.Update(proFeeModel);
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
            }catch(Exception ex)
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
            var professionals = Db.ServiceProfessionalRelation.Where(x => x.ServiceId == serviceId).ToList();
            Db.RemoveRange(professionals);
            Db.SaveChanges();

        }

        public dynamic GetProfessionalServices(long proId, long? fieldId, long? categoryId, string tag)
        {
            var proServices = Db.ServiceProfessionalRelation.Where(x => x.ProfessionalId == proId).Select(x => x.Id).ToList();
            var existingServiceIds = Db.ServiceProfessionalRelation.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();

            var query = Db.Service.Include(x => x.Field).Include(c => c.SubCategory).Include(x => x.PtFee).Include(x => x.ProFee)
                .Where(x => !existingServiceIds.Contains(x.Id))
                .Select((s) => new
                {
                    s.Id,
                    s.Name,
                    s.PTFeeId,
                    ptFeeName = s.PtFee.FeeName,
                    ptFeeA1 = s.PtFee.A1,
                    ptFeeA2 = s.PtFee.A2,
                    s.PROFeeId,
                    proFeeName = s.ProFee.FeeName,
                    proFeeA1 = s.ProFee.A1,
                    proFeeA2 = s.ProFee.A2,
                    s.FieldId,
                    s.Field,
                    s.SubcategoryId,
                    s.SubCategory,
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
            Db.ServiceProfessionalRelation.Add(new ServiceProfessionalRelation { ServiceId = serviceId, ProfessionalId = professionalId });
        }

        public void DetachProfessional(long serviceId, long professionalId)
        {
            var relation = Db.ServiceProfessionalRelation.FirstOrDefault(x => x.ServiceId == serviceId && x.ProfessionalId == professionalId);
            Db.ServiceProfessionalRelation.Remove(relation);
        }

        public dynamic GetProfessionalServicesWithInclude(long professionalId)
        {
            return Db.ServiceProfessionalRelation.Where(x => x.ProfessionalId == professionalId)
                .Include(x => x.Service).ThenInclude(x => x.PtFee)
                .Include(x => x.Service).ThenInclude(x => x.ProFee)
                .Include(x => x.Service).ThenInclude(x => x.Field)
                .Include(x => x.Service).ThenInclude(x => x.SubCategory)
                .ToList();
        }

        public dynamic GetProfessionalFeesDetail(long serviceId)
        {
            return Db.ServiceProfessionalRelation.Where(x => x.ServiceId == serviceId).Select((x) => new
            {
                x.Service.Id,
                x.Service.Name,
                proId = x.Professional.Id,
                proName = x.Professional.Name,
                ptFeeId = x.Service.PtFee.Id,
                ptFeeName = x.Service.PtFee.FeeName,
                ptFeeA1 = x.Service.PtFee.A1,
                ptFeeA2 = x.Service.PtFee.A2,

                proFeeId = x.Service.ProFee.Id,
                proFeeName = x.Service.ProFee.FeeName,
                proFeeA1 = x.Service.ProFee.A1,
                proFeeA2 = x.Service.ProFee.A2,

            }).ToList();
        }

        public dynamic GetServiceConnectedProfessionals(long serviceId)
        {
            var constractStatus = Db.StaticData.Select((s) => new { Id = s.Id, Value = s.ContractStatus }).Where(x => x.Value != null).ToList();

            return Db.ServiceProfessionalRelation.Where(x => x.ServiceId == serviceId).Select((s) => new
            {
                s.Service.Id,
                name = s.Professional.Name,
                phone = s.Professional.Telephone,
                email = s.Professional.Email,
                status = constractStatus.FirstOrDefault(x => x.Id == s.Professional.ContractStatusId).Value
            }).ToList();

        }

        public dynamic GetConnectedCustomersInvoicingEntities(long serviceId)
        {
            return (from c in Db.Customer
                    join
                    cs in Db.CustomerServiceRelation on c.Id equals cs.CustomerId
                    where cs.ServiceId == serviceId
                    select new
                    {
                        c.Name,
                        InvoiceEntity = c.InvoiceEntityId.HasValue ? c.InvoiceEntity.Name : string.Empty,
                        c.MainPhone,
                        c.Email
                    }).ToList();
        }

        public dynamic GetConnectedBookings(long serviceId)
        {
            return (from ps in Db.ServiceProfessionalRelation
                    join
                    b in Db.Booking on ps.ServiceId equals b.ServiceId
                    where ps.ServiceId == serviceId
                    select new
                    {
                        bookingName = b.Name,
                        ServiceName = ps.Service.Name,
                        PtFee = ps.Service.PtFee.FeeName,
                        PtFeeA1 = ps.Service.PtFee.A1,
                        ptFeeA2 = ps.Service.PtFee.A2,

                        ProFeeName = ps.Service.ProFee.FeeName,
                        ProFeeA1 = ps.Service.ProFee.A1,
                        proFeeA2 = ps.Service.ProFee.A2,

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
