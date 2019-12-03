using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public static class ExternalUrls
    {
        public static string MagicBaseUrl = "http://40.117.85.216:8080/MagicScripts/MGrqispi.dll";

        // location id
        /// MagicScripts/MGrqispi.dll/?Appname=TIN&Prgname=events&Arguments=-N1442
        public static string MagicEventsUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=events&Arguments=-N{0}");

        /// MagicScripts/MGrqispi.dll/?Appname=TIN&Prgname=categories
        public static string MagicCategoriesUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=categories");

        /// MagicScripts/MGrqispi.dll/?Appname=TIN&Prgname=currencies
        public static string MagicCurrenciesUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=currencies");

        // location id
        /// MagicScripts/MGrqispi.dll/?Appname=TIN&Prgname=tickets&Arguments=-N64995
        public static string MagicTicketsUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=tickets&Arguments=-N{0}");


        // lat, lang, page size
        /// MagicScripts/MGrqispi.dll/?Appname=TIN&Prgname=event_listing_2&Arguments=-N51.507,-N0.1278,-N20
        public static string MagicEventsListingUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=event_listing_2&Arguments=-N{0},-N{1},-N{2}");

        // event id for getting event detail with tickets
        /// /?Appname=TIN&Prgname=event_detail_tickets&Arguments=-N65560
        public static string MagicEventDetailByEventIdUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=event_detail_tickets&Arguments=-N{0}");

        // get location events by location name
        /// /?Appname=TIN&Prgname=event_detail_tickets&Arguments=-N65560
        public static string MagicEventsByLocationNameUrl = string.Concat(MagicBaseUrl, "/?Appname=TIN&Prgname=event_location&Arguments=-A{0},-N10");


        public static string SFBaseUrl = "http://api.tin.info/api/default";

        //1-  http://api.tin.info/api/default/albums?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFGetEntitySetRootUrl = string.Concat(SFBaseUrl, "/albums?sf_site={0}");

        //2- get news items
        // http://api.tin.info/api/default/albums?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFCreateEntityUrl = string.Concat(SFBaseUrl, "/newsitems?sf_site={0}");

        //3- http://api.tin.info/api/default/newsitems
        public static string SFCreateNewsUrl = string.Concat(SFBaseUrl, "/newsitems");

        //4- http://api.tin.info/api/default/jobs 
        public static string SFCreateJobsPostUrl = string.Concat(SFBaseUrl, "/jobs");

        //5- http://api.tin.info/api/default/blogs
        public static string SFCreateBlogsPostUrl = string.Concat(SFBaseUrl, "/blogs");

        //6- http://api.tin.info/api/default/realestates
        public static string SFCreateRealstatePostUrl = string.Concat(SFBaseUrl, "/realestates");

        //7- http://api.tin.info/api/default/blogposts
        public static string SFCreateBlogpostPostUrl = string.Concat(SFBaseUrl, "/blogposts");

        // http://api.tin.info/api/default/business?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFCreateBusinessPostUrl = string.Concat(SFBaseUrl, "/business?sf_site={0}");

        //http://api.tin.info/api/default/business?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFAddProfileImagePostUrl = string.Concat(SFBaseUrl, "/images?sf_site={0}");

        //http://api.tin.info/api/default/images?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFGetEntityBySiteUrl = string.Concat(SFBaseUrl, "/images?sf_site={0}");

        // http://api.tin.info/api/default/newsitems
        public static string SFGetNewsItemsUrl = string.Concat(SFBaseUrl, "/newsitems");

        // http://api.tin.info/api/default/newsitems?$filter=contains(TinLocation,'London')
        public static string SFGetNewsItemsByLocationUrl = string.Concat(SFBaseUrl, "/newsitems?$filter=contains(TinLocation,'{0}')");

        // http://api.tin.info/api/default/jobs
        public static string SFGetJobsUrl = string.Concat(SFBaseUrl, "/jobs");

        // http://api.tin.info/api/default/blogs
        public static string SFGetBlogsUrl = string.Concat(SFBaseUrl, "/blogs");

        // http://api.tin.info/api/default/realstates
        public static string SFGetRealStatsUrl = string.Concat(SFBaseUrl, "/realstates");

        // http://api.tin.info/api/default/blogposts
        public static string SFGetBlogPostsUrl = string.Concat(SFBaseUrl, "/blogposts");

        // http://api.tin.info/api/default/blogposts/df0fda76-4e57-46d7-9f15-f8228c333d40
        public static string SFGetBlogPostsByIdUrl = string.Concat(SFBaseUrl, "/blogposts/{0}");

        // http://api.tin.info/api/default/newsitems/07a64e95-fddb-42ef-8bd3-4a5f1bfdd2de
        public static string SFGetNewsItemsByIdUrl = string.Concat(SFBaseUrl, "/newsitems/{0}");

        // http://api.tin.info/api/default/jobs/c9b03467-9ebc-4386-bb76-6e7c8b2f2327
        public static string SFGetJobByIdUrl = string.Concat(SFBaseUrl, "/jobs/{0}");

        // http://api.tin.info/api/default/blogs/4a771abd-cb2e-4282-b611-047ab44bfacd
        public static string SFGetBlogsByIdUrl = string.Concat(SFBaseUrl, "/blogs/{0}");

        // http://api.tin.info/api/default/realstates/83326adf-11dd-4fac-90db-f5b584129f13
        public static string SFGetRealStatesByIdUrl = string.Concat(SFBaseUrl, "/realstates/{0}");

        // http://api.tin.info/api/default/business?sf_site=bd5ffb57-3543-4258-86bd-fcc04fa18b8e
        public static string SFGetBusinessUrl = string.Concat(SFBaseUrl, "/business/{0}");

        // http://api.tin.info/api/default/sites
        public static string SFGetSitesUrl = string.Concat(SFBaseUrl, "/sites");

        // http://api.tin.info/api/default/sites/58805373-f4a6-4441-8305-6da487d7935b
        public static string SFGetSiteByIdUrl = string.Concat(SFBaseUrl, "/sites{0}");
    }
}
