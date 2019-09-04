import ol from "openlayers";
import $ from "jquery";
import _ from "lodash";
import CoordsHelper from 'coordtransform'
import Vue from 'vue'

function BaseMap() {
    this.defaultOptions = {
        center: [113.55, 34.83],
        zoom: 15
    };
    // 地图实例，在Init中初始化
    this.map = null;
    // 用于实时定位的小蓝点
    this.positionFeature = null;
    // popup标注，key为标注的id
    this.popupMarkers = {};
    this.parser = new ol.format.WMTSCapabilities();
    this.geojsonFormat = new ol.format.GeoJSON();
    this.textFeatureFormat = new ol.format.WKT();
    this.CoordinatesDetials; //绘制完成后坐标信息存放位置
    this.BaseMapData = this.GetBaseMapData(); //获取基础地图配置信息
    this.BasePipeData = this.GetPipeMapData(); //获取管线配置信息
    this.PolygonControlName; //多边形控件名称

    // 路线矢量层
    this.routesVectorLayer = null;

    this.geolocationLayer = null;
    // 监听位置移动的id，用于移除监听任务
    this.watchId = "";

    //基础地图数据（包含街道、遥感）
    var MapData = this.parser.read(this.BaseMapData);
    var PipeData = this.parser.read(this.BasePipeData);

    this.DMASource = new ol.source.Vector(); //DMA矢量图层数据源声明
    this.VectorSource = new ol.source.Vector(); //DMA矢量图层数据源声明
    //遥感数据
    // this.SatellOptions = ol.source.WMTS.optionsFromCapabilities(MapData, {
    //     layer: "遥感地图"
    // });
    // //街道数据
    // this.StreetOptions = ol.source.WMTS.optionsFromCapabilities(MapData, {
    //     layer: "街道地图"
    // });
    // //管线数据
    // this.PipeOptions = ol.source.WMTS.optionsFromCapabilities(PipeData, {
    //     layer: "管线地图"
    // });
}

BaseMap.prototype.PipeLayer; //管线图层
BaseMap.prototype.SatellLayer; //遥感图层
BaseMap.prototype.StreetLayer; //街道图层
BaseMap.prototype.Vectorlayer; //声明矢量图层
BaseMap.prototype.draw; //绘制
BaseMap.prototype.snap; //图层吸附
BaseMap.prototype.modify; //修改图层
BaseMap.prototype.PointXValue = null; //定位坐标X
BaseMap.prototype.PointYValue = null; //定位坐标X

// 获取map实例
BaseMap.prototype.getInstance = function () {
    return this.map;
};
BaseMap.prototype.getExtent = function () {
    return this.map.getView().calculateExtent()
}
// 判断一个点是否在当前地图范围内
BaseMap.prototype.containsCoord = function (coord, extent = this.getExtent()) {
    return ol.extent.containsCoordinate(extent, coord)
}

