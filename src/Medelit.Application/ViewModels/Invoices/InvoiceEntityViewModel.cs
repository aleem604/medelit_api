using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
  public  class InvoiceEntityViewModel : BaseViewModel
    {
        public string IENumber { get; set; }
        public string Name { get; set; }
        public string MainPhoneNumber { get; set; }
        public string MainPhoneNumberOwner { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public string Phone3 { get; set; }
        public string Phone3Owner { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public short? RatingId { get; set; }
        public short? RelationshipWithCustomerId { get; set; }
        public int? IETypeId { get; set; }
        public string Fax { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short? CountryOfBirthId { get; set; }
        public string BillingAddress { get; set; }
        public string MailingAddress { get; set; }
        public string BillingPostCode { get; set; }
        public string MailingPostCode { get; set; }
        public int? BillingCityId { get; set; }
        public int? MailingCityId { get; set; }
        public int? BillingCountryId { get; set; }
        public int? MailingCountryId { get; set; }
        public string Description { get; set; }
        public string VatNumber { get; set; }
        public int? PaymentMethodId { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public short? InsuranceCoverId { get; set; }
        public string InvoicingNotes { get; set; }
        public int? DiscountNetworkId { get; set; }
        public string PersonOfReference { get; set; }
        public string PersonOfReferenceEmail { get; set; }
        public string PersonOfReferencePhone { get; set; }
        public short? BlackListId { get; set; }
        public short? ContractedId { get; set; }

    }
}
