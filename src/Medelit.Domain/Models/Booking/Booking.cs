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
        [Column("sr_no")]
        public int? SrNo { get; set; }

        [Column("invoice_entity_id")]
        public long? InvoiceEntityId { get; set; }
        [ForeignKey("InvoiceEntityId")]
        public InvoiceEntity InvoiceEntity { get; set; }
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
        [Column("lab_id")]
        public int? LabId { get; set; }
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
        [Column("is_all_day_visit")]
        public short? IsAllDayVisit { get; set; }
        [Column("visit_start_date")]
        public DateTime? VisitStartDate { get; set; }
        [Column("visit_end_date")]
        public DateTime? VisitEndDate { get; set; }


        [Column("pro_discount")]
        public decimal? ProDiscount { get; set; }
        [Column("cash_confirmation_email_id")]
        public short? CashConfirmationMailId { get; set; }

        [Column("patient_age")]
        public short? PatientAge { get; set; }
        public short? Cycle { get; set; }
        [Column("cycle_number")]
        public short? CycleNumber { get; set; }
        [Column("pro_invoice_number")]
        public string ProInvoiceNumber { get; set; }

        [Column("total_due")]
        public decimal? TotalDue { get; set; }
        [Column("total_paid")]
        public decimal? TotalPaid { get; set; }
        [Column("customer_id")]
        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Column("service_id")]
        public long ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }


        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public Professional Professional { get; set; }

        [Column("pt_fee_id")]
        public long PtFeeId { get; set; }
        public PtFee PtFees { get; set; }

        [Column("is_pt_fee")]
        public short IsPtFeeA1 { get; set; }

        [Column("pt_fee_a1")]
        public decimal? PtFeeA1 { get; set; }
        [Column("pt_fee_a2")]
        public decimal? PtFeeA2 { get; set; }


        [Column("pro_fee_id")]
        public long ProFeeId { get; set; }
        public ProFee ProFees { get; set; }

        [Column("is_pro_fee")]
        public short IsProFeeA1 { get; set; }

        [Column("pro_fee_a1")]
        public decimal? ProFeeA1 { get; set; }
        [Column("pro_fee_a2")]
        public decimal? ProFeeA2 { get; set; }

        [Column("quantity_hours")]
        public short? QuantityHours { get; set; }

        [Column("cycle_booking_id")]
        public long? CycleBookingId { get; set; }

        [Column("invoice_id")]
        public long? InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        [Column("is_validated")]
        public bool IsValidated { get; set; }

        [NotMapped]
        public decimal? PtFee
        {
            get
            {
                return IsPtFeeA1 == 1 ? PtFeeA1 : PtFeeA2;
            }
        }

        [NotMapped]
        public decimal? ProFee
        {
            get
            {
                return IsProFeeA1 == 1 ? ProFeeA1 : ProFeeA2;
            }
        }

        public Booking Clone()
        {
            return new Booking
            {
                CustomerId = this.CustomerId,
                Name = this.Name,
                InvoiceEntityId = this.InvoiceEntityId,
                BookingStatusId = this.BookingStatusId,
                BookingDate = this.BookingDate,
                BookingTypeId = this.BookingTypeId,
                VisitLanguageId = this.VisitLanguageId,
                VisitVenueId = this.VisitVenueId,
                VisitVenueDetail = this.VisitVenueDetail,
                AddressNotes = this.AddressNotes,
                Buzzer = this.Buzzer,
                VisitRequestingPerson = this.VisitRequestingPerson,
                VisitRequestingPersonRelationId = this.VisitRequestingPersonRelationId,
                FlatNumber = this.FlatNumber,
                Floor = this.Floor,
                BuildingTypeId = this.BuildingTypeId,
                HomeStreetName = this.HomeStreetName,
                VisitStreetName = this.VisitStreetName,
                HomePostCode = this.HomePostCode,
                VisitPostCode = this.VisitPostCode,
                HomeCityId = this.HomeCityId,
                VisitCityId = this.VisitCityId,
                PhoneNumber = this.PhoneNumber,
                Email = this.Email,
                Phone2 = this.Phone2,
                Phone2Owner = this.Phone2Owner,
                DateOfBirth = this.DateOfBirth,
                CountryOfBirthId = this.CountryOfBirthId,
                HomeCountryId = this.HomeCountryId,
                VisitCountryId = this.VisitCountryId,
                Details = this.Details,
                Diagnosis = this.Diagnosis,
                ReasonForVisit = this.ReasonForVisit,
                ImToProId = this.ImToProId,
                PtCalledForAppointmentId = this.PtCalledForAppointmentId,
                PaymentConcludedId = this.PaymentConcludedId,
                PaymentMethodId = this.PaymentMethodId,
                AddToAccountingId = this.AddToAccountingId,
                PaymentStatusId = this.PaymentStatusId,
                CCAuthorizationId = this.CCAuthorizationId,
                BankTransfterReceiptId = this.BankTransfterReceiptId,
                CCOwner = this.CCOwner,
                PaymentArrivalDate = this.PaymentArrivalDate,
                CashReturn = this.CashReturn,
                InvoiceNumber = this.InvoiceNumber,
                InvoicingNotes = this.InvoicingNotes,
                InvoiceDueDate = this.InvoiceDueDate,
                NotesOnPayment = this.NotesOnPayment,
                ReportDeliveredId = this.ReportDeliveredId,
                AddToProAccountId = this.AddToProAccountId,
                InsuranceCoverId = this.InsuranceCoverId,
                FeedbackFromPro = this.FeedbackFromPro,
                ProAvailabilityAskedId = this.ProAvailabilityAskedId,
                LabCostsForMedelit = this.LabCostsForMedelit,
                DateOnPrescription = this.DateOnPrescription,
                Lab = this.Lab,
                Vials = this.Vials,
                RepeadPrescriptionNumber = this.RepeadPrescriptionNumber,
                PrescriptionNumber = this.PrescriptionNumber,
                Notes = this.Notes,
                PrivateFee = this.PrivateFee,
                TicketFee = this.TicketFee,
                ExcemptionCode = this.ExcemptionCode,
                NHSOrPrivateId = this.NHSOrPrivateId,
                TaxType = this.TaxType,
                SubTotal = this.SubTotal,
                TaxAmount = this.TaxAmount,
                PatientDiscount = this.PatientDiscount,
                GrossTotal = this.GrossTotal,
                IsAllDayVisit = this.IsAllDayVisit,
                VisitStartDate = this.VisitStartDate,
                VisitEndDate = this.VisitEndDate,


                ProDiscount = this.ProDiscount,
                CashConfirmationMailId = this.CashConfirmationMailId,
                QuantityHours = this.QuantityHours,
                //DiscountNetworkId = this.DiscountNetworkId,
                PatientAge = this.PatientAge,
                Cycle = this.Cycle,
                CycleNumber = this.CycleNumber,
                ProInvoiceNumber = this.ProInvoiceNumber,
                TotalDue = this.TotalDue,
                TotalPaid = this.TotalPaid,
                ServiceId = this.ServiceId,
                //ProfessionalId = this.ProfessionalId,
                //IsPtFeeA1 = this.IsPtFeeA1,
                //PtFeeId = this.PtFeeId,
                //PtFeeA1 = this.PtFeeA1,
                //PtFeeA2 = this.PtFeeA2,

                //IsProFeeA1 = this.IsProFeeA1,
                //ProFeeId = this.ProFeeId,
                //ProFeeA1 = this.ProFeeA1,
                //ProFeeA2 = this.ProFeeA2,

                Status = this.Status,
                CreateDate = DateTime.UtcNow,
            };

        }

        public bool IsValid()
        {
            if ((InvoiceNumber == null && PaymentStatusId == 3 && PaymentConcludedId == 1 && PaymentMethodId != 4) || (InvoiceNumber == null && PaymentStatusId == 2 && (BookingStatusId == 4 || BookingStatusId == 6)))
            {
                return true;
            }
            return false;
        }
    }
}