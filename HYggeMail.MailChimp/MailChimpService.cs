using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MailChimp.Net;
using MailChimp.Net.Models;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;

namespace HyggeMail.MailChimp
{
    public class MailChimpService
    {
        public static string AddOrUpdateListMember(string subscriberEmail, string listId)
        {
            var dataCenter = ConfigurationManager.AppSettings["DataCenter"];
            var apiKey = ConfigurationManager.AppSettings["MailChimpApiKey"];

            var sampleListMember = JsonConvert.SerializeObject(
                new
                {
                    email_address = subscriberEmail,
                    status_if_new = "subscribed"
                });

            var hashedEmailAddress = string.IsNullOrEmpty(subscriberEmail) ? "" : CalculateMD5Hash(subscriberEmail.ToLower());
            var uri = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members/{2}", dataCenter, listId, hashedEmailAddress);
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json");
                    webClient.Headers.Add("Authorization", "apikey " + apiKey);

                    webClient.UploadString(uri, "PUT", sampleListMember);

                    return "You have joined the HyggeMail programme Successfully.";
                }
            }
            catch (WebException we)
            {
                return we.StackTrace;
                //using (var sr = new StreamReader(we.Response.GetResponseStream()))
                //{
                //    return sr.ReadToEnd();
                //}
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input.
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string.
            var sb = new StringBuilder();
            foreach (var @byte in hash)
            {
                sb.Append(@byte.ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
