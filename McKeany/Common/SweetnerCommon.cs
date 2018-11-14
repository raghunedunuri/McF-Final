using McF.Contracts;
using McF.DataAccess;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Unity;

namespace McKeany
{
    internal static class SweetnerCommon
    {
        private static DataSet SweetnerConfigData { get; set; }
        public static DataFeedType DataFeedFrequency { get; set; }
        public static string StoredProc { get; set; }
        public static int SelSweetnerType { get; set; }

        private static ICommonRepository commonRepo;
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        static SweetnerCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }
        
        public static void DisplayGroups(TreeView treeGroups, string type)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            DataRow[] dr = SweetnerConfigData.Tables[0].Select($"SweetenerType='{type}'");
            string table = String.Empty;
            if (dr != null && dr.Length > 0)
            {
                table = dr[0]["Table"].ToString();
                StoredProc = dr[0]["SPNAME"].ToString();
                string Frequency = dr[0]["FREQUENCY"].ToString();
                if (Frequency == "YEARLY")
                    DataFeedFrequency = DataFeedType.Yearly;
                else if(Frequency == "MONTHLY")
                    DataFeedFrequency = DataFeedType.Monthly;
            }
            dr = SweetnerConfigData.Tables[1].Select($"STable='{table}'");
            if (dr != null && dr.Length > 0)
            {
                foreach (DataRow dr1 in dr)
                {
                    string field = dr1["DisplayName"].ToString();
                    treeGroups.Nodes.Add(field);
                }
            }
        }
        public static void InitConfigData(TreeView treeGroups,ComboBox cmbDataSource)
        {
            treeGroups.Nodes.Clear();
            treeGroups.CheckBoxes = true;

            SweetnerConfigData = commonRepo.ExecuteDataSetFromSP("[McF_GET_SWEETNER_CONFIG]");

            int index = 0;
            foreach (DataRow dr in SweetnerConfigData.Tables[0].Rows)
            {
                string field = dr["SweetenerType"].ToString();
                cmbDataSource.Items.Add(new ComboItem(field, index++));
            }
            cmbDataSource.SelectedIndex = 0;
        }
    }
}
