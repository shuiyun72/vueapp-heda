<template>
  <div class="mui-content index_container">
    <div class="index_image_container" :style="headerBgImageStyle">
      <AccountCard
        :userName="currentUser.cAdminName"
        :roleName="currentUser.P_Role.cRoleName"
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
    <div
      class="audio"
      @click="audioPause"
      :style="'background: url(static/images/'+audioSrc+') no-repeat  center center'"
    >
      <audio src controls id="audio_index"></audio>
    </div>
    <!-- 功能块 -->
    <div class="module_section" v-for="section in newPermissionJson" :key="section.index">
      <!-- 功能块Header -->
      <SectionHeader :sectionTitle="section.cFunName" v-if="section.children.length > 0"></SectionHeader>
      <!-- 功能行 -->
      <div class="mui-row" v-if="section.children.length > 0">
        <!-- 功能项 -->
        <ModuleItem
          v-for="item in section.children"
          :key="item.index"
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
          :enabled="permissionJson.length > 0 ? permissionJson.includes(item.iFunID) : ''"
          @click="switchPage(item)"
        ></ModuleItem>
      </div>
    </div>
  </div>
</template>

<script>
import _ from "lodash";
import {
  setSessionItem,
  getSessionItem,
  setLocalItem,
  getLocalItem
} from "@common/util";
import Timer from "@common/timer";
import dateHelper from "@common/dateHelper";
import apiInspection from "@api/inspection";
import apiUser from "@api/user";
import AccountCard from "@comp/common/AccountCard";
import SectionHeader from "@comp/common/ModuleSectionHeader";
import apiMaintain from "@api/maintain";

// 该组件表示首页中一个子功能的入口
import ModuleItem from "@comp/common/ModuleItem";
import BaseMap from "@JS/Map/BaseMap";

// 指出哪些页面在跳转时需要开启稍候提示
const ShowLoadingWhenEnter = ["Map", "DMAMonitorIndex"];

