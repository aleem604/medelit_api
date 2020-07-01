using System;
using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly IStaticDataRepository _static;
        private readonly ApplicationDbContext _appContext;

        public BookingRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus, IStaticDataRepository @static, ApplicationDbContext appContext)
            : base(context, contextAccessor, bus)
        {
            _static = @static;
            _appContext = appContext;
        }

        public void FindBookings(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
                if (viewModel.SearchOnly && string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    var res = new
                    {
                        items = new List<dynamic>(),
                        totalCount = 0
                    };
                    _bus.RaiseEvent(new DomainNotification(GetType().Name, null, res));
                    return;
                }

                var langs = Db.Languages.ToList();
                var invoicingEntities = Db.InvoiceEntity.ToList();
                var services = Db.Service.ToList();
                var professionals = Db.Professional.ToList();
                var paymentMethods = _static.GetPaymentMethods().ToList();
                var bookingStatus = _static.GetBookingStatus().ToList();
                var bookingType = _static.GetBookingTypes().ToList();
                var visitVenues = _static.GetVisitVenues().ToList();
                var relations = _static.GetRelationships().ToList();
                var buildingTypes = _static.GetBuildingTypes().ToList();
                var paymentStatus = _static.GetPaymentStatuses().ToList();
                var reportDeliveryOptions = _static.GetReportDeliveryOptions().ToList();
                var labs = Db.Lab.ToList();

                var query = (from b in Db.Booking

                             select new
                             {
                                 b.Id,
                                 Name = $"{b.Name} {b.SrNo}",
                                 b.CustomerId,
                                 Customer = $"{b.Customer.SurName} {b.Customer.Name}",
                                 b.InvoiceEntityId,
                                 InvoicingEntity = b.InvoiceEntityId.HasValue ? (invoicingEntities.FirstOrDefault(x => x.Id == b.InvoiceEntityId) ?? new InvoiceEntity()).Name : "",
                                 b.BookingStatusId,
                                 BookingStatus = b.BookingStatusId > 0 ? (bookingStatus.FirstOrDefault(x => x.Id == b.BookingStatusId) ?? new FilterModel()).Value : "",
                                 b.BookingDate,
                                 b.BookingTypeId,
                                 BookingType = b.BookingTypeId > 0 ? (bookingType.FirstOrDefault(x => x.Id == b.BookingTypeId) ?? new FilterModel()).Value : "",
                                 b.VisitLanguageId,
                                 VisitLanguage = b.VisitLanguageId.HasValue ? (langs.FirstOrDefault(x => x.Id == b.VisitLanguageId) ?? new Language()).Name : "",
                                 b.VisitVenueId,
                                 VisitVenue = b.VisitVenueId > 0 ? (visitVenues.FirstOrDefault(x => x.Id == b.VisitVenueId) ?? new FilterModel()).Value : "",
                                 b.VisitVenueDetail,
                                 b.AddressNotes,
                                 b.Buzzer,
                                 b.VisitRequestingPerson,
                                 b.VisitRequestingPersonRelationId,
                                 VisitRequestingPersonRelation = b.VisitRequestingPersonRelationId > 0 ? (relations.FirstOrDefault(x => x.Id == b.VisitRequestingPersonRelationId) ?? new FilterModel()).Value : "",
                                 b.FlatNumber,
                                 b.Floor,
                                 b.BuildingTypeId,
                                 BuildingType = b.BuildingTypeId > 0 ? (buildingTypes.FirstOrDefault(x => x.Id == b.BuildingTypeId) ?? new FilterModel()).Value : "",
                                 b.VisitStreetName,
                                 b.HomeStreetName,
                                 b.HomePostCode,
                                 b.VisitPostCode,
                                 b.HomeCity,
                                 b.VisitCity,
                                 b.PhoneNumber,
                                 b.Email,
                                 b.Phone2,
                                 b.Phone2Owner,
                                 b.DateOfBirth,
                                 b.CountryOfBirthId,
                                 CountryOfBirth = b.CountryOfBirthId.HasValue ? b.CountryOfBirth.Value : string.Empty,
                                 b.HomeCountryId,
                                 HomeCountry = b.HomeCountryId.HasValue ? b.HomeCountry.Value : string.Empty,
                                 b.VisitCountryId,
                                 VisitCountry = b.VisitCountryId.HasValue ? b.VisitCountry.Value : string.Empty,
                                 b.Details,
                                 b.Diagnosis,
                                 b.ReasonForVisit,
                                 b.ImToProId,
                                 b.MailToPtId,
                                 MailToPt = b.MailToPtId.HasValue && b.MailToPtId.Value == 1 ? "Yes" : "No",
                                 b.PtCalledForAppointmentId,
                                 PtCalledForAppointment = b.PtCalledForAppointmentId.HasValue && b.PtCalledForAppointmentId.Value == 1 ? "Yes" : "No",
                                 b.PaymentConcludedId,
                                 PaymentConcluded = b.PaymentConcludedId.HasValue && b.PaymentConcludedId.Value == 1 ? "Yes" : "No",
                                 b.PaymentMethodId,
                                 PaymentMethod = b.PaymentMethodId.HasValue ? (paymentMethods.FirstOrDefault(x => x.Id == b.PaymentMethodId) ?? new FilterModel()).Value : "",
                                 b.AddToAccountingId,
                                 AddToAccounting = b.AddToAccountingId.HasValue && b.AddToAccountingId.Value == 1 ? "Yes" : "No",
                                 b.PaymentStatusId,
                                 PaymentStatus = b.PaymentStatusId.HasValue ? (paymentStatus.FirstOrDefault(x => x.Id == b.PaymentStatusId) ?? new FilterModel()).Value : "",
                                 b.CCAuthorizationId,
                                 CCAuthorization = b.CCAuthorizationId.HasValue && b.CCAuthorizationId.Value == 1 ? "Yes" : "No",
                                 b.BankTransfterReceiptId,
                                 BankTransferReceipt = b.BankTransfterReceiptId.HasValue && b.BankTransfterReceiptId.Value == 1 ? "Yes" : "No",
                                 b.CCOwner,
                                 b.PaymentArrivalDate,
                                 b.InvoiceNumber,
                                 b.InvoicingNotes,
                                 b.InvoiceDueDate,
                                 b.NotesOnPayment,
                                 b.ReportDeliveredId,
                                 ReportDelivered = b.ReportDeliveredId > 0 ? (reportDeliveryOptions.FirstOrDefault(x => x.Id == b.ReportDeliveredId) ?? new FilterModel()).Value : "",
                                 b.AddToProAccountId,
                                 AddToProAccount = b.AddToProAccountId.HasValue && b.AddToProAccountId.Value == 1 ? "Yes" : "No",
                                 b.InsuranceCoverId,
                                 InsuranceCover = b.InsuranceCoverId.HasValue && b.InsuranceCoverId.Value == 1 ? "Yes" : "No",
                                 b.FeedbackFromPro,
                                 b.ProAvailabilityAskedId,
                                 ProAvailabilityAsked = b.ProAvailabilityAskedId.HasValue && b.ProAvailabilityAskedId.Value == 1 ? "Yes" : "No",
                                 b.LabCostsForMedelit,
                                 b.DateOnPrescription,
                                 b.Lab,
                                 b.LabId,
                                 Labs = b.LabId > 0 ? (labs.FirstOrDefault(x => x.Id == b.LabId) ?? new Lab()).Name : "",
                                 b.Vials,
                                 b.RepeadPrescriptionNumber,
                                 b.PrescriptionNumber,
                                 b.Notes,
                                 b.PrivateFee,
                                 b.TicketFee,
                                 b.ExcemptionCode,
                                 b.NHSOrPrivateId,
                                 NHSOrPrivate = b.NHSOrPrivateId.HasValue && b.NHSOrPrivateId.Value == 1 ? "Yes" : "No",
                                 b.PatientDiscount,
                                 AllDayVisit = b.IsAllDayVisit.HasValue && b.IsAllDayVisit.Value == 1 ? "Yes" : "No",
                                 b.VisitStartDate,
                                 VisitDate = b.VisitStartDate,
                                 b.VisitEndDate,
                                 b.ProDiscount,
                                 b.CashConfirmationMailId,
                                 CashConfirmationMail = b.CashConfirmationMailId.HasValue && b.CashConfirmationMailId.Value == 1 ? "Yes" : "No",
                                 b.PatientAge,
                                 b.Cycle,
                                 b.CycleNumber,
                                 b.ProInvoiceNumber,
                                 b.ServiceId,
                                 Service = b.ServiceId > 0 ? (services.FirstOrDefault(x => x.Id == b.ServiceId) ?? new Service()).Name : "",
                                 b.ProfessionalId,
                                 Professional = b.ProfessionalId > 0 ? (professionals.FirstOrDefault(x => x.Id == b.ProfessionalId) ?? new Professional()).Name : "",

                                 b.PtFeeId,
                                 b.PtFee,
                                 IsPtFeeA1 = b.IsPtFeeA1 == 1 ? "Yest" : "No",
                                 b.PtFeeA1,
                                 b.PtFeeA2,
                                 PtFees = b.PtFees.FeeName,

                                 b.ProFeeId,
                                 b.ProFee,
                                 IsProFeeA1 = b.IsProFeeA1 == 1 ? "Yest" : "No",
                                 b.ProFeeA1,
                                 b.ProFeeA2,
                                 ProFees = b.ProFees.FeeName,
                                 b.QuantityHours,
                                 b.ItemNameOnInvoice,
                                 b.TaxType,
                                 b.CashReturn,
                                 b.SubTotal,
                                 b.TaxAmount,
                                 b.GrossTotal,
                                 b.TotalPaid,
                                 b.TotalDue,
                                 b.InvoiceId,
                                 b.IsValidated,
                                 b.CreateDate,
                                 b.UpdateDate,
                                 AssignedTo = GetAssignedUser(b.AssignedToId)
                             });


                if (!string.IsNullOrEmpty(viewModel.Filter.Search))
                {
                    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                    query = query.Where(x =>
                    (
                        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Customer) && x.Customer.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingEntity) && x.InvoicingEntity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BookingStatus) && x.BookingStatus.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.BookingDate.HasValue && x.BookingDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BookingType) && x.BookingType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitLanguage) && x.VisitLanguage.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenue) && x.VisitVenue.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitVenueDetail) && x.VisitVenueDetail.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddressNotes) && x.AddressNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Buzzer) && x.Buzzer.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPerson) && x.VisitRequestingPerson.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitRequestingPersonRelation) && x.VisitRequestingPersonRelation.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.FlatNumber.HasValue && x.FlatNumber.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Floor.HasValue && x.Floor.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BuildingType) && x.BuildingType.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitStreetName) && x.VisitStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeStreetName) && x.HomeStreetName.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomePostCode) && x.HomePostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitPostCode) && x.VisitPostCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeCity) && x.HomeCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitCity) && x.VisitCity.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2) && x.Phone2.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Phone2Owner) && x.Phone2Owner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CountryOfBirth) && x.CountryOfBirth.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.HomeCountry) && x.HomeCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.VisitCountry) && x.VisitCountry.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Details) && x.Details.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Diagnosis) && x.Diagnosis.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ReasonForVisit) && x.ReasonForVisit.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.MailToPt) && x.MailToPt.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtCalledForAppointment) && x.PtCalledForAppointment.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PaymentConcluded) && x.PaymentConcluded.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PaymentMethod) && x.PaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddToAccounting) && x.AddToAccounting.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PaymentStatus) && x.PaymentStatus.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CCAuthorization) && x.CCAuthorization.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.BankTransferReceipt) && x.BankTransferReceipt.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CCOwner) && x.CCOwner.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PaymentArrivalDate.HasValue && x.PaymentArrivalDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoiceNumber) && x.InvoiceNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InvoicingNotes) && x.InvoicingNotes.CLower().Contains(viewModel.Filter.Search.CLower()))
                     || (x.InvoiceDueDate.HasValue && x.InvoiceDueDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.NotesOnPayment) && x.NotesOnPayment.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ReportDelivered) && x.ReportDelivered.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AddToProAccount) && x.AddToProAccount.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.InsuranceCover) && x.InsuranceCover.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.FeedbackFromPro) && x.FeedbackFromPro.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProAvailabilityAsked) && x.ProAvailabilityAsked.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.LabCostsForMedelit.HasValue && x.LabCostsForMedelit.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.DateOnPrescription.HasValue && x.DateOnPrescription.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Lab) && x.Lab.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Labs) && x.Labs.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.RepeadPrescriptionNumber.HasValue && x.RepeadPrescriptionNumber.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PrescriptionNumber.HasValue && x.PrescriptionNumber.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Notes) && x.Notes.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PrivateFee.HasValue && x.PrivateFee.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.TicketFee.HasValue && x.TicketFee.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ExcemptionCode) && x.ExcemptionCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.NHSOrPrivate) && x.NHSOrPrivate.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PatientDiscount.HasValue && x.PatientDiscount.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.VisitDate.HasValue && x.VisitDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.AllDayVisit) && x.AllDayVisit.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.VisitStartDate.HasValue && x.VisitStartDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.VisitEndDate.HasValue && x.VisitEndDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.ProDiscount.HasValue && x.ProDiscount.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.CashConfirmationMail) && x.CashConfirmationMail.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PatientAge.HasValue && x.PatientAge.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Cycle.HasValue && x.Cycle.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CycleNumber.HasValue && x.CycleNumber.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ProInvoiceNumber) && x.ProInvoiceNumber.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Service) && x.Service.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.Professional) && x.Professional.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtFees) && x.PtFees.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PtFeeA1.HasValue && x.PtFeeA1.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.PtFeeA2.HasValue && x.PtFeeA2.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.ProFeeA1.HasValue && x.ProFeeA1.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.ProFeeA2.HasValue && x.ProFeeA2.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.QuantityHours.HasValue && x.QuantityHours.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.ItemNameOnInvoice) && x.ItemNameOnInvoice.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.TaxType.HasValue && x.TaxType.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CashReturn.HasValue && x.CashReturn.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.SubTotal.HasValue && x.SubTotal.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.TaxAmount.HasValue && x.TaxAmount.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.GrossTotal.HasValue && x.GrossTotal.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.TotalPaid.HasValue && x.TotalPaid.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.TotalDue.HasValue && x.TotalDue.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))

                    || (!string.IsNullOrEmpty(x.AssignedTo) && x.AssignedTo.CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (!string.IsNullOrEmpty(x.PtFee.ToString()) && x.PtFee.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.CreateDate.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                    || (x.Id.ToString().Contains(viewModel.Filter.Search))

                    ));
                }

                if (viewModel.Filter.BookingFilter == eBookingFilter.Pending)
                {
                    query = query.Where(x => x.BookingStatusId.Value == (short?)eBookingStatus.PendingConfirmation);
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.AwaitingPayment)
                {
                    query = query.Where(x => x.BookingStatusId.Value == (short?)eBookingStatus.Confirmed && x.PaymentStatusId == (short?)ePaymentStatus.Pending);
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.TodayVisits)
                {
                    query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value.ToString("YYYYMMDD").Equals(DateTime.UtcNow.ToString("YYYYMMDD"), StringComparison.InvariantCultureIgnoreCase));
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.FutureVisits)
                {
                    query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value > DateTime.UtcNow);
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.Delivered)
                {
                    query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value < DateTime.UtcNow);
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.ToBeInvoicesPaid)
                {
                    query = query.Where(x => x.PaymentStatusId == (short)ePaymentStatus.Paid && x.PaymentMethodId != (short)ePaymentMethods.Insurance && string.IsNullOrEmpty(x.InvoiceNumber));
                }
                else if (viewModel.Filter.BookingFilter == eBookingFilter.ToBeInvoicedNotPaid)
                {
                    query = query.Where(x => x.PaymentStatusId != (short)ePaymentStatus.Paid && x.PaymentMethodId == (short)ePaymentMethods.Insurance && string.IsNullOrEmpty(x.InvoiceNumber));
                }

                switch (viewModel.SortField)
                {
                    case "name":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;

                    case "customer":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Customer);
                        else
                            query = query.OrderByDescending(x => x.Customer);
                        break;

                    case "invoicingEntity":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.InvoicingEntity);
                        else
                            query = query.OrderByDescending(x => x.InvoicingEntity);
                        break;


                    case "service":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Service);
                        else
                            query = query.OrderByDescending(x => x.Service);
                        break;
                    case "professional":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.Professional);
                        else
                            query = query.OrderByDescending(x => x.Professional);
                        break;

                    case "bookingDate":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.BookingDate);
                        else
                            query = query.OrderByDescending(x => x.BookingDate);
                        break;

                    case "visitDate":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.VisitDate);
                        else
                            query = query.OrderByDescending(x => x.VisitDate);
                        break;
                    case "paymentMethod":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PaymentMethod);
                        else
                            query = query.OrderByDescending(x => x.PaymentMethod);
                        break;

                    case "ptFee":
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.PtFee);
                        else
                            query = query.OrderByDescending(x => x.PtFee);
                        break;
                    default:
                        if (viewModel.SortOrder.Equals("asc"))
                            query = query.OrderBy(x => x.BookingDate);
                        else
                            query = query.OrderByDescending(x => x.BookingDate);

                        break;
                }

                var totalCount = query.LongCount();
                var result = new
                {
                    items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                    totalCount
                };
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public string GetBookingInvoiceNumber(long invoiceId)
        {
            var obj = Db.Invoice.FirstOrDefault(x => x.Id == invoiceId);
            if (obj is null)
                return string.Empty;
            return obj.InvoiceNumber;
        }

        public string GetBookingName(long customerId, string name, string surName)
        {
            return $"{name} {surName}";
        }

        public int GetSrNo(long customerId)
        {
            var obj = Db.Booking.Where(x => x.CustomerId == customerId).Select(x => new { x.SrNo }).ToList();
            if (obj is null || obj.Count == 0)
                return 1;
            else
                return obj.Max(x => x.SrNo).Value + 1;
        }

        public dynamic GetBookingCycleConnectedBookings(long bookingId)
        {
            var cycleBooking = Db.Booking.FirstOrDefault(x => x.Id == bookingId).CycleBookingId;
            var bookingIds = Db.Booking.Where(x => x.CycleBookingId == bookingId).Select(s => s.Id).ToList();
            UpdateBookingStats(bookingIds);

            var result = (from b in Db.Booking
                          where b.CycleBookingId == bookingId && b.Cycle.HasValue
                          select new
                          {
                              b.Id,
                              bookingName = b.Name,
                              serviceName = b.Service.Name,
                              professional = b.Professional.Name,
                              cycleNumber = b.CycleNumber,
                              ptFee = b.PtFee,
                              proFee = b.ProFee,
                              b.SubTotal
                          }).ToList();


            return result;
        }


        public void UpdateBookingStats(List<long> bookingIds)
        {
            try
            {
                foreach (var bookingId in bookingIds)
                {
                    var bookingModel = Db.Booking.Find(bookingId);
                    if (bookingModel != null)
                    {
                        bookingModel.QuantityHours = bookingModel.QuantityHours.HasValue ? bookingModel.QuantityHours.Value : (short)1;
                        bookingModel.SubTotal = GetSubTotal(bookingModel.PtFee, bookingModel.QuantityHours);
                        bookingModel.TaxAmount = GetCusotmerTaxAmount(bookingModel.SubTotal, bookingModel.TaxType);

                        bookingModel.GrossTotal = (Convert.ToDecimal(bookingModel.SubTotal) + Convert.ToDecimal(bookingModel.TaxAmount)) - Convert.ToDecimal(bookingModel.PatientDiscount);

                        Db.Booking.Update(bookingModel);
                        Db.SaveChanges();
                    }
                }
            }
            catch
            {
            }
        }


        public dynamic BookingConnectedProfessional(long bookingId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();

            return (from b in Db.Booking
                    where b.Id == bookingId
                    select new
                    {
                        professional = b.Professional.Name,
                        phoneNumber = b.Professional.Telephone,
                        email = b.Professional.Email,
                        lastVisitDate = b.VisitStartDate,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).ToList();
        }

        public dynamic BookingConnectedInvoices(long bookingId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.BookingId == bookingId
                    select new
                    {
                        subject = ib.Invoice.Subject,
                        invoiceNumber = ib.Invoice.InvoiceNumber,
                        ieName = ib.Booking.InvoiceEntityId.HasValue ? ib.Booking.InvoiceEntity.Name : string.Empty,
                        invoiceDate = ib.Invoice.InvoiceDate,
                        invoiceDueDate = ib.Invoice.DueDate,
                        totalInvoice = ib.Invoice.TotalInvoice
                    }).ToList();

        }

        public string GetItemNameOnInvoice(long serviceId, long? ptFeeId)
        {
            string serviceName = Db.Service.Where(x => x.Id == serviceId).Select(s => s.Name).FirstOrDefault();
            string feeName = ptFeeId.HasValue ? Db.PtFee.Where(x => x.Id == ptFeeId).Select(s => s.FeeName).FirstOrDefault() : string.Empty;
            return $"{serviceName} {feeName}";
        }

        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _appContext.Users.Find(assignedToId).FullName;
        }
    }
}
