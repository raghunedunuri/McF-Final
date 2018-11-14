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
using McF.Contracts;
using Newtonsoft.Json;

namespace McKeany
{
    public partial class PhysicalComm : Form
    {
        public PhysicalComm()
        {
            InitializeComponent();
            PCCommon.InitConfigData(treeGroups);

            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, null, ChkAutoUpdate);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }
        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, null);
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, null, ChkMatrixFormat, ChkAutoUpdate);

            Show();
        }
        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, null);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_PC_DATA_EXCEL";
                excelQuery.Symbols = uiData.GetSelectedSymbols();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Weekly);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();

            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,( cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), ChkMatrixFormat.Checked, cmbRollUp.SelectedItem.ToString(),
                cmdField.SelectedItem.ToString(), "PC", 0, ChkAutoUpdate.Checked);
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

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void cmbRollUp_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ChkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
