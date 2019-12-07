using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("duration")]
    public class Duration
    {
        public long Id { get; set; }
        public long Value { get; set; }
        public string Unit  { get; set; }

    }
}
