using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using MSForms = Microsoft.Vbe.Interop.Forms;
using Microsoft.VisualBasic.CompilerServices;
using System.Data;
using Newtonsoft.Json;
using McF.Contracts;
using System.Windows.Forms;
using System.IO;

namespace McKeany
{
    public class UserLoginData
    {
        public String UserName { get; set; }
        public int UserId { get; set; }
    }
    public partial class ThisAddIn
    {
        private Excel.Shape RefreshButton;
        private Excel.Shape SaveButton;
        private Excel.Shape LoadButton;
        private MSForms.CommandButton CmdBtn;
        private MSForms.CommandButton CmdSaveBtn;
        private MSForms.CommandButton CmdLoadBtn;

        private Dictionary<string, Dictionary<Excel.Range, ExcelFieldData>> dictCoordinates;
        public static Dictionary<string, Dictionary<string, ExcelFieldData>> DictDisplayUpdates = new Dictionary<string, Dictionary<string, ExcelFieldData>>();

        public static UserLoginData UserInfo = null;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            UnityResolver.Init();
            dictCoordinates = new Dictionary<string, Dictionary<Excel.Range, ExcelFieldData>>();

            string Folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string ConfigFile = $@"{Folder}\McKeany.Config";
            if ( File.Exists(ConfigFile))
            {
                try
                {
                    UserInfo = (UserLoginData)JsonConvert.DeserializeObject<UserLoginData>(File.ReadAllText(ConfigFile));
                }
                catch(Exception ex)
                {

                }
            }
        }

        public static void GlobalWorksheet_Change(Excel.Range Target)
        {
            try
            {
                Globals.ThisAddIn.Application.EnableEvents = false;
                if (DataCommon.bPresent)
                    return;

                ExcelFieldData excelFieldData = ValidateForUpdate(Target.Row, Target.Column);
                if (excelFieldData != null )
                {
                    if (excelFieldData.ReadOnly)
                    {
                        Target.Value = excelFieldData.DataValue;
                        MessageBox.Show("Update functionality is not supported for this Field or DisplayFormat");
                    }
                    else
                    {
                        Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
                        Globals.ThisAddIn.SaveChanges(Target, excelFieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        public static string GetKey(int row, int column)
        {
            return $"ROW:{row};COLUMN:{column}";
        }

        public void PopulateUpdatedData(int row, int column, ExcelFieldData excelFieldData)
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            if (!DictDisplayUpdates.ContainsKey(currentWorksheet.Name))
                DictDisplayUpdates[currentWorksheet.Name] = new Dictionary<string, ExcelFieldData>();
            DictDisplayUpdates[currentWorksheet.Name][GetKey(row, column)] = excelFieldData;
        }

        public static ExcelFieldData ValidateForUpdate(int row, int column)
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            if (DictDisplayUpdates.ContainsKey(currentWorksheet.Name))
            {
                string Key = GetKey(row, column);
                if(DictDisplayUpdates[currentWorksheet.Name].ContainsKey(Key))
                {
                    return DictDisplayUpdates[currentWorksheet.Name][Key];
                }
            }
            return null;
        }
       
      
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            ////Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            ////Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;

            ////currentWorksheet.Shapes.SelectAll();
            ////Globals.ThisAddIn.Application.Selection.Delete();
	    }

        public void ThisAddIn_WorkSheetActive(object Sh)
        {
            UpdateData();
        }

        private void ThisAddIn_WorkbookActive(object Sh)
        {
            UpdateData();
        }

        public void SaveChanges( Excel.Range range, ExcelFieldData excelFiedlData )
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;

            if(!dictCoordinates.ContainsKey(currentWorksheet.Name))
            {
                dictCoordinates.Add(currentWorksheet.Name, new Dictionary<Excel.Range, ExcelFieldData>());
            }
            dictCoordinates[currentWorksheet.Name].Add(range, excelFiedlData);
        }

        public static void ClearData( )
        {
            DictDisplayUpdates.Clear();
        }

        private void UpdateData(bool bUpdate = true)
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            bool bAddControls = false;
            Excel.Range slRange = currentWorksheet.Range[currentWorksheet.Cells[1, 500], currentWorksheet.Cells[1, 500]];
            string query = slRange.Value;
            if (!String.IsNullOrEmpty(query))
            {
                if (bUpdate)
                {
                    currentWorksheet.Shapes.SelectAll();
                    //Globals.ThisAddIn.Application.Selection.Delete();
                }

                UIData uiData = (UIData)JsonConvert.DeserializeObject<UIData>(slRange.Value);
                if( uiData.IsAutoUpdate )
                    uiData.EndDate = DateTime.Now.ToShortDateString();

                switch (uiData.DataSource)
                {
                    case "BC":
                        BrolierChiken dia = new BrolierChiken();
                        dia.PresentData(uiData);
                        break;
                    case "CF":
                        CattleOnFeed cf = new CattleOnFeed();
                        cf.PresentData(uiData);
                        break;
                    case "CE":
                        ChickenAndEggs ce = new ChickenAndEggs();
                        ce.PresentData(uiData);
                        break;
                    case "COCOA":
                        Cocoa cocoa = new Cocoa();
                        cocoa.PresentData(uiData);
                        break;
                    case "COCOASD":
                        CocoaSD cocoasa = new CocoaSD();
                        cocoasa.PresentData(uiData);
                        break;
                    case "COT":
                        COT cot = new COT();
                        cot.PresentData(uiData);
                        break;
                    case "CROP":
                        CropProgress cp = new CropProgress();
                        cp.PresentData(uiData);
                        break;
                    case "DT":
                        DataTables dtTables = new DataTables();
                        dtTables.PresentData(uiData);
                        break;
                    case "DTN":
                        DTN dtn = new DTN();
                        dtn.PresentData(uiData);
                        break;
                    case "DTNCC":
                        DTNCC dtnCC = new DTNCC();
                        dtnCC.PresentData(uiData);
                        break;
                    case "ETHANOL":
                        Ethanol eth = new Ethanol();
                        eth.PresentData(uiData);
                        break;
                    case "FATSOILS":
                        FatsOils ftOil = new FatsOils();
                        ftOil.PresentData(uiData);
                        break;
                    case "HP":
                        HogPigs hpig = new HogPigs();
                        hpig.PresentData(uiData);
                        break;
                    case "SUGAR":
                        Sugar sugar = new Sugar();
                        sugar.PresentData(uiData);
                        break;
                    case "USWEEKLY":
                        USWeekly uw = new USWeekly();
                        uw.PresentData(uiData);
                        break;
                    case "WD":
                        WASDEDomestic wd = new WASDEDomestic();
                        wd.PresentData(uiData);
                        break;
                    case "WW":
                        WASDEWorld ww = new WASDEWorld();
                        ww.PresentData(uiData);
                        break;
                    case "SWEETNER":
                        Sweetners st = new Sweetners();
                        st.PresentData(uiData);
                        break;
                }
                currentWorksheet.Cells[1, 500] = query;
                bAddControls = true;
            }
            if (bAddControls)
            {
                if (bUpdate)
                {
                    //AddControls();
                }
            }
            Globals.ThisAddIn.Application.EnableEvents = true;
        }

