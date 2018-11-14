using McF.Contracts;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace McF.Business
{
    public class USWeeklyService : IUSWeeklyService
    {
        private IUSWeeklyRepository USWeeklyRepos = null;
        public USWeeklyService(IUSWeeklyRepository usWeeklyRepos)
        {
            this.USWeeklyRepos = usWeeklyRepos;
        }

        public SymbolInfo GetSymbolInfo()
        {
            return USWeeklyRepos.GetSymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            return USWeeklyRepos.GetRawData();
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return USWeeklyRepos.GetFieldInfo();
        }

        public USWeeklyCategoryInfo GetUSWeeklySymbolInfo()
        {
            return USWeeklyRepos.GetUSWeeklySymbolInfo();
        }
        public Dictionary<string, USWeeklyCategoryInfo> GetUSWeeklyRawData()
        {
            return USWeeklyRepos.GetUSWeeklyRawData();
        }
        public void UpdateUSWeeklyData(USWeeklyUpdateData usWeeklyUpdateData)
        {
            DateTime dt = Convert.ToDateTime(usWeeklyUpdateData.Date);
            string query = $"Update USWEEKLY_DIALY_DATA set {usWeeklyUpdateData.Field} = {usWeeklyUpdateData.Value} where Symbol = '{usWeeklyUpdateData.Symbol}' and WeekEnding = '{dt.ToString("yyyy-MM-dd")}'";
            USWeeklyRepos.UpdateUSWeeklyData(query);
        }

        public List<USWeeklyData> GetFormatedData(string Category, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return USWeeklyRepos.GetFormatedData(Category, Index, From,To );
        }

        public List<USWeeklyData> GetLastUpdatedData()
        {
            return USWeeklyRepos.GetLastUpdatedData();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {
            USWeeklyRepos.UpdateData(lstUpdateInfo);
        }

        public void PopulateData(List<USWeeklyData> lstData)
        {
            USWeeklyRepos.PopulateData(lstData);
        }
    }
}
