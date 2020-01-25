using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("professional_language_relation")]
  public  class ProfessionalLanguages
    {    
        public long Id { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public virtual Professional Professional { get; set; }
        [Column("language_id")]
        public int LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }

    }
}
