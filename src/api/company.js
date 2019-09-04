import axios from 'axios'
import config from '../../config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
  baseURL: config.apiPath.company,
  timeout: 30000,
  // 该函数指定响应数据进行的预处理，return的值会填到response.data
  // transformResponse: function (resXmlData) {
  //     // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
  //     let parser = new window.DOMParser()
  //     let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
  //     let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
  //     let parsedResData = JSON.parse(jsonStr)
  //     return parsedResData
  // }
  transformResponse: function (jsonStr) {
    return JSON.parse(jsonStr.replace(/\\/g, '').slice(1, -1))
  }
});

// 监测API
export default {
  getInstance() {
    return instance
  },
  //   获取信息分类
  GetCompanyInfoType() {
    return instance.get('/GetNewsType')
  },
  //   获取信息列表
  GetCompanyInfoList(type = "", beginTime = "", endTime = "", keywords = "", pageNumber = 1, rowsPerPage = 5000) {
    return instance.get('/GetNews', {
      params: {
        RTypeID: type,
        Begintime: beginTime,
        Endtime: endTime,
        RI_Heading: keywords,
        page: pageNumber,
        rows: rowsPerPage
      }
    })
  }
}
