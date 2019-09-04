using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

/// <summary>
/// BaseAshxPage 的摘要说明
/// </summary>
public class BaseAshxPage : IHttpHandler, IRequiresSessionState
{
    public BaseAshxPage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    
    public HttpContext Context;
    public bool IsReusable
    {
       
        get { throw new NotImplementedException(); }
    }

    public virtual void Ashx_Load()
    {

    }

    public void ProcessRequest(HttpContext context)
    {
        Context = context;

        Context.Response.Buffer = true;
        Context.Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);
        Context.Response.Expires = 0;
        Context.Response.CacheControl = "no-cache";
        Context.Response.ContentType = "text/json";
        //context.Response.ContentType = "text/plain";
        Context.Response.Charset = "UTF-8";

        Ashx_Load();
    }

    
}