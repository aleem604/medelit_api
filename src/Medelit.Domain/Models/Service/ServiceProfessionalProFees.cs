using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("service_professional_profees")]
    public class ServiceProfessionalProFees
    {
        public long Id { get; set; }

        [Column("service_id")]
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }

        [Column("pro_fee_id")]
        public long ProFeeId { get; set; }
        [ForeignKey("ProFeeId")]
        public virtual ProFee ProFee { get; set; }
    }
}
