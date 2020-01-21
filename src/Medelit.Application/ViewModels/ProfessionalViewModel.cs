using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class ProfessionalViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public short TitleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public short AccountingCodeId { get; set; }
        public string Website { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string Email2 { get; set; }
        public long FieldId { get; set; }
        public long SubCategoryId { get; set; }
        public string Fax { get; set; }
        public string CoverMap { get; set; }
        public string StreetName { get; set; }
        public short CityId { get; set; }
        public string PostCode { get; set; }
        public short CountryId { get; set; }
        public string Description { get; set; }
        public string ClinicStreetName { get; set; }
        public string ClinicPostCode { get; set; }
        public short? ClinicCityId { get; set; }
        public string ClinicPhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public string InvoicingNotes { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string WorkPlace { get; set; }
        public string ColleagueReferring { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public short ActiveCollaborationId { get; set; }
        public short ClinicAgreement { get; set; }
        public short ApplicationMethodId { get; set; }
        public short ApplicationMeansId { get; set; }
        public DateTime FirstContactDate { get; set; }
        public DateTime? LastContactDate { get; set; }
        public short ContractStatusId { get; set; }
        public short DocumentListSentId { get; set; }
        public short CalendarActivation { get; set; }
        public string ProOnlineCV { get; set; }
        public string ProtaxCode { get; set; }
        public IEnumerable<FilterModel> Languages { get; set; }
        public IEnumerable<ServiceProfessionalRelationVeiwModel> ProfessionalServices { get; set; }
    }
}
