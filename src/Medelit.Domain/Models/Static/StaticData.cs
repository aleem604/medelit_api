using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("static_data")]
  public  class StaticData
    {
        public short Id { get; set; }
        [Column("accounting_codes")]
        public string AccountingCodes  { get; set; }
        [Column("application_means")]
        public string ApplicationMeans { get; set; }
        [Column("application_methods")]
        public string ApplicationMethods { get; set; }
         [Column("booking_status")]
        public string BookingStatus { get; set; }
         [Column("booking_types")]
        public string BookingTypes { get; set; }
         [Column("building_types")]
        public string BuildingTypes { get; set; }
         [Column("collaboration_codes")]
        public string CollaborationCodes { get; set; }
         [Column("contact_methods")]
        public string ContactMethods { get; set; }
         [Column("contract_status")]
        public string ContractStatus { get; set; }
         [Column("discount_networks")]
        public string DiscountNetworks { get; set; }
         [Column("document_list_sent")]
        public string DocumentListSentOptions { get; set; }
         [Column("durations")]
        public int? Durations { get; set; }
         [Column("duration_units")]
        public string DurationUnits { get; set; }
         [Column("field_categories")]
        public string FieldCategories { get; set; }
         [Column("ie_ratings")]
        public string IERatings { get; set; }
         [Column("ie_types")]
        public string IETypes { get; set; }
         [Column("invoice_status")]
        public string InvoiceStatus { get; set; }
         [Column("lead_categories")]
        public string LeadCategories { get; set; }
         [Column("lead_sources")]
        public string LeadSources { get; set; }
         [Column("lead_status")]
        public string LeadStatus { get; set; }
         [Column("m_titles")]
        public string Titles { get; set; }
         [Column("payment_methods")]
        public string PaymentMethods { get; set; }
         [Column("payment_status")]
        public string PaymentStatus { get; set; }
        [Column("relationships")]
        public string Relationships { get; set; }
        [Column("report_delivery_options")]
        public string ReportDeliveryOptions { get; set; }
        [Column("vats")]
        public decimal? Vats { get; set; }
        [Column("vat_units")]
        public string VatUnit { get; set; }
        [Column("visit_venues")]
        public string VisitVenues { get; set; }
        [Column("added_to_account_options")]
        public string AddToAccountOptions { get; set; }


    }
}
