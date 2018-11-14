using McF.Contracts;
using System;
using System.Collections.Generic;

namespace McF.Business
{
    public interface IEthanolService : IBaseService
    {
        DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null);
        List<EthanolData> GetLastUpdatedData();
        void PopulateData(List<EthanolData> lstEthanolData);
        EthanolRawData GetEthanolRawData();
        void UpdateEthanolData(EthanolUpdateData ethanolUpdate);
    }
}
