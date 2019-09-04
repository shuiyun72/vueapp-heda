using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using WebInterface.dal;

namespace WebInterface.asmx
{
  /// <summary>
  /// Inspection 的摘要说明
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
  // [System.Web.Script.Services.ScriptService]
  public class Inspection : System.Web.Services.WebService
  {
    /// <summary>
    ///用来实现单点登录的实现
    /// </summary>
    /// <param name="name">用户名</param>
    /// <param name="personId">用户iD</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string GetUser_CheckLogin(string name, string personId)
    {
      return Data_Inspection_Dal.GetUser_CheckLogin(name, personId);
    }
    /// App用户登录验证
    /// <summary>
    /// App用户登录验证
    /// </summary>
    /// <param name="name">登录名</param>
    /// <param name="pwd">登录密码</param>
    /// <param name="smid">机器识别码</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string User_Check(string name, string pwd, string smid)
    {
      return Data_Inspection_Dal.User_CheckLogin(name, pwd, smid);
    }
    /// App和达用户登录验证
    /// <summary>
    /// App用户登录验证
    /// </summary>
    /// <param name="name">登录名</param>
    /// <param name="pwd">登录密码</param>
    /// <param name="smid">机器识别码</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string HdUser_Check(string name)
    {
      return Data_Inspection_Dal.User_CheckLogin(name, "", "");
    }
    /// 下载巡检人员的任务
    /// </summary>
    /// <param name="id">巡检人员ID</param>
    /// <param name="DateStart">任务开始时间.</param>
    /// <param name="DateEnd">任务结束时间</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string Down_Taskplan(int id, string CurrentDayDate)
    {
      return Data_Inspection_Dal.Get_Taskplan(id, CurrentDayDate);
    }
    /// 根据任务ID获取任务明细
    /// <summary>
    /// 根据任务ID获取任务明细
    /// </summary>
    /// <param name="TaskId">任务ID</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string GetTaskplanInfo(int TaskId)
    {
      return Data_Inspection_Dal.GetTaskplanInfo(TaskId);
    }
    /// 获取事件类别
    /// <summary>
    /// 获取事件类别
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"EventTypeId":"1","EventTypeName":"困难用水"},{"EventTypeId":"2","EventTypeName":"设备属性与实际不符"}]}</returns>
    [WebMethod]
    public string Get_EventType()
    {
      return Data_Inspection_Dal.Get_EventType();
    }
    ///获取事件类别小项 
    /// <summary>
    /// 获取事件类别小项
    /// </summary>
    /// <param name="ParentTypeId">事件类别ID</param>
    /// <returns>{"result":true,"message":"成功","Data":[{"EventTypeId":"1","EventTypeName":"困难用水"},{"EventTypeId":"2","EventTypeName":"设备属性与实际不符"}]}</returns>
    [WebMethod]
    public string Get_EventTypeDetail(string ParentTypeId)
    {
      return Data_Inspection_Dal.Get_EventTypeDetail(ParentTypeId);
    }
    /// 获取事件上传处理紧急程度数据
    /// <summary>
    /// 获取事件上传处理紧急程度数据
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"UrgencyId":1,"UrgencyName":"一般"},{"UrgencyId":2,"UrgencyName":"紧急"},{"UrgencyId":3,"UrgencyName":"加急"}]}</returns>
    [WebMethod]
    public string GetUrgent_Degree()
    {

      return Data_Inspection_Dal.GetUrgent_Degree();
    }
    ///获取事件处理级别数据
    /// <summary>
    /// 获取事件处理级别数据
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"HandlerLevelId":1,"HandlerLevelName":"2小时-抢险类"},{"HandlerLevelId":2,"HandlerLevelName":"4小时-正常维修"},{"HandlerLevelId":3,"HandlerLevelName":"6小时-暂缓处理"}]}</returns>
    [WebMethod]
    public string GetHandler_Level()
    {
      return Data_Inspection_Dal.GetHandler_Level();
    }
    ///事件上传接口
    /// <summary>
    /// 事件上传接口
    /// </summary>
    /// <param name="Devicename">设备/关键点名称</param>
    /// <param name="Devicesmid">设备/关键点ID</param>
    /// <param name="Uptime">上传时间</param>
    /// <param name="X">X坐标</param>
    /// <param name="Y">Y坐标</param>
    /// <param name="Longitude">经度</param>
    /// <param name="Latitude">纬度</param>
    /// <param name="PersonId">巡检员ID</param>
    /// <param name="Bae64Image">Base64图片流</param>
    /// <param name="Bae64Voice">Base64音频</param>
    /// <param name="EventId">事件id</param>
    /// <param name="EventContentId">事件内容ID</param>
    /// <param name="EventAddress">事件地址</param>
    /// <param name="Description">事件描述</param>
    /// <param name="IsHidden">是否是隐患</param>
    /// <param name="TaskId">任务ID</param>
    /// <param name="MUngercyId">紧急程度id</param>
    /// <param name="MLevelId">处理级别id</param>
    /// <param name="PointType">点类别</param>
    /// <param name="IsTemp">是否是临时事件</param>
    /// <param name="DeptId">The dept identifier.</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string HiddenTroubleReport_Insert(string Devicename, int Devicesmid, string Uptime, string X, string Y, string Longitude, string Latitude, int PersonId, string Base64Image, string Base64Voice, int EventId, int EventContentId, string EventAddress, string Description, int IsHidden, int TaskId, int MUngercyId, int MLevelId, int PointType, int IsTemp, int DeptId)
    {
      return Data_Inspection_Dal.HiddenTroubleReport_Insert(Devicename, Devicesmid, Uptime, X, Y, Longitude, Latitude, PersonId, Base64Image, Base64Voice, EventId, EventContentId, EventAddress, Description, IsHidden, TaskId, MUngercyId, MLevelId, PointType, IsTemp, DeptId);
    }
    [WebMethod]
    public string HiddenTroubleReport_Insert_Scada(string Devicename, int Devicesmid, string Uptime, string X, string Y, string Longitude, string Latitude, int PersonId, string Base64Image, string Base64Voice, int EventId, int EventContentId, string EventAddress, string Description, int IsHidden, int TaskId, int MUngercyId, int MLevelId, int PointType, int IsTemp, int DeptId, int EventFromId)
    {
      return Data_Inspection_Dal.HiddenTroubleReport_Insert(Devicename, Devicesmid, Uptime, X, Y, Longitude, Latitude, PersonId, Base64Image, Base64Voice, EventId, EventContentId, EventAddress, Description, IsHidden, TaskId, MUngercyId, MLevelId, PointType, IsTemp, DeptId, EventFromId);
    }


    /// <summary>
    /// SavEventStart 保存上传事件
    /// </summary>
    /// <param name="iAdminID">上传人ID</param>
    /// <param name="cAdminName">上传人姓名</param>
    /// <param name="iDeptID">上传人部门</param>
    /// <param name="EventFromId">事件来源</param>
    /// <param name="UrgencyId">紧急程度</param>
    /// <param name="EventTypeId">事件类型</param>
    /// <param name="EventTypeId2">事件内容</param>
    /// <param name="LinkMan">联系人</param>
    /// <param name="LinkCall">联系电话</param>
    /// <param name="EventAddress">事件地址</param>
    /// <param name="EventX"></param>
    /// <param name="EventY"></param>
    /// <param name="ExecDetpID">处理人部门</param>
    /// <param name="ExecPersonId">处理人</param>
    /// <param name="EventDesc">事件说明</param>
    /// <param name="Bae64Image">图片</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string SavEventStart(
            string iAdminID,            //上传人ID不能为空
            string cAdminName,          //上传人姓名不能为空
            string iDeptID,             //上传人部门不能为空          
            string EventFromId,         //事件来源不能为空
            string UrgencyId,           //紧急程度不能为空
            string EventTypeId,         //事件类型不能为空
            string EventTypeId2,        //事件内容不能为空
            string LinkMan,             //联系人
            string LinkCall,            //联系电话
            string EventAddress,        //事件地址
            string EventX,              //X
            string EventY,              //Y
            string ExecDetpID,          //处理人部门
            string ExecPersonId,        //处理人
            string EventDesc,            //事件说明
            string Bae64Image           //图片
        )
    {
      return Data_Inspection_Dal.SavEventStart(
              iAdminID,
              cAdminName,
              iDeptID,
              EventFromId,
              UrgencyId,
              EventTypeId,
              EventTypeId2,
              LinkMan,
              LinkCall,
              EventAddress,
              EventX,
              EventY,
              ExecDetpID,
              ExecPersonId,
              EventDesc,
              Bae64Image
          );
    }

    /// <summary>
    /// Gets the return back event list.
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string GetReturnBackEventList(string PersonId)
    {
      return Data_Inspection_Dal.GetReturnBackEventList(PersonId);
    }
    /// <summary>
    /// Gets the return back event list detail.
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="TaskId">巡检任务ID</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string GetReturnBackEventListDetail(string PersonId, string TaskId)
    {
      return Data_Inspection_Dal.GetReturnBackEventListDetail(PersonId, TaskId);
    }
    /// <summary>
    /// Hiddens the trouble report_ up again.
    /// </summary>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string HiddenTroubleReport_UpAgain(string Uptime, string Bae64Image, string Bae64Voice, int EventId, int EventContentId, string EventAddress, string Description, int MUngercyId, int MLevelId, string OperEventID, string TaskId)
    {
      return Data_Inspection_Dal.HiddenTroubleReport_UpAgain(Uptime, Bae64Image, Bae64Voice, EventId, EventContentId, EventAddress, Description, MUngercyId, MLevelId, OperEventID, TaskId);
    }
    ///接收提交考勤数据
    /// <summary>
    /// 接收提交考勤数据
    /// </summary>
    /// <param name="Lwr_PersonId">用户id</param>
    /// <param name="Lwr_Date">当前日期</param>
    /// <param name="Lwr_StartTime">签到时间</param>
    /// <param name="Lwr_EndTime">签退时间</param>
    /// <param name="Lwr_Hour">工作时长</param>
    /// <param name="Lwr_BeiZhu">备注</param>
    /// <param name="Lwr_PersonStatus">上班状态：上班下班</param>
    /// <param name="Lwr_UpTime">当前日期</param>
    /// <param name="Lwr_GpsStatus">Gps状态</param>
    /// <param name="Lwr_MobileStatus">信号状态</param>
    /// <param name="Lwr_Power">电量信息</param>
    /// <param name="Lwr_XY">经纬度</param>
    /// <param name="DeptId">部门ID</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string QianDao(string Lwr_PersonId, string Lwr_BeiZhu, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power, string Lwr_XY, int DeptId)
    {
      return Data_Inspection_Dal.QianDao(Lwr_PersonId, Lwr_BeiZhu, Lwr_GpsStatus, Lwr_MobileStatus, Lwr_Power, Lwr_XY, DeptId);
    }
    ///巡检人员上报位置
    /// <summary>
    /// 巡检人员上报位置
    /// </summary>
    /// <param name="PositionX">上报位置经度</param>
    /// <param name="PositionY">上报位置纬度</param>
    /// <param name="UpTime">位置上报时间</param>
    /// <param name="PersonId">位置上报人员</param>
    /// <param name="isOnline">是否在线0:不在线,1:在线</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string UPCoordinatePosition(string PositionX, string PositionY, string UpTime, int PersonId, int isOnline)
    {
      return Data_Inspection_Dal.UPCoordinatePosition(PositionX, PositionY, UpTime, PersonId, isOnline);
    }
    ///考勤查询
    /// <summary>
    /// 考勤查询
    /// </summary>
    /// <param name="PersonId">获取考勤人员ID</param>
    /// <param name="DateStr">获取考勤年月,格式'2017-10'</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string Get_WorkRecord(int PersonId, string DateStartStr, string DateEndStr)
    {

      return Data_Inspection_Dal.Get_WorkRecord(PersonId, DateStartStr, DateEndStr);
    }
    /// 获取公告
    /// <summary>
    /// 获取公告
    /// </summary>
    /// <returns>System.String.</returns>
    [WebMethod]
    //[WebService]
    public string Get_Notice()
    {
      return Data_Inspection_Dal.Get_Notice();
    }
    ///获取消息队列
    /// <summary>
    /// 获取消息队列
    /// </summary>
    /// <param name="PersonId">The person identifier.</param>
    /// <param name="SearchMonth">The search month.</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string Get_News_List(string PersonId, string SearchMonth)
    {
      return Data_Inspection_Dal.Get_News_List(PersonId, SearchMonth);
    }
    /// <summary>
    /// 更新消息阅读状态
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="MessageId">消息唯一标识</param>
    /// <returns>Json</returns>
    [WebMethod]
    public string UpdateNewsState(string PersonId, string MessageId)
    {
      return Data_Inspection_Dal.UpdateNewsState(PersonId, MessageId);
    }
    /// <summary>
    /// 开关阀操作日志记录接口 20180402
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="SmId">设备SmID</param>
    /// <param name="ValveOpenState">阀门开关状态</param>
    /// <param name="OperationCause">阀门操作原因</param>
    /// <param name="OperationCondition">操作状态</param>
    /// <param name="Remark">阀门备注</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string UpValveStateLog(string PersonId, int SmId, string ValveOpenState, string OperationCause, string OperationCondition, string Remark)
    {
      return Data_Inspection_Dal.UpValveStateLog(PersonId, SmId, ValveOpenState, OperationCause, OperationCondition, Remark);
    }
    #region 仅山东济宁签到使用
    ///获取下一步签到/签退步骤
    /// <summary>
    /// 获取下一步签到/签退步骤
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string GetNextSignStep(int PersonId)
    {
      return Data_Inspection_Dal.GetNextSignStep(PersonId);
    }
    [WebMethod]
    public string QianDao_JiNing(string Lwr_PersonId, string Lwr_BeiZhu, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power, string Lwr_XY, int DeptId)
    {
      return Data_Inspection_Dal.QianDao_JiNing(Lwr_PersonId, Lwr_BeiZhu, Lwr_GpsStatus, Lwr_MobileStatus, Lwr_Power, Lwr_XY, DeptId);
    }
    /// <summary>
    /// 获取签到记录 专用
    /// </summary>
    /// <param name="PersonId">The person identifier.</param>
    /// <param name="DateStartStr">The date start string.</param>
    /// <param name="DateEndStr">The date end string.</param>
    /// <returns>System.String.</returns>
    [WebMethod]
    public string Get_WorkRecord_JiNing(int PersonId, string DateStartStr, string DateEndStr)
    {

      return Data_Inspection_Dal.Get_WorkRecord_JiNing(PersonId, DateStartStr, DateEndStr);
    }

    #endregion
    #region 仙居测试使用
    [WebMethod]
    public string HiddenTroubleReport_InsertOne(string strwhere)
    {
      string sss = "{\"message\":[{\"Address\":\"497046.554,3193921.941\",\"Description\":\"\",\"HandDept\":\"\",\"HandleHuman\":\"\",\"HandleTime\":\"\",\"HandlerId\":\"0\",\"Id\":\"1707\",\"ImageInfo\":[{\"ImageName\":\"IMG_admin20170726165913.jpg\",\"ImagePath\":\"http://192.168.10.101:9091/Classes/Handler/CommonDownLoad.ashx?filePath=MobileUpload%2fIMG_admin20170726165913.jpg\"}],\"Lat\":\"497046.554\",\"Lon\":\"3193921.941\",\"ProSubName\":\"工程施工类\",\"ProTypeName\":\"工程施工类\",\"ProblemType\":\"49\",\"Process\":\"未派发\",\"Prostate\":\"未处理\",\"ProsubType\":\"53\",\"ReportDept\":\"管理员\",\"ReportTime\":\"2017-07-26 16:59:41\",\"ReportUser\":\"管理员\",\"ReportuserId\":\"1\"},{\"Address\":\"艺城中路1号\",\"Description\":\"\",\"HandDept\":\"管理员\",\"HandleHuman\":\"管理员\",\"HandleTime\":\"2017-07-27 08:31:00\",\"HandlerId\":\"1\",\"Id\":\"1708\",\"ImageInfo\":[{\"ImageName\":\"IMG_admin20170727082928.jpg\",\"ImagePath\":\"http://192.168.10.101:9091/Classes/Handler/CommonDownLoad.ashx?filePath=MobileUpload%2fIMG_admin20170727082928.jpg\"}],\"Lat\":\"496744.682\",\"Lon\":\"3194258.382\",\"ProSubName\":\"阀门维修\",\"ProTypeName\":\"工程施工类\",\"ProblemType\":\"49\",\"Process\":\"已处理\",\"Prostate\":\"已处理\",\"ProsubType\":\"52\",\"ReportDept\":\"管理员\",\"ReportTime\":\"2017-07-27 08:30:09\",\"ReportUser\":\"管理员\",\"ReportuserId\":\"1\"},{\"Address\":\"晨曦路\",\"Description\":\"还有一个DN100阀门，后面还有110PE和水泥管\",\"HandDept\":\"\",\"HandleHuman\":\"\",\"HandleTime\":\"\",\"HandlerId\":\"0\",\"Id\":\"1709\",\"ImageInfo\":null,\"Lat\":\"499953.378\",\"Lon\":\"3193169.822\",\"ProSubName\":\"设备定位\",\"ProTypeName\":\"信息GIS类\",\"ProblemType\":\"27\",\"Process\":\"未派发\",\"Prostate\":\"未处理\",\"ProsubType\":\"29\",\"ReportDept\":\"仙居县自来水供应分公司\",\"ReportTime\":\"2017-08-03 09:40:53\",\"ReportUser\":\"王柯瑜\",\"ReportuserId\":\"1225\"},{\"Address\":\"498651.348,3192183.570\",\"Description\":\"根据现场抢修发现为D160PE，请核实。\",\"HandDept\":\"\",\"HandleHuman\":\"\",\"HandleTime\":\"\",\"HandlerId\":\"0\",\"Id\":\"1710\",\"ImageInfo\":[{\"ImageName\":\"mmexport1506062147176.jpg\",\"ImagePath\":\"http://192.168.10.101:9091/Classes/Handler/CommonDownLoad.ashx?filePath=MobileUpload%2fmmexport1506062147176.jpg\"}],\"Lat\":\"498651.348\",\"Lon\":\"3192183.570\",\"ProSubName\":\"管道维修\",\"ProTypeName\":\"工程施工类\",\"ProblemType\":\"49\",\"Process\":\"未派发\",\"Prostate\":\"未处理\",\"ProsubType\":\"51\",\"ReportDept\":\"仙居县自来水供应分公司\",\"ReportTime\":\"2017-09-22 14:36:46\",\"ReportUser\":\"王柯瑜\",\"ReportuserId\":\"1225\"}],\"result\":\"true\"}";
      return sss;
    }
    #endregion
  }
}
