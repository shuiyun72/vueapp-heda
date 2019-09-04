const OUTER_ICON_STYLE = {
  fontSize: '30px',
  color: '#11A4DB'
}

const INNER_ICON_STYLE = {
  fontSize: '30px',
  color: 'teal',
  // color: 'sienna'
}


const FA_ICON_PATCH = {
  lineHeight: '30px',
  fontSize: '25px'
}

function combine(...objList) {
  return Object.assign({}, ...objList)
}


export default {
  ActionbarConfig: {
    GIS: [{
        id: "gauge",
        text: "测量",
        iconClass: 'fas fa-ruler-horizontal',
        iconStyle: combine(OUTER_ICON_STYLE, FA_ICON_PATCH),
        children: [{
            id: "area",
            text: "面 积",
            iconClass: "i_the_measure_of",
            iconStyle: INNER_ICON_STYLE
          },
          {
            id: "length",
            text: "距 离",
            iconClass: "i_measure",
            iconStyle: INNER_ICON_STYLE
          },
          {
            id: "clear",
            text: "清 除",
            iconClass: "fas fa-eraser",
            iconStyle: combine(INNER_ICON_STYLE, FA_ICON_PATCH),
          }
        ]
      },
      // {
      //   id: "pick",
      //   text: "点选",
      //   iconClass: "fas fa-mouse-pointer",
      //   iconStyle: combine(OUTER_ICON_STYLE, FA_ICON_PATCH),
      //   children: [{
      //       id: "pick-pipe",
      //       text: "管线",
      //       iconClass: "fas fa-code-branch",
      //       iconStyle: combine(INNER_ICON_STYLE, FA_ICON_PATCH)
      //     },
      //     {
      //       id: "pick-device",
      //       text: "设备",
      //       iconClass: "fab fa-delicious",
      //       iconStyle: combine(INNER_ICON_STYLE, FA_ICON_PATCH)
      //     },
      //     {
      //       id: "pick-coordinate",
      //       text: "坐标",
      //       iconClass: "fas fa-map-marker-alt",
      //       iconStyle: combine(INNER_ICON_STYLE, FA_ICON_PATCH)
      //     },
      //   ]
      // },
      {
        id: "reset",
        text: "重 置",
        iconClass: "i_zoom_out_map",
        iconStyle: OUTER_ICON_STYLE
      },
      {
        id: "location",
        text: "开启定位",
        iconClass: "i_map_locayion",
        iconStyle: OUTER_ICON_STYLE
      },
      {
        id: "legend",
        text: "图 例",
        iconClass: "i_data_pipeline_d",
        iconStyle: OUTER_ICON_STYLE
      },
      {
        id: "layer-switcher",
        text: "图层切换",
        iconClass: "i_layers",
        iconStyle: OUTER_ICON_STYLE,
        children: [{
            id: "satellite-view",
            text: "影像地图",
            image: {
              path: './static/images/layer_earth.png',
              text: '影像地图',
              style: {},
            }
          },
          {
            id: "street-view",
            text: "街道地图",
            image: {
              path: './static/images/layer_pipe.png',
              text: "街道地图",
              style: {}
            }
          },
          {
            id: "DMA-view",
            text: "DMA区域",
            image: {
              path: './static/images/layer_DMA.png',
              text: "DMA区域",
              style: {}
            }
          }
        ]
      }
    ],
    AreaPatrolMission: [{
        id: "patrol-mission",
        text: "巡检任务",
        iconClass: 'i_maintenance',
        iconStyle: OUTER_ICON_STYLE,
        children: [{
            id: "plan-area",
            text: "计划区域",
            iconClass: 'i_roadmap',
            iconStyle: INNER_ICON_STYLE,
          },
          {
            id: "patrol-point",
            text: "巡检点位",
            iconClass: 'i_map',
            iconStyle: INNER_ICON_STYLE,
          }, {
            id: "detail",
            text: "任务详情",
            iconClass: 'i_roadmap_5',
            iconStyle: INNER_ICON_STYLE,
          }
        ]
      },
      {
        id: "reset",
        text: "重置",
        iconClass: "i_zoom_out_map",
        iconStyle: OUTER_ICON_STYLE
      },
      {
        id: "location",
        text: "开启定位",
        iconClass: "i_map_locayion",
        iconStyle: OUTER_ICON_STYLE
      },
      // {
      //     id: "show-track",
      //     text: "显示轨迹",
      //     iconClass: "i_trajectory_anal",
      //     iconStyle: OUTER_ICON_STYLE
      // },
      {
        id: "layer-switcher",
        text: "图层切换",
        iconClass: "i_layers",
        iconStyle: OUTER_ICON_STYLE,
        children: [{
            id: "satellite-view",
            text: "影像地图",
            image: {
              path: './static/images/layer_earth.png',
              text: '影像地图',
              style: {},
            }
          },
          {
            id: "street-view",
            text: "街道地图",
            image: {
              path: './static/images/layer_pipe.png',
              text: "街道地图",
              style: {}
            }
          },
          {
            id: "DMA-view",
            text: "DMA区域",
            image: {
              path: './static/images/layer_DMA.png',
              text: "DMA区域",
              style: {}
            }
          }
        ]
      }
    ],
    PathPatrolMission: [{
        id: "patrol-mission",
        text: "巡检任务",
        iconClass: 'i_maintenance',
        iconStyle: OUTER_ICON_STYLE,
        children: [{
            id: "plan-area",
            text: "计划路线",
            iconClass: 'i_roadmap',
            iconStyle: INNER_ICON_STYLE,
          },
          {
            id: "patrol-point",
            text: "巡检点位",
            iconClass: 'i_map',
            iconStyle: INNER_ICON_STYLE,
          }, {
            id: "detail",
            text: "任务详情",
            iconClass: 'i_roadmap_5',
            iconStyle: INNER_ICON_STYLE,
          }
        ]
      },
      {
        id: "reset",
        text: "重置",
        iconClass: "i_zoom_out_map",
        iconStyle: OUTER_ICON_STYLE
      },
      {
        id: "location",
        text: "开启定位",
        iconClass: "i_map_locayion",
        iconStyle: OUTER_ICON_STYLE
      },
      // {
      //     id: "show-track",
      //     text: "显示轨迹",
      //     iconClass: "i_trajectory_anal",
      //     iconStyle: OUTER_ICON_STYLE
      // },
      {
        id: "layer-switcher",
        text: "图层切换",
        iconClass: "i_layers",
        iconStyle: OUTER_ICON_STYLE,
        children: [{
            id: "satellite-view",
            text: "影像地图",
            image: {
              path: './static/images/layer_earth.png',
              text: '影像地图',
              style: {},
            }
          },
          {
            id: "street-view",
            text: "街道地图",
            image: {
              path: './static/images/layer_pipe.png',
              text: "街道地图",
              style: {}
            }
          },
          {
            id: "DMA-view",
            text: "DMA区域",
            image: {
              path: './static/images/layer_DMA.png',
              text: "DMA区域",
              style: {}
            }
          }
        ]
      }
    ]
  }
}
