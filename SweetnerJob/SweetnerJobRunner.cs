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

namespace SweetnerJob
{
    public class SweetnerJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private string RawFile;

        private Dictionary<string, DataTable> dictRawTables;
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

        public SweetnerJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>(); jobService = _unityContainer.Resolve<JobService>();
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

            string files = DownloadFiles(JobParams["URL"]);

            updateJobTime.endTime = DateTime.Now;
            updateJobTime.Message = "Success";
            updateJobTime.Status = "Completed";
            updateJobTime.FilePath = files;
            updateJobTime.FileType = "xlsx";
            updateJobTime.NoOfNewRecords = 10;
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
        private string GetQuarter(int Idnt)
        {
            switch (Idnt)
            {
                case 0:
                    return "QTR1";
                case 1:
                    return "QTR2";
                case 2:
                    return "QTR3";
                case 3:
                    return "QTR4";
            }
            return String.Empty;
        }
        private WorldSweetnerData GetCurrentWorldSweetnerData(Dictionary<string, WorldSweetnerData> DictWorldSweetnerData, string Area, string Region, int Year)
        {
            WorldSweetnerData CurrentSweetnerData = null;
            String key = $"{Area}{Region}{Year}";
            if (DictWorldSweetnerData.ContainsKey(key))
                CurrentSweetnerData = DictWorldSweetnerData[key];
            else
            {
                DictWorldSweetnerData.Add(key, new WorldSweetnerData());
                CurrentSweetnerData = DictWorldSweetnerData[key];
                CurrentSweetnerData.Region = Region;
                CurrentSweetnerData.ReportYear = Year;
                CurrentSweetnerData.Area = Area;
                CurrentSweetnerData.ReportDate = DateTime.Now.ToShortDateString();
            }
            return CurrentSweetnerData;
        }

        private string UpdateWorldSweetnerData(DataRow dr, string DataColumn, string Area,
            Dictionary<string, WorldSweetnerData> DictWorldSweetnerData, string Region, int ColumnCount)
        {
            int StartYear = 2000;
            WorldSweetnerData CurrentSweetnerData = null;
            for (int i = 2; i < ColumnCount; i++)
            {
                if (dr[i] != DBNull.Value)
                {
                    if (!String.IsNullOrEmpty(Region) && DataColumn != "BEGINNING STOCKS")
                        CurrentSweetnerData = GetCurrentWorldSweetnerData(DictWorldSweetnerData, Area, Region, StartYear);

                    switch (DataColumn)
                    {
                        case "BEGINNING STOCKS":
                            Region = dr[0]?.ToString().Trim();
                            CurrentSweetnerData = GetCurrentWorldSweetnerData(DictWorldSweetnerData, Area, Region, StartYear);
                            CurrentSweetnerData.BeginningStocks = Convert.ToInt64(dr[i]);
                            break;
                        case "TOTAL SUGAR PRODUCTION":
                            CurrentSweetnerData.TotalSugarProduction = Convert.ToInt64(dr[i]);
                            break;
                        case "TOTAL IMPORTS":
                            CurrentSweetnerData.TotalImports = Convert.ToInt64(dr[i]);
                            break;
                        case "TOTAL SUPPLY":
                            CurrentSweetnerData.TotalSupply = Convert.ToInt64(dr[i]);
                            break;
                        case "TOTAL EXPORTS":
                            CurrentSweetnerData.TotalExports = Convert.ToInt64(dr[i]);
                            break;
                        case "TOTAL USE":
                            CurrentSweetnerData.TotalUse = Convert.ToInt64(dr[i]);
                            break;
                        case "ENDING STOCKS":
                            CurrentSweetnerData.EndingStocks = Convert.ToInt64(dr[i]);
                            break;
                    }
                }
                StartYear++;
            }
            return Region;
        }
        private void ProcessWorldSweetnerData( DataTable dataTable )
        {
            string Region = String.Empty;
            string Area = String.Empty;
            Dictionary<string, WorldSweetnerData> DictWorldSweetnerData = new Dictionary<string, WorldSweetnerData>();

            foreach( DataRow dr in dataTable.Rows)
            {
                string dataColumn = dr[1]?.ToString();

                if( !String.IsNullOrEmpty(dataColumn))
                {
                    dataColumn = dataColumn.ToUpper().Trim();
                    if(dataColumn == "BEGINNING STOCKS" ||
                       dataColumn == "TOTAL SUGAR PRODUCTION" ||
                       dataColumn == "TOTAL IMPORTS" ||
                       dataColumn == "TOTAL SUPPLY" ||
                       dataColumn == "TOTAL EXPORTS" ||
                       dataColumn == "TOTAL USE" ||
                       dataColumn == "ENDING STOCKS" )
                    {
                        Region = UpdateWorldSweetnerData(dr, dataColumn, Area, DictWorldSweetnerData, Region, dataTable.Columns.Count);
                    }
                }
                else if( dr[0] != DBNull.Value)
                    Area = dr[0]?.ToString();
            }

            if (DictWorldSweetnerData.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM WORLD_SWEETENER_DATA");
                foreach (KeyValuePair<string, WorldSweetnerData> kv in DictWorldSweetnerData)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }
        private void ProcessFile(string file, string table)
        {
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", file);
           // DataSet data = new DataSet();
            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new DataTable();
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    dictRawTables.Add(table.ToUpper(), dataTable);
                    break;
                }
            }
            Dictionary<string, SugarData> lstSugar = new Dictionary<string, SugarData>();
         //   DataTable dt = data.Tables[0];

            //string prevDataTable = String.Empty;
            //string currMonth = String.Empty;
            //DateTime currDate = DateTime.MinValue;

            //string Val = String.Empty;
            //Val = dt.Columns[0].ColumnName;
            //if (String.IsNullOrEmpty(currMonth) && Val.Contains("SMD Report"))
            //{
            //    currMonth = Val.Substring(0, Val.IndexOf("SMD Report"));
            //    currDate = Convert.ToDateTime(currMonth);
            //    currMonth = GetMonth(currDate.Month);
            //}

            //if (dt.Rows != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        if (dr[0] == DBNull.Value)
            //            continue;

