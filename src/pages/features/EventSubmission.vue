<template>
    <div class="event_submission_container">
        <MuiList 
            :items="items" 
            @row-click="onRowClick" 
            v-loading.fullscreen="isLoading" 
            :element-loading-text="fullscreenLoadingText"
        >
            <span :slot="descSlotName">
                <el-input 
                    name="detailDescription"
                    type="textarea"
                    :autosize="{minRows: 5, maxRows: 7}"
                    v-model="detailDescription"
                    placeholder="请输入详细描述"
                    @focus="onDescInputFocus"
                    style="width: 120%;"
                ></el-input>
            </span>
        </MuiList>
        <div 
            style="width: 100%;"
            v-loading="pictureUploaderLoading"
            element-loading-text="正在生成预览..."
            element-loading-spinner="el-icon-loading"
            element-loading-background="rgba(0, 0, 0, 0.8)"
        >
            <PictureUploaderPlus 
                :uploadLimit="uploadLimit" 
                @change="onUploaderChange" 
                @captured="pictureUploaderLoading = true"
                ref="uploader"
                class="picture_uploader"
            ></PictureUploaderPlus>
            <div class="button_group_container">
                <el-button @click="onCameraButtonClick"
                    class="camerat_button custom_bgcolor_dark"><i class="fas fa-camera" style="margin-right: 5px;"></i>拍 &nbsp;&nbsp;&nbsp;&nbsp; 照</el-button>      
                <el-button @click="onSubmitButtonClick" :disabled="!allFieldsValid || !currentAddressText"
                    class="submit_button custom_bgcolor_light" icon="el-icon-upload">上 &nbsp;&nbsp;&nbsp;&nbsp; 传</el-button>
            </div>
        </div>
    </div>
</template>

