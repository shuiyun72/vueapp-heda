<template>
    <div class="water_quality_monitor_detail_container">
        <div class="wq_charts_container">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-line '></i>
                <span>实时数据图表</span>
            </div>
            <div id="chart1" class="chart"></div>
            <div id="chart2" class="chart"></div>
            <div id="chart3" class="chart"></div>
        </div>
        <!-- 当前运行数据表格  -->
        <div class="current_wq_container">
            <div class="header">
                <i class='header_icon fas fa-lg fa-list-ul'></i>
                <span>监测点位数据</span>
            </div>
            <MuiList 
                v-loading.fullscreen.lock="fullscreenLoading"
                element-loading-text="正在调起地图应用..."
                element-loading-spinner="el-icon-loading"
                class="point_data_list"
                :items="formattedPointDataForMuiList" 
                @row-click="onRowIconClick"
            ></MuiList>
        </div>
    </div>
</template>

<script>
import _ from "lodash";
import $ from "jquery";
import apiMonitor from "@api/monitor";
import dateHelper from "@common/dateHelper";
import consts from "@pages/features/consts";
import MuiList from "@comp/common/MuiList";
import { wChart_Pie, wChart_Dashboard_Three } from "@JS/charts/charts";
import {
  BuilderGaugeChart,
  BuilderQGaugeChart,
  QOption
} from "@JS/charts/gauge";
import ChartBuilder from "@JS/charts/chart-builder";
import * as ChartOption from "@JS/charts/chart-option";
import { BuilderChart as CategoryBuilder } from "@JS/charts/category-option";
import { deepCopy } from "@common/util";
import nativeTransfer from "@JS/native/nativeTransfer";

const dataTableId = consts.dataTableId.water;

