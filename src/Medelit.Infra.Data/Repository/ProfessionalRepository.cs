using System;
using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Common.Models;
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
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        private readonly IMediatorHandler _bus;
        public ProfessionalRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor)
        {
            _bus = bus;
        }

        public IEnumerable<ServiceProfessionalPtFees> GetServiceProfessionalPtFees(long id)
        {
            return Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == id)
                    //.Include(x => x.Service).ThenInclude(i => i.PtFee)
                    //.Include(x => x.Service).ThenInclude(i => i.ProFee)
                    .Include(x => x.Service).ThenInclude(i => i.Field)
                    .Include(x => x.Service).ThenInclude(c => c.SubCategory).AsNoTracking().ToList();
        }

        public IEnumerable<ServiceProfessionalProFees> GetServiceProfessionalProFees(long id)
        {
            return Db.ServiceProfessionalProFees.Where(x => x.ProfessionalId == id)
                    //.Include(x => x.Service).ThenInclude(i => i.PtFee)
                    //.Include(x => x.Service).ThenInclude(i => i.ProFee)
                    .Include(x => x.Service).ThenInclude(i => i.Field)
                    .Include(x => x.Service).ThenInclude(c => c.SubCategory).AsNoTracking().ToList();
        }

        public IQueryable<ProfessionalLanguages> GetAllLangs()
        {
            return Db.ProfessionalLanguages;
        }

        public IQueryable<Professional> GetByIdWithIncludes(long professionalId)
        {
            return Db.Professional.Include(x => x.ProfessionalLangs).Where(x => x.Id == professionalId).AsNoTracking();
        }

        public void DeleteLangs(long id)
        {
            var langs = Db.ProfessionalLanguages.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            Db.ProfessionalLanguages.RemoveRange(langs);
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

        public dynamic GetProfessionalConnectedServices(long proId)
        {
            
            var query = (from s in Db.Service
                         join
                         pt in Db.ServiceProfessionalPtFees on s.Id equals pt.ServiceId join
                         pr in Db.ServiceProfessionalProFees on s.Id equals pr.ServiceId
                         where pt.Professional.Id == proId && pr.ProfessionalId == proId

                         select new
                         {
                             pt.Service.Id,
                             proId = proId,
                             cService = pt.Service.Name,
                             cField = pt.Service.Field.Field,
                             cSubcategory = pt.Service.SubCategory.SubCategory,
                             ptFeeRowId = pt.Service.ServiceProfessionalPtFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().Id,
                             ptFeeId = pt.Service.ServiceProfessionalPtFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().PtFeeId,
                             ptFeeName = pt.PtFeeId > 0 ? pt.Service.ServiceProfessionalPtFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().PtFee.FeeName : null,
                             ptFeeA1 = pt.PtFeeId > 0 ? pt.Service.ServiceProfessionalPtFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().PtFee.A1 : null,
                             ptFeeA2 = pt.PtFeeId > 0 ? pt.Service.ServiceProfessionalPtFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().PtFee.A2 : null,

                             proFeeRowId = pr.Service.ServiceProfessionalProFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().Id,
                             proFeeId =  pr.Service.ServiceProfessionalProFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().ProFeeId,
                             proFeeName = pr.ProFeeId >0 ? pr.Service.ServiceProfessionalProFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().ProFee.FeeName : null,
                             proFeeA1 = pr.ProFeeId > 0 ? pr.Service.ServiceProfessionalProFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().ProFee.A1 : null,
                             proFeeA2 = pr.ProFeeId > 0 ? pr.Service.ServiceProfessionalProFees.Where(p => p.ProfessionalId == proId).FirstOrDefault().ProFee.A2 : null,

                         }).ToList();

            return query;

        }


        public void DetachProfessionalConnectedService(IEnumerable<long> serviceIds, long proId)
        {
            try
            {
                var ptServices = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == proId && serviceIds.Contains(x.ServiceId)).ToList();
                Db.ServiceProfessionalPtFees.RemoveRange(ptServices);

                var proServices = Db.ServiceProfessionalProFees.Where(x => x.ProfessionalId == proId && serviceIds.Contains(x.ServiceId)).ToList();
                Db.ServiceProfessionalProFees.RemoveRange(proServices);
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public dynamic GetProfessionalServiceDetail(long serviceId, long proId)
        {
            var feeIds = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == proId).Select(x => x.PtFeeId);
            var fees = Db.PtFee.ToList();

            return (from s in Db.Service
                    join
                    sp in Db.ServiceProfessionalPtFees on s.Id equals sp.ServiceId
                    where sp.Professional.Id == proId && sp.ServiceId == serviceId

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
                    var ptFeeExists = Db.ServiceProfessionalPtFees.Find(model.PtFeeRowId);
                    if (!(ptFeeExists is null))
                    {
                        ptFeeExists.PtFeeId = model.PtFeeId;
                        Db.ServiceProfessionalPtFees.Update(ptFeeExists);
                    }
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
                        var ptFeeRowId = Db.ServiceProfessionalPtFees.Find(model.PtFeeRowId);
                        ptFeeRowId.PtFeeId = ptFee.Id;
                        Db.ServiceProfessionalPtFees.Update(ptFeeRowId);
                    }
                }

                if (model.ProFeeId > 0)
                {
                    var proFeeExists = Db.ServiceProfessionalProFees.Find(model.ProFeeRowId);
                    if (!(proFeeExists is null))
                    {
                        proFeeExists.ProFeeId = model.ProFeeId;
                        Db.ServiceProfessionalProFees.Update(proFeeExists);
                    }
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
                        var proFeeRowId = Db.ServiceProfessionalProFees.Find(model.ProFeeRowId);
                        proFeeRowId.ProFeeId = proFee.Id;
                        Db.ServiceProfessionalProFees.Update(proFeeRowId);
                    }
                }


                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
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
                var proServices = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == proId).Select(x => x.Id).ToList();
                var existingPtIds = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();
                var existingProIds = Db.ServiceProfessionalProFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();
                var union = existingPtIds.Union(existingProIds).Distinct().ToList();


                var result = Db.Service
                    .Where(x => !union.Contains(x.Id))
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
                var ptServices = Db.ServiceProfessionalPtFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).Distinct().ToList();
                var proServices = Db.ServiceProfessionalProFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).Distinct().ToList();
                var union = ptServices.Union(proServices).Distinct().ToArray();

                var services = (from s in Db.Service.Where(x => !union.Contains(x.Id))
                                select new FilterModel
                                {
                                    Id = s.Id,
                                    Value = s.Name
                                }).ToList();

                var fields = (from s in Db.Service.Where(x => !union.Contains(x.Id))
                              select new FilterModel
                              {
                                  Id = s.Id,
                                  Value = s.Field.Field
                              }).ToList();
                var subCats = (from s in Db.Service.Where(x => !union.Contains(x.Id))
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
                    if (Db.ServiceProfessionalPtFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == proId && x.PtFeeId == 0) is null)
                    {
                        Db.ServiceProfessionalPtFees.Add(new ServiceProfessionalPtFees { ServiceId = sid, ProfessionalId = proId, PtFeeId = 0 });
                        Db.SaveChanges();
                    }

                    if (Db.ServiceProfessionalProFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == proId && x.ProFeeId == 0) is null)
                    {
                        Db.ServiceProfessionalProFees.Add(new ServiceProfessionalProFees { ServiceId = sid, ProfessionalId = proId, ProFeeId = 0 });
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

        public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        {
            try
            {
                var ptRow = Db.ServiceProfessionalPtFees.Find(ptRelationRowId);
                var ptFees = Db.ServiceProfessionalPtFees.Where(x => x.Id != ptRow.Id && x.ProfessionalId == ptRow.ProfessionalId && x.ServiceId == ptRow.ServiceId).Select(x => x.PtFeeId).ToList();

                var proRow = Db.ServiceProfessionalProFees.Find(proRelationRowId);
                var proFees = Db.ServiceProfessionalProFees.Where(x => x.Id != proRow.Id && x.ProfessionalId == proRow.ProfessionalId && x.ServiceId == proRow.ServiceId).Select(x => x.ProFeeId).ToList();

                var ptFeesForFilter = Db.PtFee.Where(x => ptFees.All(y => y != x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                var proFeesForFilter = Db.ProFee.Where(x => proFees.All(y => y != x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

    }
}