//构造基础地图图层
BaseMap.prototype.Init = function (containerId, options = {}) {
    //判断控件名称是否为空
    if (!containerId) {
        containerId = "map";
    }

    this.defaultOptions.center = options.center || [113.55, 34.83];

    // 构造矢量图层，用于巡检区域与巡检设备点位展示
    this.Vectorlayer = new ol.layer.Vector({
        source: this.VectorSource,
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: "rgba(255, 255, 255, 0.5)"
            }),
            stroke: new ol.style.Stroke({
                color: "#ffcc33",
                width: 2
            }),
            image: new ol.style.Icon({
                //color: '#4271AE',
                src: "./static/images/MeterInfo.png"
            })
        })
    });

    // // 遥感图层
    // this.SatellLayer = new ol.layer.Tile({
    //     opacity: 1,
    //     source: new ol.source.WMTS(this.SatellOptions),
    //     visible: false
    // });

    // // 街道图层
    // this.StreetLayer = new ol.layer.Tile({
    //     opacity: 1,
    //     source: new ol.source.WMTS(this.StreetOptions),
    //     visible: true
    // });

    // // 管线图层
    // this.PipeLayer = new ol.layer.Tile({
    //     opacity: 1,
    //     source: new ol.source.WMTS(this.PipeOptions),
    //     visible: true
    // });
    this.StreetLayer = new ol.layer.Tile({
        source: new ol.source.XYZ({
            url: 'http://t' + Math.round(Math.random() * 7) + '.tianditu.com/DataServer?T=vec_w&x={x}&y={y}&l={z}',
            // projection: 'EPSG:4326'
        }),
        visible: true
    });
    this.TianDiTu_ZhuJi_Layers = new ol.layer.Tile({
        source: new ol.source.XYZ({
            url: 'http://t' + Math.round(Math.random() * 7) + '.tianditu.com/DataServer?T=cva_w&x={x}&y={y}&l={z}',
            // projection: 'EPSG:4326'
        }),
        visible: true
    });

    this.SatellLayer = new ol.layer.Tile({
        source: new ol.source.XYZ({
            //url: 'http://localhost:9009/arctiler/google/services/chengdu/Google/{z}/{x}/{y}.png',
            url: 'http://t' + Math.round(Math.random() * 7) + '.tianditu.com/DataServer?T=img_w&x={x}&y={y}&l={z}',
            // projection: 'EPSG:4326'
        }),
        visible: false
    });



    // DMA矢量图层
    this.DMAlayer = new ol.layer.Vector({
        source: this.DMASource,
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: "rgba(255, 255, 255, 0.5)"
            }),
            stroke: new ol.style.Stroke({
                color: "#ffcc33",
                width: 2
            }),
            image: new ol.style.Icon({
                //color: '#4271AE',
                src: "./static/images/MeterInfo.png"
            })
        }),
        visible: false
    });

    //地图初始化
    this.map = new ol.Map({
        layers: [
            this.SatellLayer,
            this.StreetLayer,
            this.TianDiTu_ZhuJi_Layers,
            this.DMAlayer,
            this.Vectorlayer
        ], // 1,2,3,4,5
        target: containerId,
        view: new ol.View({
            center: this.defaultOptions.center,
            projection: "EPSG:4326",
            zoom: this.defaultOptions.zoom
            //,minZoom: 12,
            //maxZoom: 20
        }),
        controls: [new ol.control.Zoom()]
    });

    // 当点击到一个feature时，发出事件
    this.map.on("click", event => {
        let coords = event.coordinate;
        // 判断当前点击点是否有Feature
        let features = this.map.getFeaturesAtPixel(
            event.pixel,
        );
        console.log("点击地图！", coords, features);

        if (features && features.length > 0) {
            this.map.dispatchEvent({
                type: "feature-click",
                coords: coords,
                features
            });
        }
    });
};
// 加载路线
BaseMap.prototype.setRoutes = function (points) {
    if (points === false) {
        // 清除计划路线图层
        this.routesVectorLayer && this.map.removeLayer(this.routesVectorLayer);
        this.routesVectorLayer = null;
    } else if (_.isArray(points) && _.every(points, _.isArray)) {
        // 根据参数实例化图层与路线
        // 实例化一个source
        let source = new ol.source.Vector();
        // 实例化一个折线feature
        let feature = new ol.Feature({
            geometry: new ol.geom.LineString(points)
        });
        source.addFeature(feature);
        // 设置样式
        // 该syle设计参考https://github.com/frogfans/Openlayer3-LineString
        let style = function (feature) {
            let geometry = feature.getGeometry();
            let styles = [
                new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: "#0044CC"
                    }),
                    stroke: new ol.style.Stroke({
                        lineDash: [1, 2, 3, 4, 5, 6],
                        width: 3,
                        // color: [142, 72, 198, 1]
                        color: [0, 0, 0, 1]
                    })
                })
            ];

            geometry.forEachSegment(function (start, end) {
                var arrowLonLat = [
                    (end[0] + start[0]) / 2,
                    (end[1] + start[1]) / 2
                ];
                var dx = end[0] - start[0];
                var dy = end[1] - start[1];
                var rotation = Math.atan2(dy, dx);
                styles.push(
                    new ol.style.Style({
                        geometry: new ol.geom.Point(arrowLonLat),
                        // 暂时将导航箭头隐藏
                        // image: new ol.style.Icon({
                        //     src: './static/images/route_arrow.png',
                        //     anchor: [0.75, 0.5],
                        //     // anchor: [1, 1],
                        //     rotateWithView: true,
                        //     rotation: rotation
                        // })
                    })
                );
            });
            return styles;
        };
        if (this.routesVectorLayer) {
            this.routesVectorLayer.setSource(source);
            this.routesVectorLayer.setStyle(style);
        } else {
            // 创建路线矢量图层
            this.routesVectorLayer = new ol.layer.Vector({
                source,
                style
            });
            this.map.addLayer(this.routesVectorLayer);
        }
    } else {
        console.error(
            `the first argument must be a two-dimensional array or false, passing false indicates you want to remove the routes layer`
        );
    }
};

BaseMap.prototype.geoCallback = function (position) {
    if (
        position &&
        position.longitude &&
        position.latitude
    ) {
        window.mui.toast(`经度：${position.longitude}, 纬度：${position.latitude}`, {
            duration: '1000',
            type: 'div'
        })
        try {
            this.positionFeature.setGeometry(
                new ol.geom.Point([
                    position.longitude,
                    position.latitude
                ])
            );
            this.map
                .getView()
                .setCenter([
                    position.longitude,
                    position.latitude
                ]); //平移地图
        } catch (error) {
            mui.toast('eeeeeeeeeeeeeeeeeeeeeeeeeee')
        }
    }
}

