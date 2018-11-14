using McF.Business;
using McF.Common;
using McF.Common.Interface;
using McF.Contracts;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCFJobService
{
    internal static class JobDriverFactory
    {
        public static IJobRunner Create(string JobType)
        {
            Type type = Type.GetType(JobType);
            if (type == null)
                throw new ApplicationException($"Cannot load the JobType: {JobType}");

            return Activator.CreateInstance(type, UnityResolver._unityContainer) as IJobRunner;
        }
    }
    public class McFJobHandler
    {
        private IJobService JobService;
        public McFJobHandler(IJobService jobService)
        {
            JobService = jobService;
        }
        public DateTime GetNextScheduleTime(string cronExpression)
        {
            CrontabSchedule cs = CrontabSchedule.Parse(cronExpression);
            DateTime startDate = DateTime.Now;
            return cs.GetNextOccurrence(startDate);
        }

        public IJobRunner GetJobRunner( string jobType )
        {
            return JobDriverFactory.Create(jobType);
        }

        public List<JobInfo> GetNextRunningJobs()
        {
            List<JobInfo> lsJobInfo = JobService.GetNextRunningJobs();
            foreach( JobInfo jb in lsJobInfo)
            {
                Thread thread = new Thread(() => RunJob(jb));
                thread.Start();
            }
            return lsJobInfo;
        }

        public void CreateJob(JobInfo jobInfo )
        {
            McFJobHandler mcfHandler = new McFJobHandler(JobService);
            IJobRunner jobRunner = mcfHandler.GetJobRunner(jobInfo.Module);
            int Id = JobService.CreateJobForRun(new UpdateJobTime()
            {
                JobId = jobInfo.JobId,
                UserID = 0
            });
            jobInfo.NextRunTime = GetNextScheduleTime(jobInfo.ScheduleExp);
            jobInfo.PrevRunTime = DateTime.Now;
            JobService.UpdateJobSchedule(jobInfo);
        }
        public void RunJob( JobInfo jobInfo )
        {
            Logger.Info($"Started running Job: {jobInfo.DataSource}");
            McFJobHandler mcfHandler = new McFJobHandler(JobService);
            IJobRunner jobRunner = mcfHandler.GetJobRunner(jobInfo.Module);
            int Id = JobService.CreateJobForRun(new UpdateJobTime()
            {
                JobId = jobInfo.JobId,
                UserID = 0
            });
            jobInfo.NextRunTime = GetNextScheduleTime(jobInfo.ScheduleExp);
            jobInfo.PrevRunTime = DateTime.Now;
            JobService.UpdateJobSchedule(jobInfo);

            Dictionary<string, string> Dict = jobInfo.JobParams;
            Dict.Add("NoOfDays", jobInfo.NoOfDays.ToString());
            jobRunner.RUNJob(Dict, jobInfo.DataSource, Id);
        }
    }
}
