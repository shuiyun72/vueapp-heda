import axios from 'axios'
import config from '@config/config.js'
// axios.defaults.withCredentials = true

const instance = axios.create({
  baseURL: config.apiPath.user,
  //解决跨域
  crossDomain:true,
  timeout: 10000,
  //转换res为json
  responseType: 'json',
  transformRequest:[function(data){
    return data
  }]
});

export default {
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
  }
}
