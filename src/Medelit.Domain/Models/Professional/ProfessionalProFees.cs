using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("professional_profees")]
    public class ProfessionalProFees
    {
        public long Id { get; set; }

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
