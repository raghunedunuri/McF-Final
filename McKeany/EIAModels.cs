using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McKeany
{
    public class EIAUpdaterequest
    {
        public long category_id { get; set; }
        public string command { get; set; }
        public string deep { get; set; }
        public long firstrow { get; set; }
        public long rows { get; set; }
    }

    public class Rowdata
    {
        public long rows_returned { get; set; }
        public long rows_available { get; set; }
    }

    public class dataupdates
    {
        public string series_id { get; set; }
        public string updated { get; set; }
    }
    public class EIAUpdateResponse
    {
        public EIAUpdaterequest request { get; set; }
        public Rowdata data { get; set; }
        public List<dataupdates> updates { get; set; }
    }

    public class SeriesRequest
    {
        public string command { get; set; }
        public string series_id { get; set; }
    }

    public class EIASeriesRequest
    {
        public string command { get; set; }
        public string series_id { get; set; }
    }

    public class EIASeries
    {
        public string series_id { get; set; }
        public string name { get; set; }
        public string units { get; set; }
        public string f { get; set; }
        public string description { get; set; }
        public string copyright { get; set; }
        public string source { get; set; }
        public string iso3166 { get; set; }
        public string geography { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public DateTime updated { get; set; }
        public List<List<object>> data { get; set; }
    }

    public class EIASeriesInfo
    {
        public EIASeriesRequest request { get; set; }
        public List<EIASeries> series { get; set; }
    }

    public class EIASeriesFinalData
    {
        public string EIASeriesID { get; set; }
        public string EsignalSymbol { get; set; }
        public string EiAValue { get; set; }
        public string Esignalvalue { get; set; }
        public string Description { get; set; }
        public string LastUpdated { get; set; }
    }
}
