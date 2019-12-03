using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("invoice_entity")]
    public class InvoiceEntity : BaseEntity
    {
        [Column("ie_number")]
        public string IENumber { get; set; }
        public string Name { get; set; }
        [Column("main_phone_number")]
        public string MainPhoneNumber { get; set; }
        [Column("main_phone_number_owner")]
        public string MainPhoneNumberOwner { get; set; }
        public string Phone2 { get; set; }
        [Column("phone2_owner")]
        public string Phone2Owner { get; set; }
        public string Email { get; set; }
        public string phone3 { get; set; }
        [Column("phone3_owner")]
        public string Phone3Owner { get; set; }
        public string Email2 { get; set; }
        [Column("rating_id")]
        public int? RatingId { get; set; }
        [Column("relationship_with_customer_id")]
        public int? RelationshipWithCustomerId { get; set; }
        [Column("ie_type_id")]
        public int? IETypeId { get; set; }
        public string Fax { get; set; }
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [Column("contry_of_birth_id")]
        public int? CountryOfBirthId { get; set; }
        [Column("billing_address")]
        public string BillingAddress { get; set; }
        [Column("mailing_address")]
        public string MailingAddress { get; set; }
        [Column("billing_post_code")]
        public string BillingPostCode { get; set; }
        [Column("billing_city_id")]
        public int? BillingCityId { get; set; }
        [Column("mailing_city_id")]
        public int? MailingCityId { get; set; }
        [Column("billing_country_id")]
        public int? BillingCountryId { get; set; }
        [Column("mailing_country_id")]
        public int? MailingCountryId { get; set; }
        [Column("mailing_post_code")]
        public string MailingPostCode { get; set; }
        public string Description { get; set; }
        [Column("vat_id")]
        public int? VatId { get; set; }
        [Column("payment_conditions_id")]
        public int? PaymentConditionsId { get; set; }
        public string Bank { get; set; }
        [Column("account_number")]
        public string AccountNumber { get; set; }
        [Column("sort_code")]
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        [Column("insurance_cover")]
        public bool? InsuranceCover { get; set; }
        [Column("listed_discount_network_id")]
        public int? ListedDiscountNetworkId { get; set; }
        [Column("person_of_reference")]
        public string PersonOfReference { get; set; }
        [Column("person_of_reference_email")]
        public string PersonOfReferenceEmail { get; set; }
        [Column("discount_percent")]
        public decimal? DiscountPercent { get; set; }
        [Column("assigned_to_id")]
        public long? AssignedToId { get; set; }

        /// connected customers
        /// connected services
    }
}