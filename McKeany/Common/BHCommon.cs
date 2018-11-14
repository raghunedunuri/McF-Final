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
    internal static class BHCommon
    {
        private static ICommonRepository commonRepo;
        private static DataSet BHConfigInfo;
        static BHCommon()
        {
            commonRepo = UnityResolver._unityContainer.Resolve<CommonRepository>();
        }
        public static void InitConfigData(TreeView treeFields)
        {
            treeFields.Nodes.Clear();
            treeFields.CheckBoxes = true;

            BHConfigInfo = commonRepo.ExecuteDataSetFromSP("McF_GET_BH_CONFIG");
            foreach(DataRow dr in BHConfigInfo.Tables[0].Rows)
            {
                treeFields.Nodes.Add(dr["DisplayName"].ToString());
            }
        }
    }
}
