using System;
using System.Collections.Generic;
using McF.Contracts;
using System.Data;

namespace McF.Business
{
    public interface ICropService : IBaseService
    {
        CROPSymbolFieldInfo GetCropSymbolInfo();
        CROPConditions GetCropConditionInfo();
        List<CROPData> GetCropLastUpdatedData();
        void PopulateCropSymbol(CROPMappingInfo cropSymbolInfo);
        void PopulateCropDialyData(List<CROPDialyData> lstCropData);
        DSFormatedData GetCropFormatedData(int index, DateTime? From, DateTime? To);
    }
}
