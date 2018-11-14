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
using Unity.Lifetime;
using Excel = Microsoft.Office.Interop.Excel;


namespace McKeany
{
    internal static class DTNCommon
    {
        public static DataSet DTNConfigInfo { get; set;  }
        private static IDTNRepository dtnRepository;
        private static ICommonRepository commonRepo;

        static DTNCommon()
        {
            dtnRepository = UnityResolver._unityContainer.Resolve<DTNRepository>();
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }

        private static string GetColType( DataTable dt, string columnName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Fields"].ToString().ToUpper().Trim() == columnName.ToUpper().Trim())
                    return dr["DATA_TYPE"].ToString();
            }
            return String.Empty;
        }

        public static void SaveData(Excel.Worksheet currentWorksheet, List<Excel.Range> lstRanges)
        {
            foreach(Excel.Range excl in lstRanges )
            {
                Excel.Range xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 1], currentWorksheet.Cells[excl.Row, 1]];
                var Date = xlRange.Value;
                DateTime dt = DateTime.ParseExact(Date, "dd/MM/yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture);
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[excl.Row, 2], currentWorksheet.Cells[excl.Row, 2]];
                var Symbol = xlRange.Value;
                xlRange = currentWorksheet.Range[currentWorksheet.Cells[3, excl.Column], currentWorksheet.Cells[3, excl.Column]];
                var Field = xlRange.Value;
                string dType = GetColType(DTNConfigInfo.Tables[4], Field);
                object val = excl.Value;
                switch(dType.ToUpper().Trim())
                {
                    case "DECIMAL":
                    case "BIGINT":
                    case "INT":
                        break;
                    default:
                        val = $"'{excl.Value}'";
                        break;
                }
                string query = $"Update DTN_DIALY_DATA set {Field} = {val} where IssueDescription = '{Symbol}' and DATE = '{dt.ToString("MM/dd/yyyy hh:mm:ss.fff")}'";
                dtnRepository.UpdateDTNData(query);
            }
        }
    
        private static void  AddGroups(DataTable dt, string ColumnName, TreeView tv, bool bAsChildNode)
        {
            TreeNode node = null;
            if (bAsChildNode)
            {
                node = tv.Nodes.Add(ColumnName);
            }
            
            foreach (DataRow dr in dt.Rows)
            {
                string value = dr[ColumnName.Replace(" ",String.Empty)].ToString();
                if (!String.IsNullOrEmpty(value))
                {
                    if (node != null)
                        node.Nodes.Add(dr[ColumnName.Replace(" ", String.Empty)].ToString());
                    else
                        tv.Nodes.Add(dr[ColumnName.Replace(" ", String.Empty)].ToString());
                }
            }
        }

        private static void AddSymbols(DataTable dt, TreeView tv, bool bcc = false)
        {
            string RootSymbol = String.Empty;
            TreeNode SymbolsNode = tv.Nodes.Add("Symbols"); 
            TreeNode node = null;
            foreach (DataRow dr in dt.Rows)
            {
                string symbol = dr["SYMBOL"].ToString();
                if (bcc)
                {
                    if (symbol[symbol.Length - 2] != '@' || (symbol[symbol.Length - 1] != 'C' || symbol[symbol.Length - 1] == 'c' ))
                    {
                        continue;
                    }
                }
                else if (!bcc)
                {
                    if (symbol[symbol.Length - 2] == '@' && (symbol[symbol.Length - 1] == 'c' || symbol[symbol.Length - 1] == 'C'))
                    {
                        continue;
                    }
                }
                string currentRootSymbol = $"{dr["ROOTSYMBOLDESC"].ToString()} :-: {dr["ROOTSYMBOL"].ToString()}";
                string currentSymbol = $"{dr["IssueDescription"].ToString()} :-: {dr["SYMBOL"].ToString()}";

                if (currentRootSymbol != RootSymbol)
                {
                    node = SymbolsNode.Nodes.Add(currentRootSymbol);
                    RootSymbol = currentRootSymbol;
                }
                node.Nodes.Add(currentSymbol);
            }
        }

        private static void AddCCSymbols(DataTable dt, TreeView tv, bool bcc = true)
        {
            string RootSymbol = String.Empty;
            TreeNode SymbolsNode = tv.Nodes.Add("Symbols");
            TreeNode node = null;
            foreach (DataRow dr in dt.Rows)
            {
                string symbol = dr["SYMBOL"].ToString();
                if (bcc)
                {
                    if (symbol[symbol.Length - 2] != '@' || (symbol[symbol.Length - 1] != 'C' || symbol[symbol.Length - 1] == 'c'))
                    {
                        continue;
                    }
                }
                else if (!bcc)
                {
                    if (symbol[symbol.Length - 2] == '@' && (symbol[symbol.Length - 1] == 'c' || symbol[symbol.Length - 1] == 'C'))
                    {
                        continue;
                    }
                }
                string currentSymbol = $"{dr["IssueDescription"].ToString()} :-: {dr["SYMBOL"].ToString()}";

                SymbolsNode.Nodes.Add(currentSymbol);
            }
        }
        private static void GetSelectedFilters(TreeNode parentNode, string rootName, List<string> symbols,bool bCC)
        {
            foreach (TreeNode subNode in parentNode.Nodes)
            {
                string query = String.Empty;
                if (subNode.Checked)
                {
                    query += $"'{subNode.Text}',";
                }
                if( !String.IsNullOrEmpty(query))
                {
                    string dataQuery = $"SELECT DISTINCT DD.Symbol FROM DTN_DIALY_DATA DD INNER JOIN DTN_SYMBOL_INFO DS ON DD.RootSymbol = DS.DTNRoot WHERE DS.{rootName.Replace(" ",String.Empty)} IN ({query.Trim(',')})";
                    DataSet ds = commonRepo.ProcessDataQuery(dataQuery);
                    foreach(DataRow dr in ds.Tables[0].Rows)
                    {
                        string symbol = dr["Symbol"].ToString();

                        if (bCC)
                        {
                            if (symbol[symbol.Length - 2] != '@' || (symbol[symbol.Length - 1] != 'C' || symbol[symbol.Length - 1] == 'c'))
                            {
                                continue;
                            }
                        }
                        else if (symbol[symbol.Length - 2] == '@' && (symbol[symbol.Length - 1] == 'c' || symbol[symbol.Length - 1] == 'C'))
                            {
                                continue;
                        }
                        if ( !symbols.Contains(symbol))
                            symbols.Add(symbol);
                    }
                    //comm
                }
            }
        }
        public static string GetDateQuery(string dateQuery)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = dateQuery.Split(strArray, StringSplitOptions.None);
            return DataOperations.GetDateQuery(Convert.ToInt32(filters[0]), filters[1], filters[2], "[DATE]");
        }
        
        private static string GetSymbol( string sym )
        {   
                   
           return sym.Substring(sym.IndexOf(":-:") + 3, sym.Length - (sym.IndexOf(":-:") + 3)).Trim();
        }
        private static void GetSelectedSymbols(TreeNode parentNode, List<string> symbols)
        {
            foreach (TreeNode subNode in parentNode.Nodes)
            {
                if (subNode.Nodes != null && subNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in subNode.Nodes)
                    {
                        if (node.Checked)
                        {
                            symbols.Add(GetSymbol(node.Text));
                        }
                    }
                }
                else if(subNode.Checked)
                {
                    symbols.Add(GetSymbol(subNode.Text));
                }
            }
        }
        private static string GetSelectedFields(TreeView treeField)
        {
            string Fields = String.Empty;
            foreach (TreeNode node in treeField.Nodes)
            {
                if (node.Checked)
                {
                    Fields += $"DD.[{node.Text}],";
                }
            }
            if (!String.IsNullOrEmpty(Fields))
                return Fields.Substring(0, Fields.Length - 1);

            return Fields;
        }
        public static List<string> GetSelectedQuery (TreeView treeGroups, bool bCC = false)
        {
            List<string> lstSymbols = new List<string>();

            if (bCC)
            {
                foreach (TreeNode node in treeGroups.Nodes)
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if(subNode.Checked)
                        {
                            lstSymbols.Add(GetSymbol(subNode.Text));
                        }
                    }
                }
            }
            else
            {
                foreach (TreeNode node in treeGroups.Nodes)
                {
                    GetSelectedSymbols(node, lstSymbols);
                }
            }
            return lstSymbols;
       }

        public static List<string> GetSelectedCCQuery(TreeView treeGroups, bool bCC = false)
        {
            List<string> lstSymbols = new List<string>();

            foreach (TreeNode node in treeGroups.Nodes)
            {
                //  if (node.Text == "Symbols")
                //{
                GetSelectedSymbols(node, lstSymbols);
                // }
                //else
                // {
                //   GetSelectedFilters(node, node.Text, lstSymbols, bCC);
                //}
            }
            return lstSymbols;
        }
        public static string MergeTimeQuery (string query, string datequery)
        {
            string updatePrefix = " and ";

            if (!query.Contains("WHERE"))
                updatePrefix = " WHERE ";

            query += $" {updatePrefix} {datequery}";

            query += " Order by [Date]";
            return query;
        }

       // public static void 
        public static void PresentData(Excel.Worksheet currentWorksheet, string Query)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            DataSet ds = dtnRepository.GetDTNData(Query);

            if (ds != null)
            {
                int startrow = 3;
                int column = 3;

                currentWorksheet.Cells[startrow, 1] = "DATE";
                currentWorksheet.Cells[startrow, 2] = "SYMDESC";

                foreach (DataColumn dcol in ds.Tables[0].Columns)
                {
                    if (dcol.ColumnName.ToUpper() == "DATE" || dcol.ColumnName.ToUpper() == "SYMBOL" || dcol.ColumnName.ToUpper() == "SYMDESC")
                        continue;

                    currentWorksheet.Cells[startrow, column++] = dcol.ColumnName;
                }
                startrow++;

                Dictionary<string, Dictionary<string, string>> dccol = new Dictionary<string, Dictionary<string, string>>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    currentWorksheet.Cells[startrow, 1] = ((DateTime)dr["DATE"]).ToString("dd/MM/yyyy");
                    currentWorksheet.Cells[startrow, 2] = dr["SYMDESC"].ToString();
                    column = 3;
                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        if (dcol.ColumnName.ToUpper() == "DATE" || dcol.ColumnName.ToUpper() == "SYMBOL" || dcol.ColumnName.ToUpper() == "SYMDESC")
                            continue;
                        currentWorksheet.Cells[startrow, column++] = dr[dcol.ColumnName].ToString();
                    }
                    startrow++;
                }
            }
            else
                currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, UIData uiData)
        {
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            string Query = String.Empty;
            DataSet ds = dtnRepository.GetDTNData(Query);

            if (ds != null)
            {
                int startrow = 3;
                int column = 3;

                currentWorksheet.Cells[startrow, 1] = "DATE";
                currentWorksheet.Cells[startrow, 2] = "SYMDESC";

                foreach (DataColumn dcol in ds.Tables[0].Columns)
                {
                    if (dcol.ColumnName.ToUpper() == "DATE" || dcol.ColumnName.ToUpper() == "SYMBOL" || dcol.ColumnName.ToUpper() == "SYMDESC")
                        continue;

                    currentWorksheet.Cells[startrow, column++] = dcol.ColumnName;
                }
                startrow++;

                Dictionary<string, Dictionary<string, string>> dccol = new Dictionary<string, Dictionary<string, string>>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    currentWorksheet.Cells[startrow, 1] = ((DateTime)dr["DATE"]).ToString("dd/MM/yyyy");
                    currentWorksheet.Cells[startrow, 2] = dr["SYMDESC"].ToString();
                    column = 3;
                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        if (dcol.ColumnName.ToUpper() == "DATE" || dcol.ColumnName.ToUpper() == "SYMBOL" || dcol.ColumnName.ToUpper() == "SYMDESC")
                            continue;
                        currentWorksheet.Cells[startrow, column++] = dr[dcol.ColumnName].ToString();
                    }
                    startrow++;
                }
            }
            else
                currentWorksheet.Cells[3, 1] = "THERE IS NO DATA TO REPORT";
        }
        public static void InitConfigData( TreeView treeGroups, TreeView treeFields )
        {
            treeGroups.Nodes.Clear();
            treeFields.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            treeFields.CheckBoxes = true;

            DTNConfigInfo = dtnRepository.GetDTNConfInfo();

            //AddSymbols(DTNConfigInfo.Tables[3], treeGroups);
            //AddGroups(DTNConfigInfo.Tables[0], "Root Group", treeGroups, true);
            //AddGroups(DTNConfigInfo.Tables[1], "Sub Group", treeGroups, true);
            AddGroups(DTNConfigInfo.Tables[1], "Fields", treeFields, false);
            Dictionary<string, Dictionary<string, List<string>>> Symbols = new Dictionary<string, Dictionary<string, List<string>>>();
            Dictionary<string, List<string>> MostlyUsed = new Dictionary<string, List<string>>();

            foreach (DataRow dr in DTNConfigInfo.Tables[0].Rows)
            {
                string SubGroup = dr["SubGroup"].ToString();
                string Symbol = dr["Symbol"].ToString();
                string IssueDescription = dr["IssueDescription"].ToString();
                string RootSymbol = dr["RootSymbol"].ToString();
                string RootDesc = dr["ROOTSYMBOLDESC"].ToString();

                if (Symbol[Symbol.Length - 2] == '@' && (Symbol[Symbol.Length - 1] == 'c' || Symbol[Symbol.Length - 1] == 'C'))
                {
                    continue;
                }

                string currentRootSymbol = $"{RootDesc} :-: {RootSymbol}";
                string currentSymbol = $"{IssueDescription} :-: {Symbol}";

                if( !Symbols.ContainsKey(SubGroup))
                {
                    Symbols[SubGroup] = new Dictionary<string, List<string>>();
                }
                if( !Symbols[SubGroup].ContainsKey(currentRootSymbol))
                {
                    Symbols[SubGroup][currentRootSymbol] = new List<string>();
                }
                Symbols[SubGroup][currentRootSymbol].Add(currentSymbol);

                if (dr["MostlyUsed"].ToString() == "Y")
                {
                    if (!MostlyUsed.ContainsKey(currentRootSymbol))
                        MostlyUsed[currentRootSymbol] = new List<string>();
                    MostlyUsed[currentRootSymbol].Add(currentSymbol);
                }
            }
            TreeNode node1 = treeGroups.Nodes.Add("Favorites");
            foreach (KeyValuePair<string, List<string>> kv in MostlyUsed)
            {
                TreeNode node2 = node1.Nodes.Add(kv.Key);
                foreach(string str in kv.Value)
                {
                    node2.Nodes.Add(str);
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Symbols)
            {
                TreeNode node = treeGroups.Nodes.Add(kv.Key);
                foreach (KeyValuePair<string, List<string>> mv in kv.Value)
                {
                    node1 = node.Nodes.Add(mv.Key);
                    foreach (string str in mv.Value)
                    {
                        node1.Nodes.Add(str);
                    }
                }
            }
        }

        public static void InitConfigCCData(TreeView treeGroups, TreeView treeFields)
        {
            treeGroups.Nodes.Clear();
            treeFields.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            treeFields.CheckBoxes = true;

            DTNConfigInfo = dtnRepository.GetDTNConfInfo();

            AddGroups(DTNConfigInfo.Tables[1], "Fields", treeFields, false);
            Dictionary<string, Dictionary<string, List<string>>> Symbols = new Dictionary<string, Dictionary<string, List<string>>>();
            Dictionary<string, List<string>> MostlyUsed = new Dictionary<string, List<string>>();

            foreach (DataRow dr in DTNConfigInfo.Tables[0].Rows)
            {
                string SubGroup = dr["SubGroup"].ToString();
                string Symbol = dr["Symbol"].ToString();
                string IssueDescription = dr["IssueDescription"].ToString();
                string RootSymbol = dr["RootSymbol"].ToString();
                string RootDesc = dr["ROOTSYMBOLDESC"].ToString();

                if (!(Symbol[Symbol.Length - 2] == '@' && (Symbol[Symbol.Length - 1] == 'c' || Symbol[Symbol.Length - 1] == 'C')))
                {
                    continue;
                }

                string currentRootSymbol = $"{RootDesc} :-: {RootSymbol}";
                string currentSymbol = $"{RootDesc} :-: {Symbol}";

                if (!Symbols.ContainsKey(SubGroup))
                {
                    Symbols[SubGroup] = new Dictionary<string, List<string>>();
                }
                if (!Symbols[SubGroup].ContainsKey(currentRootSymbol))
                {
                    Symbols[SubGroup][currentRootSymbol] = new List<string>();
                }
                Symbols[SubGroup][currentRootSymbol].Add(currentSymbol);

                if (dr["MostlyUsed"].ToString() == "Y")
                {
                    if (!MostlyUsed.ContainsKey(currentRootSymbol))
                        MostlyUsed[currentRootSymbol] = new List<string>();
                    MostlyUsed[currentRootSymbol].Add(currentSymbol);
                }
            }
            TreeNode node1 = treeGroups.Nodes.Add("Favorites");
            foreach (KeyValuePair<string, List<string>> kv in MostlyUsed)
            {
             //   TreeNode node2 = node1.Nodes.Add(kv.Key);
                foreach (string str in kv.Value)
                {
                    node1.Nodes.Add(str);
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kv in Symbols)
            {
                TreeNode node = treeGroups.Nodes.Add(kv.Key);
                foreach (KeyValuePair<string, List<string>> mv in kv.Value)
                {
                 //   node1 = node.Nodes.Add(mv.Key);
                    foreach (string str in mv.Value)
                    {
                        node.Nodes.Add(str);
                    }
                }
            }
        }
    }
}
;