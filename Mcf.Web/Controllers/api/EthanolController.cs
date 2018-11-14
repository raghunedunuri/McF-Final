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
    public class EthanolController : ApiController
    {
        private IEthanolService ethanolservice;
        public EthanolController(IEthanolService ethanolservice)
        {
            this.ethanolservice = ethanolservice;
        }

        [DisplayName("GetEthanolRawData")]
        public HttpResponseMessage GetEthanolRawData()
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, ethanolservice.GetEthanolRawData());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetEthanolFormattedData")]
        public HttpResponseMessage GetEthanolFormattedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, ethanolservice.GetFormatedData(index, fromDate, toDate));
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
        public void SaveGridData(EthanolUpdateData updateData)
        {
                ethanolservice.UpdateEthanolData(updateData);
        }
    }
}
