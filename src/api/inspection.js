import axios from 'axios'
import qs from 'querystring'
import config from '@config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
    baseURL: config.apiPath.inspection,
    timeout: 80000,
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
  巡检API 
**/
export default {
    // 上传当前地理位置
    UploadLocation(longitude, latitude, currentTime, personId, isOnline = 0) {
        return instance.get('/UPCoordinatePosition', {
            params: {
                PositionX: String(longitude),
                PositionY: String(latitude),
                UpTime: String(currentTime),
                PersonId: Number(personId),
                isOnline: Number(isOnline)
            },
            // baseURL: 'localhost:8585/'
        })
    },


    // 编辑阀门状态信息
    UploadValveState(reqData) {
        return instance.get('/UpValveStateLog', {
            params: {
                PersonId: String(reqData.personId),
                Smid: Number(reqData.smid),
                // 开关状态
                ValveOpenState: String(reqData.openStatus),
                // 操作原因
                OperationCause: reqData.operationCause,
                // 操作状况
                OperationCondition: reqData.condition,
                // 备注
                Remark: reqData.remark
            }
        })
    },
    // 签到
    SubmitAttendance(attendanceInfo) {
        return instance.get('/QianDao', {
            params: attendanceInfo
        })
    },
    // 获取考勤记录
    GetAttendanceRecords(conditions) {
        return instance.get('/Get_WorkRecord', {
            params: conditions
        })

    },
    // 获取巡检任务列表
    GetMissionList(userId, currentDayDateString) {
        return instance.get('/Down_Taskplan', {
            params: {
                id: userId,
                CurrentDayDate: currentDayDateString
                // CurrentDayDate: '18-06-30'
            }
        })
    },
    // 获取一个任务的关键点位（设备）信息
    GetMissionPoints(taskId) {
        return instance.get('/GetTaskplanInfo', {
            params: {
                TaskId: taskId
            }
        })
    },
    // 获取事件类型列表
    GetEventTypeList() {
        return instance.get('/Get_EventType')
    },
    // 获取事件内容列表
    GetEventContentList(eventTypeId) {
        return instance.get('/Get_EventTypeDetail', {
            params: {
                ParentTypeId: eventTypeId
            }
        })
    },
    // 获取事件紧急程度列表
    GetEmergencyList() {
        return instance.get('/GetUrgent_Degree')
    },
    // 获取事件处理级别列表
    GetEventLevelList() {
        return instance.get('/GetHandler_Level')
    },
    // 获取事件工单分派列表
    GetEventOrderList() {
        return instance.get('/GetEvent')
    },
    // 事件上报
    SubmitEvent(eventInfo) {
        console.log('api： 事件上报参数集合：', eventInfo)
        return instance.post('/SavEventStart', qs.stringify(eventInfo), {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            }
        })
    }
}
