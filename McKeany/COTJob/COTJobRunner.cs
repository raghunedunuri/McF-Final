using AutoMapper;
using McF.Business;
using McF.Common.Interface;
using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity;
using System.IO.Compression;
using System.IO;

namespace COTJob
{
    public class COTJobRunner : IJobRunner
    {
        private ICOTService cotService;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
        private DateTime ReportDataDate;
        private int NoOfDays;
        private int noOfRecords;
        public COTJobRunner( IUnityContainer _unityContainer)
        {
            cotService = _unityContainer.Resolve<COTService>();
            jobService = _unityContainer.Resolve<JobService>();
            noOfRecords = 0;
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
                UserID = 0
            };
            jobService.UpdateJobStatus(updateJobTime);
            
            // URL: https://www.cftc.gov/files/dea/history/fut_disagg_xls_[%YEAR%].zip
            string File = JobParams["URL"];
            File = File.Replace("[%YEAR%]", DateTime.Now.Year.ToString());
            File = File.Replace("[%YEAR%]", DateTime.Now.Year.ToString());
            string ReportDate = DateTime.Now.ToShortDateString();
            if (JobParams.ContainsKey("DATE"))
                ReportDate = JobParams["DATE"];

            NoOfDays = Convert.ToInt32(JobParams["NoOfDays"]);

            ReportDataDate = Convert.ToDateTime(ReportDate);

            string rawFile = DownloadFile(File);

            updateJobTime.endTime = DateTime.Now;
            updateJobTime.Message = "Success";
            updateJobTime.Status = "Completed";
            updateJobTime.FilePath = $"URL:{rawFile}";
            updateJobTime.FileType = "xlsx";
            updateJobTime.NoOfNewRecords = noOfRecords;
            jobService.UpdateJobStatus(updateJobTime);
            return true;
        }

        private string DownloadFile( string rawFileURL )
        {
            rawFileInfo = cotService.GetRawData();
            rawFileInfo.URL = rawFileURL; //"https://www.cftc.gov/files/dea/history/fut_disagg_xls_[[CURRENTYEAR]].zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
            string urlFile = rawFileURL; //rawFileInfo.URL.Replace("[[CURRENTYEAR]]", DateTime.Now.Year.ToString());
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";

            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(urlFile, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\f_year.xls";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            Dictionary<DateTime, COTData> dict = new Dictionary<DateTime, COTData>(); 
            if (File.Exists(rawFile))
            {
                DataSet ds = ParseFile(rawFile);

                List<COTData> lstCOTData = new List<COTData>();
                noOfRecords = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        DateTime recordTime = Convert.ToDateTime(dr["Report_Date_as_MM_DD_YYYY"]);
                        COTData data = Mapper.Map<COTData>(dr);
                        dict.Add(recordTime, data);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                int TotalRecords = 0;
                foreach (KeyValuePair<DateTime, COTData> kv in dict.Reverse())
                {
                    if (TotalRecords < NoOfDays || kv.Key >= ReportDataDate)
                    {
                        lstCOTData.Add(kv.Value);
                        noOfRecords++;
                    }
                }
                cotService.PopulateData(lstCOTData);
            }
            return rawFile;
        }



        private DataSet ParseFile( string fileName)
        {
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", fileName);
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
            return data;
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

    }
}
