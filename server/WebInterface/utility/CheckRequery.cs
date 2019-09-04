using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace utility
{
    public class CheckRequery
    {
        public static void checkNotNull(string requeryName, out string strValue)
        {
            checkNotNull(requeryName, "", out strValue);
        }
        public static void checkNotNull(string requeryName, string alertString, out string strValue)
        {
            strValue = string.Empty;

            if (HttpContext.Current.Request[requeryName] != null)
            {
                strValue = HttpContext.Current.Request[requeryName];
            }

            if (HttpContext.Current.Request[requeryName] == null)
            {
                HttpContext.Current.Response.Write(WebExport.ExportErrMsg(alertString + "缺少变量" + requeryName));
                HttpContext.Current.Response.End();
            }
            else if (string.IsNullOrEmpty(strValue))
            {
                HttpContext.Current.Response.Write(WebExport.ExportErrMsg(alertString + "不能为空" + requeryName));
                HttpContext.Current.Response.End();
            }
        }
        public static void checkNotNull(string requeryName, string alertString, out int intValue)
        {
            string strValue = string.Empty;
            intValue = 0;

            if (HttpContext.Current.Request[requeryName] != null)
            {
                strValue = HttpContext.Current.Request[requeryName];
                if (int.TryParse(strValue, out intValue))
                {

                }
                else
                {
                    HttpContext.Current.Response.Write(alertString + "必须为数字" + requeryName);
                    HttpContext.Current.Response.End();
                }
            }
            else
            {
                HttpContext.Current.Response.Write(WebExport.ExportErrMsg(alertString + "缺少变量" + requeryName));
                HttpContext.Current.Response.End();
            }

        }
        public static void check(string requeryName, out string strValue)
        {
            strValue = string.Empty;
            if (HttpContext.Current.Request[requeryName] != null)
            {
                strValue = HttpContext.Current.Request[requeryName];
            }
        }
        public static void check(string requeryName, out int intValue)
        {
            string strValue = string.Empty;
            intValue = 0;
            if (HttpContext.Current.Request[requeryName] != null)
            {
                strValue = HttpContext.Current.Request[requeryName];
                if (int.TryParse(strValue, out intValue))
                {

                }
                else
                {
                    HttpContext.Current.Response.Write("必须为数字" + requeryName);
                    HttpContext.Current.Response.End();
                }
            }
        }
    }
}
