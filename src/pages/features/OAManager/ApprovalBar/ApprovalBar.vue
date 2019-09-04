<template>
  <div class="approval_bar_container">
    <div class="approval_label_container">审核意见：</div>
    <div class="approval_select_container">
      <el-select name="type" v-model="result" placeholder="请选择审核结果">
        <el-option
          v-for="item in selectOption"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        ></el-option>
      </el-select>
    </div>
  </div>
</template>

<script>
import _ from "lodash";
export default {
  data() {
    return {
      result: undefined,
      selectOption: [
        {
          label: "同意",
          value: "同意"
        },
        {
          label: "不同意",
          value: "不同意"
        }
      ]
    };
  },
  methods: {
    getValue() {
      return this.result;
    },
    reset() {
      this.result = undefined;
    },
    isValid() {
      return !(this.result === undefined || this.result === "");
    }
  }
};
</script>

<style lang="less" scoped>
.approval_bar_container {
  width: 100%;
  display: flex;
  justify-content: space-around;
  align-items: center;
}
</style>

