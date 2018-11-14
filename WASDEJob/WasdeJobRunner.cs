using HtmlAgilityPack;
using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using McF.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Excel = Microsoft.Office.Interop.Excel;

namespace WasdeJob
{
    public enum YEARTYPE
    {
        NONE = 1,
        PREVYEAR,
        CURRYEAR,
        PROJYEAR
    }
    public class WasdeJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private string RawFile;
        private string ReportDate;

        private Dictionary<string, DataTable> dictRawTables;
 
        private List<WasdeCommData> lstWsdCommdat = new List<WasdeCommData>();
        private List<WasdeDomesticData> lstWsdDomesData = new List<WasdeDomesticData>();
        private Dictionary<string, WasdeDomesticField> WsdDomesticFields = new Dictionary<string, WasdeDomesticField>();
        private Dictionary<string, WasdeDomesticCommodity> WsdDomComms = new Dictionary<string, WasdeDomesticCommodity>();
        private Dictionary<string, WasdeCommodity> WsdComms = new Dictionary<string, WasdeCommodity>();
        private List<string> Units = new List<string>();

        private static string WORLD = "World";
        private static string US = "U.S.";
        private static string MEXICOSUGAR = "Mexico Sugar";
        private static string USNODOT = "U.S";
        private static string SUPPLYUSE = "Supply and Use";
        private static string BEGINNING = "Beginning";
        private static string STOCK = "Stocks";

        static string prevRegion = String.Empty;
        static string currentUnit = String.Empty;
        static string projMonth1 = String.Empty;
        static string projMonth2 = String.Empty;

        static int MonthRowIndex1 = 0;
        static int MonthRowIndex2 = 0;

