using HtmlAgilityPack;
using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using McF.DataAccess;
using MCFJobService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Unity;


namespace McKeanyServiceTest
{
    public enum YEARTYPE
    {
        NONE = 1,
        PREVYEAR,
        CURRYEAR,
        PROJYEAR
    }
    class Program
    {
        static List<WasdeCommData> lstWsdCommdat = new List<WasdeCommData>();
        static List<WasdeDomesticData> lstWsdDomesData = new List<WasdeDomesticData>();
        static Dictionary<string, WasdeDomesticField> WsdDomesticFields = new Dictionary<string, WasdeDomesticField>();
        static Dictionary<string, WasdeDomesticCommodity> WsdDomComms = new Dictionary<string, WasdeDomesticCommodity>();
        static Dictionary<string, WasdeCommodity> WsdComms = new Dictionary<string, WasdeCommodity>();
        static List<string> Units = new List<string>();

        private static string WORLD = "World";
        private static string US = "U.S.";
        private static string MEXICOSUGAR = "Mexico Sugar";
        private static string USNODOT = "U.S";
        private static string SUPPLYUSE = "Supply and Use";
        private static string BEGINNING = "Beginning";
        private static string STOCK = "Stocks";

        static string prevRegion = String.Empty;
        static string currentUnit = String.Empty;
        static string projMonth1 = String.Empty;
        static string projMonth2 = String.Empty;


        public class ExcelData
        {
            public string State { get; set; }
            public string MarketName { get; set; }
            public string DataDate { get; set; }
            public double Price { get; set; }


            public string PopulateQuery (bool bDialy = false)
            {
                if( bDialy)
                    return $"Insert into CORN_DIALY_DATA SELECT '{MarketName}','{State}','{DataDate}',{Price}";
                else
                    return $"Insert into CORN_MONTHLY_DATA SELECT '{MarketName}','{State}','{DataDate}',{Price}";

            }
        }

        public static void PopulateTable(DataTable dt, Dictionary<string, ExcelData> dictExcelData, Dictionary<int, string> MarketPlace)
        {
            int column = 0;
            foreach(DataColumn dc in dt.Columns)
            {
                if(column == 0)
                {
                    column++;
                    continue;
                }
                MarketPlace[column] = dc.ColumnName;
                column++;
            }
            foreach (DataRow dr in dt.Rows)
            {
                string date = String.Empty;
                foreach (DataColumn dc in dt.Columns)
                {
                    if( dc.ColumnName.Trim().ToUpper() == "DATE")
                    {
                        date = dr[dc.ColumnName].ToString();
                    }
                    else
                    {
                        string key = $"{dc.ColumnName}{date}";
                        if(!dictExcelData.ContainsKey(key))
                        {
                            string st = dc.ColumnName.Split(',')[1].Trim().Split(' ')[0].Trim();
                            string martketName = dc.ColumnName.Split(',')[0].Trim();
                            dictExcelData.Add(key, new ExcelData()
                            {
                                MarketName = martketName,
                                State = st,
                                DataDate = date
                                }
                            );
                        }
                        double dd = 0;
                        if (!double.TryParse(dr[dc.ColumnName]?.ToString(), out dd))
                            dd = 0;
                        dictExcelData[key].Price = dd;
                    }
                }
            }
        }
        static ICommonRepository comRep;
        static IJobService JobService;
        static DateTime LUPT = DateTime.Now;

        public class MapValues
        {
            public string Key { get; set; }
            public string Val { get; set; }

            public MapValues(string key, string val)
            {
                Key = key;
                Val = val;
            }
        }

       static int GetFrquecyInfo(string frquency)
        {
            switch(frquency.ToUpper())
            {
                case "DAILY":
                    return 1;
                case "WEEKLY":
                    return 2;
                case "MONTHLY":
                    return 3;
                case "QUARTERLY":
                    return 4;
            }
            return 8;
        }

