using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Common.Interface
{
    public interface IJobRunner
    {
        bool RUNJob(Dictionary<string, string> JobParams, string DataSource, int JobID);
    }
}
