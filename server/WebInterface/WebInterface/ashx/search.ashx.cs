using System;
using System.Web;
using System.Data;
using utility;
using Newtonsoft.Json;

namespace WebInterface.ashx
{
    /// <summary>
    /// search 的摘要说明
    /// </summary>
    public class search : BaseAshxPage
    {
        public override void Ashx_Load()
        {
            string actionType = "";
            string type = "";
            string text = "";
            CheckRequery.checkNotNull("actionType", "类型不能为空！", out actionType);
            string strSql = "";
            switch (actionType)
            {
                case "POI":
                    CheckRequery.check("text", out text);
                    strSql = string.Format("SELECT 经度,纬度,geom,st_asgeojson(geom) geometry from leakage.poi where 名称='{0}'", text);
                    DataSet ContentList = APP.PGSQL_Helper.Query(strSql);
                    Context.Response.Write(EasyUI_Pagination.ExportSuccess(ContentList.Tables[0], ContentList.Tables[0].Rows.Count));
                    break;
                case "Road":
                    CheckRequery.check("text", out text);
                    strSql = string.Format("SELECT 道路名称,geom,st_asgeojson(geom) geometry from leakage.dlzxx where 道路名称='{0}'", text);
                    ContentList = APP.PGSQL_Helper.Query(strSql);
                    Context.Response.Write(EasyUI_Pagination.ExportSuccess(ContentList.Tables[0], ContentList.Tables[0].Rows.Count));
                    break;
            }
        }
    }
}