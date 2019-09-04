<template>
  <div class="attendance_container mui-content">
    <div class="mui-row currnet_date_container">
      <i class="el-icon el-icon-date"></i>
      <!-- 由于变量中用到了&nbsp， 所以此处需采用html模式渲染 -->
      <span v-html="formattedCurrentDate"></span>
    </div>
    <!-- 打卡与定位组件 -->
    <div class="sign_in_container">
      <!-- 定位信息 -->
      <div class="locator_container">
        <i class="fas fa-map-marker-alt fa-2x"></i>
        <span class="position_label">&nbsp;&nbsp;地 址：&nbsp;&nbsp;</span>
        <span class="position_text">{{currentAddressText}}</span>
      </div>
      <!-- 签到签退按钮 -->
      <div class="sign_in_button_container">
        <button
          class="button button-glow button-circle button-action button-jumbo sign_in_button"
          :disabled="!isSignInButtonEnabled"
          @tap="onSignInClick"
        >
          <i class="fas fa-map-marker-alt fa-2x"></i>
          <div>{{state == 0 ? '签 到': (state == 1 ? '签 退' : '已签退')}}</div>
        </button>
      </div>
    </div>
    <!-- 日期检索 -->
    <div
      class="mui-table-view-cell date_selector"
      @tap="onDateSelectorClick"
    >当前选择月份： {{ selectedDate }}</div>
    <div class="table_container">
      <el-table
        :data="tableData"
        height="40vh"
        border
        stripe
        header-row-class-name="table_header_rows"
        :default-sort="{prop: 'date', order: 'descending'}"
      >
        <el-table-column
          label="日期"
          prop="date"
          sortable
          align="center"
          :sort-orders="['ascending', 'descending', null]"
        >
          <template slot-scope="scope">
            <i class="el-icon-time"></i>
            <span style="margin-left: 10px">{{ scope.row.date }}</span>
          </template>
        </el-table-column>
        <el-table-column label="签到" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.startTime }}</span>
          </template>
        </el-table-column>
        <el-table-column label="签退" align="center">
          <template slot-scope="scope">
            <!-- 这里定义了最后一列的模板，包括展开图标 暂时关掉详情功能-->
            <span>{{ scope.row.endTime }}</span>
            <!-- <i class="el-icon-arrow-right expand_icon"
                            @tap="onExpandIconClick(scope.row.id)"
            ></i>-->
          </template>
        </el-table-column>
      </el-table>
    </div>
    <!-- 日期选择器弹出框 -->
    <el-dialog
      title="选择月份查询"
      center
      width="fit-content"
      custom-class="date_selector_dialog"
      :show-close="false"
      :close-on-click-modal="false"
      :visible.sync="dateSelectorDialogVisible"
    >
      <el-date-picker
        v-model="selectedDate"
        ref="datePicker"
        name="datePicker"
        type="month"
        placeholder="选择年月"
        format="yyyy 年 M 月"
        :value-format="dateValueFormat"
        @focus="onDateSelectorFocus"
      ></el-date-picker>
      <span slot="footer" class="dialog-footer">
        <el-button @click="onDateSelectorCancel">取 消</el-button>
        <el-button type="primary" @click="onDateSelectorConfirm">确 定</el-button>
      </span>
    </el-dialog>
    <!-- 签到记录详情弹出框 -->
    <!-- <el-dialog
            title="签到记录详情"
            center
            lock-scroll
            width="80%"
            custom-class="sign_in_detail_dialog"
            :show-close="true"
            :close-on-click-modal="false"
            :visible.sync="detailDialogVisible"
        >
            <SignInDetail :detailInfo="selectedRecordDetail"></SignInDetail>
    </el-dialog>-->
  </div>
</template>

<script>
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem,
  deepCopy
} from "@common/util";
import BaseMap from "@JS/Map/BaseMap";
import CoordsHelper from "coordtransform";
import Timer from "@common/timer";
import dateHelper from "@common/dateHelper";
import { calcDistance } from "@common/util";
import SignInDetail from "@comp/attendance/SignInDetail";
import apiInspection from "@api/inspection";

import PositionUploader from "@JS/position-uploader/PositionUploader";
import nativeTransfer from "@JS/native/nativeTransfer";

