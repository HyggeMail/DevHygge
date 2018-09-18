using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace HyggeMail.Framework.Payment
{
    public class PaypalAPI
    {

        public static NameValueCollection SetExpressCheckout(PayPalModel model)
        {
            string formContent = "&VERSION=88" +
                    "&AMT=" + model.AMT +
                "&L_PAYMENTREQUEST_0_NAME0=" + model.ItemName +
                //"&L_PAYMENTREQUEST_0_QTY0=" + model.Qty +
                //"&L_PAYMENTREQUEST_0_AMT0=" + model.UnitPrice +
                //"&PAYMENTREQUEST_0_ITEMAMT=" + model.AMT +
                    "&returnUrl=" + model.returnUrl +
                    "&cancelUrl=" + model.cancelUrl +
                    "&METHOD=SetExpressCheckout";

            var responseFromServer = GetResponse(formContent);
            return responseFromServer;
        }



        public static NameValueCollection GetExpressCheckoutDetails(string token)
        {
            string formContent = "&VERSION=93" +
                      "&TOKEN=" + token +
                      "&METHOD=GetExpressCheckoutDetails";

            var responseFromServer = GetResponse(formContent);

            return responseFromServer;
        }


        public static NameValueCollection DoExpressCheckoutPayment(string amount, string token, string payerId)
        {
            string formContent = "&VERSION=93" +
                      "&TOKEN=" + token +
                      "&PAYERID=" + payerId +
                      "&PAYMENTREQUEST_0_PAYMENTACTION=SALE" +
                        "&PAYMENTREQUEST_0_AMT=" + amount +
                    "&PAYMENTREQUEST_0_CURRENCYCODE=USD" +
                      "&METHOD=DoExpressCheckoutPayment";

            var responseFromServer = GetResponse(formContent);

            return responseFromServer;
        }


        private static string GetCredentials()
        {
            return "USER=" + WebConfigurationManager.AppSettings["USER"] +
                       "&PWD=" + WebConfigurationManager.AppSettings["PWD"] +
                       "&SIGNATURE=" + WebConfigurationManager.AppSettings["SIGNATURE"];
        }

        private static NameValueCollection GetResponse(string formContent)
        {
            var useSandbox = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsSandbox"]);
            var url = string.Format("https://api-3t.{0}paypal.com/nvp", useSandbox ? "sandbox." : string.Empty);
            return APICaller.GetResponse(GetCredentials() + formContent, url);
        }
    }

    public class SkrillAPI
    {

        public static NameValueCollection GetSessionId(SkrillModel model)
        {
            string formContent =
                    "pay_to_email=" + model.payTo +
                    "&transaction_id=" + model.transactionId +
                    "&prepare_only=1" +
                    "&language=EN" +
                    "&amount=" + model.amount +
                    "&detail1_description=" + model.detail1_description +
                    "&return_url=" + model.return_url +
                    "&cancel_url=" + model.cancel_url +
                    "&status_url=" + model.status_url +
                    "&currency=USD";

            var responseFromServer = APICaller.GetResponse(formContent, "https://www.moneybookers.com/app/payment.pl");
            return responseFromServer;
        }

    }

    public class PayzaAPI
    {
        public static NameValueCollection PostToken(string token)
        {
            string formContent = "token=" + token;


            var responseFromServer = APICaller.GetResponse(formContent, "https://sandbox.Payza.com/sandbox/IPN2.ashx");

            return responseFromServer;
        }
    }

    public static class APICaller
    {
        public static NameValueCollection GetResponse(string content, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            string formContent = content;

            byte[] byteArray = Encoding.UTF8.GetBytes(formContent);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = HttpUtility.UrlDecode(reader.ReadToEnd());

            reader.Close();
            dataStream.Close();
            response.Close();

            return HttpUtility.ParseQueryString(responseFromServer);
        }
    }
}
