using McF.Contracts;
using McF.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace McF.DataAcess
{
    public interface IUSWeeklyRepository : IBaseDataRepository
    {
        USWeeklyCategoryInfo GetUSWeeklySymbolInfo();

        Dictionary<string, USWeeklyCategoryInfo> GetUSWeeklyRawData();

        List<USWeeklyData> GetFormatedData(string Category, int Index = 0, DateTime? From = null, DateTime? To = null);
        List<USWeeklyData> GetLastUpdatedData();
        void PopulateData(List<USWeeklyData> lstEthanolData);

        void UpdateUSWeeklyData(string query);
        DataSet GetUSweeklyConfigData();
        DataSet GetUSProgressUpdatedData(string query);
    }
}
