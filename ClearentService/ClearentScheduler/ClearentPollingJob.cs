using ClearentScheduler.ClearentQuestAPI;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearentScheduler
{
    public class ClearentPollingJob : IJob
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (var dbc = new SqlDBProvider())
                {
                    dbc.Init(ConfigurationManager.ConnectionStrings["SQLServerConnectionString"].ConnectionString);
                    //WriteToFile("User API Keys: " + string.Join("; ", dbc.GetUserApiKeys()) + ". Requested at " + DateTime.Now);
                    var uapik = dbc.GetUserApiKeys();
                    logger.Debug(uapik.Count > 0 ? "Found " + uapik.Count + " User API Keys (" + string.Join("; ", uapik.Values.Take(20)) + (uapik.Count > 20 ? "..." : "") + ")" : "User API Keys not Found.");

                    foreach (var apiKey in uapik)
                    {
                        logger.Debug("Requested to Clearent API with Key=" + apiKey.Value);
                        var startDate = dbc.GetLastTransactionDate(apiKey.Key) ?? DateTime.UtcNow.Date.AddDays(-7);
                        if ((DateTime.UtcNow - startDate).TotalDays >= 30)
                            startDate = startDate.AddSeconds(-startDate.Second).AddMinutes(1);

                        DateTime lastTransactionDate = DateTime.MinValue;
                        bool pageIsLast = true;
                        int requestPageNumber = 1;

                        logger.Debug("Requesting Transactions from Date: " + startDate.ToString("u"));

                        //Loop is required due to the request pagination.
                        do
                        {
                            string resp = "";
                            try
                            {
                                resp = ClearentRequestHandler.TransactionRequest(apiKey.Value, startDate, DateTime.UtcNow, requestPageNumber, approvedOnly: true);
                            }
                            catch (Exception ex)
                            {
                                logger.Info(ex, "Request failed.");
                                break;
                            }

                            logger.Debug("Clearent Response Received. Content-Length: " + resp.Length);
                            if (resp == "")
                                break; //breaks DO-loop
                            
                            ClearentResponse respObj = null;
                            try
                            {
                                respObj = JsonConvert.DeserializeObject<ClearentResponse>(resp);
                                logger.Debug("Payload type: " + respObj.Payload.PayloadType + ". Page: " + respObj.Page.Number + " of " + respObj.Page.TotalPages);
                                requestPageNumber = respObj.Page.Number + 1;
                                pageIsLast = respObj.Page.IsLast || respObj.Page.Number == respObj.Page.TotalPages;
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                break;
                            }

                            if (respObj.Payload.PayloadType == "error")
                            {
                                logger.Warn("Error: " + respObj.Payload.Error?.Message);
                                break;
                            }

                            if (respObj.Payload.PayloadType != "transactions")
                            {
                                logger.Warn("Payload type not supported: " + respObj.Payload.PayloadType);
                                break;
                            }

                            logger.Debug("Number of transactions: " + respObj.Payload.Transactions?.Length);

                            for (int i = 0; i < respObj.Payload.Transactions.Length; i++)
                            //foreach (var tran in respObj.Payload.Transactions)
                            {
                                var tran = respObj.Payload.Transactions[i];
                                if (!string.IsNullOrEmpty(tran.Invoice))
                                {
                                    logger.Debug("Processing Invoice#" + tran.Invoice);
                                    var invoiceNumber = tran.Invoice.ToInternalNumber();
                                    try
                                    {
                                        logger.Debug("Transaction#{0}: {1}", tran.Id, tran.Result);
                                        if (tran.ResultCode == "000")
                                        {
                                            if (tran.Created.HasValue && (tran.Created > lastTransactionDate))
                                                lastTransactionDate = tran.Created.Value;

                                            var res = dbc.UpdatePaymentInfo(apiKey.Key, invoiceNumber, tran.Id.ToString(), "transaction", string.IsNullOrEmpty(tran.DisplayMessage) ? "Transaction Approved" : tran.DisplayMessage, tran.Created ?? DateTime.UtcNow, tran.Amount);
                                            if (res.HasFlag(PaymentUpdateResult.PaymentCreated))
                                            {
                                                logger.Debug("Payment created.");
                                                SendGridAPI.SendGridHelper.Send(dbc.GetUserEmail(apiKey.Key), Helper.NotificationSubject(dbc.GetCustomerName(invoiceNumber, apiKey.Key), tran.Amount), Helper.NotificationContent(invoiceNumber, tran.Amount, tran.Id.ToString()), null).Wait();                                                
                                            }
                                            if (res.HasFlag(PaymentUpdateResult.InvoiceUpdated))
                                                logger.Debug("Invoice status updated.");
                                        }
                                    }
                                    catch (Exceptions.InvoiceNotFoundException ex)
                                    {
                                        logger.Warn(ex.Message);
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e);
                                    }
                                }
                                else
                                    logger.Info("Invoice number is empty, Transaction#{0}: {1} ({2})", tran.Id, tran.DisplayMessage, tran.Result);
                            }
                            //Save date of last transaction
                            if (lastTransactionDate > DateTime.MinValue)
                            {
                                dbc.SetLastTransactionDate(apiKey.Key, lastTransactionDate);
                                logger.Debug("Last Transaction Date: " + lastTransactionDate.ToString("u"));
                            }
                        }
                        while (!pageIsLast);//Request the next page.

                        //Search ACH Transactions
                        startDate = dbc.GetACHTransactionDate(apiKey.Key) ?? DateTime.UtcNow.Date.AddDays(-7);
                        if ((DateTime.UtcNow - startDate).TotalDays >= 30)
                            startDate = startDate.AddSeconds(-startDate.Second).AddMinutes(1);

                        pageIsLast = true;
                        requestPageNumber = 1;
                        lastTransactionDate = DateTime.MinValue;
                        DateTime firstPendingDate = DateTime.Today;

                        logger.Debug("Requesting ACH Transactions from Date: " + startDate.ToString("u"));

                        do
                        {
                            string resp = "";
                            try
                            {
                                resp = ClearentRequestHandler.AchTransactionRequest(apiKey.Value, startDate, requestPageNumber);
                            }
                            catch (Exception ex)
                            {
                                logger.Info(ex, "Request failed.");
                                break;
                            }
                            logger.Debug("Clearent Response Received. Content-Length: " + resp.Length);

                            if (resp == "")
                                break; //breaks DO-loop

                            ClearentResponse respObj = null;
                            try
                            {
                                respObj = JsonConvert.DeserializeObject<ClearentResponse>(resp);
                                logger.Debug("Payload type: " + respObj.Payload.PayloadType + ". Page: " + respObj.Page.Number + " of " + respObj.Page.TotalPages);
                                requestPageNumber = respObj.Page.Number + 1;
                                pageIsLast = respObj.Page.IsLast || respObj.Page.Number == respObj.Page.TotalPages;
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                break;
                            }

                            if (respObj.Payload.PayloadType == "error")
                            {
                                logger.Warn("Error: " + respObj.Payload.Error?.Message);
                                break;
                            }

                            if (respObj.Payload.PayloadType != "ach-transactions")
                            {
                                logger.Warn("Payload type not supported: " + respObj.Payload.PayloadType);
                                break;
                            }

                            logger.Debug("Number of ACH transactions: " + respObj.Payload.AchTransactions?.Length);

                            for (int i = 0; i < respObj.Payload.AchTransactions.Length; i++)
                            {
                                var tran = respObj.Payload.AchTransactions[i];
                                if (!string.IsNullOrEmpty(tran.Invoice))
                                {
                                    logger.Debug("Processing Invoice#" + tran.Invoice);
                                    var invoiceNumber = tran.Invoice.ToInternalNumber();
                                    try
                                    {
                                        logger.Debug("Transaction#{0}: {1}", tran.Id, tran.Status);
                                        if (tran.StatusChangeDate.HasValue && (tran.StatusChangeDate > lastTransactionDate))
                                            lastTransactionDate = tran.StatusChangeDate.Value;

                                        if (tran.Status == "SETTLED" || tran.Status.ToUpper() == "SETTLED")
                                        {
                                            var res = dbc.UpdatePaymentInfo(apiKey.Key, invoiceNumber, tran.Id, "ach-transaction", string.IsNullOrEmpty(tran.DisplayMessage) ? "Transaction Settled." : tran.DisplayMessage, (tran.SettledDate ?? tran.StatusChangeDate) ?? DateTime.UtcNow, tran.Amount);
                                            if (res.HasFlag(PaymentUpdateResult.PaymentCreated))
                                            {
                                                logger.Debug("Payment created.");
                                                SendGridAPI.SendGridHelper.Send(dbc.GetUserEmail(apiKey.Key), Helper.NotificationSubject(dbc.GetCustomerName(invoiceNumber, apiKey.Key), tran.Amount), Helper.NotificationContent(invoiceNumber, tran.Amount, tran.Id.ToString(), isACH: true), null).Wait();                                                
                                            }
                                            if (res.HasFlag(PaymentUpdateResult.InvoiceUpdated))
                                                logger.Debug("Invoice status updated.");
                                        }
                                        else
                                        {
                                            if (tran.Status == "PENDING" && tran.StatusChangeDate.HasValue && tran.StatusChangeDate < firstPendingDate)
                                                firstPendingDate = tran.StatusChangeDate.Value;
                                        }

                                    }
                                    catch (Exceptions.InvoiceNotFoundException ex)
                                    {
                                        logger.Warn(ex.Message);
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Error(e);
                                    }
                                }
                                else
                                    logger.Info("Invoice number is empty, Transaction#{0}: {1} ({2})", tran.Id, tran.DisplayMessage, tran.Status);
                            }
                            //Save date of last check-in
                            if (lastTransactionDate > DateTime.MinValue)
                            {
                                dbc.SetLastTransactionDate(apiKey.Key, firstPendingDate < lastTransactionDate ? firstPendingDate : lastTransactionDate, isACHTransaction: true);
                                logger.Debug("Last ACH Transaction Date: " + (firstPendingDate < lastTransactionDate ? firstPendingDate : lastTransactionDate).ToString("u"));
                            }
                        }
                        while (!pageIsLast);
                    }

                }
            }
            catch(Exception ex) {
                logger.Error(ex);
            }
        }
        
    }
}
