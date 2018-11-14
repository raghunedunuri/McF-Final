using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Contracts
{
    public class JobInfo
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string JobDesc { get; set; }
        public string DataSource { get; set; }
        public string ScheduleExp { get; set; }
        public string Module { get; set; }
        public int NoOfDays { get; set; }
        public Dictionary<string, string> JobParams { get; set; }
        public DateTime? PrevRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public int UserID { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class UpdateJobSchedule
    {
        public int JobId { get; set; }
        public string ScheduleExp { get; set; }
        public int UserID { get; set; }
    }

    public class UpdateJobParams
    {
        public int JobId { get; set; }
        public Dictionary<string, string> JobParams { get; set; }
        public int UserID { get; set; }
    }

    public class JobStatus
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string ScheduleExp { get; set; }
        public Dictionary<string, string> JobParams { get; set; }
        public string DataSource { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public string Message { get; set; }
        public int NewRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int NewSymbols { get; set; }
        public int UnmappedRecords { get; set; }
        public int UserID { get; set; }
        public JobStatus()
        {
            JobParams = new Dictionary<string, string>();
        }
    }

    public class JobSummary
    {
        public int ScheduleToRun { get; set; }
        public int Inprogress { get; set; }
        public int CompletedJobs { get; set; }
        public int Failed { get; set; }
        public int NewRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int NewSymbols { get; set; }
        public int UnmappedRecords { get; set; }
    }

    public class UpdateJobTime
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string JobParams { get; set; }
        public int NoOfNewRecords { get; set; }
        public int NoOfUpdatedRecords { get; set; }
        public int NoOfNewSymbols { get; set; }
        public int NoOfUnmappedSymbols { get; set; }
        public int UserID { get; set; }

        public UpdateJobTime()
        {
            Message = String.Empty;
            FilePath = String.Empty;
            FileType = String.Empty;
            JobParams = String.Empty;
            NoOfNewRecords = 0;
            NoOfNewSymbols = 0;
            NoOfUnmappedSymbols = 0;
            NoOfUpdatedRecords = 0;
        }
    }

    public class Messages
    {
        public string DataSource
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
