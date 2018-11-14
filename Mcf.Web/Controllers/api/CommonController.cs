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
    public class CommonController : ApiController
    {
        private ICommonService commonservice;
        public CommonController(ICommonService commonservice)
        {
            this.commonservice = commonservice;
        }

        [DisplayName("GetAllDataSources")]
        public HttpResponseMessage GetAllDataSources()
        {  
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetAllDataSources());
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }
        [DisplayName("GetCompleteFormatData")]
        public HttpResponseMessage GetCompleteFormatData(string datasource, string rootsource, int index, string from, string to)
        {
            var responseMessage = new HttpResponseMessage();
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (!String.IsNullOrWhiteSpace(from))
                fromDate = Convert.ToDateTime(from);
            if (!String.IsNullOrWhiteSpace(to))
                toDate = Convert.ToDateTime(to);
            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetCompleteFormatData(datasource, rootsource, index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetFormattedData")]
        public HttpResponseMessage GetFormattedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetWASDEWorldFormattedData")]
        public HttpResponseMessage GetWASDEWorldFormattedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetWASDEWorldFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetWASDEDomesticFormattedData")]
        public HttpResponseMessage GetWASDEDomesticFormattedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetWASDEDomesticFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }
        [DisplayName("GetFOFormatedData")]
        public HttpResponseMessage GetFOFormatedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetFOFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetCocoaFormatedData")]
        public HttpResponseMessage GetCocoaFormatedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetCocoaFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetBroilerFormatedData")]
        public HttpResponseMessage GetBroilerFormatedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetBHFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("GetCattleFormatedData")]
        public HttpResponseMessage GetCattleFormatedData(int index, string from, string to)
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
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.GetCFFormatedData(index, fromDate, toDate));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }

        [DisplayName("SaveData")]
        public HttpResponseMessage SaveData(DSUpdatedData dsUpdatedData)
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.SaveData(dsUpdatedData));
            }
            catch (Exception ex)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ResultMessage(ex));
                throw new Exception(ex.Message, ex);
            }
            return responseMessage;
        }
        [DisplayName("SaveDataSource")]
        public HttpResponseMessage SaveDataSource(DataSource datasource)
        {
            var responseMessage = new HttpResponseMessage();

            try
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, commonservice.SaveDataSource(datasource));
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
