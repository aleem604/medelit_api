using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public class CategorySearchViewModel
    {
        public CategorySearchFilterViewModel Filter { get; set; } = new CategorySearchFilterViewModel();
        public string SortOrder { get; set; } = "desc";
        public string SortField { get; set; } = "id";
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

    public class CategorySearchFilterViewModel
    {
        public eRecordStatus Status { get; set; } = eRecordStatus.All;
        public long CategoryId { get; set; }
        public long SubCategoryId { get; set; }
        public long SubSubCategoryId { get; set; }
        public string Search { get; set; }
    }
}
