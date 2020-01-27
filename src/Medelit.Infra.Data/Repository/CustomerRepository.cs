using System;
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
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MedelitContext context, IHttpContextAccessor contextAccessor)
            : base(context, contextAccessor)
        {
        }

        public IQueryable<Customer> GetAllWithService()
        {
            return Db.Customer.Include(x => x.Services);
        }
        public Customer GetByIdWithInclude(long customerId)
        {
            return Db.Customer.Include(x => x.Services).FirstOrDefault(x => x.Id == customerId);
        }

        public void RemoveCustomerServices(long id)
        {
            var services = Db.CustomerServiceRelation.Where(x => x.CustomerId == id).ToList();
            Db.RemoveRange(services);
            Db.SaveChanges();
        }

        public void SaveCustomerRelation(List<CustomerServiceRelation> newServices)
        {
            Db.CustomerServiceRelation.AddRange(newServices);
            Db.SaveChanges();
        }

        public dynamic GetCustomerConnectedCustomers(long customerId)
        {
            var titles = Db.StaticData.Select((s) => new { s.Id, Value = s.Titles }).Where(x => x.Value != null).ToList();
            return (from l in Db.Lead
                    where l.CustomerId == customerId
                    select new
                    {
                        Title = $"{l.Customer.SurName} {titles.FirstOrDefault(x => x.Id == l.Customer.TitleId).Value}",
                        l.Customer.Name,
                        l.Customer.Email,
                        phone = l.Customer.MainPhone
                    }).ToList();
        }

        public dynamic GetCustomerConnectedServices(long customerId)
        {

            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    select new
                    {
                        serviceName = b.Service.Name,
                        //PtFeeName = b.Service.PtFee.FeeName,
                        //PtFee = b.PtFee,
                        //ProFeeName = b.Service.ProFee.FeeName,
                        //ProFee = b.ProFee,
                        //Professional = b.Professional.Name, 
                        //Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.Service.PtFee.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                        //            <span class='font-500'>Pro. Fee Name :</span> {b.Service.ProFee.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).ToList();

        }

        public dynamic GetCustomerConnectedProfessionals(long customerId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();
            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    select new
                    {
                        proName = b.Professional.Name,
                        phone = b.Professional.HomePhone,
                        email = b.Professional.Email,
                        b.VisitStartDate,
                        b.Professional.ActiveCollaborationId,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x=>x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).ToList();

        }

        public dynamic GetCustomerConnectedBookings(long customerId)
        {
            return (from b in Db.Booking
                    where b.CustomerId == customerId
                    select new
                    {
                        bookingName = b.Name,

                        serviceName = b.Service.Name,
                        //PtFee = b.Service.PtFee.FeeName,
                        //PtFeeA1 = b.Service.PtFee.A1,
                        //PtFeeA2 = b.Service.PtFee.A2,
                        //ProFee = b.Service.ProFee.FeeName,
                        //ProFeeA1 = b.Service.ProFee.A1,
                        //ProFeeA2 = b.Service.ProFee.A2,
                        professional = b.Professional.Name,
                        visitDate =  b.VisitStartDate,

                        //Service = $@"<span class='font-500'>Service:</span> {b.Service.Name} <br/> <span class='font-500'>Pt Fee Name:</span> {b.Service.PtFee.FeeName} <br/> <span class='font-500'>Pt. Fee:</span> {(b.PtFee.HasValue ? b.PtFee.Value.ToString("G29") : string.Empty)} <br/> 
                        //            <span class='font-500'>Pro. Fee Name :</span> {b.Service.ProFee.FeeName} <br/> <span class='font-500'>Pro. Fee:</span> {(b.ProFee.HasValue ? b.ProFee.Value.ToString("G29") : string.Empty)}"
                    }).ToList();
        }

        public dynamic GetCustomerConnectedInvoices(long customerId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.Booking.CustomerId == customerId
                    select new
                    {
                       ib.Invoice.Subject,
                       ib.Invoice.InvoiceNumber,
                       ieName = ib.Invoice.InvoiceEntityId.HasValue ? ib.Invoice.InvoiceEntity.Name : string.Empty,
                       ib.Invoice.InvoiceDate,
                       ib.Invoice.TotalInvoice
                    }).ToList();
        }

        public dynamic GetCustomerConnectedLeads(long customerId)
        {
            return (from ls in Db.LeadServiceRelation
                    where ls.Lead.CustomerId == customerId
                    select new
                    {
                       LeadName =  $"{ls.Lead.SurName} {ls.Lead.Name}",
                       ls.Lead.CreateDate,
                       ls.Lead.UpdateDate,
                       Professional = string.Join(" <br/>", ls.Service.ServiceProfessionalPtFees.Select(x=>x.Professional.Name).ToArray()),
                       ls.Lead.LeadStatusId
                    }).ToList();
        }
    }
}
