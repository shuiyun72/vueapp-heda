using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing;
using System.Web;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;


namespace WebInterface.dal
{
    public static class Data_Mantain_Dal
    {
        /// <summary>
        /// 用户登录验证方法体
        /// </summary>
        /// <param name="name">登录名</param>
        /// <param name="pwd">登录密码</param>        
        /// <returns>{“result”:true，”message “:”成功！”，”PersonId “:”00001”，” Phone “:”13245896745”，” PersonName “:”张三”}</returns>
        public static string User_CheckLogin(string name, string pwd)
        {
            DataTable dt = null;
            DataTable dt1 = null;
            string sql = string.Empty;
            //第一步:初始化DT1的格式
            dt1 = new DataTable("user");
            dt1.Columns.Add("PersonId", typeof(string));
            dt1.Columns.Add("PersonName", typeof(string));
            dt1.Columns.Add("Phone", typeof(string));
            dt1.Columns.Add("result", typeof(bool));
            dt1.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt1.NewRow();
            newRow["PersonId"] = string.Empty;
            newRow["PersonName"] = string.Empty;
            newRow["Phone"] = string.Empty;
            newRow["result"] = false;
            newRow["message"] = "失败";
            try
            {
                sql = string.Format(@"select PersonId, PersonName ,Telephone
                                             from V_P_Admin where 1=1 and PersonName='{0}' and PassWord='{1}'", name.Trim(), pwd.Trim());
                dt = APP.SQLServer_Helper.SelectDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    //当登录成功后进行赋值登录成功信息
                    newRow["PersonId"] = dt.Rows[0]["PersonId"];
                    newRow["PersonName"] = dt.Rows[0]["PersonName"].ToString();
                    newRow["Phone"] = dt.Rows[0]["Telephone"].ToString();
                    newRow["result"] = true;
                    newRow["message"] = "成功";
                }
                else
                {
                    //当登录失败后进行赋值登录错误信息
                    newRow["result"] = false;
                    newRow["message"] = "用户名或密码错误!";
                }
            }
            catch (Exception e)
            {
                newRow["message"] = e.ToString();
            }
            dt1.Rows.Add(newRow);
            return JsonTo.ToJson(dt1);
        }
        /// <summary>
        /// 获取工单列表
        /// </summary>
        /// <param name="personId">用户编号</param>       
        /// <returns>{“result”:true，”message “:”成功！”，”data”:[{“OrderTime”:”标题”, “PreEndTime”:”公告时间”,” EventCode”:”公告内容” , “EventType”:”标题”, “EventFrom”:”公告时间”,” EventContent”:”公告内容” , “EventAddress”:”标题”, “EventDesc”:”公告时间”,” UrgencyId”:”公告内容” ,“HandlerLevelId”:”标题”, “UpTime”:”公告时间”,” DispatchPerson”:”公告内容” ,” EventState”:”公告内容” ,“EventPictures”:”标题”, “EventX”:”公告时间”,” EventY”:”公告内容” }]} </returns>

        public static string getOrderList(string personId, string Type)
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Results result = new Results();
            List<OrderList> OrderListData = new List<OrderList>();
            try
            {
                string sql = string.Format(@"select w.OrderTime,w.PreEndTime,e.EventCode,EventTypeId.EventTypeName EventTypeId,f.EventFromName EventFromId,EventTypeId2.EventTypeName EventTypeId2,
                                            e.EventAddress,e.EventDesc,u.UrgencyName UrgencyId,hl.HandlerLevelName HandlerLevelId,e.UpTime,d.DeptName DeptId,w.DispatchPerson,w.OrderId,maxht.OperTime,
                                            (case h.OperId
                                            when 1 then '待接收'
                                            when 2 then '待接收'
                                            when 3 then '处理中'
                                            when 4 then '处理中'
                                            when 5 then '处理中'
                                            when 6 then '处理中'
                                            when 7 then '已处理'
                                            else '待接收'
                                            end) EventState,h.OperId,
                                            e.EventPictures,e.EventX,e.EventY,w.OrderCode,w.OrderStatus
                                            from M_WorkOrder w 
                                            left join M_Event e on w.EventID=e.EventID
                                            left join (select MAX(OperId)OperId,OrderId from M_WorkOrder_Oper_History Group by OrderId) h on w.OrderId=h.OrderId
                                            left join (select EventTypeId,EventTypeName from M_EventType where ParentTypeId=0) EventTypeId on e.EventTypeId=EventTypeId.EventTypeId
                                            left join (select EventTypeId,EventTypeName from M_EventType where ParentTypeId<>0) EventTypeId2 on e.EventTypeId2=EventTypeId2.EventTypeId
                                            left join M_EventFrom f on e.EventFromId=f.EventFromId
                                            left join M_HandlerLevel hl on e.HandlerLevelId=hl.HandlerLevelId
                                            left join L_Department d on w.DeptId=d.DeptId
                                            left join M_Urgency u on e.UrgencyId=u.UrgencyId
                                            left join (select MAX(OperTime) OperTime,OrderId from M_WorkOrder_Oper_History Group by OrderId) maxht on w.OrderId=maxht.OrderId 
                                            where e.IsValid=1 and DeleteStatus=0 and w.PersonId={0}", personId);
                if (Type == "1")
                {
                    sql += " and h.OperId in(1,2) and w.OrderStatus <> 1 Order by OrderTime desc";
                }
                if (Type == "2")
                {
                    sql += " and h.OperId in(3,4,5,6) and w.OrderStatus <> 1  Order by OrderTime desc";
                }
                //退单事件也为完成事件 20180305001
                if (Type == "3")
                {
                    sql += " and (h.OperId=7 or w.OrderStatus = 1)  Order by OrderTime desc";
                }
                dt = APP.SQLServer_Helper.SelectDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    OrderList data;
                    result.result = true;
                    result.message = "成功";
                    foreach (DataRow Drow in dt.Rows)
                    {
                        data = new OrderList();
                        data.OrderTime = Drow["OrderTime"].ToString();
                        data.PreEndTime = Drow["PreEndTime"].ToString();
                        data.EventCode = Drow["EventCode"].ToString();
                        data.EventType = Drow["EventTypeId"].ToString();
                        data.EventFrom = Drow["EventFromId"].ToString();
                        data.EventContent = Drow["EventTypeId2"].ToString();
                        data.EventAddress = Drow["EventAddress"].ToString();
                        data.EventDesc = Drow["EventDesc"].ToString();
                        data.UrgencyId = Drow["UrgencyId"].ToString();
                        data.HandlerLevelId = Drow["HandlerLevelId"].ToString();
                        data.UpTime = Drow["UpTime"].ToString();
                        data.DeptId = Drow["DeptId"].ToString();
                        data.DispatchPerson = Drow["DispatchPerson"].ToString();
                        data.EventState = Drow["EventState"].ToString();
                        data.EventPictures = Drow["EventPictures"].ToString();
                        data.EventX = Drow["EventX"].ToString();
                        data.EventY = Drow["EventY"].ToString();
                        data.OrderCode = Drow["OrderCode"].ToString();
                        data.OrderId = Drow["OrderId"].ToString();
                        data.OperTime = Drow["OperTime"].ToString();
                        data.OperId = Drow["OperId"].ToString();
                        data.OrderStatus = Drow["OrderStatus"].ToString();
                        OrderListData.Add(data);
                    }
                }
                else
                {
                    result.result = false;
                    result.message = "未查询到工单列表!";
                }
            }
            catch (Exception e)
            {
                //异常错误信息
                result.message = e.ToString();
            }
            result.data = OrderListData;
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 工单结果返回
        /// </summary>
        internal class Results
        {
            public Boolean result = false;
            public String message = "失败";
            public List<OrderList> data = new List<OrderList>();
        }

