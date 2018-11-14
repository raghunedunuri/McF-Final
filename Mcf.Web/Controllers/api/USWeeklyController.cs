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
    public class USWeeklyController : ApiController
    {
        private IUSWeeklyService usweeklyservice;
        public USWeeklyController(IUSWeeklyService usweeklyservice)
        {
            this.usweeklyservice = usweeklyservice;
        }

        [DisplayName("GetRawData")]
        public HttpResponseMessage GetRawData()
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, usweeklyservice.GetUSWeeklyRawData());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }


        [DisplayName("GetUSWeeklyFormattedData")]
        public HttpResponseMessage GetUSWeeklyFormattedData(string category, int index, string from, string to)
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (!String.IsNullOrWhiteSpace(from))
                fromDate = Convert.ToDateTime(from);
            if (!String.IsNullOrWhiteSpace(to))
                toDate = Convert.ToDateTime(to);
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, usweeklyservice.GetFormatedData(category, index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveGridData")]
        public void SaveGridData(USWeeklyUpdateData updateData)
        {
            usweeklyservice.UpdateUSWeeklyData(updateData);
        }
    }
}
