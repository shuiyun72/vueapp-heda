<template>
  <div class="self_task_manager_detail_container">
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
              <span class="list_item_content">{{orderInfo.UpTime}}</span>
            </li>
            <li>
              <span class="list_item_label">上报人</span>
              <span class="list_item_content">{{orderInfo.PersonName}}</span>
            </li>
            <li>
              <span class="list_item_label">处理人</span>
              <span class="list_item_content">{{orderInfo.ExecPersonName}}</span>
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
              <span class="list_item_label">联系人：</span>
              <span class="list_item_content">{{orderInfo.LinkMan}}</span>
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
            <li>
              <span class="list_item_label">当前进度：</span>
              <span class="list_item_content">{{'已' + orderInfo.OperName}}</span>
            </li>
            <li>
              <span class="list_item_label">工单状态：</span>
              <span class="list_item_content">{{orderInfo.OperName2}}</span>
            </li>
            <li>
              <span class="list_item_label">
                <el-button
                  :type="orderInfo.UrgencyName | urgencyButtonType"
                >{{orderInfo.UrgencyName}}</el-button>
              </span>
              <!-- <span class="list_item_content">
                <el-button
                  :type="orderInfo.HandlerLevelId | levelButtonType"
                >{{orderInfo.HandlerLevelId}}</el-button>
              </span>-->
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
        v-if="orderOperState == 7"
        class="button button-rounded button-action button-large full_width_button"
      >已 完 成</button>
      <div class="button-group" v-if="orderOperState >= 3">
        <!-- tap事件在PC端测试的时候，会有一点bug -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-if="false"
          @click="actionDialogVisible.reply = true"
        >回复</button>
        <!-- v-if="isCurrentExecPerson && orderOperState == 11" -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-if="isCurrentExecPerson && orderOperState == 11"
          @click="actionDialogVisible.assign = true"
        >分派</button>
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-if="false"
          @click="actionDialogVisible.reject=true"
        >退回</button>
         <!-- v-if="isCurrentExecPerson && orderOperState == 11" -->
        <!-- operId 13 -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-if="false"
          @click="onDialogButtonClick('postpone')"     
        >延期确认</button>
         <!-- v-if="isCurrentExecPerson && (orderInfo.IsValid == 5 || orderOperState == 13)" -->
        <button
          type="button"
          class="button custom_bgcolor_light"
          v-if=" currentUserId == orderInfo.DispatchPerson && orderOperState == 6"
          @click="onDialogButtonClick('audit')"   
        >审核</button>
      </div>
    </div>
    <!-- 回复弹出框 -->
    <el-dialog
      title="回复"
      :visible.sync="actionDialogVisible.reply"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <div v-if="actionDialogVisible.reply" class="dialog_content dialog_content-reply">
        <div>回复信息：</div>
        <el-input
          name="reply"
          type="textarea"
          :autosize="{minRows: 5, maxRows: 7}"
          v-model="replyMessage"
          placeholder="请输入回复信息"
        ></el-input>
      </div>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button type="primary" @click="onReplyConfirmButtonClick">提 交</el-button>
      </div>
    </el-dialog>
    <!-- 退回弹出框 -->
    <el-dialog
      title="退回"
      :visible.sync="actionDialogVisible.reject"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <div v-if="actionDialogVisible.reject" class="dialog_content dialog_content-reject">
        <div>退回信息：</div>
        <el-input
          name="reject"
          type="textarea"
          :autosize="{minRows: 5, maxRows: 7}"
          v-model="rejectMessage"
          placeholder="请输入退回信息"
        ></el-input>
      </div>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button type="primary" @click="onRejectConfirmButtonClick">提 交</el-button>
      </div>
    </el-dialog>
    <!-- 分派弹出框 -->
    <el-dialog
      title="分派"
      :visible.sync="actionDialogVisible.assign"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <div v-if="actionDialogVisible.assign" class="dialog_content dialog_content-assign">
        <div class="list_row">
          <span>部门：</span>
          <span @click="onPickDeptClick">{{assignPickerValue.department.text || '点击选择部门'}}</span>
        </div>
        <div class="list_row">
          <span>人员：</span>
          <span @click="onPickPersonClick">{{assignPickerValue.person.text || '点击选择人员'}}</span>
        </div>
      </div>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button type="primary" @click="onAssignConfirmButtonClick">提 交</el-button>
      </div>
    </el-dialog>
    <!-- 审核弹出框 -->
    <el-dialog
      title="审核"
      :visible.sync="actionDialogVisible.audit"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <div v-if="actionDialogVisible.audit" class="dialog_content dialog_content-assign">
        <div class="list_row">
          <span>操作意见：</span>
          <el-input
            type="textarea"
            :autosize="{minRows: 2, maxRows:6}"
            v-model="rejectMessage"
            placeholder="请输入操作意见"
          ></el-input>
        </div>
        <div class="list_row">
          <span>满意度：</span>
          <el-input
            type="textarea"
            :autosize="{minRows: 1, maxRows:2}"
            v-model="satisfyMessage"
            placeholder="请输入满意度"
          ></el-input>
        </div>
      </div>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button type="primary" @click=" onCheckOrderButtonClick">提 交</el-button>
      </div>    
    </el-dialog>
    <!-- 延期确认弹出框 -->
    <el-dialog
      title="延期确认"
      :visible.sync="actionDialogVisible.postpone"
      width="80%"
      @close="onActionDialogClose"
      center
    >
      <div v-if="actionDialogVisible.postpone" class="dialog_content dialog_content-assign">
        <div class="list_row">
          <span>操作意见：</span>
          <el-input
            type="textarea"
            :autosize="{minRows: 2, maxRows:6}"
            v-model="rejectMessage"
            placeholder="请输入操作意见"
          ></el-input>
        </div>
      </div>
      <div slot="footer">
        <el-button @click="onActionDialogCancel">取 消</el-button>
        <el-button type="primary" @click="onCheckDelayButtonClick ">提 交</el-button> 
      </div>
    </el-dialog>
  </div>
