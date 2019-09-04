<template>
    <div class="flow_monitor_detail_container">
        <!-- 当前运行数据饼图  -->
        <div class="current_flow_charts_container" id="ChartWidthTotal">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-pie '></i>
                <span>当前瞬时数据</span>
            </div>
            <div id="FlowDataChart" class="flow_state_chart"></div>
            <!-- <div id="FlowNDataChart" class="flow_state_chart"></div>
            <div id="FlowADataChart" class="flow_state_chart"></div>
            <div id="FlowANDataChart" class="flow_state_chart"></div> -->
        </div>
        <!-- 正累计流量实时曲线折线图 -->
        <div class="wChart-Div" style="height: 380px;  width: 100%;">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-line '></i>
                <span>正瞬时流量实时曲线</span>
            </div>
            <div id="realtime_flow_chart_positive" style="height: 345px; width: 100%;">
            </div>
        </div>
        <!-- 负累计流量实时曲线折线图 -->
        <div class="wChart-Div" style="height: 380px;  width: 100%;">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-line '></i>
                <span>负瞬时流量实时曲线</span>
            </div>
            <div id="realtime_flow_chart_negative" style="height: 345px; width: 100%;"></div>
        </div>
        <!-- 日累计流量柱状图 -->
        <div class="wChart-Div" style="height: 380px;  width: 100%;">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-line '></i>
                <span>日累计流量柱状图</span>
            </div>
            <div id="daily_cumulative_flow_chart" style="height: 345px; width: 100%;"></div>
        </div>        
        <!-- 当前点位数据列表  -->
        <div class="physic_point_container">
            <div class="header">
                <i class='header_icon fas fa-lg fa-list-ul'></i>
                <span>监测点位数据</span>
            </div>
            <MuiList 
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
import echarts from "echarts";
import apiMonitor from "@api/monitor";
import dateHelper from "@common/dateHelper";
import consts from "@pages/features/consts";
import { wChart_Pie, wChart_Dashboard_Three } from "@JS/charts/charts";
import { BuilderGaugeChart, BuilderNFGaugeChart } from "@JS/charts/gauge";
import ChartBuilder from "@JS/charts/chart-builder";
import * as ChartOptions from "@JS/charts/chart-option";
import { deepCopy } from "@common/util";
import MuiList from "@comp/common/MuiList";
import nativeTransfer from "@JS/native/nativeTransfer";

