using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public interface IBaseService
    {
        List<FieldInfo> GetFieldInfo();

        SymbolInfo GetSymbolInfo();

        RawFilesInfo GetRawData();

        void UpdateData(List<UpdateInfo> lstUpdateInfo);
    }
}
