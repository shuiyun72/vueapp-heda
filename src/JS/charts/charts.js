import echarts from 'echarts'
import $ from 'jquery'
import _ from 'lodash'

// KPI
//方块-KPI 
//1、标题；2、单位；3、操作对象DOM；4、图表样式（_ChartType：Title-Top；Tiele-Bottom）；
//5、图标；6、皮肤颜色
//==操作例子==
//  var KPI_1 = new wChart_KPI("当日进厂水量", "立方米 M<sup>3</sup>", document.getElementById("Kpi_Chart_1"), "Title-Top2", "fa-area-chart", "TianLan");
//  KPI_1.Init("1222");
export function wChart_KPI(_Title, _KPI_Unit, _DomObj, _ChartType, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_KPI;
    this.DomObj = $(_DomObj);
    this.ChartType = _ChartType;
    this.KPI_Icon = _KPI_Icon;
    this.Title = _Title;
    this.KPI_Unit = _KPI_Unit;

    this.KPI_Value = "00000.00";
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);

    //初始化对象
    this.self.prototype.Init = function (_KPI_Value) {

        //alert(_KPI_Value);
        if (_KPI_Value == null) {
            this.KPI_Value = "----";
        } else {
            this.KPI_Value = _KPI_Value;
        }

        var H = 0;
        var H2 = 0;
        var T = "";
        var V = "";
        var F = true;
        var Ary = new Array();
        if (this.ChartType == "Title-Top" || this.ChartType == "" || this.ChartType == null) {
            T = ";top:0px";
            V = "style='top:35px'";
            this.DomObj.children(".wChart-KPI-Title").css({
                top: "0px"

            });
            Ary.push("<div style=\"position:absolute;bottom:1px ;height: 6px;width:100%;;background-color:");
            Ary.push(this.KPI_Skin[1]);
            Ary.push("\">&nbsp; </div><div class='wChart-KPI-Title' style=';background-color:");
            this.DomObj.css({
                "background-color": this.KPI_Skin[0],
                "font-family": '黑体'
            });
        } else {
            T = ";bottom:0px";
            V = "style='top:0px'";
            Ary.push("<div style=\"position:absolute;top:0px ;height: 6px;width:100%;;background-color:");
            Ary.push(this.KPI_Skin[1]);
            Ary.push("\">&nbsp; </div><div class='wChart-KPI-Title' style=';background-color:");
            this.DomObj.css({
                "background-color": this.KPI_Skin[0],
                "font-family": '黑体'
            });
            this.DomObj.children(".wChart-KPI-Title").css({
                bottom: "0px"
            });
        }

        Ary.push(this.KPI_Skin[1]);
        Ary.push(T);
        Ary.push("'><span style='margin-left:10px'>");
        Ary.push(this.Title);
        Ary.push("</span></div>");

        Ary.push("<div style='khtml-opacity: 0.6; opacity: 0.6;' class='wChart-KPI-Icon wChart-KPI-Transparent' ");

        Ary.push(V);
        Ary.push(" ><i class='fa ");
        Ary.push(this.KPI_Icon);
        Ary.push(" wChart-KPI-Transparent'></i></div>");

        Ary.push("<div class='wChart-KPI-V' ");
        Ary.push(V);
        Ary.push("><div class='wChart-KPI_Value' >");
        Ary.push(this.KPI_Value);
        Ary.push("</div><div class='wChart-KPI_Unit' >");
        Ary.push(this.KPI_Unit);
        Ary.push("</div></div>");
        this.DomObj.html(Ary.join(""));
        if (this.ChartType == "Title-Top" || this.ChartType == "" || this.ChartType == null) {
            H = (this.DomObj.height() - $('.wChart-KPI-V').outerHeight()) / 2;
            H2 = ($(this.DomObj).height() - 35) + "px";
        } else {
            H = (this.DomObj.height() - 35 - $('.wChart-KPI-V').outerHeight()) / 2;
            H2 = ($(this.DomObj).height() - 35) + "px";
        }
        this.DomObj.children(".wChart-KPI-V").css({
            top: H
        });

        this.DomObj.children('.wChart-KPI-Icon').css({
            'line-height': H2
        });
    };

    this.self.prototype.SetValue = function (_KPI_Value) {
        alert(_KPI_Value);
        this.KPI_Value = _KPI_Value;
        this.DomObj.find(".wChart-KPI_Value").eq(0).html(_KPI_Value);
    }
    this.self.prototype.GetDomObj = function () {
        return this.DomObj;
    }
}

