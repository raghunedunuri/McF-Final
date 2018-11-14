using McF.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace McKeany
{
    public partial class BrolierChiken : Form
    {
        public BrolierChiken()
        {
            InitializeComponent();
            BHCommon.InitConfigData(treeGroups);
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, cmbFiscal, ChkAutoUpdate);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }

        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, null);
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, cmbFiscal, checkMatrixFormat, ChkAutoUpdate);
            Show();
        }
        public void PresentData(UIData uiData, bool bShow = false)
        {
            if (bShow)
                uiData.ShowData(treeGroups, null);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_BH_DATA_EXCEL";
                excelQuery.Fields = uiData.GetSelectedFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Weekly);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();
            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), 
                checkMatrixFormat.Checked, cmbRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "BC", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);
            PresentData(uiData);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkCommodity_CheckedChanged(object sender, EventArgs e)
        {
            DataCommon.CheckNodes(treeGroups, chkCommodity.Checked);
        }
        
        private void TreeGroups_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                DataCommon.CheckNodes(e.Node, e.Node.Checked);
            }
        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, checkMatrixFormat, DataFeedType.Weekly);
        }
    }
}
