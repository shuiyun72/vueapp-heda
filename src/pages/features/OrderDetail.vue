<template>
  <div class="order_detail_container">
    <div class="order_detail_info">
      <!-- 事件内容 -->
      <el-card>
        <div slot="header" class="clearfix card_header">
          <span class="header_text">工单编号： {{orderInfo.OrderCode}}</span>
        </div>
        <div class="card_body">
          <div class="left">
            <li>
              <span class="list_item_label">事件编号：</span>
              <span class="list_item_content">{{orderInfo.EventCode}}</span>
            </li>
            <li>
              <span class="list_item_label">上报时间：</span>
              <span class="list_item_content">{{orderInfo.UpTime}}</span>
            </li>
            <li>
              <span class="list_item_label">所属部门：</span>
              <span class="list_item_content">{{orderInfo.cDepName}}</span>
            </li>
            <li>
              <span class="list_item_label">派单人：</span>
              <span class="list_item_content">{{orderInfo.ExecPersonName}}</span>
            </li>
            <li>
              <span class="list_item_label">
                <el-button :type="orderInfo.UrgencyName | urgencyButtonType">{{orderInfo.UrgencyName}}</el-button>
              </span>
              <span class="list_item_content" v-if="orderInfo.HandlerLevelId">
                <el-button
                  :type="orderInfo.HandlerLevelId | levelButtonType"
                >{{orderInfo.HandlerLevelId}}</el-button>
              </span>
            </li>
            <li @tap="onAddrRowClick">
              <!-- 点击位置信息，可以唤起手机上第三方地图app -->
              <span class="list_item_label addr_span">
                <span
                  class="list_item_icon fas fa-map-marker-alt position_icon"
                  style="color: lightgreen;"
                ></span>
                <span
                  class="address_info"
                  :style="{color: orderInfo.EventAddress ? '#001d26' : '#aaa'}"
                >{{orderInfo.EventAddress || '暂无位置信息'}}</span>
              </span>
              <span class="list_item_content"></span>
            </li>
            <li>
              <span class="list_item_label">事件来源：</span>
              <span class="list_item_content">{{orderInfo.EventFromName}}</span>
            </li>
            <li>
              <span class="list_item_label">事件类型：</span>
              <span class="list_item_content">{{orderInfo.EventTypeName}}</span>
            </li>
            <li>
              <span class="list_item_label">事件内容：</span>
              <span class="list_item_content">{{orderInfo.EventTypeName2}}</span>
            </li>
            <li>
              <span class="list_item_label">联系电话：</span>
              <span class="list_item_content" v-if="isPhone">
                <a :href="'tel:'+orderInfo.LinkCall">{{orderInfo.LinkCall}}</a>
              </span>
              <span class="list_item_content" v-else>
                {{orderInfo.LinkCall}}:联系电话不可用 
              </span>
            </li>
          </div>
        </div>
      </el-card>
      <!-- 事件描述 -->
      <el-card v-if="orderInfo.EventDesc && orderInfo.EventDesc.length > 0">
        <div slot="header" class="clearfix card_header">
          <span class="header_text">事件描述</span>
        </div>
        <div class="card_body">{{orderInfo.EventDesc}}</div>
      </el-card>
      <!-- 现场图片 -->
      <el-card v-if="pictureList && pictureList.length > 0">
        <div slot="header" class="clearfix card_header">
          <span class="header_text">现场图片</span>
        </div>
        <div class="card_body picture_list">
          <img
            v-for="(pic, index) in pictureList"
            :key="pic"
            :src="pictureBasePath + pic"
            @tap="onImageClick(index)"
          >
        </div>
      </el-card>
      <div class="flex_pic_p" @tap="onPicHide" v-show="isPicShowP">
        <img src="" class="flex_pic_img_p">
      </div>
    </div>
    <!-- 底部按钮组 -->
    <div class="fixed_footer">
      <button
        v-if="orderOperState == 6"
        class="button button-primary button-rounded button-large full_width_button"
      >审 核 中</button>
      <button
        v-if="orderOperState == 7"
        class="button button-rounded button-action button-large full_width_button"
      >已 完 成</button>
      <button
        v-if="orderOperState == 2"
        class="button button-rounded button-action button-large full_width_button custom_bgcolor_dark"
        @click="onTakeOrderClick"
      >接 单</button>
      <div class="button-group" v-if="orderOperState >= 3">
        <!-- tap事件在PC端测试的时候，会有一点bug -->
         <!-- v-if="!((orderInfo.EventFrom === '热线系统' && action.index===1) || (orderInfo.EventFrom !== '热线系统' && action.index===0))" -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-for="action in actionList"
          v-if="action.index !=0 "
          :key="action.name"
          :disabled="action.index > 2 && !(action.index === orderOperState)"
          @click="onActionClick(action)"
        >{{action.name}}</button>
      </div>
    </div>
    <!-- 操作弹出框 -->
    <el-dialog
      :title="activeAction.actionName"
      :visible.sync="actionDialogVisible"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <OrderActionsDialog :actionType="activeAction.actionTypeIndex" ref="actionDialogContent"></OrderActionsDialog>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button @click="onActionDialogConfirm" type="primary">提 交</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import _ from "lodash";
