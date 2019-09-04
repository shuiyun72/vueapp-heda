using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;

namespace DbHelper
{
    public class SqlDbHelper
    {
        public SqlDbHelper(string connectstring)
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

        public string GetSqlExceptionMessage(SqlException ex)
        {
            string ERR = "Index #\n" +

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
        public SqlConnection GetDbConnection()
        {
            return new SqlConnection(ConnectString);
        }

        /// <summary>
        /// 生成并返回一个SqlCommand对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public SqlCommand GetDbCommand(string cmdText)
        {
            SqlCommand comm = new SqlCommand(cmdText, GetDbConnection());
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

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlString, conn);
                    adapter.Fill(dt);
                    conn.Close();
                }
                catch (SqlException ex)
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
        public DataTable SelectDataTable(SqlCommand command, out string ErrInfo)
        {
            DataTable dt = new DataTable();
            ErrInfo = string.Empty;

            using (command.Connection)
            {
                try
                {
                    command.Connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                }
                catch (SqlException ex)
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

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
                    execItem = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    conn.Close();
                    ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }
        public int UpDate(string sqlString)
        {
            //ErrInfo = "";
            int execItem = 0;

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
                    execItem = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    conn.Close();
                    //ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }
        public int Insert(string sqlString)
        {
            //ErrInfo = "";
            int execItem = 0;

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
                    execItem = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    conn.Close();
                    //ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }
        public int Insert(SqlCommand cmd)
        {
            //ErrInfo = "";
            int execItem = 0;

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    //SqlCommand cmd = new SqlCommand(sqlString, conn);
                    cmd.Connection = conn;
                    execItem = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    conn.Close();
                    //ErrInfo = GetSqlExceptionMessage(ex);
                }
            }
            return execItem;
        }
        public int GetSqlExecScalar(string sqlString, out string ErrInfo)
        {
            ErrInfo = "";
            int execItem = 0;

            using (SqlConnection conn = GetDbConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
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
                catch (SqlException ex)
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
            using (SqlConnection connection = GetDbConnection())
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
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
