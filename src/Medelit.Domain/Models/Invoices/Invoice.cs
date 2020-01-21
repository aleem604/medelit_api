using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("invoice")]
    public class Invoice : BaseEntity
    {
        public string Subject { get; set; }
        [Column("invoicing_entity_id")]
        public long? InvoiceEntityId { get; set; }
        [ForeignKey("InvoiceEntityId")]
        public InvoiceEntity InvoiceEntity { get; set; }
        
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }
        [Column("due_date")]
        public DateTime? DueDate { get; set; }
        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }
        [Column("sub_total")]
        public decimal? SubTotal { get; set; }
        [Column("tax_code_id")]
        public short? TaxCodeId { get; set; }
        [Column("tax_amount")]
        public decimal? TaxAmount { get; set; }
        public decimal? Discount { get; set; }
        [Column("total_invoice")]
        public decimal? TotalInvoice { get; set; }
        [Column("status_id")]
        public short? StatusId { get; set; }
        [Column("payment_due")]
        public DateTime? PaymentDue { get; set; }
        [Column("invoice_delivery_date")]
        public DateTime? InvoiceDeliveryDate { get; set; }
        [Column("invoice_sent_by_email_id")]
        public short? InvoiceSentByEmailId { get; set; }
        [Column("invoice_sent_by_mail_id")]
        public short? InvoiceSentByMailId { get; set; }
        [Column("payment_method_id")]
        public short? PaymentMethodId { get; set; }
        [Column("patient_date_of_birth")]
        public DateTime? PatientDateOfBirth { get; set; }
        [Column("ie_billing_address")]
        public string IEBillingAddress { get; set; }
        [Column("mailing_address")]
        public string MailingAddress { get; set; }
        [Column("ie_billing_post_code")]
        public string IEBillingPostCode { get; set; }
        [Column("mailing_post_code")]
        public string MailingPostCode { get; set; }
        [Column("ie_billing_city_id")]
        public short? IEBillingCityId { get; set; }
        [Column("mailing_city_id")]
        public short? MailingCityId { get; set; }
        [Column("ie_billing_country_id")]
        public short? IEBillingCountryId { get; set; }
        [Column("mailing_country_id")]
        public short? MailingCountryId { get; set; }
        [Column("invoice_notes")]
        public string InvoiceNotes { get; set; }
        [Column("insurance_cover_id")]
        public short? InsuranceCoverId { get; set; }
        [Column("invoice_diagnosis")]
        public string InvoiceDiagnosis { get; set; }
        [Column("date_of_visits")]
        public DateTime? DateOfVisit { get; set; }
        [Column("terms_and_conditions")]
        public string TermsAndConditions { get; set; }
        [Column("invoice_description")]
        public string InvoiceDescription { get; set; }
        [Column("item_name_on_invoice")]
        public string ItemNameOnInvoice { get; set; }
        [Column("payment_arrival_date")]
        public DateTime? PaymentArrivalDate { get; set; }
        [Column("pro_invoice_date")]
        public DateTime? ProInvoiceDate { get; set; }
       
        [Column("customer_id")]
        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public ICollection<InvoiceBookings> InvoiceBookings { get; set; }

        /// connected customers
        /// connected services
    }
}