// 启用定位功能的备用方案
BaseMap.prototype.openGeolocation = function (actionType) {
    if (actionType === undefined || actionType === null) {
        actionType = true
    }
    const eventbus = Vue.prototype.$eventbus
    // map实例必须存在
    if (this.map) {
        if (actionType) {
            // 启动
            this.positionFeature = new ol.Feature();
            this.positionFeature.setStyle(
                new ol.style.Style({
                    image: new ol.style.Circle({
                        radius: 6,
                        fill: new ol.style.Fill({
                            color: "#3399CC"
                        }),
                        stroke: new ol.style.Stroke({
                            color: "#fff",
                            width: 2
                        })
                    })
                })
            );

            //创建定位点矢量图层
            this.geolocationLayer = new ol.layer.Vector({
                source: new ol.source.Vector({
                    // features: [accuracyFeature, positionFeature]
                    features: [this.positionFeature]
                })
            });

            this.map.addLayer(this.geolocationLayer);
            this.geoCallback = this.geoCallback.bind(this)
            eventbus.$on('geolocation', this.geoCallback);
            console.log(`开启监听成功`)
        } else {
            // 停用
            console.log(`准备关闭监听`)
            eventbus.$off('geolocation', this.geoCallback)
            this.positionFeature = null
            if (this.geolocationLayer) {
                this.map.removeLayer(this.geolocationLayer);
            }
            console.log(`监听定位已关闭`)
        }
    } else {
        console.warn(
            "Missing map instance! You should call method Init firstly."
        );
    }
}

// 启用定位功能  true为开启， false为关闭
BaseMap.prototype.enableGeolocation = function (actionType) {
    if (actionType === undefined || actionType === null) {
        actionType = true
    }
    // map实例必须存在
    if (this.map) {
        if (actionType) {
            // 启动
            if (!this.watchId) {
                // let view = this.map.getView()
                // 定位点要素
                var positionFeature = new ol.Feature();
                positionFeature.setStyle(
                    new ol.style.Style({
                        image: new ol.style.Circle({
                            radius: 6,
                            fill: new ol.style.Fill({
                                color: "#3399CC"
                            }),
                            stroke: new ol.style.Stroke({
                                color: "#fff",
                                width: 2
                            })
                        })
                    })
                );

                //创建定位点矢量图层
                this.geolocationLayer = new ol.layer.Vector({
                    source: new ol.source.Vector({
                        // features: [accuracyFeature, positionFeature]
                        features: [positionFeature]
                    })
                });

                this.map.addLayer(this.geolocationLayer);

                this.watchId = nativeTransfer.getLocation(position => {
                        let coordinates = position.coords;
                        // console.log('Coords, ', coordinates)
                        if (
                            coordinates &&
                            coordinates.longitude &&
                            coordinates.latitude
                        ) {
                            let coordsFor84 = CoordsHelper.gcj02towgs84(coordinates.longitude, coordinates.latitude)
                            coordinates = {
                                longitude: coordsFor84[0],
                                latitude: coordsFor84[1]
                            }

                            // window.mui.toast(`经度：${coordinates.longitude}, 纬度：${coordinates.latitude}`)
                            positionFeature.setGeometry(
                                new ol.geom.Point([
                                    coordinates.longitude,
                                    coordinates.latitude
                                ])
                            );
                            this.map
                                .getView()
                                .setCenter([
                                    coordinates.longitude,
                                    coordinates.latitude
                                ]); //平移地图
                        }
                    }
                    // err => {
                    //     console.log("geo err!!!!!", err);
                    // }, {
                    //     enableHighAccuracy: true,
                    //     maximumAge: 1100,
                    //     timeout: 5000,
                    //     // provider: 'system',
                    //     // coordsType: 'wgs84',
                    //     provider: 'baidu'
                    // }
                );
                console.log(`开启监听成功，watchId `, this.watchId)
            }
        } else {
            // 停用
            if (this.geolocationLayer) {
                this.map.removeLayer(this.geolocationLayer);
            }
            console.log(`准备关闭监听，watchId为 `, this.watchId)
            window.plus && window.plus.geolocation.clearWatch(this.watchId);
            this.watchId = ''
            console.log(`监听定位已关闭`, this.watchId)
        }
    } else {
        console.warn(
            "Missing map instance! You should call method Init firstly."
        );
    }
};
// 重置地图
BaseMap.prototype.reset = function () {
    // 清除所有标注点与巡检区域feature
    this.VectorSource.clear();
    // 重置地图中心
    this.setCenter(...this.defaultOptions.center);
    // 重置默认缩放
    this.setZoom(this.defaultOptions.zoom);
    // 清除路线矢量图层
    this.setRoutes(false);
    // 关闭测距功能
    this.enableMeasure('', false)
};
//修改图层显示 返回需要隐藏的ID
BaseMap.prototype.SetMapLayerShow = function (_SetShowLayerIndex, _IsShow) {
    //if (OBJName) { return; }
    //debugger;
    if (_SetShowLayerIndex == 1) {
        if (_IsShow == 1) {
            this.SatellLayer.setVisible(true);
            this.StreetLayer.setVisible(false);
            return 2;
        } else {
            this.SatellLayer.setVisible(false);
            this.StreetLayer.setVisible(true);
            return 1;
        }
    } else if (_SetShowLayerIndex == 2) {
        if (_IsShow == 1) {
            this.SatellLayer.setVisible(false);
            this.StreetLayer.setVisible(true);
            return 1;
        } else {
            this.SatellLayer.setVisible(true);
            this.StreetLayer.setVisible(false);
            return 2;
        }
    } else if (_SetShowLayerIndex == 3) {
        if (_IsShow == 1) {
            this.PipeLayer.setVisible(true);
        } else {
            this.PipeLayer.setVisible(false);
        }
        return 0;
    } else if (_SetShowLayerIndex == 4) {
        if (_IsShow == 1) {
            this.DMAlayer.setVisible(true);
        } else {
            this.DMAlayer.setVisible(false);
        }
        return 0;
    }
};

