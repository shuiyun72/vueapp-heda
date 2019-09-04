<template>
    <div class="statistic_container">
        <!-- 侧边滑动树 -->
        <transition name="fade">
            <div class="statistic_tree_container" v-show="treeVisible">
                <el-input
                  class="filter_input"
                  clearable=""
                  placeholder="输入关键字进行区域过滤"
                  v-model="filterText">
                </el-input>
                <el-tree
                  @node-click="onTreeNodeClick"
                  class="statistic_tree"
                  :data="treeNodes"
                  :props="defaultProps"
                  :filter-node-method="filterNode"
                  default-expand-all
                  highlight-current
                  ref="statisticTree"
                >
                </el-tree>
            </div>
        </transition>
        <!-- 时间选择器 -->
        <div class="filter_container">
            <el-date-picker
                type="datetime"
                v-model="filterStartDate"
                ref="startDatePicker"
                name="startDatePicker"
                placeholder="开始时间"
                :prefix-icon="'-'"
                :editable="false"
                format="yyyy-MM-dd hh:mm:ss"
                value-format="yyyy-MM-dd hh:mm:ss"
                style="width: 49%;"
                class="date_picker_padding_left_5"
            >
            </el-date-picker>
            <el-date-picker
                type="datetime"
                v-model="filterEndDate"
                ref="endDatePicker"
                name="endDatePicker"
                placeholder="结束时间"
                :prefix-icon="'-'"
                :editable="false"
                format="yyyy-MM-dd hh:mm:ss"
                value-format="yyyy-MM-dd hh:mm:ss"
                style="width: 49%;"
                class="date_picker_padding_left_5"
            >
            </el-date-picker>
        </div>
        <!-- 顶部action -->
        <div class="button_group_container">
            <el-button icon="el-icon-delete" class="action_button custom_bgcolor_dark" @click="onSearchResetButtonClick">重 置</el-button>
            <el-button icon="el-icon-search" class="action_button custom_bgcolor_light" @click="onSearchButtonClick">搜 索</el-button>
        </div>
        <!-- 物理检测点数据表 -->
        <div class="record_table_container">
            <el-table
                v-loading="tableLoading"
                :data="formattedTableData"
                class="record_table"
                stripe                
                border
                height="calc(84vh - 166px)"
                :header-cell-style="tableHeaderCellStyle"
                empty-text="暂未查询到符合条件的数据"
            >
                <el-table-column
                    v-for="column in columnConfig"
                    :key="column.field"
                    :label="column.label"
                    :prop="column.prop"
                    :align="column.align"
                    :fixed="column.fixed"
                    :min-width="column.width"
                ></el-table-column>
            </el-table>
        </div>
        <!-- 分页组件 -->
        <div class="pagination_container">
            <!-- 第一行 -->
            <el-pagination
                :current-page.sync="currentPageNumber"
                :page-sizes="[5, 10, 20, 50, 100, 200]"
                :page-size="currentPageSize"
                :pager-count="5"
                layout="prev, pager, next"
                :total="recordTotal"
                class="table_paginator wrap"
            >
            </el-pagination>
            <!-- 第二行 -->
            <el-pagination
                :current-page.sync="currentPageNumber"
                @size-change="onPageSizeChange"
                :page-size="currentPageSize"
                :page-sizes="[5, 10, 20, 50, 100, 200]"
                layout="total, sizes, jumper"
                :total="recordTotal"
                class="table_paginator wrap"
            >
            </el-pagination>
        </div>
        <!-- 底部action bar -->
        <div class="statistic_footer_action_bar">
            <el-button
                class="custom_bgcolor_light"
                style="width: 100%;"
                @click="onShowTreeButtonClick"
            >
                {{treeVisible ? "关 闭 监 测 点 列 表": "显 示 监 测 点 列 表"}}
            </el-button>
        </div>
    </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
