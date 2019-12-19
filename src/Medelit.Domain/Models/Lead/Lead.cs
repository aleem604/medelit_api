using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    public class Lead : BaseEntity
    {
        [Column("sur_name")]
        public string SurName { get; set; }
        [Column("title_id")]
        public short? TitleId { get; set; }
        public string Name { get; set; }
       
      
        [Column("main_phone")]
        public string MainPhone { get; set; }
        [Column("main_phone_owner")]
        public string MainPhoneOwner { get; set; }
        [Column("invoice_entity_id")]
        public long? InvoiceEntityId { get; set; }
        [Column("have_different_ie_id")]
        public short? HaveDifferentIEId { get; set; }
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
        public short? VisitRequestingPersonRelationId { get; set; }
        public string Fax { get; set; }

        public string Email { get; set; }
        public string Email2 { get; set; }
        [Column("lead_source_id")]
        public short? LeadSourceId { get; set; }
        [Column("lead_status_id")]
        public short? LeadStatusId { get; set; }
        [Column("language_id")]
        public short? LanguageId { get; set; }
        [Column("lead_category_id")]
        public short? LeadCategoryId { get; set; }
        [Column("contact_method_id")]
        public short? ContactMethodId { get; set; }
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [Column("country_of_birth_id")]
        public short? CountryOfBirthId { get; set; }
        [Column("preferred_payment_method_id")]
        public short? PreferredPaymentMethodId { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }
        [Column("insurance_cover_id")]
        public short? InsuranceCoverId { get; set; }
        [Column("listed_discount_network_id")]
        public short? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        [Column("gp_code")]
        public string GPCode { get; set; }
        [Column("address_street_name")]
        public string AddressStreetName { get; set; }
        [Column("post_code")]
        public string PostalCode { get; set; }
        [Column("city_id")]
        public short? CityId { get; set; }
        [Column("country_id")]
        public short? CountryId { get; set; }
        [Column("building_type_id")]
        public short? BuildingTypeId { get; set; }
        [Column("flat_no")]
        public short? FlatNumber { get; set; }
        public string Buzzer { get; set; }
        public short? Floor { get; set; }
        [Column("visit_venue_id")]
        public short? VisitVenueId { get; set; }
        [Column("address_notes")]
        public string AddressNotes { get; set; }
        [Column("visit_venue_detail")]
        public string VisitVenueDetail { get; set; }
        [Column("lead_description")]
        public string LeadDescription { get; set; }
        [Column("bank_name")]
        public string BankName { get; set; }
        [Column("account_number")]
        public string AccountNumber { get; set; }
        [Column("sort_code")]
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        [Column("blacklist_id")]
        public short? BlacklistId { get; set; }

        [Column("customer_id")]
        public long? CustomerId { get; set; }
        [Column("converted_at")]
        public DateTime? ConvertDate { get; set; }
        public ICollection<LeadServiceRelation> Services { get; set; }

    }
}