            //        string dataVal = dr[0]?.ToString();
            //        if (dataVal.ToUpper().Contains("TABLE 1."))
            //        {
            //            prevDataTable = "TABLE1";
            //            lstSugar.Add("OCT", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year - 1, 10,1).ToShortDateString()));
            //            lstSugar.Add("NOV", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year - 1, 11, 1).ToShortDateString()));
            //            lstSugar.Add("DEC", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year - 1, 12, 1).ToShortDateString()));
            //            lstSugar.Add("JAN", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 1, 1).ToShortDateString()));
            //            lstSugar.Add("FEB", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 2, 1).ToShortDateString()));
            //            lstSugar.Add("MAR", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 3, 1).ToShortDateString()));
            //            lstSugar.Add("APR", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 4, 1).ToShortDateString()));
            //            lstSugar.Add("MAY", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 5, 1).ToShortDateString()));
            //            lstSugar.Add("JUN", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 6, 1).ToShortDateString()));
            //            lstSugar.Add("JUL", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 7, 1).ToShortDateString()));
            //            lstSugar.Add("AUG", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 8, 1).ToShortDateString()));
            //            lstSugar.Add("SEP", new SugarData(DateTime.Now.ToShortDateString(), new DateTime(currDate.Year , 9, 1).ToShortDateString()));
            //            continue;
            //        }
            //        if (dataVal.ToUpper().Contains("TABLE 9:"))
            //        {
            //            prevDataTable = "TABLE9";
            //            continue;
            //        }
            //        if (dataVal.ToUpper().Contains("TABLE "))
            //        {
            //            prevDataTable = String.Empty;
            //            continue;
            //        }
            //        if (String.IsNullOrEmpty(prevDataTable))
            //            continue;
            //        if (dataVal.ToUpper().Trim() == BEG_STOCKS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].BEGINNNING_STOCKS = dr[1]?.ToString();
            //            lstSugar["NOV"].BEGINNNING_STOCKS = dr[2]?.ToString();
            //            lstSugar["DEC"].BEGINNNING_STOCKS = dr[3]?.ToString();
            //            lstSugar["JAN"].BEGINNNING_STOCKS = dr[4]?.ToString();
            //            lstSugar["FEB"].BEGINNNING_STOCKS = dr[5]?.ToString();
            //            lstSugar["MAR"].BEGINNNING_STOCKS = dr[6]?.ToString();
            //            lstSugar["APR"].BEGINNNING_STOCKS = dr[7]?.ToString();
            //            lstSugar["MAY"].BEGINNNING_STOCKS = dr[8]?.ToString();
            //            lstSugar["JUN"].BEGINNNING_STOCKS = dr[9]?.ToString();
            //            lstSugar["JUL"].BEGINNNING_STOCKS = dr[10]?.ToString();
            //            lstSugar["AUG"].BEGINNNING_STOCKS = dr[11]?.ToString();
            //            lstSugar["SEP"].BEGINNNING_STOCKS = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_PRO.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].TOTAL_PRODUCTION = dr[1]?.ToString();
            //            lstSugar["NOV"].TOTAL_PRODUCTION = dr[2]?.ToString();
            //            lstSugar["DEC"].TOTAL_PRODUCTION = dr[3]?.ToString();
            //            lstSugar["JAN"].TOTAL_PRODUCTION = dr[4]?.ToString();
            //            lstSugar["FEB"].TOTAL_PRODUCTION = dr[5]?.ToString();
            //            lstSugar["MAR"].TOTAL_PRODUCTION = dr[6]?.ToString();
            //            lstSugar["APR"].TOTAL_PRODUCTION = dr[7]?.ToString();
            //            lstSugar["MAY"].TOTAL_PRODUCTION = dr[8]?.ToString();
            //            lstSugar["JUN"].TOTAL_PRODUCTION = dr[9]?.ToString();
            //            lstSugar["JUL"].TOTAL_PRODUCTION = dr[10]?.ToString();
            //            lstSugar["AUG"].TOTAL_PRODUCTION = dr[11]?.ToString();
            //            lstSugar["SEP"].TOTAL_PRODUCTION = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_BEET.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].TP_BEET = dr[1]?.ToString();
            //            lstSugar["NOV"].TP_BEET = dr[2]?.ToString();
            //            lstSugar["DEC"].TP_BEET = dr[3]?.ToString();
            //            lstSugar["JAN"].TP_BEET = dr[4]?.ToString();
            //            lstSugar["FEB"].TP_BEET = dr[5]?.ToString();
            //            lstSugar["MAR"].TP_BEET = dr[6]?.ToString();
            //            lstSugar["APR"].TP_BEET = dr[7]?.ToString();
            //            lstSugar["MAY"].TP_BEET = dr[8]?.ToString();
            //            lstSugar["JUN"].TP_BEET = dr[9]?.ToString();
            //            lstSugar["JUL"].TP_BEET = dr[10]?.ToString();
            //            lstSugar["AUG"].TP_BEET = dr[11]?.ToString();
            //            lstSugar["SEP"].TP_BEET = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_CANE.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].TP_CANE = dr[1]?.ToString();
            //            lstSugar["NOV"].TP_CANE = dr[2]?.ToString();
            //            lstSugar["DEC"].TP_CANE = dr[3]?.ToString();
            //            lstSugar["JAN"].TP_CANE = dr[4]?.ToString();
            //            lstSugar["FEB"].TP_CANE = dr[5]?.ToString();
            //            lstSugar["MAR"].TP_CANE = dr[6]?.ToString();
            //            lstSugar["APR"].TP_CANE = dr[7]?.ToString();
            //            lstSugar["MAY"].TP_CANE = dr[8]?.ToString();
            //            lstSugar["JUN"].TP_CANE = dr[9]?.ToString();
            //            lstSugar["JUL"].TP_CANE = dr[10]?.ToString();
            //            lstSugar["AUG"].TP_CANE = dr[11]?.ToString();
            //            lstSugar["SEP"].TP_CANE = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_IMPORTS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].IMPORTS = dr[1]?.ToString();
            //            lstSugar["NOV"].IMPORTS = dr[2]?.ToString();
            //            lstSugar["DEC"].IMPORTS = dr[3]?.ToString();
            //            lstSugar["JAN"].IMPORTS = dr[4]?.ToString();
            //            lstSugar["FEB"].IMPORTS = dr[5]?.ToString();
            //            lstSugar["MAR"].IMPORTS = dr[6]?.ToString();
            //            lstSugar["APR"].IMPORTS = dr[7]?.ToString();
            //            lstSugar["MAY"].IMPORTS = dr[8]?.ToString();
            //            lstSugar["JUN"].IMPORTS = dr[9]?.ToString();
            //            lstSugar["JUL"].IMPORTS = dr[10]?.ToString();
            //            lstSugar["AUG"].IMPORTS = dr[11]?.ToString();
            //            lstSugar["SEP"].IMPORTS = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_SUPPLY.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].SUPPLY = dr[1]?.ToString();
            //            lstSugar["NOV"].SUPPLY = dr[2]?.ToString();
            //            lstSugar["DEC"].SUPPLY = dr[3]?.ToString();
            //            lstSugar["JAN"].SUPPLY = dr[4]?.ToString();
            //            lstSugar["FEB"].SUPPLY = dr[5]?.ToString();
            //            lstSugar["MAR"].SUPPLY = dr[6]?.ToString();
            //            lstSugar["APR"].SUPPLY = dr[7]?.ToString();
            //            lstSugar["MAY"].SUPPLY = dr[8]?.ToString();
            //            lstSugar["JUN"].SUPPLY = dr[9]?.ToString();
            //            lstSugar["JUL"].SUPPLY = dr[10]?.ToString();
            //            lstSugar["AUG"].SUPPLY = dr[11]?.ToString();
            //            lstSugar["SEP"].SUPPLY = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_EXPORTS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].EXPORTS = dr[1]?.ToString();
            //            lstSugar["NOV"].EXPORTS = dr[2]?.ToString();
            //            lstSugar["DEC"].EXPORTS = dr[3]?.ToString();
            //            lstSugar["JAN"].EXPORTS = dr[4]?.ToString();
            //            lstSugar["FEB"].EXPORTS = dr[5]?.ToString();
            //            lstSugar["MAR"].EXPORTS = dr[6]?.ToString();
            //            lstSugar["APR"].EXPORTS = dr[7]?.ToString();
            //            lstSugar["MAY"].EXPORTS = dr[8]?.ToString();
            //            lstSugar["JUN"].EXPORTS = dr[9]?.ToString();
            //            lstSugar["JUL"].EXPORTS = dr[10]?.ToString();
            //            lstSugar["AUG"].EXPORTS = dr[11]?.ToString();
            //            lstSugar["SEP"].EXPORTS = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == DOMESTIC_DEL.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DOMESTIC_DELIVERY = dr[1]?.ToString();
            //            lstSugar["NOV"].DOMESTIC_DELIVERY = dr[2]?.ToString();
            //            lstSugar["DEC"].DOMESTIC_DELIVERY = dr[3]?.ToString();
            //            lstSugar["JAN"].DOMESTIC_DELIVERY = dr[4]?.ToString();
            //            lstSugar["FEB"].DOMESTIC_DELIVERY = dr[5]?.ToString();
            //            lstSugar["MAR"].DOMESTIC_DELIVERY = dr[6]?.ToString();
            //            lstSugar["APR"].DOMESTIC_DELIVERY = dr[7]?.ToString();
            //            lstSugar["MAY"].DOMESTIC_DELIVERY = dr[8]?.ToString();
            //            lstSugar["JUN"].DOMESTIC_DELIVERY = dr[9]?.ToString();
            //            lstSugar["JUL"].DOMESTIC_DELIVERY = dr[10]?.ToString();
            //            lstSugar["AUG"].DOMESTIC_DELIVERY = dr[11]?.ToString();
            //            lstSugar["SEP"].DOMESTIC_DELIVERY = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == NON_HUM.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_NONHUMAN = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_NONHUMAN = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_NONHUMAN = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_NONHUMAN = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_NONHUMAN = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_NONHUMAN = dr[6]?.ToString();
            //            lstSugar["APR"].DD_NONHUMAN = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_NONHUMAN = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_NONHUMAN = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_NONHUMAN = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_NONHUMAN = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_NONHUMAN = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == RE_EXPORT_PROGRAM.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_PRODUCT_REEXPORT = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_PRODUCT_REEXPORT = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_PRODUCT_REEXPORT = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_PRODUCT_REEXPORT = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_PRODUCT_REEXPORT = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_PRODUCT_REEXPORT = dr[6]?.ToString();
            //            lstSugar["APR"].DD_PRODUCT_REEXPORT = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_PRODUCT_REEXPORT = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_PRODUCT_REEXPORT = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_PRODUCT_REEXPORT = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_PRODUCT_REEXPORT = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_PRODUCT_REEXPORT = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == HUMAN_USE.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_HUMANUSE = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_HUMANUSE = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_HUMANUSE = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_HUMANUSE = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_HUMANUSE = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_HUMANUSE = dr[6]?.ToString();
            //            lstSugar["APR"].DD_HUMANUSE = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_HUMANUSE = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_HUMANUSE = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_HUMANUSE = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_HUMANUSE = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_HUMANUSE = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_BY_BEET_PROCESSORS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_BEET = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_BEET = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_BEET = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_BEET = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_BEET = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_BEET = dr[6]?.ToString();
            //            lstSugar["APR"].DD_BEET = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_BEET = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_BEET = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_BEET = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_BEET = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_BEET = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_BY_CANE_REFINERS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_CANE = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_CANE = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_CANE = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_CANE = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_CANE = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_CANE = dr[6]?.ToString();
            //            lstSugar["APR"].DD_CANE = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_CANE = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_CANE = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_CANE = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_CANE = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_CANE = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == BY_NON_REP.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].DD_NONREPORTERS = dr[1]?.ToString();
            //            lstSugar["NOV"].DD_NONREPORTERS = dr[2]?.ToString();
            //            lstSugar["DEC"].DD_NONREPORTERS = dr[3]?.ToString();
            //            lstSugar["JAN"].DD_NONREPORTERS = dr[4]?.ToString();
            //            lstSugar["FEB"].DD_NONREPORTERS = dr[5]?.ToString();
            //            lstSugar["MAR"].DD_NONREPORTERS = dr[6]?.ToString();
            //            lstSugar["APR"].DD_NONREPORTERS = dr[7]?.ToString();
            //            lstSugar["MAY"].DD_NONREPORTERS = dr[8]?.ToString();
            //            lstSugar["JUN"].DD_NONREPORTERS = dr[9]?.ToString();
            //            lstSugar["JUL"].DD_NONREPORTERS = dr[10]?.ToString();
            //            lstSugar["AUG"].DD_NONREPORTERS = dr[11]?.ToString();
            //            lstSugar["SEP"].DD_NONREPORTERS = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_MISCELLANEOUS_SUPPLY_ADJUSTMENT.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].SUPPLY_ADJUSTEMENT = dr[1]?.ToString();
            //            lstSugar["NOV"].SUPPLY_ADJUSTEMENT = dr[2]?.ToString();
            //            lstSugar["DEC"].SUPPLY_ADJUSTEMENT = dr[3]?.ToString();
            //            lstSugar["JAN"].SUPPLY_ADJUSTEMENT = dr[4]?.ToString();
            //            lstSugar["FEB"].SUPPLY_ADJUSTEMENT = dr[5]?.ToString();
            //            lstSugar["MAR"].SUPPLY_ADJUSTEMENT = dr[6]?.ToString();
            //            lstSugar["APR"].SUPPLY_ADJUSTEMENT = dr[7]?.ToString();
            //            lstSugar["MAY"].SUPPLY_ADJUSTEMENT = dr[8]?.ToString();
            //            lstSugar["JUN"].SUPPLY_ADJUSTEMENT = dr[9]?.ToString();
            //            lstSugar["JUL"].SUPPLY_ADJUSTEMENT = dr[10]?.ToString();
            //            lstSugar["AUG"].SUPPLY_ADJUSTEMENT = dr[11]?.ToString();
            //            lstSugar["SEP"].SUPPLY_ADJUSTEMENT = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_USE.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].TOTAL_USE = dr[1]?.ToString();
            //            lstSugar["NOV"].TOTAL_USE = dr[2]?.ToString();
            //            lstSugar["DEC"].TOTAL_USE = dr[3]?.ToString();
            //            lstSugar["JAN"].TOTAL_USE = dr[4]?.ToString();
            //            lstSugar["FEB"].TOTAL_USE = dr[5]?.ToString();
            //            lstSugar["MAR"].TOTAL_USE = dr[6]?.ToString();
            //            lstSugar["APR"].TOTAL_USE = dr[7]?.ToString();
            //            lstSugar["MAY"].TOTAL_USE = dr[8]?.ToString();
            //            lstSugar["JUN"].TOTAL_USE = dr[9]?.ToString();
            //            lstSugar["JUL"].TOTAL_USE = dr[10]?.ToString();
            //            lstSugar["AUG"].TOTAL_USE = dr[11]?.ToString();
            //            lstSugar["SEP"].TOTAL_USE = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == TOTAL_ENDING_STOCKS.ToUpper().Trim() && prevDataTable == "TABLE1")
            //        {
            //            lstSugar["OCT"].ENDING_STOCKS = dr[1]?.ToString();
            //            lstSugar["NOV"].ENDING_STOCKS = dr[2]?.ToString();
            //            lstSugar["DEC"].ENDING_STOCKS = dr[3]?.ToString();
            //            lstSugar["JAN"].ENDING_STOCKS = dr[4]?.ToString();
            //            lstSugar["FEB"].ENDING_STOCKS = dr[5]?.ToString();
            //            lstSugar["MAR"].ENDING_STOCKS = dr[6]?.ToString();
            //            lstSugar["APR"].ENDING_STOCKS = dr[7]?.ToString();
            //            lstSugar["MAY"].ENDING_STOCKS = dr[8]?.ToString();
            //            lstSugar["JUN"].ENDING_STOCKS = dr[9]?.ToString();
            //            lstSugar["JUL"].ENDING_STOCKS = dr[10]?.ToString();
            //            lstSugar["AUG"].ENDING_STOCKS = dr[11]?.ToString();
            //            lstSugar["SEP"].ENDING_STOCKS = dr[12]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim().Contains(TOTAL_DEL.ToUpper().Trim()) &&
            //            dataVal.ToUpper().Trim().Contains(ACTUAL_WEIGTH.ToUpper().Trim()) && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].TOTAL_DEL_BEET = dr[1]?.ToString();
            //            lstSugar[currMonth].TOTAL_DEL_CANE = dr[2]?.ToString();
            //            lstSugar[currMonth].TOTAL_DEL = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == BAKERY_CER.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_BAKERY_CEREAL_REL_PROD = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_BAKERY_CEREAL_REL_PROD = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_BAKERY_CEREAL_REL_PROD = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == CONFECTIONERY_AND.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_CONF_REL_PROD = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_CONF_REL_PROD = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_CONF_REL_PROD = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == ICE_CRE.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_ICECREAM_DAIRY_PROD = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_ICECREAM_DAIRY_PROD = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_ICECREAM_DAIRY_PROD = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == BEVERAGES_BEV.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_BEVERAGES = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_BEVERAGES = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_BEVERAGES = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == CANNED_BOT.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_CANNED_BOTTLE_FROZEN = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_CANNED_BOTTLE_FROZEN = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_CANNED_BOTTLE_FROZEN = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == MULTIPLE_AND.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_MULTIPLE_OTHERFOOD = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_MULTIPLE_OTHERFOOD = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_MULTIPLE_OTHERFOOD = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == NON_USE.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_NONFOOD = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_NONFOOD = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_NONFOOD = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == HOTELS_RES.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_HOT_RES_INS = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_HOT_RES_INS = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_HOT_RES_INS = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == WHOLESALE_GRO.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_WS_JOBBERS_DEALERS = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_WS_JOBBERS_DEALERS = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_WS_JOBBERS_DEALERS = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == RETAIL_GRO.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_RETAILGROC_CHAINSTORE = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_RETAILGROC_CHAINSTORE = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_RETAILGROC_CHAINSTORE = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == GOVERNMENT_AGE.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_GOVT_AGEN = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_GOVT_AGEN = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_GOVT_AGEN = dr[3]?.ToString();
            //        }
            //        else if (dataVal.ToUpper().Trim() == ALL_OTH.ToUpper().Trim() && prevDataTable == "TABLE9")
            //        {
            //            lstSugar[currMonth].DEL_BEET_OTHERS = dr[1]?.ToString();
            //            lstSugar[currMonth].DEL_CANE_OTHERS = dr[2]?.ToString();
            //            lstSugar[currMonth].DEL_OTHERS = dr[3]?.ToString();
            //        }
            //    }
            //}
            //foreach(KeyValuePair<string,SugarData> kv in lstSugar)
            //{
            //    List<string> lstStr = kv.Value.PopulateQuery(1, 1, DateTime.Now);
            //    foreach(string str in lstStr)
            //    {
            //        commonRepo.ProcessQuery(str);
            //    }
            //}
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

