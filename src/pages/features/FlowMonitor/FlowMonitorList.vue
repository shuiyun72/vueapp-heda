<template>
  <div class="flow_monitor_container">
    <!-- 关键字查询（过滤） -->
    <div class="search_container">
      <el-input class="search_input" v-model="searchText" placeholder="按关键字查询点位"></el-input>
      <el-button icon="el-icon-search" type="primary" circle @click="onSearchButtonClick"></el-button>
    </div>
    <div class="realtime_table_container">
      <PointDataCard
        v-for="point in realTimeList"
        :key="point.DataPointID"
        :value1="point.iPositiveFlowRate"
        unit1="m³/h"
        data-type1="正瞬时流量"
        :value2="point.iNegativeFlowRae != 0 ? point.iNegativeFlowRae : (point.np == 0 ? '' : point.np)"
        :unit2="point.iNegativeFlowRae != 0 ? 'm³/h' : (point.np == 0 ? '' : 'Mpa')"
        :dataType2="point.iNegativeFlowRae != 0 ? '负瞬时流量' : (point.np == 0 ? '' : '当前压力')"
        :readTime="point.ReadDate | timeFormatter"
        :pointAddress="point.DataPointName"
        @click.native="onRowClick(point)"
      ></PointDataCard>
    </div>
  </div>
</template>

<script>
import _ from "lodash";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
import consts from "@pages/features/consts";
import { deepCopy } from "@common/util";
import PointDataCard from "./PointDataCard";
const SORT_BY_NAME_ARR = [
  "城建局房后（FM2）",
  "空港自来水进水",
  "空港高位水池进水",
  "汽车城"
];
export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      instance.beforeRouteEnterNext(instance);
    });
  },
  data() {
    return {
      searchText: "",
      // 请求获得的动态字段集合
      fieldList: [],
      // 请求获得的响应压力数据
      flowData: { rows: [] },
      // 分页相关
      currentPageSize: 50000,
      currentPageNumber: 1
    };
  },
  computed: {
    // 实时数据列表
    realTimeList() {
      let list = this.flowData.rows.map(point => {
        return Object.assign({}, point, {
          pointId: point.DataPointID,
          pointName: point.DataPointName,
          orderIndex:
            SORT_BY_NAME_ARR.indexOf(point.DataPointName) > -1
              ? SORT_BY_NAME_ARR.indexOf(point.DataPointName)
              : 10000
        });
      });
      let sortedList = _.sortBy(list, ["orderIndex"]);
      return sortedList;
    }
  },
  methods: {
    beforeRouteEnterNext(instance) {
      // 获取表格数据
      instance
        .fetchRealTimeList({
          pageNumber: instance.currentPageNumber,
          rowsPerPage: instance.currentPageSize,
          dataPointName: ""
        })
        .then(result => {
          if (result.err) {
            window.mui.toast(result.err.message);
          } else {
            console.log("表格数据加载完成！", result.data);
            instance.flowData = deepCopy(result.data);
          }
        });
    },
    fetchRealTimeList({
      pageNumber = this.currentPageNumber,
      rowsPerPage = this.currentPageSize,
      dataPointName = this.searchText
    }) {
      this.$showLoading();
      // 返回一个Promise对象
      return apiMonitor
        .GetFlowRTList(pageNumber, rowsPerPage, dataPointName)
        .then(
          res => {
            // 关闭Loading动效
            this.$hideLoading();
            if (res.data.ErrCode == 0) {
              return {
                data: res.data,
                err: null
              };
            } else {
              return {
                data: null,
                err: {
                  code: res.data.ErrCode,
                  message: res.data.ErrInfo
                }
              };
            }
          },
          err => {
            this.$hideLoading();
            window.mui.toast("获取数据失败，请稍后再试");
          }
        );
    },
    onRowClick(row) {
      this.$router.push({
        path: "./detail",
        query: {
          pointId: row.pointId,
          tableId: row.iMonitorDataTableID,
          pointName: row.pointName
        }
      });
    },
    onSearchButtonClick() {
      this.flowData.rows = [];
      this.currentPageNumber = 1;
      this.fetchRealTimeList({}).then(result => {
        if (result.err) {
          window.mui.toast(result.err.message);
        } else {
          console.log("表格数据加载完成！", result.data);
          this.flowData = deepCopy(result.data);
        }
      });
    }
  },
  filters: {
    timeFormatter(rawTime) {
      return dateHelper.format(new Date(rawTime), "yyyy-MM-dd hh:mm:ss");
    }
  },
  components: {
    PointDataCard
  }
};
</script>

<style lang="less" scoped>
.flow_monitor_container {
  background-color: #2f4554;
  min-height: calc(~"99vh - 86px");
  .search_container {
    width: 100%;
    position: fixed;
    top: calc(~"1vh + 44px");
    left: 0;
    .search_input {
      width: calc(~"100% - 45px");
    }
  }
  .realtime_table_container {
    margin-top: calc(~"1vh + 86px");
    height: calc(~"99vh - 86px");
    overflow: scroll;
  }
  .action_button_container {
    text-align: center;
    margin: 1vh 0;
  }
}
</style>


