using System;
using System;
using System.Web;
using System.Data;
using utility;
using Newtonsoft.Json;

namespace WebInterface.ashx
{
    /// <summary>
    /// GetGISDataInfo 的摘要说明
    /// </summary>
    public class GetGISDataInfo : BaseAshxPage
    {
        public override void Ashx_Load()
        {

            string Oper; //操作类型
                         
            CheckRequery.checkNotNull("actionType", "类型不能为空！", out Oper);


            switch (Oper)
            {
                case "ForSmID":
                      ForSmID();

                    break;
                case "POI":
                        GetPOI();
                    break;
            }

        }

        private void ForSmID()
        {
            string SmID, LayerName;

            //检查 and 数据赋值
            CheckRequery.checkNotNull("LayerName", "设施不能为空！", out LayerName);
            CheckRequery.checkNotNull("SmID", "SmID不能为空！", out SmID);

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
            bool IsOk = APP.SQLServer_BaseGisDB_Helper.GetDataTable(SQL,out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                Context.Response.Write(WebExport.ExportSuccess("查询成功!"));
                return;
            }
            else
            {
                Context.Response.Write(WebExport.ExportSuccess("查询失败!"));
                return;
            }

        }
        private void GetPOI()
        {
            string mc = "";
            CheckRequery.check("poi_v", out mc);
            string SQL = "select * from POI where 名称 like '%" + mc + "%'";

            DataTable DT;
            string ErrInfo = "";
            bool IsOk = APP.SQLServer_BaseGisDB_Helper.GetDataTable(SQL, out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                Context.Response.Write(WebExport.ExportSuccess("查询成功!"));
                return;
            }
            else
            {
                Context.Response.Write(WebExport.ExportSuccess("查询失败!"));
                return;
            }



        }

    }
}