using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using utility;
using System.Data;
using System.Web.Services;

namespace WebInterface.asmx
{
    /// <summary>
    /// GIS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务,请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GIS : System.Web.Services.WebService
    {

        [WebMethod]
        public string SearchForSmID(string SmID,string LayerName)
        {
            if(string.IsNullOrEmpty(SmID))
            {
                return WebExport.ExportSuccess("SmID不能为空!");
            }
            if (string.IsNullOrEmpty(LayerName))
            {
                return WebExport.ExportSuccess("LayerName!");
            }

            LayerName = LayerName.Split('@')[0];
            string SQL = "";

            switch (LayerName)
            {
                case "普通给水管线":

                    SQL = @"Select  SmID,
                                    管线编号,
                                    道路名称,
                                    管径,
                                    管长,
                                    管道材质,
                                    接口方式,
                                    起点点号,
                                    终止点号,
                                    敷设年代,
                                    竣工日期,
                                    施工单位,
                                    施工单位,
                                    起点埋深,
                                    终点埋深,
                                    平均埋深,
                                    起点高程,
                                    终点高程 From " + LayerName + " where  SmID=" + SmID;
                    break;

                case "阀门":
                    SQL = @"Select  SmID
                                    物探点号
                                    阀门编号
                                    口径,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位联系方式,
                                    施工单位,
                                    埋深,
                                    高程,
                                    开关状态,
                                    开关类型,
                                    材质,
                                    埋设方式,
                                    所在道路,
                                    道路名称,
                                    安装日期,
                                    竣工日期 From " + LayerName + " where  SmID=" + SmID;
                    break;
                case "消防栓":
                    SQL = @"Select  SmID,
                                    管径,
                                    管件口径,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位联系方式,
                                    施工单位,
                                    安装日期,
                                    竣工日期 From " + LayerName + " where  SmID=" + SmID;
                    break;
                case "阀门井":
                    SQL = @"Select  SmID,
                                    编号,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位,
                                    施工单位联系方式 From " + LayerName + " where  SmID=" + SmID;
                    break;

                case "排气阀":
                    SQL = @"Select  SmID,
                            编号,
                            施工单位,
                            道路名称 From " + LayerName + " where  SmID=" + SmID;
                    break;

                case "直通井":
                    SQL = @"Select  SmID,
                                    编号,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位,
                                    施工单位联系方式 From " + LayerName + " where  SmID=" + SmID;
                    break;
                case "四通井":
                    SQL = @"Select  SmID,
                                    编号,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位,
                                    施工单位联系方式 From " + LayerName + " where  SmID=" + SmID;
                    break;
                default:
                    SQL = @"Select  SmID,
                                    编号,
                                    井盖形状,
                                    井盖尺寸,
                                    井盖材质,
                                    井壁结构,
                                    施工单位,
                                    施工单位联系方式 From " + LayerName + " where  SmID=" + SmID;
                    break;
            }

            DataTable DT;
            string ErrInfo = "";
            bool IsOk = APP.SQLServer_BaseGisDB_Helper.GetDataTable(SQL, out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                return WebExport.ExportSuccess(DT);
            }
            else
            {
                return WebExport.ExportSuccess("查询失败!");
            }
        }

        [WebMethod]
        public string SearchForPOI(string poi_v)
        {
            if (string.IsNullOrEmpty(poi_v))
            {
                return WebExport.ExportSuccess("查询值不能为空!");
            }

            string SQL = "select top 30 * from POI where 名称 like '%" + poi_v + "%'";

            DataTable DT;
            string ErrInfo = "";
            bool IsOk = APP.SQLServer_BaseGisDB_Helper.GetDataTable(SQL, out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                return WebExport.ExportSuccess(DT);
            }
            else
            {
                return WebExport.ExportSuccess("查询失败!");
            }
        }
     }
}
