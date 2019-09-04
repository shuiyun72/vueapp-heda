<template>
    <div class="duty_log_container" v-loading.fullscreen="recordListLoading">
        <div class="filter_container">
            <el-input
                clearable
                placeholder="姓名"
                v-model="filterName"
                style="width: 49%;"
            ></el-input>
            <el-input
                v-model="filterStartDate"
                placeholder="按日期查询"
                :prefix-icon="'-'"
                :editable="false"
                style="width: 49%;"
                class="date_picker_padding_left_5"
                readonly
                @click.native="onDatePickerClick"
            >
            </el-input>
        </div>
        <div class="button_group_container">
            <el-button icon="el-icon-delete" class="action_button custom_bgcolor_light" @click="onSearchResetButtonClick">重 置</el-button>
            <el-button icon="el-icon-search" class="action_button custom_bgcolor_dark" @click="onSearchButtonClick">搜 索</el-button>
        </div>
        <div class="loadmore_container">
            <!-- <scroller 
              snapping
              style="position: unset;"
              :on-refresh="refreshRecordList"
              :on-infinite="loadMoreRecords"
            > -->
              <div class="record_list_container"
                  v-infinite-scroll="loadMoreRecords" 
                  infinite-scroll-disabled="busy" 
                  infinite-scroll-distance="5"
              >
                  <NoContent :visible="recordList.length === 0" content="未查询到符合条件的记录"></NoContent>
                  <el-card v-for="record in recordList" :key="record.id" class="detail_card"
                      @click.native="onItemClick(record)"
                  >
                      <div slot="header">
                          <div  class="summary">
                              <span class="date_text">日 期：</span><span style="color: #001d26">{{record.createTime}}</span><br><br>
                              <span class="worker_name">值班人员：</span><span style="color: #001d26">{{record.worker || '无值班人员'}}</span>
                          </div>
                          <div class="arrow">
                            <i class="el-icon el-icon-arrow-right"></i>
                          </div>
                      </div>
                  </el-card>
              </div>
            <!-- </scroller> -->
        </div>
        <!-- 日志详情弹出框 -->
        <el-dialog
            title="供水生产调度记录"
            :visible.sync="viewLogDialogVisible"
            fullscreen
            center
            @close="onDialogClose"
        >
            <div class="log_detail_container">
                <div class="period_container">
                    <span class="gray">请选择值班时段</span>
                    <el-select v-model="pickedViewPeriod" placeholder="请选择时段" style="width: 100%;">
                        <el-option
                            v-for="item in periodSelectorOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                        >
                        </el-option>
                    </el-select>
                </div>
                <div class="date_and_worker_container">
                    <div class="date">
                        <span class="date_text">日 期：</span><span>{{currentViewLogData.createTime}}</span>
                    </div>
                    <div class="worker">
                        <span class="worker_name">值班人员：</span><span>{{currentViewLogData[`c${pickedViewPeriod}Worker`] || '无值班人员'}}</span>
                    </div>
                </div>
                <div class="detail_text_container">
                    <span class="detail_label">生产调度记录详情</span>
                    <el-input
                        type="textarea"
                        readonly
                        :autosize="{ minRows: 11, maxRows: 11}"
                        :value="currentViewLogData[`c${pickedViewPeriod}Record`] || '无记录信息'"
                    >
                    </el-input>
                </div>
            </div>
            <!-- <span slot="footer">
                <el-button type="primary" style="width: 100%;" @click="viewLogDialogVisible = false">关 闭</el-button>
            </span> -->
        </el-dialog>
    </div>    
</template>