//统计-KPI 
//1、标题；2、单位；3、操作对象DOM；4、最大值[值,时间]；
//5、最大小值[值,时间]；6、平均值[值,时间]；7、图标；8、皮肤颜色
//==操作例子==
//  var TJ_1 = new wChart_KPI_TongJi("当日进厂水量", "立方米 M<sup>3</sup>",
//            document.getElementById("TongJi_Chart_1"),
//            ["234234", "2012-08-09 00:00:00"], ["123", "2012-08-09 00:00:00"], ["12312", "2012-08-09 00:00:00"]
//            , "fa-area-chart", "TianLan");
export function wChart_KPI_TongJi(_Title, _Unit, _DomObj, MaxAry, MinAry, AvgAry, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_KPI_TongJi;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);

    var Ary = new Array();

    Ary.push("<table style=\"width: 100%;border-collapse:collapse;text-align:center;font-family:'黑体';color:#FFF\">");
    Ary.push("<tr style=\"height:38px;background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push(" \">");
    Ary.push("<td style=\"width:60px;\">项目</td>");
    Ary.push("<td>数值</td><td>时间</td></tr>");

    Ary.push("<tr style=\"height:38px;background-color:");
    Ary.push(this.KPI_Skin[2]);
    Ary.push("\"><td>最大</td><td style=\"color:#000;\">");
    Ary.push(MaxAry[0]);
    Ary.push("</td><td style=\"color:#FFF;font-size:9pt;\">");
    Ary.push(MaxAry[1]);
    Ary.push("</td></tr>");
    Ary.push("<tr style=\"height:38px;background-color:");
    Ary.push(this.KPI_Skin[0]);
    Ary.push("\"><td>最小</td><td style=\"color:#000;\">");
    Ary.push(MinAry[0]);
    Ary.push("</td><td style=\"color:#FFF;font-size:9pt;\">");
    Ary.push(MinAry[1]);
    Ary.push("</td></tr><tr  style=\"height:38px;background-color:");
    Ary.push(this.KPI_Skin[2]);
    Ary.push("\"><td>平均</td><td style=\"color:#000;\">");
    Ary.push(AvgAry[0]);
    Ary.push("</td><td style=\"color:#FFF;font-size:9pt;\">");
    Ary.push(AvgAry[1]);
    Ary.push("</td></tr></table>");


    Ary.push("<table style=\"border-collapse:collapse;height:65px;width:100%;position:relative;color:#FFF;background-color:");
    Ary.push(this.KPI_Skin[0]);
    Ary.push(";font-family:'黑体'\">");
    Ary.push("<tr><td rowspan=\"2\" style=\"width:60px;color:#FFF;position:relative\">");
    Ary.push("<div style='position:absolute; bottom:2px;left:5px;color:#FFF;font-size:30pt;' class='wChart-KPI-Transparent';>");
    Ary.push("<i class='fa ");
    Ary.push(_KPI_Icon);
    Ary.push(" ' ></i></div></td>");
    Ary.push("<td style=\"text-align:right;color:#000\">单位:");
    Ary.push(_Unit);
    Ary.push("&nbsp;</td></tr><tr>");
    Ary.push("<td style=\"height:35px;background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push(";text-align:center;font-size:15pt\">");
    Ary.push(_Title);
    Ary.push("</td></tr></table>");

    $(_DomObj).html(Ary.join(""));
}