import { getSessionItem } from "@common/util";
import apiMaintain from "@api/maintain";
import apiMonitor from "@api/monitor";
import OrderActionsDialog from "@comp/order-detail/OrderActionsDialog.vue";
// 引入nativeTransfer.js
import nativeTransfer from '@JS/native/nativeTransfer';
// import encodeHelper from "@common/encodeHelper";
import BaseMap from "@JS/Map/BaseMap";

export default {
  props: {
    orderInfo: [Object, String]
  },
  beforeRouteEnter(to, from, next) {
    // 进入该路由的钩子函数，可通过next回调函数拿到组件的实例对象引用
    next(instance => {
      /* 
        在本页面直接刷新页面时，路由参数会变成String从而使页面内容无法加载，
        这里判断props的类型正确性，如果不正确，后退到上个页面
      */
      if (typeof instance.orderInfo === "string") {
        // 路由参数类型正确
        instance.$router.go(-1);
      } else {
        // 路由参数类型错误
        // 定制该路由下的设备返回键逻辑
        instance.$defineDeviceBack(defaultFunction => {
          if (instance.actionDialogVisible) {
            // 如果当前有弹出框，则返回键会关闭弹出框
            instance.actionDialogVisible = false;
          } else {
            // 如果当前没有弹出框，则使用默认返回逻辑
            defaultFunction();
          }
        });
      }
    });
  },
  data() {
    return {
      isPicShowP:false,
      defaultPicture: "./static/images/none.jpg",
      // pictureBasePath:  process.env.API_ROOT+'/api',
      pictureBasePath:  process.env.API_ROOT,
      // 本页面的四种操作及其对应调用的restful api
      actionList: [
        { name: "退 回", index: 0, interface: apiMaintain.RejectOrder },
        { name: "退 单", index: 1, interface: apiMaintain.ChargeBackOrder },
        { name: "延 期", index: 2, interface: apiMaintain.DelayOrder },
        { name: "到 场", index: 3, interface: apiMaintain.ChangeMissionStatus },
        { name: "维 修", index: 4, interface: apiMaintain.ChangeMissionStatus },
        { name: "完 工", index: 5, interface: apiMaintain.ChangeMissionStatus }
      ],
      orderOperState: Number(this.orderInfo.OperId),
      orderStatus: Number(this.orderInfo.OrderStatus),
      // 订单操作弹出框是否弹出
      actionDialogVisible: false,
      // （对于进行中的订单）当前正在进行的操作
      activeAction: {
        // String 操作名称
        actionName: "",
        // Int (1~5: 退单、延期、到场、维修、完工 )
        actionTypeIndex: ""
      },
      DeptIDList:[],
      orderInfoDeptId:""
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前用户id
    currentUserId() {
      return this.currentUser.iAdminID;
    },
    // 当前订单id
    currentOrderId() {
      return this.orderInfo.OrderId;
    },
    // 当前订单的事件类型
    currentOrderEventType() {
      return this.orderInfo.EventType;
    },
    pictureList() {
      return _.pull(this.splitPicturesStr(this.orderInfo.EventPictures),"");   
    },
    //验证电话是否符合
    isPhone(){
      let phone = /^([1]\d{10}|([\(（]?0[0-9]{2,3}[）\)]?[-]?)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?)$/.test(this.orderInfo.LinkCall);
      if(phone){                                                                                                                        
        return true
      }else{
        return false
      }   
    }
  },
  methods: { 
    onPicHide(){
      let _this = this;
      setTimeout(function(){
        _this.isPicShowP = false;
      },100) 
    },
    splitPicturesStr(str) {
      if (str && str.length > 0) {
        return str.includes("|") ? str.split("|") : [str];
      } else {
        return [];
      }
    },
    onAddrRowClick() {
      let mapController = new BaseMap();
      mapController.Init("map");
      // 调用百度地图app导航
      if (this.orderInfo.EventX && this.orderInfo.EventY) {
        let newPosition = Number(this.orderInfo.EventX) > 200 ? 
            mapController.transformProjTurn([Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)]) : 
            [Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)];  
        console.log(newPosition)
        nativeTransfer.startNavi(newPosition[0], newPosition[1], "");
      }
    },
    onTakeOrderClick() {
      console.log(this.orderInfo)
      window.mui.confirm(
        "确定接单吗？",
        "提醒：",
        ["取消", "确认"],
        result => {
          if (result.index === 1) {
            // 确认
            let reqData = 
              {
                EventID:this.orderInfo.EventID,
                StepNum:3,
                OrderId:this.currentOrderId,
                DispatchPersonID:"",
                OperRemarks:"",
                ExecPersonId:this.currentUser.iAdminID,
                ExecDetpID:this.currentUser.iDeptID
              };
            console.log(reqData)
            apiMaintain
              .ChangeMissionStatus(reqData,"")
              .then(res => {
                console.log(res)
                if (res.data.Flag) {
                  this.orderOperState++;
                  mui.toast("接单成功！");
                } else {
                  mui.toast(`接单失败！${res.data.data[0].message}`);
                }
              })
              .catch(err => {
                mui.toast("服务器异常，请稍后再试");
                console.log(err);
              });
          } else {
            // 取消
          }
        },
        "div"
      );
    },
    onActionClick(action) {
      this.activeAction = {
        actionTypeIndex: action.index,
        actionName: action.name,
        operationId: action.index + 1,
        interface: action.interface
      };
      console.log(this.activeAction);
      this.actionDialogVisible = true;
    },
    onActionDialogClose() {
      // 清空弹出框内组件的输入值
      this.$refs.actionDialogContent.reset();
    },
    onActionDialogConfirm() {
      let actionData = this.$refs.actionDialogContent.getValue();
      console.log(actionData)
      let hasDesc = actionData.description.length > 0;
      let hasPicture = actionData.pictureList.length > 0;
      let hasDate = actionData.date.length > 0;
      let hasDescOption = actionData.descOption instanceof Object;
      let pictureListStr = actionData.pictureList;
      if (this.activeAction.index == 0) {
        // 如果是退回，给出提示，描述必填
        mui.toast("请填写本退回描述信息");
      } else if (this.activeAction.operationId === 2) {
        // 如果是退单
        if (!hasDescOption) {
          mui.toast("请选择一个退单原因");
          return;
        }
      } else if (this.activeAction.operationId == 3){
        if (!hasDesc) {
          mui.toast("请填写延期原因");
          return;
        }
      } else if (this.activeAction.operationId === 4) {
        if (!hasDescOption) {
          mui.toast("请选择一个到场描述");
          return;
        }
        if (!hasPicture) {
          mui.toast("请上传至少一张图片");
          return;
        }
      } else if (
        [
          // 完工
          6
        ].includes(this.activeAction.operationId)
      ) {
        if (!hasDesc) {
          mui.toast("请填写本步骤描述信息");
          return;
        }
        if (!hasPicture) {
          mui.toast("请上传至少一张图片");
          return;
        }
      }
      // 组装请求数据
      if (hasDescOption) {
        let option = actionData.descOption.label;
        let desc = option;
        if (hasDesc) {
          desc = option + "，" + actionData.description;
        }
        actionData.description = desc;
      }
      /*let reqData = Object.assign({}, actionData, {
        personId: this.currentUserId,
        orderId: this.currentOrderId,
        eventType: this.currentOrderEventType,
        operationId: this.activeAction.operationId
      });
      console.log(`${this.activeAction.actionName} req:`, reqData);*/
      this.$showLoading();
      console.log(actionData)
      let reqData = 
        {
          EventID:this.orderInfo.EventID,
          StepNum:this.activeAction.operationId,
          OrderId:this.currentOrderId,
          DispatchPersonID:this.currentUser.iDeptID,
          OperRemarks:actionData.description,
          iAdminID:this.currentUser.iAdminID,
          PersonId:this.orderInfo.ExecPersonId,
          DeptId:this.orderInfo.DeptId,
          complishTime:actionData.date,
          ExecPersonId:this.currentUser.iAdminID,
          ExecDetpID:this.currentUser.iDeptID
        };
       console.log(reqData)
      // 发送请求
      console.log(this.activeAction.interface)
       this.activeAction.interface(reqData,pictureListStr)
        .then(res => {
          console.log("步骤操作请求的数据： ", reqData);
          console.log("步骤操作返回的数据： ", res);
          this.actionDialogVisible = false;
          this.$hideLoading();
          if (res.data.Flag) {
            mui.toast("操作成功！");
            if (
              this.activeAction.operationId === 2 ||
              this.activeAction.index == 0
            ) {
              // 如果当前是退单或者退回操作，则成功后返回上一级
              this.$router.go(-1);
            }
            if (![2, 3].includes(this.activeAction.operationId)) {
              this.orderOperState++;
            }
          } else {
            mui.toast(`操作失败！${res.data.data[0].message}`);
          }
        })
        .catch(err => {
          this.actionDialogVisible = false;
          this.$hideLoading();
          mui.toast("服务器异常，请稍后再试");
          console.log(err);
        });
    },
    onActionDialogCancel() {
      this.actionDialogVisible = false;
    },
    // 预览图片
    onImageClick(index = 0) {
      let list = this.pictureList.map(url => {
        return `${this.pictureBasePath}${url}`;
      });
      if(window.plus){
        plus.nativeUI.previewImage(list, { current: index });
      }else{
        let picImg = document.getElementsByClassName("flex_pic_img_p")[0];
        picImg.src = list[index];
        this.isPicShowP = true;
      }
    }
  },
  filters: {
    urgencyButtonType(urgency) {
      let buttonType = "primary";
      switch (urgency) {
        case "一般":
          buttonType = "primary";
          break;
        case "紧急":
          buttonType = "warning";
          break;
        case "加急":
          buttonType = "danger";
          break;
      }
      return buttonType;
    },
    levelButtonType(level) {
      let buttonType = "primary";
      if (typeof level === "string") {
        if (level.startsWith("2小时")) {
          buttonType = "danger";
        } else if (level.startsWith("4小时")) {
          buttonType = "warning";
        }
      }
      return buttonType;
    }
  },
  components: { OrderActionsDialog }
};
</script>

