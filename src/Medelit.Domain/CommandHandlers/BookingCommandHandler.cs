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
        IRequestHandler<CreateInvoiceCommand, bool>
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
                    bookingModel.Name = request.Booking.Name;
                    bookingModel.BookingDate = request.Booking.BookingDate; 
                    bookingModel.BookingTime = request.Booking.BookingTime; 
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
                    bookingModel.Lab = request.Booking.Lab; 
                    bookingModel.Vials = request.Booking.Vials; 

                    bookingModel.RepeadPrescriptionNumber = request.Booking.RepeadPrescriptionNumber; 
                    bookingModel.PrescriptionNumber = request.Booking.PrescriptionNumber; 
                    bookingModel.Notes = request.Booking.Notes; 
                    bookingModel.PrivateFee = request.Booking.PrivateFee; 
                    bookingModel.TicketFee = request.Booking.TicketFee; 
                    bookingModel.ExcemptionCode = request.Booking.ExcemptionCode; 
                    bookingModel.NHSOrPrivateId = request.Booking.NHSOrPrivateId; 
                    bookingModel.PatientDiscount = request.Booking.PatientDiscount; 
                    bookingModel.VisitDate = request.Booking.VisitDate; 
                    bookingModel.VisitTime = request.Booking.VisitTime; 
                    bookingModel.ProDiscount = request.Booking.ProDiscount; 
                    bookingModel.CashConfirmationMailId = request.Booking.CashConfirmationMailId; 
                    bookingModel.QuantityHours = request.Booking.QuantityHours; 

                    bookingModel.Phone2 = request.Booking.Phone2;
                    bookingModel.Phone2Owner = request.Booking.Phone2Owner;

                    bookingModel.Email = request.Booking.Email;
                    bookingModel.Email2 = request.Booking.Email2;
                    bookingModel.DateOfBirth = request.Booking.DateOfBirth;
                    bookingModel.CountryOfBirthId = request.Booking.CountryOfBirthId;
                    bookingModel.VisitRequestingPerson = request.Booking.VisitRequestingPerson;
                    bookingModel.VisitRequestingPersonRelationId = request.Booking.VisitRequestingPersonRelationId;

                    bookingModel.InsuranceCoverId = request.Booking.InsuranceCoverId;
                    bookingModel.InvoiceEntityId = request.Booking.InvoiceEntityId;
                    bookingModel.InvoicingNotes = request.Booking.Name;

                    bookingModel.BuildingTypeId = request.Booking.BuildingTypeId;
                    bookingModel.Buzzer = request.Booking.Buzzer;
                    bookingModel.FlatNumber = request.Booking.FlatNumber;
                    bookingModel.Floor = request.Booking.Floor;
                    bookingModel.VisitVenueId = request.Booking.VisitVenueId;
                    bookingModel.VisitVenueDetail = request.Booking.VisitVenueDetail;
                    bookingModel.BookingStatusId = request.Booking.BookingStatusId;

                    bookingModel.AddressNotes = request.Booking.AddressNotes;
                    bookingModel.Services = request.Booking.Services;

                    bookingModel.UpdateDate = DateTime.UtcNow;

                    _bookingRepository.RemoveBookingServices(bookingModel.Id);

                    _bookingRepository.Update(bookingModel);
                    commmitResult = Commit();
                    //if (commmitResult)
                    //{
                    //    var services = request.Booking.Services;
                    //    var newServices = new List<BookingServiceRelation>();
                    //    foreach (var service in services)
                    //    {
                    //        newServices.Add(new BookingServiceRelation
                    //        {
                    //            BookingId = bookingModel.Id,
                    //            ServiceId = service.ServiceId,
                    //            ProfessionalId = service.ProfessionalId,
                    //            PTFeeId = service.PTFeeId,
                    //            PTFeeA1 = service.PTFeeA1,
                    //            PTFeeA2 = service.PTFeeA2,
                    //            PROFeeId = service.PROFeeId,
                    //            PROFeeA1 = service.PROFeeA1,
                    //            PROFeeA2 = service.PROFeeA2
                    //        });
                    //    }

                    //    _bookingRepository.SaveBookingRelation(newServices);
                    //    request.Booking = bookingModel;
                    //}
                    request.Booking = bookingModel;
                    //var allBookings = _feeRepository.GetAll();
                    //foreach (var fee in allBookings)
                    //{

                    //        fee.BookingCode = fee.BookingTypeId == eBookingType.PTBooking ? $"FP{fee.Id.ToString().PadLeft(6, '0')}" : $"FS{fee.Id.ToString().PadLeft(6, '0')}";
                    //        fee.UpdateDate = DateTime.UtcNow;
                    //        _feeRepository.Update(fee);

                    //}
                    //Commit();
                }
                else
                {
                    var feeModel = request.Booking;
                    _bookingRepository.Add(feeModel);
                    commmitResult = Commit();
                    request.Booking = feeModel;
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
                foreach (var fee in request.Bookings)
                {
                    var feeModel = _bookingRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    _bookingRepository.Update(feeModel);
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
                foreach (var feeId in request.BookingIds)
                {
                    var feeModel = _bookingRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    //feeModel.DeletedById = 0;
                    _bookingRepository.Update(feeModel);
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
                //// User customer to booking instead
                ///



                var customer = _customerRepository.GetByIdWithInclude(request.CustomerId);
                //var lead = _bookingRepository.GetById(request.LeadId);
                var booking = new Booking();
                booking.Name = $"{customer.Name} {customer.SurName}";
                booking.CustomerId = customer.Id;
                booking.InvoiceEntityId = customer.InvoiceEntityId;
                booking.BookingDate = DateTime.UtcNow;
                booking.BookingTime = DateTime.UtcNow.ToString("H:mm tt");
                booking.VisitLanguageId = (short)customer.LanguageId;
                booking.VisitVenueId = (short)customer.VisitVenueId;
                booking.VisitVenueDetail = customer.VisitVenueDetail;
                booking.AddressNotes = customer.AddressNotes;
                booking.Buzzer = customer.Buzzer;
                booking.VisitRequestingPerson = customer.VisitRequestingPerson;
                booking.FlatNumber = customer.FlatNumber;
                booking.VisitRequestingPersonRelationId = (short?)customer.VisitRequestingPersonRelationId;
                booking.Floor = customer.Floor;
                booking.BuildingTypeId = (short)customer.BuildingTypeId;
                booking.VisitStreetName = customer.VisitStreetName;
                booking.HomeStreetName = customer.HomeStreetName;
                booking.VisitPostCode = customer.VisitPostCode;
                booking.HomePostCode = customer.HomePostCode;
                booking.VisitCityId = (short)customer.VisitCityId;
                booking.HomeCityId = (short)customer.HomeCityId;
                booking.VisitCountryId = (short)customer.VisitCountryId;
                booking.HomeCountryId = (short)customer.HomeCountryId;
                booking.PhoneNumber = customer.MainPhone;
                booking.Email = customer.Email;
                booking.Phone2 = customer.Phone2;
                booking.Phone2Owner = customer.Phone2Owner;

                booking.Details = customer.VisitVenueDetail;
                booking.ReasonForVisit = customer.VisitVenueDetail;
                booking.PaymentMethodId = (short)customer.PaymentMethodId;
                booking.InvoicingNotes = customer.InvoicingNotes;
                booking.InsuranceCoverId = customer.InsuranceCoverId;
                booking.PatientDiscount = customer.Discount;

                _bookingRepository.Add(booking);
                if (Commit())
                {
                    if (booking.Id > 0)
                    {
                        var services = customer.Services;
                        var newServices = new List<BookingServiceRelation>();
                        foreach (var service in services)
                        {
                            newServices.Add(new BookingServiceRelation
                            {
                                BookingId = booking.Id,
                                ServiceId = service.ServiceId,
                                ProfessionalId = service.ProfessionalId,
                                PTFeeId = service.PTFeeId,
                                PTFeeA1 = service.PTFeeA1,
                                PTFeeA2 = service.PTFeeA2,
                                PROFeeId = service.PROFeeId,
                                PROFeeA1 = service.PROFeeA1,
                                PROFeeA2 = service.PROFeeA2
                            });
                        }
                        _bookingRepository.SaveBookingRelation(newServices);
                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, booking));
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

        public Task<bool> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var booking = _bookingRepository.GetWithInclude(request.BookingId);
            var invoiceEntity = booking.InvoiceEntityId.HasValue ? _ieRepository.GetById((long)booking.InvoiceEntityId) : new InvoiceEntity();
            var customer = _customerRepository.GetAll().FirstOrDefault(x => x.Id == booking.CustomerId);
            var invoice = new Invoice();
            invoice.Subject = booking.InvoiceNumber ?? customer.Name;
            invoice.InvoiceEntityId = booking.InvoiceEntityId;
            invoice.CustomerId = booking.CustomerId;
            invoice.InvoiceNumber = DateTime.Now.ToString("yyyy");
            invoice.DueDate = booking.InvoiceDueDate ?? DateTime.Now;
            invoice.InvoiceDate = booking.VisitDate;
            //invoice.TaxCodeId = 
            invoice.StatusId = booking.BookingStatusId;
            invoice.PaymentDue = booking.InvoiceDueDate;
            invoice.PaymentDue = DateTime.Now;
            invoice.InvoiceSentByEmailId =0;
            invoice.InvoiceSentByMailId =0;
            invoice.PaymentMethodId = booking.PaymentMethodId;
            invoice.PatientDateOfBirth = booking.DateOfBirth;

            invoice.IEBillingAddress = invoice.IEBillingAddress;
            invoice.MailingAddress = invoiceEntity.MailingAddress;

            invoice.IEBillingPostCode = invoiceEntity.BillingPostCode;
            invoice.MailingPostCode = invoiceEntity.MailingPostCode;

            invoice.IEBillingCityId = invoiceEntity.BillingCityId;
            invoice.MailingCityId = invoiceEntity.MailingCityId;

            invoice.IEBillingCountryId = invoiceEntity.BillingCountryId;
            invoice.MailingCountryId = invoiceEntity.MailingCountryId;

            invoice.InvoiceNotes = booking.InvoicingNotes;
            invoice.InsuranceCoverId = booking.InsuranceCoverId;
            invoice.InvoiceDiagnosis = booking.Diagnosis;
            invoice.InvoiceDiagnosis = booking.Diagnosis;
            invoice.DateOfVisit = booking.VisitDate;
            invoice.InvoiceDescription = customer.Name;
            invoice.Quantity = booking.QuantityHours;
            invoice.PaymentArrivalDate = booking.PaymentArrivalDate;
            invoice.ProInvoiceDate = booking.InvoiceDueDate;
            
           _invoiceRepository.Add(invoice);
            if (Commit())
            {
                if (booking.Id > 0)
                {
                    var services = booking.Services;
                    var newServices = new List<InvoiceServiceRelation>();
                    foreach (var service in services)
                    {
                        newServices.Add(new InvoiceServiceRelation
                        {
                            InvoiceId = booking.Id,
                            ServiceId = service.ServiceId,
                            ProfessionalId = service.ProfessionalId,
                            PTFeeId = service.PTFeeId,
                            PTFeeA1 = service.PTFeeA1,
                            PTFeeA2 = service.PTFeeA2,
                            PROFeeId = service.PROFeeId,
                            PROFeeA1 = service.PROFeeA1,
                            PROFeeA2 = service.PROFeeA2
                        });
                    }
                   
                    var newInvoice = _invoiceRepository.GetById(invoice.Id);
                    newInvoice.InvoiceNumber = $"{invoice.Id.ToString().PadLeft(5, '0')}/{DateTime.Now.ToString("yyyy")}";
                    _bookingRepository.SaveInvoiceRelation(newServices, newInvoice);
                }

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, booking));
                return Task.FromResult(true);
            }
            else
            {
                _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                return Task.FromResult(false);
            }
        }

        private Task<bool> HandleException(string messageType, Exception ex)
        {
            _bus.RaiseEvent(new DomainNotification(messageType, ex.Message));
            return Task.FromResult(false);
        }


        public void Dispose()
        {

        }

    }
}