using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Common
{
    public class LeadCSVViewModel
    {
        [Required]
        public string SurName { get; set; }
        [Required]
        public string Title { get; set; }
        public string Name { get; set; }
        public string MainPhone { get; set; }
        public string MainPhoneOwner { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string ContactPhone { get; set; }
        public string VisitRequestingPerson { get; set; }
        public string VisitRequestingPersonRelation { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string LeadSource { get; set; }
        public string LeadStatus { get; set; }
        [Required]
        public string Language { get; set; }
        public string LeadCategory { get; set; }
        public string ContactMethod { get; set; }
        public string DateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string PreferredPaymentMethod { get; set; }
        public string InvoicingNotes { get; set; }
        public string InsuranceCover { get; set; }
        public string ListedDiscountNetwork { get; set; }
        public decimal? Discount { get; set; }
        public string GPCode { get; set; }
        [Required]
        public string AddressStreetName { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public string BuildingType { get; set; }
        public short? FlatNumber { get; set; }
        public string Buzzer { get; set; }
        public short? Floor { get; set; }
        public string VisitVenue { get; set; }
        public string AddressNotes { get; set; }
        public string VisitVenueDetail { get; set; }
        public string LeadDescription { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public string Blacklist { get; set; }
    }
}
