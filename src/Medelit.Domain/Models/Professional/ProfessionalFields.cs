using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("professional_fields")]
  public  class ProfessionalFields
    {    
        public long Id { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }
        [Column("field_id")]
        public long FieldId { get; set; }
        [ForeignKey("FieldId")]
        public FieldSubCategory Field { get; set; }

    }
}
