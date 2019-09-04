export default {
    // 监测列表页面用到了MuiList组件，此为公共的item配置
    BaseItemConfig: {
        label: "",
        labelStyle: {
            width: "60%",
            maxWidth: "60%",
            textAlign: "left"
        },
        content: "",
        contentStyle: {
            width: "30%",
            maxWidth: "30%",
            textAlign: "right"
        },
        withDefaultIcon: true
    },
    mainContentHeight: '99vh-44px',
    mapDeviceTypeToIcon: {
        '1': './static/images/guanjiandian.png',
        '2': './static/images/guanjiandian.png',
        '阀门@供水设施': './static/images/famenkai.png',
        '消防栓@供水设施': './static/images/xiaofangshuan.png'
    },
    dataTableId: {
        // 水质监测
        water: 10,
        // 流量监测
        flow: 79,
        // 压力监测
        pressure: 80
    },
    testData: {
        pressureFieldList: [{
                field: "DataPointName",
                title: "监测点名称",
                width: "200",
                align: "center",
                iDKUnit: ""
            },
            {
                field: "iPressureData",
                title: "管道压力(Mpa)",
                width: "100",
                align: "center",
                resizable: "true",
                iDataType: "2",
                iDKUnit: "Mpa",
                formatter: "reserve"
            },
            {
                field: "Voltage",
                title: "电池电压",
                width: "100",
                align: "center",
                resizable: "true",
                iDataType: "2",
                iDKUnit: "",
                formatter: "reserve"
            },
            {
                field: "NetSignal",
                title: "信号强度",
                width: "100",
                align: "center",
                resizable: "true",
                iDataType: "2",
                iDKUnit: "",
                formatter: "reserve"
            },
            {
                field: "RTUCodeValue",
                title: "RTU设备",
                width: "100",
                align: "center",
                resizable: "true",
                iDataType: "",
                iDKUnit: ""
            },
            {
                field: "ReadDate",
                title: "读取时间",
                width: "150",
                align: "center",
                formatter: "DateReplaceT"
            }
        ],

        pressureData: {
            total: 16,
            rows: [{
                    DataPointName: "开元北路（P5）",
                    RTUCodeValue: "1806110027",
                    iPressureData: 0.123,
                    Voltage: 3.58,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T19:10:09",
                    Pos: 1
                },
                {
                    DataPointName: "汽车城中压区",
                    RTUCodeValue: "1806110018",
                    iPressureData: 0.392,
                    Voltage: 3.52,
                    NetSignal: 20.0,
                    ReadDate: "2018-07-22T19:00:52",
                    Pos: 2
                },
                {
                    DataPointName: "汽车城低压区",
                    RTUCodeValue: "1806110023",
                    iPressureData: 0.289,
                    Voltage: 3.59,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T19:00:09",
                    Pos: 3
                },
                {
                    DataPointName: "如意路与兴业了交叉口（P8）",
                    RTUCodeValue: "1806110017",
                    iPressureData: 0.716,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 4
                },
                {
                    DataPointName: "迎宾大道和滨海西路北侧（p17）",
                    RTUCodeValue: "1806110044",
                    iPressureData: 0.502,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 5
                },
                {
                    DataPointName: "滨河大道与文汇路交汇处（P24）",
                    RTUCodeValue: "1806110015",
                    iPressureData: 0.476,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 6
                },
                {
                    DataPointName: "达尔扈特路与民族街交叉口（P12）",
                    RTUCodeValue: "1806110006",
                    iPressureData: 0.457,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 7
                },
                {
                    DataPointName: "创业路与其和街交叉路口（P6）",
                    RTUCodeValue: "1806110040",
                    iPressureData: 0.24,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 8
                },
                {
                    DataPointName: "金湖路与泰康东街北侧（P15）",
                    RTUCodeValue: "1806110004",
                    iPressureData: 0.196,
                    Voltage: 3.54,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 9
                },
                {
                    DataPointName: "诃额伦路与可汗街交叉路口（P20）",
                    RTUCodeValue: "1806110039",
                    iPressureData: 0.132,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 10
                },
                {
                    DataPointName: "压力检测0002",
                    RTUCodeValue: "1806110002",
                    iPressureData: 0.002,
                    Voltage: 3.59,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 11
                },
                {
                    DataPointName: "汽车城高压区",
                    RTUCodeValue: "1806110002",
                    iPressureData: 0.002,
                    Voltage: 3.59,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 12
                },
                {
                    DataPointName: "压力检测0020",
                    RTUCodeValue: "1806110020",
                    iPressureData: 0.0,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 13
                },
                {
                    DataPointName: "压力监测点0019",
                    RTUCodeValue: "1806110019",
                    iPressureData: 0.0,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 14
                },
                {
                    DataPointName: "电流压力0037",
                    RTUCodeValue: "1806110037",
                    iPressureData: 0.0,
                    Voltage: 3.62,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 15
                },
                {
                    DataPointName: "汽车城减压阀阀后压力监测",
                    RTUCodeValue: "1806110031",
                    iPressureData: 0.0,
                    Voltage: 3.61,
                    NetSignal: 0.0,
                    ReadDate: "2018-07-22T18:00:09",
                    Pos: 16
                }
            ],
            ErrCode: 0,
            ErrInfo: "",
            Data: "",
            ContentType: 0
        },

        realtimePressureChartData: {
            "xAxis": {
                "data": ["00:00", "00:15", "00:30", "00:45", "01:00", "01:15", "01:30", "01:45", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "02:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00", "03:00"],
                "type": "category"
            },
            "legend": {
                "data": ["iPressureData"]
            },
            "series": [{
                "data": ["0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.206", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207", "0.207"],
                "name": "iPressureData",
                "type": "line",
                "smooth": "true",
                "stack": null,
                "yAxisIndex": null
            }]
        },

        realtimeFlowChartData: {
            "yAxis": [{
                "name": "正瞬时流量",
                "type": "value"
            }],
            "xAxis": {
                "data": ["2018-06-29", "2018-06-30", "2018-07-01", "2018-07-02", "2018-07-03", "2018-07-04", "2018-07-05", "2018-07-06", "2018-07-07", "2018-07-08", "2018-07-09", "2018-07-10", "2018-07-11", "2018-07-12", "2018-07-13", "2018-07-14", "2018-07-15", "2018-07-16", "2018-07-17", "2018-07-18", "2018-07-19", "2018-07-22", "2018-07-23", "2018-07-24", "2018-07-25", "2018-07-26", "2018-07-27", "2018-07-28", "2018-07-29"],
                "type": "category"
            },
            "legend": {
                "data": ["iPositiveFlowRate"]
            },
            "series": [{
                "data": ["2.599", "4.136", "5.33", "2.689", "6.52", "0", "-1", "7.006", "7.653", "9.631", "10.016", "3.885", "1.488", "13.509", "-1", "-1", "22.603", "16.174", "2.98", "-1", "0", "15.408", "6.507", "-2.111", "4.772", "0", "4.606", "0", "6.747"],
                "name": "iPositiveFlowRate",
                "type": "line",
                "smooth": "true",
                "stack": null,
                "yAxisIndex": "0"
            }]
        },
        // 水质有四张图
        realtimeWQChartData: [
            // 浊度
            {
                "yAxis": {
                    "name": "浊度(NTU)",
                    "type": "value"
                },
                "title": {
                    "text": "浊度分析曲线"
                },
                "xAxis": {
                    "data": ["00:00", "00:30", "01:00", "01:30", "02:00", "02:30", "03:00", "03:30", "04:00", "04:30", "05:00", "05:30", "06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "23:30"],
                    "type": "category"
                },
                "legend": {
                    "data": ["2018-07-27", "2018-07-28", "2018-07-29"]
                },
                "series": [{
                    "data": ["0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"],
                    "name": "2018-07-27",
                    "type": "line",
                    "smooth": "true",
                    "yAxisIndex": null,
                    "markLine": {
                        "silent": true,
                        "data": []
                    }
                }, {
                    "data": ["0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"],
                    "name": "2018-07-28",
                    "type": "line",
                    "smooth": "true",
                    "yAxisIndex": null,
                    "markLine": {
                        "silent": true,
                        "data": []
                    }
                }, {
                    "data": ["0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-"],
                    "name": "2018-07-29",
                    "type": "line",
                    "smooth": "true",
                    "yAxisIndex": null,
                    "markLine": {
                        "silent": true,
                        "data": []
                    }
                }]
            },
            // 余氯
            {},
            // PH
            {}

        ]
    }
}
