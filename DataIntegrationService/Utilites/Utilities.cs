using LoggingLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace DataIntegrationService.ApiUtility
{
    public class ApiResponse<T>
    {
        public T Output { get; set; }
        public string Message { get; set; }
    }
    public class ApiRequest
    {
        public string Address { get; set; }
        public dynamic Data { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ApiService
    {
        private static HttpClient CreateHttpClient(string userName, string password)
        {
            try
            {
                HttpClient client = new HttpClient();
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, password));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.MaxResponseContentBufferSize = 1000000000;
                TimeSpan timeSpan = TimeSpan.FromHours(4);
                client.Timeout = timeSpan;
                return client;
            }
            catch(Exception ex)
            {
                LoggingManager.LogException(typeof(ApiService), ex);
                throw;
            }
        }
        public static async Task<ApiResponse<T>> InvokeApirequest<T>(ApiRequest apiRequest)
        {
            ApiResponse<T> result = new ApiResponse<T>();
            LoggingManager.LogMessage(typeof(ApiService), $"Invoking {apiRequest.Address} with parameter {apiRequest.Data} begins");

            try
            {
                var jsonResult = string.Empty;
                var client = CreateHttpClient(apiRequest.UserName, apiRequest.Password);
                HttpContent apiContent = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                var response = await client.PostAsync(apiRequest.Address, apiContent);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        jsonResult = await response.Content.ReadAsStringAsync();
                        //LoggingManager.LogMessage(typeof(ApiService),$" The response of the API {apiRequest.Address} is {Environment.NewLine} {jsonResult}");
                        var output = JsonConvert.DeserializeObject<T>(jsonResult);
                        result.Output = output;
                        result.Message = "success";
                        LoggingManager.LogMessage(typeof(ApiService),$" Successfully received the data of the api {apiRequest.Address} with pram {apiRequest.Data}");

                    }
                    else
                    {
                        result.Message = "failed";
                    }

                }
                else
                {
                    result.Message = "Unhandled exception occured";
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ApiService), ex);

                throw ex;
            }
            finally
            {
                LoggingManager.LogMessage(typeof(ApiService), $"Invoking {apiRequest.Address} with parameter {apiRequest.Data} ends");
            }

        }
    }
}