//添加鼠标移动，坐标显示事件
BaseMap.prototype.AddMousePosition = function (OBJName) {
    if (!OBJName) {
        OBJName = "mouse-position";
    }
    //鼠标纵横坐标移动显示
    var mousePositionControl = new ol.control.MousePosition({
        coordinateFormat: ol.coordinate.createStringXY(4),
        projection: "EPSG:4326",
        className: "custom-mouse-position",
        target: document.getElementById(OBJName),
        undefinedHTML: "&nbsp;"
    });
    //添加鼠标移动显示坐标
    this.map.addControl(mousePositionControl);
};

// 添加popup标注
BaseMap.prototype.addPopupMarker = function (options) {
    // 提取参数中的字段，并为不存在的参数字段提供默认值
    let {
        id,
        element,
        closerElement,
        positioning = "bottom-center",
        autoPan = true,
        stopEvent = true
    } = options;

    !closerElement && console.error("Missing popup closer element.");
    !id &&
        console.error(
            "Missing popup id when you call method: addPopupMarker(options)"
        );
    id in this.popupMarkers &&
        console.error(`Given popup id [${id}] is duplicated`);

    let popup = new ol.Overlay({
        element,
        autoPan,
        positioning,
        stopEvent,
        autoPanAnimation: {
            duration: 250
        }
    });
    // 为closer添加事件
    closerElement &&
        (closerElement.onclick = () => {
            popup.setPosition(undefined);
            closerElement.blur();
            return false;
        });

    this.map.addOverlay(popup);

    return popup;
};
// 根据获取popupMarker实例
BaseMap.prototype.getPopupById = function (id) {
    return this.popupMarkers[id];
};

//图层绘制方法
BaseMap.prototype.AddInteraction = function (DrawType) {
    if (!DrawType) {
        DrawType = "Point";
    }
    this.modify = new ol.interaction.Modify({
        source: this.VectorSource
    });
    this.snap = new ol.interaction.Snap({
        source: this.VectorSource
    });
    this.draw = new ol.interaction.Draw({
        source: this.VectorSource,
        type: DrawType
    });
    this.draw.on(
        "drawend",
        function (evt) {
            this.VectorSource.clear(); //清除所有标注点
            var feature = evt.feature; //获取当前属性
            this.CurrentFeature = feature;
            switch (DrawType) {
                case "Point":
                    this.CoordinatesDetials = feature
                        .getGeometry()
                        .getCoordinates(); //当前坐标信息
                    this.PointXValue.textbox(
                        "setValue",
                        this.CoordinatesDetials[0]
                    ); //取得经度坐标
                    this.PointYValue.textbox(
                        "setValue",
                        this.CoordinatesDetials[1]
                    ); //取得维度坐标
                    break;
                case "LineString":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    // let length = Math.round(feature.getGeometry().getLength() * 10000) / 100;
                    // console.log('############', length)
                    break;
                case "Polygon":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    this.setModifyActive();
                    break;
                case "MultiPoint":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    break;
                case "MultiLineString":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    break;
                case "MultiPolygon":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    break;
                case "Circle":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    ); //绘制图形坐标信息
                    this.SetControlForPolygon();
                    break;
                default:
                    this.CoordinatesDetials = feature
                        .getGeometry()
                        .getCoordinates(); //当前坐标信息
                    break;
            }
        },
        this
    );

    //修改已有图形
    this.modify.on(
        "modifyend",
        function (evt) {
            var feature = evt.features.item(0); //获取当前属性
            switch (DrawType) {
                case "Point":
                    this.CoordinatesDetials = feature
                        .getGeometry()
                        .getCoordinates(); //当前坐标信息
                    break;
                case "LineString":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                case "Polygon":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                case "MultiPoint":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                case "MultiLineString":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                case "MultiPolygon":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                case "Circle":
                    this.CoordinatesDetials = this.textFeatureFormat.writeFeature(
                        feature
                    );
                    this.SetControlForPolygon();
                    break;
                default:
                    this.CoordinatesDetials = feature
                        .getGeometry()
                        .getCoordinates(); //当前坐标信息
                    break;
            }
        },
        this
    );
    this.map.addInteraction(this.draw);
    this.map.addInteraction(this.snap);
    this.map.addInteraction(this.modify);
};

