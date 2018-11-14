using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Implementors
{
    public class COTRepository : ICOTRepository
    {
        private IDbHelper dbHelper = null;
        public COTRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public List<FieldInfo> GetFieldInfo()
        {
            return new List<FieldInfo>();
        }

        public SymbolInfo GetSymbolInfo()
        {
            return new SymbolInfo();
        }

        public RawFilesInfo GetRawData()
        {
            RawFilesInfo rawFilesInfo = new RawFilesInfo();
            rawFilesInfo.URL = "http://61.12.40.11:8091/RawFiles/COT/1/PET_STOC_WSTK_A_EPOOXE_SAE_MBBL_W.xls";
            rawFilesInfo.NoOfDays = 5;
            return rawFilesInfo;
        }

        public List<COTData> GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            List<COTData> lstCOTdata = new List<COTData>();
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_COT_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@Start", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@End", To);

                    IDataReader dr = dbHelper.ExecuteReader();
                    while (dr.Read())
                    {
                        COTData cotData = new COTData();
                        cotData.As_of_Date_In_Form_YYMMDD = dr["As_of_Date_In_Form_YYMMDD"].ToString();
                        cotData.CFTC_Contract_Market_Code = Convert.ToDouble(dr["CFTC_Contract_Market_Code"]);
                        cotData.CFTC_Market_Code = (dr["CFTC_Market_Code"]).ToString();
                        cotData.CFTC_Region_Code = Convert.ToDouble(dr["CFTC_Region_Code"]);
                        cotData.CFTC_Commodity_Code = Convert.ToDouble(dr["CFTC_Commodity_Code"]);
                        cotData.Open_Interest_All = Convert.ToDouble(dr["Open_Interest_All"]);
                        cotData.Prod_Merc_Positions_Long_ALL = Convert.ToDouble(dr["Prod_Merc_Positions_Long_ALL"]);
                        cotData.Prod_Merc_Positions_Short_ALL = Convert.ToDouble(dr["Prod_Merc_Positions_Short_ALL"]);
                        cotData.Swap_Positions_Long_All = Convert.ToDouble(dr["Swap_Positions_Long_All"]);
                        cotData.Swap__Positions_Short_All = Convert.ToDouble(dr["Swap__Positions_Short_All"]);
                        cotData.Swap__Positions_Spread_All = Convert.ToDouble(dr["Swap__Positions_Spread_All"]);
                        cotData.M_Money_Positions_Long_ALL = Convert.ToDouble(dr["M_Money_Positions_Long_ALL"]);
                        cotData.M_Money_Positions_Short_ALL = Convert.ToDouble(dr["M_Money_Positions_Short_ALL"]);
                        cotData.M_Money_Positions_Spread_ALL = Convert.ToDouble(dr["M_Money_Positions_Spread_ALL"]);
                        cotData.Other_Rept_Positions_Long_ALL = Convert.ToDouble(dr["Other_Rept_Positions_Long_ALL"]);
                        cotData.Other_Rept_Positions_Short_ALL = Convert.ToDouble(dr["Other_Rept_Positions_Short_ALL"]);
                        cotData.Other_Rept_Positions_Spread_ALL = Convert.ToDouble(dr["Other_Rept_Positions_Spread_ALL"]);
                        cotData.Tot_Rept_Positions_Long_All = Convert.ToDouble(dr["Tot_Rept_Positions_Long_All"]);
                        cotData.Tot_Rept_Positions_Short_All = Convert.ToDouble(dr["Tot_Rept_Positions_Short_All"]);
                        cotData.NonRept_Positions_Long_All = Convert.ToDouble(dr["NonRept_Positions_Long_All"]);
                        cotData.NonRept_Positions_Short_All = Convert.ToDouble(dr["NonRept_Positions_Short_All"]);
                        cotData.Open_Interest_Old = Convert.ToDouble(dr["Open_Interest_Old"]);
                        cotData.Prod_Merc_Positions_Long_Old = Convert.ToDouble(dr["Prod_Merc_Positions_Long_Old"]);
                        cotData.Prod_Merc_Positions_Short_Old = Convert.ToDouble(dr["Prod_Merc_Positions_Short_Old"]);
                        cotData.Swap_Positions_Long_Old = Convert.ToDouble(dr["Swap_Positions_Long_Old"]);
                        cotData.Swap_Positions_Short_Old = Convert.ToDouble(dr["Swap_Positions_Short_Old"]);
                        cotData.Swap_Positions_Spread_Old = Convert.ToDouble(dr["Swap_Positions_Spread_Old"]);
                        cotData.M_Money_Positions_Long_Old = Convert.ToDouble(dr["M_Money_Positions_Long_Old"]);
                        cotData.M_Money_Positions_Short_Old = Convert.ToDouble(dr["M_Money_Positions_Short_Old"]);
                        cotData.M_Money_Positions_Spread_Old = Convert.ToDouble(dr["M_Money_Positions_Spread_Old"]);
                        cotData.Other_Rept_Positions_Long_Old = Convert.ToDouble(dr["Other_Rept_Positions_Long_Old"]);
                        cotData.Other_Rept_Positions_Short_Old = Convert.ToDouble(dr["Other_Rept_Positions_Short_Old"]);
                        cotData.Other_Rept_Positions_Spread_Old = Convert.ToDouble(dr["Other_Rept_Positions_Spread_Old"]);
                        cotData.Tot_Rept_Positions_Long_Old = Convert.ToDouble(dr["Tot_Rept_Positions_Long_Old"]);
                        cotData.Tot_Rept_Positions_Short_Old = Convert.ToDouble(dr["Tot_Rept_Positions_Short_Old"]);
                        cotData.NonRept_Positions_Long_Old = Convert.ToDouble(dr["NonRept_Positions_Long_Old"]);
                        cotData.NonRept_Positions_Short_Old = Convert.ToDouble(dr["NonRept_Positions_Short_Old"]);
                        cotData.Open_Interest_Other = Convert.ToDouble(dr["Open_Interest_Other"]);
                        cotData.Prod_Merc_Positions_Long_Other = Convert.ToDouble(dr["Prod_Merc_Positions_Long_Other"]);
                        cotData.Prod_Merc_Positions_Short_Other = Convert.ToDouble(dr["Prod_Merc_Positions_Short_Other"]);
                        cotData.Swap_Positions_Long_Other = Convert.ToDouble(dr["Swap_Positions_Long_Other"]);
                        cotData.Swap_Positions_Short_Other = Convert.ToDouble(dr["Swap_Positions_Short_Other"]);
                        cotData.Swap_Positions_Spread_Other = Convert.ToDouble(dr["Swap_Positions_Spread_Other"]);
                        cotData.M_Money_Positions_Long_Other = Convert.ToDouble(dr["M_Money_Positions_Long_Other"]);
                        cotData.M_Money_Positions_Short_Other = Convert.ToDouble(dr["M_Money_Positions_Short_Other"]);
                        cotData.M_Money_Positions_Spread_Other = Convert.ToDouble(dr["M_Money_Positions_Spread_Other"]);
                        cotData.Other_Rept_Positions_Long_Other = Convert.ToDouble(dr["Other_Rept_Positions_Long_Other"]);
                        cotData.Other_Rept_Positions_Short_Other = Convert.ToDouble(dr["Other_Rept_Positions_Short_Other"]);
                        cotData.Other_Rept_Positions_Spread_Othr = Convert.ToDouble(dr["Other_Rept_Positions_Spread_Othr"]);
                        cotData.Tot_Rept_Positions_Long_Other = Convert.ToDouble(dr["Tot_Rept_Positions_Long_Other"]);
                        cotData.Tot_Rept_Positions_Short_Other = Convert.ToDouble(dr["Tot_Rept_Positions_Short_Other"]);
                        cotData.NonRept_Positions_Long_Other = Convert.ToDouble(dr["NonRept_Positions_Long_Other"]);
                        cotData.NonRept_Positions_Short_Other = Convert.ToDouble(dr["NonRept_Positions_Short_Other"]);
                        cotData.Change_in_Open_Interest_All = Convert.ToDouble(dr["Change_in_Open_Interest_All"]);
                        cotData.Change_in_Prod_Merc_Long_All = Convert.ToDouble(dr["Change_in_Prod_Merc_Long_All"]);
                        cotData.Change_in_Prod_Merc_Short_All = Convert.ToDouble(dr["Change_in_Prod_Merc_Short_All"]);
                        cotData.Change_in_Swap_Long_All = Convert.ToDouble(dr["Change_in_Swap_Long_All"]);
                        cotData.Change_in_Swap_Short_All = Convert.ToDouble(dr["Change_in_Swap_Short_All"]);
                        cotData.Change_in_Swap_Spread_All = Convert.ToDouble(dr["Change_in_Swap_Spread_All"]);
                        cotData.Change_in_M_Money_Long_All = Convert.ToDouble(dr["Change_in_M_Money_Long_All"]);
                        cotData.Change_in_M_Money_Short_All = Convert.ToDouble(dr["Change_in_M_Money_Short_All"]);
                        cotData.Change_in_M_Money_Spread_All = Convert.ToDouble(dr["Change_in_M_Money_Spread_All"]);
                        cotData.Change_in_Other_Rept_Long_All = Convert.ToDouble(dr["Change_in_Other_Rept_Long_All"]);
                        cotData.Change_in_Other_Rept_Short_All = Convert.ToDouble(dr["Change_in_Other_Rept_Short_All"]);
                        cotData.Change_in_Other_Rept_Spread_All = Convert.ToDouble(dr["Change_in_Other_Rept_Spread_All"]);
                        cotData.Change_in_Tot_Rept_Long_All = Convert.ToDouble(dr["Change_in_Tot_Rept_Long_All"]);
                        cotData.Change_in_Tot_Rept_Short_All = Convert.ToDouble(dr["Change_in_Tot_Rept_Short_All"]);
                        cotData.Change_in_NonRept_Long_All = Convert.ToDouble(dr["Change_in_NonRept_Long_All"]);
                        cotData.Change_in_NonRept_Short_All = Convert.ToDouble(dr["Change_in_NonRept_Short_All"]);
                        cotData.Pct_of_Open_Interest_All = Convert.ToDouble(dr["Pct_of_Open_Interest_All"]);
                        cotData.Pct_of_OI_Prod_Merc_Long_All = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Long_All"]);
                        cotData.Pct_of_OI_Prod_Merc_Short_All = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Short_All"]);
                        cotData.Pct_of_OI_Swap_Long_All = Convert.ToDouble(dr["Pct_of_OI_Swap_Long_All"]);
                        cotData.Pct_of_OI_Swap_Short_All = Convert.ToDouble(dr["Pct_of_OI_Swap_Short_All"]);
                        cotData.Pct_of_OI_Swap_Spread_All = Convert.ToDouble(dr["Pct_of_OI_Swap_Spread_All"]);
                        cotData.Pct_of_OI_M_Money_Long_All = Convert.ToDouble(dr["Pct_of_OI_M_Money_Long_All"]);
                        cotData.Pct_of_OI_M_Money_Short_All = Convert.ToDouble(dr["Pct_of_OI_M_Money_Short_All"]);
                        cotData.Pct_of_OI_M_Money_Spread_All = Convert.ToDouble(dr["Pct_of_OI_M_Money_Spread_All"]);
                        cotData.Pct_of_OI_Other_Rept_Long_All = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Long_All"]);
                        cotData.Pct_of_OI_Other_Rept_Short_All = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Short_All"]);
                        cotData.Pct_of_OI_Other_Rept_Spread_All = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Spread_All"]);
                        cotData.Pct_of_OI_Tot_Rept_Long_All = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Long_All"]);
                        cotData.Pct_of_OI_Tot_Rept_Short_All = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Short_All"]);
                        cotData.Pct_of_OI_NonRept_Long_All = Convert.ToDouble(dr["Pct_of_OI_NonRept_Long_All"]);
                        cotData.Pct_of_OI_NonRept_Short_All = Convert.ToDouble(dr["Pct_of_OI_NonRept_Short_All"]);
                        cotData.Pct_of_Open_Interest_Old = Convert.ToDouble(dr["Pct_of_Open_Interest_Old"]);
                        cotData.Pct_of_OI_Prod_Merc_Long_Old = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Long_Old"]);
                        cotData.Pct_of_OI_Prod_Merc_Short_Old = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Short_Old"]);
                        cotData.Pct_of_OI_Swap_Long_Old = Convert.ToDouble(dr["Pct_of_OI_Swap_Long_Old"]);
                        cotData.Pct_of_OI_Swap_Short_Old = Convert.ToDouble(dr["Pct_of_OI_Swap_Short_Old"]);
                        cotData.Pct_of_OI_Swap_Spread_Old = Convert.ToDouble(dr["Pct_of_OI_Swap_Spread_Old"]);
                        cotData.Pct_of_OI_M_Money_Long_Old = Convert.ToDouble(dr["Pct_of_OI_M_Money_Long_Old"]);
                        cotData.Pct_of_OI_M_Money_Short_Old = Convert.ToDouble(dr["Pct_of_OI_M_Money_Short_Old"]);
                        cotData.Pct_of_OI_M_Money_Spread_Old = Convert.ToDouble(dr["Pct_of_OI_M_Money_Spread_Old"]);
                        cotData.Pct_of_OI_Other_Rept_Long_Old = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Long_Old"]);
                        cotData.Pct_of_OI_Other_Rept_Short_Old = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Short_Old"]);
                        cotData.Pct_of_OI_Other_Rept_Spread_Old = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Spread_Old"]);
                        cotData.Pct_of_OI_Tot_Rept_Long_Old = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Long_Old"]);
                        cotData.Pct_of_OI_Tot_Rept_Short_Old = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Short_Old"]);
                        cotData.Pct_of_OI_NonRept_Long_Old = Convert.ToDouble(dr["Pct_of_OI_NonRept_Long_Old"]);
                        cotData.Pct_of_OI_NonRept_Short_Old = Convert.ToDouble(dr["Pct_of_OI_NonRept_Short_Old"]);
                        cotData.Pct_of_Open_Interest_Other = Convert.ToDouble(dr["Pct_of_Open_Interest_Other"]);
                        cotData.Pct_of_OI_Prod_Merc_Long_Other = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Long_Other"]);
                        cotData.Pct_of_OI_Prod_Merc_Short_Other = Convert.ToDouble(dr["Pct_of_OI_Prod_Merc_Short_Other"]);
                        cotData.Pct_of_OI_Swap_Long_Other = Convert.ToDouble(dr["Pct_of_OI_Swap_Long_Other"]);
                        cotData.Pct_of_OI_Swap_Short_Other = Convert.ToDouble(dr["Pct_of_OI_Swap_Short_Other"]);
                        cotData.Pct_of_OI_Swap_Spread_Other = Convert.ToDouble(dr["Pct_of_OI_Swap_Spread_Other"]);
                        cotData.Pct_of_OI_M_Money_Long_Other = Convert.ToDouble(dr["Pct_of_OI_M_Money_Long_Other"]);
                        cotData.Pct_of_OI_M_Money_Short_Other = Convert.ToDouble(dr["Pct_of_OI_M_Money_Short_Other"]);
                        cotData.Pct_of_OI_M_Money_Spread_Other = Convert.ToDouble(dr["Pct_of_OI_M_Money_Spread_Other"]);
                        cotData.Pct_of_OI_Other_Rept_Long_Other = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Long_Other"]);
                        cotData.Pct_of_OI_Other_Rept_Short_Other = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Short_Other"]);
                        cotData.Pct_of_OI_Other_Rept_Spread_Othr = Convert.ToDouble(dr["Pct_of_OI_Other_Rept_Spread_Othr"]);
                        cotData.Pct_of_OI_Tot_Rept_Long_Other = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Long_Other"]);
                        cotData.Pct_of_OI_Tot_Rept_Short_Other = Convert.ToDouble(dr["Pct_of_OI_Tot_Rept_Short_Other"]);
                        cotData.Pct_of_OI_NonRept_Long_Other = Convert.ToDouble(dr["Pct_of_OI_NonRept_Long_Other"]);
                        cotData.Pct_of_OI_NonRept_Short_Other = Convert.ToDouble(dr["Pct_of_OI_NonRept_Short_Other"]);
                        cotData.Traders_Tot_All = Convert.ToDouble(dr["Traders_Tot_All"]);
                        cotData.Traders_Prod_Merc_Long_All = Convert.ToDouble(dr["Traders_Prod_Merc_Long_All"]);
                        cotData.Traders_Prod_Merc_Short_All = Convert.ToDouble(dr["Traders_Prod_Merc_Short_All"]);
                        cotData.Traders_Swap_Long_All = Convert.ToDouble(dr["Traders_Swap_Long_All"]);
                        cotData.Traders_Swap_Short_All = Convert.ToDouble(dr["Traders_Swap_Short_All"]);
                        cotData.Traders_Swap_Spread_All = Convert.ToDouble(dr["Traders_Swap_Spread_All"]);
                        cotData.Traders_M_Money_Long_All = Convert.ToDouble(dr["Traders_M_Money_Long_All"]);
                        cotData.Traders_M_Money_Short_All = Convert.ToDouble(dr["Traders_M_Money_Short_All"]);
                        cotData.Traders_M_Money_Spread_All = Convert.ToDouble(dr["Traders_M_Money_Spread_All"]);
                        cotData.Traders_Other_Rept_Long_All = Convert.ToDouble(dr["Traders_Other_Rept_Long_All"]);
                        cotData.Traders_Other_Rept_Short_All = Convert.ToDouble(dr["Traders_Other_Rept_Short_All"]);
                        cotData.Traders_Other_Rept_Spread_All = Convert.ToDouble(dr["Traders_Other_Rept_Spread_All"]);
                        cotData.Traders_Tot_Rept_Long_All = Convert.ToDouble(dr["Traders_Tot_Rept_Long_All"]);
                        cotData.Traders_Tot_Rept_Short_All = Convert.ToDouble(dr["Traders_Tot_Rept_Short_All"]);
                        cotData.Traders_Tot_Old = Convert.ToDouble(dr["Traders_Tot_Old"]);
                        cotData.Traders_Prod_Merc_Long_Old = Convert.ToDouble(dr["Traders_Prod_Merc_Long_Old"]);
                        cotData.Traders_Prod_Merc_Short_Old = Convert.ToDouble(dr["Traders_Prod_Merc_Short_Old"]);
                        cotData.Traders_Swap_Long_Old = Convert.ToDouble(dr["Traders_Swap_Long_Old"]);
                        cotData.Traders_Swap_Short_Old = Convert.ToDouble(dr["Traders_Swap_Short_Old"]);
                        cotData.Traders_Swap_Spread_Old = Convert.ToDouble(dr["Traders_Swap_Spread_Old"]);
                        cotData.Traders_M_Money_Long_Old = Convert.ToDouble(dr["Traders_M_Money_Long_Old"]);
                        cotData.Traders_M_Money_Short_Old = Convert.ToDouble(dr["Traders_M_Money_Short_Old"]);
                        cotData.Traders_M_Money_Spread_Old = Convert.ToDouble(dr["Traders_M_Money_Spread_Old"]);
                        cotData.Traders_Other_Rept_Long_Old = Convert.ToDouble(dr["Traders_Other_Rept_Long_Old"]);
                        cotData.Traders_Other_Rept_Short_Old = Convert.ToDouble(dr["Traders_Other_Rept_Short_Old"]);
                        cotData.Traders_Other_Rept_Spread_Old = Convert.ToDouble(dr["Traders_Other_Rept_Spread_Old"]);
                        cotData.Traders_Tot_Rept_Long_Old = Convert.ToDouble(dr["Traders_Tot_Rept_Long_Old"]);
                        cotData.Traders_Tot_Rept_Short_Old = Convert.ToDouble(dr["Traders_Tot_Rept_Short_Old"]);
                        cotData.Traders_Tot_Other = Convert.ToDouble(dr["Traders_Tot_Other"]);
                        cotData.Traders_Prod_Merc_Long_Other = Convert.ToDouble(dr["Traders_Prod_Merc_Long_Other"]);
                        cotData.Traders_Prod_Merc_Short_Other = Convert.ToDouble(dr["Traders_Prod_Merc_Short_Other"]);
                        cotData.Traders_Swap_Long_Other = Convert.ToDouble(dr["Traders_Swap_Long_Other"]);
                        cotData.Traders_Swap_Short_Other = Convert.ToDouble(dr["Traders_Swap_Short_Other"]);
                        cotData.Traders_Swap_Spread_Other = Convert.ToDouble(dr["Traders_Swap_Spread_Other"]);
                        cotData.Traders_M_Money_Long_Other = Convert.ToDouble(dr["Traders_M_Money_Long_Other"]);
                        cotData.Traders_M_Money_Short_Other = Convert.ToDouble(dr["Traders_M_Money_Short_Other"]);
                        cotData.Traders_M_Money_Spread_Other = 0;
                        cotData.Traders_Other_Rept_Long_Other = Convert.ToDouble(dr["Traders_Other_Rept_Long_Other"]);
                        cotData.Traders_Other_Rept_Short_Other = Convert.ToDouble(dr["Traders_Other_Rept_Short_Other"]);
                        cotData.Traders_Other_Rept_Spread_Other = Convert.ToDouble(dr["Traders_Other_Rept_Spread_Other"]);
                        cotData.Traders_Tot_Rept_Long_Other = Convert.ToDouble(dr["Traders_Tot_Rept_Long_Other"]);
                        cotData.Traders_Tot_Rept_Short_Other = Convert.ToDouble(dr["Traders_Tot_Rept_Short_Other"]);
                        cotData.Conc_Gross_LE_4_TDR_Long_All = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Long_All"]);
                        cotData.Conc_Gross_LE_4_TDR_Short_All = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Short_All"]);
                        cotData.Conc_Gross_LE_8_TDR_Long_All = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Long_All"]);
                        cotData.Conc_Gross_LE_8_TDR_Short_All = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Short_All"]);
                        cotData.Conc_Net_LE_4_TDR_Long_All = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Long_All"]);
                        cotData.Conc_Net_LE_4_TDR_Short_All = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Short_All"]);
                        cotData.Conc_Net_LE_8_TDR_Long_All = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Long_All"]);
                        cotData.Conc_Net_LE_8_TDR_Short_All = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Short_All"]);
                        cotData.Conc_Gross_LE_4_TDR_Long_Old = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Long_Old"]);
                        cotData.Conc_Gross_LE_4_TDR_Short_Old = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Short_Old"]);
                        cotData.Conc_Gross_LE_8_TDR_Long_Old = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Long_Old"]);
                        cotData.Conc_Gross_LE_8_TDR_Short_Old = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Short_Old"]);
                        cotData.Conc_Net_LE_4_TDR_Long_Old = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Long_Old"]);
                        cotData.Conc_Net_LE_4_TDR_Short_Old = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Short_Old"]);
                        cotData.Conc_Net_LE_8_TDR_Long_Old = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Long_Old"]);
                        cotData.Conc_Net_LE_8_TDR_Short_Old = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Short_Old"]);
                        cotData.Conc_Gross_LE_4_TDR_Long_Other = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Long_Other"]);
                        cotData.Conc_Gross_LE_4_TDR_Short_Other = Convert.ToDouble(dr["Conc_Gross_LE_4_TDR_Short_Other"]);
                        cotData.Conc_Gross_LE_8_TDR_Long_Other = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Long_Other"]);
                        cotData.Conc_Gross_LE_8_TDR_Short_Other = Convert.ToDouble(dr["Conc_Gross_LE_8_TDR_Short_Other"]);
                        cotData.Conc_Net_LE_4_TDR_Long_Other = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Long_Other"]);
                        cotData.Conc_Net_LE_4_TDR_Short_Other = Convert.ToDouble(dr["Conc_Net_LE_4_TDR_Short_Other"]);
                        cotData.Conc_Net_LE_8_TDR_Long_Other = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Long_Other"]);
                        cotData.Conc_Net_LE_8_TDR_Short_Other = Convert.ToDouble(dr["Conc_Net_LE_8_TDR_Short_Other"]);
                        cotData.Contract_Units = dr["Contract_Units"].ToString();
                        cotData.CFTC_SubGroup_Code = dr["CFTC_SubGroup_Code"].ToString();
                        cotData.FutOnly_or_Combined = dr["FutOnly_or_Combined"].ToString();
                        lstCOTdata.Add(cotData);
                    }
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {
            }
            return lstCOTdata;
        }

 
        public List<COTData> GetLastUpdatedData()
        {
            return new List<COTData>();
        }

        public void UpdateData(List<UpdateInfo> lstUpdateInfo)
        {

        }
        public void PopulateData(List<COTData> lstEthanolData)
        {

        }
        public void PopulateData(DataSet cotDataSet)
        {
            UpdateCOTData("TRUNCATE TABLE COT_DIALY_DATA");
            IDbConnection dbConnection = dbHelper.OpenConnection();
           
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(dbConnection as SqlConnection))
            {
                bulkCopy.DestinationTableName = "dbo.COT_DIALY_DATA";

                try
                {
                    bulkCopy.WriteToServer(cotDataSet.Tables[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        public DataSet ExecuteSP(string SP)
        {
            dbHelper.CreateCommand(SP, CommandType.StoredProcedure);
            DataSet ds =  dbHelper.ExecuteDataSet();
            dbHelper.CloseConnection();
            return ds;
        }

        public void UpdateCOTData(string query)
        {
            try
            {
                dbHelper.CreateCommand(query);
                dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();
            }
            catch (Exception ex)
            {

            }
        }
        public DataSet GetCOTUpdatedData(string query)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
                {
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                    return ds;
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public DataSet GetCOTConfigData()
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_COT_GROUPDATA", CommandType.StoredProcedure))
                {
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                    return ds;
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }
    }
}
