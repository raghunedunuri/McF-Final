using System.Collections.Generic;
using System.Data;
using System.Linq;
using McF.Contracts;
using System;
using McF.DataAccess;
using McF.DataAccess.Repositories.Interfaces;

namespace McF.DataAcess
{
    public class EthanolRepository : IEthanolRepository
    {
        private IDbHelper dbHelper = null;
        public EthanolRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return new List<FieldInfo>();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return new SymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            return new RawFilesInfo();
        }

        public EthanolRawData GetEthanolRawData()
        {
            EthanolRawData rawData = new EthanolRawData();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("SELECT * FROM ETHANOL_JOB_INFO", CommandType.Text))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                if (dr.Read())
                {
                    rawData.StockSourceFile = dr["StockSourceFiles"].ToString();
                    rawData.PlantedSourceFile = dr["PlantedSourceFiles"].ToString();
                }
            }
            return rawData;
        }

        public DataSet ExecuteSP(string SP)
        {
            dbHelper.CreateCommand(SP, CommandType.StoredProcedure);
            DataSet ds = dbHelper.ExecuteDataSet();
            dbHelper.CloseConnection();
            return ds;
        }
        public DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData lstEthanoldata = new DSFormatedData();
            Dictionary<string, HeaderFields> headerFileds = new Dictionary<string, HeaderFields>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_ETHANOL_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@Index", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@From", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@To", To);

                    DataSet ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();

                    DataTable Fields = ds.Tables[0];
                    FormatedData formatedData = new FormatedData();
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Ethanol",
                        TableName = "ETHANOL_DIALY_DATA"
                    };

                    foreach (DataRow dr in Fields.Rows)
                    {
                        HeaderFields headerFiled = new HeaderFields()
                        {
                            Name = dr["Name"].ToString(),
                            DisplayName = dr["DisplayName"].ToString(),
                            Unit = dr["Unit"].ToString(),
                            ReadOnly = true
                        };
                        headerFileds.Add(headerFiled.Name, headerFiled);
                    }

                    DataTable values = ds.Tables[1];
                    foreach (DataColumn dc in values.Columns)
                    {
                        string Name = dc.ColumnName;
                        HeaderFields field = null;
                        if (!headerFileds.ContainsKey(Name))
                        {
                            field = new HeaderFields()
                            {
                                Name = Name,
                                DisplayName = Name,
                                ReadOnly = false
                            };
                        }
                        else
                        {
                            field = headerFileds[Name];
                        }
                        formatedData.Headers.Add(field);
                    }

                    foreach (DataRow dr in values.Rows)
                    {
                        List<string> rowData = new List<string>();
                        foreach (DataColumn dc in values.Columns)
                        {
                            rowData.Add(dr[dc.ColumnName]?.ToString());
                        }
                        formatedData.Values.Add(rowData);
                    }
                    lstEthanoldata.DictFormatedData.Add("Ethanol", formatedData);
                }
            }
            catch (Exception ex)
            {
            }
            return lstEthanoldata;
        }

        public List<EthanolData> GetLastUpdatedData()
        {
            return new List<EthanolData>();
        }

        public void UpdateEthanolData(string query)
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

        public DataSet GetEthanolUpdatedData(string query)
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
        public DataSet GetEthanolConfigData()
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_ETHANOL_GROUPDATA", CommandType.StoredProcedure))
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
        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {

        }

        public void PopulateData(List<EthanolData> lstEthanolData)
        {
            DateTime dt = DateTime.Now;
            foreach (EthanolData ethanolData in lstEthanolData)
            {
                IDbCommand dbCommand = dbHelper.CreateCommand("McF_Populate_Ethanol_Data", CommandType.StoredProcedure);
                dbHelper.AddParameter(dbCommand, "@dateValue", ethanolData.Date);
                dbHelper.AddParameter(dbCommand, "@symbol", ethanolData.Symbol);
                dbHelper.AddParameter(dbCommand, "@stock", ethanolData.Stock);
                dbHelper.AddParameter(dbCommand, "@planted", ethanolData.Planted);
                dbHelper.AddParameter(dbCommand, "@jobID", 1);
                dbHelper.AddParameter(dbCommand, "@userid", 1);
                dbHelper.AddParameter(dbCommand, "@lastUpdated", dt);
                dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();
            }
        }
    }
}
