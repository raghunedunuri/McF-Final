using System.Data.SqlClient;
using System;
using System.Data;

namespace McF.DataAcess
{
    public class SqlDbHelper : IDisposable, IDbHelper
    {
        #region Variables
        private IDbConnection connection;
        private IConnectionManager ConnectionManager;
        private IDbCommand command;
        #endregion

        #region Constructor
        public SqlDbHelper(IConnectionManager connectionManager)
        {
            try
            {
                ConnectionManager = connectionManager;
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing SqlDbHelper class.", ex);
            }
        }
        #endregion
        #region PublicMethods

        #region AddParameter
        /// <summary>
        /// Adds parameter to command
        /// </summary>
        /// <param name="parameterName"></param>        
        /// <param name="value"></param>
        public void AddParameter(string parameterName, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, value);
            command.Parameters.Add(parameter);
        }
        public void AddParameter( IDbCommand dbCommand, string parameterName, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, value);
            dbCommand.Parameters.Add(parameter);
        }
        #endregion

        #region BeginTransaction
        /// <summary>
        /// Opens the database connection and begins transaction
        /// </summary>
        public void BeginTransaction()
        {
            OpenConnection();
            connection.BeginTransaction();
        }
        #endregion

        #region CloseConnection
        /// <summary>
        /// Closes the connection to the data source
        /// </summary>
        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed) connection.Close();
        }
        #endregion

        #region CommitTransaction
        /// <summary>
        /// Commits the transaction
        /// </summary>
        public void CommitTransaction()
        {
            command.Transaction.Commit();
            CloseConnection();
        }
        #endregion

        #region CreateCommand
        /// <summary>
        /// Create command object
        /// </summary>
        /// <param name="commandText"></param>
        public IDbCommand CreateCommand(string commandText)
        {
            return CreateCommand(commandText, CommandType.Text);
        }
        #endregion

        #region CreateCommand
        /// <summary>
        /// Create command object
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        public IDbCommand CreateCommand(string commandText, CommandType commandType)
        {
            OpenConnection();

           command = new SqlCommand(commandText, connection as SqlConnection) { CommandType = commandType};
            command.CommandTimeout = 30;
            return command;
        }

        public IDbCommand CreateCommand( IDbConnection sqlconnection, string commandText, CommandType commandType)
        {
            IDbCommand Sqlcommand = new SqlCommand(commandText, connection as SqlConnection) { CommandType = commandType };
            Sqlcommand.CommandTimeout = 30;
            return Sqlcommand;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Closes the connection and disposes connection and command object
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Dispose off connection object
                if (connection != null)
                {
                    CloseConnection();
                    connection.Dispose();
                }

                // Clean Up Command Object
                if (command != null)
                {
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error disposing SqlHelper class.", ex);
            }
        }
        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// Executes an SQL statement and returns the dataset.
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            try
            {
                SqlDataAdapter adpt = new SqlDataAdapter(command as SqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// Executes an SQL statement and returns the number of rows affected.
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region ExecuteReader
        /// <summary>
        /// Executes an SQL statement and returns the data reader.
        /// </summary>        
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            try
            {
                return command.ExecuteReader();
            }
            catch
            {
                CloseConnection();
                throw;
            }
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query.
        /// Additional columns or rows are ignored.
        /// </summary>        
        /// <returns></returns>
        public object ExecuteScalar()
        {
            try
            {
                return command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region OpenConnection
        /// <summary>
        /// Opens the database connection
        /// </summary>
        public IDbConnection OpenConnection()
        {
            connection = ConnectionManager.GetConnection();
            connection.Open();
            return connection;
        }
        #endregion

        #region RollbackTransaction
        /// <summary>
        /// Rollbacks the connection
        /// </summary>
        public void RollbackTransaction()
        {
            command.Transaction.Rollback();
            CloseConnection();
        }
        #endregion

        #endregion
    }
}
