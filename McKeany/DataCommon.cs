using McF.Contracts;
using McF.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using Excel = Microsoft.Office.Interop.Excel;

namespace McKeany
{
    public enum DATALOADTYPE
    {
        NONE = 0,
        SINGLE = 1,
        MULITPLE
    }
    public class DataCommon
    {
        private static Excel.DocEvents_ChangeEventHandler EventDel_CellsChange;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private static Dictionary<DataFeedType, Dictionary<string, int>> FrequencyMappings;
        private static Dictionary<string, int> PeriodSummary;

        private static ICommonRepository commonRepo;
        static DataCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
            FrequencyMappings = new Dictionary<DataFeedType, Dictionary<string, int>>();
            FrequencyMappings[DataFeedType.Dialy] = new Dictionary<string, int>();
            FrequencyMappings[DataFeedType.Weekly] = new Dictionary<string, int>();
            FrequencyMappings[DataFeedType.Monthly] = new Dictionary<string, int>();
            FrequencyMappings[DataFeedType.Quarterly] = new Dictionary<string, int>();
            FrequencyMappings[DataFeedType.Yearly] = new Dictionary<string, int>();

            FrequencyMappings[DataFeedType.Dialy].Add("None", 0);
            FrequencyMappings[DataFeedType.Dialy].Add("Weekly", 1);
            FrequencyMappings[DataFeedType.Dialy].Add("Monthly", 2);
            FrequencyMappings[DataFeedType.Dialy].Add("Quarterly", 3);
            FrequencyMappings[DataFeedType.Dialy].Add("Calendar Year", 4);
            FrequencyMappings[DataFeedType.Dialy].Add("Fiscal Year", 5);

            FrequencyMappings[DataFeedType.Weekly].Add("None", 0);
            FrequencyMappings[DataFeedType.Weekly].Add("Monthly", 1);
            FrequencyMappings[DataFeedType.Weekly].Add("Quarterly", 2);
            FrequencyMappings[DataFeedType.Weekly].Add("Calendar Year", 3);
            FrequencyMappings[DataFeedType.Weekly].Add("Fiscal Year", 4);

            FrequencyMappings[DataFeedType.Monthly].Add("None", 0);
            FrequencyMappings[DataFeedType.Monthly].Add("Quarterly", 1);
            FrequencyMappings[DataFeedType.Monthly].Add("Calendar Year", 2);
            FrequencyMappings[DataFeedType.Monthly].Add("Fiscal Year", 3);

            FrequencyMappings[DataFeedType.Quarterly].Add("None", 0);
            FrequencyMappings[DataFeedType.Quarterly].Add("Calendar Year", 1);
            FrequencyMappings[DataFeedType.Quarterly].Add("Fiscal Year", 2);

            FrequencyMappings[DataFeedType.Yearly].Add("None", 0);

            PeriodSummary = new Dictionary<string, int>();
            PeriodSummary.Add("None", 0);
            PeriodSummary.Add("Average", 1);
            PeriodSummary.Add("Low", 2);
            PeriodSummary.Add("High", 3);
            PeriodSummary.Add("First", 4);
            PeriodSummary.Add("Last", 5);
            PeriodSummary.Add("Count", 6);
            PeriodSummary.Add("Total", 7);
            EventDel_CellsChange = null;
        }
        public static void ExcecuteQuery (ExcelFieldData excelFieldData, string val)
        {
            foreach( string str in excelFieldData.UpdateQueries)
            {
                commonRepo.ProcessQuery(str.Replace("{REPLACEVALUE}", val));
            }
        }
        public static void CheckNodes( TreeView treeNode, bool bCheckState )
        {
           foreach( TreeNode rootNode in treeNode.Nodes)
            {
                rootNode.Checked = bCheckState;
                CheckAllChildNodes(rootNode, bCheckState);
                if (rootNode.Checked)
                    rootNode.Expand();
                else
                    rootNode.Collapse();
            }
        }

        public static void CheckNodes(TreeNode rootNode, bool bCheckState)
        {
            rootNode.Checked = bCheckState;
            CheckAllChildNodes(rootNode, bCheckState);
            if (rootNode.Checked)
                rootNode.Expand();
            else
                rootNode.Collapse();
        }
        public static void InitializeCommDataLoad(ExcelQuery excelQuery, UIData uiData)
        {
            bPresent = true;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            currentWorksheet.Shapes.SelectAll();
            Globals.ThisAddIn.Application.Selection.Delete();

            ThisAddIn.ClearData();
            Globals.ThisAddIn.Application.EnableEvents = false;

            DataCommon.FormatDataToExcel(currentWorksheet, uiData, excelQuery);

            currentWorksheet.Cells[1, 500] = JsonConvert.SerializeObject(uiData);

            DataCommon.EnableEvents(currentWorksheet);

            EventDel_CellsChange = new Excel.DocEvents_ChangeEventHandler(ThisAddIn.GlobalWorksheet_Change);
            currentWorksheet.Change += EventDel_CellsChange;
            bPresent = false;
        }

        public static void InitializeForDataLoad(ExcelQuery excelQuery, UIData uiData)
        {
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            currentWorksheet.Shapes.SelectAll();
            Globals.ThisAddIn.Application.Selection.Delete();
            
            ThisAddIn.ClearData();
            Globals.ThisAddIn.Application.EnableEvents = false;
            DataCommon.PresentData(currentWorksheet, uiData, excelQuery);
            currentWorksheet.Cells[1, 500] = JsonConvert.SerializeObject(uiData); 
            DataCommon.EnableEvents(currentWorksheet);
            if (EventDel_CellsChange == null)
            {
                EventDel_CellsChange = new Excel.DocEvents_ChangeEventHandler(ThisAddIn.GlobalWorksheet_Change);
            }
            currentWorksheet.Change += EventDel_CellsChange;
            bPresent = false;
        }

