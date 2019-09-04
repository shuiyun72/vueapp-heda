<template>
    <div class="point_detail_table_container" :style="{'top': top}">
        <header>
            <div class="header_left">{{pointType}}</div>
            <div class="header_right">
                <span class="expand_icon" @tap="isFullscreen=!isFullscreen">&harr;</span>
                <span class="close_icon" @tap="onCloseIconClick">X</span>
            </div>
        </header>
        <el-table
            :data="detailData"
            style="width: 100%;"
            :height="isFullscreen ? fullscreenHeight :  defaultHeight"
        >
            <el-table-column
                prop="propertyName"
                label="属性"
                align="left"
            ></el-table-column>
            <el-table-column
                prop="propertyValue"
                label="数值"
                align="left"
            ></el-table-column>
        </el-table>
    </div>
</template>
<script>
import $ from "jquery";
export default {
  props: {
    detailData: {
      type: Array,
      default: []
    }
  },
  mounted() {
    // 根据不同的设备计算表格的默认高度与全屏高度
    this.defaultHeight = $(window).height() * 0.43 - 62 - 30;
    this.fullscreenHeight =
      $(window).height() -
      $("header.mui-bar").height() -
      $(".gis_action_bar_container").height() - 30;

    this.fullscreenTop = $("header.mui-bar").height() + "px";
  },
  computed: {
    top() {
      return this.isFullscreen ? this.fullscreenTop : this.defaultTop;
    },
    pointType() {
      let obj = _.find(this.detailData, { propertyName: "设备类型" });
      return obj ? obj.propertyValue : "经纬度";
    }
  },
  data() {
    return {
      defaultTop: "57vh",
      fullscreenTop: 0,

      defaultHeight: 200,
      fullscreenHeight: 0,

      // 当前表格是否是展开状态
      isFullscreen: false,
    };
  },
  methods: {
    onCloseIconClick() {
      this.$emit("close");
    }
  }
};
</script>

<style lang="less">
.point_detail_table_container {
  position: fixed;
  bottom: 61px;
  left: 0;
  right: 0;
  transition: 0.5s all;
  overflow: hidden;
  .el-table {
    transition: 0.5s height;
  }
  header {
    height: 30px;
    width: 100%;
    padding: 2%;
    display: flex;
    justify-content: space-between;
    align-items: center;
    background: #aaa;
    color: white;
    .header_right {
      display: inline-flex;
      width: 15%;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>


