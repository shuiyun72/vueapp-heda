
// 配置API接口地址
var rootURL = process.env.API_ROOT
import axios from 'axios'
import config from '@config/config.js'
// axios.defaults.withCredentials = true

const instance = axios.create({
  baseURL: rootURL+'/api',
  //解决跨域
  crossDomain:true,
  timeout: 10000,
  //转换res为json
  responseType: 'json'
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
  // 检查移动端版本更新
  CheckAppUpdate() {
    return instance.get('/CellphoneManage/GetLatestVersionId')
  },
  // 用户登录 
  UserLogin(loginContent, password, smid,type = 3) {
    //return instance.post('/System/Login?loginContent='+loginContent+"&password="+password+"$smid="+smid)
    return instance.post('/System/Login?loginContent='+loginContent+"&password="+password+"&systemType="+type)
  },

  // 用户权限
  GetUserPermission(id) {
    return instance('/GetAdminPurviewInfo', {
      params: {
        iAdminID: id
      }
    })
  },

  // 用户个人信息数据
  GetUserInfo(id) {
    return instance('/GetAdminInfo', {
      params: {
        iAdminID: id
      }
    })
  },

  // 所有部门数据 accountCenter页面
  GetDepartment() {
    return instance('/Department/GetUserComboboxList')
  },

  //和达账户验证
  SignInWithHdAcc(hdAcc, hdStamp, hdSSOKey) {
   // return sha1('HD#@!' + hdAcc + hdStamp) == hdSSOKey
    return instance.post('/System/HDLogin?hdAcc='+hdAcc
    +'&hdStamp='+hdStamp
    +'&hdSSOKey='+hdSSOKey
    +'&systemType=3')
  }

}
