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
    /// GetHiddenDanger 的摘要说明
    /// </summary>
    public class GetHiddenDanger : BaseAshxPage
    {

        public override void Ashx_Load()
        {
            string actionType = "";
            string hdId = "";
            CheckRequery.checkNotNull("actionType", "类型不能为空！", out actionType);
            switch (actionType)
            {
                case "getHiddenById":
                    //获取隐患Id
                    CheckRequery.check("hdId", out hdId);
                    getHiddenById(hdId);
                    break;
            }
        }
        protected void getHiddenById(string hdId)
        {
            string strSql = string.Format(@"  select p.cAdminName,d.lhd_xy,d.lhd_uptime,d.lhd_faxiantime,d.lhd_address,d.lhd_miaoshu,d.lhd_hiddenimage from Line_HiddenDanger d 
                                           left join P_Admin p on d.lhd_upname=p.iAdminID where d.lhd_id={0}", hdId);
            DataSet ContentList = APP.SQLServer_Helper.Query(strSql);
            Context.Response.Write(EasyUI_Pagination.ExportSuccess(ContentList.Tables[0], ContentList.Tables[0].Rows.Count));
        }
    }
}