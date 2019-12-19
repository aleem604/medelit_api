using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    public class Booking : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("invoice_entity_id")]
        public long? InvoiceEntityId { get; set; }
        [Column("booking_status_id")]
        public short? BookingStatusId { get; set; }
        [Column("booking_date")]
        public DateTime? BookingDate { get; set; }
        [Column("booking_type_id")]
        public short? BookingTypeId { get; set; }
        [Column("visit_language_id")]
        public short? VisitLanguageId { get; set; }
        [Column("visit_venue_id")]
        public short? VisitVenueId { get; set; }
        [Column("visit_venue_detail")]
        public string VisitVenueDetail { get; set; }
        [Column("address_notes")]
        public string AddressNotes { get; set; }
        public string Buzzer { get; set; }
        [Column("visit_requesting_person")]
        public string VisitRequestingPerson { get; set; }
        [Column("visit_requestig_person_relation_id")]
        public short? VisitRequestingPersonRelationId { get; set; }
        [Column("flat_no")]
        public short? FlatNumber { get; set; }
        public short? Floor { get; set; }
        [Column("building_type_id")]
        public short? BuildingTypeId { get; set; }
        [Column("visit_street_name")]
        public string VisitStreetName { get; set; }
        
        [Column("home_street_name")]
        public string HomeStreetName { get; set; }
        [Column("home_post_code")]
        public string HomePostCode { get; set; }
        [Column("visit_post_code")]
        public string VisitPostCode { get; set; }
        [Column("home_city_id")]
        public short? HomeCityId { get; set; }
        [Column("visit_city_id")]
        public short? VisitCityId { get; set; }
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone2 { get; set; }
        [Column("phone2_owner")]
        public string Phone2Owner { get; set; }
        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [Column("country_of_birth_id")]
        public short? CountryOfBirthId { get; set; }

        [Column("home_country_id")]
        public short? HomeCountryId { get; set; }
        [Column("visit_country_id")]
        public short? VisitCountryId { get; set; }
        public string Details { get; set; }
        public string Diagnosis { get; set; }
        [Column("reason_for_visit")]
        public string ReasonForVisit { get; set; }
        [Column("im_to_pro_id")]
        public short? ImToProId { get; set; }
        [Column("mail_to_pt_id")]
        public short? MailToPtId { get; set; }
        [Column("pt_called_for_appointment_id")]
        public short? PtCalledForAppointmentId { get; set; }
        [Column("payment_concluded_id")]
        public short? PaymentConcludedId { get; set; }
        [Column("payment_method_id")]
        public short? PaymentMethodId { get; set; }
        [Column("added_to_accounting_id")]
        public short? AddToAccountingId { get; set; }
        [Column("payment_status_id")]
        public short? PaymentStatusId { get; set; }
        [Column("cc_authorization_id")]
        public short? CCAuthorizationId { get; set; }
        [Column("bank_transfer_receipt_id")]
        public short? BankTransfterReceiptId { get; set; }
        [Column("cc_owner")]
        public string CCOwner { get; set; }
        [Column("payment_arrival_date")]
        public DateTime? PaymentArrivalDate { get; set; }
        [Column("cash_return")]
        public decimal? CashReturn { get; set; }
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }
        [Column("invoice_due_date")]
        public DateTime? InvoiceDueDate { get; set; }
        [Column("notes_on_payment")]
        public string NotesOnPayment { get; set; }
        [Column("report_delivered_id")]
        public short? ReportDeliveredId { get; set; }
        [Column("added_to_pro_account_id")]
        public short? AddToProAccountId { get; set; }
        [Column("insurance_cover_id")]
        public short? InsuranceCoverId { get; set; }
        [Column("feedback_from_pro")]
        public string FeedbackFromPro { get; set; }
        [Column("pro_availability_asked_id")]
        public short? ProAvailabilityAskedId { get; set; }
        [Column("lab_costs_for_medelit")]
        public decimal? LabCostsForMedelit { get; set; }
        [Column("date_on_prescription")]
        public DateTime? DateOnPrescription { get; set; }
        public string Lab { get; set; }
        public short? Vials { get; set; }
        [Column("repeat_prescription_number")]
        public int? RepeadPrescriptionNumber { get; set; }
        [Column("prescription_number")]
        public int? PrescriptionNumber { get; set; }
        public string Notes { get; set; }
        [Column("private_fee")]
        public decimal? PrivateFee { get; set; }
        [Column("ticket_fee")]
        public decimal? TicketFee { get; set; }
        [Column("excemption_code")]
        public string ExcemptionCode { get; set; }
        [Column("nhs_or_private_id")]
        public short? NHSOrPrivateId { get; set; }
        [Column("tax_type")]
        public short? TaxType { get; set; }
        [Column("sub_total")]
        public decimal? SubTotal { get; set; }
        [Column("tax_amount")]
        public decimal? TaxAmount { get; set; }
        [Column("patient_discount")]
        public decimal? PatientDiscount { get; set; }
        [Column("gross_total")]
        public decimal? GrossTotal { get; set; }
        [Column("visit_date")]
        public DateTime? VisitDate { get; set; }
        [Column("visit_time")]
        public string VisitTime { get; set; }
        [Column("pro_discount")]
        public decimal? ProDiscount { get; set; }
        [Column("cash_confirmation_email_id")]
        public short? CashConfirmationMailId { get; set; }
        [Column("quantity_hours")]
        public short? QuantityHours { get; set; }
        [Column("patient_age")]
        public short? PatientAge { get; set; }
        public short? Cycle { get; set; }
        [Column("cycle_number")]
        public short? CycleNumber { get; set; }
        [Column("pro_invoice_number")]
        public string ProInvoiceNumber { get; set; }
        [Column("booking_time")]
        public string BookingTime { get; set; }
        [Column("total_due")]
        public decimal? TotalDue { get; set; }
        [Column("total_paid")]
        public decimal? TotalPaid { get; set; }
        [Column("customer_id")]
        public long? CustomerId { get; set; }

        [Column("assigned_to_id")]
        public long? AssignedToId { get; set; }

        public ICollection<BookingServiceRelation> Services { get; set; }
    }
}