using McF.Contracts;
using McF.DataAccess;
using McF.DataAccess.Repositories.Implementors;
using McF.DataAccess.Repositories.Interfaces;
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
    internal static class DataTablesCommon
    {
        private static ICommonRepository commonRepo;
        private static DataSet BHConfigInfo;
        private static List<String> DataFields;
        private static List<String> DataAggreateFields;
        private static String SelectedFields;
        private static Dictionary<string, string> DataTables;
        static DataTablesCommon()
        {
            DataFields = new List<string>();
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();

          //  DataFields.Add("WeekEnding");
           // DataFields.Add("Region");
           // DataFields.Add("ReportDate");
            DataAggreateFields = new List<string>();
            DataAggreateFields.Add("Region");
            DataAggreateFields.Add("Group by Period");
        }

        
        public static string MergeTimeQuery(string query, string datequery, bool bRollUp)
        {
            string updatePrefix = " and ";

            if (!query.Contains("WHERE"))
                updatePrefix = " WHERE ";

            query += $" {updatePrefix} {datequery}";

            if (bRollUp)
            {
                string str = $"Select {String.Join(",", DataAggreateFields.ToArray())}, {SelectedFields}  from ( {query} ) A where ROWNUM = 1 Order by Group by Period";
                query = str;
            }

            if( !bRollUp)
                query += " Order by Report_Date";
            return query;
        }
       
        public static string GetDateQuery(string dateQuery, string tableName)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = dateQuery.Split(strArray, StringSplitOptions.None);
            return DataOperations.GetDateQuery(Convert.ToInt32(filters[0]), filters[1], filters[2], "Report_Date", DataTables[tableName]);
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, string Query, bool RollUp, bool YearFormat)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            DataSet ds = commonRepo.ProcessDataQuery(Query);

            if (ds != null)
            {
                int startrow = 3;
                int column = 1;

                foreach (DataColumn dcol in ds.Tables[0].Columns)
                {
                    string colName = dcol.ColumnName;
                    currentWorksheet.Cells[startrow, column] = colName;
                    column++;
                }
                startrow+= 1;

                Dictionary<string, Dictionary<string, string>> dccol = new Dictionary<string, Dictionary<string, string>>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    column = 1;
                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        if (dcol.ColumnName.Trim().ToUpper() == "WEEKENDING" || 
                            dcol.ColumnName.Trim().ToUpper() == "REPORT_DATE" ||
                            dcol.ColumnName.Trim().ToUpper() == "Group by Period" )
                        {
                            currentWorksheet.Cells[startrow, column++] = Convert.ToDateTime(dr[dcol.ColumnName]).ToShortDateString();
                        }
                        else
                        {
                            currentWorksheet.Cells[startrow, column++] = dr[dcol.ColumnName].ToString();
                        }
                    }
                    startrow++;
                }
            }
            else
                currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }

        public static string GetSelectedQuery(TreeView treeFields,bool bRollUp, string Frequency, string RollUpValue, string Tablename)
        {
            string Fields = String.Empty;
            string seleFields = String.Empty;
            //foreach( string str in DataFields)
            //{
            //    Fields += $"{str},";
            //}
            foreach (TreeNode node in treeFields.Nodes)
            {
                if (node.Checked)
                {
                   // Fields += $"{node.Text},";
                    seleFields += $"{node.Text},";

                }
            }
            if (!String.IsNullOrEmpty(Fields))
                Fields = Fields.Substring(0, Fields.Length - 1);
            SelectedFields = seleFields.TrimEnd(',');

            string Query = String.Empty;
            Query = $"Select {seleFields.TrimEnd(',')}  from  {DataTables[Tablename]}";
            return Query;
        }

        public static void SaveData(Excel.Worksheet currentWorksheet, List<Excel.Range> lstRanges)
        {
            foreach (Excel.Range excl in lstRanges)
            {
                Excel.Range xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 2], currentWorksheet.Cells[excl.Row, 2]];
                var Date = xlRange.Value;
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 1], currentWorksheet.Cells[excl.Row, 1]];
                var Symbol = xlRange.Value;
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 3], currentWorksheet.Cells[excl.Row, 3]];
                var State = xlRange.Value;
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 50], currentWorksheet.Cells[excl.Row, 50]];
                var Row = xlRange.Value;
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[Row, excl.Column], currentWorksheet.Cells[Row, excl.Column]];
                var Field = xlRange.Value;
                object val = excl.Value;
                //if (!CondMapping.ContainsKey(Field))
                //{
                //    string query = $"UPDATE CROPPROGRESS_DIALY_DATA SET  VALUE = {val} FROM CROPPROGRESS_DIALY_DATA CD INNER JOIN CROPPROGRESS_SYMBOL_INFO CS ON CD.MAPPINGID = CS.MAPPINGID WHERE CD.STATE = '{State}' AND CD.WEEKENDING = '{Date.ToString("yyyy-MM-dd")}' AND CS.SYMBOL = '{Symbol}' AND CS.FIELD ='{Field}'  AND CS.ISCondition = 0";
                //    cropRepository.UpdateCROPData(query);
                //}
                //else
                //{
                //    Field = CondMapping[Field];
                //    string query = $"UPDATE CROPPROGRESS_COND_DIALY_DATA SET  { Field } = { val } FROM CROPPROGRESS_COND_DIALY_DATA CD INNER JOIN CROPPROGRESS_CONDITIONS CC ON CD.MAPPINGID = CC.MAPPINGID INNER JOIN CROPPROGRESS_SYMBOL_INFO CS ON CS.MAPPINGID = CC.MAPPINGID AND CD.MAPPINGID = CS.MAPPINGID WHERE CD.STATE = '{State}' AND CD.WEEKENDING = '{Date.ToString("yyyy-MM-dd")}' AND CS.SYMBOL = '{Symbol}' AND CS.ISCondition = 1";
                //    cropRepository.UpdateCROPData(query);
                //}
            }
        }
        public static void GetData(string query, string datequery)
        {
            
        }
        public static void InitConfigData(TreeView treeFields, ComboBox cmbDatatables )
        {
            treeFields.Nodes.Clear();
            treeFields.CheckBoxes = true;

            DataTables = commonRepo.GetDataSources();
            int i = 0;
            foreach(KeyValuePair<string,string> kv in DataTables)
            {
                cmbDatatables.Items.Add(new ComboItem(kv.Key, i++));
            }
            cmbDatatables.SelectedIndex = 0;
        }

        public static void UpdateFields(TreeView treeFields, ComboBox cmbDatatables)
        {
            treeFields.Nodes.Clear();
            treeFields.CheckBoxes = true;

            List<string> lstFields = commonRepo.GetDataSourceFields(DataTables[cmbDatatables.SelectedItem.ToString()]);
            foreach (string str in lstFields)
            {
                treeFields.Nodes.Add(str);
            }
        }
    }
}
