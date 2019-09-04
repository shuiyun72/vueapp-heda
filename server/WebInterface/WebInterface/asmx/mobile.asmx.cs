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

namespace WebInterface.asmx
{
    /// <summary>
    /// mobile 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class mobile : System.Web.Services.WebService
    {
        string err = "";

        #region -------------------------登录方法 ------------------------------
        /// <summary>
        /// User_Check 登录方法
        /// </summary>
        /// <param name="name">登录名</param>
        /// <param name="pwd">密码</param>
        /// <param name="smid">手机识别码</param>
        /// <returns></returns>
        [WebMethod]
        public string User_Check(string name, string pwd, string smid)
        {
            return Data_dal.User_Check_(name, pwd, smid);
        }
        #endregion

        #region ------------------  获取POI/道路中心线 -----------------------
        [WebMethod]
        public string Get_POI(string str)
        {
            return Data_dal.Get_POI(str);
        }
        [WebMethod]
        public string Get_Road(string str)
        {
            return Data_dal.Get_Road(str);
        }
        #endregion

        #region -----------------  隐患管理 ------------------------------
        [WebMethod]
        /// <summary>
        /// HiddenTroubleReport_Update 隐患上报方法
        /// </summary>
        ///        
        public string HiddenTroubleReport_Insert(int shebei_type, int yinhuan_type, string address, string miaoshu, string faxiantime, string uptime, string x, string y, int peopleid, string sheshi, string imagegeshi, string base64, int rwID, int pianID, int luxianID, int ishidden)
        {
            string result = "";
            try
            {

                if (Data_dal.Insert_Hidden(ref err, shebei_type, yinhuan_type, address, miaoshu, faxiantime, uptime, x, y, peopleid, sheshi, imagegeshi, base64, rwID, pianID, luxianID, ishidden) > 0)
                {
                    result = @"[{""result"":成功}]";
                }
                else
                {
                    result = @"[{""result"":失败}]";
                }

            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
            }
            return result;
        }
        //public string HiddenTroubleReport_Insert(int lhd_hiddenname, int lhd_hiddentype, string address, string miaoshu, string faxiantime, string uptime, string x, string y, int peopleid, string sheshi, string imagegeshi, string base64, int areaId, int pointId, int patrolId, int isHidden, string exception, string chuli)
        //{
        //    string result = "";6
        //    try
        //    {

        //        if (Data_dal.Insert_Hidden(ref err, lhd_hiddenname, lhd_hiddentype, address, miaoshu, faxiantime, uptime, x, y, peopleid, sheshi, imagegeshi, base64, areaId, pointId, patrolId, isHidden, exception, chuli) > 0)
        //        {
        //            result = @"[{""result"":成功}]";
        //        }
        //        else
        //        {
        //            result = @"[{""result"":失败}]";
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
        //    }
        //    return result;
        //}
        [WebMethod]
        /// <summary>
        /// Hiddentype 隐患类型下拉框赋值
        /// </summary>
        public string Get_HiddenDangerType()
        {
            return Data_dal.Get_HiddenDangerType();
        }
        [WebMethod]
        /// <summary>
        /// 隐患设施
        /// </summary>
        public string Get_HiddenDangerDevice()
        {
            return Data_dal.Get_HiddenDangerDevice();
        }
        #endregion

        #region  --------------   计划任务 ----------------------------
        [WebMethod]
        /// <summary>
        /// Down_Taskplan  计划任务下载
        /// </summary>
        public string Down_Taskplan(int id, string date)
        {
            return Data_dal.Get_Taskplan(id, date);
        }
        [WebMethod]
        /// <summary>
        /// Get_Taskplan_area  查询片区坐标
        /// </summary>

        public string Get_Taskplan_area(int id)
        {
            return Data_dal.Get_Taskplan_area(id);
        }
        [WebMethod]
        /// <summary>
        /// 计划完成状态
        /// </summary>

        public string Update_ZT(int id)
        {
            string result = "";
            if (Data_dal.Update_Set_ZT(id) > 0)
            {
                result = @"[{""result"":true}]";
            }
            else
            {
                result = @"[{""result"":false}]";
            }
            return result;
        }
        #endregion

        #region -----------------------    隐患历史事件查询 -----

        /// <summary>
        /// Get_Select_Hidden   查询上报事件
        /// </summary>
        [WebMethod]
        public string Get_Select_Hidden(string ksdate, string jsdate, int nameid)
        {
            return Data_dal.Get_Select_Hidden(ksdate, jsdate, nameid);
        }
        #endregion

        #region ------------------  位置  上报 ------------------------
        /// <summary>
        /// UPCoordinatePosition  位置实时上报
        /// </summary>
        [WebMethod]
        public string UPCoordinatePosition(string x, string y, string uptime, string name, string zaixian)
        {
            string result = "";
            if (Data_dal.UPCoordinatePosition(x, y, uptime, name, zaixian) > 0)
            {
                result = @"[{""result"":true}]";
            }
            else
            {
                result = @"[{""result"":false}]";
            }
            return result;
        }
        /// <summary>
        /// UPCoordinatePosition  位置实时上报 新曾gps状态和异常原因
        /// </summary>
        [WebMethod]
        public string UPCoordinatePositionNew(string x, string y, string uptime, string name, string zaixian, string dignweiStatus, string yichang)
        {
            string result = "";
            if (Data_dal.UPCoordinatePosition(x, y, uptime, name, zaixian, dignweiStatus, yichang) > 0)
            {
                result = @"[{""result"":true}]";
            }
            else
            {
                result = @"[{""result"":false}]";
            }
            return result;
        }
        #endregion

        #region  ------------------   打卡  -------------------------------

        /// <summary>
        /// 签到
        /// </summary>
        [WebMethod]
        public string QianDao(string Lwr_PersonId, string Lwr_Date, string Lwr_StartTime, string Lwr_EndTime, string Lwr_Hour, string Lwr_BeiZhu, string Lwr_PersonStatus, string Lwr_UpTime, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power)
        {
            string result = "";
            try
            {

                if (Data_dal.Insert_QianDao(ref err, Lwr_PersonId, Lwr_Date, Lwr_StartTime, Lwr_EndTime, Lwr_Hour, Lwr_BeiZhu, Lwr_PersonStatus, Lwr_UpTime, Lwr_GpsStatus, Lwr_MobileStatus, Lwr_Power) > 0)
                {
                    result = @"[{""result"":成功}]";
                }
                else
                {
                    result = @"[{""result"":失败}]";
                }

            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
            }
            return result;
        }
        #endregion

        #region ------------------  考勤记录 -----------------------
        [WebMethod]
        /// <summary>
        /// id:用户ID,dateStr 月份如2016-12 返回一个月的数据
        /// </summary>
        public string Get_WorkRecord(int id, string dateStr)
        {
            return Data_dal.Get_WorkRecord(id, dateStr);
        }
        #endregion

        #region ------------------  获取所有公告 -----------------------
        [WebMethod]
        public string Get_Notice()
        {
            return Data_dal.Get_Notice();
        }
        #endregion

        #region ----------------------其他方法 ------------------------------
        /// <summary>
        /// 经纬度转换为大地坐标
        /// </summary>
        /// <param name="B">纬度，单位：度</param>
        /// <param name="L">经度差(经度减去中央经度)，单位：度</param>
        /// <param name="N">北方向坐标，单位：米</param>
        /// <param name="E">东方向坐标，单位：米</param>
        [WebMethod]
        public string GaussBL2NE(string B, string L)
        {
            double N, E = 0;
            string result = "";
            //int du=0;
            //double fen, miao = 0;
            //GaussCalc.DEG2DMS(113.497373,out du,out fen,out miao);
            //double ss = GaussCalc.DMS2DEG(du, fen, miao);

            GaussCalc.GetModulus();
            GaussCalc.GaussBL2NE(Convert.ToDouble(B), (Convert.ToDouble(L) - 114), out N, out E);
            result = "[{\"N\":\"" + N + "\",\"E\":\"" + E + "\"}]";
            return result;
        }
        [WebMethod]
        /// <summary>
        ///巡查项目主分类
        /// </summary>
        public string Get_Line_Item()
        {
            return Data_dal.Get_Line_Item();
        }
        [WebMethod]
        /// <summary>
        ///巡查项目所有分类
        /// </summary>
        public string Get_Line_Item_All()
        {
            return Data_dal.Get_Line_Item_All();
        }
        [WebMethod]
        /// <summary>
        ///巡查项目子分类
        /// </summary>
        public string Get_Line_ChildItem(int parentId)
        {
            return Data_dal.Get_Line_ChildItem(parentId);
        }

        [WebMethod]
        /// <summary>
        ///设备类型下拉框G_Repair_type
        /// </summary>
        public string Get_G_Repair_type()
        {
            return Data_dal.Get_G_Repair_type();
        }
        [WebMethod]
        /// <summary>
        ///获取服务器时间
        /// </summary>
        public string Get_ServerTime()
        {
            return Data_dal.Get_ServerTime();
        }
        #endregion
        [WebMethod]
        /// <summary>
        ///上传表井图片
        /// </summary>
        public string InsertIntoWellImage(string base64Photo, int id, string TableName, string desc)
        {
            string result = "";
            try
            {
                if (base64Photo != "")//有图片上传图片后更新描述
                {
                    if (Data_dal.ReceiveWellImage(base64Photo, id, TableName) > 0)
                    {
                        //更新设施描述
                        Data_dal.UpdateDescription(TableName, id, desc);
                        result = @"[{""result"":成功}]";
                    }
                    else
                    {
                        result = @"[{""result"":失败}]";
                    }
                }
                else//无图片直接更新描述
                {
                    //更新设施描述
                    int i = Data_dal.UpdateDescription(TableName, id, desc);
                    if (i > 0)
                    {
                        result = @"[{""result"":成功}]";
                    }
                    else
                    {
                        result = @"[{""result"":失败}]";
                    }

                }
            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
            }
            return result;
        }
        /// <summary>
        /// 删除表井
        /// </summary>
        /// <param name="TableName">图层名称</param>
        /// <param name="id">设备ID</param>
        /// <param name="ImageName">删除图片集合,|分割</param>
        /// <returns></returns>
        [WebMethod]
        public string ImageRemove(string TableName, int id, string ImageName)
        {
            string result = "";
            try
            {

                if (Data_dal.ImageRemove(TableName, id, ImageName) > 0)
                {
                    result = @"[{""result"":""成功""}]";
                }
                else
                {
                    result = @"[{""result"":""失败""}]";
                }

            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
            }
            return result;
        }
        /// <summary>
        /// 按照水表井名称下的Smid进行查询图片
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="id"></param>
        /// <param name="ImageName"></param>
        /// <returns></returns>
        [WebMethod]
        public string ImageSelect(string TableName, int id)
        {
            string result = "";
            try
            {
                string ImageSelect = Data_dal.ImageSelect(TableName, id);
                string desc = Data_dal.GetDescription(TableName, id);
                if (!string.IsNullOrEmpty(ImageSelect))
                {
                    ImageSelect = "\"" + ImageSelect + "\"";
                    TableName = "\"" + TableName + "\"";
                    result = @"[{""image"":" + ImageSelect + ",\"TableName\":" + TableName + ",\"id\":" + id + ",\"desc\":'" + desc + "'}]";
                }
                else
                {
                    result = @"[{}]";
                }
            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert(" + err + ")</script>");
            }
            return result;
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetPatroList_Dept()
        {
            string result = "";
            try
            {
                result = Data_dal.GetPatroList_Dept();
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 传入用户id进行返回该人员据有查看人员列表
        /// </summary>
        /// <param name="iadminid"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetPersonList_ByAminid(string iadminid, string deptid)
        {
            string result = "";
            try
            {
                result = Data_dal.GetPatroPersonList_ByAminid(iadminid, deptid);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 根据巡检人员id获取该人员的巡检轨迹
        /// </summary>
        /// <param name="iadmin"></param>
        /// <returns></returns>
        [WebMethod]
        public string getPatroPersonListByAminid(string iadmin, string starttime, string endtime)
        {
            string result = "";
            try
            {
                result = Data_dal.getPatroPersonListByAminid(iadmin, starttime, endtime);
            }
            catch
            {
            }
            return result;
        }
        [WebMethod]
        public string UpdatePassword(string iadminid, string oldpassword, string newpassword)
        {
            string result = "";
            try
            {
                result = Data_dal.UpdatePassword(iadminid, oldpassword, newpassword);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 绑定追踪器
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="trackerid"></param>
        /// <returns></returns>
        [WebMethod]
        public string BindTracker(int userid, string trackerid)
        {
            string result = "";
            try
            {
                result = Data_dal.BindTracker(userid, trackerid);
            }
            catch
            {
            }
            return result;
        }

        #region NFC电子标签巡检
        /// <summary>
        /// 同步标签信息
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string LoadLabels()
        {
            string result = "";
            try
            {
                result = Data_dal.LoadLabels();
            }
            catch
            {
            }
            return result;
            //return null;
        }
        /// <summary>
        /// 标签绑定
        /// </summary>
        /// <param name="labelId">电子标签的id</param>
        /// <param name="equipmentName">电子标签绑定的设备的名称</param>
        /// <param name="equipmentSmid">电子标签绑定的设备的smid号</param>
        /// <param name="coordinateX">电子标签绑定的设备的坐标x</param>
        /// <param name="coordinateY">电子标签绑定的设备的坐标y</param>
        /// <param name="adminId">绑定操作发起者的id</param>
        /// <returns></returns>
        [WebMethod]
        public string bindLabelId(string labelId, string equipmentName, int equipmentSmid, float coordinateX, float coordinateY, int adminId)
        {
            string result = "";
            try
            {
                result = Data_dal.bindLabelId(labelId, equipmentName, equipmentSmid, coordinateX, coordinateY, adminId);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 关联该井内的设备
        /// </summary>
        /// <param name="layerName">井所在图层的名称</param>
        /// <param name="smid">井的smid号</param>
        /// <returns></returns>
        [WebMethod]
        public string equipmentInWell(string layerName, int smid)
        {
            string result = "";
            try
            {
                result = Data_dal.equipmentInWell(layerName, smid);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="labelId">电子标签的id</param>
        /// <param name="adminId">解除绑定操作的发起者的id</param>
        /// <returns></returns>
        [WebMethod]
        public string unbindLabelId(string labelId, int adminId)
        {
            string result = "";
            try
            {
                result = Data_dal.unbindLabelId(labelId, adminId);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 提交巡检报告
        /// </summary>
        /// <param name="labelId">电子标签的id</param>
        /// <param name="adminId">提交巡检报告操作的发起者的id</param>
        /// <param name="equipmentName">电子标签绑定的设备的名称</param>
        /// <param name="equipmentSmid">电子标签绑定的设备的smid号</param>
        /// <param name="reportContent">巡检报告的文本内容</param>
        /// <param name="editDateTime">编辑巡检报告的日期（格式：2017-07-07 07:07:07）</param>
        /// <returns></returns>
        [WebMethod]
        public string upNFCReport(string labelId, int adminId, string equipmentName, int equipmentSmid, string reportContent, string editDateTime)
        {
            string result = "";
            try
            {
                result = Data_dal.upNFCReport(labelId, adminId, equipmentName, equipmentSmid, reportContent, editDateTime);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 获取报表
        /// </summary>
        /// <param name="adminId">提交巡检报告操作的发起者的id</param>
        /// <param name="startDate">查询报表的开始时间(格式：2017-07-07)</param>
        /// <param name="endDate">查询报表的截止时间(格式：2017-07-07)</param>
        /// <param name="formType">报表的类型。0：设备类型。1：口径。2：所在道路</param>
        /// <returns></returns>
        [WebMethod]
        public string patrolReportForm(int adminId, string startDate, string endDate, int formType)
        {
            string result = "";
            try
            {
                result = Data_dal.patrolReportForm_FromNFCReport(adminId, startDate, endDate, formType);
            }
            catch
            {
            }
            return result;
        }
        /// <summary>
        /// 报表详情
        /// </summary>
        /// <param name="adminId">提交巡检报告操作的发起者的id</param>
        /// <param name="startDate">查询报表的开始时间(格式：2017-07-07)</param>
        /// <param name="endDate">查询报表的截止时间(格式：2017-07-07)</param>
        /// <param name="formType">报表的类型。0：设备类型。1：口径。2：所在道路</param>
        /// <param name="name">第三章中，查询报表返回的字段。标识：统计数据的区别名称。</param>
        /// <param name="pageNumber">分页查询的页码（页码从1开始，每页60条数据）</param>
        /// <returns></returns>
        [WebMethod]
        public string reportFormDetails(int adminId, string startDate, string endDate, int formType, string name, int pageNumber)
        {
            string result = "";
            try
            {
                result = Data_dal.reportFormDetails(adminId, startDate, endDate, formType, name, pageNumber);
            }
            catch
            {
            }
            return result;
        }
        #endregion
    }
}
