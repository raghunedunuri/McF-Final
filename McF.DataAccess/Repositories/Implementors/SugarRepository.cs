using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Implementors
{
    public class SugarRepository: ISugarRepository
    {
        private IDbHelper dbHelper = null;
        public SugarRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public List<SugarCompaniesData> GetSugarCompaniesData()
        {
            List<SugarCompaniesData> sugarCompanies = new List<SugarCompaniesData>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Sugar_Companies", CommandType.StoredProcedure))
                {

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        sugarCompanies.Add(new SugarCompaniesData()
                        {
                            name = dr["SugarRefinary"].ToString(),
                            city = dr["City"].ToString(),
                            state_ = dr["State"].ToString(),
                            //Year = Convert.ToUInt16(dr["Year"]),
                            country = dr["Country"].ToString(),
                            lat = dr["Latitude"].ToString(),
                            lon = dr["Longitude"].ToString()
                        });
                    }
                    dr.Close();

                }

            }
            catch (Exception ex)
            {

            }
            return sugarCompanies;
        }

        public List<SugarCompaniesData> GetCornWetMillsData()
        {
            List<SugarCompaniesData> sugarCompanies = new List<SugarCompaniesData>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Corn_WetMills", CommandType.StoredProcedure))
                {

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        sugarCompanies.Add(new SugarCompaniesData()
                        {
                            category = dr["Category"].ToString(),
                            name = dr["Corn_WetMiller"].ToString(),
                            city = dr["City"].ToString(),
                            state_ = dr["State"].ToString(),
                            lat = dr["Latitude"].ToString(),
                            lon = dr["Longitude"].ToString(),
                            year = Convert.ToUInt16(dr["Year"])
                        });
                    }
                    dr.Close();

                }

            }
            catch (Exception ex)
            {

            }
            return sugarCompanies;
        }

        public List<int> GetYears()
        {
            List<int> years = new List<int>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("SELECT distinct Year FROM Sweetner_CornWetMillers", CommandType.Text))
                {

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        years.Add(Convert.ToUInt16(dr["Year"]));
                        
                    }
                    dr.Close();

                }

            }
            catch (Exception ex)
            {

            }
            return years;
        }

        public DataSet GetHFSCExports()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_HFSC_55_exports", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
        public DataSet GetSugarDelivaries()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Sugar_RegionDelData", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
        public DataSet GetUSMarketData()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Sugar_Market", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
        public DataSet GetSugarPrices()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Raw_Sugar_Prices", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
        public DataSet GetHFCSDemand()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_HFCS_Demand", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
        public List<NorthWetMills> GetNorthWetMillings()
        {
            List<NorthWetMills> wetMills = new List<NorthWetMills>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_North_American_WetMilling", CommandType.StoredProcedure))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    wetMills.Add(new NorthWetMills()
                    {
                        name = dr["Corn_WetMiller"].ToString(),
                        city = dr["City"].ToString(),
                        lat = dr["Latitude"].ToString(),
                        lon = dr["Longitude"].ToString(),
                        status = dr["Status"].ToString()
                       
                    });
                }
                dr.Close();
            }
            return wetMills;
        }
        public DataSet GetMexicanSugarReportData()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Import_MexicanSugar", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }
    }
}
