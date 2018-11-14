using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using McF.Business;
using System.ComponentModel;
using McF.Contracts;

namespace McF.api
{
    public class JobController : ApiController
    {
        private IJobService jobservice;

        public JobController(IJobService jobservice)
        {
            this.jobservice = jobservice;
        }

        [HttpGet]
        public HttpResponseMessage GetCurrentJobInfo(string datasource)
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, jobservice.GetCurrentJobInfo(datasource));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [HttpGet]
        public HttpResponseMessage GetJobSummary()
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, jobservice.GetJobSummary());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }
    }
}
