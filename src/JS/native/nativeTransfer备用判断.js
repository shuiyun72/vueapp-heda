import MobileDetect from 'mobile-detect'
const _md = new MobileDetect(window.navigator.userAgent);
const _cache = {}
const _cache2 = {}

function _postMessage(methodName, callBack, params) {
  if(window.plus){
    if(methodName == 'getLocation'){
      return _positionPlus(methodName,callBack)
    }
    if(methodName == 'startNavi'){
      return _openSysMap(methodName, callBack, params)
    }
  }else{
    let os = _md.os();
    let postEntity = _creatPostEntity(methodName, callBack, params)
    let result = []
    if (os) {
      if (os == 'AndroidOS') {
        result = window.native.postMessage(JSON.stringify(postEntity))
      } else
      if (os == 'iOS') {
        result = window.webkit.messageHandlers.postMessage.postMessage(JSON.stringify(postEntity))
      } else {
        result = "";
      }
      return result
    }
  } 
}


function _creatPostEntity(methodName, callBack, params = {}) {
  let msgid = new Date().getTime() + parseInt(Math.random() * Math.pow(10, 3));
  _cache[msgid] = callBack
  return {
    msgid: msgid,
    method: methodName,
    params: params
  }
}
window.response = obj => {
  // 原生回调传入一个json对象
  const {
    msgid,
    params,
    method
  } = obj
  if (method == "search" || method == "collect") { // 原生主动调用web
    _cache2[method] = params
    return
  }

  const cb = _cache[msgid]
  cb && cb(params)
  delete _cache[msgid]
}

window.response2 = ({
  method,
  cb
}) => {
  if (!method || !cb) return
  Object.defineProperty(_cache2, method, {
    configurable: true,
    enumerable: true,
    set(val) {
      if (val) {
        cb(val)
        delete _cache[method]
      }
    }
  })
}

//获取当前位置信息
function _positionPlus(type,callback){
    window.plus.geolocation.getCurrentPosition(
      location => {
        let newAddress =location.address.city
            + location.address.district
            + location.address.street;
        let nowLocation =  {
          "lat": location.coords.latitude,   //纬度，double类型
          "lng": location.coords.longitude,   //经度，double类型
          "addr": newAddress || '未定义地点' //地址信息
        }
        callback instanceof Function && callback(nowLocation)  
      },
      err => {
        mui.toast("获取当前位置失败");
      },
      {
        enableHighAccuracy: true,
        maximumAge: 5000,
        timeout: 10000,
        provider: "baidu",
        coordsType: "gcj02"
      }
    );
}

//从当前坐标点到指定位置导航
function _openSysMap(type,callback,params){
  window.plus.geolocation.getCurrentPosition(
    position => {
      let srcPoint = new plus.maps.Point(
        position.coords.longitude,
        position.coords.latitude
      );
      let destDesc = "目标设备";
      let destPoint = new plus.maps.Point(
        Number(params.lng),
        Number(params.lat)
      );
      window.plus.maps.openSysMap(destPoint, destDesc, srcPoint);
    },
    err => {
      window.mui.toast("定位失败，无法调起导航");
    },
    {
      enableHighAccuracy: true,
      maximumAge: 10000,
      provider: "system",
      coordsType: "wgs84"
    }
  );
}

export default {
  /**
   * 获取当前位置
   * @param {返回值} callback 
   */
  getLocation(callback) {
      return _postMessage('getLocation', callback)   
  },
  /**
   * 调用地图服务
   * @param {经度} lng 
   * @param {纬度} lat 
   * @param {地址} addr 
   * @param {返回值} callback 
   */
  startNavi(lng, lat, addr, callback) {
      return _postMessage('startNavi', callback, {
        "lat": lat, //纬度，double类型
        "lng": lng, //经度，double类型
        "addr": addr //站点名称，地址信息等
      })
  }
}
