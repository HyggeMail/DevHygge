using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace HyggeMail.Attributes
{
    public class ApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string errorid = LogExceptionToDatabase(context.Exception);//log exception in database
            base.OnException(context);
            var ex = context.Exception as HttpResponseException;
            if (ex != null && ex.Response.StatusCode != HttpStatusCode.Unauthorized)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.OK);
                context.Response.Content = new StringContent(ex.ToString());
            }

        }
        /// <summary>
        /// log exception to database
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private String LogExceptionToDatabase(Exception ex)
        {
            try
            {
                //Log exception in database
                IErrorLogManager errorlog = new ErrorLogManager();
                var result = errorlog.LogExceptionToDatabase(ex);
            }
            catch (Exception)
            {

            }

            return null;
        }
    }
}