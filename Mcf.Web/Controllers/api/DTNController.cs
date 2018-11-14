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
    public class DTNController : ApiController
    {
        private IDTNService dtnservice;
        public DTNController(IDTNService dtnservice)
        {
            this.dtnservice = dtnservice;
        }

        [DisplayName("GetDTNFormatedData")]
        public HttpResponseMessage GetDTNFormatedData(int index,string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, dtnservice.GetDTNFormatedData(index, fromDate, toDate));
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
        public void SaveGridData(DTNUpdate updateData)
        {
            dtnservice.UpdateDTNData(updateData);
        }
    }
}
