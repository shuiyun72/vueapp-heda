using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace utility
{
    /// <summary>
    /// WebExport_EasyUI_Pagination 的摘要说明
    /// </summary>
    public class EasyUI_Pagination : WebExport
    {
        public EasyUI_Pagination()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public int total = 0;
        public object rows = string.Empty;

        /// <summary>
        /// 输出正处返回的信息
        /// </summary>
        /// <param name="_Data">输出数据</param>
        /// <param name="_total">输出页面Index索引</param>
        /// <returns></returns>
        public static string ExportSuccess(Object _Data, int _total)
        {
            EasyUI_Pagination WE = new EasyUI_Pagination();
            WE.ErrCode = 0;
            WE.ErrInfo = "";

            WE.rows = _Data;
            WE.total = _total;
            string str = JsonConvert.SerializeObject(WE, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            return str;
            //return JsonConvert.SerializeObject(WE, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
        }

        /// <summary>
        /// 输出错误JSON信息
        /// </summary>
        /// <param name="_ErrInfo">错误信息</param>
        /// <returns></returns>
        public static string ExportErrMsg(string _ErrInfo)
        {
            EasyUI_Pagination WE = new EasyUI_Pagination();
            WE.ErrCode = 500;
            WE.ErrInfo = _ErrInfo;

            WE.ErrCode = 1;
            WE.rows = null;
            WE.total = 0;

            return JsonConvert.SerializeObject(WE);
        }

    }
}