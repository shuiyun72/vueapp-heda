import axios from 'axios'
import qs from 'querystring'
import config from '../../config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
    baseURL: config.apiPath.gis,
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
});

export default {
    FetchPipeData(condition, recordPerPage = 1000, pageIndex = 1) {
        return instance.get('/', {
            params: {
                page: pageIndex,
                rows: recordPerPage,
                condition: condition + '$',
            }
        })
    },

    // 获取POI搜索建议
    GetPoiSearchSuggestions(keyword) {
        let data = {
            'poi_v': keyword
        }
        return instance.post('/SearchForPOI', qs.stringify(data), {
            baseURL: 'http://218.0.0.33:9923/asmx/GIS.asmx/',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8'
            },
            // 该函数指定响应数据进行的预处理，return的值会填到response.data
            transformResponse(resXmlData) {
                // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
                let parser = new window.DOMParser()
                let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
                let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
                let parsedResData = JSON.parse(jsonStr)
                return parsedResData
            }
        })
    }
}