//去除绘制图形
BaseMap.prototype.RemoveInteraction = function () {
    this.map.removeInteraction(this.draw);
    this.map.removeInteraction(this.snap);
    this.map.removeInteraction(this.modify);
};

//添加选中事件
BaseMap.prototype.AddInteractionSelect = function () {
    this.select = new ol.interaction.Select({
        layers: [this.Vectorlayer],
        condition: ol.events.condition.click
    }); //指定图层
    this.map.addInteraction(this.select); //将该控件添加进地图中去

    this.select.on("select", function (e) {
        var len = e.target.getFeatures().getLength();
        if (len > 0) {
            debugger;
            var prop = e.selected[0].getProperties();
            //var DataTableID = prop.properties["iMonitorDataTableID"];
            //var DataPointID = prop.properties["DataPointID"];
            //单点数据分析
            //top.OpenDialog("/Bussiness_PhysicsMonitor/TabDefault.aspx?DataTableID=" + prop.properties["iMonitorDataTableID"] + "&DataPointID=" + prop.properties["DataPointID"] + '&DataPointName=' + prop.properties["DataPointName"], "Map_DataPoint_Alaysis" + DataPointID.toString(), prop.properties["DataPointName"] + "分析", $(window).width(), $(window).height());
        }
    });
};

//绘制启动
BaseMap.prototype.setDrawActive = function () {
    this.draw.setActive(true);
    this.modify.setActive(false);
};

//修改启动
BaseMap.prototype.setModifyActive = function () {
    this.draw.setActive(false);
    this.modify.setActive(true);
};

//取得遥感数据
BaseMap.prototype.GetBaseMapData = function () {
    var ReturnValue;
    $.ajax({
        type: "GET",
        url: "http://47.104.3.68:8090/iserver/services/map-XinJieDaoYaoGanTu/wmts100",
        dataType: "XML",
        async: false,
        error: function (xml) {
            alert("加载XML文档出错");
        },
        success: function (data) {
            ReturnValue = data;
        }
    });
    return ReturnValue;
};

//取得管线图层数据信息
BaseMap.prototype.GetPipeMapData = function () {
    var ReturnValue;
    $.ajax({
        type: "GET",
        url: "http://47.104.3.68:8090/iserver/services/map-GIS/wmts100",
        dataType: "XML",
        async: false,
        error: function (xml) {
            alert("加载XML文档出错");
        },
        success: function (data) {
            ReturnValue = data;
        }
    });
    return ReturnValue;
};

//清空绘制过的要素
BaseMap.prototype.removeAllFeature_Region = function () {
    var features = this.VectorSource.getFeatures();
    if (features != null && features.length > 0) {
        for (x in features) {
            this.VectorSource.removeFeature(features[x]);
        }
    }
};

//将绘制的多边形存放至控件
BaseMap.prototype.SetControlForPolygon = function () {
    if (this.CoordinatesDetials) {
        $("#" + this.PolygonControlName).html(
            this.CoordinatesDetials.toString()
        );
    } else {
        $("#" + this.PolygonControlName).val("NULL");
    }
};

// 添加新feature至默认矢量图层
BaseMap.prototype.addPolygonToDefaultLayer = function (points, {
    featureId,
    fit = false
}) {
    if (typeof featureId !== "string") {
        console.error(
            "the second argument featureId is required, it is a custom string"
        );
    } else if (this.VectorSource.getFeatureById(featureId)) {
        return this.VectorSource.getFeatureById(featureId);
    } else {
        let feature = new ol.Feature({
            geometry: new ol.geom.Polygon([points])
        });
        let style = new ol.style.Style({
            fill: new ol.style.Fill({
                color: [153, 0, 51, 0.2]
            }),
            stroke: new ol.style.Stroke({
                color: [0, 0, 0, 0.5],
                width: 2
            })
        });
        feature.setId(featureId);
        feature.setStyle(style);
        console.log("!!!", feature);
        this.VectorSource.addFeatures([feature]);
        if (fit) {
            this.map.getView().fit(feature.getGeometry())
        }
        return feature;
    }
};

// 设置DMA图层的显示与隐藏
BaseMap.prototype.showDMALayer = function (state = true) {
    this.DMAlayer.setVisible(state)
}

