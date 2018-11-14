using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface ICropProgressRepository: IBaseDataRepository
    {
        CROPSymbolFieldInfo GetCropSymbolInfo();
        CROPConditions GetCropConditionInfo();
        DataSet GetCropFormatedData(List<string> Symbols, int index, DateTime? From, DateTime? To, bool bIsRollUp, string RollUpFrequency, string RollUpText);
        List<CROPData> GetCropLastUpdatedData();
        DataSet GetCropFormatedData(int index, DateTime? From, DateTime? To);
        void PopulateCropSymbol(CROPMappingInfo cropSymbolInfo);
        void PopulateCropDialyData(List<CROPDialyData> lstCropData);
        void PopulateGroupAndSymbolInfo(Dictionary<string, CROPSymbolFieldInfo> lstCropSymbolFieldInfo, Dictionary<string, CROPConditions> lstCropConditions);
        void UpdateCROPData(string query);
    }
}
