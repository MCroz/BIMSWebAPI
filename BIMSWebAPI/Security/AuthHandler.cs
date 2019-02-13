using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BIMSWebAPI.Security
{
    public class AuthHandler : DelegatingHandler
    {
        User curUser = null;

        //Method to validate credentials from Authorization
        //header value
        private bool ValidateCredentials(AuthenticationHeaderValue authenticationHeaderVal)
        {
            try
            {
                if (authenticationHeaderVal != null
                    && !String.IsNullOrEmpty(authenticationHeaderVal.Parameter))
                {
                    string decodedCredentials = Encoding.ASCII.GetString(Convert.FromBase64String(authenticationHeaderVal.Parameter));
                    int userId = Convert.ToInt32(decodedCredentials);
                    //now decodedCredentials[0] will contain
                    //username and decodedCredentials[1] will
                    //contain password.
                    using (var context = new BimsContext())
                    {
                        var user = context.Users.Find(userId);
                        if (user == null)
                            return false;

                        //request authenticated
                        curUser = user;
                        return true;
                    }


                    
                }
                return false;//request not authenticated.
            }
            catch
            {
                return false;
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //if the credentials are validated,
            //set CurrentPrincipal and Current.User
            if (ValidateCredentials(request.Headers.Authorization))
            {
                Thread.CurrentPrincipal = new BIMSAPIPrincipal(curUser);
                HttpContext.Current.User = new BIMSAPIPrincipal(curUser);
            }
            //Execute base.SendAsync to execute default
            //actions and once it is completed,
            //capture the response object and add
            //WWW-Authenticate header if the request
            //was marked as unauthorized.

            //Allow the request to process further down the pipeline
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized
                && !response.Headers.Contains("WwwAuthenticate"))
            {
                response.Headers.Add("WwwAuthenticate", "Basic");
            }

            return response;
        }
    }
}