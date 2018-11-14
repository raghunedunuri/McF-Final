using System.Collections.Generic;
using System.Data;
using McF.Contracts;
using System;
using McF.DataAcess;
using McF.DataAccess.Repositories.Interfaces;

namespace McF.Business
{
    public class CropService : ICropService
    {
        private ICropProgressRepository cropRepos = null;

        public CropService(ICropProgressRepository cropRepos)
        {
            this.cropRepos = cropRepos;
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return null;
        }

        public SymbolInfo GetSymbolInfo()
        {
            return null;
        }

        public RawFilesInfo GetRawData()
        {
            return cropRepos.GetRawData();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {
            cropRepos.UpdateData(lstUpdateInfo);
        }

        public CROPSymbolFieldInfo GetCropSymbolInfo()
        {
            return cropRepos.GetCropSymbolInfo();
        }
        public CROPConditions GetCropConditionInfo()
        {
            return cropRepos.GetCropConditionInfo();
        }
        public DataSet GetCropFormatedData(List<string> Symbols, int index, DateTime? From, DateTime? To, bool bIsRollUp, string RollUpFrequency, string RollUpText)
        {
            return cropRepos.GetCropFormatedData(Symbols, index, From, To, bIsRollUp,RollUpFrequency,RollUpText);
        }
        public DSFormatedData GetCropFormatedData(int index, DateTime? From, DateTime? To)
        {
            DSFormatedData dsFormatedData = new DSFormatedData();
            DataSet ds = cropRepos.GetCropFormatedData(index, From, To);

            Dictionary<string, CROPData> dictCropData = new Dictionary<string, CROPData>();
            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string symbol = dr["SYMBOL"].ToString().Trim();
                    string state = dr["STATE"].ToString();
                    string weekEnding = dr["WeekEnding"].ToString();
                    string unique = $"{symbol}|{state}|{weekEnding}";
                    CROPData cropData = null;
                    if (!dictCropData.ContainsKey(unique))
                    {
                        dictCropData[unique] = new CROPData()
                        {
                            Symbol = symbol,
                            WeekEnding = weekEnding,
                            State = state,
                        };
                    }
                    cropData = dictCropData[unique];
                    string field = dr["FIELD"].ToString().Trim();
                    string value = dr["VALUE"].ToString();
                    cropData.Values.Add(field, value);
                }

                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    string symbol = dr["SYMBOL"].ToString().Trim();
                    string state = dr["STATE"].ToString();
                    string weekEnding = dr["WeekEnding"].ToString();
                    string unique = $"{symbol}|{state}|{weekEnding}";
                    CROPData cropData = null;
                    if (!dictCropData.ContainsKey(unique))
                    {
                        dictCropData[unique] = new CROPData()
                        {
                            Symbol = symbol,
                            WeekEnding = weekEnding,
                            State = state,
                        };
                    }
                    cropData = dictCropData[unique];
                    int NoOfCond = Convert.ToInt32(dr["NoOfCond"]);
                    for (int i = 1; i <= NoOfCond; i++)
                    {
                        string fieldValue = dr[$"COND{i}FIELD"].ToString().Trim();
                        string Value = dr[$"COND{i}VALUE"].ToString();
                        cropData.Conditions.Add(fieldValue, Value);
                    }
                }
            }

            foreach(KeyValuePair<string, CROPData> kv in dictCropData)
            {
                FormatedData formatedData = null;
                if (!dsFormatedData.DictFormatedData.ContainsKey(kv.Value.Symbol))
                {
                    formatedData = new FormatedData();
                    formatedData.CommodityData = new RootData()
                    {
                        Name = kv.Value.Symbol,
                        TableName = "CROPPROGRESS_DIALY_DATA,CROPPROGRESS_COND_DIALY_DATA",
                        Unit = "Percentage(%)"
                    };
                    formatedData.Headers.Add(GetHeader("Symbol", true));
                    formatedData.Headers.Add(GetHeader("WeekEnding", true));
                    formatedData.Headers.Add(GetHeader("State", true));

                    foreach (KeyValuePair<string, string> rv in kv.Value.Values)
                    {
                        formatedData.Headers.Add(GetHeader(rv.Key));
                    }
                    foreach (KeyValuePair<string, string> rv in kv.Value.Conditions)
                    {
                        formatedData.Headers.Add(GetHeader(rv.Key));
                    }
                    dsFormatedData.DictFormatedData[kv.Value.Symbol] = formatedData;
                }
                formatedData = dsFormatedData.DictFormatedData[kv.Value.Symbol];
                List<string> values = new List<string>();
                values.Add(kv.Value.Symbol);
                values.Add(kv.Value.WeekEnding);
                values.Add(kv.Value.State);
                foreach (KeyValuePair<string, string> rv in kv.Value.Values)
                {
                    values.Add(rv.Value);
                }
                foreach (KeyValuePair<string, string> rv in kv.Value.Conditions)
                {
                    values.Add(rv.Value);
                }
                formatedData.Values.Add(values);
            }
            return dsFormatedData;
        }
        private HeaderFields GetHeader(string Name,bool bReadOnly = false)
        {
            return new HeaderFields()
            {
                Name = Name,
                DisplayName = Name,
                ReadOnly = bReadOnly
            };
        }
        public List<CROPData> GetCropLastUpdatedData()
        {
            return cropRepos.GetCropLastUpdatedData();
        }
        public void PopulateCropSymbol(CROPMappingInfo cropSymbolInfo)
        {
            cropRepos.PopulateCropSymbol(cropSymbolInfo);
        }

        public void PopulateCropDialyData(List<CROPDialyData> lstCropData)
        {
            cropRepos.PopulateCropDialyData(lstCropData);
        }
    }
}
