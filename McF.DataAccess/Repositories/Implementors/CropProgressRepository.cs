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
    public class CropProgressRepository : ICropProgressRepository
    { 

        private IDbHelper dbHelper = null;
        private DataTable CreateTable(List<string> symbols)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYMBOL", typeof(string));
            foreach(string sym in symbols)
            {
                dt.Rows.Add(sym);
            }
            return dt;
        }

        public CropProgressRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
        public DataSet ExecuteSP(string SP)
        {
            dbHelper.CreateCommand(SP, CommandType.StoredProcedure);
            DataSet ds = dbHelper.ExecuteDataSet();
            dbHelper.CloseConnection();
            return ds;
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return null;
        }

        public SymbolInfo GetSymbolInfo()
        {
            return null;
        }

        public RawFilesInfo GetRawData()
        {
            RawFilesInfo rawFilesInfo = new RawFilesInfo();
            rawFilesInfo.URL = "http://61.12.40.11:8091/RawFiles/CROP/1/prog_all_tables.csv";
            rawFilesInfo.NoOfDays = 5;
            return rawFilesInfo;
        }

        public void UpdateCROPData(string query)
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

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {
            
        }

        public CROPSymbolFieldInfo GetCropSymbolInfo()
        {
            return new CROPSymbolFieldInfo();
        }
        public CROPConditions GetCropConditionInfo()
        {
            return new CROPConditions();
        }

        public DataSet GetCropFormatedData(int index, DateTime? From, DateTime? To)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_CROP_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@INDEX", index);
                    dbHelper.AddParameter(dbCommand, "@From", From);
                    dbHelper.AddParameter(dbCommand, "@To", To);
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public DataSet GetCropFormatedData(List<string> Symbols, int index, DateTime? From, DateTime? To, bool bIsRollUp, string RollUpFrequency, string RollUpText)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_CROP_UPDATED", CommandType.StoredProcedure))
                {
                    DataTable dt = CreateTable(Symbols);
                    dbHelper.AddParameter(dbCommand, "@SymbolTable", dt);
                    dbHelper.AddParameter(dbCommand, "@INDEX", index);
                    dbHelper.AddParameter(dbCommand, "@From", From);
                    dbHelper.AddParameter(dbCommand, "@To", To);
                    dbHelper.AddParameter(dbCommand, "@RollUP", bIsRollUp ? 1:0);
                    dbHelper.AddParameter(dbCommand, "@RollUpFrequency", RollUpFrequency);
                    dbHelper.AddParameter(dbCommand, "@RollUpText", RollUpText);

                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                }
            }
            catch(Exception ex)
            {

            }
            return ds;
        }

        public List<CROPData> GetCropLastUpdatedData()
        {
            return new List<CROPData>();
        }

        public void PopulateGroupAndSymbolInfo(Dictionary<string, CROPSymbolFieldInfo> lstCropSymbolFieldInfo, Dictionary<string, CROPConditions> lstCropConditions)
        {
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_CROPPROGRESS_SYMBOLDATA", CommandType.StoredProcedure))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    string symbol = dr["Commodity_Name"].ToString();
                    int condition = Convert.ToInt16(dr["IsCondition"]);
                    if (condition == 0)
                    {
                        if (!lstCropSymbolFieldInfo.ContainsKey(symbol))
                        {
                            CROPSymbolFieldInfo cropSymbol = new CROPSymbolFieldInfo();
                            cropSymbol.Symbol = symbol;
                            lstCropSymbolFieldInfo[symbol] = cropSymbol;
                        }
                        string fie = dr["Field"].ToString().ToUpper().Trim();
                        if ( !lstCropSymbolFieldInfo[symbol].Fields.Contains(fie))
                            lstCropSymbolFieldInfo[symbol].Fields.Add(fie);
                    }
                    else
                    {
                        string fie = dr["Field"].ToString().ToUpper().Trim();
                        if (!lstCropSymbolFieldInfo.ContainsKey(symbol))
                        {
                            CROPSymbolFieldInfo cropSymbol = new CROPSymbolFieldInfo();
                            cropSymbol.Symbol = symbol;
                            lstCropSymbolFieldInfo[symbol] = cropSymbol;
                        }
                        lstCropSymbolFieldInfo[symbol].Conditions.Add(fie);
                    }
                }
                dbHelper.CloseConnection();
            }
        }
        public void PopulateCropDialyData(List<CROPDialyData> lstCropData )
        {
            int jobid = 11;
            int userid = 0;
            DateTime dt = DateTime.Now;
            foreach( CROPDialyData cropDialyData in lstCropData )
            {
                try
                {
                    if (cropDialyData.isCondition)
                    {
                        foreach (KeyValuePair<string, string> kv in cropDialyData.ConditionValues)
                        {
                            string selQuery = $"SELECT COUNT(*) AS CNT FROM CROPPROGRESS_CONFIG WHERE Commodity_Name = '{cropDialyData.Commodity}' AND Field = '{kv.Key}'";
                            dbHelper.CreateCommand(selQuery);
                            IDataReader dr = dbHelper.ExecuteReader();
                            int count = 0;
                            if (dr.Read())
                            {
                                count = Convert.ToInt32(dr["CNT"]);
                            }
                            dbHelper.CloseConnection();
                            if (count == 0)
                            {
                                selQuery = $"INSERT INTO CROPPROGRESS_CONFIG SELECT '{cropDialyData.Commodity}','{kv.Key}', '{kv.Key}','Percent', 1";
                                dbHelper.CreateCommand(selQuery);
                                dbHelper.ExecuteNonQuery();
                                dbHelper.CloseConnection();
                            }
                            selQuery = $"INSERT INTO CROPPROGRESS_DATA SELECT '{cropDialyData.Commodity}', '{cropDialyData.ReportDate}','{cropDialyData.WeekEnding.Value.ToShortDateString()}', '{cropDialyData.State}', '{kv.Key}',{kv.Value}";
                            dbHelper.CreateCommand(selQuery);
                            dbHelper.ExecuteNonQuery();
                            dbHelper.CloseConnection();
                        }
                    }
                    else
                    {
                        string selQuery = $"SELECT COUNT(*) AS CNT FROM CROPPROGRESS_CONFIG WHERE Commodity_Name = '{cropDialyData.Commodity}' AND Field = '{cropDialyData.MappingValue}'";
                        dbHelper.CreateCommand(selQuery);
                        IDataReader dr = dbHelper.ExecuteReader();
                        int count = 0;
                        if (dr.Read())
                        {
                            count = Convert.ToInt32(dr["CNT"]);
                        }
                        dbHelper.CloseConnection();
                        if (count == 0)
                        {
                            selQuery = $"INSERT INTO CROPPROGRESS_CONFIG SELECT '{cropDialyData.Commodity}','{cropDialyData.MappingValue}', '{cropDialyData.MappingValue}','Percent', 0";
                            dbHelper.CreateCommand(selQuery);
                            dbHelper.ExecuteNonQuery();
                            dbHelper.CloseConnection();
                        }
                        selQuery = $"INSERT INTO CROPPROGRESS_DATA SELECT '{cropDialyData.Commodity}', '{cropDialyData.ReportDate}','{cropDialyData.WeekEnding.Value.ToShortDateString()}', '{cropDialyData.State}', '{cropDialyData.MappingValue}',{cropDialyData.Value}";
                        dbHelper.CreateCommand(selQuery);
                        dbHelper.ExecuteNonQuery();
                        dbHelper.CloseConnection();
                    }
                }
                catch(Exception ex)
                { }
            }
        }
        
        public void PopulateCropSymbol(CROPMappingInfo cropSymbolInfo)
        {
            if (!cropSymbolInfo.bExists)
            {
                IDbCommand dbCommand = dbHelper.CreateCommand("McF_Populate_Crop_Symbol_Info", CommandType.StoredProcedure);

                dbHelper.AddParameter(dbCommand, "@symbol", cropSymbolInfo.Symbol);
                dbHelper.AddParameter(dbCommand, "@field", cropSymbolInfo.Field);
                dbHelper.AddParameter(dbCommand, "@mappingID", cropSymbolInfo.MappingID);
                dbHelper.AddParameter(dbCommand, "@IsCondition", cropSymbolInfo.IsCondition ? 1 : 0);
                dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();

                if (cropSymbolInfo.IsCondition)
                {
                    string sqquery = $"INSERT INTO CROPPROGRESS_CONDITIONS (MappingID,NoOfCond,";
                    string values = $" Values('{cropSymbolInfo.MappingID}', {cropSymbolInfo.Conditions.Count},";
                    foreach (KeyValuePair<string, string> kv in cropSymbolInfo.Conditions)
                    {
                        sqquery += $"{kv.Key},";
                        values += $"'{kv.Value}',";
                    }
                    sqquery = sqquery.Substring(0, sqquery.Length - 1) + ")";
                    values = values.Substring(0, values.Length - 1) + ")";

                    dbHelper.CreateCommand(sqquery + values);
                    dbHelper.ExecuteNonQuery();
                    dbHelper.CloseConnection();
                }
            }
        }
    }
}
