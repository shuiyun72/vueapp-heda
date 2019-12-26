import axios from 'axios'
import config from '@config/config.js'
const instance = axios.create({
    baseURL: config.MapRelated.PipeLayer.url,
    timeout: 30000,
});

export default {
    //获取全部设备列表
    GetLayers() {
        return instance.get("/legend?f=pjson");
    },
    //获取单个设备信息
    GetPipeLayer(features) {
        return instance.get(features);
    }
   
}
