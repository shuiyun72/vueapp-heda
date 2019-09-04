using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebInterface.dal;

namespace WebInterface.ashx
{
    /// <summary>
    /// GetNoticeData 的摘要说明
    /// </summary>
    public class GetNoticeData : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(Data_Inspection_Dal.Get_Notice());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}