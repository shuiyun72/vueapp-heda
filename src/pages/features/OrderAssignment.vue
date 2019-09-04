<template>
    <div class="order_assignment_container mui-content" v-loading.fullscreen="fullscreenLoading">
        <NoContent :visible="orderList.length === 0 && !fullscreenLoading" content="暂无未分派工单"></NoContent>
        <el-card v-for="order in orderList" :key="order.EventId" class="order_card mui-table-view-cell" @click.native="onOrderCardClick(order)">
          <div slot="header" class="clearfix card_header">
            <span class="header_text text_ellipsis">{{order.EventCode}}</span>
          </div>
          <div class="card_body">
            <div class="left">
              <!-- 展示第一张图片 -->
              <img :src="(pictureBasePath + order.EventPictures.split('|')[0]) || defaultPicture" alt="" style="width: 35vw; height:35vw">
            </div>
            <div class="right">
              <div class="type">{{order.EventTypeName}} | {{order.EventTypeName2}}</div>
              <div class="reporter"><span style="color: #aaa">上报人： </span>{{order.PersonName}} | {{order.DeptName || "未知部门"}}</div>
              <div class="descrition text_ellipsis">{{order.EventDesc || '暂无描述信息'}}</div>
              <div class="address text_ellipsis">
                <span class="el-icon el-icon-location-outline"></span>
                <span :style="{color: order.EventAddress ? '#001d26' : '#aaa'}">{{order.EventAddress || '暂无位置信息'}}</span>
              </div>
              <!-- <div class="status" :style="{color: order.isTimeout ? 'orange' : 'lightgreen'}"><span class="el-icon el-icon-time"></span>{{order.isTimeout ? '已超时' : '进行中'}}</div> -->
              <!-- <div
                  class="distance" 
                  v-if="typeof order.distance === 'number'"
              >
                  <span class="distance_text">距离{{order.distance}}公里</span>
                  <span class="distance_icon fas fa-map-marker-alt" style="color: lightgreen"></span>
              </div>
              <div class="distance" v-else-if="order.distance === false">无法获取距离数据</div>
              <div class="distance" v-else>正在计算目的地距离...</div> -->
            </div>
          </div>
        </el-card>
    </div> 
</template>

<script>
import _ from "lodash";
import config from "@config/config";
import apiInspection from "@api/inspection";
import apiMaintain from "@api/maintain";
import dateHelper from "@common/dateHelper";
import NoContent from "@comp/common/NoContent";
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem
} from "@common/util";
export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      instance.fetchOrdersByStatus();
    });
  },
  data() {
    return {
      fullscreenLoading: false,
      activeTabName: "todo",
      orderList: [],
      pictureBasePath: config.uploadFilePath.inspection,
      defaultPicture: "./static/images/none.jpg"
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    }
  },
  methods: {
    // 根据当前选择的订单状态或许订单列表
    fetchOrdersByStatus() {
      this.fullscreenLoading = true
      apiMaintain
        .GetEventOrderList()
        .then(res => {
          console.log("工单分派列表", res);
          if (res.data.result === true) {
            this.orderList = res.data.data;
          }
          this.fullscreenLoading = false
        })
        .catch(err => {
          console.log("err", err);
          this.fullscreenLoading = false
        });
    },
    // 点击一个订单卡片，跳转到订单详情页面
    onOrderCardClick(order) {
      this.$router.push({ name: "OrderHandler", query: { orderInfo: order } });
    }
  },
  components: {
    NoContent
  }
};
</script>

<style lang="less">
.order_assignment_container {
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


