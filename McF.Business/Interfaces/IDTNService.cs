using System.Collections.Generic;
using McF.Contracts;
using System;
using System.Data;

namespace McF.Business
{
    public interface IDTNService : IBaseService
    {
        DTNJobInfo GetJobInfo();
        List<DTNFields> GetTempFieldInfo();
      //  List<DTNSymbols> GetTempSymbolInfo();
        void UpdateDTNData(DTNUpdate dtnUpdate);
        DataSet GetDTNConfInfo();
        DTNRawInfo GetDTNNRawData();
        DSFormatedData GetDTNFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        List<DTNData> GetDTNLastUpdatedData();
        void PopulateDTNData(List<DTNData> lstDTNData);
        List<DTNSymbols> GetDTNSymbolInfo();

        DataSet GetSugarReportData();
        
    }
}
 