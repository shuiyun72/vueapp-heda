import echarts from 'echarts'
import $ from 'jquery'
import _ from 'lodash'


function ChartBuilder() {
  this.EchartInfo = null; //定以Echart对象信息
  this.EchartControl = null; //定义EChart控件
  this.EchartDom = null; //定义图表显示Div
  this.ChartType = null; //EChart图表类型 1：折线 2：柱状图 3：饼图
  this.ChartSkin = null; //图标皮肤
};
ChartBuilder.prototype.DataZoomShow = false; //是否支持组件区域放大缩小
ChartBuilder.prototype.TitleShow = false; //是否显示标题
ChartBuilder.prototype.ToolboxShow = false; //是否显示工具栏
ChartBuilder.prototype.LegendShow = false; //是否显示图例
ChartBuilder.prototype.Tile = ""; //标题
ChartBuilder.prototype.ChartICON = "fa-pie-chart"; //图表头部图标
ChartBuilder.prototype.legendData = ['电压'];
ChartBuilder.prototype.xAxisData = ['1点', '2点', '3点', '4点', '5点', '6点', '7点', '8点', '9点', '10点', '11点', '12点', '13点', '14点', '15点', '16点', '17点', '18点', '19点', '20点', '21点', '22点', '23点', '24点'];
ChartBuilder.prototype.seriesData = [{
  data: [122, 50, 50, 40, 50, 80, 160, 190, 230, 180, 170, 175, 56, 78, 111, 23, 234, 12, 111, 23, 234, 12, 25, 78],
  type: 'line',
  smooth: true,
  areaStyle: {}
}];

//定义图表基本属性信息
ChartBuilder.prototype.Option = {}
//控件初始化
ChartBuilder.prototype.Init = function (EchartControl, InitOptionData, Title, SkinName, ChartType, ChartICON, TitleShow, ToolboxShow, DataZoomShow, LegendShow) {
  try {
    if (!EchartControl) {
      alert("未指定图表显示控件");
      return;
    }

    this.EchartControl = $(EchartControl); //给指定控件赋值
    this.Title = Title; //图表头
    this.ChartSkin = SkinName ? wChart_KPI_Skin(SkinName) : wChart_KPI_Skin("MeiHongKD");
    this.ChartType = ChartType ? ChartType : 1; //图表类型
    this.TitleShow = TitleShow ? TitleShow : false;
    this.ToolboxShow = ToolboxShow ? ToolboxShow : false;
    this.DataZoomShow = DataZoomShow ? DataZoomShow : false;
    this.LegendShow = LegendShow ? LegendShow : false;
    this.ChartICON = ChartICON ? ChartICON : "fa-pie-chart";

    //构造头部标题信息
    // var Ary = new Array();
    // Ary.push("<div style=\"background-color:");
    // Ary.push(this.ChartSkin[1]);
    // Ary.push("\" class=\"wChart-All-Div-Title\"><i class=\"fa ");
    // Ary.push(this.ChartICON);
    // Ary.push("\"");
    // Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    // Ary.push(this.Title);
    // Ary.push("</div>");
    // Ary.push("<div class=\"wChart-Line-DOM\" style=\"height:229px\"></div>");
    // this.EchartControl.html(Ary.join("")); //将头部元素及图表放入指定元素中去
    // this.EchartDom = this.EchartControl.children(".wChart-Line-DOM")[0]; //取得图表存放元素
    //初始化图表控件
    this.EchartInfo = echarts.init(EchartControl); //初始化图标控件
    //加载数据动画
    this.EchartInfo.showLoading({
      text: '正在加载数据...',
      effect: 'whirling'
    });
    this.Option = InitOptionData; //初始加载数据
    //指定类型控件
    switch (this.ChartType) {
      case 1: //折线图或柱状图
        this.TitleShow = this.TitleShow;
        this.ToolboxShow = this.ToolboxShow;
        this.DataZoomShow = this.DataZoomShow;
        this.LegendShow = this.LegendShow;
        this.ChartType = this.ChartType;
        this.ChartICON = this.ChartICON;
        this.LineBarChart();
        break;
      case 2: //绘制饼状图ss
        this.PieChart();
        break;
      case 3:

        break;
      case 4:
        this.ScatterChart();
        break;
      case 5:
        break;
    }
  } catch (err) {
    console.log(err);
  }
};

//绘制折线图表
ChartBuilder.prototype.LineBarChart = function () {
  try {
    //是否显示标题
    this.Option.title.show = this.TitleShow;
    this.Option.title.text = this.Title;
    //工具栏是否显示
    this.Option.toolbox.show = this.ToolboxShow;
    //是否支持组件区域放大缩小
    this.Option.dataZoom.show = this.DataZoomShow;
    //是否显示图例
    this.Option.legend.show = this.LegendShow;
    //判断选中标题是否选中
    if (this.legendData) {
      this.Option.legend.data = this.legendData;
    }
    if (this.xAxisData) {
      this.Option.xAxis[0].data = this.xAxisData;
    }
    //添加折线数据
    if (this.seriesData) {
      this.Option.series = this.seriesData;
    }
    console.log('==========================', this.Option)
    //Echart赋值并生成图表
    this.EchartInfo.setOption(this.Option);
    this.EchartInfo.hideLoading(); //隐藏加载动画
  } catch (err) {
    console.log(err);
    this.EchartInfo.hideLoading();
    this.EchartInfo.showLoading({
      text: '暂无数据'
    });
  }
};