export default {
  props: {
    pointId: {
      type: [String, Number],
      required: true
    },
    tableId: {
      type: [String, Number],
      required: true
    },
    pointName: {
      type: String,
      required: true
    }
  },
  mounted() {
    console.log("流量详情参数", this.pointId, this.pointName);
    this.$eventbus.$emit("set-title", `流量详情 | ${this.pointName}`);
    this.$showLoading();
    this.initPhysicPointData();
    this.initCurrentFlowChart();
    // 实时压力曲线
    this.initRealtimeFlowChart();
    // 日累计流量曲线
    this.initDailyCumulativeFlowChart();
  },
  data() {
    return {
      charts: {
        currentFlowChart: "",
        realtimeFlowChart: ""
      },
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
    initPhysicPointData() {
      apiMonitor
        .GetFieldList(this.tableId)
        .then(res => {
          let rawFieldList = res.data.Data;
          console.log("原始字段集合", rawFieldList);
          const ignoreFields = ["BatteryTime", "UploadCount", "EQType"];
          let fieldList = _.reject(_.uniqBy(rawFieldList, "field"), item => {
            return ignoreFields.includes(item.field);
          });
          console.log("去重后字段集合", fieldList);
          this.fieldList = fieldList;
          console.log("【流量】字段与表格初始化完成，开始加载数据表格...");
        })
        .catch(err => {
          console.log("Fields Err ", err);
        });
    },
    initCurrentFlowChart() {
      apiMonitor.GetFlowRTList(1, 500, this.pointName).then(
        res => {
          console.log("获取单个流量监测点数据", res.data.rows[0]);
          if (res.data.ErrCode == 0) {
            let data = res.data.rows[0];
            this.pointData = deepCopy(data);
            let {
              iPositiveFlowRate,
              iNegativeFlowRae,
              iPositiveAccumulativeFlow,
              iNegativeAccumulativeFlow,
              np
            } = data;
            BuilderNFGaugeChart(
              "FlowDataChart",
              iPositiveFlowRate,
              iNegativeFlowRae,
              np
            );
            // BuilderGaugeChart("FlowNDataChart", iNegativeFlowRate, "负瞬时");
            // BuilderGaugeChart(
            //   "FlowADataChart",
            //   iPositiveAccumulativeFlow,
            //   "正实际瞬时"
            // );
            // BuilderGaugeChart(
            //   "FlowANDataChart",
            //   iNegativeAccumulativeFlow,
            //   "负实际瞬时"
            // );
          } else {
            mui.toast(`获取当前监测点数据失败！`);
          }
        },
        err => {
          mui.toast(`获取当前监测点数据失败！`);
        }
      );
    },
    initRealtimeFlowChart() {
      // 正累计
      apiMonitor
        .GetChartData("title", this.tableId, this.pointId, "iPositiveFlowRate")
        .then(res => {
          let chartOption = res.data;
          console.log("cahrt", chartOption);
          let chart = (this.charts.realtimePressureChart = new ChartBuilder());
          chart.Init(
            document.getElementById("realtime_flow_chart_positive"),
            JSON.parse(JSON.stringify(ChartOptions.lineORBarOption)),
            "流量实时曲线",
            "BaiHui",
            1,
            "fa-line-chart"
          );
          chart.BuildLineChart(chartOption);
        });
      // 负累计
      apiMonitor
        .GetChartData("title", this.tableId, this.pointId, "iNegativeFlowRae")
        .then(res => {
          let chartOption = res.data;
          console.log("cahrt", chartOption);
          let chart = (this.charts.realtimePressureChart = new ChartBuilder());
          chart.Init(
            document.getElementById("realtime_flow_chart_negative"),
            JSON.parse(JSON.stringify(ChartOptions.lineORBarOption)),
            "流量实时曲线",
            "BaiHui",
            1,
            "fa-line-chart"
          );
          chart.BuildLineChart(chartOption);
          setTimeout(() => {
            this.$hideLoading();
          }, 500);
        });
    },
    initDailyCumulativeFlowChart() {
      let now = new Date();
      let oneMonthAgo = new Date().setMonth(now.getMonth() - 1);
      apiMonitor
        .GetFlowStatisticChartData(
          this.tableId,
          this.pointId,
          dateHelper.format(new Date(oneMonthAgo), "yyyy-MM-dd hh:mm:ss"),
          dateHelper.format(new Date(now), "yyyy-MM-dd hh:mm:ss")
        )
        .then(res => {
          console.log("%c日累计chart option", "color: red", res);
          let chartOption = deepCopy(res.data);
          chartOption.series[0].type = "bar";
          let container = document.getElementById(
            "daily_cumulative_flow_chart"
          );
          let dcfChart = echarts.init(container);
          dcfChart.setOption(chartOption);
        })
        .catch(err => {});
    },
    onRowIconClick(row, index, event) {
      if (
        row.id === "DataPointName" &&
        this.pointData.DataMapX &&
        this.pointData.DataMapY
      ) {
        nativeTransfer.startNavi(Number(this.pointData.DataMapX),Number(this.pointData.DataMapY), "", res=>{
          console.log(res)
        })

        /*if (window.plus && window.plus.maps && window.plus.geolocation) {
          this.fullscreenLoading = true;
           nativeTransfer.getLocation(position => {
             if(position){
                let srcPoint = new plus.maps.Point(
                  position.lng,
                  position.lat
                );
                let destDesc = "目标点位";
                let destPoint = new plus.maps.Point(
                  this.pointData.DataMapX,
                  this.pointData.DataMapY
                );
                nativeTransfer.startNavi(Number(this.pointData.DataMapX),Number(this.pointData.DataMapY), "", res=>{
                  console.log(res)
                })
                //window.plus.maps.openSysMap(destPoint, destDesc, srcPoint);
                this.fullscreenLoading = false;
              }else{
                this.fullscreenLoading = false;
                window.mui.toast("定位失败，无法调起导航");
              }
            }
          );
        } else {
          this.fullscreenLoading = false;
          window.mui.toast("定位失败，无法调起导航");
        }*/
      }
    }
  },
  components: { MuiList }
};
</script>

<style lang="less">
.flow_monitor_detail_container {
  width: 100%;
  height: calc(~"99vh-44px");
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
  .current_flow_charts_container {
    width: 100%;
    background-color: #fff;
    .flow_state_chart {
      height: 215px;
      width: 100%;
      margin: 0 auto;
    }
  }
  .physic_point_container {
    .point_data_list {
      font-size: 1.2rem;
    }
  }
}
</style>


