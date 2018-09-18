using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Common
{
    public static class Config
    {
        public static bool LogExceptionInDatabase
        {
            get { return Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["LogExceptionInDB"].ToString()); }
        }
        public static string Link
        {
            get { return ConfigurationManager.AppSettings["Link"].ToString(); }
        }

        public static string Email
        {
            get { return ConfigurationManager.AppSettings["eEmail"].ToString(); }
        }

        public static string Password
        {
            get { return ConfigurationManager.AppSettings["ePassword"].ToString(); }
        }
        public const string EncryptionKey = "&$&%^$&^%$^%hjgkjhgkjhg%$%*&&(*^*76987";

        public static string smtp
        {
            get { return ConfigurationManager.AppSettings["smtp"].ToString(); }
        }

        public static string AppName
        {
            get { return ConfigurationManager.AppSettings["appName"].ToString(); }
        }

        public static string HyggeMailCodeSecretKey { get { return "J3H7F9J6FG"; } }

        public static string BrowserKey { get { return System.Configuration.ConfigurationManager.AppSettings["BrowserKey"].ToString(); } }
        
        public static string GooglePlaceKey
        {
            get { return ConfigurationManager.AppSettings["GooglePlaceKey"].ToString(); }
        }

        public static string DemoPostCardEmail
        {
            get { return ConfigurationManager.AppSettings["DemoPostCardEmail"].ToString(); }
        }
    }

    public static class PayPalConfiguration
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;

        static PayPalConfiguration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        public static Dictionary<string, string> GetConfig()
        {
            var cc = PayPal.Api.ConfigManager.Instance.GetProperties();
            cc.Add("content-type", "application/json");
            return cc;
        }

        private static string GetAccessToken()
        {               
            string accessToken = new OAuthTokenCredential
        (ClientId, ClientSecret, GetConfig()).GetAccessToken();

            return accessToken;
        }

        public static APIContext GetAPIContext()
        {
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
        public static Dictionary<string, string> GetAcctAndConfig()
        {
            Dictionary<string, string> configMap = new Dictionary<string, string>();
            configMap = GetConfig();
            configMap.Add("account1.apiUsername", "kumar.life74-facilitator_api1.gmail.com");
            configMap.Add("account1.apiPassword", "U6YL7HV5QQ2SZV5U");
            configMap.Add("account1.apiSignature", "AFcWxV21C7fd0v3bYYYRCpSSRl31AOzxmb0WHisTP06urC.JtPAAFhty");
            configMap.Add("account1.applicationId", "APP-80W284485P519543T");
            configMap.Add("sandboxEmailAddress", "kumar.life74-facilitator@gmail.com");
            return configMap;
        }

       
    }
}
