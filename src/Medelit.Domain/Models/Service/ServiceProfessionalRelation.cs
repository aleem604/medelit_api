using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("service_professional_relation")]
  public  class ServiceProfessionalRelation
    {    
        public long Id { get; set; }
        [Column("service_id")]
        public long ServiceId { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        public virtual Service Service { get; set; }
        public virtual Professional Professional { get; set; }
    }
}
