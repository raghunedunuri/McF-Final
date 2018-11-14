using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Net.Http;

namespace McKeany
{
    public partial class NewTabRi
    {
        private void NewTabRi_Load(object sender, RibbonUIEventArgs e)
        {
           
        }

        public bool ValidateLoginUser()
        {
            if (ThisAddIn.UserInfo == null)
            {
                UserData us = new UserData();
                us.Show();
                return false;
            }
            return true;
        }
        private void Ethanol_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                Ethanol dia = new Ethanol();
                dia.Show();
            }
        }

        private void USWeekly_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                USWeekly dia = new USWeekly();
                dia.Show();
            }
        }

        private void Cocoa_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                Cocoa dia = new Cocoa();
                dia.Show();
            }
        }

        private void COT_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                COT dia = new COT();
                dia.Show();
            }
        }

        private void Sugar_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                Sugar dia = new Sugar();
                dia.Show();
            }
        }

        private void WasdeWorld_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                WASDEWorld dia = new WASDEWorld();
                dia.Show();
            }
        }

        private void WasdeDomestic_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                WASDEDomestic dia = new WASDEDomestic();
                dia.Show();
            }
        }

        private void btnDTN_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                DTN dia = new DTN();
                dia.Show();
            }
        }

        private void ChickenEggs_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                ChickenAndEggs dia = new ChickenAndEggs();
                dia.Show();
            }
        }

        private void HGPIG_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                HogPigs dia = new HogPigs();
                dia.Show();
            }
        }

        private void BROHAT_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                BrolierChiken dia = new BrolierChiken();
                dia.Show();
            }
        }

        private void CatFeed_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                CattleOnFeed dia = new CattleOnFeed();
                dia.Show();
            }
        }

        private void Crop_click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                CropProgress dia = new CropProgress();
                dia.Show();
            }
        }

        private void FatsOnOils_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                FatsOils dia = new FatsOils();
                dia.Show();
            }
        }

        private void btnCC_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                DTNCC dtncc = new DTNCC();
                dtncc.Show();
            }
        }

        private void btnUserTables_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                DataTables dt = new DataTables();
                dt.Show();
            }
        }

        private void btnSweetner_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                Sweetners sw = new Sweetners();
                sw.Show();
            }
        }

        private void btnCocoaSD_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                CocoaSD cs = new CocoaSD();
                cs.Show();
            }
        }

        private void btnPhysicalComm_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                PhysicalComm pc = new PhysicalComm();
                pc.Show();
            }
        }

        private void btnCorn_Click(object sender, RibbonControlEventArgs e)
        {
            Corn cr = new Corn();
            cr.Show();
        }

        private void btnAddTable_Click(object sender, RibbonControlEventArgs e)
        {
            if (ValidateLoginUser())
            {
                AddTable ad = new AddTable();
                ad.Show();
            }
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {

        }
    }
}
