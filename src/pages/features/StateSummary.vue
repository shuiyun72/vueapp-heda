<template>
    <div class="state_summary_container" 
        v-loading.fullscreen.lock="fullscreenLoading"
        element-loading-text="拼命加载中..."
    >
        <!-- 水量数字统计 -->
        <div class="panel_water_total">
            <div class="panel_row_water_total">
                <div class="panel_row_left">
                    <div class="wChart-KPI-Title" style="background-color: #358f8b; top: 0px"><span style="margin-left: 10px">本日供水总量</span></div>
                    <div style="opacity: 0.6; line-height: 109px;" class="wChart-KPI-Icon wChart-KPI-Transparent"><i class="fa fa-chart-area wChart-KPI-Transparent"></i></div>
                    <div class="wChart-KPI-V" style="bottom: 7px;">
                        <div class="wChart-KPI_Value" id="CKLJLL">{{flowExportTotal}}</div>
                        <div class="wChart-KPI_Unit">立方米 M<sup>3</sup></div>
                    </div>
                </div>
                <div class="panel_row_right">
                    <div style="position: absolute; bottom: 1px; height: 6px; width: 100%; background-color: #7b523c">&nbsp; </div>
                    <div class="wChart-KPI-Title" style="background-color: #7b523c; top: 0px"><span style="margin-left: 10px">供水瞬时总量</span></div>
                    <div style="opacity: 0.6; line-height: 109px;" class="wChart-KPI-Icon wChart-KPI-Transparent"><i class="fa fa-chart-area wChart-KPI-Transparent"></i></div>
                    <div class="wChart-KPI-V" style="bottom: 7px;">
                        <div class="wChart-KPI_Value" id="CKLJLL">{{flowExportInstant}}</div>
                        <div class="wChart-KPI_Unit">立方米 M<sup>3</sup>/小时</div>
                    </div>
                </div>
            </div>
            <div class="panel_row_water_total">
                <div class="panel_row_left">
                    <div class="wChart-KPI-Title" style="background-color: #358f8b; top: 0px"><span style="margin-left: 10px">本日原水总量</span></div>
                    <div style="opacity: 0.6; line-height: 109px;" class="wChart-KPI-Icon wChart-KPI-Transparent"><i class="fa fa-chart-area wChart-KPI-Transparent"></i></div>
                    <div class="wChart-KPI-V" style="bottom: 7px;">
                        <div class="wChart-KPI_Value" id="JKSSLL">{{flowImportTotal}}</div>
                        <div class="wChart-KPI_Unit">立方米 M<sup>3</sup></div>
                    </div>
                </div>
                <div class="panel_row_right">
                    <div style="position: absolute; bottom: 1px; height: 6px; width: 100%; background-color: #7b523c">&nbsp; </div>
                    <div class="wChart-KPI-Title" style="background-color: #7b523c; top: 0px"><span style="margin-left: 10px">原水瞬时总量</span></div>
                    <div style="opacity: 0.6; line-height: 109px;" class="wChart-KPI-Icon wChart-KPI-Transparent"><i class="fa fa-chart-area wChart-KPI-Transparent"></i></div>
                    <div class="wChart-KPI-V" style="bottom: 7px;">
                        <div class="wChart-KPI_Value" id="JKSSLL">{{flowImportInstant}}</div>
                        <div class="wChart-KPI_Unit">立方米 M<sup>3</sup>/小时</div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 水量饼图 -->
        <div style="height: 295px; padding-top: 10px; font-size: 1.3rem;">
            <div id="export_pie_chart" class="wChart-Pie-DOM">饼图1</div>
        </div>
        <div style="height: 295px; padding-top: 10px; font-size: 1.3rem;">
            <div id="import_pie_chart" class="wChart-Pie-DOM">饼图2</div>
        </div>
        <!-- 流量折线图 -->
        <div class="header">
            <!-- <i class='header_icon fas fa-lg fa-chart-line '></i> -->
            <span>本日限时流量曲线</span>
        </div>
        <div id="flow_line_chart" class="flow_line_chart">
        </div>
        <!-- 压力折线图 -->
        <div class="header">
            <!-- <i class='header_icon fas fa-lg fa-chart-line '></i> -->
            <span>本日压力曲线</span>
        </div>
        <div id="press_line_chart" class="press_line_chart">
        </div>
        <!-- 水质仪表盘 -->
        <div style="width: 100%; height: 510px; margin-top: 10px">
            <div style="width: 100%; color: #000; height: 35px; background-color: #199fcf; padding-left: -5px; line-height: 35px; font-size: 12pt;">
                <i class='fa fa-bar-chart-o  fa-lg wChart-KPI-Transparent wChart-All-Title'></i>出厂水质
            </div>
            <!-- 第一行 -->
            <div style="width: 100%; height: 215px; background-color: #FFF" id="SZ1">
            </div>
            <!-- 第二行 -->
            <div style="width: 100%; height: 250px; margin-top: 10px">
                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 250px; background-color: #FFF;">
                    <tr>
                        <td style="width: 50%; height: 200px">
                            <div style="width: 100%; height: 200px; position: relative" id="SZ2"></div>
                            <div style="width: 100%; height: 30px; position: relative; font-size: 20px; text-align: center">浊 度</div>
                        </td>
                        <td>
                            <div style="width: 100%; height: 200px; position: relative;" id="SZ3"></div>
                            <div style="width: 100%; height: 30px; position: relative; font-size: 20px; text-align: center">电导率</div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- 万益水源井启停状态 由于移动端宽度这里分了三个表格展示 -->
        <div class="pump_state_container">
            <table class="pump_table">
                <tr>
                    <td class="pump_label">万益水源井</td>
                    <td id="variable_172">01</td>
                    <td id="variable_199">02</td>
                    <td id="variable_221">03</td>
                    <td id="variable_216">04</td>
                    <td id="variable_211">05</td>
                    <td id="variable_205">06</td>
                    <td id="variable_231">07</td>
                    <td id="variable_227">08</td>
                    <td id="variable_222">09</td>
                </tr>
            </table>
            <table class="pump_table">
                <tr>
                    <td class="pump_label">万益水源井</td>
                    <td id="variable_248">10</td>
                    <td id="variable_242">11</td>
                    <td id="variable_235">12</td>
                    <td id="variable_177">13</td>
                    <td id="variable_174">14</td>
                    <td id="variable_200">15</td>
                    <td id="variable_170">16</td>
                    <td id="variable_201">17</td>
                    <td id="variable_122">18</td>
                </tr>
            </table>
            <table class="pump_table">
                <tr>
                    <td class="pump_label">万益水源井</td>
                    <td id="variable_171">19</td>
                    <td id="variable_217">20</td>
                    <td id="variable_243">21</td>
                    <td id="variable_187">22</td>
                    <td id="variable_236">23</td>
                    <td id="variable_178">24</td>
                    <td id="variable_203">25</td>
                    <td id="variable_228">26</td>
                    <td id="variable_223">27</td>
                </tr>
            </table>
            <!-- 甘珠庙一期水源井启停状态 -->
            <table class="pump_table">
                <tr>
                    <td class="pump_label" style="width: 100px; background-color: #000; color: #00a948;">甘珠庙一期</td>
                    <td id="variable_68">01</td>
                    <td id="variable_85">02</td>
                    <td id="variable_25">03</td>
                    <td id="variable_52">04</td>
                    <td id="variable_119">05</td>
                    <td id="variable_9">06</td>
                    <td id="variable_27">07</td>
                    <td id="variable_98">08</td>
                    <td id="variable_113">09</td>
                </tr>
            </table>
            <table class="pump_table">
                <tr>
                    <td class="pump_label" style="width: 100px; background-color: #000; color: #00a948;">甘珠庙一期</td>
                    <td id="variable_55">10</td>
                    <td id="variable_71">11</td>
                    <td id="variable_87">12</td>
                    <td id="variable_30">13</td>
                    <td id="variable_46">14</td>
                    <td id="variable_56">15</td>
                    <td id="variable_73">16</td>
                    <td id="variable_985">17</td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <div class="detail_table_container">
            <el-table 
                :data="detailTableData" 
                border
                header-row-class-name="table_header_rows"
                :row-style="rowStyle"
                :cell-class-name="cellClassName"
                :span-method="spanStrategy" 
                style="width: 100%; text-align: center;">
                <!-- 名称 -->
                <el-table-column
                  prop="name"
                  label="名称"
                  fixed
                  >
                </el-table-column>
                <!-- 进水流量 -->
                <el-table-column
                  colspan="3"
                  prop="importFlow"
                  label="进水流量">
                </el-table-column>
                <el-table-column
                  prop="importFlow1"
                  label="进水流量">
                </el-table-column>
                <el-table-column
                  prop="importFlow2"
                  label="进水流量">
                </el-table-column>
                <!-- 调节池 -->
                <el-table-column
                  prop="regulator"
                  label="调节池">
                </el-table-column>
                <el-table-column
                  prop="regulator1"
                  label="调节池"
                  >
                </el-table-column>
                <!-- 水泵 -->
                <el-table-column
                  prop="waterPump"
                  label="水泵">
                </el-table-column>
                <el-table-column
                  prop="waterPump1"
                  label="水泵">
                </el-table-column>
                <el-table-column
                  prop="waterPump2"
                  label="水泵">
                </el-table-column>
                <el-table-column
                  prop="waterPump3"
                  label="水泵">
                </el-table-column>
                <!-- 阀门 -->
                <el-table-column
                  prop="valve"
                  label="阀门"
                  >
                </el-table-column>
                <el-table-column
                  prop="valve1"
                  label="阀门">
                </el-table-column>
                <!-- 出水流量 -->
                <el-table-column
                  prop="exportFlow"
                  label="出水流量">
                </el-table-column>
                <el-table-column
                  class-name="fix_hidden_bug"
                  prop="exportFlow1"
                  label="出水流量">
                </el-table-column>
                <!-- 出水压力 -->
                <el-table-column
                  class-name="fix_hidden_bug"
                  prop="exportPress"
                  label="出水压力">
                </el-table-column>
            </el-table>
        </div>
    </div>
