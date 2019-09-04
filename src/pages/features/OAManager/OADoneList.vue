<template>
  <div class="OA_done_list_container">
    <!-- 待办事项页内容 -->
    <NoContent :visible="recordList.length === 0" content="暂无已办事项"></NoContent>
    <el-card
      v-for="record in recordList"
      :key="record.instanceId + '' + record.Sort"
      @tap.native="onCardClick(record)"
      class="record_card mui-table-view-cell"
    >
      <div slot="header" class="clearfix card_header">
        <span class="header_text">{{record.title}}</span>
      </div>
      <div class="card_body">
        <div class="card_row" v-for="field in fieldListShowInCard" :key="field.value">
          <span class="card_row-label">{{field.text}}</span>
          <span class="card_row-text">{{record[field.value]}}</span>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script>
import _ from "lodash";
import apiOA, { STEP_STATUS_MAPPING } from "@api/OA";
import NoContent from "@comp/common/NoContent";
import { setSessionItem, getSessionItem } from "@common/util";
import dateHelper from "@common/dateHelper";
export default {
  name: "OADoneList",
  beforeRouteEnter(to, from, next) {
    next(instance => {
      instance.refreshList();
    });
  },
  data() {
    return {
      recordList: [],
      fieldListShowInCard: [
        {
          text: "标题",
          value: "title"
        },
        {
          text: "流程",
          value: "flowName"
        },
        {
          text: "当前步骤",
          value: "currentStepName"
        },
        {
          text: "发送人",
          value: "sender"
        },
        {
          text: "接收时间",
          value: "_recieveTime"
        },
        {
          text: "状态",
          value: "stateText"
        },
        {
          text: "备注",
          value: "comment"
        }
      ]
    };
  },
  computed: {
    // 当前的用户PersonId
    currentUserId() {
      return JSON.parse(getSessionItem("currentUser")).PersonId;
    }
  },
  methods: {
    refreshList(userId = this.currentUserId) {
      this.$showLoading();
      apiOA.flow
        .GetOADoneList(userId)
        .then(res => {
          console.log("donelist", res);
          this.recordList = res.data.rows.map(record => {
            return Object.assign({}, record, {
              title: record.Title,
              flowId: record.FlowID.toUpperCase(),
              instanceId: record.InstanceID,
              flowName: record.FlowName,
              currentStepName: record.StepName,
              sender: record.SenderName,
              _recieveTime: dateHelper.format(new Date(record.ReceiveTime), "yyyy-MM-dd hh:mm:ss"),
              stateText: STEP_STATUS_MAPPING[record.Status],
              comment: record.Note || "无"
            });
          });
          console.log('1111', this.recordList);
          this.$hideLoading();
        })
        .catch(err => {
          this.$hideLoading();
        });
    },
    // 进入详情页面
    onCardClick(record) {
      this.$router.push({
        name: "OADoneDetail",
        query: {
          instanceId: record.instanceId,
          flowId: record.FlowID.toUpperCase(),
          stepInfo: Object.assign({}, record)
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
.OA_done_list_container {
  .record_card {
    &.mui-table-view-cell {
      padding: unset;
    }
    &.el-card {
      border-left: 4px solid #00afa9;
      margin-bottom: 5px;
      .el-card__body {
        padding: 10px 20px;
      }
    }
    .card_header {
      .header_text {
        font-size: 1.4rem;
      }
    }
    .card_body {
      .card_row {
        display: flex;
        justify-content: space-between;
        margin: 1% 0;
        &-label {
          color: #999;
          font-size: 1.1rem;
        }
        &-text {
          font-size: 1.1rem;
        }
      }
    }
    .card_footer {
      display: flex;
      justify-content: space-around;
      margin: 0 25%;
    }
  }
}
</style>