<template>
    <div class="water_quality_monitor_container">
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
        <div 
            class="realtime_table_container"
            v-loading.fullscreen="waterQualityTableLoading"
        >
            <PointDataCard
                v-for="point in realTimeList"
                :key="point.DataPointID"
                :yulv="point.fWaterChlorine"
                unitYulv="mg/L"
                dataTypeYulv="余氯"
                :zhuodu="point.fWaterTurbibd"
                unitZhuodu="ntu"
                dataTypeZhuodu="浊度"
                :ph="point.fWaterPH"
                :wendu="point.fWaterTemperature"
                :diandaolv="point.fWaterConductivity"
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
import { deepCopy } from "@common/util";
import consts from "@pages/features/consts";
import PointDataCard from "./PointDataCard";

export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      instance.beforeRouteEnterNext(instance);
    });
  },
  data() {
    return {
      waterQualityTableLoading: false,
      searchText: "",
      // 请求获得的动态字段集合
      fieldList: [],
      // 请求获得的响应压力数据
      waterQualityData: { rows: [] },
      // 分页相关（水质暂时没用到）
      currentPageSize: 50000,
      currentPageNumber: 1
    };
  },
  computed: {
    // 实时数据列表
    realTimeList() {
      return this.waterQualityData.rows.map(point => {
        return Object.assign({}, point, {
          pointId: point.DataPointID,
          pointName: point.DataPointName
        });
      });
    }
  },
  methods: {
    beforeRouteEnterNext(instance) {
      // 获取表格数据
      instance.fetchRealTimeList().then(result => {
        if (result.err) {
          window.mui.toast(result.err.message);
        } else {
          console.log("表格数据加载完成！", result.data);
          instance.waterQualityData = deepCopy(result.data);
          instance.waterQualityData.rows = instance.waterQualityData.Data;
        }
      });
    },
    fetchRealTimeList(dataPointName = this.searchText) {
      this.waterQualityTableLoading = true;
      // 返回一个Promise对象
      return apiMonitor.GetWaterQualityRTList(dataPointName).then(
        res => {
          // 关闭Loading动效
          this.waterQualityTableLoading = false;
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
          this.waterQualityTableLoading = false;
          window.mui.toast("获取数据失败，请稍后再试");
        }
      );
    },
    onRowClick(row) {
      console.log(row);
      this.$router.push({
        path: "./detail",
        query: {
          pointId: row.pointId,
          pointName: row.pointName
        }
      });
    },
    onSearchButtonClick() {
      this.waterQualityData.rows = [];
      this.currentPageNumber = 1;
      this.fetchRealTimeList().then(result => {
        if (result.err) {
          window.mui.toast(result.err.message);
        } else {
          console.log("表格数据加载完成！", result.data);
          this.waterQualityData = deepCopy(result.data);
          this.waterQualityData.rows = this.waterQualityData.Data;
        }
      });
    }
  },
  filters: {
    timeFormatter(rawTime) {
      return dateHelper.format(new Date(rawTime), "yyyy-MM-dd hh:mm:ss");
    }
  },
  components: { PointDataCard }
};
</script>

<style lang="less" scoped>
.water_quality_monitor_container {
  height: calc(~"99vh - 86px");
  background-color: #2f4554;
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


