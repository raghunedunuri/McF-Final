using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.Business
{
    public class JobService : IJobService
    {
        private IJobRepository JobRepos = null;
        public JobService(IJobRepository jobRepos)
        {
            this.JobRepos = jobRepos;
        }

        //Graph
        public JobSummary GetJobSummary()
        {
            return JobRepos.GetJobSummary();
        }

         //Dashboard If User Filters
        public List<JobStatus> GetJobInfo(string DataSource, String Status, DateTime From, DateTime To)
        {
            return JobRepos.GetJobInfo(DataSource,Status,From,To);
        }

        //Each Datasource Page
        public List<JobStatus> GetCurrentJobInfo(string DataSource = null)
        {
            return JobRepos.GetCurrentJobInfo(DataSource);
        }

        //To show more details in each Datasource Page

        public JobInfo GetCurrentJobDetails(string DataSource)
        {
            return JobRepos.GetCurrentJobDetails(DataSource);
        }

        public void UpdateJobSchedule(JobInfo jobSchedule)
        {
            JobRepos.UpdateJobSchedule(jobSchedule);
        }
        public void UpdateJobParams(UpdateJobParams jobParams)
        {
            JobRepos.UpdateJobParams(jobParams);
        }

        public void UpdateJobNextScheduleTime(UpdateJobTime updateJobTime)
        {
            JobRepos.UpdateJobNextScheduleTime(updateJobTime);
        }
        public List<JobStatus> GetJobCurrentSchedule()
        {
            return JobRepos.GetJobCurrentSchedule();
        }
        public int CreateJobForRun(UpdateJobTime updateJoinTime)
        {
           return JobRepos.CreateJobForRun(updateJoinTime);
        }
        public void UpdateJobStatus(UpdateJobTime updateJoinTime)
        {
            JobRepos.UpdateJobStatus(updateJoinTime);
        }

        public List<JobInfo> GetNextRunningJobs()
        {
            return JobRepos.GetNextRunningJobs();
        }
        public List<Messages> GetMessages(string DataSource)
        {
            return JobRepos.GetMessages(DataSource);
        }
        public JobStatus GetCurrentJob(int JobID)
        {
            return new JobStatus();
        }
    }
}
