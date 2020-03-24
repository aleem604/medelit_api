using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Application
{
    public class CustomerViewModel : BaseViewModel
    {
        [Required]
        public short TitleId { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public string Name { get; set; }
        public long? InvoiceEntityId { get; set; }
        public short? BlacklistId { get; set; }
        public string Phone2 { get; set; }
        [Required]
        public string MainPhone { get; set; }
        public string Phone2Owner { get; set; }
        public string MainPhoneOwner { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string Fax { get; set; }
        [Required]
        public short LanguageId { get; set; }
        [Required]
        public short LeadSourceId { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        public short? CountryOfBirthId { get; set; }
        public short? VisitRequestingPersonRelationId { get; set; }
        public string VisitRequestingPerson { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public string HomeStreetName { get; set; }
        [Required]
        public string VisitStreetName { get; set; }
        [Required]
        public short HomeCityId { get; set; }
        [Required]
        public short VisitCityId { get; set; }
        [Required]
        public string HomePostCode { get; set; }
        [Required]
        public string VisitPostCode { get; set; }
        [Required]
        public short HomeCountryId { get; set; }
        [Required]
        public short VisitCountryId { get; set; }
        public string VisitVenueDetail { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        [Required]
        public short InsuranceCoverId { get; set; }
        public short? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        public string GPCode { get; set; }
        public string Buzzer { get; set; }
        public short? FlatNumber { get; set; }
        public short? Floor { get; set; }
        [Required]
        public short BuildingTypeId { get; set; }
        [Required]
        public short VisitVenueId { get; set; }
        [Required]
        public short? ContactMethodId { get; set; }
        public string AddressNotes { get; set; }
        [Required]
        public short PaymentMethodId { get; set; }
        public string InvoicingNotes { get; set; }
        [Required]
        public short? HaveDifferentIEId { get; set; } = 0;
        public long? LeadId { get; set; }
        public ICollection<CustomerServiceRelationViewModel> Services { get; set; }
    }
}