//仪表盘01
//1、标题；2、操作对象DOM；3、图标；4、皮肤颜色；5、是否显示百分号0:不显示1：显示，不传：显示
//==操作例子==
// var Dashboard_1 = new wChart_Dashboard_01("当日进厂水量比例", document.getElementById("Dashboard_Chart_1"), "fa-pie-chart", "");
// Dashboard_1.SetData(
//        [{ value: 15, name: '产销差' }]
//    );
export function wChart_Dashboard_01(_Title, _DomObj, _KPI_Icon, _KPI_Skin, _IsShowPercent) {
    this.self = wChart_Dashboard_01;

    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var fomat = "{a} <br/>{b} : {c}%";
    if (_IsShowPercent != undefined) {
        if (_IsShowPercent == 0) {
            fomat = "{a} <br/>{b} : {c}";
        }
    }

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
        tooltip: {
            formatter: fomat
        },
        series: [{
            type: 'gauge',
            min: 0, // 最小值
            max: 100,
            startAngle: 140,
            endAngle: -140,
            center: ['50%', '56%'], // 默认全局居中
            detail: {
                formatter: '{value}%'
            }, //仪表盘显示数据
            axisLine: { //仪表盘轴线样式
                lineStyle: {
                    width: 30
                }
            },
            splitLine: { //分割线样式
                length: 30
            },
            data: [{
                value: 50,
                name: '完成率'
            }],
            title: {
                show: true,
                offsetCenter: ['-65%', 8], // x, y，单位px
                textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                    fontWeight: 'bolder',
                    fontSize: "15pt"
                }
            },
            detail: {
                show: true,
                backgroundColor: 'rgba(0,0,0,0)',
                borderWidth: 0,
                borderColor: '#ccc',
                width: 100,
                height: 40,
                offsetCenter: ['-65%', -15], // x, y，单位px
                formatter: '{value}',
                textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                    color: '#000',
                    fontSize: 30
                }
            }
        }]
    };


    //构造DOM
    var Ary = new Array();
    Ary.push("<div style='background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push("' class=\"wChart-All-Div-Title\"><i class='fa ");
    Ary.push(this.KPI_Icon);
    Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    Ary.push(this.Title);
    Ary.push("</div>");
    Ary.push("<div class=\"wChart-All-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));

    this.ChartDom = this.DomObj.children(".wChart-All-DOM")[0];
    // 初始化图表
    this.wChart_Dashboard_01 = echarts.init(this.ChartDom);

    //初始化对象
    // Series_PieData  数据:[{ value: 335, name: '一水厂' },{ value: 310, name: '二水厂' }, { value: 234, name: '三水厂' }]
    this.self.prototype.SetData = function (Series_PieData) {
        this.Option.series[0].data = Series_PieData;

        //图表设参
        this.wChart_Dashboard_01.setOption(this.Option);
    }

    // Series_PieData  数据:[{ value: 335, name: '一水厂' },{ value: 310, name: '二水厂' }, { value: 234, name: '三水厂' }]
    this.self.prototype.SetData2 = function (Series_PieData, ValueFormatter, DetailFontSize, TitleFontSize, MinV, MaxV) {
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].data = Series_PieData;
        }

        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].detail.formatter = ValueFormatter;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].detail.textStyle.fontSize = DetailFontSize;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].title.textStyle.fontSize = TitleFontSize;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].max = MaxV;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].min = MinV;
        }
        //图表设参
        this.wChart_Dashboard_01.setOption(this.Option);
    }

}

export function wChart_Value_IsNull(Val) {
    if (Val == null || Val == "") {
        return false;
    } else {
        return true;
    }
}

//仪表盘02
//1、标题；2、操作对象DOM；3、图标；4、皮肤颜色；5、是否显示百分号0:不显示1：显示，不传：显示
//==操作例子==
// var Dashboard_2 = new wChart_Dashboard_02("当日进厂水量比例", document.getElementById("Dashboard_Chart_2"), "fa-pie-chart", "");
// Dashboard_2.SetData(
//        [{ value: 1522222222, name: '产销差' }], '{value}', 20, 20
//    );
export function wChart_Dashboard_02(_Title, _DomObj, _KPI_Icon, _KPI_Skin, _IsShowPercent) {
    this.self = wChart_Dashboard_02;

    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var fomat = "{a} <br/>{b} : {c}%";
    if (_IsShowPercent != undefined) {
        if (_IsShowPercent == 0) {
            fomat = "{a} <br/>{b} : {c}";
        }
    }

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
        tooltip: {
            formatter: fomat
        },
        series: [{
            type: 'gauge',
            min: 0, // 最小值
            max: 80,
            //startAngle: 140,
            //endAngle: -140,
            center: ['50%', '62%'], // 默认全局居中
            detail: {
                formatter: '{value}%'
            }, //仪表盘显示数据
            axisLine: { //仪表盘轴线样式
                lineStyle: {
                    width: 30
                }
            },
            splitLine: { //分割线样式
                length: 30
            },
            data: [{
                value: 50,
                name: '完成率'
            }],
            title: {
                show: true,
                offsetCenter: [0, '-38%'], // x, y，单位px
                textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                    fontWeight: 'bolder',
                    fontSize: 20
                }
            },
            detail: {
                show: true,
                //backgroundColor: 'rgba(0,0,0,0)',
                //borderWidth: 0,
                //borderColor: '#ccc',
                //width: 100,
                //height: 40,
                //offsetCenter: ['-65%', 15],       // x, y，单位px
                formatter: '{value}兆帕',
                textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                    color: '#000000',
                    fontSize: 20
                }
            }
        }]
    };


    //构造DOM
    var Ary = new Array();


    Ary.push("<div style='background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push("' class=\"wChart-All-Div-Title\"><i class='fa ");
    Ary.push(this.KPI_Icon);
    Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    Ary.push(this.Title);
    Ary.push("</div>");
    Ary.push("<div class=\"wChart-All-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));

    this.ChartDom = this.DomObj.children(".wChart-All-DOM")[0];
    // 初始化图表
    this.wChart_Dashboard_02 = echarts.init(this.ChartDom);

    //初始化对象
    // Series_PieData  数据:[{ value: 335, name: '一水厂' },{ value: 310, name: '二水厂' }, { value: 234, name: '三水厂' }]
    this.self.prototype.SetData = function (Series_PieData, ValueFormatter, DetailFontSize, TitleFontSize) {
        this.Option.series[0].data = Series_PieData;
        this.Option.series[0].detail.formatter = ValueFormatter;
        this.Option.series[0].detail.textStyle.fontSize = DetailFontSize;
        this.Option.series[0].title.textStyle.fontSize = TitleFontSize;
        //图表设参
        this.wChart_Dashboard_02.setOption(this.Option);
    }
    this.self.prototype.SetData2 = function (Series_PieData, ValueFormatter, DetailFontSize, TitleFontSize, MinV, MaxV) {
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].data = Series_PieData;
        }

        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].detail.formatter = ValueFormatter;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].detail.textStyle.fontSize = DetailFontSize;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].title.textStyle.fontSize = TitleFontSize;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].max = MaxV;
        }
        if (wChart_Value_IsNull(Series_PieData)) {
            this.Option.series[0].min = MinV;
        }
        //图表设参
        this.wChart_Dashboard_02.setOption(this.Option);
    }
}

