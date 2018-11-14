using McF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Interfaces
{
    public interface IJobRepository
    {
        JobSummary GetJobSummary();
        //Dashboard grid
        List<JobStatus> GetJobInfo(string DataSource, String Status, DateTime From, DateTime To);
        //Each Datasource Page
        List<JobStatus> GetCurrentJobInfo(string DataSource = null);
        //To show more details in each Datasource Page
        JobInfo GetCurrentJobDetails(string DataSource);
        void UpdateJobSchedule(JobInfo jobSchedule);
        void UpdateJobParams(UpdateJobParams jobParams);
      
        List<JobStatus> GetJobCurrentSchedule();
        void UpdateJobNextScheduleTime(UpdateJobTime updateJoinTime);
        int CreateJobForRun(UpdateJobTime updateJoinTime);
        void UpdateJobStatus(UpdateJobTime updateJoinTime);
        List<JobInfo> GetNextRunningJobs();
        List<Messages> GetMessages(string DataSource);

    }
}
