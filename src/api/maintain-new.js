import axios from 'axios'
import qs from 'querystring'
import config from '../../config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
  baseURL: config.apiPath.maintainNew,
  timeout: 30000,
  // 该函数指定响应数据进行的预处理，return的值会填到response.data
  //   transformResponse: function (resXmlData) {
  //     // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
  //     let parser = new window.DOMParser()
  //     let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
  //     let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
  //     let parsedResData = JSON.parse(jsonStr)
  //     return parsedResData
  //   }
});

export default {
  GetInstance() {
    return instance
  },
  // 获取事件工单分派列表
  GetEventSourceList() {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'GetStartEventSourceInfo'
      }
    })
  },
  // 事件上报
  SubmitEvent(eventInfo) {
    return instance.post('/WorkList.ashx?Oper=GetStartEventSourceInfo', qs.stringify(eventInfo), {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      }
    })
  },
  // 个人事件待办列表
  GetOrderList(personId, status) {
    // 待办0 已办1
    if (status === 0) {
      return instance.get('/EventInfo.ashx', {
        params: {
          Oper: 'GetAllEventInfoBySql',
          ExecPersonId: personId,
          page: 1,
          rows: 4000,
          sort: 'EventID desc',
          order: 'desc'
        }
      })
    } else if (status === 1) {
      return instance.get('/WorkList.ashx', {
        params: {
          Oper: 'EventListOwn',
          OwnID: personId,
          page: 1,
          rows: 4000,
          sort: 'EventID desc',
          order: 'desc'
        }
      })
    }
  },
  // 根据EventID获取事件的详情
  GetEventDetailInfo(eventId) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'GetEventInfo',
        EventID: eventId
      }
    })
  },
  // 分派工单
  AssignOrder(iAdminId, eventId, deptId,personId) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'WordListAssignForAPP',
        iAdminID: iAdminId,
        EventID: eventId,
        PersonId: personId,
        DeptId: deptId,
      }
    })
  },
  PostReplyMessage(msg, personId, eventId) {
    return instance.get('/EventInfo.ashx', {
      params: {
        Oper: 'EventReply',
        OperRemarks: msg,
        DispatchPersonID: personId,
        EventID: eventId
      }
    })
  },
  // 工单退回
  RejectOrder(desc, personId, eventId) {
    return instance.get('/EventInfo.ashx', {
      params: {
        Oper: 'BackToOper',
        iAdminID: personId,
        EventID: eventId,
        BackDesc: desc
      }
    })
  },
  // 审核工单
  CheckOrder(personId, eventId, orderId, operId) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'CommitOrderStepSet',
        EventID: eventId,
        OrderId: orderId,
        StepNum: operId + 1,
        iAdminID: personId
      }
    })
  },
  // 获取延期申请信息
  GetDelayInfo(eventId, orderId) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'GetWordListDelayInfo',
        EventID: eventId,
        OrderId: orderId
      }
    })
  },
  // 延期审核确认
  CheckOrderDelay(personId, eventId, orderId, complishTime) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'WordListDelayExec',
        EventID: eventId,
        OrderId: orderId,
        complishTime: complishTime,
        iAdminID: personId,
      }
    })
  }
}
