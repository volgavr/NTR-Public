using Dapper;
using Newtonsoft.Json;
using NLog;
using Quartz;
using ClearentScheduler.JsonResponseWraps;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ChargeBee.Api;
using ChargeBee.Models;

namespace ClearentScheduler
{
    class ClearentChecksJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            try
            {                
                logger.Info("Starting Refersion Checking Job.");
                int hours = Convert.ToInt32(ConfigurationManager.AppSettings["timeSpanHours"]);
                int unixCreatedFrom = (int)(DateTime.UtcNow.AddHours(-hours).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int unixCreatedTo = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var createdFrom = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.UtcNow.AddHours(-hours));
                var createdTo = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.UtcNow);
                int amount = Convert.ToInt32(ConfigurationManager.AppSettings["graphQLApiQueryPagination"]);
                int skip = amount;

                string graphQLAccessToken = ConfigurationManager.AppSettings["graphQLAccessToken"];
                string graphQLApiUrl = ConfigurationManager.AppSettings["graphQLApiUrl"];

                bool lastPage = false;
                var i = 0;

                logger.Debug("Checking conversions created from {0} to {1}", createdFrom, createdTo);
                while (!lastPage)
                {
                    //PromoCreditProcess(unixCreatedFrom, unixCreatedTo, amount, skip * i, graphQLAccessToken, graphQLApiUrl, ref lastPage, i);
                    i++;
                }

