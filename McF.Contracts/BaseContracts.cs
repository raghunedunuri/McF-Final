using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class DialyData
    {
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
    }

    public class FieldInfo
    {
        public string FieldName { get; set; }
        public long FieldDisplayName { get; set; }
        public string Units { get; set; }
    }

    public class SymbolInfo
    {
        public string Symbol { get; set; }
        public string MappingSymbol { get; set; }
    }
    public class UpdateInfo
    {
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
        public Dictionary<string, string> UpdateData { get; set; }
        public int UserID { get; set; }
    }

    public class UpdateSingleInfo
    {
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public int UserID { get; set; }
    }

    public class RawFilesInfo
    {
        public string URL { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int NoOfDays { get; set; }
    }
}
