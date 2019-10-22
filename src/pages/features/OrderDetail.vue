<template>
  <div class="order_detail_container">
    <div class="order_detail_info">
      <!-- 事件内容 -->
      <el-card>
        <div slot="header" class="clearfix card_header">
          <span class="header_text">事件编号： {{orderInfo.EventCode}}</span>
        </div>
        <div class="card_body">
          <div class="left">
            <li>
              <span class="list_item_label">上报时间：</span>
              <span class="list_item_content">{{orderInfo.OrderTime}}</span>
            </li>
            <li>
              <span class="list_item_label">所属部门：</span>
              <span class="list_item_content">{{orderInfo.DeptId}}</span>
            </li>
            <li>
              <span class="list_item_label">派单人：</span>
              <span class="list_item_content">{{DispatchPerson}}</span>
            </li>
            <li>
              <span class="list_item_label">
                <el-button :type="orderInfo.UrgencyId | urgencyButtonType">{{orderInfo.UrgencyId}}</el-button>
              </span>
              <span class="list_item_content">
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
                  :style="{color: isChangeColor?'lightgreen':'#333'}"
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
              <span class="list_item_content">{{orderInfo.EventFrom}}</span>
            </li>
            <li>
              <span class="list_item_label">事件类型：</span>
              <span class="list_item_content">{{orderInfo.EventType}}</span>
            </li>
            <li>
              <span class="list_item_label">事件内容：</span>
              <span class="list_item_content">{{orderInfo.EventContent}}</span>
            </li>
            <li>
              <span class="list_item_label">联系电话：</span>
              <span class="list_item_content" v-if="isPhone"><a :href="'tel:'+orderInfo.LinkCall">{{orderInfo.LinkCall}}</a></span>
              <span class="list_item_content" v-else>{{orderInfo.LinkCall}}:无效号码</span>
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

      <!-- 延期描述 -->
      <el-card v-if="finalOrderDetail && finalOrderDetail.length > 0">
        <div slot="header" class="clearfix card_header">
          <span class="header_text">延期描述</span>
        </div>
        <el-card v-for="(item,index) in finalOrderDetail" :key="index" class="card_body">
          <div class="clearfix">
            <span>延期到：{{item.PostponeTime}}</span>
            <span class="order_detail_is_agree" :class="item.IsValid==6?'green':'red'">{{item.IsValid == 6 ? '同意' : item.IsValid == 7 ? '退回' : '暂无结果'}}</span>
          </div>
          <div class="order_detail_describe_t">
            <div>延期描述：{{item.Cause}}</div>
            <div v-if="item.Cause2 && item.Cause2.length>0">回复描述：{{item.Cause2}}</div>
          </div>
        </el-card>
      </el-card>

      <!-- 退单描述 -->
      <!-- <el-card v-if="orderInfo.EventDesc && orderInfo.EventDesc.length > 0">
        <div slot="header" class="clearfix card_header">
          <span class="header_text">退单描述</span>
        </div>
        <div class="card_body">{{orderInfo.EventDesc}}</div>
      </el-card> -->

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
        v-if="orderOperState == 8"
        class="button button-rounded button-action button-large full_width_button"
      >已 完 成</button>
      <button
        v-if="orderOperState == 2"
        class="button button-rounded button-action button-large full_width_button custom_bgcolor_dark"
        @click="onTakeOrderClick"
      >接 单</button>
      <div class="button-group" v-if="orderOperState >= 3">
        <!-- tap事件在PC端测试的时候，会有一点bug -->
         <!-- { name: "退 回", index: 0, interface: apiMaintainNew.RejectOrder }, -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-for="action in actionList"
          :key="action.name"
          v-if="!((orderInfo.EventFrom === '热线系统' && action.index===0) || (orderInfo.EventFrom !== '热线系统' && action.index===0))"
          :disabled="action.index > 2 && !(action.index === orderOperState) || tabStatus == 3"
          @click="onActionClick(action)"
        >{{action.name}}</button>
         <!-- v-if="!((orderInfo.EventFrom === '热线系统' && action.index===1) || (orderInfo.EventFrom !== '热线系统' && action.index===0))" -->
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
        <el-button @click="onActionDialogConfirm" type="primary" :disabled="!canSubmit">提 交</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import config from "@config/config";
