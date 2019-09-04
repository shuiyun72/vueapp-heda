import axios from 'axios'
import config from '../../config/config.js'
import dateHelper from "@common/dateHelper";
import uuid from '@common/uuid'
import qs from 'querystring'

// 一些常量
export const FLOW_ID = {
  // 请假
  QJ: '1DF8810F-7369-49AA-8F7A-392EF77F11AA',
  // 保养
  BY: '3A07049B-06AB-4F2E-ACF6-C6CB5C18D1A7',
  // 加油
  JY: '48CA0DEA-BA2D-4DA7-AE9F-39641C940E46',
}

export const BUTTON_NAME_MAPPING = {
  save: '保存',
  send: '发送',
  complete: '完成'
}

export const STEP_STATUS_MAPPING = [
  '待处理', '处理中', '完成', '退回', '他人已处理', '他人已退回', '终止', '他人已终止'
]

export const FORM_DATA_MAPPING = {
  QJ: {
    O_Title: "title",
    O_Type: "type",
    O_Person: "personName",
    O_DepartMent: "department",
    O_StartDate: "startTime",
    O_EndDate: "endTime",
    O_Days: "day",
    O_Content: "reason"
  },
  BY: {
    Vnumber: "plateNumber",
    Maintaintype: "maintainType",
    Maintainer: "maintainPerson",
    Maintaindate: "maintainDate",
    Costamount: "cost",
    KMamount: "distance",
    Vattachment: "attachment",
    Vcreator: "createPerson",
    Vcreationtime: "createDate",
    Vremarks: "maintainContent",
  },
  JY: {
    Vnumber: "plateNumber",
    VrefuelMan: "gasPerson",
    VrefuelDate: 'gasDate',
    VrefuelTaskJson: "gasType",
    VrefuelCapactity: 'litre',
    VunitPrice: 'price',
    VamountSpent: "cost",
    VkilometerTotal: "distance",
    Venclosure: "attachment",
    Vfounder: "createPerson",
    Vcreationtime: "createDate",
    Vremarks: "comments",
  }
}

export const Default_ID = '00000000-0000-0000-0000-000000000000'

export function getTaskJson() {
  const TASK_JSON = {
    ID: '',
    PrevID: '',
    PrevStepID: '',
    FlowID: '',
    StepID: '',
    StepName: '',
    InstanceID: '',
    GroupID: '',
    Type: 0,
    Title: '',
    SenderID: '',
    SenderName: '',
    SenderTime: '',
    ReceiveID: '',
    ReceiveName: '',
    ReceiveTime: '',
    OpenTime: '',
    CompletedTime: '',
    CompletedTime1: '',
    Comment: '',
    IsSign: '',
    Status: '',
    Note: '',
    Sort: 0,
    SubFlowGroupID: '',
    OtherType: '',
    Files: ''
  }
  return TASK_JSON
}

const instance = axios.create({
  baseURL: config.apiPath.monitor,
  timeout: 30000,
  transformResponse: function (jsonStr) {
    return JSON.parse(jsonStr.replace(/\\/g, '').slice(1, -1))
  }
});

// OA api
export default {
  getInstance() {
    return instance
  },
  // Flow API
  flow: {
    // 获取所有流程元数据
    GetFlowListInfo() {
      return instance.get('/OAFlow/GetWorkFlow', {
        transformResponse: function (jsonStr) {
          return eval(jsonStr)[0]
        }
      })
    },
    // 获取指定流程元数据
    GetFlowInfo(flowId) {
      return instance.get('/OAFlow/GetWorkFlow', {
        params: {
          FlowID: flowId
        },
        transformResponse: function (jsonStr) {
          return JSON.parse(eval(jsonStr))
        }
      })
    },
    // 获取指定用户名下的待办流程列表
    GetOATodoList(userId, pageNumber = 1, rowsPerPage = 5000) {
      return instance.get('/FlowSync/OAFlow/GetFlowWait', {
        params: {
          UserID: userId.toString(),
          page: pageNumber.toString(),
          rows: rowsPerPage.toString()
        }
      })
    },
    // 获取指定用户名下的已办流程列表
    GetOADoneList(userId, pageNumber = 1, rowsPerPage = 5000) {
      return instance.get('/FlowSync/OAFlow/GetFlowComplated', {
        params: {
          UserID: userId,
          page: pageNumber,
          rows: rowsPerPage
        }
      })
    },
    // 部门人员列表
    GetDepartmentAndPersonList() {
      return instance.get('/FlowSync/OAFlow/FlowStepSendPerson')
    }
  },
  // Form API
  form: {
    SubmitQJForm(flowData, formData, operType) {
      let reqData = {
        TaskJson: flowData,
        DataJson: formData,
        OperType: operType
      }
      return instance.post('/FlowSync/OAForm/SaveLeave', qs.stringify(reqData), {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        transformResponse: function (jsonStr) {
          return JSON.parse(JSON.parse(eval(jsonStr)))
        }
      })

    },
    SubmitJYForm(flowData, formData, operType) {
      let reqData = {
        TaskJson: flowData,
        DataJson: formData,
        OperType: operType
      }
      return instance.post('/OAForm/SaveVM_JYSQ', qs.stringify(reqData), {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        transformResponse: function (jsonStr) {
          return JSON.parse(JSON.parse(eval(jsonStr)))
        }
      })
    },
    SubmitBYForm(flowData, formData, operType) {
      let reqData = {
        TaskJson: flowData,
        DataJson: formData,
        OperType: operType
      }
      return instance.post('/OAForm/SaveVM_WXBY', qs.stringify(reqData), {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        transformResponse: function (jsonStr) {
          return JSON.parse(JSON.parse(eval(jsonStr)))
        }
      })
    },
    //获取制定ID的请假单的详细信息
    GetQJDetailById(recordId) {
      return instance.get('/FlowSync/OAForm/Get_Leave', {
        params: {
          ID: recordId
        }
      })
    },
    GetJYDetailById(recordId) {
      return instance.get('/FlowSync/OAForm/Get_JYSQ', {
        params: {
          ID: recordId
        }
      })
    },
    GetBYDetailById(recordId) {
      return instance.get('/FlowSync/OAForm/Get_WXBY', {
        params: {
          ID: recordId
        }
      })
    },
  }
}




