using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class InvoiceServiceRelationViewModel
    {
        public long Id { get; set; }
        public long PTFeeId { get; set; }
        public decimal? PTFeeA1 { get; set; }
        public decimal? PTFeeA2 { get; set; }
        public long PROFeeId { get; set; }
        public decimal? PROFeeA1 { get; set; }
        public decimal? PROFeeA2 { get; set; }
        public long InvoiceId { get; set; }
        public long ServiceId { get; set; }
        public long ProfessionalId { get; set; }

    }
}
