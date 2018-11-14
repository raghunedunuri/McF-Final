using McF.Contracts;
using McF.DataAccess;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Unity;

namespace McKeany
{
    internal static class SugarCommon
    {
        private static DataSet SugarConfigData { get; set; }
        public static DataFeedType DataFeedFrequency { get; set; }
        public static string StoredProc { get; set; }
        public static int SelSweetnerType { get; set; }
        private static ICommonRepository commonRepo;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        static SugarCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }
        public static void DisplayGroups(TreeView treeGroups, string type)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            DataRow[] dr = SugarConfigData.Tables[0].Select($"SugarType='{type}'");
            string table = String.Empty;
            if (dr != null && dr.Length > 0)
            {
                table = dr[0]["Table"].ToString();
                StoredProc = dr[0]["SPNAME"].ToString();
                string Frequency = dr[0]["FREQUENCY"].ToString();
                if (Frequency == "YEARLY")
                    DataFeedFrequency = DataFeedType.Yearly;
                else if (Frequency == "MONTHLY")
                    DataFeedFrequency = DataFeedType.Monthly;
            }
            dr = SugarConfigData.Tables[1].Select($"STable='{table}'");
            if (dr != null && dr.Length > 0)
            {
                foreach (DataRow dr1 in dr)
                {
                    string field = dr1["DisplayName"].ToString();
                    treeGroups.Nodes.Add(field);
                }
            }
        }
        public static void InitConfigData(TreeView treeGroups, ComboBox cmbDataSource)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            
            SugarConfigData = commonRepo.ExecuteDataSetFromSP("[McF_GET_SUGAR_CONFIG]");

            int index = 0;
            foreach (DataRow dr in SugarConfigData.Tables[0].Rows)
            {
                string field = dr["SugarType"].ToString();
                cmbDataSource.Items.Add(new ComboItem(field, index++));
            }
            cmbDataSource.SelectedIndex = 0;
        }
    }
}
