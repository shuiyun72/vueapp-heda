import echarts from 'echarts'

//定义对象
export var Operation = {
  title: {
    text: "",
    padding: 10,
    textStyle: {
      color: '#00afa9'
    }
  },
  grid: {
    left: '10%',
    right: '10%',
    top: '20%',
    bottom: '10%'
  },
  tooltip: {
    trigger: "axis",
    textStyle: {
      fontSize: 14
    },
    formatter: function (params) { //序列化提示数据
      var returnValue = new Array();
      if (params && params.length > 0) {
        for (var i = 0; i < params.length; i++) {
          var DesValue = null;
          if (params[i].value) {
            DesValue = params[i].value.toString().indexOf(".") == -1 ? params[i].value : parseFloat(params[i].value).toFixed(4);
          } else {
            DesValue = params[i].value;
          }
          returnValue.push(params[i].seriesName + " " + params[i].name + ' : ' + DesValue + '<br/>');
          //returnValue.push(params[i].seriesName +  ' : ' + DesValue + '<br/>');
        }
      }
      return returnValue.join("");
    }
  },
  dataZoom: {
    show: true,
    start: 0
  },
  toolbox: {
    show: false,
    feature: {
      //dataZoom: { show: true, title: { dataZoom: '区域缩放', dataZoomReset: '区域缩放后退' } },
      mark: {
        show: false
      },
      magicType: {
        show: true,
        type: ['line', 'bar']
      },
      restore: {
        show: false
      },
      saveAsImage: {
        show: true
      }
    }
  },
  legend: {
    data: {},
    textStyle: {
      color: '#ff0000'
    },
    y: "top",
    padding: [40, 50, 50, 200],
    type: 'scroll',
    width: 900
  },
  xAxis: [{
    type: "",
    data: {},
    boundaryGap: false,
    axisLabel: {
      textStyle: {
        color: '#ff0000',
        fontSize: 16
      },
    },
    axisLine: {
      lineStyle: {
        color: '#0d56a7',
        width: 3,
        type: 'solid'
      }
    },
    splitLine: {
      show: true,
      lineStyle: {
        color: '#939393',
        width: 2,
        type: 'solid'
      }
    }
  }],
  yAxis: {},
  calculable: false,
  series: {}
};
/*
    根据数据进行绘制曲线
*/

export function BuilderChart(chartData, OBJName, titleShow = true, toolboxShow = false, datazoomshow = false, legendshow = true) {
  if (typeof chartData === 'string') {
    chartData = JSON.parse(chartData)
  }
  var myChart = echarts.init(document.getElementById(OBJName));
  var computedOption = Option(chartData, titleShow, toolboxShow, datazoomshow, legendshow)
  console.log('水质Op ', computedOption)
  myChart.setOption(computedOption, true);
  return myChart

}

export function Option(data, titleShow, toolboxShow, datazoomshow, legendshow) {
  console.log('进入到Option方法', data)
  var legendJson = data.legend; //标记
  /********计算标记距顶部高度********/
  var ytop = 60;
  var ybottom = 85;

  if (datazoomshow == false) {
    ybottom = 30;
  }
  var operationData = JSON.parse(JSON.stringify(Operation)); //声明对象
  operationData.title.text = data.title.text; //标题
  operationData.legend.data = data.legend.data; //图例赋值
  operationData.xAxis[0].type = data.xAxis.type; //定义横坐标图表类型
  operationData.xAxis[0].data = data.xAxis.data; //定义横坐标数据
  operationData.yAxis = data.yAxis; //纵坐标
  for (var i = 0; i < data.series.length; i++) {
    if (data.series[i].itemStyle == null)
      data.series[i].itemStyle = {};
  }
  operationData.series = [];
  operationData.series = data.series; //数据赋值
  operationData.grid.y = ytop; //定义图例高度
  operationData.toolbox.show = true;
  if (titleShow == false) {
    operationData.title.show = titleShow;
  }
  if (toolboxShow == false) {
    operationData.toolbox.show = toolboxShow;
  }
  if (datazoomshow == false) {
    operationData.dataZoom.show = datazoomshow;
  }
  if (legendshow == false) {
    operationData.legend.show = legendshow;
  }
  return operationData;
}
