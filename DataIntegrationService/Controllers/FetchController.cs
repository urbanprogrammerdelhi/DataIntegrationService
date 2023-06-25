using DataIntegrationService.Entities;
using DataIntegrationService.Helpers;
using DataIntegrationService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Mvc;

namespace DataIntegrationService.Controllers
{
    [AuthenticationFilter]
    public class FetchController : BaseController
    {
        private BaseResponse _base = new BaseResponse();
        public FetchController()
        {

        }

        [HttpGet]
        [Route("api/fetch/getservicedetails")]
        public List<ServiceMaster> GetServiceDetails()
        {
            string result = string.Empty;
            List<ServiceMaster> serviceMasterList = null;
            try
            {
                serviceMasterList = ConnectionManager.GetServiceMasterData();                
            }
            catch (Exception ex)
            {
                _base.Error(Request, ex.Message);
                throw ex;
            }

            return serviceMasterList;
        }

        [HttpGet]
        [Route("api/fetch/getbranchdetails")]
        public List<BranchMaster> GetBranchDetails()
        {
            string result = string.Empty;
            List<BranchMaster> branchMasterList = null;
            try
            {
                branchMasterList =  ConnectionManager.GetBranchMasterData();                
            }
            catch (Exception ex)
            {
                _base.Error(Request, ex.Message);
            }

            return branchMasterList;
        }

        [HttpGet]
        [Route("api/fetch/getunitdetails")]
        public List<UnitMaster> GetUnitDetails()
        {
            string result = string.Empty;
            List<UnitMaster> unitMasterList = null;
            try
            {
                unitMasterList = ConnectionManager.GetUnitMasterData();                
            }
            catch (Exception ex)
            {
                _base.Error(Request, ex.Message);
            }

            return unitMasterList;
        }

        [HttpGet]
        [Route("api/fetch/getcitydetails")]
        public List<CityMaster> GetCityDetails()
        {
            string result = string.Empty;
            List<CityMaster> cityMasterList = null;
            try
            {
                cityMasterList = ConnectionManager.GetCityMasterData();
            }
            catch (Exception ex)
            {
                _base.Error(Request, ex.Message);
            }

            return cityMasterList;
        }

        [HttpGet]
        [Route("api/fetch/getcastedetails")]
        public List<CasteMaster> GetCasteDetails()
        {
            string result = string.Empty;
            List<CasteMaster> casteMasterList = null;
            try
            {
                casteMasterList = ConnectionManager.GetCasteMasterData();
            }
            catch (Exception ex)
            {
                _base.Error(Request, ex.Message);
            }

            return casteMasterList;
        }

    }
}