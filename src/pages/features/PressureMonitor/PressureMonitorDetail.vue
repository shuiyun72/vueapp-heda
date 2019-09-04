<template>
    <div class="pressure_monitor_detail_container">
        <!-- 当前运行数据饼图  -->
        <div class="current_pressure_container">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-pie '></i>
                <span>当前运行数据</span>
            </div>
            <div id="NPChart" style="width: 100%;height: 215px;" class="chart_container"></div>
        </div>
        <!-- 压力实时曲线折线图 -->
        <div class="wChart-Div" style="width: 100%; height: 335px;">
            <div class="header">
                <i class='header_icon fas fa-lg fa-chart-line '></i>
                <span>压力实时曲线</span>
            </div>
            <div id="realtime_pressure_chart" 
                class="chart_container" 
                style="height: 300px; width: 100%;"
            >
            </div>
        </div>
        <!-- 当前点位数据列表  -->
        <div class="physic_point_container">
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
import apiMonitor from "@api/monitor";
import dateHelper from "@common/dateHelper";
import consts from "@pages/features/consts";
import { wChart_Pie, wChart_Dashboard_Three } from "@JS/charts/charts";
import {
  BuilderGaugeChart,
  BuilderQGaugeChart,
  QOption
} from "@JS/charts/gauge";
import ChartBuilder from "@JS/charts/chart-builder";
import * as ChartOption from "@JS/charts/chart-option";
import MuiList from "@comp/common/MuiList";
import { deepCopy } from "@common/util";
import nativeTransfer from "@JS/native/nativeTransfer";

const dataTableId = consts.dataTableId.pressure;

export default {
  props: {
    pointId: {
      type: [String, Number],
      required: true
    },
    pointName: {
      type: String,
      required: true
    }
  },
  mounted() {
    console.log("压力详情参数", this.pointId, this.pointName);
    this.$eventbus.$emit("set-title", `压力详情 | ${this.pointName}`);
    this.$showLoading()
    this.initPhysicPointData();
    // 当前状态饼图
    this.initCurrentPressureChart();
    // 实时压力曲线
    this.initRealtimePressureChart();
  },
  data() {
    return {
      fullscreenLoading: false,
      charts: {
        currentPressureChart: "",
        realtimePressureChart: ""
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
    // 获取所有数据字段
    initPhysicPointData() {
      apiMonitor
        .GetFieldList(consts.dataTableId.pressure)
        .then(res => {
          let rawFieldList = res.data.Data;
          console.log("Raw Fields", rawFieldList);
          let fieldList = _.uniqBy(rawFieldList, "field");
          console.log("Uniq Fields", fieldList);
          // 不展示设备类型
          this.fieldList = _.reject(fieldList, { field: "EQType" });
          console.log("字段与列表初始化完成，开始加载列表数据...");
        })
        .catch(err => {
          console.log("Fields Err ", err);
        });
    },
    initCurrentPressureChart() {
      this.fetchPhysicPointData({}).then(
        ({ err, data }) => {
          console.log("获取单个监测点数据", data);
          if (!err) {
            this.pointData = deepCopy(data);
            let value = data.iPressureData;
            this.charts.currentPressureChart = BuilderGaugeChart(
              "NPChart",
              value
            );
          } else {
            mui.toast(`获取当前监测点数据失败！`);
          }
        },
        err => {
          mui.toast(`获取当前监测点数据失败！`);
        }
      );
    },
    initRealtimePressureChart() {
      // 图表的标题不由GetChartData的第一个参数决定，而是由下方Init的第三个参数决定
      apiMonitor
        .GetChartData("title", dataTableId, this.pointId, "iPressureData")
        .then(res => {
          let chartOption = res.data;
          console.log("cahrt", chartOption);
          let chart = (this.charts.realtimePressureChart = new ChartBuilder());
          chart.Init(
            document.getElementById("realtime_pressure_chart"),
            JSON.parse(JSON.stringify(ChartOption.lineORBarOption)),
            "压力实时曲线",
            "BaiHui",
            1,
            "fa-line-chart"
          );
          chart.BuildLineChart(chartOption);
          setTimeout(()=>{
            this.$hideLoading()
          },500)
        });
    },
    fetchPhysicPointData({
      pageNumber = 1,
      rowsPerPage = 5000,
      dataPointName = this.pointName
    }) {
      // 返回一个Promise对象
      return apiMonitor
        .GetPressureRTList(pageNumber, rowsPerPage, dataPointName)
        .then(
          res => {
            if (res.data.ErrCode == 0) {
              return {
                data: res.data.rows[0],
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
                position.coords.longitude,
                position.coords.latitude
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
.pressure_monitor_detail_container {
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
  .current_pressure_container {
    width: 100%;
    height: 250px;
    background-color: #fff;
  }
  .physic_point_container {
    .point_data_list {
      font-size: 1.2rem;
    }
  }
}
</style>


