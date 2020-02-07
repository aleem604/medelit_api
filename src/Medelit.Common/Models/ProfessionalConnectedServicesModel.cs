using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common.Models
{
   public class ProfessionalConnectedServicesModel
    {
        public long? ProfessionalId { get; set; }
        public long? ServiceId { get; set; }
        public string CService { get; set; }
        public string CField { get; set; }
        public string CSubcategory { get; set; }

        public long? PtFeeRowId { get; set; }
        public long? PtFeeId { get; set; }
        public string PtFeeName { get; set; }
        public decimal? PtFeeA1 { get; set; }
        public decimal? PtFeeA2 { get; set; }

        public long? ProFeeRowId { get; set; }
        public long? ProFeeId { get; set; }
        public string ProFeeName { get; set; }
        public decimal? ProFeeA1 { get; set; }
        public decimal? ProFeeA2 { get; set; }
    }

    //public class ProfessionalConnectedServicesComparer : IEqualityComparer<ProfessionalConnectedServicesModel>
    //{
    //    public bool Equals(ProfessionalConnectedServicesModel emp1, ProfessionalConnectedServicesModel emp2)
    //    {
    //        if (emp1.Key == emp2.Key)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public int GetHashCode(ProfessionalConnectedServicesModel obj)
    //    {
    //        return obj.Key.GetHashCode();
    //    }
    //}
}
