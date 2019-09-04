import dateHelper from "@common/dateHelper";
import apiInspection from "@api/inspection";
import Vue from 'vue'
import _ from 'lodash'
import {
    calcDistance,
    deepCopy,
    getSessionItem
} from "@common/util";

// started 1; stopped 0;
let _state = 0

let currentUser = {}
// 在调用start方法后，用来筛选出初始点位的数组
let _checkValidationArr = []
// resolved是一个flag，用来标志当前是否已通过稳定性测试
_checkValidationArr.resolved = false
let positionCallback = position => {
    console.log('Uploader: 从事件总线接收到position： ', position)
    let arrLen = _checkValidationArr.length
    if (arrLen < 4) {
        if (arrLen === 0) {
            console.log('Uploader: 开始进行定位稳定性测试', arrLen)
        } else {
            console.log('Uploader: 定位稳定性测试中... ', arrLen)
        }
        // 验证未完成，继续验证
        _checkValidationArr.push(deepCopy(position))
        if (arrLen !== 0) {
            let last = _checkValidationArr[arrLen - 1]
            let distanceForMeter = calcDistance(
                position.longitude,
                position.latitude,
                last.longitude,
                last.latitude,
                6
            ) * 1000;
            if (distanceForMeter <= 2000000) {
                // pass
            } else {
                _checkValidationArr.splice(0, arrLen)
            }
        }
    } else {
        if (!_checkValidationArr.resolved) {
            /* 用于测试的四组数据是否上传 */
            // // 组装四组参数 argsArr是一个二维数组
            // let argsArr = _checkValidationArr.map(position => ([
            //     position.longitude,
            //     position.latitude,
            //     dateHelper.format(new Date(position.timestamp), "yyyy-MM-dd hh:mm:ss"),
            //     currentUser.PersonId,
            //     // isOnline默认为1
            //     1
            // ]))
            // // 将数组中的四组数据上传
            // Promise.all(
            //     argsArr.map(args => {
            //         return apiInspection.UploadLocation(...args)
            //     })
            // ).then(resArr => {
            //     if (resArr.every(res => res.data.result)) {
            //         console.log('Uploader: 用于准确性测验的四组数据已上传成功')
            //     } else {
            //         console.log('Uploader: 用于准确性测验的四组数据上传失败！')
            //     }
            // }).catch(err => {
            //     console.warn('Uploader: 用于准确性测验的四组数据上传失败！', err)
            // })
            // 上传成功后改变flag
            _checkValidationArr.resolved = true
            console.log('Uploader: 定位稳定性测试已通过')
        } else {
            // 验证已完成，定位趋于稳定，直接上传
            console.log("Uploader: 准备上传位置----");
            let args = [
                position.longitude,
                position.latitude,
                dateHelper.format(new Date(position.timestamp), "yyyy-MM-dd hh:mm:ss"),
                currentUser.PersonId,
                // isOnline默认为1
                1
            ]
            apiInspection.UploadLocation(...args).then(res => {
                    if (res.data.result === true) {
                        console.log("UploadLocation: 上传位置成功", res);
                    } else {
                        console.log("UploadLocation: 上传位置失败", err);
                    }
                })
                .catch(err => {
                    console.log("UploadLocation: 上传位置失败", err);
                });
        }

    }
}
export default class PositionUploader {
    constructor() {}
    static start() {
        if (_state !== 1) {
            let eventbus = Vue.prototype.$eventbus
            currentUser = JSON.parse(getSessionItem("currentUser"));
            eventbus.$on('geolocation', positionCallback)
            _state = 1
            console.log("Uploader: start成功");
        }
    }

    static stop() {
        if (_state !== 0) {
            let eventbus = Vue.prototype.$eventbus
            currentUser = {}
            eventbus.$off('geolocation', positionCallback)
            _checkValidationArr = []
            _state = 0
            console.log("Uploader: stop成功");
        }
    }

    static getCurrentState() {
        return _state
    }
}
