// 公司办公
import {
  CompanyInfoIndex,
  CompanyInfoHome,
  CompanyInfoList,
  CompanyInfoDetail,
} from './CompanyInfo/index'

// 待办已办
import {
  OATodoIndex,
  OADoneIndex,
  OATodoList,
  OATodoDetail,
  OADoneList,
  OADoneDetail
} from './OAManager/index'

// OA发起
import OAPublisherIndex from './OAPublisher'

// 个人事件管理
import {
  SelfTaskManagerIndex,
  SelfTaskManagerList,
  SelfTaskManagerDetail
} from './SelfTaskManager/index'

// 生产调度
import StateSummary from './StateSummary'
import ProcessMonitor from './ProcessMonitor'
import DutyLog from './DutyLog/'

// 数据查询
import {
  StatisticIndex,
  StatisticQuery,
  StatisticDetail
} from './Statistic/index'

// 压力监测
import {
  PressureMonitorIndex,
  PressureMonitorList,
  PressureMonitorDetail
} from './PressureMonitor/index'

// 流量监测
import {
  FlowMonitorIndex,
  FlowMonitorList,
  FlowMonitorDetail
} from './FlowMonitor/index'

// 水质监测
import {
  WaterQualityMonitorIndex,
  WaterQualityMonitorList,
  WaterQualityMonitorDetail
} from './WaterQualityMonitor/index'

// DMA监测
import {
  DMAMonitorIndex,
  DMAMonitorList,
  DMAStatistic,
} from './DMAMonitor/index'


// 管网巡检
import Attendance from './Attendance'
import RepairOrders from './RepairOrders'
import OrderDetail from './OrderDetail'
import OrderAssignment from './OrderAssignment'
import OrderHandler from './OrderHandler'
import PatrolMission from './PatrolMission'
import Map from './GIS/Map'
import ConservationMission from './ConservationMission'
import EventSubmission from './EventSubmission/EventForm'
import AccountCenter from './AccountCenter'
import Setting from './Setting'

export {
  CompanyInfoIndex,
  CompanyInfoHome,
  CompanyInfoList,
  CompanyInfoDetail,
  OATodoIndex,
  OADoneIndex,
  OATodoList,
  OATodoDetail,
  OADoneList,
  OADoneDetail,
  OAPublisherIndex,
  SelfTaskManagerIndex,
  SelfTaskManagerList,
  SelfTaskManagerDetail,
  StateSummary,
  ProcessMonitor,
  DutyLog,
  StatisticIndex,
  StatisticQuery,
  StatisticDetail,
  PressureMonitorIndex,
  PressureMonitorList,
  PressureMonitorDetail,
  FlowMonitorIndex,
  FlowMonitorList,
  FlowMonitorDetail,
  WaterQualityMonitorIndex,
  WaterQualityMonitorList,
  WaterQualityMonitorDetail,
  DMAMonitorIndex,
  DMAMonitorList,
  DMAStatistic,
  Map,
  Attendance,
  RepairOrders,
  OrderDetail,
  OrderAssignment,
  OrderHandler,
  PatrolMission,
  ConservationMission,
  EventSubmission,
  AccountCenter,
  Setting
}
