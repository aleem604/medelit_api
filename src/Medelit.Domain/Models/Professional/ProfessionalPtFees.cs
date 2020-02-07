using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("professional_ptfees")]
    public class ProfessionalPtFees
    {
        public long Id { get; set; }

        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }

        [Column("pt_fee_id")]
        public long PtFeeId { get; set; }
        [ForeignKey("PtFeeId")]
        public virtual PtFee PtFee { get; set; }
    }
}
