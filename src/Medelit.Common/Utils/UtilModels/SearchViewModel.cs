using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public class SearchViewModel
    {
        public SearchFilterViewModel Filter { get; set; } = new SearchFilterViewModel();
        public string SortOrder { get; set; } = "desc";
        public string SortField { get; set; } = "id";
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

    public class SearchFilterViewModel
    {
        public eLeadsFilter Filter { get; set; }
        public eIEFilter IEFilter { get; set; }
        public eBookingFilter BookingFilter { get; set; }
        public eInvoiceFilter InvoiceFilter { get; set; }
        public eProfessionalFilter ProfessionalFilter { get; set; }

        public eRecordStatus Status { get; set; } = eRecordStatus.All;
        public eFeeType FeeType { get; set; } = eFeeType.All;
        public long RegionId { get; set; }
        public long CityId { get; set; }
        public long NeighbourhoodId { get; set; }
        public string Category { get; set; }
        public string Attribute { get; set; }
        public string Classification { get; set; }
        public string Search { get; set; }
    }
}
