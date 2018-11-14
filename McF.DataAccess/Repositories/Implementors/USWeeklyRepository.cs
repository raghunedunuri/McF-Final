using System;
using System.Collections.Generic;
using System.Data;
using McF.Contracts;
using McF.DataAcess;
using McF.DataAccess;

namespace McF.DataAccess.Repositories.Implementors
{
    public class USWeeklyRepository : IUSWeeklyRepository
    {
        private IDbHelper dbHelper = null;
        public USWeeklyRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public SymbolInfo GetSymbolInfo()
        {
            return null;
        }

        public RawFilesInfo GetRawData()
        {
            return null;
        }

        public void UpdateUSWeeklyData(string query)
        {
            try
            {
                dbHelper.CreateCommand(query);
                dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();
            }
            catch (Exception ex)
            {

            }
        }
        public DataSet ExecuteSP(string SP)
        {
            dbHelper.CreateCommand(SP, CommandType.StoredProcedure);
            DataSet ds = dbHelper.ExecuteDataSet();
            dbHelper.CloseConnection();
            return ds;
        }
        public DataSet GetUSProgressUpdatedData(string query)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
                {
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                    return ds;
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public DataSet GetUSweeklyConfigData()
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_USWEEKLY_GROUPDATA", CommandType.StoredProcedure))
                {
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                    return ds;
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return new List<FieldInfo>();
        }

        public USWeeklyCategoryInfo GetUSWeeklySymbolInfo()
        {
            return new USWeeklyCategoryInfo("Test");
        }

        public Dictionary<string, USWeeklyCategoryInfo> GetUSWeeklyRawData()
        {
            Dictionary<string ,USWeeklyCategoryInfo> usWeeklyCategoryInfo = new Dictionary<string, USWeeklyCategoryInfo>();
           try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("SELECT * FROM USWEEKLY_SYMBOL_INFO order by Commodity_Name", CommandType.Text))
                {
                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        string commodityName =  dr["Commodity_Name"].ToString();
                        if ( !usWeeklyCategoryInfo.ContainsKey(commodityName) )
                        {
                            usWeeklyCategoryInfo[commodityName] = new USWeeklyCategoryInfo(commodityName);
                        }
                        USWeeklyCategoryInfo categoryInfo = usWeeklyCategoryInfo[commodityName];
                        categoryInfo.LstSymbolInfo.Add(new USWeeklySymbolInfo()
                        {
                            MappingSymbol = dr["MappingSymbol"].ToString(),
                            Symbol = dr["Symbol"].ToString(),
                            RawFileInfo = dr["RawFileInfo"].ToString(),
                            NoOfRecords = Convert.ToInt32(dr["NoOfRecords"])
                        });
                    }
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {
            }
            return usWeeklyCategoryInfo;
        }

        public List<USWeeklyData> GetFormatedData(string Category, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            List<USWeeklyData> lstUsWeeklyData = new List<USWeeklyData>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_USWEEKLY_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@Category", Category);
                    dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@Start", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@End", To);

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        lstUsWeeklyData.Add(new USWeeklyData()
                        {
                            Symbol = dr["SYMBOL"].ToString(),
                            Date = Convert.ToDateTime(dr["WeekEnding"]),
                            Weekly_Exports = Convert.ToInt64(dr["Weekly_Exports"]),
                            Accumulated_Exports = Convert.ToInt64(dr["Accumulated_Exports"]),
                            Net_Sales = Convert.ToInt64(dr["Net_Sales"]),
                            Outstanding_Sales = Convert.ToInt64(dr["Outstanding_Sales"]),
                            Nxt_mkt_year_Net_Sales = Convert.ToInt64(dr["Nxt_mkt_year_Net_Sales"]),
                            Nxt_Mkt_year_Outstanding_Sales = Convert.ToInt64(dr["Nxt_Mkt_year_Outstanding_Sales"]),
                            Category = dr["CATEGORY"].ToString()
                        });
                    }
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {
            }
            return lstUsWeeklyData;
        }

        public List<USWeeklyData> GetLastUpdatedData()
        {
            return new List<USWeeklyData>();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {

        }

        public void PopulateData(List<USWeeklyData> lstData)
        {
            DateTime dt = DateTime.Now;
            foreach (USWeeklyData usData in lstData)
            {
                IDbCommand dbCommand = dbHelper.CreateCommand("McF_Populate_USWeekly_Data", CommandType.StoredProcedure);
                dbHelper.AddParameter(dbCommand, "@weekEnd", usData.Date);
                dbHelper.AddParameter(dbCommand, "@symbol", usData.Symbol);
                dbHelper.AddParameter(dbCommand, "@weeklyExports", usData.Weekly_Exports);
                dbHelper.AddParameter(dbCommand, "@accumulatedExports", usData.Accumulated_Exports);
                dbHelper.AddParameter(dbCommand, "@netSales", usData.Net_Sales);
                dbHelper.AddParameter(dbCommand, "@outstandingSales", usData.Outstanding_Sales);
                dbHelper.AddParameter(dbCommand, "@nxtmktyearNetSales", usData.Nxt_mkt_year_Net_Sales);
                dbHelper.AddParameter(dbCommand, "@NxtMktyearOutstandingSales", usData.Nxt_Mkt_year_Outstanding_Sales);
                dbHelper.AddParameter(dbCommand, "@jobID", 1);
                dbHelper.AddParameter(dbCommand, "@userid", 1);
                dbHelper.AddParameter(dbCommand, "@lastUpdated", dt);
                dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();
            }
        }
    }
}
