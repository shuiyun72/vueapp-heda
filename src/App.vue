<template>
  <div id="app">
    <transition name="index">
      <!-- keep-alive在路由切换时表示保留路由对应的组件 -->
      <!-- <keep-alive><router-view/></keep-alive> -->
      <router-view/>
    </transition>
  </div>
</template>

<script>
import { Loading } from "element-ui";
import Vue from "vue";
export default {
  name: "App",
};
</script>

<style lang="less">
/* 覆盖mui的默认字体大小17px */

html,
body {
  // background-color: #2F4554;
  font-size: 13px !important;
  /* 取消移动端滚动条 */
  height: 100vh;
  overflow: scroll;
  &::-webkit-scrollbar {
    display: none;
  }
}

header.mui-bar-nav {
  z-index: 9999;
  box-shadow: unset;
  -webkit-box-shadow: unset;
}

.custom_color_dark {
  color: #001d26;
}

.custom_color_light {
  color: #00afa9;
}

.custom_bgcolor_dark {
  background-color: #001d26;
  color: #fff;
  &.el-button {
    background-color: #001d26;
    color: #fff;
    &.is-disabled {
      color: #c0c4cc;
      background-color: #fff;
    }
  }
  /* 修复bootstrap默认在focus和hover时使用白色背景 */
  &.button:hover,
  &.button:focus {
    background-color: #001d26;
  }
}

.custom_bgcolor_light {
  background-color: #00afa9;
  color: #fff;
  &.el-button {
    background-color: #00afa9;
    color: #fff;
    &.is-disabled {
      color: #c0c4cc;
      background-color: #fff;
    }
  }
  /* 修复bootstrap默认在focus和hover时使用白色背景 */
  &.button:hover,
  &.button:focus {
    background-color: #00afa9;
  }
}

// 清除浮动，可用于App下的任何组件的template中
.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both;
}

// 使文字不换行，且溢出部分显示省略号
.text_ellipsis {
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

div.custom_el-card-header_color_358F8B {
  & > .el-card__header {
    background-color: #358f8b;
    color: #fff;
  }
}

// 定制el-card组件的header样式
.el-card__header {
  padding-top: 0.8rem;
  padding-bottom: 0.8rem;
}

// .mui-table-view-cell的after伪元素的背景色会影响element card组件的ui效果
div.el-card.mui-table-view-cell::after {
  background-color: unset;
}

// 覆盖element-ui的固定宽度
.el-message-box__wrapper {
  div.el-message-box {
    max-width: 80%;
    width: 80%;
    .el-message-box__btns {
      button {
        // 改变底部按钮的宽度为100%
        width: 100%;
      }
    }
  }
}

// 在安卓webview中 优先级比默认样式低，因此加一个div选择器，提升优先级
div.mui-poppicker {
  z-index: 9998;
}

.mui-backdrop {
  z-index: 3000;
}

// 取消mui中样式对elementui组件样式的影响
div.el-select input {
  margin-bottom: unset;
}

div.el-input input {
  margin-bottom: unset;
}

// 公共的表单外层容器
.common_form_container {
  width: 95%;
  margin: 0 auto;
  background-color: #fff;
  padding: 10px;
  margin-top: 2%;
  margin-bottom: 2%;
  border-radius: 8px;
}

// 图表相关

/* 直接复制了原代码 */

.wChart-KPI-Icon {
  position: absolute;
  font-size: 76px;
  color: #fff;
  left: 10px;
  bottom: 6px;
}

.wChart-KPI-V {
  position: absolute;
  right: 5px;
  font-size: 35pt;
}

.wChart-KPI-Title {
  font-size: 15pt;
  color: #fff;
  margin-left: 5px;
  position: absolute;
  /*top:0px;*/
}

.wChart-KPI_Value {
  position: relative;
  font-size: 30px;
  color: #000;
  text-align: right;
}

.wChart-KPI_Unit {
  font-size: 10pt;
  color: #fff;
  text-align: right;
}

.wChart-KPI-Title {
  width: 100%;
  height: 35px;
  position: absolute;
  left: -5px;
  line-height: 35px;
  font-size: 12pt;
  color: #fff;
}

/* ------- 柱状图 Bar --------*/

.wChart-All-Div-Title {
  position: absolute;
  border-bottom: solid 1px #808080;
  height: 35px;
  line-height: 35px;
  top: 0px;
  left: 0px;
  width: 100%;
  font-family: "黑体";
  color: #fff;
  text-align: left;
  /*background-color:#f9f9f9;*/
}

.wChart-All-Title {
  padding-right: 3px;
  padding-left: 4px;
}

.wChart-All-DOM {
  height: 100%;
  width: 100%;
}

.wChart-Bar-DOM {
  height: 100%;
  width: 100%;
}

/* ------- 饼图 pie --------*/

.wChart-Pie-DOM {
  height: 100%;
  width: 100%;
  position: relative;
  background-color: #fff;
}

.wChart-Line-DOM {
  height: 100%;
  width: 100%;
}

/*变透明设置*/

.wChart-KPI-Transparent {
  filter: alpha(opacity=50);
  -moz-opacity: 0.5;
  -khtml-opacity: 0.5;
  opacity: 0.5;
}

/*页面布局*/

.wChart-kGrid {
  width: 100%;
  position: relative;
}

.wChart-kGrid2 {
  float: left;
  background-color: #ccc;
  position: relative;
}

.wChart-kGrid_2_Float {
  float: left;
  background-color: #ccc;
  position: relative;
}

.wChart-Grid {
  float: left;
  background-color: #fff;
  /*margin:5px;*/
  /*position: relative; border: solid 1px #808080;*/
  /*width:100%;*/
}

.wChart-Grid_H {
  /*float:left;*/
  background-color: #fff;
  /*margin:5px;*/
  /*position: relative; border: solid 1px #808080;*/
  /*width:100%;*/
}

.wChart-Chart {
  float: left;
  margin: 5px;
  border: solid 1px #808080;
  background-color: #fff;
  position: relative;
}

.wChart-Grid_SetWidth {
  background-color: #ccc;
}

.wChart-Div {
  /*margin:5px;*/
  background-color: #fff;
  position: relative;
  /*border: solid 1px #808080;*/
  /*margin-bottom:10px;*/
  /*margin-top:10px*/
}

.wChart-Div-Float {
  float: left;
  margin: 5px;
  background-color: #fff;
  position: relative;
  border: solid 1px #808080;
  margin-top: 10px;
}

// div#app {
//   height: 100vh;
//   overflow: scroll;
//   &::-webkit-scrollbar {
//     display: none;
//   }
// }

/* 页面切换动效， 尝试使用translate与absolute，但在路由切换的case下效果不好 */

/* div#app {
            position: relative;
          }
          .index-enter {
            position: absolute;
            left: 100vw;
            width: 100vw;
            top: 0;
          }
          .index-leave {
            position: absolute;
            left: -100vw;
            width: 100vw;
            top: 0;
          }
          .index-enter-active {
            transition: all 25s;
            position: absolute;
            left: 100vw;
            width: 100vw;
            top: 0;
          }
          .index-leave-active {
            transition: all 25s;
            position: absolute;
            left: -100vw;
            width: 100vw;
            top: 0;
          }
          .index-enter-to,
          .index-leave {
            position: absolute;
            left: 0;
            top: 0;
          } */
</style>
