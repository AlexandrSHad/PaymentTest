using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PaymentTest.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Globalization;

namespace PaymentTest.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Test()
        {
            return Ok("test");
        }

        [HttpGet("pay")]
        public async Task<IActionResult> Pay() //[FromBody] modelType model
        {
            //TODO: Implement Realistic Implementation
            //await Task.Yield();
            //return Ok(HttpContext.Request.Form.Count);

            var crService = new CryptoService();
            var dsHelper = new DataStringHelper();

            var payment = new Payment
            {
                MerchantId = "1755156",
                TerminalId = "E7883166",
                PurchaseTime = "181107091010",
                OrderId = "VHS1036004",
                Currency = "980",
                TotalAmount = "100",
                PurchaseDesc = "Test purchase",
                SessionData = "584sds565hgj76GGjh6756248",
            };

            var dataString = dsHelper.GetDataStringForSign(
                merchantId: payment.MerchantId,
                terminalId: payment.TerminalId,
                purchaseTime: payment.PurchaseTime,
                orderId: payment.OrderId,
                currency: payment.Currency,
                totalAmount: payment.TotalAmount,
                sessionData: payment.SessionData
            );

            //payment.Signature = Base64UrlTextEncoder.Encode(crService.GetSign(dataString));
            payment.Signature = crService.GetSign(dataString);

            var contentResult = new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                //Content = "<html><body>Welcome</body></html>"
            };

            StringBuilder s = new StringBuilder();
            s.AppendFormat("<html>");
            //s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<body>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", "https://ecg.test.upc.ua/go/enter");
            s.AppendFormat("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "Version", payment.Version);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "MerchantID", payment.MerchantId);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "TerminalID", payment.TerminalId);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "PurchaseTime", payment.PurchaseTime);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "OrderID", payment.OrderId);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "Currency", payment.Currency);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "TotalAmount", payment.TotalAmount);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "SD", payment.SessionData);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "locale", "uk");
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "PurchaseDesc", payment.PurchaseDesc);
            s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", "Signature", payment.Signature);
            s.AppendFormat("<input type='submit' value='Post' />");
            s.AppendFormat("</form></body></html>");

            contentResult.Content = s.ToString();

            return contentResult;

            //var client = new HttpClient();
            //client.BaseAddress = new Uri("https://ecg.test.upc.ua");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var responce = await client.PostAsync("go/enter", new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json"));

            // IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
            //     { new KeyValuePair<string, string>("MerchantID", payment.MerchantId) },
            //     { new KeyValuePair<string, string>("TerminalId", payment.TerminalId) },
            //     { new KeyValuePair<string, string>("PurchaseTime", payment.PurchaseTime) },
            //     { new KeyValuePair<string, string>("OrderId", payment.OrderId) },
            //     { new KeyValuePair<string, string>("Currency", payment.Currency) },
            //     { new KeyValuePair<string, string>("TotalAmount", payment.TotalAmount) },
            //     { new KeyValuePair<string, string>("SD", payment.SessionData) },
            //     { new KeyValuePair<string, string>("Signature", payment.Signature) },
            // };

            // var response = await client.PostAsync("https://ecg.test.upc.ua/go/enter", new FormUrlEncodedContent(nameValueCollection));
            // var str = await response.Content.ReadAsStringAsync();

            //return Ok(str);
        }
    
        [HttpPost("notify")]
        public IActionResult Notify()
        {
            return Ok();
        }

        [HttpPost("success")]
        public IActionResult Success([FromForm] Payment payment)
        {
            return Redirect( GetUrl("payment/success") );
        }

        [HttpPost("fail")]
        public IActionResult Fail([FromForm] Payment payment)
        {
            return Redirect( GetUrl("payment/fail") );
        }

        private string GetUrl(string path)
        {
            var request = HttpContext.Request;
            var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Host;
            builder.Port = request.Host.Port ?? -1;
            builder.Path = path;
            //builder.Query = request.QueryString.ToUriComponent();

            return builder.Uri.ToString();
        }
    }

    public class Payment
    {
        public string Currency { get; set; }
        public string OrderId { get; set; }
        public string MerchantId { get; set; }
        public string PurchaseDesc { get; set; }
        public string PurchaseTime { get; set; }
        public string SessionData { get; set; }
        public string Signature { get; set; }
        public string TerminalId { get; set; }
        public string TotalAmount { get; set; }
        public string Version { get; set; } = "1";
    }
}