const ALL_PERMISSION = [{
    index: 2,
    sectionTitle: "生产调度",
    rows: [{
        index: 1,
        items: [{
            index: 1,
            title: "运行总览",
            mode: "vertical",
            destination: "StateSummary",
            picture: "./static/images/status_overview.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 2,
            title: "过程监控",
            mode: "vertical",
            destination: "ProcessMonitor",
            picture: "./static/images/process_monitoring.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 3,
            title: "值班日志",
            mode: "vertical",
            destination: "DutyLog",
            picture: "./static/images/duty_log.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 4,
            title: "数据查询",
            mode: "vertical",
            destination: "StatisticIndex",
            picture: "./static/images/statistic.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          }
        ]
      },
      {
        index: 2,
        items: [{
            index: 1,
            title: "压力监测",
            mode: "vertical",
            destination: "PressureMonitorIndex",
            picture: "./static/images/press_monitoring.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 2,
            title: "流量监测",
            mode: "vertical",
            destination: "FlowMonitorIndex",
            picture: "./static/images/flow_monitoring.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 3,
            title: "水质监测",
            mode: "vertical",
            destination: "WaterQualityMonitorIndex",
            picture: "./static/images/quality_monitoring.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          },
          {
            index: 4,
            title: "DMA监测",
            mode: "vertical",
            destination: "DMAMonitorIndex",
            picture: "./static/images/DMA_monitoring.png",
            class: "mui-col-sm-3 mui-col-xs-3"
          }
        ]
      }
    ]
  },
  {
    index: 3,
    sectionTitle: "巡检养护",
    rows: [{
        index: 1,
        items: [{
            index: 1,
            title: "移动GIS",
            desc: "管网设施浏览",
            mode: "horizontal",
            picture: "./static/images/mobile_GIS.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            destination: "Map",
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          },
          {
            index: 2,
            title: "考勤管理",
            desc: "个人签到签退",
            mode: "horizontal",
            destination: "Attendance",
            picture: "./static/images/patrol_attendance.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          }
        ]
      },
      {
        index: 2,
        items: [{
            index: 1,
            title: "维修工单",
            desc: "管网维修处理",
            mode: "horizontal",
            picture: "./static/images/repair_order.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            destination: "RepairOrders",
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          },
          {
            index: 2,
            title: "工单分派",
            desc: "维修任务分派",
            mode: "horizontal",
            picture: "./static/images/order_assignment.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            destination: "OrderAssignment",
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          }
        ]
      },
      {
        index: 3,
        items: [{
            index: 1,
            title: "巡检任务",
            desc: "巡检计划处理",
            mode: "horizontal",
            picture: "./static/images/patrol_mission.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            destination: "PatrolMission",
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          },
          {
            index: 2,
            title: "事件上报",
            desc: "临时计划处理",
            mode: "horizontal",
            picture: "./static/images/emergency_submission.png",
            pictureContainerStyle: {
              "background-size": "80% 95%"
            },
            destination: "EventSubmission",
            withBorder: true,
            class: "mui-col-sm-6 mui-col-xs-6"
          }
        ]
      }
    ]
  },
  {
    index: 4,
    sectionTitle: "系统信息",
    rows: [{
      index: 1,
      items: [{
          index: 1,
          title: "个人信息",
          desc: "配置个人信息",
          mode: "horizontal",
          picture: "./static/images/self_info.png",
          destination: "AccountCenter",
          withBorder: true,
          class: "mui-col-sm-6 mui-col-xs-6"
        },
        {
          index: 2,
          title: "系统管理",
          desc: "系统配置管理",
          mode: "horizontal",
          picture: "./static/images/setting.png",
          destination: "Setting",
          // destination: "Test",
          withBorder: true,
          class: "mui-col-sm-6 mui-col-xs-6"
        }
      ]
    }]
  }
]

// 最终权限生成没有使用该方法，而是使用v-if配合后端传来的title
export default function generateIndexPageConfig(userPermission) {

}
