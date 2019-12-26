<template>
  <div class="setting_container">
    <ul class="mui-table-view setting_section">
      <li class="mui-table-view-cell" @tap="$router.push({name: 'AccountCenter'})">
        <a class="mui-navigate-right">个人中心</a>
      </li>
    </ul>

    <ul class="mui-table-view setting_section">
      <li class="mui-table-view-cell">
        <a class="mui-navigate-right">用户指南</a>
      </li>
      <li class="mui-table-view-cell" @click="checkAppUpdateV">
        <a class="mui-navigate-right">
          版本更新
          <span class="version_span">V{{appInfo.version || '1.0.0'}}</span>
        </a>
      </li>
      <li class="mui-table-view-cell">
        <a class="mui-navigate-right">关于我们</a>
      </li>
      <li class="mui-table-view-cell">
        <div class="downloadevm">
          <img :src="erweimaUrl" />
        </div>
        <div class="downloadevm_text">扫描下载安装包</div>
      </li>
    </ul>

    <ul class="mui-table-view setting_section">
      <li class="mui-table-view-cell logout_item" @tap="onLogoutClick">
        <a>退出登录</a>
      </li>
    </ul>
  </div>
</template>

<script>
import _ from "lodash";
import apiInspection from "@api/inspection";
import dateHelper from "@common/dateHelper";
import { setSessionItem, getSessionItem } from "@common/util";
import config from "@config/config";
import VersionManager from "@JS/version-manager";
export default {
  beforeRouteEnter(to, from, next) {
    if (window.plus) {
      let { appid } = plus.runtime;
      next(instance => {
        plus.runtime.getProperty(appid, function(appInfo) {
          instance.appInfo = appInfo;
        });
      });
    } else {
      next();
    }
  },
  data() {
    return {
      appInfo: {},
      erweimaUrl: "static/images/ewm/" + config.downloadEvmUrl
    };
  },
  methods: {
    checkAppUpdateV() {
      if (window.mui) {
        window.mui.plusReady(() => {
          // 监测版本
          VersionManager.CheckUpdate().then(haveNewVersion => {
            if (haveNewVersion) {
              // 弹出框，选择是否更新
              plus.nativeUI.confirm("检测到新版本,是否更新?", e => {
                if (e.index == 0) {
                  // 下载升级包并安装
                  VersionManager.Download()
                    .then(downloadStatus => {
                      if (downloadStatus) {
                        return VersionManager.Install();
                      } else {
                        // 提醒用户下载出错
                        console.log("下载wgt失败！");
                      }
                    })
                    .then(installStatus => {
                      if (installStatus) {
                        VersionManager.RestartApp();
                      } else {
                        // 提醒用户安装失败
                        console.log("安装wgt失败！");
                      }
                    });
                }
              });
            } else {
              mui.toast("已经是最新版本");
            }
          });
        });
      }
    },
    onLogoutClick() {
      //   apiInspection.UserLogout().then(res => {
      //     if (res.data.result === true) {
      //       setSessionItem("currentUser", null);
      //       this.$router.replace({ name: "Login" });
      //     }
      //   });
      window.mui.confirm(
        "确定退出当前账户吗？",
        "退出登录",
        ["取消", "确认"],
        result => {
          if (result.index === 1) {
            // 确认
            setSessionItem("currentUser", null);
            this.$router.replace({ name: "Login" });
            mui.toast("退出登录成功！");
          } else {
            // 取消
          }
        },
        "div"
      );
    }
  }
};
</script>

<style lang="less" scoped>
.setting_container {
  .setting_section {
    font-size: 1.2rem;
    color: #666;
    margin: 2% auto;
    .version_span {
      display: inline-block;
      position: absolute;
      right: 15%;
    }
    .logout_item {
      text-align: center;
      color: orange;
    }
  }
}
.downloadevm {
  display: flex;
  vertical-align: center;
  justify-content: center;
  img {
    width: 80%;
  }
}
.downloadevm_text {
  text-align: center;
  padding: 4px 0;
}
</style>

