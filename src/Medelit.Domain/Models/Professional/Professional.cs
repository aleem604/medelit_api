using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("professional")]
    public class Professional : BaseEntity
    {
        public string Code { get; set; }
        [Column("title_id")]
        public short TitleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        [Column("accounting_code_id")]
        public short AccountingCodeId { get; set; }
        public string Website { get; set; }
        [Column("mobile_phone")]
        public string MobilePhone { get; set; }
        [Column("home_phone")]
        public string HomePhone { get; set; }
        public string Email2 { get; set; }
        [Column("field_id")]
        public long FieldId { get; set; }
        [ForeignKey("FieldId")]
        public FieldSubCategory Field { get; set; }

        [Column("subcategory_id")]
        public long SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public FieldSubCategory SubCategory { get; set; }

        public string Fax { get; set; }
        [Column("cover_map")]
        public string CoverMap { get; set; }
        [Column("street_name")]
        public string StreetName { get; set; }
        [Column("city_id")]
        public short CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
        [Column("post_code")]
        public string PostCode { get; set; }
        [Column("country_id")]
        public short CountryId { get; set; }
        [ForeignKey("CountryId")]
        public Country Country { get; set; }
        public string Description { get; set; }
        [Column("clinic_street_name")]
        public string ClinicStreetName { get; set; }
        [Column("clinic_post_code")]
        public string ClinicPostCode { get; set; }
        [Column("clinic_city_id")]
        public short? ClinicCityId { get; set; }
        [Column("clinic_phone_number")]
        public string ClinicPhoneNumber { get; set; }
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("company_number")]
        public string CompanyNumber { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        [Column("account_name")]
        public string AccountName { get; set; }
        [Column("account_number")]
        public string AccountNumber { get; set; }
        [Column("sort_code")]
        public string SortCode { get; set; }
        [Column("contract_date")]
        public DateTime? ContractDate { get; set; }
        [Column("contract_end_date")]
        public DateTime? ContractEndDate { get; set; }
        [Column("work_place")]
        public string WorkPlace { get; set; }
        [Column("colleague_referring")]
        public string ColleagueReferring { get; set; }
        [Column("insurance_expiry_date")]
        public DateTime? InsuranceExpiryDate { get; set; }

        [Column("active_collaboration_id")]
        public short ActiveCollaborationId { get; set; }
        [Column("clinic_agreement")]
        public short ClinicAgreement { get; set; }
        [Column("application_method_id")]
        public short ApplicationMethodId { get; set; }
        [Column("application_means_id")]
        public short ApplicationMeansId { get; set; }
        [Column("first_contact_date")]
        public DateTime? FirstContactDate { get; set; }
        [Column("last_contat_date")]
        public DateTime? LastContactDate { get; set; }
        [Column("contract_status_id")]
        public short ContractStatusId { get; set; }
        [Column("document_list_sent_id")]
        public short DocumentListSentId { get; set; }
        [Column("calendar_activation")]
        public short CalendarActivation { get; set; }
        [Column("pro_online_cv")]
        public string ProOnlineCV { get; set; }
        [Column("pro_tax_code")]
        public string ProtaxCode { get; set; }

        public ICollection<ProfessionalLanguages> ProfessionalLangs { get; set; }
        public ICollection<ProfessionalPtFees> ProfessionalPtFees { get; set; }
        public ICollection<ProfessionalProFees> ProfessionalProFees { get; set; }

    }
}