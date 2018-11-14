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
    public partial class Sugar : Form
    {
        public Sugar()
        {
            InitializeComponent();
            SugarCommon.InitConfigData(treeGroups,cmbDataSource);
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Monthly, cmbFiscal, ChkAutoUpdate);
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
                excelQuery.StoredProc = SugarCommon.StoredProc;
                excelQuery.Fields = uiData.GetSelectedFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, SugarCommon.DataFeedFrequency);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Globals.ThisAddIn.Application.EnableEvents = false;

            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), 
                ChkMatrixFormat.Checked, cmbRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "SUGAR", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);

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

        private void cmbRollUp_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, ChkMatrixFormat, SugarCommon.DataFeedFrequency);
        }

        private void cmbDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            SugarCommon.DisplayGroups(treeGroups, cmbDataSource.SelectedItem.ToString());
            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField,
                SugarCommon.DataFeedFrequency, cmbFiscal);
        }

    }
}
