using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
/// <summary>
/// JsonExport 的摘要说明
/// </summary>
namespace utility
{
    public enum ExportType { Json }

    public class WebExport
    {
        public WebExport() { }
        
        /// <summary>
        /// 错误代码 500 数据错误；101登录超时；201没有权限；0正常
        /// </summary>
        public int ErrCode = 0;  

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public object Data = string.Empty;
        /// <summary>
        /// 为以后扩展
        /// </summary>
        public ExportType ContentType = ExportType.Json; 

        
        
        //=========================================
        // 以下为输出功能扩展
        //=========================================



        /// <summary>
        /// 输出错误JSON信息
        /// </summary>
        /// <param name="_ErrInfo">错误信息</param>
        /// <param name="_ContentType">输出类型</param>
        /// <returns></returns>
        public static string ExportErrMsg(string _ErrInfo, ExportType _ContentType)
        {
            WebExport WE = new WebExport();
            WE.ErrCode = 500;
            WE.ErrInfo = _ErrInfo;
            WE.ContentType = _ContentType;

            return JsonConvert.SerializeObject(WE);
        }

        /// <summary>
        /// 输出错误信息 默认JSON
        /// </summary>
        /// <param name="_ErrInfo">错误信息</param>
        /// <returns></returns>
        public static string ExportErrMsg(string _ErrInfo)
        {
            return ExportErrMsg(_ErrInfo, ExportType.Json);
        }
        public static void ExportErrMsgWrite(string _ErrInfo)
        {
            HttpContext.Current.Response.Write(ExportErrMsg(_ErrInfo, ExportType.Json));
            HttpContext.Current.Response.End();
        }


        /// <summary>
        /// 输出错误信息
        /// </summary>
        /// <param name="WE">输出对象</param>
        /// <returns></returns>
        public static string Export(WebExport WE)
        {
            return JsonConvert.SerializeObject(WE);
        }

        /// <summary>
        /// 输出正处返回的信息
        /// </summary>
        /// <param name="_Data">输出数据</param>
        /// <returns></returns>
        public static string ExportSuccess(Object _Data)
        {
            WebExport WE = new WebExport();
            WE.ErrCode = 0;
            WE.Data = _Data;
            
            return JsonConvert.SerializeObject(WE, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
        }
        public static void ExportSuccessWrite(Object _Data)
        {
            WebExport WE = new WebExport();
            WE.ErrCode = 0;
            WE.Data = _Data;

            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(WE, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }));
        }
        /// <summary>
        /// 输出正处返回的信息
        /// </summary>
        /// <param name="WE">输出对象</param>
        /// <param name="_ContentType">输出类型</param>
        /// <returns></returns>
        public static string ExportSuccessMsg(WebExport WE, ExportType _ContentType)
        {
            return JsonConvert.SerializeObject(WE, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
        }


    }
}