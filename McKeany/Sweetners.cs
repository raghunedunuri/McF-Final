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
    public partial class Sweetners : Form
    {
        public Sweetners()
        {
            InitializeComponent();
            SweetnerCommon.InitConfigData(treeGroups, cmbDataSource);
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField,DataFeedType.Yearly,cmbFiscal, ChkAutoUpdate);
            cmbRollUp.Enabled = false;
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }
        public void ShowData(UIData uiData)
        {
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, SweetnerCommon.DataFeedFrequency, cmbFiscal, ChkMatrixFormat, ChkAutoUpdate);
            cmbDataSource.SelectedIndex = uiData.SelectedSource;
            uiData.ShowData(treeGroups, null);
            Show();
        }
        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, null);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.PeriodType = "Value";
                excelQuery.StoredProc = SweetnerCommon.StoredProc;
                excelQuery.Fields = uiData.GetSelectedFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, SweetnerCommon.DataFeedFrequency);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();

            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), false, cmbRollUp.SelectedItem.ToString(),
                cmdField.SelectedItem.ToString(), "SWEETNER", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);
            uiData.SelectedSource = cmbDataSource.SelectedIndex;
            PresentData(uiData, false);
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
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, ChkMatrixFormat, SweetnerCommon.DataFeedFrequency);
        }

        private void cmbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            SweetnerCommon.DisplayGroups(treeGroups, cmbDataSource.SelectedItem.ToString());
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, 
                SweetnerCommon.DataFeedFrequency, cmbFiscal);
            ChkMatrixFormat.Enabled = false;
        }
    }
}
