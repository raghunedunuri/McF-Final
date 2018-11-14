﻿using McF.Contracts;
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
    internal static class CECommon
    {
        private static ICommonRepository commonRepo;
        private static DataSet BHConfigInfo;
        private static List<String> DataFields;
        static CECommon()
        {
            DataFields = new List<string>();
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
            DataFields.Add("Report_Date");
            DataFields.Add("BOM_Month");
            DataFields.Add("DTM_Month");
        }
        public static string MergeTimeQuery(string query, string datequery)
        {
            string updatePrefix = " and ";

            if (!query.Contains("WHERE"))
                updatePrefix = " WHERE ";

            query += $" {updatePrefix} {datequery}";

            //if(true)
            //{
            //    query += $"ROWNUM = 1";
            //}

            query = $"Select * from ( {query} ) A WHERE ROWNUM = 1 Order by  Report_Date";
            return query;
        }
       
        public static string GetDateQuery(string dateQuery)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = dateQuery.Split(strArray, StringSplitOptions.None);
            return DataOperations.GetDateQuery(Convert.ToInt32(filters[0]), filters[1], filters[2], "Report_Date", "CHICKENEGGS_DIALY_DATA");
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, string Query)
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
                    if (dcol.ColumnName.Trim().ToUpper() == "ROWNUM")
                        continue;

                    string colName = dcol.ColumnName;
                    currentWorksheet.Cells[startrow+1, column] = colName;
                    DataRow[] dr = BHConfigInfo.Tables[0].Select($"Name = '{colName}'");
                    if (dr != null && dr.Length > 0)
                        currentWorksheet.Cells[startrow, column] = dr[0]["Unit"]?.ToString();
                    column++;
                }
                startrow+= 2;

                Dictionary<string, Dictionary<string, string>> dccol = new Dictionary<string, Dictionary<string, string>>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    column = 1;
                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        if (dcol.ColumnName.Trim().ToUpper() == "ROWNUM")
                            continue;

                        if (dcol.ColumnName.Trim().ToUpper() == "BOM_MONTH" || 
                            dcol.ColumnName.Trim().ToUpper() == "REPORT_DATE" ||
                            dcol.ColumnName.Trim().ToUpper() == "DTM_MONTH")
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

        public static string GetSelectedQuery(TreeView treeFields)
        {
            string Fields = String.Empty;
            foreach( string str in DataFields)
            {
                Fields += $"{str},";
            }
            foreach (TreeNode node in treeFields.Nodes)
            {
                if (node.Checked)
                {
                    Fields += $"{node.Text},";
                }
            }
            if (!String.IsNullOrEmpty(Fields))
                Fields = Fields.Substring(0, Fields.Length - 1);

            string Query = String.Empty;
            //if (true)
            //{
            //    Query = $"Select {DataOperations.GetAggregateQuery(Fields, "MONTHLY","Avg", "WeekEnding", "WeekEnding")} from CHICKENEGGS_DIALY_DATA";
            //}
            //else
            //{
                Query = $"Select {Fields},ROW_NUMBER () OVER (PARTITION BY BOM_Month,DTM_Month ORDER BY Report_Date DESC ) AS ROWNUM from CHICKENEGGS_DIALY_DATA";
            //}
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
        public static void InitConfigData(TreeView treeFields)
        {
            treeFields.Nodes.Clear();
            treeFields.CheckBoxes = true;

            BHConfigInfo = commonRepo.ExecuteDataSetFromSP("McF_GET_CE_CONFIG");
            TreeNode BOMNode = treeFields.Nodes.Add("Beginning of Month");
            TreeNode DTMNode = treeFields.Nodes.Add("During the Month");
            foreach (DataRow dr in BHConfigInfo.Tables[0].Rows)
            {
                string Name = dr["DisplayName"].ToString();
                if (Name.Contains("BOM"))
                    BOMNode.Nodes.Add(Name);
                else if(Name.Contains("DTM"))
                    DTMNode.Nodes.Add(Name);
            }
        }
    }
}