</template>

<script>
import $ from "jquery";
import _ from "lodash";
import { wChart_Pie, wChart_Dashboard_Three } from "@JS/charts/charts";
import { BuilderQGaugeChart, QOption } from "@JS/charts/gauge";
import ChartBuilder from "@JS/charts/chart-builder";
import * as ChartOption from "@JS/charts/chart-option";
import apiMonitor from "@api/monitor";

export default {
  mounted() {
    // 发送请求， 获取运行总览数据
    apiMonitor
      .GetWaterOverview()
      .then(res => {
        console.log("运行总览数据", res);
        // 计算水量数据
        let data = res.data.Data;
        this.flowImportTotal = data.FlowIn.reduce((t, e) => {
          return parseFloat(t) + parseFloat(e);
        });
        this.flowImportInstant = parseFloat(data.FlowINInstant, 2).toFixed(2);
        this.flowExportTotal = data.FlowOut.reduce((t, e) => {
          return parseFloat(t) + parseFloat(e);
        });
        this.flowExportInstant = parseFloat(data.FlowOUTInstant).toFixed(2);
        // 绘制饼图图表
        // 供水
        this.exportWaterChart = new wChart_Pie(
          "本日各供水系统供水总量占比",
          document.getElementById("export_pie_chart"),
          "fa-pie-chart",
          "TianLan"
        );
        this.exportWaterChart.SetData([
          { value: data.FlowOut[0], name: "自流DN700管线给水" },
          { value: data.FlowOut[1], name: "加压DN600管线给水" },
          { value: data.FlowOut[2], name: "调度室至康巴什" },
          { value: data.FlowOut[3], name: "空港至装备基地累计流量" },
          { value: data.FlowOut[4], name: "空港至红海子" }
        ]);
        // 原水
        this.importWaterChart = new wChart_Pie(
          "本日各原水系统供水总量占比",
          document.getElementById("import_pie_chart"),
          "fa-pie-chart",
          "MeiHong"
        );
        this.importWaterChart.SetData([
          { value: data.FlowIn[0], name: "配水厂进水" },
          { value: data.FlowIn[1], name: "甘珠庙一期进水" },
          { value: data.FlowIn[2], name: "配水厂进水南" },
          { value: data.FlowIn[3], name: "配水厂进水北" }
        ]);

        // 绘制流量与压力折线图
        // 流量折线图
        let LineChartInfo = (this.flowLineChart = new ChartBuilder());
        LineChartInfo.Init(
          document.getElementById("flow_line_chart"),
          ChartOption.lineORBarOption,
          "本日瞬时流量曲线",
          "BaiHui",
          1,
          "fa-chart-line"
        );
        //取得流量图表数据
        LineChartInfo.GetChartData(
          "http://47.104.3.68:5283/MonitorAPI/api/WaterFactory/FlowChart",
          (err, flowLineChartData) => {
            //图表数据绘制
            if (err) {
              console.error("获取流量图表数据失败", err.message);
            } else {
              console.log("流量", flowLineChartData);
              LineChartInfo.Option.legend.data = flowLineChartData.legend.data;
              LineChartInfo.Option.xAxis[0].data = flowLineChartData.xAxis.data;
              LineChartInfo.Option.series = flowLineChartData.series;
            }
            LineChartInfo.SetOption();
          }
        );

        //压力折线图
        let LineChartInfo1 = (this.pressLineChart = new ChartBuilder());
        LineChartInfo1.Init(
          document.getElementById("press_line_chart"),
          ChartOption.lineORBarOption,
          "本日压力曲线",
          "BaiHui",
          1,
          "fa-chart-line"
        );
        //取得压力图表数据
        LineChartInfo1.GetChartData(
          "http://47.104.3.68:5283/MonitorAPI/api/WaterFactory/PressureChart",
          (err, pressChartData) => {
            //图表数据绘制
            if (err) {
              console.error("获取压力图表数据失败");
            } else {
              console.log("压力", pressChartData);
              LineChartInfo1.Option.legend.data = pressChartData.legend.data;
              LineChartInfo1.Option.xAxis[0].data = pressChartData.xAxis.data;
              LineChartInfo1.Option.series = pressChartData.series;
            }
            LineChartInfo1.SetOption();
            this.fullscreenLoading = false;
          }
        );

        // 水质仪表盘
        //余氯	    yulv
        //浊度	    zhuodu
        //PH	    PH
        //电导率	DianDaolv
        //温度	    wendu
        let yulv = 0.02;
        let zhuodu = 0.001;
        let PH = 7.8;
        let DianDaolv = 0;
        let wendu = 23;
        // 万益一期水源井启停状态
        $(data.GZMSYJ).each(function(index, RowSYJ) {
          if ($("#" + RowSYJ.DataFieldEName)) {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              if (parseFloat(RowSYJ.iValue) > 0) {
                $("#" + RowSYJ.DataFieldEName).css({
                  "background-color": "#FF0000"
                });
              } else {
                $("#" + RowSYJ.DataFieldEName).css({
                  "background-color": "#009966"
                });
              }
            }
          }
        });

        // 甘珠庙水源井启停状态
        $(data.MZSYJ).each(function(index, RowSYJ) {
          if ($("#" + RowSYJ.DataFieldEName)) {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              if (parseFloat(RowSYJ.iValue) > 0) {
                $("#" + RowSYJ.DataFieldEName).css({
                  "background-color": "#FF0000"
                });
              } else {
                $("#" + RowSYJ.DataFieldEName).css({
                  "background-color": "#009966"
                });
              }
            }
          }
        });

        //甘珠庙水源井启停状态
        $(data.WaterQuality).each(function(index, RowSYJ) {
          if (RowSYJ.DataFieldEName == "yulv") {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              yulv = RowSYJ.iValue;
            }
          } else if (RowSYJ.DataFieldEName == "zhuodu") {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              zhuodu = RowSYJ.iValue;
            }
          } else if (RowSYJ.DataFieldEName == "PH") {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              PH = RowSYJ.iValue;
            }
          } else if (RowSYJ.DataFieldEName == "DianDaolv") {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              DianDaolv = RowSYJ.iValue;
            }
          } else if (RowSYJ.DataFieldEName == "wendu") {
            if (RowSYJ.iValue != "null" && RowSYJ.iValue != "") {
              wendu = RowSYJ.iValue;
            }
          }
        });

        // 余氯，浊度，PH 图表展示
        let SZChart = new wChart_Dashboard_Three(
          "",
          document.getElementById("SZ1"),
          "",
          ""
        );
        SZChart.SetData([yulv, PH, wendu], ["余氯mg/L", "PH", "温度"]);

        let ZDChart = BuilderQGaugeChart("SZ2", zhuodu); //浊度
        let DDLChart = BuilderQGaugeChart("SZ3", DianDaolv); //电导率

        // 获取并解析表格数据
        let dataTableIds = "75,76,77,78";
        this.generateDetailTableData(dataTableIds);
      })
      .catch(err => {
        console.log("运行总览获取数据过程错误", err);
        this.fullscreenLoading = false;
        window.mui.toast("获取数据失败，请检查网络");
      });
  },
  data() {
    return {
      fullscreenLoading: true,
      // 进口总流量
      flowImportTotal: 0,
      // 进口瞬时流量
      flowImportInstant: 0,
      // 出口总流量
      flowExportTotal: 0,
      // 出口瞬时流量
      flowExportInstant: 0,

      // 图表实例引用
      exportWaterChart: null,
      importWaterChart: null,
      flowLineChart: null,
      pressLineChart: null,

      // 双向绑定的表格数据
      detailTableData: [
        {
          name: "奉化水厂",
          importFlow: "圣园供水",
          importFlow1: "甘朱庙一期",
          importFlow2: "万益供水",
          regulator: 2000,
          regulator1: 5000,
          waterPump: "1#",
          waterPump1: "2#",
          waterPump2: "3#",
          waterPump3: "4#",
          valve: "600加压",
          valve1: "700自流",
          exportFlow: "600加压",
          exportFlow1: "700自流",
          exportPress: "600加压"
        },
        {
          name: "",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "甘珠庙",
          importFlow: "进水南",
          importFlow1: "进水北",
          importFlow2: "",
          regulator: "1#",
          regulator1: "2#",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "至奉化",
          valve1: "至空港",
          exportFlow: "至奉化",
          exportFlow1: "至空港",
          exportPress: "至空港"
        },
        {
          name: "",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "汽车城",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "空港",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "康巴什",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "装备基地",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        },
        {
          name: "红海子",
          importFlow: "",
          importFlow1: "",
          importFlow2: "",
          regulator: "",
          regulator1: "",
          waterPump: "",
          waterPump1: "",
          waterPump2: "",
          waterPump3: "",
          valve: "",
          valve1: "",
          exportFlow: "",
          exportFlow1: "",
          exportPress: ""
        }
      ]
    };
  },
  methods: {
    // 转换table数据结构
    generateDetailTableData(dataTableIds) {
      apiMonitor
        .GetTableData(dataTableIds)
        .then(res => {
          console.log("Data table ", res);
          let columnData = res.data.Data;
          if (false) {
            //初始化当前监测点基础数据信息
            $.each(columnData, function(index, item) {
              console.log("each", $("." + item.DataFieldEName));
              if ($("." + item.DataFieldEName)) {
                if (item.IDKID == 10 || item.IDKID == 11 || item.IDKID == 12) {
                  if ($("." + item.DataFieldEName + "_1")) {
                    if (item.IValue > 0) {
                      console.warn("dayu0");
                      $("." + item.DataFieldEName + "_1" + " div.cell").text(
                        "开"
                      );
                    } else {
                      $("." + item.DataFieldEName + "_1" + " div.cell").text(
                        "关"
                      );
                    }
                  }
                }
                if (item.IDataType == "1") {
                  //开关类型
                  if (item.IValue == 1) {
                    $("." + item.DataFieldEName).text("运行");
                  } else if (item.IValue == 0) {
                    $("." + item.DataFieldEName).text("停止");
                  }
                } else if (item.IDataType == "2") {
                  //浮点数类型
                  var DesValue = "";
                  if (item.IValue) {
                    DesValue =
                      Number(item.IValue)
                        .toString()
                        .indexOf(".") == -1
                        ? item.IValue
                        : Number(item.IValue).toFixed(2);
                  } else {
                    DesValue = item.IValue;
                  }
                  $("." + item.DataFieldEName).text(DesValue);
                } else if (item.IDataType == "3") {
                  //整数类型
                  var DesValue = "";
                  if (item.IValue) {
                    DesValue == item.IValue;
                  }
                  $("." + item.DataFieldEName).text(item.IValue);
                } else if (item.IDataType == "4") {
                  //报警类型
                  if (item.IValue == 1) {
                    $("." + item.DataFieldEName).text("故障");
                  } else if (item.IValue == 0) {
                    $("." + item.DataFieldEName).text("正常");
                  }
                } else {
                  //整数类型
                  var DesValue = "";
                  if (item.IValue) {
                    DesValue == item.IValue;
                  }
                  $("." + item.DataFieldEName).text(DesValue);
                }
              }
            });
          }
        })
        .catch(err => {
          console.log("data table err", err);
        });
      let columnData = JSON.parse(
        '[{"DataFieldEName":"variable_105_75_8","iValue":77.03993,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_22_75_8","iValue":50.0,"iDataType":2,"iDKID":16,"iDKUnit":"HZ"},{"DataFieldEName":"variable_104_75_9","iValue":34.11458,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_21_75_9","iValue":30.0,"iDataType":2,"iDKID":16,"iDKUnit":"HZ"},{"DataFieldEName":"variable_103_75_10","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_20_75_10","iValue":0.0,"iDataType":2,"iDKID":16,"iDKUnit":"HZ"},{"DataFieldEName":"variable_102_75_11","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_19_75_11","iValue":0.0,"iDataType":2,"iDKID":16,"iDKUnit":"HZ"},{"DataFieldEName":"variable_68_76_1","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_24_76_1","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_124_76_1","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_85_76_2","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_42_76_2","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_112_76_2","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_25_76_3","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_60_76_3","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_51_76_3","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_96_76_4","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_26_76_4","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_119_76_5","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_36_76_5","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_94_76_5","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_9_76_6","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_53_76_6","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_110_76_6","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_27_76_7","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_69_76_7","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_97_76_7","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_98_76_8","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_44_76_8","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_70_76_8","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_113_76_9","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_80_76_9","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_86_76_9","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_55_76_10","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_99_76_10","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_29_76_10","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_71_76_11","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_114_76_11","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_45_76_11","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_87_76_12","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_3_76_12","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_61_76_12","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_72_76_13","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_100_76_13","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_30_76_13","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_46_76_14","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_81_76_14","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_11_76_14","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_56_76_15","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_101_76_15","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_31_76_15","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_73_76_16","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_38_76_16","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_95_76_16","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"GZM_F_LJ_3_77_1","iValue":2388307.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F_3_77_1","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"GZM_fa_ai_1_77_1","iValue":7.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"GZM_F_LJ_4_77_2","iValue":2827137.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F_4_77_2","iValue":401.7578,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"GZM_fa_ai_2_77_2","iValue":16.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"GZM_F_LJ_1_77_3","iValue":3344818.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F_1_77_3","iValue":244.5313,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"GZM_F_LJ_2_77_4","iValue":1852997.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F_2_77_4","iValue":389.0625,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"ddzx_F_LJ_77_5","iValue":3.566786,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"ddzx_F_77_5","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"ddzx_fankui_77_5","iValue":30.13953,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"ddzx_P_77_5","iValue":0.4674083,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"F_LJ_1_77_6","iValue":142369.2,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"F_1_77_6","iValue":17.22656,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"F_LJ_2_77_7","iValue":237926.6,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"F_2_77_7","iValue":38.84766,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"GZM_L_1_77_8","iValue":1.895313,"iDataType":2,"iDKID":35,"iDKUnit":"m"},{"DataFieldEName":"L_1_77_9","iValue":2.855859,"iDataType":2,"iDKID":35,"iDKUnit":"m"},{"DataFieldEName":"jyf_in_77_10","iValue":0.2,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"jyf_out_77_10","iValue":0.22,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P1_in_77_11","iValue":0.48,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P1_out_77_11","iValue":0.13,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P2_in_77_12","iValue":0.48,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P2_out_77_12","iValue":0.02,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P3_in_77_13","iValue":0.64,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P3_out_77_13","iValue":0.2,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P4_in_77_14","iValue":0.31,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"SJDD_P4_out_77_14","iValue":0.39,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_172_78_1","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_139_78_1","iValue":368.3765,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_118_78_1","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_87_78_1","iValue":35.88235,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_58_78_1","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_63_78_1","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_4_78_1","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_199_78_2","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_11_78_2","iValue":373.7412,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_33_78_2","iValue":371.9529,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_130_78_2","iValue":30.73771,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_124_78_2","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_93_78_2","iValue":0.0,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_125_78_2","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_221_78_3","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_1_78_3","iValue":389.8353,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_47_78_3","iValue":384.4706,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_120_78_3","iValue":22.64706,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_164_78_3","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_113_78_3","iValue":0.7843137,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_38_78_3","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_216_78_4","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_66_78_4","iValue":368.3765,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_136_78_4","iValue":366.5882,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_162_78_4","iValue":33.23529,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_156_78_4","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_84_78_4","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_104_78_4","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_211_78_5","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_55_78_5","iValue":388.0471,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_161_78_5","iValue":388.0471,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_155_78_5","iValue":30.29412,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_27_78_5","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_65_78_5","iValue":0.0,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_149_78_5","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_205_78_6","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_77_78_6","iValue":382.6823,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_147_78_6","iValue":27.94118,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_78_78_6","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_142_78_6","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_231_78_7","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_95_78_7","iValue":373.7412,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_69_78_7","iValue":373.7412,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_36_78_7","iValue":32.94118,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_14_78_7","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_59_78_7","iValue":0.0,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_133_78_7","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_227_78_8","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_88_78_8","iValue":366.5882,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_5_78_8","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_12_78_8","iValue":32.35294,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_107_78_8","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_48_78_8","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_49_78_8","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_222_78_9","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_75_78_9","iValue":384.4706,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_165_78_9","iValue":379.1059,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_2_78_9","iValue":34.70588,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_114_78_9","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_39_78_9","iValue":0.7843137,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_157_78_9","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_248_78_10","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_67_78_10","iValue":389.8353,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_105_78_10","iValue":391.6235,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_68_78_10","iValue":32.35294,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_28_78_10","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_150_78_10","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_79_78_10","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_242_78_11","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_112_78_11","iValue":375.5294,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_21_78_11","iValue":364.8,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_56_78_11","iValue":40.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_22_78_11","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_143_78_11","iValue":1.176471,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_70_78_11","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_235_78_12","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_102_78_12","iValue":380.8941,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_15_78_12","iValue":382.6823,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_46_78_12","iValue":40.58823,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_134_78_12","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_60_78_12","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_6_78_12","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_177_78_13","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_96_78_13","iValue":382.6823,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_108_78_13","iValue":384.4706,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_37_78_13","iValue":37.94118,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_50_78_13","iValue":0.2401961,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_166_78_13","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_167_78_13","iValue":6.617647,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_174_78_14","iValue":0.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_115_78_14","iValue":0.0,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_85_78_14","iValue":394.1176,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_89_78_14","iValue":0.0,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_40_78_14","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_9_78_14","iValue":0.7843137,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_158_78_14","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_200_78_15","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_106_78_15","iValue":380.8941,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_32_78_15","iValue":380.8941,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_131_78_15","iValue":32.05882,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_29_78_15","iValue":0.05392157,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_26_78_15","iValue":0.0,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_151_78_15","iValue":5.147059,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_170_78_16","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_80_78_16","iValue":374.7059,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_154_78_16","iValue":372.9216,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_121_78_16","iValue":37.64706,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_23_78_16","iValue":0.1568628,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_101_78_16","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_144_78_16","iValue":4.852941,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_201_78_17","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_132_78_17","iValue":375.5294,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_73_78_17","iValue":395.2,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_3_78_17","iValue":31.47059,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_126_78_17","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_18_78_17","iValue":2.745098,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_71_78_17","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_171_78_18","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_16_78_18","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_94_78_18","iValue":357.6471,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_122_78_18","iValue":39.0625,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_116_78_18","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_34_78_18","iValue":1.960784,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_61_78_18","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_217_78_19","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_168_78_19","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_129_78_19","iValue":371.9529,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_57_78_19","iValue":35.29412,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_98_78_19","iValue":0.1862745,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_74_78_19","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_41_78_19","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_243_78_20","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_159_78_20","iValue":388.0471,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_169_78_20","iValue":393.4118,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_103_78_20","iValue":44.41177,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_99_78_20","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_42_78_20","iValue":0.7843137,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_160_78_20","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_187_78_21","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_148_78_21","iValue":39.11765,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_152_78_21","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_82_78_21","iValue":30.38824,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_236_78_22","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_24_78_22","iValue":414.8706,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_127_78_22","iValue":377.3177,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_97_78_22","iValue":35.88235,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_72_78_22","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_17_78_22","iValue":4.705883,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_117_78_22","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_178_78_23","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_62_78_23","iValue":384.4706,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_111_78_23","iValue":373.7412,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_141_78_23","iValue":32.05882,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_8_78_23","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_54_78_23","iValue":1.176471,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_110_78_23","iValue":6.764706,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_203_78_24","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_52_78_24","iValue":375.5294,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_53_78_24","iValue":373.7412,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_13_78_24","iValue":37.64706,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_153_78_24","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_100_78_24","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_43_78_24","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_228_78_25","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_145_78_25","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_44_78_25","iValue":370.1647,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_76_78_25","iValue":28.52941,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_91_78_25","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_146_78_25","iValue":0.0,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_31_78_25","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_223_78_26","iValue":1.0,"iDataType":1,"iDKID":17,"iDKUnit":null},{"DataFieldEName":"variable_135_78_26","iValue":332.6118,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_19_78_26","iValue":355.8588,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_123_78_26","iValue":38.52941,"iDataType":2,"iDKID":10,"iDKUnit":"A"},{"DataFieldEName":"variable_83_78_26","iValue":0.0,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_119_78_26","iValue":3.529412,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"variable_25_78_26","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_45_78_6","iValue":379.1059,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_20_78_6","iValue":0.0,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_90_78_21","iValue":null,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_30_78_21","iValue":363.0118,"iDataType":2,"iDKID":7,"iDKUnit":"V"},{"DataFieldEName":"variable_81_78_21","iValue":0.3921569,"iDataType":1,"iDKID":38,"iDKUnit":"%"},{"DataFieldEName":"GZM_F3_DAY1_77_1","iValue":0.0,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F4_DAY1_77_2","iValue":7933.349,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F1_DAY1_77_3","iValue":6630.774,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"GZM_F2_DAY1_77_4","iValue":2924.711,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"F1_DAY1_77_6","iValue":539.2579,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"F1_DAY2_77_7","iValue":895.0577,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"fa_ai_2_77_6","iValue":0.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"fa_ai_1_77_7","iValue":9.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"yulv_75_12","iValue":0.39,"iDataType":2,"iDKID":18,"iDKUnit":"mg/L"},{"DataFieldEName":"zhuodu_75_12","iValue":0.0,"iDataType":2,"iDKID":19,"iDKUnit":"NTU"},{"DataFieldEName":"PH_75_12","iValue":7.83,"iDataType":2,"iDKID":20,"iDKUnit":null},{"DataFieldEName":"DianDaolv_75_12","iValue":481.5,"iDataType":2,"iDKID":21,"iDKUnit":"s/m"},{"DataFieldEName":"wendu_75_12","iValue":29.2,"iDataType":2,"iDKID":27,"iDKUnit":null},{"DataFieldEName":"fa_ao_2_77_6","iValue":100.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"fa_ao_1_77_7","iValue":100.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"GZM_fa_ao_1_77_1","iValue":10.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"GZM_fa_ao_2_77_2","iValue":15.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"ddxz_control_77_5","iValue":0.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"ddzx_F_DAY1_77_5","iValue":null,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_32_75_1","iValue":3885675.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"variable_69_75_1","iValue":316.6667,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_28_75_1","iValue":10341.76,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_31_75_2","iValue":4942764.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"variable_114_75_2","iValue":492.7662,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_27_75_2","iValue":9378.591,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_23_75_3","iValue":870573.3,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"variable_12_75_3","iValue":12.29745,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_24_75_3","iValue":200.876,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_157_75_6","iValue":2.960069,"iDataType":2,"iDKID":35,"iDKUnit":"m"},{"DataFieldEName":"variable_33_75_7","iValue":2.893518,"iDataType":2,"iDKID":35,"iDKUnit":"m"},{"DataFieldEName":"variable_30_75_4","iValue":8113707.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"variable_158_75_4","iValue":735.4167,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_8_75_4","iValue":0.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"variable_26_75_4","iValue":15282.67,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_2_75_4","iValue":100.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"variable_29_75_5","iValue":1566474.0,"iDataType":3,"iDKID":2,"iDKUnit":"m³"},{"DataFieldEName":"variable_66_75_5","iValue":243.0556,"iDataType":2,"iDKID":1,"iDKUnit":"m³/h"},{"DataFieldEName":"variable_88_75_5","iValue":0.4003074,"iDataType":2,"iDKID":6,"iDKUnit":"Mpa"},{"DataFieldEName":"variable_11_75_5","iValue":96.49884,"iDataType":2,"iDKID":36,"iDKUnit":"%"},{"DataFieldEName":"variable_25_75_5","iValue":3739.902,"iDataType":2,"iDKID":41,"iDKUnit":"m³"},{"DataFieldEName":"variable_3_75_5","iValue":0.0,"iDataType":2,"iDKID":36,"iDKUnit":"%"}]'
      );
      if (columnData) {
        //初始化当前监测点基础数据信息
        $.each(columnData, function(index, item) {
          if ($("." + item.DataFieldEName)) {
            if (item.iDKID == 10 || item.iDKID == 11 || item.iDKID == 12) {
              if ($("." + item.DataFieldEName + "_1")) {
                if (item.iValue > 0) {
                  $("." + item.DataFieldEName + "_1" + " div.cell").text("开");
                } else {
                  $("." + item.DataFieldEName + "_1" + " div.cell").text("关");
                }
              }
            }
            if (item.iDataType == "1") {
              //开关类型
              if (item.iValue == 1) {
                $("." + item.DataFieldEName).text("运行");
              } else if (item.iValue == 0) {
                $("." + item.DataFieldEName).text("停止");
              }
            } else if (item.iDataType == "2") {
              //浮点数类型
              var DesValue = "";
              if (item.iValue) {
                DesValue =
                  item.iValue.toString().indexOf(".") == -1
                    ? item.iValue
                    : item.iValue.toFixed(2);
              } else {
                DesValue = item.iValue;
              }
              $("." + item.DataFieldEName).text(DesValue);
            } else if (item.iDataType == "3") {
              //整数类型
              var DesValue = "";
              if (item.iValue) {
                DesValue == item.iValue;
              }
              $("." + item.DataFieldEName).text(item.iValue);
            } else if (item.iDataType == "4") {
              //报警类型
              if (item.iValue == 1) {
                $("." + item.DataFieldEName).text("故障");
              } else if (item.iValue == 0) {
                $("." + item.DataFieldEName).text("正常");
              }
            } else {
              //整数类型
              var DesValue = "";
              if (item.iValue) {
                DesValue == item.iValue;
              }
              $("." + item.DataFieldEName).text(DesValue);
            }
          }
        });
      }
    },
    rowStyle({ rowIndex }) {
      if (rowIndex == 0 || rowIndex == 2) {
        return {
          color: "#000",
          // 灰色
          backgroundColor: "#AEAAAA"
        };
      } else if (rowIndex == 1) {
        return {
          color: "#000",
          // 深蓝
          backgroundColor: "#2F75B5"
        };
      } else if (rowIndex == 3) {
        return {
          color: "#000",
          // 浅蓝
          backgroundColor: "#00B0F0"
        };
      } else if (rowIndex % 2 == 0) {
        return {
          color: "#000",
          // 深蓝
          backgroundColor: "#2F75B5"
        };
      } else if (rowIndex % 2 == 1) {
        return {
          color: "#000",
          // 浅蓝
          backgroundColor: "#00B0F0"
        };
      }
    },
    cellClassName({ rowIndex, columnIndex }) {
      let className = "";
      switch (rowIndex) {
        // 奉化水厂行
        case 1:
          switch (columnIndex) {
            case 1:
              className = "variable_69_75_1";
              break;
            case 2:
              className = "variable_12_75_3";
              break;
            case 3:
              className = "variable_114_75_2";
              break;
            case 4:
              className = "variable_157_75_6";
              break;
            case 5:
              className = "variable_33_75_7";
              break;
            case 6:
              className = "variable_105_75_8_1";
              break;
            case 7:
              className = "variable_104_75_9_1";
              break;
            case 8:
              className = "variable_103_75_10_1";
              break;
            case 9:
              className = "variable_102_75_11_1";
              break;
            case 10:
              className = "variable_11_75_5";
              break;
            case 11:
              className = "lbltitle11";
              break;
            case 12:
              className = "variable_88_75_5_1";
              break;
            case 13:
              className = "variable_158_75_4";
              break;
            case 14:
              className = "variable_88_75_5";
              break;
          }
          break;
        // 甘珠庙行
        case 3:
          switch (columnIndex) {
            case 1:
              className = "GZM_F_4_77_2";
              break;
            case 2:
              className = "GZM_F_3_77_1";
              break;
            case 3:
              className = "lbltitle17";
              break;
            case 4:
              className = "GZM_L_1_77_8";
              break;
            case 5:
              className = "lbltitle19";
              break;
            case 6:
              className = "lbltitle20";
              break;
            case 7:
              className = "lbltitle21";
              break;
            case 8:
              className = "lbltitle22";
              break;
            case 9:
              className = "lbltitle23";
              break;
            case 10:
              className = "lbltitle24";
              break;
            case 11:
              className = "lbltitle25";
              break;
            case 12:
              className = "GZM_F_1_77_3";
              break;
            case 13:
              className = "GZM_F_2_77_4";
              break;
            case 14:
              className = "jyf_in_77_10";
              break;
          }
          break;
        // 康巴什
        case 6:
          switch (columnIndex) {
            case 12:
              className = "ddzx_F_77_5";
              break;
            case 14:
              className = "ddzx_P_77_5";
              break;
          }
          break;
        // 装备基地
        case 7:
          switch (columnIndex) {
            case 12:
              className = "F_1_77_6";
              break;
            case 14:
              className = "SJDD_P3_out_77_13";
              break;
          }
          break;
        // 红海子
        case 8:
          switch (columnIndex) {
            case 12:
              className = "F_2_77_7";
              break;
            case 14:
              className = "SJDD_P4_in_77_14";
              break;
          }
          break;
      }
      return className;
    },
    spanStrategy({ row, column, rowIndex, columnIndex }) {
      let normal = [1, 1];
      let hidden = [0, 0];
      let span = normal;
      switch (columnIndex) {
        case 0:
          switch (rowIndex) {
            case 0:
              span = [2, 1];
              break;
            case 1:
              span = hidden;
              break;
            case 2:
              span = [2, 1];
              break;
            case 3:
              span = hidden;
              break;
          }
          break;
        case 1:
          switch (rowIndex) {
            case 4:
              span = [1, 3];
              break;
            case 5:
              span = [1, 3];
              break;
            case 6:
              span = [1, 3];
              break;
            case 7:
              span = [1, 3];
              break;
            case 8:
              span = [1, 3];
              break;
          }
          break;
        case 2:
          switch (rowIndex) {
            case 4:
              span = hidden;
              break;
            case 5:
              span = hidden;
              break;
            case 6:
              span = hidden;
              break;
            case 7:
              span = hidden;
              break;
            case 8:
              span = hidden;
              break;
          }
          break;
        case 3:
          switch (rowIndex) {
            case 4:
              span = hidden;
              break;
            case 5:
              span = hidden;
              break;
            case 6:
              span = hidden;
              break;
            case 7:
              span = hidden;
              break;
            case 8:
              span = hidden;
              break;
          }
          break;
        case 4:
          switch (rowIndex) {
            case 5:
              span = [1, 2];
              break;
          }
          break;
        case 5:
          switch (rowIndex) {
            case 5:
              span = hidden;
              break;
          }
          break;
        case 12:
          switch (rowIndex) {
            case 5:
              span = [1, 2];
              break;
            case 6:
              span = [1, 2];
              break;
            case 7:
              span = [1, 2];
              break;
            case 8:
              span = [1, 2];
              break;
          }
          break;
        case 13:
          switch (rowIndex) {
            case 5:
              span = hidden;
              break;
            case 6:
              span = hidden;
              break;
            case 7:
              span = hidden;
              break;
            case 8:
              span = hidden;
              break;
          }
          break;
      }
      return span;
    }
  }
};
</script>

