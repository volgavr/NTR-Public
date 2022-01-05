using Newtonsoft.Json;
using System;

namespace ClearentScheduler.ClearentQuestAPI
{
    /// <summary>
    /// If the payload type is transaction, the only guaranteed fields coming back on that are ID, result-code, authorization-code, display-message.
    /// Any of the other fields (cardType, expDate, etc) they will attempt to return but we're aware of a small percentage of the time they may not be available.
    /// </summary>
    [Serializable]
    public class Transaction
    {
        /// <summary>
        /// ID (guaranteed)
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Amount of transaction
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Date in UTC the transaction was initiated.
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// transaction type
        /// Can be AUTH, SALE, REFUND, VOID, CAPTURE or FORCED SALE
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Result of the transaction.
        /// Can be APPROVED, DECLINED or ERROR
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        /// <summary>
        /// Result code associated with transaction result
        /// </summary>
        [JsonProperty("result-code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// Invoice number passed in as part of the transaction
        /// </summary>
        [JsonProperty("invoice")]
        public string Invoice { get; set; }

        /// <summary>
        /// Billing address
        /// </summary>
        [JsonProperty("billing")]
        public Address? Billing { get; set; }

        /// <summary>
        /// Shipping address
        /// </summary>
        [JsonProperty("shipping")]
        public Address? Shipping { get; set; }

        [JsonProperty("billing-is-shipping")]
        public bool BillingIsShipping { get; set; }

        /// <summary>
        /// Masked Card data from the request
        /// </summary>
        [JsonProperty("card")]
        public string CardMasked { get; set; }

        [JsonProperty("card-type")]
        public string CardType { get; set; }

        /// <summary>
        /// Last four digits of the card number the token represents
        /// </summary>
        [JsonProperty("last-four")]
        public string LastFour { get; set; }

        /// <summary>
        /// MMYY
        /// </summary>
        [JsonProperty("exp-date")]
        public string ExpDate { get; set; }

        [JsonProperty("authorization-code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// Displayable message about transaction result
        /// </summary>
        [JsonProperty("display-message")]
        public string DisplayMessage { get; set; }

        //[JsonProperty("original-amount")]
        //public decimal? OriginalAmount { get; set; }

        /// <summary>
        /// Method Used to enter transaction, Swipe/Manual.
        /// </summary>
        //[JsonProperty("entry-method")]
        //public string EntryMethod { get; set; }

        //[JsonProperty("batch-string-id")]
        //public string BatchStringId { get; set; }

        //[JsonProperty("order-id")]
        //public string OrderId { get; set; }

        //[JsonProperty("tip-adjusted")]
        //public int TipAdjusted { get; set; }

        //[JsonProperty("voided")]
        //public bool IsVoided { get; set; }

        //[JsonProperty("voided-auth")]
        //public bool IsVoidedAuth { get; set; }

        //[JsonProperty("pending")]
        //public bool IsPending { get; set; }

        //[JsonProperty("settled")]
        //public bool IsSettled { get; set; }

        //[JsonProperty("status")]
        //public string Status { get; set; }

        //[JsonProperty("base-amount")]
        //public decimal BaseAmount { get; set; }

        //[JsonProperty("merchant-id")]
        //public string MerchantId { get; set; }

        //[JsonProperty("terminal-id")]
        //public int? TerminalId { get; set; }

        //[JsonProperty("customer-id")]
        //public string CustomerId { get; set; }
    }
}
