import axios from 'axios'
import config from '@config/config.js'
import sha1 from 'js-sha1'
// axios.defaults.withCredentials = true
const instance = axios.create({
  baseURL: config.apiPath.user,
  timeout: 10000,
  // 该函数指定响应数据进行的预处理，return的值会填到response.data
  transformResponse: function (resXmlData) {
    // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
    let parser = new window.DOMParser()
    let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
    console.log('xmlDoc', xmlDoc)
    let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
    console.log('jsonStr', jsonStr)
    let parsedResData = JSON.parse(jsonStr)
    return parsedResData
  }
});

export default {
  // 用户登录
  UserLogin(name, pwd, smid) {
    return instance.get('/../Inspection.asmx/User_Check', {
      // 以对象方式写get请求的查询字符串
      params: {
        name,
        pwd,
        smid
      }
    })
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
  //和达账户验证
  SignInWithHdAcc(hdAcc, hdStamp, hdSSOKey) {
    return sha1('HD#@!' + hdAcc + hdStamp) == hdSSOKey
  },
  //和达用户登录
  HdUserLogin(name) {
    return instance.get('/../Inspection.asmx/HdUser_Check', {
      // 以对象方式写get请求的查询字符串
      params: {
        name
      }
    })
  },
}
