using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using utility;
using System.Data;

namespace WebInterface.ashx
{
    /// <summary>
    /// scadaUser 的摘要说明
    /// </summary>
    public class scadaUser : BaseAshxPage
    {
        public override void Ashx_Load() 
        {
            Context.Response.ContentType = "text/plain";
            Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            Context.Response.AddHeader("Access-Control-Allow-Methods", "POST");
            Context.Response.AddHeader("Access-Control-Max-Ag", "1000");

            string actionType = "";
            //string retStr = "";//返回值
            string ErrInfo = string.Empty;
            string sqlStr = "";
            CheckRequery.checkNotNull("actionType", "类型不能为空！", out actionType);
            string id = "";
            string personName = "";
            string personTelphone = "";
            string deptID = "";
            string personPassword = "";
            string deptName = "";
            string roleId = "";

            CheckRequery.check("deptID", out deptID);
            CheckRequery.checkNotNull("id", "不能为空！", out id);
            CheckRequery.check("personName", out personName);
            CheckRequery.check("personTelphone", out personTelphone);
            CheckRequery.check("roleId", out roleId);

            switch (actionType)
            {               
                //添加一个人员
                case "ADD":
                    CheckRequery.check("personPassword", out personPassword);
                    CheckRequery.check("deptName", out deptName);

                    sqlStr = "INSERT INTO L_Person( PersonName, Telephone, DepartmentId, PersonId, PassWord,iRoleID,DeleteStatus )VALUES ( '" + personName + "','" + personTelphone + "','" + deptID + "','" + id + "','" + personPassword + "','" + roleId + "','0')";
                    int m = APP.SQLServer_Helper.UpDate(sqlStr, out ErrInfo);
                    if (!string.IsNullOrEmpty(ErrInfo))
                    {
                        Context.Response.Write(WebExport.ExportErrMsg("添加失败，请检查数据!"));
                    }
                    if (m > 0)
                    {
                        Context.Response.Write(WebExport.ExportSuccess("添加成功!"));
                    }
                    break;

                //删除一个人员
                case "DEL":
                    //sqlStr = "delete from L_Person where PersonId = " + id + "";
                    sqlStr = " UPDATE dbo.L_Person SET DeleteStatus='1' WHERE PersonId='"+ id +"'";
                    int d = APP.SQLServer_Helper.UpDate(sqlStr, out ErrInfo);
                    if (!string.IsNullOrEmpty(ErrInfo))
                    {
                        Context.Response.Write(WebExport.ExportErrMsg("删除失败，请检查数据!"));
                    }

                    if (d > 0)
                    {
                        Context.Response.Write(WebExport.ExportSuccess("删除成功!"));
                    }
                    break;

                //更新人员
                case "MODIFY":

                    sqlStr = "update L_Person set PersonName = '" + personName + "',Telephone='" + personTelphone + "',DepartmentId='" + deptID + "',iRoleID = '" + roleId + "'  where PersonId=" + id + "";
                    int update1 = APP.SQLServer_Helper.UpDate(sqlStr, out ErrInfo);
                    if (!string.IsNullOrEmpty(ErrInfo))
                    {
                        Context.Response.Write(WebExport.ExportErrMsg("修改失败，请检查数据!"));
                    }

                    if (update1 > 0)
                    {
                        Context.Response.Write(WebExport.ExportSuccess("修改成功!"));
                    }
                    break;

                default:
                    break;
            }
            //Context.Response.Write(retStr);
        }
    }
}