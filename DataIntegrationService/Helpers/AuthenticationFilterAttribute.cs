using DataIntegrationService.Models;
using System;
using System.Net;
using System.Text;
using System.Web.Http.Filters;

namespace DataIntegrationService.Helpers
{
    public class AuthenticationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            // Gets header parameters  
            string authenticationString = actionContext.Request.Headers.Authorization.Parameter ?? null;
            string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));
            
            // Gets username and password  
            string userName = originalString.Split(':')[0];
            string password = originalString.Split(':')[1];

            // Validate username and password  
            if (ConnectionManager.ValidateUser(userName, password))
            {
                return;
            }

            BaseResponse response = new BaseResponse();
            actionContext.Response = response.CreateResponse(actionContext.Request, HttpStatusCode.Unauthorized, "Authentication failed.");
        }
    }
}