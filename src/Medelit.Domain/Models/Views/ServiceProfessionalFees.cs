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
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [Column("pt_fee_id")]
        public long? PtFeeId { get; set; }
        [Column("pro_fee_id")]
        public long? ProFeeId { get; set; }
    }
}