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
    internal static class USWeeklyCommon
    {
        private static IUSWeeklyRepository usWeeklyRepository { get; set; }

        static USWeeklyCommon()
        {
            usWeeklyRepository = UnityResolver._unityContainer.Resolve<USWeeklyRepository>();
        }
        
        public static void InitConfigData( TreeView treeGroups, TreeView treeFields )
        {
            treeGroups.Nodes.Clear();
            treeFields.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            treeFields.CheckBoxes = true;

            DataSet USWeeklyConfigInfo = usWeeklyRepository.GetUSweeklyConfigData();

            string prevComm = string.Empty;

            List<string> commSymbols = new List<string>();
            foreach( DataRow dr in USWeeklyConfigInfo.Tables[0].Rows)
            {
                string currentCom = dr["Commodity_Name"].ToString();
                string currSym = dr["MappingSymbol"].ToString();
               
                if (string.IsNullOrEmpty(prevComm))
                {
                    prevComm = currentCom;
                }
                else if (prevComm != currentCom )
                {
                    if (commSymbols.Count > 1)
                    {
                        TreeNode node = treeGroups.Nodes.Add(prevComm);
                        foreach( string str in commSymbols)
                        {
                            node.Nodes.Add(str);
                        }
                    }
                    else
                    {
                        foreach (string str in commSymbols)
                        {
                            treeGroups.Nodes.Add(str);
                        }
                    }
                    commSymbols.Clear();
                    prevComm = currentCom;
                }
                commSymbols.Add(currSym);
            }
            if(commSymbols.Count > 0)
            {
                TreeNode node = treeGroups.Nodes.Add(prevComm);
                foreach (string str in commSymbols)
                {
                    node.Nodes.Add(str);
                }
            }

            foreach (DataRow dr in USWeeklyConfigInfo.Tables[1].Rows)
            {
                treeFields.Nodes.Add(dr["DisplayName"].ToString());
            }
        }
    }
}
