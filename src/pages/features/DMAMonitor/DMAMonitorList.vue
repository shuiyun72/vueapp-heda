<template>
    <div class="DMA_monitor_container">
        <div class="DMA_map" id="DMA_map"></div>
        <transition name="fade">
            <div class="DMA_tree_container" v-show="treeVisible">
                <el-input
                  class="filter_input"
                  clearable
                  placeholder="输入关键字进行区域过滤"
                  v-model="filterText">
                </el-input>
                <el-tree
                  @node-click="onTreeNodeClick"
                  @current-change="onCurrentNodeChange"
                  class="DMA_tree"
                  check-on-click-node
                  default-expand-all
                  :data="treeNodes"
                  :props="defaultProps"
                  :filter-node-method="filterNode"
                  ref="DMATree"
                >
                </el-tree>
            </div>
        </transition>
        <div class="DMA_footer_action_bar">
            <MapActionBar :items="actionbarItems" @item-click="onActionBarItemClick"></MapActionBar>
        </div>
    </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import BaseMap from "@JS/Map/BaseMap";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
import commonConsts from "@pages/features/consts";
import { ActionbarConfig } from "./consts";
import MapActionBar from "@pages/features/GIS/ActionBar";

export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      // 定制该路由下的设备返回键逻辑
      instance.$defineDeviceBack(defaultFunction => {
        if (instance.treeVisible) {
          // 如果当前树形选择器可见，则返回键会隐藏树形选择器
          instance.treeVisible = false;
        } else {
          // 如果当前没有弹出框，则使用默认返回逻辑
          defaultFunction();
        }
      });
    });
  },
  mounted() {
    // 初始化DMA地图控制器对象并绘制地图
    this.initMapForDMA();
    // 发送请求获取地区树数据
    this.fetchDMADistrictsTree().then(this.initDMADistrictsOnMap);
  },
  beforeDestroy() {
    if (this.mapController) {
      // 在组件注销前释放对象
      this.mapController = null;
    }
  },
  data() {
    return {
      mapController: "",
      treeNodes: [
        {
          id: 1,
          label: "一级 1",
          children: [
            {
              id: 4,
              label: "二级 1-1",
              children: [
                {
                  id: 9,
                  label: "三级 1-1-1"
                },
                {
                  id: 10,
                  label: "三级 1-1-2"
                }
              ]
            }
          ]
        },
        {
          id: 2,
          label: "一级 2",
          children: [
            {
              id: 5,
              label: "二级 2-1"
            },
            {
              id: 6,
              label: "二级 2-2"
            }
          ]
        },
        {
          id: 3,
          label: "一级 3",
          children: [
            {
              id: 7,
              label: "二级 3-1"
            },
            {
              id: 8,
              label: "二级 3-2"
            }
          ]
        }
      ],
      defaultProps: {
        children: "children",
        label: "text"
      },
      treeVisible: false,
      filterText: "",
      // 当前选择的node的配置数据
      currentPickedNodeData: "",
      actionbarItems: deepCopy(ActionbarConfig)
    };
  },
  computed: {},
  methods: {
    // 初始化DMA地图控制器对象并绘制地图
    initMapForDMA() {
      let mapController = (this.mapController = new BaseMap());
      mapController.Init("DMA_map");
    },
    // 发送请求获取DMA分区树数据
    fetchDMADistrictsTree() {
      return apiMonitor.GetDMADistrictTree().then(res => {
        console.log(`树数据`, res);
        let treeNodes = (this.treeNodes = deepCopy(res.data));
        return treeNodes;
      });
    },
    // 根据已获取的分区树数据在地图上绘制分区图形
    initDMADistrictsOnMap() {
      _.each(this.treeNodes[0].children, node => {
        console.log(`Foreach Node `, node);
        if (node.Gemo && node.Gemo != "Null" && Number(node.ParentID) > 0) {
          console.log(1);
          this.mapController.addFeatureForDMADistricts(
            node.Gemo,
            node.RID,
            node.text,
            node.AreaColor,
            node.BorderColor
          );
        }
      });
      // true为显示，false为隐藏，DMA图层模式visible为false
      this.mapController.showDMALayer(true);
    },
    onActionBarItemClick({ item, layer }) {
      console.log("点击ActionBar中的一项！", item, layer);
      // GIS模式下的action
      if (layer === 0) {
        // 外层action bar
        if (item.id == "select") {
          this.onDistrictSelectClick();
        }
        if (item.id == "rank") {
          this.onDistrictRankClick();
        }
      }
    },
    onDistrictSelectClick() {
      this.treeVisible = !this.treeVisible;
    },
    onDistrictRankClick() {
      this.$router.push({ name: "DMAStatistic" });
    },
    onCurrentNodeChange(pickedNodeConfig) {
      // Do Nothing
    },
    onTreeNodeClick(pickedNodeConfig, pickedNode, tree) {
      if (!pickedNodeConfig.children || _.isEmpty(pickedNodeConfig.children)) {
        this.treeVisible = false;
        this.currentPickedNodeData = pickedNodeConfig;
        if (
          pickedNodeConfig.Gemo &&
          pickedNodeConfig.Gemo != "Null" &&
          pickedNodeConfig.ParentID > 0
        ) {
          // 这里直接改变了mapController实例中的值，为PC端逻辑，暂不清楚用处
          this.mapController.CurrentFeature = this.mapController.fitDMALayerExtentByfqId(
            pickedNodeConfig.RID
          );
          /* 以下注释块代码为PC端代码，暂不知用处 */
          // CurrentSelectNode = node;//上一次分区
          // if (CurrentSelectNode) {
          //   MapInfo.SetDrawCurrentFeatureColor(
          //     CurrentSelectNode.AreaColor,
          //     CurrentSelectNode.BorderColor,
          //     "0.3",
          //     2,
          //     CurrentSelectNode.text
          //   );
          // }
        }
      } else if (pickedNodeConfig.ParentID == 0) {
        // 点击了奉化根节点
      }
    },
    filterNode(value, data) {
      if (!value) return true;
      return data.text.indexOf(value) !== -1;
    }
  },
  watch: {
    filterText(val) {
      this.$refs.DMATree.filter(val);
    }
  },
  components: { MapActionBar }
};
</script>

<style lang="less">
.DMA_monitor_container {
  // position: relative;
  .DMA_map {
    width: 100%;
    height: calc(~"99vh - 44px");
  }
  .DMA_tree_container {
    position: fixed;
    left: 0px;
    bottom: 65px;
    width: 100vw;
    max-width: 100vw;
    height: calc(~"98vh - 107px");
    overflow: scroll;
    opacity: 0.9;

    .filter_input {
      margin: 10px auto;
    }
    /* 这是element Collapse组件自带的类，为其定制样式 */
    div.el-collapse-item__content {
      padding-left: 15px;
      padding-bottom: 0px;
    }
    /* 这是element Tree组件自带的类， 为其定制样式*/
    .el-tree-node__content {
      /* 可以在这里对Tree的每一行做样式控制 比如高度*/
      height: 50px;
      &:hover {
        background-color: #00afa9;
        color: white;
      }
    }
  }
  .DMA_footer_action_bar {
    position: fixed;
    bottom: 0px;
    left: 0px;
    right: 0px;
    width: 100%;
  }
  .fade-enter-active,
  .fade-leave-active {
    transition: left 0.2s ease-in-out;
  }
  .fade-enter,
  .fade-leave-to {
    left: -100%;
  }
  .fade-enter-to,
  .fade-leave {
    left: 0;
  }
}
</style>