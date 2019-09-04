<template>
    <div class="process_monitor_container">
        <div class="picker_container">
          <el-input 
              :value="pickedGraphName"
              suffix-icon="el-icon-arrow-down" 
              readonly
              @tap.native="onSVGPickerClick"
          >
              <div slot="prepend" style="color: #001d26">当前选项：</div>
          </el-input>
        </div>
        <div class="svg_container">
            <embed 
                :src="pickedGraphUrl" 
                id="show" 
                type="image/svg+xml" 
                @load="onSVGLoad"
            />
        </div>
    </div>    
</template>

<script>
import _ from "lodash";
import $ from "jquery";
import NativePopPicker from "@comp/native/PopPicker";
import { setDataToSVG } from "@JS/PM/svg-operator";
import apiMonitor from "@api/monitor";

// SVG宽高比
const SVGWHRatio = 2.1628;
// 进入页面默认显示的svg图的url
const DefaultSVGUrl = "./static/svg/WaterFactoryGSXT.svg";
export default {
  mounted() {
    this.initGraphContainer();
    this.graphPicker = new NativePopPicker().setData(this.pickerDataOptions);
  },
  data() {
    return {
      // 选择器select的配置对象
      pickerDataOptions: [
        {
          text: "供水系统",
          value: "./static/svg/WaterFactoryGSXT.svg",
          dataTableId: "75,76,77,78"
        },
        {
          text: "奉化水厂",
          value: "./static/svg/WaterFactoryYQSC.svg",
          dataTableId: "75"
        },
        {
          text: "甘珠庙一期",
          value: "./static/svg/WaterFactoryMZYS.svg",
          dataTableId: "76"
        },
        {
          text: "甘珠庙原水",
          value: "./static/svg/WaterFactoryGZMYS.svg",
          dataTableId: "78"
        },
        {
          text: "甘珠庙调度",
          value: "./static/svg/WaterFactoryGZMDD.svg",
          dataTableId: "77"
        },
        {
          text: "空港调度",
          value: "./static/svg/WaterFactoryKGDD.svg",
          dataTableId: "77"
        }
      ],
      // 当前选择的SVG图的可访问url
      pickedGraphUrl: DefaultSVGUrl,
      // 当前选择的SVG图的名称
      pickedGraphName: "供水系统"
    };
  },
  computed: {
    // 获得当前选择图的dataTableId，作为api参数获取图的数据
    currentGraphDataTableId() {
      return _.find(this.pickerDataOptions, { value: this.pickedGraphUrl })
        .dataTableId;
    }
  },
  methods: {
    initGraphContainer() {
      this.$showLoading();
      const ClientHeight = document.body.clientHeight * 0.99 - 90;
      document.getElementsByTagName(
        "embed"
      )[0].style.cssText = `width:${ClientHeight *
        SVGWHRatio}px;height: ${ClientHeight}px;`;
    },
    onSVGLoad() {
      console.log("loadend", this.currentGraphDataTableId);
      apiMonitor.GetTableData(this.currentGraphDataTableId).then(res => {
        console.log("获取svg图数据成功： ", res);
        setDataToSVG(res.data.Data);
        this.$hideLoading();
      });
    },
    onSVGPickerClick() {
      this.graphPicker.show().then(pickedItems => {
        this.$showLoading();
        this.pickedGraphName = pickedItems[0].text;
        this.pickedGraphUrl = pickedItems[0].value;
      });
    }
  }
};
</script>

<style lang="less" scoped>
.process_monitor_container {
  .svg_container {
    overflow: scroll;
    width: 100%;
  }
}
</style>
