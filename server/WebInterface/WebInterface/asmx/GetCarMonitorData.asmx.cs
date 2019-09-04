using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
namespace WebInterface.asmx
{
    /// <summary>
    /// GetCarMonitorData 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GetCarMonitorData : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetCarCurrentPosition(string PlateNum)
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Results_CarPosition result = new Results_CarPosition();
            result.result = false;
            result.message = "车牌号为空!";
            List<EventType_CarPosition> EventType_CarPosition = new List<EventType_CarPosition>();
            //当车牌号不为空时进行车辆当前轨迹点查询
            if (!string.IsNullOrEmpty(PlateNum))
            {
                try
                {
                    string sql = string.Format(@" select ga.PlateNum,poss.* from 
                                                         Garage ga left join (
                                                                              SELECT  lg.gpsId,lg.deviceId,lg.locTime,lg.locLon,lg.locLat,lg.mileage,lg.speed,lg.gpsSignals    
                                                                              FROM (select max(gpsId) as gpsId from Line_Gps lg group by deviceId) pos  
                                                                              left join Line_Gps lg   on pos.gpsId = lg.gpsId)poss on poss.deviceId = ga.DeviceId where ga.PlateNum = '{0}' ", PlateNum);
                    dt = APP.SQLServer_GetCarPoss.SelectDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        EventType_CarPosition data;
                        result.result = true;
                        result.message = "成功";
                        foreach (DataRow Drow in dt.Rows)
                        {
                            data = new EventType_CarPosition();
                            data.gpsId = Drow["gpsId"].ToString();
                            data.deviceId = Drow["deviceId"].ToString();
                            data.locTime = Drow["locTime"].ToString();
                            data.locLon = Drow["locLon"].ToString();
                            data.locLat = Drow["locLat"].ToString();
                            data.mileage = Drow["mileage"].ToString();
                            data.speed = Drow["speed"].ToString();
                            data.gpsSignals = Drow["gpsSignals"].ToString();
                            EventType_CarPosition.Add(data);
                        }
                    }
                    else
                    {
                        //当登录失败后进行赋值登录错误信息
                        result.result = false;
                        result.message = "未查询到车辆轨迹信息!";
                    }
                }
                catch (Exception e)
                {
                    //异常错误信息
                    result.message = e.ToString();
                }
            }
            result.Data = EventType_CarPosition;
            return JsonConvert.SerializeObject(result);

        }
        [WebMethod]
        public string GetCarHisPosition(string PlateNum, string SearchDate)
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Results_CarPosition result = new Results_CarPosition();
            result.result = false;
            result.message = "车牌号或者查询时间为空!";
            List<EventType_CarPosition> EventType_CarPosition = new List<EventType_CarPosition>();
            //当车牌号不为空时进行车辆当前轨迹点查询
            if (!string.IsNullOrEmpty(PlateNum) && !string.IsNullOrEmpty(SearchDate))
            {
                try
                {
                    string sql = string.Format(@" select ga.PlateNum,lg.* from Garage ga
                                                      left join Line_Gps lg   on ga.deviceId = lg.DeviceId where ga.PlateNum = '{0}' and lg.locTime > '{1} 00:00:00' 
                                                      and lg.locTime < '{1} 23:59:59' order by lg.locTime asc ", PlateNum, SearchDate);
                    dt = APP.SQLServer_GetCarPoss.SelectDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        EventType_CarPosition data;
                        result.result = true;
                        result.message = "成功";
                        foreach (DataRow Drow in dt.Rows)
                        {
                            data = new EventType_CarPosition();
                            data.gpsId = Drow["gpsId"].ToString();
                            data.deviceId = Drow["deviceId"].ToString();
                            data.locTime = Drow["locTime"].ToString();
                            data.locLon = Drow["locLon"].ToString();
                            data.locLat = Drow["locLat"].ToString();
                            data.mileage = Drow["mileage"].ToString();
                            data.speed = Drow["speed"].ToString();
                            data.gpsSignals = Drow["gpsSignals"].ToString();
                            EventType_CarPosition.Add(data);
                        }
                    }
                    else
                    {
                        //当登录失败后进行赋值登录错误信息
                        result.result = false;
                        result.message = "未查询到车辆轨迹信息!";
                    }
                }
                catch (Exception e)
                {
                    //异常错误信息
                    result.message = e.ToString();
                }
            }
            result.Data = EventType_CarPosition;
            return JsonConvert.SerializeObject(result);

        }

        #region 车辆位置内部类
        internal class Results_CarPosition
        {
            public Boolean result = false;
            public String message = "失败";
            public List<EventType_CarPosition> Data = new List<EventType_CarPosition>();
        }
        /// <summary>
        /// 车辆位置内部类
        /// </summary>
        internal class EventType_CarPosition
        {
            //自增长ID
            public String gpsId;
            //GPS设备ID
            public String deviceId;
            //轨迹上传时间
            public String locTime;
            //经度
            public String locLon;
            //纬度
            public String locLat;
            //里程
            public String mileage;
            //车速
            public String speed;
            //GPS信号
            public String gpsSignals;

        }
        #endregion
    }
}