        static void PopulateSp()
        {
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", @"D:\Saven\Data\Queries.xlsx");
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

            Dictionary<int, MapValues> Frquencies = new Dictionary<int, MapValues>();
            List<MapValues> Aggreation = new List<MapValues>();

            Frquencies.Add(1, new MapValues("WEEK", "Weekly"));
            Frquencies.Add(2, new MapValues("MONTH", "Monthly"));
            Frquencies.Add(3, new MapValues("QUARTER", "Quarterly"));
            Frquencies.Add(4, new MapValues("YEAR", "Yearly"));
            Frquencies.Add(5, new MapValues("YEAR", "Fiscal"));

            Aggreation.Add(new MapValues("COUNT", "Count"));
            Aggreation.Add(new MapValues("MIN", "Low"));
            Aggreation.Add(new MapValues("MAX", "High"));
            Aggreation.Add(new MapValues("AVG", "Average"));
            Aggreation.Add(new MapValues("FIRST_VALUE", "First"));
            Aggreation.Add(new MapValues("LAST_VALUE", "Last"));

            string cond = "ELSE IF @RollupValue = [[ROLLUP]] AND @RollupFequency = [[FREQUENCY]]";

            string Template = File.ReadAllText(@"D:\Saven\Data\Template.txt");

            foreach (DataRow dr in data.Tables[0].Rows)
            {
                string spName = dr[0].ToString();
                string tempTemp = Template;
                string table = dr[1].ToString();
                string dateField = dr[2].ToString();
                string Fields = dr[3]?.ToString();
                string aggField = dr[4].ToString();
                string[] AggFields = dr[4].ToString().Split(',');
                string symTable = dr[5]?.ToString();
                string FeedType = dr[6].ToString().ToUpper();
                string DateQuery = dr[7].ToString();
                string Order = dr[8]?.ToString();
                string FieldTable = dr[9].ToString();
                string SelectFields = dr[10]?.ToString();
                if (!String.IsNullOrEmpty(SelectFields))
                    SelectFields += ",";

                int freqInfo = GetFrquecyInfo(FeedType);

                tempTemp = tempTemp.Replace("[[SP_NAME]]", spName);
                tempTemp = tempTemp.Replace("[[DATEFIELD]]", dateField);
                tempTemp = tempTemp.Replace("[[TABLENAME]]", table);
                tempTemp = tempTemp.Replace("[[FIELDTABLE]]", FieldTable);

                string dateFilterQuer = $" {dateField} >= @StartDate AND {dateField} <= @EndDate";
                string symbolTableQuery = string.Empty;
                if (!String.IsNullOrEmpty(symTable))
                {
                    symbolTableQuery = symTable;
                }
                string FieldQuery = String.Empty;
                if (!String.IsNullOrEmpty(Fields))
                {
                    FieldQuery = $",ACC.{Fields}";
                }
                string appendQuery = tempTemp + $"\nSelect {dateField}{FieldQuery},{SelectFields}{aggField} FROM {table} ACC {symbolTableQuery} WHERE {dateFilterQuer} {Order}\n END\n";

                for (int ii = freqInfo; ii < 6; ii++)
                {
                    foreach (MapValues mapValues in Aggreation)
                    {
                        appendQuery += $"ELSE IF @RollupValue = '{mapValues.Val}' AND @RollupFequency = '{Frquencies[ii].Val}'\n BEGIN\n";
                        string dateRep = DateQuery.Replace("[FRQUENCY]", Frquencies[ii].Key).Replace("[DATEFIELD]", dateField);

                        if (Frquencies[ii].Val == "Fiscal")
                            dateRep = dateRep.Replace("DATEDIFF(YEAR, 0", "DATEDIFF(YEAR, @FiscalMonth");
                        appendQuery += $"Select {dateField}{FieldQuery.Replace("ACC.", String.Empty)},{SelectFields}{aggField} FROM ( SELECT {dateRep} AS {dateField}{FieldQuery},{SelectFields}\n";
                        string revOrder = String.Empty;
                        if (mapValues.Key == "FIRST_VALUE" || mapValues.Key == "LAST_VALUE")
                            revOrder = $"ORDER  BY {dateField}";
                        foreach (string str in AggFields)
                        {
                            appendQuery += $"{mapValues.Key}({str}) OVER (PARTITION BY {dateRep}{FieldQuery} {revOrder}) AS {str},\n";
                        }
                        appendQuery += $"ROW_NUMBER() OVER(PARTITION BY {dateRep}{FieldQuery} ORDER BY {dateField} DESC) AS ROWNUM\n FROM {table} ACC {symbolTableQuery} WHERE {dateFilterQuer}";
                        appendQuery += $") B WHERE B.ROWNUM = 1 {Order}\nEND\n";
                    }
                }
                appendQuery += "END";
                string fileName = $"D:\\Saven\\Data\\{spName}.sql";
                File.WriteAllText($"D:\\Saven\\Data\\{spName}.sql", appendQuery);
            }
        }

