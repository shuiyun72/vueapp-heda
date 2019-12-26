<template>
  <div class="legend_table_container">
    <div class="legend_table_header">
      <span class="sys-icon-TuLi">&nbsp;</span>
      <span>图例</span>
    </div>
    <div class="legend_table_body">
      <table
        id="TuLi_Table"
        cellpadding="0"
        cellspacing="0"
        style="width: 100%; backgroundColor:#999;"
        class="TuLi_Table"
      >
        <tr>
          <td>
            <div style="height:4px; background-color:#0880d7;width:90%"></div>
          </td>
          <td>管网</td>
        </tr>
        <tr v-for="item in allLayers" :key="item.name">
          <!-- <td>
             <img
              class="TuLi_icon"
              alt
              height="30"
              :src="'./static/gis/Templates/Black/toolIcon/mapIcon/'+item.url"
            />
          </td> -->
          <td>
             <img
              class="TuLi_icon"
              alt
              height="30"
              :src="item.url"
            />
          </td> 
          <td>{{item.name}}</td>
        </tr>
      </table>
    </div>
    <div id="TuLi_DivBootom" class="LayerControDivBootom">
      <span class="sys-icon-close">&nbsp;</span>
      <span style="cursor: pointer;" @click="onCloseClick">&nbsp;关闭&nbsp;</span>
    </div>
  </div>
</template>

<script>
export default {
  props:{
    allLayers:{
      type:Array,
      default() {
        return{
          toolIcon: [
            { name: "转折点", url: "zhuanzhedian.png" },
            { name: "三通", url: "santong.png" },
            { name: "四通", url: "sitong.png" },
            { name: "盖堵", url: "gaidu.png" },
            { name: "出水口", url: "chushuikou.png" },
            { name: "非普查区", url: "feipuchaqu.png" },
            { name: "水表井", url: "shuibiaojing.png" },
            { name: "消防栓", url: "xiaofangshuan.png" },
            { name: "消防井", url: "xiaofangjing.png" },
            { name: "检修井", url: "jianxiujing.png" },
            { name: "排气井", url: "paiqijing.png" },
            { name: "泵站", url: "bengzhan.png" },
            { name: "变径", url: "bianjing.png" },
            { name: "变材", url: "biancai.png" }
          ]
        }
      }
    }
  },
  data() {
    return {
      toolIcon: [
        { name: "转折点", url: "zhuanzhedian.png" },
        { name: "三通", url: "santong.png" },
        { name: "四通", url: "sitong.png" },
        { name: "盖堵", url: "gaidu.png" },
        { name: "出水口", url: "chushuikou.png" },
        { name: "非普查区", url: "feipuchaqu.png" },
        { name: "水表井", url: "shuibiaojing.png" },
        { name: "消防栓", url: "xiaofangshuan.png" },
        { name: "消防井", url: "xiaofangjing.png" },
        { name: "检修井", url: "jianxiujing.png" },
        { name: "排气井", url: "paiqijing.png" },
        { name: "泵站", url: "bengzhan.png" },
        { name: "变径", url: "bianjing.png" },
        { name: "变材", url: "biancai.png" }
      ]
    };
  },
  methods: {
    onCloseClick() {
      this.$emit("close");
    }
  }
};
</script>

<style lang="less" scoped>
.legend_table_container {
  .legend_table_header {
    /* 直接复用PC端样式 */
    width: 233px;
    height: 35px;
    background-color: black;
    color: #fff;
    line-height: 35px;
    text-align: left;
  }
  .legend_table_body {
    /* 直接复用PC端样式 */
    width: 233px;
    height: 334px;
    position: relative;
    table tr td:nth-child(1) {
      width: 70px;
      vertical-align: middle;
      line-height: 35px;
      height: 35px;
      padding-left: 10px;
      padding-right: 10px;
    }
  }

  /* 一下两大块直接复用PC端样式 */
  /* 表格整体样式 */
  .TuLi_Table {
    border-left: 1px #000 solid;
    border-top: 1px #000 solid;
  }
  .TuLi_Table td {
    border-right: 1px #000 solid;
    border-bottom: 1px #000 solid;
    text-align: center;
    line-height: 32px;
    vertical-align: middle;
    height: 32px;
    font-size: 12pt;
  }
  .TuLi_icon {
    height: 20px;
    width: 20px;
  }

  .TuLi_Div {
    position: absolute;
    top: 42px;
    right: 42px;
    display: none;
  }
  /*图标 样式*/
  .sys-icon-close {
    background: url(/static/gis/Black/Images/icon/one_pipe_hover.png) no-repeat
      center center;
    background-size: 21px 21px;
    height: 25px;
    width: 25px;
    cursor: pointer;
  }
  .sys-icon-layer {
    background: url(/static/gis/Black/Images/tool/TuCeng.png) no-repeat center
      center;
    background-size: 100% 100%;
    line-height: 35px;
    height: 35px;
    width: 35px;
    cursor: pointer;
  }
  .sys-icon-TuLi {
    background: url(/static/gis/Black/Images/tool/TuLi.png) no-repeat center
      center;
    background-size: 100% 100%;
    line-height: 35px;
    height: 35px;
    width: 35px;
    cursor: pointer;
  }
}
</style>