// KPI-柱状图
//==操作例子==
//wChart Bar 图表
//1、标题；2、操作对象DOM；3、图标；4、皮肤颜色
//  var Bar_1 = new wChart_Bar("当日进厂水量", document.getElementById("Bar_Chart_1"), "fa-area-chart", "QianHuang");
//  Legend_Data 图例数据：格式['进厂水量'，'XXX']
//  Series_Data  数据【本函数为一维】:[2.0, 4.9, 7.0, 23.2, 55.6, 76.7]
//  xAxis_Data  横轴坐标 轴数据数据：['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
//  Bar_1.SetData(
//        ["小时进厂水量"],
//        [2.0, 4.9, 7.0, 23.2, 55.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3],
//        wChart_KPI_xAxis("M","2016","10")
export function wChart_Bar(_Title, _DomObj, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_Bar;

    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            x: 'right',
            y: '6px',
            data: ['进厂水量'] // 图例
        },
        grid: {
            y: '60',
            x: '40',
            y2: '30',
            x2: '15'
        },
        toolbox: {
            show: false,
        },
        calculable: false,
        xAxis: [{
            type: 'category',
            axisLine: {
                lineStyle: {
                    color: '00b0f0',
                    width: 3,
                }
            },
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'] // 横轴坐标 轴数据
        }],
        yAxis: [{
            type: 'value',
            axisLine: {
                lineStyle: {
                    color: '00b0f0',
                    width: 3
                }
            }
        }],
        series: [{
            name: '小时进厂水量',
            type: 'bar',
            data: [2.0, 4.9, 7.0, 23.2, 55.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3],
            markPoint: {
                data: [{
                        type: 'max',
                        name: '最大值',
                        itemStyle: {
                            normal: {
                                color: '#EEE555',
                                label: {
                                    textStyle: {
                                        color: 'red'
                                    }
                                }
                            }
                        }
                    },
                    {
                        type: 'min',
                        name: '最小值',
                        itemStyle: {
                            normal: {
                                color: '#EEE555',
                                label: {
                                    textStyle: {
                                        color: 'red'
                                    }
                                }
                            }
                        }
                    }
                ]
            }
            //,markLine: {
            //    data: [
            //        { type: 'average', name: '平均值' }
            //    ]
            //}
            ,
            itemStyle: {
                normal: {
                    color: function (value) {
                        return SkinColor;
                    }
                }
            }
        }]
    };

    //构造DOM
    var Ary = new Array();
    Ary.push("<div style='background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push("' class=\"wChart-All-Div-Title\"><i class='fa ");
    Ary.push(this.KPI_Icon);
    Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    Ary.push(this.Title);
    Ary.push("</div>");
    Ary.push("<div class=\"wChart-Bar-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));

    this.ChartDom = this.DomObj.children(".wChart-Bar-DOM")[0];
    // 初始化图表
    this.KPI_Bar = echarts.init(this.ChartDom, "blue");



    //初始化对象
    // Legend_Data 图例数据：格式['进厂水量'，'XXX']
    // Series_Data  数据【本函数为一维】:[2.0, 4.9, 7.0, 23.2, 55.6, 76.7]
    // xAxis_Data  横轴坐标 轴数据数据：['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
    this.self.prototype.SetData = function (Legend_Data, Series_Data, xAxis_Data) {

        this.Option.legend.data = Legend_Data;
        this.Option.xAxis[0].data = xAxis_Data;
        this.Option.series[0].name = this.Option.legend.data[0];

        this.Option.series[0].data = Series_Data;
        this.Option.grid.x

        //图表设参
        this.KPI_Bar.setOption(this.Option);
    }



}

// KPI-饼图
//==操作例子==
//  var Pie_1 = new wChart_Pie("当日进厂水量比例", document.getElementById("Bar_Chart_2"), "fa-pie-chart", "");
//  Pie_1.SetData(
//         [{ value: 335, name: '一水厂' }, { value: 310, name: '二水厂' }, { value: 234, name: '三水厂' }]
//      );
export function wChart_Pie(_Title, _DomObj, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_Pie;

    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
        tooltip: {
            trigger: 'item',
            formatter: "{b} : {c} ({d}%)"
        },
        toolbox: {
            show: false,

        },
        calculable: false,
        series: [{
            type: 'pie',
            radius: '55%',
            center: ['50%', '60%'],
            data: []
        }]
    };
    //alert($(this.ChartDom).parent().width());
    //构造DOM
    var Ary = new Array();
    Ary.push("<div style='background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push(";z-index: 999;' class=\"wChart-All-Div-Title\" ><i class='fa ");
    Ary.push(this.KPI_Icon);
    Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    Ary.push(this.Title);
    Ary.push("</div>");
    Ary.push("<div class=\"wChart-Pie-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));

    //this.ChartDom = this.DomObj.children(".wChart-Pie-DOM")[0];

    //$(this.ChartDom).width($(this.ChartDom).parent().width());
    // 初始化图表
    this.KPI_Pie = echarts.init(this.DomObj.children(".wChart-Pie-DOM")[0]);



    //初始化对象
    // Series_PieData  数据:[{ value: 335, name: '一水厂' },{ value: 310, name: '二水厂' }, { value: 234, name: '三水厂' }]
    this.self.prototype.SetData = function (Series_PieData) {
        this.Option.series[0].data = Series_PieData;

        //图表设参
        this.KPI_Pie.setOption(this.Option);
    }

}

// KPI-曲线图
//==操作例子==
// var Line_1 = new wChart_Line("当日进厂水量", document.getElementById("Line_Chart_1"), "fa-line-chart", "");
// Line_1.SetData(
//         ["小时进厂水量"],
//         [[2.0, 114.9, 7.0, 23.2, 55.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]],
//         wChart_KPI_xAxis("M", "2016", "10")
//     );
export function wChart_Line(_Title, _DomObj, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_Line;

    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            x: 'right',
            y: '6px',
            data: ['本日进水量']
        },
        grid: {
            //x: 50,
            //y: 60,
            //x2: 60,
            //y2: 85
            y: '60',
            x: '40',
            y2: '30',
            x2: '15'
        },
        //dataZoom: {
        //    show: true,
        //    start: 0
        //},
        calculable: true,
        xAxis: [{
            type: 'category',
            boundaryGap: false,
            axisLine: {
                lineStyle: {
                    color: '00b0f0',
                    width: 2,
                }
            },
            data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
        }],
        yAxis: [{
            type: 'value',
            axisLine: {
                lineStyle: {
                    color: '00b0f0',
                    width: 2,
                }
            },
            axisLabel: {

                formatter: function (value) {
                    return parseFloat(value).toFixed(0)
                }
            }
        }],
        series: [

        ]
    };




    //构造DOM
    var Ary = new Array();
    Ary.push("<div style='background-color:");
    Ary.push(this.KPI_Skin[1]);
    Ary.push("' class=\"wChart-All-Div-Title\"><i class='fa ");
    Ary.push(this.KPI_Icon);
    Ary.push(" fa-lg wChart-KPI-Transparent wChart-All-Title'></i>");
    Ary.push(this.Title);
    Ary.push("</div>");
    Ary.push("<div class=\"wChart-Line-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));

    this.ChartDom = this.DomObj.children(".wChart-Line-DOM")[0];
    // 初始化图表
    this.KPI_Line = echarts.init(this.ChartDom);



    //初始化对象
    // Legend_Data 图例数据：格式['进厂水量'，'XXX']
    // Series_Data  数据【本函数为一维】:[2.0, 4.9, 7.0, 23.2, 55.6, 76.7]
    // xAxis_Data  横轴坐标 轴数据数据：['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
    this.self.prototype.SetData = function (Legend_Data, Series_Data, xAxis_Data) {
        var IColor = ["#0099cc", "#44b6ae", "#99d2dd", "#88b0bb", "#1c7099", "#bfe3f4"];
        var _Item = function () {
            return {
                name: '本日进水量',
                type: 'line',
                smooth: true,
                data: []
                    //,markPoint: {
                    //    data: [
                    //        {
                    //          type: 'max', name: '最大值', itemStyle: { normal: { label: { textStyle: { color: 'black' } } } }
                    //        },
                    //        { type: 'min', name: '最小值', itemStyle: { normal: { label: { textStyle: { color: 'black' } } } } }
                    //    ]
                    //}
                    ,
                itemStyle: {
                    normal: {
                        areaStyle: {
                            type: 'default'
                        },
                        color: IColor[i]
                    }
                }
            }
        }

        var SeriesAry = new Array();
        // 处理数据
        for (i = 0; i < Series_Data.length; i++) {
            var Item = new _Item();
            Item.name = Legend_Data[i];
            Item.data = Series_Data[i];
            SeriesAry.push(Item);

        }

        //数据赋值
        this.Option.legend.data = Legend_Data;
        this.Option.xAxis[0].data = xAxis_Data;
        //console.log("SeriesAry", SeriesAry);
        this.Option.series = SeriesAry;
        //图表设参
        this.KPI_Line.setOption(this.Option);
    }



}
// KPI-三参数仪表盘
//==操作例子==
// var Line_1 = new wChart_Line("当日进厂水量", document.getElementById("Line_Chart_1"), "fa-line-chart", "");
// Line_1.SetData(
//         ["小时进厂水量"],
//         [[2.0, 114.9, 7.0, 23.2, 55.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]],
//         wChart_KPI_xAxis("M", "2016", "10")
//     );
export function wChart_Dashboard_Three(_Title, _DomObj, _KPI_Icon, _KPI_Skin) {
    this.self = wChart_Dashboard_Three;
    //初始化参数
    this.DomObj = $(_DomObj);
    this.KPI_Icon = _KPI_Icon;
    this.KPI_Skin = wChart_KPI_Skin(_KPI_Skin);
    this.Title = _Title;

    var SkinColor = this.KPI_Skin[0];
    //alert(this.KPI_Skin[0]);
    //图表参数设置
    this.Option = {
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
                name: '1',
                type: 'gauge',
                z: 3,
                min: 0,
                max: 3,
                splitNumber: 5,
                axisLine: { // 坐标轴线
                    lineStyle: { // 属性lineStyle控制线条样式
                        width: 10
                    }
                },
                axisTick: { // 坐标轴小标记
                    length: 15, // 属性length控制线长
                    lineStyle: { // 属性lineStyle控制线条样式
                        color: 'auto'
                    }
                },
                splitLine: { // 分隔线
                    length: 20, // 属性length控制线长
                    lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
                        color: 'auto'
                    }
                },
                title: {
                    textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                        fontWeight: 'bolder',
                        fontSize: 15
                    }
                },
                detail: {
                    textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                        fontWeight: 'bolder'
                    }
                },
                data: [{
                    value: 30,
                    name: '正瞬M(3)/h'
                }]
            },
            {
                name: '2',
                type: 'gauge',
                center: ['18%', '55%'], // 默认全局居中
                radius: '50%',
                min: 0,
                max: 20,
                endAngle: 45,
                splitNumber: 2,
                axisLine: { // 坐标轴线
                    lineStyle: { // 属性lineStyle控制线条样式
                        width: 8
                    }
                },
                axisTick: { // 坐标轴小标记
                    length: 12, // 属性length控制线长
                    lineStyle: { // 属性lineStyle控制线条样式
                        color: 'auto'
                    }
                },
                splitLine: { // 分隔线
                    length: 20, // 属性length控制线长
                    lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
                        color: 'auto'
                    }
                },
                pointer: {
                    width: 5
                },
                title: {
                    offsetCenter: [0, '85%'], // x, y，单位px
                },
                detail: {
                    textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                        fontWeight: 'bolder'
                    }
                },
                data: [{
                    value: 1.5,
                    name: '负瞬M(3)/h'
                }]
            },
            {
                name: '3',
                type: 'gauge',
                center: ['82%', '55%'], // 默认全局居中
                radius: '50%',
                min: 0,
                max: 50,
                startAngle: 135,
                endAngle: -40,
                splitNumber: 2,
                axisLine: { // 坐标轴线
                    lineStyle: { // 属性lineStyle控制线条样式
                        width: 8
                    }
                },
                axisTick: { // 坐标轴小标记
                    length: 12, // 属性length控制线长
                    lineStyle: { // 属性lineStyle控制线条样式
                        color: 'auto'
                    }
                },
                splitLine: { // 分隔线
                    length: 20, // 属性length控制线长
                    lineStyle: { // 属性lineStyle（详见lineStyle）控制线条样式
                        color: 'auto'
                    }
                },
                pointer: {
                    width: 5
                },
                title: {
                    offsetCenter: [0, '85%'], // x, y，单位px
                },
                detail: {
                    textStyle: { // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                        fontWeight: 'bolder'
                    }
                },
                data: [{
                    value: 1.5,
                    name: '压力mpa'
                }]
            }
        ]
    };

    var Ary = new Array();
    Ary.push("<div class=\"wChart-Pie-DOM\" ></div>");
    this.DomObj.html(Ary.join(""));
    // 初始化图表
    this.KPI_Pie = echarts.init(this.DomObj.children(".wChart-Pie-DOM")[0]);

    //初始化对象
    // 标题 Series_Name['瞬时M(3)/h','负瞬M(3)/h','压力Mpa']
    // 数据Series_Data  ['0.2','0.3','0.45']
    //刻度 Series_KeDu [[0,],[],][]]
    this.self.prototype.SetData = function (Series_Data, Series_Name) {
        this.Option.series[0].data = [{
            value: Series_Data[0],
            name: Series_Name[0]
        }];
        this.Option.series[1].data = [{
            value: Series_Data[1],
            name: Series_Name[1]
        }];
        this.Option.series[2].data = [{
            value: Series_Data[2],
            name: Series_Name[2]
        }];

        //图表设参
        this.KPI_Pie.setOption(this.Option);
    }


}