import { getSessionItem } from "@common/util";
import apiMaintain from "@api/maintain";
import apiMaintainNew from "@api/maintain-new";
import apiMonitor from "@api/monitor";
import OrderActionsDialog from "@comp/order-detail/OrderActionsDialog.vue";
import BaseMap from "@JS/Map/BaseMap";
import CoordsHelper from "coordtransform";
import nativeTransfer from "@JS/native/nativeTransfer";

export default {
  props: {
    orderInfo: [Object, String]
  },
  mounted(){
    this.onPickDeptClick()
    console.log(this.orderInfo)
    this.poneOrderDetail()
    console.log(this.tabStatus)
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
      //显示大图片
      isPicShowP:false,
      //所选部门ID 
      DispatchPerson:null,
      defaultPicture: "./static/images/none.jpg",
      pictureBasePath: config.uploadFilePath.inspection,
      // 本页面的四种操作及其对应调用的restful api
      actionList: [
        { name: "退 回", index: 0, interface: apiMaintainNew.RejectOrder },
        { name: "退 单", index: 1, interface: apiMaintain.ChargeBackOrder },
        { name: "延 期", index: 2, interface: apiMaintain.DelayOrder },
        { name: "到 场", index: 3, interface: apiMaintain.ChangeMissionStatus },
        // { name: "维 修", index: 4, interface: apiMaintain.ChangeMissionStatus },
        { name: "完 工", index: 5, interface: apiMaintain.ChangeMissionStatus }
      ],
      orderOperState: Number(this.orderInfo.OperId) == 4 ? 5 : Number(this.orderInfo.OperId),
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
      isChangeColor:this.orderInfo.EventX,
      //任务状态中,提交按钮是否可用
      canSubmit:true,
      finalOrderDetail:[]   //延迟及退单详情
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前用户id
    currentUserId() {
      return this.currentUser.PersonId;
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
      return this.splitPicturesStr(this.orderInfo.EventPictures);
    },
    tabStatus(){
      return this.$route.query.tabStatus
    },
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
    //隐藏图片详情
    onPicHide(){
      let _this = this;
      setTimeout(function(){
        _this.isPicShowP = false;
      },100) 
    },
    //工单详情信息
    poneOrderDetail(){
      console.log("poneOrderDetail")
      apiMaintain.PoneOrderDetail(this.orderInfo.EventId,this.orderInfo.OrderId).then(data => {
        console.log(data)
        if(data.data.result){
          let finalOrderD = [];
          data.data.data2 = JSON.parse(JSON.stringify(data.data.data2).replace(/Cause/g, "Cause2"));
          _.map(data.data.data,(res1,index)=>{  
            delete data.data.data2[index].PostponeTime;
            finalOrderD.push(_.assign({},data.data.data[index],data.data.data2[index]))
          })
          console.log(finalOrderD)
          this.finalOrderDetail = finalOrderD;
        }else{
          mui.toast("获取详情数据失败！");
        }
      })
      .catch(err => {
          this.isLoading = false;
          mui.toast("网络请求超时，请稍后重试");
        });
    },
    onPickDeptClick() {
      // 调用部门接口
      apiMaintain
        .GetDepartmentList()
        .then(res => {
          if (res.data.result === true) {
            // // 打开事件分派dialog
            // this.assignDialogVisiable = true;
            let listData = res.data.data;
            console.log(listData)
            console.log(this.orderInfo.DeptId)
            let selectDept =  _.find(listData,res=>{
              return res.DeptName == this.orderInfo.DeptId
            }).DeptId;
            this.onPickPersonClick(selectDept)
          } else {
            mui.toast("获取部门数据失败！");
          }
        })
        .catch(err => {
          this.isLoading = false;
          mui.toast("网络请求超时，请稍后重试");
        });
    },
    onPickPersonClick(selectDept) {
      if (!selectDept) {
        mui.toast("请先选择部门");
      } else {
        // 调用人员接口
        // 请求指定部门人员列表
        let currentPickedDepartment = selectDept;
        console.log("当前选择的部门", selectDept);
        apiMaintain
          .GetStaffList(currentPickedDepartment)
          .then(res => {
            if (res.data.result === true) {
              let listData = res.data.data;
              console.log(listData)
              //指定的处理人员
              let DispatchPerson = _.find(listData,res=>{
                return Number(res.personId) == Number(this.orderInfo.DispatchPerson)
              });
              this.DispatchPerson = DispatchPerson ? DispatchPerson.personName:"未知"
            } else {
              mui.toast("该部门下暂无人员");
              this.isLoading = false;
            }
          })
          .catch(err => {
            this.isLoading = false;
            mui.toast("网络请求超时，请稍后重试");
          });
      }
    },
    splitPicturesStr(str) {
      if (str && str.length > 0) {
        return str.includes("|") ? str.split("|") : [str];
      } else {
        return [];
      }
    },
    GCJ02ToBD09(lng,lat){
        let x_pi = 3.14159265358979324 * 3000.0 / 180.0;
        let x = lng;
        let y = lat;
        let z =Math.sqrt(x * x + y * y) + 0.00002 * Math.sin(y * x_pi);
        let theta = Math.atan2(y, x) + 0.000003 * Math.cos(x * x_pi);
        let putlng = z * Math.cos(theta) + 0.0065;
        let putlat = z * Math.sin(theta) + 0.006;
        return [putlng,putlat];
    },
    onAddrRowClick() { 
      // 调用百度地图app导航
      if (this.orderInfo.EventX && this.orderInfo.EventY) {
        let coordinateFenghua = [];
        if(Number(this.orderInfo.EventX) > 1000){
          //初始化地图 
          let mapController = new BaseMap();
          mapController.Init("event_map");

          //地方投影转换
          coordinateFenghua = mapController.unDestinationCoordinateProj(
            [Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)]
          );
          console.log("反转coordinateFenghua",coordinateFenghua)
          coordinateFenghua = CoordsHelper.wgs84togcj02(
            coordinateFenghua[0],
            coordinateFenghua[1]
          );
          console.log("反转wgs84togcj02",coordinateFenghua)
          /*coordinateFenghua = CoordsHelper.gcj02tobd09(
            coordinateFenghua[0],
            coordinateFenghua[1]
          );
          console.log("反转gcj02tobd09",coordinateFenghua)*/
        }else{
          coordinateFenghua = [Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)]
        } 
        nativeTransfer.startNavi(Number(coordinateFenghua[0]),Number(coordinateFenghua[1]), "", res=>{
          console.log(res)
        })

        /*if (window.plus && window.plus.maps && window.plus.geolocation) {
          this.$showLoading();
          nativeTransfer.getLocation(position => {
            if(position){
              let srcPoint = new plus.maps.Point(
                position.lng,
                position.lat
              );
              let coordinateFenghua = [];
              if(Number(this.orderInfo.EventX) > 1000){
                //初始化地图 
                let mapController = new BaseMap();
                mapController.Init("event_map");
                //地方投影转换
                coordinateFenghua = mapController.unDestinationCoordinateProj(
                  [Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)]
                );
              }else{
                coordinateFenghua = [Number(this.orderInfo.EventX),Number(this.orderInfo.EventY)]
              }   
              // coordinateFenghua = CoordsHelper.wgs84togcj02(
              //   coordinateFenghua[0],
              //   coordinateFenghua[1]
              // ); 
              //coordinateFenghua = this.GCJ02ToBD09(coordinateFenghua[0],coordinateFenghua[1])            
              let destDesc = "目标设备";
              let destPoint = new plus.maps.Point(
                Number(coordinateFenghua[0]),
                Number(coordinateFenghua[1])
              );
              nativeTransfer.startNavi(Number(coordinateFenghua[0]),Number(coordinateFenghua[1]), "", res=>{
                console.log(res)
              })
              //window.plus.maps.openSysMap(destPoint, destDesc, srcPoint);
              this.$hideLoading();
            }else{
              this.$hideLoading();
              window.mui.toast("定位失败，无法调起导航");
            }
            }
          );
        }*/
      }
    },
    onTakeOrderClick() {
      window.mui.confirm(
        "确定接单吗？",
        "提醒：",
        ["取消", "确认"],
        result => {
          if (result.index === 1) {
            // 确认
            let reqData = Object.assign(
              {},
              {
                eventId:this.orderInfo.EventId,
                personId: this.currentUserId,
                orderId: this.currentOrderId,
                operationId: 3,
                description: "",
                pictureList: [],
                speechData: "",
                execPersonId:this.currentUser.PersonId,
                execDetpID:this.currentUser.DeptId
              }
            );
            apiMaintain
              .ChangeMissionStatus(reqData)
              .then(res => {
                if (res.data[0].result === true) {
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
      let hasDesc = actionData.description.length > 0;
      let hasPicture = actionData.pictureList.length > 0;
      let hasDate = actionData.date.length > 0;
      let hasDescOption = actionData.descOption instanceof Object;
      if (this.activeAction.index == 0) {
        // 如果是退回，给出提示，描述必填
        mui.toast("请填写本退回描述信息");
        return;
      } else if (this.activeAction.operationId === 2) {
        // 如果是退单
        if (!hasDescOption) {
          mui.toast("请选择一个退单原因");
          return;
        }
      } else if (this.activeAction.operationId === 4) {
        if (!hasDescOption) {
          mui.toast("请选择一个到场描述");
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
      let reqData = Object.assign({}, actionData, {
        eventId:this.orderInfo.EventId,
        personId: this.currentUserId,
        orderId: this.currentOrderId,
        eventType: this.currentOrderEventType,
        operationId: this.activeAction.operationId,
        execPersonId:this.currentUser.PersonId,
        execDetpID:this.currentUser.DeptId
      });
      console.log(`${this.activeAction.actionName} req:`, reqData);
      this.$showLoading();
      // 发送请求
      if(this.canSubmit){
        if(hasPicture){
          this.canSubmit = false;
        } 
        this.activeAction
          .interface(reqData)
          .then(res => {
            console.log("步骤操作请求的数据： ", reqData);
            console.log("步骤操作返回的数据： ", res);
            this.actionDialogVisible = false;
            this.canSubmit = true;
            this.$hideLoading();
            if (res.data[0].result === true) {
              mui.toast("操作成功！");
              if (
                this.activeAction.operationId === 2 ||
                this.activeAction.index == 0 || 
                this.activeAction.operationId === 3
              ) {
                // 如果当前是退单或者退回操作，则成功后返回上一级
                this.$router.go(-1);
              }
              if (![2, 3].includes(this.activeAction.operationId)) {
                if([4].includes(this.activeAction.operationId)){
                  this.orderOperState = this.orderOperState+2 ;
                }else{
                  this.orderOperState++;
                }
              }
              else{
                if([2].includes(this.activeAction.operationId)){
                  this.$router.go(-1);
                }
              }
            } else {
              mui.toast(`操作失败！${res.data.data[0].message}`);
            }
          })
          .catch(err => {
            this.actionDialogVisible = false;
            this.canSubmit = true;
            this.$hideLoading();
            mui.toast("服务器异常，请稍后再试");
            console.log(err);
          });
        }
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
            margin-top: 3px;
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
.order_detail_is_agree{float:right;}
.order_detail_is_agree.red{color:red;}
.order_detail_is_agree.green{color:green;}
.order_detail_describe_t div{border-top:1px solid #eee;margin-top:5px;padding-top:5px;}
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


