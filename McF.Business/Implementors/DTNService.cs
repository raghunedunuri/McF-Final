using System.Collections.Generic;
using System.Data;
using System.Linq;
using McF.Contracts;
using McF.DataAcess;
using McF.DataAccess.Repositories.Interfaces;
using System;
using System.Globalization;

namespace McF.Business
{
    public class DTNService : IDTNService
    {
        private IDTNRepository dtnRepos = null;
        private static DataSet ColTypes = null;
        public DTNService(IDTNRepository dtnRepos)
        {
            this.dtnRepos = dtnRepos;
           // ColTypes = dtnRepos.ExecuteSP("McF_GET_DTN_FIELDS");
        }

        private string GetColType(DataTable dt, string columnName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Fields"].ToString().ToUpper().Trim() == columnName.ToUpper().Trim())
                    return dr["DATA_TYPE"].ToString();
            }
            return String.Empty;
        }

        public DTNJobInfo GetJobInfo()
        {
            return dtnRepos.GetJobInfo();
        }

        public List<DTNFields> GetTempFieldInfo()
        {
            return dtnRepos.GetTempFieldInfo();
        }
    
        public void UpdateDTNData(DTNUpdate dtnUpdate)
        {
            string dType = GetColType(ColTypes.Tables[0], dtnUpdate.Field);
            object val = dtnUpdate.Value;
            DateTime dt = Convert.ToDateTime(dtnUpdate.UpdatedTime);
            switch (dType.ToUpper().Trim())
            {
                case "DECIMAL":
                case "BIGINT":
                case "INT":
                    break;
                default:
                    val = $"'{dtnUpdate.Value}'";
                    break;
            }
            string query = $"Update DTN_DIALY_DATA set {dtnUpdate.Field} = {val} where Symbol = '{dtnUpdate.Symbol}' and UPDATEDTIME = '{dt.ToString("MM/dd/yyyy hh:mm:ss.fff")}'";
            dtnRepos.UpdateDTNData(query);
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return dtnRepos.GetFieldInfo();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return dtnRepos.GetSymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            return dtnRepos.GetRawData();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {
            dtnRepos.UpdateData(lstUpdateInfo);
        }

        public void UpdateDTNData(string query)
        {
            dtnRepos.UpdateDTNData(query);
        }

        public DataSet GetDTNData(string query)
        {
            return dtnRepos.GetDTNData(query);
        }
        public DTNRawInfo GetDTNNRawData()
        {
            return dtnRepos.GetDTNNRawData();
        }
        public DSFormatedData GetDTNFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return dtnRepos.GetDTNFormatedData(Index,From,To);
        }
        public List<DTNData> GetDTNLastUpdatedData()
        {
            return dtnRepos.GetDTNLastUpdatedData();
        }

        public List<DTNSymbols> GetDTNSymbolInfo()
        {
            return dtnRepos.GetDTNSymbolInfo();
        }
        public DataSet GetDTNConfInfo()
        {
            return dtnRepos.GetDTNConfInfo();
        }

        public void PopulateDTNData(List<DTNData> lstDTNData)
        {
            dtnRepos.PopulateDTNData(lstDTNData);
        }

        public DataSet GetSugarReportData()
        {
            return dtnRepos.GetSugarReportData();
        }

        
    }
}
