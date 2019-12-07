using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
   public class FeeViewModel : BaseViewModel
    {
        public string FeeName { get; set; }
        public string FeeCode { get; set; }
        public eFeeType FeeTypeId { get; set; }
        public string Taxes { get; set; }
        public string ConnectedServices { get; set; }
        public string Tags { get; set; }
        public decimal? A1 { get; set; }
        public decimal? A2 { get; set; }
    }
}
