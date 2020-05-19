using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("company_account_info")]
    public class CompanyAccountInfo
    {
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [Column("bank_name")]
        public string BankName { get; set; }
        [Column("branch_name")]
        public string BranchName { get; set; }
        [Column("account_number")]
        public string AccountNumber { get; set; }
        [Column("sort_code")]
        public string SortCode { get; set; }
        [Column("company_number")]
        public string CompanyNumber { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string email { get; set; }
        public string website { get; set; }

    }
}