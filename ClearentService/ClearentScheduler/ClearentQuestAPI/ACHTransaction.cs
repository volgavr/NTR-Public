using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearentScheduler.ClearentQuestAPI
{
    [Serializable]
    public class ACHTransaction
    {
        /// <summary>
        /// Unique id of the ach transaction (optional)
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Amount of transaction
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// transaction type
        /// Can be Debit or Credit
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Returned from GET and can be used with search filter.
        /// Can be Pending, Settling, Settled or Returned
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Checking or Savings
        /// </summary>
        [JsonProperty("account-type")]
        public string AccountType { get; set; }

        /// <summary>
        /// Account number masked to only show last four
        /// </summary>
        [JsonProperty("account-number")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 9 digits
        /// </summary>
        [JsonProperty("routing-number")]
        public string RoutingNumber { get; set; }

        [JsonProperty("check-number")]
        public string CheckNumber { get; set; }

        /// <summary>
        /// The name of the account holder.
        /// </summary>
        [JsonProperty("individual-name")]
        public string IndividualName { get; set; }

        [JsonProperty("software-type")]
        public string SoftwareType { get; set; }

        ///TEL = telephone initiated entries, WEB = internet initiated entries
        [JsonProperty("standard-entry-class-code")]
        public string EntryClassCode { get; set; }

        /// <summary>
        ///Set to true when validating account information.
        /// </summary>
        [JsonProperty("validate-account")]
        public bool ValidateAccount { get; set; }

        /// <summary>
        /// Displayable message about transaction result
        /// </summary>
        [JsonProperty("display-message")]
        public string DisplayMessage { get; set; }

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

        //Is the billing address the same as the shipping address?
        [JsonProperty("billing-is-shipping")]
        public bool BillingIsShipping { get; set; }

        /// <summary>
        /// Date of settled transaction
        /// </summary>
        [JsonProperty("settled-date")]
        public DateTime? SettledDate { get; set; }

        /// <summary>
        /// Last date that the status of the transaction changed
        /// </summary>
        [JsonProperty("status-change-date")]
        public DateTime? StatusChangeDate { get; set; }

        /// <summary>
        /// transaction id from the ach provider
        /// </summary>
        [JsonProperty("provider-transaction-id")]
        public string ProviderTransactionId { get; set; }
    }
}
