// 配置API接口地址
var rootURL = process.env.API_ROOT
import axios from 'axios'
import qs from 'querystring'
import config from '@config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
    baseURL: rootURL+'/api',
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
/**
  养护API 
**/
export default {

   // 获取部门列表
   GetDepartmentList() {
    return instance.get('/Department/GetUserComboboxList')
  },

  // 获取指定部门人员列表
  GetStaffList(departmentId) {
      return instance.get('/User/GetUserComboboxList', {
      params: {
          deptId: departmentId
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

  // 个人工单列表
  GetOrderList(personId, status) {
    // 待办0 已办1
    if (status === 0) {
      return instance.get('/EventManageForMaintain/Get', {
        params: {
          page: 1,
          num: 4000,
          sort: 'EventID',
          ordering: 'desc',
          OperId:11,
          ExecPersonId:personId
        }
      })
    } else if (status === 1) {
      return instance.get('/EventManageForMaintain/GetEventListOwn', {
        params: {
          OwnID: personId,
          page: 1,
          rows: 4000,
          sort: 'EventID',
          ordering: 'desc' 
        }
      })
    }
  },
  
  /**
   * 获取维修工单列表
   * @param {执行人} ExecPersonId 
   * @param status 
   * { 事件状态0:无效 1: 待处理 11:待处理 2:待接受 3:待处置 4:处置中5:延期确认 6:待审核 7:审核完成  12:回复完成 null:待分派}
   */
  GetEventManage(personId,status){
    return instance.get('/EventManageForMaintain/Get', {
      params: {
        ExecPersonId: personId,
        OperId:status,
        page: 1,
        rows: 4000,
        sort: 'EventID',
        order: 'desc'
      }
    })
  },

  /**
   * 接单、到场、维修、完工
   * @param { 事件ID } EventID 
   * @param { 接单：3，到场：4, 维修：5,完成：6 } StepNum 
   * @param { 工单编号 } OrderId
   * @param { 指派人ID } DispatchPersonID 
   * @param { 操作建议 } OperRemarks 
   */
  ChangeMissionStatus(reqData,imgList) {
    let EventID = reqData.EventID;
    let OrderId = reqData.OrderId;
    let StepNum = reqData.StepNum;
    let DispatchPersonID = reqData.DispatchPersonID;
    let OperRemarks = reqData.OperRemarks; 
    let ExecPersonId = reqData.ExecPersonId;
    let ExecDetpID = reqData.ExecDetpID;
    imgList = imgList.length > 0 ? imgList.split("$") : [] ;
    console.log("imgList-------------------",imgList)
    if(StepNum == 3 || StepNum == "3"){  // 接单
      return instance({
        method: 'post',
        url: '/EventManageForMaintain/WorkListReceipt?EventID='+EventID
        +'&ExecPersonId='+ExecPersonId
        +'&ExecDetpID='+ExecDetpID
        +'&OrderId='+OrderId
        +'&StepNum='+StepNum,
        data:[]
      })
    }else
    if(StepNum == 4 || StepNum == "4"){ // 到场
      return instance({
        method: 'post',
        url: '/EventManageForMaintain/WorkListPresent?EventID='+EventID
        +'&ExecPersonId='+ExecPersonId
        +'&ExecDetpID='+ExecDetpID
        +'&OrderId='+OrderId
        +'&StepNum='+StepNum
        +'&DispatchPersonID='+DispatchPersonID
        +'&OperRemarks='+OperRemarks,
        data:imgList
      })
    }else
    if(StepNum == 5 || StepNum == "5"){ //处置
      return instance({
        method: 'post',
        url: '/EventManageForMaintain/WorkListChuZhi?EventID='+EventID
        +'&ExecPersonId='+ExecPersonId
        +'&ExecDetpID='+ExecDetpID
        +'&OrderId='+OrderId
        +'&StepNum='+StepNum
        +'&DispatchPersonID='+DispatchPersonID
        +'&OperRemarks='+OperRemarks,
        data:imgList
      })
    }
    else
    if(StepNum == 6 || StepNum == "6"){ //完工  iAdminID登录人ID
      return instance({
        method: 'post',
        url: '/EventManageForMaintain/WorkListFinished?EventID='+EventID
        +'&ExecPersonId='+ExecPersonId
        +'&ExecDetpID='+ExecDetpID
        +'&OrderId='+OrderId
        +'&StepNum='+StepNum
        +'&iAdminID='+DispatchPersonID
        +'&OperRemarks='+OperRemarks,
        data:imgList
      })
      ///EventManageForMaintain/WorkListFinished?EventID=1&OrderId=1
    }
  },

  // 延期
  DelayOrder(reqData) {
    let EventID = reqData.EventID;
    let OrderId = reqData.OrderId;
   // let StepNum = reqData.StepNum;
    let iAdminID = reqData.iAdminID;
    let OperRemarks = reqData.OperRemarks;
    let PersonId = reqData.PersonId;
    let DeptId = reqData.DeptId;
    let complishTime = reqData.complishTime;
    return instance.post('/EventManageForMaintain/WordListDelay?EventID='+EventID
    +'&OrderId='+OrderId
    +'&OperRemarks='+OperRemarks
    +'&complishTime='+complishTime
    +'&PersonId='+PersonId
    +'&DeptId='+DeptId
    +'&iAdminID='+iAdminID)
  },

  /**
   * 退单
   * @param {事件ID} EventID 
   * @param {工单编号ID} OrderId 
   * @param {登录人员ID} iAdminID 
   * @param {退单备注} BackDesc 
   * @param {登录人员部门ID} iDeptID 
   */
  ChargeBackOrder(reqData) {
    console.log("reqData",reqData)
    let EventID = reqData.EventID;
    let OrderId = reqData.OrderId;
   // let StepNum = reqData.StepNum;
    let iAdminID = reqData.iAdminID;
    let DispatchPersonID = reqData.DispatchPersonID;
    let OperRemarks = reqData.OperRemarks;
    return instance.post(
      '/EventManageForMaintain/WordListBackExec?EventID='+EventID
      +'&OrderId='+OrderId
      +'&iAdminID='+iAdminID
      +'&BackDesc='+OperRemarks
      +'&iDeptID='+DispatchPersonID)
  },
  
  /**
   * 工单退回
   * @param {事件ID} EventID 
   * @param {登录人员ID} iAdminID 
   * @param {退回备注} BackDesc 
   * @param {事件上报人} PersonId 
   * @param {事件所属部门ID} DeptId 
   */
  RejectOrder(EventID, iAdminID, BackDesc,PersonId,DeptId) {
    return instance.post('/EventManageForMaintain/WorkListBackToOper?EventID='+EventID
    +'&iAdminID='+iAdminID
    +'&BackDesc='+BackDesc
    +'&PersonId='+PersonId
    +'&DeptId='+DeptId)
  },


  // 获取事件工单分派列表  -------------------------暂时未用
  GetEventOrderList() {
    return instance.get('/GetEvent')
  },

  // 将某一订单置为无效单
  PostInvalidOrder(orderId) {
    return instance.get('/EventInvalid', {
      params: {
        EventId: orderId
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
  },

  // 根据EventID获取事件的详情
  GetEventDetailInfo(eventId) {
    return instance.get('/EventManageForMaintain/GetEventWorkorderStepForMaintain', {
      params: {
        EventID: eventId
      }
    })
  },


  /**
   * 工单分派
   * @param {事件ID} EventID 
   * @param {执行人部门ID} ExecDetpID 
   * @param {执行人ID} ExecPersonId 
   * @param {指派人ID} DispatchPersonID 
   */
  // PostOrderAssignee(EventID,ExecDetpID,ExecPersonId,DispatchPersonID) {
  //   return instance.post('/EventManageForMaintain/WorkListReAssign?EventID='+EventID
  //   +'&ExecDetpID='+ExecDetpID
  //   +'&ExecPersonId='+ExecPersonId
  //   +'&DispatchPersonID='+DispatchPersonID+
  //   +'&ExecTime=36')
  // },


  /**
   * 工单分派
   * @param {分派人ID} iAdminID 
   * @param {事件ID} eventId 
   * @param {执行人部门} deptId 
   * @param {执行人ID} personId 
   */

  AssignOrder(iAdminID,eventId,deptId, personId) {
    return instance.post('/EventManageForMaintain/WorkListAssign?EventID='+eventId
    +'&ExecDetpID='+deptId
    +'&ExecPersonId='+personId
    +'&DispatchPersonID='+iAdminID
    +'&ExecTime=36')
  },

  PostReplyMessage(eventId, orderId, personId,OperRemarks) {
    orderId = orderId || ""
    return instance.post(' /EventManageForMaintain/WorkListEventReply?EventID='+eventId
    +'&OrderId='+orderId
    +'&DispatchPersonID='+personId
    +'&OperRemarks='+OperRemarks
    )
  },

  // 审核工单 iAdminID,eventId,deptId, personId
  CheckOrder(eventId, orderId, iDeptID, iAdminID,OperRemarks,satisfaction) {
    OperRemarks = OperRemarks || "满意"
    satisfaction = satisfaction || "满意"
    return instance.post('/EventManageForMaintain/WorkListAudit?EventID='+eventId
    +'&OrderId='+orderId
    +'&iDetpID='+iDeptID
    +'&OperRemarks='+OperRemarks
    +'&satisfaction='+satisfaction
    +'&StepNum=7'
    +'&iAdminID='+iAdminID)
  },
  
  // 延期审核确认
  CheckOrderDelay(eventId, orderId, iDeptID, iAdminID,OperRemarks,complishTime) {
    OperRemarks = OperRemarks || "满意"
    console.log(eventId, orderId, iDeptID, iAdminID,OperRemarks,complishTime)
   return instance.post('/EventManageForMaintain/WorkListDelayExec?EventID='+eventId
   +'&OrderId='+orderId
   +'&iDeptID='+iDeptID
   +'&OperRemarks='+OperRemarks
   +'&complishTime='+complishTime
   +'&iAdminID='+iAdminID)
  }

}
