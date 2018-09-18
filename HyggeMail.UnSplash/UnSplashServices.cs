using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.UnSplash
{
    public class UnSplashServices
    {
        public string GetPics(int page, string keyword)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            int recordPerPage = 10;
            string clientId = ConfigurationManager.AppSettings["UnSplashClientId"];

            var client = new RestClient("https://api.unsplash.com/");
            var request = new RestRequest("search/photos", Method.GET);
            request.AddHeader("cache-control", "no-cache");
            // request.AddHeader("Access-Control-Allow-Origin", "*");
            request.AddParameter("page", page, ParameterType.QueryString);
            request.AddParameter("per_page", recordPerPage, ParameterType.QueryString);
            request.AddParameter("query", keyword, ParameterType.QueryString);
            request.AddParameter("orientation", "landscape", ParameterType.QueryString);
            request.AddParameter("client_id", clientId, ParameterType.QueryString);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                return response.Content;
            }
            else
                return null;

        }
    }
}
