using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearentScheduler.ClearentQuestAPI
{
    [Serializable]
    public class Payload
    {
        [JsonProperty("payloadType")]
        public string PayloadType { get; set; }

        [JsonProperty("transactions")]
        public TransactionArray Transactions { get; set; }

        [JsonProperty("ach-transactions")]
        public ACHTransactionArray AchTransactions { get; set; }

        [JsonProperty("error")]
        public Error? Error { get; set; }
    }

    public struct Error
    {
        [JsonProperty("result-code")]
        public string ResultCode { get; set; }

        [JsonProperty("error-message")]
        public string Message { get; set; }

        [JsonProperty("time-stamp")]
        public DateTime TimeStamp { get; set; }
    }

    //[JsonArray]
    public class TransactionArray
    {
        [JsonProperty("transaction")]
        Transaction[] Transaction { get; set; }

        public Transaction this[int index] => Transaction[index];
        public int Length
        {
            get
            {
                return Transaction.Length;
            }
        }        
    }

    public class ACHTransactionArray
    {
        [JsonProperty("ach-transaction")]
        ACHTransaction[] AchTransaction { get; set; }

        public ACHTransaction this[int index] => AchTransaction[index];

        public int Length
        {
            get
            {
                return AchTransaction.Length;
            }
        }


    }
}
