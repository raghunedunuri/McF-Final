using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface ISugarRepository
    {
        List<SugarCompaniesData> GetSugarCompaniesData();
        List<SugarCompaniesData> GetCornWetMillsData();
        List<int> GetYears();
        DataSet GetHFSCExports();
        DataSet GetSugarDelivaries();
        DataSet GetUSMarketData();
        DataSet GetSugarPrices();
        DataSet GetHFCSDemand();
        List<NorthWetMills> GetNorthWetMillings();
        DataSet GetMexicanSugarReportData();
    }
}
