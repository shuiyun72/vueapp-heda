import echarts from 'echarts'
// 导入第三方echarts液体图表插件
import liquidFillForEcharts from 'echarts-liquidfill'
import $ from 'jquery'

/*仪表盘 单压力图表表现形式1*/
export var NPOPtion = {
  tooltip: {
    formatter: "{a} <br/>{b} : {c}"
  },
  toolbox: {
    show: false,
    feature: {
      mark: {
        show: true
      },
      restore: {
        show: true
      },
      saveAsImage: {
        show: true
      }
    }
  },
  // polar:{

  // },
  series: [{
    name: '压力表',
    type: 'gauge',
    center: ['50%', '70%'], // 默认全局居中
    // startAngle: 140,
    // endAngle: -100,  
    radius: '100%',
    startAngle: 180,
    endAngle: 0,
    min: 0, // 最小值
    max: 1, // 最大值
    precision: 0, // 小数精度，默认为0，无小数点
    splitNumber: 10, // 分割段数，默认为5
    axisLine: { // 坐标轴线
      show: true, // 默认显示，属性show控制显示与否
      lineStyle: { // 属性lineStyle控制线条样式
        color: [
          [0.2, 'lightgreen'],
          [0.7, 'orange'],
          [1, 'red']
        ],
        width: 30
      }
    },
    axisTick: { // 坐标轴小标记
      show: true, // 属性show控制显示与否，默认不显示
      splitNumber: 5, // 每份split细分多少段
      length: 8, // 属性length控制线长
      lineStyle: { // 属性lineStyle控制线条样式
        color: '#eee',
        width: 1,
        type: 'solid'
      }
    },
    axisLabel: { // 坐标轴文本标签，详见axis.axisLabel
      show: true,
      formatter: function (v) {
        switch (v + '') {
          case '0.2':
            return '下限';
          case '0.7':
            return '上限';
          default:
            return '';
        }
      },
      textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
        color: '#333',
        fontSize: 15
      }
    },
    splitLine: { // 分隔线
      show: true, // 默认显示，属性show控制显示与否
      length: 30, // 属性length控制线长
      lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
        color: '#eee',
        width: 2,
        type: 'solid'
      }
    },
    pointer: {
      length: '80%',
      width: 8,
      color: 'auto'
    },
    title: {
      show: false,
      offsetCenter: ['-65%', -10], // x, y，单位px
      textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
        color: '#333',
        fontSize: 15
      }
    },
    detail: {
      show: true,
      backgroundColor: 'rgba(0,0,0,0)',
      borderWidth: 0,
      borderColor: '#ccc',
      width: 100,
      height: 40,
      offsetCenter: ['0%', '30%'], // x, y，单位px
      //formatter: '{value}mpa',
      formatter: '{value} mpa',
      textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
        color: 'auto',
        fontSize: 25
      }
    },
    data: [{
      value: 0.1,
      name: '压力表'
    }]
  }]
};