        private Dictionary<int, string> ProcessHeaderRow(DataRow dr)
        {
            int i = 0;
            Dictionary<int, string> Headers = new Dictionary<int, string>();
            while (true) //loop failse only if dr value becomes null
            {
                string value = dr[i]?.ToString();
                if (String.IsNullOrEmpty(value) &&
                    String.IsNullOrEmpty(dr[i + 1]?.ToString()))
                    break;

                if (!String.IsNullOrEmpty(value))
                {
                    value.Replace('\n', '_').Trim();
                    if (value.Contains(@"\"))
                    {
                        value = value.Trim().Substring(0, value.LastIndexOf(' ')).Trim();
                    }
                    Headers[i] = value;
                }
                i++;
            }
            return Headers;
        }

        private string GetWorldCategory(string commodity)
        {
            switch (commodity.ToUpper())
            {
                case "WHEAT":
                case "RICE":
                case "COARSE GRAIN":
                case "CORN":
                    return "Food Grains";
                case "COTTON":
                    return "Cotton";
                case "SOYABEAN":
                case "SOYABEAN MEAL":
                case "SOYABEAN OIL":
                    return "SoyaBean";
                default:
                    return "Agriculture";

            }
        }

        private WasdeCommodity GetWorldCommodity(string Commodity, string Page)
        {
            WasdeCommodity wasd = null;
            if (WsdComms.ContainsKey(Commodity.ToUpper()))
            {
                wasd = WsdComms[Commodity.ToUpper()];
            }
            else
            {
                wasd = new WasdeCommodity();
                wasd.CommodityName = Commodity;
                wasd.Category = GetWorldCategory(Commodity);
                WsdComms.Add(Commodity.ToUpper(), wasd);
            }
            wasd.Page += $"{Page.Replace("'", "")},";
            return wasd;
        }

        private WasdeDomesticCommodity GetDomesticCommodity(string Commodity, string Page)
        {
            WasdeDomesticCommodity wasd = null;
            if (WsdDomComms.ContainsKey(Commodity.ToUpper()))
            {
                wasd = WsdDomComms[Commodity.ToUpper()];
            }
            else
            {
                wasd = new WasdeDomesticCommodity();
                wasd.CommodityName = Commodity;
                wasd.Category = GetWorldCategory(Commodity);
                WsdDomComms.Add(Commodity.ToUpper(), wasd);
            }
            wasd.Page += $"{Page.Replace("'", "")},";
            return wasd;
        }
        private string GetWorldCommidityName(string world)
        {
            int index = world.IndexOf(WORLD) + WORLD.Length;
            int endindex = world.IndexOf(SUPPLYUSE) - index;
            String Commodity = world.Substring(index, endindex).Trim();
            return Commodity;
        }

        public WasdeJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>(); jobService = _unityContainer.Resolve<JobService>();
            lstWsdCommdat = new List<WasdeCommData>();
            lstWsdDomesData = new List<WasdeDomesticData>();
            WsdDomesticFields = new Dictionary<string, WasdeDomesticField>();
            WsdDomComms = new Dictionary<string, WasdeDomesticCommodity>();
            WsdComms = new Dictionary<string, WasdeCommodity>();
            Units = new List<string>();
        }
        private void ProcessWorldComm(string Page, DataTable dt, int row)
        {
            String Commodity = GetWorldCommidityName(dt.Rows[row][0].ToString());
            WasdeCommodity wasd = GetWorldCommodity(Commodity, Page);

            Dictionary<int, string> Headers = null;

            string Month = GetMonth(dt);
            bool bSawUntis = false;
            bool bAlreadyProcessing = false;
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (index++ <= row)
                {
                    string val = dr[0]?.ToString();
                    if (string.IsNullOrEmpty(Month) && !string.IsNullOrEmpty(val))
                    {
                        Month = val;
                    }
                    continue;
                }

                string firstCol = dr[0]?.ToString();
                if (!bAlreadyProcessing && String.IsNullOrEmpty(firstCol))
                    continue;

                if (!bSawUntis)
                {
                    wasd.Unit = firstCol.Replace("(", "").Replace(")", "");
                    bSawUntis = true;
                    continue;
                }

                string secondCol = dr[1]?.ToString();
                string thirdCol = dr[2]?.ToString();

                if (String.IsNullOrEmpty(secondCol) && String.IsNullOrEmpty(thirdCol))
                    continue;

                if (secondCol.Contains(BEGINNING) && secondCol.Contains(STOCK) ||
                    thirdCol.Contains(BEGINNING) && thirdCol.Contains(STOCK))
                {
                    Headers = ProcessHeaderRow(dr);
                    bAlreadyProcessing = true;
                    continue;
                }

                if (Headers == null || Headers?.Count <= 0)
                    continue;

                WasdeCommData wasComm = ProcessWorldDataRow(dr, wasd, Month, Headers);
                if (wasComm != null)
                    lstWsdCommdat.Add(wasComm);
            }
        }
        private  YEARTYPE GetWorldYearType(string year)
        {
            YEARTYPE yearType = YEARTYPE.PREVYEAR;
            if (year.Contains("Est") || (Convert.ToDateTime(ReportDate).Year-1).ToString() == year)
                yearType = YEARTYPE.CURRYEAR;
            else if (year.Contains("Proj") || (Convert.ToDateTime(ReportDate).Year).ToString() == year)
                yearType = YEARTYPE.PROJYEAR;

            return yearType;
        }
        private WasdeCommData ProcessWorldDataRow(DataRow dr, WasdeCommodity wasdComm, string Month, Dictionary<int, string> Headers)
        {
            string year = Headers[0].Trim().Substring(0,4);
            bool isMonthType = GetWorldYearType(year) == YEARTYPE.PROJYEAR ? true : false;

            int Increment = 1;
            int DataMonth = Convert.ToDateTime(ReportDate).Month;

            if (isMonthType)
            {
                DataMonth = GetMonth(dr[1]?.ToString());
                Increment = 2;
                if (!String.IsNullOrEmpty(dr[0]?.ToString()))
                {
                    prevRegion = dr[0]?.ToString().Trim();
                }
            }
            else
                prevRegion = dr[0]?.ToString().Trim();

            if (DataMonth == 0)
                return null;
            WasdeCommData wasdeCommDat = new WasdeCommData(ReportDate, wasdComm.CommodityName,
                new DateTime(Convert.ToInt32(year.Trim().Substring(0, 4)), DataMonth, 1).ToShortDateString(), prevRegion);
          
            for (int i = Increment; i < Headers.Count; i++)
            {
                if (Headers.ContainsKey(i) && !String.IsNullOrEmpty(Headers[i]))
                {
                    string val = dr[i]?.ToString().Trim().Replace("*", String.Empty).Replace(@"\",String.Empty);
                    if( val.Contains("-") && val.IndexOf("-") > 1)
                    {
                        val = val.Substring(0, val.IndexOf("-") - 1).Trim();
                    }
                    wasdeCommDat.Values.Add(Headers[i], val);
                }
            }
            return wasdeCommDat;
        }

        private bool IsUnitRow(DataRow dr, int MaxIndex)
        {
            bool bIsUnitRow = false;
            String unit = String.Empty;
            for (int i = 0; i <= MaxIndex; i++)
            {
                string val = dr[i]?.ToString();
                if (String.IsNullOrEmpty(val))
                    continue;

                double dataVal = 0;
                if (Double.TryParse(val, out dataVal))
                    return false;

                foreach (string str in Units)
                {
                    if (val.ToUpper().Contains(str))
                    {
                        bIsUnitRow = true;
                        break;
                    }
                }
            }
            if (bIsUnitRow)
            {
                for (int i = 0; i <= MaxIndex; i++)
                {
                    string val = dr[i]?.ToString();
                    if (String.IsNullOrEmpty(val))
                        continue;

                    unit += $"{val} ";
                }
                currentUnit = unit.Trim();
            }

            return bIsUnitRow;
        }
        private bool CheckSubsequentHeaders(DataRow dr, Dictionary<int, string> Headers)
        {
            int MatchCount = 0;
            foreach (KeyValuePair<int, string> kv in Headers)
            {
                if (dr[kv.Key]?.ToString().Trim() == kv.Value)
                    MatchCount++;
            }
            if (MatchCount == 4)
            {
                if (Headers.ContainsKey(0))
                    Headers[0] = dr[0]?.ToString().Trim();
                return true;
            }
            return false;
        }
        private void ProcessDomesticData(DataTable dt, int row, string Commodity, string Page)
        {
            int index = 0;
            bool bFoundHeader = false, bFoundEstMonth = false;
            Dictionary<int, string> Headers = new Dictionary<int, string>();
            WasdeDomesticCommodity wasDomesticCommodity = null;
            foreach (DataRow dr in dt.Rows)
            {
                if (index++ <= row)
                    continue;

                string val = dr[0]?.ToString();
                if (val.Trim().ToUpper() == "FILLER")
                    continue;

                if (!bFoundHeader)
                {
                    bFoundHeader = ProcessDomesticHeader(dr, dt, Headers);
                    continue;
                }

                if (CheckSubsequentHeaders(dr, Headers))
                {
                    bFoundEstMonth = false;
                    wasDomesticCommodity = GetDomesticCommodity(Headers[0], Page);
                    continue;
                }

                if (!bFoundEstMonth)
                {
                    MonthRowIndex1 = Headers.Reverse().ElementAt(1).Key;
                    MonthRowIndex2 = Headers.Reverse().ElementAt(0).Key;

                    projMonth1 = dr[MonthRowIndex1]?.ToString();
                    projMonth2 = dr[MonthRowIndex2]?.ToString();

                    bFoundEstMonth = true;
                    continue;
                }

                string firstRow = dr[0]?.ToString();
                if (firstRow.Contains(US))
                {
                    ProcessSubWheatData(dt, index);
                    break;
                }

                if (firstRow.Contains(MEXICOSUGAR))
                {
                    break;
                }

                if (IsUnitRow(dr, Headers.Reverse().ElementAt(0).Key))
                    continue;

                if (Headers.ContainsKey(0))
                    Commodity = Headers[0];

                if (wasDomesticCommodity == null)
                    wasDomesticCommodity = GetDomesticCommodity(Commodity, Page);

                ProcessDomesticRow(Headers, dr, wasDomesticCommodity);
            }
        }
        private void ProcessSubWheatData(DataTable dt, int row)
        {

        }
        private string GetDomesticCommidityName(string domestic)
        {
            int index = domestic.IndexOf(US) + US.Length;
            int endindex = domestic.IndexOf(SUPPLYUSE) - index;
            String Commodity = domestic.Substring(index, endindex).Trim();
            return Commodity;
        }
        private WasdeDomesticField PopulateField(string Field, string Commodity, string Unit = null)
        {
            if(Field.Contains(@"\"))
            {
                Field = Field.Trim().Substring(0, Field.LastIndexOf(' '));
            }
            WasdeDomesticField wasd = null;
            string key = $"{Field.ToUpper()}--{Commodity.ToUpper()}";
            if (WsdDomesticFields.ContainsKey(key))
            {
                wasd = WsdDomesticFields[key];
            }
            else
            {
                wasd = new WasdeDomesticField();
                wasd.FieldName = Field;
                wasd.Unit = !String.IsNullOrEmpty(Unit) ? Unit : currentUnit;
                wasd.Commodity = Commodity;
                WsdDomesticFields.Add(key, wasd);
            }
            return wasd;
        }

        public int GetMonth(string Mon)
        {
            string Month = Mon.ToUpper();

            if (Month.Contains("JAN"))
                return 1;
            else if (Month.Contains("FEB"))
                return 2;
            else if (Month.Contains("MAR"))
                return 3;
            else if (Month.Contains("APR"))
                return 4;
            else if (Month.Contains("MAY"))
                return 5;
            else if (Month.Contains("JUN"))
                return 6;
            else if (Month.Contains("JUL"))
                return 7;
            else if (Month.Contains("AUG"))
                return 8;
            else if (Month.Contains("SEP"))
                return 9;
            else if (Month.Contains("OCT"))
                return 10;
            else if (Month.Contains("NOV"))
                return 11;
            else if (Month.Contains("DEC"))
                return 12;

            return 0;
        }

        private void ProcessDomesticRow(Dictionary<int, string> Headers, DataRow dr, WasdeDomesticCommodity wasdCommData)
        {
            string val = dr[0]?.ToString();

            if (String.IsNullOrEmpty(val))
                return;

            foreach (KeyValuePair<int, string> kv in Headers)
            {
                if (kv.Key == 0)
                    continue;

                string Field = dr[0]?.ToString().Trim();
                string Value = dr[kv.Key]?.ToString().Trim();

                if (String.IsNullOrEmpty(Value) || String.IsNullOrEmpty(Field))
                    break;

                if (Field.Contains("(") && Field.Contains(")"))
                {
                    int index = Field.IndexOf("(");
                    string unit = Field.Substring(index + 1, Field.IndexOf(")") - index - 1);
                    Field = Field.Substring(0, index - 1);
                    PopulateField(Field, wasdCommData.CommodityName, unit);
                }
                else
                    PopulateField(Field, wasdCommData.CommodityName);

                int DataMonth = Convert.ToDateTime(ReportDate).Month;
                if (MonthRowIndex1 == kv.Key)
                {
                    DataMonth = GetMonth(projMonth1);
                }
                if (MonthRowIndex2 == kv.Key)
                {
                    DataMonth = GetMonth(projMonth2);
                }
                WasdeDomesticData wasData = new WasdeDomesticData(ReportDate, wasdCommData.CommodityName, new DateTime( Convert.ToInt32(kv.Value.Trim().Substring(0,4)), DataMonth, 1).ToShortDateString());
                wasData.Field = Field;
                wasData.Value = Value.Replace("*", string.Empty).Replace("NA","0");
                lstWsdDomesData.Add(wasData);
            }
        }
        private string GetDomesticMonth(DataTable dt, int row)
        {
            string Month = GetMonth(dt);

            int index = 0;
            if (string.IsNullOrEmpty(Month))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (index++ <= row)
                    {
                        string val = dr[0]?.ToString();
                        if (string.IsNullOrEmpty(Month) && !string.IsNullOrEmpty(val))
                        {
                            Month = val;
                            break;
                        }
                        continue;
                    }
                    break;
                }
            }
            return Month;
        }
        private string GetMonth(DataTable dt)
        {
            string Month = dt.Columns[0]?.ToString().ToUpper();

            if (Month.Contains("JAN") ||
                Month.Contains("FEB") ||
                Month.Contains("MAR") ||
                Month.Contains("APR") ||
                Month.Contains("MAY") ||
                Month.Contains("JUN") ||
                Month.Contains("JUL") ||
                Month.Contains("AUG") ||
                Month.Contains("SEP") ||
                Month.Contains("OCT") ||
                Month.Contains("NOV") ||
                Month.Contains("DEC"))
            {
                return dt.Columns[0].ToString();
            }
            return String.Empty;
        }
        private void ProcessDomestic(string Page, DataTable dt, int row)
        {
            string Commodity = GetDomesticCommidityName(dt.Rows[row][0].ToString());

            if (Commodity.ToUpper().Contains("MEATS") || Commodity.ToUpper().Contains("EGG") || Commodity.ToUpper().Contains("DAIRY"))
                return;

            ProcessDomesticData(dt, row, Commodity, Page);
        }
        private bool ProcessDomesticHeader(DataRow dr, DataTable dt, Dictionary<int, string> Headers)
        {
            bool bHeader = false;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (!String.IsNullOrEmpty(dr[i]?.ToString()))
                {
                    Headers.Add(i, dr[i].ToString().Trim());
                    bHeader = true;
                }
            }
            if (Headers.Count < 4)
            {
                Headers.Clear();
                return false;
            }
            return bHeader;
        }
        private void ParseFile(string fileName)
        {
            Units.Add("MILLION");
            Units.Add("TONS");
            Units.Add("ACRES");
            Units.Add("BUSHELS");
            Units.Add("TONS");
            Units.Add("POUNDS");
            Units.Add("HUNDREDWEIGHT");
            Units.Add("DOLLARS");
            Units.Add("LBS");
            Units.Add("DOZ");
            Units.Add("CENTS");
            Units.Add("DOL");

            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", fileName);

            WsdComms = commonRepo.GetWasdeCommodities();
            WsdDomComms = commonRepo.GetWasdeDomesticCommodities();
            WsdDomesticFields = commonRepo.GetWasdeDomesticFields();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);