        private int GetYear(DataRow dr)
        {
            int Year = 0;
            if (dr[0] != DBNull.Value)
            {
                if (int.TryParse(dr[0].ToString(), out Year))
                {
                    return Year;
                }
            }
            return Year;
        }

        private int GetStartColumn( DataTable dt, DataRow dr, int StartCol, int Increment)
        {
            for (int i = StartCol + Increment; i < dt.Columns.Count; i++)
            {
                if (dr[i] != DBNull.Value)
                {
                    double val = 0;
                    if (double.TryParse(dr[i].ToString(), out val))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private SugarUSDAPrices GetSugarUSDAPrice( int year, string reportIdnt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            string key = $"{year}{ reportIdnt}";
            if (!DictUSDASugarPrices.ContainsKey(key))
            {
                DictUSDASugarPrices[key] = new SugarUSDAPrices();
                SugarUSDAPrices sugarUSDA = DictUSDASugarPrices[key];
                sugarUSDA.ReportDate = DateTime.Now.ToShortDateString();
                sugarUSDA.ReportYear = year;
                sugarUSDA.ReportIdnt = reportIdnt;
            }
            return DictUSDASugarPrices[key];
        }
        private SweetnerUSDAHFCSImportsExports GetImportExportData(int year, string reportIdnt, string reportType, string From, string To,
            Dictionary<string, SweetnerUSDAHFCSImportsExports> DictUSDASugarPrices)
        {
            string key = $"{year}{ reportIdnt}{reportType}{From}{To}";
            if (!DictUSDASugarPrices.ContainsKey(key))
            {
                DictUSDASugarPrices[key] = new SweetnerUSDAHFCSImportsExports();
                SweetnerUSDAHFCSImportsExports sugarUSDA = DictUSDASugarPrices[key];
                sugarUSDA.ReportDate = DateTime.Now.ToShortDateString();
                sugarUSDA.Year = year;
                sugarUSDA.ReportIdnt = reportIdnt;
                sugarUSDA.FromPlace = From;
                sugarUSDA.ToPlace = To;
                sugarUSDA.ReportType = reportType;
            }
            return DictUSDASugarPrices[key];
        }
        private USDASugarProductionByState GetSugarUSDAProdByState(int year, string region, string reportIdnt, Dictionary<string, USDASugarProductionByState> DictUSDASugarProdByState)
        {
            string key = $"{year}{region}{ reportIdnt}";
            if (!DictUSDASugarProdByState.ContainsKey(key))
            {
                DictUSDASugarProdByState[key] = new USDASugarProductionByState();
                USDASugarProductionByState sugarUSDA = DictUSDASugarProdByState[key];
                sugarUSDA.ReportDate = DateTime.Now.ToShortDateString();
                sugarUSDA.Year = year;
                sugarUSDA.Region = region;
                sugarUSDA.ReportIdnt = reportIdnt;
            }
            return DictUSDASugarProdByState[key];
        }
        private void ProcessWorldRefinedSugar(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach(DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if( Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);
                   
                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRefinedSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRefinedSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    if(CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.WorldRefinedSugar = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.WorldRefinedSugar = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessWorldRawSugar(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRawSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRawSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.WorldRawSugar = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.WorldRawSugar = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessWorldRawSugarICE(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRawSugarICE = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.WorldRawSugarICE = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.WorldRawSugarICE = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.WorldRawSugarICE = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessUSRawSugar(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USRawSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USRawSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.USRawSugar = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.USRawSugar = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessUSWholesaleRefinedBeetSugar(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;

            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleRefinedBeetSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleRefinedBeetSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleRefinedBeetSugar = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleRefinedBeetSugar = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessUSRetailRefinedSugar(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USRetailRefinedSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USRetailRefinedSugar = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.USRetailRefinedSugar = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.USRetailRefinedSugar = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessUSWholesaleListDextrose(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);
                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleListDextrose = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleListDextrose = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleListDextrose = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleListDextrose = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }

        private void ProcessUSWholesaleListGlucose(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleListGlucose = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            sugarUSDA.USWholesaleListGlucose = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleListGlucose = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        sugarUSDA.USWholesaleListGlucose = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
            }
        }
        private void ProcessUSpricesforHFCS(DataTable dt, Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices)
        {
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            int FieldInd = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if( dr[1] != DBNull.Value && dr[1].ToString().ToUpper().Contains("JAN"))
                {
                    FieldInd++;
                }
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            switch (FieldInd)
                            {
                                case 1:
                                    sugarUSDA.SpotPriceHFCS42 = Convert.ToDouble(dr[i]);
                                    break;
                                case 2:
                                    sugarUSDA.WholeSaleSpotHFCS55 = Convert.ToDouble(dr[i]);
                                    break;
                                case 3:
                                    sugarUSDA.WholeSaleListHFCS42 = Convert.ToDouble(dr[i]);
                                    break;
                                case 4:
                                    sugarUSDA.WholeSaleListHFCS55 = Convert.ToDouble(dr[i]);
                                    break;
                            }
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, ReportIdnt, DictUSDASugarPrices);
                            switch (FieldInd)
                            {
                                case 1:
                                    sugarUSDA.SpotPriceHFCS42 = Convert.ToDouble(dr[i]);
                                    break;
                                case 2:
                                    sugarUSDA.WholeSaleSpotHFCS55 = Convert.ToDouble(dr[i]);
                                    break;
                                case 3:
                                    sugarUSDA.WholeSaleListHFCS42 = Convert.ToDouble(dr[i]);
                                    break;
                                case 4:
                                    sugarUSDA.WholeSaleListHFCS55 = Convert.ToDouble(dr[i]);
                                    break;
                            }
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "CALENDAR", DictUSDASugarPrices);
                        switch (FieldInd)
                        {
                            case 1:
                                sugarUSDA.SpotPriceHFCS42 = Convert.ToDouble(dr[CalendarCol]);
                                break;
                            case 2:
                                sugarUSDA.WholeSaleSpotHFCS55 = Convert.ToDouble(dr[CalendarCol]);
                                break;
                            case 3:
                                sugarUSDA.WholeSaleListHFCS42 = Convert.ToDouble(dr[CalendarCol]);
                                break;
                            case 4:
                                sugarUSDA.WholeSaleListHFCS55 = Convert.ToDouble(dr[CalendarCol]);
                                break;
                        }
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        SugarUSDAPrices sugarUSDA = GetSugarUSDAPrice(Year, "FISCAL", DictUSDASugarPrices);
                        switch (FieldInd)
                        {
                            case 1:
                                sugarUSDA.SpotPriceHFCS42 = Convert.ToDouble(dr[FiscalCol]);
                                break;
                            case 2:
                                sugarUSDA.WholeSaleSpotHFCS55 = Convert.ToDouble(dr[FiscalCol]);
                                break;
                            case 3:
                                sugarUSDA.WholeSaleListHFCS42 = Convert.ToDouble(dr[FiscalCol]);
                                break;
                            case 4:
                                sugarUSDA.WholeSaleListHFCS55 = Convert.ToDouble(dr[FiscalCol]);
                                break;
                        }
                    }
                }
            }
        }
        private int GetYearProductionArea(DataRow dr)
        {
            int Year = 0;
            if (dr[0] != DBNull.Value)
            {
                string val = dr[0].ToString();

                string strYear = val.Substring(0, val.Contains('/') ? val.IndexOf('/') : val.Length);
                if( int.TryParse(strYear, out Year) )
                {
                    return Year;
                }
            }
            return 0;
        }
        private double GetDecimalPrice( object val )
        {
            if( val != null && val != DBNull.Value )
            {
                double value = 0;
                if (Double.TryParse(val.ToString(), out value))
                {
                    return value;
                }
            }
            return 0;
        }

        private void ProcessUSDASugarProductionArea(DataTable dt)
        {
            Dictionary<string, USDASugarProductionArea> dictUSDAProdArea = new Dictionary<string, USDASugarProductionArea>();
            string Region = String.Empty;
            foreach(DataRow dr in dt.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    int Year = GetYear(dr);
                    if( Year > 0)
                    {
                        string Key = $"{Region}{Year}";
                        if(!dictUSDAProdArea.ContainsKey(Key))
                        {
                            dictUSDAProdArea.Add(Key, new USDASugarProductionArea());
                        }
                        USDASugarProductionArea usdaSugarProdAre = dictUSDAProdArea[Key];
                        usdaSugarProdAre.Region = Region;
                        usdaSugarProdAre.ReportDate = DateTime.Now.ToShortDateString();
                        usdaSugarProdAre.Year = Year;
                        usdaSugarProdAre.Total = GetDecimalPrice(dr[1]);
                        usdaSugarProdAre.AreaForSeed = GetDecimalPrice(dr[2]);
                        usdaSugarProdAre.AreaForSugar = GetDecimalPrice(dr[3]);
                        usdaSugarProdAre.PercForSeed = GetDecimalPrice(dr[4]);
                        usdaSugarProdAre.YieldForSugar = GetDecimalPrice(dr[5]);
                        usdaSugarProdAre.SugarCaneProduction = GetDecimalPrice(dr[6]);
                        usdaSugarProdAre.SugarProduction = GetDecimalPrice(dr[7]);
                        usdaSugarProdAre.RecoveryRate = GetDecimalPrice(dr[8]);
                        usdaSugarProdAre.YieldPerAcre = GetDecimalPrice(dr[9]);
                    }
                    else
                        Region = dr[0].ToString();
                }
            }

            if (dictUSDAProdArea.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sugar_USDASugarProductionArea");
                foreach (KeyValuePair<string, USDASugarProductionArea> kv in dictUSDAProdArea)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }

        private void ProcessUSDACaneBeetShare(DataTable dt)
        {
            Dictionary<string, USDACaneBeetShare> dictUSDAProdArea = new Dictionary<string, USDACaneBeetShare>();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    int Year = GetYearProductionArea(dr);
                    if (Year > 0)
                    {
                        string Key = $"{Year}";
                        if (!dictUSDAProdArea.ContainsKey(Key))
                        {
                            dictUSDAProdArea.Add(Key, new USDACaneBeetShare());
                        }
                        USDACaneBeetShare usdaSugarProdAre = dictUSDAProdArea[Key];
                        usdaSugarProdAre.ReportDate = DateTime.Now.ToShortDateString();
                        usdaSugarProdAre.Year = Year;
                        usdaSugarProdAre.BeetSugarProduced = GetDecimalPrice(dr[1]);
                        usdaSugarProdAre.CaneSugarProduced = GetDecimalPrice(dr[2]);
                        usdaSugarProdAre.BeetAndCaneProduced = GetDecimalPrice(dr[3]);
                        usdaSugarProdAre.BeetSharePercentOfProduction = GetDecimalPrice(dr[4]);
                        usdaSugarProdAre.CaneSharePercentOfProduction = GetDecimalPrice(dr[5]);
                    }
                }
            }

            if (dictUSDAProdArea.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sugar_USDACaneBeetShare");
                foreach (KeyValuePair<string, USDACaneBeetShare> kv in dictUSDAProdArea)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }
        private void ProcessUSDASupplyAndUse(DataTable dt)
        {
            List<SweetnerUSDAHFCSSupplyUse> lstSweetnerSupplyUse = new List<SweetnerUSDAHFCSSupplyUse>();
            string prevReportIdnt = "CALENDAR";
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    SweetnerUSDAHFCSSupplyUse sweetnerUSDASupplyUse = new SweetnerUSDAHFCSSupplyUse();
                    sweetnerUSDASupplyUse.ReportDate = DateTime.Now.ToShortDateString();
                    sweetnerUSDASupplyUse.Year = Year;
                    sweetnerUSDASupplyUse.ReportIdnt = prevReportIdnt;
                    sweetnerUSDASupplyUse.HFCS42Prod = GetDecimalPrice(dr[1]);
                    sweetnerUSDASupplyUse.HFCS55Prod = GetDecimalPrice(dr[2]);
                    sweetnerUSDASupplyUse.TotalProd = GetDecimalPrice(dr[3]);
                    sweetnerUSDASupplyUse.Imports = GetDecimalPrice(dr[4]);
                    sweetnerUSDASupplyUse.Supply = GetDecimalPrice(dr[5]);
                    sweetnerUSDASupplyUse.Exports = GetDecimalPrice(dr[6]);
                    sweetnerUSDASupplyUse.HFCS42Util = GetDecimalPrice(dr[7]);
                    sweetnerUSDASupplyUse.HFCS55Util = GetDecimalPrice(dr[8]);
                    sweetnerUSDASupplyUse.TotalUtil = GetDecimalPrice(dr[9]);
                    lstSweetnerSupplyUse.Add(sweetnerUSDASupplyUse);
			    }
                else if( dr[0]?.ToString().ToUpper() == "FISCAL")
                {
                    prevReportIdnt = "FISCAL";
                }
            }
            if (lstSweetnerSupplyUse.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sweetner_USDAHFCSSupplyUse");
                foreach (SweetnerUSDAHFCSSupplyUse sweetnerUSDASupplyUse in lstSweetnerSupplyUse)
                {
                    commonRepo.ProcessQuery(sweetnerUSDASupplyUse.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }
        private void ProcessUSDAProduction(DataTable dt)
        {
            Dictionary<string, USDAProduction> dictUSDAProd = new Dictionary<string, USDAProduction>();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    int Year = GetYearProductionArea(dr);
                    if (Year > 0)
                    {
                        string Key = $"{Year}";
                        if (!dictUSDAProd.ContainsKey(Key))
                        {
                            dictUSDAProd.Add(Key, new USDAProduction());
                        }
                        USDAProduction usdaSugarProd = dictUSDAProd[Key];
                        usdaSugarProd.ReportDate = DateTime.Now.ToShortDateString();
                        usdaSugarProd.Year = Year;
                        usdaSugarProd.Planted = GetDecimalPrice(dr[1]);
                        usdaSugarProd.Harvested = GetDecimalPrice(dr[2]);
                        usdaSugarProd.Produced = GetDecimalPrice(dr[3]);
                        usdaSugarProd.YieldPerAcre = GetDecimalPrice(dr[4]);
                        usdaSugarProd.SugarProduced = GetDecimalPrice(dr[5]);
                        usdaSugarProd.RecoveryRate = GetDecimalPrice(dr[6]);
                        usdaSugarProd.SugarYieldPerAcre = GetDecimalPrice(dr[7]);
                        usdaSugarProd.SugarType = "BEET";
                    }
                }
            }

            if (dictUSDAProd.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sugar_USDAProduction");
                foreach (KeyValuePair<string, USDAProduction> kv in dictUSDAProd)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }
        private void ProcessUSDASugarProductionByState(DataTable dt)
        {
            string Region = String.Empty;
            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;
            Dictionary<string, USDASugarProductionByState> dictUSDAProdByState = new Dictionary<string, USDASugarProductionByState>();
            foreach (DataRow dr in dt.Rows)
            {
                int Year = GetYear(dr);
                if (Year > 0)
                {
                    QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                    CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                    FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                    String ReportIdnt = String.Empty;
                    for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetMonth(i);
                            USDASugarProductionByState sugarUSDAProd = GetSugarUSDAProdByState(Year, Region,ReportIdnt, dictUSDAProdByState);
                            sugarUSDAProd.Production = Convert.ToDouble(dr[i]);
                        }
                    }
                    for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                    {
                        if (dr[i] != DBNull.Value)
                        {
                            ReportIdnt = GetQuarter(i - QuarterStartCol);
                            USDASugarProductionByState sugarUSDAProd = GetSugarUSDAProdByState(Year, Region,ReportIdnt, dictUSDAProdByState);
                            sugarUSDAProd.Production = Convert.ToDouble(dr[i]);
                        }
                    }
                    if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                    {
                        USDASugarProductionByState sugarUSDAProd = GetSugarUSDAProdByState(Year, Region,"CALENDAR", dictUSDAProdByState);
                        sugarUSDAProd.Production = Convert.ToDouble(dr[CalendarCol]);
                    }
                    if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                    {
                        USDASugarProductionByState sugarUSDAProd = GetSugarUSDAProdByState(Year, Region, "FISCAL", dictUSDAProdByState);
                        sugarUSDAProd.Production = Convert.ToDouble(dr[FiscalCol]);
                    }
                }
                else
                    Region = dr[0].ToString();
            }

            if (dictUSDAProdByState.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sugar_USDASugarProductionByState");
                foreach (KeyValuePair<string, USDASugarProductionByState> kv in dictUSDAProdByState)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
        }


        private Dictionary<int, string> GetHFCSOrder()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "HFCS42");
            dict.Add(2, "HFCS55");
            dict.Add(3, "Crystalline Fructose");
            dict.Add(4, "Total Fructose");
            return dict;
        }