/*仪表盘：正瞬时、负瞬时、压力*/
export var NFOption = {
  tooltip: {
    formatter: "{a} <br/>{c} {b}"
  },
  toolbox: {
    show: false,
    feature: {
      mark: {
        show: true
      },
      restore: {
        show: true
      },
      saveAsImage: {
        show: true
      }
    }
  },
  series: [{
      name: '正瞬时流量',
      type: 'gauge',
      z: 3,
      min: 0,
      max: 1000,
      splitNumber: 10,
      axisLine: { // 坐标轴线
        lineStyle: { // 属性lineStyle控制线条样式
          width: 10
        }
      },
      axisTick: { // 坐标轴小标记
        length: 12, // 属性length控制线长
        lineStyle: { // 属性lineStyle控制线条样式
          color: 'auto'
        }
      },
      splitLine: { // 分隔线
        length: 15, // 属性length控制线长
        lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
          color: 'auto'
        }
      },
      title: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 16,
          fontStyle: 'italic',
        },
        offsetCenter: [0, '110%']
      },
      pointer: {
        length: '60%',
        width: 5
      },
      detail: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 25,
        },
        offsetCenter: [0, '80%']
      },
      data: [{
        value: 40,
        name: '正瞬M(3)/h'
      }]
    },
    {
      name: '负瞬时流量',
      type: 'gauge',
      center: ['20%', '55%'], // 默认全局居中
      radius: '60%',
      min: 0,
      max: 500,
      endAngle: 45,
      splitNumber: 5,
      axisLine: { // 坐标轴线
        lineStyle: { // 属性lineStyle控制线条样式
          width: 8
        }
      },
      axisTick: { // 坐标轴小标记
        length: 10, // 属性length控制线长
        lineStyle: { // 属性lineStyle控制线条样式
          color: 'auto'
        }
      },
      splitLine: { // 分隔线
        length: 12, // 属性length控制线长
        lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
          color: 'auto'
        }
      },
      title: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 16,
          fontStyle: 'italic',
        },
        offsetCenter: [0, '110%']
      },
      pointer: {
        length: '60%',
        width: 5
      },
      detail: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 20,
        },
        offsetCenter: [0, '70%']
      },
      data: [{
        value: 1.5,
        name: '负瞬M(3)/h'
      }]
    },
    {
      name: '压力',
      type: 'gauge',
      center: ['80%', '55%'], // 默认全局居中
      radius: '60%',
      min: 0,
      max: 1,
      startAngle: 135,
      endAngle: -40,
      splitNumber: 5,
      axisLine: { // 坐标轴线
        lineStyle: { // 属性lineStyle控制线条样式
          width: 8
        }
      },
      axisTick: { // 坐标轴小标记
        length: 10, // 属性length控制线长
        lineStyle: { // 属性lineStyle控制线条样式
          color: 'auto'
        }
      },
      splitLine: { // 分隔线
        length: 12, // 属性length控制线长
        lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
          color: 'auto'
        }
      },
      title: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 16,
          fontStyle: 'italic',
        },
        offsetCenter: [0, '110%']
      },
      pointer: {
        length: '60%',
        width: 5
      },
      detail: {
        textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
          fontWeight: 'bolder',
          fontSize: 20,
        },
        offsetCenter: [0, '70%']
      },
      data: [{
        value: 1.5,
        name: '压力mpa'
      }]
    }
  ]
};



export var QOption = {
  series: [{
    type: 'liquidFill',
    // type: 'gauge', // modified by jerry
    name: 'Liquid Fill',
    radius: '85%',
    data: [0.6, 0.5, 0.4, 0.3],
    label: {
      normal: {
        formatter: '{c} NTU',
        textStyle: {
          color: 'red',
          fontSize: '13',
          insideColor: 'yellow',
        }
      }
    }
  }]
};


/*
    单压力仪表盘
*/
export function BuilderGaugeChart(OBJName, IValue, DName) {
  //EChart显示区域宽度
  var myChart = echarts.init(document.getElementById(OBJName));
  NPOPtion.series[0].data[0].value = IValue;
  if (DName) {
    NPOPtion.series[0].data[0].name = DName;
  }
  myChart.setOption(NPOPtion, true);
  return myChart
}

/*
正瞬时、负瞬时、压力仪表盘数据
*/
export function BuilderNFGaugeChart(OBJName, IValue, IValue1, Ivalue2) {
  //EChart显示区域宽度
  var myChart = echarts.init(document.getElementById(OBJName));
  NFOption.series[0].data[0].value = IValue; //正瞬时流量
  NFOption.series[1].data[0].value = IValue1; //负瞬时流量
  NFOption.series[2].data[0].value = Ivalue2; //压力
  console.log('====', NFOption)
  myChart.setOption(NFOption, true);
}

/*水质图表展示*/
export function BuilderQGaugeChart(OBJName, IValue) {
  //EChart显示区域宽度
  var myChart = echarts.init(document.getElementById(OBJName));
  QOption.series[0].data[0] = IValue;
  // 这是不太好的写法  解决运行总览电导率单位的问题
  if (OBJName === 'SZ3') {
    QOption.series[0].label.normal.formatter = '{c} S/m'
  }
  myChart.setOption(QOption, true);
  return myChart
}
