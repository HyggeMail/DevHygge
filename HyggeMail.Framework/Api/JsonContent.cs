using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HyggeMail.BLL.Common;
using HyggeMail.Framework.Api.Helpers;
using HyggeMail.BLL.Models;
namespace HyggeMail.Framework.Api
{
    public class JsonContent : HttpContent
    {
        private readonly JToken _value;
        public JsonContent(object Data = null)
        {
            Response st = new Response { result = Data };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _value = JObject.Parse(JsonConvert.SerializeObject(st, jsonSerializerSettings));
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        public JsonContent(ActionOutput Data = null)
        {
            Response st = new Response { Message = Data.Message, Status = Utilities.GetDescription(typeof(ActionStatus), Data.Status), result = Data.Results };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _value = JObject.Parse(JsonConvert.SerializeObject(st, jsonSerializerSettings));
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        public JsonContent(ActionOutput<object> Data = null)
        {
            Response st = new Response { Message = Data.Message, Status = Utilities.GetDescription(typeof(ActionStatus), Data.Status), result = Data };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _value = JObject.Parse(JsonConvert.SerializeObject(st, jsonSerializerSettings));
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        public JsonContent(string message, ActionStatus status, object data = null, string token = null, char? verified = null)
        {
            Response st = new Response { Message = message, Status = Utilities.GetDescription(typeof(ActionStatus), status), result = data };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _value = JObject.Parse(JsonConvert.SerializeObject(st, jsonSerializerSettings));
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }


        protected override Task SerializeToStreamAsync(Stream stream,
            TransportContext context)
        {
            var jw = new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }

}
