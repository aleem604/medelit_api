using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("v_service_professional_fees")]
    public class VServiceProfessionalFees
    {
        [Key]
        [Column("service_professional_id")]
        public long ServiceProfessionalId { get; set; }
        [ForeignKey("ServiceProfessionalId")]
        public ServiceProfessionals ServiceProfessionals { get; set; }

        [Column("service_id")]
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        //[ForeignKey("Professional")]
        //public Professional Professional { get; set; }

        [Column("ppt_id")]
        public long? ProfessionalPtFeesId { get; set; }
        [ForeignKey("ProfessionalPtFeesId")]
        public ProfessionalPtFees ProfessionalPtFees { get; set; }

        [Column("ppt_professional_id")]
        public long? PtFeesProfessionalId { get; set; }

        [Column("pt_fee_id")]
        public long PtFeeId { get; set; }
        [ForeignKey("PtFeeId")]
        public PtFee PtFee { get; set; }


        [Column("ppro")]
        public long? ProfessionalProFeeId { get; set; }
        [ForeignKey("ProfessionalProFeeId")]
        public ProfessionalProFees ProfessionalProFees { get; set; }

        [Column("ppro_professional_id")]
        public long? PRoFeesProfessionalId { get; set; }

        [Column("pro_fee_id")]
        public long ProFeeId { get; set; }
        [ForeignKey("ProFeeId")]
        public ProFee ProFee { get; set; }


    }
}