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
    public partial class DataTables : Form
    {
        private static Excel.DocEvents_ChangeEventHandler EventDel_CellsChange;
        public DataTables()
        {
            InitializeComponent();

            DataTablesCommon.InitConfigData(treeGroups, cmbDataTables);

            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, null, null );

            //UIData uiData = new UIData();
            //treeGroups.
            //uiData.SelectedGroups = JsonConvert.SerializeObject(treeGroups);
            //string data = JsonConvert.SerializeObject(uiData);
            //uiData.SelectedFields = tre
            //treeGroups = JsonConvert.DeserializeObject<TreeView>(uiData.SelectedGroups);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }
        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, null);
            dtPickerStartTime.Text = uiData.StartDate;
            dtPickerEndtime.Text = uiData.EndDate;
            cmbRange.SelectedIndex = uiData.DateRangeIndex;
          //  cmbRollUp.SelectedText = uiData.RollUpFrequency;
           // cmdField.SelectedText = uiData.RollUpValue;
           // ChkRollUP.Checked = uiData.IsRollUp;
            
            Show();
        }
        public void PresentData(UIData uiData)
        {
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();
            Globals.ThisAddIn.Application.EnableEvents = false;

            string data = JsonConvert.SerializeObject(uiData);
            uiData.ShowData(treeGroups, null);
            string dateCond = $"{uiData.DateRangeIndex}:-:{ uiData.StartDate}:-:{uiData.EndDate}";

            string SymQuery = DataTablesCommon.GetSelectedQuery(treeGroups, uiData.IsRollUp, uiData.RollUpFrequency, uiData.RollUpValue, uiData.DataTable);
            if (String.IsNullOrEmpty(SymQuery))
                return;

            //string EndQuery = DataTablesCommon.MergeTimeQuery(SymQuery, DateQuery, uiData.IsRollUp);
            DataTablesCommon.PresentData(currentWorksheet, SymQuery, uiData.IsRollUp, false);

            currentWorksheet.Cells[1, 500] = JsonConvert.SerializeObject(uiData); ;

            DataCommon.EnableEvents(currentWorksheet);

            EventDel_CellsChange = new Excel.DocEvents_ChangeEventHandler(ThisAddIn.GlobalWorksheet_Change);
            currentWorksheet.Change += EventDel_CellsChange;
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Close();
         
            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, null, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), cmbRange.SelectedIndex, false, false,"0", "0", "DT");
            uiData.DataTable = cmbDataTables.SelectedItem.ToString();
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

        private void cmbDataTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTablesCommon.UpdateFields(treeGroups, cmbDataTables);
        }
    }
}
