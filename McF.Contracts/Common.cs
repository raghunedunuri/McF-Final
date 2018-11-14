using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class DataCol
    {
        public string Column { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }

        public DataCol(string col)
        {
            Column = col;
        }
    }
    public class ExcelFieldData
    {
        public string DataType { get; set; }
        public bool ReadOnly { get; set; }
        public string DataValue { get; set; }
        public List<string> UpdateQueries { get; set; }

        public ExcelFieldData()
        {
            UpdateQueries = new List<string>();
        }

    }

    public class ExcelHeaderFieldData
    {
        public string Header { get; set; }
        public string DisplayName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public string DataType { get; set; }
        public string Table { get; set; }
        public string Unit { get; set; }

        public ExcelHeaderFieldData(string Name, bool isPrimaryKey, string displayName, string dataType, string tableName)
        {
            Header = Name;
            IsPrimaryKey = isPrimaryKey;
            DataType = dataType;
            Table = tableName;
            DisplayName = displayName;
            Unit = String.Empty;
        }
        public ExcelHeaderFieldData(string Name, string displayName, string dataType, string tableName, string unit)
        {
            Header = Name;
            DisplayName = !String.IsNullOrEmpty(displayName) ? displayName : Name;
            DataType = dataType;
            IsPrimaryKey = false;
            Table = tableName;
            Unit = unit;
        }
    }
    public class ExcelDisplayData
    {
        public bool bReadOnly { get; set; } //Set to true if display format is Matrix or Group by Period
        public bool bInsertAllowed { get; set; } //Set to true only if user selects all fields in a table
        public Dictionary<int, ExcelHeaderFieldData> PrimaryColumns { get; set; }
        public int HeaderRow { get; set; }
        public int UnitRow { get; set; }
        public Dictionary<int, ExcelHeaderFieldData> HeaderData { get; set; }
        public Dictionary<int, string> UnitData { get; set; }
        public Dictionary<int, Dictionary<int, ExcelFieldData>> DataFields { get; set; }

        public ExcelDisplayData()
        {
            bReadOnly = false;
            bInsertAllowed = false;
            PrimaryColumns = new Dictionary<int, ExcelHeaderFieldData>();
            HeaderData = new Dictionary<int, ExcelHeaderFieldData>();
            UnitData = new Dictionary<int, string>();
            DataFields = new Dictionary<int, Dictionary<int, ExcelFieldData>>();
        }
    }
    public class ExcelMatrixDisplayData
    {
        public string Commodity { get; set; }
        public string FieldName { get; set; }
        public string Unit { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public SortedList<int, int> Years { get; set; }
        public ExcelMatrixDisplayData(string commodity, string fieldName, string unit)
        {
            Commodity = commodity;
            FieldName = fieldName;
            Unit = unit;
            Data = new Dictionary<string, string>();
            Years = new SortedList<int, int>();
        }
    }
    public enum DataFeedType
    {
        Dialy = 0,
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3,
        Yearly = 4,
        Fiscal
    }
    public class ExcelQuery
    {
        public string StoredProc { get; set; }
        public List<string> Symbols { get; set; }
        public List<string> Fields { get; set; }
        public Dictionary<string, List<string>> CommFields { get; set; }
        public int DateIndex { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PeriodSummary { get; set; }
        public string PeriodType { get; set; }
        public DataFeedType FeedType { get; set; }
        public int FiscalMonth { get; set; }
    }
    public class HeaderFields
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public string Unit { get; set; }
    }

    public class RowData
    {
        public string Commodity { get; set; }
        public string Category { get; set; }
        public RowData()
        {
            Data = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Data { get; set; }
    }
    public class FormatedData
    {
        public FormatedData()
        {
            Headers = new List<HeaderFields>();
            Values = new List<List<string>>();
        }
        public RootData CommodityData { get; set; }
        public List<HeaderFields> Headers { get; set; }
        public List<List<string>> Values { get; set; }
    }

    public class RootData
    {
        public RootData()
        {
            Properties = new Dictionary<string, string>();
        }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public override bool Equals(object obj)
        {
            return Name == obj.ToString() ? true : false;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class DatasourceData
    {
        public DatasourceData()
        {
            DictFormatedData = new Dictionary<string, FormatedData>();
        }
        public Dictionary<string, FormatedData> DictFormatedData { get; set; }
    }

    public class DataSources
    {
        public string DisplayName { get; set; }
        public string DataSource { get; set; }
        public string RootSource { get; set; }
    }
    public class DSFormatedData
    {
        public DSFormatedData()
        {
            DictFormatedData = new Dictionary<string, FormatedData>();
        }
        public Dictionary<string, FormatedData> DictFormatedData { get; set; }
    }

    public class UserLoginData
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdatedValues
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }
    public class DSUpdatedData
    {
        public DSUpdatedData()
        {
            KeyData = new List<UpdatedValues>();
            ValueData = new List<UpdatedValues>();
        }
        public List<UpdatedValues> KeyData { get; set; }
        public List<UpdatedValues> ValueData { get; set; }
        public string DataSource { get; set; }
        public string Table { get; set; }
    }

    public class DSColumnInfo
    {
        public DSColumnInfo()
        {
        }
        public string DataType { get; set; }
        public string Column { get; set; }
        public string Table { get; set; }
    }

    public class ExcelFormatedData
    {
        public ExcelFormatedData()
        {
            Headers = new List<HeaderFields>();
            Values = new List<List<string>>();
        }
        public RootData CommodityData { get; set; }
        public List<HeaderFields> Headers { get; set; }
        public List<List<string>> Values { get; set; }
    }

    public class WasdeCommodity
    {
        public WasdeCommodity()
        {
            Page = String.Empty;
            bExists = false;
        }
        public string CommodityName { get; set; }
        public string Page { get; set; }
        public String Category { get; set; }
        public string Unit { get; set; }
        public bool bExists { get; set; }

        public string GetPopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            if (Page.Length > 1)
                Page = Page.Substring(0, Page.Length - 1);
            if (!bExists)
            {
                return $"INSERT INTO WASDE_WORLD_COMMODITIES(Commodity_Name, Commodity_Category,Units, Page, JobID, UserID, LastUpdated) Values('{CommodityName}','{CommodityName}','{Unit}', '{Page}',{JobID},{UserId},'{LastUpdated.ToString()}')";
            }
            else
            {
                return $"UPDATE WASDE_WORLD_COMMODITIES SET Page = '{Page}', Units = '{Unit}', JobID = {JobID}, UserID = {UserId}, LastUpdated = '{LastUpdated.ToString()}' where Commodity_Name = '{CommodityName}'";
            }
        }
    }

    public class WasdeDomesticCommodity
    {
        public WasdeDomesticCommodity()
        {
            Page = String.Empty;
            bExists = false;
        }
        public string CommodityName { get; set; }
        public string Page { get; set; }
        public String Category { get; set; }
        public bool bExists { get; set; }

        public string GetPopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            if (Page.Length > 1)
                Page = Page.Substring(0, Page.Length - 1);
            if (!bExists)
            {
                return $"INSERT INTO WASDE_DOMESTIC_COMMODITIES(Commodity_Name, Commodity_Category, Page, JobID, UserID, LastUpdated) Values('{CommodityName}','{CommodityName}','{Page}',{JobID},{UserId},'{LastUpdated.ToString()}')";
            }
            else
            {
                return $"UPDATE WASDE_DOMESTIC_COMMODITIES SET Page = '{Page}', JobID = {JobID}, UserID = {UserId}, LastUpdated = '{LastUpdated.ToString()}' where Commodity_Name = '{CommodityName}'";
            }
        }
    }

    public class WasdeDomesticField
    {
        public WasdeDomesticField()
        {
            bExists = false;
        }
        public string FieldName { get; set; }
        public string Commodity { get; set; }
        public string Unit { get; set; }
        public bool bExists { get; set; }
        public string GetPopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            if (!bExists)
            {
                return $"INSERT INTO WASDE_DOMESTIC_FIELDS(Field, Unit, Commodity_Name, JobID, UserID, LastUpdated) Values('{FieldName}','{Unit}','{Commodity}',{JobID},{UserId},'{LastUpdated.ToString()}')";
            }
            else
            {
                return $"UPDATE WASDE_DOMESTIC_FIELDS SET Unit = '{Unit}', JobID = {JobID}, UserID = {UserId}, LastUpdated = '{LastUpdated.ToString()}' where Commodity_Name = '{Commodity}' and Field = '{FieldName}'";
            }
        }
    }

    public static class CommonOperations
    {
        public static string GetMonthDate(string Mon, int Year)
        {
            string Month = Mon.ToUpper();

            if (Month.Contains("JAN"))
                return $"{Year}-01-31";
            if (Month.Contains("FEB"))
                return $"{Year}-02-{28 + (Year % 4 == 0 ? 1 : 0)}";
            if (Month.Contains("MAR"))
                return $"{Year}-03-31";
            if (Month.Contains("APR"))
                return $"{Year}-04-30";
            if (Month.Contains("MAY"))
                return $"{Year}-05-31";
            if (Month.Contains("JUN"))
                return $"{Year}-06-30";
            if (Month.Contains("JUL"))
                return $"{Year}-07-31";
            if (Month.Contains("AUG"))
                return $"{Year}-08-31";
            if (Month.Contains("SEP"))
                return $"{Year}-09-30";
            if (Month.Contains("OCT"))
                return $"{Year}-10-31";
            if (Month.Contains("NOV"))
                return $"{Year}-11-30";
            if (Month.Contains("DEC"))
                return $"{Year}-12-31";

            return "0001-01-01";
        }
        public static string GetMonthDate(string Mon)
        {
            int Year = DateTime.Now.Year;

            string Month = Mon.ToUpper();

            if (Month.Contains("JAN"))
                return $"{Year}-01-31";
            if (Month.Contains("FEB"))
                return $"{Year}-02-{28 + (Year % 4 == 0 ? 1 : 0)}";
            if (Month.Contains("MAR"))
                return $"{Year}-03-31";
            if (Month.Contains("APR"))
                return $"{Year}-04-30";
            if (Month.Contains("MAY"))
                return $"{Year}-05-31";
            if (Month.Contains("JUN"))
                return $"{Year}-06-30";
            if (Month.Contains("JUL"))
                return $"{Year}-07-31";
            if (Month.Contains("AUG"))
                return $"{Year}-08-31";
            if (Month.Contains("SEP"))
                return $"{Year}-09-30";
            if (Month.Contains("OCT"))
                return $"{Year}-10-31";
            if (Month.Contains("NOV"))
                return $"{Year}-11-30";
            if (Month.Contains("DEC"))
                return $"{Year}-12-31";

            return "0001-01-01";
        }

        public static string GetFOMonthDate(string Mon)
        {
            int Year = DateTime.Now.Year;
            if (Mon.Contains("-"))
                Year = Convert.ToInt32(Mon.Split('-')[1]) + 2000;
            else if (Mon.Contains(" "))
                Year = Convert.ToInt32(Mon.Split(' ')[1]);

            string Month = Mon.ToUpper();

            if (Month.Contains("JAN"))
                return $"{Year}-01-31";
            if (Month.Contains("FEB"))
                return $"{Year}-02-{28 + (Year % 4 == 0 ? 1 : 0)}";
            if (Month.Contains("MAR"))
                return $"{Year}-03-31";
            if (Month.Contains("APR"))
                return $"{Year}-04-30";
            if (Month.Contains("MAY"))
                return $"{Year}-05-31";
            if (Month.Contains("JUN"))
                return $"{Year}-06-30";
            if (Month.Contains("JUL"))
                return $"{Year}-07-31";
            if (Month.Contains("AUG"))
                return $"{Year}-08-31";
            if (Month.Contains("SEP"))
                return $"{Year}-09-30";
            if (Month.Contains("OCT"))
                return $"{Year}-10-31";
            if (Month.Contains("NOV"))
                return $"{Year}-11-30";
            if (Month.Contains("DEC"))
                return $"{Year}-12-31";

            return "0001-01-01";
        }
        public static bool IsMonthType(string Mon)
        {
            string Month = Mon.ToUpper();
            int Year = 0;
            if (int.TryParse(Month, out Year))
                return true;

            if (Month.Contains("JAN"))
                return true;
            if (Month.Contains("FEB"))
                return true;
            if (Month.Contains("MAR"))
                return true;
            if (Month.Contains("APR"))
                return true;
            if (Month.Contains("MAY"))
                return true;
            if (Month.Contains("JUN"))
                return true;
            if (Month.Contains("JUL"))
                return true;
            if (Month.Contains("AUG"))
                return true;
            if (Month.Contains("SEP"))
                return true;
            if (Month.Contains("OCT"))
                return true;
            if (Month.Contains("NOV"))
                return true;
            if (Month.Contains("DEC"))
                return true;

            return false;
        }
    }
    public class WasdeCommData
    {
        public WasdeCommData(string reportDate, string commodity, string dataDate, string region)
        {
            ReportDate = reportDate;
            DataDate = dataDate;
            Commodity = commodity;
            Region = region;
            Values = new Dictionary<string, string>();
        }
        public string Commodity { get; set; }

        public string Region { get; set; }
        public string ReportDate { get; set; }
        public string DataDate { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            string Type = "Prev";
            DateTime dt = Convert.ToDateTime(DataDate);
            int year = Convert.ToDateTime(ReportDate).Year;
            if ((year - 1) == dt.Year)
                Type = "Est";
            else if (year == dt.Year)
                Type = "Proj";

            string ProjType = $"{dt.Year}/{dt.Year + 1} {Type}";

            List<string> lstQueries = new List<string>();
            foreach (KeyValuePair<string, string> kv in Values)
            {
                lstQueries.Add($"DELETE FROM WASDE_WORLD_DATA WHERE Commodity_Name = '{Commodity}' AND ProjType = '{ProjType}' AND Region = '{Region}' AND Field = '{kv.Key}' AND ReportDate = '{ReportDate}'");
                lstQueries.Add($"INSERT INTO WASDE_WORLD_DATA SELECT '{Commodity}','{ReportDate}','{ProjType}','{Region}','{kv.Key}',{kv.Value},{JobID},{UserId},'{LastUpdated.ToString()}'");
            }
            return lstQueries;
        }
    }

    public class WasdeDomesticData
    {
        public WasdeDomesticData(string reportDate, string commodity, string dataDate)
        {
            DataDate = dataDate;
            ReportDate = reportDate;
            Commodity = commodity;
        }
        public string Commodity { get; set; }
        public string ReportDate { get; set; }
        public string DataDate { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }

        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            string Type = "Prev";
            DateTime dt = Convert.ToDateTime(DataDate);
            int year = Convert.ToDateTime(ReportDate).Year;
            if ((year - 1) == dt.Year)
                Type = "Est";
            else if (year == dt.Year)
                Type = "Proj";

            string ProjType = $"{dt.Year}/{dt.Year + 1} {Type}";

            List<string> lstQueries = new List<string>();
            lstQueries.Add($"DELETE FROM WASDE_DOMESTIC_DATA WHERE Commodity_Name = '{Commodity}' AND Field = '{Field}' AND ProjType = '{ProjType}' AND ReportDate = '{ReportDate}'");
            lstQueries.Add($"INSERT INTO WASDE_DOMESTIC_DATA SELECT '{Commodity}','{ReportDate}','{ProjType}','{Field}',{Value},{JobID},{UserId},'{LastUpdated.ToString()}'");
            return lstQueries;
        }
    }

    public class FOData
    {
        public string Commodity { get; set; }
        public string Category { get; set; }
        public string MonthEndDate { get; set; }
        public string DataMonth { get; set; }
        public string ReportMonth { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public bool IsRegionalData { get; set; }
        public string Region { get; set; }
        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            List<string> lstQueries = new List<string>();
            Value = Value.Replace("(D)", "0");
            if (IsRegionalData)
            {
                lstQueries.Add($"DELETE FROM FATSANDOIL_REGIONAL_DATA WHERE COMMODITY_NAME = '{Commodity}' AND CATEGORY = '{Category}' AND MONTH_ENDDATE = '{MonthEndDate}' AND REGION = '{Region}' AND FIELD = '{Field}'");
                lstQueries.Add($"INSERT INTO FATSANDOIL_REGIONAL_DATA(COMMODITY_NAME, CATEGORY, REPORT_DATE, MONTH_ENDDATE, REGION, FIELD, DATAVALUE, JOBID, USERID, LASTUPDATED) VALUES('{Commodity}','{Category}','{ReportMonth}','{MonthEndDate}','{Region}','{Field}',{Value},{JobID},{UserId},'{LastUpdated.ToString()}')");
            }
            else
            {
                lstQueries.Add($"DELETE FROM FATSANDOIL_DATA WHERE COMMODITY_NAME = '{Commodity}' AND CATEGORY = '{Category}' AND MONTH_ENDDATE = '{MonthEndDate}' AND FIELD = '{Field}'");
                lstQueries.Add($"INSERT INTO FATSANDOIL_DATA(COMMODITY_NAME, CATEGORY, REPORT_DATE, MONTH_ENDDATE, FIELD, DATAVALUE, JOBID, USERID, LASTUPDATED) VALUES('{Commodity}','{Category}','{ReportMonth}','{MonthEndDate}','{Field}',{Value},{JobID},{UserId},'{LastUpdated.ToString()}')");
            }
            return lstQueries;
        }
    }

    public class FOField
    {
        public FOField()
        {

        }
        public string Field { get; set; }
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public bool bExists { get; set; }
        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            if (!bExists)
            {
                return $"INSERT INTO FATSANDOIL_FIELDS (FIELD, DISPLAYNAME, CATEGORY, UNIT, JOBID, USERID, LASTUPDATED) VALUES ('{Field}','{DisplayName}','{Category}','{Unit}',{JobID},{UserId},'{LastUpdated.ToString()}')";
            }
            //    else
            //    {
            //        return $"INSERT INTO WASDE_DOMESTIC_YEARLY_DATA ( Commodity_Name, Data_Date, Data_Year,Data_Month,Field,DataValue, JobID, UserID,LastUpdated) Values ('{Commodity}','{Date}','{Year}','{DataMonth}','{Field}','{Value}',{JobID},{UserId},'{LastUpdated.ToString()}')";
            //    }
            return String.Empty;
        }
    }

    public class BHField
    {
        public BHField()
        {

        }
        public BHField(string reportDate, string region, string weekEnding)
        {
            ReportDate = reportDate;
            Region = region;
            WeekEnding = weekEnding;
        }
        public string ReportDate { get; set; }
        public string Region { get; set; }
        public string EggsSet { get; set; }
        public string ChicksPlaced { get; set; }
        public string WeekEnding { get; set; }
        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            EggsSet = EggsSet.Replace("*", String.Empty);
            ChicksPlaced = ChicksPlaced.Replace("*", String.Empty);
            WeekEnding = WeekEnding.Replace("~", "-");
            List<string> lstQueries = new List<string>();
            DateTime dt = DateTime.Parse(WeekEnding);
            lstQueries.Add($"DELETE FROM BROHAT_DIALY_DATA WHERE WeekEnding = '{dt.ToShortDateString()}' AND Region = '{Region}'");
            lstQueries.Add($"INSERT INTO BROHAT_DIALY_DATA (WeekEnding, Region, ReportDate, EggsSet, ChicksPlaced, JobID, UserID, LastUpdated) VALUES ('{dt.ToShortDateString()}','{Region}','{ReportDate}',{EggsSet},{ChicksPlaced},{JobID},{UserId},'{LastUpdated.ToString()}')");
            return lstQueries;
        }
    }

    public class CFData
    {
        public CFData()
        {
            SteersAndCalves = "0";
            HeifersAndCalves = "0";
        }
        public CFData(string reportDate, string beginDate, string endDate)
        {
            ReportDate = reportDate;
            BeginDate = beginDate;
            EndDate = endDate;
            SteersAndCalves = "0";
            HeifersAndCalves = "0";
        }
        public string ReportDate { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string Begin_Inventory { get; set; }
        public string Placed_During_Month { get; set; }
        public string Marketed_During_Month { get; set; }
        public string Other_Disappearances { get; set; }
        public string Month_Ending_Inventory { get; set; }
        public string SteersAndCalves { get; set; }
        public string HeifersAndCalves { get; set; }
        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            DateTime dt = DateTime.Parse(ReportDate);
            string bDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(-1).ToShortDateString();
            string eDate = new DateTime(dt.Year, dt.Month, 1).ToShortDateString();
            try
            {
                DateTime Begin = DateTime.Parse(BeginDate);
                int Year = dt.Year;
                if (Begin > dt)
                    Year = dt.Year - 1;

                Begin = new DateTime(Year, Begin.Month, Begin.DayOfYear);
                bDate = Begin.ToShortDateString();
            }
            catch (Exception ex)
            {

            }
            try
            {
                DateTime End = DateTime.Parse(EndDate);
                int Year = dt.Year;
                if (End > dt)
                    Year = dt.Year - 1;

                End = new DateTime(Year, End.Month, End.DayOfYear);
                eDate = End.ToShortDateString();
            }
            catch (Exception ex)
            {

            }
            List<string> str = new List<string>();
            str.Add($"DELETE FROM CATFEED_DIALY_DATA WHERE BEGINDATE = '{bDate}'");
            str.Add($"INSERT INTO CATFEED_DIALY_DATA SELECT '{bDate}','{eDate}','{ReportDate}',{Begin_Inventory},{Placed_During_Month},{Marketed_During_Month},{Other_Disappearances},{Month_Ending_Inventory},{SteersAndCalves},{HeifersAndCalves},{JobID},{UserId},'{LastUpdated.ToString()}'");
            return str;
        }
    }

    public class HPData
    {
        public HPData()
        {
            Pig_Crop = "0";
            Pig_Per_Litter = "0";
            Sows_Farrowing = "0";
            Inventory = "0";
            KeptForBreeding = "0";
            Market_Inventory = "0";
            Market_Inventory_50LB = "0";
            Market_Inventory_50_119LB = "0";
            Market_Inventory_120_179LB = "0";
            Market_Inventory_180LB = "0";
        }
        public HPData(string reportDate, string quarter)
        {
            ReportDate = reportDate;
            Quarter = quarter;
            Pig_Crop = "0";
            Pig_Per_Litter = "0";
            Sows_Farrowing = "0";
            Inventory = "0";
            KeptForBreeding = "0";
            Market_Inventory = "0";
            Market_Inventory_50LB = "0";
            Market_Inventory_50_119LB = "0";
            Market_Inventory_120_179LB = "0";
            Market_Inventory_180LB = "0";
        }
        public string ReportDate { get; set; }
        public string Quarter { get; set; }
        public string Inventory { get; set; }
        public string KeptForBreeding { get; set; }
        public string Market_Inventory { get; set; }
        public string Market_Inventory_50LB { get; set; }
        public string Market_Inventory_50_119LB { get; set; }
        public string Market_Inventory_120_179LB { get; set; }
        public string Market_Inventory_180LB { get; set; }
        public string Pig_Crop { get; set; }
        public string Pig_Per_Litter { get; set; }
        public string Sows_Farrowing { get; set; }
        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO HOGSPIGS_DIALY_DATA SELECT '{ReportDate}','{Quarter}',{Inventory},{KeptForBreeding},{Market_Inventory},{Market_Inventory_50LB},{Market_Inventory_50_119LB},{Market_Inventory_120_179LB},{Market_Inventory_180LB},{Pig_Crop},{Pig_Per_Litter},{Sows_Farrowing},{JobID},{UserId},'{LastUpdated.ToString()}'";
        }
    }

    public class CEData
    {
        public void InitData()
        {
            BOM_AllLayers = "0";
            BOM_TableEggsType = "0";
            BOM_HatchingEggType = "0";
            BOM_HatchingBroilerType = "0";
            BOM_EggTypeHatching = "0";
            BOM_EggsPer100L_AllLayers = "0";
            BOM_EggsPer100L_TableEggType = "0";
            BOM_EggsPer100L_HatchingEggType = "0";
            BOM_EggsPer100L_HatchingBroilerType = "0";
            BOM_EggsPer100L_EggTypeHatching = "0";
            DTM_EggsProduced_AllLayers = "0";
            DTM_EggsProduced_TableEggType = "0";
            DTM_EggsProduced_HatchingEggType = "0";
            DTM_EggsProduced_HatchingBroilerType = "0";
            DTM_EggsProduced_EggTypeHatching = "0";
            DTM_AllLayers = "0";
            DTM_TableEggsType = "0";
            DTM_HatchingEggType = "0";
            DTM_HatchingBroilerType = "0";
            DTM_EggTypeHatching = "0";
            DTM_EggsPer100L_AllLayers = "0";
            DTM_EggsPer100L_TableEggType = "0";
            DTM_EggsPer100L_HatchingEggType = "0";
            DTM_EggsPer100L_HatchingBroilerType = "0";
            DTM_EggsPer100L_EggTypeHatching = "0";
            BOM_Being_Molted = "0";
            BOM_Molt_Completed = "0";
            DTM_LayersSlaughtered = "0";
            DTM_Disappeared = "0";
            BOM_Pullets = "0";
            DTM_PulletsAdded = "0";
            BOM_EggType_EggsInIncubation = "0";
            DTM_EggType_IntendedPlacement_HatcherySupply = "0";
            BOM_BroilerType_EggsInIncubation = "0";
            DTM_BroilerType_ChicksHatched = "0";
            DTM_BroilerType_IntendedPlacement_HatcherySupply = "0";
            DTM_BroilerType_IntendedPlacement_CummPlacements = "0";
            DTM_EggType_ChicksHatched = "0";
            DTM_EggType_IntendedPlacement_CummPlacements = "0";
        }
        public CEData()
        {
            InitData();
        }
        public CEData(string reportDate, string bomMonth, string dtmMonth)
        {
            InitData();
        }
        public string Report_Date { get; set; }
        public string BOM_Month { get; set; }
        public string DTM_Month { get; set; }
        public string BOM_AllLayers { get; set; }
        public string BOM_TableEggsType { get; set; }
        public string BOM_HatchingEggType { get; set; }
        public string BOM_HatchingBroilerType { get; set; }
        public string BOM_EggTypeHatching { get; set; }
        public string BOM_EggsPer100L_AllLayers { get; set; }
        public string BOM_EggsPer100L_TableEggType { get; set; }
        public string BOM_EggsPer100L_HatchingEggType { get; set; }
        public string BOM_EggsPer100L_HatchingBroilerType { get; set; }
        public string BOM_EggsPer100L_EggTypeHatching { get; set; }
        public string DTM_EggsProduced_AllLayers { get; set; }
        public string DTM_EggsProduced_TableEggType { get; set; }
        public string DTM_EggsProduced_HatchingEggType { get; set; }
        public string DTM_EggsProduced_HatchingBroilerType { get; set; }
        public string DTM_EggsProduced_EggTypeHatching { get; set; }
        public string DTM_AllLayers { get; set; }
        public string DTM_TableEggsType { get; set; }
        public string DTM_HatchingEggType { get; set; }
        public string DTM_HatchingBroilerType { get; set; }
        public string DTM_EggTypeHatching { get; set; }
        public string DTM_EggsPer100L_AllLayers { get; set; }
        public string DTM_EggsPer100L_TableEggType { get; set; }
        public string DTM_EggsPer100L_HatchingEggType { get; set; }
        public string DTM_EggsPer100L_HatchingBroilerType { get; set; }
        public string DTM_EggsPer100L_EggTypeHatching { get; set; }
        public string BOM_Being_Molted { get; set; }
        public string BOM_Molt_Completed { get; set; }
        public string DTM_LayersSlaughtered { get; set; }
        public string DTM_Disappeared { get; set; }
        public string BOM_Pullets { get; set; }
        public string DTM_PulletsAdded { get; set; }
        public string BOM_EggType_EggsInIncubation { get; set; }
        public string DTM_EggType_ChicksHatched { get; set; }
        public string DTM_EggType_IntendedPlacement_HatcherySupply { get; set; }
        public string DTM_EggType_IntendedPlacement_CummPlacements { get; set; }
        public string BOM_BroilerType_EggsInIncubation { get; set; }
        public string DTM_BroilerType_ChicksHatched { get; set; }
        public string DTM_BroilerType_IntendedPlacement_HatcherySupply { get; set; }
        public string DTM_BroilerType_IntendedPlacement_CummPlacements { get; set; }

        public List<string> PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            List<string> lstString = new List<string>();
            lstString.Add($"Delete from CHICKENEGGS_DIALY_DATA WHERE BOM_Month = '{BOM_Month}' AND DTM_Month = '{DTM_Month}'");
            lstString.Add($"INSERT INTO CHICKENEGGS_DIALY_DATA SELECT '{Report_Date}','{BOM_Month}','{DTM_Month}',{ BOM_AllLayers},{ BOM_TableEggsType},{ BOM_HatchingEggType},{ BOM_HatchingBroilerType},{ BOM_EggTypeHatching},{ BOM_EggsPer100L_AllLayers},{ BOM_EggsPer100L_TableEggType},{ BOM_EggsPer100L_HatchingEggType},{ BOM_EggsPer100L_HatchingBroilerType},{ BOM_EggsPer100L_EggTypeHatching},{ DTM_EggsProduced_AllLayers},{ DTM_EggsProduced_TableEggType},{ DTM_EggsProduced_HatchingEggType},{ DTM_EggsProduced_HatchingBroilerType},{ DTM_EggsProduced_EggTypeHatching},{ DTM_AllLayers},{ DTM_TableEggsType},{ DTM_HatchingEggType},{ DTM_HatchingBroilerType},{ DTM_EggTypeHatching},{ DTM_EggsPer100L_AllLayers},{ DTM_EggsPer100L_TableEggType},{ DTM_EggsPer100L_HatchingEggType},{ DTM_EggsPer100L_HatchingBroilerType},{ DTM_EggsPer100L_EggTypeHatching},{ BOM_Being_Molted},{ BOM_Molt_Completed},{ DTM_LayersSlaughtered},{ DTM_Disappeared},{ BOM_Pullets},{ DTM_PulletsAdded},{ BOM_EggType_EggsInIncubation},{ DTM_EggType_ChicksHatched},{ DTM_EggType_IntendedPlacement_HatcherySupply},{DTM_EggType_IntendedPlacement_CummPlacements},{ BOM_BroilerType_EggsInIncubation},{ DTM_BroilerType_ChicksHatched},{ DTM_BroilerType_IntendedPlacement_HatcherySupply},{ DTM_BroilerType_IntendedPlacement_CummPlacements}, {JobID},{ UserId},'{LastUpdated.ToString()}'");
            return lstString;
        }
    }

    public class SugarData
    {
        public void InitData()
        {
            BEGINNNING_STOCKS = "0";
            TOTAL_PRODUCTION = "0";
            TP_BEET = "0";
            TP_CANE = "0";
            IMPORTS = "0";
            SUPPLY = "0";
            EXPORTS = "0";
            DOMESTIC_DELIVERY = "0";
            DD_NONHUMAN = "0";
            DD_PRODUCT_REEXPORT = "0";
            DD_HUMANUSE = "0";
            DD_BEET = "0";
            DD_CANE = "0";
            DD_NONREPORTERS = "0";
            SUPPLY_ADJUSTEMENT = "0";
            TOTAL_USE = "0";
            ENDING_STOCKS = "0";
        }
        public SugarData()
        {
            InitData();
        }
        public SugarData(string reportDate, string bomMonth)
        {
            MONTH_DATE = bomMonth;
            REPORT_DATE = reportDate;
            InitData();
        }

        public string MONTH_DATE { get; set; }
        public string REPORT_DATE { get; set; }
        public string BEGINNNING_STOCKS { get; set; }
        public string TOTAL_PRODUCTION { get; set; }
        public string TP_BEET { get; set; }
        public string TP_CANE { get; set; }
        public string IMPORTS { get; set; }
        public string SUPPLY { get; set; }
        public string EXPORTS { get; set; }
        public string DOMESTIC_DELIVERY { get; set; }
        public string DD_NONHUMAN { get; set; }
        public string DD_PRODUCT_REEXPORT { get; set; }
        public string DD_HUMANUSE { get; set; }
        public string DD_BEET { get; set; }
        public string DD_CANE { get; set; }
        public string DD_NONREPORTERS { get; set; }
        public string SUPPLY_ADJUSTEMENT { get; set; }
        public string TOTAL_USE { get; set; }
        public string ENDING_STOCKS { get; set; }
        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO SUGAR_SUPPLY_DEMAND_DATA SELECT '{ REPORT_DATE}','{ MONTH_DATE}',{ BEGINNNING_STOCKS},{ TOTAL_PRODUCTION},{ TP_BEET},{ TP_CANE},{ IMPORTS},{ SUPPLY},{ EXPORTS},{ DOMESTIC_DELIVERY},{ DD_NONHUMAN},{ DD_PRODUCT_REEXPORT},{ DD_HUMANUSE},{ DD_BEET},{ DD_CANE},{ DD_NONREPORTERS},{ SUPPLY_ADJUSTEMENT},{ TOTAL_USE},{ ENDING_STOCKS},{JobID},{UserId},'{LastUpdated.ToShortDateString()}';";
        }
    }

    public class SugarBuyerData
    {
        public void InitData()
        {
            TOTAL_DEL = "0";
            TOTAL_DEL_BEET = "0";
            TOTAL_DEL_CANE = "0";
            DEL_BAKERY_CEREAL_REL_PROD = "0";
            DEL_BEET_BAKERY_CEREAL_REL_PROD = "0";
            DEL_CANE_BAKERY_CEREAL_REL_PROD = "0";
            DEL_CONF_REL_PROD = "0";
            DEL_BEET_CONF_REL_PROD = "0";
            DEL_CANE_CONF_REL_PROD = "0";
            DEL_ICECREAM_DAIRY_PROD = "0";
            DEL_BEET_ICECREAM_DAIRY_PROD = "0";
            DEL_CANE_ICECREAM_DAIRY_PROD = "0";
            DEL_BEVERAGES = "0";
            DEL_BEET_BEVERAGES = "0";
            DEL_CANE_BEVERAGES = "0";
            DEL_CANNED_BOTTLE_FROZEN = "0";
            DEL_BEET_CANNED_BOTTLE_FROZEN = "0";
            DEL_CANE_CANNED_BOTTLE_FROZEN = "0";
            DEL_MULTIPLE_OTHERFOOD = "0";
            DEL_BEET_MULTIPLE_OTHERFOOD = "0";
            DEL_CANE_MULTIPLE_OTHERFOOD = "0";
            DEL_NONFOOD = "0";
            DEL_BEET_NONFOOD = "0";
            DEL_CANE_NONFOOD = "0";
            DEL_HOT_RES_INS = "0";
            DEL_BEET_HOT_RES_INS = "0";
            DEL_CANE_HOT_RES_INS = "0";
            DEL_WS_JOBBERS_DEALERS = "0";
            DEL_BEET_WS_JOBBERS_DEALERS = "0";
            DEL_CANE_WS_JOBBERS_DEALERS = "0";
            DEL_RETAILGROC_CHAINSTORE = "0";
            DEL_BEET_RETAILGROC_CHAINSTORE = "0";
            DEL_CANE_RETAILGROC_CHAINSTORE = "0";
            DEL_GOVT_AGEN = "0";
            DEL_BEET_GOVT_AGEN = "0";
            DEL_CANE_GOVT_AGEN = "0";
            DEL_OTHERS = "0";
            DEL_BEET_OTHERS = "0";
            DEL_CANE_OTHERS = "0";
        }
        public SugarBuyerData(string reportDate, string bomMonth)
        {
            MONTH_DATE = bomMonth;
            REPORT_DATE = reportDate;
            InitData();
        }
        public string MONTH_DATE { get; set; }
        public string REPORT_DATE { get; set; }
        public string TOTAL_DEL { get; set; }
        public string TOTAL_DEL_BEET { get; set; }
        public string TOTAL_DEL_CANE { get; set; }
        public string DEL_BAKERY_CEREAL_REL_PROD { get; set; }
        public string DEL_BEET_BAKERY_CEREAL_REL_PROD { get; set; }
        public string DEL_CANE_BAKERY_CEREAL_REL_PROD { get; set; }
        public string DEL_CONF_REL_PROD { get; set; }
        public string DEL_BEET_CONF_REL_PROD { get; set; }
        public string DEL_CANE_CONF_REL_PROD { get; set; }
        public string DEL_ICECREAM_DAIRY_PROD { get; set; }
        public string DEL_BEET_ICECREAM_DAIRY_PROD { get; set; }
        public string DEL_CANE_ICECREAM_DAIRY_PROD { get; set; }
        public string DEL_BEVERAGES { get; set; }
        public string DEL_BEET_BEVERAGES { get; set; }
        public string DEL_CANE_BEVERAGES { get; set; }
        public string DEL_CANNED_BOTTLE_FROZEN { get; set; }
        public string DEL_BEET_CANNED_BOTTLE_FROZEN { get; set; }
        public string DEL_CANE_CANNED_BOTTLE_FROZEN { get; set; }
        public string DEL_MULTIPLE_OTHERFOOD { get; set; }
        public string DEL_BEET_MULTIPLE_OTHERFOOD { get; set; }
        public string DEL_CANE_MULTIPLE_OTHERFOOD { get; set; }
        public string DEL_NONFOOD { get; set; }
        public string DEL_BEET_NONFOOD { get; set; }
        public string DEL_CANE_NONFOOD { get; set; }
        public string DEL_HOT_RES_INS { get; set; }
        public string DEL_BEET_HOT_RES_INS { get; set; }
        public string DEL_CANE_HOT_RES_INS { get; set; }
        public string DEL_WS_JOBBERS_DEALERS { get; set; }
        public string DEL_BEET_WS_JOBBERS_DEALERS { get; set; }
        public string DEL_CANE_WS_JOBBERS_DEALERS { get; set; }
        public string DEL_RETAILGROC_CHAINSTORE { get; set; }
        public string DEL_BEET_RETAILGROC_CHAINSTORE { get; set; }
        public string DEL_CANE_RETAILGROC_CHAINSTORE { get; set; }
        public string DEL_GOVT_AGEN { get; set; }
        public string DEL_BEET_GOVT_AGEN { get; set; }
        public string DEL_CANE_GOVT_AGEN { get; set; }
        public string DEL_OTHERS { get; set; }
        public string DEL_BEET_OTHERS { get; set; }
        public string DEL_CANE_OTHERS { get; set; }

        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO SUGAR_BUYER_DATA SELECT '{ REPORT_DATE}','{ MONTH_DATE}',{ TOTAL_DEL},{ TOTAL_DEL_BEET},{ TOTAL_DEL_CANE},{ DEL_BAKERY_CEREAL_REL_PROD},{ DEL_BEET_BAKERY_CEREAL_REL_PROD},{ DEL_CANE_BAKERY_CEREAL_REL_PROD},{ DEL_CONF_REL_PROD},{ DEL_BEET_CONF_REL_PROD},{ DEL_CANE_CONF_REL_PROD},{ DEL_ICECREAM_DAIRY_PROD},{ DEL_BEET_ICECREAM_DAIRY_PROD},{ DEL_CANE_ICECREAM_DAIRY_PROD},{ DEL_BEVERAGES},{ DEL_BEET_BEVERAGES},{ DEL_CANE_BEVERAGES},{ DEL_CANNED_BOTTLE_FROZEN},{ DEL_BEET_CANNED_BOTTLE_FROZEN},{ DEL_CANE_CANNED_BOTTLE_FROZEN},{ DEL_MULTIPLE_OTHERFOOD},{ DEL_BEET_MULTIPLE_OTHERFOOD},{ DEL_CANE_MULTIPLE_OTHERFOOD},{ DEL_NONFOOD},{ DEL_BEET_NONFOOD},{ DEL_CANE_NONFOOD},{ DEL_HOT_RES_INS},{ DEL_BEET_HOT_RES_INS},{ DEL_CANE_HOT_RES_INS},{ DEL_WS_JOBBERS_DEALERS},{ DEL_BEET_WS_JOBBERS_DEALERS},{ DEL_CANE_WS_JOBBERS_DEALERS},{ DEL_RETAILGROC_CHAINSTORE},{ DEL_BEET_RETAILGROC_CHAINSTORE},{ DEL_CANE_RETAILGROC_CHAINSTORE},{ DEL_GOVT_AGEN},{ DEL_BEET_GOVT_AGEN},{ DEL_CANE_GOVT_AGEN},{ DEL_OTHERS},{ DEL_BEET_OTHERS},{ DEL_CANE_OTHERS},{JobID},{UserId},'{LastUpdated.ToShortDateString()}';";
        }
    }

    public class SugarRegionDelData //Table 8
    {
        public void InitData()
        {
            TOTAL_DEL = "0";
            DEL_BAKERY_CEREAL_REL_PROD = "0";
            DEL_CONF_REL_PROD = "0";
            DEL_ICECREAM_DAIRY_PROD = "0";
            DEL_BEVERAGES = "0";
            DEL_CANNED_BOTTLE_FROZEN = "0";
            DEL_MULTIPLE_OTHERFOOD = "0";
            DEL_NONFOOD = "0";
            DEL_HOT_RES_INS = "0";
            DEL_WS_JOBBERS_DEALERS = "0";
            DEL_RETAILGROC_CHAINSTORE = "0";
            DEL_GOVT_AGEN = "0";
            DEL_OTHERS = "0";
        }
        public SugarRegionDelData()
        {
            InitData();
        }
        public SugarRegionDelData(string reportDate, string bomMonth, string region)
        {
            MONTH_DATE = bomMonth;
            REPORT_DATE = reportDate;
            Region = region;
            InitData();
        }

        public string MONTH_DATE { get; set; }
        public string REPORT_DATE { get; set; }
        public string Region { get; set; }
        public string TOTAL_DEL { get; set; }
        public string DEL_BAKERY_CEREAL_REL_PROD { get; set; }
        public string DEL_CONF_REL_PROD { get; set; }
        public string DEL_ICECREAM_DAIRY_PROD { get; set; }
        public string DEL_BEVERAGES { get; set; }
        public string DEL_CANNED_BOTTLE_FROZEN { get; set; }
        public string DEL_MULTIPLE_OTHERFOOD { get; set; }
        public string DEL_NONFOOD { get; set; }
        public string DEL_HOT_RES_INS { get; set; }
        public string DEL_WS_JOBBERS_DEALERS { get; set; }
        public string DEL_RETAILGROC_CHAINSTORE { get; set; }
        public string DEL_GOVT_AGEN { get; set; }
        public string DEL_OTHERS { get; set; }

        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_RegionDelData SELECT '{ REPORT_DATE}','{ MONTH_DATE}','{Region}',{ TOTAL_DEL},{ DEL_BAKERY_CEREAL_REL_PROD},{ DEL_CONF_REL_PROD},{ DEL_ICECREAM_DAIRY_PROD},{ DEL_BEVERAGES},{ DEL_CANNED_BOTTLE_FROZEN},{ DEL_MULTIPLE_OTHERFOOD},{ DEL_NONFOOD},{ DEL_HOT_RES_INS},{ DEL_WS_JOBBERS_DEALERS},{ DEL_RETAILGROC_CHAINSTORE},{ DEL_GOVT_AGEN},{ DEL_OTHERS},{JobID},{UserId},'{LastUpdated.ToShortDateString()}';";
        }
    }

    public class SugarImportData //Table 1B
    {
        public void InitData()
        {
            Mex_CRRawImports = "0";
            Mex_CRRefinedImports = "0";
            Mex_NonRefinedImports = "0";
            Mex_TotalImports = "0";
            ROW_CRRawImports = "0";
            ROW_CRRefinedImports = "0";
            ROW_NonRefinedImports = "0";
            ROW_TotalImports = "0";
            Total_CRRawImports = "0";
            Total_CRRefinedImports = "0";
            Total_NonRefinedImports = "0";
            TotalImports = "0";
        }
        public SugarImportData()
        {
            InitData();
        }
        public SugarImportData(string reportDate, string bomMonth)
        {
            MONTH_DATE = bomMonth;
            REPORT_DATE = reportDate;
            InitData();
        }

        public string MONTH_DATE { get; set; }
        public string REPORT_DATE { get; set; }
        public string Mex_CRRawImports { get; set; }
        public string Mex_CRRefinedImports { get; set; }
        public string Mex_NonRefinedImports { get; set; }
        public string Mex_TotalImports { get; set; }
        public string ROW_CRRawImports { get; set; }
        public string ROW_CRRefinedImports { get; set; }
        public string ROW_NonRefinedImports { get; set; }
        public string ROW_TotalImports { get; set; }
        public string Total_CRRawImports { get; set; }
        public string Total_CRRefinedImports { get; set; }
        public string Total_NonRefinedImports { get; set; }
        public string TotalImports { get; set; }
        public string PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_ImportData SELECT '{ REPORT_DATE}','{ MONTH_DATE}',{ Mex_CRRawImports},{ Mex_CRRefinedImports},{ Mex_NonRefinedImports},{ Mex_TotalImports},{ ROW_CRRawImports},{ ROW_CRRefinedImports},{ ROW_NonRefinedImports},{ ROW_TotalImports},{ Total_CRRawImports},{ Total_CRRefinedImports},{ Total_NonRefinedImports},{ TotalImports},{JobID},{UserId},'{LastUpdated.ToShortDateString()}';";
        }
    }
    public class WorldSweetnerData
    {
        public string ReportDate { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public int ReportYear { get; set; }
        public long BeginningStocks { get; set; }
        public long TotalSugarProduction { get; set; }
        public long TotalImports { get; set; }
        public long TotalSupply { get; set; }
        public long TotalExports { get; set; }
        public long TotalUse { get; set; }
        public long EndingStocks { get; set; }

        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO WORLD_SWEETENER_DATA SELECT '{ReportDate}', '{Area}','{Region}', {ReportYear}, {BeginningStocks},{TotalSugarProduction},{TotalImports},{TotalSupply},{TotalExports},{TotalUse},{EndingStocks},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class SugarUSDAPrices
    {
        public string ReportDate { get; set; }
        public int ReportYear { get; set; }
        public string ReportIdnt { get; set; }
        public double WorldRefinedSugar { get; set; }
        public double WorldRawSugar { get; set; }
        public double WorldRawSugarICE { get; set; }
        public double USRawSugar { get; set; }
        public double USWholesaleRefinedBeetSugar { get; set; }
        public double USRetailRefinedSugar { get; set; }
        public double USWholesaleListDextrose { get; set; }
        public double USWholesaleListGlucose { get; set; }
        public double SpotPriceHFCS42 { get; set; }
        public double WholeSaleSpotHFCS55 { get; set; }
        public double WholeSaleListHFCS42 { get; set; }
        public double WholeSaleListHFCS55 { get; set; }

        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDAPrices SELECT '{ReportDate}', {ReportYear},'{ReportIdnt}', {WorldRefinedSugar}, {WorldRawSugar},{WorldRawSugarICE},{USRawSugar},{USWholesaleRefinedBeetSugar},{USRetailRefinedSugar},{USWholesaleListDextrose},{USWholesaleListGlucose},{SpotPriceHFCS42},{WholeSaleSpotHFCS55},{WholeSaleListHFCS42},{WholeSaleListHFCS55},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }

    public class USDASugarProductionArea
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string Region { get; set; }
        public double Total { get; set; }
        public double AreaForSeed { get; set; }
        public double AreaForSugar { get; set; }
        public double PercForSeed { get; set; }
        public double YieldForSugar { get; set; }
        public double SugarCaneProduction { get; set; }
        public double SugarProduction { get; set; }
        public double RecoveryRate { get; set; }
        public double YieldPerAcre { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDASugarProductionArea SELECT '{ReportDate}', {Year},'{Region}', {Total}, {AreaForSeed},{AreaForSugar},{PercForSeed},{YieldForSugar},{SugarCaneProduction}, {SugarProduction},{RecoveryRate},{YieldPerAcre},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class USDACaneBeetShare
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public double BeetSugarProduced { get; set; }
        public double CaneSugarProduced { get; set; }
        public double BeetAndCaneProduced { get; set; }
        public double BeetSharePercentOfProduction { get; set; }
        public double CaneSharePercentOfProduction { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDACaneBeetShare SELECT '{ReportDate}', {Year},{BeetSugarProduced},{CaneSugarProduced},{BeetAndCaneProduced}, {BeetSharePercentOfProduction},{CaneSharePercentOfProduction},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class USDAProduction
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public double Planted { get; set; }
        public double Harvested { get; set; }
        public double Produced { get; set; }
        public double YieldPerAcre { get; set; }
        public double SugarProduced { get; set; }
        public double RecoveryRate { get; set; }
        public double SugarYieldPerAcre { get; set; }
        public string SugarType { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDAProduction SELECT '{ReportDate}', {Year},{Planted},{Harvested},{Produced}, {YieldPerAcre},{SugarProduced},{RecoveryRate},{SugarYieldPerAcre},'{SugarType}',{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }

    public class USDASugarProductionByState
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string Region { get; set; }
        public string ReportIdnt { get; set; }
        public double Production { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDASugarProductionByState SELECT '{ReportDate}', {Year},'{Region}','{ReportIdnt}',{Production},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class USDASupplyUse //24a (NOT LOADED)
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public double Beginningstocks { get; set; }
        public double TotalProduction { get; set; }
        public double BeetSugar { get; set; }
        public double CaneSugar { get; set; }
        public double Florida { get; set; }
        public double Louisiana { get; set; }
        public double Texas { get; set; }
        public double Hawaii { get; set; }
        public double PuertoRico { get; set; }
        public double Totalimports { get; set; }
        public double TariffQuotaImports { get; set; }
        public double OtherProgramImports { get; set; }
        public double NonProgramImports { get; set; }
        public double Mexico { get; set; }
        public double TotalSupply { get; set; }
        public double TotalExports { get; set; }
        public double QuotaExemptforReexport { get; set; }
        public double OtherExports { get; set; }
        public double CCCdisposalforExport { get; set; }
        public double Miscellaneous { get; set; }
        public double CCCDisposalforNonfood { get; set; }
        public double RefiningLossAdjustment { get; set; }
        public double StatisticalAdjustment { get; set; }
        public double DeliveriesforDomesticUse { get; set; }
        public double ReexportProgram { get; set; }
        public double TransferToAlcoholFeedEthanol { get; set; }
        public double DomestiFoodandBeverageUse { get; set; }
        public double TotalUse { get; set; }
        public double EndingStocks { get; set; }
        public double PrivatelyOwned { get; set; }
        public double CCC { get; set; }
        public double StocksToUseRatio { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDASupplyUse SELECT '{ReportDate}', {Year},{Beginningstocks},{TotalProduction},{BeetSugar},{CaneSugar},{Florida},{Louisiana},{Texas},{ Hawaii},{ PuertoRico},{Totalimports},{TariffQuotaImports},{OtherProgramImports},{NonProgramImports},{Mexico},{TotalSupply},{TotalExports},{QuotaExemptforReexport},{OtherExports},{CCCdisposalforExport},{ Miscellaneous},{CCCDisposalforNonfood},{RefiningLossAdjustment},{StatisticalAdjustment },{ DeliveriesforDomesticUse},{ ReexportProgram},{ TransferToAlcoholFeedEthanol},{ DomestiFoodandBeverageUse },{ TotalUse },{ EndingStocks},{ PrivatelyOwned},{ CCC}, { StocksToUseRatio} ,{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class USDASupplyUseEstimates //25,26
    {
        public string ReportDate { get; set; }
        public string DataDate { get; set; }
        public double Beginningstocks { get; set; }
        public double TotalProduction { get; set; }
        public double BeetSugar { get; set; }
        public double CaneSugar { get; set; }
        public double Florida { get; set; }
        public double Louisiana { get; set; }
        public double Texas { get; set; }
        public double Hawaii { get; set; }
        public double Totalimports { get; set; }
        public double TariffQuotaImports { get; set; }
        public double OtherProgramImports { get; set; }
        public double NonProgramImports { get; set; }
        public double Mexico { get; set; }
        public double TotalSupply { get; set; }
        public double TotalExports { get; set; }
        public double Adjustments { get; set; }
        public double TotalDeliveries { get; set; }
        public double DomestiFoodandBeverageUse { get; set; }
        public double OtherUse { get; set; }
        public double TotalUse { get; set; }
        public double EndingStocks { get; set; }
        public double StocksToUseRatio { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDASupplyUseEstimates SELECT '{ReportDate}', '{DataDate}',{Beginningstocks},{TotalProduction},{BeetSugar},{CaneSugar},{Florida},{Louisiana},{Texas},{ Hawaii},{Totalimports},{TariffQuotaImports},{OtherProgramImports},{NonProgramImports},{Mexico},{TotalSupply},{TotalExports},{ Adjustments},{ TotalDeliveries},{ DomestiFoodandBeverageUse},{ OtherUse},{ TotalUse},{ EndingStocks}, { StocksToUseRatio} ,{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        } //NOT LOADED
    }
    public class SweetnerUSDASupplyUseEstimates //27
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public double HFCS { get; set; }
        public double GlucoseSyrupandDextrose { get; set; }
        public double TotalCornSweetener { get; set; }
        public double CornStarch { get; set; }
        public double WetmillingexcludingAlcohol { get; set; }
        public double AlcoholFuel { get; set; }
        public double AlcoholBeverage { get; set; }
        public double AlcoholTotal { get; set; }
        public double Total { get; set; }
        public double USCornCrop { get; set; }
        public double CornSweetenerShare { get; set; }
        public double WetmillingExcludingAlcoholShare { get; set; }
        public double AlcoholShare { get; set; }
        public double TotalPerc { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sweetner_USDACornSupplyUse SELECT '{ReportDate}', {Year},{HFCS},{GlucoseSyrupandDextrose},{TotalCornSweetener},{CornStarch},{WetmillingexcludingAlcohol},{AlcoholFuel},{ AlcoholBeverage},{ AlcoholTotal},{ Total},{ USCornCrop},{ CornSweetenerShare},{ WetmillingExcludingAlcoholShare},{ AlcoholShare},{ TotalPerc},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        } //NOT LOADED
    }

    public class SweetnerUSDAHFCSSupplyUse //30
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string ReportIdnt { get; set; }
        public double HFCS42Prod { get; set; }
        public double HFCS55Prod { get; set; }
        public double TotalProd { get; set; }
        public double Imports { get; set; }
        public double Supply { get; set; }
        public double Exports { get; set; }
        public double HFCS42Util { get; set; }
        public double HFCS55Util { get; set; }
        public double TotalUtil { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sweetner_USDAHFCSSupplyUse SELECT '{ReportDate}', {Year},'{ReportIdnt}',{HFCS42Prod},{HFCS55Prod},{TotalProd},{Imports},{Supply},{Exports},{ HFCS42Util},{ HFCS55Util},{ TotalUtil},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class SweetnerUSDAHFCSImportsExports //(34a, 34b, 35a, 35b
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string ReportIdnt { get; set; }
        public string ReportType { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public double HFCS42 { get; set; }
        public double HFCS55 { get; set; }
        public double CrystallineFructose { get; set; }
        public double TotalFructose { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sweetner_USDAHFCSImportsExports SELECT '{ReportDate}', {Year}, '{ReportIdnt}','{ReportType}','{FromPlace}','{ToPlace}',{HFCS42},{HFCS55},{CrystallineFructose},{TotalFructose},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }
    }
    public class SweetnerUSDAGlucoseDextrose //(37,38)
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string SweetnerType { get; set; }
        public double Production { get; set; }
        public double Imports { get; set; }
        public double TotalSupply { get; set; }
        public double NetChangeinStocks { get; set; }
        public double TotalUse { get; set; }
        public double Exports { get; set; }
        public double ShipmentstoPuertoRico { get; set; }
        public double NonFoodUse { get; set; }
        public double FoodandBeverageUse { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sweetner_USDAGlucoseDextrose SELECT '{ReportDate}', {Year}, '{SweetnerType}',{Production},{Imports},{TotalSupply},{NetChangeinStocks},{TotalUse},{Exports},{ShipmentstoPuertoRico},{NonFoodUse},{FoodandBeverageUse},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        } //NOT LOADED
    }
    public class SugarUSDAMexicoSupplyUse //(54,55)
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public string ReportIdnt { get; set; }
        public string SugarType { get; set; }
        public double NominalPrice { get; set; }
        public double RealPrice { get; set; }
        public double USCentperPound { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDAMexicoSupplyUse SELECT '{ReportDate}', {Year}, '{ReportIdnt}','{SugarType}',{NominalPrice},{RealPrice},{USCentperPound},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }//NOT LOADED
    }
    public class SugarUSDAMexicoProduction //(56)
    {
        public string ReportDate { get; set; }
        public int Year { get; set; }
        public double BeginningStocks { get; set; }
        public double Production { get; set; }
        public double Imports { get; set; }
        public double Supply { get; set; }
        public double HumanConsumption { get; set; }
        public double OtherConsumption { get; set; }
        public double MiscAdjustment { get; set; }
        public double Total { get; set; }
        public double Exports { get; set; }
        public double TotalSse { get; set; }
        public double EndingStocks { get; set; }
        public String PopulateQuery(int JobID, int UserId, DateTime LastUpdated)
        {
            return $"INSERT INTO Sugar_USDAMexicoProduction SELECT '{ReportDate}', {Year}, {BeginningStocks},{Production},{Imports},{Supply},{HumanConsumption},{OtherConsumption},{MiscAdjustment},{Total},{Exports},{TotalSse},{EndingStocks},{JobID},{UserId},'{LastUpdated.ToShortDateString()}'";
        }//NOT LOADED
    }
    public class DataSource
    {
        public DataSource()
        {
            Headers = new List<Header>();
            Values = new List<List<string>>();
        }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public List<Header> Headers { get; set; }
        public List<List<string>> Values { get; set; }
    }
    public class Header
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
