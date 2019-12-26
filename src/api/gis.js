import axios from 'axios'
import config from '@config/config.js'
const instance = axios.create({
    baseURL: config.MapRelated.PoiLayer ? config.MapRelated.PoiLayer.url : "",
    timeout: 30000,
});

export default {
    // 获取POI搜索建议   config.MapRelated.PoiLayer.url
    GetPoiSearchSuggestions(keyword) {
        return instance.get('/0/query', {
            params: {
                where: "name like '%" + keyword + "%'",
                f: "pjson"
            }
        })
    }
}
