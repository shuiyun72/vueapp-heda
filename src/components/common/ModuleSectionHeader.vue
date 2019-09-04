<template>
  <div class="mui-row module_section-header">
    <div class="color_box"></div>
    <span class="title_text">{{sectionTitle}}</span>
  </div>
</template>

<script>
export default {
  props: {
    // 功能块的标题
    sectionTitle: {
      type: String
    }
  }
};
</script>

<style lang="less" scoped>
.module_section-header {
  .color_box {
    background-color: #01273a;
    width: 10px;
    height: 17px;
    display: inline-block;
    vertical-align: middle;
  }
  .title_text {
    vertical-align: middle;
  }
}
</style>

