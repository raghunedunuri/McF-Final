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

namespace BHJob
{
    public enum BHType
    {
        None = 0,
        Eggs,
        Chicks
    }
    public class BHJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private string DataSource;
        private int JobID;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
        private string ReportDate;
        private const string selectStates = "SELECTED STATES";
        private Dictionary<string, BHField> DictFields;
        private int NoOfRecords;
        public BHJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>();
            NoOfRecords = 0;
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
            Dictionary<int, string> DataRow = new Dictionary<int, string>();
            Dictionary<int, string> HeaderRow = new Dictionary<int, string>();

            DictFields = new Dictionary<string, BHField>();
            BHType bhType = BHType.None;
            //string ReportDate = "2018-07-11";
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    string[] fields = SplitCSV(line);
                    int column = 1;

                    foreach (string field in fields)
                    {
                        string val = field.Replace("\"", "").Trim().ToUpper();
                        switch (column)
                        {
                            case 1:
                                Code = Convert.ToInt32(val);
                                break;
                            case 2:
                                RecordType = val.ToUpper();
                                break;
                            default:
                                switch (RecordType)
                                {
                                    case "T":
                                        if (val.Contains(selectStates))
                                        {
                                            if(val.Contains("EGGS"))
                                            {
                                                bhType = BHType.Eggs;
                                            }
                                            else if (val.Contains("CHICKS"))
                                            {
                                                bhType = BHType.Chicks;
                                            }
                                            HeaderRow.Clear();
                                        }
                                        break;
                                    case "U":
                                        break;
                                    case "H":
                                        if (!String.IsNullOrEmpty(val) 
                                            && CommonOperations.IsMonthType(val))
                                        {
                                            if (!HeaderRow.ContainsKey(column))
                                                HeaderRow[column] = val;
                                            else
                                                HeaderRow[column] += $" {val}";
                                        }
                                        break;
                                    case "D":
                                        if (!String.IsNullOrEmpty(val))
                                        {
                                            DataRow[column] = val;
                                        }
                                        break;
                                }
                                break;
                        }
                        column++;
                    }
                    if (RecordType == "D" && DataRow.Count > 0)
                    {
                        string Region = DataRow.ElementAt(0).Value;
                        for (int i = 1; i < DataRow.Count; i++)
                        {
                            string WeekEnding = HeaderRow[DataRow.ElementAt(i).Key];
                            string Value = DataRow.ElementAt(i).Value;
                            string Key = $"{WeekEnding}{ReportDate}{Region}";
                            if(!DictFields.ContainsKey(Key))
                            {
                                DictFields.Add(Key, new BHField(ReportDate, Region, WeekEnding));
                            }
                            switch(bhType)
                            {
                                case BHType.Eggs:
                                    DictFields[Key].EggsSet = Value;
                                    break;
                                case BHType.Chicks:
                                    DictFields[Key].ChicksPlaced = Value;
                                    break;
                            }
                        }  
                    }
                    row++;
                }
                catch (Exception ex)
                {

                }
            }
            file.Close();

            DateTime dt = DateTime.Now;

            foreach(KeyValuePair<string, BHField> kv in DictFields)
            {
                foreach (string str in kv.Value.PopulateQuery(11, 0, dt))
                {
                    commonRepo.ProcessQuery(str);
                }
            }
        }

        private string DownloadFile(string rawUrl)
        {
            rawFileInfo = new RawFilesInfo();
            rawFileInfo.URL = rawUrl;//"http://usda.mannlib.cornell.edu/usda/current/BroiHatc/BroiHatc-07-11-2018.zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
            //string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", "07-11-2018" /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";

            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\brls_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            if (!File.Exists(rawFile))
                rawFile = $@"{path}\brls_all.csv";
            return rawFile;
        }
    }
}
