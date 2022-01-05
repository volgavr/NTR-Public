using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearentScheduler.JsonResponseWraps
{
    #region Wraps

    #region PaymentWraps
    public class PaymentRootObject
    {
        public PaymentRootObject()
        {
            conversion_ids = new List<int>();
        }

        public string refersion_public_key { get; set; }
        public string refersion_secret_key { get; set; }
        public List<int> conversion_ids { get; set; }
        public string payment_method { get; set; }
    }

    public class PaymentResult
    {
        public int payment_id { get; set; }
    }

    #endregion

    #region ConversionWraps
    public class ConversionsRootObject
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public Data()
        {
            Conversions = new List<Conversion>();
        }
        public List<Conversion> Conversions { get; set; }
    }

    public class Conversion
    {
        public int Id { get; set; }
        public int Affiliate_Id { get; set; }
        public string Subscription_id { get; set; }
        public bool PromoCreditAdded { get; set; }
    }
    #endregion

    #region AffiliateWraps
    public class AffiliateAndConversions
    {
        public AffiliateAndConversions()
        {
            Conversions = new List<Conversion>();
        }
        public string RefersionId { get; set; }
        public int RefersionGraphQLId { get; set; }
        public List<Conversion> Conversions { get; set; }
    }
    #endregion

    #region PagesWraps
    public class PagesConversion
    {
        public int id { get; set; }
    }

    public class TotalResults
    {
        public int Conversions { get; set; }
    }

    public class PageInfo
    {
        public TotalResults Total_results { get; set; }
    }

    public class PagesData
    {
        //public List<PagesConversion> conversions { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PagesRootObject
    {
        public PagesData Data { get; set; }
    }
    #endregion

    #endregion
}
