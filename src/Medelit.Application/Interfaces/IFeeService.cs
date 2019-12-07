using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IFeeService : IDisposable
    {
        void SaveFee(FeeViewModel feeViewModel);
        dynamic GetFees();
        dynamic FindFees(SearchViewModel model);
        void UpdateStatus(IList<FeeViewModel> fees, eRecordStatus status);
        void DeleteFees(IList<long> feeIds);
    }
}
