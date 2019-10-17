<template>
  <div class="mui-content index_container">
    <div class="index_image_container" :style="headerBgImageStyle">
      <AccountCard
        :userName="currentUser.PersonName"
        :roleName="currentUser.RoleName"
        :avatarPath="currentAvatar"
        class="account_card_position"
      >
        <!-- 这里button做为插槽内容，传递给组件，并替代组件内部的slot占位符 -->
        <button
          slot="action"
          type="button"
          class="mui-btn mui-btn-warning"
          style="background-color: #009688; border: unset;"
          @tap="$router.push({name: 'AccountCenter'})"
        >基本信息</button>
      </AccountCard>
    </div>
    <!-- 功能块 -->
    <div class="module_section" v-for="section in sections" :key="section.index">
      <!-- 功能块Header -->
      <SectionHeader :sectionTitle="section.sectionTitle"></SectionHeader>
      <!-- 功能行 -->
      <div class="mui-row" v-for="row in section.rows" :key="row.index">
        <!-- 功能项 -->
        <ModuleItem
          v-for="item in row.items"
          :key="item.id"
          :title="item.title"
          :desc="item.desc"
          :mode="item.mode"
          :withBorder="item.withBorder"
          :picture="item.picture"
          :pictureContainerStyle="item.pictureContainerStyle"
          :class="item.class"
          :eventCountPatrol="eventCountPatrol"
          :eventCountSelfTask="eventCountSelfTask"
          :eventCountOrders="eventCountOrders"
          :enabled="permissionJson ? permissionJson.includes(item.id) : false"
          @click="switchPage(item)"
        ></ModuleItem>
      </div>
    </div>
  </div>
</template>

<script>
import _ from "lodash";
import { setSessionItem, getSessionItem } from "@common/util";
import Timer from "@common/timer";
import dateHelper from "@common/dateHelper";
import apiInspection from "@api/inspection";
import apiUser from "@api/user";
import AccountCard from "@comp/common/AccountCard";
import SectionHeader from "@comp/common/ModuleSectionHeader";
import apiMaintainNew from "@api/maintain-new";
import apiMaintain from "@api/maintain";
// 该组件表示首页中一个子功能的入口
import ModuleItem from "@comp/common/ModuleItem";

// 指出哪些页面在跳转时需要开启稍候提示
const ShowLoadingWhenEnter = ["Map", "DMAMonitorIndex"];