//将绘制的多边形添加至地图，用于DMA区域绘制
BaseMap.prototype.addFeatureForDMADistricts = function (
    GeoJson,
    AreaID,
    PMonitorAreaName,
    FillColor,
    StorkeColor
) {
    //判断当前分区是否存在，如果存在，不进行重复添加
    if (this.DMASource.getFeatureById(AreaID)) {
        return;
    }
    //先获取feature
    var features = this.textFeatureFormat.readFeatures(GeoJson);
    //再获取feature的geometry
    var geometry = features[0].getGeometry();
    var feature2 = new ol.Feature({
        geometry: geometry,
        name: PMonitorAreaName,
        properties: {
            id: AreaID,
            DataPointName: PMonitorAreaName
        }
    });
    feature2.setId(AreaID);

    /************色值赋值开始*****************/
    if (!FillColor) {
        FillColor = "rgba(0,0,0,0)";
    }
    if (!StorkeColor) {
        StorkeColor = "rgb(153, 0, 51)";
    }

    FillColor = this.colorRgb(FillColor); //将16进制转为rgba的颜色格式
    //为Feature填充颜色
    var featureStyle = new ol.style.Style({
        fill: new ol.style.Fill({
            color: FillColor
        }),
        stroke: new ol.style.Stroke({
            color: StorkeColor,
            width: 2
        }),
        text: this.createTextStyle(feature2, PMonitorAreaName)
    });
    /**************色值赋值结束***************/
    feature2.setStyle(featureStyle); //为Feature赋值
    this.DMASource.addFeatures([feature2]);
    return feature2;
};

//添加标注点
BaseMap.prototype.ADDFeatureForPoint = function (X, Y, iconConfig = {}, props = {}) {
    let defaultSrc = "/MainMintor/App_Themes/images/MeterInfo.png";
    let defaultColor = '#fff';
    var feature = new ol.Feature({
        geometry: new ol.geom.Point([X, Y]),
        name: props.name,
        properties: props
    });
    var style = new ol.style.Style({
        image: new ol.style.Icon(
            ({
                color: iconConfig.color || defaultColor,
                src: iconConfig.url || defaultSrc,
                scale: 0.5
            })
        )
    });
    feature.setId(props.id);
    feature.setStyle(style);
    this.VectorSource.addFeatures([feature]);
};

//选中多边形定位
BaseMap.prototype.fitDMALayerExtentByfqId = function (AreaID) {
    var feature = this.DMASource.getFeatureById(AreaID);
    var extent = feature.getGeometry().getExtent();
    this.map.getView().fit(extent, this.map.getSize());
    return feature;
};

BaseMap.prototype.fitToFeature = function (feature) {
    this.map.getView().fit(feature.getGeometry())
    return feature;
};

//根据分区编号设定分区颜色
BaseMap.prototype.SetFeatureColor = function (
    AreaID,
    FillColor,
    StorkeColor,
    TM,
    StyleWidth
) {
    var feature = this.DMASource.getFeatureById(AreaID); //获取当前Feature
    if (!StyleWidth) {
        StyleWidth = 2;
    }
    if (!FillColor) {
        FillColor = "rgba(0,0,0,0)";
    }
    if (!StorkeColor) {
        StorkeColor = "rgb(153, 0, 51)";
    }
    FillColor = this.colorRgb(FillColor, TM); //将16进制转为rgba的颜色格式
    //为Feature填充颜色
    var featureStyle = new ol.style.Style({
        fill: new ol.style.Fill({
            color: FillColor
        }),
        stroke: new ol.style.Stroke({
            color: StorkeColor,
            width: StyleWidth
        }),
        text: this.createTextStyle(feature, feature.getGeometryName())
    });
    /**************色值赋值结束***************/
    feature.setStyle(featureStyle); //为Feature赋值
};

//判断当前Feature是否存在
BaseMap.prototype.ISExistFeature = function (AreaID) {
    var feature = this.VectorSource.getFeatureById(AreaID); //获取当前Feature
    if (feature) {
        return true;
    }
    return false;
};

// 用在DMA选中分区节点时
BaseMap.prototype.SetDrawCurrentFeatureColor = function (
    FillColor,
    StorkeColor,
    TM,
    StyleWidth,
    _text
) {
    var feature = this.CurrentFeature; //获取当前Feature当前绘制的Feature
    if (!StyleWidth) {
        StyleWidth = 2;
    }
    if (!FillColor) {
        FillColor = "rgba(0,0,0,0)";
    }
    if (!StorkeColor) {
        StorkeColor = "rgb(153, 0, 51)";
    }
    FillColor = this.colorRgb(FillColor, TM); //将16进制转为rgba的颜色格式
    //为Feature填充颜色
    var featureStyle = new ol.style.Style({
        fill: new ol.style.Fill({
            color: FillColor
        }),
        stroke: new ol.style.Stroke({
            color: StorkeColor,
            width: StyleWidth
        }),
        text: this.createTextStyle(feature, _text)
    });
    /**************色值赋值结束***************/
    feature.setStyle(featureStyle); //为Feature赋值
};

