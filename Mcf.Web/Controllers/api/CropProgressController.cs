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
    public class CropProgressController : ApiController
    {
        private ICropService cropservice;
        public CropProgressController(ICropService cropservice)
        {
            this.cropservice = cropservice;
        }

        [DisplayName("GetCropRawData")]
        public HttpResponseMessage GetCropRawData()
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, cropservice.GetRawData());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetCropFormattedData")]
        public HttpResponseMessage GetCropFormattedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, cropservice.GetCropFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        //[AcceptVerbs("GET", "POST")]
        //[ActionName("SaveGridData")]
        //public void SaveGridData(EthanolUpdateData updateData)
        //{
        //        cropservice.UpdateEthanolData(updateData);
        //}
    }
}