//参数 xAxis_Data_Type 类型：D 天，W 周，M 月
//参数 _M 月份  _Y 年份
export function wChart_KPI_xAxis(xAxis_Data_Type, _Y, _M) {
    var Ary = new Array();
    switch (xAxis_Data_Type) {
        case "H":
            for (var i = 0; i < 24; i++) {
                //Ary[i] = (i + 1) + "点";
                Ary[i] = i + "点";
            }
            break;
        case "M":
            for (var i = 0; i < 12; i++) {
                Ary[i] = (i + 1) + "月";
            }
            break;
        case "W":
            for (var i = 0; i <= 7; i++) {
                Ary[i] = "周" + (i + 1);
            }
            break;
        case "D":
            var isLeapYear = false;
            var is31 = false;
            if ((_M == 2) && ((_Y % 4 == 0) && (_Y % 100 != 0 || _Y % 400 == 0))) {
                for (var i = 0; i < 29; i++) {
                    Ary[i] = (i + 1) + "号";
                }
            } else if (_M == 2) {
                for (var i = 0; i < 28; i++) {
                    Ary[i] = (i + 1) + "号";
                }
            } else if (_M == 1 || _M == 3 || _M == 5 || _M == 7 || _M == 8 || _M == 10 || _M == 12) {
                for (var i = 0; i < 31; i++) {
                    Ary[i] = (i + 1) + "号";
                }
            } else {
                for (var i = 0; i < 30; i++) {
                    Ary[i] = (i + 1) + "号";
                }
            }
            break;
        case "O":
            Ary = _M;
            break;
    }
    return Ary;
}
//KPI皮肤
export function wChart_KPI_Skin(_SkinName) {
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
        case "BaiHui":
            Skin[0] = "#000";
            Skin[1] = "#000";
            Skin[2] = "#000";
            break;
        default:
            Skin[0] = "#44b6ae";
            Skin[1] = "#358f8b";
            Skin[2] = "#67c7c2";
            break;
    }
    return Skin;
}


