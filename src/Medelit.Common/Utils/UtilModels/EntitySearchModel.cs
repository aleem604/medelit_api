using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public class EntitySearchModel
    {
        public long LocationId { get; set; }
        public string SearchCategory { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public eSortBy? SortBy { get; set; } = eSortBy.Name_ASC;
        public string sLocation { get; set; }
        public string sCategory { get; set; }
        public string sKeyword { get; set; }
    }
}
