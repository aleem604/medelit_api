using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Interfaces
{
    public interface IDataContext : IDisposable
    {
        string Source { get; }
        void SaveChanges();
    }
}