<style lang="less">
.state_summary_container {
  .header {
    width: 100%;
    color: rgb(255, 255, 255);
    height: 35px;
    background-color: #666;
    line-height: 35px;
    font-size: 1.3rem;
    padding-left: 2%;
    margin-top: 10px;
    .header_icon {
      color: #fff;
    }
  }
}
/* 解决合并行列case下，某些th的visibility为hidden的bug */
th.fix_hidden_bug {
  visibility: unset;
  div {
    visibility: unset !important;
  }
}
/* 直接复制了原代码 */
/* ------- KPI --------*/
.wChart-KPI-Icon {
  position: absolute;
  font-size: 76px;
  color: #fff;
  left: 10px;
  bottom: 6px;
}
.wChart-KPI-V {
  position: absolute;
  right: 5px;
  font-size: 35pt;
}
.wChart-KPI-Title {
  font-size: 15pt;
  color: #fff;
  margin-left: 5px;
  position: absolute;
  /*top:0px;*/
}
.wChart-KPI_Value {
  position: relative;
  font-size: 30px;
  color: #000;
  text-align: right;
}
.wChart-KPI_Unit {
  font-size: 10pt;
  color: #fff;
  text-align: right;
}
.wChart-KPI-Title {
  width: 100%;
  height: 35px;
  position: absolute;
  left: -5px;
  line-height: 35px;
  font-size: 12pt;
  color: #fff;
}
/* ------- 柱状图 Bar --------*/
.wChart-All-Div-Title {
  position: absolute;
  border-bottom: solid 1px #808080;
  height: 35px;
  line-height: 35px;
  top: 0px;
  left: 0px;
  width: 100%;
  font-family: "黑体";
  color: white;
  text-align: left;
  /*background-color:#f9f9f9;*/
}
.wChart-All-Title {
  padding-right: 3px;
  padding-left: 4px;
}
.wChart-All-DOM {
  height: 100%;
  width: 100%;
}
.wChart-Bar-DOM {
  height: 100%;
  width: 100%;
}
/* ------- 饼图 pie --------*/

