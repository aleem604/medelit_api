using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class ProfessionalRequestViewModel : BaseViewModel
    {
        public string Code { get; set; }
        public int TitleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public int AccountingCodeId { get; set; }
        public string Website { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string Email2 { get; set; }
        public string Fax { get; set; }
        public string CoverMap { get; set; }
        public string StreetName { get; set; }
        public int CityId { get; set; }
        public string PostCode { get; set; }
        public int CountryId { get; set; }
        public string Description { get; set; }
        public string ClinicStreetName { get; set; }
        public string ClinicPostCode { get; set; }
        public int? ClinicCityId { get; set; }
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
        public int ActiveCollaborationId { get; set; }
        public short ClinicAgreement { get; set; }
        public int ApplicationMethodId { get; set; }
        public int ApplicationMeansId { get; set; }
        public DateTime FirstContactDate { get; set; }
        public DateTime? LastContactDate { get; set; }
        public int ContractStatusId { get; set; }
        public int DocumentListSentId { get; set; }
        public short CalendarActivation { get; set; }
        public string ProOnlineCV { get; set; }
        public string ProtaxCode { get; set; }
        public long? AssignedToId { get; set; }
        public IList<FilterModel> Languages { get; set; }
    }
}
