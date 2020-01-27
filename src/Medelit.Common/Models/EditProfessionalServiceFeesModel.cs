using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common.Models
{
   public class EditProfessionalServiceFeesModel
    {
        public long ServiceId { get; set; }
        public long ProfessionalId { get; set; }

        public long PtFeeRowId { get; set; }
        public long PtFeeId { get; set; }
        public string PtFeeName { get; set; }
        public decimal? PtFeeA1 { get; set; }
        public decimal? PtFeeA2 { get; set; }
        public string PtFeeTags { get; set; }

        public long ProFeeRowId { get; set; }
        public long ProFeeId { get; set; }
        public string ProFeeName { get; set; }
        public decimal? ProFeeA1 { get; set; }
        public decimal? ProFeeA2 { get; set; }
        public string ProFeeTags { get; set; }
    }
}
