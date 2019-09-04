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
    /// getPointsStatus 的摘要说明
    /// </summary>
    public class getPointsStatus : BaseAshxPage
    {

        public override void Ashx_Load()
        {
            string actionType = "";
            string lpId = "";
            CheckRequery.checkNotNull("actionType", "类型不能为空！", out actionType);
            switch (actionType)
            {
                case "getTaskStatus":
                    //获取任务Id
                    CheckRequery.check("lpId", out lpId);
                    getTaskStatus(lpId);
                    break;
            }
        }
        protected void getTaskStatus(string lpId) {
            
            string strSql = string.Format(@"select lpa.lpa_typeid,lpa.lpa_xy,lpa.lpa_id,lpa.lpa_jihuaname,lhd.lp_id as lhd_id from Line_Patrol lp
                                                    left join Line_PatrolArea lpa on lp.lp_fanwei = lpa.lpa_typeid
                                                    left join (select  distinct  lp_id,lpa_id,lpa_ids from Line_HiddenDanger group by lp_id,lpa_id,lpa_ids) lhd on lhd.lp_id = lp.lp_id and lhd.lpa_id = lpa.lpa_typeid and lhd.lpa_ids = lpa.lpa_id
                                                    where 1=1 and lp.lp_shenhe = '已审核' and lp.lp_id={0}", lpId);
            DataSet ContentList = APP.SQLServer_Helper.Query(strSql);
            Context.Response.Write(EasyUI_Pagination.ExportSuccess(ContentList.Tables[0], ContentList.Tables[0].Rows.Count));
        }
    }
}