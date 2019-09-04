import axios from 'axios'
import qs from 'querystring'
import config from '../../config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
  baseURL: config.apiPath.maintain,
  timeout: 30000,
  // 该函数指定响应数据进行的预处理，return的值会填到response.data
  transformResponse: function (resXmlData) {
    // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
    let parser = new window.DOMParser()
    let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
    let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
    let parsedResData = JSON.parse(jsonStr)
    return parsedResData
  }
});

/**
  养护API 
**/
export default {
  GetInstance() {
    return instance
  },
  // 获取事件工单分派列表
  GetEventOrderList() {
    return instance.get('/GetEvent')
  },

  // 将某一订单置为无效单
  PostInvalidOrder(orderId) {
    return instance.get('/EventInvalid', {
      params: {
        eventId:data.eventId,
        EventId: orderId
      }
    })
  },
  // 获取部门列表
  GetDepartmentList() {
    return instance.get('/GetDep')
  },
  // 获取指定部门人员列表
  GetStaffList(departmentId) {
    return instance.get('/GetDepPerson', {
      params: {
        DeptId: departmentId
      }
    })
  },
  // 工单分派
  PostOrderAssignee(reqData) {
    return instance.get('/SaveWorkOrder', {
      params: reqData
    })

  },
  // 获取维修任务列表
  GetOrderList(userId, orderStatus) {
    return instance.get('/getOrderList', {
      params: {
        PersonId: userId,
        Type: orderStatus
      }
    })
  },
  // 退单
  ChargeBackOrder(data) {
    return instance.get('CommitChargeBack', {
      params: {
        eventId:data.eventId,
        OrderId: String(data.orderId),
        PersonId: String(data.personId),
        describe: data.description
      }
    })
  },
  // 延期
  DelayOrder(data) {
    return instance.get('CommitDelay', {
      params: {
        eventId:data.eventId,
        OrderId: data.orderId,
        complishTime: data.date,
        describe: data.description
      }
    })
  },
  // 接单、到场、维修、完工
  ChangeMissionStatus(data) {
    return instance.post('CommitOrderTask', qs.stringify({
      eventId:data.eventId,
      OrderId: data.orderId,
      PersonId: data.personId,
      describe: data.description,
      EventPictures: data.pictureList[0] || '',
      EventVoices: data.speechData,
      /*
        接单：3,
        到场：4,
        维修：5,
        完成：6
      */
      TaskType: data.operationId
    }), {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      }
    })
  },
  // 获取养护任务列表
  GetConservationList(userId) {
    return instance.get('/GetMainTainTask', {
      params: {
        PersonId: userId
      }
    })
  }
}
