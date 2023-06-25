using DataIntegrationService.Entities;
using DataIntegrationService.Helpers;
using LoggingLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace DataIntegrationService.Models
{
    public class ConnectionManager
    {        
        public static string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public static string _filePath = ConfigurationManager.AppSettings["FilePath"].ToString();
        static Dictionary<string, string> prefixValuePair = new Dictionary<string, string>();
        static DataTable employeeEntitlementTable = null;
        static List<string> entitlementList = new List<string>() { "Basic", "F.D.A", "Unit Allowance", "P.F. Deduction", "E.S.I. Deduction", "Weekly Off Allowance", "Professional Tax", "Gun Duity Allowance" };
        static List<string> entitlementSalHead = new List<string>() { "SHC000001", "SHC000003", "SHC000004", "SHC000005", "SHC000006", "SHC000007", "SHC000017", "SHC000019" };
        public static List<ServiceMaster> GetServiceMasterData()
        {
            List<ServiceMaster> serviceMasterList = new List<ServiceMaster>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT ServiceCode, Description AS Rank from ServiceMaster";
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            ServiceMaster obj = new ServiceMaster();
                            obj.ServiceCode = reader[0] == null ? string.Empty : reader[0].ToString();
                            obj.Rank = reader[1] == null ? string.Empty : reader[1].ToString();

                            serviceMasterList.Add(obj);
                        }
                        
                    }
                    reader.Close();
                }                
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to get service details");
            }            
            return serviceMasterList;
        }

        public static List<BranchMaster> GetBranchMasterData()
        {
            List<BranchMaster> branchMasterList = new List<BranchMaster>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = @"Select BM.code, BM.name AS BranchName, Prefix from BranchMaster BM INNER JOIN BranchHirarchy BH ON BM.code=BH.Branchcode INNER JOIN  
                                        sequenceMaster SM ON SM.Head = BM.code";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            BranchMaster obj = new BranchMaster();
                            obj.Code = reader[0] == null ? string.Empty : reader[0].ToString();
                            obj.BranchName = reader[1] == null ? string.Empty : reader[1].ToString();
                            obj.Prefix = reader[2] == null ? string.Empty : reader[2].ToString();
                            branchMasterList.Add(obj);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to get branch details");
            }
            return branchMasterList;
        }

        public static List<UnitMaster> GetUnitMasterData()
        {
            List<UnitMaster> unitMasterList = new List<UnitMaster>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT RegionID,RegionName,BranchCode,BranchName,UnitCode,SiteName,SiteAddress from View_UnitMaster  WHERE Disbanded = '0'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            UnitMaster obj = new UnitMaster();
                            obj.RegionID = reader[0] == null ? string.Empty : reader[0].ToString();
                            obj.RegionName = reader[1] == null ? string.Empty : reader[1].ToString();
                            obj.BranchCode = reader[2] == null ? string.Empty : reader[2].ToString();
                            obj.BranchName = reader[3] == null ? string.Empty : reader[3].ToString();
                            obj.UnitCode = reader[4] == null ? string.Empty : reader[4].ToString();
                            obj.SiteName = reader[5] == null ? string.Empty : reader[5].ToString();
                            obj.SiteAddress = reader[6] == null ? string.Empty : reader[6].ToString();
                            unitMasterList.Add(obj);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to get unit details");
            }
            return unitMasterList;
        }

        public static List<CityMaster> GetCityMasterData()
        {
            List<CityMaster> cityMasterList = new List<CityMaster>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT CityID,CityName from CityMaster";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CityMaster obj = new CityMaster();
                            obj.CityID = reader[0] == null ? string.Empty : reader[0].ToString();
                            obj.CityName = reader[1] == null ? string.Empty : reader[1].ToString();

                            cityMasterList.Add(obj);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to get city details");
            }
            return cityMasterList;
        }

        public static List<CasteMaster> GetCasteMasterData()
        {
            List<CasteMaster> casteMasterList = new List<CasteMaster>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT CasteCode,CasteName FROM CasteMaster";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            CasteMaster obj = new CasteMaster();
                            obj.CasteCode = reader[0] == null ? string.Empty : reader[0].ToString();
                            obj.CasteName = reader[1] == null ? string.Empty : reader[1].ToString();

                            casteMasterList.Add(obj);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to get city details");
            }
            return casteMasterList;
        }

        public static bool ValidateUser(string userName,string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = string.Format("SELECT COUNT(*) FROM Users WHERE Username = '{0}' and Password = '{1}'", userName, password);
                    object value  = cmd.ExecuteScalar();

                    return int.Parse(value.ToString()) == 0 ? false : true;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex, "Failed to validate user");
            }
            return false;
        }

        private static bool IsEmployeeNumberExists(string employeeNumber,SqlConnection connection)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = string.Format("SELECT COUNT(*) FROM EmployeeMaster WHERE BranchEmpCode = '{0}'", employeeNumber);
                object result = command.ExecuteScalar();

                return (int.Parse(result.ToString()) == 0) ? false : true;
            }
            catch(Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex);
            }
            return false;
        }

        public static void UpsertBlueTreeERPData(List<BTEmployeeInformation> employeeInformationList,SqlConnection connection)
        {
            try
            {
                if (employeeInformationList != null && employeeInformationList.Count > 0)
                {
                    DataSet blueTreeInsertDataSet = CreateBlueTreeDataSet();
                    
                    foreach (BTEmployeeInformation employeeInformation in employeeInformationList)
                    {
                        //LoggingManager.LogMessage(typeof(ConnectionManager),$"Procesing the data {JsonConvert.SerializeObject(employeeInformation)} ");

                        LoggingManager.LogMessage(typeof(ConnectionManager), $"Processing the employee {employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber} begins");
                        if (IsEmployeeNumberExists(employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber, connection))
                        {
                            LoggingManager.LogMessage(typeof(ConnectionManager), employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + " Already Exists");
                        }
                        else
                        {
                            try
                            {
                                #region Filling tables for BlueTree
                                DataRow newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KEmployeeBasicDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNumber] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KAadharAddress] = employeeInformation?.employeeBasicDetails[0]?.AadharAddress;
                                newRow[GlobalConstants.KAadharNo] = employeeInformation?.employeeBasicDetails[0]?.AadharNo;
                                newRow[GlobalConstants.KAadharVerified] = employeeInformation?.employeeBasicDetails[0]?.AadharVerified;
                                newRow[GlobalConstants.KActive] = employeeInformation?.employeeBasicDetails[0]?.Active;
                                newRow[GlobalConstants.KBloodGroup] = employeeInformation?.employeeBasicDetails[0]?.BloodGroup;
                                newRow[GlobalConstants.KColorCode] = employeeInformation?.employeeBasicDetails[0]?.ColorCode;
                                newRow[GlobalConstants.KCustomer] = employeeInformation?.employeeBasicDetails[0]?.Customer;
                                newRow[GlobalConstants.KCustomerLocation] = employeeInformation?.employeeBasicDetails[0]?.CustomerLocation;
                                newRow[GlobalConstants.KDateofBirth] = employeeInformation?.employeeBasicDetails[0]?.DateofBirth;
                                newRow[GlobalConstants.KDateofVerification] = employeeInformation?.employeeBasicDetails[0]?.DateofVerification;
                                newRow[GlobalConstants.KEmail] = employeeInformation?.employeeBasicDetails[0]?.Email;
                                newRow[GlobalConstants.KEsiNo] = employeeInformation?.employeeBasicDetails[0]?.EsiNo;
                                newRow[GlobalConstants.KFatherName] = employeeInformation?.employeeBasicDetails[0]?.FatherName;
                                newRow[GlobalConstants.KGender] = employeeInformation?.employeeBasicDetails[0]?.Gender;
                                newRow[GlobalConstants.KHomePhone] = employeeInformation?.employeeBasicDetails[0]?.HomePhone;
                                newRow[GlobalConstants.KJobRoleName] = employeeInformation?.employeeBasicDetails[0]?.JobRoleName;
                                newRow[GlobalConstants.KLastName] = employeeInformation?.employeeBasicDetails[0]?.LastName;
                                newRow[GlobalConstants.KLocationName] = employeeInformation?.employeeBasicDetails[0]?.LocationName;
                                newRow[GlobalConstants.KMaritalStatus] = employeeInformation?.employeeBasicDetails[0]?.MaritalStatus;
                                newRow[GlobalConstants.KMiddleName] = employeeInformation?.employeeBasicDetails[0]?.MiddleName;
                                newRow[GlobalConstants.KMobileNo] = employeeInformation?.employeeBasicDetails[0]?.MobileNo;
                                newRow[GlobalConstants.KMotherName] = employeeInformation?.employeeBasicDetails[0]?.MotherName;
                                newRow[GlobalConstants.KName] = employeeInformation?.employeeBasicDetails[0]?.Name;
                                newRow[GlobalConstants.KOrganizationName] = employeeInformation?.employeeBasicDetails[0]?.OrganizationName;
                                newRow[GlobalConstants.KOtherName] = employeeInformation?.employeeBasicDetails[0]?.OtherName;
                                newRow[GlobalConstants.KRegion] = employeeInformation?.employeeBasicDetails[0]?.Region;
                                newRow[GlobalConstants.KSpouseName] = employeeInformation?.employeeBasicDetails[0]?.SpouseName;
                                newRow[GlobalConstants.KStatus] = employeeInformation?.employeeBasicDetails[0]?.Status;
                                newRow[GlobalConstants.KVerifier] = employeeInformation?.employeeBasicDetails[0]?.Verifier;
                                newRow[GlobalConstants.KWorkLocation] = employeeInformation?.employeeBasicDetails[0]?.WorkLocation;
                                newRow[GlobalConstants.KWorkLocationName] = employeeInformation?.employeeBasicDetails[0]?.WorkLocationName;
                                newRow["CreatedUser"] = employeeInformation?.employeeBasicDetails?[0]?.CreateUser;
                                newRow["CreatedDate"] = employeeInformation?.employeeBasicDetails?[0]?.CreatedDate;
                                newRow["EmployeePhoto"] = Convert.FromBase64String(employeeInformation?.employeeBasicDetails?[0]?.EmployeePhoto);
                                newRow["LocationID"] = employeeInformation?.employeeBasicDetails[0]?.LocationID;
                                blueTreeInsertDataSet.Tables[GlobalConstants.KEmployeeBasicDetails].Rows.Add(newRow);

                                if (employeeInformation?.employeeBasicDetails[0]?.Address != null)
                                {
                                    newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTEmployeeBasicDetailsAddress].NewRow();
                                    newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                    newRow[GlobalConstants.KAddress1] = employeeInformation?.employeeBasicDetails[0]?.Address.Address1;
                                    newRow[GlobalConstants.KAddress2] = employeeInformation?.employeeBasicDetails[0]?.Address.Address2;
                                    newRow[GlobalConstants.KAddress3] = employeeInformation?.employeeBasicDetails[0]?.Address.Address3;
                                    newRow[GlobalConstants.KCity] = employeeInformation?.employeeBasicDetails[0]?.Address.City;
                                    newRow[GlobalConstants.KCountry] = employeeInformation?.employeeBasicDetails[0]?.Address.Country;
                                    newRow[GlobalConstants.KState] = employeeInformation?.employeeBasicDetails[0]?.Address.State;
                                    newRow[GlobalConstants.KZipCode] = employeeInformation?.employeeBasicDetails[0]?.Address.ZipCode;
                                    blueTreeInsertDataSet.Tables[GlobalConstants.KBTEmployeeBasicDetailsAddress].Rows.Add(newRow);
                                }

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTAdditionalDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KPosition] = employeeInformation?.additionalDetails?.Position;
                                newRow[GlobalConstants.KEmployeeType] = employeeInformation?.additionalDetails?.EmployeeType;
                                newRow[GlobalConstants.KDepartment] = employeeInformation?.additionalDetails?.Department;
                                newRow[GlobalConstants.KDesignation] = employeeInformation?.additionalDetails?.Designation;
                                newRow[GlobalConstants.KJoiningDate] = employeeInformation?.additionalDetails?.JoiningDate;
                                newRow[GlobalConstants.KHQualification] = employeeInformation?.additionalDetails?.HQualification;
                                newRow["Religion"] = employeeInformation?.additionalDetails?.Religion;
                                newRow["Caste"] = employeeInformation?.additionalDetails?.Caste;
                                newRow["candidatePlaceOfBirth"] = employeeInformation?.additionalDetails?.candidatePlaceOfBirth;
                                newRow["erpLastName"] = employeeInformation?.additionalDetails?.erpLastName;
                                newRow["CreatedDateTime"] = DateTime.Now.ToString("dd-MM-yyyy");
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTAdditionalDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTAadharDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KCo] = employeeInformation?.AadharDetails[0]?.Co;
                                newRow[GlobalConstants.KDistrict] = employeeInformation?.AadharDetails[0]?.District;
                                newRow[GlobalConstants.KEmail] = employeeInformation?.AadharDetails[0]?.Email;
                                newRow[GlobalConstants.KFailureReason] = employeeInformation?.AadharDetails[0]?.FailureReason;
                                newRow[GlobalConstants.KGender] = employeeInformation?.AadharDetails[0]?.Gender;
                                newRow[GlobalConstants.KHouse] = employeeInformation?.AadharDetails[0]?.House;
                                newRow[GlobalConstants.KId] = employeeInformation?.AadharDetails[0]?.Id;
                                newRow[GlobalConstants.KLandmark] = employeeInformation?.AadharDetails[0]?.Landmark;
                                newRow[GlobalConstants.KLc] = employeeInformation?.AadharDetails[0]?.Lc;
                                newRow[GlobalConstants.KName] = employeeInformation?.AadharDetails[0]?.Name;
                                newRow[GlobalConstants.KNew] = employeeInformation?.AadharDetails[0]?.New;
                                newRow[GlobalConstants.KPhoneNumber] = employeeInformation?.AadharDetails[0]?.PhoneNumber;
                                newRow[GlobalConstants.KPincode] = employeeInformation?.AadharDetails[0]?.Pincode;
                                newRow[GlobalConstants.KPostOffice] = employeeInformation?.AadharDetails[0]?.PostOffice;
                                newRow[GlobalConstants.KSaveResponse] = employeeInformation?.AadharDetails[0]?.SaveResponse;
                                newRow[GlobalConstants.KState] = employeeInformation?.AadharDetails[0]?.State;
                                newRow[GlobalConstants.KStreet] = employeeInformation?.AadharDetails[0]?.Street;
                                newRow[GlobalConstants.KSubdistrict] = employeeInformation?.AadharDetails[0]?.Subdistrict;
                                newRow[GlobalConstants.KUIDTag] = employeeInformation?.AadharDetails[0]?.UIDTag;
                                newRow[GlobalConstants.KUuId] = employeeInformation?.AadharDetails[0]?.UuId;
                                newRow[GlobalConstants.KVtc] = employeeInformation?.AadharDetails[0]?.Vtc;
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTAadharDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTBankDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KPANNumber] = employeeInformation?.bankAccountDetails?.PANNumber;
                                newRow[GlobalConstants.KBankAccountNumber] = employeeInformation?.bankAccountDetails?.BankAccountNumber;
                                newRow[GlobalConstants.KIFSCCode] = employeeInformation?.bankAccountDetails?.IFSCCode;
                                newRow[GlobalConstants.KBankName] = employeeInformation?.bankAccountDetails?.BankName;
                                newRow[GlobalConstants.KBankBranch] = employeeInformation?.bankAccountDetails?.BankBranch;
                                newRow[GlobalConstants.KAddress] = employeeInformation?.bankAccountDetails?.Address;
                                newRow[GlobalConstants.KCity] = employeeInformation?.bankAccountDetails?.City;
                                newRow[GlobalConstants.KState] = employeeInformation?.bankAccountDetails?.State;
                                newRow["BankDocument"] = ConvertintoDocument(employeeInformation?.bankAccountDetails?.BankDocument, employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + "_BankDocument", "BlueTree\\" + employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTBankDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTCurrentAddress].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KAddress1] = employeeInformation?.currentAddress?.Address1;
                                newRow[GlobalConstants.KAddress2] = employeeInformation?.currentAddress?.Address2;
                                newRow[GlobalConstants.KCity] = employeeInformation?.currentAddress?.City;
                                newRow[GlobalConstants.KState] = employeeInformation?.currentAddress?.State;
                                newRow[GlobalConstants.KDistrict] = employeeInformation?.currentAddress?.District;
                                newRow[GlobalConstants.KCountry] = employeeInformation?.currentAddress?.Country;
                                newRow[GlobalConstants.KPostalCode] = employeeInformation?.currentAddress?.PostalCode;
                                newRow["Nationality"] = employeeInformation?.currentAddress?.Nationality;
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTCurrentAddress].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTFamilyDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KNameasPerAadhar] = employeeInformation?.familyDetails?.NameasPerAadhar;
                                newRow[GlobalConstants.KRelationshipWithEmployee] = employeeInformation?.familyDetails?.RelationshipWithEmployee;
                                newRow[GlobalConstants.KAadharNo] = employeeInformation?.familyDetails?.AadharNo;
                                newRow[GlobalConstants.KGender] = employeeInformation?.familyDetails?.Gender;
                                newRow[GlobalConstants.KDOB] = employeeInformation?.familyDetails?.DOB;
                                newRow[GlobalConstants.KDependentYN] = employeeInformation?.familyDetails?.DependentYN;
                                newRow[GlobalConstants.KResidingWithEmployee] = employeeInformation?.familyDetails?.ResidingWithEmployee;
                                newRow[GlobalConstants.KAddressAsPerAadhar] = employeeInformation?.familyDetails?.AddressasPerAadhar;
                                newRow[GlobalConstants.KAddress1] = employeeInformation?.familyDetails?.Address1;
                                newRow[GlobalConstants.KState] = employeeInformation?.familyDetails?.State;
                                newRow[GlobalConstants.KDistrict] = employeeInformation?.familyDetails?.District;
                                newRow[GlobalConstants.KFamilyDetailsDocument] = ConvertintoDocument(employeeInformation?.familyDetails?.FamilyDetailsDocument, employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + "_FamilyDetailsDocument", "BlueTree\\" + employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTFamilyDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTNomineeDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KName] = employeeInformation?.nomineeInformation?.Name;
                                newRow[GlobalConstants.KRelationshipWithEmployee] = employeeInformation?.nomineeInformation?.ResidingWithEmployee;
                                newRow[GlobalConstants.KAadharNo] = employeeInformation?.nomineeInformation?.AadharNo;
                                newRow[GlobalConstants.KGender] = employeeInformation?.nomineeInformation?.Gender;
                                newRow[GlobalConstants.KDOB] = employeeInformation?.nomineeInformation?.DOB;
                                newRow[GlobalConstants.KMobileNo] = employeeInformation?.nomineeInformation?.MobileNo;
                                newRow[GlobalConstants.KAddressAsPerAadhar] = employeeInformation?.nomineeInformation?.AddressasPerAadhar;
                                newRow[GlobalConstants.KAddress] = employeeInformation?.nomineeInformation?.Address;
                                newRow[GlobalConstants.KState] = employeeInformation?.nomineeInformation?.State;
                                newRow[GlobalConstants.KDistrict] = employeeInformation?.nomineeInformation?.District;
                                newRow[GlobalConstants.KPincode] = string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.Pincode) ? "0" : employeeInformation?.nomineeInformation?.Pincode;
                                newRow[GlobalConstants.KResidingWithEmployee] = employeeInformation?.nomineeInformation?.ResidingWithEmployee;
                                newRow[GlobalConstants.KSupportingDoc1Upload] = employeeInformation?.nomineeInformation?.SupportingDoc1Upload;
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTNomineeDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTPFDetails].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KNoPFAccount] = employeeInformation?.PFDetails?.NoPFAccount;
                                newRow[GlobalConstants.KPreviousPFAccountNumber] = employeeInformation?.PFDetails?.PreviousPFAccountNumber;
                                newRow[GlobalConstants.KdateOfExitFromPreviousEmployment] = employeeInformation?.PFDetails?.dateOfExitFromPreviousEmployment;
                                newRow[GlobalConstants.KschemeCertificateNoIfIssued] = employeeInformation?.PFDetails?.schemeCertificateNoIfIssued;
                                newRow[GlobalConstants.KppoNumberIfIssued] = employeeInformation?.PFDetails?.ppoNumberIfIssued;
                                newRow[GlobalConstants.KinternationalWorkerYesno] = employeeInformation?.PFDetails?.ppoNumberIfIssued;
                                newRow[GlobalConstants.KifYesNameOfTheCountry] = employeeInformation?.PFDetails?.ifYesNameOfTheCountry;
                                newRow[GlobalConstants.KpfNumber] = employeeInformation?.PFDetails?.pfNumber;
                                newRow[GlobalConstants.KplaceOfIssue] = employeeInformation?.PFDetails?.placeOfIssue;
                                newRow[GlobalConstants.KdateOfIssue] = employeeInformation?.PFDetails?.dateOfIssue;
                                newRow[GlobalConstants.KIssuingAuthority] = employeeInformation?.PFDetails?.IssuingAuthority;
                                newRow[GlobalConstants.KValidFrom] = employeeInformation?.PFDetails?.ValidFrom;
                                newRow[GlobalConstants.KPreviousEmploymentUAN] = employeeInformation?.PFDetails?.PreviousEmploymentUAN;
                                newRow[GlobalConstants.KisFromNortheastnepalbhutan] = employeeInformation?.PFDetails?.isFromNortheastnepalbhutan;
                                newRow[GlobalConstants.KuanNumber] = employeeInformation?.PFDetails?.uanNumber;
                                newRow[GlobalConstants.KPFDocUpload] = ConvertintoDocument(employeeInformation?.PFDetails?.PFDocUpload,  employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + "_PFDocument", "BlueTree\\" + employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTPFDetails].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables[GlobalConstants.KBTPermanentAddress].NewRow();
                                newRow[GlobalConstants.KEmployeeNo] = employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber;
                                newRow[GlobalConstants.KAddressAsPerAadhar] = employeeInformation?.permanentAddress?.AddressAsPerAadhar;
                                newRow[GlobalConstants.KAddressAsPerCurrentAddress] = employeeInformation?.permanentAddress?.AddressAsPerCurrentAddress;
                                newRow[GlobalConstants.KAddress1] = employeeInformation?.permanentAddress?.Address1;
                                newRow[GlobalConstants.KAddress2] = employeeInformation?.permanentAddress?.Address2;
                                newRow[GlobalConstants.KCity] = employeeInformation?.permanentAddress?.City;
                                newRow[GlobalConstants.KState] = employeeInformation?.permanentAddress?.State;
                                newRow[GlobalConstants.KDistrict] = employeeInformation?.permanentAddress?.District;
                                newRow[GlobalConstants.KCountry] = employeeInformation?.permanentAddress?.Country;
                                newRow[GlobalConstants.KPostalCode] = employeeInformation?.permanentAddress?.PostalCode;
                                blueTreeInsertDataSet.Tables[GlobalConstants.KBTPermanentAddress].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables["BTESIInformation"].NewRow();
                                newRow["EmployeeNo"] = employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber;
                                newRow["ESICSubCodeLocation"] = employeeInformation?.esiInformation?.ESICSubCodeLocation;
                                newRow["Dispensary"] = employeeInformation?.esiInformation?.Dispensary;
                                blueTreeInsertDataSet.Tables["BTESIInformation"].Rows.Add(newRow);

                                newRow = blueTreeInsertDataSet.Tables["BTBackgroundVerificationDetails"].NewRow();
                                newRow["employeeNumber"] = employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber;
                                newRow["BGVRequired"] = employeeInformation?.backgroundVerificationDetails?.BGVRequired;
                                newRow["BackgroundVerificationDocUpload"] = ConvertintoDocument(employeeInformation?.backgroundVerificationDetails?.BackgroundVerificationDocUpload, employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + "_BGVDocument", "BlueTree\\" + employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                                newRow["AppointmentLetterUpload"] = ConvertintoDocument(employeeInformation?.backgroundVerificationDetails?.AppointmentLetterUpload, employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber + "_AppointmentLetter", "BlueTree\\" + employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                                blueTreeInsertDataSet.Tables["BTBackgroundVerificationDetails"].Rows.Add(newRow);
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                LoggingManager.LogException(typeof(ConnectionManager), ex);
                            }

                            #region Filling Data for ERP
                            InsertDataintoERP(employeeInformation, connection);
                            #endregion
                            LoggingManager.LogMessage(typeof(ConnectionManager), $"Processing the employee {employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber} ends");
                        }
                    }

                    InsertData(blueTreeInsertDataSet, connection);
                    prefixValuePair.Clear();
                    blueTreeInsertDataSet = null;

                }
            }
            catch(Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager),ex);
            }
        }

        private static DataSet CreateBlueTreeDataSet()
        {
            DataSet blueTreeDataSet = new DataSet();
            DataTable table = new DataTable(GlobalConstants.KEmployeeBasicDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNumber);
            table.Columns.Add(GlobalConstants.KAadharAddress);
            table.Columns.Add(GlobalConstants.KAadharNo);
            table.Columns.Add(GlobalConstants.KAadharVerified);
            table.Columns.Add(GlobalConstants.KActive);            
            table.Columns.Add(GlobalConstants.KBloodGroup);
            table.Columns.Add(GlobalConstants.KColorCode);
            table.Columns.Add(GlobalConstants.KCustomer);
            table.Columns.Add(GlobalConstants.KCustomerLocation);
            table.Columns.Add(GlobalConstants.KDateofBirth);
            table.Columns.Add(GlobalConstants.KDateofVerification );
            table.Columns.Add(GlobalConstants.KEmail );
            table.Columns.Add(GlobalConstants.KEsiNo);
            table.Columns.Add(GlobalConstants.KFatherName);
            table.Columns.Add(GlobalConstants.KGender);
            table.Columns.Add(GlobalConstants.KHomePhone);
            table.Columns.Add(GlobalConstants.KJobRoleName);
            table.Columns.Add(GlobalConstants.KLastName);
            table.Columns.Add(GlobalConstants.KLocationName);
            table.Columns.Add(GlobalConstants.KMaritalStatus);
            table.Columns.Add(GlobalConstants.KMiddleName);
            table.Columns.Add(GlobalConstants.KMobileNo);
            table.Columns.Add(GlobalConstants.KMotherName);
            table.Columns.Add(GlobalConstants.KName);
            table.Columns.Add(GlobalConstants.KOrganizationName);
            table.Columns.Add(GlobalConstants.KOtherName);
            table.Columns.Add(GlobalConstants.KRegion);
            table.Columns.Add(GlobalConstants.KSpouseName);
            table.Columns.Add(GlobalConstants.KStatus);
            table.Columns.Add(GlobalConstants.KVerifier);
            table.Columns.Add(GlobalConstants.KWorkLocation);
            table.Columns.Add(GlobalConstants.KWorkLocationName);
            table.Columns.Add("CreatedUser");
            table.Columns.Add("CreatedDate");
            table.Columns.Add("EmployeePhoto", typeof(Byte[]));
            table.Columns.Add("LocationID");
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTEmployeeBasicDetailsAddress);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KAddress1);
            table.Columns.Add(GlobalConstants.KAddress2);
            table.Columns.Add(GlobalConstants.KAddress3);
            table.Columns.Add(GlobalConstants.KCity);            
            table.Columns.Add(GlobalConstants.KCountry);
            table.Columns.Add(GlobalConstants.KState);           
            table.Columns.Add(GlobalConstants.KZipCode);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTAdditionalDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KPosition);
            table.Columns.Add(GlobalConstants.KEmployeeType);
            table.Columns.Add(GlobalConstants.KDepartment);
            table.Columns.Add(GlobalConstants.KDesignation);
            table.Columns.Add(GlobalConstants.KJoiningDate);
            table.Columns.Add(GlobalConstants.KHQualification);            
            table.Columns.Add("Religion");
            table.Columns.Add("Caste");
            table.Columns.Add("candidatePlaceOfBirth");
            table.Columns.Add("erpLastName");
            table.Columns.Add("CreatedDateTime");
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTAadharDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KCo);
            table.Columns.Add(GlobalConstants.KDistrict);
            table.Columns.Add(GlobalConstants.KEmail );
            table.Columns.Add(GlobalConstants.KFailureReason);
            table.Columns.Add(GlobalConstants.KGender);
            table.Columns.Add(GlobalConstants.KHouse);
            table.Columns.Add(GlobalConstants.KId);
            table.Columns.Add(GlobalConstants.KLandmark);
            table.Columns.Add(GlobalConstants.KLc);
            table.Columns.Add(GlobalConstants.KName);
            table.Columns.Add(GlobalConstants.KNew);
            table.Columns.Add(GlobalConstants.KPhoneNumber);
            table.Columns.Add(GlobalConstants.KPincode);
            table.Columns.Add(GlobalConstants.KPostOffice);
            table.Columns.Add(GlobalConstants.KSaveResponse);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add(GlobalConstants.KStreet);
            table.Columns.Add(GlobalConstants.KSubdistrict);
            table.Columns.Add(GlobalConstants.KUIDTag);
            table.Columns.Add(GlobalConstants.KUuId);
            table.Columns.Add(GlobalConstants.KVtc);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTCurrentAddress);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KAddress1);
            table.Columns.Add(GlobalConstants.KAddress2);
            table.Columns.Add(GlobalConstants.KCity);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add(GlobalConstants.KDistrict);
            table.Columns.Add(GlobalConstants.KCountry);
            table.Columns.Add(GlobalConstants.KPostalCode);
            table.Columns.Add("Nationality");
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTBankDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KPANNumber);
            table.Columns.Add(GlobalConstants.KBankAccountNumber);
            table.Columns.Add(GlobalConstants.KIFSCCode);
            table.Columns.Add(GlobalConstants.KBankName);
            table.Columns.Add(GlobalConstants.KBankBranch);
            table.Columns.Add(GlobalConstants.KAddress);
            table.Columns.Add(GlobalConstants.KCity);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add("BankDocument");
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTPFDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KNoPFAccount);
            table.Columns.Add(GlobalConstants.KPreviousPFAccountNumber);
            table.Columns.Add(GlobalConstants.KdateOfExitFromPreviousEmployment);
            table.Columns.Add(GlobalConstants.KschemeCertificateNoIfIssued);
            table.Columns.Add(GlobalConstants.KppoNumberIfIssued);
            table.Columns.Add(GlobalConstants.KinternationalWorkerYesno);
            table.Columns.Add(GlobalConstants.KifYesNameOfTheCountry);
            table.Columns.Add(GlobalConstants.KpfNumber);
            table.Columns.Add(GlobalConstants.KplaceOfIssue);
            table.Columns.Add(GlobalConstants.KdateOfIssue);
            table.Columns.Add(GlobalConstants.KIssuingAuthority);
            table.Columns.Add(GlobalConstants.KValidFrom);
            table.Columns.Add(GlobalConstants.KPreviousEmploymentUAN);
            table.Columns.Add(GlobalConstants.KisFromNortheastnepalbhutan);
            table.Columns.Add(GlobalConstants.KuanNumber);
            table.Columns.Add(GlobalConstants.KPFDocUpload);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTPermanentAddress);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KAddressAsPerAadhar);
            table.Columns.Add(GlobalConstants.KAddressAsPerCurrentAddress);
            table.Columns.Add(GlobalConstants.KAddress1);
            table.Columns.Add(GlobalConstants.KAddress2);
            table.Columns.Add(GlobalConstants.KCity);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add(GlobalConstants.KDistrict);
            table.Columns.Add(GlobalConstants.KCountry);
            table.Columns.Add(GlobalConstants.KPostalCode);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTFamilyDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KNameasPerAadhar);
            table.Columns.Add(GlobalConstants.KRelationshipWithEmployee);
            table.Columns.Add(GlobalConstants.KAadharNo);
            table.Columns.Add(GlobalConstants.KGender);
            table.Columns.Add(GlobalConstants.KDOB);
            table.Columns.Add(GlobalConstants.KDependentYN);
            table.Columns.Add(GlobalConstants.KResidingWithEmployee);
            table.Columns.Add(GlobalConstants.KAddressAsPerAadhar);
            table.Columns.Add(GlobalConstants.KAddress1);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add(GlobalConstants.KDistrict);
            table.Columns.Add(GlobalConstants.KFamilyDetailsDocument);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable(GlobalConstants.KBTNomineeDetails);
            table.Columns.Add(GlobalConstants.KEmployeeNo);
            table.Columns.Add(GlobalConstants.KName);
            table.Columns.Add(GlobalConstants.KRelationshipWithEmployee);
            table.Columns.Add(GlobalConstants.KAadharNo);
            table.Columns.Add(GlobalConstants.KGender);
            table.Columns.Add(GlobalConstants.KDOB);
            table.Columns.Add(GlobalConstants.KMobileNo);
            table.Columns.Add(GlobalConstants.KAddressAsPerAadhar);
            table.Columns.Add(GlobalConstants.KAddress);
            table.Columns.Add(GlobalConstants.KState);
            table.Columns.Add(GlobalConstants.KDistrict);
            table.Columns.Add(GlobalConstants.KPincode);
            table.Columns.Add(GlobalConstants.KResidingWithEmployee);
            table.Columns.Add(GlobalConstants.KSupportingDoc1Upload);
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable("BTESIInformation");
            table.Columns.Add("EmployeeNo");
            table.Columns.Add("ESICSubCodeLocation");
            table.Columns.Add("Dispensary");
            blueTreeDataSet.Tables.Add(table);

            table = new DataTable("BTBackgroundVerificationDetails");
            table.Columns.Add("employeeNumber");
            table.Columns.Add("BGVRequired");
            table.Columns.Add("BackgroundVerificationDocUpload");
            table.Columns.Add("AppointmentLetterUpload");
            blueTreeDataSet.Tables.Add(table);
            return blueTreeDataSet;
        }

        private static void InsertDataintoERP(BTEmployeeInformation employeeInformation,SqlConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Closed)
                connection.Open();
            SqlTransaction sqlTransaction = connection.BeginTransaction();

            try
            {
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Validating the Employee {employeeInformation?.employeeBasicDetails?[0].EmployeeNumber} begins");

                ValidateData(employeeInformation);

                LoggingManager.LogMessage(typeof(ConnectionManager), $"Validating the Employee {employeeInformation?.employeeBasicDetails?[0].EmployeeNumber} ends");

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = sqlTransaction;

                #region Create CandNo            
                string selectQuery = string.Empty;

                if (prefixValuePair.Count == 0)
                {
                    selectQuery = ("SELECT Prefix,LastValue FROM SequenceMaster WHERE Prefix IN ('APP','EMP','SWR')");
                    command.CommandText = selectQuery;
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        {
                        while (dataReader.Read())
                            if (!prefixValuePair.ContainsKey(dataReader[0].ToString()))
                                prefixValuePair.Add(dataReader[0].ToString(), dataReader[1].ToString());
                        }
                    }
                    dataReader.Close();
                }

                selectQuery = string.Format(@"SELECT SequenceMaster.Prefix FROM SequenceMaster WHERE Head = '{0}'", employeeInformation?.employeeBasicDetails[0]?.WorkLocation);
                command.CommandText = selectQuery;
                string seqPrefix = command.ExecuteScalar().ToString();                

                string lastValue = ("000000" + prefixValuePair["APP"]);
                lastValue = lastValue.Substring(lastValue.Length - 6);

                int newValue = (int.Parse(lastValue) + 1);
                string CandNo = seqPrefix + "-APP" + newValue;
                prefixValuePair["APP"] = newValue.ToString();

                command.CommandText = string.Format("UPDATE SequenceMaster SET LastValue = {0}  WHERE Prefix = 'APP'", newValue);
                command.ExecuteNonQuery();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Created the Candidate number {CandNo}");
                #endregion

                #region Create EmpId            
                lastValue = ("000000" + prefixValuePair["EMP"]);
                lastValue = lastValue.Substring(lastValue.Length - 6);

                newValue = (int.Parse(lastValue) + 1);
                string EMPID = seqPrefix + "-EMP" + newValue;
                prefixValuePair["EMP"] = newValue.ToString();

                command.CommandText = string.Format("UPDATE SequenceMaster SET LastValue = {0}  WHERE Prefix = 'EMP'", newValue);
                command.ExecuteNonQuery();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Created the Employee Id {EMPID}");
                #endregion

                #region Create SWRID
                lastValue = ("000000" + prefixValuePair["SWR"]);
                lastValue = lastValue.Substring(lastValue.Length - 6);

                newValue = (int.Parse(lastValue) + 1);
                string SWRID = seqPrefix + "-SWR" + newValue;
                prefixValuePair["SWR"] = newValue.ToString();

                command.CommandText = string.Format("UPDATE SequenceMaster SET LastValue = {0}  WHERE Prefix = 'SWR'", newValue);
                command.ExecuteNonQuery();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Created the SWR ID {SWRID}");

                #endregion

                #region Insert data into CandidatePersonalDetails
                string insertQuery = @"INSERT INTO CandidatePersonalDetails(CandNo, ApplicationDate, CandName, ApplicationType, PostAppliedFor, CandSurName, CandFirstName, CandMiddleName, 
                CandPresAddress, CandPermAddress, CandNationaity, CandState, CandDob, MobileNo, Email, CandTelNo, CandPlaceofBirth, Sex,CandMaritalStatus, CandDependents, Father,
                CandFathHusbName, Profession, BandType, Reference, CandMotherName, isSelected, isCancel, CityID, CandPresentCity, CandCast, Remarks, HQual, CreatedBy
                , CreatedOn, ModifiedBy, ModifiedOn, OldRegNo, RegNo, BranchCode, ESICNumber, RefereeRegNo, UANNo, IsActive, UnitCode, AadharNo, IsChargableUniform,CandReligion)
                 VALUES
                (@CandNo, @ApplicationDate, @CandName, @ApplicationType, @PostAppliedFor, @CandSurName, @CandFirstName, @CandMiddleName, @CandPresAddress, @CandPermAddress
                , @CandNationaity, @CandState, @CandDob, @MobileNo, @Email, @CandTelNo, @CandPlaceofBirth,@Sex, @CandMaritalStatus,@CandDependents, @Father, @CandFathHusbName, @Profession,
                @BandType, @Reference,@CandMotherName, @isSelected,@isCancel, @CityID, @CandPresentCity, @CandCast, @Remarks, @HQual, @CreatedBy, @CreatedOn,
                @ModifiedBy, @ModifiedOn, @OldRegNo, @RegNo, @BranchCode, @ESICNumber, @RefereeRegNo, @UANNo, @IsActive, @UnitCode, @AadharNo, @IsChargableUniform,@CandReligion)";

                command.Parameters.AddWithValue("@CandNo", CandNo);
                command.Parameters.AddWithValue("@ApplicationDate", DateTime.Now);
                command.Parameters.AddWithValue("@CandName", (employeeInformation?.employeeBasicDetails?[0].Name + " " + employeeInformation?.employeeBasicDetails?[0].MiddleName + " " + employeeInformation?.employeeBasicDetails?[0].LastName).ToUpper());
                command.Parameters.AddWithValue("@ApplicationType","SGTOSSI");
                command.Parameters.AddWithValue("@PostAppliedFor", employeeInformation?.additionalDetails?.Designation);
                command.Parameters.AddWithValue("@CandSurName", string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0].LastName) ? string.Empty : employeeInformation?.employeeBasicDetails?[0].LastName.ToUpper());
                command.Parameters.AddWithValue("@CandFirstName", string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0].Name) ? string.Empty: employeeInformation?.employeeBasicDetails?[0].Name.ToUpper());
                command.Parameters.AddWithValue("@CandMiddleName", string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0].MiddleName) ? string.Empty : employeeInformation?.employeeBasicDetails?[0].MiddleName.ToUpper());
                command.Parameters.AddWithValue("@CandPresAddress", employeeInformation?.currentAddress?.Address1 + " " + employeeInformation?.currentAddress?.Address2 + " " + employeeInformation?.currentAddress?.District + " " + employeeInformation?.currentAddress?.PostalCode);
                command.Parameters.AddWithValue("@CandPermAddress", employeeInformation?.permanentAddress?.Address1);// + " " + employeeInformation?.permanentAddress?.Address2 + " " + employeeInformation?.permanentAddress?.City + " " + employeeInformation?.permanentAddress?.District + " " + employeeInformation?.permanentAddress?.State + " " + employeeInformation?.permanentAddress?.PostalCode);
                command.Parameters.AddWithValue("@CandNationaity", employeeInformation?.currentAddress?.Nationality);
                command.Parameters.AddWithValue("@CandState", employeeInformation?.currentAddress.State);
                command.Parameters.AddWithValue("@CandDob", DateTime.ParseExact(employeeInformation?.employeeBasicDetails?[0].DateofBirth,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@MobileNo", employeeInformation?.employeeBasicDetails?[0].MobileNo);
                command.Parameters.AddWithValue("@Email", employeeInformation?.employeeBasicDetails?[0].Email);
                command.Parameters.AddWithValue("@CandTelNo", employeeInformation?.employeeBasicDetails?[0].HomePhone);
                command.Parameters.AddWithValue("@CandPlaceofBirth", employeeInformation?.additionalDetails?.candidatePlaceOfBirth);
                command.Parameters.AddWithValue("@Sex", employeeInformation?.employeeBasicDetails?[0].Gender);
                command.Parameters.AddWithValue("@CandMaritalStatus", employeeInformation?.employeeBasicDetails?[0].MaritalStatus);
                command.Parameters.AddWithValue("@CandDependents", DBNull.Value);
                command.Parameters.AddWithValue("@Father", employeeInformation?.employeeBasicDetails?[0]?.MaritalStatus.ToUpper() == "MARRIED" ? true : false);

                if (!string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0].SpouseName))
                    command.Parameters.AddWithValue("@CandFathHusbName", employeeInformation?.employeeBasicDetails?[0].SpouseName);
                else
                    command.Parameters.AddWithValue("@CandFathHusbName", employeeInformation?.employeeBasicDetails?[0].FatherName);

                command.Parameters.AddWithValue("@Profession", DBNull.Value);
                command.Parameters.AddWithValue("@BandType", employeeInformation?.additionalDetails?.Position);
                command.Parameters.AddWithValue("@Reference", DBNull.Value);
                command.Parameters.AddWithValue("@CandMotherName", employeeInformation?.employeeBasicDetails?[0].MotherName);
                command.Parameters.AddWithValue("@isSelected", 0);
                command.Parameters.AddWithValue("@isCancel", 0);
                command.Parameters.AddWithValue("@CityID", employeeInformation?.currentAddress.City);
                command.Parameters.AddWithValue("@CandPresentCity",employeeInformation?.currentAddress.City);
                command.Parameters.AddWithValue("@CandCast", employeeInformation?.additionalDetails?.Caste);
                command.Parameters.AddWithValue("@Remarks", DBNull.Value);
                command.Parameters.AddWithValue("@HQual", employeeInformation?.additionalDetails?.HQualification);
                command.Parameters.AddWithValue("@CreatedBy", employeeInformation?.employeeBasicDetails?[0]?.CreateUser);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.ParseExact(employeeInformation?.employeeBasicDetails?[0]?.CreatedDate,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
                command.Parameters.AddWithValue("@ModifiedOn", DBNull.Value);
                command.Parameters.AddWithValue("@OldRegNo", DBNull.Value);
                command.Parameters.AddWithValue("@RegNo", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                command.Parameters.AddWithValue("@BranchCode", employeeInformation?.employeeBasicDetails?[0].WorkLocation);
                command.Parameters.AddWithValue("@ESICNumber", employeeInformation?.employeeBasicDetails?[0].EsiNo);
                command.Parameters.AddWithValue("@RefereeRegNo", DBNull.Value);
                command.Parameters.AddWithValue("@UANNo", employeeInformation?.PFDetails?.uanNumber);
                command.Parameters.AddWithValue("@IsActive", DBNull.Value);
                command.Parameters.AddWithValue("@UnitCode", employeeInformation?.employeeBasicDetails?[0].Customer);
                command.Parameters.AddWithValue("@AadharNo", employeeInformation?.employeeBasicDetails?[0].AadharNo);
                command.Parameters.AddWithValue("@IsChargableUniform", DBNull.Value);
                command.Parameters.AddWithValue("@CandReligion", employeeInformation?.additionalDetails?.Religion);

                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate Personal Details has been created");

                #endregion

                selectQuery = string.Format("SELECT BasicPrice FROM ServiceMaster WHERE ServiceCode = '{0}'", employeeInformation?.additionalDetails?.Designation);
                command.CommandText = selectQuery;
                object value = command.ExecuteScalar();

                decimal basePrice = value == null ? 0 : decimal.Parse(value.ToString());

                #region Insert data into EmployeeMaster
                insertQuery = @"INSERT INTO EmployeeMaster(EmpID,CandNo,BranchEmpCode,CardNo,Name,Department,IsExecutive,Designation,Grade,BranchCode,Position,HierarchyCode,PresAddress,
                PresCity,PermAddress,PermCity,PhoneNo,PinCode,Email,Mobile,PFNo,PFDate,ESICNo,PanNo,Married,DoB,JoiningDt,Sex,FatherName,MotherName,BankAcNo,BankName,Dispensary,HQualification,BloodGroup,ESICSubCodeLocation,GrossSalary,FixSalary)
                VALUES
                (@EmpID,@CandNo,@BranchEmpCode,@CardNo,@Name,@Department,@IsExecutive,@Designation,@Grade,@BranchCode,@Position,@HierarchyCode,@PresAddress,@PresCity,@PermAddress,@PermCity,
                @PhoneNo,@PinCode,@Email,@Mobile,@PFNo,@PFDate,@ESICNo,@PanNo,@Married,@DoB,@JoiningDt,@Sex,@FatherName,@MotherName,@BankAcNo,@BankName,@Dispensary,@HQualification,@BloodGroup,@ESICSubCodeLocation,@GrossSalary,@FixSalary)";

                command.Parameters.AddWithValue("@EmpID", EMPID);
                command.Parameters.AddWithValue("@CandNo", CandNo);
                command.Parameters.AddWithValue("@BranchEmpCode", employeeInformation?.employeeBasicDetails[0]?.EmployeeNumber);
                command.Parameters.AddWithValue("@CardNo", 0);
                command.Parameters.AddWithValue("@Name", (employeeInformation?.employeeBasicDetails[0]?.Name + " " + employeeInformation?.employeeBasicDetails[0]?.MiddleName + " " + employeeInformation?.employeeBasicDetails?[0].LastName).ToUpper());
                command.Parameters.AddWithValue("@Department", employeeInformation?.additionalDetails?.Department);
                command.Parameters.AddWithValue("@IsExecutive", "SGTOSSI");
                command.Parameters.AddWithValue("@Designation", employeeInformation?.additionalDetails?.Designation);
                command.Parameters.AddWithValue("@Grade", "SG000082");
                command.Parameters.AddWithValue("@BranchCode", employeeInformation?.employeeBasicDetails[0]?.WorkLocation);
                command.Parameters.AddWithValue("@Position", employeeInformation?.additionalDetails?.Position);
                command.Parameters.AddWithValue("@HierarchyCode", employeeInformation?.additionalDetails?.Position);
                command.Parameters.AddWithValue("@PresAddress", employeeInformation?.currentAddress?.Address1 + " " + employeeInformation?.currentAddress?.Address2 + " " + employeeInformation?.currentAddress?.District + " " +  employeeInformation?.currentAddress?.PostalCode);
                command.Parameters.AddWithValue("@PresCity", employeeInformation?.currentAddress.City);
                command.Parameters.AddWithValue("@PermAddress", employeeInformation?.permanentAddress?.Address1);// + " " + employeeInformation?.permanentAddress?.Address2 + " " + employeeInformation?.permanentAddress?.City + " " + employeeInformation?.permanentAddress?.District + " " + employeeInformation?.permanentAddress?.State + " " + employeeInformation?.permanentAddress?.PostalCode);
                command.Parameters.AddWithValue("@PermCity", employeeInformation?.permanentAddress?.City);
                command.Parameters.AddWithValue("@PhoneNo", employeeInformation?.employeeBasicDetails?[0]?.HomePhone);
                command.Parameters.AddWithValue("@PinCode", employeeInformation?.currentAddress?.PostalCode);
                command.Parameters.AddWithValue("@Email", employeeInformation?.employeeBasicDetails?[0]?.Email);
                command.Parameters.AddWithValue("@Mobile", employeeInformation?.employeeBasicDetails?[0]?.MobileNo);
                string pfNumber = employeeInformation?.PFDetails?.pfNumber;
                command.Parameters.AddWithValue("@PFNo", pfNumber == "" ? "0" : pfNumber.Substring(pfNumber.Length - 5));
                command.Parameters.AddWithValue("@PFDate", DBNull.Value);
                command.Parameters.AddWithValue("@ESICNo", employeeInformation?.employeeBasicDetails?[0]?.EsiNo);
                command.Parameters.AddWithValue("@PanNo", employeeInformation?.bankAccountDetails?.PANNumber);
                command.Parameters.AddWithValue("@Married", employeeInformation?.employeeBasicDetails?[0]?.MaritalStatus);
                command.Parameters.AddWithValue("@DoB", DateTime.ParseExact(employeeInformation?.employeeBasicDetails?[0]?.DateofBirth,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@JoiningDt", DateTime.ParseExact(employeeInformation?.additionalDetails?.JoiningDate,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@Sex", employeeInformation?.employeeBasicDetails?[0]?.Gender);
                command.Parameters.AddWithValue("@FatherName", employeeInformation?.employeeBasicDetails?[0]?.FatherName);
                command.Parameters.AddWithValue("@MotherName", employeeInformation?.employeeBasicDetails?[0]?.MotherName);
                command.Parameters.AddWithValue("@BankAcNo", employeeInformation?.bankAccountDetails?.BankAccountNumber);
                command.Parameters.AddWithValue("@BankName", employeeInformation?.bankAccountDetails?.BankName);
                command.Parameters.AddWithValue("@Dispensary", employeeInformation?.esiInformation?.Dispensary);
                command.Parameters.AddWithValue("@HQualification", employeeInformation?.additionalDetails?.HQualification);
                command.Parameters.AddWithValue("@BloodGroup", employeeInformation?.employeeBasicDetails?[0]?.BloodGroup);
                command.Parameters.AddWithValue("@ESICSubCodeLocation", employeeInformation?.esiInformation?.ESICSubCodeLocation);
                command.Parameters.AddWithValue("@GrossSalary", basePrice);
                command.Parameters.AddWithValue("@FixSalary", basePrice);

                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate records have been inserted in Employee Master");

                #endregion

                #region Insert data into Employee Entitlement


                GetSchema();

                DataRow row;

                for (int counter = 1; counter < 9; counter++)
                {
                    row = employeeEntitlementTable.NewRow();
                    row["EmpCode"] = EMPID;
                    row["Sno"] = counter;
                    row["SalHead"] = entitlementSalHead[counter - 1];
                    row["IsEditable"] = (counter == 5 || counter == 7) ? true : false;
                    row["Entitle"] = null;
                    row["Changedddate"] = null;
                    row["Remarks"] = "N.A";
                    row["EntCatg"] = "SCD000001";
                    row["Type"] = null;
                    row["Deduction"] = null;
                    row["LedgerCode"] = null;
                    row["EmployeeSuperSeded"] = counter == 4 ? true : false;
                    row["UnitSuperSeded"] = counter == 4 ? false : true;
                    row["OfficeOrderNo"] = string.Empty;
                    row["Edate"] = DateTime.Now;
                    row["HeadDescription"] = entitlementList[counter - 1];
                    row["EarnDeduction"] = (counter == 4 || counter == 5 || counter == 7) ? "Deduction" : "Earning";
                    row["PercentageEarnDeduction"] = 0.0;
                    row["OfSalaryHead"] = 0;
                    row["isBasic"] = counter == 1 ? true : false;
                    row["InsalarySlip"] = counter == 4 ? "1" : "0";
                    row["WEFDate"] = DateTime.Now;
                    row["AsperRule"] = (counter == 5 || counter == 7) ? true : false;
                    row["FormulaId"] = 0;
                    row["EntitlementHead"] = null;
                    row["FixedAmount"] = counter == 1 ? Convert.ToDecimal(string.Format("{0:0.00}", basePrice)) : Convert.ToDecimal(string.Format("{0:0.00}", 0));
                    row["FinalAmount"] = counter == 1 ? Convert.ToDecimal(string.Format("{0:0.00}", basePrice)) : Convert.ToDecimal(string.Format("{0:0.00}", 0));
                    row["AmountEarningDeduction"] = counter == 1 ? Convert.ToDecimal(string.Format("{0:0.000}", basePrice)) : Convert.ToDecimal(string.Format("{0:0.000}", 0));
                    row["Stop_Entitlement"] = false;
                    employeeEntitlementTable.Rows.Add(row);
                }
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Employee entitlement data has been created ");

                #endregion

                #region Insert data into Candidate Photo
                insertQuery = @"INSERT INTO CandidatePhoto(CandNo,Image,OldCode,ImagePath,UploadDocumentPath,UploadBankDocumentPath,UploladPFDocumentPath)
                                VALUES (@CandNo,@Image,@OldCode,@ImagePath,@UploadDocumentPath,@UploadBankDocumentPath,@UploladPFDocumentPath)";                
                command.Parameters.AddWithValue("@CandNo", CandNo);
                command.Parameters.AddWithValue("@Image", Convert.FromBase64String(employeeInformation?.employeeBasicDetails?[0]?.EmployeePhoto));
                command.Parameters.AddWithValue("@OldCode", DBNull.Value);
                command.Parameters.AddWithValue("@ImagePath", ConvertintoImage(employeeInformation?.employeeBasicDetails?[0]?.EmployeePhoto,CandNo,employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber));
                command.Parameters.AddWithValue("@UploadDocumentPath",ConvertintoDocument(employeeInformation?.familyDetails?.FamilyDetailsDocument,CandNo + "_FamilyDoc", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber));
                command.Parameters.AddWithValue("@UploadBankDocumentPath", ConvertintoDocument(employeeInformation?.bankAccountDetails?.BankDocument, CandNo + "_BankDoc", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber));
                command.Parameters.AddWithValue("@UploladPFDocumentPath",ConvertintoDocument(employeeInformation?.PFDetails?.PFDocUpload, CandNo + "_PFDoc", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber));
                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate Photo have been created");

                #endregion

                #region Insert data into CandidateNomineeDetails
                insertQuery = @"INSERT INTO EmployeeNomineeDetails(RegNo,NomineeName,RelationshipWithEmployee,Gender,DateofBirth,MobileNo,AddressAsPerAadhar,Address)
                VALUES (@RegNo,@NomineeName,@RelationshipWithEmployee,@Gender,@DateofBirth,@MobileNo,@AddressAsPerAadhar,@Address)";
                command.Parameters.AddWithValue("@RegNo", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                command.Parameters.AddWithValue("@NomineeName", employeeInformation?.nomineeInformation?.Name);
                command.Parameters.AddWithValue("@RelationshipWithEmployee", string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.RelationshipwithEmployees) ? string.Empty : employeeInformation?.nomineeInformation?.RelationshipwithEmployees);
                command.Parameters.AddWithValue("@Gender", employeeInformation?.nomineeInformation?.Gender);
                command.Parameters.AddWithValue("@DateofBirth", DateTime.ParseExact(employeeInformation?.nomineeInformation?.DOB,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@MobileNo", employeeInformation?.nomineeInformation?.MobileNo);
                command.Parameters.AddWithValue("@AddressAsPerAadhar", employeeInformation?.nomineeInformation?.AddressasPerAadhar == null ? "" : employeeInformation?.nomineeInformation?.AddressasPerAadhar);
                if (employeeInformation?.nomineeInformation?.AddressasPerAadhar == "Yes")
                {
                    command.Parameters.AddWithValue("@Address", employeeInformation?.permanentAddress?.Address1 + " " + employeeInformation?.permanentAddress?.Address2 + " " + employeeInformation?.permanentAddress?.City + " " + employeeInformation?.permanentAddress?.District + " " + employeeInformation?.permanentAddress?.State + " " + employeeInformation?.permanentAddress?.PostalCode);
                }
                else
                {
                    command.Parameters.AddWithValue("@Address", employeeInformation?.nomineeInformation?.Address);
                }
                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate Nominee Details have been created");
                #endregion

                #region Insert data into BankDetails
                insertQuery = @"INSERT INTO EmployeeBankDetail(SisBranchCode,Date,BankCode,EmpRegNo,AcNo,ModifiedBy,ModifiedOn,CardNo,EmpCode,ActivationCharge,Active,IFSCCode,BankName)
                VALUES (@SisBranchCode,@Date,@BankCode,@EmpRegNo,@AcNo,@ModifiedBy,@ModifiedOn,@CardNo,@EmpCode,@ActivationCharge,@Active,@IFSCCode,@BankName)";
                command.Parameters.AddWithValue("@SisBranchCode", "BR000088"); 
                command.Parameters.AddWithValue("@Date", DateTime.Now);
                command.Parameters.AddWithValue("@BankCode", "CFM-BNK000090");
                command.Parameters.AddWithValue("@EmpRegNo", employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber);
                command.Parameters.AddWithValue("@AcNo", employeeInformation?.bankAccountDetails?.BankAccountNumber);
                command.Parameters.AddWithValue("@ModifiedBy", employeeInformation?.employeeBasicDetails?[0]?.CreateUser);
                command.Parameters.AddWithValue("@ModifiedOn", DateTime.Now);
                command.Parameters.AddWithValue("@CardNo", string.Empty);
                command.Parameters.AddWithValue("@EmpCode", EMPID);
                command.Parameters.AddWithValue("@ActivationCharge", 1);
                command.Parameters.AddWithValue("@Active", 1);
                command.Parameters.AddWithValue("@IFSCCode", employeeInformation?.bankAccountDetails?.IFSCCode);
                command.Parameters.AddWithValue("@BankName", employeeInformation?.bankAccountDetails?.BankName);
                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate Bank Details have been created");

                #endregion

                #region Insert data into SEWARegistration
                insertQuery = @"INSERT INTO SEWARegistration (SEWACode,MemDate,CandidateNumber,RegNo,OldRegNo,RankOrBandPromotionDate,CategoryCode,Remarks,WtRegNo,RegFee,Relationship,
                    MonthlySubFee,NomineeName,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,Band,IsActive)
                    VALUES(@SEWACode,@MemDate,@CandidateNumber,@RegNo,@OldRegNo,@RankOrBandPromotionDate,@CategoryCode,@Remarks,@WtRegNo,@RegFee,@Relationship,
                    @MonthlySubFee,@NomineeName,@CreatedBy,@CreatedOn,@ModifiedBy,@ModifiedOn,@Band,@IsActive)";
                command.Parameters.AddWithValue("@SEWACode",SWRID);
                command.Parameters.AddWithValue("@MemDate",DateTime.Now);
                command.Parameters.AddWithValue("@CandidateNumber",DBNull.Value);
                command.Parameters.AddWithValue("@RegNo", EMPID);
                command.Parameters.AddWithValue("@OldRegNo",DBNull.Value );
                command.Parameters.AddWithValue("@RankOrBandPromotionDate",DateTime.Now );
                command.Parameters.AddWithValue("@CategoryCode", "118112");
                command.Parameters.AddWithValue("@Remarks","N.A");
                command.Parameters.AddWithValue("@WtRegNo", "N.A");
                command.Parameters.AddWithValue("@RegFee",20);
                command.Parameters.AddWithValue("@Relationship",DBNull.Value );
                command.Parameters.AddWithValue("@MonthlySubFee",20);
                command.Parameters.AddWithValue("@NomineeName",DBNull.Value );
                command.Parameters.AddWithValue("@CreatedBy", employeeInformation?.employeeBasicDetails?[0]?.CreateUser);
                command.Parameters.AddWithValue("@CreatedOn",DateTime.Now);
                command.Parameters.AddWithValue("@ModifiedBy", employeeInformation?.employeeBasicDetails?[0]?.CreateUser);
                command.Parameters.AddWithValue("@ModifiedOn",DateTime.Now );
                command.Parameters.AddWithValue("@Band",DBNull.Value );
                command.Parameters.AddWithValue("@IsActive", 1);
                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                LoggingManager.LogMessage(typeof(ConnectionManager), $"Candidate SEWA Registeration has been created");

                #endregion

                sqlTransaction.Commit();

                InsertData(employeeEntitlementTable);
                employeeEntitlementTable.Rows.Clear();

                LoggingManager.LogMessage(typeof(ConnectionManager), "Employee integrated into ERP: " + employeeInformation?.employeeBasicDetails?[0].EmployeeNumber);
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                LoggingManager.LogMessage(typeof(ConnectionManager), "Employee Number : " + employeeInformation?.employeeBasicDetails?[0].EmployeeNumber);
                LoggingManager.LogException(typeof(ConnectionManager), ex);
            }
        }

        public static void ValidateData(BTEmployeeInformation employeeInformation)
        {
            try
            {
                int result;

                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber))
                    throw new Exception("Employee Number is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.Name))
                    throw new Exception("Name is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.Department))
                    throw new Exception("Department is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.Designation))
                    throw new Exception("Designation is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.WorkLocation))
                    throw new Exception("Branch Code is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.Position))
                    throw new Exception("Position is missing");               
                if (string.IsNullOrEmpty(employeeInformation?.currentAddress?.City))
                    throw new Exception("Present City is missing");
                if (!int.TryParse(employeeInformation?.currentAddress.City, out result))
                    throw new Exception("Present City is not number");               
                if (string.IsNullOrEmpty(employeeInformation?.permanentAddress?.City))
                    throw new Exception("Permanent City is missing");
                if (!int.TryParse(employeeInformation?.permanentAddress.City, out result))
                    throw new Exception("Permanent City is not number");
                if (string.IsNullOrEmpty(employeeInformation?.currentAddress?.PostalCode))
                    throw new Exception("Pin Code is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.MobileNo))
                    throw new Exception("Mobile number is missing");               
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.MaritalStatus))
                    throw new Exception("Marital status is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.DateofBirth))
                    throw new Exception("Date of birth is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.JoiningDate))
                    throw new Exception("Joining Date is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.Gender))
                    throw new Exception("Gender is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.FatherName))
                    throw new Exception("Father name is missing");              
                if (string.IsNullOrEmpty(employeeInformation?.bankAccountDetails?.BankAccountNumber))
                    throw new Exception("Bank account number is missing");
                if (string.IsNullOrEmpty(employeeInformation?.bankAccountDetails?.IFSCCode))
                    throw new Exception("Bank IFSC code is missing");
                if (string.IsNullOrEmpty(employeeInformation?.bankAccountDetails?.BankName))
                    throw new Exception("Bank name is missing");
                if (string.IsNullOrEmpty(employeeInformation?.esiInformation?.ESICSubCodeLocation))
                    throw new Exception("ESIC subcode location is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.HQualification))
                    throw new Exception("HQualification is missing");
                if (string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.Name))
                    throw new Exception("Nominee name is missing");
                if (string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.RelationshipwithEmployees))
                    throw new Exception("Relationship with employee is missing");
                if (string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.Gender))
                    throw new Exception("Nominee Gender is missing");
                if (string.IsNullOrEmpty(employeeInformation?.nomineeInformation?.DOB))
                    throw new Exception("Nominee date of birth is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.BloodGroup))
                    throw new Exception("Blood group is missing");
                if (string.IsNullOrEmpty(employeeInformation?.currentAddress?.Nationality))
                    throw new Exception("Nationality is missing");               
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.Customer))
                    throw new Exception("Unit code is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.AadharNo))
                    throw new Exception("Aadhar number is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.CreateUser))
                    throw new Exception("Created By is missing");
                if (string.IsNullOrEmpty(employeeInformation?.employeeBasicDetails?[0]?.CreatedDate))
                    throw new Exception("Created Date is missing");
                if (string.IsNullOrEmpty(employeeInformation?.additionalDetails?.candidatePlaceOfBirth))
                    throw new Exception("Candidate place of birth is missing");
                if(employeeInformation?.additionalDetails?.Designation?.StartsWith("SER") == false)
                    throw new Exception("Designation is not a code");
            }

            catch(Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex,employeeInformation?.employeeBasicDetails?[0]?.EmployeeNumber + " " +  ex.Message);
                throw ex;
            }
        }

        public static void GetSchema()
        {
            if (employeeEntitlementTable == null)
            {
                employeeEntitlementTable = new DataTable("EmployeeEntitlement");
                employeeEntitlementTable.Columns.Add("EntitlementID");
                employeeEntitlementTable.Columns.Add("EmpCode");
                employeeEntitlementTable.Columns.Add("Sno");
                employeeEntitlementTable.Columns.Add("SalHead");
                employeeEntitlementTable.Columns.Add("IsEditable");
                employeeEntitlementTable.Columns.Add("Entitle");
                employeeEntitlementTable.Columns.Add("Changedddate");
                employeeEntitlementTable.Columns.Add("Remarks");
                employeeEntitlementTable.Columns.Add("EntCatg");
                employeeEntitlementTable.Columns.Add("Type");
                employeeEntitlementTable.Columns.Add("Deduction");
                employeeEntitlementTable.Columns.Add("LedgerCode");
                employeeEntitlementTable.Columns.Add("EmployeeSuperSeded");
                employeeEntitlementTable.Columns.Add("UnitSuperSeded");
                employeeEntitlementTable.Columns.Add("OfficeOrderNo");
                employeeEntitlementTable.Columns.Add("Edate");
                employeeEntitlementTable.Columns.Add("HeadDescription");
                employeeEntitlementTable.Columns.Add("EarnDeduction");
                employeeEntitlementTable.Columns.Add("PercentageEarnDeduction");
                employeeEntitlementTable.Columns.Add("OfSalaryHead");
                employeeEntitlementTable.Columns.Add("isBasic");
                employeeEntitlementTable.Columns.Add("InsalarySlip");
                employeeEntitlementTable.Columns.Add("WEFDate");
                employeeEntitlementTable.Columns.Add("AsperRule");
                employeeEntitlementTable.Columns.Add("FormulaId");
                employeeEntitlementTable.Columns.Add("EntitlementHead");
                employeeEntitlementTable.Columns.Add("FixedAmount");
                employeeEntitlementTable.Columns.Add("FinalAmount");
                employeeEntitlementTable.Columns.Add("AmountEarningDeduction");
                employeeEntitlementTable.Columns.Add("Stop_Entitlement");
            }
        }
        
        public static void InsertData(DataSet dataSet, SqlConnection connection)
        {
            try
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        if (dataTable.Rows.Count == 0)
                            continue;
                        try
                        {
                            LoggingManager.LogMessage(typeof(ConnectionManager),$"Inserting data for table {dataTable.TableName}");
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connection))
                            {
                                bulkcopy.BulkCopyTimeout = 660;
                                bulkcopy.DestinationTableName = dataTable.TableName;
                                bulkcopy.WriteToServer(dataTable);
                                bulkcopy.Close();
                            }
                        }
                        catch(Exception e)
                        {
                            LoggingManager.LogException(typeof(ConnectionManager), e);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex);
            }

        }

        public static void InsertData(DataTable dataTable)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    try
                    {
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connection))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = dataTable.TableName;
                            bulkcopy.WriteToServer(dataTable);
                            bulkcopy.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        LoggingManager.LogException(typeof(ConnectionManager), e);
                    }

                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex);
            }

        }

        public static string ConvertintoDocument(string base64String,string fileName,string employeeNumber)
        {
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(base64String))
            {
                var encodedByte = System.Convert.FromBase64String(base64String);
                filePath = Path.Combine(_filePath, employeeNumber);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath = Path.Combine(filePath, fileName + ".pdf");                

                if (File.Exists(filePath))
                {                    
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, encodedByte);
            }
            return filePath;
        }

        public static string ConvertintoImage(string base64String, string fileName,string employeeNumber)
        {
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(base64String))
            {
                filePath = Path.Combine(_filePath, employeeNumber);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = Path.Combine(filePath, fileName + ".jpeg");
                var bytes = Convert.FromBase64String(base64String);
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
            }
            return filePath;
        }

        public static void UpdateDataintoERP(List<BTEmployeeESIInformation> employeeESIInformationList,SqlConnection connection)
        {
            try
            {
                if(employeeESIInformationList != null && employeeESIInformationList.Count > 0 )
                {
                    DataTable table = CreateBlueTreeTable();

                    foreach (BTEmployeeESIInformation employeeESIInformation in employeeESIInformationList)
                    {
                        if (IsEmployeeNumberExists(employeeESIInformation?.employeeEsiPfDetails?.employeeNumber, connection))
                        {
                            LoggingManager.LogMessage(typeof(ConnectionManager), $"Updating  the Employee {employeeESIInformation?.employeeEsiPfDetails?.employeeNumber} begins");

                            DataRow newRow = table.NewRow();
                            newRow[GlobalConstants.KAadharNo] = employeeESIInformation?.employeeEsiPfDetails?.aadhaarNo;
                            newRow["createdBy"] = employeeESIInformation?.employeeEsiPfDetails?.createdBy;
                            newRow["createdDate"] = employeeESIInformation?.employeeEsiPfDetails?.createdDate;
                            newRow["employeeNumber"] = employeeESIInformation?.employeeEsiPfDetails?.employeeNumber;
                            newRow["esiNo"] = employeeESIInformation?.employeeEsiPfDetails?.esiNo;
                            newRow["lastModifiedBy"] = employeeESIInformation?.employeeEsiPfDetails?.lastModifiedBy;
                            newRow["lastModifiedDate"] = employeeESIInformation?.employeeEsiPfDetails?.lastModifiedDate;
                            newRow["lastName"] = employeeESIInformation?.employeeEsiPfDetails?.lastName;
                            newRow["name"] = employeeESIInformation?.employeeEsiPfDetails?.name;
                            newRow["uanNo"] = employeeESIInformation?.employeeEsiPfDetails?.uanNo;
                            table.Rows.Add(newRow);

                            if (connection != null && connection.State == ConnectionState.Closed)
                                connection.Open();
                            SqlCommand command = new SqlCommand();
                            command.Connection = connection;

                            string updateQuery = @"UPDATE CandidatePersonalDetails SET ESICNumber = @ESICNumber,ModifiedBy = @ModifiedBy,ModifiedOn = @ModifiedOn,UANNO = @UANNO WHERE RegNo = 
                                                '" + employeeESIInformation?.employeeEsiPfDetails?.employeeNumber + "'";
                            command.Parameters.AddWithValue("@ESICNumber", employeeESIInformation?.employeeEsiPfDetails?.esiNo);
                            command.Parameters.AddWithValue("@ModifiedBy", employeeESIInformation?.employeeEsiPfDetails?.lastModifiedBy);
                            command.Parameters.AddWithValue("@ModifiedOn", DateTime.ParseExact(employeeESIInformation?.employeeEsiPfDetails?.lastModifiedDate,"dd-MM-yyyy",CultureInfo.InvariantCulture));
                            command.Parameters.AddWithValue("@UANNO", employeeESIInformation?.employeeEsiPfDetails?.uanNo);
                            command.CommandText = updateQuery;
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                            LoggingManager.LogMessage(typeof(ConnectionManager), $"Updating the Candidate Personal details");


                            updateQuery = @"UPDATE EmployeeMaster SET ESICNo = @ESICNo WHERE BranchEmpCode = '" + employeeESIInformation?.employeeEsiPfDetails?.employeeNumber + "'";
                            command.Parameters.AddWithValue("@ESICNo", employeeESIInformation?.employeeEsiPfDetails?.esiNo);                            
                            command.CommandText = updateQuery;
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                            LoggingManager.LogMessage(typeof(ConnectionManager), $"Updating the Employee Master");
                            LoggingManager.LogMessage(typeof(ConnectionManager), $"Updating  the Employee {employeeESIInformation?.employeeEsiPfDetails?.employeeNumber} ends");


                        }
                        else
                        {
                            LoggingManager.LogMessage(typeof(ConnectionManager), employeeESIInformation?.employeeEsiPfDetails?.employeeNumber + " Does not Exists");
                        }
                    }

                    InsertData(table);
                    table = null;
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex);
            }
        }

        private static DataTable CreateBlueTreeTable()
        {            
            DataTable table = new DataTable("BTESIPFDETAILS");
            table.Columns.Add(GlobalConstants.KAadharNo);
            table.Columns.Add("createdBy");
            table.Columns.Add("createdDate");
            table.Columns.Add("employeeNumber");
            table.Columns.Add("esiNo");
            table.Columns.Add("lastModifiedBy");
            table.Columns.Add("lastModifiedDate");
            table.Columns.Add("lastName");
            table.Columns.Add("name");
            table.Columns.Add("uanNo");            
            return table;
        }

        public static bool CheckDataforDate(DateTime dateToCheck)
        {
            try
            {
                int count = 0;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = @"SELECT COUNT(*) FROM CandidatePersonalDetails WHERE ApplicationDate >= '" + dateToCheck.ToString("yyyy-MM-dd 23:00:00") + "' and CandNo not like 'SMR%' and RefereeRegNo is null";
                    count = int.Parse(command.ExecuteScalar().ToString());
                }

                return count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(typeof(ConnectionManager), ex);
                return false;
            }
        }
    }
}