        public static void InitializeForDataLoad(UIData uiData, ComboBox cmbField, List<string> lstSymbols,
                                                     DataFeedType dataFeedType, string proc)
        {
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();
            Globals.ThisAddIn.Application.EnableEvents = false;

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            excelQuery.StoredProc = proc;
            excelQuery.Symbols = lstSymbols;
            excelQuery.FeedType = DataCommon.GetDataFeedType(cmbField, dataFeedType);
            DataCommon.PresentData(currentWorksheet, uiData,  excelQuery);

            currentWorksheet.Cells[1, 500] = JsonConvert.SerializeObject(uiData); ;

            DataCommon.EnableEvents(currentWorksheet);

            EventDel_CellsChange = new Excel.DocEvents_ChangeEventHandler(ThisAddIn.GlobalWorksheet_Change);
            currentWorksheet.Change += EventDel_CellsChange;
        }

        public static void EnableEvents( Excel.Worksheet currentWorksheet)
        {
            Excel.Range range = currentWorksheet.Range[currentWorksheet.Cells[1, 500], currentWorksheet.Cells[1, 500]];
            range.EntireColumn.Hidden = true;
            range = currentWorksheet.Range[currentWorksheet.Cells[1, 501], currentWorksheet.Cells[1, 501]];
            range.EntireColumn.Hidden = true;
            range = currentWorksheet.Range[currentWorksheet.Cells[1, 502], currentWorksheet.Cells[1, 502]];
            range.EntireColumn.Hidden = true;

            Globals.ThisAddIn.Application.EnableEvents = true;

            if (currentWorksheet.Shapes.Count <= 0)
            {
                Globals.ThisAddIn.AddControls();
            }
        }
        public static DataFeedType GetDataFeedType (ComboBox cmbField, DataFeedType currFeedType)
        {
            DataFeedType dataFeedType = currFeedType;
            if( cmbField != null)
            {
                switch(cmbField.SelectedItem.ToString().ToUpper())
                {
                    case "WEEKLY":
                        dataFeedType = DataFeedType.Weekly;
                        break;
                    case "MONTHLY":
                        dataFeedType = DataFeedType.Monthly;
                        break;
                    case "QUARTERLY":
                        dataFeedType = DataFeedType.Quarterly;
                        break;
                    case "CALENDAR YEAR":
                        dataFeedType = DataFeedType.Yearly;
                        break;
                    case "FISCAL YEAR":
                        dataFeedType = DataFeedType.Fiscal;
                        break;
                }
            }
            return dataFeedType;
        }

        public static void FrequencyChanged( ComboBox cmbFrequency, ComboBox cmbMonth , 
            CheckBox chkMatrixFormat, 
           DataFeedType dataFeedType = DataFeedType.Monthly)
        {
            if (cmbFrequency.SelectedItem.ToString() == "Fiscal Year")
                cmbMonth.Visible = true;
            else
                cmbMonth.Visible = false;

            if (cmbFrequency.SelectedItem.ToString() == "Fiscal Year" || 
                cmbFrequency.SelectedItem.ToString() == "Calendar Year" ||
                (cmbFrequency.SelectedIndex == 0 && dataFeedType == DataFeedType.Yearly) ||
                (cmbFrequency.SelectedIndex == 0 && dataFeedType == DataFeedType.Dialy))
            {
                chkMatrixFormat.Enabled = false;
            }
            else
            {
                chkMatrixFormat.Enabled = true;
            }
        }
        public static void InitializeDateFilters(DateTimePicker dtPickerStartTime, DateTimePicker dtPickerEndtime,
            ComboBox cmbRange, ComboBox cmbRollUP = null, ComboBox cmbField = null, 
            DataFeedType dataFeedType = DataFeedType.Monthly, ComboBox cmbMonth = null, CheckBox chkAutoUpdate = null)
        {
            dtPickerStartTime.Format = DateTimePickerFormat.Custom;
            dtPickerStartTime.CustomFormat = "MM/dd/yyyy";
            dtPickerStartTime.Text = DateTime.Now.AddYears(-1).ToShortDateString();
            dtPickerEndtime.Format = DateTimePickerFormat.Custom;
            dtPickerEndtime.CustomFormat = "MM/dd/yyyy";

            cmbRange.Items.Clear();
            cmbRange.Items.Add(new ComboItem("None", 0));
            cmbRange.Items.Add(new ComboItem("Last Updated", 1));
            cmbRange.Items.Add(new ComboItem("Last Week", 2));
            cmbRange.Items.Add(new ComboItem("Last 1 Month", 3));
            cmbRange.Items.Add(new ComboItem("Last 3 Months", 4));
            cmbRange.Items.Add(new ComboItem("Last 6 Months", 5));
            cmbRange.Items.Add(new ComboItem("Last 1 Year", 6));
            cmbRange.Items.Add(new ComboItem("Last 3 Years", 7));
            cmbRange.Items.Add(new ComboItem("Last 5 Years", 8));
            cmbRange.Items.Add(new ComboItem("All Time", 9));
            cmbRange.SelectedIndex = 0;

            if(chkAutoUpdate != null)
            {
                chkAutoUpdate.Checked = true;
            }

            if (cmbField != null)
            {
                cmbField.Items.Clear();
                cmbField.Enabled = true;
                switch (dataFeedType)
                {
                    case DataFeedType.Dialy:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Items.Add(new ComboItem("Weekly", 1));
                        cmbField.Items.Add(new ComboItem("Monthly", 2));
                        cmbField.Items.Add(new ComboItem("Quarterly", 3));
                        cmbField.Items.Add(new ComboItem("Calendar Year", 4));
                        cmbField.Items.Add(new ComboItem("Fiscal Year", 5));
                        break;
                    case DataFeedType.Weekly:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Items.Add(new ComboItem("Monthly", 1));
                        cmbField.Items.Add(new ComboItem("Quarterly", 2));
                        cmbField.Items.Add(new ComboItem("Calendar Year", 3));
                        cmbField.Items.Add(new ComboItem("Fiscal Year", 4));
                        break;
                    case DataFeedType.Monthly:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Items.Add(new ComboItem("Quarterly", 1));
                        cmbField.Items.Add(new ComboItem("Calendar Year", 2));
                        cmbField.Items.Add(new ComboItem("Fiscal Year", 3));
                        break;
                    case DataFeedType.Quarterly:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Items.Add(new ComboItem("Calendar Year", 1));
                        cmbField.Items.Add(new ComboItem("Fiscal Year", 2));
                        break;
                    case DataFeedType.Yearly:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Enabled = false;
                        break;
                    default:
                        cmbField.Items.Add(new ComboItem("None", 0));
                        cmbField.Enabled = false;
                        break;
                }
                cmbField.SelectedIndex = 0;
            }
            if (cmbRollUP != null)
            {
                cmbRollUP.Items.Clear();
                cmbRollUP.Items.Add(new ComboItem("None", 0));
                cmbRollUP.Items.Add(new ComboItem("Average", 1));
                cmbRollUP.Items.Add(new ComboItem("Low", 2));
                cmbRollUP.Items.Add(new ComboItem("High", 3));
                cmbRollUP.Items.Add(new ComboItem("First", 4));
                cmbRollUP.Items.Add(new ComboItem("Last", 5));
                cmbRollUP.Items.Add(new ComboItem("Count", 6));
                cmbRollUP.Items.Add(new ComboItem("Total", 7));
                cmbRollUP.SelectedIndex = 1;

                cmbRollUP.Enabled = cmbField.Enabled;
            }
            if (cmbMonth != null)
            {
                cmbMonth.Items.Clear();
                cmbMonth.Items.Add(new ComboItem("JAN", 0));
                cmbMonth.Items.Add(new ComboItem("FEB", 1));
                cmbMonth.Items.Add(new ComboItem("MAR", 2));
                cmbMonth.Items.Add(new ComboItem("APR", 3));
                cmbMonth.Items.Add(new ComboItem("MAY", 4));
                cmbMonth.Items.Add(new ComboItem("JUN", 5));
                cmbMonth.Items.Add(new ComboItem("JUL", 6));
                cmbMonth.Items.Add(new ComboItem("AUG", 7));
                cmbMonth.Items.Add(new ComboItem("SEP", 8));
                cmbMonth.Items.Add(new ComboItem("OCT", 9));
                cmbMonth.Items.Add(new ComboItem("NOV", 10));
                cmbMonth.Items.Add(new ComboItem("DEC", 11));
                cmbMonth.SelectedIndex = 9;
                cmbMonth.Visible = false;
            }
        }

