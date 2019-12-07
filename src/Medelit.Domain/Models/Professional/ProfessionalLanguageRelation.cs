using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("professional_language_relation")]
  public  class ProfessionalLanguageRelation
    {    
        public long Id { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [Column("language_id")]
        public long LanguageId { get; set; }
        public virtual Professional Professional { get; set; }
    }
}
