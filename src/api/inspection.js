// 配置API接口地址
var rootURL = process.env.API_ROOT
import axios from 'axios'
import qs from 'querystring'
//import config from '@config/config.js'
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
  巡检API 
**/
export default {
    // 上传当前地理位置
    UploadLocation(longitude, latitude, currentTime, personId, isOnline = 0) {
        return instance.post('/CurrentPosition/Post?positionX='+longitude
        +'&positionY='+latitude
        +'&upTime='+currentTime
        +'&personId='+personId
        +'&isOnline='+isOnline)
    },
    /**
     *  获取巡检任务列表
     * userId, (巡检员ID)
     * currentDayDateString,(当天的时间)
     */
    
    GetMissionList(userId, currentDayDateString) {
        return instance.get('/InspectionPlan/TaskManage/GetPlanManageInfo', {
            params: {
                iAdminID: userId,
                startTime: currentDayDateString,
                endTime:currentDayDateString
                // CurrentDayDate: '18-06-30'
            }
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
    // /AttendanceRecord/QianDao?Lwr_PersonId=1266&Lwr_XY=123%2C33&DeptId=19
    // 签到
    SubmitAttendance(attendanceInfo) {
        return instance.post('/AttendanceRecord/QianDao?Lwr_PersonId='+attendanceInfo.Lwr_PersonId
            +'&Lwr_XY='+attendanceInfo.Lwr_XY
            +'&DeptId='+attendanceInfo.DeptId
        )
    },
    // 获取考勤记录
    GetAttendanceRecords(conditions) {
        return instance.get('/AttendanceRecord/Get', {
            params: conditions
        })
    },

    // 获取一个任务的关键点位（设备）信息
    GetMissionPoints(taskId) {
        return instance.get('/InspectionPlan/TaskManage/GetTaskplanInfo', {
            params: {
                taskId: taskId
            }
        })
    },

    /**
     * http://47.104.3.68:9819/api/InspectionPlan/TaskManage/GetTaskplanInfo?taskId=1307  的返回值中
     * 
     * res.data.Data.Result.ImportPointData 中的 PatroState   (1:关键点)
     * res.data.Data.Result.EquPointData  中的 PatroState    (2:点位)
     * 
     * 改变一个任务的关键点位状态
     * 关键点的类型 pointType  (1:关键点,2:点位)
     * 关键点Id   pointSmid  
     */  
    TaskPointState(pointType,pointSmid) {
        return instance.post('/InspectionPlan/TaskManage/PointState',
	    {
            pointType: Number(pointType),
            pointSmid:Number(pointSmid)
        })
    },


    // 获取事件类型列表
    GetEventTypeList() {
        return instance.get('/EventType/GetCommboboxList')
    },
    // 获取事件内容列表
    GetEventContentList(eventTypeId) {
        return instance.get('/EventContent/GetByEventTypeId', {
            params: {
                eventTypeId: eventTypeId,
                sort:"EventTypeName",
                ordering:"desc",
                num:50,
                page:1
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
    SubmitEvent(eventInfo,imgList) {
       // return instance.post('/EventManage/Post?'+qs.stringify(eventInfo))
       imgList = imgList.length > 0 ? imgList.split("$") : [] ;
        return instance({
            method: 'post',
            url: '/EventManage/Post',
            params: eventInfo,
            data:imgList
        })
    },
    //巡检到位点
    PostTaskEqument(taskId,devicename,devicesmid,x,y,personId,equType){
        equType = equType ? ( '&equType=' + equType ) : "";
        return instance.post('/InspectionPlan/TaskManage/PostTaskEqument?taskId=' + taskId + '&devicename=' + devicename + '&devicesmid=' + devicesmid + '&x=' + x + '&y=' + y + '&personId=' + personId + equType)
    }
}
