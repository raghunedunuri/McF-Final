using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public interface IUSWeeklyService : IBaseService
    {
        USWeeklyCategoryInfo GetUSWeeklySymbolInfo();
        Dictionary<string, USWeeklyCategoryInfo> GetUSWeeklyRawData();
        List<USWeeklyData> GetFormatedData(string Category, int Index = 0, DateTime? From = null, DateTime? To = null);
        List<USWeeklyData> GetLastUpdatedData();
        void PopulateData(List<USWeeklyData> lstEthanolData);
        void UpdateUSWeeklyData(USWeeklyUpdateData usWeeklyUpdateData);
    }
}
