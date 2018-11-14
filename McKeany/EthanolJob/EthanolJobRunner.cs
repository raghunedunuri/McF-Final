using McF.Business;
using McF.Common.Interface;
using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Excel = Microsoft.Office.Interop.Excel;

namespace EthanolJob
{
    public class EthanolJobRunner : IJobRunner
    {
        private IEthanolService ethanolService;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private string StockFile;
        private string PlantFile;
        private int NoOfDays;
        private Dictionary<DateTime, Dictionary<string, EthanolData>> dictEthanolData;
        

        public EthanolJobRunner(IUnityContainer _unityContainer)
        {
            ethanolService = _unityContainer.Resolve<EthanolService>();
            jobService = _unityContainer.Resolve<JobService>();
        }
        public bool RUNJob(Dictionary<string, string> jobParams, string dataSource, int jobID)
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

            string ReportDate = DateTime.Now.ToShortDateString();
            if (jobParams.ContainsKey("DATE"))
                ReportDate = jobParams["DATE"];

            StockFile = jobParams["EndingStock"];
            PlantFile = jobParams["PlantProduction"];
            NoOfDays = Convert.ToInt32(jobParams["NoOfDays"]);

            DateTime reportDataDate = Convert.ToDateTime(ReportDate);
            DownloadFiles();

            dictEthanolData = new Dictionary<DateTime,Dictionary<string, EthanolData>>();
            ProcessFile(StockFile);
            ProcessFile(PlantFile, true);

            int NoofRecords = 0;
            int TotalRecords = 0;
            List<EthanolData> lstEthanolData = new List<EthanolData>();

            if (NoOfDays == 0)
                NoOfDays = int.MaxValue;

            foreach( KeyValuePair<DateTime, Dictionary<string,EthanolData>> kv in dictEthanolData.Reverse())
            {
                if ( kv.Key >= reportDataDate ||  TotalRecords < NoOfDays)
                {
                    foreach (KeyValuePair<string, EthanolData> dv in kv.Value)
                    {
                        NoofRecords++;
                        lstEthanolData.Add(dv.Value);
                    }
                }
                TotalRecords++;
            }
            ethanolService.PopulateData(lstEthanolData);

            updateJobTime.endTime = DateTime.Now;
            updateJobTime.Message = "Success";
            updateJobTime.Status = "Completed";
            updateJobTime.FilePath = $"EndingStock:{StockFile};PlantProduction:{PlantFile}";
            updateJobTime.FileType = "web";
            updateJobTime.NoOfNewRecords = NoofRecords;
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
        private void AddSymbolData( string symbol, DateTime dt, long value, bool bPlanted )
        {
            EthanolData ethanolData = null;
            if (!dictEthanolData[dt].ContainsKey(symbol))
            {
                dictEthanolData[dt][symbol] = new EthanolData();
            }
            ethanolData = dictEthanolData[dt][symbol];
            ethanolData.Date = dt;
            ethanolData.Symbol = symbol;
            if (bPlanted)
                ethanolData.Planted = value;
            else
                ethanolData.Stock = value;
        }
        private void ProcessFile( string file, bool bPlanted = false )
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(file);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[2];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            for (int i = 4; i <= rowCount; i++)
            {
                DateTime dt = FromExcelSerialDate(xlRange.Cells[i, 1].Value2);
                if (!dictEthanolData.ContainsKey(dt))
                    dictEthanolData[dt] = new Dictionary<string, EthanolData>();
                AddSymbolData("US", dt, ConvertToInt(xlRange.Cells[i, 2].Value2), bPlanted);
                AddSymbolData("EastCoast_PADD1", dt, ConvertToInt(xlRange.Cells[i, 3].Value2), bPlanted);
                AddSymbolData("MidWest_PADD2", dt, ConvertToInt(xlRange.Cells[i, 4].Value2), bPlanted);
                AddSymbolData("GulfCoast_PADD3", dt, ConvertToInt(xlRange.Cells[i, 5].Value2), bPlanted);
                AddSymbolData("RockyMountain_PADD4", dt, ConvertToInt(xlRange.Cells[i, 6].Value2), bPlanted);
                AddSymbolData("WestCoast_PADD5", dt, ConvertToInt(xlRange.Cells[i, 7].Value2), bPlanted);
            }
            xlWorkbook.Close();
        }
        private void DownloadFiles()
        {
            EthanolRawData ethanolData = new EthanolRawData(); //ethanolService.GetRawData();
            ethanolData.StockSourceFile = StockFile;
            ethanolData.PlantedSourceFile = PlantFile;
            ethanolData.NoOfDays = NoOfDays;

            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
            StockFile = $@"{path}\Stock.xls";
            PlantFile = $@"{path}\Plant.xls";

            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(ethanolData.StockSourceFile, StockFile);
            Client.DownloadFile(ethanolData.PlantedSourceFile, PlantFile);
        }

    }
}


