using Medelit.Common;
using Medelit.Common.Models;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Infra.Data.Repository
{
    public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
    {
        public ProfessionalRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        {
        }

        public IEnumerable<ServiceProfessionalFees> GetProfessionalServices(long id)
        {
            return Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == id)
                    .Include(i => i.Service)
                    .Include(i => i.Professional)
                    .AsNoTracking().ToList();
        }

        public IEnumerable<ServiceProfessionalFees> GetProfessionalProFees(long id)
        {
            return Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == id)
                    //.Include(x => x.Service).ThenInclude(i => i.PtFee)
                    //.Include(x => x.Service).ThenInclude(i => i.ProFee)
                    .AsNoTracking().ToList();
        }

        public IQueryable<ProfessionalLanguages> GetAllLangs()
        {
            return Db.ProfessionalLanguages;
        }

        public IQueryable<Professional> GetByIdWithIncludes(long professionalId)
        {
            return Db.Professional.Include(x => x.ProfessionalLanguages)
                .Include(x => x.ProfessionalFields)
                .Include(x => x.ProfessionalSubCategories)
                .Where(x => x.Id == professionalId).AsNoTracking();
        }

        public void DeleteProfessionalRelations(long id)
        {
            var langs = Db.ProfessionalLanguages.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            var fields = Db.ProfessionalFields.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            var subCats = Db.ProfessionalSubCategories.Where(x => x.ProfessionalId == id).AsNoTracking().ToList();
            Db.ProfessionalLanguages.RemoveRange(langs);
            Db.ProfessionalFields.RemoveRange(fields);
            Db.ProfessionalSubCategories.RemoveRange(subCats);
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


        public void GetProfessionalConnectedServices(long proId)
        {
            try
            {
                var result = (from ps in Db.ServiceProfessionalFees
                              where ps.Professional.Id == proId

                              select new ProfessionalConnectedServicesModel
                              {
                                  ProfessionalId = ps.ProfessionalId,
                                  ServiceId = ps.ServiceId,
                                  CService = ps.Service.Name,
                                  CField = ps.Service.Field.Field,
                                  CSubcategory = ps.Service.SubCategory.SubCategory,
                                  PtFeeRowId = ps.Id,
                                  PtFeeId = ps.PtFeeId,
                                  PtFeeName = ps.PtFeeId == null ? default : (ps.PtFee == null ? default : ps.PtFee.FeeName),
                                  PtFeeA1 = ps == null ? default : (ps.PtFee == null ? default : ps.PtFee.A1),
                                  PtFeeA2 = ps == null ? default : (ps.PtFee == null ? default : ps.PtFee.A2),

                                  ProFeeRowId = ps.Id,
                                  ProFeeId = ps.ProFeeId,
                                  ProFeeName = ps.ProFeeId == null ? default : (ps.ProFee == null ? default : ps.ProFee.FeeName),
                                  ProFeeA1 = ps == null ? default : (ps.ProFee == null ? default : ps.ProFee.A1),
                                  ProFeeA2 = ps == null ? default : (ps.ProFee == null ? default : ps.ProFee.A2),

                              }).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void DetachProfessionalConnectedService(IEnumerable<EditProfessionalServiceFeesModel> serviceObjs, long proId)
        {
            try
            {
                foreach (var row in serviceObjs)
                {
                    var rowObj = Db.ServiceProfessionalFees.Find(row.PtFeeRowId);
                    Db.ServiceProfessionalFees.Remove(rowObj);
                }
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, Db.SaveChanges()));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public dynamic GetProfessionalServiceDetail(long professionalPtFeeRowId, long professionalProFeeRowId)
        {
            return (from sp in Db.ServiceProfessionalFees
                    join
                    ps in Db.ServiceProfessionalFees on sp.ProfessionalId equals ps.ProfessionalId
                    join
                    pro in Db.ServiceProfessionalFees on sp.ProfessionalId equals pro.ProfessionalId
                    where ps.Id == professionalPtFeeRowId && pro.Id == professionalProFeeRowId

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
                    var ptFeeExists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                    if (!(ptFeeExists is null))
                    {
                        ptFeeExists.PtFeeId = model.PtFeeId.Value;
                        Db.ServiceProfessionalFees.Update(ptFeeExists);
                    }
                    else
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { PtFeeId = model.PtFeeId.Value, ProfessionalId = model.ProfessionalId.Value });
                    }

                    Db.SaveChanges();
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
                        var ptFeeRowId = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                        if (ptFeeRowId != null)
                        {
                            ptFeeRowId.PtFeeId = ptFee.Id;
                            Db.ServiceProfessionalFees.Update(ptFeeRowId);
                        }
                    }
                    Db.SaveChanges();
                }

                if (model.ProFeeId > 0)
                {
                    var proFeeExists = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                    if (!(proFeeExists is null))
                    {
                        proFeeExists.ProFeeId = model.ProFeeId.Value;
                        Db.ServiceProfessionalFees.Update(proFeeExists);

                    }
                    else
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ProFeeId = model.ProFeeId.Value, ProfessionalId = model.ProfessionalId.Value });
                    }
                    Db.SaveChanges();
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
                        var proFeeRowId = Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == model.ServiceId && x.ProfessionalId == model.ProfessionalId);
                        proFeeRowId.ProFeeId = proFee.Id;
                        Db.ServiceProfessionalFees.Update(proFeeRowId);
                    }
                    Db.SaveChanges();
                }

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, true));
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
                var existingServices = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).ToList();

                var result = Db.Service
                    .Where(x => !existingServices.Contains(x.Id))
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
                var existingProIds = Db.ServiceProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => x.ServiceId).Distinct().ToList();

                var services = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
                                select new FilterModel
                                {
                                    Id = s.Id,
                                    Value = s.Name
                                }).ToList();

                var fields = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
                              select new FilterModel
                              {
                                  Id = s.Id,
                                  Value = s.Field.Field
                              }).ToList();
                var subCats = (from s in Db.Service.Where(x => !existingProIds.Contains(x.Id))
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
                    if (Db.ServiceProfessionalFees.FirstOrDefault(x => x.ServiceId == sid && x.ProfessionalId == proId) is null)
                    {
                        Db.ServiceProfessionalFees.Add(new ServiceProfessionalFees { ServiceId = sid, ProfessionalId = proId });
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

        //public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        //{
        //    try
        //    {
        //        var ptFeesForFilter = new List<FilterModel>();
        //        var proFeesForFilter = new List<FilterModel>();

        //        var row = Db.ServiceProfessionalFees.Find(ptRelationRowId);

        //        if (row != null)
        //        {
        //            var ptFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId).Select(x => x.PtFeeId).Distinct().ToList();
        //            ptFeesForFilter = Db.PtFee.Where(x => ptFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();

        //            var proFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == row.ServiceId).Select(x => x.ProFeeId).Distinct().ToList();
        //            proFeesForFilter = Db.ProFee.Where(x => proFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
        //        }


        //        _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));

        //    }
        //    catch (Exception ex)
        //    {
        //        _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
        //    }
        //}

        public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        {
            try
            {
                var ptFeesForFilter = new List<FilterModel>();
                var proFeesForFilter = new List<FilterModel>();

                var ptRow = Db.ServiceProfessionalFees.Find(ptRelationRowId);
                var proRow = Db.ServiceProfessionalFees.Find(proRelationRowId);

                if (ptRow != null)
                {
                    var ptFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == ptRow.ServiceId && x.ProfessionalId == ptRow.ProfessionalId && x.PtFeeId != ptRow.PtFeeId).Select(x => x.PtFeeId).Distinct().ToList();
                    ptFeesForFilter = Db.PtFee.Where(x => !ptFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }

                if (proRow != null)
                {
                    var proFees = Db.ServiceProfessionalFees.Where(x => x.ServiceId == ptRow.ServiceId && x.ProfessionalId == ptRow.ProfessionalId && x.ProFeeId != proRow.ProFeeId).Select(x => x.ProFeeId).Distinct().ToList();
                    proFeesForFilter = Db.ProFee.Where(x => !proFees.Contains(x.Id)).Select(x => new FilterModel { Id = x.Id, Value = x.FeeName }).ToList();
                }


                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, new { pt = ptFeesForFilter, pro = proFeesForFilter }));

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

    }
}
