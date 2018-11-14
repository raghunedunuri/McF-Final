using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Common
{
    public class MCFExcelColumn
    {
        public int Column { get; set; }
        public int Data { get; set; }
        public string Unit { get; set; }
        public bool ReadOnly { get; set; }
    }
    public class MCFExcelColumnHeader : MCFExcelColumn
    {
        public string DataType { get; set; }
        public string Table { get; set; }
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
    }
    public class MCFExcelRowData
    {
        public int Row { get; set; }
        public Dictionary<int, MCFExcelColumn> Columns { get; set; }
    }
    public class MCFExcelRowDataHeader
    {
        public int Row { get; set; }
        public Dictionary<int, MCFExcelColumnHeader> Columns { get; set; }
    }
    public class MCFExcelContract
    {
        public string Name { get; set; }
        public int HeaderRow { get; set; }
        public int UnitRow { get; set; }
        public MCFExcelRowDataHeader MCFHeader { get; set; }
        public Dictionary<int, MCFExcelRowData> MCFRows { get; set; }
    }
    public class MCFExcelData
    {
        public Dictionary<string, MCFExcelContract> MCFData { get; set; }
    }
    public class MCFSingleElementData
    {
        public int ElementDefRow { get; set; }
        public MCFExcelColumn MCFColumn { get; set; }
        public int HeaderRow { get; set; }
        public MCFExcelRowDataHeader MCFHeader { get; set; }
        public Dictionary<int, MCFExcelRowData> MCFRowData { get; set; }
    }
    public class MCFSingleExcelData
    {
        public Dictionary<string, MCFSingleElementData> MCFData { get; set; }
    }
}