        public static void GetUser(string user, string passWord)
        {
            DataSet userData = commonRepo.ProcessDataQuery($"Select * from users where UserName = '{user}' and Password = '{passWord}'");
            
            if( userData != null && userData.Tables[0].Rows.Count > 0)
            {
                DataTable dt = userData.Tables[0];
                ThisAddIn.UserInfo = new UserLoginData()
                {
                    UserId = Convert.ToInt32(dt.Rows[0]["ID"]),
                    UserName = user
                };
            }
        }

        public static void PopulateUser(string user, string passWord)
        {
            DataSet userData = commonRepo.ProcessDataQuery($"Select * from users where UserName = '{user}' and Password = '{passWord}'");

            if (userData != null && userData.Tables[0].Rows.Count > 0)
            {
                DataTable dt = userData.Tables[0];
                ThisAddIn.UserInfo = new UserLoginData()
                {
                    UserId = Convert.ToInt32(dt.Rows[0]["ID"]),
                    UserName = user
                };
            }
        }

        public static void RePopulateFilters(UIData uiData, DateTimePicker dtPickerStartTime, DateTimePicker dtPickerEndtime,
          ComboBox cmbRange, ComboBox cmbRollUP = null, ComboBox cmbField = null,
          DataFeedType dataFeedType = DataFeedType.Monthly, ComboBox cmbMonth = null, CheckBox chkMatrix = null, CheckBox ChkAutoUpdate = null)
        {
            dtPickerStartTime.Text = uiData.StartDate;
            dtPickerEndtime.Text = uiData.EndDate;
            cmbRange.SelectedIndex = uiData.DateRangeIndex;
            
            if (cmbField != null && cmbField.Enabled)
            {
                cmbField.SelectedIndex = FrequencyMappings[dataFeedType][uiData.RollUpValue];
            }
            if (cmbRollUP != null && cmbRollUP.Enabled )
            {
                cmbRollUP.SelectedIndex = PeriodSummary[uiData.RollUpFrequency];
            }
            if (cmbMonth != null && cmbMonth.Enabled)
            {
                cmbMonth.SelectedIndex = uiData.FiscalMonth;
                if(cmbRollUP.SelectedItem.ToString() != "Fiscal Year")
                    cmbMonth.Visible = false;
            }
            if (chkMatrix != null && chkMatrix.Enabled )
                chkMatrix.Checked = uiData.IsMatrix;

            if (ChkAutoUpdate != null)
                ChkAutoUpdate.Checked = uiData.IsAutoUpdate;
        }
        private static void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        public static void AddToColMap(int column, string key, Dictionary<string, int> ColMap)
        {
            ColMap.Add(key, column);
        }

        public static string PopulateExcelIndvCol(Dictionary<string, ExcelHeaderFieldData> DictSingleRow, DataTable FieldInfo)
        {
            string Table = String.Empty;
            foreach (DataRow dr in FieldInfo.Rows)
            {
                string Name = dr["ColumnName"].ToString();
                if (String.IsNullOrEmpty(Table))
                    Table = dr["TableName"].ToString();
                if (Name != "Field" && Name != "DataValue")
                {
                    string DataType = dr["Datatype"].ToString();
                    ExcelHeaderFieldData excelFieldData = null;
                    excelFieldData = new ExcelHeaderFieldData(Name, true, Name,DataType, Table);
                    DictSingleRow.Add(Name, excelFieldData);
                }
            }
            return Table;
        }