<script>
import _ from "lodash";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
import NoContent from "@comp/common/NoContent";
export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      // 定制该路由下的设备返回键逻辑
      instance.$defineDeviceBack(defaultFunction => {
        if (instance.viewLogDialogVisible) {
          // 如果当前有弹出框，则返回键会关闭弹出框
          instance.viewLogDialogVisible = false;
        } else {
          // 如果当前没有弹出框，则使用默认返回逻辑
          defaultFunction();
        }
      });
      // 获取表格数据
      instance.refreshRecordList.call(instance);
    });
  },
  mounted() {
    // 实例化date picker组件
    this._datePicker = new window.mui.DtPicker({
      type: "date",
      labels: ["年", "月", "日"]
    });
  },
  beforeDestroy() {
    this._datePicker.dispose();
  },
  data() {
    return {
      busy: false,
      currentPageSize: 20,
      // 表格数据
      recordList: [],
      recordListLoading: false,
      currentPageNumber: 1,
      // 顶部姓名过滤器的值
      filterName: "",
      // 顶部日期选择器的值
      filterStartDate: "",
      // 详情相关
      viewLogDialogVisible: false,
      pickedViewPeriod: "Morning",
      // 选择器select的配置对象
      periodSelectorOptions: [
        {
          label: "上午",
          value: "Morning"
        },
        {
          label: "下午",
          value: "Afternoon"
        },
        {
          label: "夜晚",
          value: "Night"
        }
      ],
      currentViewLogData: {}
    };
  },
  computed: {
    pickedStartDate() {
      return this.filterStartDate ? `${this.filterStartDate} 00:00:00` : "";
    },
    pickedEndDate() {
      return this.filterStartDate ? `${this.filterStartDate} 23:59:59` : "";
    }
  },
  methods: {
    fetchLogRecordList(
      {
        pageNumber = this.currentPageNumber,
        rowsPerPage = this.currentPageSize,
        workerName = "",
        startTime = "",
        endTime = "",
        recordId = ""
      },
      callback
    ) {
      this.recordListLoading = true;
      apiMonitor
        .GetDutyLogData(
          pageNumber,
          rowsPerPage,
          workerName,
          startTime,
          endTime,
          recordId
        )
        .then(res => {
          this.recordListLoading = false;
          if (res.data.ErrCode == 0) {
            let recordList = _.map(res.data.rows, record => {
              return Object.assign(
                {
                  id: record.iDutyRecordID,
                  name: record.dDutyTimeYMD,
                  createTime: record.dDutyTimeYMD,
                  worker: record.cMorningWorker,
                  desc: record.cMorningRecord
                },
                record
              );
            });
            if (callback) {
              callback(null, { recordList });
            }
          } else {
            if (callback instanceof Function) {
              callback({
                code: res.data.ErrCode,
                message: res.data.ErrInfo
              });
            }
          }
        })
        .catch(err => {
          console.log(err);
          this.recordListLoading = false;
          window.mui.toast("获取数据失败，请稍后再试");
        });
    },
    refreshRecordList() {
      console.log("refresh");
      this.currentPageNumber = 1;
      // 开启loadmore
      this.busy = false;
      this.fetchLogRecordList(
        {
          pageNumber: this.currentPageNumber,
          workerName: this.filterName,
          startTime: this.pickedStartDate,
          endTime: this.pickedEndDate
        },
        (err, data) => {
          if (err) {
            window.mui.toast(err.message);
          } else {
            this.recordList = data.recordList;
          }
        }
      );
    },
    loadMoreRecords() {
      this.busy = true;
      this.fetchLogRecordList(
        {
          pageNumber: this.currentPageNumber + 1,
          workerName: this.filterName,
          startTime: this.pickedStartDate,
          endTime: this.pickedEndDate
        },
        (err, data) => {
          if (err) {
            window.mui.toast(err.message);
            this.busy = false;
          } else {
            if (data.recordList.length === 0) {
              this.busy = true;
            } else {
              this.recordList = this.recordList.concat(data.recordList);
              this.currentPageNumber++;
              this.busy = false;
            }
            console.log(
              `%cLoadmore: ${this.currentPageNumber} | ${
                data.recordList.length
              }`,
              "color: red"
            );
          }
        }
      );
    },
    onDatePickerClick() {
      // 日期选择
      this._datePicker.show(result => {
        let pickedDate = result.value;
        this.filterStartDate = pickedDate;
      });
    },
    onSearchButtonClick() {
      console.log({
        workerName: this.filterName,
        startTime: this.pickedStartDate,
        endTime: this.pickedEndDate
      });
      this.refreshRecordList();
    },
    onSearchResetButtonClick() {
      // 清空过滤器控件的值
      this.filterName = "";
      this.filterStartDate = "";
      // 更新表格数据
      this.refreshRecordList();
    },
    onItemClick(record) {
      this.currentViewLogData = record;
      let { createTime } = record;
      this.$eventbus.$emit("set-title", `${createTime} 日志详情`);
      this.pickedViewPeriod = "Morning";
      this.viewLogDialogVisible = true;
    },
    onDialogClose() {
      this.$eventbus.$emit("set-title", "值班日志");
    }
  },
  components: {
    NoContent
  }
};
</script>

<style lang="less">
.duty_log_container {
  div {
    font-size: 1.2rem;
  }
  div.date_picker_padding_left_5 {
    &.el-input {
      input {
        padding-left: 5px;
        font-size: 14px;
      }
    }
  }
  .detail_card {
    &.el-card {
      margin-bottom: 5px;
      // background-color: #001A25;
      background-color: #ddd;
      .el-card__header {
        border-bottom: none;
        padding-bottom: unset;
      }
      .el-card__body {
        color: #fff;
        padding: 5px;
      }
    }
    .summary {
      display: inline-block;
      vertical-align: middle;
      width: 90%;
    }
    .arrow {
      display: inline-block;
    }
  }
  .loadmore_container {
    height: calc(~"97vh - 128px");
    overflow: scroll;
  }
  .record_list_container {
    height: calc(~"97vh - 128px");
    overflow: scroll;
  }
  .filter_container {
    width: 100%;
    text-align: center;
  }
  .button_group_container {
    text-align: center;
    margin: 1vh 0;
    .action_button {
      width: 49%;
      /* 覆盖elementui默认的margin-left */
      margin-left: unset;
    }
  }

  .date_text,
  .worker_name,
  .detail_label {
    display: inline-block;
    color: #00afa9;
    margin: 2% 0;
  }
}
</style>


