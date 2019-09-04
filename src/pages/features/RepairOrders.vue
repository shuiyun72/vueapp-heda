<template>
  <div class="repair_orders_container mui-content" 
      v-loading.fullscreen="fullscreenLoading"
  >
    <el-tabs v-model="activeTabName" @tab-click="onTabClick" :before-leave="beforeTabSwitch">
      <el-tab-pane class="tab_panel" label="未接工单" name="todo">
        <!-- 未接工单页内容 -->
        <NoContent :visible="todoOrders.length === 0 && !fullscreenLoading" content="暂无未接工单"></NoContent>
        <el-card v-for="order in todoOrders" :key="order.OrderId" class="order_card mui-table-view-cell" @click.native="onOrderClick(order)">
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.OrderCode}}</span>
            <!-- <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button> -->
          </div>
          <div class="card_body">
            <div class="left">
              <img :src="order.EventPictures ? (pictureBasePath + order.EventPictures.split('|')[0]) : defaultPicture"  style="width: 35vw; height:35vw">
            </div>
            <div class="right">
              <div class="type">{{order.EventType}} | {{order.EventContent}}</div>
              <div class="descrition ellipsis">{{order.EventDesc}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span :style="{color: order.EventAddress ? '#001d26' : '#aaa'}">{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <div class="status" :style="{color: order.isTimeout ? 'orange' : 'lightgreen'}"><span class="el-icon el-icon-time"></span>{{order.isTimeout ? '已超时' : '进行中'}}</div>
              <div
                  class="distance" 
                  v-if="typeof order.distance === 'number'"
              >
                  <span class="distance_text">距离{{order.distance}}公里</span>
                  <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div>
            </div>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane class="tab_panel" label="已接工单" name="doing">
        <NoContent :visible="doingOrders.length === 0 && !fullscreenLoading" content="暂无进行中的工单"></NoContent>
        <!-- 进行中工单页内容 -->
        <el-card v-for="order in doingOrders" :key="order.orderId" class="order_card mui-table-view-cell" @click.native="onOrderClick(order)">
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.OrderCode}}</span>
            <!-- <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button> -->
          </div>
          <div class="card_body">
            <div class="left">
              <img :src="order.EventPictures ? (pictureBasePath + order.EventPictures.split('|')[0]) : defaultPicture"  style="width: 35vw; height:35vw">
            </div>
            <div class="right">
              <div class="type">{{order.EventType}} | {{order.EventContent}}</div>
              <div class="descrition ellipsis">{{order.EventDesc}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span :style="{color: order.EventAddress ? '#001d26' : '#aaa'}">{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <div class="status" :style="{color: order.isTimeout ? 'orange' : 'lightgreen'}"><span class="el-icon el-icon-time"></span>{{order.isTimeout ? '已超时' : '进行中'}}</div>
              <div
                  class="distance" 
                  v-if="typeof order.distance === 'number'"
              >
                  <span class="distance_text">距离{{order.distance}}公里</span>
                  <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div>
            </div>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane class="tab_panel" label="完成工单" name="done">
        <NoContent :visible="doneOrders.length === 0 && !fullscreenLoading" content="未查询到已完成工单"></NoContent>
        <!-- 已完成工单页内容 -->
        <el-card v-for="order in doneOrders" :key="order.orderId" class="order_card mui-table-view-cell" @click.native="onOrderClick(order)">
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.OrderCode}}</span>
            <!-- <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button> -->
          </div>
          <div class="card_body">
            <div class="left">
              <img :src="order.EventPictures ? (pictureBasePath + order.EventPictures.split('|')[0]) : defaultPicture"  style="width: 35vw; height:35vw">
            </div>
            <div class="right">
              <div class="type">{{order.EventType}} | {{order.EventContent}}</div>
              <div class="descrition ellipsis">{{order.EventDesc}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span :style="{color: order.EventAddress ? '#001d26' : '#aaa'}">{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <!-- <div class="status" :style="{color: order.isTimeout ? 'orange' : 'lightgreen'}"><span class="el-icon el-icon-time"></span>{{order.isTimeout ? '已超时' : '进行中'}}</div> -->
              <div class="status" :style="{color: 'lightblue'}"><span class="el-icon el-icon-time"></span>已完成</div>
              <div
                  class="distance" 
                  v-if="typeof order.distance === 'number'"
              >
                  <span class="distance_text">距离{{order.distance}}公里</span>
                  <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div>
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
import apiMaintain from "@api/maintain";
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
import nativeTransfer from "@JS/native/nativeTransfer";
import BaseMap from "@JS/Map/BaseMap";
import CoordsHelper from "coordtransform";

export default {
  created() {
    this.fetchOrdersByStatus((err, rawOrderList) => {
      if (!err) {
        this[`${this.activeTabName}Orders`] = this.calcTimeout(rawOrderList);
        this.calcDistance(this[`${this.activeTabName}Orders`], newList => {
          this[`${this.activeTabName}Orders`] = newList;
        });
      }
    });
  },
  data() {
    return {
      fullscreenLoading: false,
      defaultPicture: "./static/images/none.jpg",
      pictureBasePath: config.uploadFilePath.inspection,
      activeTabName: "todo",
      todoOrders: [],
      doingOrders: [],
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
    },
    activeTabStatus() {
      let statusNumber =
        this.activeTabName === "todo"
          ? 1
          : this.activeTabName === "doing" ? 2 : 3;
      return statusNumber;
    }
  },
  methods: {
    // 根据当前选择的订单状态或许订单列表
    fetchOrdersByStatus(callback) {
      this.fullscreenLoading = true;
      let userId = this.currentUserId;
      let status = this.activeTabStatus;
      apiMaintain
        .GetOrderList(userId, status)
        .then(res => {
          this.fullscreenLoading = false;
          console.log("res", res);
          if (res.data.result) {
            let orderList = res.data.data;
            callback(null, orderList);
          }
        })
        .catch(err => {
          this.fullscreenLoading = false;
          if (status == 1) {
            console.log("从cache读取未接订单");
            let cacheData = JSON.parse(
              getLocalItem(`RepairOrders${this.currentUserId}`)
            );
            callback instanceof Function && callback(null, cacheData);
          } else {
            callback instanceof Function && callback(err);
            mui.toast("暂无网络");
             console.log("从cache读取未接订单暂无网络")
          }
        });
    },
    // 计算任务工单是否超时
    calcTimeout(orderList) {
      let newList = orderList.map(order => {
        let now = new Date();
        let deadline = new Date(order.PreEndTime);
        // 比较当前时间与最后期限时间的大小
        let isTimeout =
          dateHelper.compareDate(now, deadline) === 1 ? true : false;
        return Object.assign({}, order, {
          isTimeout
        });
      });
      console.log("new", newList);
      return newList;
    },
    // 计算任务工单目的地与当前用户设备的地理距离
    calcDistance(orderList, callback) {
      if (!(callback instanceof Function)) {
        console.error("callback must be a function!");
      }
      //if (window.plus) {
        nativeTransfer.getLocation(position => {
          if(position){
            // 当前纬度
            let currentLatitude = position.lat;
            // 当前经度
            let currentLongitude = position.lng;
            let newList = orderList.map(order => {
              // 目的地纬度
              let eventLatitude = Number(order.EventY);
              // 目的地经度
              let eventLongitude = Number(order.EventX);

              //初始化地图 
              let mapController = new BaseMap();
              mapController.Init("event_map");
              //地方投影转换
              let coordinateFenghua = mapController.unDestinationCoordinateProj(
                [eventLongitude,eventLatitude]
              );
              coordinateFenghua = CoordsHelper.wgs84togcj02(
                coordinateFenghua[0],
                coordinateFenghua[1]
              );
              // 计算距离，输出公里
              let distance = calcDistance(
                currentLongitude,
                currentLatitude,
                coordinateFenghua[0],
                coordinateFenghua[1]
              );

              return Object.assign({}, order, {
                distance
              });
            });
            callback(newList);
          }else{
            let newList = orderList.map(order => {
              return Object.assign({}, order, { distance: false });
            });
            callback(newList);
          }
         
        });
      /*} else {
        let newList = orderList.map(order => {
          return Object.assign({}, order, { distance: false });
        });
        callback(newList);
      }*/
    },
    // 点击一个具体的订单卡片，进入详情页面
    onOrderClick(orderInfo) {
      this.$router.push({ name: "OrderDetail", query: { orderInfo } });
    },
    onTabClick(target) {
      console.log("tab click!", target);
    },
    beforeTabSwitch(target) {
      console.log("before switch!", target);
    }
  },
  watch: {
    activeTabStatus() {
      this.fetchOrdersByStatus((err, rawOrderList) => {
        if (!err) {
          this[`${this.activeTabName}Orders`] = this.calcTimeout(rawOrderList);
          this.calcDistance(this[`${this.activeTabName}Orders`], newList => {
            this[`${this.activeTabName}Orders`] = newList;
          });
        }
      });
    }
  },
  components: {
    NoContent
  }
};
</script>

<style lang="less">
.repair_orders_container {
  // 设置tab item的样式
  div[role="tab"] {
    width: 33vw;
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
        display: inline-block;
        width: 70%;
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


