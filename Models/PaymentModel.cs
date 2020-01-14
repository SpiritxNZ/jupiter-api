using System;
namespace jupiterCore.Models
{
    public class PaymentModel
    {
        public int? PaymentId { get; set; }
        public int CardId { get; set; }
        public int Success { get; set; }

        public string TxnId { get; set; }
        public string ClientInfo { get; set; }
        public string ResponseText { get; set; }
        public string AmountSettlemen { get; set; }
        public string cardName { get; set; }
        public string cardNumber { get; set; }
        public string dateExpiry { get; set; }
        public string cardHolderName { get; set; }
        public string currencySettlement { get; set; }
        public string currencyInput { get; set; }
        public string txnMac { get; set; }
        public string url { get; set; }

    }
}
