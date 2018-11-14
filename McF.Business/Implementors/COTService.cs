using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public class COTService : ICOTService
    {
        private ICOTRepository COTRepos = null;
        public COTService(ICOTRepository cOTRepos)
        {
            this.COTRepos = cOTRepos;
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {

        }

        public List<FieldInfo> GetFieldInfo()
        {
            return COTRepos.GetFieldInfo();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return COTRepos.GetSymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            return COTRepos.GetRawData();
        }

        public List<COTData> GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return COTRepos.GetCOTFormatedData(Index, From, To);
        }

        public List<COTData> GetLastUpdatedData()
        {
            return COTRepos.GetLastUpdatedData();
        }

        public void UpdateData(UpdateSingleInfo UpdateInfo)
        {
           // DateTime dt = DateTime.ParseExact(UpdateInfo.Date, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            string query = $"Update COT_DIALY_DATA set {UpdateInfo.Field} = {UpdateInfo.Value} where Market_and_Exchange_Names = '{UpdateInfo.Symbol}' and Report_Date_as_MM_DD_YYYY = '{UpdateInfo.Date.ToString("yyyy-MM-dd")}'";
            COTRepos.UpdateCOTData(query);
        }
        public void PopulateData(List<COTData> lstCOTData)
        {
            COTRepos.PopulateData(lstCOTData);
        }

        public void PopulateData(DataSet cotDataSet)
        {
            COTRepos.PopulateData(cotDataSet);
        }
    }
}
