using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("vat")]
    public class Vat
    {
        public long Id { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }

    }
}
