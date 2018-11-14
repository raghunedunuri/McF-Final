using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class EthanolData : DialyData
    {
        public long Stock { get; set; }
        public long Planted { get; set; }
    }

    public class EthanolDisplayData : DialyData
    {
        public long EndingStock { get; set; }
        public long PlantProduction { get; set; }
    }

    public class EthanolRawData
    {
        public string StockSourceFile { get; set; }
        public string PlantedSourceFile { get; set; }
        public int NoOfDays { get; set; }
    }

    public class EthanolUpdateData 
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
