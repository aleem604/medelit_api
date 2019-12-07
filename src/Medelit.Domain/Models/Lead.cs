using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    public class Lead : BaseEntity
    {
        [Column("sur_name")]
        public string SurName { get; set; }
        [Column("title_id")]
        public int? TitleId { get; set; }
        public string Name { get; set; }
        [Column("requested_service_id")]
        public long RequestedServiceId { get; set; }
        [Column("main_phone")]
        public string MainPhone { get; set; }
        [Column("main_phone_owner")]
        public string MainPhoneOwner { get; set; }
        [Column("invoice_entity_id")]
        public long? InvoiceEntityId { get; set; }
        public string Phone2 { get; set; }
        [Column("phone2_owner")]
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        [Column("phone3_owner")]
        public string Phone3Owner { get; set; }
        [Column("contact_phone")]
        public string ContactPhone { get; set; }
        [Column("visit_requesting_person")]
        public string VisitRequestingPerson { get; set; }
        [Column("visit_requestig_person_relation_id")]
        public int? VisitRequestingPersonRelationId { get; set; }
        public string Fax { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        public string Email { get; set; }
        [Column("lead_source_id")]
        public int? LeadSourceId { get; set; }
        [Column("lead_status_id")]
        public int? LeadStatusId { get; set; }
        [Column("language_id")]
        public int? LanguageId { get; set; }
        [Column("lead_category_id")]
        public int? LeadCategoryId { get; set; }
        [Column("contact_method_id")]
        public int? ContactMethodId { get; set; }
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [Column("country_of_birth_id")]
        public int? CountryOfBirthId { get; set; }
        [Column("preferred_payment_method_id")]
        public int? PreferredPaymentMethodId { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }
        [Column("insurance_cover_id")]
        public short? InsuranceCover { get; set; }
        [Column("listed_discount_network_id")]
        public int? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        [Column("gp_code")]
        public string GPCode { get; set; }
        [Column("address_street_name")]
        public string AddressStreetName { get; set; }
        [Column("post_code")]
        public string PostalCode { get; set; }
        [Column("city_id")]
        public int? CityId { get; set; }
        [Column("country_id")]
        public int? CountryId { get; set; }
        [Column("building_type_id")]
        public int? BuildingTypeId { get; set; }
        [Column("flat_no")]
        public short? FlatNumber { get; set; }
        public string Buzzer { get; set; }
        public int? Floor { get; set; }
        [Column("visit_venue_id")]
        public int? VisitVenueId { get; set; }
        [Column("address_notes")]
        public string AddressNotes { get; set; }
        [Column("visit_venue_detail")]
        public string VisitVenueDetail { get; set; }
        public string Description { get; set; }
        [Column("from_customer_id")]
        public long? FromCustomerId { get; set; }
        [Column("converted_at")]
        public DateTime? ConvertDate { get; set; }
    }
}