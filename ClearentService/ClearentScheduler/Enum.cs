namespace ClearentScheduler
{
    public enum TransactionStatus { Approved, Declined };
    public enum InvoiceStatus { Pending, NotPaid, Paid };
    public enum PaymentMethod { Cash, Card, ACH }
    public enum PaymentStatus { Completed, Pending }
    public enum ACHDatePeriod { custom_range, last_24_hours, yesterday, last_week, last_month, last_180_days }
    public enum OptionCode { APIKey = 1301, PublicKey = 1302, LastTransactionDate = 1320, ACHCheckInDate = 1321 }
    [System.Flags]
    public enum PaymentUpdateResult { Nothing, PaymentCreated, InvoiceUpdated }
}