//16进制转RGB
BaseMap.prototype.colorRgb = function (sColor, TM) {
    var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/;
    if (!TM) {
        TM = "0.5";
    }
    TM = "0.4";
    var sColor = sColor.toLowerCase();
    if (sColor && reg.test(sColor)) {
        if (sColor.length === 4) {
            var sColorNew = "#";
            for (var i = 1; i < 4; i += 1) {
                sColorNew += sColor
                    .slice(i, i + 1)
                    .concat(sColor.slice(i, i + 1));
            }
            sColor = sColorNew;
        }
        //处理六位的颜色值
        var sColorChange = [];
        for (var i = 1; i < 7; i += 2) {
            sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
        }
        return "rgba(" + sColorChange.join(",") + "," + TM + ")";
    } else {
        return sColor;
    }
};

//设置地图坐标中心点
BaseMap.prototype.setCenter = function (X, Y) {
    //取得View视图
    var CurrentView = this.map.getView();
    //定位
    CurrentView.setCenter([parseFloat(X), parseFloat(Y)]);
    /*定义缩放级别*/
    //var zoom = this.map.view.getZoom();
    //this.map.view.setZoom(zoom);
    //MapInfo.map.view.setCenter([X, Y]);
};

// 设置缩放
BaseMap.prototype.setZoom = function (zoom) {
    // 取得View视图
    var CurrentView = this.map.getView();
    // 设置缩放
    CurrentView.setZoom(zoom);
};

//注册地图点击事件
BaseMap.prototype.ReigsterEvent = function (mapClick) {
    this.map.on("singleclick", mapClick);
};

//========================

/**
 * 创建文本样式函数
 * @param {ol.Feature} feature 要素
 * @param  dom 要素样式html对象
 */
BaseMap.prototype.createTextStyle = function (feature, _Text) {
    //return;
    //读取当前面板设置的样式值
    var align = "center"; //文本位置
    var baseline = "middle"; //基准线
    var size = "20px"; //字体大小
    var offsetX = parseInt(0, 10); //偏移量X
    var offsetY = parseInt(-100, 10); //偏移量Y
    var weight = "normal"; //字体粗细
    var rotation = parseFloat(0); //角度
    var font = weight + " " + size + "  Arial"; //文字样式（粗细、大小、字体）
    var fillColor = "#000000"; //字体颜色
    var outlineColor = "#FFFFFF"; //外框颜色
    var outlineWidth = "4"; //外框宽度

    //返回实例化的文本样式对象（ol.style.Text）
    return new ol.style.Text({
        textAlign: align, //位置
        offsetX: offsetX, //偏移量X
        offsetY: offsetY, //偏移量Y
        rotation: rotation, //角度
        textBaseline: baseline, //基准线
        font: font, //文字样式
        text: _Text, //文本内容
        fill: new ol.style.Fill({
            color: fillColor
        }), //文本填充样式（即文字颜色）
        stroke: new ol.style.Stroke({
            color: outlineColor,
            width: outlineWidth
        }) //文本外框样式（颜色与宽度）
    });
};

