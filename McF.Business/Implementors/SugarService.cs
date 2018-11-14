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
    public class SugarService: ISugarService
    {
        private ISugarRepository SugarRepos = null;
        //private static DataSet ColTypes = null;
        public SugarService(ISugarRepository sugarRepos)
        {
            this.SugarRepos = sugarRepos;
        }

        public List<SugarCompaniesData> GetSugarCompaniesData()
        {
            return SugarRepos.GetSugarCompaniesData();
        }
        public List<SugarCompaniesData> GetCornWetMillsData()
        {
            return SugarRepos.GetCornWetMillsData();
        }
        public List<int> GetYears()
        {
            return SugarRepos.GetYears();
        }
        public DataSet GetHFSCExports()
        {
            return SugarRepos.GetHFSCExports();
        }
        public DataSet GetSugarDelivaries()
        {
            return SugarRepos.GetSugarDelivaries();
        }
        public DataSet GetUSMarketData()
        {
            return SugarRepos.GetUSMarketData();
        }
        public DataSet GetSugarPrices()
        {
            return SugarRepos.GetSugarPrices();
        }
        public DataSet GetHFCSDemand()
        {
            return SugarRepos.GetHFCSDemand();
        }
        public List<NorthWetMills> GetNorthWetMillings()
        {
            return SugarRepos.GetNorthWetMillings();
        }
        public DataSet GetMexicanSugarReportData()
        {
            return SugarRepos.GetMexicanSugarReportData();
        }
    }
}
