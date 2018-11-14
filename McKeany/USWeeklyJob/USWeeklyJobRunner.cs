using HtmlAgilityPack;
using McF.Business;
using McF.Common.Interface;
using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace USWeeklyJob
{
    public class USWeeklyJobRunner : IJobRunner
    {
        private IUSWeeklyService usWeeklyService;
        private IJobService jobService;
        private int JobID;
        private string DataSource;
        private DateTime JobStartTime;
        private List<USWeeklyData> usWeeklyData;
        private int NoOfRecords;
        private int NoOfDays;

        public USWeeklyJobRunner(IUnityContainer _unityContainer)
        {
            usWeeklyService = _unityContainer.Resolve<USWeeklyService>();
            jobService = _unityContainer.Resolve<JobService>();
            NoOfRecords = 0;
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
                JobId = JobID,
                startTime = JobStartTime,
                Status = "RUNNING",
                UserID = 0 //Indicates JOB Runner
            };
            jobService.UpdateJobStatus(updateJobTime);

            string ReportDate = DateTime.Now.ToShortDateString();
            if (jobParams.ContainsKey("DATE"))
                ReportDate = jobParams["DATE"];

            NoOfDays = Convert.ToInt32(jobParams["NoOfDays"]);

            DateTime reportDataDate = Convert.ToDateTime(ReportDate);

            usWeeklyData = new List<USWeeklyData>();
       
            List<USWeeklySymbolInfo> lstUsWeeklyInfo = new List<USWeeklySymbolInfo>();
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_HRW", jobParams["Wheat_HRW"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_SRW", jobParams["Wheat_SRW"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_HRS", jobParams["Wheat_HRS"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_White", jobParams["Wheat_White"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_Durum", jobParams["Wheat_Durum"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat", jobParams["Wheat"]));
            lstUsWeeklyInfo.Add(AddSymbol("Wheat_WP", jobParams["Wheat_WP"]));
            lstUsWeeklyInfo.Add(AddSymbol("Cotton_USAPima", jobParams["Cotton_USAPima"]));
            lstUsWeeklyInfo.Add(AddSymbol("Cotton_Upland_1V1by16Over", jobParams["Cotton_Upland_1V1by16Over"]));
    
            lstUsWeeklyInfo.Add(AddSymbol("Cotton_Upland_1V1by16", jobParams["Cotton_Upland_1V1by16"]));
            lstUsWeeklyInfo.Add(AddSymbol("Cotton_Upland_Under1", jobParams["Cotton_Upland_Under1"]));
            lstUsWeeklyInfo.Add(AddSymbol("Cotton_Upland", jobParams["Cotton_Upland"]));
            lstUsWeeklyInfo.Add(AddSymbol("FG_Barley", jobParams["FG_Barley"]));
            lstUsWeeklyInfo.Add(AddSymbol("FG_Corn", jobParams["FG_Corn"]));
            lstUsWeeklyInfo.Add(AddSymbol("FG_GrainSorghums", jobParams["FG_GrainSorghums"]));
    
            lstUsWeeklyInfo.Add(AddSymbol("OS_Soybeans", jobParams["OS_Soybeans"]));
            lstUsWeeklyInfo.Add(AddSymbol("OS_SoybeanCakeandMeal", jobParams["OS_SoybeanCakeandMeal"]));
            lstUsWeeklyInfo.Add(AddSymbol("OS_SoybeanOil", jobParams["OS_SoybeanOil"]));
            lstUsWeeklyInfo.Add(AddSymbol("OS_SunflowerseedOil", jobParams["OS_SunflowerseedOil"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_LongGrainRough", jobParams["Rice_LongGrainRough"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_MediumShortOtherClassesRough", jobParams["Rice_MediumShortOtherClassesRough"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_LongGrainBrown", jobParams["Rice_LongGrainBrown"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_MediumShortOtherClassesBrown", jobParams["Rice_MediumShortOtherClassesBrown"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_LongGrainMilled", jobParams["Rice_LongGrainMilled"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice_MediumShortandOtherClassesMilled", jobParams["Rice_MediumShortandOtherClassesMilled"]));
            lstUsWeeklyInfo.Add(AddSymbol("Rice", jobParams["Rice"]));
            lstUsWeeklyInfo.Add(AddSymbol("HS_CattleHidesWholeExcludingWetBlues", jobParams["HS_CattleHidesWholeExcludingWetBlues"]));
            lstUsWeeklyInfo.Add(AddSymbol("Beef", jobParams["Beef"]));
            lstUsWeeklyInfo.Add(AddSymbol("Pork", jobParams["Pork"]));

            string FilePath = String.Empty;
            foreach( USWeeklySymbolInfo usSymbolInfo in lstUsWeeklyInfo)
            {
                FilePath += $"{usSymbolInfo.Symbol}:{usSymbolInfo.RawFileInfo};";
                DownloadFiles(usSymbolInfo.Symbol, usSymbolInfo.RawFileInfo, reportDataDate);
            }
            FilePath.TrimEnd(';');
            usWeeklyService.PopulateData(usWeeklyData);
            updateJobTime.endTime = DateTime.Now;
            updateJobTime.Message = "Success";
            updateJobTime.Status = "Completed";
            updateJobTime.FilePath = FilePath;
            updateJobTime.FileType = "web";
            updateJobTime.NoOfNewRecords = NoOfRecords;
            return true;
        }

        private USWeeklySymbolInfo AddSymbol( string symbol, string URL )
        {
            USWeeklySymbolInfo usWeeklyInfo = new USWeeklySymbolInfo();
            usWeeklyInfo.Symbol = symbol;
            usWeeklyInfo.RawFileInfo = URL;
            return usWeeklyInfo;
        }

        private long ConvertToLong( string ovrdata )
        {
            int multiFactor = 1; 
            string val = ovrdata.Replace(",", "");
            if (val.Contains("("))
                multiFactor = -1;
            return Convert.ToInt64(val.Replace("(", "").Replace(")", "")) * multiFactor;
        }
        private void DownloadFiles(string symbol, string url, DateTime reportDataDate)
        {
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument resultat = web.Load(url);
            var data =
             from table in resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "table"))
             from tr in table.Descendants("tr")
             select tr;

            string parentNode = String.Empty;
            string prevNode = String.Empty;
            int row = 0;
            Dictionary<DateTime, USWeeklyData> dictUsWeekly = new Dictionary<DateTime, USWeeklyData>();
            foreach (HtmlNode node in data)
            {
                var tddata = from td in node.Descendants("td") select td;
                if (tddata.Count<HtmlNode>() > 6)
                {
                    int i = 0;
                    row++;
                    if (row < 3)
                        continue;

                    USWeeklyData dt = new USWeeklyData();
                    dt.Symbol = symbol;
                    foreach (HtmlNode tdNode in tddata)
                    {
                        string ovrData = tdNode.InnerText.Replace("\n", String.Empty).Replace("\t", String.Empty).Replace("\r", String.Empty).Trim();
                        switch (i)
                        {
                            case 0:
                                dt.Date = Convert.ToDateTime(ovrData);
                                break;
                            case 1:
                                dt.Weekly_Exports = ConvertToLong(ovrData);
                                break;
                            case 2:
                                dt.Accumulated_Exports = ConvertToLong(ovrData);
                                break;
                            case 3:
                                dt.Net_Sales = ConvertToLong(ovrData);
                                break;
                            case 4:
                                dt.Outstanding_Sales = ConvertToLong(ovrData);
                                break;
                            case 5:
                                dt.Nxt_mkt_year_Net_Sales = ConvertToLong(ovrData);
                                break;
                            case 6:
                                dt.Nxt_Mkt_year_Outstanding_Sales = ConvertToLong(ovrData);
                                break;
                        }
                        i++;
                    }
                    dictUsWeekly.Add(dt.Date, dt);
                }
            }

            int totalRecords = 0;
            foreach (KeyValuePair<DateTime, USWeeklyData> kv in dictUsWeekly.Reverse())
            {
                if (kv.Key >= reportDataDate || totalRecords < NoOfDays)
                {
                    NoOfRecords++;
                    usWeeklyData.Add(kv.Value);
                }
                totalRecords++;
            }
        }
    }
}
