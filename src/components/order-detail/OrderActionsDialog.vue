<template>
  <div class="order_actions_dialog_container">
    <!-- 日期选择器 -->
    <div class="date_row mui-table-view-cell" v-if="datePickerEnabled" @tap="onDateRowClick">
      <span class="label gray">延期时间：</span>
      <span class="content">{{pickedDate}}</span>
      <span class="icon el-icon el-icon-date"></span>
    </div>
    <div class="desc_selector" v-if="descSelectorEnabled">
      <el-select v-model="descSelectorValue" placeholder="请选择描述">
        <el-option
          v-for="item in descSelectorOptions"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        ></el-option>
      </el-select>
    </div>
    <!-- 多行文本框（描述信息） -->
    <div class="desc_textarea" v-if="textareaEnabled">
      <el-input
        type="textarea"
        :autosize="{ minRows: 4, maxRows: 10}"
        placeholder="请输入描述信息"
        v-model="textareaValue"
      ></el-input>
    </div>
    <!-- 拍照上传 -->
    <div class="picture_uploader" v-if="pictureUploaderEnabled">
      <PictureUploader
        :uploadLimit="4"
        enableButton
        @change="onPictureUploaderChange"
        ref="pictureUploader"
      ></PictureUploader>
    </div>
    <!-- 语音上传 -->
    <div class="speech_uploader" v-if="speechEnabled">
      <el-button @click="onSpeechClick">开始录音</el-button>
    </div>
  </div>
</template>

<script>
/********************************************
 * 维修工单详情页中
 * 五种订单操作的公共弹出框内容
 * 包括五种组件：日期选择器、多行文本框、拍照上传、语音上传、 下拉菜单（预定义的描述信息）
 * 根据actionType选择性加载相应组件
 ***********************************************/
import _ from "lodash";
import dateHelper from "@common/dateHelper";
import {
  setSessionItem,
  getSessionItem,
  getLocalItem,
  setLocalItem
} from "@common/util";
import PictureUploader from "@comp/common/PictureUploaderPlus.vue";
import encodeHelper from "@common/encodeHelper";
import apiMonitor from "@api/monitor";
export default {
  props: {
    /**************
     * @actionType
     * 1 => 退单
     * 2 => 延期
     * 3 => 到场
     * 4 => 维修
     * 5 => 完工
     *************/
    actionType: {
      type: [Number, String],
      required: true
    }
  },
  created() {},
  mounted() {
    // 实例化datePicker组件
    this._datePicker = new window.mui.DtPicker({
      type: "datetime",
      // beginDate: new Date(2015, 4, 25),
      // endDate: new Date(2018, 11, 25),
      labels: ["年", "月", "日", "时", "分"]
    });
    this.fetchDescSelectorOptions();
  },
  data() {
    return {
      // 所选日期与时间
      pickedDate: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
      // 多行文本框的输入内容
      textareaValue: "",
      // 当前拍摄的图片列表
      pictureList: [],
      speechData: "",
      descSelectorOptions: [],
      descSelectorValue: ""
    };
  },
  computed: {
    datePickerEnabled() {
      // 延期
      return this.actionType == 2;
    },
    textareaEnabled() {
      // 全部
      return true;
    },
    pictureUploaderEnabled() {
      // 到场、维修、完工
      return [3, 4, 5].includes(this.actionType);
    },
    speechEnabled() {
      // 到场、维修、完工
      // return [3, 4, 5].includes(this.actionType);
      // 暂时把录音功能隐藏掉
      return false;
    },
    descSelectorEnabled() {
      // 退单、到场
      return [1, 3].includes(this.actionType);
    }
  },
  methods: {
    fetchDescSelectorOptions() {
      let type = "";
      if (this.actionType == 1) {
        // 退单Options
        type = "charge-back";
      } else if (this.actionType == 3) {
        // 到场Options
        type = "arrive";
      } else {
        return;
      }
      apiMonitor
        .GetOrderSelectorOptions(type)
        .then(res => {
          console.log(res.data);
          this.descSelectorOptions = JSON.parse(res.data);
        })
        .catch("获取描述下拉菜单值失败！");
    },
    onSpeechClick() {
      window.mui.plusReady(() => {
        console.log("plus ready!");
        try {
          // 获取录音对象
          let recorder = window.plus.audio.getRecorder();
          console.log("获取录音对象成功", recorder);
          recorder.record(
            { filename: "_doc/audio/" },
            recordFile => {
              console.log("record success!", recordFile);
            },
            err => {
              console.log("record err ", err);
            }
          );
          setTimeout(() => {
            console.log("stop");
            recorder.stop();
          }, 5000);
        } catch (error) {
          console.log("try catch err!", err);
        }
      });
    },

    onDateRowClick() {
      // 日期选择
      this._datePicker.show(result => {
        this.pickedDate = dateHelper.format(
          new Date(result.value),
          "yyyy-MM-dd hh:mm:ss"
        );
      });
    },
    onPictureUploaderChange(pictureList) {
      this.pictureList = pictureList;
    },
    // 清空当前所有输入值
    reset() {
      this.pickedDate = "";
      this.textareaValue = "";
      if (this.descSelectorEnabled) {
        this.descSelectorValue = "";
      }
      if (this.pictureUploaderEnabled) {
        this.$refs.pictureUploader.reset();
      }
      console.log("reset");
    },
    // 获取当前数据
    getValue() {
      return {
        date: this.pickedDate,
        description: this.textareaValue,
        descOption: this.descSelectorValue
          ? _.find(this.descSelectorOptions, { value: this.descSelectorValue })
          : undefined,
        pictureList: this.pictureList.map(picture => {
          return encodeHelper.formatBase64(picture.base64);
        }),
        speechData: this.speechData
      };
    }
  },
  watch: {
    actionType() {
      this.fetchDescSelectorOptions();
    }
  },
  components: {
    PictureUploader
  }
};
</script>

<style lang="less">
.order_actions_dialog_container {
  /* 避免el-dialog组件高度太高而导致操作不便 */
  max-height: 50vh;
  overflow: scroll;
  .date_row {
    // 覆盖.mui-table-view-cell默认padding
    padding-left: 15px;
    padding-right: 0;
    .gray {
      color: #999;
    }
    & > * {
      display: inline-block;
      vertical-align: middle;
      &.label {
        max-width: 37%;
        padding-right: 2%;
      }
      &.content {
        /* 列表项最右侧的箭头是绝对定位，且字体大小为继承，此属性可避免内容与图标重叠*/
        margin-right: calc(~"1% + 1em");
        max-width: calc(~"100% - 32% - 1em");
        padding-left: 2%;
      }
      &.icon {
        position: absolute;
        top: 50%;
        right: 15px;
        font-size: inherit;
        color: #bbb;
        transform: translateY(-50%);
      }
    }
  }
  .desc_textarea {
    margin: 3% 15px;
  }
  .desc_selector {
    margin: 3% 15px;
  }
  .picture_uploader {
    margin: 3% 15px;
  }
  .speech_uploader {
    margin: 3% 15px;
  }
}
</style>


