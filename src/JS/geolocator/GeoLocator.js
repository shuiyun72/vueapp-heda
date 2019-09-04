import Vue from 'vue'
import _ from 'lodash'
import BaseMap from "@JS/Map/BaseMap";
import CoordsHelper from 'coordtransform'
import {
  deepCopy,
  calcDistance,
  setSessionItem
} from "@common/util";
import nativeTransfer from "@JS/native/nativeTransfer";

// 开发环境toast的开关
const LOG = false
// 全局事件总线
let eventbus = null
// 默认config，参考 http://www.html5plus.org/doc/zh_cn/geolocation.html#plus.geolocation.PositionOptions
const defaultConfig = {
  // 默认不使用高精度，除非必要，因为其会占用大量资源并使获取位置的时间变久导致定位延迟或获取位置失败
  enableHighAccuracy: true,
  maximumAge: 5000,
  timeout: 5000,
  provider: "baidu",
  coordsType: "gcj02",
  // 经纬度范围，如果在范围之外，则（获取但）不记录该位置点
  extent: {
    longitude: [],
    latitude: []
  }
}

// 最终的config
let computedConfig = {}

// 0: 未初始化 1: 已初始化 2: 运行中 3: 已停止（暂停）
let _state = 0

let _taskId = ''

let _lastPosition = null

let _failedCount = 0

let _coords_per_2s = {}

export default class GeoLocator {
  constructor() { }
  static init(initConfig) {
    //window.plus.device.setWakelock(true)
    // 只有未初始化状态和已销毁的状态下（_state=0）才能init
    if (_state === 0) {
      // 每次start都要将_failedCount清零
      _failedCount = 0
      return new Promise((resolve, reject) => {
        
          // 添加一系列设备相关的事件
          document.addEventListener('pause', () => {
            console.info('App Pause')
          })
          document.addEventListener('resume', () => {
            console.info('App Resume')
            // GeoLocator.restart()
          })
          // 整理配置
          computedConfig = _.assign({}, defaultConfig, initConfig)
          eventbus = Vue.prototype.$eventbus
          if (!eventbus) {
            console.error(`Vue全局总线没有被定义，将影响该模块的功能，请在调用init方法之前，为Vue的原型添加名为 '$eventbus' 的事件总线`)
          }
          _state = 1
          console.log(`%cGeoLocator: 初始化成功， 配置对象为： `, 'color: blue', computedConfig)
          resolve(deepCopy(computedConfig))
        
      })
    } else {
      console.warn(`GeoLocator: GeoLocator在其他地方已被初始化，配置对象为： `, computedConfig)
    }
  }

  static start() {
    if (_state === 1 || _state === 3) {
      console.log('%cGeoLocator: 开启start...', 'color: blue')
      _failedCount = 0
      // 抛弃刚开始的三个点
      let initThreeCount = 0
      let getLocationTimer = null;
      clearInterval(getLocationTimer);
      if (!getLocationTimer) {
        getLocationTimer = setInterval(function () {
          _taskId = nativeTransfer.getLocation(coords => {
            if (coords) {
              setSessionItem("coordsMsg", JSON.stringify(coords));
              // window.mui.toast('拿到位置了')
              if (_state == 2) {
                if (initThreeCount < 3) {
                  // console.log(`%c过滤掉第${initThreeCount+1}个点：`, 'color: blue', coords)
                  initThreeCount++
                  if (initThreeCount === 3) {
                    //   console.log('%c前三个点已过滤，正式开始...', 'color: green')
                  }
                  return
                }
                let timestamp = new Date();



                // 坐标转换+地方投影转换
                let coordsFor84 = CoordsHelper.gcj02towgs84(coords.lng, coords.lat)
                let mapController = new BaseMap();
                mapController.Init("event_map");
                coordsFor84 = mapController.destinationCoordinateProj(
                  [coordsFor84[0], coordsFor84[1]]
                );

                // 组装数据对象
                let position = {
                  longitude: coordsFor84[0],
                  latitude: coordsFor84[1],
                  timestamp
                }
                //  console.log("%cGeoLocator: 获取到一次位置： ", 'color: green', position);
                // 判断位置是否在規定范围之內
                _checkCoordsBounds(position, result => {
                  if (result) {
                    // 比較上個點，計算這次的點是否合理
                    _compareWithLastCoords(position, (result) => {
                      if (result) {
                        _lastPosition = position
                        LOG && window.mui.toast(`经度：${position.longitude}, 纬度：${position.latitude}`, {
                          duration: '1000',
                          type: 'div'
                        })
                        _failedCount = 0
                        eventbus.$emit('geolocation', position)
                      } else {
                        LOG && window.mui.toast('与上次校验时失败。。。')
                        _failedCount++
                        if (_failedCount === 3) {
                          GeoLocator.restart()
                        }
                      }
                    })
                  } else {
                    LOG && window.mui.toast('越界校验时失败。。。')
                    _failedCount++
                    if (_failedCount === 5) {
                      GeoLocator.restart()
                    }
                  }
                })
              }
            } else {
              console.log('%cGeoLocator: 获取位置点出错: ', 'color: red', err)
              // _failedCount++
              // console.log('%cGeoLocator: 失敗次數 ', 'color: red', _failedCount)
              GeoLocator.restart()
            }
          })
        }, 5000)
      }  
      // 改变当前状态，至此往后的值才有效
      _state = 2
      //  console.log('%cGeoLocator: 定时任务已开启', 'color: blue')
    } else if (_state === 0) {
      // console.error('GeoLocator: 调用start之前必须先调用init方法并传入一个自定义配置对象')
    } else if (_state === 2) {
      // console.error(`GeoLocator: _taskId为 ${_taskId} 的定时任务正在进行中，无法重复start`)
    }
  }