let a = {
  "ID": "1df8810f-7369-49aa-8f7a-392ef77f11aa",
  "Name": "请销假",
  "Type": "7bc7c158-3492-41dd-8082-388495edf20c",
  "Manager": "u_00000000-0000-0000-0000-000000000211",
  "InstanceManager": "u_00000000-0000-0000-0000-000000000211",
  "CreateDate": "",
  "CreateUserID": "00000000-0000-0000-0000-000000000211",
  "DesignJSON": {
    "id": "1df8810f-7369-49aa-8f7a-392ef77f11aa",
    "name": "请销假",
    "type": "",
    "manager": "u_00000000-0000-0000-0000-000000000211",
    "instanceManager": "u_00000000-0000-0000-0000-000000000211",
    "removeCompleted": "0",
    "debug": "0",
    "debugUsers": "",
    "note": "",
    "databases": [{
      "link": "06075250-30dc-4d32-bf97-e922cb30fac8",
      "linkName": "平台连接",
      "table": "OA_Leave",
      "primaryKey": "ID"
    }],
    "titleField": {
      "link": "",
      "table": "",
      "field": ""
    },
    "steps": [{
      "id": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "type": "normal",
      "name": "请假申请",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "0",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 58,
        "y": 327,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "0",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 0,
        "showTitle": "发送"
      }, {
        "id": "29b358e1-ad64-4f09-846c-4554ae6b85c4",
        "sort": 1,
        "showTitle": "打印"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 2,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "0",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "type": "normal",
      "name": "分管经理审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 696,
        "y": 22,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 0,
        "showTitle": "过程查看"
      }, {
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 1,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 2,
        "showTitle": "发送"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "type": "normal",
      "name": "部长审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 689,
        "y": 245,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 0,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 1,
        "showTitle": "发送"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "8120d452-52b8-41a7-a350-4484ed872e27",
      "type": "normal",
      "name": "总经理审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 1150,
        "y": 330,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 0,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 1,
        "showTitle": "发送"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "type": "normal",
      "name": "反馈发起人",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "0",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 1550,
        "y": 329,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "954effa8-03b8-461a-aaa8-8727d090dcb9",
        "sort": 0,
        "showTitle": "完成"
      }, {
        "id": "29b358e1-ad64-4f09-846c-4554ae6b85c4",
        "sort": 1,
        "showTitle": "打印"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "0",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }],
    "lines": [{
      "id": "882455db-a1a5-4781-8b61-ef9335a30289",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "customMethod": "",
      "text": "请假大于一周",
      "sql": "O_Person != '折红霞'  and O_Person != '曹燕' and O_Person != '单广田' and O_Person != '郭伟' and  O_Person != '奇峰' and O_Person != '杨小平' and O_Person != '杨莹' and O_Person != '杨娜' and O_Person != '李瑞东' and O_Person != '乔小龙' ",
      "organize": []
    }, {
      "id": "3788e3c9-0ab4-4240-bacf-c282a3cb5c18",
      "from": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "to": "8120d452-52b8-41a7-a350-4484ed872e27",
      "customMethod": "",
      "text": "",
      "sql": "O_Days>=7",
      "organize": []
    }, {
      "id": "d04fa364-5842-4121-99a6-b8a8305c3a4a",
      "text": "",
      "from": "8120d452-52b8-41a7-a350-4484ed872e27",
      "to": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "customMethod": "",
      "sql": "",
      "noaccordMsg": ""
    }, {
      "id": "31464c89-8f48-4879-8a2e-c7470a63b82d",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "8120d452-52b8-41a7-a350-4484ed872e27",
      "customMethod": "",
      "text": "副总经理请假",
      "sql": "",
      "organize": [{
        "khleft": "(",
        "usertype": "1",
        "in1": "0",
        "users": "0",
        "selectorganize": "00000000-0000-0000-0000-000000000019,u_00000000-0000-0000-0000-000000000208,u_00000000-0000-0000-0000-000000000216,u_00000000-0000-0000-0000-000000000217,u_00000000-0000-0000-0000-000000000219,u_00000000-0000-0000-0000-000000000220,u_00000000-0000-0000-0000-000000000271,u_00000000-0000-0000-0000-000000000221,u_00000000-0000-0000-0000-000000000272",
        "tjand": "",
        "khright": ")"
      }]
    }, {
      "id": "74d19e20-35ec-4710-a149-3c08bf6e1581",
      "from": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "to": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "customMethod": "",
      "text": "",
      "sql": "",
      "organize": []
    }, {
      "id": "4a082d72-0286-4082-a8ee-cb475db15a4b",
      "from": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "to": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "customMethod": "",
      "text": "",
      "sql": "O_Days<7",
      "organize": []
    }, {
      "id": "2dc5cf84-1f84-4666-8095-78f189a872cc",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "customMethod": "",
      "text": "",
      "sql": "O_Person='曹燕'or O_Person='单广田' or O_Person='郭伟' or O_Person='奇峰' or O_Person='杨小平' or O_Person='杨莹' or O_Person='杨娜' or O_Person='李瑞东' or O_Person='乔小龙' or O_Person='折红霞'",
      "organize": []
    }]
  },
  "InstallDate": "",
  "InstallUserID": "00000000-0000-0000-0000-000000000248",
  "RunJSON": {
    "id": "1df8810f-7369-49aa-8f7a-392ef77f11aa",
    "name": "请销假",
    "type": "",
    "manager": "u_00000000-0000-0000-0000-000000000211",
    "instanceManager": "u_00000000-0000-0000-0000-000000000211",
    "removeCompleted": "0",
    "debug": "0",
    "debugUsers": "",
    "note": "",
    "databases": [{
      "link": "06075250-30dc-4d32-bf97-e922cb30fac8",
      "linkName": "平台连接",
      "table": "OA_Leave",
      "primaryKey": "ID"
    }],
    "titleField": {
      "link": "",
      "table": "",
      "field": ""
    },
    "steps": [{
      "id": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "type": "normal",
      "name": "请假申请",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "0",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 58,
        "y": 327,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "0",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 0,
        "showTitle": "发送"
      }, {
        "id": "29b358e1-ad64-4f09-846c-4554ae6b85c4",
        "sort": 1,
        "showTitle": "打印"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 2,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "0",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "type": "normal",
      "name": "分管经理审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 696,
        "y": 22,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 0,
        "showTitle": "过程查看"
      }, {
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 1,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 2,
        "showTitle": "发送"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "type": "normal",
      "name": "部长审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 689,
        "y": 245,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 0,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 1,
        "showTitle": "发送"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "8120d452-52b8-41a7-a350-4484ed872e27",
      "type": "normal",
      "name": "总经理审批",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "2",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 1150,
        "y": 330,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "86b7fa6c-891f-4565-9309-81672d3ba80a",
        "sort": 0,
        "showTitle": "退回"
      }, {
        "id": "8982b97c-adba-4a3a-afd9-9a3ef6ff12d8",
        "sort": 1,
        "showTitle": "发送"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }, {
        "id": "3b271f67-0433-4082-ad1a-8df1b967b879",
        "sort": 3,
        "showTitle": "保存"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "1",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "1",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }, {
      "id": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "type": "normal",
      "name": "反馈发起人",
      "opinionDisplay": "1",
      "expiredPrompt": "1",
      "signatureType": "0",
      "workTime": "",
      "limitTime": "",
      "otherTime": "",
      "archives": "0",
      "archivesParams": "",
      "note": "",
      "position": {
        "x": 1550,
        "y": 329,
        "width": 108,
        "height": 50
      },
      "countersignature": 0,
      "sendShowMsg": "",
      "backShowMsg": "",
      "behavior": {
        "flowType": "1",
        "runSelect": "1",
        "handlerType": "0",
        "selectRange": "",
        "handlerStep": "",
        "valueField": "",
        "defaultHandler": "",
        "hanlderModel": "0",
        "backModel": "1",
        "backType": "0",
        "backStep": "",
        "percentage": "",
        "countersignature": "0",
        "copyFor": "",
        "concurrentModel": "0",
        "countersignaturePercentage": "",
        "defaultHandlerSqlOrMethod": ""
      },
      "forms": [{
        "id": "11b09720-4ce0-4111-bb98-f414a3078b05",
        "name": "",
        "type": "626480b3-eaa9-4705-acbb-82901db4fda4",
        "srot": 0
      }],
      "buttons": [{
        "id": "954effa8-03b8-461a-aaa8-8727d090dcb9",
        "sort": 0,
        "showTitle": "完成"
      }, {
        "id": "29b358e1-ad64-4f09-846c-4554ae6b85c4",
        "sort": 1,
        "showTitle": "打印"
      }, {
        "id": "b8a7af17-7ad5-4699-b679-d421691dd737",
        "sort": 2,
        "showTitle": "过程查看"
      }],
      "fieldStatus": [{
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.ID",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_SerialNO",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Title",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Type",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Person",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_DepartMent",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_StartDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_EndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Days",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_Content",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_AEndDate",
        "status": "0",
        "check": "0"
      }, {
        "field": "06075250-30dc-4d32-bf97-e922cb30fac8.OA_Leave.O_ADays",
        "status": "0",
        "check": "0"
      }],
      "event": {
        "submitBefore": "",
        "submitAfter": "",
        "backBefore": "",
        "backAfter": ""
      }
    }],
    "lines": [{
      "id": "882455db-a1a5-4781-8b61-ef9335a30289",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "customMethod": "",
      "text": "请假大于一周",
      "sql": "O_Person != '折红霞'  and O_Person != '曹燕' and O_Person != '单广田' and O_Person != '郭伟' and  O_Person != '奇峰' and O_Person != '杨小平' and O_Person != '杨莹' and O_Person != '杨娜' and O_Person != '李瑞东' and O_Person != '乔小龙' ",
      "organize": []
    }, {
      "id": "3788e3c9-0ab4-4240-bacf-c282a3cb5c18",
      "from": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "to": "8120d452-52b8-41a7-a350-4484ed872e27",
      "customMethod": "",
      "text": "",
      "sql": "O_Days>=7",
      "organize": []
    }, {
      "id": "d04fa364-5842-4121-99a6-b8a8305c3a4a",
      "text": "",
      "from": "8120d452-52b8-41a7-a350-4484ed872e27",
      "to": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "customMethod": "",
      "sql": "",
      "noaccordMsg": ""
    }, {
      "id": "31464c89-8f48-4879-8a2e-c7470a63b82d",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "8120d452-52b8-41a7-a350-4484ed872e27",
      "customMethod": "",
      "text": "副总经理请假",
      "sql": "",
      "organize": [{
        "khleft": "(",
        "usertype": "1",
        "in1": "0",
        "users": "0",
        "selectorganize": "00000000-0000-0000-0000-000000000019,u_00000000-0000-0000-0000-000000000208,u_00000000-0000-0000-0000-000000000216,u_00000000-0000-0000-0000-000000000217,u_00000000-0000-0000-0000-000000000219,u_00000000-0000-0000-0000-000000000220,u_00000000-0000-0000-0000-000000000271,u_00000000-0000-0000-0000-000000000221,u_00000000-0000-0000-0000-000000000272",
        "tjand": "",
        "khright": ")"
      }]
    }, {
      "id": "74d19e20-35ec-4710-a149-3c08bf6e1581",
      "from": "8567aa3c-bda5-4dcf-8280-7d169e6768e5",
      "to": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "customMethod": "",
      "text": "",
      "sql": "",
      "organize": []
    }, {
      "id": "4a082d72-0286-4082-a8ee-cb475db15a4b",
      "from": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "to": "7481649d-ba19-4ef9-a8c8-6cc06711478f",
      "customMethod": "",
      "text": "",
      "sql": "O_Days<7",
      "organize": []
    }, {
      "id": "2dc5cf84-1f84-4666-8095-78f189a872cc",
      "from": "7d3d59d5-9766-4beb-af11-f11e805aab62",
      "to": "907de5ac-3058-4592-8737-c4c9d86481eb",
      "customMethod": "",
      "text": "",
      "sql": "O_Person='曹燕'or O_Person='单广田' or O_Person='郭伟' or O_Person='奇峰' or O_Person='杨小平' or O_Person='杨莹' or O_Person='杨娜' or O_Person='李瑞东' or O_Person='乔小龙' or O_Person='折红霞'",
      "organize": []
    }]
  },
  "Status": "2"
}
