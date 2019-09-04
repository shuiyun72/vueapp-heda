<template>
  <div class="mui-content login_container" :style="bgImage">
    <el-form
      id="loginForm"
      class="login_form"
      ref="loginForm"
      :model="loginFormData"
      :rules="loginFormValidation"
      @submit.native.prevent
    >
      <el-form-item label="用户名" prop="username">
        <el-input
          name="username"
          v-model.trim="loginFormData.username"
          size="large"
          class="username_input"
        ></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input
          name="password"
          v-model.trim="loginFormData.password"
          size="large"
          type="password"
          @focus="onPasswordInputFocus"
          class="password_input"
        ></el-input>
      </el-form-item>
      <el-form-item prop="rememberPassword" border>
        <el-checkbox v-model="rememberPassword" size="large" class="remember_box">记住密码</el-checkbox>
      </el-form-item>
      <el-form-item>
        <el-button
          type="primary"
          class="login_button"
          @click="onLoginClick"
          :loading="isLoginLoading"
          v-html="loginButtonText"
        ></el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import apiUser from "@api/user";
import _ from "lodash";
// 从/common/util.js导入两个工具函数
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem
} from "@common/util";
export default {
  beforeRouteEnter(to, from, next) {
    next(() => {
      // 每次进入登录页面的时候，固定body的高度，为了解决安卓下软键盘弹出致页面高度塌陷的问题
      let loginPageDOM = document.querySelector(".login_container");
      let originalHeight = `${document.body.clientHeight}px`;
      loginPageDOM.style.height = originalHeight;
      let paddingTop = window.getComputedStyle(loginPageDOM).paddingTop;
      loginPageDOM.style.paddingTop = paddingTop;
    });
  },
  created() {
    // 读取被保存的上次登录用户认证信息
    window.mui.plusReady(() => {
      this.deviceInfo = window.plus.device;
    });
    let lastLogin = JSON.parse(getLocalItem("lastLogin"));
    // 如果存在，填入表单
    if (lastLogin) {
      this.loginFormData = lastLogin;
      this.rememberPassword = true;
    }

    //如果是和达用户登陆
    // if (
    //   this.$route.query.HDACC &&
    //   this.$route.query.HDSTAMP &&
    //   this.$route.query.HDSSOKEY
    // ) {
    //   this.signInWithHdAcc(
    //     this.$route.query.HDACC,
    //     this.$route.query.HDSTAMP,
    //     this.$route.query.HDSSOKEY
    //   );
    // }
  },
  data() {
    return {
      bgImage: { "background-image": `url(./static/images/login_bg.png)` },
      // 当前设备信息
      deviceInfo: {},
      loginFormData: {
        username: "",
        password: ""
      },
      loginFormValidation: {
        username: [
          {
            required: true,
            message: "用户名不能为空",
            trigger: "blur"
          }
        ],
        password: [
          {
            required: true,
            message: "密码名不能为空",
            trigger: "blur"
          },
          {
            min: 6,
            max: 18,
            message: "密码长度有误",
            trigger: "blur"
          }
        ]
      },

      // 登录按钮相关
      loadingText: "正 在 登 录...",
      defaultText: "登  &nbsp;&nbsp; 录",
      loginButtonText: "登  &nbsp;&nbsp; 录",
      isLoginLoading: false,
      // 记住密码
      rememberPassword: false
    };
  },
  methods: {
    onPasswordInputFocus() {
      // 解决登录按钮被软键盘盖住的问题，当用户点击密码框，使登录按钮可见
      document.querySelector(".username_input").scrollIntoView();
      setTimeout(() => {
        document.querySelector(".username_input").scrollIntoView();
      }, 1000);
    },
    onLoginClick() {
      this.isLoginLoading = true;
      this.loginButtonText = this.loadingText;
      let username = this.loginFormData.username;
      let password = this.loginFormData.password;
      let smid = this.deviceInfo.uuid || 1;
      apiUser
        .UserLogin(username, password, smid)
        .then(res => {
          this.isLoginLoading = false;
          this.loginButtonText = this.defaultText;
          let resData = res.data[0];
          if (resData.IsSuccess === false || resData.IsSuccess === "false") {
            // 认证失败
            mui.toast("用户名或密码错误", {
              duration: "long",
              type: "div"
            });
            this.isLoginLoading = false;
            this.loginButtonText = this.defaultText;
          } else {
            this.skipToRootRouter(resData);
            console.log("登陆成功")
          }
        })
        .catch(err => {
          this.isLoginLoading = false;
          this.loginButtonText = this.defaultText;
          mui.toast("网络连接超时");
        });
    },

    skipToRootRouter(resData) {
      // 认证成功
      // 将当前登录用户信息存储到sesstionStorage
      let userInfo = Object.assign({}, resData, {
        // 将当前设备信息整合进userInfo
        deviceInfo: this.deviceInfo
      });
      console.log("setSessionItem--179",JSON.stringify(userInfo))
      setSessionItem("currentUser", JSON.stringify(userInfo));
      this.$router.replace({
        path: "/"
      });
      // 勾选了记住密码
      if (this.rememberPassword) {
        setLocalItem("lastLogin", JSON.stringify(this.loginFormData));
      } else {
        setLocalItem("lastLogin", null);
      }
    }
  }
};
</script>

<style lang="less">
div.login_container {
  /* 背景图片 */
  width: 100vw;
  height: 100vh;
  background-repeat: no-repeat;
  background-position: center;
  background-size: 100% 100%;
  /* 表单距离页面顶部高度为百分比设备高度 */
  padding-top: 21vh;
  /* 表单百分比宽度水平居中 */
  .login_form {
    width: 60%;
    margin: 0 auto;
  }
  /* 复选框显示在右边 */
  .remember_box {
    float: right;
    /* 复选框的label颜色 */
    [class*="label"] {
      color: #fff;
    }
  }
  /* 登录按钮长度 */
  .login_button {
    width: 100%;
  }
  /* 所有input的label的颜色 */
  [class="el-form-item__label"] {
    color: #a6a8a9;
  }
  /* 登录页字体偏大 */
  * {
    font-size: 17px;
  }
}
</style>


