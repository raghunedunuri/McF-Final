using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public interface ICommonService
    {
        DSFormatedData GetFormatedData(int index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetWASDEWorldFormatedData(int index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetWASDEDomesticFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetFOFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetCocoaFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        bool SaveData(DSUpdatedData dsUpdatedData);
        DSFormatedData GetBHFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetCFFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        void PopulateUser(UserLoginData userLoginData);
        bool ValidateUser(string user, string passWord);
        List<DataSources> GetAllDataSources();
        DSFormatedData GetCompleteFormatData(string DataSource, string RootSource, int Index = 0, DateTime? From = null, DateTime? To = null);
        bool SaveDataSource(DataSource datasource);
        bool ResetPassword(string user, string passWord, string newPassWord);
    }
}
