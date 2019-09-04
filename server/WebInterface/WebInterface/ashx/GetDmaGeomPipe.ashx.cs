using System;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using utility;

namespace WebInterface.ashx
{
    /// <summary>
    /// GetDmaGeomPipe 的摘要说明
    /// </summary>
    public class GetDmaGeomPipe : BaseAshxPage
    {

        public override void Ashx_Load()
        {
            //string strConn = "Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=dmaluzhou;";
            //string strConn = "Server=192.168.0.237;Port=5432;User Id=postgres;Password=sa@123;Database=postgis;";
            string iddma = "all";
            CheckRequery.check("iddma", out iddma);
            //string strcmd = "select \"gid\",\"sid\",\"dn\",ST_AsGeoJson(\"pip\".\"geom\") as Geom,ST_Length(pip.geom) as len from dmapipenet.t_pipenet_list_pipe pip group by \"pipe\".\"dn\";";

            //分页必备设置
            int pageIndex = 1, dataCount = 2000;
            string biaoming = "";
            //变量赋值
            CheckRequery.checkNotNull("pageIndex", "", out pageIndex);
            CheckRequery.checkNotNull("dataCount", "", out dataCount);
           
            CheckRequery.checkNotNull("biaoming", "", out biaoming);

            string sql = "select \"gid\",ST_AsGeoJson(\"pip\".\"geom\") as Geom,ST_Length(pip.geom) as len from " + biaoming + " pip limit " + dataCount + " offset " + (pageIndex * dataCount) + " ;";


            string strjsonbeg = "{\"type\": \"FeatureCollection\",\"crs\":{\"type\":\"name\",\"properties\": {\"name\": \"EPSG:4326\"}},\"features\": [";
            string strjsonend = "]}";
            string strjsonitembeg = "{\"type\":\"Feature\",\"geometry\": ";
            string strjsonitemmid = ",\"properties\": {\"name\": ";
            string strjsonitemend = "}},";

            string ErrInfo = string.Empty;
            //APP.PGSQL_Helper.ConnectString = strConn;


            DataTable dt = APP.PGSQL_Helper.SelectDataTable(sql, out ErrInfo);

            string geoitem = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(strjsonbeg);

            if (string.IsNullOrEmpty(ErrInfo))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //geoitem = "";
                    string x = dt.Rows[i]["Geom"].ToString();
                    //string y = dt.Rows[i]["gid"].ToString();
                    sb.Append(strjsonitembeg + dt.Rows[i]["Geom"].ToString() + strjsonitemmid + '"' + biaoming +":"+ dt.Rows[i]["gid"].ToString() + '"' + strjsonitemend);
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(strjsonend);
            }
            else
            {
                sb.Clear();
                sb.Append(strjsonbeg + strjsonend);
            }
            Context.Response.Write(sb.ToString());
        }
    }
}