import commonConsts from "@pages/features/consts";
import localConsts from "./consts";
// import Tree from "@comp/common/Tree";

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

      instance.fetchPhysicPointTreeData().then(treeNodes => {
        // 改变state
        instance.treeNodes = treeNodes;
      });
    });
  },
  data() {
    return {
      /* 树相关 */
      treeNodes: [],
      defaultProps: {
        children: "children",
        label: "text"
      },
      treeVisible: false,
      filterText: "",

      /*************灵性分割线**************/
      /* 日期选择器 */
      filterStartDate: "",
      filterEndDate: "",

      /*************灵性分割线**************/
      /* 表格相关 */
      tableLoading: false,
      tableHeaderCellStyle: {
        backgroundColor: "#001d26",
        color: "#fff",
        textAlign: "center",
        padding: "5px 0"
      },
      // 后端生成的一个点位的动态属性（字段），用于动态生成列
      fieldList: [],
      tableData: [],
      rawRecordList: [],
      // 当前选择的点位的相关信息，从tree中点击node时获取
      currentPickedNodeInfo: {},

      /*************灵性分割线**************/
      /* 分页相关 */
      recordTotal: 0,
      // 当前页码
      currentPageNumber: 1,
      // 当前每页条数
      currentPageSize: 20
    };
  },
  computed: {
    // 当前选择的点位的DataTableID，从tree中点击node时获取
    currentDataTableId() {
      return this.currentPickedNodeInfo.DataTableID;
    },
    // 当前选择的点位的DataPointID，从tree中点击node时获取
    currentDataPointId() {
      return this.currentPickedNodeInfo.RID;
    },
    // el-table的column配置，通过v-for动态生成列
    columnConfig() {
      return this.fieldList.map((fieldObj, index) => {
        return {
          // 固定第一列和日期列
          fixed:
            index === 0 ? "left" : fieldObj.field === "ReadDate" && "right",
          width: index === 0 ? 100 : Number(fieldObj.width) * 0.6 || 110,
          prop: fieldObj.field,
          label: fieldObj.title,
          align: fieldObj.align || "center"
        };
      });
    },
    formattedTableData() {
      let result = [];
      if (!_.isEmpty(this.rawRecordList)) {
        result = this.rawRecordList.map(record => {
          return _.assign({}, record, {
            ReadDate: dateHelper.format(
              new Date(record.ReadDate),
              "yyyy-MM-dd hh:mm:ss"
            )
          });
        });
      }
      console.log("#", result);
      return result;
    }
  },
  methods: {
    /* NodeTree相关 */
    // 判断一个节点是否为叶子节点
    isLeaf(nodeConfig) {
      return !nodeConfig.children || _.isEmpty(nodeConfig.children);
    },
    onShowTreeButtonClick() {
      this.treeVisible = !this.treeVisible;
    },
    onTreeNodeClick(nodeConfig, node) {
      console.log("点击node！", nodeConfig);
      if (this.isLeaf(nodeConfig)) {
        this.currentPickedNodeInfo = nodeConfig;
        this.treeVisible = false;
        this.fetchFieldList(
          this.currentDataTableId,
          this.currentDataPointId
        ).then(fieldList => {
          // 更新state
          this.fieldList = fieldList;
          this.onSearchButtonClick();
        });
      }
    },
    filterNode(value, data) {
      if (!value) return true;
      return data.text.indexOf(value) !== -1;
    },
    // 获取物理检测点树形数据
    fetchPhysicPointTreeData() {
      return apiMonitor.GetPhysicPointTree().then(res => {
        console.log("物理检测点树形菜单, ", res);
        let treeNodes = deepCopy(res.data);
        return treeNodes;
      });
    },

    /* 表格相关 */
    // 获取当前点击的物理检测点的属性字段列表
    fetchFieldList(
      dataTableId = this.currentDataTableId,
      dataPointId = this.currentDataPointId
    ) {
      console.log("开始获取fieldList");
      return apiMonitor.GetPointFieldList(dataTableId, dataPointId).then(
        res => {
          let rawFieldList = res.data.Data;
          console.log("Raw Fields", rawFieldList);
          let fieldList = _.uniqBy(rawFieldList, "field");
          console.log("获取到字段列表 ", fieldList);
          return fieldList;
        },
        err => {
          console.log("Fields Err ", err);
        }
      );
    },
    // 根据当前点选的监测点node信息获取相关的历史数据
    fetchTableData(
      {
        dataTableId = "",
        dataPointId = "",
        pageNumber = 1,
        rowsPerPage = 20,
        startTime = "",
        endTime = ""
      },
      callback
    ) {
      this.tableLoading = true;
      apiMonitor
        .GetPhysicPointRecordList(
          dataTableId,
          dataPointId,
          pageNumber,
          rowsPerPage,
          startTime,
          endTime
        )
        .then(res => {
          console.log("获取点位查询数据 ", res);
          this.tableLoading = false;
          if (res.data.ErrCode == 0) {
            let recordList = deepCopy(res.data.rows);
            let total = res.data.total;
            if (callback instanceof Function) {
              callback(null, { recordList, total });
            }
          } else {
            if (callback instanceof Function) {
              callback({
                code: res.data.ErrCode,
                message: res.data.ErrInfo
              });
            }
          }
        })
        .catch(err => {
          this.tableLoading = false;
          window.mui.toast("获取数据失败，请稍后再试");
        });
    },
    onSearchResetButtonClick() {
      this.filterStartDate = "";
      this.filterEndDate = "";
      // 更新表格数据
      if (this.currentPageNumber == 1 && this.currentPageSize == 20) {
        this.onSearchButtonClick();
      } else {
        // 由于设置了watcher，改变以下两个值的时候就会发送请求
        this.currentPageSize = 20;
        this.currentPageNumber = 1;
      }
    },
    onSearchButtonClick() {
      this.fetchTableData(
        {
          dataTableId: this.currentDataTableId,
          dataPointId: this.currentDataPointId,
          pageNumber: this.currentPageNumber,
          rowsPerPage: this.currentPageSize,
          startTime: this.filterStartDate,
          endTime: this.filterEndDate
        },
        (err, data) => {
          if (err) {
            console.log(`获取表格数据请求出错`, err);
          } else {
            this.rawRecordList = data.recordList;
            this.recordTotal = data.total;
          }
        }
      );
    },
    onPageSizeChange(size) {
      this.currentPageSize = size;
      this.fetchTableData(
        {
          dataTableId: this.currentDataTableId,
          dataPointId: this.currentDataPointId,
          pageNumber: this.currentPageNumber,
          rowsPerPage: this.currentPageSize,
          startTime: this.filterStartDate,
          endTime: this.filterEndDate
        },
        (err, data) => {
          if (err) {
            console.log(`获取表格数据请求出错`, err);
          } else {
            this.rawRecordList = data.recordList;
            this.recordTotal = data.total;
          }
        }
      );
    }
  },
  watch: {
    filterText(val) {
      this.$refs.statisticTree.filter(val);
    },
    currentPageNumber(newVal) {
      this.fetchTableData(
        {
          dataTableId: this.currentDataTableId,
          dataPointId: this.currentDataPointId,
          pageNumber: this.currentPageNumber,
          rowsPerPage: this.currentPageSize,
          startTime: this.filterStartDate,
          endTime: this.filterEndDate
        },
        (err, data) => {
          if (err) {
            window.mui.toast(err.message);
          } else {
            this.rawRecordList = data.recordList;
            this.recordTotal = data.total;
          }
        }
      );
    }
  },
  directives: {}
  // components: { Tree }
};
</script>

