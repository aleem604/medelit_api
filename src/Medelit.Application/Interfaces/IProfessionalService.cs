using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IProfessionalService : IDisposable
    {
        dynamic GetProfessionals();
        dynamic FindProfessionals(SearchViewModel model);
    }
}
