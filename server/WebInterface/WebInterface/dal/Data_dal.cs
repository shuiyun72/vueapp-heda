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


public static class Data_dal
{
    #region  --------------  登录方法  --------------
    public static string User_Check_(string name, string pwd, string smid)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        DataTable dt1 = null;
        try
        {
            string sql = "select iadminid, cadminname,cadminpassword,cdepname,crolename,smid,isEdit,upMeter from V_P_Admin where cadminname='" + name + "' and cadminpassword='" + pwd + "'";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                dt1 = new DataTable("user");
                dt1.Columns.Add("ID", typeof(string));
                dt1.Columns.Add("用户名", typeof(string));
                dt1.Columns.Add("密码", typeof(string));
                dt1.Columns.Add("部门", typeof(string));
                dt1.Columns.Add("类型", typeof(string));
                dt1.Columns.Add("识别码", typeof(string));
                dt1.Columns.Add("isEdit", typeof(int));
                dt1.Columns.Add("upMeter", typeof(int));
                DataRow newRow;
                newRow = dt1.NewRow();
                newRow["ID"] = dt.Rows[0]["iadminid"].ToString();
                newRow["用户名"] = dt.Rows[0]["cadminname"].ToString();
                newRow["密码"] = dt.Rows[0]["cadminpassword"].ToString();
                newRow["部门"] = dt.Rows[0]["cdepname"].ToString();
                newRow["类型"] = dt.Rows[0]["crolename"].ToString();
                newRow["识别码"] = dt.Rows[0]["smid"].ToString();
                newRow["isEdit"] = dt.Rows[0]["isEdit"];
                newRow["upMeter"] = dt.Rows[0]["upMeter"];
                dt1.Rows.Add(newRow);
                //将新的smid存入数据库
                if (smid != "" && smid != null)
                {
                    sql = string.Format("update P_Admin set smid='{0}' where iadminid={1}", smid, dt.Rows[0]["iadminid"].ToString());
                    //int i = db.OperateDB(sql);
                    int i = APP.SQLServer_Helper.UpDate(sql);
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

            // string d = JsonTo.ToJson(dt);
        }
        catch (Exception)
        {

        }
        return JsonTo.ToJson(dt1);
    }
    #endregion
    #region --------------POI道路中心线查询--------------
    /// <summary>
    /// POI查询
    /// </summary>
    /// <param name="str">输入的关键字</param>
    /// <returns></returns>
    public static string Get_POI(string str)
    {
        DataTable dt = null;
        try
        {
            string sql = "SELECT gid 编号,名称 from leakage.poi where 名称 like '%" + str + "%'";
            dt = APP.PGSQL_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return d;
    }
    /// <summary>
    /// 获取道路中心线
    /// </summary>
    public static string Get_Road(string str)
    {
        DataTable dt = null;
        try
        {
            string sql = "select distinct 道路名称 名称 from leakage.dlzxx where 道路名称 is not null and 道路名称 like '%" + str + "%'";
            dt = APP.PGSQL_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return d;
    }
    #endregion
    #region ----------------  隐患管理 ---------------
    public static int Insert_Hidden(ref string errorMessage, int shebei_type, int yinhuan_type, string address, string miaoshu, string faxiantime, string uptime, string x, string y, int peopleid, string sheshi, string imagegeshi, string base64, int rwID, int pianID, int luxianID, int ishidden)
    {
        UploadPicture(imagegeshi, base64, peopleid);
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        int num = 0;
        string xy = x + "|" + y;
        string sql = "insert into Line_HiddenDanger(lhd_hiddenname,lhd_hiddentype,lhd_address,lhd_miaoshu,lhd_uptime,lhd_faxiantime,lhd_xy,lhd_upname,lhd_sheshiname,lhd_hiddenimage,lp_id,lpa_id,lpa_ids,lhd_ishidden) values(" +
                     " '" + shebei_type + "'," +
                     "" + yinhuan_type + "," +
                     "'" + address + "'," +
                     "'" + miaoshu + "'," +
                      "'" + uptime + "'," +
                      "'" + faxiantime + "'," +
                     "'" + xy + "'," +
                     "" + peopleid + ",'" + sheshi + "','" + physicalDir + "'," + rwID + "," + pianID + "," + luxianID + "," + ishidden + ") SELECT @@IDENTITY AS Id";
        try
        {
            //SqlCommand smd = db.OperateDB_id(sql);
            //SqlDataReader R = smd.ExecuteReader();
            //if (R.Read())
            //{
            //    return Convert.ToInt16(R.GetValue(0));
            //}
            num = APP.SQLServer_Helper.Insert(sql);
        }
        catch (Exception ex)
        {
            errorMessage = ex.ToString();
        }

        return num;
    }
    //public static int Insert_Hidden(ref string errorMessage, int lhd_hiddenname, int lhd_hiddentype, string address, string miaoshu, string faxiantime, string uptime, string x, string y, int peopleid, string sheshi, string imagegeshi, string base64, int areaId, int pointId, int patrolId, int isHidden, string exception, string chuli)
    //{
    //    //string image = UploadPicture(imagegeshi, base64, peopleid);
    //    UploadPicture(imagegeshi, base64, peopleid);
    //    //DataBaseClass db = new DataBaseClass(dbType.sql);
    //    int num = 0;
    //    string xy = x + "|" + y;
    //    //string sql = "insert into Line_HiddenDanger(lhd_hiddenname,lhd_hiddentype,lhd_address,lhd_miaoshu,lhd_uptime,lhd_faxiantime,lhd_xy,lhd_upname,lhd_sheshiname,lhd_hiddenimage) values(" +
    //    //             " '" + shebei_type + "',"+
    //    //             "" + yinhuan_type + ","+
    //    //             "'" + address + "',"+
    //    //             "'" + miaoshu + "',"+
    //    //              "'" + uptime + "'," +
    //    //              "'" + faxiantime + "'," +
    //    //             "'" + xy + "',"+
    //    //             "" + peopleid + ",'" + sheshi + "','" + physicalDir + "') SELECT @@IDENTITY AS Id";
    //    //string sql = string.Format("insert into Line_HiddenDanger(lhd_address,lhd_miaoshu,lhd_uptime,lhd_faxiantime,lhd_xy,lhd_upname,lhd_sheshiname,lhd_hiddenimage,areaId,pointId,patrolId,isHidden,lhd_Exception,lhd_ChuLi,lhd_hiddenname,lhd_hiddentype) values('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}',{8},{9},{10},{11},'{12}','{13}',{14},{15}) SELECT @@IDENTITY AS Id",
    //    //  address, miaoshu, uptime, faxiantime, xy, peopleid, sheshi, physicalDir, areaId, pointId, patrolId, isHidden, exception, chuli, lhd_hiddenname, lhd_hiddentype);
    //    try
    //    {
    //        //SqlCommand smd = db.OperateDB_id(sql);
    //        //SqlDataReader R = smd.ExecuteReader();
    //        //if (R.Read())
    //        //{
    //        //    return Convert.ToInt16(R.GetValue(0));
    //        //}
    //        num = APP.SQLServer_Helper.Insert(sql);
    //    }
    //    catch (Exception ex)
    //    {
    //        errorMessage = ex.ToString();
    //    }

    //    return num;
    //}
    static string physicalDir = "";
    public static void UploadPicture(string suffix, string base64Photo, int id)
    {
        physicalDir = "";
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
            // string guid = Guid.NewGuid().ToString();
            //string relativePath = string.Format("{0}/{1}{2}", relativeDir, id, suffix);
            string imageName = string.Format("{0:yyyyMMddHHmmssffff}", currentDay);
            string relativePath = string.Format("{0}/{1}{2}", relativeDir, imageName, geshi);
            //绝对物理目录：作用是需要判断该绝对物理目录是否存在
            string nowPhysicalDir = System.Web.HttpContext.Current.Server.MapPath(relativePath);//当前图片路径

            physicalDir += System.Web.HttpContext.Current.Server.MapPath(relativePath) + "|";//以|分割组合的所有图片路径


            //在保存之前进行判断
            //if (!Directory.Exists(physicalDir))
            //{
            //    Directory.CreateDirectory(physicalDir);
            //}
            //绝对物理路径：作用是存储文件时需要绝对物理路径
            //   string physicalFullPath = string.Format("{0}\\{1}{2}", physicalDir, id, suffix);
            UploadReturn imageReturn = new UploadReturn();
            //传过来的Base64Photo要进行URL编码
            //if (Base64StringToImage(base64Photo, physicalDir))
            if (Base64StringToImage(base64Image, nowPhysicalDir))
            {
                imageReturn.IsSuccess = true;
                string imagePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + relativePath;
                //返回的URL要进行URL编码，接收端要进行URL解码
                // imageReturn.Path = HttpUtility.UrlEncode(imagePath, System.Text.Encoding.UTF8);
                imageReturn.Path = imagePath;
            }

        }
        if (physicalDir != "")
        {
            physicalDir = physicalDir.Substring(0, physicalDir.Length - 1);
        }
        //return JSON.stringify(imageReturn);
    }
    //Base64字符串转换为图像并保存
    private static bool Base64StringToImage(string inputStr, string fullPath)
    {
        try
        {
            string path = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // string ww = Convert.ToBase64String(System.IO.File.ReadAllBytes("C://11.jpg"));
            //  byte[] arr = Convert.FromBase64String(ww);
            //var outputStr = HttpUtility.UrlDecode(inputStr, System.Text.Encoding.UTF8); 原先
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
    /// Hiddentype 隐患类型下拉框赋值
    /// </summary>
    public static string Get_HiddenDangerType()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = "select lhdt_id as 隐患类型编号,lhdt_typename as 隐患类型名称 from Line_HiddenDangerType";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    /// <summary>
    /// 隐患设施
    /// </summary>
    public static string Get_HiddenDangerDevice()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = "select * from Line_HiddenDangerDevice";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    #endregion
    #region --------------------  计划任务 -----------------------
    public static string Get_Taskplan(int id, string date)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            //string sql = "SELECT dbo.Line_Patrol.lp_id,dbo.Line_Patrol.lp_fanwei, dbo.Line_Patrol.lp_anpaidate, dbo.Line_Patrol.lp_jieshudate, dbo.Line_Patrol.lp_beizhu, dbo.Line_Patrol.lp_kaishidate, dbo.P_Admin.cAdminName,dbo.Line_Patrol.lp_name,i.iItemID,i.cItemName" +
            //              " FROM  dbo.P_Admin INNER JOIN dbo.Line_Patrol ON dbo.P_Admin.iAdminID = dbo.Line_Patrol.lp_name INNER JOIN " +
            //              "dbo.Line_Cycle ON dbo.Line_Patrol.lp_lc_id = dbo.Line_Cycle.lc_id" +
            //              " left join Line_Item i " +
            //              " on dbo.Line_Patrol.lp_itemId=i.iItemID " +
            //              " where dbo.Line_Patrol.lp_anpaidate <= '" + date + "' AND dbo.P_Admin.iadminid = " + id + " and dbo.Line_Patrol.lp_shenhe='已审核'";
            string sql = "SELECT dbo.Line_Patrol.lp_id,dbo.Line_Patrol.lp_fanwei, dbo.Line_Patrol.lp_anpaidate, dbo.Line_Patrol.lp_jieshudate, dbo.Line_Patrol.lp_beizhu, dbo.Line_Patrol.lp_kaishidate, dbo.P_Admin.cAdminName,dbo.Line_Patrol.lp_name" +
              " FROM  dbo.P_Admin INNER JOIN dbo.Line_Patrol ON dbo.P_Admin.iAdminID = dbo.Line_Patrol.lp_name INNER JOIN " +
              "dbo.Line_Cycle ON dbo.Line_Patrol.lp_lc_id = dbo.Line_Cycle.lc_id" +
              " where dbo.Line_Patrol.lp_anpaidate <= '" + date + "' AND dbo.P_Admin.iadminid = " + id + " and dbo.Line_Patrol.lp_shenhe='已审核'";
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    public static string Get_Taskplan_area(int a_id)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        //DataBaseClass db1 = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        DataTable dt1 = null;
        DataTable dt_1 = null;
        try
        {
            string sql = "select lpa_id,lpa_xy,lpa_jihuaname from Line_PatrolArea where lpa_typeid=" + a_id + "";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string sql1 = "select lpa_jihuaname,lpa_time from Line_PatrolArea where lpa_id=" + a_id + "";
                //dt_1 = db1.getTable(sql1);
                dt_1 = APP.SQLServer_Helper.SelectDataTable(sql1);

                dt1 = new DataTable("user");
                dt1.Columns.Add("lpa_id", typeof(string));
                dt1.Columns.Add("lpa_xy", typeof(string));
                dt1.Columns.Add("lpa_jihuaname", typeof(string));
                dt1.Columns.Add("lpa_jihuaname1", typeof(string));
                dt1.Columns.Add("lpa_time", typeof(string));//巡检片区的时间， 巡检片区下的巡检点添加修改删除操作需要更新此片区的时间
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow newRow;
                    newRow = dt1.NewRow();
                    newRow["lpa_id"] = dt.Rows[i]["lpa_id"].ToString();
                    newRow["lpa_xy"] = dt.Rows[i]["lpa_xy"].ToString();
                    newRow["lpa_jihuaname"] = dt.Rows[i]["lpa_jihuaname"].ToString();

                    newRow["lpa_jihuaname1"] = dt_1.Rows[0]["lpa_jihuaname"].ToString();
                    newRow["lpa_time"] = dt_1.Rows[0]["lpa_time"].ToString();
                    dt1.Rows.Add(newRow);
                }
            }
        }
        catch (Exception)
        {

        }
        string d = JsonTo.ToJson(dt1);
        return JsonTo.ToJson(dt1);
    }
    public static int Update_Set_ZT(int id)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        int num = 0;
        try
        {
            string sql = "update Line_Patrol set lp_ty='已完成' where lp_id=" + id + "";
            if (APP.SQLServer_Helper.UpDate(sql) > 0)
            {
                num = 1;
            }
        }
        catch (Exception)
        {
        }
        return num;
    }
    #endregion
    #region  --------------  其他方法  -------------------------
    /// <summary>
    /// 设备类型
    /// </summary>
    public static string Get_G_Repair_type()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = "select r_type_id as 设备类型编号,r_type_name as 设备类型名称 from G_Repair_type";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    /// <summary>
    /// 获取服务器时间
    /// </summary>
    public static string Get_ServerTime()
    {
        DateTime dt = DateTime.Now;
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
    /// <summary>
    /// 巡查项目主分类
    /// </summary>
    public static string Get_Line_Item()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = "select iItemID 分类编号,cItemName 分类名称 from Line_Item where iItemParentID =0 order by cItemMenuOrder";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    /// <summary>
    /// 巡查项目所有分类
    /// </summary>
    public static string Get_Line_Item_All()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = "select iItemID,cItemName,iItemParentID,cItemMenuOrder from Line_Item order by iItemID";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    /// <summary>
    /// 巡查项目子分类
    /// </summary>
    public static string Get_Line_ChildItem(int parentId)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = string.Format("select iItemID 分类编号,cItemName 分类名称 from Line_Item where iItemParentID ={0} order by iItemID", parentId);
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    #endregion
    #region ----------------------------- 隐患历史事件查询 -----------------------------
    /// <summary>
    /// Get_Select_Hidden   查询上报事件
    /// </summary>
    public static string Get_Select_Hidden(string ksdate, string jsdate, int id)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            //string sql = "  select case hdh.lhdh_id when hdh.lhdh_id  then '1' else '0' end handleStatus,hd.lhd_id,hd.lhd_uptime,hd.lhd_address,hd.lhd_faxiantime," +
            //            "  hd.lhd_hiddenimage,hd.lhd_xy,hd.lhd_sheshiname," +
            //            "a.cAdminName,pa.lpa_jihuaname xunjianpian,pa1.lpa_jihuaname xunjiandian," +
            //            "  hd.patrolId,hd.lhd_Exception,hd.lhd_ChuLi,hd.lhd_miaoshu,li.cItemName bigName,li1.cItemName smallName" +
            //            "  from Line_HiddenDanger hd  left join Line_HiddenDangerHandle hdh on hd.lhd_id=hdh.lhd_id left join P_Admin a" +
            //            "  on hd.lhd_upname=a.iAdminID " +
            //            "  left join Line_PatrolArea pa " +
            //            "  on hd.areaId=pa.lpa_id " +
            //            "  left join Line_PatrolArea pa1" +
            //            "  on hd.pointId=pa1.lpa_id" +
            //            " left join Line_Item li on hd.lhd_hiddenname=li.iItemID " +
            //            " left join Line_Item li1 on hd.lhd_hiddentype=li1.iItemID " +
            //            "  where hd.isHidden=1 and hd.lhd_uptime>='" + ksdate + "' and hd.lhd_uptime<='" + jsdate + "' and hd.lhd_upname=" + id + "";
            string sql = "SELECT dbo.Line_HiddenDanger.lhd_ishidden,dbo.Line_HiddenDangerType.lhdt_typename,dbo.Line_HiddenDanger.lhd_hiddenimage,dbo.Line_HiddenDangerDevice.lhdd_name, dbo.Line_HiddenDanger.lhd_id, dbo.Line_HiddenDanger.lhd_uptime, dbo.Line_HiddenDanger.lhd_hiddenname," +
                          "dbo.Line_HiddenDanger.lhd_hiddentype, dbo.Line_HiddenDanger.lhd_address, dbo.Line_HiddenDanger.lhd_miaoshu, dbo.Line_HiddenDanger.lhd_faxiantime," +
                          "dbo.Line_HiddenDanger.lhd_hiddenimage, dbo.Line_HiddenDanger.lhd_xy, dbo.Line_HiddenDanger.lhd_smid, dbo.Line_HiddenDanger.lhd_upname" +
                           " FROM dbo.Line_HiddenDanger INNER JOIN " +
                         "dbo.Line_HiddenDangerType ON dbo.Line_HiddenDanger.lhd_hiddentype = dbo.Line_HiddenDangerType.lhdt_id" +
                          " INNER JOIN dbo.Line_HiddenDangerDevice ON dbo.Line_HiddenDanger.lhd_hiddenname = dbo.Line_HiddenDangerDevice.lhdd_id" +
                          " where lhd_uptime>='" + ksdate + "' and lhd_uptime<='" + jsdate + "' and lhd_upname=" + id + "";
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return JsonTo.ToJson(dt);
    }
    #endregion
    #region  ---------------------  位置上报  ---------------------
    /// <summary>
    /// UPCoordinatePosition  位置实时上报
    /// </summary>
    public static int UPCoordinatePosition(string x, string y, string uptime, string name, string zaixian)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        int num = 0;
        try
        {
            string xy = x + "|" + y;
            string sql = "insert into Line_CoordinatePosition(lcp_xy,lcp_uptime,lcp_name,Lcp_zaixiantype) values('" + xy + "','" + uptime + "','" + name + "','" + zaixian + "')";
            if (APP.SQLServer_Helper.Insert(sql) > 0)
            {
                num = 1;
            }
        }
        catch (Exception)
        {
        }
        return num;
    }
    /// <summary>
    /// UPCoordinatePosition  位置实时上报
    /// </summary>
    public static int UPCoordinatePosition(string x, string y, string uptime, string name, string zaixian, string dignweiStatus, string yichang)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        int num = 0;
        try
        {
            string xy = x + "|" + y;
            string sql = "insert into Line_CoordinatePosition(lcp_xy,lcp_uptime,lcp_name,Lcp_zaixiantype,dingweiStatus,yichang) values('" + xy + "','" + uptime + "','" + name + "','" + zaixian + "','" + dignweiStatus + "','" + yichang + "')";
            if (APP.SQLServer_Helper.Insert(sql) > 0)
            {
                num = 1;
            }
        }
        catch (Exception)
        {
        }
        return num;
    }
    #endregion
    #region -------------------------  打卡  --------------------------
    public static int Insert_QianDao(ref string errorMessage, string Lwr_PersonId, string Lwr_Date, string Lwr_StartTime, string Lwr_EndTime, string Lwr_Hour, string Lwr_BeiZhu, string Lwr_PersonStatus, string Lwr_UpTime, string Lwr_GpsStatus, string Lwr_MobileStatus, string Lwr_Power)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        int num = 0;
        string sql = "insert into Line_WorkRecord(Lwr_PersonId,Lwr_Date,Lwr_StartTime,Lwr_EndTime,Lwr_Hour,Lwr_BeiZhu,Lwr_PersonStatus,Lwr_UpTime,Lwr_GpsStatus,Lwr_MobileStatus,Lwr_Power) values(" +
                     " " + Lwr_PersonId + "," +
                     "'" + Lwr_Date + "'," +
                     "'" + Lwr_StartTime + "'," +
                     "'" + Lwr_EndTime + "'," +
                      "'" + Lwr_Hour + "'," +
                      "'" + Lwr_BeiZhu + "'," +
                     "'" + Lwr_PersonStatus + "'," +
                     "'" + Lwr_UpTime + "','" + Lwr_GpsStatus + "','" + Lwr_MobileStatus + "','" + Lwr_Power + "')";
        try
        {
            if (APP.SQLServer_Helper.Insert(sql) > 0)
            {
                num = 1;
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.ToString();
        }

        return num;
    }
    #endregion
    #region ------------------  考勤记录 -----------------------
    /// <summary>
    /// id:用户ID,dateStr 月份如2016-12 返回一个月的数据
    /// </summary>
    public static string Get_WorkRecord(int id, string dateStr)
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        string startDate = dateStr + "-01";
        string endDate = dateStr + "-31";
        try
        {
            string sql = string.Format(" select * from Line_WorkRecord where Lwr_UpTime>'{0} 00:00:00' and Lwr_UpTime<'{1} 23:59:59' and Lwr_PersonId={2} order by Lwr_UpTime desc", startDate, endDate, id);
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson(dt);
        return d;
    }
    #endregion
    #region ------------------  获取所有公告 -----------------------
    public static string Get_Notice()
    {
        //DataBaseClass db = new DataBaseClass(dbType.sql);
        DataTable dt = null;
        try
        {
            string sql = string.Format("  select * from Line_Notice");
            //dt = db.getTable(sql);
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
        }
        catch (Exception)
        {
        }
        string d = JsonTo.ToJson_Time(dt);
        return d;
    }
    #endregion
    #region ---------------------------水表井图片上传----------------------
    /// <summary>
    /// 水表井图片上传
    /// </summary>
    /// <param name="base64Photo">图片数据流</param>
    /// <param name="id">水表井ID</param>
    /// <param name="TableName">水表井名称,供水设施名称</param>
    /// <returns></returns>
    public static int ReceiveWellImage(string base64Photo, int id, string TableName)
    {
        string[] base64All = base64Photo.Split('$');
        int SuccessCount = 0;
        //将图片的名称进行拼接存储数据库
        string NeedSaveToDBImageName = string.Empty;
        //在数据库中进行查询该图层下的供水设施存储过的图片
        NeedSaveToDBImageName = SelectImageList(TableName, id);
        foreach (string item in base64All)
        {
            string[] arr = item.Split('|');
            //base64编码的图片
            string base64Image = arr[0];
            //图片格式带点
            string geshi = arr[1];
            //获取当前时间
            DateTime currentDay = DateTime.Now;
            //获取当前年份,为了创建目录使用
            string CurrentYear = currentDay.ToString("yyyy");
            //相对目录：作用是需要根据相对目录获取服务器上该目录的绝对物理目录
            //所有图片放在Image目录下，按照/表名称/设备ID/的层次建立文件夹，文件名用guid命名
            string relativeDir = "/image/" + TableName + "/" + id + "/" + CurrentYear;
            //相对路径：作用是需要返回已存储文件的网络路径
            string imageName = string.Format("{0:yyyyMMddHHmmssffff}", currentDay);
            string relativePath = string.Format("{0}/{1}{2}", relativeDir, imageName, geshi);
            //绝对物理目录：作用是需要判断该绝对物理目录是否存在
            string nowPhysicalDir = System.Web.HttpContext.Current.Server.MapPath(relativePath);//当前图片路径
            UploadReturn imageReturn = new UploadReturn();
            if (Base64StringToImage(base64Image, nowPhysicalDir))
            {
                //拼接需要在数据库中进行保存的图片名称
                if (!string.IsNullOrEmpty(NeedSaveToDBImageName))
                {
                    NeedSaveToDBImageName = imageName + geshi + "|" + NeedSaveToDBImageName;
                }
                else
                {
                    NeedSaveToDBImageName = imageName + geshi;
                }

                SuccessCount++;
                imageReturn.IsSuccess = true;
                string imagePath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + relativePath;
                imageReturn.Path = imagePath;
            }
        }
        if (SuccessCount < base64All.Length)
        {
            return 0;
        }
        else
        {
            SaveImagePath(TableName, id, NeedSaveToDBImageName);
            return 1;
        }
    }
    #endregion
    #region ---------------------------水表井图片删除----------------------
    /// <summary>
    /// 水表井图片删除
    /// </summary>
    /// <param name="TableName">水表井名称,供水设施名称</param>
    /// <param name="id">设施ID</param>
    /// <param name="ImageName">图片名称,多个图片之间以$分割,名称与图片类型之间以|分割</param>
    public static int ImageRemove(string TableName, int id, string ImageName)
    {

        //需要进行删除的图片数组
        string[] ImageArray = ImageName.Split('|');
        //在数据库中进行查询该图层下的供水设施存储过的图片
        string ImageListPath = SelectImageList(TableName, id);
        //当前需要进行存储未删除的图片
        string CurrentNeedSavePath = string.Empty;
        //进行迭代删除需要进行删除的图片
        //for (int i = 0; i < ImageArray.Length; i++)
        //{

        //}
        //如果数据库中不存在图片信息,直接返回错误
        if (string.IsNullOrEmpty(ImageListPath))
        {
            return 0;
        }
        else
        {
            //将返回的图片串分割为图片名称数据
            string[] iamgeAll = ImageListPath.Split('|');
            //循环比对找到需要进行删除的图片
            for (int m = 0; m < iamgeAll.Length; m++)
            {
                for (int n = 0; n < ImageArray.Length; n++)
                {
                    //如果数据库中包含需要进行删除的图片名称,则添加到删除队列
                    if (iamgeAll[m].ToString() == ImageArray[n].ToString())
                    {
                        break;
                    }//如果最后一行都不相同,则认为该图片不需要进行删除操作
                    else if (iamgeAll[m].ToString() != ImageArray[n].ToString() && n == ImageArray.Length - 1)
                    {
                        if (!string.IsNullOrEmpty(CurrentNeedSavePath))
                        {//拼接需要进行保存的图片名称
                            CurrentNeedSavePath = CurrentNeedSavePath + "|" + iamgeAll[m].ToString();
                        }
                        else
                        {
                            //拼接需要进行保存的图片名称
                            CurrentNeedSavePath = iamgeAll[m].ToString();
                        }
                    }
                }
            }
            //数据库保存返回1时算是成功进行物理删除文件,否则不进行操作,返回失败
            if (SaveImagePath(TableName, id, CurrentNeedSavePath) == 1)
            {
                try
                {
                    for (int k = 0; k < ImageArray.Length; k++)
                    {
                        //获取当前图片的名称
                        string CurrentDeleteImgName = ImageArray[k];
                        //获取当前图片的年份
                        string CurrentYearImgFile = CurrentDeleteImgName.Substring(0, 4);
                        //所有图片放在Image目录下，按照/表名称/设备ID/的层次建立文件夹
                        string relativeDir = "/image/" + TableName + "/" + id + "/" + CurrentYearImgFile + "/" + CurrentDeleteImgName;
                        //目前采用物理删除图片的方式  物理删除成功与否都算成功,因为数据库已经进行删除成功
                        File.Delete(System.Web.HttpContext.Current.Server.MapPath(relativeDir));
                    }
                }
                catch
                {
                    //获取到删除异常的时候也进行返回成功标志
                    return 1;
                }
                return 1;
            }
            //失败返回0
            else
            {
                return 0;
            }
        }
    }
    /// <summary>
    /// 根据图层名称和Smid进行查询该表井下的图片信息
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string ImageSelect(string TableName, int id)
    {
        return SelectImageList(TableName, id);
    }
    /// <summary>
    /// 根据表名称,设备ID进行查询图片集合
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string SelectImageList(string TableName, int id)
    {
        //数据库中图片名称集合
        string ImageListPath = string.Empty;
        //拼接查询表下设备存储图片sql语句
        string sql = " select image from " + TableName + " where SmID = " + id + " ";
        //初始化dt数据,防止空指针异常
        DataTable dt = new DataTable();
        //执行sql语句进行数据库查询
        dt = APP.SQLServer_Source_Helper.SelectDataTable(sql);
        //判断查询数据集数目,如果存在数据进行返回查询到的数据,否侧返回空
        if (dt.Rows.Count > 0)
        {
            //拼接图片名称集合
            ImageListPath = dt.Rows[0][0].ToString();
        }
        return ImageListPath;
    }
    /// <summary>
    /// 保存操作后的图片名称串
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="id"></param>
    /// <param name="NeedSavePath"></param>
    /// <returns></returns>
    public static int SaveImagePath(string TableName, int id, string NeedSavePath)
    {
        string errinfo = string.Empty;
        //拼接更新语句
        string sql = " update " + TableName + "  set image='" + NeedSavePath + "' where SmID=" + id + " ";
        APP.SQLServer_Source_Helper.UpDate(sql, out errinfo);
        //如果错误信息为空则成功了,否则失败
        if (string.IsNullOrEmpty(errinfo))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 更新设施描述
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="id"></param>
    /// <param name="desc"></param>
    /// <returns></returns>
    public static int UpdateDescription(string TableName, int id, string desc)
    {
        string errinfo = string.Empty;
        //拼接更新语句
        string sql = " update " + TableName + "  set description='" + desc + "' where SmID=" + id + " ";
        APP.SQLServer_Source_Helper.UpDate(sql, out errinfo);
        //如果错误信息为空则成功了,否则失败
        if (string.IsNullOrEmpty(errinfo))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static string GetDescription(string TableName, int id)
    {
        //数据库中图片名称集合
        string desc = string.Empty;
        //拼接查询表下设备存储图片sql语句
        string sql = " select description from " + TableName + " where SmID = " + id + " ";
        //初始化dt数据,防止空指针异常
        DataTable dt = new DataTable();
        //执行sql语句进行数据库查询
        dt = APP.SQLServer_Source_Helper.SelectDataTable(sql);
        //判断查询数据集数目,如果存在数据进行返回查询到的数据,否侧返回空
        if (dt.Rows.Count > 0)
        {
            //拼接图片名称集合
            desc = dt.Rows[0][0].ToString();
        }
        return desc;
    }
    #endregion

    #region 巡检轨迹查看模块
    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <returns></returns>
    public static string GetPatroList_Dept()
    {

        //初始化错误信息 
        string ErrInfo = string.Empty;
        string sqlStr = "";//sql查询语句
        sqlStr = " SELECT  iDeptID,cDepName,cDepEmail,iIsLocked  FROM P_Department  ";
        //执行SQL
        DataTable dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);

        string unionStr = string.Empty;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            unionStr += "{ \"iDeptID\": \"" + dt.Rows[i]["iDeptID"] + "\", \"cDepName\": \"" + dt.Rows[i]["cDepName"] + "\" },";
        }
        unionStr = "[" + unionStr.Substring(0, unionStr.ToString().Length - 1) + "]";

        return unionStr;
    }
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="iadminid"></param>
    /// <param name="deptid"></param>
    /// <returns></returns>
    public static string GetPatroPersonList_ByAminid(string iadminid, string deptid)
    {
        string sqlStr = "";//sql查询语句
        sqlStr = string.Format(@"  select pa.iAdminID,pa.cAdminName from P_InspectionPurview ppv left join P_Admin pa on ppv.iadmin_hv = pa.iAdminID where ppv.iadminid = {0} and powerType = 1   and pa.iDeptID = {1} ", iadminid, deptid);
        //执行SQL
        DataTable dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);

        string unionStr = string.Empty;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            unionStr += "{ \"iAdminID\": \"" + dt.Rows[i]["iAdminID"] + "\", \"cAdminName\": \"" + dt.Rows[i]["cAdminName"] + "\" },";
        }
        if (!string.IsNullOrEmpty(unionStr))
        {
            unionStr = "[" + unionStr.Substring(0, unionStr.ToString().Length - 1) + "]";
        }
        else
        {
            unionStr = "[]";
        }
        return unionStr;
    }

    //public static string getPatroPersonListByAminid(string iadmin, string starttime, string endtime)
    //{
    //    //查询时间开始时间赋值
    //    starttime = starttime + " 00:00:00";
    //    //查询时间结束时间赋值
    //    endtime = endtime + " 23:59:59";
    //    string sqlStr = "";//sql查询语句
    //    sqlStr = string.Format(@" select lc.lcp_id,lc.lcp_xy,lc.lcp_uptime,pa.cAdminName,lc.Lcp_zaixiantype  from  Line_CoordinatePosition lc left join P_Admin pa on pa.iAdminID = lc.lcp_name where lc.lcp_name = '{0}'  and lc.lcp_uptime >= '{1}' and lc.lcp_uptime <='{2}'  order by lcp_uptime asc ", iadmin, starttime, endtime);
    //    //执行SQL
    //    DataTable dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);
    //    string unionStr = string.Empty;
    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    {
    //        unionStr += "{ \"lcp_xy\": \"" + dt.Rows[i]["lcp_xy"] + "\", \"lcp_uptime\": \"" + dt.Rows[i]["lcp_uptime"] + "\" },";
    //    }
    //    if (!string.IsNullOrEmpty(unionStr))
    //    {
    //        unionStr = "[" + unionStr.Substring(0, unionStr.ToString().Length - 1) + "]";
    //    }
    //    else
    //    {
    //        unionStr = "[]";
    //    }
    //    return unionStr;
    //}
    public static string getPatroPersonListByAminid(string iadmin, string starttime, string endtime)
    {
        //查询时间开始时间赋值
        starttime = starttime + " 00:00:00";
        //查询时间结束时间赋值
        endtime = endtime + " 23:59:59";
        string sqlStr = "";//sql查询语句
        sqlStr = string.Format(@" select a.iAdminID,b.trackerid,g.locLon,g.locLat,g.locTime from Line_Gps g 
left join 
Line_BindTracker b
on g.deviceId=b.trackerid
left join 
P_Admin a
on b.userid=a.iAdminID where a.iAdminID={0} and g.locTime>='{1}' and g.locTime<='{2}' and g.speed !='000' order by g.locTime desc
 ", iadmin, starttime, endtime);
        //执行SQL
        DataTable dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);
        string unionStr = string.Empty;
        string gps_lon = "";
        string gps_lat = "";
        string lcp_xy = "";
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            gps_lon = dt.Rows[i]["locLon"].ToString();
            gps_lat = dt.Rows[i]["locLat"].ToString();
            lcp_xy = gps_lon + "|" + gps_lat;
            unionStr += "{ \"lcp_xy\": \"" + lcp_xy + "\", \"lcp_uptime\": \"" + dt.Rows[i]["locTime"] + "\" },";
        }
        if (!string.IsNullOrEmpty(unionStr))
        {
            unionStr = "[" + unionStr.Substring(0, unionStr.ToString().Length - 1) + "]";
        }
        else
        {
            unionStr = "[]";
        }
        return unionStr;
    }
    //修改用户密码
    public static string UpdatePassword(string iadminid, string oldpassword, string newpassword)
    {
        string sqlStr = "";//sql查询语句
        string unionStr = string.Empty;
        sqlStr = string.Format(@"select top 1 pa.iAdminID,pa.cAdminName,pa.cAdminPassWord from P_Admin pa where pa.iAdminID = {0} and  cAdminPassWord = '{1}' ", iadminid, oldpassword);
        //执行SQL
        DataTable dt = new DataTable();
        dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);
        if (dt.Rows.Count > 0)
        {
            sqlStr = string.Format(@"update P_Admin set cAdminPassWord = '{0}' where iAdminID = '{1}'  ", newpassword, iadminid);
            int affectCount = 0;
            affectCount = APP.SQLServer_Helper.UpDate(sqlStr);
            if (affectCount > 0)
            {
                unionStr = "[{issuccess:1,ErrInfo:\"\"}]";
            }
            else
            {
                unionStr = "[{issuccess:0,ErrInfo:\"更新新密码失败\"}]";
            }
        }
        else
        {
            unionStr = "[{issuccess:0,ErrInfo:\"旧密码输入错误,请联系管理员!!\"}]";
        }
        return unionStr;
    }
    /// <summary>
    /// 绑定追踪器
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="trackerid"></param>
    /// <returns></returns>
    public static string BindTracker(int userid, string trackerid)
    {
        string sqlStr = "";//sql查询语句
        string unionStr = string.Empty;
        sqlStr = string.Format(@"select top 1 userid from Line_BindTracker where userid={0}", userid);
        //执行SQL
        DataTable dt = new DataTable();
        dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);
        if (dt.Rows.Count > 0)
        {
            sqlStr = string.Format(@"update Line_BindTracker set trackerid = '{0}' where userid = '{1}'  ", trackerid, userid);
            int affectCount = 0;
            affectCount = APP.SQLServer_Helper.UpDate(sqlStr);
            if (affectCount > 0)
            {
                unionStr = "[{issuccess:1,ErrInfo:\"\"}]";
            }
            else
            {
                unionStr = "[{issuccess:0,ErrInfo:\"更新新密码失败\"}]";
            }
        }
        else
        {
            sqlStr = string.Format(@"  insert into Line_BindTracker(userid,trackerid) values({0},'{1}')", userid, trackerid);
            int affectCount = 0;
            affectCount = APP.SQLServer_Helper.UpDate(sqlStr);
            if (affectCount > 0)
            {
                unionStr = "[{issuccess:1,ErrInfo:\"\"}]";
            }
            else
            {
                unionStr = "[{issuccess:0,ErrInfo:\"更新新密码失败\"}]";
            }
        }
        return unionStr;
    }
    #endregion
    #region NFC电子标签巡检
    public static string retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//通用返回值
    public static string retStrBind = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"code\":" + "/*Code*/" + ",\"data\":" + "/*Data*/}]";//标签绑定返回值
    public static string retStrInWell = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"road\":\"/*Road*/\"" + ",\"data\":" + "/*Data*/}]";//井内关联设备返回值
    //所属井编号='新307-100排气阀井-3' 
    public static string baseSelectSql = @"  select '阀门' layerName,SmID smid,阀门口径 口径,所在道路   from 阀门 where {0} 
                      union all
                      select '消火栓' layerName,xhs.SmID smid,'' 口径,xhs.所在道路   from 消火栓 xhs left join 消火栓井 xhsj on xhs.所属井编号=xhsj.编号  where {0} 
                      union all
                      select '排气阀' layerName,SmID smid,口径,所在道路   from 排气阀 where {0} 
                      union all
                      select '水龙头' layerName,SmID smid,口径,所在道路   from 水龙头 where {0} 
                      union all
                      select '堵板' layerName,SmID smid,口径,所在道路   from 堵板 where {0} ";
    public static string ErrorInfo = "";
    /// <summary>
    /// 同步标签信息
    /// </summary>
    /// <returns></returns>
    public static string LoadLabels()
    {
        retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//返回值
        DataTable dt = null;
        try
        {
            string sql = "select * from NFCLabels";
            dt = APP.SQLServer_Helper.SelectDataTable(sql);
            string str = JsonTo.ToJson_Time(dt);
            retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", str);
        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }
        return retStr;
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
    public static string bindLabelId(string labelId, string equipmentName, int equipmentSmid, float coordinateX, float coordinateY, int adminId)
    {
        retStrBind = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"code\":" + "/*Code*/" + ",\"data\":" + "/*Data*/}]";//返回值
        string sqlStr = "";//sql查询语句
        string unionStr = string.Empty;
        sqlStr = string.Format(@"select top 1 id from NFCLabels where labelId='{0}'", labelId);
        //sqlStr = string.Format(@"select top 1 id from NFCLabels where labelId='{0}' and equipmentName='{1}' and equipmentSmid={2} ", labelId, equipmentName, equipmentSmid);
        try
        {
            //执行SQL
            DataTable dt = new DataTable();
            dt = APP.SQLServer_Helper.SelectDataTable(sqlStr);
            if (dt.Rows.Count > 0)//已经存在绑定信息，返回已经绑定
            {
                retStrBind = retStrBind.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：已经被绑定").Replace("/*Code*/", "1").Replace("/*Data*/", "[]");
            }
            else//不存在，则插入数据库，保存绑定信息
            {
                string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sqlStr = string.Format(@" insert into NFCLabels(labelId,equipmentName,equipmentSmid,coordinateX,coordinateY,adminId,fixTime) values('{0}','{1}',{2},{3},{4},{5},'{6}') ",
                                      labelId, equipmentName, equipmentSmid, coordinateX, coordinateY, adminId, dateStr);
                int affectCount = 0;
                affectCount = APP.SQLServer_Helper.UpDate(sqlStr, out ErrorInfo);
                if (affectCount > 0)
                {
                    retStrBind = retStrBind.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Code*/", "0").Replace("/*Data*/", "[]");
                }
                else
                {
                    retStrBind = retStrBind.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ErrorInfo).Replace("/*Code*/", "0").Replace("/*Data*/", "[]");
                }
            }
        }
        catch (Exception ex)
        {

            retStrBind = retStrBind.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ex.Message).Replace("/*Code*/", "0").Replace("/*Data*/", "[]");
        }

        return retStrBind;
    }
    /// <summary>
    /// 解绑
    /// </summary>
    /// <param name="labelId">电子标签的id</param>
    /// <param name="adminId">解除绑定操作的发起者的id</param>
    /// <returns></returns>
    public static string unbindLabelId(string labelId, int adminId)
    {
        retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//返回值
        try
        {
            string sql = string.Format(@"delete from NFCLabels where labelId='{0}'", labelId);
            int affectCount = 0;
            affectCount = APP.SQLServer_Helper.UpDate(sql, out ErrorInfo);
            if (affectCount > 0)
            {
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", "[]");
            }
            else
            {
                retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ErrorInfo).Replace("/*Data*/", "[]");
            }
        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }
        return retStr;
    }
    /// <summary>
    /// 关联该井内的设备
    /// </summary>
    /// <param name="layerName">井所在图层的名称</param>
    /// <param name="smid">井的smid号</param>
    /// <returns></returns>
    public static string equipmentInWell(string layerName, int smid)
    {
        retStrInWell = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"road\":\"/*Road*/\"" + ",\"data\":" + "/*Data*/}]";//井内关联设备返回值
        try
        {
            string sql = string.Format(@"select SmID,编号,所在道路 from {0} where SmID={1};", layerName, smid);
            DataTable dt = new DataTable();
            dt = APP.SQLServer_Source_Helper.SelectDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string bianHao = dt.Rows[0]["编号"].ToString();
                string daoLu = dt.Rows[0]["所在道路"].ToString();
                if (!string.IsNullOrEmpty(bianHao))
                {
                    //假设bianHao（编号）为101，从阀门，消火栓，排气阀，水龙头，堵板属性表中读取所属井编号为101的数据
                    string sqlProperty = string.Format(@"select '阀门' layerName,fm.SmID smid,阀门口径 caliber,fmj.运行状态 switchState from 阀门 fm left join 阀门井 fmj on fm.所属井编号=fmj.编号 where fm.所属井编号='{0}'
                union all
                select '消火栓' layerName,xhs.SmID smid,xhsj.口径 caliber,xhsj.运行状态 switchState from 消火栓 xhs left join 消火栓井 xhsj on xhs.所属井编号=xhsj.编号 where 所属井编号='{0}'
                union all
                select '排气阀' layerName,pqf.SmID smid,pqf.口径 caliber,pqfj.运行状态 switchState from 排气阀 pqf left join 排气阀井 pqfj on pqf.所属井编号=pqfj.编号 where 所属井编号='{0}'
                union all
                select '水龙头' layerName,SmID smid,口径 caliber,'' switchState   from 水龙头 where 所属井编号='{0}'
                union all
                select '堵板' layerName,SmID smid,口径 caliber,'' switchState   from 堵板 where 所属井编号='{0}'", bianHao);
                    DataTable dt2 = new DataTable();
                    dt2 = APP.SQLServer_Source_Helper.SelectDataTable(sqlProperty);
                    string str = JsonTo.ToJson(dt2);
                    retStrInWell = retStrInWell.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Road*/", daoLu).Replace("/*Data*/", str);
                }
                else
                {
                    retStrInWell = retStrInWell.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：井内编号为空").Replace("/*Road*/", daoLu).Replace("/*Data*/", "[]");
                }
            }
            else
            {
                retStrInWell = retStrInWell.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：此" + layerName + "中没有SmId为" + smid + "的信息").Replace("/*Road*/", "").Replace("/*Data*/", "[]");
            }


        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStrInWell = retStrInWell.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Road*/", "").Replace("/*Data*/", "[]");
        }
        return retStrInWell;
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
    public static string upNFCReport(string labelId, int adminId, string equipmentName, int equipmentSmid, string reportContent, string editDateTime)
    {
        retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//返回值
        //根据设备和编号，查找横坐标，纵坐标，所在道路，井内设备
        double x = 0;
        double y = 0;
        string eqInRoad = "";
        string eqInWell = "";
        string eqBianHao = "";
        try
        {
            string sqlStr = string.Format(@"select SmID,SmX,SmY,编号,所在道路 from {0} where SmID={1}", equipmentName, equipmentSmid);
            DataTable dtJing = new DataTable();
            dtJing = APP.SQLServer_Source_Helper.SelectDataTable(sqlStr, out ErrorInfo);
            if (dtJing != null && dtJing.Rows.Count > 0)
            {
                x = double.Parse(dtJing.Rows[0]["SmX"].ToString());
                y = double.Parse(dtJing.Rows[0]["SmY"].ToString());
                eqInRoad = dtJing.Rows[0]["所在道路"].ToString();
                eqBianHao = dtJing.Rows[0]["编号"].ToString();
                eqInWell = getEquipmentInWell(eqBianHao);//获取井内设备
                string sql = string.Format(@"INSERT INTO [NFCReport]
                   ([labelId]
                   ,[reportContent]
                   ,[adminId]
                   ,[equipmentName]
                   ,[equipmentSmid]
                   ,[editDateTime]
                   ,[coordinateX]
                   ,[coordinateY]
                   ,[equipmentInRoad]
                   ,[equipmentInWell])
             VALUES
                   ('{0}'
                   ,'{1}'
                   ,{2}
                   ,'{3}'
                   ,{4}
                   ,'{5}',{6},{7},'{8}','{9}')", labelId, reportContent, adminId, equipmentName, equipmentSmid, editDateTime, x, y, eqInRoad, eqInWell);
                int affectCount = 0;
                affectCount = APP.SQLServer_Helper.UpDate(sql, out ErrorInfo);
                if (affectCount > 0)
                {
                    retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", "[]");
                }
                else
                {
                    retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ErrorInfo).Replace("/*Data*/", "[]");
                }
            }
            else
            {
                retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ErrorInfo).Replace("/*Data*/", "[]");
            }


        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }
        return retStr;
    }
    /// <summary>
    /// 获取井内设备  阀门,1841,150|阀门,1842,150
    /// </summary>
    /// <param name="eqBianHao">井编号</param>
    /// <returns></returns>
    private static string getEquipmentInWell(string eqBianHao)
    {
        string condition = string.Format(" 所属井编号='{0}'", eqBianHao);
        string sql = string.Format(baseSelectSql, condition);
        string unionStr = "";
        DataTable dt = new DataTable();
        dt = APP.SQLServer_Source_Helper.SelectDataTable(sql);
        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string shebeiName = dt.Rows[i]["layerName"].ToString();
                string smid = dt.Rows[i]["smid"].ToString();
                string koujing = string.IsNullOrEmpty(dt.Rows[i]["口径"].ToString()) ? "无" : dt.Rows[i]["口径"].ToString();
                unionStr += shebeiName + "," + smid + "," + koujing;
                if (i < dt.Rows.Count - 1)
                {
                    unionStr += "|";
                }
            }
        }
        return unionStr;
    }
    #region 获取报表 从设备表中读取
    /// <summary>
    /// 获取报表
    /// </summary>
    /// <param name="adminId">提交巡检报告操作的发起者的id</param>
    /// <param name="startDate">查询报表的开始时间(格式：2017-07-07)</param>
    /// <param name="endDate">查询报表的截止时间(格式：2017-07-07)</param>
    /// <param name="formType">报表的类型。0：设备类型。1：口径。2：所在道路</param>
    /// <returns></returns>
    public static string patrolReportForm(int adminId, string startDate, string endDate, int formType)
    {
        retStr = "{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}";//巡检结果统计的数量返回值
        string strStartDate = startDate + " 00:00:00";
        string strEndDate = endDate + "23:59:59";
        //1获取井设备巡检数量的汇总信息,根据人员，时间组合sql语句，如下所示
        //equipmentName equipmentSmid   num
        //排气阀井         1             1
        //消火栓井         1             1
        //阀门井          20             1
        //阀门井          110            3
        string sqlStr = string.Format(@"select equipmentName,equipmentSmid,COUNT(equipmentSmid) num from [NFCReport]  report
                                        where editDateTime<='{0}' and editDateTime>='{1}' and adminId={2} 
                                        group by equipmentName,equipmentSmid ", endDate, startDate, adminId);
        DataTable dt = null;
        try
        {
            dt = APP.SQLServer_Helper.SelectDataTable(sqlStr, out ErrorInfo);
            if (dt.Rows.Count > 0)
            {
                string jingStrSql = "";
                string bianHaoCondition = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //2循环每一条数据，获取编号，以 or 连接，组合查询条件
                    //获取每一行井的名称，SmID,汇总的数量
                    string eqName = dt.Rows[i]["equipmentName"].ToString();//井名称，如阀门井
                    int eqSmid = int.Parse(dt.Rows[i]["equipmentSmid"].ToString());
                    int zongNum = int.Parse(dt.Rows[i]["num"].ToString());
                    jingStrSql += string.Format(@"select SmID,编号,所在道路 from {0} where SmID={1}", eqName, eqSmid);
                    if (i < dt.Rows.Count - 1)
                    {
                        jingStrSql += " union all ";
                    }
                }
                DataTable dtJing = new DataTable();
                dtJing = APP.SQLServer_Source_Helper.SelectDataTable(jingStrSql);
                for (int j = 0; j < dtJing.Rows.Count; j++)
                {
                    string bh = dtJing.Rows[j]["编号"].ToString();
                    bianHaoCondition += string.Format(" 所属井编号='{0}'", bh);
                    if (j < dtJing.Rows.Count - 1)
                    {
                        bianHaoCondition += " or ";
                    }
                }
                //根据组合号的所属井编号，进行阀门，消火栓等设备的记录统计  条件举例：所属井编号='北涧河-150加密阀井-1' or 所属井编号='凤凰路东-250蝶阀井-1' or 所属井编号='新307-100排气阀井-3'
                DataTable dtSheBei = new DataTable();
                //基础查询语句
                string baseStrSql = string.Format(@"select '阀门' layerName,SmID smid,阀门口径 口径,所在道路   from 阀门 where {0}
                  union all
                  select '消火栓' layerName, xhs.SmID smid, '' 口径, xhs.所在道路   from 消火栓 xhs left join 消火栓井 xhsj on xhs.所属井编号 = xhsj.编号  where {0}
                  union all
                  select '排气阀' layerName, SmID smid, 口径, 所在道路   from 排气阀 where {0}
                  union all
                  select '水龙头' layerName, SmID smid, 口径, 所在道路   from 水龙头 where {0}
                  union all
                  select '堵板' layerName, SmID smid, 口径, 所在道路   from 堵板 where {0}", bianHaoCondition);
                //统计个数查询语句
                string tongJiStrSql = "";
                //根据查询类别组合sql语句
                if (formType == 0)//按设备类型  如：阀门，消火栓，排气阀，水龙头，堵板
                {
                    tongJiStrSql = string.Format(@"  select a.layerName name,count(a.layerName) number from 
                      ({0}) a
                      group by a.layerName
                    ", baseStrSql);
                }
                else if (formType == 1)//按口径  如100，200口径巡查结果统计数量
                {
                    tongJiStrSql = string.Format(@"  select cast(a.口径 as int) name,count(a.layerName) number from 
                      ({0}) a
                      group by a.口径
                    ", baseStrSql);
                }
                else if (formType == 2)//按所在道路  如 江山路
                {
                    tongJiStrSql = string.Format(@"  select a.所在道路 name,count(a.layerName) number from 
                      ({0}) a
                      group by a.所在道路
                    ", baseStrSql);
                }
                dtSheBei = APP.SQLServer_Source_Helper.SelectDataTable(tongJiStrSql, out ErrorInfo);
                //修正返回字符串
                if (string.IsNullOrEmpty(ErrorInfo))
                {
                    string strSheBeiJson = JsonTo.ToJson(dtSheBei);
                    retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", strSheBeiJson);
                }
                else
                {
                    retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + ErrorInfo).Replace("/*Data*/", "[]");
                }
            }
            else
            {
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！获取井设备巡检数量的汇总信息数据为空").Replace("/*Data*/", "[]");
            }

        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }

        return retStr;
    }
    #endregion
    /// <summary>
    /// 获取报表  从NFCReport表中读取
    /// </summary>
    /// <param name="adminId">提交巡检报告操作的发起者的id</param>
    /// <param name="startDate">查询报表的开始时间(格式：2017-07-07)</param>
    /// <param name="endDate">查询报表的截止时间(格式：2017-07-07)</param>
    /// <param name="formType">报表的类型。0：设备类型。1：口径。2：所在道路</param>
    /// <returns></returns>
    public static string patrolReportForm_FromNFCReport(int adminId, string startDate, string endDate, int formType)
    {
        retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//巡检结果统计的数量返回值
        string strStartDate = startDate + " 00:00:00";
        string strEndDate = endDate + " 23:59:59";
        //1获取井设备巡检数量的汇总信息,根据人员，时间组合sql语句，如下所示
        //  equipmentName equipmentSmid   num equipmentInWell equipmentInRoad
        //阀门井 20  1   阀门,1538,250 凤凰路
        //阀门井 110 4   阀门,1841,150 | 阀门,1842,150 则天大街
        //  排气阀井    1   1   排气阀,30,100  新307国道
        //  消火栓井    1   1   消火栓,141,0 | 排气阀,46,100    凤凰路
        string sqlStr = string.Format(@"select equipmentName,equipmentSmid,COUNT(equipmentSmid) num,equipmentInWell,equipmentInRoad from [NFCReport]  report
                                        where editDateTime<='{0}' and editDateTime>='{1}' and adminId={2} 
                                        group by equipmentName,equipmentSmid,equipmentInWell,equipmentInRoad ", strEndDate, strStartDate, adminId);
        DataTable dt = null;
        try
        {
            dt = APP.SQLServer_Helper.SelectDataTable(sqlStr, out ErrorInfo);
            if (dt.Rows.Count > 0)
            {
                DataTable dtEqInWell = new DataTable();
                dtEqInWell.Columns.Add("设备类型");
                dtEqInWell.Columns.Add("SmID", typeof(Int32));
                dtEqInWell.Columns.Add("口径");
                dtEqInWell.Columns.Add("所在道路");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string eqInWell = dt.Rows[i]["equipmentInWell"].ToString();
                    int num = int.Parse(dt.Rows[i]["num"].ToString());//此井出现的次数
                    string daoLu = dt.Rows[i]["equipmentInRoad"].ToString();
                    string[] arr = eqInWell.Split('|');//阀门,1841,150|阀门,1842,150
                    if (!string.IsNullOrEmpty(eqInWell))
                    {
                        for (int j = 0; j < arr.Length; j++)
                        {
                            string strSingle = arr[j];//阀门,1841,150
                            string[] arrSingle = strSingle.Split(',');
                            for (int k = 0; k < num; k++)//例如num为4，则循环四次加载数据
                            {
                                DataRow dr = dtEqInWell.NewRow();
                                dr["设备类型"] = arrSingle[0];//阀门
                                dr["SmID"] = int.Parse(arrSingle[1]);//1841
                                dr["口径"] = arrSingle[2];//150
                                dr["所在道路"] = daoLu;
                                dtEqInWell.Rows.Add(dr);
                            }

                        }
                    }

                }
                string strSheBeiJson = "";
                //根据查询类别组合sql语句
                if (formType == 0)//按设备类型  如：阀门，消火栓，排气阀，水龙头，堵板
                {
                    //使用linq to DataTable group by实现
                    var query = from t in dtEqInWell.AsEnumerable()
                                group t by new { t1 = t.Field<string>("设备类型") } into m
                                select new
                                {
                                    name = m.Key.t1,
                                    number = m.Count()
                                };
                    strSheBeiJson = Newtonsoft.Json.JsonConvert.SerializeObject(query);

                }
                else if (formType == 1)//按口径  如100，200口径巡查结果统计数量
                {
                    //使用linq to DataTable group by实现
                    var query = from t in dtEqInWell.AsEnumerable()
                                group t by new { t2 = t.Field<string>("口径") } into m
                                select new
                                {
                                    name = m.Key.t2,
                                    //sum = m.Sum(n => n.Field<int>("SmID"))
                                    number = m.Count()
                                };
                    //if (query.ToList().Count > 0)
                    //{
                    //    query.ToList().ForEach(q =>
                    //    {
                    //        Console.WriteLine(q.koujing + "," + q.count);
                    //    });
                    //}
                    strSheBeiJson = Newtonsoft.Json.JsonConvert.SerializeObject(query);
                }
                else if (formType == 2)//按所在道路  如 江山路
                {
                    //使用linq to DataTable group by实现
                    var query = from t in dtEqInWell.AsEnumerable()
                                group t by new { t1 = t.Field<string>("所在道路") } into m
                                select new
                                {
                                    name = m.Key.t1,
                                    number = m.Count()
                                };
                    strSheBeiJson = Newtonsoft.Json.JsonConvert.SerializeObject(query);
                }
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", strSheBeiJson);

            }
            else
            {
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！获取井设备巡检数量的汇总信息数据为空").Replace("/*Data*/", "[]");
            }

        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }

        return retStr;
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
    public static string reportFormDetails(int adminId, string startDate, string endDate, int formType, string name, int pageNumber)
    {
        retStr = "[{\"result\":" + "/*Result*/" + ",\"message\":\"/*Message*/\"" + ",\"data\":" + "/*Data*/}]";//巡检结果统计的数量返回值
        string strStartDate = startDate + " 00:00:00";
        string strEndDate = endDate + " 23:59:59";
        //1获取井设备巡检数量的汇总信息,根据人员，时间组合sql语句，如下所示
        //  equipmentName equipmentSmid   num equipmentInWell equipmentInRoad
        //阀门井 20  1   阀门,1538,250 凤凰路
        //阀门井 110 4   阀门,1841,150 | 阀门,1842,150 则天大街
        //  排气阀井    1   1   排气阀,30,100  新307国道
        //  消火栓井    1   1   消火栓,141,0 | 排气阀,46,100    凤凰路
        string sqlStr = string.Format(@"SELECT TOP 1000 [id]
                                      ,[labelId]
                                      ,[reportContent]
                                      ,[adminId]
                                      ,[equipmentName]
                                      ,[equipmentSmid]
                                      ,[coordinateX]
                                      ,[coordinateY]
                                      ,[equipmentInRoad]
                                      ,[equipmentInWell]
                                      ,[editDateTime]
                                  FROM [NFCReport] where editDateTime<='{0}' and editDateTime>='{1}' and adminId={2}", strEndDate, strStartDate, adminId);
        DataTable dt = null;
        try
        {
            dt = APP.SQLServer_Helper.SelectDataTable(sqlStr, out ErrorInfo);
            if (dt.Rows.Count > 0)
            {
                DataTable dtEqInWell = new DataTable();
                dtEqInWell.Columns.Add("设备类型");
                dtEqInWell.Columns.Add("SmID", typeof(int));
                dtEqInWell.Columns.Add("口径");
                dtEqInWell.Columns.Add("所在道路");
                dtEqInWell.Columns.Add("横坐标");
                dtEqInWell.Columns.Add("纵坐标");
                dtEqInWell.Columns.Add("巡检内容");
                dtEqInWell.Columns.Add("上报时间");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string eqInWell = dt.Rows[i]["equipmentInWell"].ToString();
                    string daoLu = dt.Rows[i]["equipmentInRoad"].ToString();
                    string x = dt.Rows[i]["coordinateX"].ToString();
                    string y = dt.Rows[i]["coordinateY"].ToString();
                    string content = dt.Rows[i]["reportContent"].ToString();
                    string sj = dt.Rows[i]["editDateTime"].ToString();
                    string[] arr = eqInWell.Split('|');//阀门,1841,150|阀门,1842,150
                    for (int j = 0; j < arr.Length; j++)
                    {
                        string strSingle = arr[j];//阀门,1841,150
                        string[] arrSingle = strSingle.Split(',');
                        DataRow dr = dtEqInWell.NewRow();
                        dr["设备类型"] = arrSingle[0];//阀门
                        dr["SmID"] = int.Parse(arrSingle[1]);//1841
                        dr["口径"] = arrSingle[2];//150
                        dr["所在道路"] = daoLu;
                        dr["横坐标"] = x;
                        dr["纵坐标"] = y;
                        dr["巡检内容"] = content;
                        dr["上报时间"] = sj;
                        dtEqInWell.Rows.Add(dr);
                    }
                }
                string strSheBeiJson = "";
                string typeStr = "";
                //根据查询类别组合sql语句
                if (formType == 0)//按设备类型  如：阀门，消火栓，排气阀，水龙头，堵板
                {
                    typeStr = "设备类型";
                }
                else if (formType == 1)//按口径  如100，200口径巡查结果统计数量
                {
                    typeStr = "口径";
                }
                else if (formType == 2)//按所在道路  如 江山路
                {
                    typeStr = "所在道路";
                }
                int pageSize = 60;//分页显示个数
                //使用linq to DataTable 条件查询实现
                var query = (from t in dtEqInWell.AsEnumerable()
                             where t.Field<string>(typeStr) == name
                             orderby t.Field<string>("上报时间") descending
                             select new
                             {
                                 equipmentName = t.Field<string>("设备类型"),
                                 smid = t.Field<int>("SmID"),
                                 coordinateX = t.Field<string>("横坐标"),
                                 coordinateY = t.Field<string>("纵坐标"),
                                 reportContent = t.Field<string>("巡检内容"),
                                 editDateTime = t.Field<string>("上报时间")

                             }).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                strSheBeiJson = Newtonsoft.Json.JsonConvert.SerializeObject(query);
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！").Replace("/*Data*/", strSheBeiJson);

            }
            else
            {
                retStr = retStr.Replace("/*Result*/", "true").Replace("/*Message*/", "成功！获取井设备巡检数量的汇总信息数据为空").Replace("/*Data*/", "[]");
            }

        }
        catch (Exception ex)
        {
            string err = ex.Message;
            retStr = retStr.Replace("/*Result*/", "false").Replace("/*Message*/", "失败：" + err).Replace("/*Data*/", "[]");
        }
        return retStr;
    }
    #endregion
}
