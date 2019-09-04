<template>
  <div class="module_item" :class="currentClass" @tap="enabled && $emit('click')">
    <li 
        class="mui-table-view-cell" 
        :style="{display:enabled ? 'block' : 'none'}"
    >
      <div class="module_item-part">
        <!-- 图片与图标不共存，图片优先级高 -->
        <div v-if="picture !== ''"
          class="picture_container"
          :style="bgImage"
        ></div>
        <slot v-else name="icon"></slot>  
        <em class="count" v-if="eventCount > 0">{{eventCount}}</em>  
      </div>
      <div class="module_item-part">
        <span class="module_item-title">{{title}}</span>
        <br>
        <span class="module_item-desc">{{desc}}</span>
      </div>
    </li>
  </div>
</template>

<script>
export default {
  name: "ModuleItem",
  props: {
    enabled: Boolean,
    // horizental || vertical
    mode: {
      type: String,
      // 默认为水平模式， 即图片与文字水平排列
      default: "horizontal"
    },
    // 是否启用边框效果
    withBorder: {
      type: Boolean,
      default: false
    },
    // 主标题
    title: {
      type: String,
      default: ""
    },
    // 主标题下方的描述文字
    desc: {
      type: String,
      default: ""
    },
    // 功能模块的图片（注：图片与图标不共存， 通过v-if、v-else控制）
    picture: {
      type: String,
      default: ""
    },
    pictureContainerStyle: {
      type: Object,
      default() {
        return {};
      }
    },
    eventCountPatrol:{
      default(){
        return 0;
      }
    },
    eventCountSelfTask:{
      default(){
        return 0;
      }
    },
    eventCountOrders:{
      default(){
        return 0;
      }
    }
  },
  data() {
    return {};
  },
  computed: {
    eventCount(){    
      if(this.title == "巡检任务"){
        return this.eventCountPatrol;
      }else
      if(this.title == "个人工单"){
        return this.eventCountSelfTask;
      }else
      if(this.title == "维修工单"){
        return this.eventCountOrders;
      } 
    },
    currentClass() {
      let modeClass =
        this.mode === "horizontal" ? "horizontal_mode" : "vertical_mode";
      let borderClass = { border: this.withBorder };
      return [modeClass, borderClass];
    },
    bgImage() {
      let computedStyle = Object.assign({}, this.pictureContainerStyle, {
        "background-image": `url(${this.picture})`
      });
      return computedStyle;
    }
  }
};
</script>

<style lang="less" scoped>
.module_item {
  background-color: #fff;
  list-style-type: none;

  &.border {
    border: 1px solid #eee;
  }

  &-part {
    vertical-align: middle;
    .picture_container {
      width: 100%;
      height: 18vw;
      background-repeat: no-repeat;
      background-position: center;
      background-size: 80% 100%;
    }
    span.module_item-desc {
      color: #aaa;
    }
  }
}

.horizontal_mode {
  /* 在超小型机下，水平模式的item会受到挤压换行，此属性会防止布局失败 */
  text-align: center;
  .module_item-part {
    /* 水平模式下， 图片与标题水平排列*/
    display: inline-block;
    .picture_container {
      height: 17vw;
    }
    &:nth-child(1) {
      width: 45%;
    }
    &:nth-child(2) {
      width: auto;
    }
  }
}
.vertical_mode .module_item-part {
  // 仅垂直模式下 文字水平居中， 水平模式下文字默认左对齐
  text-align: center;
}

/* 覆盖mui默认的padding */
li.mui-table-view-cell {
  padding: 11px 5px;
  position: relative;
  em.count{
    position:absolute;
    width: 18px;
    height: 18px;
    background:#f75b5b;
    color: #fff;
    line-height: 18px;
    text-align: center;
    top:9px;
    left: 52px;
    border-radius:50%;
    font-style: normal;
  }
}
</style>


