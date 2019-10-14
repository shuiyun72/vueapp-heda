// 项目前端配置  
export default {
  apiPath: {
    gis: 'http://218.0.0.33:9921/GisWeb/PipeManage/GetGisData.aspx',
    //monitor&company为监控类api 巡检可以不用
    monitor: 'http://218.0.0.33:9922/MonitorAPI/api',
    company: 'http://218.0.0.33:9922/MonitorAPI/api/FlowSync/Web_NewsInfo'
  },
  // 地图服务sdk Key
  mapKey: 'VunddhrWNA4rloeRPi7KYifYSFBqVwPv',
  // 超图iServer地址
  superMapIServer: {
    url: 'http://39.100.62.29:8090/iserver/services/map-arcgis-LmFHGWT/rest/maps/图层管理',
    tablesName: {
      '普通给水管线': '普通给水管线@供水设施#1',
      '阀门': '阀门@供水设施',
      '消防栓': '消防栓@供水设施',
      '消防井': '消防井@供水设施',
      '阀门井': '阀门井@供水设施',
      '排气阀': '排气阀@供水设施',
      '直通井': '直通井@供水设施',
      '三通井': '三通井@供水设施',
      '四通井': '四通井@供水设施',
      '支井': '支井@供水设施',
      '排沙井': '排沙井@供水设施',
      '排气井': '排气井@供水设施',
      '检修井': '检修井@供水设施',
    },
    tableColumnConfig: {
      '普通给水管线': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "管线编号",
          "title": "管线编号",
        },
        {
          "field": "道路名称",
          "title": "所在道路",
        },
        {
          "field": "管径",
          "title": "管径",
        },
        {
          "field": "管长",
          "title": "管长",
        },
        {
          "field": "管道材质",
          "title": "管道材质",
        },
        {
          "field": "接口方式",
          "title": "接口方式",
        },
        {
          "field": "起点点号",
          "title": "起点点号",
        },
        {
          "field": "终止点号",
          "title": "终止点号",
        },
        {
          "field": "敷设年代",
          "title": "敷设年代",
        },
        {
          "field": "竣工日期",
          "title": "竣工日期",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        },
        {
          "field": "起点埋深",
          "title": "起点埋深",
        },
        {
          "field": "终点埋深",
          "title": "终点埋深",
        },
        {
          "field": "平均埋深",
          "title": "平均埋深",
        },
        {
          "field": "起点高程",
          "title": "起点高程",
        },
        {
          "field": "终点高程",
          "title": "终点高程",
        }
      ],
      '阀门': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "物探点号",
          "title": "物探点号",
        },
        {
          "field": "阀门编号",
          "title": "阀门编号",
        },
        {
          "field": "口径",
          "title": "口径",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        },
        {
          "field": "埋深",
          "title": "埋深",
        },
        {
          "field": "高程",
          "title": "高程",
        },
        {
          "field": "开关状态",
          "title": "开关状态",
        },
        {
          "field": "开关类型",
          "title": "开关类型",
        },
        {
          "field": "材质",
          "title": "材质",
        },
        {
          "field": "埋设方式",
          "title": "埋设方式",
        },
        {
          "field": "所在道路",
          "title": "所在道路",
        },
        {
          "field": "道路名称",
          "title": "道路名称",
        },
        {
          "field": "安装日期",
          "title": "安装日期",
        },
        {
          "field": "竣工日期",
          "title": "竣工日期",
        }
      ],
      '消防栓': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "管径",
          "title": "管径",
        },
        {
          "field": "管件口径",
          "title": "管件口径",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        },
        {
          "field": "安装日期",
          "title": "安装日期",
        },
        {
          "field": "竣工日期",
          "title": "竣工日期",
        }
      ],
      '消防井': [],
      '阀门井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '排气阀': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "道路名称",
          "title": "所在道路",
        }
      ],
      '直通井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '三通井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '四通井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '支井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '排沙井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '排气井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
      '检修井': [{
          "field": "SmID",
          "title": "编号",
        },
        {
          "field": "编号",
          "title": "物探点号",
        },
        {
          "field": "井盖形状",
          "title": "井盖形状",
        },
        {
          "field": "井盖尺寸",
          "title": "井盖尺寸",
        },
        {
          "field": "井盖材质",
          "title": "井盖材质",
        },
        {
          "field": "井壁结构",
          "title": "井壁结构",
        },
        {
          "field": "施工单位",
          "title": "施工单位",
        },
        {
          "field": "施工单位联系方式",
          "title": "施工单位联系方式",
        }
      ],
    }
  },
  //图层配置
  LayerConfiguration: [{
    layerCName: "标识", //图层中文名称
    layerName: 'PipeMarkLayer', //图层编号
    layerIndex: 0, //对应ArcGis图层编号
    isSpatialSearch:false,
    layerTableName: "PipeMark", //数据库表名
    listViewColumn: "E_PipeMark_Columns", //对应前端列表控件列
    viewIndex: 1, //图层显示顺序
    isActive: true, //是否显示
    featureColumn: [ //点选显示
        {
            field: "equipment_number",
            text: "管线编号",
            type: "Number"
        },
        {
            field: "material_science",
            text: "材质"
        },
        {
            field: "caliber",
            text: "口径",
            type: "Number"
        },
        {
            field: "length",
            text: "长度",
            type: "Number"
        },
        {
            field: "startingpoint_elevation",
            text: "起点高程",
            type: "Number"
        },
        {
            field: "endpoint_elevation",
            text: "终点高程",
            type: "Number"
        },
        {
            field: "Installation_address",
            text: "道路名"
        },
        {
            field: "construction_unit",
            text: "施工单位"
        },
        {
            field: "management_unit",
            text: "管理单位"
        },
        {
            field: "completion_date",
            text: "竣工日期",
            type: "Data"
        },
        {
            field: "laying_age",
            text: "敷设年代",
            type: "Number"
        },
        {
            field: "startingpoint_depth",
            text: "起点埋深",
            type: "Number"
        },
        {
            field: "endpoint_depth",
            text: "终点埋深",
            type: "Number"
        },
        {
            field: "embedding_mode",
            text: "埋设方式"
        },
        {
            field: "Interface_form",
            text: "接口形式"
        }
    ]
},{
      layerCName: "管线", //图层中文名称
      layerName: 'PipeLineLayer', //图层编号
      layerIndex: 2, //对应ArcGis图层编号
      isSpatialSearch:false,
      layerTableName: "Pipe", //数据库表名
      listViewColumn: "E_Pipe_Columns", //对应前端列表控件列
      viewIndex: 1, //图层显示顺序
      isActive: true, //是否显示
      featureColumn: [ //点选显示
          {
              field: "equipment_number",
              text: "管线编号",
              type: "Number"
          },
          {
              field: "material_science",
              text: "材质"
          },
          {
              field: "caliber",
              text: "口径",
              type: "Number"
          },
          {
              field: "length",
              text: "长度",
              type: "Number"
          },
          {
              field: "startingpoint_elevation",
              text: "起点高程",
              type: "Number"
          },
          {
              field: "endpoint_elevation",
              text: "终点高程",
              type: "Number"
          },
          {
              field: "Installation_address",
              text: "道路名"
          },
          {
              field: "construction_unit",
              text: "施工单位"
          },
          {
              field: "management_unit",
              text: "管理单位"
          },
          {
              field: "completion_date",
              text: "竣工日期",
              type: "Data"
          },
          {
              field: "laying_age",
              text: "敷设年代",
              type: "Number"
          },
          {
              field: "startingpoint_depth",
              text: "起点埋深",
              type: "Number"
          },
          {
              field: "endpoint_depth",
              text: "终点埋深",
              type: "Number"
          },
          {
              field: "embedding_mode",
              text: "埋设方式"
          },
          {
              field: "Interface_form",
              text: "接口形式"
          }
      ]
  }, {
      layerCName: "点位",
      layerName: 'ExhaustvalveLayer',
      layerIndex: 1,
      isSpatialSearch:true,
      layerTableName: "Exhaustvalve",
      listViewColumn: "E_Exhaustvalve_Columns",
      isActive: true,
      viewIndex: 2,
      featureColumn: [{
              field: "equipment_number",
              text: "编号",
              type: "Number"
          },
          {
              field: "material_science",
              text: "材质"
          },
          {
              field: "caliber",
              text: "口径",
              type: "Number"
          },
          {
              field: "elevation",
              text: "高程",
              type: "Number"
          },
          {
              field: "depth",
              text: "埋深",
              type: "Number"
          },
          {
              field: "Installation_address",
              text: "安装地址"
          },
          {
              field: "management_unit",
              text: "管理单位"
          },
          {
              field: "completion_date",
              text: "竣工日期",
              type: "Data"
          },
          {
              field: "laying_age",
              text: "敷设年代",
              type: "Number"
          },
          {
              field: "embedding_mode",
              text: "埋设方式"
          },
          {
              field: "equipment_type",
              text: "设备类型"
          },
          {
              field: "coordinate_x",
              text: "横坐标",
              type: "Number"
          },
          {
              field: "coordinate_y",
              text: "纵坐标",
              type: "Number"
          },
          {
              field: "Interface_form",
              text: "接口形式"
          },
          {
              field: "specifications",
              text: "规格"
          },
          {
              field: "manufacturer",
              text: "生产厂家"
          },
          {
              field: "businessarea",
              text: "营业片区"
          }
      ]
  }],
  // 开启定位功能支持的地理范围
  locationExtent: {
    longitude: [113.35975811932538, 113.71391131426837],
    latitude: [34.7501362981023, 34.876861366065775]
  }
}
