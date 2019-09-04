<template>
    <div class="pressure_monitor_container">
        <!-- 关键字查询（过滤） -->
        <div class="search_container">
            <el-input
                class="search_input"
                v-model="searchText"
                placeholder="按关键字查询点位"
            >
            </el-input>
            <el-button 
                icon="el-icon-search" 
                type="primary" 
                circle
                @click="onSearchButtonClick"
            ></el-button>
        </div>
        <div class="realtime_list"
            v-loading.fullscreen="pressureListLoading"
            v-infinite-scroll="loadMore" 
            infinite-scroll-disabled="busy" 
            infinite-scroll-distance="10"
        >
            <PointDataCard
                v-for="point in realTimeList"
                :key="point.DataPointID"
                :value="point.iPressureData"
                unit="Mpa"
                dataType="压力"
                :readTime="point.ReadDate | timeFormatter"
                :pointAddress="point.DataPointName"
                @click.native="onRowClick(point)"
            ></PointDataCard>
        </div>
    </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
import consts from "@pages/features/consts";
import PointDataCard from "@comp/common/PointDataCard";

export default {
  data() {
    return {
      pressureListLoading: true,
      searchText: "",
      /* 分页相关 */
      currentPageSize: 15,
      currentPageNumber: 0,
      // 是否正在loadmore
      busy: false,
      // 请求获得的动态字段集合
      fieldList: [],
      // 请求获得的响应压力数据
      pressureData: {
        rows: []
      }
    };
  },
  computed: {
    // 实时数据列表
    realTimeList() {
      return this.pressureData.rows.map(point => {
        return Object.assign({}, point, {
          pointId: point.DataPointID,
          pointName: point.DataPointName
        });
      });
    }
  },
  methods: {
    fetchRealTimeList({
      pageNumber = 1,
      rowsPerPage = 5000,
      dataPointName = ""
    }) {
      this.pressureListLoading = true;
      // 返回一个Promise对象
      return apiMonitor
        .GetPressureRTList(pageNumber, rowsPerPage, dataPointName)
        .then(
          res => {
            // 关闭Loading动效
            this.pressureListLoading = false;
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
            this.pressureListLoading = false;
            window.mui.toast("获取数据失败，请稍后再试");
          }
        );
    },
    loadMore() {
      console.log("Loadmore!");
      this.busy = true;
      this.fetchRealTimeList({
        pageNumber: this.currentPageNumber + 1,
        rowsPerPage: this.currentPageSize,
        dataPointName: this.searchText
      }).then(result => {
        if (result.err) {
          window.mui.toast(result.err.message);
          this.busy = false;
        } else {
          console.log("表格数据加载完成！", result.data);
          if (result.data.rows.length === 0) {
            this.busy = true;
          } else {
            this.pressureData.rows = this.pressureData.rows.concat(
              deepCopy(result.data).rows
            );
            this.currentPageNumber++;
            this.busy = false;
          }
        }
      });
    },
    onRowClick(row) {
      this.$router.push({
        path: "./detail",
        query: {
          pointId: row.pointId,
          pointName: row.pointName
        }
      });
    },
    onSearchButtonClick() {
      this.pressureData.rows = [];
      this.currentPageNumber = 0;
      this.loadMore();
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
.pressure_monitor_container {
  background-color: #2f4554;
  height: calc(~"99vh - 86px");
  .search_container {
    width: 100%;
    position: fixed;
    top: calc(~"1vh + 44px");
    left: 0;
    .search_input {
      width: calc(~"100% - 45px");
    }
  }
  .realtime_list {
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