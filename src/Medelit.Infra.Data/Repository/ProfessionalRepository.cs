using System.Collections.Generic;
using System.Linq;
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
        public ProfessionalRepository(MedelitContext context, IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {

        }

        public IEnumerable<ServiceProfessionalRelation> GetProfessionalServices(long id)
        {
            return Db.ServiceProfessionalRelation.Where(x => x.ProfessionalId == id)
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
                           Service =  ls.Service.Name,
                           l.CreateDate,
                           l.UpdateDate,
                           l.LeadStatusId,
                           LeadStatus = leadStatus.FirstOrDefault(x=>x.Id == l.LeadStatusId).Value,
                           Professional = ls.Professional.Name
                        };
            return query.ToList();
        }

        public dynamic GetProfessionalConnectedServices(long proId)
        {
            var feeIds = Db.ProfessionalFees.Where(x => x.ProfessionalId == proId).Select(x => new { x.FeeId, x.FeeType });
            var fees = Db.Fee.ToList();

            var query = (from s in Db.Service join               
                            sp in Db.ServiceProfessionalRelation on s.Id equals sp.ServiceId
                         where sp.Professional.Id == proId
                         
                         select new { 
                            cService = sp.Service.Name,
                            cField = sp.Service.Field.Field,
                            cSubcategory = sp.Service.SubCategory.SubCategory,
                            ptFeeId = sp.Professional.ProfessionalFees.Where(x=>x.FeeType == Common.eFeeType.PTFee),
                            proFeeId = sp.Professional.ProfessionalFees.Where(x=>x.FeeType == Common.eFeeType.PROFee),                            
                         }).ToList();

            return query;

        }

        public dynamic DetachProfessionalConnectedService(IEnumerable<long> serviceIds, long proId)
        {
            var spr = Db.ServiceProfessionalRelation.Where(x => x.ProfessionalId == proId && serviceIds.Contains(x.ServiceId)).ToList();
            Db.ServiceProfessionalRelation.RemoveRange(spr);
            
            return Db.SaveChanges();
        }

    }
}
