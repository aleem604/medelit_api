using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    [Table("tin_user", Schema = TinDbObjects.ActiveSchema)]
    public class TinUser : BaseAuth
    {
        public long Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        [Column("primary_address")]
        public string PrimaryAddress { get; set; }
        [Column("secondary_address")]
        public string SecondaryAddress { get; set; }
        [Column("user_type")]
        public eUserType UserType { get; set; }
        [Column("image_url")]
        public string ImageUrl { get; set; }
        [Column("verification_token")]
        public string VerificationToken { get; set; }
        public List<TinUserRole> TinUserRole { get; set; }
    }
}
