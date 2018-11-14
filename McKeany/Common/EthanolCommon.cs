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
    internal static class EthanolCommon
    {
        public static Dictionary<string,string> SymbolMapping { get; set; }
        private static IEthanolRepository ethanolRepository;

        static EthanolCommon()
        {
            SymbolMapping = new Dictionary<string, string>();
            ethanolRepository = UnityResolver._unityContainer.Resolve<EthanolRepository>();
        }
        
        public static void InitConfigData( TreeView treeGroups, TreeView treeFields )
        {
            treeGroups.Nodes.Clear();
            treeFields.Nodes.Clear();
            treeGroups.CheckBoxes = true;
            treeFields.CheckBoxes = true;

            DataSet EthanolConfigInfo = ethanolRepository.GetEthanolConfigData();

            foreach( DataRow dr in EthanolConfigInfo.Tables[0].Rows)
            {
                string MappingSymbol = dr["MappingSymbol"].ToString();
                string Symbol = dr["Symbol"].ToString();
                treeGroups.Nodes.Add(MappingSymbol);
                SymbolMapping[MappingSymbol] = Symbol;
            }

            foreach (DataRow dr in EthanolConfigInfo.Tables[1].Rows)
            {
                 treeFields.Nodes.Add(dr["DisplayName"].ToString());
            }
        }

        public static List<string> GetSelectedSymbols(TreeView treeGroups)
        {
            List<string> symbols = new List<string>();
            foreach (TreeNode n1 in treeGroups.Nodes)
            {
                if (n1.Checked)
                {
                    symbols.Add(n1.Text);
                }
            }
            return symbols;
        }
    }
}
