using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using WebInterface.dal;
using System;

namespace WebInterface.asmx
{
    /// <summary>
    /// Maintain 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Mantain : System.Web.Services.WebService
    {
        /// <summary>
        /// 养护系统登录验证
        /// </summary>
        /// <param name="PersonName">用户名</param>
        /// <param name="PassWord">密码</param>
        /// <returns>{“result”:true，”message “:”成功！”，”PersonId “:”00001”，” Phone “:”13245896745”，” PersonName “:”张三”}</returns>
        [WebMethod]
        public string Login(string PersonName, string PassWord)
        {
            return Data_Mantain_Dal.User_CheckLogin(PersonName, PassWord);
        }
        /// <summary>
        /// 获取工单列表
        /// </summary>
        /// <param name="PersonId">用户编号</param>
        /// <returns>{“result”:true，”message “:”成功！”，”data”:[{“OrderTime”:”标题”, “PreEndTime”:”公告时间”,” EventCode”:”公告内容” , “EventType”:”标题”, “EventFrom”:”公告时间”,” EventContent”:”公告内容” , “EventAddress”:”标题”, “EventDesc”:”公告时间”,” UrgencyId”:”公告内容” ,“HandlerLevelId”:”标题”, “UpTime”:”公告时间”,” DispatchPerson”:”公告内容” ,” EventState”:”公告内容” ,“EventPictures”:”标题”, “EventX”:”公告时间”,” EventY”:”公告内容” }]}</returns>
        [WebMethod]
        public string getOrderList(string PersonId, string Type)
        {
            return Data_Mantain_Dal.getOrderList(PersonId, Type);
        }
        /// <summary>
        /// 获取事件列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetEvent()
        {
            return Data_Mantain_Dal.GetEvent();
        }

        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetDep()
        {
            return Data_Mantain_Dal.GetDep();
        }

        /// <summary>
        /// 获取部门中的人员
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetDepPerson(int DeptId)
        {
            return Data_Mantain_Dal.GetDepPerson(DeptId);
        }

        /// <summary>
        /// 事件分派提交
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string SaveWorkOrder(int DepId, int PersonId, string OrderTime, string PreEndTime, int EventId, string UserName)
        {
            return Data_Mantain_Dal.SaveWorkOrder(DepId, PersonId, OrderTime, PreEndTime, EventId, UserName);
        }
        /// <summary>
        /// 事件无效
        /// </summary>
        /// <param name="EventID"></param>
        /// <returns></returns>
        [WebMethod]
        public string EventInvalid(int EventID)
        {
            return Data_Mantain_Dal.EventInvalid(EventID);
        }

        /// <summary>
        /// 提交退单
        /// </summary>
        /// <param name="OrderCode">工单编号</param>
        /// <param name="PersonId">提交人编号</param>
        /// <param name="describe">退单描述</param>
        /// <returns>{“result”:true，”message “:”成功！”}</returns>
        [WebMethod]
        public string CommitChargeBack(string OrderId, string PersonId, string describe)
        {
            return Data_Mantain_Dal.CommitChargeBack(OrderId, PersonId, describe);
        }
        /// <summary>
        /// 养护系统提交延期
        /// </summary>
        /// <param name="OrderId">工单编号</param>
        /// <param name="PersonId">提交人编号</param>
        /// <param name="describe">延期原因</param>
        /// <param name="complishTime">预计完成时间</param>
        /// <returns>结果格式：{“result”:true，”message “:”成功！”}</returns>
        [WebMethod]
        public string CommitDelay(int OrderId, string describe, string complishTime)
        {

            return Data_Mantain_Dal.CommitDelay(OrderId, describe, complishTime);

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
        [WebMethod]
        public string CommitOrderTask(int OrderId, int PersonId, string describe, string EventPictures, string EventVoices, int TaskType)
        {

            return Data_Mantain_Dal.CommitOrderTask(OrderId, PersonId, describe, EventPictures, EventVoices, TaskType);
        }
        /// <summary>
        /// 获取养护管理任务列表数据
        /// </summary>
        /// <param name="PersonId">The person identifier.</param>
        /// <returns>System.String.</returns>
        [WebMethod]
        public string GetMainTainTask(int PersonId)
        {
            return Data_Mantain_Dal.GetMainTainTask(PersonId);
        }
        /// <summary>
        /// 获取养护管理任务明细
        /// </summary>
        /// <param name="TaskID">The task identifier.</param>
        /// <returns>System.String.</returns>
        [WebMethod]
        public string GetMainTainTaskDetail(int TaskID)
        {
            return Data_Mantain_Dal.GetMainTainTaskDetail(TaskID);
        }
        /// <summary>
        /// 获取上次上传设备的信息
        /// </summary>
        /// <param name="SmID">The sm identifier.</param>
        /// <param name="EquType">Type of the equ.</param>
        /// <returns>System.String.</returns>
        [WebMethod]
        public string GetLastEqUpInfo(int SmID, string EquType)
        {
            return Data_Mantain_Dal.GetLastEqUpInfo(SmID, EquType);
        }
        /// <summary>
        /// 养护设备养护信息上传
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
        [WebMethod]
        public string UpEquipmentInfo(int SmID, int Y_TaskId, string Device_Status, int Change_Number, int Stop_Number, string Device_Img, string Environment_Img, string FinishRemark)
        {
            return Data_Mantain_Dal.UpEquipmentInfo(SmID, Y_TaskId, Device_Status, Change_Number, Stop_Number, Device_Img, Environment_Img, FinishRemark);
        }
        /// <summary>
        /// 获取设备养护历史列表
        /// </summary>
        /// <param name="Y_TaskId">The y_ task identifier.</param>
        /// <param name="SmID">The sm identifier.</param>
        /// <param name="EquType">Type of the equ.</param>
        /// <returns>System.String.</returns>
        [WebMethod]
        public string GetEquMainTainInfoList(int Y_TaskId, int SmID, string EquType)
        {
            return Data_Mantain_Dal.GetEquMainTainInfoList(Y_TaskId, SmID, EquType);
        }
    }
}