export default {
  created() {},
  beforeRouteEnter(to, from, next) {
    // 进入该路由的钩子函数，可通过next回调函数拿到组件的实例对象引用
    next(instance => {
      // 定制该路由下的设备返回键逻辑
      instance.$defineDeviceBack(defaultFunction => {
        if (
          instance.dateSelectorDialogVisible ||
          instance.detailDialogVisible
        ) {
          // 如果当前有弹出框，则返回键会关闭弹出框
          instance.dateSelectorDialogVisible = false;
          instance.detailDialogVisible = false;
        } else {
          // 如果当前没有弹出框，则使用默认返回逻辑
          defaultFunction();
        }
      });
      // 设置日期选择器的默认时间区间
      instance.selectedDate = dateHelper.format(
        instance.today,
        instance.dateValueFormat
      );
      instance.lastSelectedDate = instance.selectedDate;
      instance.fetchAttendanceRecords();  
    });
  },
  data() {
    return {
      // 当前定位信息
      locationInfo: {},
      // 今天的时间
      today: new Date(),
      // 当前日期选择器的值， 按照下方dateValueFormat格式
      selectedDate: "",
      // 日期选择器的js代码中的值的格式定义
      dateValueFormat: "yyyy-MM",
      // 记录未改变之前的时间，如果用户在日期选择时点击了取消，则还原到之前选择的时间
      lastSelectedDate: "",
      // 日期选择器弹出框是否可见
      dateSelectorDialogVisible: false,
      // 签到表格数据
      tableData: [],
      // 当前（今天）出勤状态 （0：为签到  1：已签到未签退   2：已签退）
      state: 0,
      //当前位置获取状态 0 为为获取 1为已获取
      TransferStatu:0
    };
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 格式化后的今天的日期
    formattedCurrentDate() {
      let weekday = dateHelper.getWeek(
        this.today,
        dateHelper.WEEKTYPE.ZH_DAYNAME
      );
      let date = dateHelper.format(this.today, "yyyy年M月d日");
      return `&nbsp;&nbsp;${weekday}&nbsp;&nbsp;${date}`;
    },
    // 格式化后的日期选择器中的当前选择年月
    formattedSelectedDate() {
      // @return {year: '2018', month: '1'}
      let [year, month] = this.selectedDate.split("-");
      return {
        year,
        month
      };
    },
    currentAddressText() {
      let addr = this.locationInfo;
      return addr instanceof Object
        ? addr.poiName || this.locationInfo.addr || "定位中..."
        : "获取位置信息失败";
    },
    // 签到按钮可用状态
    isSignInButtonEnabled() {
      console.log(this.locationInfo)
      return (
        this.locationInfo &&
        this.locationInfo.lng&&
        this.locationInfo.lat &&
        this.state < 2
      );
    },
    // 提交考勤的接口请求数据
    reqSubmitAttendance() {
      // 当前经纬数据
      let xy = "";
      if (
        this.locationInfo &&
        this.locationInfo.lng &&
        this.locationInfo.lat
      ) {
        xy = `${this.locationInfo.lng},${this.locationInfo.lat}`;
      }
      // 请求数据
      let data = {
        Lwr_PersonId: this.currentUser.PersonId,
        Lwr_BeiZhu: "",
        Lwr_GpsStatus: "",
        Lwr_MobileStatus: "",
        Lwr_Power: "",
        Lwr_XY: xy,
        DeptId: this.currentUser.DeptId
      };
      console.log("resAtt", data);
      return data;
    },
    // 获取考勤记录接口的请求数据
    reqGetAttendanceRecords() {
      return {
        PersonId: this.currentUser.PersonId,
        DateStartStr: this.selectedDate + "-01",
        DateEndStr: this.selectedDate + "-31"
      };
    }
  },
  mounted() {   
    //获取位置信息
    this.getLocation(); 
    console.log("11111+getLocation")
  },
  methods: {
    //获取位置信息
    getLocation(){
      console.log("diaoyon//获取位置信息")
      
      let position = JSON.parse(getSessionItem("coordsMsg"));
      if (position) {
        console.warn("=====考勤管理定位成功", position);
        // 坐标转换
        let coordsFor84 = CoordsHelper.gcj02towgs84(
          position.lng,
          position.lat
        );
        //初始化地图
        let mapController = new BaseMap();
        mapController.Init("event_map");
        //转换为地方坐标
        coordsFor84 = mapController.destinationCoordinateProj(coordsFor84);
        position.lng = coordsFor84[0];
        position.lat = coordsFor84[1];
        this.locationInfo = deepCopy(position); 
        this.TransferStatu  = 1;       
      } else {
        console.warn("=====考勤管理定位错误");
      }
      let _this = this; 
      let TransferTimer = setTimeout(function(){
        if(this.TransferStatu == 0){
          _this.getLocation();  
        }else{
          TransferTimer = null;
          clearTimeout(TransferTimer)
        }
      },2000)
    },
    // 获取考勤记录数据
    fetchAttendanceRecords() {
      // 发送请求
      apiInspection
        .GetAttendanceRecords(this.reqGetAttendanceRecords)
        .then(res => {
          if (res.data.result === true) {
            let records = res.data.Data;
            console.log("gaga", records);
            // 处理原数据，得到table接收的数据格式
            let mappedRecords = records.map(record => {
              return {
                date: record.Lwr_Date,
                startTime: record.Lwr_StartTime.split(" ")[1],
                endTime: record.Lwr_EndTime.split(" ")[1],
                personStatus: record.Lwr_PersonStatus,
                comments: record.Lwr_BeiZhu
              };
            });
            // 将数据写进table
            this.tableData = mappedRecords;
            // 计算当前的考勤状态
            if (
              dateHelper.format(new Date(), "yyyy-MM-dd") ===
              mappedRecords[0].date
            ) {
              if (mappedRecords[0].startTime === mappedRecords[0].endTime) {
                this.state = 1;
                window.SIGN_STATUS = 1;
              } else if (
                mappedRecords[0].startTime < mappedRecords[0].endTime
              ) {
                this.state = 2;
                window.SIGN_STATUS = 2;
              }
            } else {
              this.state = 0;
              window.SIGN_STATUS = 0;
            }
          } else {
            // mui.toast(res.data.message);
          }
        });
    },
    onDateSelectorClick() {
      this.dateSelectorDialogVisible = true;
    },
    onDateSelectorConfirm() {
      this.dateSelectorDialogVisible = false;
      this.lastSelectedDate = this.selectedDate;
      // 清除原表格数据
      this.tableData = [];
      this.fetchAttendanceRecords();
    },
    onDateSelectorCancel() {
      this.selectedDate = this.lastSelectedDate;
      this.dateSelectorDialogVisible = false;
    },
    onDateSelectorFocus() {
      // 用于解决手机键盘弹出问题
      let picker = document.querySelector('input[name="datePicker"]');
      picker.blur();
    },
    // 点击打卡按钮
    onSignInClick() {
      if (this.isSignInButtonEnabled) {
        // 每次点击打卡按钮时，表格需要展示当前自然月的签到记录。
        let currentMonth = dateHelper.format(this.today, this.dateValueFormat);
        // 判断当前日期选择器的值是否等于当前自然月，若不相等，则改变日期选择器的值为当前自然月
        if (this.selectedDate !== currentMonth) {
          this.selectedDate = currentMonth;
          this.lastSelectedDate = currentMonth;
        }
        apiInspection.SubmitAttendance(this.reqSubmitAttendance).then(res => {
          if (res.data.result === true) {
            mui.toast(res.data.message);
            // 启动/关闭上传位置任务
            if (this.state === 0) {
              // 开启
              console.log("签到中开启上传任务！");
              PositionUploader.start();
            } else if (this.state === 1) {
              // 关闭
              console.log("关闭");
              PositionUploader.stop();
            }
            // 刷新考勤记录表格 (由于上文的逻辑，签到后表格中始终显示当前自然月的考勤记录)
            this.fetchAttendanceRecords();
          }
        });
      } else if (this.state == 2) {
        mui.toast("今日无需重复签退！");
      } else {
        mui.toast("正在定位中，请稍后重试");
      }
    }
  },
  components: {
    SignInDetail
  }
};
</script>

