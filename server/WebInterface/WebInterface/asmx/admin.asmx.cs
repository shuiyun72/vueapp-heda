using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using utility;
using System.Data;
using System.Web.Services;
using System.Text;
namespace WebInterface.asmx
{
    /// <summary>
    /// admin 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class admin : System.Web.Services.WebService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="iAdminID">用户ID</param>
        /// <returns></returns>
        [WebMethod]
        public string GetAdminInfo(string iAdminID)
        {
            if (string.IsNullOrEmpty(iAdminID))
            {
                return WebExport.ExportSuccess("iAdminID!");
            }


            DataTable DT;
            string ErrInfo = "";
            bool IsOk = APP.SQLServer_PingTai.GetDataTable("select * from V_P_Admin where iAdminID="+ iAdminID.Trim(), out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                return WebExport.ExportSuccess(DT);
            }
            else
            {
                return WebExport.ExportSuccess("获取用户信息失败!");
            }
        }

        [WebMethod]
        public string GetAdminPurviewInfo(string iAdminID)
        {
            if (string.IsNullOrEmpty(iAdminID))
            {
                return WebExport.ExportSuccess("iAdminID!");
            }


            DataTable DT;
            string ErrInfo = "";
            bool IsOk = APP.SQLServer_PingTai.GetDataTable("select * from V_P_Admin where iAdminID=" + iAdminID.Trim(), out DT, out ErrInfo);
            //执行完成返回
            if (IsOk)
            {
                if(DT.Rows.Count>0)
                {
                    string Sql = "SELECT distinct [iFunID] ,[cFunName],iFunFatherID,cFunMenuOrder  FROM [V_P_FunPurview]  where (iPurviewID =" + iAdminID.Trim() + "  and iPurviewType=1) or (iPurviewID =" + DT.Rows[0]["iRoleID"].ToString() + "  and iPurviewType=2) or (iPurviewID =" + DT.Rows[0]["iDeptID"].ToString() + "  and iPurviewType=3)";
                    DataTable DT2;
                    IsOk = APP.SQLServer_PingTai.GetDataTable(Sql, out DT2, out ErrInfo);
                    if (IsOk)
                    {
                        StringBuilder listStr = new StringBuilder();
                        bool[] arrShowLine = new bool[10];
                        listStr.Append("[{\"id\":0,\"text\":\"APP系统\",\"children\":");
                        listStr.Append(getChidrenTree(DT2, "1000016", 0, false, "iFunFatherID", "cFunMenuOrder", " asc", arrShowLine));
                        listStr.Append("}]");
                        return listStr.ToString();
                    }
                    else
                    {
                        return WebExport.ExportErrMsg("权限获取中用户权限信息失败!"+ErrInfo);
                    }

                    
                }
                else
                {
                    return WebExport.ExportErrMsg("权限获取中没有查询到用户信息!");
                }
                
            }
            else
            {
                return WebExport.ExportErrMsg("权限获取中用户信息失败!");
            }
        }

        private string getChidrenTree(DataTable a, string strID, int num, bool isLast, string slelectField, string orderField, string order, bool[] arrShowLine)
        {

            StringBuilder itemString = new StringBuilder();
            DataRow[] c = a.Select(slelectField + "=" + strID, orderField + order, DataViewRowState.CurrentRows);
            int cItemNum = c.GetLength(0);
            if (cItemNum == 0)
            {
                return "[]";
            }

            bool isLast2 = true;
            num++; //深度
            itemString.Append("[");
            for (int j = 0; j < cItemNum; j++)
            {
                itemString.Append("{");
                itemString.Append("\"id\":\"");
                itemString.Append(c[j]["iFunID"].ToString());
                itemString.Append("\",\"text\":\"");
                itemString.Append(c[j][1].ToString());
                itemString.Append("\"");



                string CT = getChidrenTree(a, c[j][0].ToString(), num, isLast2, slelectField, orderField, order, arrShowLine);
                if (!string.IsNullOrEmpty(CT))
                {
                    itemString.Append(",\"children\":");
                    itemString.Append(CT);
                }
                else
                {
                    //if (!string.IsNullOrEmpty(c[j]["pur"].ToString()))
                    //{
                    //    itemString.Append(",\"checked\":true");
                    //}
                }

                itemString.Append("}");
                if (j == (cItemNum - 1))
                {
                    itemString.Append("]");
                }
                else
                {
                    itemString.Append(",");
                }
            }
            return itemString.ToString();
        }


    }
}
