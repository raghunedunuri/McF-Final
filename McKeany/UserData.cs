using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace McKeany
{
    public partial class UserData : Form
    {
        public UserData()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DataCommon.GetUser(txtUserName.Text, txtPassword.Text);
            if( ThisAddIn.UserInfo == null )
            {
                MessageBox.Show("Invalid Username and Password");
            }
            else
            {
                string Data = JsonConvert.SerializeObject(ThisAddIn.UserInfo);
                string Folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string ConfigFile = $@"{Folder}\McKeany.Config";
                File.WriteAllText(ConfigFile, Data);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
