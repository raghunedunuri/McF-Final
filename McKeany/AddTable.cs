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
using MSForms = Microsoft.Vbe.Interop.Forms;
using McF.Contracts;
using Microsoft.VisualBasic.CompilerServices;

namespace McKeany
{
    public partial class AddTable : Form
    {
        private static string TableName;
        private static string DataSource;

        private Excel.Shape AddButton;
        private MSForms.CommandButton CmdAddBtn;
       
        public AddTable()
        {
            InitializeComponent();
            
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if( String.IsNullOrEmpty(txtDataSource.Text) || String.IsNullOrEmpty(txtTable.Text))
            {
                MessageBox.Show(" Need to give DataSource and TableName");
                return;
            }

            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange.Clear();

            currentWorksheet.Shapes.SelectAll();
            Globals.ThisAddIn.Application.Selection.Delete();

            DataCommon.bPresent = true;
            currentWorksheet.Cells[1, 1] = txtTable.Text;
            // currentWorksheet.Cells[2, 1] = "Table should start from Row 4";
            currentWorksheet.Cells[4, 1] = "Field1";
            currentWorksheet.Cells[4, 2] = "Field2";
            currentWorksheet.Cells[4, 3] = "Field3";
            currentWorksheet.Cells[4, 4] = "Field4";
            currentWorksheet.Cells[4, 5] = "Field5";
            currentWorksheet.Cells[5, 1] = "int,decimal,string,date";
            currentWorksheet.Cells[5, 2] = "int,decimal,string,date";
            currentWorksheet.Cells[5, 3] = "int,decimal,string,date";
            currentWorksheet.Cells[5, 4] = "int,decimal,string,date";
            currentWorksheet.Cells[5, 5] = "int,decimal,string,date";
            currentWorksheet.Cells[6, 1] = "unit1";
            currentWorksheet.Cells[6, 2] = "unit2";
            currentWorksheet.Cells[6, 3] = "unit3";
            currentWorksheet.Cells[6, 4] = "unit4";
            currentWorksheet.Cells[6, 5] = "unit5";
            currentWorksheet.Cells[7, 1] = "value1";
            currentWorksheet.Cells[7, 2] = "value2";
            currentWorksheet.Cells[7, 3] = "value3";
            currentWorksheet.Cells[7, 4] = "value4";
            currentWorksheet.Cells[7, 5] = "value5";

            DataSource = txtDataSource.Text;
            TableName = txtTable.Text;
            xlRange = currentWorksheet.Range[currentWorksheet.Cells[1, 5], currentWorksheet.Cells[2, 6]];
            AddButton = (Excel.Shape)currentWorksheet.Shapes.AddOLEObject("Forms.CommandButton.1", Type.Missing, false, false, Type.Missing, Type.Missing, Type.Missing, xlRange.Left, xlRange.Top, xlRange.Width, xlRange.Height);

            AddButton.Name = "btnClick";
            Excel.Application xlApp = (Excel.Application)Globals.ThisAddIn.Application;
            CmdAddBtn = (MSForms.CommandButton)NewLateBinding.LateGet((Excel.Worksheet)xlApp.ActiveSheet, null, "btnClick", new object[0], null, null, null);
            CmdAddBtn.FontSize = 10;
            CmdAddBtn.FontBold = true;
            CmdAddBtn.Caption = "Add Data";
            CmdAddBtn.Click += new Microsoft.Vbe.Interop.Forms.CommandButtonEvents_ClickEventHandler(CmdBtn_Click);
            this.Close();
        }

      

        private void CmdBtn_Click()
        {
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            Excel.Range dataRange = currentWorksheet.Range[currentWorksheet.Cells[4, 1], currentWorksheet.Cells[xlRange.Rows.Count, xlRange.Columns.Count]];
            DataSet ds = new DataSet();
            DataTable table = ds.Tables.Add(TableName);

            if (rowCount <= 4)
            {
                MessageBox.Show("No data to Create Table");
                return;
            }

            string strQuery = String.Empty;
            int rowIndex = 1;
            Dictionary<int, DataCol> dc = new Dictionary<int, DataCol>();

            int columnCount = 1;
            foreach (Excel.Range row in dataRange.Rows)
            {
                object[] data = new object[row.Columns.Count];
                int columnIndex = 0;
                columnCount = 1;
                foreach (Excel.Range col in dataRange.Columns)
                {
                    Excel.Range valRange = currentWorksheet.Range[currentWorksheet.Cells[row.Row, col.Column], currentWorksheet.Cells[row.Row, col.Column]];
                    var val = valRange.Value;

                    if (rowIndex == 1)
                    {
                        dc[columnCount++] = new DataCol(val);

                    }
                    else if (rowIndex == 2)
                    {
                        dc[columnCount++].Type = val;
                    }
                    else if (rowIndex == 3)
                    {
                        dc[columnCount++].Unit = val;
                    }
                    else
                    {
                        data[columnIndex++] = val;
                    }
                }
                if(rowIndex == 3)
                {
                    foreach (KeyValuePair<int, DataCol> kv in dc)
                    {
                        switch(kv.Value.Type.ToUpper())
                        {
                            case "INT":
                                table.Columns.Add(kv.Value.Column, typeof(Int32));
                                break;
                            case "DECIMAL":
                                table.Columns.Add(kv.Value.Column, typeof(double));
                                break;
                            case "STRING":
                                table.Columns.Add(kv.Value.Column, typeof(String));
                                break;
                            case "DATE":
                                table.Columns.Add(kv.Value.Column, typeof(DateTime));
                                break;
                            default:
                                table.Columns.Add(kv.Value.Column, typeof(String));
                                break;
                        }
                    }
                }
                if (rowIndex > 3)
                    table.Rows.Add(data);
                rowIndex++;
            }

            Excel.Range datarange = currentWorksheet.Range[currentWorksheet.Cells[1, 1], currentWorksheet.Cells[1,1]];
            DataCommon.BulkInsertTable(TableName, DataSource, table, dc);

            AddButton.Delete();
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataTableGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
