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

namespace FatsAndOils
{
    public class FOJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
        private DateTime CaptureDate;
        private string ReportDate;
        private List<string> OneRowCol;
        private bool bIsRegionalData;
        private string Appender;
        private string Category;
        private string Commodity;
        private const string selectStates = "SELECTED STATES";
        private const string Condition = "CONDITION";
        private const string weekEnding = "SELECTED STATES: WEEK ENDING";
        private const string state = "STATE";
        private Dictionary<string, FOField> DictFields;
        private List<FOData> lstFOdata;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private int NoOfRecords;
        public FOJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>();
            lstFOdata = new List<FOData>();
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
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(rawFile);
            int Code = 0, row = 1;
            string RecordType = String.Empty;
            string prevTRecord = String.Empty;
            List<string> prevHRecords = new List<string>();
            Dictionary<int, string> prevURecord = new Dictionary<int, string>();
            Dictionary<int, Dictionary<int, string>> Headers = new Dictionary<int, Dictionary<int, string>>();
            List<Dictionary<int, string>> DataRows = new List<Dictionary<int, string>>();
            Dictionary<int, string> DataRow = new Dictionary<int, string>();

            //  DictFields = commonRepo.GetFOFields();
            DictFields = new Dictionary<string, FOField>();
            List<string> SingleCol = new List<string>();
            OneRowCol = new List<string>();
            string PrevRecordType = string.Empty;
            Appender = string.Empty;
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
                                if (String.IsNullOrEmpty(PrevRecordType))
                                    PrevRecordType = RecordType;

