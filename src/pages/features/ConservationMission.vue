<template>
  <div class="maintain_mission_container mui-content">
    <el-tabs v-model="activeTabName">
      <el-tab-pane class="tab_panel" label="未完成" name="todo">
        <!-- 未完成任务列表 -->
        <el-card v-for="order in todoOrderList" :key="order.TaskId" class="order_card mui-table-view-cell">
          <div slot="header" class="clearfix card_header">
            <span class="header_text">{{order.TaskName}}</span>
            <!-- <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button> -->
          </div>
          <div class="card_body">
            <div class="left">
              <li><span class="list_item_label">任务类别：</span><span class="list_item_content">{{order.PlanTypeName}}</span></li>
              <li><span class="list_item_label">开始时间：</span><span class="list_item_content">{{order.PersonName}}</span></li>
              <li><span class="list_item_label">结束时间：</span><span class="list_item_content">{{order.VisitStarTime}}</span></li>
              <li><span class="list_item_label">任务类型：</span><span class="list_item_content">{{order.VisitOverTime}}</span></li>
              <li><span class="list_item_label">是否需要反馈：</span><span class="list_item_content">{{order.BoolFeedBack | mapNeedFeedback}}</span></li>
            </div>
            <div class="right"><span class="el-icon el-icon-arrow-right"></span></div>
          </div>
        </el-card>
      </el-tab-pane>

      <el-tab-pane class="tab_panel" label="已完成" name="done">
        <!-- 已完成工单页内容 -->
        <el-card v-for="order in doneOrderList" :key="order.TaskId" class="order_card mui-table-view-cell">
          <div slot="header" class="clearfix card_header">
            <span class="header_text">{{order.TaskName}}</span>
            <!-- <el-button style="float: right; padding: 3px 0" type="text">操作按钮</el-button> -->
          </div>
          <div class="card_body">
            <div class="left">
              <li><span class="list_item_label">任务类别：</span><span class="list_item_content">{{order.PlanTypeName}}</span></li>
              <li><span class="list_item_label">开始时间：</span><span class="list_item_content">{{order.PersonName}}</span></li>
              <li><span class="list_item_label">结束时间：</span><span class="list_item_content">{{order.VisitStarTime}}</span></li>
              <li><span class="list_item_label">任务类型：</span><span class="list_item_content">{{order.VisitOverTime}}</span></li>
              <li><span class="list_item_label">是否需要反馈：</span><span class="list_item_content">{{order.BoolFeedBack | mapNeedFeedback}}</span></li>
            </div>
            <div class="right"><span class="el-icon el-icon-arrow-right"></span></div>
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div> 
</template>

<script>
/* 巡检任务页面 */
import _ from "lodash";
import apiInspection from "@api/inspection";
import apiMaintain from "@api/maintain";
import dateHelper from "@common/dateHelper";
import { mapNeedFeedback } from "@filters/common";
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem
} from "@common/util";
export default {
  created() {
    this.fetchOrderList((err, rawOrderList) => {
      if (!err) {
        this.orderList = rawOrderList;
      }
    });
  },
  data() {
    return {
      activeTabName: "todo",
      orderList: []
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
    todoOrderList() {
      return _.filter(this.orderList, ["Finish", 0]);
    },
    doneOrderList() {
      return _.filter(this.orderList, ["Finish", 1]);
    }
  },
  methods: {
    // 根据当前选择的订单状态或许订单列表
    fetchOrderList(callback) {
      let userId = this.currentUserId;
      apiMaintain
        .GetConservationList(userId)
        .then(res => {
          if (res.data.result === true) {
            let orderList = res.data.Data;
            if (callback instanceof Function) {
              callback(null, orderList);
            }
          }
        })
        .catch(err => {
          if (callback instanceof Function) {
            callback(err);
          } else {
            console.log("err", err);
          }
        });
    }
  },
  watch: {
    activeTabName() {
      this.fetchOrderList((err, rawOrderList) => {
        if (!err) {
          this.orderList = rawOrderList;
          // 判断当前tab对应的orderList是否为空，若为空，给出提示
          if (_.isEmpty(this[`${this.activeTabName}OrderList`])) {
            mui.toast("未查询到任务数据");
          }
        }
      });
    }
  },
  filters: { mapNeedFeedback }
};
</script>

<style lang="less">
.maintain_mission_container {
  // 设置tab item的样式
  div[role="tab"] {
    width: 45vw;
    text-align: center;
  }
  .order_card {
    &.mui-table-view-cell {
      /* mui-table-view-cell类会给元素添加padding，这里将padding unset*/
      padding: unset;
    }
    .card_header {
      .header_text {
        font-size: 1.4rem;
      }
    }
    .card_body {
      font-size: 1.2rem;
      .left {
        display: inline-block;
        width: 85%;
        vertical-align: middle;
        .list_item_label {
          font-size: 1.1rem;
          color: #999;
        }
        li {
          list-style-type: none;
        }
      }
      .right {
        display: inline-block;
        font-size: 1.8rem;
        color: #999;
      }
    }
  }
}
</style>