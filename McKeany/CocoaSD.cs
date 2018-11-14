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
    public partial class CocoaSD : Form
    {
        public CocoaSD()
        {
            InitializeComponent();
            CocoaSDCommon.InitConfigData(treeGroups, null);

            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, comboRollUp, cmdField, DataFeedType.Yearly, cmbFiscal,  ChkAutoUpdate);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }

        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, null);
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, comboRollUp, cmdField, DataFeedType.Yearly, cmbFiscal, ChkMatrixFormat, ChkAutoUpdate);
            Show();
        }

        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, null);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_COCOA_ANNUAL_GRIND_EXCEL";
                excelQuery.Symbols = new List<string>(); ;
                excelQuery.Fields = uiData.GetSymbolsAndFieldsFromGroup(excelQuery.Symbols);
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Yearly);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();
          
            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,(comboRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), ChkMatrixFormat.Checked,
                comboRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "COCOASD", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);
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

        private void TreeFields_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                DataCommon.CheckNodes(e.Node, e.Node.Checked);
            }
        }
        
      
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, ChkMatrixFormat, DataFeedType.Yearly);
        }
    }
}