export default {
  beforeRouteEnter(to, from, next) {
    if (from.name === "Login") {
      next(instance => {
        let id = JSON.parse(getSessionItem("currentUser")).PersonId;
        apiUser.GetUserPermission(id).then(res => {
          console.log("权限", res);
          let permissionList = res.data;
          // 拿到的权限列表必须是非空数组
          if (Array.isArray(permissionList) && !_.isEmpty(permissionList)) {
            // json字符串化的权限列表
            instance.permissionJson = JSON.stringify(permissionList);
            // 存储到sessionStorage
            setSessionItem("permission", instance.permissionJson);
            //实时监控巡检任务
            instance.hintMsgCount();
            if(!instance.hintMsgCountTimer){
              instance.hintMsgCountTimer = setInterval(()=>{
                instance.hintMsgCount();
              },1500)
            }
          } else {
            instance.$router.push({ name: "Login" });
            mui.toast("获取权限失败，请重新登录", {
              type: "div"
            });
          }
        });
        // .catch(err => {
        //   console.log("权限错误 ", err);
        //   instance.$router.push({ name: "Login" });
        //   mui.toast("获取权限失败，请重新登录", {
        //     type: "div"
        //   });
        // });
      });
    } else {
      next(instance => {
        instance.permissionJson = getSessionItem("permission");
      });
    }
  },
  created(){   
    this.isHedaUser();
  },
  beforeDestroy(){
    clearInterval(this.hintMsgCountTimer);
    this.hintMsgCountTimer = null;
  },
  mounted() {
    // 从全局变量读取并恢复滚动条状态
    // 不优雅的实现
    setTimeout(() => {
      if (window.scrollbarState) {
        window.document.body.scrollTo(...window.scrollbarState);
        window.scrollbarState = null;
      }
    }, 110);
  },
  data() {
    return {
      hintMsgCountTimer:null,   //实时更新任务数据定时器
      permissionJson:getSessionItem("permission")||"",
      currentUser:getSessionItem("currentUser") || "",
      eventCountPatrol:0,
      eventCountSelfTask:0,
      eventCountOrders:0,
      permissionJson: "",
      headerBgImagePath: "./static/images/index_header.png",
      // 首页功能块的配置信息
      sections: [     
        {
          index: 1,
          sectionTitle: "巡检养护",
          rows: [
            {
              index: 1,
              items: [
                {
                  index: 1,
                  id: 90049089,
                  title: "移动GIS",
                  desc: "管网设施浏览",
                  mode: "horizontal",
                  picture: "./static/images/mobile_GIS.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  destination: "Map",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 2,
                  id: 90049090,
                  title: "考勤管理",
                  desc: "个人签到签退",
                  mode: "horizontal",
                  destination: "Attendance",
                  picture: "./static/images/patrol_attendance.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 3,
                  id: 90049091,
                  title: "维修工单",
                  desc: "管网维修处理",
                  mode: "horizontal",
                  picture: "./static/images/repair_order.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  destination: "RepairOrders",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 4,
                  id: 90049093,
                  title: "巡检任务",
                  desc: "巡检计划处理",
                  mode: "horizontal",
                  picture: "./static/images/patrol_mission.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  destination: "PatrolMission",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 5,
                  id: 90049095,
                  title: "事件上报",
                  desc: "临时计划处理",
                  mode: "horizontal",
                  picture: "./static/images/emergency_submission.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  destination: "EventSubmission",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                }           
              ]
            }
          ]
        },
        {
          index: 2,
          sectionTitle: "系统信息",
          rows: [
            {
              index: 1,
              items: [
                {
                  index: 1,
                  id: 90049096,
                  title: "个人信息",
                  desc: "配置个人信息",
                  mode: "horizontal",
                  picture: "./static/images/self_info.png",
                  destination: "AccountCenter",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 2,
                  id: 90049097,
                  title: "系统管理",
                  desc: "系统配置管理",
                  mode: "horizontal",
                  picture: "./static/images/setting.png",
                  destination: "Setting",
                  // destination: "Test",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                }
              ]
            }
          ]
        }
      ]
    };
  },
  computed: {
     /*currentUser() {
       return JSON.parse(getSessionItem("currentUser")) || this.currentUser;
     },*/
    // 当前用户id
    currentUserId() {
      return this.currentUser.PersonId;
    },
    currentAvatar() {
      return "./static/images/avatar_male.png";
    },
    headerBgImageStyle() {
      return { "background-image": `url(${this.headerBgImagePath})` };
    }
  },
  methods: {
    //验证是否是和达用户
    isHedaUser(){
      //如果是和达用户登陆
      if (
        this.$route.query.HDACC &&
        this.$route.query.HDSTAMP &&
        this.$route.query.HDSSOKEY
      ) {
        this.signInWithHdAcc(
          this.$route.query.HDACC,
          this.$route.query.HDSTAMP,
          this.$route.query.HDSSOKEY
        );
        console.log("index--$route",this.$route.query)
      }else{      
        let sss = getSessionItem("currentUser");
        if(sss === null || sss === "null" ){
          this.$router.replace({
            path: "/login"
          });
        }else{
             console.log("denglu")
        }  
      }
    },
    //和达登陆
    signInWithHdAcc(hdAcc, hdStamp, hdSSOKey) {
      if (apiUser.SignInWithHdAcc(hdAcc, hdStamp, hdSSOKey)) {
        apiUser
          .HdUserLogin(hdAcc)
          .then(res => {
            //this.isLoginLoading = false;
            //this.loginButtonText = this.defaultText;
            let resData = res.data[0];
            if (resData.IsSuccess === false || resData.IsSuccess === "false") {
              // 认证失败
              mui.toast(`没有 ${hdAcc} 该用户`, {
                duration: "long",
                type: "div"
              });
              //this.isLoginLoading = false;
              //this.loginButtonText = this.defaultText;
              this.$router.replace({
                path: "/login"
              });
            } else {
              this.skipToRootRouter(resData)
            }
          })
          .catch(err => {
            //this.isLoginLoading = false;
            //this.loginButtonText = this.defaultText;
            //mui.toast("网络连接超时");
            this.$router.replace({
              path: "/login"
            });
          });
      }
    },
    //登陆验证
    skipToRootRouter(resData) {
      // 认证成功
      // 将当前登录用户信息存储到sesstionStorage
      let userInfo = Object.assign({}, resData, {
        // 将当前设备信息整合进userInfo
        deviceInfo: this.deviceInfo
      });
      setSessionItem("currentUser", JSON.stringify(userInfo));
      this.currentUser = userInfo;
      console.log(this.currentUser)
      let id = this.currentUser.PersonId;
      apiUser.GetUserPermission(id).then(res => {
        console.log("权限", res);
        let permissionList = res.data;
        // 拿到的权限列表必须是非空数组
        if (Array.isArray(permissionList) && !_.isEmpty(permissionList)) {
          // json字符串化的权限列表
          this.permissionJson = JSON.stringify(permissionList);
          // 存储到sessionStorage
          setSessionItem("permission", this.permissionJson);
          this.permission = this.permissionJson;
          //实时监控巡检任务
          this.hintMsgCount();
          if(!this.hintMsgCountTimer){
            this.hintMsgCountTimer = setInterval(()=>{
              this.hintMsgCount();
            },1500)
          }
        } else {
          this.$router.push({ name: "Login" });
          mui.toast("获取权限失败，请重新登录", {
            type: "div"
          });
        }
      });
    },
    //任务信息推送
    hintMsgCount(){
      this.getEventCount();
    //  this.getSelfTask();
      this.getOredes();
    },
    //维修工单
    getOredes(){
      if(this.currentUserId != null && this.currentUserId != "null"){
        apiMaintain
        .GetOrderList(this.currentUserId, 1)
        .then(res => {     
          if (res.data.result) {
            this.eventCountOrders = res.data.data.length;
          }
        })
      }     
    },
    //个人工单-代办工单
    getSelfTask(){
      if(this.currentUserId != null && this.currentUserId != "null"){
        apiMaintainNew.GetOrderList(this.currentUserId,0).then(res => {
          if (res.data.ErrCode == 0) {
            this.eventCountSelfTask = res.data.rows.length
          } else {
            mui.toast('获取个人工单失败')
          }
        }).catch(err=>{
          mui.toast('网络错误，获取工单失败')
        });
      }     
    },
    //监听巡检任务
    getEventCount(){
      if(this.currentUserId != null && this.currentUserId != "null"){
        let userId = this.currentUserId;
        let currentDayDate = dateHelper.format(new Date(), "yy-MM-dd");
        apiInspection
          .GetMissionList(userId, currentDayDate)
          .then(res => {
            if (res.data.result === true) {
              this.eventCountPatrol = res.data.Data.length;
            }
            this.$hideLoading();
          })
          .catch(err => {        
              console.log("err", err);
          });
      }
    },
    // 点击某个具体功能块时切换路由（页面）
    switchPage(itemConfig) {
      if (ShowLoadingWhenEnter.includes(itemConfig.destination)) {
        mui.toast("正在加载，请稍候...", {
          type: "div"
        });
        // this.$showLoading();
      }
      // 此处使用路由的name进行跳转，因此请参照router/index中的路由name
      let destination = itemConfig.destination;
      window.rr = this.$router;

      this.$router.push({ name: destination });
    }
  },
  components: {
    AccountCard,
    SectionHeader,
    ModuleItem
  }
}
</script>

<style scoped lang="less">
div.index_container {
  padding-top: 36vh;
  .index_image_container {
    position: fixed;
    height: 35vh;
    width: 100%;
    top: 0;
    z-index: 500;
    background-repeat: no-repeat;
    background-position: center;
    background-size: 100% 100%;
    .account_card_position {
      position: relative;
      top: 65%;
      left: 10%;
    }
  }
  .module_section {
    margin: 2px auto;
    &-header {
      border: 1px solid #eee;
      background-color: #fff;
      padding: 8px 2px;
      font-size: 1.2rem;
    }
    .module_item {
      font-size: 1.1rem;
    }
  }
}
</style>


