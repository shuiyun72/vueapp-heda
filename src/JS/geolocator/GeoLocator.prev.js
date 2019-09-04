import Vue from 'vue'
import _ from 'lodash'
import BaseMap from "@JS/Map/BaseMap";
import CoordsHelper from 'coordtransform'
import nativeTransfer from "@JS/native/nativeTransfer";
import {
    calcDistance,
    deepCopy,
} from "@common/util";
// 全局事件总线
let eventbus = null
// 默认config，参考 http://www.html5plus.org/doc/zh_cn/geolocation.html#plus.geolocation.PositionOptions
const defaultConfig = {
    // 默认不使用高精度，除非必要，因为其会占用大量资源并使获取位置的时间变久导致定位延迟或获取位置失败
    enableHighAccuracy: true,
    /* 
       根据html5+api文档，maximumAge参数只在使用plus.watchPosition方法时生效，
       本模块没有使用watchPosition方法，
       而是使用setInterval+plus.getCurrentPosition方法周期性获取位置
       因此本模块使用一个自定义的interval参数表示定时任务的周期
    */
    maximumAge: 5000,
    interval: 2000,
    provider: "baidu",
    coordsType: "gcj02",
    timeout: 8000,
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

export default class GeoLocator {
    constructor() { }

    static init(initConfig) {
        if (_state === 0) {
            _failedCount = 0
            return new Promise((resolve, reject) => {
                //if (window.plus && window.plus.geolocation) {
                    // 添加一系列设备相关的事件
                    document.addEventListener('pause', () => {
                        window.mui.toast('Pause')
                        console.info('App Pause')
                    })
                    document.addEventListener('resume', () => {
                        window.mui.toast('Resume')
                        console.info('App Resume')
                        GeoLocator.restart()
                    })
                    // 整理配置
                    computedConfig = _.assign({}, defaultConfig, initConfig)
                    eventbus = Vue.prototype.$eventbus
                    if (!eventbus) {
                        console.error(`Vue全局总线没有被定义，将影响该模块的功能，请在调用init方法之前，为Vue的原型添加名为 '$eventbus' 的事件总线`)
                    }
                    _state = 1
                    console.log(`GeoLocator: 初始化成功， 配置对象为： `, computedConfig)
                    resolve(JSON.parse(JSON.stringify(computedConfig)))
                /*} else {
                    reject(new Error('GeoLocator: 不在html5+运行环境，初始化失败。'))
                }*/
            })
        } else {
            console.warn(`GeoLocator: GeoLocator在其他地方已被初始化，配置对象为： `, computedConfig)
        }
    }

    static start() {
        if (_state === 1 || _state === 3) {
            _failedCount = 0
            _taskId = window.setInterval(() => {

                nativeTransfer.getLocation(coords => {
                    if (coords) {
                        // 坐标转换+投影转换
                        let coordsFor84 = CoordsHelper.gcj02towgs84(coords.lng, coords.lat)
                        let mapController = new BaseMap();
                        mapController.Init("event_map");
                        coordsFor84 = mapController.destinationCoordinateProj(
                            [coordsFor84[0], coordsFor84[1]]
                        );

                        // 组装数据对象
                        let timestamp = new Date();
                        let position = {
                            longitude: coordsFor84[0],
                            latitude: coordsFor84[1],
                            timestamp
                        }
                        console.log("%cGeoLocator: 获取到一次位置： ", 'color: green', position);
                        // 判断位置是否超过约定范围
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
                                // position.longitude <= extent.longitude[1] &&
                                // position.longitude >= extent.longitude[0] &&
                                // position.latitude <= extent.latitude[1] &&
                                // position.latitude >= extent.latitude[0]
                                true
                            ) {
                                // 位置点在给定范围内
                                console.log("%cGeoLocator: 位置点有效", 'color: green');
                                // 判断该点是否合理
                                if (!_lastPosition) {
                                    _lastPosition = position
                                    _failedCount = 0
                                    eventbus.$emit('geolocation', position)
                                } else {
                                    let distanceForMeter = calcDistance(
                                        position.longitude,
                                        position.latitude,
                                        _lastPosition.longitude,
                                        _lastPosition.latitude,
                                        6
                                    ) * 1000;
                                    // 最大阀值为15m/秒
                                    if (distanceForMeter <= 0.015 * computedConfig.interval) {
                                        console.log("%cGeoLocator: 位置点合理 %s", 'color: green', distanceForMeter);
                                        eventbus.$emit('geolocation', position)
                                        _lastPosition = position
                                        _failedCount = 0
                                    } else {
                                        console.warn('GeoLocator: 位置点不合理: ', distanceForMeter)
                                        _failedCount++
                                        if (_failedCount === 3) {
                                            GeoLocator.restart()
                                        }
                                    }
                                }
                            } else {
                                console.warn('GeoLocator: 该位置点越界')
                                _failedCount++
                                if (_failedCount === 3) {
                                    GeoLocator.restart()
                                }
                            }

                        } else {
                            // 无范围限定
                            console.log("%cGeoLocator: 位置点有效且合理 %s", 'color: green');
                            eventbus.$emit('geolocation', position)
                        }
                    } else {
                        console.error('GeoLocator: 获取位置点出错: ', err)
                        _failedCount++
                        if (_failedCount === 3) {
                            GeoLocator.restart()
                        }
                    }
                })
            }, computedConfig.interval)
            _state = 2
            console.log('%cGeoLocator: 定时任务已开启', 'color: blue')
        } else if (_state === 0) {
            console.error('GeoLocator: 调用start之前必须先调用init方法并传入一个自定义配置对象')
        } else if (_state === 2) {
            console.error(`GeoLocator: _taskId为 ${_taskId} 的定时任务正在进行中，无法重复start`)
        }
    }

    static stop() {
        if (_state === 2 && _taskId) {
            window.clearInterval(_taskId)
            _lastPosition = null
            _state = 3
            _failedCount = 0
            console.log('%cGeoLocator: 定时任务已停止', 'color: orange')
        } else {
            console.warn('%cGeoLocator: 定时任务尚不存在，无法（没有必要）执行stop方法', 'color: yellow')
        }
    }

    static restart() {
        console.log('%cGeoLocator: 准备重启...', 'color: blue')
        GeoLocator.stop()
        GeoLocator.start()
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
