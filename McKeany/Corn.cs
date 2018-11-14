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
    public partial class Corn : Form
    {
        private static Excel.DocEvents_ChangeEventHandler EventDel_CellsChange;
        public Corn()
        {
            InitializeComponent();
            CornCommon.InitConfigData(treeGroups);
            cmbDataSource.Items.Add(new ComboItem("Daily", 0));
            cmbDataSource.Items.Add(new ComboItem("Monthly", 0));
            cmbDataSource.SelectedIndex = 0;
            cmbDataSource.Visible = false;
           
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField,DataFeedType.Dialy, cmbStartYear);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }
        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, null);
            dtPickerStartTime.Text = uiData.StartDate;
            dtPickerEndtime.Text = uiData.EndDate;
            cmbRange.SelectedIndex = uiData.DateRangeIndex;
            cmbRollUp.SelectedText = uiData.RollUpFrequency;
            cmdField.SelectedText = uiData.RollUpValue;
            
            Show();
        }
        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, null);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_CORN_DATA_EXCEL";
                excelQuery.Fields = uiData.GetSelectedFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Weekly);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();

            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), false, cmbRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "WW");
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
            if (cmbRollUp.SelectedItem.ToString() == "Fiscal")
            {
                cmbStartYear.Visible = true;
            }
        }
    }
}
