using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;
using WebInterface.Model;
using NLog;

namespace WebInterface.dal
{
  public static class Data_Inspection_Dal
  {
    static readonly Logger _logger = LogManager.GetLogger("Data_Inspection_Dal");

    //获取当前接口使用的坐标类别:1:84经纬度 2:84魔卡托平面坐标
    public static string Map_Type = ConfigurationManager.AppSettings["Map_Type"].ToString();
    #region 数据处理方法
    public static string GetUser_CheckLogin(string name, string personId)
    {
      Results_Login Results_Login = new Results_Login();
      Data_Logion Data_Logion = new Data_Logion();
      try
      {
        string sql = string.Format(@"select iAdminID as PersonId, cAdminName as PersonName,Smid
                                             from P_Admin where 1=1 and cAdminName='{0}' and iAdminID='{1}'", name, personId);
        DataTable dt = new DataTable();
        dt = APP.SQLServer_PingTai.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          Results_Login.result = true;
          Results_Login.message = "成功";
          Data_Logion.Smid = dt.Rows[0]["Smid"].ToString();
          Results_Login.Data.Add(Data_Logion);
        }
        else
        {
          Results_Login.result = false;
          Results_Login.message = "未查询到该用户登录信息!";
        }

      }
      catch (Exception ex)
      {
        Results_Login.result = false;
        Results_Login.message = ex.ToString();
      }
      return JsonConvert.SerializeObject(Results_Login);


    }
    /// <summary>
    /// 获取消息队列
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="SearchMonth">查询月份格式:yyyy-MM</param>
    /// <returns>Json</returns>
    public static string Get_News_List(string PersonId, string SearchMonth)
    {

      //初始化返回结果类
      NewsResult NewsResult = new NewsResult();
      NewsResult.result = false;
      NewsResult.message = "失败!";
      //初始化消息队列
      List<NewsContentList> NewsContentList = new List<NewsContentList>();
      try
      {
        //初始化开始时间
        string strStartDate = SearchMonth + "-01 00:00:00";
        //初始化结束时间
        string strEndDate = Convert.ToDateTime(SearchMonth + "-01").AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59";
        //初始化查询sql
        string strSqlSelect = string.Format(@" select MessageId,MessageInfo,BoolReadStatus from L_Message where 1=1 and PersonId = {0} and SendTime > '{1}' and SendTime < '{2}' ", PersonId, strStartDate, strEndDate);
        //初始化查询结果集
        DataTable dtMessage = new DataTable();
        //执行数据库查询
        dtMessage = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
        //当数据结果集存在时进行处理
        if (dtMessage.Rows.Count > 0)
        {
          NewsResult.result = true;
          NewsResult.message = "查询成功!";
          for (int i = 0; i < dtMessage.Rows.Count; i++)
          {
            NewsContentList news = new NewsContentList();
            news.MessageId = dtMessage.Rows[i]["MessageId"].ToString();
            news.MessageInfo = dtMessage.Rows[i]["MessageInfo"].ToString();
            news.BoolReadStatus = dtMessage.Rows[i]["BoolReadStatus"].ToString();
            NewsContentList.Add(news);
          }

        }
        else
        {
          NewsResult.result = true;
          NewsResult.message = "无消息!";
        }
      }
      catch (Exception ex)
      {
        NewsResult.result = false;
        NewsResult.message = ex.ToString();
      }
      NewsResult.Data = NewsContentList;
      return JsonConvert.SerializeObject(NewsResult);
    }
    /// <summary>
    /// 更新消息阅读状态
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="MessageId">消息唯一标识</param>
    /// <returns>Json</returns>
    public static string UpdateNewsState(string PersonId, string MessageId)
    {

      //初始化返回结果类
      UpdateNewsStateResult UpdateNewsStateResult = new UpdateNewsStateResult();
      UpdateNewsStateResult.result = false;
      UpdateNewsStateResult.message = "失败!";
      try
      {
        //初始化查询sql
        string strSqlSelect = string.Format(@" update L_Message set BoolReadStatus = 1 where PersonId = {0} and MessageId = {1} ", PersonId, MessageId);
        //初始化查询结果集
        int result = 0;
        //执行数据库查询
        result = APP.SQLServer_Helper.UpDate(strSqlSelect);
        //当数据结果集存在时进行处理
        if (result > 0)
        {
          UpdateNewsStateResult.result = true;
          UpdateNewsStateResult.message = "成功!";


        }
        else
        {
          UpdateNewsStateResult.result = false;
          UpdateNewsStateResult.message = "失败!";
        }
      }
      catch (Exception ex)
      {
        UpdateNewsStateResult.result = false;
        UpdateNewsStateResult.message = ex.ToString();
      }
      return JsonConvert.SerializeObject(UpdateNewsStateResult);
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
    public static string UpValveStateLog(string PersonId, int SmId, string ValveOpenState, string OperationCause, string OperationCondition, string Remark)
    {
      //第一步:查询出操作人员中文名称

      Results_EventUpload result = new Results_EventUpload();
      try
      {
        string strSqlSelect = string.Format(@" select PersonName from L_Person  where PersonId = {0} ", PersonId);
        DataTable dtSelect = new DataTable();
        dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
        string lurutime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (dtSelect.Rows.Count <= 0)
        {
          result.result = false;
          result.message = "查询巡检人员名称失败!";
        }
        else
        {
          string strSqlInsert = string.Format("insert into valve_Manage(vm_famenid,vm_lururen,vm_lurutime,vm_caozuoren,vm_caozuoleixing,vm_caozuoyuanyin,vm_caozuozhuangkuang,vm_beizhu) values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
          , SmId, dtSelect.Rows[0]["PersonName"], lurutime, dtSelect.Rows[0]["PersonName"], ValveOpenState, OperationCause, OperationCondition, Remark);
          int intResult = 0;
          intResult = APP.SQLServer_BaseGisDB_Helper.Insert(strSqlInsert);
          if (intResult > 0)
          {
            result.result = true;
            result.message = "阀门开关阀操作日志添加成功!";
          }
          else
          {
            result.result = false;
            result.message = "阀门开关阀操作日志添加失败!";
          }
        }
      }
      catch (Exception ex)
      {
        result.result = false;
        result.message = ex.ToString();
      }
      return JsonConvert.SerializeObject(result);
    }
    ///用户登录验证方法体
    /// <summary>
    /// 用户登录验证方法体
    /// </summary>
    /// <param name="name">登录名</param>
    /// <param name="pwd">登录密码</param>
    /// <param name="smid">手机机器码</param>
    /// <returns>登录结果0:失败,1:成功</returns>
    public static string User_CheckLogin(string name, string pwd, string smid)
    {
      DataTable dt = null;
      DataTable dt1 = null;
      try
      {
        string sql = "";
        if (!string.IsNullOrEmpty(pwd))
          sql = string.Format(@"select iAdminID as PersonId,iDeptID as DepartmentId, cAdminName as PersonName,cAdminPassWord as PassWord,cDepName as DeptName,'巡检员' RoleName,Smid,1 as IsEdit,1 as  UpMeter,iRoleID  
                                             from V_P_Admin where cAdminName='{0}' and cAdminPassWord='{1}'", name, pwd);
        else
          sql = string.Format(@"select top (1) iAdminID as PersonId,iDeptID as DepartmentId, cAdminName as PersonName,cAdminPassWord as PassWord,cDepName as DeptName,'巡检员' RoleName,Smid,1 as IsEdit,1 as  UpMeter,iRoleID  
                                             from V_P_Admin where cAdminName='{0}' ", name);
        dt = APP.SQLServer_PingTai.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          switch (dt.Rows[0]["iRoleID"].ToString())
          {
            case "1":
              dt.Rows[0]["RoleName"] = "管理员";
              break;
            case "2":
              dt.Rows[0]["RoleName"] = "巡检员";
              break;
            case "3":
              dt.Rows[0]["RoleName"] = "抄表员";
              break;
            case "4":
              dt.Rows[0]["RoleName"] = "养护员";
              break;
          }
          dt1 = new DataTable("user");
          dt1.Columns.Add("PersonId", typeof(int));
          dt1.Columns.Add("DeptId", typeof(int));
          dt1.Columns.Add("PersonName", typeof(string));
          dt1.Columns.Add("PassWord", typeof(string));
          dt1.Columns.Add("DeptName", typeof(string));
          dt1.Columns.Add("RoleName", typeof(string));
          dt1.Columns.Add("Smid", typeof(string));
          dt1.Columns.Add("IsEdit", typeof(int));
          dt1.Columns.Add("UpMeter", typeof(int));
          dt1.Columns.Add("iRoleID", typeof(int));
          DataRow newRow;
          newRow = dt1.NewRow();
          newRow["PersonId"] = dt.Rows[0]["PersonId"];
          newRow["DeptId"] = dt.Rows[0]["DepartmentId"];
          newRow["PersonName"] = dt.Rows[0]["PersonName"].ToString();
          newRow["PassWord"] = dt.Rows[0]["PassWord"].ToString();
          newRow["DeptName"] = dt.Rows[0]["DeptName"].ToString();
          newRow["RoleName"] = dt.Rows[0]["RoleName"].ToString();
          newRow["Smid"] = dt.Rows[0]["Smid"].ToString();
          newRow["IsEdit"] = dt.Rows[0]["IsEdit"];
          newRow["UpMeter"] = dt.Rows[0]["UpMeter"];
          newRow["iRoleID"] = dt.Rows[0]["iRoleID"];
          dt1.Rows.Add(newRow);
          //将新的smid存入数据库
          if (smid != "" && smid != null)
          {
            //sql = string.Format("update L_Person set Smid='{0}' where PersonId={1}", smid, dt.Rows[0]["PersonId"].ToString());
            ////int i = db.OperateDB(sql);
            //int i = APP.SQLServer_Helper.UpDate(sql);
            sql = string.Format("update P_Admin set Smid='{0}' where iAdminID={1}", smid, dt.Rows[0]["PersonId"].ToString());
            int i = APP.Menu_Helper.UpDate(sql);
          }
        }
        else
        {
          dt1 = new DataTable("user");
          dt1.Columns.Add("IsSuccess", typeof(string));
          DataRow newRow;
          newRow = dt1.NewRow();
          newRow["IsSuccess"] = "false";
          dt1.Rows.Add(newRow);
        }
      }
      catch (Exception)
      {
      }
      return JsonTo.ToJson(dt1);
    }
    ///按照用户ID获取任务列表
    /// <summary>
    /// 按照用户ID获取任务列表
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="DateStart">任务开始时间</param>
    /// <param name="DateEnd">任务结束时间</param>
    /// <returns>System.String.</returns>
    public static string Get_Taskplan(int id, string CurrentDayDate)
    {
      //初始化数据结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results_TaskList result = new Results_TaskList();
      List<Data_TaskList> EventType_Record = new List<Data_TaskList>();
      //按照用户id,日期进行查询巡检任务列表
      try
      {
        string sql = string.Format(@"SELECT lt.TaskId,lpl.PlanAreaId,lt.OperateDate,lt.VisitStarTime,lt.VisitOverTime,lt.Descript as Remark,lp.PersonName,lp.PersonId,lt.TaskName,lt.Finish,lpt.PlanTypeName,lpl.BoolFeedBack
                                             FROM L_Person lp 
                                             INNER JOIN L_Task lt on lt.ProraterId = lp.PersonId
                                             left join L_PLAN lpl on lpl.PlanId = lt.PlanId
                                             left join L_PLANTYPE lpt on lpt.PlanTypeId = lpl.PlanTypeId
                                             where 1=1 and lt.TaskState !=0 and lt.AssignState = 1 and lp.PersonId ='{1}' and convert(date,lt.VisitOverTime,110) >= '{0}' and convert(date,lt.VisitStarTime,110) <='{0}'
                                            ", DateTime.Parse(CurrentDayDate).ToString("yyyy-MM-dd"), id);
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          Data_TaskList data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new Data_TaskList();
            data.TaskId = int.Parse(Drow["TaskId"].ToString());
            data.PlanAreaId = int.Parse(Drow["PlanAreaId"].ToString());
            data.OperateDate = Drow["OperateDate"].ToString();
            data.VisitStarTime = Drow["VisitStarTime"].ToString();
            data.VisitOverTime = Drow["VisitOverTime"].ToString();

            data.Remark = Drow["Remark"].ToString();
            data.PersonName = Drow["PersonName"].ToString();
            data.PersonId = int.Parse(Drow["PersonId"].ToString());
            data.TaskName = Drow["TaskName"].ToString();
            data.Finish = int.Parse((string.IsNullOrEmpty(Drow["Finish"].ToString()) ? "0" : Drow["Finish"].ToString()));

            data.PlanTypeName = Drow["PlanTypeName"].ToString();
            data.BoolFeedBack = int.Parse(Drow["BoolFeedBack"].ToString());
            EventType_Record.Add(data);
          }
        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到记录!";
        }
      }
      catch (Exception e)
      {
        result.result = false;
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Record;
      return JsonConvert.SerializeObject(result);
    }
    ///按照任务ID获取任务明细
    /// <summary>
    /// 按照任务ID获取任务明细
    /// </summary>
    /// <param name="TaskId">任务ID</param>
    /// <returns>System.String.</returns>
    public static string GetTaskplanInfo(int TaskId)
    {
      //初始化返回结果集对象
      Results_TaskInfo result = new Results_TaskInfo();
      //关键点队列对象创建
      List<Data_TaskInfo_ImportPoint> ImportPoint = new List<Data_TaskInfo_ImportPoint>();
      //设备实体队列对象创建
      List<Data_TaskInfo_EquPoint> EquPoint = new List<Data_TaskInfo_EquPoint>();
      //巡检任务详情
      List<Data_TaskInfo> TaskInfo = new List<Data_TaskInfo>();

      try
      {
        //第一步:获取到该计划的计划类别
        //1-1:查询任务计划类别的sql拼接
        string sqlPlanType = string.Format(@" select lp.PlanTypeId,lp.BoolFeedBack from L_Task lt 
                                                  left join L_PLAN lp on lp.PlanId = lt.PlanId  where 1=1 and  lt.TaskId = '{0}' ", TaskId);
        //1-2:初始化计划类别结果集
        DataTable dtPlanType = new DataTable();
        //1-3:查询计划类别结果集
        dtPlanType = APP.SQLServer_Helper.SelectDataTable(sqlPlanType);
        //1-4:判断是否查询到计划类别ID
        if (dtPlanType.Rows.Count <= 0)
        {
          result.result = false;
          result.message = "未查询到任务的计划类别!";
        }
        else
        {
          //第二步按照计划类别查询关键点数据集,设备数据集
          string PlanType = dtPlanType.Rows[0]["PlanTypeId"].ToString();
          string BoolFeedBack = dtPlanType.Rows[0]["BoolFeedBack"].ToString();//该巡检任务是否需要反馈 0:不需要反馈 1:需要反馈
                                                                              //2-1:路线巡检查询关键点
          Data_TaskInfo_ImportPoint ImportData;
          if (PlanType == "2")
          {
            string strWhereLuXian = string.Empty;//路线关键点查询条件
                                                 //需要反馈
            if (BoolFeedBack == "1")
            {
              strWhereLuXian = string.Format(@" and (ltc.IsFeedback is null or ltc.IsFeedback = '0') ");
            }
            DataTable dtPlanLine = new DataTable();
            string sqlSelectImport_Plan = string.Format(@" select pl.X,pl.Y,ltc.ID,pl.ImportPointName,pl.PlanLineDetaiId,1 as PointType  from PlanLineDetail pl
                                                                       left join L_PlanLine lp on pl.PlanLineId = lp.PlanLineId
                                                                       left join L_PLAN lpl on lpl.PlanLineId = lp.PlanLineId
                                                                       left join L_Task lt on lt.PlanId = lpl.PlanId
                                                                       left join L_Task_CompleteDetail ltc on ltc.Devicesmid = pl.PlanLineDetaiId and ltc.PointType = 2 and ltc.TaskId = lt.TaskId 
                                                                       where 1=1 and lt.TaskId = '{0}'  and pl.ImportPointType = 1 {1} ", TaskId, strWhereLuXian);
            dtPlanLine = APP.SQLServer_Helper.SelectDataTable(sqlSelectImport_Plan);
            if (dtPlanLine.Rows.Count > 0)
            {

              foreach (DataRow drow in dtPlanLine.Rows)
              {
                ImportData = new Data_TaskInfo_ImportPoint();
                ImportData.ImportPointId = int.Parse(drow["PlanLineDetaiId"].ToString());
                ImportData.ImportPointName = drow["ImportPointName"].ToString();
                if (BoolFeedBack == "1")
                {
                  //ImportData.PatroState = 0;
                  ImportData.PatroState = string.IsNullOrEmpty(drow["ID"].ToString()) ? 0 : 1;
                }
                else
                {
                  ImportData.PatroState = string.IsNullOrEmpty(drow["ID"].ToString()) ? 0 : 1;
                }
                ImportData.PointType = 2;
                ImportData.X = drow["X"].ToString();
                ImportData.Y = drow["Y"].ToString();
                ImportPoint.Add(ImportData);
              }
            }
          }
          //2-2:区域巡检查询关键点
          else if (PlanType == "1")
          {
            string strWhereQuYu = string.Empty;//路线关键点查询条件
                                               //需要反馈
            if (BoolFeedBack == "1")
            {
              strWhereQuYu = string.Format(@" and (ltc.IsFeedback is null or ltc.IsFeedback = '0') ");
            }
            DataTable dtPlanArea = new DataTable();
            string sqlSelectImport_Area = string.Format(@" select pl.PointX,pl.PointY,ltc.ID,pl.PointName,pl.PointId from PointAreaInfo pl
                                                                       left join L_PlanArea lp on pl.PlanAreaId = lp.PlanAreaId
                                                                       left join L_PLAN lpl on lpl.PlanAreaId = lp.PlanAreaId
                                                                       left join L_Task lt on lt.PlanId = lpl.PlanId
                                                                       left join L_Task_CompleteDetail ltc on ltc.Devicesmid = pl.PointId and ltc.PointType = 1 and ltc.TaskId = lt.TaskId 
                                                                       where 1=1 and lt.TaskId = '{0}' {1} ", TaskId, strWhereQuYu);
            dtPlanArea = APP.SQLServer_Helper.SelectDataTable(sqlSelectImport_Area);

            if (dtPlanArea.Rows.Count > 0)
            {

              foreach (DataRow drow in dtPlanArea.Rows)
              {
                ImportData = new Data_TaskInfo_ImportPoint();
                ImportData.ImportPointId = int.Parse(drow["PointId"].ToString());
                ImportData.ImportPointName = drow["PointName"].ToString();
                if (BoolFeedBack == "1")
                {
                  //ImportData.PatroState = 0;
                  ImportData.PatroState = string.IsNullOrEmpty(drow["ID"].ToString()) ? 0 : 1;
                }
                else
                {
                  ImportData.PatroState = string.IsNullOrEmpty(drow["ID"].ToString()) ? 0 : 1;
                }

                ImportData.PointType = 1;
                ImportData.X = drow["PointX"].ToString();
                ImportData.Y = drow["PointY"].ToString();
                ImportPoint.Add(ImportData);
              }
            }
          }
          string strWhereEqu = string.Empty;//路线关键点查询条件
                                            //需要反馈
          if (BoolFeedBack == "1")
          {
            strWhereEqu = string.Format(@" and (ltc.IsFeedback is null or ltc.IsFeedback = '0') ");
          }
          //2-3:设备实体查询
          Data_TaskInfo_EquPoint EquData;
          DataTable dtSelectEqu = new DataTable();
          string sqlSelectEqu_Task = string.Format(@"   select lp.SmX,lp.SmY,lp.SmID,lp.EquType, 0 as PointType,ltc.ID from  L_PlanEquipmentDetail lp
                                                                        left join  L_PLAN pl on pl.PlanId = lp.PlanID
                                                                        left join L_Task lt on lt.PlanId = pl.PlanId
                                                                        left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lp.SmID and ltc.PointType = 0 and ltc.TaskId = lt.TaskId 
                                                                        where 1=1 and lt.TaskId = '{0}' {1} ", TaskId, strWhereEqu);
          dtSelectEqu = APP.SQLServer_Helper.SelectDataTable(sqlSelectEqu_Task);
          if (dtSelectEqu.Rows.Count > 0)
          {

            foreach (DataRow drow in dtSelectEqu.Rows)
            {
              EquData = new Data_TaskInfo_EquPoint();
              EquData.EquType = drow["EquType"].ToString();
              if (BoolFeedBack == "1")
              {
                EquData.PatroState = 0;
              }
              else
              {
                EquData.PatroState = string.IsNullOrEmpty(drow["ID"].ToString()) ? 0 : 1;
              }
              EquData.PointType = 0;
              EquData.Smid = int.Parse(drow["SmID"].ToString());
              EquData.X = drow["SmX"].ToString();
              EquData.Y = drow["SmY"].ToString();
              EquPoint.Add(EquData);
            }
          }
          //2-4:巡检区域任务主体查询
          Data_TaskInfo TaskInfoData;
          if (PlanType == "1")
          {
            DataTable dtSelectTaskInfo = new DataTable();
            string sqlSelectTaskInfo = string.Format(@"select lpa.GeoText,lpa.PlanAreaName,lp.PlanPath,'' as PlanPathName  from L_Task lt 
                                                                   left join L_PLAN lp on lp.PlanId = lt.PlanId 
                                                                   left join L_PlanArea lpa on lpa.PlanAreaId = lp.PlanAreaId
                                                                   where 1=1 and lt.TaskId = '{0}' ", TaskId);
            dtSelectTaskInfo = APP.SQLServer_Helper.SelectDataTable(sqlSelectTaskInfo);
            if (dtSelectTaskInfo.Rows.Count > 0)
            {

              foreach (DataRow drow in dtSelectTaskInfo.Rows)
              {
                TaskInfoData = new Data_TaskInfo();
                TaskInfoData.PlanAreaGeoText = drow["GeoText"].ToString();
                TaskInfoData.PlanAreaName = drow["PlanAreaName"].ToString();
                TaskInfoData.PlanPathGeoText = drow["PlanPath"].ToString();
                TaskInfoData.PlanPathName = string.Empty;
                TaskInfo.Add(TaskInfoData);
              }
            }
          }
          //2-5:巡检路线任务主体查询
          else if (PlanType == "2")
          {
            DataTable dtSelectTaskInfo = new DataTable();
            string sqlSelectTaskInfo = string.Format(@" select lpl.PatroGeoText,'' as PlanAreaName ,lpl.PlanLineName,lpl.GeoText  from L_Task lt 
                                                                    left join  L_PLAN pl on pl.PlanId = lt.PlanID
                                                                    left join L_PlanLine lpl on lpl.PlanLineId = pl.PlanLineId
                                                                    where 1=1 and lt.TaskId = '{0}' ", TaskId);
            dtSelectTaskInfo = APP.SQLServer_Helper.SelectDataTable(sqlSelectTaskInfo);
            if (dtSelectTaskInfo.Rows.Count > 0)
            {

              foreach (DataRow drow in dtSelectTaskInfo.Rows)
              {
                TaskInfoData = new Data_TaskInfo();
                TaskInfoData.PlanAreaGeoText = drow["PatroGeoText"].ToString();
                TaskInfoData.PlanAreaName = string.Empty;
                TaskInfoData.PlanPathGeoText = drow["GeoText"].ToString();
                TaskInfoData.PlanPathName = drow["PlanLineName"].ToString();
                TaskInfo.Add(TaskInfoData);
              }
            }
          }

        }
        result.result = true;
        result.message = "查询成功!";
        result.Data = TaskInfo;
        result.EquPointData = EquPoint;
        result.ImportPointData = ImportPoint;
      }
      catch (Exception e)
      {
        result.result = false;
        result.message = e.ToString();
      }


      //将结果对象转化为json对象进行返回
      return JsonConvert.SerializeObject(result);
    }
    ///获取事件类别
    /// <summary>
    /// 获取事件类别
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"EventTypeId":"1","EventTypeName":"困难用水"},{"EventTypeId":"2","EventTypeName":"设备属性与实际不符"}]}</returns>
    public static string Get_EventType()
    {

      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results result = new Results();
      List<EventType> EventTypeData = new List<EventType>();
      try
      {
        string sql = string.Format(@" SELECT  EventTypeId,EventTypeName,ParentTypeId
                                                  FROM M_EventType me where me.ParentTypeId = 0 ");
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new EventType();
            data.EventTypeId = int.Parse(Drow["EventTypeId"].ToString());
            data.EventTypeName = Drow["EventTypeName"].ToString();
            EventTypeData.Add(data);
          }

        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到事件类别!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventTypeData;
      return JsonConvert.SerializeObject(result);

    }
    ///获取紧急类别数据
    /// <summary>
    /// 获取紧急类别数据
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"UrgencyId":1,"UrgencyName":"一般"},{"UrgencyId":2,"UrgencyName":"紧急"},{"UrgencyId":3,"UrgencyName":"加急"}]}</returns>
    public static string GetUrgent_Degree()
    {

      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results_Urgent_Degree result = new Results_Urgent_Degree();
      List<EventType_Urgent_Degree> EventType_Urgent_Degree = new List<EventType_Urgent_Degree>();
      try
      {
        string sql = string.Format(@" SELECT  UrgencyId,UrgencyName
                                                  FROM M_Urgency where 1=1 ");
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType_Urgent_Degree data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new EventType_Urgent_Degree();
            data.UrgencyId = int.Parse(Drow["UrgencyId"].ToString());
            data.UrgencyName = Drow["UrgencyName"].ToString();
            EventType_Urgent_Degree.Add(data);
          }

        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到紧急类别数据!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Urgent_Degree;
      return JsonConvert.SerializeObject(result);

    }
    ///获取处理级别数据
    /// <summary>
    /// 获取处理级别数据
    /// </summary>
    /// <returns>{"result":true,"message":"成功","Data":[{"HandlerLevelId":1,"HandlerLevelName":"2小时-抢险类"},{"HandlerLevelId":2,"HandlerLevelName":"4小时-正常维修"},{"HandlerLevelId":3,"HandlerLevelName":"6小时-暂缓处理"}]}</returns>
    public static string GetHandler_Level()
    {

      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results_Handler_Level result = new Results_Handler_Level();
      List<EventType_Handler_Level> EventType_Handler_Level = new List<EventType_Handler_Level>();
      try
      {
        string sql = string.Format(@" SELECT  HandlerLevelId,HandlerLevelName
                                                  FROM M_HandlerLevel where 1=1 ");
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType_Handler_Level data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new EventType_Handler_Level();
            data.HandlerLevelId = int.Parse(Drow["HandlerLevelId"].ToString());
            data.HandlerLevelName = Drow["HandlerLevelName"].ToString();
            EventType_Handler_Level.Add(data);
          }

        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到处理级别数据!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Handler_Level;
      return JsonConvert.SerializeObject(result);

    }
    ///获取事件类别小项
    /// <summary>
    /// 获取事件类别小项
    /// </summary>
    /// <param name="ParentTypeId">事件类别ID</param>
    /// <returns>{"result":true,"message":"成功","Data":[{"EventTypeId":"1","EventTypeName":"困难用水"},{"EventTypeId":"2","EventTypeName":"设备属性与实际不符"}]}</returns>
    public static string Get_EventTypeDetail(string ParentTypeId)
    {
      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results result = new Results();
      List<EventType> EventTypeData = new List<EventType>();
      try
      {
        //判断获取的事件ID是否为空
        if (string.IsNullOrEmpty(ParentTypeId))
        {
          result.result = false;
          result.message = "事件类别ID不能为空";
        }
        else
        {
          string sql = string.Format(@" SELECT  EventTypeId,EventTypeName,ParentTypeId
                                                  FROM M_EventType me where 1=1 and  me.ParentTypeId = '{0}' ", ParentTypeId);
          dt = APP.SQLServer_Helper.SelectDataTable(sql);
          if (dt.Rows.Count > 0)
          {
            EventType data;
            result.result = true;
            result.message = "成功";
            foreach (DataRow Drow in dt.Rows)
            {
              data = new EventType();
              data.EventTypeId = int.Parse(Drow["EventTypeId"].ToString());
              data.EventTypeName = Drow["EventTypeName"].ToString();
              EventTypeData.Add(data);
            }

          }
          else
          {
            //当登录失败后进行赋值登录错误信息
            result.result = false;
            result.message = "未查询到事件类别小项!";
          }
        }

      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventTypeData;
      return JsonConvert.SerializeObject(result);

    }

    /// <summary>
    /// 保存事件
    /// </summary>
    public static string SavEventStart(
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
            string EventDesc,            //处理说明
            string Bae64Image           //图片
        )
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;

      if (string.IsNullOrEmpty(iAdminID))
      {
        upload.message = "发起人为空！";

        return JsonConvert.SerializeObject(upload); ;
      }

      //1-1:初始化图片的路径
      string EventPictures = string.Empty;
      //1-3:初始化数据保存结果
      int SaveResult = 0;
      //2:当Base64不为空时进行获取图片路径,否则不操作
      if (!string.IsNullOrEmpty(Bae64Image))
      {
        EventPictures = SaveImage(Bae64Image);
      }

      Tool.DB.MakeSql sql = new Tool.DB.MakeSql(Tool.DB.SqlStringType.INSERT, "M_Event");

      sql.AddField("EventCode", "{0}", Tool.DB.FieldType.INT);
      sql.AddField("EventAddress", EventAddress, Tool.DB.FieldType.STR);
      sql.AddField("UpTime", DateTime.Now.ToString(), Tool.DB.FieldType.DATE);
      sql.AddField("PersonId", iAdminID, Tool.DB.FieldType.INT);
      sql.AddField("PName", cAdminName, Tool.DB.FieldType.STR);
      sql.AddField("DeptId", iDeptID, Tool.DB.FieldType.INT);
      sql.AddField("EventTypeId", EventTypeId, Tool.DB.FieldType.INT);
      sql.AddField("EventTypeId2", EventTypeId2, Tool.DB.FieldType.INT);
      sql.AddField("EventFromId", EventFromId, Tool.DB.FieldType.INT);
      sql.AddField("UrgencyId", UrgencyId, Tool.DB.FieldType.INT);
      sql.AddField("HandlerLevelId", 1, Tool.DB.FieldType.INT);
      sql.AddField("EventDesc", EventDesc, Tool.DB.FieldType.STR);
      sql.AddField("EventX", EventX, Tool.DB.FieldType.STR);
      sql.AddField("EventY", EventY, Tool.DB.FieldType.STR);
      sql.AddField("EventUpdateTime", DateTime.Now, Tool.DB.FieldType.DATE);
      sql.AddField("IsValid", 1, Tool.DB.FieldType.INT);
      sql.AddField("DeleteStatus", "0", Tool.DB.FieldType.STR);
      sql.AddField("TaskId", -1, Tool.DB.FieldType.INT);
      sql.AddField("ExecTime", 36, Tool.DB.FieldType.INT);
      sql.AddField("LinkMan", LinkMan, Tool.DB.FieldType.STR);
      sql.AddField("LinkCall", LinkCall, Tool.DB.FieldType.STR);
      sql.AddField("EventPictures", EventPictures, Tool.DB.FieldType.STR);

      string EventCode = "";
      if (EventFromId == "1")
      {
        EventCode = "DH";
      }
      else if (EventFromId == "2")
      {
        EventCode = "RX";
      }
      else if (EventFromId == "3")
      {
        EventCode = "XJ";
      }
      else
      {
        EventCode = "LS";
      }

      string SQL = string.Format(sql.OutInsertSQL(), " (SELECT '" + EventCode + "' + SUBSTRING(CONVERT(NVARCHAR, YEAR(SYSDATETIME())), 3, 2) + right('0000000' + CONVERT(NVARCHAR, MAX(EventID + 1)), 7) FROM M_Event )");
      sql = new Tool.DB.MakeSql(Tool.DB.SqlStringType.INSERT, "M_WorkOrder_Oper_History");

      sql.AddField("EventID", "@@IDENTITY", Tool.DB.FieldType.INT);
      sql.AddField("OperId", 11, Tool.DB.FieldType.INT);
      sql.AddField("OperTime", DateTime.Now, Tool.DB.FieldType.STR);
      sql.AddField("ExecPersonId", ExecPersonId, Tool.DB.FieldType.STR);
      sql.AddField("DispatchPersonID", iAdminID, Tool.DB.FieldType.INT);
      sql.AddField("ExecDetpID", ExecDetpID, Tool.DB.FieldType.STR);


      //int iEventID = APP.OutSide_Helper.GetSqlExecScalar(SQL + ";select @@IDENTITY;",out ErrInfo_);
      string ErrInfo_ = "";
      if (APP.SQLServer_Helper.UpDate(SQL + ";" + sql.ToString(), out ErrInfo_) > 0)
      {
        upload.result = true;
        upload.message = "事件上传成功";
      }
      else
      {
        upload.result = false;
        upload.message = "事件上传失败，" + ErrInfo_;
      }

      return JsonConvert.SerializeObject(upload);
    }
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
    public static string HiddenTroubleReport_Insert(string Devicename, int Devicesmid, string Uptime, string X, string Y, string Longitude, string Latitude, int PersonId, string Bae64Image, string Bae64Voice, int EventId, int EventContentId, string EventAddress, string Description, int IsHidden, int TaskId, int MUngercyId, int MLevelId, int PointType, int IsTemp, int DeptId, int EventFromId = 3)
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;
      upload.message = "上传失败,请先巡检该设备点后再反馈设备情况!";
      try
      {
        //1-1:初始化图片的路径
        string ImagePath = string.Empty;
        //1-2:初始化数据保存语句
        StringBuilder SqlInsertEqu = new StringBuilder();
        //1-3:初始化数据保存结果
        int SaveResult = 0;
        //2:当Base64不为空时进行获取图片路径,否则不操作
        if (!string.IsNullOrEmpty(Bae64Image))
        {
          ImagePath = SaveImage(Bae64Image);
        }
        //3:事件上传类别判断,当是任务时,同时无隐患时上传到任务完成明细列表
        if (!string.IsNullOrEmpty(IsHidden.ToString()))
        {
          SqlInsertEqu.Append("set xact_abort off begin tran");
          //当没有异常时认为是正常的任务上报
          if (IsHidden == 0)
          {
            if (TaskId > 0)
            {
              //                        SqlInsertEqu.AppendFormat(@" insert into L_Task_CompleteDetail(TaskId,Devicename,Devicesmid,Uptime,x,y,Peopleid,ImagePath,Address,Miaoshu,PointType,VoicePath,IsHidden,IsFeedback ) 
              //                                                                         values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',1) "
              //                                                                               , TaskId, Devicename, Devicesmid, Uptime, X, Y, PersonId, ImagePath, EventAddress, Description, PointType, ImagePath, IsHidden);
              SqlInsertEqu.AppendFormat(@" update  L_Task_CompleteDetail set IsFeedback=1 where Devicesmid='{0}' and PointType={1} and  TaskId = {2} ", Devicesmid, PointType, TaskId);
            }
            else
            {
              upload.result = false;
              //upload.message = "事件上传成功";
              upload.message = "无隐患，并无此任务，请验证传递参数！";
            }

          }
          //当有异常的时候将数据上传到异常异常事件表
          else if (IsHidden == 1)
          {
            //                        SqlInsertEqu.AppendFormat(@" insert into L_Task_CompleteDetail(TaskId,Devicename,Devicesmid,Uptime,x,y,Peopleid,ImagePath,Address,Miaoshu,PointType,VoicePath,IsHidden,IsFeedback ) 
            //                                                                         values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',1) "
            //                                                                               , TaskId, Devicename, Devicesmid, Uptime, X, Y, PersonId, ImagePath, EventAddress, Description, PointType, ImagePath, IsHidden);
            string EventFromCode = string.Empty;
            if (EventFromId == 3)
            {
              EventFromCode = "XJ";
            }
            else
            {
              EventFromCode = "JK";
            }
            string strSqlUpdate = string.Format(@" update  L_Task_CompleteDetail set IsFeedback=1 where Devicesmid='{0}' and PointType={1} and  TaskId = {2} ", Devicesmid, PointType, TaskId);
            SaveResult = APP.SQLServer_Helper.UpDate(strSqlUpdate);
            if (SaveResult <= 0)
            {
              SqlInsertEqu.AppendFormat(@" insert into L_Task_CompleteDetail(TaskId,Devicename,Devicesmid,Uptime,x,y,Peopleid,PointType,IsHidden,IsFeedback ) 
                                                                         values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',1,1) ",
                                                                   TaskId, Devicename, Devicesmid, Uptime, X, Y, PersonId, PointType);
            }
            SqlInsertEqu.AppendFormat(@" insert into M_Event(EventCode,EventAddress,UpTime,PersonId,EventTypeId,EventTypeId2,EventFromId,UrgencyId
                                                                  ,HandlerLevelId,EventPictures,EventDesc,EventX,EventY,EventUpdateTime,IsValid,Devicesmid,DeptId,DevicesType,TaskId)values(
                                                                   (select '" + EventFromCode + "'+convert(varchar(50),(" + System.DateTime.Now.ToString("yy") + "*10000000+isnull(MAX(eventid),0)+1)) from M_Event),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}') "
                                                       , EventAddress, Uptime, PersonId, EventId, EventContentId, EventFromId, MUngercyId, MLevelId, ImagePath, Description, X, Y, Uptime, 1, Devicesmid, DeptId, Devicename, TaskId);
          }
          SqlInsertEqu.AppendFormat(@" commit tran ");

          _logger.Info("事件上报接口第一条SQL：" + SqlInsertEqu.ToString());

          SaveResult = APP.SQLServer_Helper.Insert(SqlInsertEqu.ToString());

          _logger.Info("事件上报接口第一条SQL输出结果：" + SaveResult);
        }
        if (SaveResult > 0)
        {
          upload.result = true;
          upload.message = "事件上传成功";
        }
        //判断任务是否完成
        //第一步:获取到该计划的计划类别
        //1-1:查询任务计划类别的sql拼接
        string sqlPlanType = string.Format(@" select lp.PlanTypeId,lp.BoolFeedBack from L_Task lt 
                                                  left join L_PLAN lp on lp.PlanId = lt.PlanId  where 1=1 and  lt.TaskId = '{0}' ", TaskId);
        //1-2:初始化计划类别结果集
        DataTable dtPlanType = new DataTable();
        dtPlanType = APP.SQLServer_Helper.SelectDataTable(sqlPlanType);
        string ErrMessage = string.Empty;
        //当时巡检事件上报的时候才进行计算该巡检任务是否完成
        if (dtPlanType.Rows.Count > 0 && EventFromId == 3)
        {
          //第二步按照计划类别查询关键点数据集,设备数据集
          string PlanType = dtPlanType.Rows[0]["PlanTypeId"].ToString();
          string strWhere = string.Empty;
          if (dtPlanType.Rows[0]["BoolFeedBack"].ToString() == "0")
          {
            strWhere = " and ltc1.Devicesmid is null ";
          }
          else if (dtPlanType.Rows[0]["BoolFeedBack"].ToString() == "1")
          {
            strWhere = " and (ltc1.Devicesmid is null or (ltc1.Devicesmid is not null and (ltc1.IsFeedback = 0 or ltc1.IsFeedback is null))) ";
          }
          DataTable DtSelectUnDo = new DataTable();
          //2-1:判断路线巡检
          if (PlanType == "2")
          {
            //判断该任务是否完成,完成时直接更新任务的完成状态
            string StrSelectUnDo = string.Format(@"  select ltc1.Devicesmid from L_Task lt
                                                                left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                left join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= lpd.SmID 
                                                                and ltc1.Devicename = lpd.EquType and ltc1.PointType = 0 
                                                                and ltc1.TaskId = lt.TaskId 
                                                                where lt.TaskId = '{0}' and lpd.SmID is not null {1}
                                                                union all 
                                                                 select ltc1.Devicesmid from L_Task lt
                                                                left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                left join PlanLineDetail pld on pld.PlanLineId = lp.PlanLineId and pld.ImportPointType=1
                                                                left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= pld.PlanLineDetaiId 
                                                                and ltc1.PointType = 2 and ltc1.TaskId = lt.TaskId  
                                                                where lt.TaskId = '{0}' and pld.PlanLineDetaiId is not null {1}  ", TaskId, strWhere);
            DtSelectUnDo = APP.SQLServer_Helper.SelectDataTable(StrSelectUnDo, out ErrMessage);
          }
          else if (PlanType == "1")
          {
            //判断该任务是否完成,完成时直接更新任务的完成状态
            string StrSelectUnDo = string.Format(@"  select ltc1.Devicesmid from L_Task lt
                                                                 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                 left join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                 left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= lpd.SmID 
                                                                 and ltc1.Devicename = lpd.EquType and ltc1.PointType = 0 
                                                                 and ltc1.TaskId = lt.TaskId 
                                                                 where lt.TaskId = '{0}' and lpd.SmID is not null {1}
                                                                 union all 
                                                                  select ltc1.Devicesmid from L_Task lt
                                                                 left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                 left join PointAreaInfo pld on pld.PlanAreaId = lp.PlanAreaId 
                                                                 left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= pld.PointId 
                                                                 and ltc1.PointType = 1 and ltc1.TaskId = lt.TaskId  
                                                                 where lt.TaskId = '{0}'  and pld.PointId is not null {1}   ", TaskId, strWhere);
            DtSelectUnDo = APP.SQLServer_Helper.SelectDataTable(StrSelectUnDo, out ErrMessage);
          }
          //判断是否完成
          if (DtSelectUnDo.Rows.Count == 0 && string.IsNullOrEmpty(ErrMessage))
          {
            string StrUpdateState = string.Format(@" update L_Task set Finish = 1 where TaskId = '{0}' ", TaskId);
            APP.SQLServer_Helper.UpDate(StrUpdateState);
          }
        }
      }
      catch (Exception e)
      {
        upload.result = false;
        upload.message = e.ToString();
        _logger.Error(e.ToString());
      }
      return JsonConvert.SerializeObject(upload);

    }
    /// <summary>
    /// Gets the return back event list.
    /// </summary>
    /// <returns>System.String.</returns>
    public static string GetReturnBackEventList(string PersonId)
    {
      //事件返回结果集
      BackEvent BackEvent = new BackEvent();
      try
      {
        //初始化查询结果集语句
        string strSqlSelect = string.Format(@" select isnull(lt.TaskId,-1) as TaskId,isnull(lt.TaskName,'临时事件') as TaskName,COUNT(me.EventID) as FailCount from M_Event me
                                                       left join L_Task lt on lt.TaskId = me.TaskId
                                                       where me.IsValid = 2 and me.DeleteStatus = 0 and PersonId = '{0}'
                                                       group by lt.TaskId,lt.TaskName  ", PersonId);
        //初始化数据结果集
        DataTable dtSelect = new DataTable();
        //执行数据库查询
        dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
        //处理查询结果集
        if (dtSelect.Rows.Count > 0)
        {
          //迭代结果集
          for (int i = 0; i < dtSelect.Rows.Count; i++)
          {
            BackEventList BackEventList = new BackEventList();
            //任务ID
            BackEventList.TaskId = dtSelect.Rows[i]["TaskId"].ToString();
            //任务名称
            BackEventList.TaskName = dtSelect.Rows[i]["TaskName"].ToString();
            //退回事件数量
            BackEventList.FailCount = dtSelect.Rows[i]["FailCount"].ToString();
            BackEvent.Data.Add(BackEventList);
          }
          BackEvent.message = "成功!";
        }
        else
        {
          BackEvent.message = "未查询到该用户的退回事件!";
        }
        BackEvent.result = true;
      }
      catch (Exception ex)
      {
        BackEvent.result = false;
        BackEvent.message = ex.ToString();
      }
      return JsonConvert.SerializeObject(BackEvent);
    }
    /// <summary>
    /// Gets the return back event list detail.
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <param name="TaskId">巡检任务ID</param>
    /// <returns>System.String.</returns>
    public static string GetReturnBackEventListDetail(string PersonId, string TaskId)
    {
      {
        //事件返回结果集
        BackEventListDetail BackEventListDetail = new BackEventListDetail();
        try
        {
          //初始化查询条件
          string strWhere = string.Empty;
          if (string.IsNullOrEmpty(TaskId) || Convert.ToInt32(TaskId) <= 0)
          {
            strWhere = string.Format(@" and  me.PersonId = '{0}' and (me.TaskId is null or me.TaskId = -1)", PersonId);
          }
          else
          {
            strWhere = string.Format(@" and  me.PersonId = '{0}' and me.TaskId = '{1}' ", PersonId, TaskId);
          }
          //初始化查询结果集语句
          string strSqlSelect = string.Format(@" select me.EventID as OperEventID,me.DevicesType as Devicename,isnull(me.Devicesmid, -1) as Devicesmid,me.Uptime,me.EventX as X,me.EventY as  Y,
                                                           me.EventX as Longitude,me.EventY as  Latitude,isnull(me.PersonId,-1) as PersonId,me.EventPictures as ImageUrl,
                                                           me.EventVoices as VoiceUrl,isnull(me.EventTypeId,-1) as EventId,isnull(me.EventTypeId2,-1) as EventContentId,
                                                           me.EventAddress,me.EventDesc as Description,1 as IsHidden,isnull(me.TaskId,-1) as TaskId,isnull(me.UrgencyId,-1) as MUngercyId,
                                                           isnull(me.HandlerLevelId,-1) as MLevelId,
                                                           case when me.TaskId is null then 1 else 0 end as IsTemp,isnull(DeptId,-1) as DeptId ,lt.TaskName,me.Remark_Back
                                                           ,mu.UrgencyName,mh1.HandlerLevelName ,met1.EventTypeName as EventTypeName1 ,met2.EventTypeName as EventTypeName2
                                                           from M_Event me 
                                                           left join L_Task lt on lt.TaskId = me.TaskId  
                                                           left join M_Urgency mu on mu.UrgencyId = me.UrgencyId 
                                                           left join M_HandlerLevel mh1 on mh1.HandlerLevelId = me.HandlerLevelId 
                                                           left join  M_EventType met1 on met1.EventTypeId = me.EventTypeId
                                                           left join  M_EventType met2 on met2.EventTypeId = me.EventTypeId2 where 1=1 and me.IsValid = 2 {0}   ", strWhere);
          //初始化数据结果集
          DataTable dtSelect = new DataTable();
          //执行数据库查询
          dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
          //处理查询结果集
          if (dtSelect.Rows.Count > 0)
          {
            //迭代结果集
            for (int i = 0; i < dtSelect.Rows.Count; i++)
            {
              BackEventListDetailInfo BackEventListDetailInfo = new BackEventListDetailInfo();
              BackEventListDetailInfo.Devicesmid = Convert.ToInt32(dtSelect.Rows[i]["Devicesmid"].ToString());
              BackEventListDetailInfo.Devicename = dtSelect.Rows[i]["Devicename"].ToString();
              BackEventListDetailInfo.Uptime = dtSelect.Rows[i]["Uptime"].ToString();
              BackEventListDetailInfo.X = dtSelect.Rows[i]["X"].ToString();
              BackEventListDetailInfo.Y = dtSelect.Rows[i]["Y"].ToString();
              BackEventListDetailInfo.Longitude = dtSelect.Rows[i]["Longitude"].ToString();
              BackEventListDetailInfo.Latitude = dtSelect.Rows[i]["Latitude"].ToString();
              BackEventListDetailInfo.PersonId = Convert.ToInt32(dtSelect.Rows[i]["PersonId"].ToString());
              BackEventListDetailInfo.EventId = Convert.ToInt32(dtSelect.Rows[i]["EventId"].ToString());
              BackEventListDetailInfo.EventContentId = Convert.ToInt32(dtSelect.Rows[i]["EventContentId"].ToString());
              BackEventListDetailInfo.EventAddress = dtSelect.Rows[i]["EventAddress"].ToString();
              BackEventListDetailInfo.Description = dtSelect.Rows[i]["Description"].ToString();
              BackEventListDetailInfo.IsHidden = Convert.ToInt32(dtSelect.Rows[i]["IsHidden"].ToString());
              BackEventListDetailInfo.TaskId = Convert.ToInt32(dtSelect.Rows[i]["TaskId"].ToString());
              BackEventListDetailInfo.MUngercyId = Convert.ToInt32(dtSelect.Rows[i]["MUngercyId"].ToString());
              BackEventListDetailInfo.MLevelId = Convert.ToInt32(dtSelect.Rows[i]["MLevelId"].ToString());
              BackEventListDetailInfo.IsTemp = Convert.ToInt32(dtSelect.Rows[i]["IsTemp"].ToString());
              BackEventListDetailInfo.DeptId = Convert.ToInt32(dtSelect.Rows[i]["DeptId"].ToString());
              BackEventListDetailInfo.OperEventID = Convert.ToInt32(dtSelect.Rows[i]["OperEventID"].ToString());
              BackEventListDetailInfo.TaskName = dtSelect.Rows[i]["TaskName"].ToString();
              BackEventListDetailInfo.Remark_Back = dtSelect.Rows[i]["Remark_Back"].ToString();

              BackEventListDetailInfo.UrgencyName = dtSelect.Rows[i]["UrgencyName"].ToString();
              BackEventListDetailInfo.HandlerLevelName = dtSelect.Rows[i]["HandlerLevelName"].ToString();
              BackEventListDetailInfo.EventTypeName1 = dtSelect.Rows[i]["EventTypeName1"].ToString();
              BackEventListDetailInfo.EventTypeName2 = dtSelect.Rows[i]["EventTypeName2"].ToString();
              //迭代添加图片
              if (!string.IsNullOrEmpty(dtSelect.Rows[i]["ImageUrl"].ToString()))
              {
                string ImageUrl = dtSelect.Rows[i]["ImageUrl"].ToString();
                string[] ImageUrlArray = ImageUrl.Split('|');
                for (int j = 0; j < ImageUrlArray.Length; j++)
                {
                  BackEventInfoImage BackEventInfoImage = new BackEventInfoImage();
                  BackEventInfoImage.ImageUrl = ImageUrlArray[j];//ImageAndVoice_Url + 
                  BackEventListDetailInfo.ImageUrl.Add(BackEventInfoImage);
                }

              }
              //迭代添加声音
              if (!string.IsNullOrEmpty(dtSelect.Rows[i]["VoiceUrl"].ToString()))
              {
                string VoiceUrl = dtSelect.Rows[i]["VoiceUrl"].ToString();
                string[] VoiceUrlArray = VoiceUrl.Split('|');
                for (int j = 0; j < VoiceUrlArray.Length; j++)
                {
                  BackEventInfoVoice BackEventInfoVoice = new BackEventInfoVoice();
                  BackEventInfoVoice.VoiceUrl = VoiceUrlArray[j];//ImageAndVoice_Url +
                  BackEventListDetailInfo.VoiceUrl.Add(BackEventInfoVoice);
                }

              }
              BackEventListDetail.Data.Add(BackEventListDetailInfo);
            }
            BackEventListDetail.message = "成功!";
          }
          else
          {
            BackEventListDetail.message = "未查询到该巡检任务下的退回事件!";
          }
          BackEventListDetail.result = true;
        }
        catch (Exception ex)
        {
          BackEventListDetail.result = false;
          BackEventListDetail.message = ex.ToString();
        }
        return JsonConvert.SerializeObject(BackEventListDetail);
      }
    }
    public static string HiddenTroubleReport_UpAgain(string Uptime, string Bae64Image, string Bae64Voice, int EventId, int EventContentId, string EventAddress, string Description, int MUngercyId, int MLevelId, string OperEventID, string TaskId)
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;
      upload.message = "上传失败,请先巡检该设备点后再反馈设备情况!";
      try
      {
        //1-1:初始化图片的路径
        string ImagePath = string.Empty;
        //1-2:初始化数据保存语句
        StringBuilder SqlInsertEqu = new StringBuilder();
        //1-3:初始化数据保存结果
        int SaveResult = 0;
        //2:当Base64不为空时进行获取图片路径,否则不操作
        if (!string.IsNullOrEmpty(Bae64Image))
        {
          ImagePath = SaveImage(Bae64Image);
        }
        //3:事件上传类别判断,当是任务时,同时无隐患时上传到任务完成明细列表

        SqlInsertEqu.Append("set xact_abort off begin tran");
        SqlInsertEqu.AppendFormat(@" update  M_Event set EventAddress='{0}',EventTypeId='{1}',EventTypeId2='{2}',UrgencyId='{3}'
                                                                ,HandlerLevelId='{4}',EventPictures='{5}',EventDesc='{6}',
                                                                 EventUpdateTime='{7}',IsValid=1 where EventID = '{8}' "
                                                   , EventAddress, EventId, EventContentId, MUngercyId, MLevelId, ImagePath, Description, Uptime, OperEventID);

        SqlInsertEqu.AppendFormat(@" commit tran ");
        SaveResult = APP.SQLServer_Helper.Insert(SqlInsertEqu.ToString());

        if (SaveResult > 0)
        {
          upload.result = true;
          upload.message = "事件上传成功";
        }
        //判断任务是否完成
        //第一步:获取到该计划的计划类别
        //1-1:查询任务计划类别的sql拼接
        string sqlPlanType = string.Format(@" select lp.PlanTypeId,lp.BoolFeedBack from L_Task lt 
                                                  left join L_PLAN lp on lp.PlanId = lt.PlanId  where 1=1 and  lt.TaskId = '{0}' ", TaskId);
        //1-2:初始化计划类别结果集
        DataTable dtPlanType = new DataTable();
        dtPlanType = APP.SQLServer_Helper.SelectDataTable(sqlPlanType);
        string ErrMessage = string.Empty;
        //当时巡检事件上报的时候才进行计算该巡检任务是否完成
        if (dtPlanType.Rows.Count > 0)
        {
          //第二步按照计划类别查询关键点数据集,设备数据集
          string PlanType = dtPlanType.Rows[0]["PlanTypeId"].ToString();
          string strWhere = string.Empty;
          if (dtPlanType.Rows[0]["BoolFeedBack"].ToString() == "0")
          {
            strWhere = " and ltc1.Devicesmid is null ";
          }
          else if (dtPlanType.Rows[0]["BoolFeedBack"].ToString() == "1")
          {
            strWhere = " and (ltc1.Devicesmid is null or (ltc1.Devicesmid is not null and (ltc1.IsFeedback = 0 or ltc1.IsFeedback is null))) ";
          }
          DataTable DtSelectUnDo = new DataTable();
          //2-1:判断路线巡检
          if (PlanType == "2")
          {
            //判断该任务是否完成,完成时直接更新任务的完成状态
            string StrSelectUnDo = string.Format(@"  select ltc1.Devicesmid from L_Task lt
                                                                left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                left join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= lpd.SmID 
                                                                and ltc1.Devicename = lpd.EquType and ltc1.PointType = 0 
                                                                and ltc1.TaskId = lt.TaskId 
                                                                where lt.TaskId = '{0}' and lpd.SmID is not null {1}
                                                                union all 
                                                                 select ltc1.Devicesmid from L_Task lt
                                                                left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                left join PlanLineDetail pld on pld.PlanLineId = lp.PlanLineId and pld.ImportPointType=1
                                                                left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= pld.PlanLineDetaiId 
                                                                and ltc1.PointType = 2 and ltc1.TaskId = lt.TaskId  
                                                                where lt.TaskId = '{0}' and pld.PlanLineDetaiId is not null {1}  ", TaskId, strWhere);
            DtSelectUnDo = APP.SQLServer_Helper.SelectDataTable(StrSelectUnDo, out ErrMessage);
          }
          else if (PlanType == "1")
          {
            //判断该任务是否完成,完成时直接更新任务的完成状态
            string StrSelectUnDo = string.Format(@"  select ltc1.Devicesmid from L_Task lt
                                                                 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                 left join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                 left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= lpd.SmID 
                                                                 and ltc1.Devicename = lpd.EquType and ltc1.PointType = 0 
                                                                 and ltc1.TaskId = lt.TaskId 
                                                                 where lt.TaskId = '{0}' and lpd.SmID is not null {1}
                                                                 union all 
                                                                  select ltc1.Devicesmid from L_Task lt
                                                                 left join L_PLAN lp on lp.PlanId = lt.PlanId
                                                                 left join PointAreaInfo pld on pld.PlanAreaId = lp.PlanAreaId 
                                                                 left join L_Task_CompleteDetail ltc1 on ltc1.Devicesmid= pld.PointId 
                                                                 and ltc1.PointType = 1 and ltc1.TaskId = lt.TaskId  
                                                                 where lt.TaskId = '{0}'  and pld.PointId is not null {1}   ", TaskId, strWhere);
            DtSelectUnDo = APP.SQLServer_Helper.SelectDataTable(StrSelectUnDo, out ErrMessage);
          }
          //判断是否完成
          if (DtSelectUnDo.Rows.Count == 0 && string.IsNullOrEmpty(ErrMessage))
          {
            string StrUpdateState = string.Format(@" update L_Task set Finish = 1 where TaskId = '{0}' ", TaskId);
            APP.SQLServer_Helper.UpDate(StrUpdateState);
          }
        }
      }
      catch (Exception e)
      {
        upload.result = false;
        upload.message = e.ToString();
      }
      return JsonConvert.SerializeObject(upload);

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
    /// <returns>System.String.</returns>
    public static string QianDao(string Lwr_PersonId, string Lwr_BeiZhu, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power, string Lwr_XY, int DeptId)
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;
      upload.message = "失败";
      try
      {
        //判断用户ID不能为空
        if (string.IsNullOrEmpty(Lwr_PersonId))
        {
          upload.result = false;
          upload.message = "用户ID不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        if (string.IsNullOrEmpty(DeptId.ToString()))
        {
          upload.result = false;
          upload.message = "部门ID不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        if (string.IsNullOrEmpty(Lwr_XY))
        {
          upload.result = false;
          upload.message = "用户定位经纬度不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        //初始化系统当前时间
        DateTime DatetimeNow = DateTime.Now;
        //初始化用户上传时间
        string Lwr_UpTime = DatetimeNow.ToString("yyyy-MM-dd HH:mm:ss");
        //初始化用户上传日期
        string Lwr_Date = DatetimeNow.ToString("yyyy-MM-dd");
        //初始化当天开始时间
        string DayStartTime = DatetimeNow.ToString("yyyy-MM-dd") + " 00:00:00";
        //初始化开始时间
        string Lwr_StartTime = DatetimeNow.ToString("HH:mm:ss");
        //初始化结束时间
        string Lwr_EndTime = DatetimeNow.ToString("HH:mm:ss");
        //初始化上班时间
        string Lwr_Hour = string.Empty;
        //初始化签到状态 上班签到  下班签退
        string Lwr_PersonStatus = string.Empty;
        //初始化查询记录语句
        string strSelectSql = string.Empty;
        //初始化查询数据集
        DataTable dtSelect = new DataTable();
        strSelectSql = string.Format(@"  select Id,PersonId,DeptId,Date,StartTime,EndTime,Hour,BeiZhu,PersonStatus,UpTime
                                             ,GpsStatus,MobileStatus,Power,XY from L_DeviceStatus where PersonId = '{0}' and UpTime >= '{1}' ", Lwr_PersonId, DayStartTime);
        dtSelect = APP.SQLServer_Helper.SelectDataTable(strSelectSql);
        //进行判断用户是签到还是签退
        if (dtSelect.Rows.Count > 0)
        {
          Lwr_PersonStatus = "用户下班";
          Lwr_StartTime = string.Empty;
        }
        else
        {
          Lwr_PersonStatus = "上班正常";
          Lwr_EndTime = string.Empty;
        }
        //1-1:初始化数据保存语句
        string sql = string.Empty;
        //1-2:初始化数据保存结果
        int SaveResult = 0;


        sql = string.Format(@"insert into L_DeviceStatus(PersonId,DeptId,Date,StartTime,EndTime,Hour,BeiZhu,PersonStatus,UpTime,GpsStatus,MobileStatus,Power,XY) 
                                                                 values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')"
                                    , Lwr_PersonId, DeptId, Lwr_Date, Lwr_StartTime, Lwr_EndTime, Lwr_Hour, Lwr_BeiZhu, Lwr_PersonStatus, Lwr_UpTime, Lwr_GpsStatus, Lwr_MobileStatus, Lwr_Power, Lwr_XY);

        SaveResult = APP.SQLServer_Helper.Insert(sql);
        if (SaveResult > 0)
        {
          upload.result = true;
          upload.message = "提交考勤成功";
        }
        return JsonConvert.SerializeObject(upload);
      }
      catch (Exception e)
      {
        upload.result = false;
        upload.message = e.ToString();
        return JsonConvert.SerializeObject(upload);
      }
    }

    ///巡检人员上报位置
    /// <summary>
    /// 巡检人员上报位置
    /// </summary>
    /// <param name="PositionX">上报位置经度</param>
    /// <param name="PositionY">上报位置纬度</param>
    /// <param name="UpTime">位置上报事件</param>
    /// <param name="PersonId">位置上报人员</param>
    /// <param name="isOnline">是否在线0:不在线,1:在线</param>
    /// <returns>System.String.</returns>
    public static string UPCoordinatePosition(string PositionX, string PositionY, string UpTime, int PersonId, int isOnline)
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;
      upload.message = "失败";
      string CurrentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      ////string CurrentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      //string CurrentDateTime =Convert.ToDateTime(UpTime).ToString();
      try
      {
        //1-1:初始化数据保存语句
        string sql = string.Empty;
        //1-2:初始化数据保存结果
        int SaveResult = 0;
        sql = string.Format(@"insert into L_Position(PositionX,PositionY,UpTime,PersonId,isOnline) 
                                                                 values('{0}','{1}','{2}','{3}','{4}')"
                                    , PositionX, PositionY, UpTime, PersonId, isOnline);



        SaveResult = APP.SQLServer_Helper.Insert(sql);
        //1:查询所有的仅到位的任务点sql语句拼接
        string sqlSelect = string.Format(@"--巡检设备
                                                  select  '0' as PointType, lt.PlanId,lt.TaskId,lpd.SmID,lt.TaskName,lpd.SmX,lpd.SmY,lpd.EquType as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.SmID and ltc.PointType = 0 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1}  and lt.TaskState !=0 and lt.AssignState = 1  and ltc.Devicesmid is null and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'			
                                                  union all
                                                  --路线关键点
                                                  select   '2' as PointType,lt.PlanId,lt.TaskId,lpd.PlanLineDetaiId as SmID,lt.TaskName,lpd.X as SmX,lpd.Y as SmY,lpd.ImportPointName as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join PlanLineDetail lpd on lp.PlanLineId = lpd.PlanLineId and lpd.ImportPointType = 1
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.PlanLineDetaiId and ltc.PointType = 2 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1} and lt.TaskState !=0 and lt.AssignState = 1 and ltc.Devicesmid is null and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'	
                                                  --区域关键点
                                                  union all
                                                  select '1' as PointType, lt.PlanId,lt.TaskId,lpd.PointId as SmID,lt.TaskName,lpd.PointX as SmX,lpd.PointY as SmY,lpd.PointName as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join PointAreaInfo lpd on lp.PlanAreaId = lpd.PlanAreaId
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.PointId and ltc.PointType = 1 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1}  and lt.TaskState !=0 and lt.AssignState = 1 and ltc.Devicesmid is null  and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'", CurrentDateTime, PersonId);
        //2:初始化数据集,同时执行sql
        DataTable dtPoint = new DataTable();
        dtPoint = APP.SQLServer_Helper.SelectDataTable(sqlSelect);
        //3:计算上传该点位完成的巡检点
        DataTable dtResult = CalculateCompletePoint(dtPoint, PositionX, PositionY);

        if (dtResult.Rows.Count > 0)
        {
          //4:初始化完成点位插入语句
          StringBuilder SqlInsertLineData = new StringBuilder();
          SqlInsertLineData.AppendFormat(" set xact_abort off begin tran ");
          //4:迭代处理完成的巡检点
          for (int i = 0; i < dtResult.Rows.Count; i++)
          {
            SqlInsertLineData.AppendFormat(@" insert into L_Task_CompleteDetail(TaskId,Devicename,Devicesmid,Uptime,x,y,Peopleid,PointType,IsHidden,IsFeedback ) 
                                                                         values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',0,0) ",
                                                                        dtResult.Rows[i]["TaskId"], dtResult.Rows[i]["PointName"], dtResult.Rows[i]["SmID"], UpTime, dtResult.Rows[i]["SmX"], dtResult.Rows[i]["SmY"], PersonId, dtResult.Rows[i]["PointType"]);
          }
          SqlInsertLineData.AppendFormat(@" commit tran ");
          APP.SQLServer_Helper.UpDate(SqlInsertLineData.ToString());
        }
        #region  开始 判断该任务是否完成
        DataTable dtSourcePoint = SelectDistinctTaskSource(dtPoint);
        DataTable dtResultPoint = SelectDistinctTaskResult(dtResult);
        for (int i = 0; i < dtSourcePoint.Rows.Count; i++)
        {
          for (int j = 0; j < dtResultPoint.Rows.Count; j++)
          {
            if (int.Parse(dtSourcePoint.Rows[i]["TaskId"].ToString()) == int.Parse(dtResultPoint.Rows[j]["TaskId"].ToString()) && int.Parse(dtSourcePoint.Rows[i]["FinishedCount"].ToString()) == int.Parse(dtResultPoint.Rows[j]["FinishedCount"].ToString()))
            {
              //初始化查询条件
              string strWhere = string.Empty;
              //仅到位
              if (dtSourcePoint.Rows[i]["BoolFeedBack"].ToString() == "0")
              {
                string strSelect = string.Format(@" select * from ({0}) where TaskId = {1} ", sqlSelect, dtResultPoint.Rows[j]["TaskId"]);
                DataTable dtTaskPoint = new DataTable();
                dtTaskPoint = APP.SQLServer_Helper.SelectDataTable(strSelect);
                if (dtTaskPoint.Rows.Count <= 0)
                {
                  string strUpdate = string.Format(@" update  L_Task  set Finish = 1 where TaskId = {0} ", dtResultPoint.Rows[j]["TaskId"]);
                  APP.SQLServer_Helper.SelectDataTable(strUpdate);
                }
              }
              //需反馈
              else if (dtSourcePoint.Rows[i]["BoolFeedBack"].ToString() == "1")
              {
                sqlSelect = string.Format(@"--巡检设备
                                                  select  '0' as PointType, lt.PlanId,lt.TaskId,lpd.SmID,lt.TaskName,lpd.SmX,lpd.SmY,lpd.EquType as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join L_PlanEquipmentDetail lpd on lpd.PlanID = lp.PlanId
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.SmID and ltc.PointType = 0 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1} and  lt.TaskId = {2}  and lt.TaskState !=0 and lt.AssignState = 1  and ltc.Devicesmid is null and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'			
                                                  union all
                                                  --路线关键点
                                                  select   '2' as PointType,lt.PlanId,lt.TaskId,lpd.PlanLineDetaiId as SmID,lt.TaskName,lpd.X as SmX,lpd.Y as SmY,lpd.ImportPointName as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join PlanLineDetail lpd on lp.PlanLineId = lpd.PlanLineId and lpd.ImportPointType = 1
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.PlanLineDetaiId and ltc.PointType = 2 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1} and  lt.TaskId = {2} and lt.TaskState !=0 and lt.AssignState = 1 and (ltc.Devicesmid is null or (ltc.Devicesmid is not null and ltc.IsFeedback = 0)) and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'	
                                                  --区域关键点
                                                  union all
                                                  select '1' as PointType, lt.PlanId,lt.TaskId,lpd.PointId as SmID,lt.TaskName,lpd.PointX as SmX,lpd.PointY as SmY,lpd.PointName as PointName,lp.BoolFeedBack from L_Task lt
                                                  				 inner join L_PLAN lp on lp.PlanId = lt.PlanId
                                                  				 inner join PointAreaInfo lpd on lp.PlanAreaId = lpd.PlanAreaId
                                                                 left join L_Task_CompleteDetail ltc on ltc.Devicesmid = lpd.PointId and ltc.PointType = 1 and ltc.TaskId = lt.TaskId
                                                  where 1=1 and ProraterId = {1}  and  lt.TaskId = {2} and lt.TaskState !=0 and lt.AssignState = 1 and (ltc.Devicesmid is null or (ltc.Devicesmid is not null and ltc.IsFeedback = 0))  and  lt.VisitStarTime <='{0}' and lt.VisitOverTime >='{0}'", CurrentDateTime, PersonId, dtResultPoint.Rows[i]["TaskId"]);
                DataTable dtTaskPoint = new DataTable();
                dtTaskPoint = APP.SQLServer_Helper.SelectDataTable(sqlSelect);
                if (dtTaskPoint.Rows.Count <= 0)
                {
                  string strUpdate = string.Format(@" update  L_Task  set Finish = 1 where TaskId = {0} ", dtResultPoint.Rows[i]["TaskId"]);
                  APP.SQLServer_Helper.SelectDataTable(strUpdate);
                }
              }
            }
          }
        }
        #endregion
        if (SaveResult > 0)
        {
          upload.result = true;
          upload.message = "位置上报成功";
        }
      }
      catch (Exception e)
      {
        upload.result = false;
        upload.message = e.ToString();
      }
      return JsonConvert.SerializeObject(upload);
    }
    /// <summary>
    /// Selects the distinct task source.
    /// </summary>
    /// <param name="dtPoint">The dt point.</param>
    /// <returns>DataTable.</returns>
    public static DataTable SelectDistinctTaskSource(DataTable dtPoint)
    {
      DataTable dtCompletePoint = new DataTable();
      dtCompletePoint.Columns.Add("PlanId", Type.GetType("System.String"));// 计划ID
      dtCompletePoint.Columns.Add("TaskId", Type.GetType("System.String"));// 任务ID    
      dtCompletePoint.Columns.Add("FinishedCount", Type.GetType("System.String"));// 任务ID
      dtCompletePoint.Columns.Add("BoolFeedBack", Type.GetType("System.String"));//点位名称
      try
      {
        //迭代处理所有结果集
        for (int i = 0; i < dtPoint.Rows.Count; i++)
        {
          //判断处理结果数量是否为0,0认为是空,直接添加行
          if (dtCompletePoint.Rows.Count <= 0)
          {
            DataRow dr = dtCompletePoint.NewRow();
            dr["PlanId"] = dtPoint.Rows[i]["PlanId"];
            dr["TaskId"] = dtPoint.Rows[i]["TaskId"];
            dr["FinishedCount"] = 1;
            dr["BoolFeedBack"] = dtPoint.Rows[i]["BoolFeedBack"];
            dtCompletePoint.Rows.Add(dr);
          }
          //当行数不为0时认为已经存在数据需要进行判断
          else
          {
            //迭代遍历已经计算完成的结果集
            for (int j = 0; j < dtCompletePoint.Rows.Count; j++)
            {
              //当待处理和已处理数据集的TaskId一致时,将已处理的数量进行+1操作
              if (int.Parse(dtCompletePoint.Rows[j]["TaskId"].ToString()) == int.Parse(dtPoint.Rows[i]["TaskId"].ToString()))
              {
                dtCompletePoint.Rows[j]["FinishedCount"] = int.Parse(dtCompletePoint.Rows[j]["FinishedCount"].ToString()) + 1;
                break;
              }
              //当已经是最后一行数据,但是待处理数据集与已经处理数据集仍未匹配,则添加
              else if (j == (dtCompletePoint.Rows.Count - 1) && int.Parse(dtCompletePoint.Rows[j]["TaskId"].ToString()) != int.Parse(dtPoint.Rows[i]["TaskId"].ToString()))
              {
                DataRow dr = dtCompletePoint.NewRow();
                dr["PlanId"] = dtPoint.Rows[i]["PlanId"];
                dr["TaskId"] = dtPoint.Rows[i]["TaskId"];
                dr["FinishedCount"] = 1;
                dr["BoolFeedBack"] = dtPoint.Rows[i]["BoolFeedBack"];
                dtCompletePoint.Rows.Add(dr);
                break;
              }
            }
          }
        }
        return dtCompletePoint;
      }
      catch
      {
        return dtCompletePoint;
      }
    }
    /// <summary>
    /// 按任务计算完成点位数量
    /// </summary>
    /// <param name="dtResult">The dt result.</param>
    /// <returns>DataTable.</returns>
    public static DataTable SelectDistinctTaskResult(DataTable dtResult)
    {
      DataTable dtCompletePoint = new DataTable();
      dtCompletePoint.Columns.Add("PlanId", Type.GetType("System.String"));// 计划ID
      dtCompletePoint.Columns.Add("TaskId", Type.GetType("System.String"));// 任务ID    
      dtCompletePoint.Columns.Add("FinishedCount", Type.GetType("System.String"));// 任务ID
      dtCompletePoint.Columns.Add("BoolFeedBack", Type.GetType("System.String"));//点位名称
      try
      {
        //迭代处理所有结果集
        for (int i = 0; i < dtResult.Rows.Count; i++)
        {
          //判断处理结果数量是否为0,0认为是空,直接添加行
          if (dtCompletePoint.Rows.Count <= 0)
          {
            DataRow dr = dtCompletePoint.NewRow();
            dr["PlanId"] = dtResult.Rows[i]["PlanId"];
            dr["TaskId"] = dtResult.Rows[i]["TaskId"];
            dr["FinishedCount"] = 1;
            dr["BoolFeedBack"] = dtResult.Rows[i]["BoolFeedBack"];
            dtCompletePoint.Rows.Add(dr);
          }
          //当行数不为0时认为已经存在数据需要进行判断
          else
          {
            //迭代遍历已经计算完成的结果集
            for (int j = 0; j < dtCompletePoint.Rows.Count; j++)
            {
              //当待处理和已处理数据集的TaskId一致时,将已处理的数量进行+1操作
              if (int.Parse(dtCompletePoint.Rows[j]["TaskId"].ToString()) == int.Parse(dtResult.Rows[i]["TaskId"].ToString()))
              {
                dtCompletePoint.Rows[j]["FinishedCount"] = int.Parse(dtCompletePoint.Rows[j]["FinishedCount"].ToString()) + 1;
                break;
              }
              //当已经是最后一行数据,但是待处理数据集与已经处理数据集仍未匹配,则添加
              else if (j == (dtCompletePoint.Rows.Count - 1) && int.Parse(dtCompletePoint.Rows[j]["TaskId"].ToString()) != int.Parse(dtResult.Rows[i]["TaskId"].ToString()))
              {
                DataRow dr = dtCompletePoint.NewRow();
                dr["PlanId"] = dtResult.Rows[i]["PlanId"];
                dr["TaskId"] = dtResult.Rows[i]["TaskId"];
                dr["FinishedCount"] = 1;
                dr["BoolFeedBack"] = dtResult.Rows[i]["BoolFeedBack"];
                dtCompletePoint.Rows.Add(dr);
                break;
              }
            }
          }
        }
        return dtCompletePoint;
      }
      catch
      {
        return dtCompletePoint;
      }
    }
    /// <summary>
    /// Calculates the complete point.
    /// </summary>
    /// <param name="dtPoint">仅到位点位集</param>
    /// <param name="PositionX">人员x坐标</param>
    /// <param name="PositionY">人员y坐标</param>
    /// <returns>DataTable.</returns>
    public static DataTable CalculateCompletePoint(DataTable dtPoint, string PositionX, string PositionY)
    {
      DataTable dtCompletePoint = new DataTable();
      dtCompletePoint.Columns.Add("PlanId", Type.GetType("System.String"));// 计划ID
      dtCompletePoint.Columns.Add("TaskId", Type.GetType("System.String"));// 任务ID
      dtCompletePoint.Columns.Add("SmID", Type.GetType("System.String"));// 点位ID
      dtCompletePoint.Columns.Add("TaskName", Type.GetType("System.String"));//任务名称
      dtCompletePoint.Columns.Add("SmX", Type.GetType("System.String"));//点位X坐标
      dtCompletePoint.Columns.Add("SmY", Type.GetType("System.String"));//点位X坐标
      dtCompletePoint.Columns.Add("PointName", Type.GetType("System.String"));//点位名称
      dtCompletePoint.Columns.Add("PointType", Type.GetType("System.String"));//点位名称
      dtCompletePoint.Columns.Add("BoolFeedBack", Type.GetType("System.String"));//点位名称
      try
      {
        double BetweenLength = 0;
        //进行迭代计算上传该点位于仅到位点位的距离是否满足完成点位的条件
        for (int i = 0; i < dtPoint.Rows.Count; i++)
        {
          BetweenLength = 0;
          //84:经纬度计算两点距离
          if (Map_Type == "2")
          {
            BetweenLength = CalculateDistance(double.Parse(dtPoint.Rows[i]["SmY"].ToString()), double.Parse(dtPoint.Rows[i]["SmX"].ToString()), double.Parse(PositionY), double.Parse(PositionX));
          }
          //84魔卡托:计算两点距离
          else if (Map_Type == "1")
          {
            BetweenLength = CalculateDistance(double.Parse(dtPoint.Rows[i]["SmY"].ToString()), double.Parse(dtPoint.Rows[i]["SmX"].ToString()), double.Parse(PositionY), double.Parse(PositionX));
            //BetweenLength = Math.Sqrt(Math.Pow(double.Parse(dtPoint.Rows[i]["SmX"].ToString()) - double.Parse(PositionX), 2) + Math.Pow(double.Parse(dtPoint.Rows[i]["SmY"].ToString()) - double.Parse(PositionY), 2));
          }
          //if (BetweenLength <= 100)
          if (BetweenLength <= 60)
          {
            DataRow dr = dtCompletePoint.NewRow();
            dr["PlanId"] = dtPoint.Rows[i]["PlanId"];
            dr["TaskId"] = dtPoint.Rows[i]["TaskId"];
            dr["SmID"] = dtPoint.Rows[i]["SmID"];
            dr["TaskName"] = dtPoint.Rows[i]["TaskName"];
            dr["SmX"] = dtPoint.Rows[i]["SmX"];
            dr["SmY"] = dtPoint.Rows[i]["SmY"];
            dr["PointName"] = dtPoint.Rows[i]["PointName"];
            dr["PointType"] = dtPoint.Rows[i]["PointType"];
            dr["BoolFeedBack"] = dtPoint.Rows[i]["BoolFeedBack"];
            dtCompletePoint.Rows.Add(dr);
          }
        }
        return dtCompletePoint;
      }
      catch (Exception ex)
      {
        return dtCompletePoint;
      }
    }
    ///获取考勤记录
    /// <summary>
    /// 获取考勤记录
    /// </summary>
    /// <param name="PersonId">获取考勤人员ID</param>
    /// <param name="DateStr">获取考勤年月,格式'2017-10'</param>
    /// <returns>System.String.</returns>
    public static string Get_WorkRecord(int PersonId, string DateStartStr, string DateEndStr)
    {
      //初始化查询结果集
      DataTable dt = null;
      string startDate = DateStartStr + " 00:00:00";
      string endDate = DateEndStr + " 23:59:59";
      //第二步初始化返回结果
      Results_Record result = new Results_Record();
      List<EventType_Record> EventType_Record = new List<EventType_Record>();
      try
      {
        string sql = string.Format(@" SELECT p.DeptId,p.PersonName,d.*
	                                                 FROM dbo.L_Person AS p 
	                                                      INNER JOIN(SELECT  Date  as UpTime,'' as PersonStatus,MIN(UpTime) StartTime,MAX(UpTime) EndTime,'' as BeiZhu,PersonId 
					                                                         FROM dbo.L_DeviceStatus where 1=1 and PersonId = '{2}' and  UpTime >= '{0}' and  UpTime <= '{1}' GROUP BY PersonId,Date)AS d 
                                                                     on p.PersonId=d.PersonId  order by UpTime desc ", startDate, endDate, PersonId);
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType_Record data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new EventType_Record();
            data.Lwr_Date = Drow["UpTime"].ToString();
            data.Lwr_PersonStatus = Drow["PersonStatus"].ToString();
            data.Lwr_StartTime = Drow["StartTime"].ToString();
            data.Lwr_EndTime = Drow["EndTime"].ToString();
            data.Lwr_BeiZhu = Drow["BeiZhu"].ToString();
            EventType_Record.Add(data);
          }
        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到考勤记录!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Record;
      return JsonConvert.SerializeObject(result);

    }
    ///获取下一步签到/签退步骤 山东济宁专用
    /// <summary>
    /// 获取下一步签到/签退步骤
    /// </summary>
    /// <param name="PersonId">巡检人员ID</param>
    /// <returns>System.String.</returns>
    public static string GetNextSignStep(int PersonId)
    {
      return JsonConvert.SerializeObject(GetNextSignStepByPersonId(PersonId));
    }
    ///山东济宁专用签到
    /// <summary>
    /// 山东济宁专用签到
    /// </summary>
    /// <param name="Lwr_PersonId">The LWR_ person identifier.</param>
    /// <param name="Lwr_BeiZhu">The LWR_ bei zhu.</param>
    /// <param name="Lwr_GpsStatus">The LWR_ GPS status.</param>
    /// <param name="Lwr_MobileStatus">The LWR_ mobile status.</param>
    /// <param name="Lwr_Power">The LWR_ power.</param>
    /// <param name="Lwr_XY">The LWR_ xy.</param>
    /// <param name="DeptId">The dept identifier.</param>
    /// <returns>System.String.</returns>
    public static string QianDao_JiNing(string Lwr_PersonId, string Lwr_BeiZhu, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power, string Lwr_XY, int DeptId)
    {
      //初始化返回对象
      Results_EventUpload upload = new Results_EventUpload();
      upload.result = false;
      upload.message = "失败";
      try
      {
        //判断用户ID不能为空
        if (string.IsNullOrEmpty(Lwr_PersonId))
        {
          upload.result = false;
          upload.message = "用户ID不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        if (string.IsNullOrEmpty(DeptId.ToString()))
        {
          upload.result = false;
          upload.message = "部门ID不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        if (string.IsNullOrEmpty(Lwr_XY))
        {
          upload.result = false;
          upload.message = "用户定位经纬度不能为空!";
          return JsonConvert.SerializeObject(upload);
        }
        //初始化系统当前时间
        DateTime DatetimeNow = DateTime.Now;
        //初始化用户上传时间
        string Lwr_UpTime = DatetimeNow.ToString("yyyy-MM-dd HH:mm:ss");
        //初始化用户上传日期
        string Lwr_Date = DatetimeNow.ToString("yyyy-MM-dd");
        //初始化当天开始时间
        string DayStartTime = DatetimeNow.ToString("yyyy-MM-dd") + " 00:00:00";
        //初始化开始时间
        string Lwr_StartTime = DatetimeNow.ToString("HH:mm:ss");
        //初始化结束时间
        string Lwr_EndTime = DatetimeNow.ToString("HH:mm:ss");
        //初始化上班时间
        string Lwr_Hour = string.Empty;
        //初始化签到状态 上班签到  下班签退
        string Lwr_PersonStatus = string.Empty;
        //初始化查询记录语句
        string strSelectSql = string.Empty;
        //初始化查询数据集
        DataTable dtSelect = new DataTable();
        //获取数据库中签到信息
        NextSignStep NextSignStep = GetNextSignStepByPersonId(int.Parse(Lwr_PersonId));
        if (!NextSignStep.result)
        {
          upload.result = false;
          upload.message = "用户的签到记录异常!";
          return JsonConvert.SerializeObject(upload);
        }
        //进行判断用户是签到还是签退
        if (NextSignStep.NextStep == 1)
        {
          Lwr_PersonStatus = "用户下班";
          Lwr_StartTime = string.Empty;
        }
        else
        {
          Lwr_PersonStatus = "上班正常";
          Lwr_EndTime = string.Empty;
        }
        //1-1:初始化数据保存语句
        string sql = string.Empty;
        //1-2:初始化数据保存结果
        int SaveResult = 0;


        sql = string.Format(@"insert into L_DeviceStatus(PersonId,DeptId,Date,StartTime,EndTime,Hour,BeiZhu,PersonStatus,UpTime,GpsStatus,MobileStatus,Power,XY) 
                                                                 values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')"
                                    , Lwr_PersonId, DeptId, Lwr_Date, Lwr_StartTime, Lwr_EndTime, Lwr_Hour, Lwr_BeiZhu, Lwr_PersonStatus, Lwr_UpTime, Lwr_GpsStatus, Lwr_MobileStatus, Lwr_Power, Lwr_XY);
        SaveResult = APP.SQLServer_Helper.Insert(sql);
        if (SaveResult > 0)
        {
          upload.result = true;
          upload.message = "提交考勤成功";
        }
        return JsonConvert.SerializeObject(upload);
      }
      catch (Exception e)
      {
        upload.result = false;
        upload.message = e.ToString();
        return JsonConvert.SerializeObject(upload);
      }
    }
    public static string Get_WorkRecord_JiNing(int PersonId, string DateStartStr, string DateEndStr)
    {
      //初始化查询结果集
      DataTable dt = null;
      string startDate = DateStartStr + " 00:00:00";
      string endDate = DateEndStr + " 23:59:59";
      //第二步初始化返回结果
      Results_Record result = new Results_Record();
      List<EventType_Record> EventType_Record = new List<EventType_Record>();
      try
      {
        string sql = string.Format(@" select lp.DeptId,lp.PersonId,lp.PersonName,ld.Date as UpDay,'' as  PersonStatus ,ld.UpTime   from L_Person lp 
                                              inner join  L_DeviceStatus  ld on lp.PersonId = ld.PersonId
                                              where lp.PersonId = {2} and ld.UpTime > '{0}' and ld.UpTime <= '{1}' order by UpTime desc "
                                   , startDate, endDate, PersonId);
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType_Record data;
          result.result = true;
          result.message = "成功";
          //处理上午签到
          DataRow[] drowAM = dt.Select(" UpTime <= '" + DateStartStr + " 12:00:00' ");
          if (drowAM.Length > 0)
          {
            data = new EventType_Record();
            data.Lwr_Date = drowAM[0]["UpDay"].ToString();
            data.Lwr_PersonStatus = string.Empty;
            data.Lwr_StartTime = drowAM[0]["UpTime"].ToString();
            data.Lwr_EndTime = drowAM.Length > 1 ? drowAM[drowAM.Length - 1]["UpTime"].ToString() : string.Empty;
            data.Lwr_BeiZhu = string.Empty;
            EventType_Record.Add(data);
          }
          //处理下午签到
          DataRow[] drowPM = dt.Select(" UpTime >= '" + DateStartStr + " 12:00:00' ");
          if (drowPM.Length > 0)
          {
            data = new EventType_Record();
            data.Lwr_Date = drowPM[0]["UpDay"].ToString();
            data.Lwr_PersonStatus = string.Empty;
            data.Lwr_StartTime = drowPM[0]["UpTime"].ToString();
            data.Lwr_EndTime = drowPM.Length > 1 ? drowPM[drowPM.Length - 1]["UpTime"].ToString() : string.Empty;
            data.Lwr_BeiZhu = string.Empty;
            EventType_Record.Add(data);
          }
        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到考勤记录!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Record;
      return JsonConvert.SerializeObject(result);

    }
    /// <summary>
    /// Gets the next sign step by person identifier.
    /// </summary>
    /// <param name="PersonId">The person identifier.</param>
    /// <returns>NextSignStep.</returns>
    public static NextSignStep GetNextSignStepByPersonId(int PersonId)
    {
      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      NextSignStep NextSignStep = new NextSignStep();
      //获取当前系统日期
      string CurrentDate = System.DateTime.Now.ToString("yyyy-MM-dd");
      //获取当前时间
      string CurrentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      //今天开始时间
      string TodayStart = CurrentDate + " 00:00:00";
      //今天结束时间
      string TodayEnd = CurrentDate + " 23:59:59";
      try
      {
        //初始化sql 查询巡检人员当天
        string sql = string.Format(@" SELECT Id,PersonId,DeptId,Date,StartTime,EndTime,Hour,BeiZhu,PersonStatus,UpTime,GpsStatus,MobileStatus,Power,XY
                                              FROM L_DeviceStatus where PersonId = {0} and date = '{1}' order by UpTime desc ", PersonId, CurrentDate);
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        //1:当签到记录为0条时,可直接签到
        if (dt.Rows.Count == 0)
        {
          //异常错误信息
          NextSignStep.message = "成功";
          NextSignStep.result = true;//成功
          NextSignStep.NextStep = 0;//允许签到
        }
        //2:当签到记录有4条的时候,不能进行签到/签退操作
        else if (dt.Rows.Count == 4)
        {
          NextSignStep.message = "今天已经签到/签退完成,不能在进行签到/签退!";
          NextSignStep.result = true;//成功
          NextSignStep.NextStep = -1;//不允许签到
        }
        //3:当签到记录大于等于一条小于4条时
        else if (dt.Rows.Count >= 1 && dt.Rows.Count < 4)
        {
          //第一步:获取最后一条上传的签到/签退记录签到时间
          DateTime LastSignTime = Convert.ToDateTime(dt.Rows[0]["UpTime"].ToString());
          //第二步:判断最后一条上传的记录与 13:00:00时间比较的情况   大于13点时
          if (DateTime.Compare(LastSignTime, DateTime.Parse(CurrentDate + " 13:00:00")) > 0)
          {
            DataRow[] drow = dt.Select(" UpTime > '" + CurrentDate + " 13:00:00' ");
            //当小于二则下午只有一条,则可以继续签退
            if (drow.Length < 2)
            {
              //12点之后只有一条签到信息,则只能进行签退
              NextSignStep.message = "成功!";
              NextSignStep.result = true;//成功
              NextSignStep.NextStep = 1;//签退
            }
            //当>=2时下午签到/签退结束 不能做任何操作
            else
            {
              NextSignStep.message = "成功!";
              NextSignStep.result = true;//失败
              NextSignStep.NextStep = -1;//签退
            }
          }
          //                                                      小于13点时
          else
          {
            //最后一条签到/签退时间小于 12点 但是当前时间大于12点时 直接可以进行签到
            if (DateTime.Compare(DateTime.Now, DateTime.Parse(CurrentDate + " 13:00:00")) > 0)
            {
              NextSignStep.message = "成功,可以进行签到!";
              NextSignStep.result = true;//成功
              NextSignStep.NextStep = 0;//签到
            }
            else
            {
              DataRow[] drow = dt.Select(" UpTime <= '" + CurrentDate + " 13:00:00' ");
              //当小于二则下午只有一条,则可以继续签退
              if (drow.Length < 2)
              {
                //12点之后只有一条签到信息,则只能进行签退
                NextSignStep.message = "成功,可以签退!";
                NextSignStep.result = true;//成功
                NextSignStep.NextStep = 1;//签退
              }
              //当>=2时下午签到/签退结束 不能做任何操作
              else
              {
                NextSignStep.message = "成功!";
                NextSignStep.result = true;//失败
                NextSignStep.NextStep = -1;//不能做任何操作
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        NextSignStep.message = e.ToString();
        NextSignStep.result = false;
        NextSignStep.NextStep = -1;
      }
      return NextSignStep;
    }
    /// 获取公告
    /// <summary>
    /// 获取公告
    /// </summary>
    /// <returns>System.String.</returns>
    public static string Get_Notice()
    {
      //初始化查询结果集
      DataTable dt = null;
      //第二步初始化返回结果
      Results_Notice result = new Results_Notice();
      List<EventType_Notice> EventType_Notice = new List<EventType_Notice>();
      try
      {
        string sql = string.Format(@" select ID,text,title,people,time from L_Notice where 1=1 and delStatua=0 order by time desc ");
        dt = APP.SQLServer_Helper.SelectDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          EventType_Notice data;
          result.result = true;
          result.message = "成功";
          foreach (DataRow Drow in dt.Rows)
          {
            data = new EventType_Notice();
            data.id = int.Parse(Drow["ID"].ToString());
            data.text = Drow["text"].ToString();
            data.title = Drow["title"].ToString();
            data.people = Drow["people"].ToString();
            data.time = Drow["time"].ToString();
            EventType_Notice.Add(data);
          }
        }
        else
        {
          //当登录失败后进行赋值登录错误信息
          result.result = false;
          result.message = "未查询到公告信息!";
        }
      }
      catch (Exception e)
      {
        //异常错误信息
        result.message = e.ToString();
      }
      result.Data = EventType_Notice;
      return JsonConvert.SerializeObject(result);

    }
    static string physicalDir = "";
    static string relaImagePath = string.Empty;
    /// <summary>
    /// 保存base64格式的数据到本地,同时转换为相对路径
    /// </summary>
    /// <param name="base64Photo">需要进行保存的图片base64图片</param>
    /// <returns></returns>
    public static string SaveImage(string base64Photo)
    {
      physicalDir = string.Empty;
      relaImagePath = string.Empty;
      string[] base64All = base64Photo.Split('$');
      foreach (string item in base64All)
      {
        string[] arr = item.Split('|');
        string base64Image = arr[0];//base64编码的图片
        string geshi = arr[1];//图片格式带点
        DateTime currentDay = DateTime.Now;
        //相对目录：作用是需要根据相对目录获取服务器上该目录的绝对物理目录
        //所有图片放在Image目录下，按照年月日的层次建立文件夹，文件名用guid命名
        string relativeDir = "/image/" + currentDay.Year + "/" + currentDay.Month + "/" + currentDay.Day;
        //相对路径：作用是需要返回已存储文件的网络路径
        string imageName = string.Format("{0:yyyyMMddHHmmssffff}", currentDay);
        string relativePath = string.Format("{0}/{1}{2}", relativeDir, imageName, geshi);
        //绝对物理目录：作用是需要判断该绝对物理目录是否存在
        string nowPhysicalDir = System.Web.HttpContext.Current.Server.MapPath(relativePath);//当前图片路径
        physicalDir += System.Web.HttpContext.Current.Server.MapPath(relativePath) + "|";//以|分割组合的所有图片路径
        UploadReturn imageReturn = new UploadReturn();
        //传过来的Base64Photo要进行URL编码
        //if (Base64StringToImage(base64Photo, physicalDir))
        if (Base64StringToImage(base64Image, nowPhysicalDir))
        {
          imageReturn.IsSuccess = true;
          string imagePath = relativePath;//HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" += HttpContext.Current.Request.Url.Port + 
                                          //返回的URL要进行URL编码，接收端要进行URL解码
          imageReturn.Path = imagePath;
          relaImagePath += imagePath + "|";
        }

      }
      //if (physicalDir != "")
      //{
      //    physicalDir = physicalDir.Substring(0, physicalDir.Length - 1);
      //}
      if (relaImagePath != "")
      {
        relaImagePath = relaImagePath.Substring(0, relaImagePath.Length - 1);
      }
      return relaImagePath;
    }
    /// <summary>
    /// 将base64图片保存到物理路径
    /// </summary>
    /// <param name="inputStr"></param>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private static bool Base64StringToImage(string inputStr, string fullPath)
    {
      try
      {
        string path = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(path))
        {
          Directory.CreateDirectory(path);
        }
        byte[] arr = Convert.FromBase64String(inputStr);
        MemoryStream ms = new MemoryStream(arr);
        Bitmap bmp = new Bitmap(ms);
        bmp.Save(fullPath);
        ms.Close();
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Base64StringToImage 转换失败\nException：" + ex.Message);
        return false;
      }
    }
    #endregion
    #region 内部类
    #region 事件类别内部类
    internal class Results
    {
      public Boolean result = false;
      public String message = "失败";
      public List<EventType> Data = new List<EventType>();
    }
    /// <summary>
    /// 事件类别内部类
    /// </summary>
    internal class EventType
    {
      //事件类别
      public int EventTypeId;
      //事件名称
      public String EventTypeName;
    }
    #endregion
    #region 紧急程度内部类
    internal class Results_Urgent_Degree
    {
      public Boolean result = false;
      public String message = "失败";
      public List<EventType_Urgent_Degree> Data = new List<EventType_Urgent_Degree>();
    }
    /// <summary>
    /// 事件类别内部类
    /// </summary>
    internal class EventType_Urgent_Degree
    {
      //事件类别
      public int UrgencyId;
      //事件名称
      public String UrgencyName;
    }
    #endregion
    #region 事件处理级别内部类
    internal class Results_Handler_Level
    {
      public Boolean result = false;
      public String message = "失败";
      public List<EventType_Handler_Level> Data = new List<EventType_Handler_Level>();
    }
    /// <summary>
    /// 事件类别内部类
    /// </summary>
    internal class EventType_Handler_Level
    {
      //事件类别
      public int HandlerLevelId;
      //事件名称
      public String HandlerLevelName;
    }
    #endregion
    #region 隐患上传事件结果内部类
    internal class Results_EventUpload
    {
      public Boolean result = false;
      public String message = "失败";
    }
    #endregion
    #region 考勤数据内部类
    internal class Results_Record
    {
      public Boolean result = false;
      public String message = "失败";
      public List<EventType_Record> Data = new List<EventType_Record>();
    }
    /// <summary>
    /// 事件类别内部类
    /// </summary>
    internal class EventType_Record
    {
      //日期
      public String Lwr_Date;
      //上班or下班的标识
      public String Lwr_PersonStatus;
      //上班时间
      public String Lwr_StartTime;
      //下班时间
      public String Lwr_EndTime;
      //备注信息
      public String Lwr_BeiZhu;

    }
    #endregion
    #region 公告数据内部类
    internal class Results_Notice
    {
      public Boolean result = false;
      public String message = "失败";
      public List<EventType_Notice> Data = new List<EventType_Notice>();
    }
    /// <summary>
    /// 事件类别内部类
    /// </summary>
    internal class EventType_Notice
    {
      //公告ID
      public int id;
      //公告内容
      public String text;
      //公告标题
      public String title;
      //作者
      public String people;
      //发布时间
      public String time;

    }
    #endregion
    #region 任务列表内部类
    internal class Results_TaskList
    {
      public Boolean result = false;
      public String message = "失败";
      public List<Data_TaskList> Data = new List<Data_TaskList>();
    }
    internal class Data_TaskList
    {
      public int TaskId;//1:任务ID
      public int PlanAreaId; //2:区域ID
      public String VisitStarTime;//3:任务开始时间
      public String VisitOverTime;//4:任务结束时间
      public String OperateDate; //5:操作时间
      public String Remark;//6:任务备注
      public String PersonName;//7:巡检员名字
      public int PersonId;//8:巡检员ID
      public string TaskName;//9:任务名称
      public int Finish;//10:完成状态
      public String PlanTypeName;//11:计划类别
      public int BoolFeedBack;//12:是否需要反馈
    }
    #endregion
    #region 任务明细内部类
    internal class Results_TaskInfo
    {
      public Boolean result = false;
      public String message = "失败";
      public List<Data_TaskInfo_ImportPoint> ImportPointData = new List<Data_TaskInfo_ImportPoint>();
      public List<Data_TaskInfo_EquPoint> EquPointData = new List<Data_TaskInfo_EquPoint>();
      public List<Data_TaskInfo> Data = new List<Data_TaskInfo>();
    }
    /// <summary>
    /// 任务主体信息
    /// </summary>
    internal class Data_TaskInfo
    {
      public String PlanAreaGeoText;//1:巡检区域GeoText
      public String PlanAreaName;//2:巡检区域名称
      public String PlanPathGeoText;//3:巡检路线GeoText
      public String PlanPathName;//4:巡检路线名称
    }
    /// <summary>
    /// 关键点内部类
    /// </summary>
    internal class Data_TaskInfo_ImportPoint
    {
      public String X;//1:X坐标
      public String Y;//2:Y坐标
      public int PatroState; //3:完成情况 0:未完成 1:已完成
      public String ImportPointName; //4:关键点名称        
      public int ImportPointId;//5:关键点id
      public int PointType;//6:关键点类别 1:关键点 2:设备实体
    }
    /// <summary>
    /// 设备实体内部类
    /// </summary>
    internal class Data_TaskInfo_EquPoint
    {
      public String X;//1:X坐标
      public String Y;//2:Y坐标
      public String EquType; //3:设备类别
      public int PatroState; //4:完成情况  0:未完成 1:已完成
      public int Smid;//5:设备实体ID
      public int PointType;//6:关键点类别 1:关键点 2:设备实体
    }
    #endregion
    #region
    internal class Results_Login
    {
      public Boolean result = false;
      public String message = "失败";
      public List<Data_Logion> Data = new List<Data_Logion>();
    }
    /// <summary>
    /// 用户登录验证
    /// </summary>
    internal class Data_Logion
    {
      //手机设备唯一标识
      public String Smid;
    }
    #endregion
    #endregion
    /// <summary>
    /// 计算两点之间的距离
    /// <remarks>#0 add qhy 20180109</remarks>
    /// </summary>
    /// <param name="latA">第一个点的纬度</param>
    /// <param name="lonA">第一个点的经度</param>
    /// <param name="latB">第二个点的纬度</param>
    /// <param name="lonB">第二个点的纬度</param>
    /// <returns>System.Double.</returns>
    private static double CalculateDistance(double x1, double y1, double x2, double y2)
    {
      var radLat1 = y1 * Math.PI / 180.0;
      var radLat2 = y2 * Math.PI / 180.0;
      var a = radLat1 - radLat2;
      var b = x1 * Math.PI / 180.0 - x2 * Math.PI / 180.0;
      var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
      s = s * 6378.137;
      s = Math.Round(s * Math.Pow(10, 9)) / Math.Pow(10, 9);
      return s * 1000;

      //double earthR = 6371000.0;
      //double x = Math.Cos(latA * Math.PI / 180.0) * Math.Cos(latB * Math.PI / 180.0) * Math.Cos((lonA - lonB) * Math.PI / 180.0);
      //double y = Math.Sin(latA * Math.PI / 180.0) * Math.Sin(latB * Math.PI / 180.0);
      //double s = x + y;
      //if (s > 1) s = 1;
      //if (s < -1) s = -1;
      //double alpha = Math.Acos(s);
      //double distance = alpha * earthR;
      //return distance;
    }
  }

}
