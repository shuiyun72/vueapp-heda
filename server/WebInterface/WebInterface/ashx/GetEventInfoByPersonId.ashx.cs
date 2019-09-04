using Newtonsoft.Json;
using System;
using System.Data;
using utility;

namespace WebInterface.ashx
{
    public class GetEventInfoByPersonId : BaseAshxPage
    {
        public override void Ashx_Load()
        {
            string ErrInfo_ = string.Empty;
            string pageIndex = "1", dataCount = "10",dataSort = "EventID desc", PersonId;
            string retStr = "{\"rows\":" + "/*Rows*/" + ",\"total\":/*Total*/" + ",\"ErrCode\":0}";
            CheckRequery.check("PersonId", out PersonId);
            CheckRequery.checkNotNull("page", out pageIndex);
            CheckRequery.checkNotNull("rows", out dataCount);
            string sql = string.Format(@"Select a.EventID,a.UpTime,a.EventAddress,a.EventX,a.EventY,a.EventPictures,a.EventDesc,a.PersonId, b.PersonName,c.EventFromName,d.DeptName,ET1.EventTypeName as EventType,ET2.EventTypeName as EventContent,f.UrgencyName,g.HandlerLevelName,wo.OperName 
              From M_Event as a 
              left join L_Person as b ON a.PersonId = b.PersonId 
              left join M_EventFrom as c ON a.EventFromId = c.EventFromId 
              left join L_Department as d ON a.DeptId = d.DeptId 
              left join (select EventTypeId, EventTypeName from M_EventType where ParentTypeId = 0) ET1 on a.EventTypeId = ET1.EventTypeId 
              left join (select EventTypeId, EventTypeName from M_EventType where ParentTypeId <> 0) ET2 on a.EventTypeId2 = ET2.EventTypeId 
              left join M_Urgency as f ON a.UrgencyId = f.UrgencyId 
              left join M_HandlerLevel as g ON a.HandlerLevelId = g.HandlerLevelId 
              left join L_Person p on a.DispatchPerson = p.PersonId 
              left join M_WorkOrder mw on mw.EventID = a.EventID and mw.OrderStatus = 0 
              left join (select MAX(OperId) OperId, OrderId from M_WorkOrder_Oper_History Group by OrderId) h ON mw.OrderId = h.OrderId 
              left join M_WorkOrder_Oper as wo on h.OperId = wo.OperId 
              Where a.DeleteStatus = 0 and a.PersonId = {0}", PersonId);
            DataTable dt = APP.SQLServer_Helper.SelectDataTable(sql, out ErrInfo_);
            string sqlStr = APP.MakeSqlByPage(sql, dataSort, Convert.ToInt16(pageIndex), Convert.ToInt16(dataCount));
            DataSet ds = APP.SQLServer_Helper.Query(sqlStr);
            if (string.IsNullOrEmpty(ErrInfo_))
            {
                Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                retStr = retStr.Replace("/*Total*/", ds.Tables[1].Rows[0][0].ToString()).Replace("/*Rows*/", JsonConvert.SerializeObject(ds.Tables[0], timeConverter));
                Context.Response.Write(retStr);
            }
            else
                Context.Response.Write(EasyUI_Pagination.ExportErrMsg(ErrInfo_));
        }
    }
}