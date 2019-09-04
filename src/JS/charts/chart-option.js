//绘制曲线或柱状图显示
export let lineORBarOption = {
    title: {
        text: "",
        padding: 10,
        textStyle: { color: '#ff0000' }
    },
    grid: {
        left: '15%',
        right: '5%',
    },
    tooltip: {
        trigger: "axis",
        textStyle: {
            fontSize: 14
        },
        formatter: function (params) {      //序列化提示数据
            var returnValue = new Array();
            if (params && params.length > 0) {
                for (var i = 0; i < params.length; i++) {
                    var DesValue = null;
                    if (params[i].value) {
                        DesValue = params[i].value.toString().indexOf(".") == -1 ? params[i].value : parseFloat(params[i].value).toFixed(4);
                    }
                    else {
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
            mark: { show: false },
            magicType: { show: true, type: ['line', 'bar'] },
            restore: { show: false },
            saveAsImage: { show: true }
        }
    },
    legend: {
        data: {},
        textStyle: { color: '#ff0000' },
        y: "top",
        padding: [60, 50, 50, 200],
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
        //axisLine: {
        //    lineStyle: {
        //        color: '#0d56a7',
        //        width: 3,
        //        type: 'solid'
        //    }
        //},
        splitLine: {
            show: true,
            lineStyle: {
                color: '#939393',
                width: 2,
                type: 'solid'
            }
        }
    }],
    yAxis: [
        {
            type: 'value'
        }
    ],
    calculable: false,
    series: {}
};

//饼状图显示
export let pieOption = {
    tooltip: {
        trigger: 'item',
        formatter: "{b} : {c} ({d}%)"
    },
    toolbox: {
        show: false,

    },
    calculable: false,
    series: [
        {
            type: 'pie',
            radius: '70%',
            center: ['50%', '60%'],
            data: [{ value: 89, name: '开机' }, { value: 11, name: '关机' }]
        }
    ]
};

//散点图显示
export let scatterOption = {
    xAxis: {
        scale: true
    },
    yAxis: {
        scale: true
    },
    series: [{
        type: 'scatter',
        data: [
            [161.2, 51.6], [167.5, 59.0], [159.5, 49.2], [157.0, 63.0]
        ]
    }]
};