</template>
<script>
import _ from "lodash";
import { getSessionItem } from "@common/util";
import apiMaintain from "@api/maintain";
import OrderActionsDialog from "@comp/order-detail/OrderActionsDialog.vue";
// 引入nativeTransfer.js
import nativeTransfer from '@JS/native/nativeTransfer'
import BaseMap from "@JS/Map/BaseMap";

export default {
  props: {
    oriOrderInfo: [Object, String]
  },
  beforeRouteEnter(to, from, next) {
    // 进入该路由的钩子函数，可通过next回调函数拿到组件的实例对象引用
    next(instance => {
      console.log("orderInfo", instance.orderInfo);
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
          if (
            Object.keys(instance.actionDialogVisible).some(visible => {
              return visible === true;
            })
          ) {
            // 如果当前有弹出框，则返回键会关闭弹出框
            Object.keys(this.actionDialogVisible).forEach((value, index) => {
              this.actionDialogVisible[value] = false;
            });
          } else {
            // 如果当前没有弹出框，则使用默认返回逻辑
            defaultFunction();
          }
        });
      }
      
    });
  },
  created(){
     this.refreshOrderDetail();
  },
  mounted() {
    // 实例化picker组件
    this._popPicker = new window.mui.PopPicker({ layer: 1 });
  },
  data() {
    return {
      isPicShowP:false,
      orderInfo: {},
      defaultPicture: "./static/images/none.jpg",
      // pictureBasePath: process.env.API_ROOT+'/api',
      pictureBasePath: process.env.API_ROOT,
      // 订单操作弹出框是否弹出
      actionDialogVisible: {
        assign: false,
        reply: false,
        reject: false,
        audit:false,
        postpone:false
      },

      // 弹出框相关数据
      assignPickerValue: {
        department: {
          text: "",
          value: ""
        },
        person: {
          text: "",
          value: ""
        }
      },
      replyMessage: "",
      rejectMessage: "",
      satisfyMessage:"",  //满意度
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
    // 当前用户是否是该单的处理人
    isCurrentExecPerson() {
      return this.currentUserId === this.orderInfo.ExecPersonId
    },
    pictureList() {
      return _.pull(this.splitPicturesStr(this.orderInfo.EventPictures),"");
    },
    orderOperState() {
      return Number(this.orderInfo.OperId);
    },
    isPhone(){
      let phone = !(/^([1]\d{10}|([\(（]?0[0-9]{2,3}[）\)]?[-]?)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?)$/.test(this.orderInfo.LinkCall));
      if(phone){                                                                                                                        
        return false
      }else{
        return true
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
    refreshOrderDetail() {
      apiMaintain.GetEventDetailInfo(this.oriOrderInfo.EventID).then(res => {
        console.log("刷新工单详情信息res", res);
        if (res.data.Flag) {
          let detailInfo = res.data.Data[0];
          detailInfo = _.assignIn({},this.oriOrderInfo,detailInfo);
          console.log(detailInfo)
          this.orderInfo = detailInfo;
        }
      });
    },
    // 拿到列表数据后打开picker，并处理后续选择逻辑
    openPicker(listData, keys, callback) {
      this.$hideLoading();
      let pickerListItems = listData.map(item => {
        return { value: item[keys.valueKey], text: item[keys.textKey] };
      });
      this._popPicker.setData(pickerListItems);
      this._popPicker.show(pickedItems => {
        let pickedItem = pickedItems[0];
        if (callback instanceof Function) {
          callback(pickedItem);
        }
      });
    },
    splitPicturesStr(str) {
      if (str && str.length > 0) {
        return str.includes("|") ? str.split("|") : [str];
      } else {
        return [];
      }
    },
    onPickDeptClick() {
      // 调用部门接口
      apiMaintain
        .GetDepartmentList()
        .then(res => {
          if (res.data.Flag) {
            // // 打开事件分派dialog
            // this.assignDialogVisiable = true;
            let listData = res.data.Data.Result;
            this.openPicker(
              listData,
              {
                valueKey: "iDeptID",
                textKey: "cDepName"
              },
              pickedItem => {
                this.assignPickerValue.department = pickedItem;
                console.log(this.assignPickerValue.department)
              }
            );
          } else {
            mui.toast("获取部门数据失败！");
          }
        })
        .catch(err => {
          this.isLoading = false;
          mui.toast("网络请求超时，请稍后重试");
        });
    },
    onPickPersonClick() {
      if (!this.assignPickerValue.department.value) {
        mui.toast("请先选择部门");
      } else {
        // 调用人员接口
        // 请求指定部门人员列表
        let currentPickedDepartment = this.assignPickerValue.department.value;
        console.log("当前选择的部门", this.assignPickerValue.department);
        apiMaintain
          .GetStaffList(currentPickedDepartment)
          .then(res => {
            if (res.data.Flag) {
              let listData = res.data.Data.Result;
              this.openPicker(
                listData,
                {
                  valueKey: "iAdminID",
                  textKey: "cAdminName"
                },
                pickedItem => {
                  this.assignPickerValue.person = pickedItem;
                }
              );
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
    // 工单回复
    onReplyConfirmButtonClick() {
      if (!this.replyMessage) {
        mui.toast("请输入回复消息");
      } else {
        // 调用回复接口
        apiMaintain
          .PostReplyMessage(
            this.orderInfo.EventID,
            this.orderInfo.OrderId,
            this.currentUser.iAdminID,
            this.replyMessage
          )
          .then(res => {
            console.log("回复接口res", res);
            if (res.data.Flag) {
              mui.toast("回复成功！");
              this.actionDialogVisible.assign = false;
              // 刷新详情
              this.refreshOrderDetail();
            } else {
              mui.toast("回复失败，请稍后重试");
            }
          })
          .catch(err => {
            mui.toast("回复失败，请稍候再试");
          });
      }
    },
    // 工单退回
    onRejectConfirmButtonClick() {
      if (!this.rejectMessage) {
        mui.toast("请输入退回消息");
      } else {
        // 调用退回接口
        //EventID, iAdminID, BackDesc,PersonId,DeptId
        apiMaintain
          .RejectOrder(
            this.orderInfo.EventID,
            this.currentUserId,
            this.rejectMessage,
            this.orderInfo.ExecPersonId,
            this.orderInfo.DeptId 
          )
          .then(res => {
            console.log("退回接口res", res);
            if (res.data.Flag) {
              mui.toast("退回成功！");
              this.$router.go(-1);
              this.actionDialogVisible.reject = false;
              // 刷新详情
              this.refreshOrderDetail();
            } else {
              mui.toast("退回失败，请稍后重试");
            }
          })
          .catch(err => {
            mui.toast("退回失败，请稍候再试");
          });
      }
    },
    // 工单分派提交
    onAssignConfirmButtonClick() {
      if (
        this.assignPickerValue.department.value &&
        this.assignPickerValue.person.value
      ) {
        // 调用分派接口
        apiMaintain
          .AssignOrder(
            this.currentUserId,
            this.orderInfo.EventID,
            this.assignPickerValue.department.value,
            this.assignPickerValue.person.value
          )
          .then(res => {
            console.log("分派！！", res);
            if (res.data.Flag) {
              mui.toast("分派成功！");
              this.actionDialogVisible.assign = false;
              this.$router.go(-1);
              // 刷新详情
              this.refreshOrderDetail();
            } else {
              mui.toast("分派失败，请稍后重试");
            }
          })
          .catch(err => {
            mui.toast("分派失败！");
          });
      } else {
        mui.toast("请选择部门与人员");
      }
    },
    //审核及延期弹出框
    onDialogButtonClick(el){
      //audit  postpone
      //审核
      if(el == "audit"){
        this.actionDialogVisible.audit = true;
      }
      //延期
      else if(el == "postpone"){
        this.actionDialogVisible.postpone = true;
      }
    },
    // 审核
    onCheckOrderButtonClick() {
      this.$showLoading();
      console.log("审核")
      apiMaintain
        .CheckOrder(
          this.orderInfo.EventID,
          this.orderInfo.OrderId,
          this.currentUser.iDeptID,
          this.currentUserId,
          this.rejectMessage,
          this.satisfyMessage
        )
        .then(res => {
          console.log("审核res", res);
          if (res.data.Flag) {
            mui.toast("审核完成！");
            this.$hideLoading();
            this.$router.go(-1);
            // 刷新详情
            this.refreshOrderDetail();
          } else {
            mui.toast("提交审核失败，请稍后重试");
          }
        })
        .catch(err => {
          mui.toast("提交审核失败！");
        });
    },
    // 延期确认
    onCheckDelayButtonClick() {
       console.log("延期确认")
      apiMaintain
        .GetEventDetailInfo(this.orderInfo.EventID)
        .then(yanMsg => {
          this.$showLoading();
          console.log("获取到延期申请信息", yanMsg);
          if(yanMsg.data.Flag){
            let DelayTime = _.filter(yanMsg.data.Data.Result,msg=>{
              return msg.IsValid == 5
            })[0].PostponeTime;
            apiMaintain
            .CheckOrderDelay(
              this.orderInfo.EventID,
              this.orderInfo.OrderId,
              this.currentUser.iDeptID,
              this.currentUserId,
              this.rejectMessage,
              DelayTime)
            .then(res => {
              console.log("延期确认res", res);
              this.$hideLoading();
              if (res.data.Flag) {
                mui.toast("确认成功！");
                this.$router.go(-1);
                // 刷新详情
                this.refreshOrderDetail();
              } else {
                mui.toast(res.data.ErrInfo);
              }
            })
            .catch(err => {
              this.$hideLoading();

              mui.toast("延期确认失败！");
            });

          }else{
            mui.toast("获取延期信息失败！");
          }
          /*let data = res.data.rows[0];
          let time = data.ApplicationTime;
          let personId = this.currentUserId;
          let eventId = data.EventID;
          let orderId = data.OrderId;
          let desc = data.Cause;*/
          /*apiMaintain
            .CheckOrderDelay(personId, eventId, orderId, time)
            .then(res => {
              console.log("延期确认res", res);
              this.$hideLoading();
              if (res.data.ErrCode == 0) {
                mui.toast("确认成功！");
                // 刷新详情
                this.refreshOrderDetail();
              } else {
                mui.toast(res.data.ErrInfo);
              }
            })
            .catch(err => {
              this.$hideLoading();

              mui.toast("延期确认失败！");
            });*/
        });
    },
    onAddrRowClick() {
      // 调用百度地图app导航
      if (this.orderInfo.EventX && this.orderInfo.EventY) {
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
      }
    },

    onActionDialogClose() {
      // 清空工单分派的数据
      this.assignPickerValue.department = {};
      this.assignPickerValue.person = {};
      //   清空回复消息
      this.replyMessage = "";
    },
    onActionDialogCancel() {
      console.log(Object.keys(this.actionDialogVisible));
      Object.keys(this.actionDialogVisible).forEach((value, index) => {
        this.actionDialogVisible[value] = false;
      });
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
.self_task_manager_detail_container {
  /* 覆盖默认padding */
  div.el-dialog__body {
    padding: 3%;
  }
  //   3/7分的表单行
  .list_row {
    margin: 3%;
    & > span {
      display: inline-block;
      &:nth-child(1) {
        width: 30%;
        color: #999;
      }
      &:nth-child(2) {
        width: 68%;
      }
    }
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
          & > span {
            display: inline-block;
          }
        }
        .addr_span {
          display: flex;
          .position_icon {
            margin-right: 2%;
          }
        }
        .list_item_label {
          color: #999;
          width: 30%;
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

