<template>
  <div class="tree_container">
        <el-collapse @change="onCollapseChange()">
            <el-collapse-item  
                v-for="(node) in nodes" 
                :key="node.name" 
                :title="node.title" 
                :name="node.name"
            >
                <div slot="title" 
                    :style="{borderLeft: `4px solid ${pickTitleColor()}`,paddingLeft: '10px'}"
                >
                    {{node.title}}
                </div>
                <Tree 
                    v-if="node.nodes.length > 0"
                    :nodes="node.nodes"
                    :key="node.name" 
                    :depth="depth + 1"  
                ></Tree>
            </el-collapse-item>
        </el-collapse>  
  </div>  
</template>

<script>
import _ from "lodash";
import Tree from "./Tree";
const DEFAULT_COLOR_PLATE = ["#00afa9", "#999", "#001d26"];
export default {
  name: "Tree",
  /***********
   * {
   *   depth: 0,
   *   nodes:[
   *     {
   *       depth: 1,
   *       title: String,
   *       name: String,
   *       nodes: [    二级项集合
   *         {
   *           index: 2,
   *           title: String, 二级项标题
   *           name: String,  二级项唯一标识符
   *           nodes: [
   *             ...     三级项集合
   *           ]
   *         }
   *       ]
   *     }
   *   ]
   * }
   *
   *
   *
   ***********/
  props: {
    depth: {
      type: [String, Number],
      required: true
    },
    nodes: {
      type: Array,
      required: true,
      default() {
        return [];
      }
    },
    colorPlate: {
      type: Array,
      default() {
        return DEFAULT_COLOR_PLATE;
      }
    }
    // title: String,
    // name: {
    //   type: String,
    //   required: true
    // }
  },
  data() {
    return {};
  },
  computed: {},
  methods: {
    pickTitleColor() {
      const COLOR_PLATE_LEN = this.colorPlate.length;
      return this.colorPlate[this.depth % COLOR_PLATE_LEN];
    },
    onCollapseChange() {
      console.log(`当前深度depth：`, this.depth);
    }
  },
  components: { Tree }
};
</script>

<style lang="less" scoped>
</style>


