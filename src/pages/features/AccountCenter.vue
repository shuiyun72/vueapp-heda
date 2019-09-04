<template>
    <div class="account_center_container">
        <el-card>   
            <div class="avatar">
                <img :src="currentAvatar">
            </div> 
            <div class="summary" >
                <div class="username">{{userInfo.cAdminName}}</div>
                <div class="role">{{userInfo.cRoleName}} | {{userInfo.cDepName}}</div>
            </div>
        </el-card>

        <!-- <el-card style="margin-top: 1%">
          <el-button type="info" round class="action_buttons">版本更新</el-button>
          <el-button type="primary" class="action_buttons">修改密码</el-button>
          <el-button type="danger" class="action_buttons">退出登录</el-button>
          <el-button type="primary" class="action_buttons">编辑个人信息</el-button>
        </el-card> -->
        <ul class="user_info_ul">
          <li>
            <span class="user_info_label">用户姓名</span>
            <span class="user_info_content">{{userInfo.cAdminName}}</span>
          </li>
          <li>
             <span class="user_info_label">性别</span>
             <span class="user_info_content">{{userInfo.cAdminSex}}</span>
          </li>
          <li>
            <span class="user_info_label">联系电话</span>
            <span class="user_info_content">{{userInfo.cAdminTel}}</span>
          </li>
          <li>
            <span class="user_info_label">电子邮件</span>
            <span class="user_info_content">{{userInfo.cAdminEmail}}</span>
          </li>
          <li>
            <span class="user_info_label">是否锁定</span>
            <span class="user_info_content">{{userInfo.iIsLocked}}</span>
          </li>
          <li>
            <span class="user_info_label">账户过期时间</span>
            <span class="user_info_content">{{userInfo.dExpireDate | timeFormatter}}</span>
          </li>   
        </ul>
    </div>
</template>

<script>
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem
} from "@common/util";
import apiUser from "@api/user";
import dateHelper from "@common/dateHelper";

export default {
  mounted() {
    this.fetchUserInfo();
  },
  data() {
    return {
      userInfo: {}
    };
  },
  computed: {
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    currentAvatar() {
      return this.userInfo.cAdminSex === "男"
        ? "./static/images/avatar_male.png"
        : "./static/images/avatar_female.png";
    }
  },
  methods: {
    fetchUserInfo() {
      apiUser
        .GetUserInfo(this.currentUser.PersonId)
        .then(res => {
          console.log("==", res);
          if (res.data.ErrCode === 0) {
            let userInfo = res.data.Data[0];
            this.userInfo = userInfo;
          } else {
            mui.toast("获取用户信息失败！", { type: "div" });
          }
        })
        .catch(err => {
          mui.toast("获取用户信息失败！", { type: "div" });
        });
    }
  },
  filters: {
    timeFormatter(time) {
      return dateHelper.format(new Date(time), "yyyy-MM-dd hh:mm:ss");
    }
  }
};
</script>

<style lang="less" scoped>
.account_center_container {
  .avatar {
    margin: 1% auto;
    display: block;
    width: 30vw;
    height: 30vw;
    img {
      /* display: inline-block; */
      width: inherit;
      height: inherit;
      border-radius: 50%;
      border-radius: 50%;
    }
  }
  .summary {
    display: block;
    & > div {
      margin: 2% auto;
      text-align: center;
    }
    .username {
      font-size: 1.6rem;
      font-weight: bold;
      color: #001d26;
    }
    .role {
      font-size: 1.2rem;
      // color: #999;
      color: #00afa9;
    }
  }
  .action_buttons {
    width: 100%;
    margin: 2% 0;
  }
  .user_info_ul {
    list-style-type: none;
    text-align: left;
    padding-left: unset;
    margin: 4% 0;
    padding: 0 2%;
    font-size: 1.3rem;
    line-height: 3rem;
    background-color: #fff;
    li {
      display: flex;
      justify-content: space-between;
      margin: 5px 0 0 0;
      border-bottom: 1px solid lightskyblue;
      .user_info_label {
        display: inline-block;
        color: #999;
        width: 35%;
      }
      .user_info_content {
        display: inline-block;
        margin-right: 3%;
      }
    }
  }
}
</style>