                logger.Info("Refersion Checking Job is done.");
            }
            catch (Exception e)
            {
                logger.Error(e, "An error occured while executing RefersionChecksJob");
                return;
            }           
        }

        //void PromoCreditProcess(int createdFrom, int createdTo, int amount, int skip, string graphQLAccessToken, string graphQLApiUrl, ref bool lastPage, int page)
        //{
        //    string query = "{ conversions (created_from: " + createdFrom + " created_to: " + createdTo + " status: \"APPROVED\" payment_status: \"UNPAID\" first: " + amount + " offset: " + skip + ") { id, affiliate_id, subscription_id } }";

        //    logger.Info("Getting conversions for affiliates (page: {0}) ...", page);

        //    var conversions = new ConversionsRootObject();
        //    string jsonResult = string.Empty;

        //    try
        //    {
        //        jsonResult = GetGraphQLResponse(graphQLAccessToken, query, graphQLApiUrl);
        //        conversions = JsonConvert.DeserializeObject<ConversionsRootObject>(jsonResult);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e, "An error occured while getting conversions from Refersion GraphQL Api: json_response - {0}", jsonResult);
        //        return;
        //    }

        //    if (conversions.Data.Conversions.Count < amount)
        //        lastPage = true;

        //    var receivedAffiliatesAndConversions = conversions.Data.Conversions
        //        .GroupBy(g => g.Affiliate_Id)
        //        .Select(x => new
        //        {
        //            affiliate = x.Key,
        //            conversions = x.Select(y => y).ToList()
        //        })
        //        .ToList();

        //    var affAmount = receivedAffiliatesAndConversions.Count;

        //    if (affAmount == 0)
        //    {
        //        logger.Debug("Total amount of received affiliates on page {0} : {1}", page, affAmount);
        //        return;
        //    }

        //    logger.Info("Total amount of received affiliates: {0}", affAmount);

        //    //logger.Trace("Recieved affiliates ids: "); // Do we need to record received affiliate ids in log file?

        //    logger.Info("Filtering received affiliates...");

        //    var affiliatesAndProcessedConversions = new List<AffiliateAndConversions>();

        //    logger.Info("Getting data from 'onlinerespondents' and 'processed_conversions'...");

        //    var connectionString = ConfigurationManager.AppSettings["SchedulerDBConnectionString"];
        //    IDbConnection db = new SqlConnection(connectionString);
        //    db.Open();
        //    var d = db
        //            .Query<dynamic>("select ors.refersionId, ors.refersionGraphQLId, pc.subscriptionId, pc.conversionGraphQLId, pc.promoCreditAdded " +
        //            "from onlinerespondents ors " +
        //            "left join processed_conversions pc on (ors.refersionGraphQLId = pc.refersionGraphQLId) " +
        //            "where ors.refersionId is not null")
        //            .ToList()
        //            .GroupBy(g => new { Refersion = g.refersionId, GraphQL = g.refersionGraphQLId })
        //            .ToList();

        //    logger.Info("Getting subscriptionIds of conversions, which already have payment object in refersion...");

        //    var paidSubscriptions = db
        //        .Query<dynamic>("select pc.subscriptionId from processed_conversions pc where pc.promoCreditAdded = 0").ToList();

        //    db.Close();


        //    logger.Info("Preparing buff object...");
        //    var buff = d
        //            .Select(x => new
        //            {
        //                RefersionId = x.Key.Refersion,
        //                RefersionGraphQLId = x.Key.GraphQL,
        //                Conversions = x.Select(y =>
        //                new
        //                {
        //                    subscriptionId = y.subscriptionId ?? null,
        //                    conversionGraphQLId = y.conversionGraphQLId ?? null,
        //                    y.promoCreditAdded
        //                }
        //                )
        //                .Where(z => z.subscriptionId != null && z.conversionGraphQLId != null)
        //                .Select(u => new { u.subscriptionId, u.conversionGraphQLId, u.promoCreditAdded })
        //                .ToList()
        //            })
        //            .ToList();

        //    logger.Info("Forming collection of affiliates and processed conversions...");

        //    foreach (var item in buff)
        //    {
        //        var aac = new AffiliateAndConversions { RefersionId = item.RefersionId, RefersionGraphQLId = item.RefersionGraphQLId };
        //        foreach (var item1 in item.Conversions)
        //        {
        //            aac.Conversions.Add(new Conversion { Id = item1.conversionGraphQLId, Subscription_id = item1.subscriptionId, PromoCreditAdded = item1.promoCreditAdded });
        //        }
        //        affiliatesAndProcessedConversions.Add(aac);
        //    }

        //    logger.Info("Forming collection of affiliates and conversions to pay...");

        //    var affiliatesAndConversionsToPay = receivedAffiliatesAndConversions
        //        // Filter requested conversions to get only conversions bonded to our affiliates
        //        .Where(x => affiliatesAndProcessedConversions
        //                    .Any(y => y.RefersionGraphQLId == x.affiliate)
        //        )
        //        .Select(z => new
        //        {
        //            conversions = z.conversions.
        //            // Exclude processed conversions - conversions, which are new and unpaid (no binded "Payment" object) and paid ones (we cannot "pay" them again, need to filter them later)
        //            Where(o => !affiliatesAndProcessedConversions
        //                        .SelectMany(b => b.Conversions)
        //                        .Any(v => v.Subscription_id == o.Subscription_id)
        //                        ||
        //                        affiliatesAndProcessedConversions
        //                        .SelectMany(b => b.Conversions)
        //                        .Any(v => v.Subscription_id == o.Subscription_id && !v.PromoCreditAdded)
        //            )
        //        }
        //        ).SelectMany(c => c.conversions)
        //        .ToList()
        //        .GroupBy(g => g.Affiliate_Id)
        //        .Join(affiliatesAndProcessedConversions,
        //                   x => x.Key,
        //                   y => y.RefersionGraphQLId,
        //                   (x, y) => new
        //                   {
        //                       Conversions = x.ToList(),
        //                       y.RefersionId,
        //                       y.RefersionGraphQLId
        //                   }
        //        ).ToList();

        //    affAmount = affiliatesAndConversionsToPay.Count;

        //    logger.Debug("Amount of required affiliates: {0}", affAmount);

        //    if (affAmount > 0)
        //    {
        //        logger.Info("Conversions processing...");

        //        db.Open();

        //        foreach (var item in affiliatesAndConversionsToPay)
        //        {
        //            //Get Customer A subscription_id
        //            var values = db
        //            .Query("select chargeBeeId, refersionId, email from onlinerespondents where refersionGraphQLId = @gid",
        //                    new { gid = item.RefersionGraphQLId })
        //            .FirstOrDefault();

        //            string custASubscId = values.chargeBeeId;
        //            string custARefersionId = values.refersionId;
        //            string custAEmail = values.email;

        //            logger.Debug("Sending payment to Refersion for conversions of affiliate {0}", item.RefersionId);

        //            string paymentResponse = string.Empty;

        //            try
        //            {
        //                var payment = new PaymentRootObject
        //                {
        //                    payment_method = "MANUAL",
        //                    refersion_public_key = ConfigurationManager.AppSettings["refersionPublicKey"],
        //                    refersion_secret_key = ConfigurationManager.AppSettings["refersionSecretKey"],
        //                    // paid subscriptions filter, no sent promo for those subscriptions
        //                    conversion_ids = item.Conversions.Where(y => !paidSubscriptions.Any(z => z.subscriptionId == y.Subscription_id)).Select(x => x.Id).ToList()
        //                };

        //                var paymentJson = JsonConvert.SerializeObject(payment, Formatting.Indented);

        //                //Send payment to refersion for conversions
        //                paymentResponse = GetJsonResponse(ConfigurationManager.AppSettings["refersionProcessManualPayment"], paymentJson);
        //                //var paymentResult = JsonConvert.DeserializeObject<PaymentResult>(jsonResult);
        //            }
        //            catch (Exception e)
        //            {
        //                // either something went wrong on Refersion, or there is no way to deserialize response to required type

        //                logger.Error(e, "An error occured while sending payment to Refersion Api: json_response - {0}", paymentResponse);
        //                continue;
        //            }

        //            //Foreach 
        //            foreach (var item1 in item.Conversions)
        //            {
        //                try
        //                {
        //                    logger.Debug("Storing processed conversion info : affiliate_id - {0}, conversion_id - {1}", item1.Affiliate_Id, item1.Id);

        //                    var stored = db.Query<string>("select id from processed_conversions where subscriptionId = @sid", new { sid = item1.Subscription_id }).Count() > 0;

        //                    if (!stored)
        //                    {
        //                        db.Execute("insert into processed_conversions (refersionGraphQLId, subscriptionId, conversionGraphQLId, promoCreditAdded) values (@A, @B, @C, @D)"
        //                            , new { A = item.RefersionGraphQLId, B = item1.Subscription_id, C = item1.Id, D = 0 });
        //                    }

        //                    logger.Debug("(Workaround) Mark conversion as processed for invited customer (subscription_id : {0})", item1.Subscription_id);
        //                    //SendPromoCredit(item1.Subscription_id);

        //                    db.Execute("update processed_conversions set promoCreditAdded = @D where subscriptionId = @E"
        //                            , new { D = 1, E = item1.Subscription_id });                            
        //                }
        //                catch (Exception e)
        //                {
        //                    logger.Error(e, "An error occured while sending promotional credit to ChargeBee : subscription_id : {0}", item1.Subscription_id);
        //                    continue;
        //                }

        //                logger.Debug("Sending promo-credit for customer with affiliate (subscription_id : {0})", custASubscId);
        //                try
        //                {
        //                    SendPromoCreditAndAddCouponCode(custASubscId, custARefersionId);
        //                }
        //                catch(Exception e)
        //                {
        //                    logger.Error(e, "An error occured while sending promotional credit to ChargeBee : subscription_id : {0}", custASubscId);
        //                }

        //                SendEventToVero(custAEmail, "referral_conversion");

        //            }
        //        }

        //        db.Close();
        //    }


        //    db.Dispose();
        //}

        //void SendPromoCredit(string subscriptionId)
        //{            
        //        var amount = Convert.ToInt32(ConfigurationManager.AppSettings["chargeBeePromoCreditAmount"]);
        //        ApiConfig.Configure(ConfigurationManager.AppSettings["ChargeBeeSiteName"], ConfigurationManager.AppSettings["ChargeBeeApiKey"]);
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        EntityResult resultSubs = Subscription.Retrieve(subscriptionId).Request();
        //        EntityResult result = PromotionalCredit.Add()
        //                          .CustomerId(resultSubs.Customer.Id)
        //                          .Amount(amount)
        //                          .Description("add promotional credits").Request();
        //        logger.Debug("ChargeBee promo credit Id : {0}", result.PromotionalCredit.Id);

        //}

        //void SendPromoCreditAndAddCouponCode(string subscriptionId, string refersionId)
        //{
        //    var amount = Convert.ToInt32(ConfigurationManager.AppSettings["chargeBeePromoCreditAmount"]);
        //    var couponSetId = ConfigurationManager.AppSettings["chargeBeeCouponSetId"];

        //    ApiConfig.Configure(ConfigurationManager.AppSettings["ChargeBeeSiteName"], ConfigurationManager.AppSettings["ChargeBeeApiKey"]);
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    EntityResult resultSubs = Subscription.Retrieve(subscriptionId).Request();
        //    EntityResult result = PromotionalCredit.Add()
        //                      .CustomerId(resultSubs.Customer.Id)
        //                      .Amount(amount)
        //                      .Description("add promotional credits").Request();
        //    logger.Debug("ChargeBee promo credit Id : {0}", result.PromotionalCredit.Id);

        //    ListResult result2 = CouponCode.List().Request();

        //    if (!result2.List.Select(x => x.CouponCode.Code).Contains("SHARECE" + refersionId))
        //    {
        //        EntityResult result1 = CouponSet.AddCouponCodes(couponSetId)
        //                        .Code(new List<String>(new String[] { "SHARECE" + refersionId }))
        //                        .Request();
        //        logger.Debug("ChargeBee coupon code Id : {0}", result1.Coupon.Id);
        //    }                       
        //}
        string GetClearentResponse(string accessToken, string query, string url)
        {
            string jsonResponse;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("X-Access-Key", accessToken);

            var queryParams = new Dictionary<string, string>
            {
                { "query", query }
            };

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(queryParams, Formatting.Indented); ;

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                logger.Debug("Request headers: {0}, Request body: {1}", httpWebRequest.Headers.ToString(), json);

            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                jsonResponse = streamReader.ReadToEnd();
            }

            logger.Trace("Response: {0}", jsonResponse);

            return jsonResponse;
        }

        string GetGraphQLResponse(string graphQLAccessToken, string query, string url)
        {
            string jsonResponse;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("X-Refersion-Key", graphQLAccessToken);

            var queryParams = new Dictionary<string, string>
            {
                { "query", query }
            };

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(queryParams, Formatting.Indented); ;

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                logger.Debug("Request headers: {0}, Request body: {1}", httpWebRequest.Headers.ToString(), json);

            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                jsonResponse = streamReader.ReadToEnd();
            }

            logger.Trace("Response: {0}", jsonResponse);

            return jsonResponse;
        }

        string GetJsonResponse(string url, string data)
        {
            string result;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";


            logger.Debug("Request headers: {0}, Request body: {1}", httpWebRequest.Headers.ToString(), data);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            logger.Trace("Response: {0}", result);
            return result;
        }

        //void SendEventToVero(string customerEmail, string eventName)
        //{
        //    try
        //    {
        //        string result;
        //        var identity = new Dictionary<string, string> {
        //            {"id", customerEmail},
        //            {"email", customerEmail }
        //        };
        //        var data = new Dictionary<string, object>
        //        {
        //            {"auth_token", ConfigurationManager.AppSettings["veroAuthToken"]},
        //            {"identity", identity},
        //            {"event_name", eventName}
        //        };

        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["veroApiUrlEvent"]);
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Method = "POST";

        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //            streamWriter.Close();

        //            logger.Debug("Request headers: {0}, Request body: {1}", httpWebRequest.Headers.ToString(), json);
        //        }

        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            result = streamReader.ReadToEnd();
        //        }

        //        logger.Trace("Response: {0}", result);
        //    }
        //    catch(Exception e)
        //    {
        //        logger.Error(e, "An error occured while sending referral_conversion event to Vero : email : {0}", customerEmail);
        //    }
            
        //}
    }
}