                                if (PrevRecordType != RecordType )
                                {
                                    if ( PrevRecordType == "T" && !String.IsNullOrEmpty(prevTRecord))
                                    {
                                        Category = textInfo.ToTitleCase(prevTRecord.Split(' ')[0].ToLower());
                                        switch (Category.ToUpper())
                                        {
                                            case "SOYBEAN":
                                                Category = "Soybeans";
                                                break;
                                            case "SELECTED":
                                                Category = "Oil seeds";
                                                break;
                                        }
                                    if (prevTRecord.Contains("REGIONAL"))
                                            bIsRegionalData = true;
                                        else
                                            bIsRegionalData = false;
                                    }
                                    else if(RecordType == "T")
                                    {
                                        Category = String.Empty;
                                        bIsRegionalData = false;
                                    }
                                    else if(PrevRecordType == "D")
                                    {
                                        ProcessDataRow(prevURecord, Headers, DataRows,SingleCol);
                                        prevURecord.Clear();
                                        Headers.Clear();
                                        DataRows.Clear();
                                        SingleCol.Clear();
                                    }
                                    PrevRecordType = RecordType;
                                }
                                break;
                            default:
                                switch (RecordType)
                                {
                                    case "T":
                                        prevTRecord = val;
                                        break;
                                    case "U":
                                        if (!String.IsNullOrEmpty(val))
                                            prevURecord.Add(column, val);
                                        break;
                                    case "H":
                                        if (!String.IsNullOrEmpty(val))
                                        {
                                            if (!Headers.ContainsKey(row))
                                                Headers[row] = new Dictionary<int, string>();
                                            Headers[row][column] = val;
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
                        if (DataRow.Count == 1)
                        {
                            if (DataRows.Count > 0)
                            {
                                ProcessDataRow(prevURecord, Headers, DataRows, SingleCol);
                                DataRows.Clear();
                                SingleCol.Clear();
                            }
                            SingleCol.Add(DataRow.ElementAt(0).Value);
                        }
                        else
                        {
                            DataRows.Add(DataRow);
                        }
                        // ProcessDataRow(prevURecord, Headers, DataRow);
                        DataRow = new Dictionary<int, string>();
                    }
                    row++;
                }
                catch (Exception ex)
                {

                }
            }
            file.Close();

            DateTime dt = DateTime.Now;

            foreach(KeyValuePair<string, FOField> kv in DictFields)
            {
                commonRepo.ProcessQuery(kv.Value.PopulateQuery(11, 0, dt));
            }
            foreach(FOData foData in lstFOdata)
            {
                foreach( string query in foData.PopulateQuery(11, 0, dt))
                {
                    NoOfRecords++;
                    commonRepo.ProcessQuery(query);
                }
            }
        }

        private Dictionary<int, string> GetYearHeader(Dictionary<int, Dictionary<int, string>> Headers)
        {
            if (Headers.Count == 1)
                return Headers.ElementAt(0).Value;
            foreach(KeyValuePair<int, Dictionary<int,string>> kv in Headers)
            {
                if( bIsRegionalData && kv.Value.ElementAt(0).Value.ToUpper().Contains("REGION"))
                    return kv.Value;
                else if(kv.Value.ElementAt(0).Value.ToUpper().Contains("ITEM"))
                    return kv.Value;
            }
            return null;
        }
        private void ProcessDataRow(Dictionary<int, string> UnitRecord, 
            Dictionary<int, Dictionary<int, string>> Headers, List<Dictionary<int, string>> Data, List<string> SingleCol)
        {
            string FieldAppendix = String.Empty;

            if (SingleCol.Count > 0)
            {
                switch (Category.ToUpper())
                {
                    case "SOYBEANS":
                        Commodity = "Soybeans";
                        FieldAppendix = $"{SingleCol.ElementAt(0)}_";
                        break;
                    case "OIL SEEDS":
                        if (SingleCol.Count > 1)
                        {
                            Commodity = textInfo.ToTitleCase(SingleCol.ElementAt(0).ToLower());
                            FieldAppendix = $"{SingleCol.ElementAt(1)}_";
                        }
                        else
                        {
                            FieldAppendix = $"{SingleCol.ElementAt(0)}_";
                        }
                        break;
                    case "ANIMAL":
                        Commodity = textInfo.ToTitleCase(SingleCol.ElementAt(0).ToLower());
                        FieldAppendix = String.Empty;
                        break;
                }
            }
            string prevOneRow = String.Empty;
            string prevSecondRow = String.Empty;
            foreach (Dictionary<int, string> dc in Data)
            {
                int increment = 0;
                String Unit = String.Empty;

                if (dc.Count == 5)
                {
                    Unit = dc.ElementAt(1).Value;
                    increment = 1;
                }
                string FieldName = $"{FieldAppendix}{dc.ElementAt(0).Value}";
                if(bIsRegionalData)
                {
                    FieldName = $"{FieldAppendix}{Headers.ElementAt(0).Value.ElementAt(0).Value}";
                }
                string Field = $"{Category}--{FieldName}";
                if( UnitRecord.Count > 0)
                {
                    Unit = UnitRecord.ElementAt(1).Value;
                }

                if(!DictFields.ContainsKey(Field))
                {
                    FOField field = new FOField()
                    {
                        Field = FieldName,
                        DisplayName = textInfo.ToTitleCase(FieldName.ToLower()).Replace("_", String.Empty),
                        Category = Category,
                        Unit = Unit,
                        bExists = false
                    };
                    DictFields.Add(Field, field);
                }

                FOData foData1 = new FOData();
                FOData foData2 = new FOData();

                Dictionary<int, string> YearFields = GetYearHeader(Headers);
                
                foData1.ReportMonth = YearFields.Reverse().ElementAt(1).Value;
                foData2.ReportMonth = YearFields.Reverse().ElementAt(0).Value;
                foData1.Value = dc.ElementAt(2 + increment).Value;
                foData2.Value = dc.ElementAt(3 + increment).Value;
                foData1.MonthEndDate = CommonOperations.GetFOMonthDate(foData1.ReportMonth);
                foData2.MonthEndDate = CommonOperations.GetFOMonthDate(foData2.ReportMonth);
                foData1.Field = FieldName;
                foData2.Field = FieldName;
                foData1.Category = Category;
                foData2.Category = Category;
                foData1.Commodity = Commodity;
                foData2.Commodity = Commodity;
                foData1.ReportMonth = ReportDate;
                foData2.ReportMonth = ReportDate;
                if (YearFields.Count > 0)
                {
                    if (bIsRegionalData)
                    {
                        foData1.IsRegionalData = bIsRegionalData;
                        foData2.IsRegionalData = bIsRegionalData;
                        foData1.Region = dc.ElementAt(0).Value;
                        foData2.Region = dc.ElementAt(0).Value;
                    }
                    else
                    {
                        foData1.Commodity = Commodity;
                        foData2.Commodity = Commodity;
                    }
                }
                lstFOdata.Add(foData1);
                lstFOdata.Add(foData2);
            }
        }

        //private FOData PopulateFOData(, Dictionary<int, string> YearFields, string OverRide)
        private string DownloadFile(string rawUrl)
        {
            rawFileInfo = new RawFilesInfo();
            rawFileInfo.URL = rawUrl; //"http://usda.mannlib.cornell.edu/usda/current/FatsOils/FatsOils-07-02-2018.zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
          //  string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", "07-02-2018" /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";

            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawUrl, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\cafo_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            return rawFile;
        }
    }
}
