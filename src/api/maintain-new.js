import axios from 'axios'
import qs from 'querystring'
import config from '@config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
    baseURL: config.apiPath.inspection,
    //解决跨域
    crossDomain:true,
    timeout: 30000,
    //转换res为json
    responseType: 'json',
});

// request拦截器
instance.interceptors.request.use(
    config => {
        // 每次发送请求之前检测都vuex存有token,那么都要放在请求头发送给服务器
        if (1) {
          config.headers.Token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJVc2VySWQiOjEsIlVzZXJOYW1lIjoiYWRtaW4iLCJFeHBpcmVUaW1lIjoiMjAxOS0wNS0yOFQxMDowMTo0MC41NjYyMjI0KzA4OjAwIiwiSVAiOiIifQ.WCPR9mXenLrizGVGITHWWG4-PybJ9BK34pTnDclUxSQ'
        }
    
        return config
    },
    err => {
    return Promise.reject(err)
    }
)

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
  AssignOrder(personId, eventId, deptId) {
    return instance.get('/WorkList.ashx', {
      params: {
        Oper: 'WordListAssignForAPP',
        iAdminID: personId,
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
