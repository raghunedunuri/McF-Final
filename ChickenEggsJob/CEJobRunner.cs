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

namespace ChickenEggsJob
{
    public class CEJobRunner : IJobRunner
    {
        private ICommonRepository commonRepo;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        RawFilesInfo rawFileInfo;
        private Dictionary<int, CROPRawData> dictCROPRawData;
        private DateTime CaptureDate;
        private List<string> OneRowCol;
        private bool bIsRegionalData;
        private string Appender;
        private string Category;
        private string Commodity;
        private string ReportDate;
        private string PrevTRecord;
        private string PrevDRecord;
        private CEData CurrCEData;
        private const string LayersOnHand = "Layers on Hand and Eggs Produced by Type";
        private const string LayersDuring = "Layers during";
        private const string AllLayes = "All layers";
        private const string TableEggType = "Table egg type";
        private const string HatchingEggType = "Hatching egg type";
        private const string BroilerTypeHat = "Broiler-type hatching";
        private const string EggTypeHat = "Egg-type hatching";
        private const string EggsPer100During = "Eggs per 100 layers during";
        private const string EggsProduced = "Eggs produced during";
        private const string LayersOn = "Layers on";
        private const string EggsPer100On = "Eggs per 100 layers on";
        private const string MoltedLayersOn = "Molted layers on";
        private const string BeingMolted = "Being molted";
        private const string MoltCompleted = "Molt completed";
        private const string LayersForSlaughter = "Layers sold for slaughter during";
        private const string LayersRen = "Layers rendered, died, destroyed, composted";
        private const string LayersDisp = "or disappeared for any reason during";
        private const string PulletsOn = "Pullets on";
        private const string PulletsDuring = "Pullets added during ";
        private const string HatcProd = "Hatchery Production";
        private const string EggType = "Egg-type";
        private const string EggsInIncubators = "Eggs in incubators on";
        private const string ChicksHatDuring = "Chicks hatched during";
        private const string PulletsHat = "Pullets hatched during";
        private const string HatcSuppFolks = "Hatchery supply flocks";
        private const string HatcCumm = "Cumulative potential placements";
        private const string BroilerType = "Broiler-type";