export default {
  beforeRouteEnter(to, from, next) {
    if (from.name === "Login") {
      next(instance => {
        let id = JSON.parse(getSessionItem("currentUser")).iAdminID;
        // let permissionList = `[{"id":0,"text":"APP系统","children":[{"id":"90049076","text":"维修养护","children":[{"id":"90049089","text":"移动GIS","children":[]},{"id":"90049090","text":"巡检签到","children":[]},{"id":"90049091","text":"维修工单","children":[]},{"id":"90049092","text":"工单分派","children":[]},{"id":"90049093","text":"巡检任务","children":[]},{"id":"90049094","text":"养护任务","children":[]},{"id":"90049095","text":"事件上报","children":[]},{"id":"90049096","text":"个人信息","children":[]},{"id":"90049097","text":"系统管理","children":[]}]}]}]`;
        let permissionList = JSON.parse(getSessionItem("currentUser"))
          .UserAuthority;
        instance.permissionJson = JSON.stringify(permissionList);
        setSessionItem("permission", instance.permissionJson);
        // apiUser.GetUserPermission(id).then(res => {
        //   console.log("权限", res);
        //   let permissionList = res.data;
        //   // 拿到的权限列表必须是非空数组
        //   if (Array.isArray(permissionList) && !_.isEmpty(permissionList)) {
        //     // json字符串化的权限列表
        //     instance.permissionJson = JSON.stringify(permissionList);
        //     // 存储到sessionStorage
        //     setSessionItem("permission", instance.permissionJson);
        //   } else {
        //     instance.$router.push({ name: "Login" });
        //     mui.toast("获取权限失败，请重新登录", {
        //       type: "div"
        //     });
        //   }
        // });
      });
    } else {
      next(instance => {
        instance.permissionJson = getSessionItem("permission");
      });
    }
  },
  mounted() {
    //语音提示设置初始化数据
    let OrdersMsg = JSON.parse(getLocalItem("OrdersMsg"));
    this.OrdersMsg = OrdersMsg ? OrdersMsg : {};
    this.eventCountPatrol =
      JSON.stringify(this.OrdersMsg) !== "{}"
        ? this.OrdersMsg.eventCountPatrol
        : undefined;
    this.memoryCountPatrol = this.eventCountPatrol;
    this.eventCountSelfTask =
      JSON.stringify(this.OrdersMsg) !== "{}"
        ? this.OrdersMsg.eventCountSelfTask
        : undefined;
    this.memoryCountSelfTask = this.eventCountSelfTask;
    this.eventCountOrders =
      JSON.stringify(this.OrdersMsg) !== "{}"
        ? this.OrdersMsg.eventCountOrders
        : undefined;
    this.memoryCountOrders = this.eventCountOrders;
    //权限控制要显示的页面及排序
    this.isShowPage();
    // 从全局变量读取并恢复滚动条状态
    // 不优雅的实现
    setTimeout(() => {
      if (window.scrollbarState) {
        window.document.body.scrollTo(...window.scrollbarState);
        window.scrollbarState = null;
      }
    }, 110);

    //实时监控巡检任务
    if (!this.hintMsgCountTimer) {
      this.hintMsgCountTimer = setInterval(() => {
        this.hintMsgCount();
      }, 5000);
    }

    this.mapController = new BaseMap();
    this.mapController.Init();
    //上传位置信息
    if (!this.UploadLocationTimer) {
      this.UploadLocationTimer = setInterval(() => {
        this.onUploadLocation();
      }, 6000);
    }
  },
  beforeDestroy() {
    clearInterval(this.hintMsgCountTimer);
    this.hintMsgCountTimer = null;
    /*clearInterval(this.UploadLocationTimer);
    this.UploadLocationTimer = null;*/
  },
  data() {
    return {
      audioVolume: true, //是否开启声音
      hintMsgCountTimer: null,
      UploadLocationTimer: null,
      eventCountPatrol: undefined, //巡检任务
      eventCountSelfTask: undefined, //个人工单
      eventCountOrders: undefined, //维修工单
      memoryCountPatrol: undefined, //历史巡检任务
      memoryCountSelfTask: undefined, //历史个人工单
      memoryCountOrders: undefined, //历史维修工单
      OrdersMsg: {},
      permissionJson: "",
      headerBgImagePath: "./static/images/index_header.png",
      newPermissionJson: [],
      mapController: undefined,
      // 首页功能块的配置信息
      sections: [
        //         {
        //           index: 1,
        //           sectionTitle: "日常办公",
        //           rows: [
        //             {
        //               index: 1,
        //               items: [
        //                 {
        //                   id: 90049077,
        //                   index: 1,
        //                   title: "公司信息",
        //                   mode: "vertical",
        //                   picture: "./static/images/boardcast.png",
        //                   destination: "CompanyInfoIndex",
        //                   class: "mui-col-sm-3 mui-col-xs-3"
        //                 },
        //                 {
        //                   id: 90049078,
        //                   index: 2,
        //                   title: "待办流程",
        //                   mode: "vertical",
        //                   picture: "./static/images/pending_process.png",
        //                   destination: "OATodoIndex",
        //                   class: "mui-col-sm-3 mui-col-xs-3"
        //                 },
        //                 {
        //                   id: 90049079,
        //                   index: 3,
        //                   title: "已办流程",
        //                   mode: "vertical",
        //                   picture: "./static/images/done_process.png",
        //                   destination: "OADoneIndex",
        //                   class: "mui-col-sm-3 mui-col-xs-3"
        //                 },
        //                 {
        //                   id: 90049080,
        //                   index: 4,
        //                   title: "OA发起",
        //                   mode: "vertical",
        //                   picture: "./static/images/process_mgmt.png",
        //                   destination: "OAPublisherIndex",
        //                   class: "mui-col-sm-3 mui-col-xs-3"
        //                 }
        //               ]
        //             }
        //           ]
        //         },
        // {
        //   index: 2,
        //   sectionTitle: "生产调度",
        //   rows: [
        //     {
        //       index: 1,
        //       items: [
        //         {
        //           id: 90049081,
        //           index: 1,
        //           title: "运行总览",
        //           mode: "vertical",
        //           destination: "StateSummary",
        //           picture: "./static/images/status_overview.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           id: 90049082,
        //           index: 2,
        //           title: "过程监控",
        //           mode: "vertical",
        //           destination: "ProcessMonitor",
        //           picture: "./static/images/process_monitoring.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           index: 3,
        //           id: 90049083,
        //           title: "值班日志",
        //           mode: "vertical",
        //           destination: "DutyLog",
        //           picture: "./static/images/duty_log.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           index: 4,
        //           id: 90049084,
        //           title: "数据查询",
        //           mode: "vertical",
        //           destination: "StatisticIndex",
        //           picture: "./static/images/statistic.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         }
        //       ]
        //     },
        //     {
        //       index: 2,
        //       items: [
        //         {
        //           index: 1,
        //           id: 90049085,
        //           title: "压力监测",
        //           mode: "vertical",
        //           destination: "PressureMonitorIndex",
        //           picture: "./static/images/press_monitoring.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           index: 2,
        //           id: 90049086,
        //           title: "流量监测",
        //           mode: "vertical",
        //           destination: "FlowMonitorIndex",
        //           picture: "./static/images/flow_monitoring.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           index: 3,
        //           id: 90049087,
        //           title: "水质监测",
        //           mode: "vertical",
        //           destination: "WaterQualityMonitorIndex",
        //           picture: "./static/images/quality_monitoring.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         },
        //         {
        //           index: 4,
        //           id: 90049088,
        //           title: "DMA监测",
        //           mode: "vertical",
        //           destination: "DMAMonitorIndex",
        //           picture: "./static/images/DMA_monitoring.png",
        //           class: "mui-col-sm-3 mui-col-xs-3"
        //         }
        //       ]
        //     }
        //   ]
        // },
        {
          index: 3,
          sectionTitle: "巡检养护",
          rows: [
            {
              index: 1,
              items: [
                {
                  index: 2,
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
                  index: 1,
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
                  id: 90049092,
                  title: "个人工单",
                  desc: "维修任务分派",
                  mode: "horizontal",
                  picture: "./static/images/order_assignment.png",
                  pictureContainerStyle: {
                    "background-size": "80% 95%"
                  },
                  // destination: "OrderAssignment",
                  destination: "SelfTaskManagerIndex",
                  withBorder: true,
                  class: "mui-col-sm-6 mui-col-xs-6"
                },
                {
                  index: 5,
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
                // {
                //   index: 2,
                //   title: "养护任务",
                //   desc: "管网养护处理",
                //   mode: "horizontal",
                //   picture: "./static/images/conservation_mission.png",
                //   destination: "ConservationMission",
                //   withBorder: true,
                //   class: "mui-col-sm-6 mui-col-xs-6"
                // }
                {
                  index: 6,
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
                },
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
    audioSrc() {
      return this.audioVolume ? "audioTrue.png" : "audioFalse.png";
    },
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前用户id
    currentUserId() {
      return this.currentUser.iAdminID;
    },
    currentAvatar() {
      return "./static/images/avatar_male.png";
    },
    headerBgImageStyle() {
      return { "background-image": `url(${this.headerBgImagePath})` };
    },
    sectionsValue() {
      let sectionsCom = [this.sections, this.permissionJson];
      return sectionsCom;
    }
  },
  methods: {
    //权限控制要显示的页面及排序
    isShowPage() {
      let permissionList = JSON.parse(getSessionItem("currentUser"))
        .UserAuthority;
      this.permissionJson = JSON.stringify(permissionList);

      let arrParent = []; //父级对象
      let sectionsItem = this.sections[0].rows[0].items;
      _.map(permissionList, per => {
        _.map(sectionsItem, res => {
          if (res.destination == per.cFunUrl) {
            arrParent.push(_.assignIn({}, per, res));
          }
        });
      });
      //排序
      arrParent = _.orderBy(arrParent, res => {
        return res.iFunOrder;
      });
      let newPermissionJson = [];
      _.map(permissionList, res => {
        if (res.cFunUrl == 0 && res.cFunName != "APP管理") {
          newPermissionJson.push(res);
        }
      });
      console.log(newPermissionJson);
      _.map(newPermissionJson, newPer => {
        newPer.children = []; //子级对象
        _.map(arrParent, res => {
          if (res.iFunFatherID == newPer.iFunID) {
            newPer.children.push(res);
          }
        });
      });
      console.log(newPermissionJson);
      this.newPermissionJson = newPermissionJson;
    },
    //任务信息推送
    hintMsgCount() {
      this.getEventCount();
      this.getSelfTask();
      this.getOredes();
    },
    //控制播放器静音
    audioPause() {
      console.log(this.audioVolume ? "暂停" : "播放");
      let AudioId = document.getElementById("audio_index");
      if (this.audioVolume) {
        AudioId.volume = 0;
        this.audioVolume = false;
      } else {
        AudioId.volume = 1;
        this.audioVolume = true;
      }
    },
    //更新推送信息数据
    updateOrdersMsg(num, trueData) {
      console.log("//更新推送信息数据", num, trueData);
      let this_ = this;
      let AudioId = document.getElementById("audio_index");
      let AudioIdSrc = [
        "static/audio/weixiugongdan.mp3",
        "static/audio/gerengongdan.mp3",
        "static/audio/xunjianrenwu.mp3"
      ];
      Promise.all([this.getEventCount, this.getSelfTask, this.getOredes]).then(
        res => {
          console.log(this_.OrdersMsg);
          if (
            JSON.stringify(this.OrdersMsg) === "{}" &&
            this_.eventCountOrders &&
            this_.eventCountSelfTask &&
            this_.eventCountPatrol
          ) {
            this_.OrdersMsg = {
              eventCountOrders: this_.eventCountOrders,
              eventCountSelfTask: this_.eventCountSelfTask,
              eventCountPatrol: this_.eventCountPatrol
            };
            setLocalItem("OrdersMsg", JSON.stringify(this_.OrdersMsg));
          } else {
            if (trueData != undefined) {
              console.log(num);
              if (num == 1) {
                if (
                  this_.memoryCountOrders < trueData &&
                  this_.eventCountOrders != undefined
                ) {
                  //发送消息推送
                  console.log("//发送消息推送");
                  AudioId.src = AudioIdSrc[0];
                  AudioId.play();
                }
                this_.memoryCountOrders = trueData;
                this_.$set(this_.OrdersMsg, "eventCountOrders", trueData);
              } else if (num == 2) {
                console.log(this_.memoryCountSelfTask);
                console.log(trueData);
                console.log(this_.memoryCountSelfTask < trueData);
                console.log(this_.memoryCountSelfTask != undefined);
                if (
                  this_.memoryCountSelfTask < trueData &&
                  this_.eventCountSelfTask != undefined
                ) {
                  //发送消息推送
                  console.log("//发送消息推送");
                  AudioId.src = AudioIdSrc[1];
                  AudioId.play();
                }
                this_.memoryCountSelfTask = trueData;
                this_.$set(this_.OrdersMsg, "eventCountSelfTask", trueData);
              } else if (num == 3) {
                if (
                  this_.memoryCountPatrol < trueData &&
                  this_.eventCountPatrol != undefined
                ) {
                  //发送消息推送
                  console.log("//发送消息推送");
                  AudioId.src = AudioIdSrc[2];
                  AudioId.play();
                }
                this_.memoryCountPatrol = trueData;
                this_.$set(this_.OrdersMsg, "eventCountPatrol", trueData);
              }
              setLocalItem("OrdersMsg", JSON.stringify(this_.OrdersMsg));
            }
          }
        }
      );
    },
    //上传位置信息
    onUploadLocation() {
      let personIds = JSON.parse(getSessionItem("currentUser"));
      let locationInfo = JSON.parse(getSessionItem("coordsMsg")) || "";
      let personId =
        JSON.parse(getSessionItem("currentUser")).iAdminID ||
        this.currentUser.iAdminID;
      if (personId && locationInfo) {
        //坐标转换
        let YingLocation = this.mapController.transformProjReturn([
          Number(locationInfo.lng),
          Number(locationInfo.lat)
        ]);
        console.log("onUploadLocation----", YingLocation[0],
            YingLocation[1],
            dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
            personId)
        apiInspection
          .UploadLocation(
            YingLocation[0],
            YingLocation[1],
            dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
            personId,
            1
          )
          .then(res => {
            console.log("上传位置信息成功", res.data.Msg, "14444");
          });
      }
    },
    //维修工单
    getOredes() {
      let this_ = this;
      let eventCountOrders_;
      if (JSON.stringify(this.OrdersMsg) !== "{}") {
        eventCountOrders_ = this_.eventCountOrders;
      }
      return new Promise(function(resolve, reject) {
        apiMaintain
          .GetEventManage(this_.currentUserId, 2)
          .then(res => {
            if (res.data.Flag) {
              if (eventCountOrders_ != res.data.Data.TotalRows) {
                this_.updateOrdersMsg(1, res.data.Data.TotalRows);
              }
              this_.eventCountOrders = res.data.Data.TotalRows;
              this_.$set(
                this_.OrdersMsg,
                "eventCountOrders",
                this_.eventCountOrders
              );
              resolve(this_.eventCountOrders);
            } else {
              mui.toast("获取维修工单失败");
            }
          })
          .catch(err => {
            mui.toast("网络错误，请联系管理员,");
          });
      });
    },
    //个人工单-代办工单
    getSelfTask() {
      let this_ = this;
      let eventCountSelfTask_;
      if (JSON.stringify(this.OrdersMsg) !== "{}") {
        eventCountSelfTask_ = this_.eventCountSelfTask;
      }
      return new Promise(function(resolve, reject) {
        apiMaintain
          .GetOrderList(this_.currentUserId, 0)
          .then(res => {
            this_.fullscreenLoading = false;
            if (res.data.Flag) {
              if (eventCountSelfTask_ != res.data.Data.TotalRows) {
                this_.updateOrdersMsg(2, res.data.Data.TotalRows);
              }
              this_.eventCountSelfTask = res.data.Data.TotalRows;
              this_.$set(
                this_.OrdersMsg,
                "eventCountSelfTask",
                this_.eventCountSelfTask
              );
              resolve(this_.eventCountSelfTask);
            } else {
              mui.toast("获取个人工单失败");
            }
          })
          .catch(err => {
            mui.toast("网络错误，请联系管理员");
          });
      });
    },
    //监听巡检任务
    getEventCount() {
      let this_ = this;
      let eventCountPatrol_;
      if (JSON.stringify(this.OrdersMsg) !== "{}") {
        eventCountPatrol_ = this_.eventCountPatrol;
      }
      return new Promise(function(resolve, reject) {
        let userId = this_.currentUserId;
        let currentDayDate = dateHelper.format(new Date(), "yy-MM-dd");
        apiInspection
          .GetMissionList(userId, currentDayDate)
          .then(res => {
            if (res.data.Flag) {
              if (eventCountPatrol_ != res.data.Data.TotalRows) {
                this_.updateOrdersMsg(3, res.data.Data.TotalRows);
              }
              this_.eventCountPatrol = res.data.Data.TotalRows;
              this_.$set(
                this_.OrdersMsg,
                "eventCountPatrol",
                this_.eventCountPatrol
              );
              resolve(this_.eventCountPatrol);
            } else {
              mui.toast("获取巡检任务失败");
            }
            this_.$hideLoading();
          })
          .catch(err => {
            mui.toast("网络错误，请联系管理员");
          });
      });
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
};
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
.audio {
  width: 40px;
  height: 40px;
  position: fixed;
  top: 10px;
  right: 10px;
  z-index: 501;
  overflow: hidden;
}
#audio_index {
  opacity: 0;
  position: absolute;
  top: 100px;
}
</style>


