using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using Unity;

namespace CROPJob
{
    public class CROPJobRunner : IJobRunner
    {
        private ICropService cropService;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
        private Dictionary<int, CROPRawData> dictCROPRawData;
        private DateTime CaptureDate;
        private string ReportDate;
        private const string selectStates = "SELECTED STATES";
        private const string Condition = "CONDITION";
        private const string weekEnding = "SELECTED STATES: WEEK ENDING";
        private const string state = "STATE";
        private int NoOfRecords;
        public CROPJobRunner(IUnityContainer _unityContainer)
        {
            cropService = _unityContainer.Resolve<CropService>();
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

        public void ParseLine(string line)
        {
            string[] fields = SplitCSV(line);
            foreach (string field in fields)
            {
                string val = field.Replace("\"", "");
            }
        }

        private DateTime? ProcessHFields(Dictionary<int, Dictionary<int, string>> dictHFields)
        {
            foreach (KeyValuePair<int, Dictionary<int, string>> kv in dictHFields)
            {
                if (kv.Value.Count < 4 || !kv.Value[3].Contains(state))
                    continue;
                string dateStruct = kv.Value[6].Contains(",") ? kv.Value[6].Substring(0, kv.Value[6].Length - 1) : kv.Value[6];
                DateTime date = Convert.ToDateTime(ReportDate);
                try
                {
                    date = DateTime.ParseExact(dateStruct, "MMMM dd", CultureInfo.InvariantCulture);
                }
                catch(Exception ex)
                {
                    date = DateTime.ParseExact(dateStruct, "MMMM d", CultureInfo.InvariantCulture);
                }
                return date;
            }
            return null;
        }

        private Dictionary<string, string> ProcessHCondFields(Dictionary<int, Dictionary<int, string>> dictHFields)
        {
            Dictionary<string, string> Conditions = new Dictionary<string, string>();
            foreach (KeyValuePair<int, Dictionary<int, string>> kv in dictHFields)
            {
                if (kv.Value.Count < 3 || !kv.Value[3].Contains(state))
                    continue;

                for( int i = 4; i < kv.Value.Count-1+4; i++)
                {
                    if (kv.Value[i] == "VP")
                        kv.Value[i] = "VERY POOR";
                    else if (kv.Value[i] == "P")
                        kv.Value[i] = "POOR";
                    else if (kv.Value[i] == "F")
                        kv.Value[i] = "FAIR";
                    else if (kv.Value[i] == "G")
                        kv.Value[i] = "GOOD";
                    else if (kv.Value[i] == "EX")
                        kv.Value[i] = "EXCELLENT";
                    Conditions.Add($"COND{i - 3}", kv.Value[i]);
                }
                break;
            }
            return Conditions;
        }

        private void ProcessDFields( Dictionary<int, Dictionary<int, string>> dictDFields, CROPMappingInfo cropMapping)
        {
            foreach (KeyValuePair<int, Dictionary<int, string>> kv in dictDFields)
            {
                if (!cropMapping.IsCondition)
                {
                    if (kv.Value.Count < 3 || string.IsNullOrEmpty(kv.Value[3]))
                        continue;

                    cropMapping.LstCropData.Add(new CROPDialyData(cropMapping.MappingID, cropMapping.Symbol, cropMapping.WeekEnding, cropMapping.Field, kv.Value[3], ConvertToIntString(kv.Value[6]), ReportDate));
                }
                else if (kv.Value.Count > 2 && !string.IsNullOrEmpty(kv.Value[3]))
                {
                    Dictionary<string, string> cond = new Dictionary<string, string>();
                    int prevEntry = 3;
                    for (int i = 4; i < kv.Value.Count - 1 + 4; i++)
                    {
                        int outVal = 0;
                        if (int.TryParse(kv.Value[i], out outVal))
                        {
                            cond.Add(cropMapping.Conditions[$"COND{i - 3}"], outVal.ToString());
                        }
                        else
                        {
                            cropMapping.LstCropData.Add(new CROPDialyData(cropMapping.MappingID, cropMapping.Symbol, cropMapping.WeekEnding, kv.Value[prevEntry], cond, ReportDate));
                            cond.Clear();
                            prevEntry = i;
                        }
                    }
                    cropMapping.LstCropData.Add(new CROPDialyData(cropMapping.MappingID, cropMapping.Symbol, cropMapping.WeekEnding, kv.Value[prevEntry], cond, ReportDate));
                }
            }
        }
        private CROPMappingInfo ProcessTFields( int code, 
                                                Dictionary<int , Dictionary<int, string>> dictTFields,
                                                Dictionary<int, Dictionary<int, string>> dictHFields,
                                                Dictionary<int, CROPMappingInfo> dictCotMapping)
        {
           if(dictCotMapping.ContainsKey(code))
                return dictCotMapping[code]; ;

            CROPMappingInfo cropMappingInfo = null;
            foreach (KeyValuePair<int, Dictionary<int, string>> kv in dictTFields)
            {
                if (kv.Value.Count < 1)
                    continue;
                if (!kv.Value[3].Contains(selectStates) && !kv.Value[3].Contains("PERCENT") && !kv.Value[3].Contains("CONDITION"))
                    continue;
                if (kv.Value[3].Contains(selectStates) && kv.Value[3].IndexOf(selectStates) < 3)
                    continue;
                if (kv.Value[3].Contains("PERCENT") && kv.Value[3].IndexOf("PERCENT") < 3)
                    continue;
                if (kv.Value[3].Contains("PERCENT") && kv.Value[3].Contains(selectStates) && kv.Value[3].Contains("BY"))
                    continue;

                string selValue = kv.Value[3];
                string sstr = String.Empty;
                if (selValue.Contains("PERCENT") )
                {
                    sstr = selValue.Replace("PERCENT", String.Empty).Replace(":", String.Empty).Replace(",", string.Empty);
                }
                else if( selValue.Contains(selectStates))
                {
                    sstr = selValue.Substring(0, selValue.IndexOf(selectStates) - 3).Trim();
                }
                else 
                {
                    sstr = selValue.Replace("CROP", String.Empty).Replace(":", String.Empty).Replace(",", string.Empty); ;
                }
                string symbol = sstr.Substring(0, sstr.IndexOf(' ')).Trim();
                string field = sstr.Substring(sstr.IndexOf(' ')).Trim();
                if (sstr.Contains("WHEAT") || sstr.Contains("CONDITION") || sstr.Contains("PASTURE AND RAISE"))
                {
                    symbol = sstr.Substring(0, sstr.LastIndexOf(' ')).Trim();
                    field = sstr.Substring(sstr.LastIndexOf(' '));
                    if (sstr.Contains("PASTURE AND RANGE"))
                    {
                        symbol = "PASTURE AND RANGE";
                    }
                    else if(sstr.Contains("CROP CONDITION BY"))
                    {
                        symbol = sstr.Substring(0, sstr.IndexOf(' ')).Trim();
                    }
                }
                else if (sstr.Contains("DAYS SUITABLE"))
                {
                    symbol = "CROP";
                    field = "DAYS SUITABLE FOR FIELDWORK";
                }
                DateTime? dtEnding =  Convert.ToDateTime(ReportDate);
                bool bIsCond = false;
                if (selValue.Contains(Condition))
                {
                    bIsCond = true;
                }
                cropMappingInfo = new CROPMappingInfo(symbol,field,code,dtEnding, bIsCond);
                if (cropMappingInfo.IsCondition)
                {
                    cropMappingInfo.Conditions = ProcessHCondFields(dictHFields);
                }
           //   cropService.PopulateCropSymbol(cropMappingInfo);
                dictCotMapping.Add(code, cropMappingInfo);
                break;
            }
            return dictCotMapping[code];
        }
        private string ConvertToIntString(string val)
        {
            int outVal = 0;
            if (int.TryParse(val, out outVal))
                return val;
            else
                return "0";
        }
        public bool RUNJob(Dictionary<string, string> JobParams, string dataSource, int jobID)
        {
            JobStartTime = DateTime.Now;
            JobID = jobID;
            UpdateJobTime updateJobTime = new UpdateJobTime()
            {
                Id = JobID,
                startTime = JobStartTime,
                Status = "RUNNING",
                UserID = 0 //Indicates JOB Runner
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

                    Dictionary<int, CROPMappingInfo> dictCotMapping = new Dictionary<int, CROPMappingInfo>(); //GET it from DB

                    foreach (KeyValuePair<int, CROPRawData> kv in dictCROPRawData)
                    {
                        CROPMappingInfo cropMappingInfo = ProcessTFields(kv.Key, kv.Value.DataValues["T"], kv.Value.DataValues["H"], dictCotMapping);
                        ProcessDFields(kv.Value.DataValues["D"], cropMappingInfo);
                    }

                    foreach (KeyValuePair<int, CROPMappingInfo> kv in dictCotMapping)
                    {
                        NoOfRecords++;
                        cropService.PopulateCropDialyData(kv.Value.LstCropData);
                    }
                    RawData = $"URL:{RawFile}";
                }
                updateJobTime.endTime = DateTime.Now;
                updateJobTime.Message = "Success";
                updateJobTime.Status = "Completed";
                updateJobTime.FilePath = RawData;
                updateJobTime.FileType = "xlsx";
                updateJobTime.NoOfNewRecords = NoOfRecords;
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

        //private void ProcessTFields( strin)
        private void ProcessFile(string rawFile)
        {
            dictCROPRawData = new Dictionary<int, CROPRawData>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(rawFile);
            CROPRawData cropRawData = null;
            Dictionary<int, Dictionary<int, string>> dataFieldInfo = null;
            int Code = 0, row = 1;
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
                                if (!dictCROPRawData.ContainsKey(Code))
                                {
                                    dictCROPRawData[Code] = new CROPRawData(Code);
                                }
                                cropRawData = dictCROPRawData[Code];
                                break;
                            case 2:
                                val = val.ToUpper();
                                if (!cropRawData.DataValues.ContainsKey(val))
                                {
                                    cropRawData.DataValues[val] = new Dictionary<int, Dictionary<int, string>>();
                                }
                                dataFieldInfo = cropRawData.DataValues[val];
                                break;
                            default:
                                if( !dataFieldInfo.ContainsKey(row))
                                {
                                    dataFieldInfo[row] = new Dictionary<int, string>();
                                }
                                dataFieldInfo[row].Add(column, val);
                                break;
                        }
                        column++;
                    }
                    row++;
                }
                catch(Exception ex)
                {

                }
            }
            file.Close();
        }
        private string DownloadFile(string rawUrl)
        {
            rawFileInfo = cropService.GetRawData();
            rawFileInfo.URL = rawUrl;//"http://usda.mannlib.cornell.edu/usda/current/CropProg/CropProg-[[mm-DD-YYYY]].zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
           // string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", "07-16-2018" /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";

            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\prog_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            if (!File.Exists(rawFile))
                rawFile = $@"{path}\PROG_ALL.CSV";
            return rawFile;
        }
    }
}
