using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class BookingViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int? SrNo { get; set; }
        public long? InvoiceEntityId { get; set; }
        public short? BookingStatusId { get; set; }
        public DateTime? BookingDate { get; set; }
        public short? BookingTypeId { get; set; }
        public short? VisitLanguageId { get; set; }
        public short? VisitVenueId { get; set; }
        public string VisitVenueDetail { get; set; }
        public string AddressNotes { get; set; }
        public string Buzzer { get; set; }
        public string VisitRequestingPerson { get; set; }
        public short? VisitRequestingPersonRelationId { get; set; }
        public short? FlatNumber { get; set; }
        public short? Floor { get; set; }
        public short? BuildingTypeId { get; set; }
        public string VisitStreetName { get; set; }

        public string HomeStreetName { get; set; }
        public string HomePostCode { get; set; }
        public string VisitPostCode { get; set; }
        public string HomeCity { get; set; }
        public string VisitCity { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone2 { get; set; }
        public string Phone2Owner { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short? CountryOfBirthId { get; set; }

        public short? HomeCountryId { get; set; }
        public short? VisitCountryId { get; set; }
        public string Details { get; set; }
        public string Diagnosis { get; set; }
        public string ReasonForVisit { get; set; }
        public short? ImToProId { get; set; }
        public short? MailToPtId { get; set; }
        public short? PtCalledForAppointmentId { get; set; }
        public short? PaymentConcludedId { get; set; }
        public short? PaymentMethodId { get; set; }
        public short? AddToAccountingId { get; set; }
        public short? PaymentStatusId { get; set; }
        public short? CCAuthorizationId { get; set; }
        public short? BankTransfterReceiptId { get; set; }
        public string CCOwner { get; set; }
        public DateTime? PaymentArrivalDate { get; set; }
        public decimal? CashReturn { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoicingNotes { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public string NotesOnPayment { get; set; }
        public short? ReportDeliveredId { get; set; }
        public short? AddToProAccountId { get; set; }
        public short? InsuranceCoverId { get; set; }
        public string FeedbackFromPro { get; set; }
        public short? ProAvailabilityAskedId { get; set; }
        public decimal? LabCostsForMedelit { get; set; }
        public DateTime? DateOnPrescription { get; set; }
        public string Lab { get; set; }
        public int? LabId { get; set; }
        public short? Vials { get; set; }
        public int? RepeadPrescriptionNumber { get; set; }
        public int? PrescriptionNumber { get; set; }
        public string Notes { get; set; }
        public decimal? PrivateFee { get; set; }
        public decimal? TicketFee { get; set; }
        public string ExcemptionCode { get; set; }
        public short? NHSOrPrivateId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? PatientDiscount { get; set; }
        public decimal? GrossTotal { get; set; }
        public bool? IsAllDayVisit { get; set; }
        public DateTime? VisitStartDate { get; set; }
        public DateTime? VisitEndDate { get; set; }

        public decimal? ProDiscount { get; set; }
        public short? CashConfirmationMailId { get; set; }

        public short? PatientAge { get; set; }
        public short? Cycle { get; set; }
        public short? CycleNumber { get; set; }
        public string ProInvoiceNumber { get; set; }

        public decimal? TotalDue { get; set; }
        public decimal? TotalPaid { get; set; }
        public long? CustomerId { get; set; }
        public long ServiceId { get; set; }
        public decimal? TaxType { get; set; }
        public long ProfessionalId { get; set; }

        public long PtFeeId { get; set; }
        public short IsPtFeeA1 { get; set; }
        public decimal? PtFeeA1 { get; set; }
        public decimal? PtFeeA2 { get; set; }

        public long ProFeeId { get; set; }
        public short IsProFeeA1 { get; set; }
        public decimal? ProFeeA1 { get; set; }
        public decimal? ProFeeA2 { get; set; }

        public short? QuantityHours { get; set; }
        public string ItemNameOnInvoice { get; set; }

        public string CustomerName { get; set; }
        public string InvoiceEntityName { get; set; }
        public long? InvoiceId { get; set; }

        public string BookingName
        {
            get
            {
                if (SrNo > 0)
                    return $"{Name} {SrNo}";
                else
                    return Name;
            }
        }
    }
}
