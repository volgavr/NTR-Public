using System;
using System.Configuration;
using System.Linq;

namespace ClearentScheduler
{
    public static class Helper
    {
        public static string ToInternalNumber(this string invoiceInumber)
        {
            return invoiceInumber?.StartsWith("CE") == true ? new string(invoiceInumber.ToCharArray().SkipWhile(c => !Char.IsDigit(c) || c == '0').ToArray()) : invoiceInumber;
        }

        public static TransactionStatus ToTransactionStatus(this string resultCode)
        {
            return (resultCode == "000" || resultCode == "APPROVED") ? TransactionStatus.Approved : TransactionStatus.Declined;
        }

        public static string NotificationSubject(string customerName, decimal amount)
        {
            return ConfigurationManager.AppSettings["mailSubjectTemplate"]?.Replace("{{amount}}", amount.ToString("C2"))?.Replace("{{customer name}}", customerName);
        }

        public static string NotificationContent(string invoiceNumber, decimal amount, string transactionId, bool isACH = false)
        {
            return ConfigurationManager.AppSettings["mailBodyTemplate"]?.Replace("{{invoice number}}", invoiceNumber)?.Replace("{{amount}}", amount.ToString("C2"))?.Replace("{{transaction id}}", transactionId)?.Replace("{{ACH}}", isACH ? "ACH" : "");
        }
    }
}
