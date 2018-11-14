using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using McF.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;

namespace CattleOnFeedJob
{
    public enum BHType
    {
        None = 0,
        Eggs,
        Chicks
    }
    public class CFJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private string DataSource;
        private int JobID;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
    
        private string ReportDate;

        public CFJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>();
        }

        private string[] SplitCSV(string input)
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
            List<string> list = new List<string>();
            string curr = null;
            foreach (Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length)
                {
                    list.Add("");
                }
                list.Add(curr.TrimStart(','));
            }
            return list.ToArray();
        }

        public bool RUNJob(Dictionary<string, string> JobParams, string dataSource, int jobID)
        {
            JobStartTime = DateTime.Now;
            JobID = jobID;
            DataSource = dataSource;
            //Update JobStatus to Running and StartTime
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

            Dictionary<string, string> data = McFHTMLHelper.ParseHTMLFile(JobParams["URL"], reportDataDate);
            string RawData = String.Empty;
            if (data.Count > 0)
            {
                foreach (KeyValuePair<string, string> kyData in data)
                {
                    string RawFile = DownloadFile(kyData.Value);
                    ProcessFile(RawFile);

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
        private void ProcessFile(string rawFile)
        {
            string line, RecordType = String.Empty;
            System.IO.StreamReader file = new System.IO.StreamReader(rawFile);
            int Code = 0, row = 1;
            Dictionary<int,Dictionary<string, string>> DataRow = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> HeaderRow = new Dictionary<int, string>();
          
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    string[] fields = SplitCSV(line);
                    int column = 1;
                    string dataKey = string.Empty;
                    string dataVal = string.Empty;
                    foreach (string field in fields)
                    {
                        string val = field.Replace("\"", "").Trim().ToUpper();
                        switch (column)
                        {
                            case 1:
                                Code = Convert.ToInt32(val);
                                if(Code == 1 || Code == 2 || Code == 3)
                                {
                                    if (!DataRow.ContainsKey(Code))
                                        DataRow[Code] = new Dictionary<string, string>();
                                }
                                break;
                            case 2:
                                RecordType = val.ToUpper();
                                break;
                            default:
                                switch (RecordType)
                                {
                                    case "D":
                                        if (!String.IsNullOrEmpty(val) && (Code == 1 || Code == 2 || Code ==3 ) )
                                        {
                                            if (column == 3)
                                                dataKey = val;
                                            else if (column == 5)
                                                dataVal = val;
                                        }
                                        break;
                                }
                                break;
                        }
                        column++;
                    }
                    if ( !String.IsNullOrEmpty(dataKey) && !String.IsNullOrEmpty(dataVal))
                    {
                        DataRow[Code].Add(dataKey, dataVal);
                        dataKey = string.Empty;
                        dataVal = string.Empty;
                    }
                    row++;
                }
                catch (Exception ex)
                {

                }
            }
            file.Close();

            DateTime dt = DateTime.Now;

            List<CFData> lstCFData = new List<CFData>();
            if( DataRow.Count > 0)
            {
                if (DataRow.ContainsKey(1))
                {
                    CFData cfData = FillCFdata(DataRow[1]);
                    if (DataRow.ContainsKey(3))
                    {
                        cfData.SteersAndCalves = DataRow[3].ElementAt(0).Value;
                        cfData.HeifersAndCalves = DataRow[3].ElementAt(1).Value;
                    }
                    lstCFData.Add(cfData);
                }
                if(DataRow.ContainsKey(2))
                     lstCFData.Add(FillCFdata(DataRow[2]));
            }
            foreach (CFData cfData in lstCFData)
            {
                foreach (string str in cfData.PopulateQuery(11, 0, dt))
                {
                    commonRepo.ProcessQuery(str);
                }
            }
        }

        private CFData FillCFdata(Dictionary<string, string> dictCol)
        {
            CFData cfData = new CFData();
            cfData.ReportDate = ReportDate;
            int i = 0;
            foreach (KeyValuePair<string, string> kv in dictCol)
            {
                switch (i++)
                {
                    case 0:
                        if(kv.Key.ToUpper().Contains("ON FEED"))
                        {
                            string Month = kv.Key.Substring(7, kv.Key.Length - 7);
                            cfData.BeginDate = Month;
                            cfData.Begin_Inventory = kv.Value;
                        }
                        break;
                    case 1:
                        cfData.Placed_During_Month = kv.Value;
                        break;
                    case 2:
                        cfData.Marketed_During_Month = kv.Value;
                        break;
                    case 3:
                        cfData.Other_Disappearances = kv.Value;
                        break;
                    case 4:
                        if (kv.Key.ToUpper().Contains("ON FEED"))
                        {
                            string Month = kv.Key.Substring(7, kv.Key.Length - 7);
                            cfData.EndDate = Month;
                            cfData.Month_Ending_Inventory = kv.Value;
                        }
                        break;
                }
            }
            return cfData;
        }
        private string DownloadFile(string rawUrl)
        {
            rawFileInfo = new RawFilesInfo();
            rawFileInfo.URL = rawUrl;//"http://usda.mannlib.cornell.edu/usda/nass/CattOnFe//2010s/2018/CattOnFe-06-23-2018.zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
            //string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", "06-23-2018" /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";
         //   ReportDate = "2018-06-23";
            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\cofd_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            if (!File.Exists(rawFile))
                rawFile = $@"{path}\COFD_ALL.CSV";
            return rawFile;
        }
    }
}
