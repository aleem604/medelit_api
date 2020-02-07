using System.Linq;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class InvoiceEntityRepository : Repository<InvoiceEntity>, IInvoiceEntityRepository
    {
        public InvoiceEntityRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        { }

        public dynamic InvoiceEntityConnectedServices(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        serviceName = b.Service.Name,
                        //PtFeeName = b.Service.PtFee.FeeName,
                        b.PtFee,
                        //ProFeeName = b.Service.ProFee.FeeName,
                        b.ProFee,
                        Professional = b.Professional.Name,
                        //Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.Service.PtFee.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                        //            <span class='font-500'>Pro. Fee Name :</span> {b.Service.ProFee.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).ToList();
        }
        public dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        title = $"{b.Customer.SurName} {b.Customer.Name}",
                        phone = b.Customer.MainPhone,
                        b.Customer.Email,
                        services = b.Customer.Services.Select(x => new { x.Service.Name, x.PtFeeId, x.PTFeeA1, x.PTFeeA2, x.PROFeeId, x.PROFeeA1, x.PROFeeA2 }).ToList(),
                        visitDate = b.VisitStartDate,
                        professional = b.Professional.Name

                    }
                ).ToList();
        }

        public dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        professional = b.Professional.Name,
                        phoneNumber = b.Professional.HomePhone,
                        b.Professional.Email,
                        lastVisitDate = b.VisitStartDate,
                        b.Professional.ActiveCollaborationId,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).ToList();
        }

        public dynamic InvoiceEntityConnectedBookings(long invoiceEntityId)
        {
            return (from b in Db.Booking
                    where b.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        bookingName = b.Name,
                        //Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.Service.PtFee.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                        //            <span class='font-500'>Pro. Fee Name :</span> {b.Service.ProFee.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}",
                        professional = b.Professional.Name,
                        visitDate = b.VisitStartDate,

                    }).ToList();

        }
        public dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId)
        {
            return (from i in Db.Invoice
                    where i.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        subject = i.Subject,
                        invoiceNumber = i.InvoiceNumber,
                        ieName = i.InvoiceEntityId.HasValue ? i.InvoiceEntity.Name : string.Empty,
                        invoiceDate = i.InvoiceDate,
                        totalInvoice = i.TotalInvoice
                    }).ToList();
        }

        public dynamic InvoiceEntityConnectedLeads(long invoiceEntityId)
        {
            var statuses = Db.StaticData.Select((s) => new { s.Id, Value = s.LeadStatus }).Where(x => x.Value != null).ToList();
            return (from l in Db.Lead
                    where l.InvoiceEntityId == invoiceEntityId
                    select new
                    {
                        services = l.Services.Select(x => new { x.Service.Name, x.PtFeeId, x.PTFeeA1, x.PTFeeA2, x.PROFeeId, x.PROFeeA1, x.PROFeeA2 }).ToList(),
                        invoiceNumber = l.CreateDate,
                        ieName = l.UpdateDate,
                        professional = l.Services.Select(x => x.Professional.Name).ToArray(),
                        l.LeadStatusId,
                        Status = l.LeadStatusId.HasValue ? statuses.FirstOrDefault(x => x.Id == l.LeadStatusId).Value : string.Empty
                    }).ToList();
        }


    }
}
