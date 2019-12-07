using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class LeadViewModel : BaseViewModel
    {
        public string SurName { get; set; }
        public int? TitleId { get; set; }
        public string Name { get; set; }
        public long RequestedServiceId { get; set; }
        public string MainPhone { get; set; }
        public string MainPhoneOwner { get; set; }
        public long? InvoiceEntityId { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string ContactPhone { get; set; }
        public string VisitRequestingPerson { get; set; }
        public int? VisitRequestingPersonRelationId { get; set; }
        public string Fax { get; set; }
        public long ProfessionalId { get; set; }
        public string Email { get; set; }
        public int? LeadSourceId { get; set; }
        public int? LeadStatusId { get; set; }
        public int? LanguageId { get; set; }
        public int? LeadCategoryId { get; set; }
        public int? ContactMethodId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CountryOfBirthId { get; set; }
        public int? PreferredPaymentMethodId { get; set; }
        public string InvoicingNotes { get; set; }
        public short? InsuranceCover { get; set; }
        public int? ListedDiscountNetworkId { get; set; }
        public decimal? Discount { get; set; }
        public string GPCode { get; set; }
        public string AddressStreetName { get; set; }
        public string PostalCode { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public int? BuildingTypeId { get; set; }
        public short? FlatNumber { get; set; }
        public string Buzzer { get; set; }
        public int? Floor { get; set; }
        public int? VisitVenueId { get; set; }
        public string AddressNotes { get; set; }
        public string VisitVenueDetail { get; set; }
        public string Description { get; set; }
        public long? FromCustomerId { get; set; }
        public DateTime? ConvertDate { get; set; }
    }
}
