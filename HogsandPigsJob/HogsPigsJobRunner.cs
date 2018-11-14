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

namespace HogsPigsJob
{
    public class HogsPigsJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private string DataSource;
        private int JobID;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;

        private string ReportDate;

        public HogsPigsJobRunner(IUnityContainer _unityContainer)
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

        private void FillValue(HPData hpData, string key, string value, int code)
        {
            
            if(key.Contains("ALL HOGS AND PIGS"))
            {
                hpData.Inventory = value;
            }
            else if (key.Contains("KEPT FOR BREEDING"))
            {
                hpData.KeptForBreeding = value;
            }
            else if (key.Contains("MARKET"))
            {
                hpData.Market_Inventory = value;
            }
            else if (key.Contains("UNDER 50 POUNDS") || key.Contains("UNDER 60 POUNDS") )
            {
                hpData.Market_Inventory_50LB = value;
            }
            else if (key.Contains("50-119 POUNDS") || key.Contains("60-119 POUNDS") )
            {
                hpData.Market_Inventory_50_119LB = value;
            }
            else if (key.Contains("120-179 POUNDS"))
            {
                hpData.Market_Inventory_120_179LB = value;
            }
            else if (key.Contains("180 POUNDS AND OVER"))
            {
                hpData.Market_Inventory_180LB = value;
            }
            else if (key.Contains("SOWS FARROWING"))
            {
                hpData.Sows_Farrowing = value;
            }
            else if (key.Contains("PIG CROP"))
            {
                hpData.Pig_Crop = value;
            }
            else if (key.Contains("PIGS PER LITTER"))
            {
                hpData.Pig_Per_Litter = value;
            }
        }
        public class DataMap
        {
            public DataMap(string year)
            {
                Year = year;
            }
            public string Year { get; set; }
            public string DataKey { get; set; }
            public string DataValue { get; set; }
            public string DataYear { get; set; }
        }
        private void ProcessFile(string rawFile)
        {
            string line, RecordType = String.Empty;
            System.IO.StreamReader file = new System.IO.StreamReader(rawFile);
            int Code = 0, row = 1;
            string Year = String.Empty;
            Dictionary<string, HPData> dictHPData = new Dictionary<string, HPData>();
            string Quarter = String.Empty;
            int ConsiderSarrowCode = 0;
            string dataKey = string.Empty;
            bool bExitLoop = false;
            bool bProcessed = false;


            Dictionary<int, DataMap> dictDataMap = new Dictionary<int, DataMap>();
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    if (bExitLoop)
                        break;

                    string[] fields = SplitCSV(line);
                    int column = 1;
                    string dataVal = string.Empty;
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
                                        if (column == 3 && (val.Contains("PIG CROP, AND PIGS PER LITTER")))
                                            ConsiderSarrowCode = Code;
                                        else if (ConsiderSarrowCode > 0 && bProcessed)
                                            bExitLoop = true;
                                        break;
                                    case "H":
                                        if(!String.IsNullOrEmpty(val) && Code == 1)
                                        {
                                            if (line.Contains("Item") && !dictDataMap.ContainsKey(column) && column > 3)
                                            {
                                                int dyear = 0;
                                                if (int.TryParse(val, out dyear))
                                                {
                                                    dictDataMap[column] = new DataMap(val);
                                                    Year = val;
                                                }
                                            }
                                        }
                                        break;
                                    case "D":
                                        if (!String.IsNullOrEmpty(val) && Code == 1)
                                        {
                                            if (val.Contains("INVENTORY"))
                                                Quarter = val;
                                            else if (fields.Length >= 5)
                                            {
                                                if (column == 3)
                                                {
                                                    foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                                    {
                                                        kvdm.Value.DataKey = val;
                                                    }
                                                    //dataKey = val;
                                                }
                                                else if (column > 3 && dictDataMap.ContainsKey(column))
                                                {
                                                    
                                                    dictDataMap[column].DataValue = val;
                                                }
                                            }
                                        }
                                        else if (!String.IsNullOrEmpty(val) && Code == ConsiderSarrowCode)
                                        {
                                            if (val.Contains("SOWS FARROWING") || val.Contains("PIG CROP") || val.Contains("PIGS PER LITTER"))
                                            {
                                                bProcessed = true;
                                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                                {
                                                    kvdm.Value.DataKey = val;
                                                }
                                            }
                                            else if (fields.Length > 3)
                                            {
                                                if (column == 3)
                                                    Quarter = val;
                                                else if (column > 3 && dictDataMap.ContainsKey(column))
                                                    dictDataMap[column].DataValue = val;
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                        column++;
                    }

                    if ( !String.IsNullOrEmpty(Quarter) && dictDataMap.Count > 0)
                    {
                        if(Code == ConsiderSarrowCode)
                        {
                     
                            if (Quarter.Contains("DEC") && Quarter.Contains("FEB"))
                            {
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 12, 1).ToShortDateString(); ;
                                }

                               // Quarter = new DateTime(Convert.ToInt32(Year),12,1).ToShortDateString();
                            }
                            else if (Quarter.Contains("MAR") && Quarter.Contains("MAY"))
                            {
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 3, 1).ToShortDateString(); ;
                                }

                              //  Quarter = new DateTime(Convert.ToInt32(Year), 3, 1).ToShortDateString();
                            }
                            else if (Quarter.Contains("JUN") && Quarter.Contains("AUG"))
                            {
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 6, 1).ToShortDateString(); ;
                                }

                            //    Quarter = new DateTime(Convert.ToInt32(Year), 6, 1).ToShortDateString();
                            }
                            else if (Quarter.Contains("SEP") && Quarter.Contains("NOV"))
                            {
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 9, 1).ToShortDateString(); ;
                                }

                             //   Quarter = new DateTime(Convert.ToInt32(Year), 9, 1).ToShortDateString();
                            }
                            else
                                continue;
                        }
                        if( Code == 1)
                        {
                            if (Quarter.Contains("DEC") )
                            {
                              //  Quarter = new DateTime(Convert.ToInt32(Year), 12, 1).ToShortDateString();
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 12, 1).ToShortDateString(); ;
                                }

                            }
                            else if (Quarter.Contains("MAR") )
                            {
                             //   Quarter = new DateTime(Convert.ToInt32(Year), 3, 1).ToShortDateString();
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 3, 1).ToShortDateString(); ;
                                }

                            }
                            else if (Quarter.Contains("JUN") )
                            {
                               // Quarter = new DateTime(Convert.ToInt32(Year), 6, 1).ToShortDateString();
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 6, 1).ToShortDateString(); ;
                                }

                            }
                            else if (Quarter.Contains("SEP"))
                            {
                               // Quarter = new DateTime(Convert.ToInt32(Year), 9, 1).ToShortDateString();
                                foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                                {
                                    kvdm.Value.DataYear = new DateTime(Convert.ToInt32(kvdm.Value.Year), 9, 1).ToShortDateString(); ;
                                }

                            }
                            else
                                continue;
                        }
                        foreach (KeyValuePair<int, DataMap> kvdm in dictDataMap)
                        {
                            if (!String.IsNullOrEmpty(kvdm.Value.DataKey) && !String.IsNullOrEmpty(kvdm.Value.DataValue))
                            {
                                string hpKey = kvdm.Value.DataYear;
                                if (!dictHPData.ContainsKey(hpKey))
                                {
                                    dictHPData[hpKey] = new HPData()
                                    {
                                        ReportDate = ReportDate,
                                        Quarter = hpKey
                                    };
                                }

                                FillValue(dictHPData[hpKey], kvdm.Value.DataKey, kvdm.Value.DataValue, Code);
                              //  kvdm.Value.DataKey = String.Empty;
                                kvdm.Value.DataValue = String.Empty;
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
            foreach (KeyValuePair<string, HPData> kvData in dictHPData)
            {
                commonRepo.ProcessQuery($"DELETE FROM HOGSPIGS_DIALY_DATA WHERE QUARTER = '{kvData.Value.Quarter}'");
                commonRepo.ProcessQuery(kvData.Value.PopulateQuery(13, 0, dt));
            }
        }
          
        private string DownloadFile(string rawUrl)
        {
            rawFileInfo = new RawFilesInfo();
            rawFileInfo.URL = rawUrl;//"http://usda.mannlib.cornell.edu/usda/nass/HogsPigs//2010s/2017/HogsPigs-09-28-2017.zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
         //   string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", "09-28-2017" /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";
            //ReportDate = "09-28-2017";
            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\hgpg_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            return rawFile;
        }
    }
}
