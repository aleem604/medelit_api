using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
  public  class InvoiceViewModel
    {
        public long Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Subject { get; set; }
        public long? BookingId { get; set; }
        public long? InvocingEntityId { get; set; }
        public long CustomerId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? SubTotal { get; set; }
        public int? TaxCodeId { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TotalInvoice { get; set; }
        public int? InvoiceStatusId { get; set; }
        public DateTime? PaymentDue { get; set; }
        public DateTime? InvoiceDeliveryDate { get; set; }
        public bool? InvoiceSentByEmail { get; set; }
        public bool? InvoiceSentByMail { get; set; }
        public int? PaymentMethodId { get; set; }
        public DateTime? PatientDOB { get; set; }
        public string IEBillingAddress { get; set; }
        public string MailingAddress { get; set; }
        public string IEPostCode { get; set; }
        public string MailingPostCode { get; set; }
        public int? IECityId { get; set; }
        public int? MailingCityId { get; set; }
        public int? IeBillingCountryId { get; set; }
        public int? MailingCountryId { get; set; }
        public string InvoiceNotes { get; set; }
        public int? InsuranceCoverId { get; set; }
        public string InvoiceDiagnosis { get; set; }
        public DateTime? DateOfVist { get; set; }
        public string TermsAndConditions { get; set; }
        public string InvoiceDescription { get; set; }
        public string ItemNameOnInvoice { get; set; }
        public int? Quantity { get; set; }
        public DateTime? PaymentArrivalDate { get; set; }
        public DateTime? ProInvoiceDate { get; set; }
        public long? AssignedToId { get; set; }
       
        public eRecordStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public long? CreatedById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long? DeletedById { get; set; }

    }
}
