<template>
    <div class="no_content_container" v-if="visible">
        <div :class="computedClass" :style="contentStyle" v-html="content"></div>
    </div>
</template>

<script>
const DefaultContentClass = "content";
export default {
  props: {
    visible: {
      type: Boolean,
      default: false
    },
    content: {
      type: String,
      default: "暂无数据"
    },
    contentStyle: {
      type: [Object, String],
      default: ""
    },
    contentClass: {
      type: [Array, String],
      default() {
        return [];
      }
    }
  },
  computed: {
    computedClass() {
      if (typeof this.contentClass === "string") {
        let splitFlag = " ";
        if (
          this.contentClass.includes(" ") &&
          !this.contentClass.includes(",")
        ) {
          splitFlag = " ";
        } else if (
          !this.contentClass.includes(" ") &&
          this.contentClass.includes(",")
        ) {
          splitFlag = ",";
        } else {
          console.error(
            `使用contentStyle属性时，可以传入字符串或数组，当传入字符串时，必须使用空格或逗号二者之一做为多个class名的分隔符，不能同时使用`
          );
          return [DefaultContentClass];
        }
        let classArray = this.contentClass.split(splitFlag);
        classArray.push(DefaultContentClass);
        return classArray;
      } else if (Array.isArray(this.contentClass)) {
        return this.contentClass.concat([DefaultContentClass]);
      }
    }
  }
};
</script>


<style lang="less" scoped>
.no_content_container {
  // padding: 10px;
  width: 100%;
  .content {
    text-align: center;
    color: #999;
    padding: 10px;
  }
}
</style>

