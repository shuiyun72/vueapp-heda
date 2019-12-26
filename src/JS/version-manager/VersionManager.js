import ApiMonitor from '@api/user'
var rootURL = process.env.API_ROOT

let _newVersionInfo = {
  versionCode: '',
  description: '',
  url: '',
  filePath: ''
}

export default class VersionManager {
  constructor() {}
  static CheckUpdate() {
    // 先监测是否在plus环境下
     if (window && window.plus) {
      // 发送ajax请求
      return ApiMonitor.CheckAppUpdate().then(res => {
        console.log('版本更新res', res.data.Data.Result[0].VERSIONID)
        let VERSIONID = res.data.Data.Result[0].VERSIONID;
        return new Promise((resolve, reject) => {
          window.plus.runtime.getProperty(window.plus.runtime.appid, function (inf) {
            let wgtVer = inf.version;
            console.log("当前应用版本：" + wgtVer, '最新版本:' + VERSIONID);
            let haveNewVersion = (VERSIONID > wgtVer)
            _newVersionInfo.versionCode = VERSIONID
            // _newVersionInfo.description = res.data[0].description
            _newVersionInfo.description = "最新版本为"+VERSIONID+",是否下载更新？"
            _newVersionInfo.url = rootURL+"/H5188E903_0425021627.apk"
            resolve(haveNewVersion)
          });
        })
      }, err => {
        console.log('检测版本更新接口出错', err)
      })
    } else {
      console.error('当前不在plus环境下，无法执行版本更新检查')
    }
  }
  static Download() {
    let newVersionUrl = _newVersionInfo.url
    // 这里暂时没有做url格式验证
    if (newVersionUrl.length > 0) {
      window.plus.nativeUI.showWaiting("正在下载新版本...");
      return new Promise((resolve, reject) => {
        window.plus.downloader.createDownload(newVersionUrl, {
          filename: "_doc/update/"
        }, (d, status) => {
          if (status == 200) {
            _newVersionInfo.filePath = d.filename
            resolve(true)
          } else {
            resolve(false)
          }
          plus.nativeUI.closeWaiting();
        }).start();
      })
    } else {
      console.warn('版本管理未发现可用的新版本下载地址，无法下载')
    }
  }

  static Install() {
    plus.nativeUI.showWaiting("正在安装...请稍候");
    return new Promise((resolve, reject) => {
      plus.runtime.install(_newVersionInfo.filePath, {
        force: true
      }, () => {
        plus.nativeUI.closeWaiting();
        resolve(true)
      }, err => {
        resolve(false, err)
      });
    })
  }

  static RestartApp() {
    plus.runtime.restart();
  }
}
