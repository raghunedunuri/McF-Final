using McF.Contracts;
using System;
using System.Windows.Forms;

namespace McKeany
{
    public partial class DTNCC : Form
    {
        public DTNCC()
        {
            InitializeComponent();
            DTNCommon.InitConfigCCData(treeGroups, treeFields);

            DataCommon.InitializeDateFilters(dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Dialy, cmbFiscal, ChkAutoUpdate);
            treeGroups.AfterCheck += TreeGroups_AfterCheck;
        }

        public void ShowData(UIData uiData)
        {
            uiData.ShowData(treeGroups, treeFields);
            DataCommon.RePopulateFilters(uiData, dtPickerStartTime, dtPickerEndtime, cmbRange, cmbRollUp, cmdField, DataFeedType.Dialy, cmbFiscal, ChkMatrixFormat, ChkAutoUpdate);
            Show();
        }
       private void TreeGroups_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                    if (e.Node.Checked)
                        e.Node.Expand();
                }
            }
        }

        private void TreeFields_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                     this.CheckAllChildNodes(e.Node, e.Node.Checked);
                    if (e.Node.Checked)
                        e.Node.Expand();
                }
            }
        }
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        public void PresentData(UIData uiData, bool bShow = true)
        {
            if (bShow)
                uiData.ShowData(treeGroups, treeFields);

            ExcelQuery excelQuery = uiData.GetExcelQuery();
            if (excelQuery != null)
            {
                excelQuery.StoredProc = "McF_GET_DTN_DATA_EXCEL";
                excelQuery.Symbols = DTNCommon.GetSelectedQuery(treeGroups, true);
                excelQuery.Fields = uiData.GetSelectedFromFields();
                excelQuery.FeedType = DataCommon.GetDataFeedType(cmdField, DataFeedType.Dialy);
                DataCommon.InitializeForDataLoad(excelQuery, uiData);
            }
        }
        private void ShowDTNData()
        {
            this.Close();
            
            UIData uiData = new UIData();
            uiData.UpdateUIData(treeGroups, treeFields, dtPickerStartTime.Value.ToShortDateString(), dtPickerEndtime.Value.ToShortDateString(), 
                cmbRange.SelectedIndex,(cmbRollUp.SelectedIndex > 0 & cmdField.SelectedIndex > 0), ChkMatrixFormat.Checked, 
                cmbRollUp.SelectedItem.ToString(), cmdField.SelectedItem.ToString(), "DTNCC", cmbFiscal.SelectedIndex, ChkAutoUpdate.Checked);
            PresentData(uiData, false);
        }
                
        private void btnRun_Click(object sender, EventArgs e)
        {
            ShowDTNData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkCategories_CheckedChanged(object sender, EventArgs e)
        {
            DataCommon.CheckNodes(treeGroups, chkCategories.Checked);
        }

        private void chkFeilds_CheckedChanged(object sender, EventArgs e)
        {
            DataCommon.CheckNodes(treeFields, chkFeilds.Checked);
        }

        private void cmdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataCommon.FrequencyChanged(cmdField, cmbFiscal, ChkMatrixFormat, DataFeedType.Dialy);
        }
    }
}
