import MobileDetect from 'mobile-detect'
const _md = new MobileDetect(window.navigator.userAgent);
const _cache = {}
const _cache2 = {}

function _postMessage(methodName, callBack, params) {

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
