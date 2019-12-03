using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("invoice")]
    public class Invoice : BaseEntity
    {
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }
        public string Subject { get; set; }
        [Column("booking_id")]
        public long? BookingId { get; set; }
        [Column("invocing_entity_id")]
        public long? InvocingEntityId { get; set; }
        [Column("customer_id")]
        public long CustomerId { get; set; }
        [Column("due_date")]
        public DateTime? DueDate { get; set; }
        [Column("invoice_date")]
        public DateTime? InvoiceDate { get; set; }
        [Column("sub_total")]
        public decimal? SubTotal { get; set; }
        [Column("tax_code_id")]
        public int? TaxCodeId { get; set; }
        [Column("tax_amount")]
        public decimal?  TaxAmount { get; set; }
        public decimal? Discount { get; set; }
        [Column("total_invoice")]
        public decimal? TotalInvoice { get; set; }
        [Column("invoice_status_id")]
        public int? InvoiceStatusId { get; set; }
        [Column("payment_due")]
        public DateTime? PaymentDue { get; set; }
        [Column("invoice_delivery_date")]
        public DateTime? InvoiceDeliveryDate { get; set; }
        [Column("invoice_sent_by_email")]
        public bool? InvoiceSentByEmail { get; set; }
        [Column("invoice_sent_by_mail")]
        public bool? InvoiceSentByMail { get; set; }
        [Column("payment_method_id")]
        public int? PaymentMethodId { get; set; }
        [Column("patient_dob")]
        public DateTime? PatientDOB { get; set; }
        [Column("ie_billing_address")]
        public string IEBillingAddress { get; set; }
        [Column("mailing_address")]
        public string MailingAddress { get; set; }
        [Column("ie_post_code")]
        public string IEPostCode { get; set; }
        [Column("mailing_post_code")]
        public string MailingPostCode { get; set; }
        [Column("ie_city_id")]
        public int? IECityId { get; set; }
        [Column("mailing_city_id")]
        public int? MailingCityId { get; set; }
        [Column("ie_billing_country_id")]
        public int? IeBillingCountryId { get; set; }
        [Column("mailing_country_id")]
        public int? MailingCountryId { get; set; }
        [Column("invoice_notes")]
        public string InvoiceNotes { get; set; }
        [Column("insurance_cover_id")]
        public int? InsuranceCoverId { get; set; }
        [Column("invoice_diagnosis")]
        public string InvoiceDiagnosis { get; set; }
        [Column("date_of_vist")]
        public DateTime? DateOfVist { get; set; }
        [Column("terms_and_conditions")]
        public string TermsAndConditions { get; set; }
        [Column("invoice_description")]
        public string InvoiceDescription { get; set; }
        [Column("item_name_on_invoice")]
        public string ItemNameOnInvoice { get; set; }
        public int? Quantity { get; set; }
        [Column("payment_arrival_date")]
        public DateTime? PaymentArrivalDate { get; set; }
        [Column("pro_invoice_date")]
        public DateTime? ProInvoiceDate { get; set; }
        [Column("assigned_to_id")]
        public long? AssignedToId { get; set; }

        /// connected customers
        /// connected services
    }
}