//KPI-Grid 页面布局系统
export function kGrid_Init() {
    var kGridObj = $(".wChart-kGrid");

    $.each(kGridObj,
        function (index) {

            var Obj = kGridObj.eq(index);
            //alert();
            var W_L = Obj.children().length;
            var W = ((Obj.parent().width() - (17 * W_L)) / W_L);
            //alert(W);
            Obj.children().width(W).height(Obj.height());
        }
    )

}

export function kGrid_InitA() {
    var kGridObj = $(".wChart-kGrid");

    $.each(kGridObj,
        function (index) {

            var Obj = kGridObj.eq(index);
            //alert(Obj.parent().width());
            var W_L = Obj.children().length;
            var W = ((Obj.parent().width() - 17) / W_L);
            //alert(W + "  " + Obj.height());
            Obj.children().width(W);
        }
    );
    kGrid_Init_2();
    kGrid_Init_Float();
}

export function kGrid_Init_2() {
    var kGridObj = $(".wChart-kGrid_2");
    //var W_L = kGridObj.children().length;
    //if (W_L > 1)
    //{
    $.each(kGridObj,
        function (index) {

            var Obj = kGridObj.eq(index);
            //alert(Obj.children().length);
            var W_L = Obj.children().length;
            var W = ((Obj.width() - (12 * W_L) - W_L) / W_L);
            //var W = ((Obj.parent().width() - W_L) / W_L);
            var H = ((Obj.height() - (5 * W_L)) / W_L);
            //alert(Obj.height());
            Obj.children().width(W);
            //Obj.children().width(W).height(Obj.height());

            //Obj.children().width(W);
        }
    )
    //}

}