        public static Dictionary<string, ExcelDisplayData> PopulateExcelHeaderData(Dictionary<string, List<String>> Fields,
                                                                 Dictionary<string, ExcelHeaderFieldData> DictSingleRow,
                                                                 DataTable CommFieldInfo,
                                                                 string Table,
                                                                 Dictionary<string, int> ColMap, UIData uiData)
        {
            Dictionary<string, ExcelDisplayData> dictExcelDisplayData = new Dictionary<string, ExcelDisplayData>();
            foreach (KeyValuePair<string, List<string>> kv in Fields)
            {
                int column = 1;
                ExcelDisplayData excelDisplayData = new ExcelDisplayData();

                if (uiData.IsRollUp || uiData.IsMatrix)
                {
                    excelDisplayData.bReadOnly = true;
                    excelDisplayData.bInsertAllowed = false;
                }

                //ColMap.Add($"{kv.Key}Commodity_Name", column);
                //ExcelHeaderFieldData excelHeaderFieldData = new ExcelHeaderFieldData("Commodity_Name", true, "Commodity_Name","VARCHAR", Table);
                //excelDisplayData.PrimaryColumns.Add(column, excelHeaderFieldData);
                //excelDisplayData.HeaderData.Add(column++, excelHeaderFieldData);

                //ColMap.Add($"{kv.Key}ReportDate", column);
                //excelHeaderFieldData = new ExcelHeaderFieldData("ReportDate", false, "ReportDate", "Date", Table);
                //excelDisplayData.PrimaryColumns.Add(column, excelHeaderFieldData);
                //excelDisplayData.HeaderData.Add(column++, excelHeaderFieldData);

                foreach (KeyValuePair<string, ExcelHeaderFieldData> rv in DictSingleRow)
                {
                    ColMap.Add($"{kv.Key}{rv.Value.Header}", column);
                    excelDisplayData.PrimaryColumns.Add(column, rv.Value);
                    excelDisplayData.HeaderData.Add(column++, rv.Value);
                }
                foreach (string str in kv.Value)
                {
                    DataRow[] dr = CommFieldInfo.Select($"Commodity_Name='{kv.Key}' AND DisplayName = '{str}'");
                    if (dr != null && dr.Length > 0)
                    {
                        string FieldName = dr[0]["Field"].ToString();
                        string Unit = dr[0]["Unit"]?.ToString();
                        ColMap.Add($"{kv.Key}{FieldName}", column);
                        excelDisplayData.HeaderData.Add(column++, new ExcelHeaderFieldData(FieldName, str, "double", Table, Unit));
                    }
                }
                dictExcelDisplayData[textInfo.ToTitleCase(kv.Key.ToLower())] = excelDisplayData;
            }
            return dictExcelDisplayData;
        }

        public static int DisplayHeaderData(Excel.Worksheet currentWorksheet, ExcelDisplayData excelDisplayData, int currentRow)
        {
            foreach (KeyValuePair<int, ExcelHeaderFieldData> kv in excelDisplayData.HeaderData)
            {
                excelDisplayData.UnitRow = currentRow;
                excelDisplayData.HeaderRow = currentRow + 1;
                currentWorksheet.Cells[excelDisplayData.UnitRow, kv.Key] = kv.Value.Unit;
                currentWorksheet.Cells[excelDisplayData.HeaderRow, kv.Key] = kv.Value.DisplayName;
            }
            return currentRow + 2;
        }