        private void UpdateValue(SweetnerUSDAHFCSImportsExports sugarUSDA, double val, int currentRow, Dictionary<int, string> dictOrder)
        {
            switch(dictOrder[currentRow])
            {
                case "HFCS42":
                    sugarUSDA.HFCS42 = val;
                    break;
                case "HFCS55":
                    sugarUSDA.HFCS55 = val;
                    break;
                case "Crystalline Fructose":
                    sugarUSDA.CrystallineFructose = val;
                    break;
                case "Total Fructose":
                    sugarUSDA.TotalFructose = val;
                    break;
            }
        }
        private void ProcessImportExport(DataTable dt, string reportType, string From, string To)
        {
            Dictionary<int, string> dictOrder = GetHFCSOrder();
            Dictionary<string, SweetnerUSDAHFCSImportsExports> dictSweetnerHFCSImportExport = new Dictionary<string, SweetnerUSDAHFCSImportsExports>();

            int MonthStartCol = 1;
            int QuarterStartCol = 0;
            int CalendarCol = 0;
            int FiscalCol = 0;

            int currentRow = 1;
            bool bProcessStarted = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    int Year = GetYear(dr);
                    if (Year > 0)
                    {
                        bProcessStarted = true;
                        QuarterStartCol = GetStartColumn(dt, dr, MonthStartCol, 12);
                        CalendarCol = GetStartColumn(dt, dr, QuarterStartCol, 4);
                        FiscalCol = GetStartColumn(dt, dr, CalendarCol, 1);

                        String ReportIdnt = String.Empty;
                        for (int i = MonthStartCol; i < MonthStartCol + 12; i++)
                        {
                            if (dr[i] != DBNull.Value)
                            {
                                ReportIdnt = GetMonth(i);
                                SweetnerUSDAHFCSImportsExports sugarUSDA = GetImportExportData(Year, ReportIdnt, reportType, From, To, dictSweetnerHFCSImportExport);
                                UpdateValue(sugarUSDA, GetDecimalPrice(dr[i]), currentRow, dictOrder);
                            }
                        }
                        for (int i = QuarterStartCol; i < QuarterStartCol + 4; i++)
                        {
                            if (dr[i] != DBNull.Value)
                            {
                                ReportIdnt = GetQuarter(i - QuarterStartCol);
                                SweetnerUSDAHFCSImportsExports sugarUSDA = GetImportExportData(Year, ReportIdnt, reportType, From, To, dictSweetnerHFCSImportExport);
                                UpdateValue(sugarUSDA, GetDecimalPrice(dr[i]), currentRow, dictOrder);
                            }
                        }
                        if (CalendarCol > 0 && dr[CalendarCol] != DBNull.Value)
                        {
                            SweetnerUSDAHFCSImportsExports sugarUSDA = GetImportExportData(Year, "CALENDAR", reportType, From, To, dictSweetnerHFCSImportExport);
                            UpdateValue(sugarUSDA, GetDecimalPrice(dr[CalendarCol]), currentRow, dictOrder);
                        }
                        if (FiscalCol > 0 && dr[FiscalCol] != DBNull.Value)
                        {
                            SweetnerUSDAHFCSImportsExports sugarUSDA = GetImportExportData(Year, "FISCAL", reportType, From, To, dictSweetnerHFCSImportExport);
                            UpdateValue(sugarUSDA, GetDecimalPrice(dr[FiscalCol]), currentRow, dictOrder);
                        }
                    }
                }
                else if (bProcessStarted)
                {
                    currentRow++;
                }
            }

            foreach (KeyValuePair<string, SweetnerUSDAHFCSImportsExports> kv in dictSweetnerHFCSImportExport)
            {
                commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
            }
        }
        
        private string DownloadFiles(string rawUrl)
        {
            var web = new HtmlWeb();

            HtmlAgilityPack.HtmlDocument resultat = web.Load(rawUrl);
            var data = resultat.DocumentNode.SelectNodes("//td[@class='DataFileItem']");

            Dictionary<string, string> TableFiles = new Dictionary<string, string>();
            foreach (HtmlNode node in data)
            {
                var innerText = node.InnerHtml;
                if (innerText.CaseInsensitiveContains("TABLE"))
                {
                    int startlen = innerText.IndexOf("href=\"", StringComparison.CurrentCultureIgnoreCase) + 6;
                    int endlen = innerText.IndexOf("\"", startlen, StringComparison.CurrentCultureIgnoreCase);
                    string url = innerText.Substring(startlen, endlen - startlen);

                    startlen = url.IndexOf("TABLE", StringComparison.CurrentCultureIgnoreCase);
                    endlen = url.IndexOf("?v=0", startlen, StringComparison.CurrentCultureIgnoreCase);
                    string tablename = url.Substring(startlen, endlen - startlen);

                    string fullUrl = $"https://www.ers.usda.gov{url}";
                    string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
                    Directory.CreateDirectory(path);
                    RawFile = $@"{path}\{tablename}";

                    WebClient Client = new WebClient();
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                    Client.DownloadFile(fullUrl, RawFile);

                    string tb = tablename.Substring(0, tablename.IndexOf('.'));
                    TableFiles[tb.ToUpper()] = RawFile;
                    ProcessFile(RawFile, tb);
                }
            }

            Dictionary<string, SugarUSDAPrices> DictUSDASugarPrices = new Dictionary<string, SugarUSDAPrices>();
            Dictionary<string, SweetnerUSDAHFCSImportsExports> dictSweetnerHFCSImportExport = new Dictionary<string, SweetnerUSDAHFCSImportsExports>();
            if( dictRawTables.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sweetner_USDAHFCSImportsExports");

            }
            string file = String.Empty;
            foreach (KeyValuePair<string, DataTable> kv in dictRawTables)
            {
                switch (kv.Key.ToUpper())
                {
                    case "TABLE01":
                    case "TABLE1":
                        file += $"World Sweetener Data:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessWorldSweetnerData(kv.Value);
                        break;
                    case "TABLE02":
                    case "TABLE2":
                        file += $"World Refined Sugar:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessWorldRefinedSugar(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE03A":
                    case "TABLE3A":
                        file += $"World Raw Sugar:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessWorldRawSugar(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE03B":
                    case "TABLE3B":
                        file += $"World Raw Sugar ICE:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessWorldRawSugarICE(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE04":
                    case "TABLE4":
                        file += $"US Raw Sugar:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSRawSugar(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE05":
                    case "TABLE5":
                        file += $"US Refined Beet Sugar:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSWholesaleRefinedBeetSugar(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE06":
                    case "TABLE6":
                        file += $"US Retail Sugar:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSRetailRefinedSugar(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE07":
                    case "TABLE7":
                        file += $"US Wholesale Dextrose:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSWholesaleListDextrose(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE08":
                    case "TABLE8":
                        file += $"US Wholesale Glucose:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSWholesaleListGlucose(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE09":
                    case "TABLE9":
                        file += $"US Prices HFCS:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSpricesforHFCS(kv.Value, DictUSDASugarPrices);
                        break;
                    case "TABLE15":
                        file += $"USDA Sugar Production:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSDASugarProductionArea(kv.Value);
                        break;
                    case "TABLE16":
                        file += $"USDA CaneBeet Share:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSDACaneBeetShare(kv.Value);
                        break;
                    case "TABLE17":
                        file += $"USDA Production:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSDAProduction(kv.Value);
                        break;
                    case "TABLE18":
                        file += $"USDA Production By State:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSDASugarProductionByState(kv.Value);
                        break;
                    case "TABLE30":
                        file += $"USDA Supply and Use:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessUSDASupplyAndUse(kv.Value);
                        break;
                    case "TABLE34A":
                        file += $"Export US Mexico:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessImportExport(kv.Value, "Export", "US", "Mexico");
                        break;
                    case "TABLE34B":
                        file += $"Export US All:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessImportExport(kv.Value, "Export", "US", "All");
                        break;
                    case "TABLE35A":
                        file += $"Import US Mexico:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessImportExport(kv.Value, "Import", "US", "Mexico");
                        break;
                    case "TABLE35B":
                        file += $"Import All Mexico:{TableFiles[kv.Key.ToUpper()]};";
                        ProcessImportExport(kv.Value, "Import", "All", "Mexico");
                        break;
                }
            }

            file.TrimEnd(';');
            if (DictUSDASugarPrices.Count > 0)
            {
                commonRepo.ProcessQuery("DELETE FROM Sugar_USDAPrices");
                foreach (KeyValuePair<string, SugarUSDAPrices> kv in DictUSDASugarPrices)
                {
                    commonRepo.ProcessQuery(kv.Value.PopulateQuery(JobID, 0, DateTime.Now));
                }
            }
            return file;
        }
    }
}