export function kGrid_Init_Float() {
    var kGridObj = $(".wChart-kGrid_2_Float");
    $.each(kGridObj,
        function (index) {

            var Obj = kGridObj.eq(index);
            //alert(Obj.height());
            var W_L = Obj.children().length;
            var W = ((Obj.parent().width() - (10 * W_L) - W_L) / W_L);
            var H = ((Obj.height() - (5 * W_L) - W_L) / W_L);
            alert(W);
            Obj.children().width(W).height(H);
            //Obj.children().width(W);
        }
    )
}


export function kGrid_InitC() {
    var kGridObj = $(".wChart-kGrid");

    //alert($(document).width());

    var W = kGridObj.eq(0).width();

    //alert(W)

    kGrid_Grid_1(W);
    kGrid_Grid_2(W);
    kGrid_Grid_1_Chart(W);
    kGrid_Grid_2_Chart(W);
    kGrid_Grid_3(W);
    kGrid_Grid_D3_Chart(W);
    Test(W);


}

export function kGrid_Grid_1(_W) {
    var W = _W / 4
    var kGridObj = $(".wChart-kGrid_01");
    //alert(W);


    //kGridObj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 0) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.height());
                var W_C = Obj.children().length;
                var H = ((Obj.height() - 4 - (10 * W_C) - W_C * 2) / W_C);
                Obj.width(W);
                //alert(Obj.height()+"-"+H);
                //Obj.children().height(H);

            }
        )
    }
}