        internal class OrderList
        {
            //工单返回状态ID
            public string OperId;
            //工单完成时间
            public string OperTime;
            //工单ID
            public string OrderId;
            //派单时间
            public string OrderTime;
            //截止时间
            public string PreEndTime;
            //事件编号
            public string EventCode;
            //事件类型
            public string EventType;
            //事件来源
            public string EventFrom;
            //事件内容
            public string EventContent;
            //事件地址
            public string EventAddress;
            //事件描述
            public string EventDesc;
            //紧急程度
            public string UrgencyId;
            //处理级别
            public string HandlerLevelId;
            //上报时间
            public string UpTime;
            //派发部门
            public string DeptId;
            //上报人
            public string DispatchPerson;
            //事件状态
            public string EventState;
            //现场图片
            public string EventPictures;
            //X坐标
            public string EventX;
            //Y坐标
            public string EventY;
            //工单编码
            public string OrderCode;
            //退单状态
            public string OrderStatus;
        }
        /// <summary>
        /// 获取事件列表
        /// </summary>
        /// <returns></returns>
        public static string GetEvent()
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Result result = new Result();
            List<EventList> EventListData = new List<EventList>();
            try
            {
                string sql = string.Format(@"select EventCode,EventUpdateTime,EventAddress,p.PersonName,d.DeptName,et.EventTypeName EventTypeName,et2.EventTypeName EventTypeName2,
                                            ef.EventFromName,hl.HandlerLevelName,u.UrgencyName,EventDesc,EventX,EventY,e.EventID,e.EventPictures,e.EventVoices from M_Event e
                                            left join L_Person p on e.PersonId=p.PersonId
                                            left join L_Department d on e.DeptId=d.DeptId
                                            left join M_EventType et on e.EventTypeId=et.EventTypeId
                                            left join M_EventType et2 on e.EventTypeId2=et2.EventTypeId
                                            left join M_EventFrom ef on e.EventFromId=ef.EventFromId
                                            left join M_HandlerLevel hl on e.HandlerLevelId=hl.HandlerLevelId
                                            left join M_Urgency u on e.UrgencyId=u.UrgencyId 
                                            left join M_WorkOrder w on e.EventID=w.EventID
                                            left join (select MAX(OperId) OperId,OrderId from M_WorkOrder_Oper_History Group by OrderId) wh on w.OrderId=wh.OrderId
                                            where (wh.OperId is null or wh.OperId=1) and IsValid=1 and e.DeleteStatus=0
                                            order by EventUpdateTime desc");
                dt = APP.SQLServer_Helper.SelectDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    EventList data;
                    result.result = true;
                    result.message = "成功";
                    foreach (DataRow Drow in dt.Rows)
                    {
                        data = new EventList();
                        data.EventCode = Drow["EventCode"].ToString();
                        data.EventUpdateTime = Drow["EventUpdateTime"].ToString();
                        data.EventAddress = Drow["EventAddress"].ToString();
                        data.PersonName = Drow["PersonName"].ToString();
                        data.DeptName = Drow["DeptName"].ToString();
                        data.EventTypeName = Drow["EventTypeName"].ToString();
                        data.EventTypeName2 = Drow["EventTypeName2"].ToString();
                        data.EventFromName = Drow["EventFromName"].ToString();
                        data.HandlerLevelName = Drow["HandlerLevelName"].ToString();
                        data.UrgencyName = Drow["UrgencyName"].ToString();
                        data.EventDesc = Drow["EventDesc"].ToString();
                        data.EventX = Drow["EventX"].ToString();
                        data.EventY = Drow["EventY"].ToString();
                        data.EventId = Drow["EventId"].ToString();
                        data.EventPictures = Drow["EventPictures"].ToString();
                        data.EventVoices = Drow["EventVoices"].ToString();
                        EventListData.Add(data);
                    }
                }
                else
                {
                    result.result = true;
                    result.message = "未查询到事件列表!";
                }
            }
            catch (Exception e)
            {
                //异常错误信息
                result.message = e.ToString();
            }
            result.data = EventListData;
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 事件返回值
        /// </summary>
        internal class Result
        {
            public Boolean result = false;
            public String message = "失败";
            public List<EventList> data = new List<EventList>();
        }

        internal class EventList
        {
            //事件编号
            public string EventCode;
            //上报时间
            public string EventUpdateTime;
            //事件地址
            public string EventAddress;
            //上报人
            public string PersonName;
            //所属部门
            public string DeptName;
            //事件类型
            public string EventTypeName;
            //事件内容
            public string EventTypeName2;
            //事件来源
            public string EventFromName;
            //处理级别
            public string HandlerLevelName;
            //紧急程度
            public string UrgencyName;
            //事件描述
            public string EventDesc;
            //X坐标
            public string EventX;
            //Y坐标
            public string EventY;
            //事件id
            public string EventId;
            //图片
            public string EventPictures;
            //声音
            public string EventVoices;
        }
        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public static string GetDep()
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Rs result = new Rs();
            List<DepList> DepListData = new List<DepList>();
            try
            {
                string sql = string.Format(@"select DeptId,DeptName from L_Department");
                dt = APP.SQLServer_Helper.SelectDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    DepList data;
                    result.result = true;
                    result.message = "成功";
                    foreach (DataRow Drow in dt.Rows)
                    {
                        data = new DepList();
                        data.DeptId = Drow["DeptId"].ToString();
                        data.DeptName = Drow["DeptName"].ToString();
                        DepListData.Add(data);
                    }
                }
                else
                {
                    result.result = false;
                    result.message = "未查询到部门!";
                }
            }
            catch (Exception e)
            {
                //异常错误信息
                result.message = e.ToString();
            }
            result.data = DepListData;
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 部门返回值
        /// </summary>
        internal class Rs
        {
            public Boolean result = false;
            public String message = "失败";
            public List<DepList> data = new List<DepList>();
        }

