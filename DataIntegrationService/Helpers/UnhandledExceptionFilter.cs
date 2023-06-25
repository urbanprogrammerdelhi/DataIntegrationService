using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace DataIntegrationService.Helpers
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
           // UtilidexLogger.LogException(actionExecutedContext?.Request?.GetType(), actionExecutedContext.Exception, $"Requested Url : {actionExecutedContext?.Request?.RequestUri?.AbsoluteUri}");

            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            actionExecutedContext.Response.ReasonPhrase = "Internal error :-(";
        }
    }
}