  static stop() {
    console.log('%cGeoLocator: 停止stop...', 'color: blue')
    if (_state === 2 && _taskId) {
     // window.plus.geolocation.clearWatch(_taskId)
      _taskId = null
      _lastPosition = null
      _state = 3
      _failedCount = 0
      //console.log('%cGeoLocator: 定时任务已停止', 'color: orange')
    } else {
      console.warn('%cGeoLocator: 定时任务尚不存在，无法（没有必要）执行stop方法', 'color: yellow')
    }
  }

  static restart() {
    // console.log('%cGeoLocator: 准备重启...', 'color: blue')
    LOG && window.mui.toast('准备重启。。。。。。。。')
    GeoLocator.stop()
    setTimeout(() => {
      GeoLocator.start()
    }, 1000)
  }

  static destroy() {
    if (_state === 2) {
      GeoLocator.stop()
    }
    computedConfig = {}
    _state = 0
    _failedCount = 0
  }

  static getCurrentState() {
    return _state
  }
}

// 與上次的點作比較，判斷獲得的點是否合理
function _compareWithLastCoords(position, callback) {
  // 位置点在给定范围内
  if (!_lastPosition || !_lastPosition.longitude || !_lastPosition.latitude) {
    console.log('%c上個點不存在或無效，將本次點填入...', 'color: lightgreen')
    _lastPosition = position
    callback(true)
  } else {
    let distanceForMeter = calcDistance(
      position.longitude,
      position.latitude,
      _lastPosition.longitude,
      _lastPosition.latitude,
      6
    ) * 1000;
    // 最大阀值为15m/秒   //奉化坐标转换后为普通坐标差值得*10000
    if (distanceForMeter <= 2000000) {
      console.log("%cGeoLocator: 位置点合理 %s", 'color: green', distanceForMeter);
      callback(true)
    } else {
      console.log(position.longitude +" " +position.latitude)
      console.warn('GeoLocator: 位置点不合理: ', distanceForMeter)
      callback(false)
    }
  }
}

// 計算點位是否越界
function _checkCoordsBounds(position, callback) {
  let extent = {}
  if (computedConfig.extent.longitude &&
    computedConfig.extent.latitude &&
    computedConfig.extent.longitude.length === 2 &&
    computedConfig.extent.latitude.length === 2) {
    extent = computedConfig.extent
  }
  if (extent && !_.isEmpty(extent)) {
    // 有范围限定
    if (
      position.longitude <= extent.longitude[1] &&
      position.longitude >= extent.longitude[0] &&
      position.latitude <= extent.latitude[1] &&
      position.latitude >= extent.latitude[0]
      // position.longitude > 10 && position.latitude > 10
    ) {
      //console.log("%cGeoLocator: 该位置点有效", 'color: green');
      callback(true)
    } else {
      // console.log("position.longitude",position.longitude,position.latitude)
      // console.log("extent.longitude",extent.longitude[0],"-",extent.longitude[1])
      // console.log("extent.latitude",extent.latitude[0],"-",extent.latitude[1])
      console.warn('GeoLocator: 该位置点越界無效')
      // console.log(position.longitude)
      callback(false)
    }
  } else {
    // 无范围限定
    // console.log("%cGeoLocator: 该位置点有效", 'color: green');
    callback(true)
  }
}
