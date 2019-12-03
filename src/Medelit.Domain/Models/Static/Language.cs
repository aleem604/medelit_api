using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("language")]
    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