<style lang="less">
.statistic_container {
  .statistic_tree_container {
    position: fixed;
    left: 0px;
    width: 100vw;
    max-width: 100vw;
    height: calc(~"99vh - 94px");
    overflow: scroll;
    opacity: 1;
    z-index: 1000;
    background: #eee;
    & > div.tree_container div {
      background-color: #ccc;
    }

    .filter_input {
      margin: 10px auto;
    }
    /* 这是element Collapse组件自带的类，为其定制样式 */
    div.el-collapse-item__content {
      padding-left: 15px;
      padding-bottom: 0px;
    }
    /* 这是element Tree组件自带的类， 为其定制样式*/
    // 这里改变选中时的行样式
    .el-tree--highlight-current
      .el-tree-node.is-current
      > .el-tree-node__content {
      background-color: #00afa9;
      color: white;
    }
    /* 这是element Tree组件自带的类， 为其定制样式*/
    .el-tree-node__content {
      /* 可以在这里对Tree的每一行做样式控制 比如高度*/
      height: 50px;
    }
  }
  div.date_picker_padding_left_5 {
    &.el-input {
      input {
        padding-left: 5px;
        font-size: 14px;
      }
    }
  }
  .pagination_container {
    width: 100%;
    .table_paginator {
      text-align: center;
      width: 100%;
      &.wrap {
        white-space: pre-wrap;
      }
    }
    .el-pagination__jump {
      margin-left: 1.5%;
    }
  }
  .filter_container {
    padding-top: 0.5vh;
    width: 100%;
    text-align: center;
  }
  .button_group_container {
    text-align: center;
    margin: 0.5vh 0;
    .el-button + .el-button {
      margin-left: unset;
    }
    .action_button {
      width: 49%;
    }
  }
  .statistic_footer_action_bar {
    position: fixed;
    bottom: 0px;
    width: 100%;
    // min-height: 50px;
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