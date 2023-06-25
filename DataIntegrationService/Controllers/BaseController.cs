using DataIntegrationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DataIntegrationService.Controllers
{
    public class BaseController : ApiController
    {
        protected readonly BaseResponse _baseResponse;

        public BaseController()
        {
            _baseResponse = new BaseResponse();
        }
    }
}