// 测量距离与面积 (参数action为true时开启，false时关闭)
BaseMap.prototype.enableMeasure = function (measureType, action = true) {
    if (this.measureTooltip) {
        this.map.removeOverlay(this.measureTooltip)
        this.measureTooltip = null;
        this.measureTooltipElement = null;
    }
    if (this.measureDraw) {
        this.map.removeInteraction(this.measureDraw); //移除绘制图形
        this.measureDraw = null
    }
    if (this.measureSource) {
        this.measureSource.clear()
        this.measureSource = null;
    }
    // 去掉原来的测绘图层
    if (this.measureVectorLayer) {
        this.map.removeLayer(this.measureVectorLayer)
        this.measureVectorLayer = null
    }
    if (!action) {
        return
    }
    //加载测量的绘制矢量层
    this.measureSource = new ol.source.Vector(); //图层数据源
    this.measureVectorLayer = new ol.layer.Vector({
        source: this.measureSource,
        style: new ol.style.Style({ //图层样式
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 255, 0.2)' //填充颜色
            }),
            stroke: new ol.style.Stroke({
                color: '#ffcc33', //边框颜色
                width: 2 // 边框宽度
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: '#ffcc33'
                })
            })
        })
    });
    this.map.addLayer(this.measureVectorLayer);
    // 定义一个球对象，在计算距离与面积值的时候做为参数传入
    let wgs84Sphere = new ol.Sphere(6378137);
    /**
     * 加载交互绘制控件函数 
     */

    let type = (measureType === 'area' ? 'Polygon' : 'LineString');
    this.measureDraw = new ol.interaction.Draw({
        source: this.source, //测量绘制层数据源
        type: type, //几何图形类型
        style: new ol.style.Style({ //绘制几何图形的样式
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 255, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.5)',
                lineDash: [10, 10],
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 5,
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.7)'
                }),
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.2)'
                })
            })
        })
    });
    this.map.addInteraction(this.measureDraw);


    createMeasureTooltip.call(this);

    let sketchGeoChangelistener = '';
    //绑定交互绘制工具开始绘制的事件
    this.measureDraw.on('drawstart', function (evt) {

        this.sketch = evt.feature; //绘制的要素
        let tooltipCoord = evt.coordinate; // 绘制的坐标
        console.log('触发 drawstart 事件 ', tooltipCoord)
        this.lastTooltipCoord = tooltipCoord
        //绑定change事件，根据绘制几何类型得到测量长度值或面积值，并将其设置到测量工具提示框中显示
        sketchGeoChangelistener = this.sketch.getGeometry().on('change', function (evt) {
            let geom = evt.target; //绘制几何要素
            let output = 0;
            if (geom instanceof ol.geom.Polygon) {
                output = formatArea.call(this, geom, wgs84Sphere); //面积值
                tooltipCoord = geom.getInteriorPoint().getCoordinates(); //坐标
            } else if (geom instanceof ol.geom.LineString) {
                output = formatLength.call(this, geom, wgs84Sphere); //长度值
                tooltipCoord = geom.getLastCoordinate(); //坐标
            }
            this.lastTooltipCoord = tooltipCoord

            this.measureTooltipElement.innerHTML = output; //将测量值设置到测量工具提示框中显示
            this.measureTooltip.setPosition(tooltipCoord); //设置测量工具提示框的显示位置
        }.bind(this));
    }.bind(this), this);
    //绑定交互绘制工具结束绘制的事件
    this.measureDraw.on('drawend', function (evt) {
        console.log('触发 drawend ')
        this.measureTooltipElement.className = 'tooltip tooltip-static'; //设置测量提示框的样式
        this.measureTooltip.setOffset([0, -7]);
        // this.measureTooltipElement = null; //置空测量工具提示框对象
        //重新创建一个测试工具提示框显示结果， 这里注释掉，移动端不需要双击确定结果了
        createMeasureTooltip.call(this);
        // this.measureTooltip.setPosition(this.lastTooltipCoord);
        ol.Observable.unByKey(sketchGeoChangelistener);
    }.bind(this), this);


}


/**
 *创建一个新的测量工具提示框（tooltip）
 */
function createMeasureTooltip() {
    console.log('创建Message Tool')
    if (this.measureTooltip) {
        this.measureTooltip = null
        this.map.removeOverlay(this.measureTooltip)
    }
    if (this.measureTooltipElement) {
        this.measureTooltipElement.parentNode.removeChild(this.measureTooltipElement);
        this.measureTooltipElement = null;
    }
    this.measureTooltipElement = document.createElement('div');
    this.measureTooltipElement.className = 'tooltip tooltip-measure';
    this.measureTooltip = new ol.Overlay({
        element: this.measureTooltipElement,
        offset: [0, -15],
        positioning: 'bottom-center'
    });
    this.map.addOverlay(this.measureTooltip);
}
/**
 * 测量长度输出
 */
function formatLength(line, wgs84Sphere) {
    var length;
    var coordinates = line.getCoordinates(); //解析线的坐标
    length = 0;
    var sourceProj = this.map.getView().getProjection(); //地图数据源投影坐标系
    //通过遍历坐标计算两点之前距离，进而得到整条线的长度
    for (var i = 0, ii = coordinates.length - 1; i < ii; ++i) {
        var c1 = ol.proj.transform(coordinates[i], sourceProj, 'EPSG:4326');
        var c2 = ol.proj.transform(coordinates[i + 1], sourceProj, 'EPSG:4326');
        length += wgs84Sphere.haversineDistance(c1, c2);
    }
    var output;
    if (length > 100) {
        output = (Math.round(length / 1000 * 10000) / 10000) + ' ' + 'km'; //换算成KM单位
    } else {
        output = (Math.round(length * 10000) / 10000) + ' ' + 'm'; //m为单位
    }
    return output; //返回线的长度
};
/**
 * 测量面积输出
 */
function formatArea(polygon, wgs84Sphere) {
    var area;
    var sourceProj = this.map.getView().getProjection(); //地图数据源投影坐标系
    var geom = (polygon.clone().transform(sourceProj, 'EPSG:4326')); //将多边形要素坐标系投影为EPSG:4326
    var coordinates = geom.getLinearRing(0).getCoordinates(); //解析多边形的坐标值
    area = Math.abs(wgs84Sphere.geodesicArea(coordinates)); //获取面积
    var output;
    if (area > 10000) {
        output = (Math.round(area / 1000000 * 10000) / 10000) + ' ' + 'km<sup>2</sup>'; //换算成KM单位
    } else {
        output = (Math.round(area * 10000) / 10000) + ' ' + 'm<sup>2</sup>'; //m为单位
    }
    return output; //返回多边形的面积
};

export default BaseMap;
