// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from "vue";
// import VConsole from 'vconsole/dist/vconsole.min.js'
// let vConsole = new VConsole()
import App from "./App";
import router from "./router";
import AppConfig from '@config/config.js'


Vue.config.productionTip = false;
// 初始化mui
if (window.mui) {
  window.mui.init({
    // 声明手势事件配置
    gestureConfig: {
      tap: true, //默认为true
      doubletap: true, //默认为false
      longtap: true, //默认为false
      swipe: true //默认为true
    }
  });
}

// 加载mui picker插件
import '@assets/css/mui.picker.all.css'
import mountPickerPlugin from '@assets/js/mui.picker.esm.js'
mountPickerPlugin()
// 加载完成后可使用picker组件, mui picker 用法：http://dev.dcloud.net.cn/mui/ui/#picker

// 加载EasyUI组件库
import EasyUI from 'vx-easyui';
import 'vx-easyui/dist/themes/icon.css';
import 'vx-easyui/dist/themes/default/easyui.css';
import 'vx-easyui/dist/themes/vue.css';
Vue.use(EasyUI);

// 加载ElementUI组件库
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
Vue.use(ElementUI);

// 启用上拉刷新和下拉加载
import VueScroller from 'vue-scroller'
Vue.use(VueScroller)

import infiniteScroll from 'vue-infinite-scroll'
Vue.use(infiniteScroll)

// 引入openlayers的样式
import 'ol/ol.css'

// 为每个Vue实例添加方法
((Vue, mui) => {
  let defaultBack = mui.back;
  // 定制设备返回键事件处理逻辑
  Vue.prototype.$defineDeviceBack = function (callback) {
   // console.error(11111)

    mui.back = () => {
      callback(defaultBack)
    }
    this.$hasCustomDeviceBack = true
  }
  // 还原默认的返回逻辑
  Vue.prototype.$revertDefaultDeviceBack = function () {
    mui.back = defaultBack
    this.$hasCustomDeviceBack = false
  }
})(Vue, window.mui);


// 注册全局事件总线
((Vue) => {
  let eventbus = new Vue()
  Vue.prototype.$eventbus = eventbus
  console.log(`事件总线已创建！`, eventbus)
})(Vue)

// 实例化Vue应用
const Root = new Vue({
  // 在index.html的#app处挂载
  el: "#app",
  // 使用router对象实例化路由
  router,
  // 实例的模板，直接渲染App组件
  template: "<App/>",
  // 本应用实例依赖的组件
  components: {
    App
  }
});

// 添加操作全局loading实例的原型方法
Vue.prototype.$showLoading = function (options) {
  let instance = this.$loading(options || {
    lock: true,
    text: "正在加载，请稍候..."
  })
  Vue.prototype.$loadingInstance = instance
}

Vue.prototype.$hideLoading = function () {
  if (this.$loadingInstance) {
    this.$nextTick(() => { // 以服务的方式调用的 Loading 需要异步关闭
      this.$loadingInstance.close();
      Vue.prototype.$loadingInstance = null
    });
  }
}

window.onerror = (errMsg) => {
  // 作为补丁抵消mui的一个bug，该bug使得其内置的mui.back方法会消失
  if (errMsg.includes('back is not a function')) {
    Vue.prototype.$revertDefaultDeviceBack()
    window.mui.back()
  }
}

// 初始化定位服务，并通过全局时间总线分发点位数据
import GeoLocator from "@JS/geolocator/GeoLocator";
// 版本管理
import VersionManager from '@JS/version-manager'
if (window.mui) {

    // 初始化GeoLocator
    GeoLocator.init({
      extent: AppConfig.locationExtent || {}
    })
    .then(GeoLocator.start)
    .catch(err => {
      console.log(`开启定位服务失败 `, err);
    });

    // 监测版本
    /*VersionManager.CheckUpdate().then((haveNewVersion) => {
      if (haveNewVersion) {
        // 弹出框，选择是否更新
        plus.nativeUI.confirm("检测到新版本,是否更新?", (e) => {
          if (e.index == 0) {
            // 下载升级包并安装
            VersionManager.Download().then((downloadStatus) => {
              if (downloadStatus) {
                return VersionManager.Install()
              } else {
                // 提醒用户下载出错
                console.log("下载wgt失败！");
              }
            }).then((installStatus) => {
              if (installStatus) {
                VersionManager.RestartApp()
              } else {
                // 提醒用户安装失败
                console.log("安装wgt失败！");
              }
            })
          }
        })
      }
    })*/
    
 
}







// 初始化vue-worker插件
// import VueWorker from 'vue-worker'
// Vue.use(VueWorker)
// const TEST = 'TEST'
// Vue.prototype.$worker.run((test) => {
//     return setInterval(() => {
//         return 1
//     }, 1000)
// }, [TEST]).then(console.log).catch(console.err)



// plus.geolocation.getCurrentPosition(position => {
//     let coords = position.coords;
//     // let time = 'shijian'
//     let time = (new Date()).toLocaleTimeString()
//     const STATUS = true
//     let outputPosition = Object.assign({}, coords, {
//         status: STATUS,
//         time
//     })
//     worker.postMessage('current-position', [outputPosition])
// }, err => {
//     const STATUS = false
//     // let time = 'shijian'
//     let time = (new Date()).toLocaleTimeString()
//     let coords = {
//         longitude: 'null',
//         latitude: 'null'
//     };
//     let outputPosition = Object.assign({}, coords, {
//         status: STATUS,
//         time
//     })
//     worker.postMessage('current-position', [outputPosition])
// }, {
//     enableHighAccuracy: true,
//     maximumAge: 5000,
//     // provider: "baidu",
//     provider: "system",
//     coordsType: "wgs84"
// });
