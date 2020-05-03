using AutoMapper;
using Medelit.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Application
{
  public  class InvoiceViewModel : BaseViewModel
    {
        public string Subject { get; set; }
        public long? InvoiceEntityId { get; set; }
        public string InvoiceEntity { get; set; }
        public long? CustomerId { get; set; }
        public string Customer { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? SubTotal { get; set; }
        public short? TaxCodeId { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TotalInvoice { get; set; }
        public short? StatusId { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public DateTime? InvoiceDeliveryDate { get; set; }
        public short? InvoiceSentByEmailId { get; set; } = 0;
        public short? InvoiceSentByMailId { get; set; } = 0;
        public short? PaymentMethodId { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
        public string IEBillingAddress { get; set; }
        public string MailingAddress { get; set; }
        public string IEBillingPostCode { get; set; }
        public string MailingPostCode { get; set; }
        public string IEBillingCity { get; set; }
        public string MailingCity { get; set; }
        public short? IEBillingCountryId { get; set; }
        public short? MailingCountryId { get; set; }
        public string InvoiceNotes { get; set; }
        public short? InsuranceCoverId { get; set; }
        public string InvoiceDiagnosis { get; set; }
        public DateTime? DateOfVisit { get; set; }
        [Required]
        public string TermsAndConditions { get; set; }
        public string InvoiceDescription { get; set; }
        public string ItemNameOnInvoice { get; set; }

        public DateTime? PaymentArrivalDate { get; set; }
        public DateTime? ProInvoiceDate { get; set; }
        public bool IsProforma { get; set; }

        [IgnoreMap]
        public dynamic InvoiceBookings { get; set; }

    }
}
