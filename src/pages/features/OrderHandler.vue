<template>
    <div class="order_handler_container">
        <div class="order_detail_info">
            <!-- 事件内容 -->
            <el-card class="custom_el-card-header_color_358F8B">
              <div slot="header" class="clearfix card_header">
                <span class="header_text">事件编号： {{orderInfo.EventCode}}</span>
              </div>
              <div class="card_body">
                <div class="left">
                  <li><span class="list_item_label">上报时间： </span><span class="list_item_content">{{orderInfo.EventUpdateTime}}</span></li>
                  <li><span class="list_item_label">所属部门： </span><span class="list_item_content">{{orderInfo.DeptName}}</span></li>
                  <li><span class="list_item_label">派单人： </span><span class="list_item_content">{{orderInfo.PersonName}}</span></li>
                  <li>
                      <span class="list_item_label"><el-button :type="orderInfo.UrgencyName | urgencyButtonType">{{orderInfo.UrgencyName}}</el-button></span>
                      <span class="list_item_content"><el-button :type="orderInfo.HandlerLevelName | levelButtonType">{{orderInfo.HandlerLevelName}}</el-button></span>
                  </li>
                  <li @tap="onAddrRowClick">
                    <span class="list_item_label addr_span" >
                      <span class="list_item_icon fas fa-map-marker-alt position_icon">
                      </span>
                      <span class="address_info" :style="{color: orderInfo.EventAddress ? '#001d26' : '#aaa'}">{{orderInfo.EventAddress || '暂无位置信息'}}</span>
                    </span>
                    <span class="list_item_content"></span>
                  </li>
                  <li><span class="list_item_label">事件来源： </span><span class="list_item_content">{{orderInfo.EventFromName}}</span></li>
                  <li><span class="list_item_label">事件类型： </span><span class="list_item_content">{{orderInfo.EventTypeName}}</span></li>
                  <li><span class="list_item_label">事件内容： </span><span class="list_item_content">{{orderInfo.EventTypeName2}}</span></li>
                </div>
              </div>
            </el-card>
            <!-- 事件描述 -->
            <el-card class="custom_el-card-header_color_358F8B">
              <div slot="header" class="clearfix card_header">
                <span class="header_text">事件描述</span>
              </div>
              <div class="card_body">
                {{orderInfo.EventDesc || '暂无描述信息'}}
              </div>
            </el-card>
            <!-- 现场图片 -->
            <el-card class="custom_el-card-header_color_358F8B">
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
        </div>
        <!-- 底部按钮组 -->
        <div class="fixed_footer">
            <div class="button-group">
                <!-- tap事件在PC端测试的时候，会有一点bug -->
                <button type="button" class="button left_button custom_bgcolor_dark" @tap="onAssignButtonClick">分 派</button>
                <button type="button" class="button right_button custom_bgcolor_light" @tap="onInvalidateButtonClick">无 效</button>
            </div>
        </div>
        <el-dialog
            title="事件分派"
            width="90%"
            center
            :modal="false"
            :close-on-click-modal="false"
            :visible.sync="assignDialogVisiable"
            @close="onAssignDialogClose"
        >
            <AssignForm ref="assignForm" :departmentList="departmentList"></AssignForm>
            <div slot="footer" >
                <el-button type="primary" @click="onConfirmClick">确 定</el-button>
                <el-button @click="onCancelClick">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
import config from "@config/config";
import { getSessionItem } from "@common/util";
import dateHelper from "@common/dateHelper";
import MuiList from "@comp/common/MuiList.vue";
import AssignForm from "@comp/order-assignment/AssignForm.vue";
import apiInspection from "@api/inspection";
import apiMaintain from "@api/maintain";
import nativeTransfer from "@JS/native/nativeTransfer";

