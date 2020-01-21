using System;
using System.Threading;
using System.Threading.Tasks;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Medelit.Domain.CommandHandlers
{
    public class BookingCommandHandler : CommandHandler,
        IRequestHandler<SaveBookingCommand, bool>,
        IRequestHandler<UpdateBookingsStatusCommand, bool>,
        IRequestHandler<DeleteBookingsCommand, bool>,
        IRequestHandler<BookingFromCustomerCommand, bool>,
        IRequestHandler<CreateCloneCommand, bool>,
        IRequestHandler<CreateCycleCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IInvoiceEntityRepository _ieRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConfiguration _config;


        public BookingCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IInvoiceEntityRepository ieRepository,
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IInvoiceRepository invoiceRepository,
            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _invoiceRepository = invoiceRepository;
            _ieRepository = ieRepository;
        }

        public Task<bool> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Booking.Id > 0)
                {
                    var bookingModel = _bookingRepository.GetById(request.Booking.Id);
                    //bookingModel.InvoiceEntityId = request.Booking.InvoiceEntityId;

                    bookingModel.Name = request.Booking.Name;
                    bookingModel.BookingDate = request.Booking.BookingDate;

                    bookingModel.BookingTypeId = request.Booking.BookingTypeId;
                    bookingModel.VisitLanguageId = request.Booking.VisitLanguageId;
                    bookingModel.VisitStreetName = request.Booking.VisitStreetName;
                    bookingModel.HomeStreetName = request.Booking.HomeStreetName;
                    bookingModel.VisitPostCode = request.Booking.VisitPostCode;
                    bookingModel.HomePostCode = request.Booking.HomePostCode;
                    bookingModel.VisitCityId = request.Booking.VisitCityId;
                    bookingModel.PhoneNumber = request.Booking.PhoneNumber;

                    bookingModel.VisitCountryId = request.Booking.VisitCountryId;
                    bookingModel.HomeCountryId = request.Booking.HomeCountryId;
                    bookingModel.Details = request.Booking.Details;

                    bookingModel.Diagnosis = request.Booking.Diagnosis;
                    bookingModel.ReasonForVisit = request.Booking.ReasonForVisit;
                    bookingModel.ImToProId = request.Booking.ImToProId;
                    bookingModel.MailToPtId = request.Booking.MailToPtId;
                    bookingModel.PtCalledForAppointmentId = request.Booking.PtCalledForAppointmentId;
                    bookingModel.PaymentConcludedId = request.Booking.PaymentConcludedId;
                    bookingModel.PaymentMethodId = request.Booking.PaymentMethodId;
                    bookingModel.AddToAccountingId = request.Booking.AddToAccountingId;
                    bookingModel.PaymentStatusId = request.Booking.PaymentStatusId;
                    bookingModel.CCAuthorizationId = request.Booking.CCAuthorizationId;

                    bookingModel.BankTransfterReceiptId = request.Booking.BankTransfterReceiptId;
                    bookingModel.CCOwner = request.Booking.CCOwner;
                    bookingModel.PaymentArrivalDate = request.Booking.PaymentArrivalDate;
                    bookingModel.InvoiceDueDate = request.Booking.PaymentArrivalDate;
                    bookingModel.NotesOnPayment = request.Booking.NotesOnPayment;
                    bookingModel.ReportDeliveredId = request.Booking.ReportDeliveredId;
                    bookingModel.AddToProAccountId = request.Booking.AddToProAccountId;
                    bookingModel.FeedbackFromPro = request.Booking.FeedbackFromPro;
                    bookingModel.ProAvailabilityAskedId = request.Booking.ProAvailabilityAskedId;
                    bookingModel.LabCostsForMedelit = request.Booking.LabCostsForMedelit;
                    bookingModel.DateOnPrescription = request.Booking.DateOnPrescription;
                    bookingModel.LabId = request.Booking.LabId;
                    bookingModel.Vials = request.Booking.Vials;

                    bookingModel.RepeadPrescriptionNumber = request.Booking.RepeadPrescriptionNumber;
                    bookingModel.PrescriptionNumber = request.Booking.PrescriptionNumber;
                    bookingModel.Notes = request.Booking.Notes;
                    bookingModel.PrivateFee = request.Booking.PrivateFee;
                    bookingModel.TicketFee = request.Booking.TicketFee;
                    bookingModel.ExcemptionCode = request.Booking.ExcemptionCode;
                    bookingModel.NHSOrPrivateId = request.Booking.NHSOrPrivateId;
                    bookingModel.PatientDiscount = request.Booking.PatientDiscount;

                    bookingModel.IsAllDayVisit = request.Booking.IsAllDayVisit;
                    bookingModel.VisitStartDate = request.Booking.VisitStartDate;
                    bookingModel.VisitEndDate = request.Booking.VisitEndDate;

                    bookingModel.ProDiscount = request.Booking.ProDiscount;
                    bookingModel.CashConfirmationMailId = request.Booking.CashConfirmationMailId;


                    bookingModel.Phone2 = request.Booking.Phone2;
                    bookingModel.Phone2Owner = request.Booking.Phone2Owner;

                    bookingModel.Email = request.Booking.Email;
                    bookingModel.Email2 = request.Booking.Email2;
                    bookingModel.DateOfBirth = request.Booking.DateOfBirth;
                    bookingModel.CountryOfBirthId = request.Booking.CountryOfBirthId;
                    bookingModel.VisitRequestingPerson = request.Booking.VisitRequestingPerson;
                    bookingModel.VisitRequestingPersonRelationId = request.Booking.VisitRequestingPersonRelationId;

                    bookingModel.InsuranceCoverId = request.Booking.InsuranceCoverId;

                    bookingModel.InvoicingNotes = request.Booking.Name;

                    bookingModel.BuildingTypeId = request.Booking.BuildingTypeId;
                    bookingModel.Buzzer = request.Booking.Buzzer;
                    bookingModel.FlatNumber = request.Booking.FlatNumber;
                    bookingModel.Floor = request.Booking.Floor;
                    bookingModel.VisitVenueId = request.Booking.VisitVenueId;
                    bookingModel.VisitVenueDetail = request.Booking.VisitVenueDetail;
                    bookingModel.BookingStatusId = request.Booking.BookingStatusId;

                    bookingModel.AddressNotes = request.Booking.AddressNotes;
                    bookingModel.ServiceId = request.Booking.ServiceId;
                    bookingModel.ProfessionalId = request.Booking.ProfessionalId;

                    bookingModel.PtFee = request.Booking.PtFee;
                    bookingModel.ProFee = request.Booking.ProFee;

                    bookingModel.CashReturn = request.Booking.PtFee;
                    bookingModel.QuantityHours = request.Booking.QuantityHours;
                    bookingModel.TaxType = request.Booking.TaxType;
                    bookingModel.SubTotal = GetSubTotal(request.Booking.PtFee, request.Booking.QuantityHours);
                    bookingModel.TaxAmount = GetCusotmerTaxAmount(bookingModel.SubTotal, bookingModel.TaxType);
                    bookingModel.GrossTotal = bookingModel.SubTotal + bookingModel.TaxAmount;

                    bookingModel.UpdateDate = DateTime.UtcNow;
                    bookingModel.UpdatedById = CurrentUser.Id;
                    _bookingRepository.Update(bookingModel);
                    commmitResult = Commit();

                    request.Booking = bookingModel;
                }
                else
                {
                    var bookingModel = request.Booking;
                    _bookingRepository.Add(bookingModel);
                    commmitResult = Commit();
                    request.Booking = bookingModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Booking));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }

            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }
        public Task<bool> Handle(UpdateBookingsStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var booking in request.Bookings)
                {
                    var bookingModel = _bookingRepository.GetById(booking.Id);
                    bookingModel.Status = request.Status;
                    bookingModel.UpdateDate = DateTime.UtcNow;
                    bookingModel.UpdatedById = CurrentUser.Id;
                    _bookingRepository.Update(bookingModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Bookings));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(DeleteBookingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var bookingId in request.BookingIds)
                {
                    var bookingModel = _bookingRepository.GetById(bookingId);
                    bookingModel.Status = eRecordStatus.Deleted;
                    bookingModel.DeletedAt = DateTime.UtcNow;
                    bookingModel.DeletedById = CurrentUser.Id;
                    _bookingRepository.Update(bookingModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.BookingIds));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(BookingFromCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {

                _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                return Task.FromResult(false);

            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }

        }

        public Task<bool> Handle(CreateCloneCommand request, CancellationToken cancellationToken)
        {
            var booking = _bookingRepository.GetById(request.BookingId);
            try
            {
                var clones = request.NumberOfClones;
                if (clones > 0)
                {
                    while (clones > 0)
                    {
                        var newBooking = booking.Clone();
                        newBooking.Id = 0;
                        newBooking.Name = _bookingRepository.GetBookingName(booking.Name, string.Empty);
                        newBooking.VisitStartDate = null;
                        newBooking.QuantityHours = null;
                        newBooking.CreatedById = CurrentUser.Id;
                        newBooking.AssignedToId = CurrentUser.Id;

                        _bookingRepository.Add(newBooking);
                        Commit();
                        clones--;
                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, clones));
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }

            _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
            return Task.FromResult(false);

        }

        public Task<bool> Handle(CreateCycleCommand request, CancellationToken cancellationToken)
        {
            var booking = _bookingRepository.GetById(request.BookingId);
            try
            {

                if (request.NumberOfCycles > 0)
                {
                    for (short i = 0; i < request.NumberOfCycles; i++)
                    {
                        var newBooking = booking.Clone();
                        newBooking.Cycle = request.NumberOfCycles;
                        newBooking.CycleNumber = Convert.ToInt16(i + 1);
                        newBooking.Id = 0;
                        newBooking.Name = _bookingRepository.GetBookingName(booking.Name, string.Empty);
                        newBooking.VisitStartDate = null;
                        newBooking.QuantityHours = null;
                        newBooking.CycleBookingId = booking.Id;
                        newBooking.CreatedById = CurrentUser.Id;
                        newBooking.AssignedToId = CurrentUser.Id;
                        _bookingRepository.Add(newBooking);
                        Commit();

                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.NumberOfCycles));
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }

            _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
            return Task.FromResult(false);

        }



        private decimal? GetSubTotal(decimal? ptFee, short? quantityHours)
        {
            if (ptFee.HasValue && quantityHours.HasValue)
                return ptFee.Value * quantityHours.Value;
            return null;
        }

        private decimal? GetCusotmerTaxAmount(decimal? subTotal, short? taxType)
        {
            if (subTotal.HasValue && taxType.HasValue)
                return subTotal.Value * taxType.Value * (decimal)0.01;
            return null;
        }

        public void Dispose()
        {

        }

    }
}