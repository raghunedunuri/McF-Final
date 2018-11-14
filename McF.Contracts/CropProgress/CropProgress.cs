using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class CROPData 
    {
        public CROPData()
        {
            Values = new Dictionary<string, string>();
            Conditions = new Dictionary<string, string>();
        }
        public int MappingID { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public Dictionary<string, string> Conditions { get; set; }
        public string State { get; set; }
        public string Symbol { get; set; }
        public string WeekEnding { get; set; }
    }

    public class CROPSymbolFieldInfo
    {
        public CROPSymbolFieldInfo()
        {
            Fields = new List<string>();
            Conditions = new List<string>();
        }
        public string Symbol { get; set; }
        public List<string> Fields { get; set; }
        public List<string> Conditions { get; set; }
    }
    public class CROPConditions 
    {
        public CROPConditions()
        {
            Symbols = new List<string>();
        }
        public string Condition { get; set; }
        public List<string> Symbols { get; set; }
    }

    public class CROPSelectionData
    {
        public CROPSelectionData()
        {
            dctCROPCondInfo = new Dictionary<string, CROPConditions>();
            dctCROPFieldInfo = new Dictionary<string, CROPSymbolFieldInfo>();
            lstSymbols = new List<string>();
        }
        public Dictionary<string, CROPConditions> dctCROPCondInfo { get; set; }
        public Dictionary<string, CROPSymbolFieldInfo> dctCROPFieldInfo { get; set; }
        public List<string> lstSymbols { get; set; }
        public string RollUpFrequency { get; set; }
        public string RollUpData { get; set; }
        public bool IsRollUp { get; set; }
        public bool IsMatrixMethod { get; set; }
    }
    public class CROPRawData
    {
        public CROPRawData(int _code)
        {
            Code = _code;
            DataValues = new Dictionary<string, Dictionary<int, Dictionary<int, string>>>();
        }
        public int Code;
        public Dictionary<string, Dictionary<int, Dictionary<int, string>>> DataValues;
    }

    public class CROPDialyData 
    {
        public CROPDialyData(int mappingID, string commodity, DateTime? weekEnding, string state, Dictionary<string, string> cond, string reportDate)
        {
            State = state;
            ConditionValues = cond;
            MappingID = mappingID;
            WeekEnding = weekEnding;
            isCondition = true;
            Commodity = commodity;
            ReportDate = reportDate;
        }
        public CROPDialyData(int mappingID, string commodity, DateTime? weekEnding, string field, string state, string value, string reportDate )
        {
            State = state;
            Value = value;
            MappingID = mappingID;
            WeekEnding = weekEnding;
            isCondition = false;
            Commodity = commodity;
            ReportDate = reportDate;
            MappingValue = field;
        }
        public string Commodity;
        public bool isCondition;
        public int MappingID;
        public string MappingValue;
        public DateTime? WeekEnding;
        public string ReportDate;
        public string State;
        public string Value;
        public Dictionary<string, string> ConditionValues;
    }

    public class CROPMappingInfo
    {
        public CROPMappingInfo(string symbol, string field, int mappingId, DateTime? date, bool bIsCond = false)
        {
            Symbol = symbol.Trim();
            Field = field.Trim();
            MappingID = mappingId;
            IsCondition = bIsCond;
            WeekEnding = date;
            Conditions = new Dictionary<string, string>();
            LstCropData = new List<CROPDialyData>();
            bExists = false;
        }
        public bool bExists { get; set; }
        public string Symbol { get; set; }
        public string Field { get; set; }
        public int MappingID { get; set; }
        public bool IsCondition { get; set; }
        public DateTime? WeekEnding { get; set; }
        public Dictionary<string, string> Conditions { get; set; }
        public List<CROPDialyData> LstCropData { get; set; }
    }
}