<style lang="less">
.attendance_container {
  div.currnet_date_container {
    font-size: 1.4rem;
    padding: 3%;
    margin: 1vh auto;
    background-color: white;
    i {
      color: #0070c0;
    }
  }
  .sign_in_container {
    background-color: lightblue;
    padding: 5vw;
    .locator_container {
      .position_label {
        color: white;
        font-size: 1.3rem;
      }
      .position_text {
        color: black;
        font-size: 1.4rem;
      }
    }
    .sign_in_button_container {
      margin: 2vh 0;
      text-align: center;
      .sign_in_button {
        width: 35vw;
        height: 35vw;
        // mui默认的input行高
        line-height: 1.42em;
      }
    }
  }
  .date_selector {
    text-align: center;
    /* 会覆盖mui默认的鼠标点击样式 */
    background-color: #fff;
    color: #999;
    font-size: 1.2rem;
    margin: 1vh 0;
    &.mui-active {
      /* 这里是mui默认的鼠标点击样式， 但由于优先级的关系，上面的白色背景会覆盖默认的，所以这里重写一遍 */
      background-color: #eee;
    }
  }
  .date_selector_dialog {
    /* 日期选择器中的日历icon与清除icon的默认高度有些问题 */
    i.el-icon-date,
    i.el-icon-circle-close {
      height: 67%;
    }
  }
  .table_container {
    .table_header_rows th {
      background-color: #001d26;
      color: white;
      text-align: center;
      /* 覆盖elementui的默认12px */
      padding-top: 5px;
      padding-bottom: 5px;
    }
    .expand_icon {
      /* 相对于td */
      position: absolute;
      right: 10%;
      top: 37%;
    }
  }
}
</style>
