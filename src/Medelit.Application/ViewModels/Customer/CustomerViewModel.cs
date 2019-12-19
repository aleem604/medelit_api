using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class CustomerViewModel : BaseViewModel
    {
        public short? TitleId { get; set; }
        public string SurName { get; set; }
        public string Name { get; set; }
        public short? LanguageId { get; set; }
        public string MainPhone { get; set; }
        public string MainPhoneOwner { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Fax { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short? CountryOfBirthId { get; set; }
        public string VisitRequestingPerson { get; set; }
        public short? VisitRequestingPersonRelationId { get; set; }
        public string HomeStreetName { get; set; }
        public string VisitStreetName { get; set; }
        public short? HomeCityId { get; set; }
        public short? VisitCityId { get; set; }
        public string HomePostCode { get; set; }
        public string VisitPostCode { get; set; }
        public short? HomeCountryId { get; set; }
        public short? VisitCountryId { get; set; }
        public string GPCode { get; set; }
        public short? VisitVenueId { get; set; }
        public string VisitVenueDetail { get; set; }
        public string Buzzer { get; set; }
        public short? FlatNumber { get; set; }
        public short? Floor { get; set; }
        public int? BuildingTypeId { get; set; }
        public short? ContactMethodId { get; set; }
        public string AddressNotes { get; set; }
        public short? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public short? InsuranceCoverId { get; set; }
        public short? HaveDifferentIEId { get; set; }
        public long? InvoiceEntityId { get; set; }
        public short? PaymentMethodId { get; set; }
        public string InvoicingNotes { get; set; }
        public short? BlacklistId { get; set; }
        public long? LeadId { get; set; }
        //public ICollection<CustomerServiceRelationViewModel> Services { get; set; }
    }
}
