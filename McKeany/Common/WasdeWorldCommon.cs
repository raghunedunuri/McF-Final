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
    internal static class WasdeWorldCommon
    {
        private static DataSet WASDEDomesticConfigData { get; set; }
        private static ICommonRepository commonRepo;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        static WasdeWorldCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }

        public static void InitConfigData(TreeView treeGroups)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;

            Dictionary<string, List<string>> dictCategories = new Dictionary<string, List<string>>();

            WASDEDomesticConfigData = commonRepo.ExecuteDataSetFromSP("McF_GET_WASDEWORLD_GROUPDATA");

            TreeNode childNode = null;
            foreach (DataRow dr in WASDEDomesticConfigData.Tables[0].Rows)
            {
                string field = dr["DisplayName"].ToString();
                string commodity = textInfo.ToTitleCase(dr["Commodity_Name"].ToString().ToLower());
                string unit = dr["Unit"].ToString();

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