<style lang="less">
.order_detail_container {
  /* 覆盖默认padding */
  div.el-dialog__body {
    padding: 3%;
  }
  .order_detail_info {
    margin-bottom: 10vh;
    .el-card {
      margin-bottom: 1%;
    }
    .card_header {
      font-size: 1.4rem;
    }
    .card_body {
      font-size: 1.2rem;
      .left {
        li {
          list-style-type: none;
          margin: 3% 0;
        }
        .addr_span {
          display: flex;
          .position_icon {
            margin-right: 2%;
          }
        }
        .list_item_label {
          color: #999;
        }
      }
    }
    .picture_list {
      display: flex;
      flex-flow: row wrap;
      justify-content: start;
      img {
        width: 35vw;
        height: 40vw;
        margin-bottom: 4%;
        margin-left: 2%;
        border: 1px solid royalblue;
        border-radius: 10px;
      }
    }
  }
  .fixed_footer {
    position: fixed;
    bottom: 0;
    width: 100%;
    height: 9vh;
    .full_width_button {
      width: 100%;
      height: inherit;
      margin: 0 auto;
    }
    .button-group {
      width: inherit;
      height: inherit;
      display: flex;
      flex-flow: row nowrap;
      button {
        flex-grow: 1;
        height: inherit;
        padding: 0;
      }
    }
  }
}
.flex_pic_p{
  width: 100%;
  height: calc(100vh);
  background: #eee;
  z-index: 1000;
  position:fixed;
  top: 0;
  left: 0;
  display: flex;
  justify-content:center;
  align-items: center
}
.flex_pic_img_p{
  width: 100%;
  position: absolute;
}
</style>


