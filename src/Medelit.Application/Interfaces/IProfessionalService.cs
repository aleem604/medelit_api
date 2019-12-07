using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IProfessionalService : IDisposable
    {
        dynamic FindProfessionals(SearchViewModel model);
        dynamic GetProfessionals();
        dynamic GetProfessionalById(long professionalId);
        void SaveProvessional(ProfessionalRequestViewModel model);
        void UpdateStatus(IEnumerable<ProfessionalRequestViewModel> pros, eRecordStatus status);
        void DeleteFees(IEnumerable<long> feeIds);
    }
}
