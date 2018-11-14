using McF.Contracts;
using McF.DataAccess.Repositories.Interfaces;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess.Repositories.Implementors
{
    public class JobRepository : IJobRepository
    {
        private IDbHelper dbHelper = null;
        public JobRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
        //Graph
        public JobSummary GetJobSummary()
        {
            JobSummary jobSummary = new JobSummary();
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_JOB_SUMMARY", CommandType.StoredProcedure))
                {
                    IDataReader dr = dbCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        jobSummary.ScheduleToRun = Convert.ToInt32(dr["JOB_SCHEDULED"]);
                        jobSummary.CompletedJobs = Convert.ToInt32(dr["COMPLETED_JOBS"]);
                        jobSummary.Failed = Convert.ToInt32(dr["FAILED_JOBS"]);
                        jobSummary.Inprogress = Convert.ToInt32(dr["INPROGRESS_JOBS"]);
                        jobSummary.NewRecords = Convert.ToInt32(dr["NEW_RECORDS"]);
                        jobSummary.NewSymbols = Convert.ToInt32(dr["NEW_SYMBOLS"]);
                        jobSummary.UpdatedRecords = Convert.ToInt32(dr["UPDATED_RECORDS"]);
                        jobSummary.UnmappedRecords = Convert.ToInt32(dr["UNMAPPEDSYMBOLS"]);
                    }
                }
            }
            return jobSummary;
        }

        //Dashboard If User Filters
        public List<JobStatus> GetJobInfo(string DataSource, String Status, DateTime From, DateTime To)
        {

            return new List<JobStatus>();
        }

        //Each Datasource Page
        public List<JobStatus> GetCurrentJobInfo( string DataSource = null )
        {
            List<JobStatus> lstJobStatus = new List<JobStatus>();
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_LATEST_JOB_INFO", CommandType.StoredProcedure))
                {
                    if( !String.IsNullOrEmpty(DataSource))
                        dbHelper.AddParameter(dbCommand, "@JOBNAME", DataSource);

                    IDataReader dr = dbCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        JobStatus jobStatus = new JobStatus();
                        jobStatus.JobId = Convert.ToInt32(dr["ID"]);
                        jobStatus.JobName = dr["JobName"].ToString();
                        jobStatus.DataSource = dr["DATASOURCE"].ToString();
                        jobStatus.ScheduleExp = dr["SCHEDULEEXP"].ToString();
                        if (dr["NEXTSCHEDULETIME"] != DBNull.Value)
                            jobStatus.NextRunTime = Convert.ToDateTime(dr["NEXTSCHEDULETIME"]);
                        if (dr["STARTTIME"] != DBNull.Value)
                            jobStatus.StartTime = Convert.ToDateTime(dr["STARTTIME"]);
                        if (dr["ENDTIME"] != DBNull.Value)
                            jobStatus.EndTime = Convert.ToDateTime(dr["ENDTIME"]);
                        jobStatus.Status = dr["STATUS"].ToString();
                        jobStatus.Message = dr["MESSAGE"] == DBNull.Value ? String.Empty : dr["MESSAGE"].ToString();
                        jobStatus.UserID = dr["USERID"] == DBNull.Value ? -1 : Convert.ToInt32(dr["USERID"]);
                        jobStatus.NewRecords = dr["NoOFNEWRECORDS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NoOFNEWRECORDS"]);
                        jobStatus.NewSymbols = dr["NOOFNEWSYMBOLS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NOOFNEWSYMBOLS"]);
                        jobStatus.UpdatedRecords = dr["NOOFUPDATEDRECORDS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NOOFUPDATEDRECORDS"]);
                        jobStatus.UnmappedRecords = dr["NOOFUNMAPPEDSYMBOLS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["NOOFUNMAPPEDSYMBOLS"]);

                        string JobParams = dr["JOBPARAMS"] == DBNull.Value ? String.Empty : dr["JOBPARAMS"].ToString();
                        if (!String.IsNullOrEmpty(JobParams))
                        {
                            string[] strparams = JobParams.Split('|');
                            foreach (string str in strparams)
                            {
                                string[] datavalues = str.Split('=');
                                jobStatus.JobParams[datavalues[0]] = datavalues[1];
                            }
                        }
                        lstJobStatus.Add(jobStatus);
                    }
                }
            }
            return lstJobStatus;
        }

        public JobInfo GetCurrentJobDetails( string DataSource )
        {
            return new JobInfo();
        }

        public void UpdateJobSchedule(JobInfo jobSchedule)
        {
            string query = $"UPDATE ETL_JOBS_INFO SET NextScheduleTime = '{jobSchedule.NextRunTime.ToString()}', PrevRunTime = '{jobSchedule.PrevRunTime.ToString()}', UserID = {0}, LastUpdatedTime = '{DateTime.Now.ToString()}'" +
                $" where JobID = {jobSchedule.JobId}";
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GetNextRunningScheduleJob", CommandType.Text))
                {
                    dbCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateJobParams(UpdateJobParams jobParams)
        {

        }

        public void UpdateJobNextScheduleTime(UpdateJobTime updateJobTime)
        {

        }

        public List<JobInfo> GetNextRunningJobs()
        {
            List<JobInfo> lstJobInfo = new List<JobInfo>();
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GetNextRunningScheduleJob", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@ScheduleDate", DateTime.Now);
                    IDataReader dr = dbCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        string JobParams = dr["JobParams"] == DBNull.Value ? String.Empty : dr["JobParams"].ToString();
                        Dictionary<string, string> dictParams = new Dictionary<string, string>();
                        if( !String.IsNullOrEmpty(JobParams))
                        {
                            string [] strparams = JobParams.Split(';');
                            foreach(string str in strparams)
                            {
                                string[] datavalues = str.Split(':');
                                dictParams[datavalues[0]] = datavalues[1];
                            }
                        }
                        lstJobInfo.Add(new JobInfo()
                        {
                            JobId = Convert.ToInt32(dr["JobID"]),
                            JobName = dr["JobName"].ToString(),
                            DataSource = dr[" DataSource"].ToString(),
                            ScheduleExp = dr[" ScheduleExp"].ToString(),
                            Module = dr["Module"].ToString(),
                            NoOfDays = Convert.ToInt32(dr["NoOfDays"]),
                            NextRunTime = dr["NextScheduleTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["NextScheduleTime"]),
                            JobParams = dictParams
                        });
                    }
                }
            }
            return lstJobInfo;
        }
        public List<JobStatus> GetJobCurrentSchedule()
        {
            List<JobStatus> lstJobStatus = new List<JobStatus>();
            return lstJobStatus;
        }
        public int CreateJobForRun(UpdateJobTime updateJoinTime)
        {
           string query = $"INSERT INTO ETL_JOBS_RUN (JobID, Status, JobParams, UserID, NoOfNewRecords,NoOfUpdatedRecords,NoOfNewSymbols,NoOfUnmappedSymbols, ScheduledStartTime) " +
                    $"VALUES ({updateJoinTime.JobId}, 'SCHEDULED', '{updateJoinTime.JobParams}', {updateJoinTime.UserID} , 0, 0,0,0, '{DateTime.Now.ToString()}'";

            int id = 0;
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
                {
                    id = Convert.ToInt32(dbCommand.ExecuteScalar());
                }

                using (IDbCommand dbCommand = dbHelper.CreateCommand("select max(id) as ID from ETL_JOBS_RUN", CommandType.Text))
                {
                    IDataReader dr = dbCommand.ExecuteReader();
                    if (dr.Read())
                    {
                        id = Convert.ToInt32(dr["ID"]);
                    }
                }
            }
            return id;
        }
        public void UpdateJobStatus(UpdateJobTime updateJoinTime)
        {
            string query = String.Empty;

            int Id = updateJoinTime.Id;
            if (updateJoinTime.Status.ToUpper() == "SCHEDULED")
            {
                query = $"INSERT INTO ETL_JOBS_RUN (JobID, Status, JobParams, UserID, NoOfNewRecords,NoOfUpdatedRecords,NoOfNewSymbols,NoOfUnmappedSymbols) " +
                    $"VALUES ({updateJoinTime.JobId}, '{updateJoinTime.Status}', '{updateJoinTime.JobParams}', {updateJoinTime.UserID} , 0, 0,0,0)";
            }
            else if(updateJoinTime.Status.ToUpper() == "RUNNING")
            {
                query = $"UPDATE ETL_JOBS_RUN SET Status = 'RUNNING', StartTime = '{updateJoinTime.startTime.ToString()}' WHERE ID = {updateJoinTime.Id}";
            }
            else if(updateJoinTime.Status.ToUpper() == "COMPLETED")
            {
                query = $"UPDATE ETL_JOBS_RUN SET Status = 'COMPLETED', FilePath = '{updateJoinTime.FilePath}', FileType = '{updateJoinTime.FileType}'," +
                    $"Message = '{updateJoinTime.Message}', NoOfNewRecords = {updateJoinTime.NoOfNewRecords},  NoOfUpdatedRecords = {updateJoinTime.NoOfUpdatedRecords}," +
                    $"NoOfNewSymbols = {updateJoinTime.NoOfNewSymbols}, NoOfUnmappedSymbols = {updateJoinTime.NoOfUnmappedSymbols}," +
                    $"EndTime = '{updateJoinTime.endTime.Value.ToString()}'  WHERE ID = {updateJoinTime.Id}";
            }

            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
                {
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
                

        public List<Messages> GetMessages(string DataSource)
        {
            return new List<Messages>();
        }
    }
}