//绘制饼状图
ChartBuilder.prototype.PieChart = function () {
  try {
    // this.Option.series[0].data = DataJson;
    //图表设参
    this.EchartInfo.setOption(this.Option);
    this.EchartInfo.hideLoading();
  } catch (err) {
    console.log(err);
    this.EchartInfo.hideLoading();
    this.EchartInfo.showLoading({
      text: '暂无数据'
    });
  }
};

//三点图绘制
ChartBuilder.prototype.ScatterChart = function () {
  try {
    //图表设参
    this.EchartInfo.setOption(this.Option);
    this.EchartInfo.hideLoading();
  } catch (err) {
    console.log(err);
    this.EchartInfo.hideLoading();
    this.EchartInfo.showLoading({
      text: '暂无数据'
    });
  }
};

//重新绘制图表
ChartBuilder.prototype.SetOption = function () {
  try {
    this.EchartInfo.setOption(this.Option, true);
  } catch (err) {
    console.log(err);
  }
}

//获取图表数据
ChartBuilder.prototype.GetChartData = function (LinkURL, callback) {
  $.ajax({
    type: "GET",
    url: LinkURL,
    cache: false,
    dataType: "json",
    success: function (data) {
      console.log('折线图数据', JSON.parse(data))
      let resErr = null
      let resData = null
      if (typeof data === 'string') {
        resData = JSON.parse(data)
        if (resData.ErrCode == 0) {
          resData = resData.Data
        } else {
          resErr = new Error(resData.ErrInfo)
          resData = {
            legend: {},
            xAxis: {},
            series: []
          }
        }
      }
      callback(resErr, resData)
    },
    error: function (err) {
      callback(err)
    }
  });
}

/**
 * 构造柱状图和折线图
 */
ChartBuilder.prototype.BuildLineChart = function (LinkURL, callback) {
  if (typeof LinkURL === 'object') {
    let ChartData = LinkURL
    this.Option.legend.data = ChartData.legend.data;
    if (ChartData.yAxis) {
      this.Option.yAxis = [];
      if (Array.isArray(ChartData.yAxis)) {
        $.each(ChartData.yAxis, function (index, row) {
          var newYXis = Object.assign({}, this.YAXIS, row);
          this.Option.yAxis.push(newYXis);
        }.bind(this));
      } else {
        var newYXis = Object.assign({}, this.YAXIS, ChartData.yAxis);
        this.Option.yAxis.push(newYXis);
      }
    }
    this.Option.xAxis[0].data = ChartData.xAxis.data;
    this.Option.series = ChartData.series;
    this.SetOption();
    if (callback) {
      callback();
    }

  } else if (typeof LinkURL === 'string') {
    $.ajax({
      type: "GET",
      url: LinkURL,
      dataType: "json",
      success: function (ChartData) {
        this.Option.legend.data = eval(ChartData.legend.data);
        if (ChartData.yAxis) {
          this.Option.yAxis = [];
          if (Array.isArray(ChartData.yAxis)) {
            $.each(eval(ChartData.yAxis), function (index, row) {
              var newYXis = Object.assign({}, this.YAXIS, row);
              this.Option.yAxis.push(newYXis);
            }.bind(this));
          } else {
            var newYXis = Object.assign({}, this.YAXIS, ChartData.yAxis);
            this.Option.yAxis.push(newYXis);
          }
        }
        this.Option.xAxis[0].data = eval(ChartData.xAxis.data);
        this.Option.series = eval(ChartData.series);
        this.SetOption();
        if (callback) {
          callback();
        }
      }.bind(this),
      error: function (d) {
        console.log(d);
      }
    });
  }
};

//图表皮肤样式
function wChart_KPI_Skin(_SkinName) {
  var Skin = new Array();
  switch (_SkinName) {
    case "TianLan":
      Skin[0] = "#44b6ae";
      Skin[1] = "#358f8b";
      Skin[2] = "#67c7c2";
      break;
    case "MeiHong":
      Skin[0] = "#e35b5d";
      Skin[1] = "#df3939";
      Skin[2] = "#ee9a9c"
      break;
    case "QianLan":
      Skin[0] = "#3598dc";
      Skin[1] = "#1f77b3";
      Skin[2] = "#67c7c2";
      break;
    case "QianZi":
      Skin[0] = "#8775a9";
      Skin[1] = "#6b5b8b";
      Skin[2] = "#d8c4fd";
      break;
    case "ZhongLan":
      Skin[0] = "#578ebd";
      Skin[1] = "#3f76a3";
      Skin[2] = "#99cffd";
      break;
    case "CaoLv":
      Skin[0] = "#80c535";
      Skin[1] = "#679e2a";
      Skin[2] = "#c8fa92";
      break;
    case "ShuiLan":
      Skin[0] = "#00b0f0";
      Skin[1] = "#0094c8";
      Skin[2] = "#96e0fb";
      break;
    case "ShenHong":
      Skin[0] = "#c00000";
      Skin[1] = "#960000";
      Skin[2] = "#fc9b9b";
      break;
    case "QianHuang":
      Skin[0] = "#ffc000";
      Skin[1] = "#d09e00";
      Skin[2] = "#fee597";
      break;
    case "DaZi":
      Skin[0] = "#7030a0";
      Skin[1] = "#532476";
      Skin[2] = "#d199fb";
      break;
    case "ShenLan":
      Skin[0] = "#002f8e";
      Skin[1] = "#002060";
      Skin[2] = "#95b7fb";
      break;
    case "QingLv":
      Skin[0] = "#00b050";
      Skin[1] = "#008e40";
      Skin[2] = "#95f8c2";
      break;
    default:
      Skin[0] = "#44b6ae";
      Skin[1] = "#358f8b";
      Skin[2] = "#67c7c2";
      break;
  }
  return Skin;
}

export {
  ChartBuilder
}
export default ChartBuilder
