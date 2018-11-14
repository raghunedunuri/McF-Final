using McF.Business;
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

namespace SugarJob
{
    public class SugarJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private string RawFile;
        private int NoOfRecords;
        private const string TOTAL_DEL = "Total Deliveries";
        private const string ACTUAL_WEIGTH = "actual weight";
        private const string BAKERY_CER = "Bakery, cereal, and related products";
        private const string CONFECTIONERY_AND = "Confectionery and related products";
        private const string ICE_CRE = "Ice cream and dairy products";
        private const string BEVERAGES_BEV = "Beverages";
        private const string CANNED_BOT = "Canned, bottled and frozen foods";
        private const string MULTIPLE_AND = "Multiple and all other food uses";
        private const string NON_USE = "Non-food uses";
        private const string HOTELS_RES = "Hotels, restaurants, institutions";
        private const string WHOLESALE_GRO = "Wholesale grocers, jobbers, dealers";
        private const string RETAIL_GRO = "Retail grocers, chain stores";
        private const string GOVERNMENT_AGE = "Government agencies";
        private const string ALL_OTH = "All other deliveries";
        private const string BEG_STOCKS = "Beginning stocks";
        private const string TOTAL_PRO = "Total production:";
        private const string TOTAL_BEET = "Beet";
        private const string TOTAL_CANE = "Cane";
        private const string TOTAL_IMPORTS = "Imports";
        private const string TOTAL_SUPPLY = "Supply";
        private const string TOTAL_EXPORTS = "Exports";
        private const string DOMESTIC_DEL = "Domestic deliveries:";
        private const string NON_HUM = "Non-human";
        private const string RE_EXPORT_PROGRAM = "Products Re-export Program";
        private const string HUMAN_USE = "Human use:";
        private const string TOTAL_BY_BEET_PROCESSORS = "By beet processors";
        private const string TOTAL_BY_CANE_REFINERS="By cane refiners/processors";
        private const string BY_NON_REP = "By non-reporters";
        private const string TOTAL_MISCELLANEOUS_SUPPLY_ADJUSTMENT = "Miscellaneous supply adjustment";
        private const string TOTAL_USE = "Use";
        private const string TOTAL_ENDING_STOCKS = "Total ending stocks";

