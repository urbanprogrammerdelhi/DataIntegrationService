using DataIntegrationService.ApiUtility;
using DataIntegrationService.Entities;
using DataIntegrationService.Helpers;
using DataIntegrationService.Models;
using LoggingLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataIntegrationService
{
    public sealed class SchedulerService
    {
        static readonly SchedulerService _schedulerService = new SchedulerService();
        static readonly object lockObject = new object();        
        readonly string dbConnectionString;
        readonly int hoursToPoll = 0;
        readonly DateTime timeOfDay;
        static Type _type;
        bool isServiceOn = false;
        string blueTreeURL = string.Empty;
        string blueTreeESIURL = string.Empty;
        const string DateTimeformatForComparision = "yyyy-MM-dd HH:mm:00";
      
        static readonly string username = ConfigurationManager.AppSettings["username"].ToString();
        static readonly string password = ConfigurationManager.AppSettings["password"].ToString();
        DateTime autoFailOverTime;
      
        private SchedulerService()
        {
            dbConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            hoursToPoll = int.Parse(ConfigurationManager.AppSettings["HoursToPoll"].ToString());
            timeOfDay = DateTime.ParseExact(ConfigurationManager.AppSettings["TimeofDay"].ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
            _type = typeof(SchedulerService);
            isServiceOn = bool.Parse(ConfigurationManager.AppSettings["IsServiceOn"].ToString());
            blueTreeURL = ConfigurationManager.AppSettings["BlueTreeURL"].ToString();
            blueTreeESIURL = ConfigurationManager.AppSettings["blueTreeESIURL"].ToString();
            autoFailOverTime = DateTime.ParseExact(ConfigurationManager.AppSettings["AutoFailOverTime"].ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static SchedulerService Instance { get { return _schedulerService; } }

        public async void Start()
        {
            try
            {
                LoggingManager.LogMessage(typeof(ConnectionManager), "Service Started");
                DateTime currentDateTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,timeOfDay.Hour,timeOfDay.Minute,timeOfDay.Second);
                DateTime failOverDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, autoFailOverTime.Hour, autoFailOverTime.Minute, autoFailOverTime.Second);
                while (true)
                {
                    try
                    {
                        LoggingManager.LogMessage(typeof(ConnectionManager), $"Time is : {DateTime.Now.ToString(GlobalConstants.EndTimeformat)}");
                        if (DateTime.Now >= currentDateTime && isServiceOn)
                        {
                            try
                            {
                                LoggingManager.LogMessage(typeof(ConnectionManager),"Extracting records from API");
                                //Fetch the data from the Blue Tree API and insert into ERP database
                                //GetDatafromAPI(DateTime.Now.ToString("yyyy-MM-dd"));
                                //GetDatafromESIAPI(DateTime.Now.ToString("yyyy-MM-dd"));           
                                await ProcessData(currentDateTime.ToString(GlobalConstants.StartTimeformat), currentDateTime.ToString(GlobalConstants.EndTimeformat));

                            }
                            catch (Exception e)
                            {
                                LoggingManager.LogException(_type, e);
                            }
							
							DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);
                            currentDateTime = newDateTime.AddDays(1);
							
                            //currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);
                        }
                        else if (DateTime.Now >= failOverDateTime && isServiceOn)
                        {
                            try
                            {
                                LoggingManager.LogMessage(typeof(ConnectionManager), "Extracting records from API for AutoFailOver");
                                string dateToCheck = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                                var dateToCheckTime = DateTime.Now.AddDays(-1);
                                if (!ConnectionManager.CheckDataforDate(DateTime.Now.AddDays(-1)))
                                {
                                    //GetDatafromAPI(dateToCheck);
                                    //GetDatafromESIAPI(dateToCheck);
                                    await ProcessData(dateToCheckTime.ToString(GlobalConstants.StartTimeformat), dateToCheckTime.ToString(GlobalConstants.EndTimeformat));

                                }
                            }
                            catch (Exception e)
                            {
                                LoggingManager.LogException(_type, e);
                            }
							
							DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, autoFailOverTime.Hour, autoFailOverTime.Minute, autoFailOverTime.Second);
                            failOverDateTime = newDateTime.AddDays(1);
							
                            //failOverDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, autoFailOverTime.Hour, autoFailOverTime.Minute, autoFailOverTime.Second);
                        }                       
                        else
                        {
                            int timeDifference = hoursToPoll * 60 * 1000; //Minutes changed to milliseconds
                            if (timeDifference > 0)
                            {
                                Thread.Sleep(timeDifference);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingManager.LogException(_type, e);
                    }
                }               
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(_type, ex);
            }
            LoggingManager.LogMessage(typeof(ConnectionManager), $"Service Exit at {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}");
        }

        #region Commented

        //private void GetDatafromAPI(string date)
        //{
        //    try
        //    {
        //        username = ConfigurationManager.AppSettings["username"].ToString();
        //        password = ConfigurationManager.AppSettings["password"].ToString();

        //        //Get the employee records
        //        var req = (HttpWebRequest)WebRequest.Create(blueTreeURL);
        //        req.Method = "POST";
        //        req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}:{1}", username, password)));
        //        var data = Encoding.Default.GetBytes("{\"dateOfVerification\":\"" + date + "\"}");
        //       // var data = Encoding.Default.GetBytes("{\"dateOfVerification\":\""+ DateTime.Now.ToString("yyyy-MM-dd") +"\"}");
        //        req.ContentType = "application/json";
        //        req.ContentLength = data.Length;                
        //        req.Accept = "*/*";

        //        using (var stream = req.GetRequestStream())
        //        {
        //            stream.Write(data, 0, data.Length);
        //        }

        //        var response = (HttpWebResponse)req.GetResponse();

        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();                

        //        //parse json and fill all the objects               
        //        List<BTEmployeeInformation> employeeInformationList = JsonConvert.DeserializeObject<List<BTEmployeeInformation>>(responseString);

        //        LoggingManager.LogMessage(typeof(ConnectionManager),"On Date : " + DateTime.Now.ToString("yyyy-MM-dd") + " Total records extracted are :" + employeeInformationList.Count);
        //        SaveDatainERPDB(employeeInformationList);

        //        LoggingManager.LogMessage(typeof(ConnectionManager), "Records integrated into ERP successfully");
        //    }
        //    catch(Exception ex)
        //    {
        //        LoggingManager.LogException(_type, ex);
        //    }
        //}

        //private void SaveDatainERPDB(List<BTEmployeeInformation> employeeInformationList)
        //{
        //    try
        //    {
        //        if(employeeInformationList != null)
        //        {
        //            SqlConnection connection = new SqlConnection(dbConnectionString);                   
        //            //Upsert BlueTree and ERP Data
        //            ConnectionManager.UpsertBlueTreeERPData(employeeInformationList, connection);  
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogException(_type, ex);
        //    }
        //}

        //private void GetDatafromESIAPI(string date)
        //{
        //    try
        //    {
        //        username = ConfigurationManager.AppSettings["username"].ToString();
        //        password = ConfigurationManager.AppSettings["password"].ToString();

        //        //Get the employee records
        //        var req = (HttpWebRequest)WebRequest.Create(blueTreeESIURL);
        //        req.Method = "POST";
        //        req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}:{1}", username, password)));
        //        var data = Encoding.Default.GetBytes("{\"dateOfVerification\":\""+ date + "\"}");
        //        //var data = Encoding.Default.GetBytes("{\"dateOfVerification\":\""+ DateTime.Now.ToString("yyyy-MM-dd") +"\"}");
        //        req.ContentType = "application/json";
        //        req.ContentLength = data.Length;
        //        req.Accept = "*/*";

        //        using (var stream = req.GetRequestStream())
        //        {
        //            stream.Write(data, 0, data.Length);
        //        }

        //        var response = (HttpWebResponse)req.GetResponse();

        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //        //parse json and fill all the objects               
        //        List<BTEmployeeESIInformation> employeeESIInformationList = JsonConvert.DeserializeObject<List<BTEmployeeESIInformation>>(responseString);

        //        LoggingManager.LogMessage(typeof(ConnectionManager), "On Date : " + DateTime.Now.ToString("yyyy-MM-dd") + " Total records extracted are :" + employeeESIInformationList.Count);
        //        UpdateDatainERPDB(employeeESIInformationList);

        //        LoggingManager.LogMessage(typeof(ConnectionManager), "Records integrated into ERP successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogException(_type, ex);
        //    }
        //}

        //private void UpdateDatainERPDB(List<BTEmployeeESIInformation> employeeESIInformationList)
        //{
        //    try
        //    {
        //        if (employeeESIInformationList != null)
        //        {
        //            SqlConnection connection = new SqlConnection(dbConnectionString);

        //            //Upsert BlueTree and ERP Data
        //            ConnectionManager.UpdateDataintoERP(employeeESIInformationList, connection);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LoggingManager.LogException(_type, ex);
        //    }
        //}
        #endregion

        private async Task GetDatafromAPI(string StartDate, string EndDate)
        {
            try
            {
                ApiRequest request = new ApiRequest
                {
                    Address = blueTreeURL,
                    Data = CreateBlueTreeInputParameter(StartDate, EndDate),
                    Password = password,
                    UserName = username
                };
                var response = await Task.Run(() => ApiService.InvokeApirequest<List<BTEmployeeInformation>>(request));
                if (response != null && !string.IsNullOrEmpty(response.Message) && response.Message.ToLower() == "success")
                {
                    var employeeInformationList = response.Output;
                    LoggingManager.LogMessage(typeof(ConnectionManager), "On Date : " + DateTime.Now.ToString("yyyy-MM-dd") + " Total records extracted are :" + employeeInformationList.Count);
                    SaveDatainERPDB(employeeInformationList);

                    LoggingManager.LogMessage(typeof(ConnectionManager), "Records integrated into ERP successfully");
                }
                else
                {
                    LoggingManager.LogMessage(typeof(ConnectionManager), "Records intigration into ERP failed.");

                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
        }

        private void SaveDatainERPDB(List<BTEmployeeInformation> employeeInformationList)
        {
            try
            {
                if (employeeInformationList != null)
                {
                    SqlConnection connection = new SqlConnection(dbConnectionString);
                    //Upsert BlueTree and ERP Data
                    ConnectionManager.UpsertBlueTreeERPData(employeeInformationList, connection);
                }

            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
        }

        private async Task GetDatafromESIAPI(string StartDate, string EndDate)
        {
            try
            {
                var request = new ApiRequest
                {
                    Address = blueTreeESIURL,
                    Data = CreateBlueTreeInputParameter(StartDate, EndDate),
                    Password = password,
                    UserName = username
                };

                var esiResponse = await Task.Run(() => ApiService.InvokeApirequest<List<BTEmployeeESIInformation>>(request));
                if (esiResponse != null && !string.IsNullOrEmpty(esiResponse.Message) && esiResponse.Message.ToLower() == "success")
                {
                    var employeeESIInformationList = esiResponse.Output;
                    LoggingManager.LogMessage(typeof(ConnectionManager), "On Date : " + DateTime.Now.ToString("yyyy-MM-dd") + " Total records extracted are :" + employeeESIInformationList.Count);
                    UpdateDatainERPDB(employeeESIInformationList);

                    LoggingManager.LogMessage(typeof(ConnectionManager), "Records integrated into ERP successfully");
                }
                else
                {
                    LoggingManager.LogMessage(typeof(ConnectionManager), "Records integration into ERP failed.");

                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
        }

        private void UpdateDatainERPDB(List<BTEmployeeESIInformation> employeeESIInformationList)
        {
            try
            {
                if (employeeESIInformationList != null)
                {
                    SqlConnection connection = new SqlConnection(dbConnectionString);

                    //Upsert BlueTree and ERP Data
                    ConnectionManager.UpdateDataintoERP(employeeESIInformationList, connection);
                }

            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
        }

        private async Task ProcessData(string FromDate, string ToDate)
        {
            try
            {
                LoggingManager.LogMessage(this.GetType(), $"From Date : {FromDate} and To Date : {ToDate}");
                LoggingManager.LogMessage(this.GetType(), $"Processing part 1 begins \n");
                await GetDatafromAPI(FromDate, ToDate);
                LoggingManager.LogMessage(this.GetType(), $"Processing part 1 ends \n");
                LoggingManager.LogMessage(this.GetType(), $"------------------------------\n");
                LoggingManager.LogMessage(this.GetType(), $"Processing part 2 begins \n");
                await GetDatafromESIAPI(FromDate, ToDate);
                LoggingManager.LogMessage(this.GetType(), $"Processing part 2 ends \n");
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);

            }
        }
        dynamic CreateBlueTreeInputParameter(string StartDate, string EndDate)
        {
            return new
            {
                dateOfVerification = StartDate,
                toDate = EndDate
            };

        }

    }
}