        public static Dictionary<string, ExcelDisplayData> FormatDataToExcel(
                                                        Excel.Worksheet currentWorksheet,
                                                        UIData uiData,
                                                        ExcelQuery excelQuery  )
        {
            DataSet ds = commonRepo.GetExcelFormatedData(excelQuery);
            Dictionary<string, ExcelDisplayData> dictExcelDisplayData = null;
            if (ds != null && ds.Tables.Count == 3)
            {
                DataFeedType dataFeedType = excelQuery.FeedType;
                Dictionary<string, List<String>> Fields = excelQuery.CommFields;

                DataTable FieldInfo = ds.Tables[0];
                DataTable CommFieldInfo = ds.Tables[1];
                DataTable Data = ds.Tables[2];

                Dictionary<string, ExcelHeaderFieldData> DictSingleRow = new Dictionary<string, ExcelHeaderFieldData>();
                string Table = PopulateExcelIndvCol(DictSingleRow, FieldInfo);

                Dictionary<string, int> ColMap = new Dictionary<string, int>();
                Dictionary<string, int> RowMap = new Dictionary<string, int>();

                dictExcelDisplayData = PopulateExcelHeaderData(Fields, DictSingleRow, CommFieldInfo, Table, ColMap, uiData);

                if (!uiData.IsMatrix || dataFeedType == DataFeedType.Yearly || dataFeedType == DataFeedType.Fiscal)
                {
                    int row = 3;
                    string PrevCommodity = String.Empty;

                    foreach (DataRow dr in Data.Rows)
                    {
                        string Commodity = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                        string Field = dr["Field"].ToString();
                        string DataValue = dr["DataValue"].ToString();
                  
                        ExcelDisplayData excelDisplayData = dictExcelDisplayData[Commodity];

                        if (PrevCommodity != Commodity)
                        {
                            if (!String.IsNullOrEmpty(PrevCommodity))
                                row += 3;
                            PrevCommodity = Commodity;
                            row = DisplayHeaderData(currentWorksheet, excelDisplayData, row);
                        }

                        string InsertQuery = String.Empty;
                        string UpdateQuery = $"Where ";

                        string Key = String.Empty;
                        foreach (KeyValuePair<string, ExcelHeaderFieldData> kv in DictSingleRow)
                        {
                            string hValue = dr[kv.Key].ToString();
                            switch (kv.Value.DataType.ToUpper())
                            {
                                case "DATE":
                                case "DATETIME":
                                    hValue = Convert.ToDateTime(hValue).ToShortDateString();
                                    UpdateQuery += $" {kv.Key} = '{hValue}' AND";
                                    InsertQuery += $" {kv.Key}= {hValue};";
                                    break;
                                case "VARCHAR":
                                    UpdateQuery += $" {kv.Key} = '{hValue}' AND";
                                    InsertQuery += $" {kv.Key}= {hValue};";
                                    break;
                                default:
                                    UpdateQuery += $" {kv.Key} = '{hValue}' AND";
                                    InsertQuery += $" {kv.Key}= {hValue};";
                                    break;
                            }
                            Key += dr[kv.Key].ToString();
                        }

                        if (!RowMap.ContainsKey(Key))
                        {
                            foreach (KeyValuePair<int, ExcelHeaderFieldData> heaexcel in excelDisplayData.PrimaryColumns)
                            {
                               // string headerval = textInfo.ToTitleCase(dr[heaexcel.Value.Header].ToString().ToLower());
                                int col = ColMap[$"{Commodity}{heaexcel.Value.Header}"];
                                string val = (heaexcel.Value.DataType.ToUpper() == "DATE" || heaexcel.Value.DataType.ToUpper() == "DATETIME") ? Convert.ToDateTime(dr[heaexcel.Value.Header]).ToShortDateString() : dr[heaexcel.Value.Header].ToString();
                                val = textInfo.ToTitleCase(val.ToLower());
                                if (!excelDisplayData.DataFields.ContainsKey(row))
                                    excelDisplayData.DataFields[row] = new Dictionary<int, ExcelFieldData>();

                                ExcelFieldData excelFieldData = new ExcelFieldData()
                                {
                                    DataType = heaexcel.Value.DataType,
                                    ReadOnly = true,
                                    DataValue = val
                                };
                                excelDisplayData.DataFields[row].Add(col, excelFieldData);
                                Globals.ThisAddIn.PopulateUpdatedData(row, col, excelFieldData);
                                currentWorksheet.Cells[row, col] = val;
                            }
                            RowMap[Key] = row++;
                        }
                        int currentRow = RowMap[Key];
                        string colKey = $"{Commodity}{Field}";
                        if (ColMap.ContainsKey(colKey))
                        {
                            int datacol = ColMap[$"{Commodity}{Field}"];

                            ExcelHeaderFieldData excelHD = excelDisplayData.HeaderData[datacol];
                            DataValue = (excelHD.DataType.ToUpper() == "DATE" || excelHD.DataType.ToUpper() == "DATETIME") ? Convert.ToDateTime(DataValue).ToShortDateString() : DataValue;
                            currentWorksheet.Cells[currentRow, datacol] = DataValue;
                            ExcelFieldData exFDData = new ExcelFieldData()
                            {
                                DataType = excelHD.DataType,
                                ReadOnly = false,
                                DataValue = DataValue
                            };
                            InsertQuery = $"INSERT INTO AuditData SELECT '{Table}','{Field}','{DataValue}','{InsertQuery}', 0, '{DateTime.Now.ToShortDateString()}'"; 
                            UpdateQuery = $"UPDATE {Table} SET DataValue = {GetFormatMethod(excelHD.DataType)} {UpdateQuery} Field = '{Field}'";
                            exFDData.UpdateQueries.Add(InsertQuery);
                            exFDData.UpdateQueries.Add(UpdateQuery);
                            excelDisplayData.DataFields[currentRow].Add(datacol, exFDData);
                            Globals.ThisAddIn.PopulateUpdatedData(currentRow, datacol, exFDData);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, ExcelDisplayData> kvDisplay in dictExcelDisplayData)
                    {
                        Dictionary<string, ExcelHeaderFieldData> PrimaryKeys = new Dictionary<string, ExcelHeaderFieldData>();

                        string MatrixDataField = String.Empty;
                        foreach (KeyValuePair<int, ExcelHeaderFieldData> kvHeader in kvDisplay.Value.PrimaryColumns)
                        {
                            if (kvHeader.Value.DataType.ToUpper() == "DATE" || kvHeader.Value.DataType.ToUpper() == "DATETIME")
                                MatrixDataField = kvHeader.Value.Header;
                            else
                            {
                                PrimaryKeys.Add(kvHeader.Value.Header, kvHeader.Value);
                            }
                        }
                        Dictionary<string, ExcelMatrixDisplayData> MatrixData = FormatMatrixData(PrimaryKeys, null, MatrixDataField, Data, Fields, CommFieldInfo);
                        DisplayMatrixFormatData(currentWorksheet, MatrixData, dataFeedType);
                    }
                }
            }
            return dictExcelDisplayData;
        }

        public static Dictionary<string, ExcelMatrixDisplayData> FormatMatrixData(Dictionary<string, ExcelHeaderFieldData> PrimaryKeys,
                                                    Dictionary<string, ExcelHeaderFieldData> DataHeader,
                                                    string MatrixDataField, DataTable Data, bool bKeyAsVal = false)
        {
            Dictionary<string, ExcelMatrixDisplayData> MatrixData = new Dictionary<string, ExcelMatrixDisplayData>();

            foreach (DataRow dr in Data.Rows)
            {
                string Key = String.Empty;

                foreach (KeyValuePair<string, ExcelHeaderFieldData> kv in PrimaryKeys)
                {
                    Key += $"{kv.Key}={dr[kv.Key].ToString()};";
                }
                if (!bKeyAsVal)
                {
                    foreach (KeyValuePair<string, ExcelHeaderFieldData> kv in DataHeader)
                    {
                        string Unique = Key + $"[{kv.Key}]";
                        if (!MatrixData.ContainsKey(Unique))
                        {
                            MatrixData.Add(Unique, new ExcelMatrixDisplayData(String.Empty, kv.Value.DisplayName, kv.Value.Unit));
                        }
                        DateTime dt = Convert.ToDateTime(dr[MatrixDataField]);
                        ExcelMatrixDisplayData excelMatrixDisplayData = MatrixData[Unique];
                        excelMatrixDisplayData.Data.Add(dr[MatrixDataField]?.ToString(), dr[kv.Key]?.ToString());
                        if (!excelMatrixDisplayData.Years.ContainsKey(dt.Year))
                            excelMatrixDisplayData.Years.Add(dt.Year, 0);
                    }
                }
                else
                {
                    string Unique = Key + $"[{dr["Field"].ToString()}]";
                    if (!MatrixData.ContainsKey(Unique))
                    {
                        MatrixData.Add(Unique, new ExcelMatrixDisplayData(String.Empty, dr["Field"].ToString(), String.Empty));
                    }
                    DateTime dt = Convert.ToDateTime(dr[MatrixDataField]);
                    ExcelMatrixDisplayData excelMatrixDisplayData = MatrixData[Unique];
                    excelMatrixDisplayData.Data.Add(dr[MatrixDataField]?.ToString(), dr["DataValue"]?.ToString());
                    if (!excelMatrixDisplayData.Years.ContainsKey(dt.Year))
                        excelMatrixDisplayData.Years.Add(dt.Year, 0);
                }
            }
            return MatrixData;
        }
        public static Dictionary<string, ExcelMatrixDisplayData> FormatMatrixData(Dictionary<string, ExcelHeaderFieldData> PrimaryKeys,
                                                   Dictionary<string, ExcelHeaderFieldData> DataHeader,
                                                   string MatrixDataField, DataTable Data, Dictionary<string, List<string>> Fields, DataTable commFields)
        {
            Dictionary<string, ExcelMatrixDisplayData> MatrixData = new Dictionary<string, ExcelMatrixDisplayData>();

            foreach (DataRow dr in Data.Rows)
            {
                string Key = String.Empty;

                string commodity = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                if (Fields.ContainsKey(commodity))
                {
                    List<string> sel = Fields[commodity];
                    string fieName = dr["Field"].ToString().ToLower();
                    DataRow [] FieldRow = commFields.Select($"Commodity_Name='{commodity}' AND Field = '{fieName}'");

                    if( FieldRow != null && FieldRow.Length > 0)
                    {
                        fieName = textInfo.ToTitleCase(FieldRow[0]["DisplayName"].ToString().ToLower());
                    }
                //    string displayName = DataHeader
                    if (sel.Contains(fieName))
                    {
                        foreach (KeyValuePair<string, ExcelHeaderFieldData> kv in PrimaryKeys)
                        {
                            Key += $"{kv.Key}={dr[kv.Key].ToString()};";
                        }

                        string Unique = Key + $"[{fieName}]";
                        if (!MatrixData.ContainsKey(Unique))
                        {
                            MatrixData.Add(Unique, new ExcelMatrixDisplayData(String.Empty, fieName, String.Empty));
                        }
                        DateTime dt = Convert.ToDateTime(dr[MatrixDataField]);
                        ExcelMatrixDisplayData excelMatrixDisplayData = MatrixData[Unique];
                        excelMatrixDisplayData.Data.Add(dr[MatrixDataField]?.ToString(), dr["DataValue"]?.ToString());
                        if (!excelMatrixDisplayData.Years.ContainsKey(dt.Year))
                            excelMatrixDisplayData.Years.Add(dt.Year, 0);
                    }
                }
            }
            return MatrixData;
        }
        public static void DisplayMatrixFormatData(Excel.Worksheet currentWorksheet,
                                                  Dictionary<string, ExcelMatrixDisplayData> MatrixData, DataFeedType dataFeedType)
        {
            int row = 3;
            int column = 1;
            for (int i = 0; i < MatrixData.Count; i++)
            {
                string dataKey = MatrixData.ElementAt(i).Key;
                ExcelMatrixDisplayData dataValue = MatrixData.ElementAt(i).Value;

                string[] displayFields = dataKey.Split('[')[0].Split(';');
                foreach (string str in displayFields)
                {
                    if (!String.IsNullOrEmpty(str))
                    {
                        currentWorksheet.Cells[row++, column] = str.Split('=')[1];
                    }
                }
                currentWorksheet.Cells[row++, column] = dataValue.FieldName;
                currentWorksheet.Cells[row++, column] = dataValue.Unit;
                for (int j = 0; j < dataValue.Years.Count; j++)
                {
                    int year = dataValue.Years.ElementAt(j).Key;
                    dataValue.Years[year] = j + 2;
                    currentWorksheet.Cells[row, j + 2] = year;
                }
                row++;
                column = 1;
                Dictionary<string, int> DictRow = new Dictionary<string, int>();
                switch (dataFeedType)
                {
                    case DataFeedType.Weekly:
                        for (int x = 1; x <= 53; x++)
                        {
                            string Week = $"Week-{x}";
                            DictRow.Add(Week, row);
                            currentWorksheet.Cells[row++, 1] = Week;
                        }
                        break;
                    case DataFeedType.Monthly:
                        AddMonths(currentWorksheet, "Jan", row++, DictRow);
                        AddMonths(currentWorksheet, "Feb", row++, DictRow);
                        AddMonths(currentWorksheet, "Mar", row++, DictRow);
                        AddMonths(currentWorksheet, "Apr", row++, DictRow);
                        AddMonths(currentWorksheet, "May", row++, DictRow);
                        AddMonths(currentWorksheet, "Jun", row++, DictRow);
                        AddMonths(currentWorksheet, "Jul", row++, DictRow);
                        AddMonths(currentWorksheet, "Aug", row++, DictRow);
                        AddMonths(currentWorksheet, "Sep", row++, DictRow);
                        AddMonths(currentWorksheet, "Oct", row++, DictRow);
                        AddMonths(currentWorksheet, "Nov", row++, DictRow);
                        AddMonths(currentWorksheet, "Dec", row++, DictRow);
                        break;
                    case DataFeedType.Quarterly:
                        for (int x = 1; x <= 4; x++)
                        {
                            string Week = $"QTR-{x}";
                            DictRow.Add(Week, row);
                            currentWorksheet.Cells[row++, 1] = Week;
                        }
                        break;
                }

                foreach (KeyValuePair<string, string> kv in dataValue.Data)
                {
                    try
                    {
                        DateTime date = Convert.ToDateTime(kv.Key);
                        int DataRow = 0; int DataCol = 0;
                        switch (dataFeedType)
                        {
                            case DataFeedType.Weekly:
                                DataCol = dataValue.Years[date.Year];
                                DataRow = DictRow[$"Week-{(date.DayOfYear / 7) + 1}"];
                                break;
                            case DataFeedType.Monthly:
                                DataCol = dataValue.Years[date.Year];
                                DataRow = DictRow[GetMonth(date.Month)];
                                break;
                            case DataFeedType.Quarterly:
                                DataCol = dataValue.Years[date.Year];
                                DataRow = DictRow[$"QTR-{((date.Month - 1) / 3) + 1}"];
                                break;
                        }
                        currentWorksheet.Cells[DataRow, DataCol] = kv.Value;
                    }
                    catch(Exception ex)
                    {

                    }
                }
                row++;
            }
        }

        public static ExcelDisplayData mExcelDisplayData = null;
    
        public static bool bMatrix = false;
        public static bool bPresent = false;
        public static void PresentData( Excel.Worksheet currentWorksheet,
                                        UIData uiData,
                                        ExcelQuery excelQuery)
        {
            DataFeedType dataFeedType = excelQuery.FeedType;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            Dictionary<string, ExcelDisplayData> dictExcelDisplayData = new Dictionary<string, ExcelDisplayData>();
            ExcelDisplayData excelDisplayData = new ExcelDisplayData();
            if (uiData.IsRollUp || uiData.IsMatrix)
            {
                excelDisplayData.bReadOnly = true;
                excelDisplayData.bInsertAllowed = false;
            }
            excelQuery.FiscalMonth = uiData.FiscalMonth;
            DataSet ds = commonRepo.GetExcelFormatedData(excelQuery);

            if (ds != null && ds.Tables.Count == 2)
            {
                List<string> Fields = excelQuery.Fields;
                  
                DataTable FieldInfo = ds.Tables[0];
                DataTable BHData = ds.Tables[1];

                if (!uiData.IsMatrix || dataFeedType == DataFeedType.Yearly || dataFeedType == DataFeedType.Fiscal)
                {
                    int column = 1;
                    foreach (DataRow dr in FieldInfo.Rows)
                    {
                        int PrimaryKey = Convert.ToInt16(dr["PrimaryKey"]);
                        string Name = dr["ColumnName"].ToString();
                        string DataType = dr["Datatype"].ToString();
                        string Table = dr["TableName"].ToString();
                        string DisplayName = dr["DisplayName"] == DBNull.Value ? Name : dr["DisplayName"].ToString();

                        if (PrimaryKey == 1)
                        {
                            ExcelHeaderFieldData excelFieldData = new ExcelHeaderFieldData(Name, true, DisplayName, DataType, Table);
                            excelDisplayData.PrimaryColumns.Add(column, excelFieldData);
                            excelDisplayData.HeaderData.Add(column, excelFieldData);
                            column++;
                        }
                        else
                        {
                            if (Fields == null || Fields.Contains(DisplayName))
                            {
                                string Unit = dr["Unit"] == DBNull.Value ? String.Empty : dr["Unit"].ToString();
                                excelDisplayData.HeaderData.Add(column++, new ExcelHeaderFieldData(Name, DisplayName, DataType, Table, Unit));
                            }
                        }
                    }

                    int row = 3;
                    foreach (KeyValuePair<int, ExcelHeaderFieldData> kv in excelDisplayData.HeaderData)
                    {
                        excelDisplayData.UnitRow = row;
                        excelDisplayData.HeaderRow = row + 1;
                        currentWorksheet.Cells[excelDisplayData.UnitRow, kv.Key] = kv.Value.Unit;
                        currentWorksheet.Cells[excelDisplayData.HeaderRow, kv.Key] = kv.Value.DisplayName;
                    }
                    row += 2;

                    foreach (DataRow dr in BHData.Rows)
                    {
                        column = 1;
                        foreach (KeyValuePair<int, ExcelHeaderFieldData> kv in excelDisplayData.HeaderData)
                        {
                            string value = dr[kv.Value.Header]?.ToString();
                            ExcelFieldData excelFieldData = new ExcelFieldData();

                            if (kv.Value.IsPrimaryKey)
                            {
                                excelFieldData.ReadOnly = true;
                            }
                            else
                            {
                                excelFieldData.ReadOnly = false;
                                string updateQuery = GetUpdateQuery(dr, excelDisplayData.PrimaryColumns);
                                string insertQuery = GetInsertQuery(dr, excelDisplayData.PrimaryColumns);
                                excelFieldData.UpdateQueries.Add($"Insert into AuditData SELECT '{kv.Value.Table}', '{kv.Value.Header}', {GetFormatValue(kv.Value.DataType, value)}, '{insertQuery}', 0, '{DateTime.Now.ToShortDateString()}'");
                                excelFieldData.UpdateQueries.Add($"UPDATE {kv.Value.Table} SET [{kv.Value.Header}] = {GetFormatMethod(kv.Value.DataType)} {updateQuery}");
                            }
                            if (!String.IsNullOrEmpty(value))
                            {
                                switch (kv.Value.DataType.ToUpper())
                                {
                                    case "DATE":
                                    case "DATETIME":
                                        excelFieldData.DataValue = Convert.ToDateTime(value).ToShortDateString();
                                        break;
                                    default:
                                        excelFieldData.DataValue = value;
                                        break;
                                }
                            }
                            else
                                excelFieldData.DataValue = String.Empty;

                            if (!excelDisplayData.DataFields.ContainsKey(row))
                                excelDisplayData.DataFields[row] = new Dictionary<int, ExcelFieldData>();

                            excelDisplayData.DataFields[row].Add(kv.Key, excelFieldData);
                            currentWorksheet.Cells[row, kv.Key] = excelFieldData.DataValue;
                            Globals.ThisAddIn.PopulateUpdatedData(row, kv.Key, excelFieldData);
                        }
                        row++;
                    }
                    dictExcelDisplayData["ONE"] = excelDisplayData;
                  
                }
                else
                {
                    bMatrix = true;
                    Dictionary<string, ExcelHeaderFieldData> PrimaryKeys = new Dictionary<string, ExcelHeaderFieldData>();
                    Dictionary<string, ExcelHeaderFieldData> DataHeader = new Dictionary<string, ExcelHeaderFieldData>();

                    string MatrixDataField = String.Empty;
                    foreach (DataRow dr in FieldInfo.Rows)
                    {
                        int PrimaryKey = Convert.ToInt16(dr["PrimaryKey"]);
                        string Name = dr["ColumnName"].ToString();
                        string DataType = dr["Datatype"].ToString();
                        string Table = dr["TableName"].ToString();
                        string DisplayName = dr["DisplayName"] == DBNull.Value ? Name : dr["DisplayName"].ToString();

                        if (PrimaryKey == 1)
                        {
                            ExcelHeaderFieldData excelFieldData = new ExcelHeaderFieldData(Name, true, DisplayName, DataType, Table);
                            if (DataType.ToUpper() == "DATE" || DataType.ToUpper() == "DATETIME")
                                MatrixDataField = Name;
                            else
                                PrimaryKeys.Add(Name, excelFieldData);
                        }
                        else
                        {
                            if (Fields == null || Fields.Contains(DisplayName))
                            {
                                string Unit = dr["Unit"] == DBNull.Value ? String.Empty : dr["Unit"].ToString();
                                DataHeader.Add(Name, new ExcelHeaderFieldData(Name, DisplayName, DataType, Table, Unit));
                            }
                        }
                    }
                    Dictionary<string, ExcelMatrixDisplayData> MatrixData = FormatMatrixData(PrimaryKeys, DataHeader, MatrixDataField, BHData);
                    DisplayMatrixFormatData(currentWorksheet, MatrixData, dataFeedType);
                }
            }
            else
                currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }
        public static string GetMonth(int Month)
        {
            switch (Month)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
            }
            return String.Empty;
        }
        public static void AddMonths(Excel.Worksheet currentWorksheet, string Month, int row, Dictionary<string, int> DictRow)
        {
            Excel.Range slRange = currentWorksheet.Range[currentWorksheet.Cells[row, 1], currentWorksheet.Cells[row, 1]];
            slRange.Locked = true;
            currentWorksheet.Cells[row, 1] = Month;
            DictRow[Month] = row;
        }
        public static string GetUpdateQuery(DataRow dr, Dictionary<int, ExcelHeaderFieldData> dictFieldInfo)
        {
            string query = $" WHERE ";

            foreach (KeyValuePair<int, ExcelHeaderFieldData> kv in dictFieldInfo)
            {
                string val = dr[kv.Value.Header].ToString();
                query += $" {kv.Value.Header} = {GetFormatValue(kv.Value.DataType, val)} AND";
            }
            return query.Substring(0, query.Length - 3);
        }
        public static string GetInsertQuery(DataRow dr, Dictionary<int, ExcelHeaderFieldData> dictFieldInfo)
        {
            string query = String.Empty;

            foreach (KeyValuePair<int, ExcelHeaderFieldData> kv in dictFieldInfo)
            {
                string val = dr[kv.Value.Header].ToString();
                query += $" {kv.Value.Header} = {val};";
            }
            return query.Substring(0, query.Length - 1);
        }
        public static string GetFormatMethod(string DataType)
        {
            switch (DataType.ToUpper())
            {
                case "DATE":
                case "DATETIME":
                case "VARCHAR":
                    return "'{REPLACEVALUE}'";
                default:
                    return "{REPLACEVALUE}";
            }
        }
        public static string GetFormatValue(string DataType, string Value)
        {
            switch (DataType.ToUpper())
            {
                case "DATE":
                case "VARCHAR":
                case "DATETIME":
                    return $"'{Value}'";
                default:
                    return $"{Value}";
            }
        }

        public static void BulkInsertTable(string table, string dSource, DataTable dTable, Dictionary<int, DataCol> dc)
        {
            string createQuery = SqlTableCreator.GetCreateFromDataTableSQL(table, dTable);
            string query = "INSERT INTO FREEFLOW_CONFIG SELECT ";
            foreach (KeyValuePair<int, DataCol> kv in dc)
            {
                string dataQuery = query + $"'{table}', '{kv.Value.Column}','{kv.Value.Column}','{kv.Value.Unit}'";
                commonRepo.ProcessQuery(dataQuery);
            }
            commonRepo.ProcessQuery($"INSERT INTO FREEFlowTables SELECT '{dSource}', '{table}', 0, '{DateTime.Now.ToShortDateString()}'");
            commonRepo.ProcessQuery(createQuery);
            commonRepo.BulkInsertDataTable(table, dTable);
        }
    }
}
