using System;
using System.Collections.Generic;
using System.Web;
using Npgsql;
using System.Text;
using System.Data;

namespace DbHelper

{
    /// <summary>
    /// PGSQL_DBHelper 的摘要说明
    /// </summary>
    public class PGSQL_DBHelper
    {
        public PGSQL_DBHelper(string connectstring)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.ConnectString = connectstring;
        }
        private string connStr;
        public string ConnectString
        {
            get { return connStr; }
            set { connStr = value; }
        }

        public string GetSqlExceptionMessage(NpgsqlException ex)
        {
            string ERR="Index #\n" +

                    "Message: " + ex.Message + "\n" +
                    "LineNumber: " + ex.HelpLink + "\n" +
                    "Source: " + ex.Source + "\n" +
                    "Procedure: " + ex.InnerException + "\n";

            return ERR;
        }

        /// <summary>
        /// 生成并返回一个SqlConnection对象
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnection GetDbConnection()
        {
            return new NpgsqlConnection(ConnectString);
        }

        /// <summary>
        /// 生成并返回一个SqlCommand对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public NpgsqlCommand  GetDbCommand(string cmdText)
        {
            NpgsqlCommand comm = new NpgsqlCommand(cmdText, GetDbConnection());
            comm.CommandTimeout = 5000;
            return comm;
        }

        /// <summary>
        /// 执行指定的SQL语句，返回DataTable
        /// </summary>
        public DataTable SelectDataTable(string sqlString)
        {
            string err = "";
            if (!string.IsNullOrWhiteSpace(err))
            {
                throw new Exception(err);
            }
            return SelectDataTable(sqlString, out err);
        }

        /// <summary>
        /// 执行指定的SQL语句，返回DataTable,同时通过 out string ErrInf 返回错误信息
        /// </summary>
        public DataTable SelectDataTable(string sqlString, out string ErrInfo)
        {
            DataTable dt = new DataTable();
            ErrInfo = string.Empty;

            using (NpgsqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sqlString, conn);
                    adapter.Fill(dt);
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    ErrInfo = GetSqlExceptionMessage(ex);
                    conn.Close();
                }
            }

            return dt;
        }

        public bool GetDataTable(string sqlString, out DataTable dt, out string ErrInfo)
        {
            dt = SelectDataTable(sqlString, out ErrInfo);
            if (string.IsNullOrEmpty(ErrInfo))
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        /// 运行指定的DBCommand，返回DataTable,同时通过 out string ErrInf 返回错误信息
        /// </summary>
        public DataTable SelectDataTable(NpgsqlCommand command, out string ErrInfo)
        {
            DataTable dt = new DataTable();
            ErrInfo = string.Empty;

            using (command.Connection)
            {
                try
                {
                    command.Connection.Open();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    adapter.Fill(dt);

                }
                catch (NpgsqlException ex)
                {
                    ErrInfo = GetSqlExceptionMessage(ex);
                }
            }

            return dt;
        }

        /// <summary>
        /// 执行指定的Update语句，返回受影响的行数,同时通过 out string ErrInf 返回错误信息,没有错误发生的时候ErrInfo为空
        /// </summary>
        public int UpDate(string sqlString, out string ErrInfo)
        {
            ErrInfo = "";
            int execItem = 0;

            using (NpgsqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
                    execItem = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    conn.Close();
                    ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }

        public int GetSqlExecScalar(string sqlString, out string ErrInfo)
        {
            ErrInfo = "";
            int execItem = 0;

            using (NpgsqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
                    object o = cmd.ExecuteScalar();
                    if (o == DBNull.Value)
                    {
                        execItem = 0;
                    }
                    else
                    {
                        execItem = Convert.ToInt32(o);
                    }
                    cmd.Dispose();
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    conn.Close();
                    ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString)
        {
            using (NpgsqlConnection connection = GetDbConnection())
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    NpgsqlDataAdapter command = new NpgsqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                    connection.Close();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
    }

}