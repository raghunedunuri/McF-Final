using McF.Common;
using McF.Contracts;
using McF.DataAccess.Repositories.Implementors;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace McKeany
{
    // public class 
    public class UIData
    {
        public Dictionary<string, Dictionary<string, List<string>>> Groups { get; set;}
        public Dictionary<string, Dictionary<string, List<string>>> Fields { get; set; }
        public int DateRangeIndex { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsRollUp { get; set; }
        public bool IsMatrix { get; set; }
        public bool IsAutoUpdate { get; set; }
        public string RollUpFrequency { get; set; }
        public string RollUpValue { get; set; }
        public string DataSource { get; set; }
        public string DataTable { get; set; }
        public bool IncludeRegionalData { get; set; }
        public int FiscalMonth { get; set; }
        public int SelectedSource { get; set; }
        public bool IsSymbolInGroups { get; set; }
        public UIData()
        {
            Groups = null;
            Fields = null;
            IsSymbolInGroups = false;
            FiscalMonth = 0;
            SelectedSource = 0;
            IsAutoUpdate = false;
        }

        public ExcelQuery GetExcelQuery()
        {
            ExcelQuery excelQuery = new ExcelQuery();
            excelQuery.DateIndex = DateRangeIndex;
            if (DateRangeIndex == 0)
            {
                excelQuery.StartDate = Convert.ToDateTime(StartDate);
                excelQuery.EndDate = Convert.ToDateTime(EndDate);
                if (excelQuery.StartDate > excelQuery.EndDate)
                {
                    MessageBox.Show("Start date is greater than End date");
                    return null;
                }
            }
            if (!String.IsNullOrEmpty(RollUpValue) && RollUpValue != "None")
            {
                excelQuery.PeriodSummary = RollUpValue;
                if (excelQuery.PeriodSummary == "Calendar Year")
                    excelQuery.PeriodSummary = "Yearly";
                else if (excelQuery.PeriodSummary == "Fiscal Year")
                    excelQuery.PeriodSummary = "Fiscal";
            }
            if (!String.IsNullOrEmpty(RollUpFrequency) && RollUpFrequency != "None")
                excelQuery.PeriodType = RollUpFrequency;
            excelQuery.FiscalMonth = FiscalMonth;
            return excelQuery;
        }

        public void UpdateUIData(TreeView treeGroups, TreeView treeFields, string startDate, 
            string endDate, int dateRange, bool isRollUp, bool isMatrix, 
            string rollUpFreq, string rollUpValue, string dataSource, int  fiscalMonth = 0, bool isAutoUpdate = false)
        {
            PresentFields(treeGroups, treeFields);
            StartDate = startDate;
            EndDate = endDate;
            DateRangeIndex = dateRange;
            IsRollUp = isRollUp;
            IsMatrix = isMatrix;
            RollUpFrequency = rollUpFreq;
            RollUpValue = rollUpValue;
            DataSource = dataSource;
            FiscalMonth = fiscalMonth;
            IsAutoUpdate = isAutoUpdate;
        }

        public Dictionary<string, List<String>> GetCommodityFields( List<string> Commodities )
        {
            Dictionary<string, List<String>> CommFields = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups )
            {
                Commodities.Add(kv.Key);
                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        if (kvSub.Value != null)
                        {
                            CommFields[kvSub.Key] = kvSub.Value;
                        }
                        else
                        {
                            if (!CommFields.ContainsKey(kv.Key))
                                CommFields[kv.Key] = new List<string>();
                            CommFields[kv.Key].Add(kvSub.Key);
                        }
                    }
                }
            }
            return CommFields;
        }

        public Dictionary<string, List<String>> GetFOCommodityFields(List<string> Commodities)
        {
            Dictionary<string, List<String>> CommFields = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        Commodities.Add(kvSub.Key);
                        if (kvSub.Value != null)
                        {
                            CommFields[kvSub.Key] = kvSub.Value;
                        }
                        else
                        {
                            if (!CommFields.ContainsKey(kv.Key))
                                CommFields[kv.Key] = new List<string>();
                            CommFields[kv.Key].Add(kvSub.Key);
                        }
                    }
                }
                else
                {
                    Commodities.Add(kv.Key);
                }
            }
            return CommFields;
        }

        public Dictionary<string, List<String>> GetCROPCommodityFields(List<string> Commodities)
        {
            Dictionary<string, List<String>> CommFields = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                Commodities.Add(kv.Key);

                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        if (!CommFields.ContainsKey(kv.Key))
                                CommFields[kv.Key] = new List<string>();
                        string val = kvSub.Key.ToUpper();
                        if (val == "CONDITION")
                        {
                            foreach (string str in kvSub.Value)
                            {
                                CommFields[kv.Key].Add(str);
                            }
                        }
                        else
                        {
                            CommFields[kv.Key].Add(kvSub.Key);
                        }
                    }
                }
                else
                {
                    Commodities.Add(kv.Key);
                }
            }
            return CommFields;
        }

        public List<String> GetSelectedFields()
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                SelectedFields.Add(kv.Key);
            }
            return SelectedFields;
        }

        public List<String> GetSelectedFromFields()
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Fields)
            {
                SelectedFields.Add(kv.Key);
            }
            return SelectedFields;
        }

        public List<String> GetSelectedSymbols()
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        SelectedFields.Add(kvSub.Key);
                    }
                }
                else
                    SelectedFields.Add(kv.Key);
            }
            return SelectedFields;
        }

        public List<String> GetSelectedSymbols(Dictionary<string, string> Mapping)
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        SelectedFields.Add(Mapping[kvSub.Key]);
                    }
                }
                else
                    SelectedFields.Add(Mapping[kv.Key]);
            }
            return SelectedFields;
        }

        public List<String> GetSymbolsAndFieldsFromGroup(List<string> symbols)
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Groups)
            {
                if (kv.Value != null)
                {
                    SelectedFields.Add(kv.Key);

                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        symbols.Add( kvSub.Key);

                    }
                }
            }
            return SelectedFields;
        }

        public List<String> GetFieldsFromSubGroup()
        {
            List<String> SelectedFields = new List<String>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Fields)
            {
                if (kv.Value != null)
                {
                    foreach (KeyValuePair<string, List<string>> kvSub in kv.Value)
                    {
                        SelectedFields.Add(kvSub.Key);
                    }
                }
                else
                {
                    SelectedFields.Add(kv.Key);
                }
            }
            return SelectedFields;
        }

        public void PresentFields(TreeView treeGroups, TreeView treeFields)
        {
            if (treeGroups != null)
            {
                if (Groups == null)
                    Groups = new Dictionary<string, Dictionary<string, List<string>>>();

                foreach (TreeNode n1 in treeGroups.Nodes)
                {
                    Dictionary<string, List<string>> dictSubNodes = new Dictionary<string, List<string>>();
                    if (n1.Nodes.Count > 0)
                    {
                        foreach (TreeNode n2 in n1.Nodes)
                        {
                            List<string> lstSubNodes = new List<string>();
                            if (n2.Nodes.Count > 0)
                            {
                                foreach (TreeNode n3 in n2.Nodes)
                                {
                                    if (n3.Checked)
                                    {
                                        lstSubNodes.Add(n3.Text);
                                    }
                                }
                                if (lstSubNodes.Count > 0)
                                    dictSubNodes.Add(n2.Text, lstSubNodes);
                            }
                            else if (n2.Checked)
                            {
                                dictSubNodes.Add(n2.Text, null);
                            }
                        }

                        if (dictSubNodes.Count > 0)
                            Groups.Add(n1.Text, dictSubNodes);
                    }
                    else if (n1.Checked)
                    {
                        Groups.Add(n1.Text, null);
                    }
                }
            }
            if (treeFields != null)
            {
                if (Fields == null)
                    Fields = new Dictionary<string, Dictionary<string, List<string>>>();

                foreach (TreeNode n1 in treeFields.Nodes)
                {
                    if (n1.Nodes.Count > 0)
                    {
                        Dictionary<string, List<string>> dictSubNodes = new Dictionary<string, List<string>>();

                        foreach (TreeNode n2 in n1.Nodes)
                        {
                            List<string> lstSubNodes = new List<string>();
                            if (n2.Nodes.Count > 0)
                            {
                                foreach (TreeNode n3 in n2.Nodes)
                                {
                                    if (n3.Checked)
                                    {
                                        lstSubNodes.Add(n3.Text);
                                    }
                                }
                                if (lstSubNodes.Count > 0)
                                    dictSubNodes.Add(n2.Text, lstSubNodes);
                            }
                            else if (n2.Checked)
                            {
                                dictSubNodes.Add(n2.Text, null);
                            }
                        }
                        if (dictSubNodes.Count > 0)
                            Fields.Add(n1.Text, dictSubNodes);
                    }
                    else if (n1.Checked)
                    {
                        Fields.Add(n1.Text, null);
                    }
                }
            }
        }

        public void ShowData(TreeView treeGroups, TreeView treeFields)
        {
            if (treeGroups != null)
            {
                foreach (TreeNode n1 in treeGroups.Nodes)
                {
                    bool bCheckHomeNode = true;
                    if (Groups.ContainsKey(n1.Text) && Groups[n1.Text] == null)
                    {
                        n1.Checked = true;
                    }
                    if (n1.Nodes.Count > 0)
                    {
                        foreach (TreeNode n2 in n1.Nodes)
                        {
                            if (Groups.ContainsKey(n1.Text) && Groups[n1.Text] != null && Groups[n1.Text].ContainsKey(n2.Text) && Groups[n1.Text][n2.Text] == null)
                            {
                                n2.Checked = true;
                            }
                            if( n2.Nodes.Count > 0)
                            {
                                bool bCheckRootNode = true;
                                foreach (TreeNode n3 in n2.Nodes)
                                {
                                    if (Groups.ContainsKey(n1.Text) && Groups[n1.Text] != null &&
                                        Groups[n1.Text].ContainsKey(n2.Text) && Groups[n1.Text][n2.Text] != null &&
                                        Groups[n1.Text][n2.Text].Contains(n3.Text))
                                    {
                                        n3.Checked = true;
                                    }
                                    else
                                        bCheckRootNode = false;
                                }
                                n2.Checked = bCheckRootNode;
                            }

                            if (!n2.Checked)
                                bCheckHomeNode = false;
                        }
                        n1.Checked = bCheckHomeNode;
                    }
                }
            }
            if (treeFields != null)
            {
                foreach (TreeNode n1 in treeFields.Nodes)
                {
                    bool bCheckHomeNode = true;

                    if (Fields.ContainsKey(n1.Text) && Fields[n1.Text] == null)
                    {
                        n1.Checked = true;
                    }
                    if (n1.Nodes.Count > 0)
                    {
                        foreach (TreeNode n2 in n1.Nodes)
                        {
                            if (Fields.ContainsKey(n1.Text) && Fields[n1.Text] != null && 
                                Fields[n1.Text].ContainsKey(n2.Text) && Fields[n1.Text][n2.Text] == null)
                            {
                                n2.Checked = true;
                            }
                            if (n2.Nodes.Count > 0)
                            {
                                bool bCheckRootNode = true;
                                foreach (TreeNode n3 in n2.Nodes)
                                {
                                    if (Fields.ContainsKey(n1.Text) && Fields[n1.Text] != null &&
                                        Fields[n1.Text].ContainsKey(n2.Text) && Fields[n1.Text][n2.Text] != null &&
                                        Fields[n1.Text][n2.Text].Contains(n3.Text))
                                    {
                                        n3.Checked = true;
                                    }
                                    else
                                        bCheckRootNode = false;
                                }
                                n2.Checked = bCheckRootNode;
                            }
                            if (!n2.Checked)
                                bCheckHomeNode = false;
                        }
                        n1.Checked = bCheckHomeNode;
                    }
                }
            }
        }
        //public string PersistData(TreeView treeGroups, TreeView treeFields)
        //{
        //    object json = new object();
        //    //json.Ap
        //  //  string json =
        //}
}

    //public class TreeviewPersist
    //{

    //    // Persist the TreeView to a JSON string.
    //    static public string ToJson(TreeView treeView)
    //    {
    //        Chilkat.JsonObject tvJson = new Chilkat.JsonObject();
    //        Chilkat.JsonArray tvNodes = tvJson.AppendArray("treeViewNodes");

    //        TreeNodeCollection nodes = treeView.Nodes;
    //        foreach (TreeNode n in nodes)
    //        {
    //            serializeTree(tvNodes, n);
    //        }

    //        tvJson.EmitCompact = false;
    //        return tvJson.Emit();
    //    }

    //    // Clears the passed-in treeView and rebuilds from JSON.
    //    static public void FromJson(string strJson, TreeView treeView)
    //    {
    //        treeView.Nodes.Clear();

    //        Chilkat.JsonObject tvJson = new Chilkat.JsonObject();
    //        tvJson.Load(strJson);
    //        Chilkat.JsonArray tvNodes = tvJson.ArrayOf("treeViewNodes");

    //        int numNodes = tvNodes.Size;
    //        for (int i = 0; i < numNodes; i++)
    //        {
    //            Chilkat.JsonObject json = tvNodes.ObjectAt(i);

    //            if (json.IsNullOf("parentName"))
    //            {
    //                TreeNode node = treeView.Nodes.Add(json.StringOf("name"), json.StringOf("text"));
    //                restoreNode(node, json);
    //            }
    //            else
    //            {
    //                // Assumes unique names (i.e. keys)
    //                TreeNode[] foundNodes = treeView.Nodes.Find(json.StringOf("parentName"), true);
    //                if (foundNodes.Length > 0)
    //                {
    //                    TreeNode node = foundNodes[0].Nodes.Add(json.StringOf("name"), json.StringOf("text"));
    //                    restoreNode(node, json);
    //                }
    //            }

    //        }
    //    }

    //    // Restore the properties of a TreeNode from JSON.
    //    static private void restoreNode(TreeNode node, Chilkat.JsonObject json)
    //    {
    //        node.Tag = json.StringOf("tag");
    //        node.Text = json.StringOf("text");
    //        node.ToolTipText = json.StringOf("toolTipText");
    //        node.Checked = json.BoolOf("checked");
    //    }

    //    // Recursive method to add TreeView nodes to the JSON.
    //    static private void serializeTree(Chilkat.JsonArray tvNodes, TreeNode treeNode)
    //    {
    //        tvNodes.AddObjectAt(-1);

    //        Chilkat.JsonObject json = tvNodes.ObjectAt(tvNodes.Size - 1);
    //        json.UpdateString("name", treeNode.Name);

    //        TreeNode parent = treeNode.Parent;
    //        if (parent != null)
    //        {
    //            json.UpdateString("parentName", treeNode.Parent.Name);
    //        }
    //        else
    //        {
    //            json.UpdateNull("parentName");
    //        }

    //        json.UpdateString("tag", (string)treeNode.Tag);
    //        json.UpdateString("text", treeNode.Text);
    //        json.UpdateString("toolTipText", treeNode.ToolTipText);
    //        json.UpdateBool("checked", treeNode.Checked);

    //        foreach (TreeNode tn in treeNode.Nodes)
    //        {
    //            serializeTree(tvNodes, tn);
    //        }

    //    }


    //}
    internal static class DataOperations
    {
        public static string GetQuery()
        {
            return string.Empty;
        }
        public static string GetDateQuery(int Index , string From, string To, string FieldName = "UpdatedTime" , string tableName = "DTN_DIALY_DATA")
        {
            string dateFilter = String.Empty;
            switch (Index)
            {
                case 0:
                    if (!String.IsNullOrEmpty(To) && !String.IsNullOrEmpty(From))
                        dateFilter = $"{FieldName} >= '{From}' and {FieldName} <= '{To}'";
                    else if (!String.IsNullOrEmpty(From))
                        dateFilter = $"{FieldName} >= '{From}'";
                    break;
                case 1:
                    dateFilter = $"{FieldName} = ( select max({FieldName}) from {tableName} )";
                    break;
                case 2:
                    dateFilter = $"{FieldName} >= '{DateTime.Now.AddDays(-7).ToString()}'";
                    break;
                case 3:
                    dateFilter = $"{FieldName} >= '{DateTime.Now.AddDays(-30).ToString()}'";
                    break;
                case 4:
                    dateFilter = $"{FieldName} >= '{DateTime.Now.AddDays(-90).ToString()}'";
                    break;
            }
            return dateFilter;
        }

        public static string GetColType(DataTable dt, string columnName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["COLUMN_NAME"].ToString().ToUpper().Trim() == columnName.ToUpper().Trim())
                    return dr["DATA_TYPE"].ToString();
            }
            return String.Empty;
        }
        public static void ShowDTNData(Excel.Worksheet currentWorksheet, string headers, string fields, string query, int StartRow)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            String[] FeildList  = fields.Split(',');
            String[] HeaderList = headers.Split(',');
            SqlDbHelper sqlDBHelper = new SqlDbHelper(new SqlConnectionManager());
            DTNRepository dtnr = new DTNRepository(sqlDBHelper);
            DataSet ds = dtnr.GetDTNData(query);
            Dictionary<string, List<string>> dc = new Dictionary<string, List<string>>();
            int Column = 2;
            foreach (string str in HeaderList)
            {
                currentWorksheet.Cells[StartRow, Column++] = str;
            }
            StartRow++;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string date = dr["DATE"].ToString();
                if (!dc.ContainsKey(date))
                {
                    dc.Add(date, new List<string>());
                }
                foreach (string str in FeildList)
                {
                    if( str != "date" && str != "symbol")
                        dc[date].Add(dr[str].ToString());
                }
            }
            
            foreach (KeyValuePair<string, List<String>> kv in dc)
            {
                Column = 1;
                currentWorksheet.Cells[StartRow, Column] = kv.Key;
                foreach (string str in kv.Value)
                {
                    currentWorksheet.Cells[StartRow, ++Column] = str;
                }
                StartRow++;
            }
        }

        public static void UpdateDTNData(string query)
        {
            SqlDbHelper sqlDBHelper = new SqlDbHelper(new SqlConnectionManager());
            DTNRepository dtnr = new DTNRepository(sqlDBHelper);
            dtnr.UpdateDTNData(query);
        }

        public static string GetFieldsAggregatedQuery(string DataFields, string Frquency, string RollUpValue, string DateField, string groupFileds)
        {
            string[] strArr = DataFields.Split(',');
            string query = String.Empty;
            foreach(string str in strArr)
            {
                query += GetAggregateQuery(str, Frquency, RollUpValue, DateField, groupFileds);
            }
            query += GetRowNumberQuery(Frquency, DateField, groupFileds);
            return query;
        }
        public static string GetAggregateQuery(string Field, string Frquency, string RollUpValue, string DateField, string groupFileds)
        {
            string appendFrequency = String.Empty;
            switch (Frquency.ToUpper())
            {
                case "WEEKLY":
                    appendFrequency = "WEEK";
                    break;
                case "MONTHLY":
                    appendFrequency = "MONTH";
                    break;
                case "QUARTERLY":
                    appendFrequency = "QUARTER";
                    break;
                case "YEARLY":
                    appendFrequency = "YEAR";
                    break;
            }
            string dateField = $"DATEADD({appendFrequency}, DATEDIFF({appendFrequency}, 0, {DateField}), 0)";
            string partitionField = $"OVER (PARTITION BY {dateField}, {groupFileds} ) AS ";
            string partitField = $"OVER (PARTITION BY {dateField}, {groupFileds} ORDER BY {DateField} ) AS ";
            string selText = String.Empty;
            switch (RollUpValue.ToUpper())
            {
                case "AVG":
                    selText = "Avg";
                    break;
                case "MIN":
                    selText = "MIN";
                    break;
                case "MAX":
                    selText = "MAX";
                    break;
                case "COUNT":
                    selText = "Count";
                    break;
                case "FIRST VALUE":
                    selText = "FIRST_VALUE";
                    break;
                case "LAST VALUE":
                    selText = "LAST_VALUE";
                    break;
            }
            string dataField = $"{selText}({Field}) {partitField} {Field} ";
            string fieldValue = $"{dataField},";
            return fieldValue;
            //ROW_NUMBER() OVER(PARTITION BY DATEADD(MONTH, DATEDIFF(MONTH, 0, WeekEnding), 0), STATE, MAPPINGID ORDER BY WeekEnding) ROWNUM
        }
        public static string GetRowNumberQuery(string Frquency, string DateField, string groupFileds)
        {
            string appendFrequency = String.Empty;
            switch (Frquency.ToUpper())
            {
                case "WEEKLY":
                    appendFrequency = "WEEK";
                    break;
                case "MONTHLY":
                    appendFrequency = "MONTH";
                    break;
                case "QUARTERLY":
                    appendFrequency = "QUARTER";
                    break;
                case "YEARLY":
                    appendFrequency = "YEAR";
                    break;
            }
            string dateField = $"DATEADD({appendFrequency}, DATEDIFF({appendFrequency}, 0, {DateField}), 0)";
            string partitionField = $"OVER (PARTITION BY {dateField}, {groupFileds} ";
            return $"ROW_NUMBER() {partitionField} ORDER BY {DateField}) ROWNUM, {dateField} AS Group by Period";
        }
    }
}
