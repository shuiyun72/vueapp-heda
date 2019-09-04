
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

/// <summary>
/// APP 的摘要说明
/// </summary>
public static class APP
{
  /// <summary>
  /// PostGreSql数据库操作
  /// </summary>
  public static DbHelper.PGSQL_DBHelper PGSQL_Helper = new DbHelper.PGSQL_DBHelper(ConfigurationManager.AppSettings["PGSQL_ConnString"].ToString());
  /// <summary>
  /// 一体化菜单读取
  /// </summary>
  public static DbHelper.SqlDbHelper Menu_Helper = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStrCaiDan"].ToString());
  /// <summary>
  /// Gis巡检据库操作
  /// </summary>
  public static DbHelper.SqlDbHelper SQLServer_Helper = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStr"].ToString());
  /// <summary>
  /// Gis基本库数据库操作
  /// </summary>
  public static DbHelper.SqlDbHelper SQLServer_BaseGisDB_Helper = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStr_BaseGisDB"].ToString());

  /// <summary>
  /// 获取车辆经纬度坐标
  /// </summary>
  public static DbHelper.SqlDbHelper SQLServer_GetCarPoss = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStr_CarGPS"].ToString());
  /// <summary>
  /// Gis供水设施源数据库操作
  /// </summary>
  public static DbHelper.SqlDbHelper SQLServer_Source_Helper = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStrSourceData"].ToString());

  public static DbHelper.SqlDbHelper SQLServer_PingTai = new DbHelper.SqlDbHelper(ConfigurationManager.AppSettings["ConnStr_PingTai"].ToString());

  public static string MakeSqlByPage(string SQLString, string Ordering, int PageIndex, int PageSize)
  {
    int beginNum = (PageIndex - 1) * (PageSize);
    beginNum = beginNum < 0 ? 0 : beginNum;
    int endNum = beginNum + PageSize;
    string ExcSql = string.Empty;
    if (SQLString.ToLower().Contains("from"))
    {
      ExcSql = "SELECT * FROM(SELECT * , ROW_NUMBER() OVER ( Order by " + Ordering + ") AS Pos FROM (" + SQLString + ") as T) AS TT where TT.Pos > " + beginNum.ToString() + " and TT.Pos <= " + endNum.ToString();
      ExcSql = ExcSql + ";select count(*) RowsCount from (" + SQLString + ") as tt";
    }
    else
    {
      ExcSql = "SELECT * FROM(SELECT * , ROW_NUMBER() OVER ( Order by " + Ordering + ") AS Pos FROM " + SQLString + " as T) AS TT where TT.Pos > " + beginNum.ToString() + " and TT.Pos <= " + endNum.ToString();
      ExcSql = ExcSql + ";select count(*) RowsCount from " + SQLString + " as tt";
    }
    return ExcSql;
  }
  /// <summary>
  /// 写入管理日志
  /// </summary>
  /// <param name="action_type"></param>
  /// <param name="remark"></param>
  /// <returns></returns>
  public static bool AddAdminLog(string action_type, string remark)
  {
    string sqlStr = @"INSERT INTO L_Log (LogType,LogContent,LogTime,UserId,UserIP,LoginName)
                              VALUES (@LogType,@LogContent,@LogTime,@UserId,@UserIP,@LoginName)";

    SqlCommand cmd = APP.SQLServer_Helper.GetDbCommand(sqlStr);
    cmd.Parameters.AddWithValue("@LogType", action_type);
    cmd.Parameters.AddWithValue("@LogContent", remark);
    cmd.Parameters.AddWithValue("@LogTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    cmd.Parameters.AddWithValue("@UserId", HttpContext.Current.Session["iAdminID"].ToString());
    cmd.Parameters.AddWithValue("@UserIP", GetIP());
    cmd.Parameters.AddWithValue("@LoginName", HttpContext.Current.Session["cAdminName"].ToString());

    int i = APP.SQLServer_Helper.Insert(cmd);
    if (i > 0)
      return true;
    else
      return false;
  }
  /// <summary>
  /// 获得当前页面客户端的IP
  /// </summary>
  /// <returns>当前页面客户端的IP</returns>
  public static string GetIP()
  {
    string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; //GetDnsRealHost();
    if (string.IsNullOrEmpty(result))
      result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    if (string.IsNullOrEmpty(result))
      result = HttpContext.Current.Request.UserHostAddress;
    if (string.IsNullOrEmpty(result) || !IsIP(result))
      return "127.0.0.1";
    return result;
  }
  /// <summary>
  /// 是否为ip
  /// </summary>
  /// <param name="ip"></param>
  /// <returns></returns>
  public static bool IsIP(string ip)
  {
    return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
  }
}