export function kGrid_Grid_1_Chart(_W) {
    var W = _W / 4
    var kGridObj = $(".wChart-kGrid_01_Chart");
    //alert(W);

    kGridObj.width(W - 12);
    //kGridObj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 0) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.height());
                var W_C = Obj.children().length;
                var H = (Obj.height() - 17);
                //alert(Obj.height() + "-" + H);
                // Obj.height(H);
                //Obj.width(W);

            }
        )
    }
}

export function kGrid_Grid_2(_W) {
    var W = _W / 2
    var kGridObj = $(".wChart-kGrid_02");
    //alert(W);
    kGridObj.width(W);

    //kGridObj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 1) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.height());
                var W_C = Obj.children().length;
                var H = ((Obj.height() - 4 - (10 * W_C) - W_C * 2) / W_C);
                //alert(Obj.height()+"-"+H);
                Obj.children().height(H);

            }
        )
    }
}

export function kGrid_Grid_3(_W) {
    var W = (_W - _W / 4);
    var kGridObj = $(".wChart-kGrid_03");
    //alert(W);
    kGridObj.width(W);

    //kGridO//bj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 0) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.width());
                var W_C = Obj.children().length;
                var H = (Obj.height() + 12);
                //var WW = ((W - (12 * 3) - 3) / 3);
                //alert(Obj.height()+"-"+H);
                Obj.height(H);
                //Obj.children().width(WW);

            }
        )
    }
}

export function kGrid_Grid_2_Chart(_W) {
    var W = _W / 2
    var kGridObj = $(".wChart-kGrid_02_Chart");
    //alert(W);

    kGridObj.width(W - 12);
    //kGridObj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 0) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.height());
                var W_C = Obj.children().length;
                var H = (Obj.height() - 17);
                //alert(Obj.height() + "-" + H);
                Obj.height(H);
                //Obj.width(W);

            }
        )
    }
}

export function kGrid_Grid_D3_Chart(_W) {
    //var W = (_W / 3);
    var kGridObj = $(".wChart-kGrid_Grid_D3_Chart");
    //alert(kGridObj.length);
    //kGridObj.width(W - 12);
    //kGridObj.children().width(W-10);
    var W_L = kGridObj.length;
    if (W_L > 0) {
        $.each(kGridObj,
            function (index) {

                var Obj = $(this);
                //alert(Obj.parent().width());

                //var W_C = Obj.children().length;
                var W = ((Obj.parent().width() - (12 * 3) - 3) / 3);
                var H = (Obj.height() - 17);
                //alert(Obj.height() + "-" + H);
                Obj.height(H);
                Obj.width(W);

            }
        )
    }
}