        private void CmdBtn_Click()
        {
            UpdateData(false);
        }
        private void SaveBtn_Click()
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            if (dictCoordinates.ContainsKey(currentWorksheet.Name))
            {
                foreach( KeyValuePair<Excel.Range, ExcelFieldData> kv in dictCoordinates[currentWorksheet.Name])
                {
                    var val = kv.Key.Value;
                    DataCommon.ExcecuteQuery(kv.Value, val.ToString());
                }
                MessageBox.Show("Data Saved Successfully");
            }
            dictCoordinates.Clear();
            Globals.ThisAddIn.Application.EnableEvents = true;
        }
        private void LoadBtn_Click()
        {
            Globals.ThisAddIn.Application.EnableEvents = false;
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
          
            Excel.Range slRange = currentWorksheet.Range[currentWorksheet.Cells[1, 500], currentWorksheet.Cells[1, 500]];
            UIData uiData = (UIData)JsonConvert.DeserializeObject<UIData>(slRange.Value);
            switch(uiData.DataSource)
            {
                case "BC":
                    BrolierChiken dia = new BrolierChiken();
                    dia.ShowData(uiData);
                    break;
                case "CF":
                    CattleOnFeed cf = new CattleOnFeed();
                    cf.ShowData(uiData);
                    break;
                case "CE":
                    ChickenAndEggs ce = new ChickenAndEggs();
                    ce.ShowData(uiData);
                    break;
                case "COCOA":
                    Cocoa cocoa = new Cocoa();
                    cocoa.ShowData(uiData);
                    break;
                case "COCOASD":
                    CocoaSD cocoasa = new CocoaSD();
                    cocoasa.ShowData(uiData);
                    break;
                case "COT":
                    COT cot = new COT();
                    cot.ShowData(uiData);
                    break;
                case "CROP":
                    CropProgress cp = new CropProgress();
                    cp.ShowData(uiData);
                    break;
                case "DT":
                    DataTables dtTables = new DataTables();
                    dtTables.ShowData(uiData);
                    break;
                case "DTN":
                    DTN dtn = new DTN();
                    dtn.ShowData(uiData);
                    break;
                case "DTNCC":
                    DTNCC dtnCC = new DTNCC();
                    dtnCC.ShowData(uiData);
                    break;  
                case "ETHANOL":
                    Ethanol eth = new Ethanol();
                    eth.ShowData(uiData);
                    break;
                case "FATSOILS":
                    FatsOils ftOil = new FatsOils();
                    ftOil.ShowData(uiData);
                    break;
                case "HP":
                    HogPigs hpig = new HogPigs();
                    hpig.ShowData(uiData);
                    break;
                case "SUGAR":
                    Sugar sugar = new Sugar();
                    sugar.ShowData(uiData);
                    break;
                case "USWEEKLY":
                    USWeekly uw = new USWeekly();
                    uw.ShowData(uiData);
                    break;
                case "WD":
                    WASDEDomestic wd = new WASDEDomestic();
                    wd.ShowData(uiData);
                    break;
                case "WW":
                    WASDEWorld ww = new WASDEWorld();
                    ww.ShowData(uiData);
                    break;
                case "SWEETNER":
                    Sweetners st = new Sweetners();
                    st.ShowData(uiData);
                    break;
            }
            Globals.ThisAddIn.Application.EnableEvents = true;
        }
        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
            this.Application.WorkbookActivate += new Excel.AppEvents_WorkbookActivateEventHandler(
            ThisAddIn_WorkbookActive);
            this.Application.SheetActivate += new Excel.AppEvents_SheetActivateEventHandler(
            ThisAddIn_WorkSheetActive);
        }

        public void AddControls()
        {
            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Worksheet currentWorksheet = oWorkbook.ActiveSheet;
            Excel.Range xlRange = currentWorksheet.UsedRange;
            xlRange = currentWorksheet.Range[currentWorksheet.Cells[1, 3], currentWorksheet.Cells[2, 4]];
            RefreshButton = (Excel.Shape)currentWorksheet.Shapes.AddOLEObject("Forms.CommandButton.1", Type.Missing, false, false, Type.Missing, Type.Missing, Type.Missing, xlRange.Left, xlRange.Top, xlRange.Width, xlRange.Height);

            RefreshButton.Name = "btnClick";
            Excel.Application xlApp = (Excel.Application)Globals.ThisAddIn.Application;
            CmdBtn = (MSForms.CommandButton)NewLateBinding.LateGet((Excel.Worksheet)xlApp.ActiveSheet, null, "btnClick", new object[0], null, null, null);
            CmdBtn.FontSize = 10;
            CmdBtn.FontBold = true;
            CmdBtn.Caption = "Refresh Data";
            CmdBtn.Click += new Microsoft.Vbe.Interop.Forms.CommandButtonEvents_ClickEventHandler(CmdBtn_Click);

            xlRange = currentWorksheet.Range[currentWorksheet.Cells[1, 7], currentWorksheet.Cells[2, 9]];
            SaveButton = (Excel.Shape)currentWorksheet.Shapes.AddOLEObject("Forms.CommandButton.1", Type.Missing, false, false, Type.Missing, Type.Missing, Type.Missing, xlRange.Left, xlRange.Top, xlRange.Width, xlRange.Height);

            SaveButton.Name = "btnSaveClick";
            CmdSaveBtn = (MSForms.CommandButton)NewLateBinding.LateGet((Excel.Worksheet)xlApp.ActiveSheet, null, "btnSaveClick", new object[0], null, null, null);
            CmdSaveBtn.FontSize = 10;
            CmdSaveBtn.FontBold = true;
            CmdSaveBtn.Caption = "Save Data";
            CmdSaveBtn.Click += new Microsoft.Vbe.Interop.Forms.CommandButtonEvents_ClickEventHandler(SaveBtn_Click);

            xlRange = currentWorksheet.Range[currentWorksheet.Cells[1, 12], currentWorksheet.Cells[2, 14]];
            LoadButton = (Excel.Shape)currentWorksheet.Shapes.AddOLEObject("Forms.CommandButton.1", Type.Missing, false, false, Type.Missing, Type.Missing, Type.Missing, xlRange.Left, xlRange.Top, xlRange.Width, xlRange.Height);

            LoadButton.Name = "btnLoadClick";
            CmdLoadBtn = (MSForms.CommandButton)NewLateBinding.LateGet((Excel.Worksheet)xlApp.ActiveSheet, null, "btnLoadClick", new object[0], null, null, null);
            CmdLoadBtn.FontSize = 10;
            CmdLoadBtn.FontBold = true;
            CmdLoadBtn.Caption = "Edit Query";
            CmdLoadBtn.Click += new Microsoft.Vbe.Interop.Forms.CommandButtonEvents_ClickEventHandler(LoadBtn_Click);
        }
        #endregion
    }
}
