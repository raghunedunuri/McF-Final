using McF.DataAccess.Repositories.Implementors;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
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
    internal static class COTCommon
    {
        private static ICOTRepository cotRepository;
        public static Dictionary<string, string> COTGroups;
        static COTCommon()
        {
            cotRepository = UnityResolver._unityContainer.Resolve<COTRepository>();
            COTGroups = new Dictionary<string, string>();
        }

        public static void InitConfigData( TreeView treeGroups, TreeView treeFields )
        {
            treeGroups.Nodes.Clear();
            treeFields.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            treeFields.CheckBoxes = true;

            DataSet COTConfigInfo = cotRepository.GetCOTConfigData();
            COTGroups.Clear();
            Dictionary<string, List<string>> COTCategories = new Dictionary<string, List<string>>();
            List<string> MostlyUsed = new List<string>();
            foreach ( DataRow dr in COTConfigInfo.Tables[0].Rows)
            {
                string Name = dr["MarketName"].ToString();
                string DisplayName = dr["DisplayName"].ToString();
                COTGroups.Add(DisplayName, Name);
                if (COTCategories.ContainsKey(dr["Category"].ToString()))
                    COTCategories[dr["Category"].ToString()].Add(DisplayName);
                else
                {
                    COTCategories[dr["Category"].ToString()] = new List<string>();
                    COTCategories[dr["Category"].ToString()].Add(DisplayName);
                }

                if (dr["MostlyUsed"].ToString() == "Y")
                    MostlyUsed.Add(DisplayName);
            }
            TreeNode node1 = treeGroups.Nodes.Add("Favorites");
            foreach (string str in MostlyUsed)
            {
                node1.Nodes.Add(str);
            }

            foreach (KeyValuePair<string, List<string>> kv in COTCategories)
            {
                TreeNode node = treeGroups.Nodes.Add(kv.Key);
                foreach( string str in kv.Value)
                {
                    node.Nodes.Add(str);
                }
            }

            Dictionary<string, List<string>> Fields = new Dictionary<string, List<string>>();
        
            foreach (DataRow dr in COTConfigInfo.Tables[1].Rows)
            {
                string group = dr["GroupName"].ToString();

                if (!Fields.ContainsKey(group))
                    Fields[group] = new List<string>();
                Fields[group].Add(dr["DisplayName"].ToString());
            }

            foreach(KeyValuePair<string,List<string>> kv in Fields)
            {
                TreeNode node = treeFields.Nodes.Add(kv.Key);
                foreach(string str in kv.Value)
                {
                    node.Nodes.Add(str);
                }
            }
        }
    }
}
