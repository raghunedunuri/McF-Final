using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Implementors
{
    public class DTNRepository : IDTNRepository
    {
        private IDbHelper dbHelper = null;
        public DTNRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public DTNJobInfo GetJobInfo()
        {
            DTNJobInfo dtnJobInfo = new DTNJobInfo();
            dbHelper.CreateCommand("select * from MCF_JOB_INFO");
            IDataReader dr = dbHelper.ExecuteReader();

            if (dr.Read())
            {
                dtnJobInfo.ProgID = dr["RTDProgID"].ToString();
            }
            dr.Close();
            dbHelper.CloseConnection();
            return dtnJobInfo;
        }

        public DataSet ExecuteSP(string SP)
        {
            dbHelper.CreateCommand(SP, CommandType.StoredProcedure);
            DataSet ds = dbHelper.ExecuteDataSet();
            dbHelper.CloseConnection();
            return ds;
        }

        public List<DTNFields> GetTempFieldInfo()
        {
            List<DTNFields> dtnFields = new List<DTNFields>();
            dbHelper.CreateCommand("select * from DTN_FIELD_INFO");
            IDataReader dr = dbHelper.ExecuteReader();

            while (dr.Read())
            {
                dtnFields.Add(new DTNFields()
                {
                    Name = dr["NAME"].ToString(),
                    RTDTopic = dr["RTDTOPIC"].ToString()
                });
            }
            dr.Close();
            return dtnFields;
        }
        public DataSet GetDTNConfInfo()
        {
            DataSet groupsDS;

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_DTN_GROUP_DATA", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();
            }
            
            return groupsDS;
        }
        public void UpdateDTNData( List<DTNUpdateInfo> dtnUpdateInfo)
        {
            foreach( DTNUpdateInfo dtn in dtnUpdateInfo )
            {
                try
                {
                    string query = $"Insert into DTN_DIALY_DATA (";
                    string fields = String.Empty;
                    string values = String.Empty;

                    fields += $"symbolid,Date,symbol,";
                    values += $"{dtn.SymnolID},'{dtn.UpdatedTime}','{dtn.Symbol}',";
                    foreach (DTNFieldUpdate fup in dtn.FieldInfo)
                    {
                        fields += $"{fup.field},";
                        values += $"'{fup.value}',";
                    }
                    fields = fields.Substring(0, fields.Length - 1);
                    values = values.Substring(0, values.Length - 1);
                    query += $"{fields}) Values({values})";
                    dbHelper.CreateCommand(query);
                    dbHelper.ExecuteNonQuery();
                    dbHelper.CloseConnection();
                }
                catch(Exception ex)
                {

                }
            }
        }

        public void UpdateDTNData(string query)
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

        public List<DTNSymbols> GetDTNSymbolInfo()
        {
            List <DTNSymbols> dtnSymbols = new List<DTNSymbols>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("SELECT Commodity_Name,DTNRoot,Unit FROM DTN_SYMBOL_INFO", CommandType.Text))
                {

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        dtnSymbols.Add(new DTNSymbols()
                        {
                            Symbol = dr["Commodity_Name"].ToString(),
                            DTNRoot = dr["DTNRoot"].ToString(),
                            Unit = dr["Unit"].ToString()
                        });
                    }
                    dr.Close();

                }

            }
            catch (Exception ex)
            {

            }
            return dtnSymbols;
        }


        public List<FieldInfo> GetFieldInfo()
        {
            return new List<FieldInfo>();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return null;
        }

        public RawFilesInfo GetRawData()
        {
            return null;
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {

        }

        public DataSet GetDTNData(string query)
        {
            dbHelper.CreateCommand(query);
            return dbHelper.ExecuteDataSet();
        }

        public DTNRawInfo GetDTNNRawData()
        {
            DTNRawInfo dtRawInfo = new DTNRawInfo();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_DTN_RAW_DATA", CommandType.StoredProcedure))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                if (dr.Read())
                {
                    dtRawInfo.URL = dr["URL"].ToString();
                    dtRawInfo.URL = dr["UserName"].ToString();
                    dtRawInfo.URL = dr["Password"].ToString();
                }
                dbHelper.CloseConnection();
            }
            return dtRawInfo;
        }
        public DSFormatedData GetDTNFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dsFormatedData = new DSFormatedData();
            FormatedData forData = new FormatedData();
            forData.CommodityData = new RootData();
            forData.CommodityData.Name = "DTN";
            forData.CommodityData.TableName = "DTN_DIALY_DATA";
            dsFormatedData.DictFormatedData.Add("DTN", forData);
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_DTN_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@Start", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@End", To);

                    DataSet ds = dbHelper.ExecuteDataSet();

                    DataTable Data = ds.Tables[0];
                    dbHelper.CloseConnection();

                    foreach (DataColumn dc in Data.Columns)
                    {
                        forData.Headers.Add(new HeaderFields()
                        {
                            Name = dc.ColumnName,
                            DisplayName = dc.ColumnName,
                            ReadOnly = (dc.ColumnName == "Date" || dc.ColumnName == "RootSymbol" || dc.ColumnName == "Commodity_Name" || dc.ColumnName == "Symbol" || dc.ColumnName == "IssueDescription" || dc.ColumnName == "Unit")?true:false
                        });
                    }

                    foreach ( DataRow dr in Data.Rows)
                    {
                        List<string> row = new List<string>();
                        foreach(DataColumn dc in Data.Columns)
                        {
                            row.Add(dr[dc.ColumnName].ToString());
                        }
                        forData.Values.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dsFormatedData;
        }

        public List<DTNData> GetDTNLastUpdatedData()
        {
            return new List<DTNData>();
        }

        public void PopulateDTNData(List<DTNData> lstDTNData)
        {
            foreach( DTNData dt in lstDTNData )
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_DTN_Populate_Data", CommandType.StoredProcedure))
                {
                    try
                    {
                        dbHelper.AddParameter(dbCommand, "@Date", dt.Date);
                        dbHelper.AddParameter(dbCommand, "@Symbol", dt.Symbol);
                        dbHelper.AddParameter(dbCommand, "@RootSymbol", dt.RootSymbol);
                        dbHelper.AddParameter(dbCommand, "@IssueDescription", dt.IssueDescription);
                        dbHelper.AddParameter(dbCommand, "@Last", dt.Last);
                        dbHelper.AddParameter(dbCommand, "@High", dt.High);
                        dbHelper.AddParameter(dbCommand, "@Open", dt.Open);
                        dbHelper.AddParameter(dbCommand, "@Close", dt.Close);
                        dbHelper.AddParameter(dbCommand, "@Low", dt.Low);
                        dbHelper.AddParameter(dbCommand, "@Previous", dt.Previous);
                        dbHelper.AddParameter(dbCommand, "@OpenInterest", dt.OpenInterest);
                        dbHelper.AddParameter(dbCommand, "@Change", dt.Change);
                        dbHelper.AddParameter(dbCommand, "@Volume", dt.Volume);
                        dbHelper.AddParameter(dbCommand, "@CumVolume", dt.CumVolume);
                        dbHelper.AddParameter(dbCommand, "@TradeCount", dt.TradeCount);
                        dbHelper.AddParameter(dbCommand, "@DaystoExpiration", dt.DaystoExpiration);
                        dbHelper.AddParameter(dbCommand, "@Volatility", dt.Volatility);
                        dbHelper.AddParameter(dbCommand, "@VWAP", dt.VWAP);
                        dbHelper.AddParameter(dbCommand, "@BidAskSpreadAvg", dt.BidAskSpreadAvg);
                        dbHelper.AddParameter(dbCommand, "@SettlementPrice", dt.SettlementPrice);
                        dbHelper.AddParameter(dbCommand, "@ExpirationDate", dt.ExpirationDate);
                        dbHelper.AddParameter(dbCommand, "@LTrade", dt.LTrade);
                        dbHelper.AddParameter(dbCommand, "@CntrLow", dt.CntrLow);
                        dbHelper.AddParameter(dbCommand, "@CntrHigh", dt.CntrHigh);
                        dbHelper.ExecuteNonQuery();
                        dbHelper.CloseConnection();
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }

        public DataSet GetSugarReportData()
        {
            DataSet groupsDS;
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_Reports_Sugar_Data", CommandType.StoredProcedure))
            {
                groupsDS = dbHelper.ExecuteDataSet();
            }
            return groupsDS;
        }

       
        
    }
}
