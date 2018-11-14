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
    public partial class Ethanol : Form
    {
        private static Excel.DocEvents_ChangeEventHandler EventDel_CellsChange;
        public Ethanol()
        {
            InitializeComponent();
            EthanolCommon.InitConfigData(treeGroups, treeFields);

            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, cmbFiscal, ChkAutoUpdate);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
            treeFields.AfterCheck += TreeFields_AfterCheck;
        }

        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, treeFields);
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Weekly, cmbFiscal, ChkMatrixFormat, ChkAutoUpdate);
            Show();
        }

        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, treeFields);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_ETHANOL_DATA_EXCEL";
                excelQuery.Symbols =  uiData.GetSelectedFields();
                excelQuery.Fields = uiData.GetSelectedFromFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Weekly);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();
          
            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, treeFields, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0),
                ChkMatrixFormat.Checked, cmbRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "ETHANOL", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);

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

        private void TreeFields_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                DataCommon.CheckNodes(e.Node, e.Node.Checked);
            }
        }
        
        private void chkCondition_CheckedChanged(object sender, EventArgs e)
        {
            DataCommon.CheckNodes(treeFields, chkCondition.Checked);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, ChkMatrixFormat, DataFeedType.Weekly);
        }
    }
}