        public SugarJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>(); jobService = _unityContainer.Resolve<JobService>();
            NoOfRecords = 0;
        }
        public bool RUNJob(Dictionary<string, string> JobParams, string dataSource, int jobID)
        {
            JobStartTime = DateTime.Now;
            JobID = jobID;
            DataSource = dataSource;
            JobStatus jobStatus = jobService.GetCurrentJob(JobID);
            //Update JobStatus to Running and StartTime
            UpdateJobTime updateJobTime = new UpdateJobTime()
            {
                Id = JobID,
                startTime = JobStartTime,
                Status = "RUNNING",
                UserID = 0 //Indicates JOB Runner
            };
            jobService.UpdateJobStatus(updateJobTime);

            DownloadFiles(JobParams["FILE"]);

            ProcessFile(RawFile);

            updateJobTime.endTime = DateTime.Now;
            updateJobTime.Message = "Success";
            updateJobTime.Status = "Completed";
            updateJobTime.FilePath = RawFile;
            updateJobTime.FileType = "xlsx";
            updateJobTime.NoOfNewRecords = NoOfRecords;
            jobService.UpdateJobStatus(updateJobTime);

            return true;
        }
        private long ConvertToInt(object str)
        {
            if (str == null)
                return 0;
            string str1 = str.ToString();
            if (String.IsNullOrEmpty(str1))
                return 0;
            else
                return Convert.ToInt64(str1);
        }
        private DateTime FromExcelSerialDate(object SerialDate)
        {
            if (SerialDate == null)
                return DateTime.MinValue;
            long val = Convert.ToInt64(SerialDate.ToString());

            if (val > 59) val -= 1;
            return new DateTime(1899, 12, 31).AddDays(val);
        }
        private void FillBegin(string file)
        {

        }

        private string GetMonth(int Month)
        {
            switch(Month)
            {
                case 1:
                    return "JAN";
                case 2:
                    return "FEB";
                case 3:
                    return "MAR";
                case 4:
                    return "APR";
                case 5:
                    return "MAY";
                case 6:
                    return "JUN";
                case 7:
                    return "JUL";
                case 8:
                    return "AUG";
                case 9:
                    return "SEP";
                case 10:
                    return "OCT";
                case 11:
                    return "NOV";
                case 12:
                    return "DEC";
            }
            return String.Empty;
        }

        private void ProcessImportData( DataRow dr, Dictionary<int, SugarImportData> lstSugarImport)
        {
            string dataVal = dr[0]?.ToString();

            if (dataVal.Trim() == "Cane Refiner Raw Imports from MX")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Mex_CRRawImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Cane Refiner Refined Imports from MX")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Mex_CRRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Non Reporter Refined Imports from MX")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Mex_NonRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Imports from MX")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Mex_TotalImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Cane Refiner Raw Imports from ROW")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].ROW_CRRawImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Cane Refiner Refined Imports from ROW")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].ROW_CRRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Non Reporter Refined Imports from ROW")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].ROW_NonRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Imports from ROW")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].ROW_TotalImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Cane Refiner Raw Imports")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Total_CRRawImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Cane Refiner Refined Imports")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Total_CRRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Non Reporter Refined Imports")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].Total_NonRefinedImports = dr[i]?.ToString();
                }
            }
            else if (dataVal.Trim() == "Total Imports")
            {
                for (int i = 1; i < 13; i++)
                {
                    lstSugarImport[i].TotalImports = dr[i]?.ToString();
                }
            }
        }
        private void ProcessRegionDeliveryData( DataRow dr, Dictionary<int, SugarRegionDelData> dictRegionDelData )
        {
            //, OCTOBER 2001 (SHORT TONS, RAW VALUE)   1/
            const string BUYER_REG_INDEX = "AND BY REGION,";
            string dataVal = dr[0]?.ToString();
            if (dataVal.ToUpper().Trim().Contains(BUYER_REG_INDEX))
            {
                string val = dataVal.Substring(dataVal.IndexOf(BUYER_REG_INDEX) + BUYER_REG_INDEX.Length+1).Trim();
                string[] arr = val.Split(' ');
                if(arr.Length > 1)
                for (int i = 1; i <= 6; i++)
                {
                    string Year = !String.IsNullOrEmpty(arr[1].Trim()) ? arr[1] : arr[2];
                    dictRegionDelData[i].MONTH_DATE = Convert.ToDateTime($"{arr[0]} {Year}").ToShortDateString();
                }
            }
            else if ( dataVal.ToUpper().Trim().Contains(TOTAL_DEL.ToUpper().Trim()) &&
                 dataVal.ToUpper().Trim().Contains(ACTUAL_WEIGTH.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].TOTAL_DEL = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(BAKERY_CER.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_BAKERY_CEREAL_REL_PROD = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(CONFECTIONERY_AND.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_CONF_REL_PROD = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(ICE_CRE.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_ICECREAM_DAIRY_PROD = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(BEVERAGES_BEV.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_BEVERAGES = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(CANNED_BOT.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_CANNED_BOTTLE_FROZEN = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(MULTIPLE_AND.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_MULTIPLE_OTHERFOOD = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(NON_USE.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_NONFOOD = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(HOTELS_RES.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_HOT_RES_INS = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(WHOLESALE_GRO.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_WS_JOBBERS_DEALERS = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(RETAIL_GRO.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_RETAILGROC_CHAINSTORE = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(GOVERNMENT_AGE.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_GOVT_AGEN = dr[i]?.ToString();
                }
            }
            else if (dataVal.ToUpper().Trim().Contains(ALL_OTH.ToUpper().Trim()))
            {
                for (int i = 1; i <= 6; i++)
                {
                    dictRegionDelData[i].DEL_OTHERS = dr[i]?.ToString();
                }
            }
        }

        private void ProcessSugarBuyerData( DataRow dr, SugarBuyerData sugarBuyerData )
        {
            string dataVal = dr[0]?.ToString();
            //AND BY TYPE OF SUGAR, FOR 
            const string BUYER_MONTH_INDEX = "AND BY TYPE OF SUGAR, FOR";
            if (dataVal.ToUpper().Trim().Contains(BUYER_MONTH_INDEX))
            {
                string val = dataVal.Substring(dataVal.IndexOf(BUYER_MONTH_INDEX) + BUYER_MONTH_INDEX.Length+1).Trim();
                string[] arr = val.Split(' ');
                if (arr.Length > 1)
                    sugarBuyerData.MONTH_DATE = Convert.ToDateTime($"{arr[0]} {arr[1].Substring(0,4)}").ToShortDateString();
            }
            else if (dataVal.ToUpper().Trim().Contains(TOTAL_DEL.ToUpper().Trim()) &&
                        dataVal.ToUpper().Trim().Contains(ACTUAL_WEIGTH.ToUpper().Trim()))
            {
                sugarBuyerData.TOTAL_DEL_BEET = dr[1]?.ToString();
                sugarBuyerData.TOTAL_DEL_CANE = dr[2]?.ToString();
                sugarBuyerData.TOTAL_DEL = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains( BAKERY_CER.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_BAKERY_CEREAL_REL_PROD = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_BAKERY_CEREAL_REL_PROD = dr[2]?.ToString();
                sugarBuyerData.DEL_BAKERY_CEREAL_REL_PROD = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(CONFECTIONERY_AND.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_CONF_REL_PROD = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_CONF_REL_PROD = dr[2]?.ToString();
                sugarBuyerData.DEL_CONF_REL_PROD = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(ICE_CRE.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_ICECREAM_DAIRY_PROD = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_ICECREAM_DAIRY_PROD = dr[2]?.ToString();
                sugarBuyerData.DEL_ICECREAM_DAIRY_PROD = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(BEVERAGES_BEV.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_BEVERAGES = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_BEVERAGES = dr[2]?.ToString();
                sugarBuyerData.DEL_BEVERAGES = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(CANNED_BOT.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_CANNED_BOTTLE_FROZEN = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_CANNED_BOTTLE_FROZEN = dr[2]?.ToString();
                sugarBuyerData.DEL_CANNED_BOTTLE_FROZEN = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(MULTIPLE_AND.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_MULTIPLE_OTHERFOOD = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_MULTIPLE_OTHERFOOD = dr[2]?.ToString();
                sugarBuyerData.DEL_MULTIPLE_OTHERFOOD = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(NON_USE.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_NONFOOD = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_NONFOOD = dr[2]?.ToString();
                sugarBuyerData.DEL_NONFOOD = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(HOTELS_RES.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_HOT_RES_INS = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_HOT_RES_INS = dr[2]?.ToString();
                sugarBuyerData.DEL_HOT_RES_INS = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(WHOLESALE_GRO.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_WS_JOBBERS_DEALERS = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_WS_JOBBERS_DEALERS = dr[2]?.ToString();
                sugarBuyerData.DEL_WS_JOBBERS_DEALERS = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(RETAIL_GRO.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_RETAILGROC_CHAINSTORE = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_RETAILGROC_CHAINSTORE = dr[2]?.ToString();
                sugarBuyerData.DEL_RETAILGROC_CHAINSTORE = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(GOVERNMENT_AGE.ToUpper().Trim()) )
            {
                sugarBuyerData.DEL_BEET_GOVT_AGEN = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_GOVT_AGEN = dr[2]?.ToString();
                sugarBuyerData.DEL_GOVT_AGEN = dr[3]?.ToString();
            }
            else if (dataVal.ToUpper().Trim().Contains(ALL_OTH.ToUpper().Trim()))
            {
                sugarBuyerData.DEL_BEET_OTHERS = dr[1]?.ToString();
                sugarBuyerData.DEL_CANE_OTHERS = dr[2]?.ToString();
                sugarBuyerData.DEL_OTHERS = dr[3]?.ToString();
            }
        }
        private void ProcessFile(string file)
        {
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", file);
            DataSet data = new DataSet();
            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                }
            }
            Dictionary<string, SugarData> lstSugar = new Dictionary<string, SugarData>();
            Dictionary<int, SugarImportData> lstSugarImport = new Dictionary<int, SugarImportData>();
            SugarBuyerData sugarBuyerData = null;
            Dictionary<int, SugarRegionDelData> lstSugarRegionDelData = new Dictionary<int, SugarRegionDelData>();
            DataTable dt = data.Tables[0];

            string prevDataTable = String.Empty;
            string currMonth = String.Empty;
            DateTime currDate = DateTime.MinValue;

            string Val = String.Empty;
            Val = dt.Columns[0].ColumnName;
            if (String.IsNullOrEmpty(currMonth) && Val.Contains("SMD Report"))
            {
                currMonth = Val.Substring(0, Val.IndexOf("SMD Report"));
                if (currMonth.Contains("FINAL - "))
                    currMonth = currMonth.Substring(8).Trim();
                currDate = Convert.ToDateTime(currMonth);
                currMonth = GetMonth(currDate.Month);
            }

            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[0] == DBNull.Value)
                        continue;

                    string dataVal = dr[0]?.ToString();

                    if (string.IsNullOrEmpty(dataVal))
                        continue;

                    if (dataVal.ToUpper().Contains("TABLE 1-B"))
                    {
                        prevDataTable = "TABLE1B";
                        int Var = currDate.Month >= 10 ? 0 : - 1;
                        string currentDate = DateTime.Now.ToShortDateString();
                        int Month = 10;
                        for (int i = 1; i < 13; i++)
                        {
                            lstSugarImport.Add(i, new SugarImportData(currentDate, new DateTime(currDate.Year + Var, Month++, 1).ToShortDateString()));
                            if( Month == 12)
                            {
                                Var += 1;
                                Month = 1;
                            }
                        }
                        continue;
                    }
                    else if (dataVal.ToUpper().Contains("TABLE 1."))
                    {
                        prevDataTable = "TABLE1";
                        int Year = currDate.Month >= 10 ? currDate.Year : currDate.Year - 1;
                        lstSugar.Add("OCT", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year, 10,1).ToShortDateString()));
                        lstSugar.Add("NOV", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year, 11, 1).ToShortDateString()));
                        lstSugar.Add("DEC", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year, 12, 1).ToShortDateString()));
                        lstSugar.Add("JAN", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year+1, 1, 1).ToShortDateString()));
                        lstSugar.Add("FEB", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1 , 2, 1).ToShortDateString()));
                        lstSugar.Add("MAR", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 3, 1).ToShortDateString()));
                        lstSugar.Add("APR", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 4, 1).ToShortDateString()));
                        lstSugar.Add("MAY", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 5, 1).ToShortDateString()));
                        lstSugar.Add("JUN", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 6, 1).ToShortDateString()));
                        lstSugar.Add("JUL", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 7, 1).ToShortDateString()));
                        lstSugar.Add("AUG", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 8, 1).ToShortDateString()));
                        lstSugar.Add("SEP", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(Year + 1, 9, 1).ToShortDateString()));
                        continue;
                    }
                    else if (dataVal.ToUpper().Contains("TABLE 8:"))
                    {
                        prevDataTable = "TABLE8";
                        string currentDate = DateTime.Now.ToShortDateString();
                        if( lstSugarRegionDelData.Count > 0 )
                        {
                            foreach (KeyValuePair<int, SugarRegionDelData> kv in lstSugarRegionDelData)
                            {
                                commonRepo.ProcessQuery(kv.Value.PopulateQuery(1, 1, DateTime.Now));
                            }
                        }
                        lstSugarRegionDelData.Clear();
                        lstSugarRegionDelData.Add(1, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "New England"));
                        lstSugarRegionDelData.Add(2, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "Mid Atlantic")); 
                        lstSugarRegionDelData.Add(3, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "North Central"));
                        lstSugarRegionDelData.Add(4, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "South"));
                        lstSugarRegionDelData.Add(5, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "West"));
                        lstSugarRegionDelData.Add(6, new SugarRegionDelData(currentDate, currDate.ToShortDateString(), "PuertoRico"));
                    }
                    else if (dataVal.ToUpper().Contains("TABLE 9:"))
                    {
                        prevDataTable = "TABLE9";
                        if(sugarBuyerData!=null)
                        {
                            commonRepo.ProcessQuery(sugarBuyerData.PopulateQuery(JobID, 1, DateTime.Now));
                        }
                        sugarBuyerData = new SugarBuyerData(DateTime.Now.ToShortDateString(), currDate.ToShortDateString());
                        continue;
                    }
                    else if (dataVal.ToUpper().Contains("TABLE "))
                    {
                        prevDataTable = String.Empty;
                        continue;
                    }
                    if (String.IsNullOrEmpty(prevDataTable))
                        continue;

                    switch(prevDataTable)
                    {
                        case "TABLE1B":
                            ProcessImportData(dr, lstSugarImport);
                            break;
                        case "TABLE1":
                            break;
                        case "TABLE8":
                            ProcessRegionDeliveryData(dr, lstSugarRegionDelData);
                            break;
                        case "TABLE9":
                            ProcessSugarBuyerData(dr, sugarBuyerData);
                            break;
                    }

                    if (dataVal.ToUpper().Trim() == BEG_STOCKS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].BEGINNNING_STOCKS = dr[1]?.ToString();
                        lstSugar["NOV"].BEGINNNING_STOCKS = dr[2]?.ToString();
                        lstSugar["DEC"].BEGINNNING_STOCKS = dr[3]?.ToString();
                        lstSugar["JAN"].BEGINNNING_STOCKS = dr[4]?.ToString();
                        lstSugar["FEB"].BEGINNNING_STOCKS = dr[5]?.ToString();
                        lstSugar["MAR"].BEGINNNING_STOCKS = dr[6]?.ToString();
                        lstSugar["APR"].BEGINNNING_STOCKS = dr[7]?.ToString();
                        lstSugar["MAY"].BEGINNNING_STOCKS = dr[8]?.ToString();
                        lstSugar["JUN"].BEGINNNING_STOCKS = dr[9]?.ToString();
                        lstSugar["JUL"].BEGINNNING_STOCKS = dr[10]?.ToString();
                        lstSugar["AUG"].BEGINNNING_STOCKS = dr[11]?.ToString();
                        lstSugar["SEP"].BEGINNNING_STOCKS = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_PRO.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].TOTAL_PRODUCTION = dr[1]?.ToString();
                        lstSugar["NOV"].TOTAL_PRODUCTION = dr[2]?.ToString();
                        lstSugar["DEC"].TOTAL_PRODUCTION = dr[3]?.ToString();
                        lstSugar["JAN"].TOTAL_PRODUCTION = dr[4]?.ToString();
                        lstSugar["FEB"].TOTAL_PRODUCTION = dr[5]?.ToString();
                        lstSugar["MAR"].TOTAL_PRODUCTION = dr[6]?.ToString();
                        lstSugar["APR"].TOTAL_PRODUCTION = dr[7]?.ToString();
                        lstSugar["MAY"].TOTAL_PRODUCTION = dr[8]?.ToString();
                        lstSugar["JUN"].TOTAL_PRODUCTION = dr[9]?.ToString();
                        lstSugar["JUL"].TOTAL_PRODUCTION = dr[10]?.ToString();
                        lstSugar["AUG"].TOTAL_PRODUCTION = dr[11]?.ToString();
                        lstSugar["SEP"].TOTAL_PRODUCTION = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_BEET.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].TP_BEET = dr[1]?.ToString();
                        lstSugar["NOV"].TP_BEET = dr[2]?.ToString();
                        lstSugar["DEC"].TP_BEET = dr[3]?.ToString();
                        lstSugar["JAN"].TP_BEET = dr[4]?.ToString();
                        lstSugar["FEB"].TP_BEET = dr[5]?.ToString();
                        lstSugar["MAR"].TP_BEET = dr[6]?.ToString();
                        lstSugar["APR"].TP_BEET = dr[7]?.ToString();
                        lstSugar["MAY"].TP_BEET = dr[8]?.ToString();
                        lstSugar["JUN"].TP_BEET = dr[9]?.ToString();
                        lstSugar["JUL"].TP_BEET = dr[10]?.ToString();
                        lstSugar["AUG"].TP_BEET = dr[11]?.ToString();
                        lstSugar["SEP"].TP_BEET = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_CANE.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].TP_CANE = dr[1]?.ToString();
                        lstSugar["NOV"].TP_CANE = dr[2]?.ToString();
                        lstSugar["DEC"].TP_CANE = dr[3]?.ToString();
                        lstSugar["JAN"].TP_CANE = dr[4]?.ToString();
                        lstSugar["FEB"].TP_CANE = dr[5]?.ToString();
                        lstSugar["MAR"].TP_CANE = dr[6]?.ToString();
                        lstSugar["APR"].TP_CANE = dr[7]?.ToString();
                        lstSugar["MAY"].TP_CANE = dr[8]?.ToString();
                        lstSugar["JUN"].TP_CANE = dr[9]?.ToString();
                        lstSugar["JUL"].TP_CANE = dr[10]?.ToString();
                        lstSugar["AUG"].TP_CANE = dr[11]?.ToString();
                        lstSugar["SEP"].TP_CANE = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_IMPORTS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].IMPORTS = dr[1]?.ToString();
                        lstSugar["NOV"].IMPORTS = dr[2]?.ToString();
                        lstSugar["DEC"].IMPORTS = dr[3]?.ToString();
                        lstSugar["JAN"].IMPORTS = dr[4]?.ToString();
                        lstSugar["FEB"].IMPORTS = dr[5]?.ToString();
                        lstSugar["MAR"].IMPORTS = dr[6]?.ToString();
                        lstSugar["APR"].IMPORTS = dr[7]?.ToString();
                        lstSugar["MAY"].IMPORTS = dr[8]?.ToString();
                        lstSugar["JUN"].IMPORTS = dr[9]?.ToString();
                        lstSugar["JUL"].IMPORTS = dr[10]?.ToString();
                        lstSugar["AUG"].IMPORTS = dr[11]?.ToString();
                        lstSugar["SEP"].IMPORTS = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_SUPPLY.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].SUPPLY = dr[1]?.ToString();
                        lstSugar["NOV"].SUPPLY = dr[2]?.ToString();
                        lstSugar["DEC"].SUPPLY = dr[3]?.ToString();
                        lstSugar["JAN"].SUPPLY = dr[4]?.ToString();
                        lstSugar["FEB"].SUPPLY = dr[5]?.ToString();
                        lstSugar["MAR"].SUPPLY = dr[6]?.ToString();
                        lstSugar["APR"].SUPPLY = dr[7]?.ToString();
                        lstSugar["MAY"].SUPPLY = dr[8]?.ToString();
                        lstSugar["JUN"].SUPPLY = dr[9]?.ToString();
                        lstSugar["JUL"].SUPPLY = dr[10]?.ToString();
                        lstSugar["AUG"].SUPPLY = dr[11]?.ToString();
                        lstSugar["SEP"].SUPPLY = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_EXPORTS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].EXPORTS = dr[1]?.ToString();
                        lstSugar["NOV"].EXPORTS = dr[2]?.ToString();
                        lstSugar["DEC"].EXPORTS = dr[3]?.ToString();
                        lstSugar["JAN"].EXPORTS = dr[4]?.ToString();
                        lstSugar["FEB"].EXPORTS = dr[5]?.ToString();
                        lstSugar["MAR"].EXPORTS = dr[6]?.ToString();
                        lstSugar["APR"].EXPORTS = dr[7]?.ToString();
                        lstSugar["MAY"].EXPORTS = dr[8]?.ToString();
                        lstSugar["JUN"].EXPORTS = dr[9]?.ToString();
                        lstSugar["JUL"].EXPORTS = dr[10]?.ToString();
                        lstSugar["AUG"].EXPORTS = dr[11]?.ToString();
                        lstSugar["SEP"].EXPORTS = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == DOMESTIC_DEL.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DOMESTIC_DELIVERY = dr[1]?.ToString();
                        lstSugar["NOV"].DOMESTIC_DELIVERY = dr[2]?.ToString();
                        lstSugar["DEC"].DOMESTIC_DELIVERY = dr[3]?.ToString();
                        lstSugar["JAN"].DOMESTIC_DELIVERY = dr[4]?.ToString();
                        lstSugar["FEB"].DOMESTIC_DELIVERY = dr[5]?.ToString();
                        lstSugar["MAR"].DOMESTIC_DELIVERY = dr[6]?.ToString();
                        lstSugar["APR"].DOMESTIC_DELIVERY = dr[7]?.ToString();
                        lstSugar["MAY"].DOMESTIC_DELIVERY = dr[8]?.ToString();
                        lstSugar["JUN"].DOMESTIC_DELIVERY = dr[9]?.ToString();
                        lstSugar["JUL"].DOMESTIC_DELIVERY = dr[10]?.ToString();
                        lstSugar["AUG"].DOMESTIC_DELIVERY = dr[11]?.ToString();
                        lstSugar["SEP"].DOMESTIC_DELIVERY = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == NON_HUM.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_NONHUMAN = dr[1]?.ToString();
                        lstSugar["NOV"].DD_NONHUMAN = dr[2]?.ToString();
                        lstSugar["DEC"].DD_NONHUMAN = dr[3]?.ToString();
                        lstSugar["JAN"].DD_NONHUMAN = dr[4]?.ToString();
                        lstSugar["FEB"].DD_NONHUMAN = dr[5]?.ToString();
                        lstSugar["MAR"].DD_NONHUMAN = dr[6]?.ToString();
                        lstSugar["APR"].DD_NONHUMAN = dr[7]?.ToString();
                        lstSugar["MAY"].DD_NONHUMAN = dr[8]?.ToString();
                        lstSugar["JUN"].DD_NONHUMAN = dr[9]?.ToString();
                        lstSugar["JUL"].DD_NONHUMAN = dr[10]?.ToString();
                        lstSugar["AUG"].DD_NONHUMAN = dr[11]?.ToString();
                        lstSugar["SEP"].DD_NONHUMAN = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == RE_EXPORT_PROGRAM.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_PRODUCT_REEXPORT = dr[1]?.ToString();
                        lstSugar["NOV"].DD_PRODUCT_REEXPORT = dr[2]?.ToString();
                        lstSugar["DEC"].DD_PRODUCT_REEXPORT = dr[3]?.ToString();
                        lstSugar["JAN"].DD_PRODUCT_REEXPORT = dr[4]?.ToString();
                        lstSugar["FEB"].DD_PRODUCT_REEXPORT = dr[5]?.ToString();
                        lstSugar["MAR"].DD_PRODUCT_REEXPORT = dr[6]?.ToString();
                        lstSugar["APR"].DD_PRODUCT_REEXPORT = dr[7]?.ToString();
                        lstSugar["MAY"].DD_PRODUCT_REEXPORT = dr[8]?.ToString();
                        lstSugar["JUN"].DD_PRODUCT_REEXPORT = dr[9]?.ToString();
                        lstSugar["JUL"].DD_PRODUCT_REEXPORT = dr[10]?.ToString();
                        lstSugar["AUG"].DD_PRODUCT_REEXPORT = dr[11]?.ToString();
                        lstSugar["SEP"].DD_PRODUCT_REEXPORT = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == HUMAN_USE.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_HUMANUSE = dr[1]?.ToString();
                        lstSugar["NOV"].DD_HUMANUSE = dr[2]?.ToString();
                        lstSugar["DEC"].DD_HUMANUSE = dr[3]?.ToString();
                        lstSugar["JAN"].DD_HUMANUSE = dr[4]?.ToString();
                        lstSugar["FEB"].DD_HUMANUSE = dr[5]?.ToString();
                        lstSugar["MAR"].DD_HUMANUSE = dr[6]?.ToString();
                        lstSugar["APR"].DD_HUMANUSE = dr[7]?.ToString();
                        lstSugar["MAY"].DD_HUMANUSE = dr[8]?.ToString();
                        lstSugar["JUN"].DD_HUMANUSE = dr[9]?.ToString();
                        lstSugar["JUL"].DD_HUMANUSE = dr[10]?.ToString();
                        lstSugar["AUG"].DD_HUMANUSE = dr[11]?.ToString();
                        lstSugar["SEP"].DD_HUMANUSE = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_BY_BEET_PROCESSORS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_BEET = dr[1]?.ToString();
                        lstSugar["NOV"].DD_BEET = dr[2]?.ToString();
                        lstSugar["DEC"].DD_BEET = dr[3]?.ToString();
                        lstSugar["JAN"].DD_BEET = dr[4]?.ToString();
                        lstSugar["FEB"].DD_BEET = dr[5]?.ToString();
                        lstSugar["MAR"].DD_BEET = dr[6]?.ToString();
                        lstSugar["APR"].DD_BEET = dr[7]?.ToString();
                        lstSugar["MAY"].DD_BEET = dr[8]?.ToString();
                        lstSugar["JUN"].DD_BEET = dr[9]?.ToString();
                        lstSugar["JUL"].DD_BEET = dr[10]?.ToString();
                        lstSugar["AUG"].DD_BEET = dr[11]?.ToString();
                        lstSugar["SEP"].DD_BEET = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_BY_CANE_REFINERS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_CANE = dr[1]?.ToString();
                        lstSugar["NOV"].DD_CANE = dr[2]?.ToString();
                        lstSugar["DEC"].DD_CANE = dr[3]?.ToString();
                        lstSugar["JAN"].DD_CANE = dr[4]?.ToString();
                        lstSugar["FEB"].DD_CANE = dr[5]?.ToString();
                        lstSugar["MAR"].DD_CANE = dr[6]?.ToString();
                        lstSugar["APR"].DD_CANE = dr[7]?.ToString();
                        lstSugar["MAY"].DD_CANE = dr[8]?.ToString();
                        lstSugar["JUN"].DD_CANE = dr[9]?.ToString();
                        lstSugar["JUL"].DD_CANE = dr[10]?.ToString();
                        lstSugar["AUG"].DD_CANE = dr[11]?.ToString();
                        lstSugar["SEP"].DD_CANE = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == BY_NON_REP.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].DD_NONREPORTERS = dr[1]?.ToString();
                        lstSugar["NOV"].DD_NONREPORTERS = dr[2]?.ToString();
                        lstSugar["DEC"].DD_NONREPORTERS = dr[3]?.ToString();
                        lstSugar["JAN"].DD_NONREPORTERS = dr[4]?.ToString();
                        lstSugar["FEB"].DD_NONREPORTERS = dr[5]?.ToString();
                        lstSugar["MAR"].DD_NONREPORTERS = dr[6]?.ToString();
                        lstSugar["APR"].DD_NONREPORTERS = dr[7]?.ToString();
                        lstSugar["MAY"].DD_NONREPORTERS = dr[8]?.ToString();
                        lstSugar["JUN"].DD_NONREPORTERS = dr[9]?.ToString();
                        lstSugar["JUL"].DD_NONREPORTERS = dr[10]?.ToString();
                        lstSugar["AUG"].DD_NONREPORTERS = dr[11]?.ToString();
                        lstSugar["SEP"].DD_NONREPORTERS = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_MISCELLANEOUS_SUPPLY_ADJUSTMENT.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].SUPPLY_ADJUSTEMENT = dr[1]?.ToString();
                        lstSugar["NOV"].SUPPLY_ADJUSTEMENT = dr[2]?.ToString();
                        lstSugar["DEC"].SUPPLY_ADJUSTEMENT = dr[3]?.ToString();
                        lstSugar["JAN"].SUPPLY_ADJUSTEMENT = dr[4]?.ToString();
                        lstSugar["FEB"].SUPPLY_ADJUSTEMENT = dr[5]?.ToString();
                        lstSugar["MAR"].SUPPLY_ADJUSTEMENT = dr[6]?.ToString();
                        lstSugar["APR"].SUPPLY_ADJUSTEMENT = dr[7]?.ToString();
                        lstSugar["MAY"].SUPPLY_ADJUSTEMENT = dr[8]?.ToString();
                        lstSugar["JUN"].SUPPLY_ADJUSTEMENT = dr[9]?.ToString();
                        lstSugar["JUL"].SUPPLY_ADJUSTEMENT = dr[10]?.ToString();
                        lstSugar["AUG"].SUPPLY_ADJUSTEMENT = dr[11]?.ToString();
                        lstSugar["SEP"].SUPPLY_ADJUSTEMENT = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_USE.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].TOTAL_USE = dr[1]?.ToString();
                        lstSugar["NOV"].TOTAL_USE = dr[2]?.ToString();
                        lstSugar["DEC"].TOTAL_USE = dr[3]?.ToString();
                        lstSugar["JAN"].TOTAL_USE = dr[4]?.ToString();
                        lstSugar["FEB"].TOTAL_USE = dr[5]?.ToString();
                        lstSugar["MAR"].TOTAL_USE = dr[6]?.ToString();
                        lstSugar["APR"].TOTAL_USE = dr[7]?.ToString();
                        lstSugar["MAY"].TOTAL_USE = dr[8]?.ToString();
                        lstSugar["JUN"].TOTAL_USE = dr[9]?.ToString();
                        lstSugar["JUL"].TOTAL_USE = dr[10]?.ToString();
                        lstSugar["AUG"].TOTAL_USE = dr[11]?.ToString();
                        lstSugar["SEP"].TOTAL_USE = dr[12]?.ToString();
                    }
                    else if (dataVal.ToUpper().Trim() == TOTAL_ENDING_STOCKS.ToUpper().Trim() && prevDataTable == "TABLE1")
                    {
                        lstSugar["OCT"].ENDING_STOCKS = dr[1]?.ToString();
                        lstSugar["NOV"].ENDING_STOCKS = dr[2]?.ToString();
                        lstSugar["DEC"].ENDING_STOCKS = dr[3]?.ToString();
                        lstSugar["JAN"].ENDING_STOCKS = dr[4]?.ToString();
                        lstSugar["FEB"].ENDING_STOCKS = dr[5]?.ToString();
                        lstSugar["MAR"].ENDING_STOCKS = dr[6]?.ToString();
                        lstSugar["APR"].ENDING_STOCKS = dr[7]?.ToString();
                        lstSugar["MAY"].ENDING_STOCKS = dr[8]?.ToString();
                        lstSugar["JUN"].ENDING_STOCKS = dr[9]?.ToString();
                        lstSugar["JUL"].ENDING_STOCKS = dr[10]?.ToString();
                        lstSugar["AUG"].ENDING_STOCKS = dr[11]?.ToString();
                        lstSugar["SEP"].ENDING_STOCKS = dr[12]?.ToString();
                    }
                }
            }

            if ((currDate.Year == DateTime.Now.Year) || (currDate.Year + 1 == DateTime.Now.Year && currDate.Month > 11))
            {
                commonRepo.ProcessQuery("DELETE T FROM (SELECT TOP (12) * FROM SUGAR_SUPPLY_DEMAND_DATA ORDER BY MONTH_DATE DESC ) T");
                commonRepo.ProcessQuery("DELETE T FROM(SELECT TOP(12) * FROM Sugar_ImportData ORDER BY DATADATE DESC) T");
            }

            if(sugarBuyerData != null)
            {
                NoOfRecords++;
                commonRepo.ProcessQuery($"DELETE FROM SUGAR_BUYER_DATA WHERE  MONTH_DATE = '{sugarBuyerData.MONTH_DATE}'");
                commonRepo.ProcessQuery(sugarBuyerData.PopulateQuery(1, 1, DateTime.Now));
            }
            foreach(KeyValuePair<string,SugarData> kv in lstSugar)
            {
                NoOfRecords++;
                commonRepo.ProcessQuery(kv.Value.PopulateQuery(1,1,DateTime.Now));
            }
            foreach (KeyValuePair<int, SugarImportData> kv in lstSugarImport)
            {
                NoOfRecords++;
                commonRepo.ProcessQuery(kv.Value.PopulateQuery(1, 1, DateTime.Now));
            }
            foreach (KeyValuePair<int, SugarRegionDelData> kv in lstSugarRegionDelData)
            {
                NoOfRecords++;
                commonRepo.ProcessQuery($"DELETE FROM Sugar_RegionDelData WHERE DataDate = '{kv.Value.MONTH_DATE}' AND Region = '{kv.Value.Region}'");
                commonRepo.ProcessQuery(kv.Value.PopulateQuery(1, 1, DateTime.Now));
            }
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
            Directory.CreateDirectory(path);
            RawFile = $@"{path}\all_smd_tables.xlsx";
          
            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, RawFile);
        }
    }
}