        static void TempCode()
        {
            //    PopulateSp();
            //  return;
            //List<string> Fields = new List<string>();

            //string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", @"D:\Saven\Data\price dataset 2018-0920 v5.xlsx");
            //DataSet data = new DataSet();
            //foreach (var sheetName in GetExcelSheetNames(connectionString))
            //{
            //    using (OleDbConnection con = new OleDbConnection(connectionString))
            //    {
            //        var dataTable = new DataTable();
            //        string query = string.Format("SELECT * FROM [{0}]", sheetName);
            //        con.Open();
            //        OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
            //        adapter.Fill(dataTable);
            //        data.Tables.Add(dataTable);
            //    }
            //}

            //Dictionary<string, ExcelData> dictExcelData = new Dictionary<string, ExcelData>();
            //Dictionary<string, ExcelData> dictDialyExcelData = new Dictionary<string, ExcelData>();
            //Dictionary<int, string> DailyMarket = new Dictionary<int, string>();
            //Dictionary<int, string> Market = new Dictionary<int, string>();

            //PopulateTable(data.Tables[0], dictExcelData, Market);
            //PopulateTable(data.Tables[1], dictDialyExcelData, DailyMarket);

            //foreach(DataRow dr in data.Tables[3].Rows)
            //{
            //    int column = 0;
            //    string date = String.Empty;

            //    foreach (DataColumn dc in data.Tables[3].Columns)
            //    {
            //        if(column == 0)
            //        {
            //            date = dr[dc.ColumnName].ToString();
            //        }
            //        else
            //        {
            //            string marker = Market[column];
            //            string key = $"{marker}{date}";
            //            if (!dictExcelData.ContainsKey(key))
            //            {
            //                string st = marker.Split(',')[1].Trim().Split(' ')[0].Trim();
            //                string martketName = marker.Split(',')[0].Trim();
            //                dictExcelData.Add(key, new ExcelData()
            //                {
            //                    MarketName = martketName,
            //                    State = st,
            //                    DataDate = date
            //                }
            //                );
            //            }
            //            double dd = 0;
            //            if (!double.TryParse(dr[column]?.ToString(), out dd))
            //                dd = 0;
            //            dictExcelData[key].Price = dd;
            //        }
            //        column++;
            //    }
            //}
            //  Dictionary<string,>

            //  foreach(Dat)

            //            < div id = "archived-docs" >< a name = "anchor2010s" id = "anchor2010s" ></ a >< div class="dirElement" id="n2010s">
            //<a>2010s</a><a name = "anchor2018" id="anchor2018"></a>
            //<div class="dirElement" id="n2018" style="display: none;">
            //<a>2018</a>
            //<div class="fileElement" id="ChicEggs-07-23-2018.zip" style="display: none;">
            //Chickens and Eggs, 07.23.2018 [<a href="http://usda.mannlib.cornell.edu/usda/nass/ChicEggs//2010s/2018/ChicEggs-07-23-2018.zip">zip</a>]<br>
            //</div>
            //var web = new HtmlWeb();
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1028");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1010");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1020");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1902");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1086");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1048");
            //http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1020 

            //   var data = resultat.DocumentNode.SelectNodes("//div[@class='fileElement']");
            //  HtmlAgilityPack.HtmlDocument resultat = web.Load("https://fsa.usda.gov/programs-and-services/economic-and-policy-analysis/dairy-and-sweeteners-analysis/index");
            //HtmlAgilityPack.HtmlDocument resultat = web.Load("https://www.ers.usda.gov/data-products/sugar-and-sweeteners-yearbook-tables/sugar-and-sweeteners-yearbook-tables/#U.S.%20Sugar%20Supply%20and%20Use");
            //var data = resultat.DocumentNode.SelectNodes("//td[@class='DataFileItem']");

            // HtmlDocument doc = new HtmlDocument();
            //Dictionary<string, string> str = new Dictionary<string, string>();

            //HtmlAgilityPack.HtmlDocument resultat = web.Load("https://www.fsa.usda.gov/programs-and-services/economic-and-policy-analysis/dairy-and-sweeteners-analysis/index");
            //foreach (HtmlNode node in resultat.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            //{
            //    string href = node.Attributes["href"].Value;
            //    if (!String.IsNullOrEmpty(href) && href.Contains("smd_tables") && (href.Contains(".xlsx") || href.Contains(".xls")))
            //    {
            //        str[href] = $"https://www.fsa.usda.gov{href}";
            //    }
            //}

            //foreach (HtmlNode node in data)
            //{
            //    string innerText = node.InnerHtml;
            //    if (innerText.Contains("TABLE"))
            //    {
            //        int startlen = innerText.IndexOf("href=\"") + 6;
            //        int endlen = innerText.IndexOf("\"", startlen);
            //        //int startend = innerText.
            //        string url = innerText.Substring(startlen, endlen - startlen);

            //        startlen = url.IndexOf("TABLE");
            //        endlen = url.IndexOf("?v=0", startlen);
            //        string tablename = url.Substring(startlen, endlen - startlen);
            //    }
            //}
            //foreach (HtmlNode node in resultat.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            //{
            //    string href = node.Attributes["href"].Value;
            //    if (!String.IsNullOrEmpty(href) && href.Contains("smd_tables") && (href.Contains(".xlsx") || href.Contains(".xls")))
            //    {
            //        str[href] = $"https://www.fsa.usda.gov{href}";
            //    }
            //}

            //var data =
            // from table in resultat.DocumentNode.Descendants().Where
            //(x => (x.Name == "div"))
            // from tr in table.Descendants("fileElement")
            // select tr;

            //string parentNode = String.Empty;
            //string prevNode = String.Empty;
            //int row = 0;
            //foreach (HtmlNode node in data)
            //{
            //    string innerText = node.InnerHtml;
            //    if (innerText.Contains(".zip"))
            //    {
            //        int startlen = innerText.IndexOf("href=\"") + 6;
            //        int endlen = innerText.IndexOf("\"", startlen);
            //        //int startend = innerText.
            //        string url = innerText.Substring(startlen, endlen - startlen);
            //        string date = url.Substring(url.Length - 14, 10);
            //        str[date] = url;
            //    }
            //}

            //DateTime.ParseExact("June 2, 2018", "MMMM dd, yyyy", CultureInfo.InvariantCulture);
        }

        public class CocoaData
        {
            public string Date { get; set; }
            public long   USA { get; set; }
            public long   Europe { get; set; }
            public long Asia { get; set; }

            public CocoaData(string date)
            {
                Date = date;
                USA = 0;
                Europe = 0;
                Asia = 0;
            }

            public string PopulateQuery()
            {
                return $"Insert into COCOA_GRIND_DATA SELECT '{Date}', {USA},{Europe},{Asia}";
            }
        }

        public static string GetDate(string Quarter, int Year)
        {
            switch(Quarter.ToUpper())
            {
                case "Q1":
                    return new DateTime(Year, 3, 31).ToShortDateString();
                case "Q2":
                    return new DateTime(Year, 6, 30).ToShortDateString();
                case "Q3":
                    return new DateTime(Year, 9, 30).ToShortDateString();
                case "Q4":
                    return new DateTime(Year, 12, 31).ToShortDateString();
            }
            return String.Empty;
        }

        public static void PopulateCocoaData(string Quarter, int Year, Dictionary<string, CocoaData> cocadata, string feed, long data)
        {
            if (data <= 0)
                return;
            string date = GetDate(Quarter, Year);
            CocoaData cocaObj = null;
            if (!cocadata.ContainsKey(date))
            {
                cocadata[date] = new CocoaData(date);
            }
            cocaObj = cocadata[date];
            switch(feed)
            {
                case "USA":
                    cocaObj.USA = data;
                    break;
                case "EUROPE":
                    cocaObj.Europe = data;
                    break;
                case "ASIA":
                    cocaObj.Asia = data;
                    break;
            }
        }

        private static void AddRegionSugarData(DataTable dt, List<SugarRegionDelData> lstSugarRegionData, string region)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string date = dr[0].ToString();
                int Year = 0;
                if (!Int32.TryParse(date, out Year))
                    continue;

                int Month = Convert.ToInt32(dr[1]);
                string MonthDate = new DateTime(Year, Month, 1).ToShortDateString();

                SugarRegionDelData sug = new SugarRegionDelData(DateTime.Now.ToShortDateString(), MonthDate, region);
                sug.TOTAL_DEL = dr[2].ToString().Replace(",", String.Empty);
                sug.DEL_BAKERY_CEREAL_REL_PROD = dr[3].ToString().Replace(",", String.Empty);
                sug.DEL_CONF_REL_PROD = dr[4].ToString().Replace(",", String.Empty);
                sug.DEL_ICECREAM_DAIRY_PROD = dr[5].ToString().Replace(",", String.Empty);
                sug.DEL_BEVERAGES = dr[6].ToString().Replace(",", String.Empty);
                sug.DEL_CANNED_BOTTLE_FROZEN = dr[7].ToString().Replace(",", String.Empty);
                sug.DEL_MULTIPLE_OTHERFOOD = dr[8].ToString().Replace(",", String.Empty);
                sug.DEL_NONFOOD = dr[9].ToString().Replace(",", String.Empty);
                sug.DEL_HOT_RES_INS = dr[10].ToString().Replace(",", String.Empty);
                sug.DEL_WS_JOBBERS_DEALERS = dr[11].ToString().Replace(",", String.Empty);
                sug.DEL_RETAILGROC_CHAINSTORE = dr[12].ToString().Replace(",", String.Empty);
                sug.DEL_GOVT_AGEN = dr[13].ToString().Replace(",", String.Empty);
                sug.DEL_OTHERS = dr[14].ToString().Replace(",", String.Empty);
                lstSugarRegionData.Add(sug);
            }
        }
        private static void ParseCocoa(ICommonRepository commRep)
        {
            string connectionString = string.Format("provider=Microsoft.ACE.OLEDB.12.0; data source={0};Extended Properties=Excel 8.0;", @"D:\Saven\SugarData_ByUSeCategory.xlsx");
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
            DataTable CaneBeet = data.Tables[0];
            List<SugarRegionDelData> lstSugarRegionData  = new List<SugarRegionDelData>();
            AddRegionSugarData(data.Tables[2], lstSugarRegionData, "New England");
            AddRegionSugarData(data.Tables[1], lstSugarRegionData, "Mid Altantic");
            AddRegionSugarData(data.Tables[3], lstSugarRegionData, "North Central");
            AddRegionSugarData(data.Tables[6], lstSugarRegionData, "South");
            AddRegionSugarData(data.Tables[8], lstSugarRegionData, "West");
            AddRegionSugarData(data.Tables[4], lstSugarRegionData, "PuertoRico");
            
            List<SugarBuyerData> lstSugar = new List<SugarBuyerData>();

            foreach (DataRow dr in CaneBeet.Rows)
            {
                string date = dr[0].ToString();
                DateTime monthDate = DateTime.MinValue;
                if (!DateTime.TryParse(date, out monthDate))
                    continue;

                SugarBuyerData sugarDate = new SugarBuyerData(DateTime.Now.ToShortDateString(), monthDate.ToShortDateString());
                sugarDate.TOTAL_DEL_BEET = dr[1].ToString().Replace(",", String.Empty);
                sugarDate.TOTAL_DEL_CANE = dr[14].ToString().Replace(",", String.Empty);
                sugarDate.TOTAL_DEL = (Convert.ToDouble(sugarDate.TOTAL_DEL_BEET) + Convert.ToDouble(sugarDate.TOTAL_DEL_CANE)).ToString();
                sugarDate.DEL_BEET_BAKERY_CEREAL_REL_PROD = dr[2].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_BAKERY_CEREAL_REL_PROD = dr[15].ToString().Replace(",", String.Empty);
                sugarDate.DEL_BAKERY_CEREAL_REL_PROD = (Convert.ToDouble(sugarDate.DEL_BEET_BAKERY_CEREAL_REL_PROD) + Convert.ToDouble(sugarDate.DEL_CANE_BAKERY_CEREAL_REL_PROD)).ToString();
                sugarDate.DEL_BEET_CONF_REL_PROD = dr[3].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_CONF_REL_PROD = dr[16].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CONF_REL_PROD = (Convert.ToDouble(sugarDate.DEL_CANE_CONF_REL_PROD) + Convert.ToDouble(sugarDate.DEL_BEET_CONF_REL_PROD)).ToString();
                sugarDate.DEL_BEET_ICECREAM_DAIRY_PROD = dr[4].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_ICECREAM_DAIRY_PROD = dr[17].ToString().Replace(",", String.Empty);
                sugarDate.DEL_ICECREAM_DAIRY_PROD = (Convert.ToDouble(sugarDate.DEL_BEET_ICECREAM_DAIRY_PROD) + Convert.ToDouble(sugarDate.DEL_CANE_ICECREAM_DAIRY_PROD)).ToString();
                sugarDate.DEL_BEET_BEVERAGES = dr[5].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_BEVERAGES = dr[18].ToString().Replace(",", String.Empty);
                sugarDate.DEL_BEVERAGES = (Convert.ToDouble(sugarDate.DEL_BEET_BEVERAGES) + Convert.ToDouble(sugarDate.DEL_CANE_BEVERAGES)).ToString();
                sugarDate.DEL_BEET_CANNED_BOTTLE_FROZEN = dr[6].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_CANNED_BOTTLE_FROZEN = dr[19].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANNED_BOTTLE_FROZEN = (Convert.ToDouble(sugarDate.DEL_BEET_CANNED_BOTTLE_FROZEN) + Convert.ToDouble(sugarDate.DEL_CANE_CANNED_BOTTLE_FROZEN)).ToString();
                sugarDate.DEL_BEET_MULTIPLE_OTHERFOOD = dr[7].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_MULTIPLE_OTHERFOOD = dr[20].ToString().Replace(",", String.Empty);
                sugarDate.DEL_MULTIPLE_OTHERFOOD = (Convert.ToDouble(sugarDate.DEL_BEET_MULTIPLE_OTHERFOOD) + Convert.ToDouble(sugarDate.DEL_CANE_MULTIPLE_OTHERFOOD)).ToString();
                sugarDate.DEL_BEET_NONFOOD = dr[8].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_NONFOOD = dr[21].ToString().Replace(",", String.Empty);
                sugarDate.DEL_NONFOOD = (Convert.ToDouble(sugarDate.DEL_BEET_NONFOOD) + Convert.ToDouble(sugarDate.DEL_CANE_NONFOOD)).ToString();
                sugarDate.DEL_BEET_HOT_RES_INS = dr[9].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_HOT_RES_INS = dr[22].ToString().Replace(",", String.Empty);
                sugarDate.DEL_HOT_RES_INS = (Convert.ToDouble(sugarDate.DEL_BEET_HOT_RES_INS) + Convert.ToDouble(sugarDate.DEL_CANE_HOT_RES_INS)).ToString();
                sugarDate.DEL_BEET_WS_JOBBERS_DEALERS = dr[10].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_WS_JOBBERS_DEALERS = dr[23].ToString().Replace(",", String.Empty);
                sugarDate.DEL_WS_JOBBERS_DEALERS = (Convert.ToDouble(sugarDate.DEL_BEET_WS_JOBBERS_DEALERS) + Convert.ToDouble(sugarDate.DEL_CANE_WS_JOBBERS_DEALERS)).ToString();
                sugarDate.DEL_BEET_RETAILGROC_CHAINSTORE = dr[11].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_RETAILGROC_CHAINSTORE = dr[24].ToString().Replace(",", String.Empty);
                sugarDate.DEL_RETAILGROC_CHAINSTORE = (Convert.ToDouble(sugarDate.DEL_BEET_RETAILGROC_CHAINSTORE) + Convert.ToDouble(sugarDate.DEL_CANE_RETAILGROC_CHAINSTORE)).ToString();
                sugarDate.DEL_BEET_GOVT_AGEN = dr[12].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_GOVT_AGEN = dr[25].ToString().Replace(",", String.Empty);
                sugarDate.DEL_GOVT_AGEN = (Convert.ToDouble(sugarDate.DEL_BEET_GOVT_AGEN) + Convert.ToDouble(sugarDate.DEL_CANE_GOVT_AGEN)).ToString();
                sugarDate.DEL_BEET_OTHERS = dr[13].ToString().Replace(",", String.Empty);
                sugarDate.DEL_CANE_OTHERS = dr[26].ToString().Replace(",", String.Empty);
                sugarDate.DEL_OTHERS = (Convert.ToDouble(sugarDate.DEL_BEET_OTHERS) + Convert.ToDouble(sugarDate.DEL_CANE_OTHERS)).ToString();

                lstSugar.Add(sugarDate);
            }

            foreach(SugarBuyerData su in lstSugar)
            {
                commRep.ProcessQuery( su.PopulateQuery(0, 0, DateTime.Now));
            }

            foreach (SugarRegionDelData su in lstSugarRegionData)
            {
                commRep.ProcessQuery(su.PopulateQuery(0, 0, DateTime.Now));
            }
            //foreach(DataRow dr in asia.Rows)
            //{
            //    int Year = Convert.ToInt32(dr["year"]);
            //    long Q1 = Convert.ToDouble(dr["Q1"]);
            //    long Q2 = Convert.ToDouble(dr["Q2"]);
            //    long Q3 =  Convert.ToDouble(dr["Q3"]);
            //    long Q4 = Convert.ToDouble(dr["Q4"]);

            //    PopulateCocoaData("Q1", Year, coca, "ASIA", Q1);
            //    PopulateCocoaData("Q2", Year, coca, "ASIA", Q2);
            //    PopulateCocoaData("Q3", Year, coca, "ASIA", Q3);
            //    PopulateCocoaData("Q4", Year, coca, "ASIA", Q4);
            //}

            //foreach (DataRow dr in europe.Rows)
            //{
            //    string edata = dr["Data"].ToString();
            //    string[] qData = edata.Split(' ');
            //    int Year = Convert.ToInt32(dr["year"]);
            //    long Q1 = 0;
            //    long Q2 = 0;
            //    long Q3 = 0;
            //    long Q4 = 0;
            //    if (qData.Length > 2)
            //    {
            //        Q1 = Convert.ToDouble(qData[3].Replace(",", String.Empty));
            //        Q2 = Convert.ToDouble(qData[2].Replace(",", String.Empty));
            //        Q3 = Convert.ToDouble(qData[1].Replace(",", String.Empty));
            //        Q4 = Convert.ToDouble(qData[0].Replace(",", String.Empty));
            //    }
            //    else
            //    {
            //        Q1 = Convert.ToDouble(qData[0].Replace(",", String.Empty));
            //        Q2 = Convert.ToDouble(qData[1].Replace(",", String.Empty));
            //    }
            //    PopulateCocoaData("Q1", Year, coca, "EUROPE", Q1);
            //    PopulateCocoaData("Q2", Year, coca, "EUROPE", Q2);
            //    PopulateCocoaData("Q3", Year, coca, "EUROPE", Q3);
            //    PopulateCocoaData("Q4", Year, coca, "EUROPE", Q4);
            //}

            //foreach (DataRow dr in usa.Rows)
            //{
            //    int Year = Convert.ToInt32(dr["year"]);
            //    string Quarter = dr["Quarter"].ToString();
            //    long val = Convert.ToDouble(dr["Data"].ToString().Replace(",", String.Empty));
            //    PopulateCocoaData(Quarter, Year, coca, "USA", val);
            //}

            //foreach(KeyValuePair<string,CocoaData> kv in coca)
            //{
            //    commRep.ProcessQuery(kv.Value.PopulateQuery());
            //}

        }
        static void Main(string[] args)
        {
            Logger.Info("Logging started");
            UnityResolver.Init();
            comRep = UnityResolver._unityContainer.Resolve<CommonRepository>();
            JobService = UnityResolver._unityContainer.Resolve<JobService>();
            McFJobHandler mcFJobHandler = new McFJobHandler(JobService);
            List<JobInfo> lstJobs = mcFJobHandler.GetNextRunningJobs();
            //JobService
            //     ParseCocoa(comRep);
            //   PopulateSp();
            // return;

            string DataFeed = String.Empty;
            DateTime DataDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (args.Length > 0)
                DataFeed = args[0];

            DataDate = DateTime.MinValue;
            if (args.Length > 1)
                DataDate = Convert.ToDateTime(args[1]);
            
           
            JobService = UnityResolver._unityContainer.Resolve<JobService>();
            McFJobHandler mcfHandler = new McFJobHandler(JobService);
            if( !String.IsNullOrEmpty(DataFeed))
                ProcessDataByQuery(DataFeed, mcfHandler, DataDate);
            else
            {
                ProcessDataByQuery("DTN", mcfHandler, DataDate);
                ProcessDataByQuery("SUGAR", mcfHandler, DataDate);
                ProcessDataByQuery("USWEEKLY", mcfHandler, DataDate);
                ProcessDataByQuery("ETHANOL", mcfHandler, DataDate);
                ProcessDataByQuery("WASDE", mcfHandler, DataDate);
                ProcessDataByQuery("SWEETNER", mcfHandler, DataDate);
                ProcessDataByQuery("COCOA", mcfHandler, DataDate);
                ProcessDataByQuery("CROP", mcfHandler, DataDate);
                ProcessDataByQuery("BROILERANDHATCHERY", mcfHandler, DataDate);
                ProcessDataByQuery("CHICKENANDEGGS", mcfHandler, DataDate);
                ProcessDataByQuery("HOGSANDPIGS", mcfHandler, DataDate);
                ProcessDataByQuery("CATTLEONFEED", mcfHandler, DataDate);
                ProcessDataByQuery("FATSANDOILS", mcfHandler, DataDate);
                ProcessDataByQuery("CHICKENANDEGGS", mcfHandler, DataDate);
            }
            return;
        }

        private static void ProcessOtherDataSource(string dataSource, McFJobHandler mcfHandler, DateTime DataDate)
        {
            IJobRunner jobRunner = null;
            string url = String.Empty;

            Dictionary<string, string> DataDict = new Dictionary<string, string>();
            int JobID = 1;
            switch (dataSource.ToUpper())
            {
                case "USWEEKLY":
                    JobID = 3;
                    jobRunner = mcfHandler.GetJobRunner("USWeeklyJob.USWeeklyJobRunner, USWeeklyJob");
                    break;
                case "SUGAR":
                    JobID = 14;
                    ProcessSugarRunner(mcfHandler, DataDate);
                    return;
                case "ETHANOL":
                    JobID = 2;
                    jobRunner = mcfHandler.GetJobRunner("EthanolJob.EthanolJobRunner, EthanolJob");
                    break;
                case "DTN":
                    JobID = 1;
                    jobRunner = mcfHandler.GetJobRunner("DTNJob.DTNJobRunner, DTNJob");
                    break;
                case "SWEETNER":
                    JobID = 16;
                    jobRunner = mcfHandler.GetJobRunner("SweetnerJob.SweetnerJobRunner, SweetnerJob");
                    DataDict.Add("FILE", "https://www.ers.usda.gov/data-products/sugar-and-sweeteners-yearbook-tables/sugar-and-sweeteners-yearbook-tables/#U.S.%20Sugar%20Supply%20and%20Use");
                    break;
                case "COCOA":
                    JobID = 15;
                    break;

            }
            DataDict["DATE"] = DataDate.ToShortDateString();
            int Id = JobService.CreateJobForRun(new UpdateJobTime()
            {
                JobId = JobID,
                UserID = 0
            });
            jobRunner.RUNJob(DataDict, dataSource, Id);
        }

        private static void ProcessCOTRunner( McFJobHandler mcfHandler )
        {
            IJobRunner jobRunner = mcfHandler.GetJobRunner("COTJob.COTJobRunner, COTJob");

            Dictionary<string, string> dict = new Dictionary<string, string>();
          
            dict = new Dictionary<string, string>();
            dict.Add("REPLACEFILE", "true");
            dict.Add("FILE", "https://www.cftc.gov/files/dea/history/fut_disagg_xls_2018.zip");

            jobRunner.RUNJob(dict, "COT", 3);
        }
        
        private static void ProcessSugarRunner(McFJobHandler mcfHandler, DateTime DataDate)
        {
            var web = new HtmlWeb();
            Dictionary<string, string> str = new Dictionary<string, string>();

            HtmlAgilityPack.HtmlDocument resultat = web.Load("https://www.fsa.usda.gov/programs-and-services/economic-and-policy-analysis/dairy-and-sweeteners-analysis/index");
            foreach (HtmlNode node in resultat.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            {
                string href = node.Attributes["href"].Value;
                if (!String.IsNullOrEmpty(href) && href.Contains("smd_tables") && (href.Contains(".xlsx") || href.Contains(".xls")))
                {
                    str[href] = $"https://www.fsa.usda.gov{href}";
                    break;
                }
            }

            int JobID = 1;
            foreach (KeyValuePair<string, string> kv in str.Reverse())
            {
                Dictionary<string, string> DataDict = new Dictionary<string, string>();
                DataDict["FILE"] = kv.Value;
                DataDict["DATE"] = DataDate.ToShortDateString();
                IJobRunner jobRunner = mcfHandler.GetJobRunner("SugarJob.SugarJobRunner, SugarJob");
                int Id = JobService.CreateJobForRun(new UpdateJobTime()
                {
                    JobId = 14,
                    UserID = 0
                });
                jobRunner.RUNJob(DataDict, "SUGAR", Id);
            }
        }
        private static void ProcessWasdeRunner(McFJobHandler mcfHandler, DateTime DataDate)
        {
            var web = new HtmlWeb();
            Dictionary<string, string> str = new Dictionary<string, string>();

            HtmlAgilityPack.HtmlDocument resultat = web.Load("https://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1194");
            foreach (HtmlNode node in resultat.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
            {
                string href = node.Attributes["href"].Value;
                if (href.Contains(".xlsx") || href.Contains(".xls"))
                {
                    string lastString = href.Substring(href.LastIndexOf('/') + 1);
                    string lastSubString = lastString.Substring(lastString.IndexOf('-') + 1);
                    if (lastSubString.Length <= 15)
                    {
                        string date = DateTime.Parse(lastSubString.Substring(0, 10)).ToShortDateString();
                        str[date] = href;
                    }
                }
            }

            int JobID = 12;
        //    DateTime dt = Convert.ToDateTime("2015-08-12");
            foreach (KeyValuePair<string, string> kv in str.Reverse())
            {
              Dictionary<string, string> DataDict = new Dictionary<string, string>();
                DataDict["FILE"] = kv.Value;
                DataDict["DATE"] = kv.Key;
                DateTime dt = Convert.ToDateTime(kv.Key);
                if (dt >= DataDate)
                {
                    int Id = JobService.CreateJobForRun(new UpdateJobTime()
                    {
                        JobId = JobID,
                        UserID = 0
                    });
                    IJobRunner jobRunner = mcfHandler.GetJobRunner("WasdeJob.WasdeJobRunner, WasdeJob");
                    Console.WriteLine($"Started processing {kv.Value}");
                    jobRunner.RUNJob(DataDict, "WASDE", Id);
                }
            }
        }
        private static void ProcessDataByQuery(string dataSource, McFJobHandler mcfHandler, DateTime DataDate)
        {
            IJobRunner jobRunner = null; 
            string url = String.Empty;
            int Id = 0;
            int JobId = 0;

            switch (dataSource.ToUpper())
            {
                case "CROP":
                    jobRunner = mcfHandler.GetJobRunner("CROPJob.CROPJobRunner, CROPJob");
                    JobId = 5;
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1048";
                    break;
                case "BROILERANDHATCHERY":
                    jobRunner = mcfHandler.GetJobRunner("BHJob.BHJobRunner, BHJob");
                    JobId = 9;
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1010";
                    break;
                case "HOGSANDPIGS":
                    JobId = 11;
                    jobRunner = mcfHandler.GetJobRunner("HogsPigsJob.HogsPigsJobRunner, HogsPigsJob");
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1086";
                    break;
                case "CATTLEONFEED":
                    JobId = 7;
                    jobRunner = mcfHandler.GetJobRunner("CattleOnFeedJob.CFJobRunner, CattleOnFeedJob");
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1020";
                    break;
                case "FATSANDOILS":
                    JobId = 10;
                    jobRunner = mcfHandler.GetJobRunner("FatsAndOils.FOJobRunner, FatsAndOils");
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1902";
                    break;
                case "CHICKENANDEGGS":
                    JobId = 8;
                    jobRunner = mcfHandler.GetJobRunner("ChickenEggsJob.CEJobRunner, ChickenEggsJob");
                    url = "http://usda.mannlib.cornell.edu/MannUsda/viewDocumentInfo.do?documentID=1028";
                    break;
                case "WASDE":
                    ProcessWasdeRunner(mcfHandler, DataDate);
                    return;
                case "COT":
                    ProcessCOTRunner(mcfHandler);
                    return;
                default:
                    ProcessOtherDataSource(dataSource, mcfHandler, DataDate);
                   return;
            }
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument resultat = web.Load(url);
            var data = resultat.DocumentNode.SelectNodes("//div[@class='fileElement']");
            Dictionary<string, string> str = new Dictionary<string, string>();
            foreach (HtmlNode node in data)
            {
                string innerText = node.InnerHtml;
                if (innerText.Contains(".zip"))
                {
                    int startlen = innerText.IndexOf("href=\"") + 6;
                    int endlen = innerText.IndexOf("\"", startlen);
                    //int startend = innerText.
                    string dataurl = innerText.Substring(startlen, endlen - startlen);
                    string fileName = Path.GetFileNameWithoutExtension(dataurl);
                    string date = fileName.Replace("CropProg-", String.Empty).Replace("_revision", string.Empty).
                        Replace("_correction",string.Empty).Replace("HogsPigs-", String.Empty).
                        Replace("ChicEggs-", String.Empty).Replace("BroiHatc-", String.Empty).Replace("CattOnFe-", String.Empty).
                        Replace("FatsOils-", String.Empty).Replace("_Non Ambulatory Cattle and Calves", string.Empty);
                    date = date.Substring(0, 10);
                    str[date] = dataurl;
                }
            }
            int dataRow = 19999;
            foreach (KeyValuePair<string, string> kv in str.Reverse())
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("REPLACEFILE", "true");
                dict.Add("FILE", kv.Value);
                dict.Add("DATE", kv.Key);
                Console.WriteLine(kv.Value);
                DateTime dt = Convert.ToDateTime(kv.Key);
                if (dt >= DataDate)
                {
                    Id  = JobService.CreateJobForRun(new UpdateJobTime()
                    {
                        JobId = JobId,
                        UserID = 0
                    });

                    jobRunner.RUNJob(dict, dataSource, Id);
                }
                dataRow++;
            }
        }
        private static YEARTYPE GetWorldYearType( string year )
        {
            YEARTYPE yearType = YEARTYPE.PREVYEAR;
            if (year.Contains("Est"))
                yearType = YEARTYPE.CURRYEAR;
            else if (year.Contains("Proj"))
                yearType = YEARTYPE.PROJYEAR;

            return yearType;
        }
     
        private static string GetWorldCategory(string commodity)
        {
            switch(commodity.ToUpper())
            {
                case "WHEAT":
                case "RICE":
                case "COARSE GRAIN":
                case "CORN":
                    return "Food Grains";
                case "COTTON":
                    return "Cotton";
                case "SOYABEAN":
                case "SOYABEAN MEAL":
                case "SOYABEAN OIL":
                    return "SoyaBean";
                default:
                    return "Agriculture";

            }
        }

        private static Dictionary<int, string> ProcessHeaderRow(DataRow dr)
        {
            int i = 0;
            Dictionary<int, string> Headers = new Dictionary<int, string>();
            while ( true ) //loop failse only if dr value becomes null
            {
                string value = dr[i]?.ToString();
                if (String.IsNullOrEmpty(value) && 
                    String.IsNullOrEmpty(dr[i+1]?.ToString()))
                    break;

                if (!String.IsNullOrEmpty(value))
                {
                    value.Replace('\n', '_').Trim();
                    Headers[i] = value;
                }
                i++;
            }
            return Headers;
        }

        private static WasdeCommodity GetWorldCommodity( string Commodity, string Page)
        {
            WasdeCommodity wasd = null;
            if (WsdComms.ContainsKey(Commodity.ToUpper()))
            {
                wasd = WsdComms[Commodity.ToUpper()];
            }
            else
            {
                wasd = new WasdeCommodity();
                wasd.CommodityName = Commodity;
                wasd.Category = GetWorldCategory(Commodity);
                WsdComms.Add(Commodity.ToUpper(), wasd);
            }
            wasd.Page += $"{Page.Replace("'","")},";
            return wasd;
        }

        private static WasdeDomesticCommodity GetDomesticCommodity(string Commodity, string Page)
        {
            WasdeDomesticCommodity wasd = null;
            if (WsdDomComms.ContainsKey(Commodity.ToUpper()))
            {
                wasd = WsdDomComms[Commodity.ToUpper()];
            }
            else
            {
                wasd = new WasdeDomesticCommodity();
                wasd.CommodityName = Commodity;
                wasd.Category = GetWorldCategory(Commodity);
                WsdDomComms.Add(Commodity.ToUpper(), wasd);
            }
            wasd.Page += $"{Page.Replace("'", "")},";
            return wasd;
        }
        private static string GetWorldCommidityName(string world)
        {
            int index = world.IndexOf(WORLD) + WORLD.Length;
            int endindex = world.IndexOf(SUPPLYUSE) - index;
            String Commodity = world.Substring(index, endindex).Trim();
            return Commodity;
        }

        private static string GetDomesticCommidityName(string domestic)
        {
            int index = domestic.IndexOf(US) + US.Length;
            int endindex = domestic.IndexOf(SUPPLYUSE) - index;
            String Commodity = domestic.Substring(index, endindex).Trim();
            return Commodity;
        }
          
        private static string[] GetExcelSheetNames(string connectionString)
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
