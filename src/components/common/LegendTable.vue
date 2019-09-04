<template>
    <div class="legend_table_container">
        <div class="legend_table_header">
            <span  class="sys-icon-TuLi">&nbsp;</span>
            <span>图例</span>
        </div>
        <div class="legend_table_body">
            <table id="TuLi_Table" cellpadding="0" cellspacing="0" style="width: 100%; backgroundColor:#999;" class="TuLi_Table" >
                <tr>
                    <td >
                        <div style="height:5px; background-color:#00ff00;border:2px black solid;width:90%"></div>
                    </td>
                    <td>&lt;=DN100</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#669933;border:2px black solid;width:90%"></div>
                    </td>
                    <td>DN100-200</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#0000ff;border:2px black solid;width:90%"></div>
                    </td>
                    <td>DN200-300</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#ff0000;border:2px black solid;width:90%"></div>
                    </td>
                    <td>DN300-400</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#cc0000;border:2px black solid;width:90%"></div>
                    </td>
                    <td>DN400-500</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#9900ff;border:2px black solid;width:90%"></div>
                    </td>
                    <td>DN500-600</td>
                </tr>
                <tr>
                    <td >
                        <div style="height:5px; background-color:#663399;border:2px black solid;width:90%"></div>
                    </td>
                    <td>>DN600</td>
                </tr>
                <tr>
                    <td><img class="TuLi_icon" alt="" height="30" :src="'./static/gis/Templates/Black/Images/icon/famenbi.png'" /></td>
                    <td>阀门闭</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/famenkai.png'"/></td>
                    <td>阀门开</td>
                </tr>
                <tr>
                    <td>
                    <img  class="TuLi_icon" alt="" :src="'./static/gis/Templates/Black/Images/icon/famenjing.png'"/></td>
                    <td>阀门井</td>
                </tr>
                <tr>
                    <td>
                    <img  class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/jiayazhan.png'"/></td>
                    <td>加压站</td>
                </tr>
                <tr>
                    <td>
                    <img  class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/jianxiujing.png'"/></td>
                    <td>检修井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/paiqifa.png'"/></td>
                    <td>排气阀</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/paiqijing.png'"/></td>
                    <td>排气井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/paishajing.png'"/></td>
                    <td>排沙井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/santongjing.png'"/></td>
                    <td>三通井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/sitongjing.png'"/></td>
                    <td>四通井</td>
                </tr>
                <tr>
                    <td>
                    <img  class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/xiaofangjing.png'"/></td>
                    <td>消防井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/xiaofangshuan.png'"/></td>
                    <td>消防栓</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/zhijing.png'"/></td>
                    <td>支井</td>
                </tr>
                <tr>
                    <td>
                    <img class="TuLi_icon"  alt="" :src="'./static/gis/Templates/Black/Images/icon/zhitongjing.png'"/></td>
                    <td>直通井</td>
                </tr>
            </table>
        </div>
        <div 
            id="TuLi_DivBootom" 
            class="LayerControDivBootom"
        >
            <span  class="sys-icon-close">&nbsp;</span>
            <span  style="cursor: pointer;" @click="onCloseClick">&nbsp;关闭&nbsp;</span> 
        </div>
    </div>
</template>

<script>
export default {
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
    height: 30px;
    width: 30px;
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


