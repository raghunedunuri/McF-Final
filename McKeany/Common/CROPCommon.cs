using McF.Contracts;
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
    public class CROPPresentData
    {
        public int row { get; set; }
        public int column { get; set; }
        public bool isCond { get; set; }
        public string dbFieldName { get; set; }
        public string displayName { get; set; }
        public string value { get; set; }

        public override string ToString()
        {
            return dbFieldName;
        }
    }

    public class CROPSymData
    {
        public CROPSymData()
        {
            cropFieldInfo = new Dictionary<string, CROPPresentData>();
        }
        public string state { get; set; }
        public string WeekEnding { get; set; }
        public Dictionary<string, CROPPresentData> cropFieldInfo { get; set; }
        public override string ToString()
        {
            return $"{state}-{WeekEnding}";
        }
    }

    public class CROPSymPresent
    {
        public string symbol { get; set; }
        public int row{ get; set; }
        public Dictionary<string,CROPSymData> cropSymInfo { get; set; }
        public CROPSymPresent()
        {
            cropSymInfo = new Dictionary<string, CROPSymData>();
        }
    }
    public class SymbolCndValues
    {
        public string WeekEnding { get; set; }
        public string State { get; set; }
        public string Value { get; set; }
        public string Symbol { get; set; }

        public int TopRow { get; set; }
    }

    internal static class CROPCommon
    {
        private static Dictionary<string, CROPSymPresent> SymbolMapping { get; set; }
        private static Dictionary<string, string> CondMapping { get; set; }
        private static Dictionary<string, List<SymbolCndValues>> CondValues { get; set; }
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private static ICropProgressRepository cropRepository;

        static CROPCommon()
        {
            cropRepository = UnityResolver._unityContainer.Resolve<CropProgressRepository>();
            SymbolMapping = new Dictionary<string, CROPSymPresent>();
            CondMapping = new Dictionary<string, string>();
            CondValues = new Dictionary<string, List<SymbolCndValues>>();
        }

        private static string GetCondString(DataRow dr, string condValue )
        {
            for( int i = 1; i <= 10; i++)
            {
                string condField = $"COND{i}FIELD";
                if (dr[condField] != DBNull.Value)
                {
                    string fieldValue = dr[condField].ToString().Trim();
                    if (fieldValue == condValue)
                    {
                        CondMapping[condValue] = $"COND{i}";
                        return dr[$"COND{i}VALUE"].ToString().Trim();
                    }
                }
            }
            return String.Empty;
        }

        private static void CheckForCondString(DataRow dr, Dictionary<string, CROPConditions> cropConds)
        {
            for (int i = 1; i <= 10; i++)
            {
                string condField = $"COND{i}FIELD";
                if (dr[condField] != DBNull.Value)
                {
                    string fieldValue = dr[condField].ToString().Trim();
                    if (cropConds.ContainsKey(fieldValue))
                    {
                        CondMapping[fieldValue] = $"COND{i}";

                        if (!CondValues.ContainsKey(fieldValue))
                        {
                            CondValues[fieldValue] = new List<SymbolCndValues>();
                        }
                        CondValues[fieldValue].Add(new SymbolCndValues()
                        {
                            State = dr["State"].ToString(),
                            WeekEnding = Convert.ToDateTime(dr["WeekEnding"]).ToShortDateString(),
                            Value = dr[$"COND{i}VALUE"].ToString(),
                            Symbol = dr["Symbol"].ToString()
                        });
                    }
                }
            }
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, CROPSelectionData cropSelData, string Filters)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = Filters.Split(strArray, StringSplitOptions.None);
            PresentData(currentWorksheet, cropSelData, Convert.ToInt32(filters[0]), filters[1], filters[2]);
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, CROPSelectionData cropSelData, int index, string From, string To)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            SymbolMapping.Clear();
            CondMapping.Clear();
            CondValues.Clear();

            DateTime? dTo = null, dFrom =null;
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

            DataSet ds = cropRepository.GetCropFormatedData(cropSelData.lstSymbols, index, dFrom, dTo, cropSelData.IsRollUp, cropSelData.RollUpFrequency, cropSelData.RollUpData );

            if( ds!= null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string symbol = dr["Symbol"].ToString().Trim();
                    string field = dr["Field"].ToString().Trim();

                    if (cropSelData.dctCROPFieldInfo.ContainsKey(symbol))
                    {
                        List<string> fields = cropSelData.dctCROPFieldInfo[symbol].Fields;
                        if (fields.Contains(field))
                        {
                            string state = dr["State"].ToString();
                            string WeekEnding = String.Empty;
                            if( !cropSelData.IsRollUp )
                                WeekEnding = Convert.ToDateTime(dr["WeekEnding"]).ToShortDateString();
                            else if( dr["Group by Period"] != DBNull.Value)
                                WeekEnding = Convert.ToDateTime(dr["Group by Period"]).ToShortDateString();

                            CROPSymPresent cropSym = null;
                            if (!SymbolMapping.ContainsKey(symbol))
                            {
                                cropSym = new CROPSymPresent()
                                {
                                    symbol = symbol
                                };
                                SymbolMapping[symbol] = cropSym;
                            }
                            cropSym = SymbolMapping[symbol];
                            string unique = $"{state}|{WeekEnding}";
                            CROPSymData corpSymdata = null;
                            if (!cropSym.cropSymInfo.ContainsKey(unique))
                            {
                                corpSymdata = new CROPSymData()
                                {
                                    state = state,
                                    WeekEnding = WeekEnding
                                };
                                cropSym.cropSymInfo[unique] = corpSymdata;
                            }
                            CROPPresentData cropPresentData = new CROPPresentData()
                            {
                                isCond = false,
                                displayName = field,
                                dbFieldName = field,
                                value = dr["Value"].ToString()
                            };
                            corpSymdata = cropSym.cropSymInfo[unique];
                            corpSymdata.cropFieldInfo[field] = cropPresentData;
                        }
                    }
                }

                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    string symbol = dr["Symbol"].ToString().Trim();
                    string state = dr["State"].ToString();
                    string WeekEnding = String.Empty;
                    if (!cropSelData.IsRollUp)
                        WeekEnding = Convert.ToDateTime(dr["WeekEnding"]).ToShortDateString();
                    else if (dr["Group by Period"] != DBNull.Value)
                        WeekEnding = Convert.ToDateTime(dr["Group by Period"]).ToShortDateString();
                    
                    // if( cropSelData.dctCROPCondInfo.)
                    CheckForCondString(dr, cropSelData.dctCROPCondInfo);
                    if (cropSelData.dctCROPFieldInfo.ContainsKey(symbol))
                    {
                        foreach (string cond in cropSelData.dctCROPFieldInfo[symbol].Conditions)
                        {
                            CROPSymPresent cropSym = null;
                            if (!SymbolMapping.ContainsKey(symbol))
                            {
                                cropSym = new CROPSymPresent()
                                {
                                    symbol = symbol
                                };
                                SymbolMapping[symbol] = cropSym;
                            }
                            cropSym = SymbolMapping[symbol];
                            string unique = $"{state}|{WeekEnding}";
                            CROPSymData corpSymdata = null;
                            if (!cropSym.cropSymInfo.ContainsKey(unique))
                            {
                                corpSymdata = new CROPSymData()
                                {
                                    state = state,
                                    WeekEnding = WeekEnding
                                };
                                cropSym.cropSymInfo[unique] = corpSymdata;
                            }
                            string val = GetCondString(dr, cond);
                            CROPPresentData cropPresentData = new CROPPresentData()
                            {
                                isCond = true,
                                displayName = cond,
                                dbFieldName = cond,
                                value = val
                            };
                            corpSymdata = cropSym.cropSymInfo[unique];
                            corpSymdata.cropFieldInfo[cond] = cropPresentData;
                        }
                    }
                }


                int startrow = 3;
                int runningRow = 4;
                int column = 4;

                if (SymbolMapping.Count > 0)
                {
                    foreach (KeyValuePair<string, CROPSymPresent> kv in SymbolMapping)
                    {
                        if (kv.Value.cropSymInfo.Count > 0)
                        {
                            currentWorksheet.Cells[startrow+1, 1] = "Symbol";
                            currentWorksheet.Cells[startrow+1, 2] = "WeekEnding";
                            currentWorksheet.Cells[startrow+1, 3] = "State";
                            bool bFirst = true;
                            kv.Value.row = startrow;
                            runningRow = startrow + 2;

                            foreach (KeyValuePair<string, CROPSymData> ks in kv.Value.cropSymInfo)
                            {
                                column = 4;
                                foreach (KeyValuePair<string, CROPPresentData> kp in ks.Value.cropFieldInfo)
                                {
                                    if (bFirst)
                                    {
                                        currentWorksheet.Cells[startrow+1, column] = kp.Value.displayName;
                                        currentWorksheet.Cells[startrow, column] = "%";
                                    }
                                    currentWorksheet.Cells[runningRow, column++] = kp.Value.value;
                                }
                                currentWorksheet.Cells[runningRow, 1] = kv.Key;
                                currentWorksheet.Cells[runningRow, 2] = ks.Value.WeekEnding;
                                currentWorksheet.Cells[runningRow, 3] = ks.Value.state;
                                currentWorksheet.Cells[runningRow, 50] = startrow;
                                runningRow++;
                                bFirst = false;
                            }
                            startrow = runningRow + 1;
                        }
                    }
                }

                if (CondValues.Count > 0)
                {
                    foreach (KeyValuePair<string, List<SymbolCndValues>> kv in CondValues)
                    {
                        currentWorksheet.Cells[startrow, 1] = "Symbol";
                        currentWorksheet.Cells[startrow, 2] = "WeekEnding";
                        currentWorksheet.Cells[startrow, 3] = "State";
                        currentWorksheet.Cells[startrow, 4] = kv.Key;
                        runningRow = startrow + 1;
                        foreach (SymbolCndValues symCnd in kv.Value)
                        {
                            currentWorksheet.Cells[runningRow, 1] = symCnd.Symbol;
                            currentWorksheet.Cells[runningRow, 2] = symCnd.WeekEnding;
                            currentWorksheet.Cells[runningRow, 3] = symCnd.Symbol;
                            currentWorksheet.Cells[runningRow, 4] = symCnd.Value;
                            currentWorksheet.Cells[runningRow++, 50] = startrow;
                        }
                        startrow = runningRow + 1;
                    }
                }
            }
            else 
              currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }
        public static CROPSelectionData GetSelectedQuery(TreeView treeGroups, TreeView treeFields, 
            bool bIsRollUp, bool bIsMatrixFormat, string RollUpFrequency, string RollUpText)
        {
            CROPSelectionData cropSelData = new CROPSelectionData();
            cropSelData.IsRollUp = bIsRollUp;
            cropSelData.IsMatrixMethod = bIsMatrixFormat;
            cropSelData.RollUpFrequency = RollUpFrequency;
            cropSelData.RollUpData = RollUpText;
            foreach (TreeNode node in treeGroups.Nodes)
            {
                CROPSymbolFieldInfo cropSymFieldInfo = new CROPSymbolFieldInfo();
                cropSymFieldInfo.Symbol = node.Text.ToUpper();
                foreach ( TreeNode subNode in node.Nodes)
                {
                    if (subNode.Nodes.Count > 0)
                    {
                        foreach (TreeNode condNode in subNode.Nodes)
                        {
                            if (condNode.Checked)
                                cropSymFieldInfo.Conditions.Add(condNode.Text.Trim().ToUpper());
                        }
                    }
                    else if (subNode.Checked)
                        cropSymFieldInfo.Fields.Add(subNode.Text.Trim().ToUpper());
                }
                if (cropSymFieldInfo.Conditions.Count > 0 || cropSymFieldInfo.Fields.Count > 0)
                {
                    cropSelData.dctCROPFieldInfo[node.Text.Trim().ToUpper()] = cropSymFieldInfo;
                    if (!cropSelData.lstSymbols.Contains(node.Text.Trim().ToUpper()))
                        cropSelData.lstSymbols.Add(node.Text.Trim().ToUpper());
                }
            }

            if (treeFields != null)
            {
                foreach (TreeNode node in treeFields.Nodes)
                {
                    CROPConditions cropConds = new CROPConditions();
                    cropConds.Condition = node.Text;
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if (subNode.Checked || node.Checked)
                        {
                            cropConds.Symbols.Add(subNode.Text.Trim().ToUpper());
                            if (!cropSelData.lstSymbols.Contains(subNode.Text.Trim().ToUpper()))
                                cropSelData.lstSymbols.Add(subNode.Text.Trim().ToUpper());
                        }
                    }
                    if (cropConds.Symbols.Count > 0)
                        cropSelData.dctCROPCondInfo.Add(node.Text.Trim().ToUpper(), cropConds);

                }
            }
            return cropSelData;
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
                Field = Field.Substring(0, Field.LastIndexOf("\n(%"));
                object val = excl.Value;
                if (!CondMapping.ContainsKey(Field))
                {
                    string query = $"UPDATE CROPPROGRESS_DIALY_DATA SET  VALUE = {val} FROM CROPPROGRESS_DIALY_DATA CD INNER JOIN CROPPROGRESS_SYMBOL_INFO CS ON CD.MAPPINGID = CS.MAPPINGID WHERE CD.STATE = '{State}' AND CD.WEEKENDING = '{Date.ToString("yyyy-MM-dd")}' AND CS.SYMBOL = '{Symbol}' AND CS.FIELD ='{Field}'  AND CS.ISCondition = 0";
                    cropRepository.UpdateCROPData(query);
                }
                else
                {
                    Field = CondMapping[Field];
                    string query = $"UPDATE CROPPROGRESS_COND_DIALY_DATA SET  { Field } = { val } FROM CROPPROGRESS_COND_DIALY_DATA CD INNER JOIN CROPPROGRESS_CONDITIONS CC ON CD.MAPPINGID = CC.MAPPINGID INNER JOIN CROPPROGRESS_SYMBOL_INFO CS ON CS.MAPPINGID = CC.MAPPINGID AND CD.MAPPINGID = CS.MAPPINGID WHERE CD.STATE = '{State}' AND CD.WEEKENDING = '{Date.ToString("yyyy-MM-dd")}' AND CS.SYMBOL = '{Symbol}' AND CS.ISCondition = 1";
                    cropRepository.UpdateCROPData(query);
                }
            }
        }
        public static void GetData(string query, string datequery)
        {
            
        }
        public static void InitConfigData(TreeView treeGroups, TreeView treeFields)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            if (treeFields != null)
            {
                treeFields.Nodes.Clear();
                treeFields.CheckBoxes = true;
            }

            Dictionary<string, CROPSymbolFieldInfo> dictCropSymbolFieldInfo = new Dictionary<string, CROPSymbolFieldInfo>();
            Dictionary<string, CROPConditions> dictCropConditions = new Dictionary<string, CROPConditions>();
            cropRepository.PopulateGroupAndSymbolInfo(dictCropSymbolFieldInfo, dictCropConditions);

            foreach (KeyValuePair<string,CROPSymbolFieldInfo> kv in dictCropSymbolFieldInfo)
            {
                TreeNode node = treeGroups.Nodes.Add(textInfo.ToTitleCase(kv.Key.ToLower()));
                foreach ( string field in kv.Value.Fields)
                {

                    node.Nodes.Add(textInfo.ToTitleCase(field.ToLower()));
                }
                if (kv.Value.Conditions.Count > 0)
                {
                    node = node.Nodes.Add("Condition");
                    foreach (string cond in kv.Value.Conditions)
                    {
                        node.Nodes.Add(textInfo.ToTitleCase(cond.ToLower()));
                    }
                }
            }

            if (treeFields != null)
            {
                foreach (KeyValuePair<string, CROPConditions> kv in dictCropConditions)
                {
                    TreeNode node = treeFields.Nodes.Add(textInfo.ToTitleCase(kv.Key.ToLower()));
                    foreach (string sym in kv.Value.Symbols)
                    {
                        node.Nodes.Add(textInfo.ToTitleCase(sym.ToLower()));
                    }
                }
            }
        }
    }
}