                    int i = 0;
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        if (dr[0] != DBNull.Value)
                        {
                            string str = dr[0].ToString();

                            if (str.Contains(WORLD) && str.Contains(SUPPLYUSE) && !str.Contains(USNODOT))
                            {
                                ProcessWorldComm(sheetName, dataTable, i);
                                break;
                            }
                            else if (str.Contains(USNODOT) && str.Contains(SUPPLYUSE) && !str.Contains(WORLD))
                            {
                                ProcessDomestic(sheetName, dataTable, i);
                                break;
                            }
                        }
                        i++;
                    }
                }
            }

            foreach (KeyValuePair<string, WasdeDomesticField> kv in WsdDomesticFields)
            {
                commonRepo.ProcessQuery(kv.Value.GetPopulateQuery(JobID, 1, DateTime.Now));
            }
            foreach (KeyValuePair<string, WasdeDomesticCommodity> kv in WsdDomComms)
            {
                commonRepo.ProcessQuery(kv.Value.GetPopulateQuery(JobID, 1, DateTime.Now));
            }

            foreach (KeyValuePair<string, WasdeCommodity> kv in WsdComms)
            {
                commonRepo.ProcessQuery(kv.Value.GetPopulateQuery(JobID, 1, DateTime.Now));
            }

            foreach (WasdeCommData commData in lstWsdCommdat)
            {
                foreach (string str in commData.PopulateQuery(JobID, 1, DateTime.Now))
                {
                    commonRepo.ProcessQuery(str);
                }
            }
            foreach (WasdeDomesticData commDomData in lstWsdDomesData)
            {
                foreach (string str in commDomData.PopulateQuery(JobID, 1, DateTime.Now))
                {
                    commonRepo.ProcessQuery(str);
                }
            }
        }
        public bool RUNJob(Dictionary<string, string> JobParams, string dataSource, int jobID)
        {
            JobStartTime = DateTime.Now;
            JobID = jobID;
            DataSource = dataSource;
            UpdateJobTime updateJobTime = new UpdateJobTime()
            {
                Id = JobID,
                startTime = JobStartTime,
                Status = "RUNNING",
                UserID = 0
            };
            jobService.UpdateJobStatus(updateJobTime);
            string ReportDate = DateTime.Now.ToShortDateString();
            if (JobParams.ContainsKey("DATE"))
                ReportDate = JobParams["DATE"];

            DateTime reportDataDate = Convert.ToDateTime(ReportDate);

            Dictionary<string, string> data = McFHTMLHelper.ParseWASDEFile(JobParams["URL"], reportDataDate);
            string RawData = String.Empty;
            if (data.Count > 0)
            {
                foreach (KeyValuePair<string, string> kyData in data)
                {
                    DownloadFiles(kyData.Value);
                    ParseFile(RawFile);
                    RawData = $"URL:{RawFile}";
                }
                updateJobTime.endTime = DateTime.Now;
                updateJobTime.Message = "Success";
                updateJobTime.Status = "Completed";
                updateJobTime.FilePath = RawData;
                updateJobTime.FileType = "xlsx";
                updateJobTime.NoOfNewRecords = 10;
                jobService.UpdateJobStatus(updateJobTime);
            }
            else
            {
                updateJobTime.endTime = DateTime.Now;
                updateJobTime.Message = "No File Today";
                updateJobTime.Status = "Failed";
                updateJobTime.NoOfNewRecords = 0;
                jobService.UpdateJobStatus(updateJobTime);
            }

            return true;
        }
        private string[] GetExcelSheetNames(string connectionString)
        {
            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            return excelSheetNames;
        }
         
        private void DownloadFiles(string rawUrl)
        {
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Console.WriteLine(path);
            Directory.CreateDirectory(path);
            RawFile = $@"{path}\WASDE{Path.GetExtension(rawUrl)}";

            using (WebClient Client = new WebClient())
            {
                Client.Proxy = null;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                Client.DownloadFile(rawUrl, RawFile);
            }
        }
    }
}



