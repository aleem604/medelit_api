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
        [Column("main_phone_owner")]
        public string MainPhoneNumberOwner { get; set; }
        public string Phone2 { get; set; }
        [Column("phone2_owner")]
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        [Column("phone3_owner")]
        public string Phone3Owner { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        [Column("rating_id")]
        public short? RatingId { get; set; }
        [Column("relationship_with_customer_id")]
        public short? RelationshipWithCustomerId { get; set; }
        [Column("ie_type_id")]
        public short? IETypeId { get; set; }
        public string Fax { get; set; }
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("country_of_birth_id")]
        public short? CountryOfBirthId { get; set; }

        [Column("billing_address")]
        public string BillingAddress { get; set; }

        [Column("mailing_address")]
        public string MailingAddress { get; set; }

        [Column("billing_post_code")]
        public string BillingPostCode { get; set; }

        [Column("mailing_post_code")]
        public string MailingPostCode { get; set; }

        [Column("billing_city")]
        public string BillingCity { get; set; } = "London";

        [Column("mailing_city")]
        public string MailingCity { get; set; } = "London";

        [Column("billing_country_id")]
        public short? BillingCountryId { get; set; } = 1003;
        [ForeignKey("BillingCountryId")]
        public Country BillingCountry { get; set; }

        [Column("mailing_country_id")]
        public short? MailingCountryId { get; set; } = 1003;
        [ForeignKey("MailingCountryId")]
        public Country MailingCountry { get; set; }

        public string Description { get; set; }

        [Column("vat_number")]
        public string VatNumber { get; set; }

        [Column("payment_method_id")]
        public short? PaymentMethodId { get; set; }

        public string Bank { get; set; }

        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Column("sort_code")]
        public string SortCode { get; set; }

        public string IBAN { get; set; }

        [Column("insurance_cover_id")]
        public short? InsuranceCoverId { get; set; }

        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }

        [Column("discount_network_id")]
        public short? DiscountNetworkId { get; set; }

        [Column("person_of_reference")]
        public string PersonOfReference { get; set; }

        [Column("person_of_reference_email")]
        public string PersonOfReferenceEmail { get; set; }

        [Column("person_of_reference_telephone")]
        public string PersonOfReferencePhone { get; set; }

        [Column("black_list_id")]
        public short? BlackListId{ get; set; }

        [Column("contracted_id")]
        public short? ContractedId { get; set; }

        /// connected customers
        /// connected services
    }
}