export default {
  props: {
    pointName: {
      type: String,
      required: true
    },
    pointId: {
      type: [String, Number],
      required: true
    }
  },
  mounted() {
    console.log("水质详情参数", this.pointId, this.pointName);
    this.$showLoading()
    this.$eventbus.$emit("set-title", `水质详情 | ${this.pointName}`);
    this.fetchDataFieldList();
    this.fetchPointDataList();
    // 实时水质图表（3个）
    this.initRealtimeWQCharts();
  },
  data() {
    return {
      charts: {
        currentWQChart: "",
        // 水质实时有三张表
        realtimeWQChart: [{}, {}, {}]
      },
      fullscreenLoading: false,
      fieldList: [],
      pointData: {}
    };
  },
  computed: {
    formattedPointDataForMuiList() {
      const Location_ICON_CONFIG = {
        iconClass: "fas fa-map-marker-alt",
        iconStyle: "color: lightgreen"
      };

      if (_.isEmpty(this.fieldList) || _.isEmpty(this.pointData)) {
        return [];
      } else {
        let pointData = Object.assign(
          {},
          this.pointData,
          // 这一步旨在格式化源数据中的ReadDate属性值的format
          {
            ReadDate: dateHelper.format(
              new Date(this.pointData.ReadDate),
              "yyyy-MM-dd hh:mm:ss"
            )
          }
        );
        let muiListConfig = [];
        _.each(this.fieldList, fieldObj => {
          let { title, field } = fieldObj;
          let value = pointData[field];
          let rowConfig = {
            id: field,
            label: title,
            content: value,
            contentClass: "custom_color_light",
            labelClass: "gray label_w_30per"
          };
          // 在点位名称行加上位置icon
          if (field === "DataPointName") {
            Object.assign(rowConfig, Location_ICON_CONFIG);
          }
          muiListConfig.push(rowConfig);
        });
        console.log("MuiList Config!", muiListConfig);
        return muiListConfig;
      }
    }
  },
  methods: {
    // 获取所有数据字段
    fetchDataFieldList() {
      apiMonitor
        .GetFieldList(dataTableId)
        .then(res => {
          let rawFieldList = res.data.Data;
          console.log("Raw Fields", rawFieldList);
          let fieldList = _.uniqBy(rawFieldList, "field");
          console.log("Uniq Fields", fieldList);
          this.fieldList = fieldList;
          console.log("字段与表格初始化完成，开始加载数据表格...");
        })
        .catch(err => {
          console.log("Fields Err ", err);
        });
    },
    fetchPointDataList() {
      this.fetchPhysicPointData({}).then(
        ({ err, data }) => {
          console.log("获取单个监测点数据", data);
          if (!err) {
            this.pointData = deepCopy(data);
          } else {
            mui.toast(`获取当前监测点数据失败！`);
          }
        },
        err => {
          mui.toast(`获取当前监测点数据失败！`);
        }
      );
    },
    fetchPhysicPointData({
      pageNumber = 1,
      rowsPerPage = 5000,
      dataPointName = this.pointName
    }) {
      // 返回一个Promise对象
      return apiMonitor.GetWaterQualityRTList(dataPointName).then(
        res => {
          if (res.data.ErrCode == 0) {
            console.log("####", res);
            return {
              data: res.data.Data[0],
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
        err => {}
      );
    },
    initRealtimeWQCharts() {
      // 浊度
      apiMonitor
        .GetChartData("浊度", dataTableId, this.pointId, "fWaterTurbibd")
        .then(res => {
          let chartOption = res.data;
          console.log("yulv cahrt option:", chartOption);
          this.charts.realtimeWQChart[0] = CategoryBuilder(
            chartOption,
            "chart1"
          );
        });
      // 余氯
      apiMonitor
        .GetChartData("余氯", dataTableId, this.pointId, "fWaterChlorine")
        .then(res => {
          let chartOption = res.data;
          console.log("yulv cahrt option:", chartOption);
          this.charts.realtimeWQChart[0] = CategoryBuilder(
            chartOption,
            "chart2"
          );
        });
      // PH值
      apiMonitor
        .GetChartData("PH值", dataTableId, this.pointId, "fWaterPH")
        .then(res => {
          let chartOption = res.data;
          console.log("ph cahrt option:", chartOption);
          this.charts.realtimeWQChart[0] = CategoryBuilder(
            chartOption,
            "chart3"
          );
          // 最好用Promise.all处理并行任务，这里暂时这样
          setTimeout(()=>{
            this.$hideLoading()
          }, 500)
        });
    },
    onRowIconClick(row, index, event) {
      if (
        row.id === "DataPointName" &&
        this.pointData.DataMapX &&
        this.pointData.DataMapY
      ) { 
          nativeTransfer.startNavi(Number(this.pointData.DataMapX),Number(this.pointData.DataMapY), "", res=>{
            
          })     
      }
    }
  },
  components: {
    MuiList
  }
};
</script>

<style lang="less">
.water_quality_monitor_detail_container {
  width: 100%;
  height: calc(~"99vh-44px");
  .current_wq_container {
    width: 100%;
    background-color: #fff;
  }
  .header {
    width: 100%;
    color: rgb(255, 255, 255);
    height: 35px;
    background-color: #666;
    line-height: 35px;
    font-size: 1.3rem;
    padding-left: 2%;
    .header_icon {
      color: #fff;
    }
  }
  .w130- {
    width: 150px;
    height: 36px;
    line-height: 36px;
    text-align: right;
    background-color: #a2c9e9;
    font-size: 16px;
  }
  .w280- {
    text-align: left;
    width: 280px;
    color: #ffffff;
    background-color: #106199;
    height: 36px;
    line-height: 36px;
    padding-left: 10px;
    font-size: 14px;
  }
  .wq_charts_container {
    .chart {
      width: 100%;
      height: 350px;
      margin: 2vh 0;
    }
  }
  .physic_point_container {
    .point_data_list {
      font-size: 1.2rem;
    }
  }
}
</style>


