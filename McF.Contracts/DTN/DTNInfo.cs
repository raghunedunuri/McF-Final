using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class DTNFieldUpdate
    {
        public string field { get; set; }
        public string value { get; set; }
    }

    public class DTNUpdateInfo
    {
        public string SymnolID { get; set; }
        public string UpdatedTime { get; set; }
        public string Symbol { get; set; }
        public List<DTNFieldUpdate> FieldInfo { get; set; }
    }
    public class DTNUpdate
    {
        public string UpdatedTime { get; set; }
        public string Symbol { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }

    }

    public class DTNFields
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RTDTopic { get; set; }
        public string FieldType { get; set; }
    }
    public class DTNSymbols
    {
        public string Symbol { get; set; }
        public string Unit { get; set; }
        public string DTNRoot { get; set; }
    }
    public class DTNGroup
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string DTNRoot { get; set; }
        public string RootGroup { get; set; }
        public string SubGroup { get; set; }
        public string MarketType { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Symbol} :-: {DTNRoot}";
        }

        public List<DTNSymbols> LstSymbolInfo;

    }

    public class DTNJobInfo
    {
        public string ProgID { get; set; }
        public string JobName { get; set; }
        public string MasterUnits { get; set; }
        public string FieldType { get; set; }
    }

    public class DTNData : DialyData
    {
        public string  RootSymbol { get; set; }
        public double  Last { get; set; }
        public double High          { get; set; }
        public double Open        { get; set; }
        public double Close           { get; set; }
        public double Low             { get; set; }
        public double Previous         { get; set; }
        public long Volume       { get; set; }
        public long  CumVolume       { get; set; }
        public double OpenInterest     { get; set; }
        public long  TradeCount         { get; set; }
        public double Change           { get; set; }
        public long  DaystoExpiration      { get; set; }
        public double Volatility      { get; set; }
        public double VWAP             { get; set; }
        public double BidAskSpreadAvg { get; set; }
        public double SettlementPrice    { get; set; }
        public DateTime? ExpirationDate   { get; set; }
        public double LTrade             { get; set; }
        public string IssueDescription { get; set; }
        public double CntrLow           { get; set; }
        public double CntrHigh           { get; set; }
    }
    public class DTNSymbolInfo
    {
        public List<String> Groups { get; set; }
        public List<String> SubGroups { get; set; }
        public List<String> MarketTypes { get; set; }
        public List<String> Symbols { get; set; }
    }

    public class DTNRawInfo
    {
        public string URL{ get; set; }
        public string UserName  { get; set; }
        public string Password { get; set; }
    }
}
