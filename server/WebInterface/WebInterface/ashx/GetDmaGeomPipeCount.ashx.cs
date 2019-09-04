using System;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using utility;

namespace WebInterface.ashx
{
    /// <summary>
    /// GetDmaGeomPipeCount 的摘要说明
    /// </summary>
    public class GetDmaGeomPipeCount : BaseAshxPage
    {
        public override void Ashx_Load()
        {
            string biaoming = "";
            CheckRequery.checkNotNull("biaoming", "", out biaoming);

            //string sql = "select count(0) from leakage.ptjsgx;";
            string sql = "select count(0) from "+ biaoming;

            string ErrInfo = string.Empty;
            DataTable dt = new DataTable();


            dt = APP.PGSQL_Helper.SelectDataTable(sql.ToString(), out ErrInfo);

            if (string.IsNullOrEmpty(ErrInfo))
            {
                Context.Response.Write(EasyUI_Pagination.ExportSuccess(dt));
            }
            else
            {
                Context.Response.Write(EasyUI_Pagination.ExportErrMsg(ErrInfo));
            }

        }
    }
}