.wChart-Pie-DOM {
  height: 100%;
  width: 100%;
  position: relative;
  background-color: #fff;
}
.wChart-Line-DOM {
  height: 100%;
  width: 100%;
}

/*变透明设置*/
.wChart-KPI-Transparent {
  filter: alpha(opacity=50);
  -moz-opacity: 0.5;
  -khtml-opacity: 0.5;
  opacity: 0.5;
}

/*页面布局*/
.wChart-kGrid {
  width: 100%;
  position: relative;
}
.wChart-kGrid2 {
  float: left;
  background-color: #ccc;
  position: relative;
}
.wChart-kGrid_2_Float {
  float: left;
  background-color: #ccc;
  position: relative;
}
.wChart-Grid {
  float: left;
  background-color: #fff;
  /*margin:5px;*/
  /*position: relative; border: solid 1px #808080;*/
  /*width:100%;*/
}

.wChart-Grid_H {
  /*float:left;*/
  background-color: #fff;
  /*margin:5px;*/
  /*position: relative; border: solid 1px #808080;*/
  /*width:100%;*/
}
.wChart-Chart {
  float: left;
  margin: 5px;
  border: solid 1px #808080;
  background-color: #fff;
  position: relative;
}
.wChart-Grid_SetWidth {
  background-color: #ccc;
}
.wChart-Div {
  /*margin:5px;*/
  background-color: #fff;
  position: relative;
  /*border: solid 1px #808080;*/
  /*margin-bottom:10px;*/
  /*margin-top:10px*/
}
.wChart-Div-Float {
  float: left;
  margin: 5px;
  background-color: #fff;
  position: relative;
  border: solid 1px #808080;
  margin-top: 10px;
}

.state_summary_container {
  background-color: #2f4554;
  .panel_water_total {
    .panel_row_water_total {
      width: 100%;
      height: 120px;
      .panel_row_left {
        height: 100%;
        width: 60%;
        position: relative;
        background-color: #44b6ae;
        float: left;
      }
      .panel_row_right {
        height: 100%;
        width: 40%;
        position: relative;
        background-color: #9d694c;
        float: right;
      }
    }
  }
  .flow_line_chart,
  .press_line_chart {
    height: 250px;
    background-color: #fff;
    // margin-top: 10px;
    position: relative;
  }
  .pump_state_container {
    .pump_table {
      margin-top: 10px;
      text-align: center;
      background-color: #00a948;
      width: 100%;
      height: 30px;
      td {
        line-height: 35px;
        border: solid 1px #000000;
      }
      .pump_label {
        width: 100px;
        background-color: #000;
        color: #00a948;
      }
    }
  }
  .detail_table_container {
    margin-top: 10px;
    .table_header_rows {
      th {
        background-color: #001d26;
        color: white;
        text-align: center;
        /* 覆盖elementui的默认12px */
      }
    }
  }
}
</style>


