using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pxweb.dtn.telvent.com;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DTNJob
{
    public class Attr
    {
        public string Description { get; set; }
        public string Market { get; set; }
        public string MarketName { get; set; }
        public string RequestedSymbol { get; set; }
        public string Status { get; set; }
        public string TickerSymbol { get; set; }
        public string Vendor { get; set; }
    }

    public class Datum
    {
        public string Close { get; set; }
        public string Date { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string OI { get; set; }
        public string Open { get; set; }
        public string Volume { get; set; }
    }

    public class DTNHist
    {
        public string Count { get; set; }
        public string DataSrc { get; set; }
        public string Oper { get; set; }
        public string Time { get; set; }
        public string UserID { get; set; }
        public Attr Attr { get; set; }
        public IList<Datum> Data { get; set; }
    }
    public class DTNJobRunner : IJobRunner
    {
        private IDTNService dtnService;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private DateTime ReportDateTime;
        private int NoofRecords;
        private Dictionary<string, string> JobParams;
        private string UserId;
        private string PassWord;

        private Dictionary <string, DTNData > DTNDialyData;

        public DTNJobRunner(IUnityContainer _unityContainer)
        {
            dtnService = _unityContainer.Resolve<DTNService>();
            jobService = _unityContainer.Resolve<JobService>();
            DTNDialyData = new Dictionary<string, DTNData>();
            NoofRecords = 0;
        }

        public bool RUNJob(Dictionary<string, string> jobParams, string dataSource, int jobID)
        {
            Logger.Info("DTN Job Started");
            JobStartTime = DateTime.Now;
            JobID = jobID;
            DataSource = dataSource;
            JobParams = jobParams;

            UserId = JobParams["UserID"];
            PassWord = JobParams["Password"];

            JobStatus jobStatus = jobService.GetCurrentJob(JobID);
            UpdateJobTime updateJobTime = new UpdateJobTime()
            {
                Id = JobID,
                startTime = JobStartTime,
                Status = "RUNNING",
                UserID = 0
            };
            jobService.UpdateJobStatus(updateJobTime);

            Logger.Info("DTN Job Update Job Status in ETL_JOBS_RUN");
            try
            {
                string ReportDate = DateTime.Now.ToShortDateString();
                if ( jobParams.ContainsKey("DATE"))
                    ReportDate = jobParams["DATE"];

                Logger.Info($"DTN Job started running with Date: {ReportDate}");

                ReportDateTime = Convert.ToDateTime(ReportDate);

                List<DTNSymbols> lstSymbols = dtnService.GetDTNSymbolInfo();
                if ( ReportDateTime < DateTime.Now )
                {
                    foreach (DTNSymbols dnSym in lstSymbols)
                    {
                        PopulateDTNDialyHistoryData(dnSym.DTNRoot);
                    }
                }
                foreach (DTNSymbols dnSym in lstSymbols)
                {
                    PopulateDTNDialyData(dnSym.DTNRoot);
                }
                List<DTNData> dtnData = new List<DTNData>();
                foreach (KeyValuePair<string, DTNData> kv in DTNDialyData)
                {
                    NoofRecords++;
                    dtnData.Add(kv.Value);
                }
                dtnService.PopulateDTNData(dtnData);
                updateJobTime.FilePath = $"UserID:{UserId};Password:{PassWord}";
                updateJobTime.FileType = "Data";
                updateJobTime.endTime = DateTime.Now;
                updateJobTime.Message = "Success";
                updateJobTime.Status = "Completed";
                updateJobTime.NoOfNewRecords = NoofRecords;
                jobService.UpdateJobStatus(updateJobTime);
            }
            catch(Exception ex)
            {
                updateJobTime.endTime = DateTime.Now;
                updateJobTime.Message = $"Error: {ex.Message}";
                updateJobTime.Status = "Completed";
                updateJobTime.NoOfNewRecords = NoofRecords;
                jobService.UpdateJobStatus(updateJobTime);
            }
            return true;
        }

        private void PopulateDTNDialyData(string symbol)
        {
            PXServiceDtoClient client = new PXServiceDtoClient("WsHttp");

            GetQuoteSnapDto request = new GetQuoteSnapDto()
            {
                UserID = UserId,
                Password = PassWord,
                Type = "F",
                Symbol = symbol + "`##",
                Format = "JSN"
            };
            string str = client.GetQuoteSnap(request);
            ParseSymbols(str, symbol);
            request = new GetQuoteSnapDto()
            {
                UserID = UserId,
                Password = PassWord,
                Type = "F",
                Symbol = symbol + "@c",
                Format = "JSN"
            };
            str = client.GetQuoteSnap(request);
            ParseSymbols(str, symbol);
            client.Close();
        }
        private void PopulateDTNDialyHistoryData(string symbol)
        {
            PXServiceDtoClient client = new PXServiceDtoClient("WsHttp");
            GetDailyHistoryDto request = new GetDailyHistoryDto()
            {
                UserID = UserId,
                Password = PassWord,
                Symbol = symbol + "`##",
                Format = "JSN"
            };
            string str = client.GetDailyHistory(request);
            ParseHistorySymbols(str, symbol);
            request = new GetDailyHistoryDto()
            {
                UserID = UserId,
                Password = PassWord,
                Symbol = symbol + "@c",
                Format = "JSN"
            };
            str = client.GetDailyHistory(request);
            ParseHistorySymbols(str, symbol);
            client.Close();
           
        }
        private double ConvertToDouble(object obj)
        {
            if (obj == null)
                return 0;

            double val = 0;
            if (Double.TryParse(obj.ToString(), out val))
                return val;
            return 0;
        }

        private int ConvertToInt(object obj)
        {
            if (obj == null)
                return 0;

            int val = 0;
            if (int.TryParse(obj.ToString(), out val))
                return val;
            return 0;
        }

        private DateTime? ConvertToDateTime(object obj)
        {
            if (obj == null)
                return null;

            return DateTime.ParseExact(obj.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        private void ParseSymbols( string response, string rootSymbol)
        {
            dynamic records = JArray.Parse(response);
            dynamic Header = records[0];

            int count = Convert.ToInt32(Header.Count);
            for (int i = 1; i <= count; i++)
            {
                dynamic DataObject = records[i];

                if (DataObject.Attr.Status == "0")
                {
                    DTNData dtnData = new DTNData();
                    dtnData.Symbol = DataObject.Attr.TickerSymbol.ToString();
                    dtnData.Date = Convert.ToDateTime(DataObject.Data.Date);
                    dtnData.ExpirationDate = ConvertToDateTime(DataObject.Data.ExpirationDate);
                    dtnData.RootSymbol = rootSymbol;
                    dtnData.IssueDescription = DataObject.Data.IssueDescription;
                    dtnData.Last = ConvertToDouble(DataObject.Data.Last);
                    dtnData.High = ConvertToDouble(DataObject.Data.High);
                    dtnData.Open = ConvertToDouble(DataObject.Data.Open);
                    dtnData.Close = ConvertToDouble(DataObject.Data.Close);
                    dtnData.Low = ConvertToDouble(DataObject.Data.Low);
                    dtnData.Previous = ConvertToDouble(DataObject.Data.Previous);
                    dtnData.OpenInterest = ConvertToDouble(DataObject.Data.OpenInterest);
                    dtnData.Change = ConvertToDouble(DataObject.Data.Change);
                    dtnData.Volatility = ConvertToDouble(DataObject.Data.Volatility);
                    dtnData.VWAP = ConvertToDouble(DataObject.Data.VWAP);
                    dtnData.BidAskSpreadAvg = ConvertToDouble(DataObject.Data.BidAskSpreadAvg);
                    dtnData.SettlementPrice = ConvertToDouble(DataObject.Data.SettlementPrice);
                    dtnData.LTrade = ConvertToDouble(DataObject.Data.LTrade);
                    dtnData.CntrLow = ConvertToDouble(DataObject.Data.CntrLow);
                    dtnData.CntrHigh = ConvertToDouble(DataObject.Data.CntrHigh);
                    dtnData.Volume = ConvertToInt(DataObject.Data.Volume);
                    dtnData.CumVolume = ConvertToInt(DataObject.Data.CumVolume);
                    dtnData.TradeCount = ConvertToInt(DataObject.Data.TradeCount);
                    dtnData.DaystoExpiration = ConvertToInt(DataObject.Data.DaystoExpiration);
                    string key = $"{dtnData.Symbol}{dtnData.Date.ToString()}";
                    DTNDialyData[key] = dtnData;
                }
            }
        }

        private void ParseHistorySymbols(string response, string rootSymbol)
        {
            dynamic records = JArray.Parse(response);
            dynamic Header = records[0];

            int count = Convert.ToInt32(Header.Count);
            for (int i = 1; i <= count; i++)
            {
                dynamic DataObject = records[i];

                if (DataObject.Attr.Status == "0")
                {
                    foreach ( var item in DataObject.Data.Children())
                    {
                        DateTime dt = Convert.ToDateTime(item.Date);
                        if (dt < ReportDateTime)
                            continue;

                        DTNData dtnData = new DTNData();
                        dtnData.Symbol = DataObject.Attr.TickerSymbol.ToString();
                        dtnData.IssueDescription = DataObject.Attr.Description.ToString();
                        dtnData.Date = Convert.ToDateTime(item.Date);
                        dtnData.RootSymbol = rootSymbol;
                        dtnData.Last = 0;
                        dtnData.High = ConvertToDouble(item.High);
                        dtnData.Close = ConvertToDouble(item.Close);
                        dtnData.Open = ConvertToDouble(item.Open);
                        dtnData.Low = ConvertToDouble(item.Low);
                        dtnData.Previous = 0;
                        dtnData.OpenInterest = ConvertToDouble(item.OI);
                        dtnData.Change = 0;
                        dtnData.Volatility = 0;
                        dtnData.VWAP = 0;
                        dtnData.BidAskSpreadAvg = 0;
                        dtnData.SettlementPrice = 0;
                        dtnData.LTrade = 0;
                        dtnData.CntrLow = 0;
                        dtnData.CntrHigh = 0;
                        dtnData.Volume = ConvertToInt(item.Volume);
                        dtnData.CumVolume = 0;
                        dtnData.TradeCount = 0;
                        dtnData.DaystoExpiration = 0;
                        dtnData.ExpirationDate = JobStartTime;
                        string key = $"{dtnData.Symbol}{dtnData.Date.ToString()}";
                        DTNDialyData[key] = dtnData;
                    }
                }
            }
        }
    }
}
