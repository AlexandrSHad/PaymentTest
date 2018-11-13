namespace PaymentTest.Services
{
    public class DataStringHelper
    {
        public string GetDataStringForSign(string merchantId, string terminalId, string purchaseTime, string orderId, string currency, string totalAmount, string sessionData = null, string delay = null, string altCurrency = null, string altTotalAmount = null)
        {
            // "MerchantId;TerminalId;PurchaseTime;OrderId,Delay;CurrencyId,AltCurrencyId;Amount,AltAmount;SessionData(SD);"

            string orderString = GetOrderString(orderId, delay);
            string amountString = GetAmountString(totalAmount, altTotalAmount);
            string currencyString = GetCurrencyString(currency, altCurrency);

            var result = string.Join(';', new string[]
            {
                merchantId,
                terminalId,
                purchaseTime,
                orderString,
                currencyString,
                amountString,
                sessionData
            }) + ";";
            
            return result;
        }

        public string GetDataStringForVerify(string merchantId, string terminalId, string purchaseTime, string orderId, string xId, string currency, string totalAmount, string tranCode, string approvalCode, string sessionData = null, string delay = null, string altCurrency = null, string altTotalAmount = null)
        {
            // "MerchantId;TerminalId;PurchaseTime;OrderId,Delay;Xid;CurrencyId,AltCurrencyId;Amount,AltAmount;SessionData;TranCode;ApprovalCode;"

            string orderString = GetOrderString(orderId, delay);
            string amountString = GetAmountString(totalAmount, altTotalAmount);
            string currencyString = GetCurrencyString(currency, altCurrency);

            var result = string.Join(';', new string[]
            {
                merchantId,
                terminalId,
                purchaseTime,
                orderString,
                xId,
                currencyString,
                amountString,
                sessionData,
                tranCode,
                approvalCode
            }) + ";";

            return result;
        }

        private string GetOrderString(string orderId, string delay)
        {
            return delay == null ? orderId : string.Join(',', orderId, delay);
        }

        private string GetAmountString(string totalAmount, string altTotalAmount)
        {
            return altTotalAmount == null ? totalAmount : string.Join(',', totalAmount, altTotalAmount);
        }

        private string GetCurrencyString(string currency, string altCurrency)
        {
            return altCurrency == null ? currency : string.Join(',', currency, altCurrency);
        }
    }
}