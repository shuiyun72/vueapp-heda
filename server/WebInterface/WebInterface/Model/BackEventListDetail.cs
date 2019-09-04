using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.Model
{
    public class BackEventListDetail
    {
        public Boolean result = false;
        public String message = "失败";
        public List<BackEventListDetailInfo> Data = new List<BackEventListDetailInfo>();
    }
    public class BackEventListDetailInfo
    {
        public string Devicename;
        public int Devicesmid;
        public string Uptime;
        public string X;
        public string Y;
        public string Longitude;
        public string Latitude;
        public int PersonId;
        public List<BackEventInfoImage> ImageUrl = new List<BackEventInfoImage>();
        public List<BackEventInfoVoice> VoiceUrl = new List<BackEventInfoVoice>();
        public int EventId;
        public int EventContentId;
        public string EventAddress;
        public string Description;
        public int IsHidden;
        public int TaskId;
        public int MUngercyId;
        public int MLevelId;      
        public int IsTemp;
        public int DeptId;
        public int OperEventID;
        public string TaskName;
        public string Remark_Back;
        public string UrgencyName;
        public string HandlerLevelName;
        public string EventTypeName1;
        public string EventTypeName2;
    }
    public class BackEventInfoImage
    {
        //事件图片
        public string ImageUrl;
    }
    public class BackEventInfoVoice
    {
        //事件图片
        public string VoiceUrl;
    }
}