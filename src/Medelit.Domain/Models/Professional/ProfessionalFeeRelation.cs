using Medelit.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("professional_fees")]
  public  class ProfessionalFees
    {
        public long Id { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public Professional Professional { get; set; }
        [Column("fee_id")]
        public long FeeId { get; set; }
        [Column("fee_type_id")]
        public eFeeType FeeType { get; set; }
    }
}
