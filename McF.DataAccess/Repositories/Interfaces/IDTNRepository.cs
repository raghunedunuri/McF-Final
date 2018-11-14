using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface IDTNRepository : IBaseDataRepository
    {
        DTNJobInfo GetJobInfo();
        List<DTNFields> GetTempFieldInfo();
        void UpdateDTNData(List<DTNUpdateInfo> dtnUpdateInfo);
        void UpdateDTNData(string query);
        DTNRawInfo GetDTNNRawData();
        DSFormatedData GetDTNFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        List<DTNData> GetDTNLastUpdatedData();
        DataSet GetDTNData(string query);
        void PopulateDTNData(List<DTNData> lstDTNData);
        DataSet GetDTNConfInfo();
        List<DTNSymbols> GetDTNSymbolInfo();
        DataSet GetSugarReportData();
        
        

    }
}
