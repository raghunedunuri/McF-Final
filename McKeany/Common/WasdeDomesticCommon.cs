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
    public class WASDEDomesticSelectedData
    {
        public Dictionary<string, Dictionary<string, bool>> SelectedData { get; set; }
        public bool IncludeRegionalData { get; set; }
    }

    internal static class WASDEDomesticCommon
    {
        private static DataSet WASDEDomesticConfigData { get; set; }
        private static ICommonRepository commonRepo;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        static WASDEDomesticCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, WASDEDomesticSelectedData selectedData, string Filters)
        {
            string dateFilter = String.Empty;
            string[] strArray = { ":-:" };
            string[] filters = Filters.Split(strArray, StringSplitOptions.None);
            PresentData(currentWorksheet, selectedData, Convert.ToInt32(filters[0]), filters[1], filters[2]);
        }

        public static void PresentData(Excel.Worksheet currentWorksheet, WASDEDomesticSelectedData selectedData, int index, string From, string To, bool bProj = true)
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
            DSFormatedData dsFormated = commonRepo.GetWasdeDomesticFormatedData(lstSymbols, index, dFrom, dTo);
            int column = 1;

            if (dsFormated != null)
            {
                foreach (KeyValuePair<string, FormatedData> kv in dsFormated.DictFormatedData)
                {
                    if (!bProj && kv.Value.CommodityData.Name.ToUpper().Contains("PROJECT"))
                        continue;
                    currentWorksheet.Cells[startrow, column++] = textInfo.ToTitleCase(kv.Value.CommodityData.Name.ToLower());
                    currentWorksheet.Cells[startrow, column] = !String.IsNullOrEmpty(kv.Value.CommodityData.Unit) ? $"\n({kv.Value.CommodityData.Unit})" : String.Empty;
                    column = 1;
                    startrow++;
                    foreach (HeaderFields headerFields in kv.Value.Headers)
                    {
                        currentWorksheet.Cells[startrow, column] = headerFields.Name;
                        if(!String.IsNullOrEmpty(headerFields.Unit))
                        currentWorksheet.Cells[startrow+1, column] = headerFields.Unit;
                        column++;
                    }
                    startrow+=2;
                    column = 1;
                    foreach (List<string> data in kv.Value.Values)
                    {
                        foreach (string val in data)
                        {
                            currentWorksheet.Cells[startrow, column++] = val;
                        }
                        startrow++;
                        column = 1;
                    }
                    startrow++;
                    column = 1;
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
        public static WASDEDomesticSelectedData GetSelectedQuery(TreeView treeGroups)
        {
            WASDEDomesticSelectedData foSelectData = new WASDEDomesticSelectedData();
            foSelectData.SelectedData = new Dictionary<string, Dictionary<string, bool>>();
            foreach (TreeNode node in treeGroups.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    foreach (TreeNode subNode in node.Nodes)
                    {
                        if (subNode.Checked)
                        {
                            if(!foSelectData.SelectedData.ContainsKey(node.Text.Trim()))
                                foSelectData.SelectedData[node.Text.Trim()] = new Dictionary<string, bool>();
                            foSelectData.SelectedData[node.Text.Trim()].Add(subNode.Text.Trim(), true);
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

            WASDEDomesticConfigData = commonRepo.ExecuteDataSetFromSP("McF_GET_WASDEDOMESTIC_GROUPDATA");

            TreeNode childNode = null;
            foreach (DataRow dr in WASDEDomesticConfigData.Tables[0].Rows)
            {
                string field = dr["DisplayName"].ToString();
                string commodity = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                string category = dr["Unit"].ToString();

                if (!dictCategories.ContainsKey(commodity))
                {
                    dictCategories.Add(commodity, new List<string>());
                    childNode = treeGroups.Nodes.Add(commodity);
                }
                childNode.Nodes.Add(field);
            }
        }
    }
}
