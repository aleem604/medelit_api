using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Application
{
    public class LeadViewModel : BaseViewModel
    {
        [Required]
        public string SurName { get; set; }
        [Required]
        public short TitleId { get; set; }
        [Required]
        public string Name { get; set; }
        public string MainPhone { get; set; }
        public string MainPhoneOwner { get; set; }
        public long? InvoiceEntityId { get; set; }
        public short? HaveDifferentIEId { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string ContactPhone { get; set; }
        public string VisitRequestingPerson { get; set; }
        public short? VisitRequestingPersonRelationId { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public short? LeadSourceId { get; set; }
        public short? LeadStatusId { get; set; }
        [Required]
        public short LanguageId { get; set; }
        public short? LeadCategoryId { get; set; }
        public short? ContactMethodId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short? CountryOfBirthId { get; set; }
        public short? PreferredPaymentMethodId { get; set; }
        public string InvoicingNotes { get; set; }
        public short? InsuranceCoverId { get; set; }
        public short? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        public string GPCode { get; set; }
        [Required]
        public string AddressStreetName { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public short CityId { get; set; }
        [Required]
        public short CountryId { get; set; }
        public short? BuildingTypeId { get; set; }
        public short? FlatNumber { get; set; }
        public string Buzzer { get; set; }
        public short? Floor { get; set; }
        public short? VisitVenueId { get; set; }
        public string AddressNotes { get; set; }
        public string VisitVenueDetail { get; set; }
        public string LeadDescription { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public short? BlacklistId { get; set; }
        public long? CustomerId { get; set; }
        public string Customer { get; set; }
        public DateTime? ConvertDate { get; set; }
        public ICollection<LeadServiceRelationViewModel> Services { get; set; }
    }
}
