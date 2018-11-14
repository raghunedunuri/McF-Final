using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess
{
    public interface ICommonRepository
    {
        List<DataSources> GetAllDataSources();
        Dictionary<string, string> GetDataSources();
        List<string> GetDataSourceFields(string tableName);
        DSFormatedData GetFormatedData(int index = 0, DateTime? StartDate = null, DateTime? EndDate = null);
        Dictionary<string, WasdeCommodity> GetWasdeCommodities();
        Dictionary<string, WasdeDomesticField> GetWasdeDomesticFields();
        Dictionary<string, WasdeDomesticCommodity> GetWasdeDomesticCommodities();
        DSFormatedData GetWasdeWorldFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetWasdeDomesticFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetWasdeWorldFormatedData(List<string> symbols, int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetWasdeDomesticFormatedData(List<string> symbols, int Index = 0, DateTime? From = null, DateTime? To = null);
        int ProcessQuery(string Query);
        DataSet ProcessDataQuery(string Query);
        DSFormatedData GetFOFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        Dictionary<string, FOField> GetFOFields();
        DSFormatedData GetCocoaFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        bool SaveData(DSUpdatedData dsUpdatedData);
        DSFormatedData GetBHFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DSFormatedData GetCFFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        DataSet ExecuteDataSetFromSP(string storedProc);
        DataSet GetTableFormatedData(String SP, List<string> Symbols, int index, DateTime? From, DateTime? To);
        DataSet DeleteExistingRecords(string SP, string Reportdate);
        DataSet GetExcelFormatedData(ExcelQuery excelQuery);
        void BulkInsertDataTable(string tableName, DataTable table);
        void PopulateUser(UserLoginData userLoginData);
        bool ValidateUser(string user, string passWord);
        DSFormatedData GetCompleteFormatData(string DataSource, string RootSource, int Index = 0, DateTime? From = null, DateTime? To = null);
        bool SaveDataSource(DataSource datasource);
        bool ResetPassword(string user, string passWord, string newPassWord);
    }
}
