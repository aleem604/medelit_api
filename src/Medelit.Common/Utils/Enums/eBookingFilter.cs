namespace Medelit.Common
{
    
    public enum eBookingFilter : short
    {
        All,
        Pending,
        AwaitingPayment,
        TodayVisits,
        FutureVisits,
        Delivered,
        ToBeInvoicesPaid,       // To Be Invoiced (payment status paid, payment method not insurance, empty invoice number )
        ToBeInvoicedNotPaid,    // To Be Invoiced (payment status not paid, payment method insurance, empty invoice number )
    }
}