export default {
  props: {
    orderInfo: [Object, String]
  },
  data() {
    return {
      assignDialogVisiable: false,
      departmentList: [],
      pictureBasePath: config.uploadFilePath.inspection
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    currentUserName() {
      return this.currentUser.PersonName;
    },
    orderId() {
      return Number(this.orderInfo.EventId);
    },
    pictureList() {
      return this.splitPicturesStr(this.orderInfo.EventPictures);
    }
  },
  methods: {
    splitPicturesStr(str) {
      if (str.length > 0) {
        return str.includes("|") ? str.split("|") : [str];
      } else {
        return [];
      }
    },
    // 预览图片
    onImageClick(index = 0) {
      let list = this.pictureList.map(url => {
        return `${this.pictureBasePath}${url}`;
      });
      plus.nativeUI.previewImage(list, { current: index });
    },
    onAddrRowClick() {
      if (this.orderInfo.EventX && this.orderInfo.EventY) {
        nativeTransfer.startNavi( Number(this.orderInfo.EventX),  Number(this.orderInfo.EventY), "", res=>{
          console.log(res)
        })
       /* if (window.plus && window.plus.maps && window.plus.geolocation) {
          this.$showLoading();
          nativeTransfer.getLocation(position => {
            if(position){
              let srcPoint = new plus.maps.Point(
                position.coords.longitude,
                position.coords.latitude
              );
              let destDesc = "目标设备";
              let destPoint = new plus.maps.Point(
                Number(this.orderInfo.EventX),
                Number(this.orderInfo.EventY)
              );
              nativeTransfer.startNavi( Number(this.orderInfo.EventX),  Number(this.orderInfo.EventY), "", res=>{
                console.log(res)
              })
              //window.plus.maps.openSysMap(destPoint, destDesc, srcPoint);
              this.$hideLoading();
            }else{
              this.$hideLoading();
              window.mui.toast("定位失败，无法调起导航");
            }
          });
        }*/
      }
    },
    onAssignButtonClick() {
      // 打开事件分派dialog
      this.assignDialogVisiable = true;
      apiMaintain.GetDepartmentList().then(res => {
        if (res.data.result === true) {
          // // 打开事件分派dialog
          // this.assignDialogVisiable = true;
          let list = res.data.data;
          this.departmentList = list.map((item, index) => {
            return {
              departmentName: item.DeptName,
              id: item.DeptId,
              staff: []
            };
          });
        } else {
          mui.toast("获取部门数据失败！");
        }
      });
    },
    onInvalidateButtonClick() {
      mui.confirm(
        "确定将该单置为无效吗？",
        "提示：",
        ["取消", "确认"],
        result => {
          // 点击了确认
          if (result.index === 1) {
            apiMaintain.PostInvalidOrder(Number(this.orderId)).then(res => {
              if (res.data[0].result === true) {
                mui.toast("操作成功！");
                this.$router.go(-1);
              } else {
                mui.toast("操作失败！");
              }
            });
          }
        },
        "div"
      );
    },
    // 事件分派弹出框中点击确定
    onConfirmClick() {
      this.$refs.assignForm.getValue((isValid, formData) => {
        if (isValid) {
          let reqData = {
            DepId: Number(formData.departmentId),
            PersonId: Number(formData.assigneeId),
            PreEndTime: formData.deadlineTime,
            OrderTime: dateHelper.format(new Date(), "yyyy-MM-dd"),
            EventId: Number(this.orderId),
            UserName: this.currentUserName
          };
          apiMaintain.PostOrderAssignee(reqData).then(
            res => {
              this.assignDialogVisiable = false;
              if (res.data[0].result === true) {
                mui.toast("分派成功！");
                this.$router.go(-1);
              } else {
                mui.toast("分派失败，请稍后重试！");
              }
            },
            err => {
              mui.toast("分派失败，请稍后重试！");
            }
          );
        } else {
          // 表单数据有值为空
          mui.toast("请输入完整信息");
        }
      });
    },
    // 事件分派弹出框中点击取消
    onCancelClick() {
      this.assignDialogVisiable = false;
    },
    onAssignDialogClose() {
      this.$refs.assignForm.reset();
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
      if (level.startsWith("2小时")) {
        buttonType = "danger";
      } else if (level.startsWith("4小时")) {
        buttonType = "warning";
      }
      return buttonType;
    }
  },
  components: { MuiList, AssignForm }
};
</script>

<style lang="less">
.order_handler_container {
  // .order_info_list {
  //   margin-bottom: 10vh;
  //   .gray {
  //     color: #999;
  //   }
  // }

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
        .position_icon {
          color: #00afa9;
          font-size: 1.5rem;
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
    .button-group {
      width: inherit;
      height: inherit;
      button {
        width: 50%;
        height: inherit;
      }
    }
  }
}
</style>


