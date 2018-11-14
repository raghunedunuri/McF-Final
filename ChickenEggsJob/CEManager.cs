using McF.Contracts;
using McF.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenEggsJob
{
    public class CEManager
    {
        private ICommonRepository CommonRepo;
        private Dictionary<string, string> DictValues { get; set; }

        public CEManager(ICommonRepository commonRepo)
        {
            CommonRepo = commonRepo;
            DictValues = new Dictionary<string, string>();
        }

        public void DeleteExistingRecords(string reportDate)
        {
            DataSet ds = CommonRepo.DeleteExistingRecords("McF_GET_CE_CHANGE_DATA", reportDate);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ReportDate = dr["Report_Date"].ToString();
                    string BomMonth = dr["BOM_Month"].ToString();
                    string DTMMonth = dr["DTM_Month"].ToString();
                    string FieldName = dr["Field_Name"].ToString();
                    string PrevValue = dr["PrevValue"].ToString();

                    string key = $"{ReportDate}|{BomMonth}|{DTMMonth}|{FieldName}";
                    DictValues[key] = PrevValue;
                }
            }
        }

        //
    }
}
