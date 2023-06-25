using DataIntegrationService.ApiUtility;
using DataIntegrationService.Entities;
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
    public sealed class SchedulerServiceV3
    {
        #region Fields and properties
        const string DateTimeformatForComparision = "yyyy-MM-dd HH:mm:00";
        static readonly string FirstCallStartTime = Convert.ToString(ConfigurationManager.AppSettings["FirstCallStartTime"]);
        static readonly string FirstCallEndTime = Convert.ToString(ConfigurationManager.AppSettings["FirstCallEndTime"]);
        static readonly string SecondCallStartTime = Convert.ToString(ConfigurationManager.AppSettings["SecondCallStartTime"]);
        static readonly string SecondCallEndTime = Convert.ToString(ConfigurationManager.AppSettings["SecondCallEndTime"]);
        static readonly bool isServiceOn = bool.Parse(ConfigurationManager.AppSettings["IsServiceOn"].ToString());
        static readonly string blueTreeURL = ConfigurationManager.AppSettings["BlueTreeURL"].ToString();
        static readonly string blueTreeESIURL = ConfigurationManager.AppSettings["blueTreeESIURL"].ToString();
        static readonly string username = ConfigurationManager.AppSettings["username"].ToString();
        static readonly string password = ConfigurationManager.AppSettings["password"].ToString();
        static readonly string dbConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        static bool FirstTime;
        private static readonly Lazy<SchedulerServiceV3> lazy = new Lazy<SchedulerServiceV3>(() => new SchedulerServiceV3());
        static bool FirstCallComplete;
        static bool SecondCallComplete;
        string FirstCallStartDate { get { return string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), FirstCallStartTime); } }
        string FirstCallEndDate { get { return string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), FirstCallEndTime); } }
        string SecondCallStartDate { get { return string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), SecondCallStartTime); } }
        string SecondCallEndDate { get { return string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), SecondCallEndTime); } }
        #endregion

        public static SchedulerServiceV3 Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        /// <summary>
        /// This method will create input parameter to invoke the Blue Tree API's
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        dynamic CreateBlueTreeInputParameter(string StartDate, string EndDate)
        {
            return new
            {
                dateOfVerification = StartDate,
                toDate = EndDate
            };

        }


        /// <summary>
        /// constructor
        /// </summary>
        private SchedulerServiceV3()
        {
            try
            {
                FirstTime = true;
                FirstCallComplete = false;
                SecondCallComplete = false;
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
        }

        public async void Start()
        {
            LoggingManager.LogMessage(typeof(ConnectionManager), "Service Started");
            try
            {
                while (true)
                {
                    if (FirstTime) // First call 
                    {
                        if (Convert.ToDateTime(DateTime.Now.ToString(DateTimeformatForComparision)) == Convert.ToDateTime(FirstCallEndDate))
                        {
                            // Invoke the Method 
                            await ProcessData(FirstCallStartDate.ToString(), FirstCallEndDate.ToString());
                            // sleep the Thread
                            LoggingManager.LogMessage(this.GetType(), DateTime.Now.ToString());

                            FirstCallComplete = true;
                            SecondCallComplete = false;
                            var timeDifference = (int)(Convert.ToDateTime(SecondCallEndDate) - DateTime.Now).TotalMilliseconds;
                            Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));
                        }
                        else
                        {
                            //Wait till the Next Loop.                            
                            var timeDifference = (int)(Convert.ToDateTime(FirstCallEndDate) - DateTime.Now).TotalMilliseconds;
                            Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));
                        }
                        FirstTime = false;
                    }
                    else
                    {
                        if (!FirstCallComplete && !SecondCallComplete)
                        {
                            if (Convert.ToDateTime(DateTime.Now.ToString(DateTimeformatForComparision)) == Convert.ToDateTime(FirstCallEndDate))
                            {
                                // Invoke the Method 
                                await ProcessData(FirstCallStartDate.ToString(), FirstCallEndDate.ToString());

                                // sleep the Thread
                                LoggingManager.LogMessage(this.GetType(), DateTime.Now.ToString());

                                FirstCallComplete = true;
                                SecondCallComplete = false;
                                var timeDifference = (int)(Convert.ToDateTime(SecondCallEndDate) - DateTime.Now).TotalMilliseconds;
                                Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));
                            }
                            else
                            {
                                var timeDifference = (int)(Convert.ToDateTime(FirstCallEndDate) - DateTime.Now).TotalMilliseconds;
                                Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));

                            }
                        }
                        else if (FirstCallComplete && !SecondCallComplete)
                        {
                            if (Convert.ToDateTime(DateTime.Now.ToString(DateTimeformatForComparision)) == Convert.ToDateTime(SecondCallEndDate))
                            {
                                // Invoke the Method 
                                await ProcessData(SecondCallStartDate.ToString(), SecondCallEndDate.ToString());

                                // sleep the Thread
                                LoggingManager.LogMessage(this.GetType(), DateTime.Now.ToString());

                                SecondCallComplete = true;
                                FirstCallComplete = false;
                                var timeDifference = (int)(Convert.ToDateTime(FirstCallEndDate).AddDays(1) - DateTime.Now).TotalMilliseconds;
                                LoggingManager.LogMessage(this.GetType(), "Next Call after " + TimeSpan.FromMilliseconds(timeDifference).Hours.ToString() + " hours");

                                LoggingManager.LogMessage(this.GetType(), "Next Call : " + Convert.ToDateTime(FirstCallEndDate).AddDays(1).ToString());

                                Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));
                            }
                            else
                            {
                                var timeDifference = (int)(Convert.ToDateTime(SecondCallEndDate) - DateTime.Now).TotalMilliseconds;
                                Thread.Sleep(TimeSpan.FromMilliseconds(timeDifference));

                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                LoggingManager.LogException(this.GetType(), ex);
            }
            LoggingManager.LogMessage(typeof(ConnectionManager), $"Service Exit at {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}");
        }

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

    }
}