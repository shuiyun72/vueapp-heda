<template>
  <div class="map_container" style="text-align: center">
    <div id="map">
      <!-- poi弹出框 -->
      <div class="ol-popup" id="poi-popup-container">
        <a href="#" id="poi-popup-closer" class="ol-popup-closer" @click="onPoiPopupCloseClick"></a>
        <div id="popup-content">
          <!-- 当前内容为关键点内容 -->
          <ul>
            <li>
              <span class="popup-content_label">名称</span>
              <span class="popup-content_text">{{currentPoiPopupContent.name}}</span>
            </li>
            <li>
              <span class="popup-content_label">坐标</span>
              <span class="popup-content_text">{{currentPoiPopupContent.address}}</span>
            </li>
            <!-- <li>
              <span class="popup-content_label">电话</span>
              <span class="popup-content_text">{{currentPoiPopupContent.mobile}}</span>
            </li>
            <li>
              <span class="popup-content_label">类别</span>
              <span class="popup-content_text">{{currentPoiPopupContent.type}}</span>
            </li> -->
          </ul>
        </div>
      </div>
      <!-- 设备详情弹出框 -->
      <div class="ol-popup" id="popup_container">
        <a href="#" id="popup-closer" class="ol-popup-closer"></a>
        <div id="popup-content">
          <!-- 当前内容为关键点内容 -->
          <ul v-if="currentPopupContent && currentPopupContent.role == 'key_point'">
            <li>
              <span class="popup-content_label">关键点名称</span>
              <span class="popup-content_text">{{currentPopupPointName}}</span>
            </li>
            <li>
              <span class="popup-content_label">关键点id</span>
              <span class="popup-content_text">{{currentPopupContent.id}}</span>
            </li>
            <li>
              <span class="popup-content_label">巡检完成</span>
              <span class="popup-content_text">{{currentPopupContent.state == 0 ? '否': '是'}}</span>
            </li>
          </ul>
          <!-- 当前内容为设备内容 -->
          <ul v-else-if="currentPopupContent && currentPopupContent.role == 'device_point'">
            <li>
              <span class="popup-content_label">设备类型</span>
              <span class="popup-content_text">{{currentPopupPointName}}</span>
            </li>
            <li>
              <span class="popup-content_label">设备id</span>
              <span class="popup-content_text">{{currentPopupContent.id}}</span>
            </li>
            <li>
              <span class="popup-content_label">巡检完成</span>
              <span class="popup-content_text">{{currentPopupContent.state == 0 ? '否': '是'}}</span>
            </li>
          </ul>
          <button type="button" class="mui-btn mui-btn-primary" @click="onEventSubmitClick">事件上报</button>
        </div>
      </div>
    </div>
    <div class="search_input_container">
      <el-select
        v-model="searchTypeSelectValue"
        placeholder="请选择"
        style="width: 25%; vertical-align: top;"
      >
        <el-option
          v-for="item in searchTypeSelectOptions"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        ></el-option>
      </el-select>
      <el-autocomplete
        style="width: 75%"
        clearable
        placeholder="输入关键字查询地点、路线"
        v-model="searchValue"
        value-key="名称"
        :trigger-on-focus="false"
        :fetch-suggestions="fetchSearchSuggestions"
        class="input-with-select"
        @select="onSuggestionItemSelect"
        v-if="searchTypeSelectValue === 'keyword'"
      >
        <!-- <i
          class="el-icon-edit el-input__icon"
          slot="suffix"
          @click="handleIconClick">
        </i> -->
        <template slot-scope="{ item }">
          <div class="name">{{ item.value }}</div>
        </template>
      </el-autocomplete>
      <el-input
        style="width: 32%"
        clearable
        placeholder="经度"
        v-model.number="searchLongitude"
        v-if="searchTypeSelectValue === 'coordinate'"
      ></el-input>
      <el-input
        style="width: 32%"
        clearable
        placeholder="纬度"
        v-model.number="searchLatitude"
        v-if="searchTypeSelectValue === 'coordinate'"
      ></el-input>
      <el-button
        icon="el-icon-search"
        type="primary"
        v-if="searchTypeSelectValue === 'coordinate'"
        @click="onCoordSearchButtonClick"
      ></el-button>
    </div>
    <div
      class="gis_legend_table_container"
      style="position:fixed; top:46px;  height: 85vh; right:0;overflow:scroll;"
    >
      <LegendTable @close="onLegendTableClose" v-if="legendTableVisible"  :allLayers="allLayers.allLayerImg"></LegendTable>
    </div>
    <div class="gis_point_detail_table_container">
      <PointDetailTable
        v-show="pointDetailTableVisible"
        @close="onPointDetailTableClose"
        :detailData="currentPickedPointDetailData"
      ></PointDetailTable>
    </div>
    <div class="gis_action_bar_container">
      <ActionBar :items="actionbarItems" @item-click="onActionBarItemClick"></ActionBar>
    </div>
    <div class="gis_action_equipment_layer" v-show="isEquipmentlayers">
      <div class="title">请选择要点选的设备:</div>
      <div class="li" v-for="item in equipmentLayersShow" :key="item.id" @click="selectEquipment(item.id)">
        <img :src="items" v-for="(items,index) in item.url" :key="index">
        <span>{{item.name}}</span>
      </div>
    </div>
  </div>
</template>

