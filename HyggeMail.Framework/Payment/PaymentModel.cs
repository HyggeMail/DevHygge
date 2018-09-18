using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
namespace HyggeMail.Framework.Payment
{
    public class PayPalModel
    {
        public string cmd { get; set; }
        public string business { get; set; }
        public string no_shipping { get; set; }
        public string @return { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public string item_name { get; set; }
        public string amount { get; set; }
        public string actionURL { get; set; }

        // For Express CheckOut
        public string USER { get; set; }
        public string PWD { get; set; }
        public string SIGNATURE { get; set; }
        public string METHOD { get; set; }

        public string VERSION { get; set; }
        public string AMT { get; set; }
        public string cancelUrl { get; set; }
        public string returnUrl { get; set; }

        public string ItemName { get; set; }
        public int Qty { get; set; }
        public double UnitPrice { get; set; }

        public string apiURL { get; set; }

        public PayPalModel(bool useSandbox)
        {
            USER = WebConfigurationManager.AppSettings["USER"];
            PWD = WebConfigurationManager.AppSettings["PWD"];
            SIGNATURE = WebConfigurationManager.AppSettings["SIGNATURE"];
            METHOD = "SetExpressCheckout";
            VERSION = "88";
            cancelUrl = WebConfigurationManager.AppSettings["cancelUrl"];
            returnUrl = WebConfigurationManager.AppSettings["pay_returnUrl"];
            apiURL = string.Format("https://www.{0}paypal.com/cgi-bin/webscr", useSandbox ? "sandbox." : "");
        }
    }

    public class SkrillModel
    {
        public string payTo { get; set; }
        public string return_url { get; set; }
        public string cancel_url { get; set; }
        public string status_url { get; set; }
        public string amount { get; set; }
        public string secret { get; set; }
        public string transactionId { get; set; }
        public string detail1_description { get; set; }

        public SkrillModel()
        {
            //WebConfigurationManager.AppSettings["IsSandbox"]
            payTo = WebConfigurationManager.AppSettings["skrill_email"];
            return_url = WebConfigurationManager.AppSettings["returnUrl"];
            cancel_url = WebConfigurationManager.AppSettings["cancelUrl"];
            status_url = WebConfigurationManager.AppSettings["skrill_status_url"];
            secret = WebConfigurationManager.AppSettings["skrill_secret"];

        }
    }

    public class PayzaModel
    {
        public string ap_merchant { get; set; }
        public string ap_purchasetype { get; set; }
        public string ap_itemname { get; set; }
        public string ap_amount { get; set; }
        public string ap_currency { get; set; }
        public string ap_returnurl { get; set; }
        public string ap_cancelurl { get; set; }
        public string ap_alerturl { get; set; }
        public string apc_1 { get; set; }
        public string ap_url { get; set; }

        public PayzaModel()
        {
            ap_merchant = WebConfigurationManager.AppSettings["ap_merchant"];
            ap_returnurl = WebConfigurationManager.AppSettings["returnUrl"];
            ap_cancelurl = WebConfigurationManager.AppSettings["cancelUrl"];
            ap_alerturl = WebConfigurationManager.AppSettings["ap_alerturl"];
            ap_currency = "USD";
            ap_purchasetype = "item";

            var useSandbox = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsSandbox"]);
            ap_url = useSandbox ? "https://sandbox.Payza.com/sandbox/payprocess.aspx" : "https://secure.payza.com/checkout";


        }
    }
}
