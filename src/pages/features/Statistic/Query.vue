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
                <div class="tree_scroller">
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
            </div>
        </transition>
        <div class="point_input_container">
            <el-input
                v-model="currentPickedNodeInfo.text"
                placeholder="点击选择监测点"
                style="width: 100%;"
                readonly
                @click.native="showTree"
            ></el-input>
        </div>
        <!-- 时间选择器 -->
        <div class="filter_container">
            <el-input
                v-model="filterStartDate"
                placeholder="开始日期"
                class="date_picker_padding_left_5"
                readonly
                @click.native="onStartDatePickerClick"
            >
            </el-input>
            <el-input
                v-model="filterEndDate"
                placeholder="结束日期"
                class="date_picker_padding_left_5"
                style="margin-left: 5px;"
                readonly
                @click.native="onEndDatePickerClick"
            >
            </el-input>
            <el-button 
                icon="el-icon-refresh" 
                circle
                @click="onSearchResetButtonClick"
            ></el-button>
            <el-button 
                icon="el-icon-search" 
                type="primary" 
                circle
                @click="onSearchButtonClick"
            ></el-button>
        </div>
        <!-- 物理检测点数据表 -->
        <div 
            class="record_table_container"   
        >
            <!-- <scroller 
              snapping
              style="position: unset;"
              :on-refresh="refreshTableData"
              :on-infinite="loadMoreData"
              ref="tableScroller"
              height="calc(98vh - 124px)"
            > -->
              <el-table
                  v-loading="tableLoading"
                  :data="formattedTableData"
                  class="record_table"
                  stripe                
                  border
              height="calc(98vh - 124px)"                  
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
            <!-- </scroller> -->
        </div>
    </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";

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
  mounted() {
    // 实例化date picker组件
    this._startDatePicker = new window.mui.DtPicker({
      type: "date",
      labels: ["年", "月", "日"]
    });
    this._endDatePicker = new window.mui.DtPicker({
      type: "date",
      labels: ["年", "月", "日"]
    });
    // 无限滚动
    this.scrollerDom = document.querySelector(".el-table__body-wrapper");
    this.tbodyDom = document.querySelector(".el-table__body-wrapper>table");
    this.scrollerDom.addEventListener("scroll", this.onTableScroll);
  },
  beforeDestroy() {
    this.scrollerDom.removeEventListener("scroll", this.onTableScroll);
    this._startDatePicker.dispose();
    this._endDatePicker.dispose();
    this.$revertDefaultDeviceBack();
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
      // 表格滚动条
      scrollerDom: null,
      tbodyDom: null,
      // loadmore标志，正在loadmore时，不能再次触发
      busy: false,
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
      return _.reject(this.fieldList, { field: "DataPointName" }).map(
        (fieldObj, index) => {
          return {
            // 固定日期列
            fixed: fieldObj.field === "ReadDate" && "left",
            width:
              fieldObj.field === "ReadDate"
                ? 100
                : Number(fieldObj.width) * 0.6 || 100,
            prop: fieldObj.field,
            label: fieldObj.title,
            align: fieldObj.align || "center"
          };
        }
      );
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
    },
    formattedStartTime() {
      if (this.filterStartDate) {
        return `${this.filterStartDate} 00:00:00`;
      } else {
        return this.filterStartDate;
      }
    },
    formattedEndTime() {
      if (this.filterEndDate) {
        return `${this.filterEndDate} 23:59:59`;
      } else {
        return this.filterEndDate;
      }
    }
  },
  methods: {
    // 数据表格滚动回调函数
    onTableScroll() {
      let distance =
        this.tbodyDom.clientHeight -
        this.scrollerDom.clientHeight -
        this.scrollerDom.scrollTop;
      if (distance < 10 && !this.busy) {
        console.log("%cLoadMore!!!!!!!!!", "color: red");
        this.loadMoreData();
      }
    },
    /* NodeTree相关 */
    // 判断一个节点是否为叶子节点
    isLeaf(nodeConfig) {
      return !nodeConfig.children || _.isEmpty(nodeConfig.children);
    },
    showTree() {
      this.treeVisible = !this.treeVisible;
    },
    onTreeNodeClick(nodeConfig, node) {
      console.log("点击node！", nodeConfig);
      if (this.isLeaf(nodeConfig)) {
        this.currentPickedNodeInfo = nodeConfig;
        this.$eventbus.$emit("set-title", `数据查询 | ${nodeConfig.text}`);
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
    onStartDatePickerClick() {
      // 开始日期选择
      this._startDatePicker.show(result => {
        let pickedDate = result.value;
        this.filterStartDate = pickedDate;
      });
    },
    onEndDatePickerClick() {
      // 结束日期选择
      this._endDatePicker.show(result => {
        let pickedDate = result.value;
        this.filterEndDate = pickedDate;
      });
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
    refreshTableData() {
      // 还原滚动条，否则会自动loadmore
      this.scrollerDom && (this.scrollerDom.scrollTop = 0);
      if (this.currentPickedNodeInfo && this.currentPickedNodeInfo.text) {
        this.currentPageNumber = 1;
        this.fetchTableData(
          {
            dataTableId: this.currentDataTableId,
            dataPointId: this.currentDataPointId,
            pageNumber: this.currentPageNumber,
            rowsPerPage: this.currentPageSize,
            startTime: this.formattedStartTime,
            endTime: this.formattedEndTime
          },
          (err, data) => {
            if (err) {
              console.log(`获取表格数据请求出错`, err);
              this.rawRecordList = [];
            } else {
              this.rawRecordList = data.recordList;
            }
          }
        );
      }
    },
    loadMoreData(done) {
      console.log("==================================================");
      this.busy = true;
      if (this.currentPickedNodeInfo && this.currentPickedNodeInfo.text) {
        this.fetchTableData(
          {
            dataTableId: this.currentDataTableId,
            dataPointId: this.currentDataPointId,
            pageNumber: this.currentPageNumber + 1,
            rowsPerPage: this.currentPageSize,
            startTime: this.formattedStartTime,
            endTime: this.formattedEndTime
          },
          (err, data) => {
            if (err) {
              console.log(`获取表格数据请求出错`, err);
              this.busy = false;
            } else {
              if (data.recordList.length === 0) {
                done instanceof Function && done(true);
                this.busy = true;
              } else {
                this.rawRecordList = this.rawRecordList.concat(data.recordList);
                this.currentPageNumber++;
                this.busy = false;
                // done();
              }
            }
          }
        );
      }
    },
    onSearchResetButtonClick() {
      // 还原date-picker
      this.filterStartDate = "";
      this.filterEndDate = "";
      // 更新表格数据
      this.refreshTableData();
    },
    onSearchButtonClick() {
      // 还原滚动条，否则会自动loadmore
      this.scrollerDom && (this.scrollerDom.scrollTop = 0);
      // 重置页码
      this.currentPageNumber = 1;
      this.fetchTableData(
        {
          dataTableId: this.currentDataTableId,
          dataPointId: this.currentDataPointId,
          pageNumber: this.currentPageNumber,
          rowsPerPage: this.currentPageSize,
          startTime: this.formattedStartTime,
          endTime: this.formattedEndTime
        },
        (err, data) => {
          if (err) {
            console.log(`获取表格数据请求出错`, err);
          } else {
            this.rawRecordList = data.recordList;
          }
        }
      );
    }
  },
  watch: {
    // 过滤监测点树
    filterText(val) {
      this.$refs.statisticTree.filter(val);
    }
  }
};
</script>

<style lang="less">
.statistic_container {
  .statistic_tree_container {
    position: fixed;
    .tree_scroller {
      left: 0px;
      width: 100vw;
      max-width: 100vw;
      height: calc(~"99vh - 104px");
      overflow: scroll;
    }
    left: 0px;
    width: 100vw;
    max-width: 100vw;
    height: calc(~"99vh - 44px");
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
  .filter_container {
    margin: 0.5vh 0;
    width: 99%;
    text-align: center;
    display: flex;
    flex-flow: row nowrap;
    button {
      margin-left: 5px;
    }
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
  .record_table_container {
    height: calc(~"98vh - 124px");
    overflow: scroll;
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