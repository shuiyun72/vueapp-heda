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
            <li class="mui-table-view-cell">
                <a class="mui-navigate-right">版本更新 <span class="version_span">V{{appInfo.version || '1.0.0'}}</span></a>
            </li>
            <li class="mui-table-view-cell">
                <a class="mui-navigate-right">关于我们</a>
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
export default {
  beforeRouteEnter(to, from, next) {
    if (window.plus) {
      let { appid } = plus.runtime;
      next(instance => {
        plus.runtime.getProperty(appid, function(appInfo) {
          instance.appInfo = appInfo
        });
      });
    } else {
      next();
    }
  },
  data() {
    return {
      appInfo: {}
    };
  },
  methods: {
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
</style>

