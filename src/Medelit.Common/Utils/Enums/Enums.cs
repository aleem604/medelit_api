using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Common
{
    public enum UserRoles
    {
        [Display(Name = "Admin")]
        Admin = 0,

        [Display(Name = "UserManagementModule")]
        UserManagementModule = 1,

        [Display(Name = "LeadManagementModule")]
        LeadManagementModule = 3,

        [Display(Name = "CustomerManagementModule")]
        CustomerManagementModule = 4,

        [Display(Name = "BookingManagementModule")]
        BookingManagementModule = 5,

        [Display(Name = "InvoiceManagementModule")]
        InvoiceManagementModule = 6,

        [Display(Name = "InvoiceEntityManagementModule")]
        InvoiceEntityManagementModule = 7,

        [Display(Name = "FeeManagementModule")]
        FeeManagementModule = 8,

        [Display(Name = "ProfessionalManagementModule")]
        ProfessionalManagementModule = 9,

        [Display(Name = "ServiceManagementModule")]
        ServiceManagementModule = 10,


    }

    public enum eLeadsFilter : short
    {
        All,
        HotLeads,
    }

    public enum eLeadsStatus : short
    {
        Hot = 1,
        Warm = 2,
        Cold = 3
    }

    public enum eIEFilter : short
    {
        All,
        Contracted,
    }


    public enum eFeedbackType : short
    {
        All = -1,
        QualityOfService = 101,
        Cleanliness = 102,
        PriceToQuality_ratio = 103,
        VariabilityOfService = 104,
        TasteOfDishes = 105
    }

    public enum eFeeType : int
    {
        All = -1,
        [Description("PT Fee")]
        PTFee = 0,
        [Description("PRO Fee")]
        PROFee = 1
    }

    public enum eEntityServiceType : short
    {
        ViewAll = -1,
        QualityOfService = 1,
        DesignAndCreation = 2,
        Cleanliness = 3,
        PriceToQualityRatio = 4,
        VariabilityOfService = 5,
        TasteOfDishes = 6
    }

    public enum eRating : short
    {
        Terrible = 1,
        Poor = 2,
        Average = 3,
        VeryGood = 4,
        Excellent = 5
    }

    public enum eUserType : short
    {
        Consumer = 1,
        Merchant = 2
    }
    public enum eTinUser : byte
    {
        TinUser = byte.MaxValue
    }
    public enum eRecordStatus : short
    {
        All = -1,
        Pending = 0,
        Active = 1,
        Suspended = 2,
        Deleted = 3
    }
    public enum eLocationType : int
    {
        Country = 1384,
        Region = 1385,
        City = 1386,
        Neighbourhood = 1387
    }
    public enum eProfileType : short
    {
        Attribute = 8020,
        Review = 8021,
        Section = 8022
    }
    public enum eProfileSubType : short
    {
        Profile1 = 1,
        Profile2 = 2,
    }
    public enum eContactType : short
    {
        Primary = 8100,
        Business = 8114
    }

    public enum eEoTypes : short
    {
        ENTITY = 1010,   /// BUSINESS
        COMPANY = 1011,
        LOGON = 1012,
        CUSTOMER = 1020,
        ROLE = 1030,
        SALES_ORDER = 2260,
        APP_PRG = 2270,
        APP_PRG_FIELD = 2271,
        TEMPLATE = 11,
        PRODUCT = 2050,
        SUBSCRIPTION = 2051,
        ISSUE = 2290,
        PAGE_BLOCK = 2340,
        LOCATION = 2410,
        LOCATIONTYPE = 2411,
        LOCATION_PROGRAM = 2412,
        CLASSIFICATION = 2420,
        CLASSIFICATIONTYPE = 2421,
        REVIEW_CATEGORY_UL = 2424,
        REVIEW_CATEGORY_LI = 2425,
        ATTRIBUTE = 2430,
        GALLERY = 2431,
        IMAGE = 2432,
        VIDEO = 2433,
        FOLDER = 2434,
        FILE = 2435,
        LIST = 2530,
        LIST_ITEM = 2531,

        RELATION_RELATION = 2700, // location_category_id______business_id
        EO_ATTRIBUTE = 2711,
        LOCATION_EO = 2712,
        LOCATION_CATEGORY = 2710
    }


    public enum eEoSubTypes : short
    {
        INSTANCE = 12,
        CLASS = 10
    }
    public enum eRelationTypes : short
    {
        RELATION_RELATION = 2700, // location_category_id______business_id
        EO_ATTRIBUTE = 2711,
        LOCATION_EO = 2712,
        LOCATION_CATEGORY = 2710
    }

    public enum eTrackingType : short
    {
        TRACKING_PRICE = 3040,
        TRACKING_COUPON = 3032,
        EO_TYPE_SUBSCRIPTION = 2051,
        TRACKING_TICKET = 3033,
        TRACKING_EVENT = 3031
    }
    public enum eIncludeOptions : short
    {
        Slim,
        Fetch,
        Default,
        All,
        Contact,
        ProfileAttribute,
        ProfileSection,
        ProfileReview,
        Tracking
    }

    public enum eSortBy : int
    {
        [Description("Price asc")]
        Price_ASC = 10001,
        [Description("Price desc")]
        Price_DESC = 10002,

        [Description("Rating asc")]
        Rating_ASC = 10003,
        [Description("Rating desc")]
        Rating_DESC = 10004,

        [Description("Name asc")]
        Name_ASC = 10005,
        [Description("Name desc")]
        Name_DESC = 10006,

        [Description("Review asc")]
        Review_ASC = 10007,
        [Description("Review desc")]
        Review_DESC = 10008,
    }

    public enum eSocial : int
    {
        [Description("Facebook")]
        facebook = 1001,
        [Description("Twitter")]
        twitter = 1002,
        [Description("Youtube")]
        youtube = 1003,
        [Description("Linkedin")]
        linkedIn = 1004,
        [Description("Dribble")]
        dribble = 1005,
        [Description("Instagram")]
        instagram = 1006
    }

}
