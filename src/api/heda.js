import axios from 'axios'
import qs from 'qs';
/*const instance = axios.create({
  baseURL:'http://www.htboy.cn',
  ws: true,
  // //解决跨域
   crossDomain:true,
  timeout: 30000,
 

});*/
// instance.setHeader("Access-Control-Allow-Origin", "*");
// //告诉浏览器编码方式  
// instance.setHeader("Content-Type","text/html;charset=UTF-8" ); 
export default {
  accessHeda() {
      let data = {
        "grant_type": "client_credentials",
        "client_id":"5d90149e6951c227849c84a1",
        "client_secret":"321b7c2888bdbcd750b25cfcb3c4cbea327c228a27091b93dc4e9639fc94d45b"
      }
      return axios.post('/apii/hdl/oauth/v1.0/access.json', qs.stringify(data), {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
          'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8'
        }
      })
  },
}
