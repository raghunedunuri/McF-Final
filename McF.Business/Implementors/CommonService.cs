using McF.Contracts;
using McF.DataAccess;
using McF.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public class CommonService : ICommonService
    {
        private ICommonRepository commRepos = null;
        public CommonService(ICommonRepository commRepos)
        {
            this.commRepos = commRepos;
        }
        //Graph
        public DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetFormatedData(Index, From, To);
        }
        public DSFormatedData GetWASDEWorldFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetWasdeWorldFormatedData(Index, From, To);
        }
        public DSFormatedData GetWASDEDomesticFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetWasdeDomesticFormatedData(Index, From, To);
        }

        public DSFormatedData GetFOFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetFOFormatedData(Index, From, To);
        }

        public DSFormatedData GetCocoaFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetCocoaFormatedData(Index, From, To);
        }

        public DSFormatedData GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetCOTFormatedData(Index, From, To);
        }

        public bool SaveData(DSUpdatedData dsUpdatedData)
        {
            return commRepos.SaveData(dsUpdatedData);
        }
        public DSFormatedData GetBHFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetBHFormatedData(Index, From, To);
        }

        public DSFormatedData GetCFFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetCFFormatedData(Index, From, To);
        }

        public void PopulateUser(UserLoginData userLoginData)
        {
            commRepos.PopulateUser(userLoginData);
        }

        public bool ValidateUser(string user, string passWord)
        {
            return commRepos.ValidateUser(user, passWord);
        }

        public List<DataSources> GetAllDataSources()
        {
            return commRepos.GetAllDataSources();
        }

        public DSFormatedData GetCompleteFormatData(string DataSource, string RootSource, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return commRepos.GetCompleteFormatData(DataSource, RootSource, Index, From, To);
        }
        public bool SaveDataSource(DataSource datasource)
        {
            return commRepos.SaveDataSource(datasource);
        }

        public bool ResetPassword(string user, string passWord, string newPassWord)
        {
            return commRepos.ResetPassword(user, passWord, newPassWord);
        }
    }
}
