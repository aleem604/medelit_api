using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public enum eInvoiceFilter : short
    {
        All,
        ToBeSent,
        InsurancePending,
        Refunded
    }
}
