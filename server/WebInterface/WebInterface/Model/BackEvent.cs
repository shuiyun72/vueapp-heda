using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.Model
{
    public class BackEvent
    {
        public Boolean result = false;
        public String message = "失败";
        public List<BackEventList> Data = new List<BackEventList>();
    }
    public class BackEventList
    {
        //巡检任务ID
        public string TaskId;
        //巡检任务名称
        public string TaskName;
        //事件上传退回事件数
        public string FailCount;
    }
}