<script>
import _ from "lodash";
import config from "@config/config";
import consts from "../consts";
import mapConsts from "./consts";
import BaseMap from "@JS/Map/BaseMap";
import apiGIS from "@api/gis";
import apiInspection from "@api/inspection";
import apiMonitor from "@api/monitor";
import apiPipeLayer from "@api/pipe-layer";
import { deepCopy, calcDistance ,getSessionItem } from "@common/util";
import ActionBar from "./ActionBar";
import LegendTable from "@comp/common/LegendTable";
import PointDetailTable from "./PointDetailTable";
import ol from "openlayers";
import "@supermap/iclient-openlayers";
//ol 5.3.2
import { Point as PointGeom } from "ol/geom";
import { GeoJSON } from "ol/format";
import { Style as StyleStyle, Stroke as StrokeStyle } from "ol/style";
import ConfigBase from '@config/config';
export default {
  props: {
    // gis || patrol
    mode: {
      type: [String, Number],
      default: "gis"
    },
    taskId: {
      type: [String, Number]
    },
    taskName: {
      type: [String, Number]
    },
    feedBack: {
     type: [String, Number]
    },
    // 路线巡检: path
    // 区域巡检: area
    taskType: {
      type: String
    }
  },
  created() {
    console.log("当前地图Mode为 ", this.mode);
    if (this.mode === "patrol") {
      console.log("当前任务类型为: ", this.taskType);
    }
  },
  mounted() {
    // 实例化MapController并挂到组件实例上
    let mapController = this.initMapController();
    this.mapController.SetMapLayerShow(2, 1);
    // 初始化化两个Popup
    this.addCommonPopMarkerToMap(mapController);
    this.addPoiPopMarkerToMap(mapController);
    // 获取DMA分区数据并绘制图层（此时DMA图层Visible为false，在mapController中控制）
    this.fetchDMADistrictsTree().then(this.initDMADistrictsOnMap);
    // this.$hideLoading()
    // 为地图添加点击事件，用于点选功能
    //获取设备列表
    let mapInstance = (this.mapInstance = window.m = mapController.getInstance());
    mapInstance.on("click", event => {
      console.log("currentPopupContent",this.currentPopupContent)
      console.log(
        "点击地图！！当前点选模式为：" + (this.currentPickMode || "无"),
        "event对象",
        event
      );
      this.currentPickedPointDetailData = [];
      if (this.currentPickMode === "pick-pipe") {   //管线
        // 清除掉原来的feature
        this.mapController.objectQuerySource.clear();
        // 关闭点选，若想重新点选需要在菜单中重新开启
        this.currentPickMode = "";
        console.log("开始点选device, 中心点为: ", event.coordinate);
        if(!this.selectEquipmentId){
          mui.toast("请选择要查看的管网");
          return;
        }
        this.$showLoading();
        this.createSpatialSearch(event.coordinate,this.selectEquipmentId,resData=>{
          this.selectPublic(event.coordinate,resData)
        });          
      } else if (this.currentPickMode === "pick-device") {  //设备
        // 清除掉原来的feature
        this.mapController.objectQuerySource.clear();
        // 关闭点选，若想重新点选需要在菜单中重新开启
        this.currentPickMode = "";
        console.log("开始点选device, 中心点为: ", event.coordinate);
        if(!this.selectEquipmentId){
          mui.toast("请选择要查看的设备");
          return;
        }
          this.$showLoading();
          this.createSpatialSearch(event.coordinate,this.selectEquipmentId,resData=>{
          this.selectPublic(event.coordinate,resData)
        });
       
      } else if (this.currentPickMode === "pick-coordinate") {
        console.log("coord", event.coordinate);
        this.mapController.addQueryObjectFeature(event.coordinate);
        this.currentPickedPointDetailData = [
          { propertyName: "经度", propertyValue: event.coordinate[0] },
          { propertyName: "纬度", propertyValue: event.coordinate[1] }
        ];
        this.pointDetailTableVisible = true;
      }
    });
    //获取设备数据
    this.getLayers();
  },
  beforeDestroy() {
    if (this.mapController) {
      // 在组件注销前关闭定位，否则监听位置变化的任务不会关闭
      // this.mapController.enableGeolocation(false);
      this.mapController.openGeolocation(false);
      this.mapController = null;
    }
    if (this.mode === "patrol") {
      // 关闭订阅，避免多次进入巡检地图组件时重复订阅
      this.$eventbus.$off("geolocation", this.geoCallback);
      console.log("离开当前页面： 事件总线已关闭");
    }
    this.$eventbus.$off("map-poi-popup");
  },
  data() {
    return {
      map: "",
      mapController: "",
      mapInstance: "",
      // DMA分区树形数据
      DMATreeNodes: [],
      currentLon: "",
      currentLat: "",
      popupAccessId: "popup",
      currentPopupContent: {},
      poiPopupAccessId: "poiPopup",
      poiPopup: null,
      currentPoiPopupContent: {},

      // 图例表格是否可见
      legendTableVisible: false,

      // 点选设备/管线详情表格是否可见
      pointDetailTableVisible: false,
      // 点选设备/管线详情数据
      currentPickedPointDetailData: [],

      recommendRouteEnabled: false,
      trackEnabled: false,
      geolocationEnabled: false,
      // 当前是否启用了DMA图层
      DMALayerEnabled: false,
      actionbarItems: deepCopy(
        this.mode === "gis"
          ? mapConsts.ActionbarConfig.GIS
          : this.taskType === "path"
          ? mapConsts.ActionbarConfig.PathPatrolMission
          : mapConsts.ActionbarConfig.AreaPatrolMission
      ),
      // 由于地图注册了feature-click事件，因此此数组存储那些忽略此事件的feature
      clickIgnoreFeatureList: ["plan_area", "poi-feature"],
      // 点选功能，当前点选的模式 pick-device/pick-pipe/空
      currentPickMode: "",
      // 地理检索相关
      searchTypeSelectOptions: [
        {
          value: "keyword",
          label: "关键字"
        },
        {
          value: "coordinate",
          label: "坐标"
        }
      ],
      // keyword 关键字查询 || coordinate XY坐标查询
      searchTypeSelectValue: "keyword",
      searchValue: "",
      searchLongitude: "",
      searchLatitude: "",
      // 当前任务所有的关键点(包括未巡检和已巡检的)
      importantArr: [],
      // 当前任务所有的设备点(包括未巡检和已巡检的)
      deviceArr: [],
      // 当前是否已加载巡检点位（是否开始巡检）
      STARTED: false,
      //设备信息
      allLayers:{   
        equipment:[], //设备储存的信息
        pipe:[],  //管网储存的信息
        allLayerImg:[]
      },
      equipmentLayersShow:[],//页面展示的设备信息
      selectEquipmentId:undefined,
      isEquipmentlayers:false
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前被点击而弹出详情框的设备的格式化后的name
    currentPopupPointName() {
      if (!_.isEmpty(this.currentPopupContent)) {
        if (this.currentPopupContent.role == "key_point") {
          return this.currentPopupContent.name.includes("@")
            ? this.currentPopupContent.name.replace(
                /\@.*/g,
                this.currentPopupContent.id
              )
            : this.currentPopupContent.name + this.currentPopupContent.id;
        } else if (this.currentPopupContent.role == "device_point") {
          return this.currentPopupContent.type.includes("@")
            ? this.currentPopupContent.type.replace(
                /\@.*/g,
                this.currentPopupContent.id
              )
            : this.currentPopupContent.type + this.currentPopupContent.id;
        }
      }
    },
    // 计算出当前未巡检的关键点列表
    unfinishedImportantArr() {
      return _.filter(this.importantArr, { state: 0 }) || [];
    },
    unfinishedDeviceArr() {
      return _.filter(this.deviceArr, { state: 0 }) || [];
    }
    // 计算出当前未巡检的设备点列表
  },
  methods: {
    //点选设备或管网应用
    selectPublic(coordinate,resData){
       if(JSON.stringify(resData.data.features) != '[]'){
        let props = resData.data.features[0].attributes;
        let longitude = Number(props["coordinate_x"]);
          let latitude = Number(props["coordinate_y"]);
          this.mapController.addQueryObjectFeature(coordinate);
          // 设置中心点与缩放
          this.setMapCenterAndZoom(coordinate, 9);
          // 组装table数据
          const TableColumnConfig = resData.data.fieldAliases;
          let allKey = [];
          _.findKey(TableColumnConfig,function(value, key){
            allKey.push(key);
          })
          let formattedTableData = [];
            _.map(allKey,res=>{
              if(props[res]){
                formattedTableData.push({
                  propertyName:resData.data.fieldAliases[res],
                  propertyValue:props[res]
                })
              }
            })
          this.currentPickedPointDetailData = formattedTableData;
          this.$hideLoading();
          this.pointDetailTableVisible = true;
        }else{
          this.pointDetailTableVisible = true;
          this.$hideLoading();
        }
    },
    //初始化设备数据
    getLayers(){
      let this_ = this;
      apiPipeLayer.GetLayers().then(res=>{
         console.log( res)     
        if(res.data.layers.length > 0){
          let layers = [];
          let layerPipe = [];
          let allLayerImgs = [];
          _.map(res.data.layers,(text,indexN)=>{
            let textName = text.layerName;
            if(textName.search("管网") == -1){
              let legendsUrl = [];
              _.map(text.legend,legends=>{
                legendsUrl.push(ConfigBase.MapRelated.PipeLayer.url+'/'+(indexN)+'/images/'+legends.url);
                allLayerImgs.push({
                  name:legends.label || text.layerName,
                  url:ConfigBase.MapRelated.PipeLayer.url+'/'+(indexN)+'/images/'+legends.url,
                  sort:1
                })
              })
              layers.push({
                name:text.layerName,
                id:text.layerId,
                url:legendsUrl
              })
            }else{
              let legendsUrl = [];
              _.map(text.legend,legends=>{
                legendsUrl.push(ConfigBase.MapRelated.PipeLayer.url+'/'+(indexN+1)+'/images/'+legends.url);
                allLayerImgs.push({
                  name:legends.label || text.layerName,
                  url:ConfigBase.MapRelated.PipeLayer.url+'/'+(indexN+1)+'/images/'+legends.url,
                  sort:2
                })
              })
              layerPipe.push({
                name:text.layerName,
                id:text.layerId,
                url:legendsUrl
              })
            }
          })
          console.log(allLayerImgs); 
          allLayerImgs = _.sortBy(allLayerImgs, function(item) {
            return -item.sort;
          });
          this.$set(this.allLayers,'equipment',layers);
          this.$set(this.allLayers,'pipe',layerPipe);
          this.$set(this.allLayers,'allLayerImg',allLayerImgs);        
        }
      })
    },
    selectEquipment(id){
      this.selectEquipmentId = id; 
      this.isEquipmentlayers = false;  
    },
    /**
     * 创建矢量的Source 
     * @param {矢量图层编号} _layerURL 
     * @param {坐标对象} CoordinatesArray
     */
    createSpatialSearch(coordinate,_layerURL,callback) {
      let deviation = 0;
      if(_layerURL == 18 ){
        deviation = 1;
      }else{
        deviation = 0.6;
      }
      let xmin = coordinate[0]-deviation;
      let xmax = coordinate[0]+deviation;
      let ymin = coordinate[1]-deviation;
      let ymax = coordinate[1]+deviation;
      let MapRelated = ConfigBase.MapRelated;
      let geometryStr = {
          "rings": [
              [
                  [xmin,ymax],
                  [xmin,ymin],
                  [xmax,ymin],
                  [xmax,ymax],
                  [xmin,ymax]
              ]
          ],
          "_ring": 0,
          "spatialReference": {
              "wkid": MapRelated.EPSG.number,
              "latestWkid": MapRelated.EPSG.number
          },
          "cache": {
              "_extent": {
                  "xmin": xmin,
                  "ymin": ymin,
                  "xmax": xmax,
                  "ymax": ymax,
                  "spatialReference": {
                      "wkid":MapRelated.EPSG.number,
                      "latestWkid":MapRelated.EPSG.number
                  }
              },
              "_partwise": null
          }
      };
      _layerURL = _layerURL?_layerURL:2;
      let url = '/' + _layerURL + '/query/?f=json&' +
          'returnGeometry=true&spatialRel=esriSpatialRelIntersects&geometry=' + JSON.stringify(geometryStr) +
          '&geometryType=esriGeometryPolygon&inSR=' +MapRelated.EPSG.number + '&outFields=*' +
          '&outSR=' +MapRelated.EPSG.number;

      apiPipeLayer.GetPipeLayer(url).then(resultValue => {
        callback instanceof Function && callback(resultValue)
      }).catch(err => {
        callback instanceof Function && callback({"resData":"error"})
      });
  },
    handleIconClick(el){
      console.log(el)
    },
    setMapCenterAndZoom(center, zoom, mapInstance = this.mapInstance) {
      mapInstance.getView().setCenter(center);
      mapInstance.getView().setZoom(zoom);
    },
    // 初始化popup marker用以在点击地图上的点位时显示点位信息
    addCommonPopMarkerToMap(mapController = this.mapController) {
      // 添加popupMarker
      let element = document.getElementById("popup_container");
      let closerElement = document.getElementById(`popup-closer`);
      let popup = mapController.addPopupMarker({
        id: this.popupAccessId,
        element: element,
        closerElement: closerElement
      });
      mapController.getInstance().on("feature-click", event => {
        let clickedFeatures = event.features;
        let pointFeature = clickedFeatures.filter(feature => {
          return (
            this.clickIgnoreFeatureList.includes(feature.getId()) === false
          );
        })[0];
        let featureProps = pointFeature
          ? pointFeature.getProperties().properties
          : null;
        if (featureProps && !_.isEmpty(featureProps)) {
          // 改变popup的content
          // this.currentPopupContent = featureProps;
          if (featureProps.role === "device_point") {
            console.warn("======", featureProps);
            this.currentPopupContent = featureProps;
          } else {
            this.currentPopupContent = featureProps;
          }
          // 弹出popup
          // if (popup.getPosition() == undefined) {
          popup.setPosition(event.coords);
          // }
        }
      });
    },
    addPoiPopMarkerToMap(mapController = this.mapController) {
      // 添加popupMarker
      let element = document.getElementById("poi-popup-container");
      let closerElement = document.getElementById(`poi-popup-closer`);
      let popup = (this.poiPopup = mapController.addPopupMarker({
        id: this.poiPopupAccessId,
        element: element,
        closerElement: closerElement
      }));
      this.$eventbus.$on("map-poi-popup", data => {
        this.currentPoiPopupContent = {
          // name: data["名称"],
          // address: data["地址"],
          // mobile: data["电话"],
          // type: data["类别"]
          name: data.name,
          address:data.address
        };
        popup.setPosition([data.X, data.Y]);
      });
    },
    // 事件总线中geolocation事件的回调函数
    geoCallback(position) {
      console.log("Map:  通过事件总线拿到位置！", position);
      // 遍历关键点列表
      if (!_.isEmpty(this.unfinishedImportantArr)) {
        _.each(this.unfinishedImportantArr, point => {
          let pointPosition = this.mapController.transformProjTurn([point.longitude,point.latitude])
          let distanceForMeter =
            calcDistance(
              position.longitude,
              position.latitude,
              pointPosition[0],
              pointPosition[1],
              6
            ) * 1000;
          console.log("计算得到的距离：", distanceForMeter);
          if (distanceForMeter <= 30) {
            _.find(this.importantArr, { id: point.id }).state = 1;
            // mui.toast(`您已到位名称为 ${point.name} 的关键点`);
            this.mapController.reachPointWhichId(point.id);
            apiInspection.PostTaskEqument(
              this.taskId,
              point.name,
              point.id,
              point.longitude,
              point.latitude,
              this.currentUser.iAdminID,
              0
            ).then(res=>{
              console.log(res)
            })
            this.$alert(`您已到位名称为 ${point.name} 的关键点`, "到位通知");
          }
        });
      }
      // 遍历设备点列表
      if (!_.isEmpty(this.unfinishedDeviceArr)) {
        _.each(this.unfinishedDeviceArr, deivce => {
          let deivcePosition = this.mapController.transformProjTurn([deivce.longitude,deivce.latitude])
          let distanceForMeter =
            calcDistance(
              position.longitude,
              position.latitude,
              deivcePosition[0],
              deivcePosition[1],
              6
            ) * 1000;
          if (distanceForMeter <= 30) {
            _.find(this.deviceArr, { smid: deivce.smid }).state = 1;
            // mui.toast(`您已到位SMID为 ${deivce.smid} 的设备点`);
            this.mapController.reachPointWhichId(deivce.smid);
            //isTemp=1&deviceSmid=3696&pointType=0&taskId=1447&taskName=7月_测试检测726_区域巡检
            apiInspection.PostTaskEqument(
              this.taskId,
              deivce.name || "点位",
              deivce.smid,
              deivce.longitude,
              deivce.latitude,
              this.currentUser.iAdminID,
              deivce.deviceType
            ).then(res=>{
              console.log(res)
            })

            this.$alert(`您已到位SMID为 ${deivce.smid} 的设备点`, "到位通知");
          }
        });
      }
    },
    startPatrol() {
      console.log("开启巡检： 准备开启巡检");
      // 订阅来自GeoLocator的定位数据
      this.$eventbus.$on("geolocation", this.geoCallback);
      // 获取待到位的关键点与设备点列表
      this.fetchPointsInfo((err, data) => {
        if (err) {
          // http响应报错
          window.mui.toast("网络出错，获取关键点失败！");
        } else if (data.message) {
          // 请求成功，但result为false，数据为空
          window.mui.toast(message);
        } else {
          console.warn("====", data);
          /* 
            将数据格式化为一个任务点位数组，并开启事件总线，在每次获得到位置点时，
            与数组中每一个任务点位进行测距，如果与其中某个距离小于等于50米，
            则给出该点到位的提示信息
          */
          this.importantArr = data.importantPoints
            .map(point => {
              return {
                id: point.ImportPointId,
                name: point.ImportPointName,
                state: point.PatroState,
                longitude: Number(point.X),
                latitude: Number(point.Y)
              };
            })
            .filter(point => point.state === 0);
          console.table(this.importantArr);
          this.deviceArr = data.equPoints
            .map(device => {
              return {
                smid: device.Smid,
                state: device.PatroState,
                longitude: Number(device.X),
                latitude: Number(device.Y),
                deviceType: device.EquType
              };
            })
            .filter(device => device.state === 0);
          console.table(this.deviceArr);
        }
      });
    },
    onLegendTableClose() {
      this.legendTableVisible = false;
    },
    onEventSubmitClick() {
      this.$router.push({
        name: "EventSubmission",
        query: {
          isTemp: 1,
          deviceName: this.currentPopupPointName,
          deviceSmid: this.currentPopupContent.id,
          pointType: this.currentPopupContent.pointType,
          taskId: this.taskId,
          taskName: this.taskName
        }
      });
    },
    // 初始化地图控制器对象并绘制地图
    initMapController() {
      let mapController = (this.mapController = new BaseMap());
      mapController.Init("map");
      return mapController;
    },
    // 发送请求获取DMA分区树数据
    fetchDMADistrictsTree() {
      return apiMonitor.GetDMADistrictTree().then(res => {
        console.log(`树数据`, res);
        let treeNodes = (this.DMATreeNodes = deepCopy(res.data));
        return treeNodes;
      });
    },
    // 根据已获取的分区树数据在地图上绘制分区图形
    initDMADistrictsOnMap() {
      // console.log(`init`);
      // _.each(this.DMATreeNodes[0].children, node => {
      //   if (node.Gemo && node.Gemo != "Null" && Number(node.ParentID) > 0) {
      //     if (!this.clickIgnoreFeatureList.includes(node.RID)) {
      //       this.clickIgnoreFeatureList.push(node.RID);
      //     }
      //     this.mapController.addFeatureForDMADistricts(
      //       node.Gemo,
      //       node.RID,
      //       node.text,
      //       node.AreaColor,
      //       node.BorderColor
      //     );
      //   }
      // });
    },
    // 获取POI搜索建议
    fetchSearchSuggestions(querystr, callback) {
      console.log(
        "%c开始获取搜索建议：[mode: %s， queryMode: %s, model: %s]",
        "color: green",
        this.searchMode,
        this.searchValue,
        querystr
      );
      apiGIS
        .GetPoiSearchSuggestions(querystr)
        .then(res => {
          console.log("%c搜索建议响应： ", "color: blue", res);
          if (res.status == 200) {
            let rawList = res.data.features;
            console.log(rawList)
            for (let i in rawList) {
              rawList[i]["value"] = rawList[i].attributes.NAME;    //ps:必须为每个对象增加value字段！！因为autocomplete只识别value字段并在下拉列中显示
            }
            // rawList = querystr ? rawList.filter(this.createFilter(querystr)) : rawList;
            console.log(rawList)
            callback(rawList);
          } else {
            callback([]);
          }
        })
        .catch(err => {
          callback([]);
        });
    },
    // 选中搜索建议的一项时，打印出数据对象
    onSuggestionItemSelect(item) {
      console.log("%c选中搜索建议中的一项", "color: green", item);
      let coords = [item.geometry.x, item.geometry.y];
      this.mapController.addPoiFeature(coords);
      // 弹出地点详情框，如果不用timeout，位置会偏移（暂不知原因）
      item = {
        name: item.value,
        address: item.geometry.x+" "+item.geometry.y
      }
      setTimeout(() => {
        this.$eventbus.$emit("map-poi-popup", item);
      }, 100);
    },
    // 选中搜索建议的一项时，打印出数据对象
    // onSuggestionItemSelect(item) {
    //   console.log("%c选中搜索建议中的一项", "color: green", item);
    //   let coords = [item.X, item.Y];
    //   this.mapController.addPoiFeature(coords);
    //   // 弹出地点详情框，如果不用timeout，位置会偏移（暂不知原因）
    //   setTimeout(() => {
    //     this.$eventbus.$emit("map-poi-popup", item);
    //   }, 100);
    // },
    onPoiPopupCloseClick() {
      this.mapController.removePoiFeature();
    },
    // 经纬度搜索按钮
    onCoordSearchButtonClick() {
      let extent = config.locationExtent;
      if (
        typeof this.searchLongitude === "number" &&
        typeof this.searchLatitude === "number"
      ) {
        if (
          this.searchLongitude <= extent.longitude[1] &&
          this.searchLongitude >= extent.longitude[0] &&
          this.searchLatitude <= extent.latitude[1] &&
          this.searchLatitude >= extent.latitude[0]
        ) {
          console.log(111);
          this.mapController.addPoiFeature([
            this.searchLongitude,
            this.searchLatitude
          ]);
        } else {
          window.mui.toast(`输入的经纬度越界，请重新输入`);
        }
      } else {
        window.mui.toast(`经纬度必须为数字，请重新输入`);
      }
    },
    // 点选管线
    onPickPipeClick() {
      this.pointDetailTableVisible = false;
      this.currentPickMode = "pick-pipe";
      this.selectEquipmentId = undefined;
      this.equipmentLayersShow = this.allLayers.pipe;
      this.isEquipmentlayers = true;
      this.onClearClick();
      // 关闭点选设备
      // 开启点选管线
    },
    // 点选设备
    onPickDeviceClick() {
      this.pointDetailTableVisible = false;
      this.currentPickMode = "pick-device";
      this.selectEquipmentId = undefined;
      this.equipmentLayersShow = this.allLayers.equipment;
      this.isEquipmentlayers = true;
      this.onClearClick();
      // 关闭点选管线
      // 开启点选设备
    },
    // 点选坐标
    onPickCoordinateClick() {
      this.pointDetailTableVisible = false;
      this.currentPickMode = "pick-coordinate";
      this.onClearClick();
    },
    // 面积选择
    onAreaClick() {
      // 关闭点选功能
      this.closePickPoint();
      this.mapController.enableMeasure("area");
    },
    // 距离选择
    onLengthClick() {
      // 关闭点选功能
      this.closePickPoint();
      this.mapController.enableMeasure("line");
    },
    // 清除测距的Interaction和Feature
    onClearClick() {
      this.mapController.enableMeasure("", false);
    },
    // 定位选择
    onLocationClick() {
      // 如果当前位置不在地图范围内，则提示开启定位失败
        window.mui.toast(
          `正在${this.geolocationEnabled ? "关闭" : "开启"}定位...`
        );
        if (this.geolocationEnabled) {
          // 关闭定位
          // this.mapController.enableGeolocation(false);
          this.mapController.openGeolocation(false);
          this.geolocationEnabled = false;
          console.log("关闭定位成功");
          window.mui.toast("关闭定位成功");
          _.find(this.actionbarItems, ["id", "location"]).text = "开启定位";
        } else {
          // this.mapController.enableGeolocation(true);
          this.mapController.openGeolocation(true);
          this.geolocationEnabled = true;
          console.log("开启定位成功");
          window.mui.toast(`开启定位成功`);
          _.find(this.actionbarItems, ["id", "location"]).text = "关闭定位";
        }
    },
    //刷新选择
    onResetClick() {
      this.mapController.reset();
      this.pointDetailTableVisible = false;
      this.poiPopup && this.poiPopup.setPosition(undefined);
      if (this.DMALayerEnabled) {
        this.mapController.SetMapLayerShow(4, 0);
        this.DMALayerEnabled = !this.DMALayerEnabled;
      }
    },
    // 关闭点选功能
    closePickPoint() {
      this.pointDetailTableVisible = false;
      this.currentPickMode = "";
      this.mapController.objectQuerySource.clear();
    },
    // 显示图例表格
    onShowLegendClick() {
      this.legendTableVisible = !this.legendTableVisible;
      this.onClearClick();
    },
    // 切换到卫星影像模式
    onSatelliteViewClick() {
      this.mapController.SetMapLayerShow(1, 1);
    },
    // 切换到街道模式
    onStreetViewClick() {
      this.mapController.SetMapLayerShow(2, 1);
    },
    // 开启DMA图层
    onDMAViewClick() {
      this.mapController.SetMapLayerShow(4, this.DMALayerEnabled ? 0 : 1);
      this.DMALayerEnabled = !this.DMALayerEnabled;
    },
    onPointDetailTableClose() {
      console.log("detail table close occur!");
      this.pointDetailTableVisible = false;
    },
    onShowPointsClick() {
      // 调用此方法相当于开始巡检，因此先判断用户签到状态，并给出提示
      if (window.SIGN_STATUS === 0) {
        console.log("red");
        // 当前用户当日尚未签到
        this.$alert("开始巡检前请先在考勤管理中完成当日签到", "提示");
        // 无法开始巡检
        return;
      } else if (window.SIGN_STATUS === 2) {
        // 当前用户当日已签退
        this.$alert("查询到您当日已签退，可以继续巡检但无法同步数据", "提示");
        // 允许开始巡检，但无法同步当日位置数据
      }
      if (this.STARTED) {
        window.mui.toast("当前正在巡检，已加载点位信息，无需重复加载");
        return;
      }
      /**************************
       *  获取点位数据
       *  根据数据在地图上添加feature，同时根据当前巡检状态设置feature的颜色
       *  监听事件总线的实时位置，开始巡检模式
       **************************/
      this.$showLoading();
      this.fetchPointsInfo((err, data) => {
        if (err) {
          // http响应报错
          window.mui.toast("网络出错，获取关键点失败！");
        } else if (data.message) {
          // 请求成功，但result为false，数据为空
          window.mui.toast(message);
        } else {
          console.log("111===point 成功", data);
          this.initPointMarkers(data);
          /* 
            将数据格式化为一个任务点位数组，
            且计算属性会计算一个未完成点位数组，并开启事件总线，在每次获得到位置点时，
            与未完成数组中每一个任务点位进行测距，如果与其中某个距离小于等于40米，
            则给出该点到位的提示信息
          */
          this.importantArr = data.importantPoints
            .map(point => {
              return {
                id: point.ImportPointId,
                name: point.ImportPointName,
                state: point.PatroState,
                longitude: Number(point.X),
                latitude: Number(point.Y)
              };
            })
            .filter(point => point.state === 0);
          //console.table(this.importantArr);
          this.deviceArr = data.equPoints
            .map(device => {
              return {
                smid: device.Smid,
                state: device.PatroState,
                longitude: Number(device.X),
                latitude: Number(device.Y),
                deviceType: device.EquType
              };
            })
            .filter(device => device.state === 0);
          //console.table(this.deviceArr);
          // 开启事件总线
          this.$eventbus.$on("geolocation", this.geoCallback);
          this.$hideLoading();
          this.STARTED = true;
        }
      });
    },
    // 显示本次任务的计划区域
    onPlanAreaClick() {
      this.fetchPointsInfo((err, data) => {
        if (err) {
          // http响应报错
          window.mui.toast("网络出错，获取计划数据失败！");
        } else if (data.message) {
          // 请求成功，但result为false，数据为空
          window.mui.toast(message);
        } else {
          let planInfo = data.planInfo[0];
          let areaStr = planInfo.PlanAreaGeoText;
          let areaName =
            this.taskType === "path"
              ? planInfo.PlanPathName
              : planInfo.PlanAreaName;
          let areaPoints = _.chunk(
            areaStr.match(/\d*\.\d*/g).map(parseFloat),
            2
          );
          let f = this.mapController.addPolygonToDefaultLayer(areaPoints, {
            featureId: "plan_area",
            // fit字段表示该图形feature添加到地图上后，自动适配地图视图中心点与缩放到该feature上
            fit: true
          });
          window.mui.toast(
            `当前计划${
              this.taskType === "path" ? "路线" : "区域"
            }为： ${areaName}`
          );
        }
      });
      // this.mapController.setRoutes()
    },
    onRecommendRouteClick() {
      if (this.recommendRouteEnabled) {
        this.mapController.setRoutes(false);
        this.recommendRouteEnabled = !this.recommendRouteEnabled;
      } else {
        this.fetchPointsInfo((err, data) => {
          if (err) {
            // http响应报错
            window.mui.toast("网络出错，获取路线失败！");
          } else if (data.message) {
            // 请求成功，但result为false，数据为空
            window.mui.toast(message);
          } else {
            console.log("获取轨迹数据成功！", data);
            let rawRouteStr = data.planInfo[0].PlanPathGeoText;
            console.log(rawRouteStr, typeof rawRouteStr);
            if (!rawRouteStr || typeof rawRouteStr != "string") {
              window.mui.toast(`当前任务无推荐路线`);
            } else {
              let routeStr = rawRouteStr.slice(
                rawRouteStr.indexOf("((") + 2,
                rawRouteStr.indexOf("))")
              );
              let coordsArray = routeStr.split(",").map(s => {
                return s.split(" ").map(ss => {
                  return parseFloat(ss);
                });
              });
              console.log("解析推荐路线，生成coords数组", coordsArray);
              this.mapController.setRoutes(coordsArray);
              this.recommendRouteEnabled = !this.recommendRouteEnabled;
            }
          }
        });
      }
    },
    // 显示当前巡检员关于此次任务的历史轨迹， 与计划路线互斥，只能显示一个
    onHistoryTrackClick() {
      if (this.trackEnabled) {
        this.mapController.setRoutes(false);
        this.trackEnabled = !this.trackEnabled;
      } else {
        this.fetchPointsInfo((err, data) => {
          if (err) {
            // http响应报错
            window.mui.toast("网络出错，获取历史轨迹失败！");
          } else if (data.message) {
            // 请求成功，但result为false，数据为空
            window.mui.toast(message);
          } else {
            console.log("获取轨迹数据成功！", data);
            let allPoints = Object.assign(
              [],
              data.importantPoints,
              data.equPoints
            );
            let checkedPoints = allPoints.filter(point => {
              return point.PatroState == 1;
            });
            // let checkedPoints = allPoints;
            if (_.isEmpty(checkedPoints)) {
              window.mui.toast(`当前暂无历史轨迹，请依次巡检设备`);
            } else {
              let coordsArray = checkedPoints.map(point => {
                return [parseFloat(point.X), parseFloat(point.Y)];
              });
              this.mapController.setRoutes(coordsArray);
              this.trackEnabled = !this.trackEnabled;
            }
          }
        });
      }
    },
    fetchPointsInfo(callback) {
      apiInspection
        .GetMissionPoints(this.taskId)
        .then(res => {
          if (res.data.Data) {
            console.warn("+++++", res);
            if (callback instanceof Function) {
              callback(null, {
                planInfo: res.data.Data.Result.Data,
                importantPoints: res.data.Data.Result.ImportPointData,
                equPoints: res.data.Data.Result.EquPointData
              });
            }
          } else {
            if (callback instanceof Function) {
              callback(null, { message: res.data.Data.Result.message });
            }
          }
        })
        .catch(err => {
          console.log("Errrr!!!", err);
          if (callback instanceof Function) {
            callback(err);
          }
        });
    },
    getHistoryTrackFromTaskData(taskData) {},
    initPointMarkers(data, mapInstance) {
      // 验证mapInstance的合法性
      if (!mapInstance) {
        mapInstance = this.mapController;
      }
      this.initKeyPositionMarkers(data, mapInstance);
      this.initDeviceMarkers(data, mapInstance);
    },

    initKeyPositionMarkers(data, mapInstance) {
      // 初始化关键地点的marker
      let importantPoints = data.importantPoints;
      //let importantPoints =[{ImportPointId: 173,ImportPointName: "4324",PatroState: 1,PointType: 1,X: "113.55410901222211",Y: "34.828918085616564"}]
      console.log("222====imp point", importantPoints);
      _.each(importantPoints, deviceInfo => {
        let X = Number(deviceInfo.X);
        let Y = Number(deviceInfo.Y);
        let props = {
          // 设备点位id
          id: deviceInfo.ImportPointId,
          // 点位角色（该点位为设备点还是必达关键点）
          role: "key_point",
          // 设备点位名称
          name: deviceInfo.ImportPointName,
          // 设备详细点位类型
          type: deviceInfo.PointType,
          // 设备巡检状态（是否巡检完成 0 || 1）
          state: deviceInfo.PatroState,
          // 后端传来的原始PointType，描述关键点类型，为1或2
          pointType: deviceInfo.PointType
        };
        console.log("props---1030", props);
        let iconUrl = consts.mapDeviceTypeToIcon[deviceInfo.PointType];
        mapInstance.ADDFeatureForPoint(
          X,
          Y,
          // 已巡检过的变为灰色
          { url: iconUrl, color: props.state === 1 ? "#555" : "" },
          props
        );
      });
      console.log("333====设置关键点Markers完成！");
    },

    initDeviceMarkers(data, mapInstance) {
      // 初始化设备点的marker
      let devicePoints = data.equPoints;
      //  let devicePoints = [{X: "113.55364381900006", Y: "34.828176057000064", EquType: 2, PatroState: 1, Smid: 2002},
      //  {X: "113.55374333000009", Y: "34.82816349700005", EquType: 2, PatroState: 0, Smid: 3657},
      //   {X: "113.55362400500007", Y: "34.82817921100008", EquType: 2, PatroState: 0, Smid: 1897}];
      console.log("444====devices point", devicePoints);
      _.each(devicePoints, deviceInfo => {
        let X = Number(deviceInfo.X);
        let Y = Number(deviceInfo.Y);
        let props = {
          // 设备点位id
          id: deviceInfo.Smid,
          // 点位角色（该点位为设备点还是必达关键点）
          // role: "device_point",
          role: "poi-feature",
          // 设备点位名称
          name: deviceInfo.Smid,
          // 设备点位类型
          type: deviceInfo.EquType,
          // 设备巡检状态（是否巡检完成 0 || 1）
          state: deviceInfo.PatroState,
          pointType: deviceInfo.PointType
        };
        let iconUrl = consts.mapDeviceTypeToIcon[deviceInfo.PointType];
        console.log("D M!", { X, Y, iconUrl, props });
        mapInstance.ADDFeatureForPoint(
          X,
          Y,
          // 已巡检过的变为灰色
          { url: iconUrl, color: props.state === 1 ? "#555" : "" },
          props
        );
      });
      console.log("555====设置Markers完成！");
    },
    onMissionDetailClick() {
      // TBD
    },
    onActionBarItemClick({ item, layer }) {
      console.log(`点击ActionBar中的一项！Layer=${layer}`, item);
      // 点击除图例外的任何一项时，如果图例表格开着，先关闭表格
      this.isEquipmentlayers = false;
      this.legendTableVisible &&
        item.id !== "legend" &&
        (this.legendTableVisible = false);
      if (this.mode == "gis") {
        // GIS模式下的action
        if (layer === 0) {
          // 外层action bar
          if (item.id == "reset") {
            this.onResetClick();
          }
          if (item.id == "location") {
            this.onLocationClick();
          }
          if (item.id == "legend") {
            this.onShowLegendClick();
          }
        } else if (layer === 1) {
          // 内层action bar
          if (item.id == "area") {
            this.onAreaClick();
          }
          if (item.id == "length") {
            this.onLengthClick();
          }
          if (item.id == "clear") {
            this.onClearClick();
          }
          if (item.id == "satellite-view") {
            this.onSatelliteViewClick();
          }
          if (item.id == "street-view") {
            this.onStreetViewClick();
          }
          if (item.id == "DMA-view") {
            this.onDMAViewClick();
          }
          if (item.id == "pick-pipe") {
            this.onPickPipeClick();
          }
          if (item.id == "pick-device") {
            this.onPickDeviceClick();
          }
          if (item.id == "pick-coordinate") {
            this.onPickCoordinateClick();
          }
        }
      } else if (this.taskType === "area") {
        // 区域巡检任务模式下的action
        if (layer == 0) {
          if (item.id == "reset") {
            this.onResetClick();
          }
          if (item.id == "location") {
            this.onLocationClick();
          }
          if (item.id == "show-track") {
            this.onHistoryTrackClick();
          }
        } else if (layer == 1) {
          if (item.id == "plan-area") {
            this.onPlanAreaClick();
          }
          if (item.id == "patrol-point") {
            this.onShowPointsClick();
          }
          if (item.id == "detail") {
            this.onMissionDetailClick();
          }
          if (item.id == "satellite-view") {
            this.onSatelliteViewClick();
          }
          if (item.id == "street-view") {
            this.onStreetViewClick();
          }
          if (item.id == "DMA-view") {
            this.onDMAViewClick();
          }
        }
      } else if (this.taskType === "path") {
        // 路线巡检模式下的action
        if (layer == 0) {
          if (item.id == "reset") {
            this.onResetClick();
          }
          if (item.id == "location") {
            this.onLocationClick();
          }
          if (item.id == "show-track") {
            this.onHistoryTrackClick();
          }
        } else if (layer == 1) {
          if (item.id == "plan-area") {
            this.onPlanAreaClick();
            this.onRecommendRouteClick();
          }
          if (item.id == "patrol-point") {
            this.onShowPointsClick();
          }
          if (item.id == "detail") {
            this.onMissionDetailClick();
          }
          if (item.id == "satellite-view") {
            this.onSatelliteViewClick();
          }
          if (item.id == "street-view") {
            this.onStreetViewClick();
          }
          if (item.id == "DMA-view") {
            this.onDMAViewClick();
          }
        }
      }
    }
  },
  components: { ActionBar, LegendTable, PointDetailTable }
};
</script>

<style lang="less">
.tooltip-measure_dblock{
  display: block;
}
.map_container {
  position: relative;
  .search_input_container {
    display: flex;
    font-size: 0px;
    position: fixed;
    top: 55px;
    left: 0px;
    right: 0px;
    .el-select .el-input {
      color: #001d26;
    }
    .input-with-select {
      width: 100%;
      .el-input-group__prepend {
        background-color: #fff;
      }
    }
    .search_append_button {
      background-color: #2e3847;
      color: white;
      border-radius: unset;
    }
  }
  .gis_action_bar_container {
    position: absolute;
    bottom: 0px;
    left: 0px;
    right: 0px;
    width: 100%;
  }

  /* 提示框的样式信息 */
  .tooltip {
    position: relative;
    display: block;
    background: rgba(0, 0, 0, 0.5);
    border-radius: 4px;
    color: white;
    padding: 4px 8px;
    opacity: 0.7;
    white-space: nowrap;
  }
  .tooltip-measure {
    opacity: 1;
    font-weight: bold;
  }
  .tooltip-static {
    background-color: #ffcc33;
    color: black;
    border: 1px solid white;
  }
  .tooltip-measure:before,
  .tooltip-static:before {
    border-top: 6px solid rgba(0, 0, 0, 0.5);
    border-right: 6px solid transparent;
    border-left: 6px solid transparent;
    content: "";
    position: absolute;
    bottom: -6px;
    margin-left: -7px;
    left: 50%;
  }
  .tooltip-static:before {
    border-top-color: #ffcc33;
  }

  #map {
    height: calc(~"99vh - 44px");
  }
  #mouse-position {
    position: absolute;
    bottom: 50px;
    right: 0px;
  }
  #MapBottomButton {
    position: absolute;
    bottom: 0px;
    text-align: center;
  }
  #MapRightButton {
    position: absolute;
    bottom: 70px;
    right: 0;
  }
  #Current-location {
    position: absolute;
    bottom: 70px;
    left: 12px;
  }
  /* zoom控件的样式 */
  div.ol-zoom {
    top: 65%;
    left: 1%;
  }
  /* popup的样式 */
  .ol-popup {
    position: absolute;
    background-color: white;
    -webkit-filter: drop-shadow(0 1px 4px rgba(0, 0, 0, 0.2));
    filter: drop-shadow(0 1px 4px rgba(0, 0, 0, 0.2));
    padding: 15px;
    border-radius: 10px;
    border: 1px solid #cccccc;
    bottom: 62px;
    left: 62px;
  }
  .ol-popup:after,
  .ol-popup:before {
    top: 100%;
    border: solid transparent;
    content: " ";
    height: 0;
    width: 0;
    position: absolute;
    pointer-events: none;
  }
  .ol-popup:after {
    border-top-color: white;
    border-width: 10px;
    left: 48px;
    margin-left: -10px;
  }
  .ol-popup:before {
    border-top-color: #cccccc;
    border-width: 11px;
    left: 48px;
    margin-left: -11px;
  }
  .ol-popup-closer {
    text-decoration: none;
    position: absolute;
    top: 2px;
    right: 8px;
  }
  .ol-popup-closer:after {
    content: "✖";
  }
  #popup-content {
    min-width: 60vw;
    font-size: 14px;
    font-family: "微软雅黑";
    .markerInfo {
      font-weight: bold;
    }
    ul {
      padding-left: unset;
      list-style-type: none;
      text-align: left;
      li {
        margin: 5px 0;
        border-bottom: 1px solid lightskyblue;
        .popup-content_label {
          display: inline-block;
          color: #999;
          width: 35%;
        }
        .popup-content_text {
          display: inline-block;
        }
      }
    }
  }
}
.gis_action_equipment_layer{
  position:absolute;
  left: 0;
  bottom:61px;
  background: rgba(0,0,0,.7);
  padding: 2px 0px 0px 4px;
  text-align: left;
  width: 100%;
  box-shadow:0 2px 4px -1px rgba(0,0,0,.2);
  .title{
    color: #eee;
    font-size: 14px;
    padding: 6px 0 6px 2px;
    border-bottom: 1px solid #222;
    margin-bottom: 2px;
    box-shadow:0 1px 1px -1px rgba(255,255,255,.6);
  }
  .li{
    display:inline-block;
    border: 1px solid #999;
    padding: 6px 8px;
    margin: 4px;
    color: #eee;
    border-radius:2px;
    img{
      vertical-align: middle;
    }
  }
}
</style>
