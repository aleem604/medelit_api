using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IFieldSubcategoryService : IDisposable
    {
        dynamic GetFields();
        dynamic FindFields(SearchViewModel model);
    }
}
