using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface ICOTRepository: IBaseDataRepository
    {
        List<COTData> GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        List<COTData> GetLastUpdatedData();
        void PopulateData(List<COTData> lstEthanolData);
        void PopulateData(DataSet cotDataSet);

        void UpdateCOTData(string query);
        DataSet GetCOTConfigData();

        DataSet GetCOTUpdatedData(string query);
    }
}
