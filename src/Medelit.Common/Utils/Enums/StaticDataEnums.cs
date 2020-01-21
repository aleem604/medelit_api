using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public enum eAccountingCodes : short
    {
        SoleTrader = 1,
        Company
    }

    public enum eApplicationMeans : short
    {
        Email = 1,
        Phone,
        InPerson
    }

    public enum eApplicationMethods : short
    {
        WebForm = 1,
        DirectEmail,
        Indeed,
        InternalReferral,
        ContactFromMedelit
    }

    public enum eBookingStatus : short
    {
        Created = 1,
        PendingConfirmation,
        Confirmed,
        Delivered,
        Postponed,
        CancelledAfterAcceptance,
        Refused
    }

    public enum eBookingTypes : short
    {
        FirstBookingCycle = 1,
        InternalReferral,
        ReturningPatient
    }

    public enum eBuildingTypes : short
    {
        House = 1,
        Flat
    }

    public enum eCollaborationCodes : short
    {
        Yes = 1,
        No,
        PreContract
    }

    public enum eContactMethods : short
    {
        Telephone = 1,
        Webform,
        Email,
        Social,
        Directory,
        Other
    }

    public enum eContractStatus : short
    {
        Active = 1,
        Closed,
        WaitingForSignedContract,
        NotSignedRefused,
        WaitingForContractMeeting
    }

    public enum eDiscountNetworks : short
    {
        Groupon = 1,
        Google,
        Facebook
    }

    public enum eDocumentListSent : short
    {
        Yes = 1,
        No,
        NotRequested
    }

    public enum eDurations : short
    {
        Min15 = 1,
        Min30,
        Min45,
        Min60
    }

    public enum eInvoiceEntityRating : short
    {
        MedelitPatient = 1,
        Supplier,
        Laboratory,
        Employer,
        CareHome,
        Insurance,
        NetworkCompany
    }
    public enum eInvoiceEntityTypes : short
    {
        Person = 1,
        Company,
        CareHomeHospital,
        Clinic,
        NHSFoundation,
        Charity,
        Lab
    }
    public enum eInvoiceStatus : short
    {
        Proforma = 1,
        Created,
        Pending,
        Paid,
        Sent
    }
    public enum eLeadCategories : short
    {
        Patient = 1,
        Insurance,
        Bank,
        Council
    }

    public enum eLeadSource : short
    {
        Website = 1,
        Supplier,
        Request
    }

    public enum eLeadStatus : short
    {
        Hot = 1,
        Warm,
        Cold
    }

    public enum eTitles : short
    {
        Mr = 1,
        Mrs,
        Miss,
        Dr,
        Prof
    }

    public enum ePaymentMethods : short
    {
        Cash = 1,
        CreditDebitCard,
        BankTransfer,
        Insurance,
        None
    }

    public enum ePaymentStatus : short
    {
        NotRequested = 1,
        Pending,
        Paid,
        Incomplete
    }

    public enum eRelationships : short
    {
        SonDaughter = 1,
        Partner,
        Carer,
        OtherFamily,
        PAOrEmployee,
        Concierge,
        Employer,
        Insurance,
        Friend
    }

    public enum eReportDeliveryOptions : short
    {
        Yes = 1,
        AlreadySigned,
        No,
        Lost,
        NotProvided,
        Stolen
    }

    public enum eVisitVenues : short
    {
        Home,
        Hotel,
        Clinic,
        PublicVenue,
        Residence,
        HostelStudent,
        House,
        Office,
        CareHome,
        Hopsital,
        Prison,
        Other
    }

    public enum eAddedToAccount : short
    {
        Yes,
        No,
        NotNecessary
    }

}
