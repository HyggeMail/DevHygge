using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HyggeMail.Attributes;
using HyggeMail.BLL.Models;

namespace HyggeMail.Areas.API
{
    [HandelException, CheckAuthorization, ValidateModel, ApiExceptionAttribute]
    public class BaseAPIController : ApiController
    {
        public ApiUserModel LOGGED_IN_USER { get; set; }     
        public string Token { get; set; }

    }
}