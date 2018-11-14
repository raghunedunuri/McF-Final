using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess
{
    public interface IBaseDataRepository
    {
        List<FieldInfo> GetFieldInfo();

        SymbolInfo GetSymbolInfo();

        RawFilesInfo GetRawData();

        void UpdateData(List<UpdateInfo> lstUpdateInfo);

        DataSet ExecuteSP(string SP);
    }
}
