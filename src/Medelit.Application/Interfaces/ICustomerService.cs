using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface ICustomerService : IDisposable
    {
        dynamic GetCustomers();
        dynamic FindCustomers(SearchViewModel model);
        CustomerViewModel GetCustomerById(long leadId);
        void SaveCustomer(CustomerViewModel model);
        void UpdateStatus(IEnumerable<CustomerViewModel> leads, eRecordStatus status);
        void DeleteCustomers(IEnumerable<long> leadIds);
        void CreateBooking(CustomerViewModel viewModel);
    }
}
