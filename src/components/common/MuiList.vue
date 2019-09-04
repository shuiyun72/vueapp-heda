<!-- MUI风格的列表
  支持label, content, icon三部分内容，其中content可以通过slot方式自定义
  引用到的部分css类详见 http://dev.dcloud.net.cn/mui/ui/#listview
-->
<template>
    <div class="mui_list_container">
        <ul class="mui-table-view">
            <li class="mui-table-view-cell" v-for="(item, index) in items" :key="index" @tap="onItemClick(item, index, $event)">
                <div :class="['list_item', {'mui-navigate-right': item.withDefaultIcon && !item.iconClass}]">
                    <!-- label参数存在才渲染该span -->
                    <span 
                        class="list_item_label" 
                        v-if="item.label" 
                        :class="[item.labelClass, {'with_saperator': item.saperate}]"
                        :style="item.labelStyle"
                        v-html="item.label"
                    ></span>
                    <span 
                        class="list_item_content" 
                        v-if="item.content != undefined"
                        :class="item.contentClass" 
                        :style="item.contentStyle"
                        v-html="item.content === '' ? item.placeholder: item.content"
                    ></span>
                    <span 
                        class="list_item_content"
                        v-else
                        :class="item.contentClass" 
                        :style="item.contentStyle"
                    ><slot :name="'item_' +  (index + 1)"></slot></span>                    
                    <span 
                        v-if="!item.withDefaultIcon" 
                        class="list_item_icon" 
                        :class="item.iconClass" 
                        :style="item.iconStyle"
                        @tap="onIconClick(item, index, $event)"
                    ></span>
                </div>
            </li>
        </ul>
    </div>
</template>

<script>
/* 
  @props 
  item: Object => { 
    label: String, 
    labelClass: String,
    labelStyle: String, Object
    content: String, 
    contentStyle: String, Object
    placeholder:String, 
    withDefaultIcon: Boolean, 
    saperate: Boolean, 
    iconClass: String,
    iconStyle: String || Object
  }

  @slot
  item_{{row.index}}
  <span slot="item_1">自定义内容</span>
*/
import { deepCopy } from "@common/util";
export default {
  props: {
    items: {
      type: Array,
      // 对象与数组的默认值，必须使用工厂函数返回
      default() {
        return [];
      }
    }
  },
  methods: {
    onItemClick(row, rowIndex, event) {
      // 子组件通过发射事件的方式向父组件传递数据， 父组件通过事件监听获取数据
      this.$emit("row-click", row, rowIndex, event);
    },
    onIconClick(row, rowIndex, event) {
      // 子组件通过发射事件的方式向父组件传递数据， 父组件通过事件监听获取数据
      this.$emit("icon-click", row, rowIndex, event);
    }
  }
};
</script>

<style lang="less">
.mui_list_container {
  .list_item {
    .gray {
      color: #999;
    }
    .label_w_30per {
      &.list_item_label {
        width: 30%;
      }
    }
    & > * {
      display: inline-block;
      vertical-align: middle;
      &.list_item_label {
        max-width: 30%;
        padding-right: 2%;
        &.with_saperator {
          border-right: 1px solid #bbb;
        }
      }
      &.list_item_content {
        // margin-right: calc(~"1% + 1em");
        /* 列表项最右侧的箭头是绝对定位，且字体大小为继承，此属性可避免内容与图标重叠*/
        max-width: calc(~"100% - 32% - 1em");
        padding-left: 2%;
      }
      &.list_item_icon {
        position: absolute;
        top: 50%;
        right: 15px;
        font-size: inherit;
        color: #bbb;
        transform: translateY(-50%);
      }
    }
  }
}
</style>


