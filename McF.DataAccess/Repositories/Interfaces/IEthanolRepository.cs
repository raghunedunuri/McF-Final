using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface IEthanolRepository: IBaseDataRepository
    {
        DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        List<EthanolData> GetLastUpdatedData();
        DataSet GetEthanolUpdatedData(string query);
        void PopulateData(List<EthanolData> lstEthanolData);
        void UpdateEthanolData(string query);
        DataSet GetEthanolConfigData();
        EthanolRawData GetEthanolRawData();
    }
}
