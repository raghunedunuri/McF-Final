namespace McKeany
{
    partial class NewTabRi : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public NewTabRi()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.McKeany = this.Factory.CreateRibbonTab();
            this.Futures = this.Factory.CreateRibbonGroup();
            this.btnCC = this.Factory.CreateRibbonButton();
            this.btnDTN = this.Factory.CreateRibbonButton();
            this.btnPhysicalComm = this.Factory.CreateRibbonButton();
            this.WASDE = this.Factory.CreateRibbonGroup();
            this.WasdeDomestic = this.Factory.CreateRibbonButton();
            this.WasdeWorld = this.Factory.CreateRibbonButton();
            this.DataGroup = this.Factory.CreateRibbonGroup();
            this.CropProgress = this.Factory.CreateRibbonButton();
            this.FAOlil = this.Factory.CreateRibbonButton();
            this.Sugar = this.Factory.CreateRibbonButton();
            this.btnSweetner = this.Factory.CreateRibbonButton();
            this.grpCocoa = this.Factory.CreateRibbonGroup();
            this.Cocoa = this.Factory.CreateRibbonButton();
            this.btnCocoaSD = this.Factory.CreateRibbonButton();
            this.Livestock = this.Factory.CreateRibbonGroup();
            this.CatFeed = this.Factory.CreateRibbonButton();
            this.BROHAT = this.Factory.CreateRibbonButton();
            this.ChickenEggs = this.Factory.CreateRibbonButton();
            this.HGPIG = this.Factory.CreateRibbonButton();
            this.Others = this.Factory.CreateRibbonGroup();
            this.COT = this.Factory.CreateRibbonButton();
            this.USWeekly = this.Factory.CreateRibbonButton();
            this.Ethanol = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.btnUserTables = this.Factory.CreateRibbonButton();
            this.btnAddTable = this.Factory.CreateRibbonButton();
            this.McKeany.SuspendLayout();
            this.Futures.SuspendLayout();
            this.WASDE.SuspendLayout();
            this.DataGroup.SuspendLayout();
            this.grpCocoa.SuspendLayout();
            this.Livestock.SuspendLayout();
            this.Others.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // McKeany
            // 
            this.McKeany.Groups.Add(this.Futures);
            this.McKeany.Groups.Add(this.WASDE);
            this.McKeany.Groups.Add(this.DataGroup);
            this.McKeany.Groups.Add(this.grpCocoa);
            this.McKeany.Groups.Add(this.Livestock);
            this.McKeany.Groups.Add(this.Others);
            this.McKeany.Groups.Add(this.group1);
            this.McKeany.Label = "McKeany - Flavell";
            this.McKeany.Name = "McKeany";
            // 
            // Futures
            // 
            this.Futures.Items.Add(this.btnCC);
            this.Futures.Items.Add(this.btnDTN);
            this.Futures.Items.Add(this.btnPhysicalComm);
            this.Futures.Label = "Prices";
            this.Futures.Name = "Futures";
            // 
            // btnCC
            // 
            this.btnCC.Image = global::McKeany.Properties.Resources.Share;
            this.btnCC.Label = "Continuous Contracts";
            this.btnCC.Name = "btnCC";
            this.btnCC.ShowImage = true;
            this.btnCC.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCC_Click);
            // 
            // btnDTN
            // 
            this.btnDTN.Image = global::McKeany.Properties.Resources.Share;
            this.btnDTN.Label = "Monthly Contracts";
            this.btnDTN.Name = "btnDTN";
            this.btnDTN.ShowImage = true;
            this.btnDTN.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDTN_Click);
            // 
            // btnPhysicalComm
            // 
            this.btnPhysicalComm.Image = global::McKeany.Properties.Resources.Share;
            this.btnPhysicalComm.Label = "Physical Commodities";
            this.btnPhysicalComm.Name = "btnPhysicalComm";
            this.btnPhysicalComm.ShowImage = true;
            this.btnPhysicalComm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnPhysicalComm_Click);
            // 
            // WASDE
            // 
            this.WASDE.Items.Add(this.WasdeDomestic);
            this.WASDE.Items.Add(this.WasdeWorld);
            this.WASDE.Label = "WASDE";
            this.WASDE.Name = "WASDE";
            // 
            // WasdeDomestic
            // 
            this.WasdeDomestic.Image = global::McKeany.Properties.Resources.index;
            this.WasdeDomestic.Label = "Domestic              ";
            this.WasdeDomestic.Name = "WasdeDomestic";
            this.WasdeDomestic.ShowImage = true;
            this.WasdeDomestic.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.WasdeDomestic_Click);
            // 
            // WasdeWorld
            // 
            this.WasdeWorld.Image = global::McKeany.Properties.Resources.index;
            this.WasdeWorld.Label = "World";
            this.WasdeWorld.Name = "WasdeWorld";
            this.WasdeWorld.ShowImage = true;
            this.WasdeWorld.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.WasdeWorld_Click);
            // 
            // DataGroup
            // 
            this.DataGroup.Items.Add(this.CropProgress);
            this.DataGroup.Items.Add(this.FAOlil);
            this.DataGroup.Items.Add(this.Sugar);
            this.DataGroup.Items.Add(this.btnSweetner);
            this.DataGroup.Label = "USDA";
            this.DataGroup.Name = "DataGroup";
            // 
            // CropProgress
            // 
            this.CropProgress.Image = global::McKeany.Properties.Resources.usda;
            this.CropProgress.Label = "Crop Progress";
            this.CropProgress.Name = "CropProgress";
            this.CropProgress.ShowImage = true;
            this.CropProgress.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Crop_click);
            // 
            // FAOlil
            // 
            this.FAOlil.Image = global::McKeany.Properties.Resources.usda;
            this.FAOlil.Label = "Fats and Oils";
            this.FAOlil.Name = "FAOlil";
            this.FAOlil.ShowImage = true;
            this.FAOlil.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.FatsOnOils_Click);
            // 
            // Sugar
            // 
            this.Sugar.Image = global::McKeany.Properties.Resources.usda;
            this.Sugar.Label = "Sugar SD";
            this.Sugar.Name = "Sugar";
            this.Sugar.ShowImage = true;
            this.Sugar.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Sugar_Click);
            // 
            // btnSweetner
            // 
            this.btnSweetner.Image = global::McKeany.Properties.Resources.usda;
            this.btnSweetner.Label = "Sweeteners";
            this.btnSweetner.Name = "btnSweetner";
            this.btnSweetner.ShowImage = true;
            this.btnSweetner.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSweetner_Click);
            // 
            // grpCocoa
            // 
            this.grpCocoa.Items.Add(this.Cocoa);
            this.grpCocoa.Items.Add(this.btnCocoaSD);
            this.grpCocoa.Label = "Cocoa";
            this.grpCocoa.Name = "grpCocoa";
            // 
            // Cocoa
            // 
            this.Cocoa.Image = global::McKeany.Properties.Resources.cocoa;
            this.Cocoa.Label = "Regional Grind";
            this.Cocoa.Name = "Cocoa";
            this.Cocoa.ShowImage = true;
            this.Cocoa.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Cocoa_Click);
            // 
            // btnCocoaSD
            // 
            this.btnCocoaSD.Image = global::McKeany.Properties.Resources.cocoa;
            this.btnCocoaSD.Label = "Cocoa SD";
            this.btnCocoaSD.Name = "btnCocoaSD";
            this.btnCocoaSD.ShowImage = true;
            this.btnCocoaSD.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCocoaSD_Click);
            // 
            // Livestock
            // 
            this.Livestock.Items.Add(this.CatFeed);
            this.Livestock.Items.Add(this.BROHAT);
            this.Livestock.Items.Add(this.ChickenEggs);
            this.Livestock.Items.Add(this.HGPIG);
            this.Livestock.Label = "Livestock";
            this.Livestock.Name = "Livestock";
            // 
            // CatFeed
            // 
            this.CatFeed.Image = global::McKeany.Properties.Resources.livestock;
            this.CatFeed.Label = "Cattle on Feed";
            this.CatFeed.Name = "CatFeed";
            this.CatFeed.ShowImage = true;
            this.CatFeed.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CatFeed_Click);
            // 
            // BROHAT
            // 
            this.BROHAT.Image = global::McKeany.Properties.Resources.livestock;
            this.BROHAT.Label = "Broiler and Hatchery      ";
            this.BROHAT.Name = "BROHAT";
            this.BROHAT.ShowImage = true;
            this.BROHAT.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BROHAT_Click);
            // 
            // ChickenEggs
            // 
            this.ChickenEggs.Image = global::McKeany.Properties.Resources.livestock;
            this.ChickenEggs.Label = "Chicken and Eggs";
            this.ChickenEggs.Name = "ChickenEggs";
            this.ChickenEggs.ShowImage = true;
            this.ChickenEggs.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ChickenEggs_Click);
            // 
            // HGPIG
            // 
            this.HGPIG.Image = global::McKeany.Properties.Resources.livestock;
            this.HGPIG.Label = "Hogs and Pigs";
            this.HGPIG.Name = "HGPIG";
            this.HGPIG.ShowImage = true;
            this.HGPIG.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.HGPIG_Click);
            // 
            // Others
            // 
            this.Others.Items.Add(this.COT);
            this.Others.Items.Add(this.USWeekly);
            this.Others.Items.Add(this.Ethanol);
            this.Others.Label = "Others";
            this.Others.Name = "Others";
            // 
            // COT
            // 
            this.COT.Image = global::McKeany.Properties.Resources.Ehtanol;
            this.COT.Label = "Trader Positions";
            this.COT.Name = "COT";
            this.COT.ShowImage = true;
            this.COT.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.COT_Click);
            // 
            // USWeekly
            // 
            this.USWeekly.Image = global::McKeany.Properties.Resources.Ehtanol;
            this.USWeekly.Label = "Weekly Exports         ";
            this.USWeekly.Name = "USWeekly";
            this.USWeekly.ShowImage = true;
            this.USWeekly.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.USWeekly_Click);
            // 
            // Ethanol
            // 
            this.Ethanol.Image = global::McKeany.Properties.Resources.Ehtanol;
            this.Ethanol.Label = "Ethanol";
            this.Ethanol.Name = "Ethanol";
            this.Ethanol.ShowImage = true;
            this.Ethanol.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Ethanol_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.btnUserTables);
            this.group1.Items.Add(this.btnAddTable);
            this.group1.Label = "User Tables";
            this.group1.Name = "group1";
            // 
            // btnUserTables
            // 
            this.btnUserTables.Image = global::McKeany.Properties.Resources.user;
            this.btnUserTables.Label = "Data Table";
            this.btnUserTables.Name = "btnUserTables";
            this.btnUserTables.ShowImage = true;
            this.btnUserTables.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnUserTables_Click);
            // 
            // btnAddTable
            // 
            this.btnAddTable.Image = global::McKeany.Properties.Resources.user;
            this.btnAddTable.Label = "Add Table";
            this.btnAddTable.Name = "btnAddTable";
            this.btnAddTable.ShowImage = true;
            this.btnAddTable.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAddTable_Click);
            // 
            // NewTabRi
            // 
            this.Name = "NewTabRi";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.McKeany);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.NewTabRi_Load);
            this.McKeany.ResumeLayout(false);
            this.McKeany.PerformLayout();
            this.Futures.ResumeLayout(false);
            this.Futures.PerformLayout();
            this.WASDE.ResumeLayout(false);
            this.WASDE.PerformLayout();
            this.DataGroup.ResumeLayout(false);
            this.DataGroup.PerformLayout();
            this.grpCocoa.ResumeLayout(false);
            this.grpCocoa.PerformLayout();
            this.Livestock.ResumeLayout(false);
            this.Livestock.PerformLayout();
            this.Others.ResumeLayout(false);
            this.Others.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab McKeany;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup DataGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CropProgress;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton FAOlil;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton CatFeed;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton BROHAT;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton HGPIG;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ChickenEggs;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Futures;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDTN;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup WASDE;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton WasdeDomestic;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton WasdeWorld;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Others;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Sugar;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton COT;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Cocoa;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton USWeekly;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Ethanol;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCC;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnUserTables;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSweetner;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpCocoa;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCocoaSD;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Livestock;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPhysicalComm;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAddTable;
    }

    partial class ThisRibbonCollection
    {
        internal NewTabRi NewTabRi
        {
            get { return this.GetRibbon<NewTabRi>(); }
        }
    }
}