<script>
import CoordsHelper from "coordtransform";
import MuiList from "@comp/common/MuiList.vue";
import PictureUploaderPlus from "@comp/common/PictureUploaderPlus.vue";
import dateHelper from "@common/dateHelper";
import encodeHelper from "@common/encodeHelper";
import apiInspection from "@api/inspection";
// lodash是一个js函数库
import _ from "lodash";
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem,
  deepCopy
} from "@common/util";
export default {
  props: {
    isTemp: {
      type: [Number, String],
      default: 1
    },
    deviceName: {
      type: String,
      default: ""
    },
    deviceSmid: {
      type: [Number, String],
      default: -1
    },
    pointType: {
      type: [Number, String],
      default: -1
    },
    taskId: {
      type: [Number, String],
      default: -1
    },
    taskName: {
      type: [Number, String],
      default: ""
    }
  },
  mounted() {
    // 实例化picker组件
    this._popPicker = new window.mui.PopPicker({ layer: 1 });
    let newTitle = this.isTemp == 1 ? "临时事件上报" : "点位事件上报";
    setTimeout(() => {
      this.$eventbus.$emit("set-title", newTitle);
    }, 1);
  },

  beforeRouteEnter(to, from, next) {
    // 每次路由切换到该页面时，开始定位，并计算当前时间
    next(instance => {
      // 动态生成list的动态配置
      if (instance.isTemp == 1) {
        instance.items = [
          {
            id: "date",
            label: "时间",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },
            content: ""
          },
          {
            id: "location",
            label: "事件地",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },
            content: "",
            placeholder: "定位中...",
            iconClass: "fas fa-map-marker-alt",
            iconStyle: "color: lightgreen"
          },
          {
            id: "eventType",
            label: "事件类型",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择事件类型",

            content: ""
          },
          {
            id: "eventContent",
            label: "事件内容",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择事件内容",

            content: ""
          },
          {
            id: "emergency",
            label: "紧急程度",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择紧急程度",

            content: ""
          },
          {
            id: "eventLevel",
            label: "处理级别",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择处理级别",

            content: ""
          },
          {
            id: "detailDescrition",
            label: "详细描述",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" }
          }
        ];
      } else if (instance.isTemp == 0) {
        instance.items = [
          {
            id: "deviceName",
            label: "设备名称",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            content: instance.deviceName
          },
          {
            id: "hiddenDanger",
            label: "是否有隐患",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            content: "有"
          },
          {
            id: "date",
            label: "时间",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            content: ""
          },
          {
            id: "location",
            label: "事件地",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },
            content: "",

            placeholder: "定位中...",

            iconClass: "fas fa-map-marker-alt",
            iconStyle: "color: lightgreen"
          },
          {
            id: "mission",
            label: "任务",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择任务计划",

            content: instance.taskName
          },
          {
            id: "eventType",
            label: "事件类型",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择事件类型",

            content: ""
          },
          {
            id: "eventContent",
            label: "事件内容",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择事件内容",

            content: ""
          },
          {
            id: "emergency",
            label: "紧急程度",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择紧急程度",

            content: ""
          },
          {
            id: "eventLevel",
            label: "处理级别",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" },

            placeholder: "请选择处理级别",

            content: ""
          },
          {
            id: "detailDescrition",
            label: "详细描述",
            labelStyle: { color: "#999", fontSize: "1.2rem", width: "25%" }
          }
        ];
      }
      // 定位
      window.mui.plusReady(() => {
        nativeTransfer.getLocation(location => {
          if(location){
            console.log("事件上报， location ", location);
            // 坐标转换
            let coordsFor84 = CoordsHelper.gcj02towgs84(
              location.lng,
              location.lat
            );
            //转换为地方坐标
            coordsFor84 = this.mapController.destinationCoordinateProj(
              coordsFor84
            );
            location.longitude = coordsFor84[0];
            location.latitude = coordsFor84[1];
            instance.locationInfo = deepCopy(location);
            // 从items配置项中找到date项
            let locationItem = _.find(instance.items, item => {
              return item.id === "location";
            });
            // 填入当前定位信息
            locationItem.content = instance.currentAddressText;
          }else{
            mui.toast("获取当前位置失败");
          }
        }/*,
          err => {},
          {
            enableHighAccuracy: true,
            maximumAge: 5000,
            timeout: 10000,
            provider: "baidu",
            coordsType: "gcj02"
          }*/
        );
      });
      // 如果是非临时，根据路由参数设置当前事件上报选择的任务
      if (instance.isTemp == 0) {
        instance.pickerValue.mission = {
          text: instance.taskName,
          value: instance.taskId
        };
      }

      // 计算当前时间
      let now = new Date();
      // 从items配置项中找到date项
      let dateItem = _.find(instance.items, item => {
        return item.id === "date";
      });
      // 填入当前时间
      dateItem.content = dateHelper.format(now, "yyyy-MM-dd hh:mm:ss");
    });
  },
  data() {
    return {
      isLoading: false,
      fullscreenLoadingText: "正在加载...",
      // 当前定位信息
      locationInfo: {},
      // 所有列表项的配置， 在每次进入当前路由（beforeRouteEnter）时动态初始化
      items: [],
      // 所有下拉菜单的值
      /* 
      @format 
        mission: {
          text: String, 后端返回的每一项的name 
          value: String, 后端返回的每一项的id
        }
      */
      pickerValue: {
        // 是否有隐患
        hasHiddenDanger: {
          text: "有",
          value: 1
        },
        // 任务列表
        mission:
          // 临时任务不用选任务，默认值如下
          this.isTemp == 1
            ? {
                text: "无",
                value: -1
              }
            : {},
        // 事件类型列表
        eventType: {},
        // 事件内容列表
        eventContent: {},
        // 紧急程度列表
        emergency: {},
        // 处理级别列表
        eventLevel: {}
      },
      // 双向绑定的描述信息
      detailDescription: "",
      // 是否有隐患
      // hasHiddenDanger: 0,
      // 上传图片的数量上限
      uploadLimit: 2,
      // 当前已选择的上传图片列表
      pictureList: [],
      pictureUploaderLoading: false
    };
  },
  computed: {
    // 当前的用户信息
    /*
    @format
      {
        PersonId: Int,
        DeptId: Int,
        PersonName: String,
        PassWord: String,
        DeptName: String,
        RoleName: String,
        Smid: String,
        IsEdit: Int,
        UpMeter: Int,
        iRoleID: Int,
        deviceInfo: {
          uuid: String, 设备唯一标识
          imei: String, 国际移动设备身份码
          imsi: String, 国际移动用户识别码
          model: String, 设备型号
          vendor: String, 设备厂商
        }
      }
    */
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前设备信息
    currentDevice() {
      return this.currentUser.deviceInfo;
    },
    currentAddressText() {
      let addr = this.locationInfo.address;
      return addr instanceof Object
        ? addr.poiName || this.locationInfo.addresses || ""
        : "";
    },
    // 描述信息的slot name
    descSlotName() {
      return (
        "item_" +
        (_.findIndex(this.items, item => {
          return item.id === "detailDescrition";
        }) +
          1)
      );
    },
    // 隐患单选按钮的slot name
    hiddenDangerSlotName() {
      return (
        "item_" +
        (_.findIndex(this.items, item => {
          return item.id === "hiddenDanger";
        }) +
          1)
      );
    },
    allFieldsValid() {
      return _.every(this.pickerValue, item => {
        return item.value !== undefined;
      });
    },
    pictureListStr() {
      if (this.pictureList && this.pictureList.length > 0) {
        return this.pictureList
          .map(obj => {
            return encodeHelper.formatBase64(obj.base64);
          })
          .join("$");
      } else {
        return "";
      }
    },
    eventSubmissionData() {
      return {
        X: this.locationInfo.longitude.toString(),
        Y: this.locationInfo.latitude.toString(),
        Longitude: this.locationInfo.longitude.toString(),
        Latitude: this.locationInfo.latitude.toString(),
        PersonId: this.currentUser.PersonId,
        DeptId: this.currentUser.DeptId,
        // 只能上传一张图片
        // 格式化原始的base64字符串为后端能接受的base64格式
        Base64Image: this.pictureListStr,
        // 暂时关闭了麦克风录音功能  by: 2018/06/24
        Base64Voice: "",
        EventId: this.pickerValue.eventType.value,
        EventContentId: this.pickerValue.eventContent.value,
        EventAddress: this.currentAddressText,
        Description: this.detailDescription,
        TaskId: this.pickerValue.mission.value || -1,
        MUngercyId: this.pickerValue.emergency.value,
        MLevelId: this.pickerValue.eventLevel.value,
        Uptime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),

        // 以下四个字段值来自非临时事件路由参数， 且有默认值
        Devicename: this.deviceName,
        Devicesmid: this.deviceSmid,
        PointType: this.pointType,
        // 是否是临时事件
        IsTemp: this.isTemp,

        // 来自非临时事件的表单中的单选按钮， 默认值为1
        IsHidden: this.pickerValue.hasHiddenDanger.value
        // IsHidden: 1
      };
    }
  },
  methods: {
    // 列表组件的某一行被点击时
    onRowClick(row, rowIndex) {
      if (
        row.id !== "location" &&
        row.id !== "detailDescrition" &&
        row.id !== "date" &&
        row.id !== "deviceName" &&
        row.id !== "hiddenDanger" &&
        this.isTemp == 1
      ) {
        this.isLoading = true;
        this.fullscreenLoadingText = `正在加载${row.label}数据`;
      }
      switch (row.id) {
        case "hiddenDanger":
          console.log("enter case");
          //  打开是否有隐患选择器
          this.openPicker(
            [{ text: "有", value: 1 }, { text: "没有", value: 0 }],
            row,
            rowIndex,
            {
              valueKey: "value",
              textKey: "text"
            }
          );
          break;
        case "mission":
          // 非临时上报时，任务名称从路由参数获得
          if (this.isTemp == 1) {
            // 请求任务列表
            let currentUserId = this.currentUser.PersonId;
            let currentDayDate = dateHelper.format(new Date(), "yy-MM-dd");
            apiInspection
              .GetMissionList(currentUserId, currentDayDate)
              .then(res => {
                if (res.data.result === true) {
                  let listData = deepCopy(res.data.Data);
                  listData.unshift({
                    TaskId: 0,
                    TaskName: "无"
                  });
                  console.log("任务列表·····", listData);
                  this.openPicker(listData, row, rowIndex, {
                    valueKey: "TaskId",
                    textKey: "TaskName"
                  });
                } else {
                  if (res.data.Data.length == 0) {
                    let listData = [
                      {
                        TaskId: 0,
                        TaskName: "无"
                      }
                    ];
                    this.openPicker(listData, row, rowIndex, {
                      valueKey: "TaskId",
                      textKey: "TaskName"
                    });
                  }
                  // console.log('C1')
                  // this.isLoading = false;
                  // mui.toast("网络请求超时，请稍后重试");
                }
              })
              .catch(err => {
                this.isLoading = false;
                mui.toast("网络请求超时，请稍后重试");
              });
          }
          break;
        case "eventType":
          // 请求事件类型列表
          apiInspection
            .GetEventTypeList()
            .then(res => {
              if (res.data.result === true) {
                let listData = res.data.Data;
                this.openPicker(listData, row, rowIndex, {
                  valueKey: "EventTypeId",
                  textKey: "EventTypeName"
                });
              } else {
                this.isLoading = false;
                mui.toast("网络请求超时，请稍后重试");
              }
            })
            .catch(err => {
              this.isLoading = false;
              mui.toast("网络请求超时，请稍后重试");
            });
          break;
        case "eventContent":
          // 当前已选择的事件类型的id
          let currentPickedEventTypeId = this.pickerValue.eventType.value;
          if (!currentPickedEventTypeId) {
            this.isLoading = false;
            window.mui.toast("请先选择事件类型");
          } else {
            // 请求事件内容列表
            apiInspection
              .GetEventContentList(currentPickedEventTypeId)
              .then(res => {
                if (res.data.result === true) {
                  let listData = res.data.Data;
                  this.openPicker(listData, row, rowIndex, {
                    valueKey: "EventTypeId",
                    textKey: "EventTypeName"
                  });
                } else {
                  this.isLoading = false;
                  mui.toast("网络请求超时，请稍后重试");
                }
              })
              .catch(err => {
                this.isLoading = false;
                mui.toast("网络请求超时，请稍后重试");
              });
          }

          break;
        case "emergency":
          // 请求紧急程度列表
          apiInspection
            .GetEmergencyList()
            .then(res => {
              if (res.data.result === true) {
                let listData = res.data.Data;
                this.openPicker(listData, row, rowIndex, {
                  valueKey: "UrgencyId",
                  textKey: "UrgencyName"
                });
              } else {
                this.isLoading = false;
                mui.toast("网络请求超时，请稍后重试");
              }
            })
            .catch(err => {
              this.isLoading = false;
              mui.toast("网络请求超时，请稍后重试");
            });
          break;
        case "eventLevel":
          // 请求处理级别列表
          apiInspection
            .GetEventLevelList()
            .then(res => {
              if (res.data.result === true) {
                let listData = res.data.Data;
                this.openPicker(listData, row, rowIndex, {
                  valueKey: "HandlerLevelId",
                  textKey: "HandlerLevelName"
                });
              } else {
                this.isLoading = false;
                mui.toast("网络请求超时，请稍后重试");
              }
            })
            .catch(err => {
              this.isLoading = false;
              mui.toast("网络请求超时，请稍后重试");
            });
          break;
      }
    },
    // 拿到列表数据后打开picker，并处理后续选择逻辑
    openPicker(listData, row, rowIndex, keys) {
      this.isLoading = false;
      let pickerListItems = listData.map(item => {
        return { value: item[keys.valueKey], text: item[keys.textKey] };
      });
      this._popPicker.setData(pickerListItems);
      this._popPicker.show(pickedItems => {
        let pickedItem = pickedItems[0];
        /* 
          如果当前打开的是事件类型的picker
          则每次选择后判断本次是否与之前选择的事件类型一致
          若发生改变，则应清空下方事件内容项的内容
        */
        if (row.id === "eventType") {
          if (pickedItem.value !== this.pickerValue.eventType.value) {
            _.find(this.items, item => {
              return item.id === "eventContent";
            }).content =
              "";
          }
        }
        this.pickerValue[row.id] = pickedItem;
        this.items[rowIndex].content = pickedItem.text;
      });
    },
    // 情况描述输入框聚焦时
    onDescInputFocus(event) {
      let targetDomElement = event.target;
      // 调整详情输入框的位置，由于手机虚拟键盘弹出会挤压原页面高度
      targetDomElement.scrollIntoView();
      // 某些手机键盘弹出有延迟
      setTimeout(() => {
        targetDomElement.scrollIntoView();
      }, 400);
    },
    // 上传组件状态改变时
    onUploaderChange(pictureList) {
      // 获取最新的list，并赋值给当前上下文的pictureList
      this.pictureList = pictureList;
      this.pictureUploaderLoading = false;
      console.log("最新的当前图片列表 ", this.pictureList);
    },
    // 点击拍照按钮
    onCameraButtonClick() {
      this.$refs.uploader.openCamera();
    },
    // 点击上传按钮，上传全部事件数据
    onSubmitButtonClick() {
      this.isLoading = true;
      this.fullscreenLoadingText = "正在上报事件，请耐心等待...";
      console.log("event submit data!", this.eventSubmissionData);
      apiInspection
        .SubmitEvent(this.eventSubmissionData)
        .then(res => {
          this.isLoading = false;
          console.log("submit res", res);
          if (res.data.result === true) {
            this.$router.go(-1);
            mui.toast("事件上报成功！");
          } else {
            mui.toast(`事件上报失败！`);
          }
        })
        .catch(e => {
          this.isLoading = false;
          console.log("%c上报失败", "color: red", e);
          mui.toast(`事件上报失败！`);
        });
    }
  },
  components: {
    MuiList,
    PictureUploaderPlus
  }
};
</script>

<style lang="less">
.event_submission_container {
  width: 100%;
  font-size: 1.3rem;
  /* el-inpit与el-textarea都有默认的下边距，在列表中效果不好，所以清除掉 */
  // .el-select {
  //   input {
  //     margin-bottom: 0px;
  //   }
  // }
  .picture_uploader {
    width: 100%;
    min-height: 20vw;
  }
  .el-textarea {
    textarea {
      margin-bottom: 0px;
    }
  }
  .button_group_container {
    text-align: center;
    margin: 1vh auto;
    button {
      width: 40%;
      font-size: 1.3rem;
    }
  }
}
</style>