        private const string state = "STATE";
        private Dictionary<string, FOField> DictFields;
        private List<FOData> lstFOdata;
        public CEJobRunner(IUnityContainer _unityContainer)
        {
            commonRepo = _unityContainer.Resolve<CommonRepository>();
            jobService = _unityContainer.Resolve<JobService>();
            lstFOdata = new List<FOData>();
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
            string Mon = String.Empty;
            Dictionary<string, CEData> DictCEData = new Dictionary<string, CEData>();
            CurrCEData = null;
            Dictionary<string, Dictionary<int, string>> DictRows = new Dictionary<string, Dictionary<int, string>>();
            string strRecord = String.Empty;
            string unit = String.Empty;

            int Year = 0;
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
                                break;
                            case 2:
                                RecordType = val.ToUpper();
                                break;
                            default:
                                if (!String.IsNullOrEmpty(val) && RecordType == "T" && val[0] != '[')
                                {
                                    string molt = "and Forced Molt, United States";
                                    string plac = "and Placements by Type, United States";
                                    if (!(val.Contains(molt.ToUpper()) || val.Contains(plac.ToUpper())))
                                        PrevTRecord = val;
                                }
                                else if (!String.IsNullOrEmpty(val) && RecordType == "H" &&
                                    column == 6)
                                {
                                    int temp = 0;
                                    if (int.TryParse(val, out temp))
                                        Year = temp;
                                }
                                else if (!String.IsNullOrEmpty(val) && RecordType == "D" &&
                                    (PrevTRecord.Contains(LayersOnHand.ToUpper()) ||
                                     PrevTRecord.Contains(HatcProd.ToUpper())))
                                {
                                    if (val.StartsWith(LayersDuring.ToUpper()) ||
                                        val.StartsWith(EggsPer100During.ToUpper()) ||
                                        val.StartsWith(EggsProduced.ToUpper()) ||
                                        val.StartsWith(LayersOn.ToUpper()) ||
                                        val.StartsWith(EggsPer100On.ToUpper()) ||
                                        val.StartsWith(MoltedLayersOn.ToUpper()) ||
                                        val.StartsWith(LayersRen.ToUpper()) ||
                                        val == EggType.ToUpper() ||
                                        val == BroilerType.ToUpper())
                                    {
                                        PrevDRecord = val;
                                    }
                                    else if (val.StartsWith(PulletsHat.ToUpper()))
                                    {
                                        PrevDRecord = $"{PrevDRecord}:-:{val}";
                                    }
                                    else if (column == 3)
                                    {
                                        strRecord = $"{PrevDRecord}:-:{val}";
                                    }
                                    else
                                    {
                                        if (!DictRows.ContainsKey(strRecord) && !string.IsNullOrEmpty(strRecord))
                                        {
                                            DictRows[strRecord] = new Dictionary<int, string>();
                                        }
                                        if (column == 4)
                                            unit = val;

                                        if (!unit.Contains("1,000 DOZEN"))
                                        {
                                            DictRows[strRecord][column] = val;
                                        }
                                    }
                                }  
                                break;
                        }
                        column++;
                    }
                    row++;
                }
                catch (Exception ex)
                {

                }
            }
            file.Close();
            DateTime dt = DateTime.Now;

            List<string> str = new List<string>();
            str.Add(":-:");
            CEData ceData = null;
            DateTime reportCap = Convert.ToDateTime(ReportDate);
            foreach (KeyValuePair<string, Dictionary<int, string>> kv in DictRows)
            {
                string[] strArr = kv.Key.Split(str.ToArray(), StringSplitOptions.None);
                if( kv.Key.StartsWith(LayersDuring.ToUpper()) && kv.Key.Contains(AllLayes.ToUpper()))
                {
                    if( ceData != null)
                    {
                        foreach (string query in ceData.PopulateQuery(JobID, 0, dt))
                        {
                            commonRepo.ProcessQuery(query);
                        }
                    }
                    ceData = new CEData();
                    ceData.Report_Date = ReportDate;
                    ceData.DTM_AllLayers = kv.Value[6];
                    string Month = strArr[0].Substring(strArr[0].IndexOf(LayersDuring.ToUpper())+ LayersDuring.Length).Trim();
                    ceData.DTM_Month = CommonOperations.GetMonthDate(Month, reportCap.Year);
                    DateTime MonthDtm = Convert.ToDateTime(ceData.DTM_Month);
                    Year = reportCap.Year;
                    if (MonthDtm > reportCap)
                    {
                        Year = reportCap.Year - 1;
                    }
                    ceData.DTM_Month = CommonOperations.GetMonthDate(Month,Year);
                    DateTime BomMonth =  Convert.ToDateTime(ceData.DTM_Month).AddDays(1);
                    ceData.BOM_Month = BomMonth.ToShortDateString();
                }
                else
                {
                    if(ceData == null)
                    {
                        ceData = new CEData();
                        ceData.Report_Date = ReportDate;
                    }
                    if (kv.Key.StartsWith(LayersDuring.ToUpper()) && kv.Key.Contains(TableEggType.ToUpper()))
                    {
                        ceData.DTM_TableEggsType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersDuring.ToUpper()) && kv.Key.Contains(HatchingEggType.ToUpper()))
                    {
                        ceData.DTM_HatchingEggType = kv.Value[6];
                    }
                    if (kv.Key.StartsWith(LayersDuring.ToUpper()) && kv.Key.Contains(BroilerTypeHat.ToUpper()))
                    {
                        ceData.DTM_HatchingBroilerType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersDuring.ToUpper()) && kv.Key.Contains(EggTypeHat.ToUpper()))
                    {
                        ceData.DTM_EggTypeHatching = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100During.ToUpper()) && kv.Key.Contains(AllLayes.ToUpper()))
                    {
                        ceData.DTM_EggsPer100L_AllLayers = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100During.ToUpper()) && kv.Key.Contains(TableEggType.ToUpper()))
                    {
                        ceData.DTM_EggsPer100L_TableEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100During.ToUpper()) && kv.Key.Contains(HatchingEggType.ToUpper()))
                    {
                        ceData.DTM_EggsPer100L_HatchingEggType = kv.Value[6];
                    }
                    if (kv.Key.StartsWith(EggsPer100During.ToUpper()) && kv.Key.Contains(BroilerTypeHat.ToUpper()))
                    {
                        ceData.DTM_EggsPer100L_HatchingBroilerType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100During.ToUpper()) && kv.Key.Contains(EggTypeHat.ToUpper()))
                    {
                        ceData.DTM_EggsPer100L_EggTypeHatching = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsProduced.ToUpper()) && kv.Key.Contains(AllLayes.ToUpper()))
                    {
                        ceData.DTM_EggsProduced_AllLayers = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsProduced.ToUpper()) && kv.Key.Contains(TableEggType.ToUpper()))
                    {
                        ceData.DTM_EggsProduced_TableEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsProduced.ToUpper()) && kv.Key.Contains(HatchingEggType.ToUpper()))
                    {
                        ceData.DTM_EggsProduced_HatchingEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsProduced.ToUpper()) && kv.Key.Contains(BroilerTypeHat.ToUpper()))
                    {
                        ceData.DTM_EggsProduced_HatchingBroilerType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsProduced.ToUpper()) && kv.Key.Contains(EggTypeHat.ToUpper()))
                    {
                        ceData.DTM_EggsProduced_EggTypeHatching = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersOn.ToUpper()) && kv.Key.Contains(AllLayes.ToUpper()))
                    {
                        ceData.BOM_AllLayers = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersOn.ToUpper()) && kv.Key.Contains(TableEggType.ToUpper()))
                    {
                        ceData.BOM_TableEggsType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersOn.ToUpper()) && kv.Key.Contains(HatchingEggType.ToUpper()))
                    {
                        ceData.BOM_HatchingEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersOn.ToUpper()) && kv.Key.Contains(BroilerTypeHat.ToUpper()))
                    {
                        ceData.BOM_HatchingBroilerType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersOn.ToUpper()) && kv.Key.Contains(EggTypeHat.ToUpper()))
                    {
                        ceData.BOM_EggTypeHatching = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100On.ToUpper()) && kv.Key.Contains(AllLayes.ToUpper()))
                    {
                        ceData.BOM_EggsPer100L_AllLayers = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100On.ToUpper()) && kv.Key.Contains(TableEggType.ToUpper()))
                    {
                        ceData.BOM_EggsPer100L_TableEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100On.ToUpper()) && kv.Key.Contains(HatchingEggType.ToUpper()))
                    {
                        ceData.BOM_EggsPer100L_HatchingEggType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100On.ToUpper()) && kv.Key.Contains(BroilerTypeHat.ToUpper()))
                    {
                        ceData.BOM_EggsPer100L_HatchingBroilerType = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(EggsPer100On.ToUpper()) && kv.Key.Contains(EggTypeHat.ToUpper()))
                    {
                        ceData.BOM_EggsPer100L_EggTypeHatching = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(MoltedLayersOn.ToUpper()) && kv.Key.Contains(BeingMolted.ToUpper()))
                    {
                        ceData.BOM_Being_Molted = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(MoltedLayersOn.ToUpper()) && kv.Key.Contains(MoltCompleted.ToUpper()))
                    {
                        ceData.BOM_Molt_Completed = kv.Value[6];
                    }
                    else if (kv.Key.Contains(LayersForSlaughter.ToUpper()))
                    {
                        ceData.DTM_LayersSlaughtered = kv.Value[6];
                    }
                    else if (kv.Key.StartsWith(LayersRen.ToUpper()) && kv.Key.Contains(LayersDisp.ToUpper()))
                    {
                        ceData.DTM_Disappeared = kv.Value[6];
                    }
                    else if (kv.Key.Contains(PulletsOn.ToUpper()))
                    {
                        ceData.BOM_Pullets = kv.Value[6];
                    }
                    else if (kv.Key.Contains(PulletsDuring.ToUpper()))
                    {
                        ceData.DTM_PulletsAdded = kv.Value[6];
                    }
                    else if (kv.Key.Contains(EggType.ToUpper()) && kv.Key.Contains(EggsInIncubators.ToUpper()))
                    {
                        ceData.BOM_EggType_EggsInIncubation= kv.Value[5];
                    }
                    else if (kv.Key.Contains(EggType.ToUpper()) && kv.Key.Contains(ChicksHatDuring.ToUpper()))
                    {
                        ceData.DTM_EggType_ChicksHatched = kv.Value[5];
                    }
                    else if (kv.Key.Contains(EggType.ToUpper()) && kv.Key.Contains(PulletsHat.ToUpper()) && kv.Key.Contains(HatcSuppFolks.ToUpper()))
                    {
                        ceData.DTM_EggType_IntendedPlacement_HatcherySupply = kv.Value[5];
                    }
                    else if (kv.Key.Contains(EggType.ToUpper()) && kv.Key.Contains(PulletsHat.ToUpper()) && kv.Key.Contains(HatcCumm.ToUpper()))
                    {
                        ceData.DTM_EggType_IntendedPlacement_CummPlacements = kv.Value[5];
                    }
                    else if (kv.Key.Contains(BroilerType.ToUpper()) && kv.Key.Contains(EggsInIncubators.ToUpper()))
                    {
                        ceData.BOM_BroilerType_EggsInIncubation = kv.Value[5];
                    }
                    else if (kv.Key.Contains(BroilerType.ToUpper()) && kv.Key.Contains(ChicksHatDuring.ToUpper()))
                    {
                        ceData.DTM_BroilerType_ChicksHatched = kv.Value[5];
                    }
                    else if (kv.Key.Contains(BroilerType.ToUpper()) && kv.Key.Contains(PulletsHat.ToUpper()) && kv.Key.Contains(HatcSuppFolks.ToUpper()))
                    {
                        ceData.DTM_BroilerType_IntendedPlacement_HatcherySupply = kv.Value[5];
                    }
                    else if (kv.Key.Contains(BroilerType.ToUpper()) && kv.Key.Contains(PulletsHat.ToUpper()) && kv.Key.Contains(HatcCumm.ToUpper()))
                    {
                        ceData.DTM_BroilerType_IntendedPlacement_CummPlacements = kv.Value[5];
                    }
                }
            }
            if (ceData != null)
            {
                foreach (string query in ceData.PopulateQuery(JobID, 0, dt))
                {
                    commonRepo.ProcessQuery(query);
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
                        if (kv.Key.ToUpper().Contains("ON FEED"))
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
        private string DownloadFile(string URL)
        {
            rawFileInfo = new RawFilesInfo();
            rawFileInfo.URL = URL;//"http://usda.mannlib.cornell.edu/usda/nass/ChicEggs//2010s/2018/ChicEggs-07-23-2018.zip";
            rawFileInfo.NoOfDays = 5;
            rawFileInfo.Type = "zip";
            string downloadFileLocation = ConfigurationManager.AppSettings["RawFileLocation"];
            string path = $@"{ConfigurationManager.AppSettings["RawFileLocation"]}\{DataSource}\{JobID}";
            Directory.CreateDirectory(path);
         //   string urlFile = rawFileInfo.URL.Replace("[[mm-DD-YYYY]]", ReportDate /*DateTime.Now.ToString("mm-DD-YYYY")*/);
            downloadFileLocation = $@"{path}\{DataSource}.{rawFileInfo.Type}";
         //   ReportDate = "2018-07-23";
            WebClient Client = new WebClient();
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            Client.DownloadFile(rawFileInfo.URL, downloadFileLocation);

            string zipFileName = Path.GetFileNameWithoutExtension(downloadFileLocation);
            string rawFile = $@"{path}\ckeg_all_tables.csv";
            ZipFile.ExtractToDirectory(downloadFileLocation, path);
            if (!File.Exists(rawFile))
                rawFile = $@"{path}\ckeg_all.csv";
            return rawFile;
        }
    }
}
