using ClearentScheduler.ClearentQuestAPI;
using Newtonsoft.Json;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ClearentScheduler
{
    public static class ClearentRequestHandler
    {
        private static string host = ConfigurationManager.AppSettings["serverHost"]; //"gateway-sb.clearent.net";
        private static string transactionEndpoint = ConfigurationManager.AppSettings["transactionEndpoint"]; // "/rest/v2/transactions";
        private static string achTransactionEndpoint = ConfigurationManager.AppSettings["ach-transaction-endpoint"]; // "/rest/v2/ach/transactions";
        private static int apiKeyValidLength = GetApiKeyTokenLength();
        private static bool trustAllCerts = true;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //private string apiKey = "82017d98c7b74c99898f8b5b53bb93ef";
        
        public static string TransactionRequest(string apiKey, DateTime fromDate, DateTime toDate, int page, bool approvedOnly = false)
        {
            //Trust all certificates
            if (trustAllCerts)
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var query = string.Format("start-date={0}&end-date={1}", WebUtility.UrlEncode(fromDate.ToString("yyyy-MM-dd HH:mm")), WebUtility.UrlEncode(toDate.ToString("yyyy-MM-dd HH:mm")));
            if(page > 0)
            {
                query += "&page="+page.ToString();
            }

            if (approvedOnly)
            {
                query += "&status=APPROVED";
            }

            var request = (HttpWebRequest)WebRequest.Create("https://" + host + transactionEndpoint + "?" + query);
            request.Method = "GET";
            //request.UserAgent = RequestConstants.UserAgentValue;
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("api-key", apiKey);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;
            try
            {
#if DEBUG
                if (apiKeyValidLength > 0 && apiKey.Length != apiKeyValidLength)
                {
                    throw new ArgumentException("API Key: " + apiKey + " is considered to be invalid (skipped)");
                }
#endif
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return content;

                    if(response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Server responded with status: " + response.StatusCode.ToString());
                    }

                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(request.RequestUri);
                throw;
            }
            return content;
        }

        public static string AchTransactionRequest(string apiKey, DateTime fromDate, int page)
        {
            //Trust all certificates
            if (trustAllCerts)
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            var query = string.Format("status-change-start-date={0}&date-format={1}", WebUtility.UrlEncode(fromDate.ToString("yyyy-MM-dd HH:mm")), WebUtility.UrlEncode("yyyy-MM-dd HH:mm"));
            if (page > 0)
                query += "&page=" + page.ToString();

            var request = (HttpWebRequest)WebRequest.Create("https://" + host + achTransactionEndpoint + "?" + query);
            request.Method = "GET";
            //request.UserAgent = RequestConstants.UserAgentValue;
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("api-key", apiKey);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;
            try
            {
#if DEBUG
                if (apiKeyValidLength > 0 && apiKey.Length != apiKeyValidLength)
                {
                    throw new ArgumentException("API Key: " + apiKey + " is considered to be invalid (skipped)");
                }
#endif
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return content;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Server responded with status: " + response.StatusCode.ToString());
                    }

                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(request.RequestUri);
                throw;
            }
            return content;
        }

        private static int GetApiKeyTokenLength()
        {
            var lenStr = ConfigurationManager.AppSettings["api-key-token-length"];
            if (string.IsNullOrWhiteSpace(lenStr))
                return 0;

            try
            {
                return int.Parse(lenStr);
            }      
            catch(Exception ex)
            {
                throw new ConfigurationErrorsException("api-key-token-length is incorrect", ex);
            }
        }
    }
}
