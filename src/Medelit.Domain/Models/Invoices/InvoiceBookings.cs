using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("invoice_bookings")]
    public class InvoiceBookings
    {
        public long Id { get; set; }

        [Column("invoice_id")]
        public long InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        [Column("booking_id")]
        public long BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

    }
}
