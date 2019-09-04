using System;
using System.Web;
using System.Data;
//using Tool.WebAPI;
using utility;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;

namespace WebInterface.ashx
{
    /// <summary>
    /// OperationDatabase 的摘要说明
    /// </summary>
    public class OperationDatabase : BaseAshxPage
    {

        public override void Ashx_Load()
        {
            var sql="";
            //变量赋值
            CheckRequery.checkNotNull("strSql", "", out sql);

            string strSql = sql; 
            string ErrInfo = string.Empty;
            DataSet ContentList = APP.PGSQL_Helper.Query(strSql);
            Context.Response.Write(EasyUI_Pagination.ExportSuccess(ContentList.Tables[0], ContentList.Tables[0].Rows.Count));
        }
    }
}