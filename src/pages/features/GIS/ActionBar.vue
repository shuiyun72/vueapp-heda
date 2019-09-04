<template>
    <div class="map_action_bar_container">
        <div  class="inner_bar bar" v-if="innerbarVisible && currentInnerbarItems.length > 0">
            <div 
                class="inner_item item" 
                v-for="innerItem in currentInnerbarItems" 
                :key="innerItem.id" 
                @click="onActionItemClick(innerItem, 1)"
            >
                <template v-if="innerItem.image">
                    <img 
                        class="item_image" 
                        :src="innerItem.image.path" 
                        :alt="innerItem.image.alt" 
                        :style="innerItem.image.style"
                    >
                    <div class="item_image_text">{{innerItem.image.text}}</div>
                </template>
                <template v-else>
                    <i :class="innerItem.iconClass" :style="innerItem.iconStyle"></i>
                    <div class="item_text">{{innerItem.text}}</div>
                </template>
            </div>
        </div>
        <div class="outer_bar bar">
            <div 
                class="outer_item item" 
                v-for="outerItem in items" 
                :key="outerItem.id" 
                @click="onActionItemClick(outerItem, 0)"
            >
                <template v-if="outerItem.image">
                    <img 
                        class="item_image" 
                        :src="outerItem.image.path" 
                        :alt="outerItem.image.alt" 
                        :style="outerItem.image.style"
                    >
                    <div class="item_image_text">{{outerItem.image.text}}</div>
                </template>
                <template v-else>
                    <i :class="outerItem.iconClass" :style="outerItem.iconStyle"></i>
                    <div class="item_text">{{outerItem.text}}</div>
                </template>
            </div>
        </div>
    </div>
</template>
<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
export default {
  props: {
    items: {
      type: Array,
      default() {
        return [
          {
            id: "gauge",
            text: "测量工具",
            icon: "",
            children: [
              { id: "area", text: "面积", icon: "" },
              { id: "length", text: "距离", icon: "" }
            ]
          },
          {
            id: "reset",
            text: "重置",
            icon: ""
          },
          {
            id: "location",
            text: "开启定位",
            icon: ""
          },
          {
            id: "legend",
            text: "图例",
            icon: ""
          },
          {
            id: "layer-switcher",
            text: "图层切换",
            icon: "",
            children: [
              { id: "satellite-view", text: "影像地图", icon: "" },
              { id: "street-view", text: "街道地图", icon: "" },
              { id: "DMA-view", text: "DMA区域", icon: "" }
            ]
          }
        ];
      }
    }
  },
  data() {
    return {
      innerbarVisible: false,
      // 最近一次点击的外层bar的item，用于计算当前子bar的可选项
      currentClickedOuterItem: {},
      lastClickedOuterItem: {}
    };
  },
  computed: {
    currentInnerbarItems() {
      if (
        this.currentClickedOuterItem.children &&
        !_.isEmpty(this.currentClickedOuterItem.children)
      ) {
        return deepCopy(this.currentClickedOuterItem.children);
      } else {
        // 当前选中的外层项没有对应的子项
        return [];
      }
    }
  },
  methods: {
    // 点击某一个某一个项时
    onActionItemClick(item, layer) {
      // 点击的是外层工具条
      if (layer === 0) {
        if (item.id !== this.lastClickedOuterItem.id) {
          this.innerbarVisible = false;
        }
        this.currentClickedOuterItem = deepCopy(item);
        this.innerbarVisible = !this.innerbarVisible;
        this.lastClickedOuterItem = item;
      } else if (layer === 1) {
        // 点击内层工具条
        this.innerbarVisible = false;
      }
      this.$emit("item-click", {
        item: deepCopy(item),
        layer
      });
    }
  }
};
</script>
<style lang="less" scoped>
.map_action_bar_container {
  .bar {
    width: 100%;
    display: flex;
    flex-flow: row nowrap;
    &.inner_bar {
      min-height: 65px;
      background-color: #222;
      opacity: 0.7;
      .item {
        margin: 10px auto 5px auto;
        .item_text {
          color: #fff;
        }
      }
    }
    &.outer_bar {
      min-height: 55px;
      background-color: #3b3e43;
      justify-content: space-around;
      .item {
        padding: 5px 5px 5px 5px;
        .item_text {
          color: #11a4db;
        }
      }
    }
    .item {
      text-align: center;
      position: relative;
      .item_image {
        width: 100%;
        height: 100%;
        display: inline-block;
        border: 3px solid #000;
        border-radius: 5px;
      }
      .item_image_text {
        background-color: #000;
        position: absolute;
        text-align: right;
        right: 0px;
        bottom: 0px;
        color: #fff;
      }
    }
  }
}
</style>


