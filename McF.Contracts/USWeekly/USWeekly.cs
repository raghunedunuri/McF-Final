using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class USWeeklyData : DialyData
    {
        public long Weekly_Exports { get; set; }
        public long Accumulated_Exports { get; set; }
        public long Net_Sales { get; set; }
        public long Outstanding_Sales { get; set; }
        public long Nxt_mkt_year_Net_Sales { get; set; }
        public long Nxt_Mkt_year_Outstanding_Sales { get; set; }

        public string Category { get; set; }
    }
    public class USWeeklyUpdateData
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Field { get; set; }
        public long Value { get; set; }
    }

    public class USWeeklySymbolInfo : SymbolInfo
    {
        public string RawFileInfo { get; set; }
        public int NoOfRecords { get; set; }
    }

    public class USWeeklyCategoryInfo
    {
        public string Commodity_Name { get; set; }
        public List<USWeeklySymbolInfo> LstSymbolInfo { get; set; }

        public USWeeklyCategoryInfo(string commName)
        {
            Commodity_Name = commName;
            LstSymbolInfo = new List<USWeeklySymbolInfo>();
        }
    }
}
