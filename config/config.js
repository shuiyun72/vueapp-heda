// 项目前端配置  
export default {
  apiPath: {
    gis: 'http://218.0.0.33:9921/GisWeb/PipeManage/GetGisData.aspx',
    //monitor&company为监控类api 巡检可以不用
    monitor: 'http://58.218.184.194/MonitorAPI/api',
    company: 'http://58.218.184.194/MonitorAPI/api/FlowSync/Web_NewsInfo'
  },
  // 地图服务sdk Key
  mapKey: 'VunddhrWNA4rloeRPi7KYifYSFBqVwPv',
  //二维码地址
  downloadEvmUrl:"fengxian.png",
  //地图相关设置
  MapRelated:{   
    EPSG:{
      number:"4548", 
      compile:"+proj=tmerc +lat_0=0 +lon_0=117 +k=1 +x_0=500000 +y_0=0 +ellps=GRS80 +units=m +no_defs",
      extent:[433703.6710154075,3804180.357570882,490369.6617651227, 3872663.384633132]
    },
    //地图中心点
    mapCenter:[462505.65184260975,3840769.4965086486],
    mapZoom:3,
    // 遥感图层
    SatellLayer:{
      url:"http://58.218.184.194:6080/arcgis/rest/services/fx/FX_yxt/MapServer",
      extent:[ 433715.02683222864, 3805779.2912019757,490370.2181425005,3870170.1127892]
    },
    // 街道图层
    StreetLayer:{
      url:'http://58.218.184.194:6080/arcgis/rest/services/fx/FX_jdt/MapServer',
      extent: [433703.6710154075,3804180.357570882,490369.6617651227, 3872663.384633132]
    },
    // 管线图层
    PipeLayer:{
      url:'http://58.218.184.194:6080/arcgis/rest/services/fx/FX_pipe/MapServer',
      extent:[433034.02236330963, 3834196.1179717313,485064.4389241428,3867652.74738499]
    },
    // ,
    // //矢量图层
    // VectorLayer:{
    //   url:'http://113.135.193.141:6080/arcgis/rest/services/sm/SM_vector/MapServer',
    //   extent:[424071.91199999955, 4282256.365, 431614.28809999954, 4294264.1546]
    // }
     // POI图层
    PoiLayer:{
      url:'http://58.218.184.194:6080/arcgis/rest/services/fx/FX_poi/MapServer',
      extent:[441501.12739999965,3810286.8399, 487376.5273000002,3867216.8835000005]
    }
  },
  // 开启定位功能支持的地理范围
  locationExtent: {
    longitude: [ 433703.6710154075,490369.6617651227],
    latitude: [3804180.357570882,3872663.384633132]
  }
}
