import axios from 'axios'
import config from '../../config/config.js'
// axios.defaults.withCredentials = true
const instance = axios.create({
  baseURL: config.apiPath.monitor,
  timeout: 30000,
  // 该函数指定响应数据进行的预处理，return的值会填到response.data
  // transformResponse: function (resXmlData) {
  //     // 将相应数据从xml格式转换为js Object，返回值即为then回调中的res.data
  //     let parser = new window.DOMParser()
  //     let xmlDoc = parser.parseFromString(resXmlData, 'text/xml')
  //     let jsonStr = xmlDoc.getElementsByTagName('string')[0].innerHTML
  //     let parsedResData = JSON.parse(jsonStr)
  //     return parsedResData
  // }
  transformResponse: function (jsonStr) {
    return JSON.parse(jsonStr.replace(/\\/g, '').slice(1, -1))
  }
});

// 监测API
export default {
  getInstance() {
    return instance
  },

  // 获取维修工单模块，到场和退单原因下拉菜单的配置
  GetOrderSelectorOptions(type) {
    let jsonFileName = ''
    if (type === 'arrive') {
      jsonFileName = 'arrive.json'
    } else if (type === 'charge-back') {
      jsonFileName = 'chargeback.json'
    }
    return instance.get(`/../../monitorapi/${jsonFileName}`, {
      transformResponse(res) {
        return res
      }
    })
  },

  // 检查移动端版本更新
  CheckAppUpdate() {
    return instance.get('/AndroidVersion')
  },

  // 运行总览
  GetWaterOverview() {
    return instance.get('/WaterFactoryBaseData/BaseDataFlow')
  },

  // 获取运行总览下方表格数据，同时也是过程监控的数据
  GetTableData(dataTableIds) {
    // 默认值
    dataTableIds || (dataTableIds = '75,76,77,78')
    // dataPointId || (dataPointId = Math.random())
    return instance.get('/DataRunTimeForDataTable', {
      params: {
        DataTableID: dataTableIds,
      }
    })
  },

  // 获得值班日志列表
  GetDutyLogData(pageNumber, rowsPerPage = 20, workerName = '', startTime = '', endTime = '', recordId = '') {
    return instance.get('/NoticeMessage/GetLogBook', {
      params: {
        page: pageNumber,
        rows: rowsPerPage,
        cWorkerName: workerName,
        iDutyRecordID: recordId,
        Begintime: startTime,
        Endtime: endTime
      }
    })

  },

  // 获取（压力、流量、水质监测）动态字段的列表
  GetFieldList(dbTableId) {
    return instance.get('/FieldsColumn', {
      params: {
        DataTableID: dbTableId,
      }
    })
  },

  GetPointFieldList(dbTableId, dataPointId) {
    return instance.get('/FieldsColumn', {
      params: {
        DataTableID: dbTableId,
        DataPointID: dataPointId
      }
    })
  },

  // 获得实时压力列表数据
  GetPressureRTList(pageNumber = 1, rowsPerPage = 500, dataPointName = '') {
    return instance.get('/NPressure', {
      params: {
        DataPointName: dataPointName,
        page: pageNumber,
        rows: rowsPerPage
      }
    })
  },

  // 获得实时流量列表数据
  GetFlowRTList(pageNumber = 1, rowsPerPage = 500, dataPointName = '') {
    return instance.get('/BMeterInfo', {
      params: {
        DataPointName: dataPointName,
        page: pageNumber,
        rows: rowsPerPage
      }
    })
  },

  // 获得实时水质列表数据
  GetWaterQualityRTList(dataPointName) {
    return instance.get('/WaterQuality', {
      params: {
        DataPointName: dataPointName
      }
    })
  },

  // 共用的charts option接口
  GetChartData(title, dataTableId, pointId, dataFieldName, beginTime = '', endTime = '') {
    return instance.get('/PhysicsPointChart/GetEChart', {
      params: {
        TitleName: title,
        DataTableID: dataTableId,
        DataPointID: pointId,
        DataFieldName: dataFieldName,
        Begintime: beginTime,
        Endtime: endTime
      },
      transformResponse(res) {
        let formattedStr = JSON.parse(JSON.parse(res))
        return JSON.parse(formattedStr)
      }
    })
  },
  // 累积流量柱状图
  GetFlowStatisticChartData(dataTableId, dataPointId, beginDate, endDate, operType = 'monthchart', lineType = 0) {
    return instance.get('/BMeterInfoStatic', {
      params: {
        operType,
        LineType: lineType,
        DataTableID: dataTableId,
        DataPointID: dataPointId,
        beginDate,
        endDate
      }
    })
  },

  /* DMA监测 */
  GetDMADistrictTree(id = 0) {
    return instance.get('/DMA', {
      params: {
        id
      }
    })
  },
  // DMA统计数据
  GetDMAStatistics(startDate, endDate) {
    return instance.get('/DMAStatistics', {
      params: {
        StartDate: startDate,
        EndDate: endDate
      }
    })
  },

  /* 数据查询 */
  // 获取物理检测点树形数据
  GetPhysicPointTree(areaId = 0, pointType = '', monitorPointId = '') {
    return instance.get('/MonitorLeftMenu', {
      params: {
        AreaID: areaId,
        PointType: pointType,
        MonitorPointId: monitorPointId
      }
    })
  },

  GetPhysicPointRecordList(dataTableId, dataPointId, pageNumber, rowsPerPage, startTime, endTime) {
    return instance.get('/PhysicsPoint', {
      params: {
        DataTableID: dataTableId,
        DataPointID: dataPointId,
        page: pageNumber,
        rows: rowsPerPage,
        Begintime: startTime,
        Endtime: endTime
      }
    })
  }
}
