using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.Model
{
    /// <summary>
    /// 群发消息类
    /// </summary>
    public class NewsResult
    {
        public Boolean result = false;
        public String message = "失败";
        public List<NewsContentList> Data = new List<NewsContentList>();
    }
    public class NewsContentList
    {
        //消息ID
        public string MessageId;
        //消息主体
        public string MessageInfo;
        //是否已查看
        public string BoolReadStatus;
    }
}