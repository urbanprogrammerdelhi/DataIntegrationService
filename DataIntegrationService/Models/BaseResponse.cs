using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace DataIntegrationService.Models
{
    public class BaseResponse
    {
        protected HttpResponseMessage _httpResponseMessage;

        public HttpResponseMessage OK(HttpRequestMessage requestMessage, object value)
        {
            _httpResponseMessage = requestMessage.CreateResponse(HttpStatusCode.OK);
            _httpResponseMessage.Content = new StringContent(value != null ? JsonConvert.SerializeObject(value) : string.Empty);
            return _httpResponseMessage;
        }

        public HttpResponseMessage OK(HttpRequestMessage requestMessage, System.IO.Stream value)
        {
            _httpResponseMessage = requestMessage.CreateResponse(HttpStatusCode.OK);
            _httpResponseMessage.Content = new StreamContent(value);
            return _httpResponseMessage;
        }

        public HttpResponseMessage OK(HttpRequestMessage requestMessage)
        {
            return OK(requestMessage, null);
        }

        public HttpResponseMessage Error(HttpRequestMessage requestMessage, string message)
        {
            _httpResponseMessage = requestMessage.CreateResponse(HttpStatusCode.BadRequest);
            _httpResponseMessage.Content = new StringContent(message);
            return _httpResponseMessage;
        }

        public HttpResponseMessage CreateResponse(HttpRequestMessage requestMessage, HttpStatusCode httpStatusCode, object value)
        {
            _httpResponseMessage = requestMessage.CreateResponse(httpStatusCode);
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                _httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(value));
            return _httpResponseMessage;
        }
    }
}