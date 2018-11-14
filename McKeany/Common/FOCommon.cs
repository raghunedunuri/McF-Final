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
    public class FOSelectedData
    {
        public Dictionary<string, Dictionary<string, bool>> SelectedData { get; set; }
        public bool IncludeRegionalData { get; set; }
    }

    internal static class FOCommon
    {
        private static DataSet FOConfigData { get; set; }
        private static ICommonRepository commonRepo;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        public static Dictionary<string, string> FiedMap;
        static FOCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
            FiedMap = new Dictionary<string, string>();
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, FOSelectedData selectedData, string Filters)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = Filters.Split(strArray, StringSplitOptions.None);
            PresentData(currentWorksheet, selectedData, Convert.ToInt32(filters[0]), filters[1], filters[2] );
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, FOSelectedData selectedData, int index, string From, string To)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            int startrow = 3;
            DateTime? dTo = null, dFrom = null;
            switch (index)
            {
                case 0:
                    if (!String.IsNullOrEmpty(To) && !String.IsNullOrEmpty(From))
                    {
                        dFrom = Convert.ToDateTime(From);
                        dTo = Convert.ToDateTime(To);
                    }
                    else if (!String.IsNullOrEmpty(From))
                        dFrom = Convert.ToDateTime(From);
                    break;
                case 2:
                    dFrom = DateTime.Now.AddDays(-7);
                    break;
                case 3:
                    dFrom = DateTime.Now.AddDays(-30);
                    break;
                case 4:
                    dFrom = DateTime.Now.AddDays(-90);
                    break;
            }
            List<string> lstSymbols = new List<string>();
           
            foreach (KeyValuePair<string, Dictionary<string, bool>> kv in selectedData.SelectedData)
            {
                lstSymbols.Add(kv.Key);
            }
            DataSet ds = commonRepo.GetTableFormatedData("McF_GET_FO_UPDATED", lstSymbols, index, dFrom, dTo);
            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, RowData> dictRegionRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();
            Dictionary<string, List<RowData>> dictRegionCommData = new Dictionary<string, List<RowData>>();
            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ComName = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                    string Category = textInfo.ToTitleCase(dr["Category"].ToString().ToLower());
                    string ReportDate = Convert.ToDateTime(dr["Report_Date"]).ToShortDateString();
                    string MonthEndDate = Convert.ToDateTime(dr["Month_EndDate"]).ToShortDateString();
                    string Field = textInfo.ToTitleCase(dr["Field"].ToString().ToLower());
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{Category}{ReportDate}{MonthEndDate}";

                    if (selectedData.SelectedData.ContainsKey(ComName))
                    {
                        Dictionary<string, bool> Fields = selectedData.SelectedData[ComName];
                        if (Fields.ContainsKey(Field))
                        {
                            if (dictRowData.ContainsKey(key))
                            {
                                dictRowData[key].Data.Add(Field, value);
                            }
                            else
                            {
                                dictRowData.Add(key, new RowData());
                                dictRowData[key].Commodity = ComName;
                                dictRowData[key].Category = Category;
                                dictRowData[key].Data.Add("CommodityName", ComName);
                                dictRowData[key].Data.Add("Category", Category);
                                dictRowData[key].Data.Add("ReportDate", ReportDate);
                                dictRowData[key].Data.Add("MonthEndDate", MonthEndDate);
                                dictRowData[key].Data.Add(Field, value);
                            }
                        }
                    }
                }

                if(selectedData.IncludeRegionalData)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        string ComName = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                        string Category = textInfo.ToTitleCase(dr["Category"].ToString().ToLower());
                        string ReportDate = Convert.ToDateTime(dr["Report_Date"]).ToShortDateString();
                        string MonthEndDate = Convert.ToDateTime(dr["Month_EndDate"]).ToShortDateString();
                        string Field = textInfo.ToTitleCase(dr["Field"].ToString().ToLower());
                        string value = dr["DataValue"].ToString();
                        string region = textInfo.ToTitleCase(dr["Region"].ToString().ToLower());
                        string key = $"{ComName}{Category}{ReportDate}{MonthEndDate}{region}";

                        if (selectedData.SelectedData.ContainsKey(ComName))
                        {
                            Dictionary<string, bool> Fields = selectedData.SelectedData[ComName];
                            if (Fields.ContainsKey(Field))
                            {
                                if (dictRegionRowData.ContainsKey(key))
                                {
                                    dictRegionRowData[key].Data.Add(Field, value);
                                }
                                else
                                {
                                    dictRegionRowData.Add(key, new RowData());
                                    dictRegionRowData[key].Commodity = ComName;
                                    dictRegionRowData[key].Category = Category;
                                    dictRegionRowData[key].Data.Add("CommodityName", ComName);
                                    dictRegionRowData[key].Data.Add("Category", Category);
                                    dictRegionRowData[key].Data.Add("ReportDate", ReportDate);
                                    dictRegionRowData[key].Data.Add("MonthEndDate", MonthEndDate);
                                    dictRegionRowData[key].Data.Add("Region", region);
                                    dictRegionRowData[key].Data.Add(Field, value);
                                }
                            }
                        }
                    }
                }
                //Dictionary<string, string> DistinctSymbols = new Dictionary<string, string>();
                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    if (!dictCommData.ContainsKey(kv.Value.Commodity))
                        dictCommData[kv.Value.Commodity] = new List<RowData>();
                    dictCommData[kv.Value.Commodity].Add(kv.Value);
                }
                foreach (KeyValuePair<string, RowData> kv in dictRegionRowData)
                {
                    if (!dictRegionCommData.ContainsKey(kv.Value.Commodity))
                        dictRegionCommData[kv.Value.Commodity] = new List<RowData>();
                    dictRegionCommData[kv.Value.Commodity].Add(kv.Value);
                }


                bool bFirstRow = true;
                int runningRow = startrow + 2;
                int headerRow = 0;
                foreach ( KeyValuePair<string,List<RowData>> kv in dictCommData )
                {
                   
                    bFirstRow = true;
                    currentWorksheet.Cells[startrow, 1] = "Commodity";
                    currentWorksheet.Cells[startrow, 2] = "Category";
                    int column = 3;
                    foreach (RowData rowData in kv.Value)
                    {
                        column = 3;
                        currentWorksheet.Cells[runningRow, 1] = rowData.Commodity;
                        currentWorksheet.Cells[runningRow, 2] = rowData.Category;
                        foreach (KeyValuePair<string, string> rv in rowData.Data)
                        {
                            if (bFirstRow)
                            {
                                headerRow = startrow;
                                string key = rv.Key;
                                currentWorksheet.Cells[startrow+1, column] = key;
                                DataRow[] dr = FOConfigData.Tables[1].Select($"Commodity_Name='{rowData.Commodity}' AND FIELD = '{rv.Key}'");
                                if (dr != null && dr.Length > 0)
                                {
                                    currentWorksheet.Cells[startrow, column] = dr[0]["unit"].ToString();
                                }
                            }
                            currentWorksheet.Cells[runningRow, column++] = rv.Value;
                        }
                        bFirstRow = false;
                        currentWorksheet.Cells[runningRow, 450] = headerRow;
                        runningRow++;
                        startrow++;
                    }
                    runningRow += 3;
                    startrow += 3;
               }

                foreach (KeyValuePair<string, List<RowData>> kv in dictRegionCommData)
                {
                    bFirstRow = true;
                    currentWorksheet.Cells[startrow, 1] = "Commodity\n(Regional)";
                    currentWorksheet.Cells[startrow, 2] = "Category";
                    int column = 3;
                    foreach (RowData rowData in kv.Value)
                    {
                        currentWorksheet.Cells[runningRow, 1] = rowData.Commodity;
                        currentWorksheet.Cells[runningRow, 2] = rowData.Category;
                        foreach (KeyValuePair<string, string> rv in rowData.Data)
                        {
                            if (bFirstRow)
                            {
                                headerRow = startrow;
                                string key = rv.Key;
                                currentWorksheet.Cells[startrow+1, column] = key;
                                DataRow[] dr = FOConfigData.Tables[1].Select($"Commodity_Name='{rowData.Commodity}' AND FIELD = '{rv.Key}'");
                                if (dr != null && dr.Length > 0)
                                {
                                    currentWorksheet.Cells[startrow, column] = dr[0]["unit"].ToString();
                                }
                            }
                            currentWorksheet.Cells[runningRow, column++] = rv.Value;
                        }
                        bFirstRow = false;
                        currentWorksheet.Cells[runningRow, 450] = headerRow;
                        runningRow++;
                        startrow++;
                    }
                    runningRow++;
                    startrow++;
                }
            }
            else
                currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }
        private static int GetRandom(int min, int max)
        {
            Random r = new Random();
            return r.Next(min, max);
        }
        public static FOSelectedData GetSelectedQuery(TreeView treeGroups)
        {
            FOSelectedData foSelectData = new FOSelectedData();
            foSelectData.SelectedData = new Dictionary<string, Dictionary<string, bool>>();
            foreach (TreeNode node in treeGroups.Nodes)
            {
                if (node.Text.ToUpper() == "SOYBEANS")
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if( subNode.Checked )
                        {
                            if (subNode.Checked)
                            {
                                if (!foSelectData.SelectedData.ContainsKey(node.Text))
                                {
                                    foSelectData.SelectedData.Add(node.Text.Trim(), new Dictionary<string, bool>());
                                }
                                foSelectData.SelectedData[node.Text.Trim()].Add(subNode.Text.Trim(), true);
                            }
                        }
                    }
                }
                else
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if (subNode.Checked && subNode.Nodes.Count > 0)
                        {
                            foSelectData.SelectedData.Add(subNode.Text.Trim(), new Dictionary<string, bool>());
                            foreach (TreeNode condNode in subNode.Nodes)
                            {
                                if (condNode.Checked)
                                    foSelectData.SelectedData[subNode.Text.Trim()].Add(condNode.Text.Trim(), true);
                            }
                        }
                    }
                }
            }
            return foSelectData;
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

        public static void InitConfigData(TreeView treeGroups)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;
          
            Dictionary<string, List<string>> dictCategories = new Dictionary<string, List<string>>();

            FOConfigData = commonRepo.ExecuteDataSetFromSP("McF_GET_FATSOIL_GROUPDATA");
         
            TreeNode childNode = null;
            foreach(DataRow dr in FOConfigData.Tables[0].Rows)
            {
                string category = textInfo.ToTitleCase(dr["Category"].ToString().ToLower());
                string commodity = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());

                if (!dictCategories.ContainsKey(category))
                {
                    dictCategories.Add(category, new List<string>());
                    childNode = treeGroups.Nodes.Add(category);
                }
                TreeNode comNode = null;
                if (commodity.ToUpper() == "SOYBEANS")
                    comNode = childNode;
                else
                    comNode = childNode.Nodes.Add(commodity);
                foreach(DataRow cr in FOConfigData.Tables[1].Select($"Commodity_Name='{commodity.ToUpper()}'"))
                {
                    comNode.Nodes.Add(cr["DisplayName"].ToString());
                    FiedMap[cr["DisplayName"].ToString()] = cr["Field"].ToString();
                }
            }
        }
    }
}
