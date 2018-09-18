using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LotteryApp.Framework.Api.Helpers;
using System.Net;
using System.Security.Principal;
using LotteryApp.Data.Service;

namespace LotteryApp.Framework.Api.Security
{
    public class TokenInspector : DelegatingHandler
    {

        private readonly IUserService _userService;


        public TokenInspector(IUserService userService)
        {
            if (userService == null)
                throw new ArgumentNullException("factory");
            _userService = userService;
        }


      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            const string TOKEN_NAME = "X-Token";

            if (request.Headers.Contains(TOKEN_NAME))
            {
                string encryptedToken = request.Headers.GetValues(TOKEN_NAME).First();
                try
                {
                    Token token = Token.Decrypt(encryptedToken);
                    if (!_userService.VerifyUserByToken(token.UserId, token.UserToken))
                    {
                        HttpResponseMessage reply = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            Content = new JsonContent(Messages.INVALID_IDENTITY, Status.failed)
                        };
                        return Task.FromResult(reply);
                    }

                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(token.UserId), null);
                }
                catch (Exception ex)
                {
                    HttpResponseMessage reply = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new JsonContent(Messages.INVALID_TOKEN, Status.failed)
                    };
                    return Task.FromResult(reply);
                }
            }
            else
            {
                HttpResponseMessage reply = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new JsonContent(Messages.MISSING_TOKEN, Status.failed)
                };
                return Task.FromResult(reply);
            }

            return base.SendAsync(request, cancellationToken);
        }

    }
}
