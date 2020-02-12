using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("professional_subcategories")]
  public  class ProfessionalSubCategories
    {    
        public long Id { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }
        [Column("sub_category_id")]
        public long SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public FieldSubCategory SubCategory { get; set; }

    }
}
