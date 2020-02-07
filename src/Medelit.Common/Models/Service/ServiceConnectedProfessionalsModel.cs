using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common.Models
{
   public class ServiceConnectedProfessionalsModel
    {
        public long? ProfessionalId { get; set; }
        public string Professional { get; set; }
        public long? ServiceId { get; set; }
        public string CService { get; set; }
        public string CField { get; set; }
        public string CSubcategory { get; set; }

        public long? PtFeeRowId { get; set; }
        public long? PtFeeId { get; set; }
        public string PtFeeName { get; set; }
        public decimal? PtFeeA1 { get; set; }
        public decimal? PtFeeA2 { get; set; }

        public long? ProFeeRowId { get; set; }
        public long? ProFeeId { get; set; }
        public string ProFeeName { get; set; }
        public decimal? ProFeeA1 { get; set; }
        public decimal? ProFeeA2 { get; set; }
    }

}