        internal class DepList
        {
            //部门ID
            public string DeptId;
            //部门名称
            public string DeptName;
        }

        /// <summary>
        /// 获取部门中的人员
        /// </summary>
        /// <returns></returns>
        public static string GetDepPerson(int DeptId)
        {
            //初始化查询结果集
            DataTable dt = null;
            //第二步初始化返回结果
            Rst result = new Rst();
            List<PersonList> PersonListData = new List<PersonList>();
            try
            {
                string sql = string.Format(@"select PersonId,PersonName from L_Person where DepartmentId=" + DeptId);
                dt = APP.SQLServer_Helper.SelectDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    PersonList data;
                    result.result = true;
                    result.message = "成功";
                    foreach (DataRow Drow in dt.Rows)
                    {
                        data = new PersonList();
                        data.personId = Convert.ToInt32(Drow["PersonId"]);
                        data.personName = Drow["PersonName"].ToString();
                        PersonListData.Add(data);
                    }
                }
                else
                {
                    result.result = false;
                    result.message = "未查询到部门人员!";
                }
            }
            catch (Exception e)
            {
                //异常错误信息
                result.message = e.ToString();
            }
            result.data = PersonListData;
            return JsonConvert.SerializeObject(result);
        }

        internal class Rst
        {
            public Boolean result = false;
            public String message = "失败";
            public List<PersonList> data = new List<PersonList>();
        }
        internal class PersonList
        {
            //部门ID
            public int personId;
            //部门名称
            public string personName;
        }

        public static string SaveWorkOrder(int DepId, int PersonId, string OrderTime, string PreEndTime, int EventId, string UserName)
        {
            string ErrInfo = string.Empty;
            DataTable dt1 = null;
            string sql = string.Empty;
            //第一步:初始化DT1的格式
            dt1 = new DataTable("user");
            dt1.Columns.Add("result", typeof(bool));
            dt1.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt1.NewRow();
            newRow["result"] = false;
            newRow["message"] = "失败";
            try
            {
                sql = string.Format(@"insert into M_WorkOrder (EventID,DeptId,PersonId,DispatchPerson,OrderTime,PreEndTime)
                                    values ('{0}','{1}','{2}','{3}','{4}','{5}')", EventId, DepId, PersonId, UserName, OrderTime, PreEndTime);
                string sqlCode = sql + " select @@identity;";
                int GDID = APP.SQLServer_Helper.GetSqlExecScalar(sqlCode, out ErrInfo);
                string type = "GD";
                string timenum = System.DateTime.Now.ToString("yyyy").Substring(2, 2);
                string OrderCode = type + timenum + GDID.ToString().PadLeft(7, '0');
                string sqlUpdate = "update M_WorkOrder Set OrderCode='" + OrderCode + "' where OrderId='" + GDID + "'";
                int h = APP.SQLServer_Helper.UpDate(sqlUpdate);
                string sqlEvent = "select * from M_Event where EventID=" + EventId;
                DataTable dt = APP.SQLServer_Helper.SelectDataTable(sqlEvent);
                string sqlh = "insert into M_WorkOrder_Oper_History(OrderId,OperId,OperTime,Pictures,Voices) values ('" + GDID + "','1','" + OrderTime + "','" + dt.Rows[0]["EventPictures"] + "','" + dt.Rows[0]["EventVoices"] + "')insert into M_WorkOrder_Oper_History(OrderId,OperId,OperTime,Pictures,Voices) values ('" + GDID + "','2','" + OrderTime + "','" + dt.Rows[0]["EventPictures"] + "','" + dt.Rows[0]["EventVoices"] + "')";
                int s = APP.SQLServer_Helper.Insert(sqlh);
                if (string.IsNullOrEmpty(ErrInfo) && h > 0 && s > 0)
                {
                    //成功后
                    newRow["result"] = true;
                    newRow["message"] = "成功";
                }
                else
                {
                    //失败后
                    newRow["result"] = false;
                    newRow["message"] = "失败!";
                }
            }
            catch (Exception e)
            {
                newRow["message"] = e.ToString();
            }
            dt1.Rows.Add(newRow);
            return JsonTo.ToJson(dt1);
        }

        public static string EventInvalid(int EventID)
        {
            DataTable dt = null;
            //第一步:初始化DT的格式
            dt = new DataTable("EventInvalid");
            dt.Columns.Add("result", typeof(bool));
            dt.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt.NewRow();
            newRow["result"] = false;
            newRow["message"] = "失败";
            try
            {
                string sql = string.Format(@"update M_Event set IsValid=0 where EventID={0}", EventID);
                int n = APP.SQLServer_Helper.UpDate(sql);
                if (n > 0)
                {
                    //成功后
                    newRow["result"] = true;
                    newRow["message"] = "成功";
                }
                else
                {
                    //失败后
                    newRow["result"] = false;
                    newRow["message"] = "失败!";
                }
            }
            catch (Exception e)
            {
                newRow["message"] = e.ToString();
            }
            dt.Rows.Add(newRow);
            return JsonTo.ToJson(dt);
        }

        /// <summary>
        /// 提交退单
        /// </summary>
        /// <param name="OrderCode">工单编号</param>
        /// <param name="PersonId">提交人编号</param>
        /// <param name="describe">退单描述</param>
        /// <returns>{“result”:true，”message “:”成功！”}</returns>
        public static string CommitChargeBack(String OrderId, String PersonId, String describe)
        {
            DataTable dt = null;
            //第一步:初始化DT的格式
            dt = new DataTable("CommitChargeBack");
            dt.Columns.Add("result", typeof(bool));
            dt.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt.NewRow();
            newRow["result"] = false;
            newRow["message"] = "失败";
           
            string ErrInfo = "";
            string sql = string.Format(@"select * from M_WorkOrder where OrderId={0}", OrderId);
            DataTable DT;
            if (APP.SQLServer_Helper.GetDataTable(sql, out DT, out ErrInfo))
            {
                if (DT.Rows.Count > 0)
                {

                    string SQL = string.Format(@"begin transaction;update M_WorkOrder Set OrderStatus=1 where OrderId='{0}'
                                             insert into M_WorkOrder_Back (DeptId,PersonId,OrderId,BackTime,BackRemarks)
                                             values ((select DepartmentId from L_Person where PersonId='{1}'),'{1}','{0}','{2}','{3}')", OrderId, PersonId, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), describe.Trim());
                    SQL += ";update M_Event Set IsValid=4,Remark_Back = '" + describe.Trim() + "' where EventID=" + DT.Rows[0]["EventID"];
                    //建立SQL
                    Tool.DB.MakeSql MS = new Tool.DB.MakeSql(Tool.DB.SqlStringType.INSERT, "M_WorkOrder_Oper_History");
                    MS.AddField("EventID", DT.Rows[0]["EventID"], Tool.DB.FieldType.STR);
                    MS.AddField("OrderId", OrderId, Tool.DB.FieldType.STR);
                    MS.AddField("OperId", 11, Tool.DB.FieldType.STR);
                    MS.AddField("OperTime", DateTime.Now.ToString(), Tool.DB.FieldType.STR);
                    MS.AddField("OperRemarks", describe.Trim(), Tool.DB.FieldType.STR);
                    MS.AddField("DispatchPersonID", PersonId, Tool.DB.FieldType.STR);
                    MS.AddField("ExecPersonId", "(SELECT TOP 1 DispatchPersonID  FROM M_WorkOrder_Oper_History where EventID=" + DT.Rows[0]["EventID"] + " AND dbo.M_WorkOrder_Oper_History.OperId =11   ORDER BY ExecUpDateTime DESC )", Tool.DB.FieldType.INT);
                    MS.AddField("ExecDetpID", "(SELECT TOP 1 DeptId  FROM M_WorkOrder where OrderId=" + OrderId + " ORDER BY OrderId DESC )", Tool.DB.FieldType.INT);
                    MS.AddField("IsValid", "4", Tool.DB.FieldType.STR);
                    SQL += ";" + MS.ToString() + ";commit transaction";

                    try
                    {
                        int N = APP.SQLServer_Helper.UpDate(SQL);
                        if (N > 0)
                        {
                            //成功后
                            newRow["result"] = true;
                            newRow["message"] = "成功"+ SQL;
                        }
                        else
                        {
                            //失败后
                            newRow["result"] = false;
                            newRow["message"] = "失败!";
                        }
                    }
                    catch (Exception e)
                    {
                        newRow["message"] = e.ToString();
                    }

                }
                else
                {
                    //失败后
                    newRow["result"] = false;
                    newRow["message"] = "失败!";
                }


            }
            else
            {
                //失败后
                newRow["result"] = false;
                newRow["message"] = "失败!";
            }

            dt.Rows.Add(newRow);
            return JsonTo.ToJson(dt);
        }
        /// <summary>
        /// 养护系统提交延期
        /// </summary>
        /// <param name="OrderId">工单编号</param>
        /// <param name="describe">延期原因</param>
        /// <param name="complishTime">预计完成时间</param>
        /// <returns>结果格式：{“result”:true，”message “:”成功！”}</returns>
        public static string CommitDelay(int OrderId, string describe, string complishTime)
        {

            int INSERT = 0;
            DataTable dt1 = null;
            string sql = string.Empty;
            //第一步:初始化DT1的格式
            dt1 = new DataTable("user");
            dt1.Columns.Add("result", typeof(bool));
            dt1.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt1.NewRow();
            newRow["result"] = false;
            newRow["message"] = "失败";


            string ErrInfo = "";
            sql = string.Format(@"select * from M_WorkOrder where OrderId={0}", OrderId);
            DataTable DT;
            if (APP.SQLServer_Helper.GetDataTable(sql, out DT, out ErrInfo))
            {
                if (DT.Rows.Count > 0)
                {

                    string SQL = string.Format(@"begin transaction; Insert INTO M_PostponeOrder(EventID,OrderId, Cause, PostponeTime, ApplicationTime) VALUES({0},{1},'{2}','{3}','{4}')", DT.Rows[0]["EventID"], OrderId, describe.Trim(), complishTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                           SQL += ";update M_Event Set IsValid=5,Remark_Back = '" + describe.Trim() + "' where EventID=" + DT.Rows[0]["EventID"];
                    //建立SQL
                    Tool.DB.MakeSql MS = new Tool.DB.MakeSql(Tool.DB.SqlStringType.INSERT, "M_WorkOrder_Oper_History");
                    MS.AddField("EventID", DT.Rows[0]["EventID"], Tool.DB.FieldType.STR);
                    MS.AddField("OrderId", OrderId, Tool.DB.FieldType.STR);
                    MS.AddField("OperId", "(SELECT TOP 1 OperId  FROM M_WorkOrder_Oper_History where OrderId=" + OrderId + " ORDER BY ExecUpDateTime DESC )", Tool.DB.FieldType.INT);
                    MS.AddField("OperTime", DateTime.Now.ToString(), Tool.DB.FieldType.STR);
                    MS.AddField("OperRemarks", describe.Trim(), Tool.DB.FieldType.STR);
                    MS.AddField("DispatchPersonID", DT.Rows[0]["PersonId"], Tool.DB.FieldType.STR);

                    //MS.AddField("ExecPersonId", "(SELECT TOP 1 DispatchPersonID  FROM M_WorkOrder_Oper_History where EventID=" + DT.Rows[0]["EventID"] + " AND dbo.M_WorkOrder_Oper_History.OperId =11   ORDER BY ExecUpDateTime DESC )", Tool.DB.FieldType.INT);
                    //MS.AddField("ExecDetpID", "(SELECT TOP 1 DeptId  FROM M_WorkOrder where OrderId=" + OrderId + " ORDER BY OrderId DESC )", Tool.DB.FieldType.INT);

                    MS.AddField("ExecPersonId", "(SELECT TOP 1 PersonId  FROM M_Event where EventID=" + DT.Rows[0]["EventID"] + " ORDER BY EventID DESC )", Tool.DB.FieldType.INT);
                    MS.AddField("ExecDetpID", "(SELECT TOP 1 DeptId  FROM M_Event where EventID=" + DT.Rows[0]["EventID"] + " ORDER BY EventID DESC )", Tool.DB.FieldType.INT);

                    MS.AddField("IsValid", "5", Tool.DB.FieldType.STR);
                    SQL += ";" + MS.ToString() + ";commit transaction";

                    try
                    {
                        INSERT = APP.SQLServer_Helper.Insert(SQL);
                        if (INSERT > 0)
                        {
                            //成功后
                            newRow["result"] = true;
                            newRow["message"] = "成功";
                        }
                        else
                        {
                            //失败后
                            newRow["result"] = false;
                            newRow["message"] = "失败!";
                        }
                    }
                    catch (Exception e)
                    {
                        newRow["message"] = e.ToString();
                    }

                }
                else
                {
                    //失败后
                    newRow["result"] = false;
                    newRow["message"] = "失败!";
                }


            }
            else
            {
                //失败后
                newRow["result"] = false;
                newRow["message"] = "失败!";
            }
            dt1.Rows.Add(newRow);
            return JsonTo.ToJson(dt1);
        }

        /// <summary>
        /// 养护系统提交到场、维修、完工
        /// </summary>
        /// <param name="OrderId">工单编号</param>
        /// <param name="PersonId">提交人编号</param>
        /// <param name="describe">描述</param>
        /// <param name="EventPictures">图片总路径</param>
        ///  <param name="EventVoices">音频总路径</param>
        ///  <param name="TaskType">4:到场；5：处理；6：完工</param>
        /// <returns>结果格式：{“result”:true，”message “:”成功！”}</returns>
        public static string CommitOrderTask(int OrderId, int PersonId, string describe, string EventPictures, string EventVoices, int TaskType)
        {
            int INSERT = 0;
            DataTable dt1 = null;
            string sql = string.Empty;
            //第一步:初始化DT1的格式
            dt1 = new DataTable("user");
            dt1.Columns.Add("result", typeof(bool));
            dt1.Columns.Add("message", typeof(string));
            //第二步:初始化值
            DataRow newRow;
            newRow = dt1.NewRow();
            newRow["result"] = false;
            newRow["message"] = "失败";
            string NOW = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ImagePath = "";
            string VoicesPath = "";

            if (!string.IsNullOrEmpty(EventPictures))
            {
                string[] arr = EventPictures.Split('&');
                int i = 0;
                foreach (string liu in arr)
                {
                    ImagePath += SaveImage(liu);
                    if (i != arr.Length - 1)
                    {
                        ImagePath += "|";
                    }
                    i++;
                }

            }
            if (!string.IsNullOrEmpty(EventVoices))
            {
                VoicesPath = SaveTolo(EventVoices);
            }
            try
            {

                if(TaskType==6)
                {
                    string ErrInfo = "";
                    sql = string.Format(@"select * from M_WorkOrder where OrderId={0}",OrderId);
                    DataTable DT;
                    if (APP.SQLServer_Helper.GetDataTable(sql, out DT, out ErrInfo))
                    {
                        if(DT.Rows.Count>0)
                        {
                            sql = string.Format(@"Insert INTO M_WorkOrder_Oper_History 
                                                  ( EventID,OrderId,OperId,OperTime,DispatchPersonID,OperRemarks,ExecPersonId,ExecDetpID,Pictures,Voices) 
                                                   VALUES ({0},'{1}','{2}','{3}',{4},'{5}',{6},{7},'{8}','{9}')", 
                                                   
                                                   DT.Rows[0]["EventID"], DT.Rows[0]["OrderId"], TaskType, NOW, PersonId,
                                                   describe.Trim(),
                                                   "(SELECT TOP 1 PersonId  FROM M_Event where EventID=" + DT.Rows[0]["EventID"] + " ORDER BY EventID DESC )",
                                                   "(SELECT TOP 1 DeptId  FROM M_Event where EventID=" + DT.Rows[0]["EventID"] + " ORDER BY EventID DESC )",
                                                   ImagePath, VoicesPath);

                        }
                        else
                        {
                            //失败后
                            newRow["result"] = false;
                            newRow["message"] = "失败!";
                        }


                    }
                    else
                    {
                        //失败后
                        newRow["result"] = false;
                        newRow["message"] = "失败!";
                    }


                }
                else if(TaskType == 7)
                {
                    string ErrInfo = "";
                    sql = string.Format(@"select * from M_WorkOrder where OrderId={0}", OrderId);
                    DataTable DT;
                    if (APP.SQLServer_Helper.GetDataTable(sql, out DT, out ErrInfo))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            sql = string.Format(@"Insert INTO M_WorkOrder_Oper_History 
                                                  ( EventID,OrderId,OperId,OperTime,DispatchPersonID,OperRemarks,Pictures,Voices) 
                                                   VALUES ({0},'{1}','{2}','{3}',{4},'{5}','{6}','{7}')",
                                                   DT.Rows[0]["EventID"], DT.Rows[0]["OrderId"], TaskType, NOW, PersonId,
                                                   describe.Trim(),
                                                   ImagePath, VoicesPath);
                        }
                        else
                        {
                            //失败后
                            newRow["result"] = false;
                            newRow["message"] = "失败!";
                        }


                    }
                    else
                    {
                        //失败后
                        newRow["result"] = false;
                        newRow["message"] = "失败!";
                    }
                }
                else
                {
                    sql = string.Format(@"Insert INTO M_WorkOrder_Oper_History (OrderId,OperRemarks,Pictures,Voices,OperId,OperTime) VALUES ({0},'{1}','{2}','{3}',{4},'{5}')", OrderId, describe.Trim(), ImagePath, VoicesPath, TaskType, NOW);
                }



                INSERT = APP.SQLServer_Helper.Insert(sql);
                if (INSERT > 0)
                {
                    //成功后
                    newRow["result"] = true;
                    newRow["message"] = "成功";
                }
                else
                {
                    //失败后
                    newRow["result"] = false;
                    newRow["message"] = "失败!";
                }
            }
            catch (Exception e)
            {
                newRow["message"] = e.ToString();
            }
            dt1.Rows.Add(newRow);
            return JsonTo.ToJson(dt1);
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
                    //string imagePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + relativePath;
                    string imagePath = relativePath;
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
        /// <summary>
        /// 保存base64格式的数据到本地,同时转换为相对路径
        /// </summary>
        /// <param name="base64Tolo">需要进行保存的文件base64文件</param>
        /// <returns></returns>
        public static string SaveTolo(string base64Tolo)
        {
            base64Tolo = base64Tolo.Trim().Replace(" ", "");

            physicalDir = string.Empty;
            relaImagePath = string.Empty;
            string[] base64All = base64Tolo.Split('$');
            foreach (string item in base64All)
            {
                string[] arr = item.Split('|');
                string base64Image = arr[0];//base64编码的文件
                string geshi = string.Empty;
                if (arr[1].Split('.').Length >= 2)
                {
                    geshi = "." + arr[1].Split('.')[1];
                }
                else
                {
                    geshi = arr[1];//文件格式带点
                }
                DateTime currentDay = DateTime.Now;
                //相对目录：作用是需要根据相对目录获取服务器上该目录的绝对物理目录
                //所有文件放在Image目录下，按照年月日的层次建立文件夹，文件名用guid命名
                string relativeDir = "/image/" + currentDay.Year + "/" + currentDay.Month + "/" + currentDay.Day;
                //相对路径：作用是需要返回已存储文件的网络路径
                string imageName = string.Format("{0:yyyyMMddHHmmssffff}", currentDay);
                string relativePath = string.Format("{0}/{1}{2}", relativeDir, imageName, geshi);
                //绝对物理目录：作用是需要判断该绝对物理目录是否存在
                string nowPhysicalDir = System.Web.HttpContext.Current.Server.MapPath(relativePath);//当前文件路径
                physicalDir += System.Web.HttpContext.Current.Server.MapPath(relativePath) + "|";//以|分割组合的所有文件路径
                UploadReturn imageReturn = new UploadReturn();
                //传过来的base64Tolo要进行URL编码
                //if (Base64StringTolo(base64Tolo, physicalDir))
                if (Base64StringTolo(base64Image, nowPhysicalDir))
                {
                    imageReturn.IsSuccess = true;
                    //string imagePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + relativePath;
                    string imagePath = relativePath;
                    //返回的URL要进行URL编码，接收端要进行URL解码
                    imageReturn.Path = imagePath;
                    if (imagePath.Substring(imagePath.Length - 3, 3) == "amr" || imagePath.Substring(imagePath.Length - 3, 3) == "m4a")
                    {
                        imagePath = imagePath.Substring(0, imagePath.Length - 3) + "mp3";
                    }
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
        /// 将base64文件保存到物理路径
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>

        private static bool Base64StringTolo(string inputStr, string fullPath)
        {
            try
            {
                string string1 = inputStr;
                string fileName = fullPath;
                //获取文件目录
                string path = Path.GetDirectoryName(fullPath);
                //确定获取的目录是否存在
                if (!Directory.Exists(path))
                { //创建目录文件夹
                    Directory.CreateDirectory(path);
                }
                byte[] bytes = Convert.FromBase64String(inputStr);
                //文件流：如果存在则打开 如果不存在则创建 
                using (FileStream writer = new FileStream(fullPath, FileMode.OpenOrCreate))
                {
                    writer.Write(bytes, 0, bytes.Length);
                }
                //当声音格式为amr时需要转化为MP3格式
                if (fullPath.Substring(fullPath.Length - 3, 3) == "amr" || fullPath.Substring(fullPath.Length - 3, 3) == "m4a")
                {
                    string relativeDir = "/image/ffmpeg.exe";
                    string PhysicalDir = System.Web.HttpContext.Current.Server.MapPath(relativeDir);
                    Process pro = new Process();
                    pro.StartInfo.FileName = PhysicalDir;
                    pro.StartInfo.Arguments = " -i " + fullPath + " " + fullPath.Substring(0, fullPath.Length - 3) + "mp3";
                    pro.Start();
                    if (pro.HasExited)
                    {
                        try { pro.Kill(); }
                        catch { }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Base64StringTolo 转换失败\nException：" + ex.Message);
                return false;
            }
        }       

        #region 养护管理模块
        /// <summary>
        /// Gets the main tain task.
        /// </summary>
        /// <param name="PersonId">The person identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetMainTainTask(int PersonId)
        {
            result_Maintain result_Maintain = new result_Maintain();
            List<MaintainTask> MaintainTaskList = new List<MaintainTask>();
            result_Maintain.message = "失败";
            result_Maintain.result = false;
            try
            {
                string strCurrentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string strSqlSelect = string.Format(@" select ym.Y_TaskId,ym.Y_TaskName,ym.RoadName,ym.Y_FinishState,ym.Y_StarTime,ym.Y_EndTime ,ymde.FinishCount,ymde.UnFinishCount
                                                              from  Y_MainTainTask ym  
                                                              left join(
                                                                        select isnull(SUM(case when ymd.FinishState = 1 then 1 else 0 end ),0) as  FinishCount,
                                                                               isnull(SUM(case when ymd.FinishState = 0 then 1 else 0 end ),0) as  UnFinishCount,ymd.Y_TaskId
                                                                        from  Y_MainTainDetail ymd group by ymd.Y_TaskId) ymde  on ym.Y_TaskId   = ymde.Y_TaskId 
                                                                              where 1=1 and Y_DeleteState = 1 and Y_AssignState = 1 and Y_ProraterId ={0}  and Y_StarTime <='{1}' and Y_EndTime >='{1}' ", PersonId, strCurrentDate);
                DataTable dtSelect = new DataTable();
                dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
                if (dtSelect.Rows.Count > 0)
                {
                    result_Maintain.message = "成功";
                    result_Maintain.result = true;
                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        MaintainTask MaintainTask = new MaintainTask();
                        MaintainTask.Y_TaskId = int.Parse(dtSelect.Rows[i]["Y_TaskId"].ToString());//养护任务ID 
                        MaintainTask.Y_TaskName = dtSelect.Rows[i]["Y_TaskName"].ToString();//养护任务名称
                        MaintainTask.Y_StarTime = dtSelect.Rows[i]["Y_StarTime"].ToString();//养护开始时间
                        MaintainTask.Y_EndTime = dtSelect.Rows[i]["Y_EndTime"].ToString();//养护结束时间
                        MaintainTask.RoadName = dtSelect.Rows[i]["RoadName"].ToString();//养护道路名称
                        MaintainTask.Y_FinishState = int.Parse(dtSelect.Rows[i]["Y_FinishState"].ToString());//完成状态
                        MaintainTask.FinishCount = int.Parse(dtSelect.Rows[i]["FinishCount"].ToString());//已养护数量
                        MaintainTask.UnFinishCount = int.Parse(dtSelect.Rows[i]["UnFinishCount"].ToString());//已养护数量
                        MaintainTaskList.Add(MaintainTask);
                    }
                }
                else
                {
                    result_Maintain.message = "未查询到养护任务!";
                    result_Maintain.result = true;
                }
                result_Maintain.data = MaintainTaskList;
                return JsonConvert.SerializeObject(result_Maintain);
            }
            catch (Exception ex)
            {
                result_Maintain.message = ex.ToString();
                result_Maintain.result = false;
                result_Maintain.data = MaintainTaskList;
                return JsonConvert.SerializeObject(result_Maintain);
            }
        }
        /// <summary>
        /// 获取养护任务明细
        /// </summary>
        /// <param name="TaskId">The task identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetMainTainTaskDetail(int TaskId)
        {
            result_MaintainTask result_MaintainTask = new result_MaintainTask();
            List<MaintainTaskDetail> MaintainTaskDetailList = new List<MaintainTaskDetail>();
            result_MaintainTask.message = "失败";
            result_MaintainTask.result = false;
            try
            {
                string strSqlSelect = string.Format(@" SELECT SmID,SmX,SmY,EquType,FinishState  FROM Y_MainTainDetail where 1=1 and Y_TaskId = {0} ", TaskId);
                DataTable dtSelect = new DataTable();
                dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
                if (dtSelect.Rows.Count > 0)
                {
                    result_MaintainTask.message = "成功";
                    result_MaintainTask.result = true;
                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        MaintainTaskDetail MaintainTaskDetail = new MaintainTaskDetail();
                        MaintainTaskDetail.SmID = int.Parse(dtSelect.Rows[i]["SmID"].ToString());//设备ID
                        MaintainTaskDetail.FinishState = int.Parse(dtSelect.Rows[i]["FinishState"].ToString());//完成状态
                        MaintainTaskDetail.SmX = dtSelect.Rows[i]["SmX"].ToString();//经度
                        MaintainTaskDetail.SmY = dtSelect.Rows[i]["SmY"].ToString();//纬度
                        MaintainTaskDetail.EquType = dtSelect.Rows[i]["EquType"].ToString();//纬度
                        MaintainTaskDetailList.Add(MaintainTaskDetail);
                    }
                }
                else
                {
                    result_MaintainTask.message = "未查询到养护任务!";
                    result_MaintainTask.result = true;
                }
                result_MaintainTask.data = MaintainTaskDetailList;
                return JsonConvert.SerializeObject(result_MaintainTask);
            }
            catch (Exception ex)
            {
                result_MaintainTask.message = ex.ToString();
                result_MaintainTask.result = false;
                result_MaintainTask.data = MaintainTaskDetailList;
                return JsonConvert.SerializeObject(result_MaintainTask);
            }
        }
        /// <summary>
        /// Gets the last eq up information.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetLastEqUpInfo(int SmID, string EquType)
        {
            result_LastEqUp result_LastEqUp = new result_LastEqUp();
            List<LastEqUpInfo> LastEqUpInfoList = new List<LastEqUpInfo>();
            result_LastEqUp.message = "失败";
            result_LastEqUp.result = false;
            try
            {
                string strSqlSelect = string.Format(@" select top 1 isnull(Change_Number,0) as Change_Number,
			                                           isnull(Stop_Number,0) as Stop_Number,
			                                           isnull(Device_Status,'') as Device_Status			   
                                                       from Y_MainTainDetail where SmID = {0} and  FinishState = 1 and EquType like '{1}%' order by FinishDate desc ", SmID, EquType);
                DataTable dtSelect = new DataTable();
                dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
                if (dtSelect.Rows.Count > 0)
                {
                    result_LastEqUp.message = "成功";
                    result_LastEqUp.result = true;
                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        LastEqUpInfo LastEqUpInfo = new LastEqUpInfo();
                        LastEqUpInfo.Change_Number = int.Parse(dtSelect.Rows[i]["Change_Number"].ToString());//设备ID
                        LastEqUpInfo.Stop_Number = int.Parse(dtSelect.Rows[i]["Stop_Number"].ToString());//完成状态
                        LastEqUpInfo.Device_Status = dtSelect.Rows[i]["Device_Status"].ToString();//经度                       
                        LastEqUpInfoList.Add(LastEqUpInfo);
                    }
                }
                else
                {
                    result_LastEqUp.message = "未查询到数据!";
                    result_LastEqUp.result = true;
                }
                result_LastEqUp.data = LastEqUpInfoList;
                return JsonConvert.SerializeObject(result_LastEqUp);
            }
            catch (Exception ex)
            {
                result_LastEqUp.message = ex.ToString();
                result_LastEqUp.result = false;
                result_LastEqUp.data = LastEqUpInfoList;
                return JsonConvert.SerializeObject(result_LastEqUp);
            }
        }
        /// <summary>
        /// Ups the equipment information.
        /// </summary>
        /// <param name="SmID">The sm identifier.</param>
        /// <param name="Y_TaskId">The y_ task identifier.</param>
        /// <param name="Device_Status">The device_ status.</param>
        /// <param name="Change_Number">The change_ number.</param>
        /// <param name="Stop_Number">The stop_ number.</param>
        /// <param name="Device_Img">The device_ img.</param>
        /// <param name="Environment_Img">The environment_ img.</param>
        /// <param name="FinishRemark">The finish remark.</param>
        /// <returns>System.String.</returns>
        public static string UpEquipmentInfo(int SmID, int Y_TaskId, string Device_Status, int Change_Number, int Stop_Number, string Device_Img, string Environment_Img, string FinishRemark)
        {
            result_UpEquipmentInfo result_UpEquipmentInfo = new result_UpEquipmentInfo();
            result_UpEquipmentInfo.message = "失败";
            result_UpEquipmentInfo.result = false;
            int INSERT = 0;
            string sql = string.Empty;
            string NOW = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ImagePath_Equ = "";
            string ImagePath_Env = "";
            //处理设备图片
            if (!string.IsNullOrEmpty(Device_Img))
            {
                string[] arr = Device_Img.Split('&');
                int i = 0;
                foreach (string liu in arr)
                {
                    ImagePath_Equ += SaveImage(liu);
                    if (i != arr.Length - 1)
                    {
                        ImagePath_Equ += "|";
                    }
                    i++;
                }

            }
            //处理环境图片
            if (!string.IsNullOrEmpty(Environment_Img))
            {
                string[] arr = Environment_Img.Split('&');
                int i = 0;
                foreach (string liu in arr)
                {
                    ImagePath_Env += SaveImage(liu);
                    if (i != arr.Length - 1)
                    {
                        ImagePath_Env += "|";
                    }
                    i++;
                }

            }
            try
            {
                sql = string.Format(@" update Y_MainTainDetail set FinishRemark='{2}',Device_Status='{3}',Change_Number={4},Stop_Number={5},Device_Img='{6}',Environment_Img='{7}',FinishState=1,FinishDate='{8}'
                                       where Y_TaskId={0} and SmID={1};", Y_TaskId, SmID, FinishRemark, Device_Status, Change_Number, Stop_Number, ImagePath_Equ, ImagePath_Env, NOW);
                INSERT = APP.SQLServer_Helper.UpDate(sql);
                if (INSERT > 0)
                {
                    string strSqlSelect = string.Format(@" select COUNT(SmID) as UnFinishCount from Y_MainTainDetail where Y_TaskId = {0} and FinishState = 0 ", Y_TaskId);
                    DataTable dtSelect = new DataTable();
                    dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
                    if (dtSelect.Rows.Count > 0)
                    {
                        if (int.Parse(dtSelect.Rows[0]["UnFinishCount"].ToString()) <= 0)
                        {
                            string strSqlUpdate = string.Format(@"   update Y_MainTainTask set Y_FinishDate='{0}',Y_FinishState=1 where Y_TaskId={1} ", NOW, Y_TaskId);
                            int intResult = 0;
                            intResult = APP.SQLServer_Helper.UpDate(strSqlUpdate);
                            if (intResult > 0)
                            {
                                result_UpEquipmentInfo.result = true;
                                result_UpEquipmentInfo.message = "上传成功,任务已经完成!";
                            }
                            else {
                                //成功后
                                result_UpEquipmentInfo.result = false;
                                result_UpEquipmentInfo.message = "上传成功,但更新任务完成状态!";
                            }
                        }
                        else {
                            result_UpEquipmentInfo.result = true;
                            result_UpEquipmentInfo.message = "上传成功!";
                        }
                    }
                    else {
                        //成功后
                        result_UpEquipmentInfo.result = false;
                        result_UpEquipmentInfo.message = "上传成功,但查询未完成数量失败!";
                    }
                    
                }
                else
                {
                    //失败后
                    result_UpEquipmentInfo.result = false;
                    result_UpEquipmentInfo.message = "失败!";
                }
            }
            catch (Exception e)
            {
                //失败后
                result_UpEquipmentInfo.result = false;
                result_UpEquipmentInfo.message = e.ToString();
            }

            return JsonConvert.SerializeObject(result_UpEquipmentInfo);
        }
        /// <summary>
        /// Gets the equ main tain information list.
        /// </summary>
        /// <param name="Y_TaskId">The y_ task identifier.</param>
        /// <param name="SmID">The sm identifier.</param>
        /// <param name="EquType">Type of the equ.</param>
        /// <returns>System.String.</returns>
        public static string GetEquMainTainInfoList(int Y_TaskId, int SmID, string EquType)
        {
            result_EqMainTainList result_EqMainTainList = new result_EqMainTainList();
            List<EqMainTain> EqMainTainList = new List<EqMainTain>();
            result_EqMainTainList.message = "失败";
            result_EqMainTainList.result = false;
            string strWhere = string.Empty;
            if (Y_TaskId >0)
            {
                strWhere = string.Format(@"and ymd.Y_TaskId < {0}", Y_TaskId);
            }
            try
            {
                string strSqlSelect = string.Format(@" select top 10 ymd.Device_Img,ymd.Environment_Img,ymt.Y_ProraterName,ymt.Y_EndTime,ymt.Y_StarTime,ymt.Y_TaskName,ymt.Y_TaskId
                                                       from    Y_MainTainDetail  ymd      
                                                       left join  Y_MainTainTask ymt on ymd.Y_TaskId = ymt.Y_TaskId
                                                       where 1=1 and  ymd.SmID = {0}    and ymd.EquType like '{1}%' {2} and ymd.FinishState =1 and ymt.Y_DeleteState = 1 order by ymt.Y_TaskId desc ", SmID, EquType, strWhere);
                DataTable dtSelect = new DataTable();
                dtSelect = APP.SQLServer_Helper.SelectDataTable(strSqlSelect);
                if (dtSelect.Rows.Count > 0)
                {
                    result_EqMainTainList.message = "成功";
                    result_EqMainTainList.result = true;
                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        EqMainTain EqMainTain = new EqMainTain();
                        EqMainTain.Device_Img = dtSelect.Rows[i]["Device_Img"].ToString();//设备图片
                        EqMainTain.Environment_Img = dtSelect.Rows[i]["Environment_Img"].ToString();//环境图片
                        EqMainTain.Y_ProraterName = dtSelect.Rows[i]["Y_ProraterName"].ToString();//养护人员 
                        EqMainTain.Y_StarTime = dtSelect.Rows[i]["Y_StarTime"].ToString();//养护开始时间
                        EqMainTain.Y_EndTime = dtSelect.Rows[i]["Y_EndTime"].ToString();//养护结束时间
                        EqMainTain.Y_TaskName = dtSelect.Rows[i]["Y_TaskName"].ToString();//养护任务名称
                        EqMainTain.Y_TaskId = int.Parse(dtSelect.Rows[i]["Y_TaskId"].ToString());//养护任务ID
                        EqMainTainList.Add(EqMainTain);
                    }
                }
                else
                {
                    result_EqMainTainList.message = "未查询到数据!";
                    result_EqMainTainList.result = true;
                }
                result_EqMainTainList.data = EqMainTainList;
                return JsonConvert.SerializeObject(result_EqMainTainList);
            }
            catch (Exception ex)
            {
                result_EqMainTainList.message = ex.ToString();
                result_EqMainTainList.result = false;
                result_EqMainTainList.data = EqMainTainList;
                return JsonConvert.SerializeObject(result_EqMainTainList);
            }
        }
        #region 养护管理-养护任务内部类
        internal class result_Maintain
        {
            public Boolean result = false;
            public String message = "失败";
            public List<MaintainTask> data = new List<MaintainTask>();
        }
        internal class MaintainTask
        {
            //任务ID
            public int Y_TaskId;
            //任务名称
            public string Y_TaskName;
            //开始时间
            public string Y_StarTime;
            //结束时间
            public string Y_EndTime;
            //已养护数量
            public int FinishCount;
            //未完成数量
            public int UnFinishCount;
            //所在道路
            public string RoadName;
            //完成状态
            public int Y_FinishState;
        }
        #endregion
        #region 养护管理-养护任务明细内部类
        internal class result_MaintainTask
        {
            public Boolean result = false;
            public String message = "失败";
            public List<MaintainTaskDetail> data = new List<MaintainTaskDetail>();
        }
        internal class MaintainTaskDetail
        {
            //设备ID
            public int SmID;
            //经度
            public string SmX;
            //纬度
            public string SmY;
            //设备类别
            public string EquType;
            //完成状态
            public int FinishState;
        }
        #endregion
        #region 养护管理-设备上次回传信息内部类
        internal class result_LastEqUp
        {
            public Boolean result = false;
            public String message = "失败";
            public List<LastEqUpInfo> data = new List<LastEqUpInfo>();
        }
        internal class LastEqUpInfo
        {
            //更换次数
            public int Change_Number;
            //设备状态
            public string Device_Status;
            //关停次数
            public int Stop_Number;
        }
        #endregion
        #region 养护管理-设备信息上传结果内部类
        internal class result_UpEquipmentInfo
        {
            public Boolean result = false;
            public String message = "失败";
        }
        #endregion
        #region 养护管理-设备养护内容列表内部类
        internal class result_EqMainTainList
        {
            public Boolean result = false;
            public String message = "失败";
            public List<EqMainTain> data = new List<EqMainTain>();
        }
        internal class EqMainTain
        {
            //任务ID
            public int Y_TaskId;
            //任务名称
            public string Y_TaskName;
            //任务开始时间
            public string Y_StarTime;
            //任务结束时间
            public string Y_EndTime;
            //养护人员姓名
            public string Y_ProraterName;
            //设备图片
            public string Device_Img;
            //环境图片
            public string Environment_Img;
        }
        #endregion
        #endregion
    }
}