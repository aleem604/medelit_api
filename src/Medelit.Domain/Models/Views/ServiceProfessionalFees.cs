using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("service_professional_fees")]
    public class ServiceProfessionalFees
    {
        public long Id { get; set; }
        [Column("service_id")]
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public Professional Professional { get; set; }

        [Column("pt_fee_id")]
        public long? PtFeeId { get; set; }
        [ForeignKey("PtFeeId")]
        public PtFee PtFee { get; set; }

        [Column("pro_fee_id")]
        public long? ProFeeId { get; set; }
        [ForeignKey("ProFeeId")]
        public ProFee ProFee { get; set; }
    }
}