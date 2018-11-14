using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class SugarCompaniesData
    {
        public string name { get; set; }
        public string city { get; set; }
        public string state_ { get; set; }
        public string country { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public int year { get; set; }
        public string category { get; set; }
    }
    public class Years
    {
        public int year { get; set; }
    }

    public class HFSCData
    {
        public int year { get; set; }
        public string month { get; set; }
        public float value { get; set; }
    }
    public class NorthWetMills
    {
        public string name { get; set; }
        public string city { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string status { get; set; }
    }
}
