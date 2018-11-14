using System;
using System.Collections.Generic;
using System.Data;
using McF.Contracts;
using McF.DataAcess;
using McF.DataAccess.Repositories.Interfaces;
using System.Globalization;

namespace McF.Business
{
    public class EthanolService : IEthanolService
    {
        private IEthanolRepository EthanolRepository = null;
    //    private static DataSet ColTypes = null;
        public EthanolService(IEthanolRepository EthanolRepository)
        {
            this.EthanolRepository = EthanolRepository;
           // ColTypes = EthanolRepository.ExecuteSP("McF_GET_ETHANOL_FIELDS");
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return EthanolRepository.GetFieldInfo();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return EthanolRepository.GetSymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            return EthanolRepository.GetRawData();
        }

        public EthanolRawData GetEthanolRawData()
        {
            return EthanolRepository.GetEthanolRawData();
        }

        public DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            return EthanolRepository.GetFormatedData(Index,From, To);
        }
        public void UpdateEthanolData(EthanolUpdateData ethanolUpdate)
        {
            DateTime dt = Convert.ToDateTime(ethanolUpdate.Date);
            string query = $"Update ETHANOL_DIALY_DATA set {ethanolUpdate.Field} = {ethanolUpdate.Value} where Symbol = '{ethanolUpdate.Symbol}' and DataDate = '{dt.ToString("yyyy-MM-dd")}'";
            EthanolRepository.UpdateEthanolData(query);
        }

        public List<EthanolData> GetLastUpdatedData()
        {
            return EthanolRepository.GetLastUpdatedData();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {
            EthanolRepository.UpdateData(lstUpdateInfo);
        }
        public void PopulateData(List<EthanolData> lstEthanolData)
        {
            EthanolRepository.PopulateData(lstEthanolData);
        }
    }
}
