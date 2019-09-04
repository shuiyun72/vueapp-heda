<template>
  <div class="self_task_manager_list_container mui-content">
    <el-tabs v-model="activeTabName">
      <el-tab-pane class="tab_panel" label="待办" name="todo">
        <!-- 待处理页内容 -->
        <NoContent :visible="todoOrders.length === 0" content="暂无待办事件"></NoContent>
        <el-card
          v-for="(order, index) in todoOrders"
          :key="order.OrderId || index"
          class="order_card mui-table-view-cell"
          @tap.native="onOrderClick(order)"
        >
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.EventCode}}</span>
            <span style="display: inline-block; width: 38%;vertical-align: bottom;">{{order.PersonName}} | {{order.EventFromName}}</span>
          </div>
          <div class="card_body">
            <div class="left">
              <img
                :src="order.EventPictures ? (pictureBasePath + order.EventPictures.split('|')[0]) : defaultPicture"
                style="width: 35vw; height:35vw"
              >
            </div>
            <div class="right">
              <div class="type">{{order.EventTypeName}} | {{order.EventTypeName2}}</div>
              <div class="descrition text_ellipsis">{{order.EventDesc || '暂无描述'}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span
                  :style="{color: order.EventAddress ? '#001d26' : '#aaa'}"
                >{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <div class="status">
                <el-button type="primary">{{'已' + order.OperName}}</el-button>
                <el-button type="info">{{order.OperName2}}</el-button>
              </div>
              <div class="timestamp">{{order.UpTime}}</div>
              <!-- <div class="distance" v-if="typeof order.distance === 'number'">
                <span class="distance_text">距离{{order.distance}}公里</span>
                <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div> -->
            </div>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane class="tab_panel" label="已办" name="done">
        <NoContent :visible="doneOrders.length === 0" content="未查询到已办事件"></NoContent>
        <!-- 已完成工单页内容 -->
        <el-card
          v-for="(order, index) in doneOrders"
          :key="index"
          class="order_card mui-table-view-cell"
          @tap.native="onOrderClick(order)"
        >
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.EventCode}}</span>
            <span style="display: inline-block; width: 38%;vertical-align: bottom;">{{order.PersonName}} | {{order.EventFromName}}</span>
          </div>
          <div class="card_body">
            <div class="left">
              <img
                :src="order.EventPictures ? (pictureBasePath + order.EventPictures.split('|')[0]) : defaultPicture"
                style="width: 35vw; height:35vw"
              >
            </div>
            <div class="right">
              <div class="type">{{order.EventTypeName}} | {{order.EventTypeName2}}</div>
              <div class="descrition text_ellipsis">{{order.EventDesc || '暂无描述'}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span
                  :style="{color: order.EventAddress ? '#001d26' : '#aaa'}"
                >{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <div class="status">
                <el-button type="primary">{{'已' + order.OperName}}</el-button>
                <el-button type="info">{{order.OperName2}}</el-button>
              </div>
              <div class="timestamp">{{order.UpTime}}</div>
              <!-- <div class="distance" v-if="typeof order.distance === 'number'">
                <span class="distance_text">距离{{order.distance}}公里</span>
                <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div> -->
            </div>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import _ from "lodash";
import config from "@config/config";
import apiInspection from "@api/inspection";
import apiMaintain from "@api/maintain";
import apiMaintainNew from "@api/maintain-new";
import dateHelper from "@common/dateHelper";
import NoContent from "@comp/common/NoContent";
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem,
  // 根据经纬度计算公里距离的工具函数
  calcDistance
} from "@common/util";

export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      instance.activeTabName = 'todo'
    });
  },
  data() {
    return {
      defaultPicture: "./static/images/none.jpg",
      pictureBasePath: config.uploadFilePath.inspection,
      activeTabName: "",
      todoOrders: [],
      doneOrders: []
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
    }
  },
  methods: {
    refreshOrderList(activeTabName) {
      console.log("refresh! tabName is ", activeTabName);
      let status = 0;
      if (activeTabName === "todo") {
        status = 0;
      } else if (activeTabName === "done") {
        status = 1;
      }
      apiMaintainNew.GetOrderList(this.currentUserId,  status).then(res => {
        console.log("refresh res", status, res);
        if (res.data.ErrCode == 0) {
          let list = res.data.rows
          this[status === 0 ? 'todoOrders' : 'doneOrders'] = list
        } else {
          mui.toast('获取个人工单失败')
        }
      }).catch(err=>{
        mui.toast('网络错误，获取工单失败')
      });
    },
    // 点击一个具体的订单卡片，进入详情页面
    onOrderClick(orderInfo) {
      this.$router.push({ name: "SelfTaskManagerDetail",  query: { oriOrderInfo: orderInfo } });
    }
  },
  watch: {
    activeTabName() {
      console.log('tab改变！！')
      // 当前激活的tab改变时，访问接口获取新的事件列表
      this.refreshOrderList(this.activeTabName);
    }
  },
  components: {
    NoContent
  }
};
</script>

<style lang="less">
.self_task_manager_list_container {
  // 设置tab item的样式
  div[role="tab"] {
    width: 50vw;
    text-align: center;
  }
  .tab_panel {
    height: calc(~"99vh - 99px");
    overflow: scroll;
  }
  .order_card {
    &.mui-table-view-cell {
      padding: unset;
    }
    &.el-card {
      border-left: 4px solid #00afa9;
      margin-bottom: 5px;
    }
    .card_header {
      .header_text {
        vertical-align: bottom;
        display: inline-block;
        width: 60%;
        font-size: 1.4rem;
      }
    }
    .card_body {
      display: flex;
      flex-flow: row nowrap;
      justify-content: space-between;
      font-size: 1.1rem;
      .left {
        display: inline-block;
        vertical-align: middle;
        width: 35vw;
        height: 35vw;
      }
      .right {
        display: inline-flex;
        flex-flow: column nowrap;
        justify-content: space-between;
        width: calc(~"60vw - 40px");
        height: 35vw;
        .type,
        .address {
          color: #aaa;
        }
        .address {
          font-size: 1.2rem;
          color: #001d26;
        }
        .status {
          .el-button {
            padding: 3px;
          }
        }
        .distance {
          color: #aaa;
          .distance_text {
            display: inline-block;
            width: 70%;
            vertical-align: middle;
          }
          .distance_icon {
            display: inline-block;
            color: lightgreen;
          }
        }
      }
    }
  }
}
</style>

