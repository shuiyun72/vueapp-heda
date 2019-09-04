<template>
    <div class="DMA-statistic-root">
        <!-- 时间选择器 -->
        <div class="date_picker_container">
            <el-input
                v-model="pickerStartDate"
                placeholder="开始日期"
                class="date_picker_padding_left_5"
                readonly
                @tap.native="onStartDatePickerClick"
            >
            </el-input>
            <el-input
                v-model="pickerEndDate"
                placeholder="结束日期"
                class="date_picker_padding_left_5"
                style="margin-left: 5px;"
                readonly
                @tap.native="onEndDatePickerClick"
            >
            </el-input>
            <el-button 
                icon="el-icon-search" 
                type="primary" 
                circle
                @tap.native="onSearchButtonClick"
            ></el-button>
        </div>
        <!-- DMA统计数据表 -->
        <div class="tree_table_container">
						<TreeGrid 
						    style="height:100%"
                :border="false"
							  :data="tableData" 
						  	idField="RID" 
							  treeField="text"
                frozenWidth="100px"
                virtualScroll
						>
								<GridColumn field="text" title="分区名称" frozen></GridColumn>
								<GridColumn field="CXC" title="参考产销差"></GridColumn>
								<GridColumn field="SumSiteFlow" title="供水总量(m³)"></GridColumn>
								<GridColumn field="SumUserFlow" title="参考用户抄见量(m³)"></GridColumn>
								<GridColumn field="FlowIn" title="进水量(m³)"></GridColumn>
								<GridColumn field="FlowOut" title="出水量(m³)"></GridColumn>
								<GridColumn field="Sum_SiteLosses" title="无收益水量(m³)"></GridColumn>
						</TreeGrid>
				</div>
    </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import dateHelper from "@common/dateHelper";
import apiMonitor from "@api/monitor";
// 当月
const CURRENT_MONTH = dateHelper.format(new Date(), "yyyy-MM");
export default {
  mounted() {
    this.refreshTableData();
    // 实例化date picker组件
    this._startDatePicker = new window.mui.DtPicker({
      type: "month",
      labels: ["年", "月"]
    });
    this._endDatePicker = new window.mui.DtPicker({
      type: "month",
      labels: ["年", "月"]
    });
  },
  beforeDestroy() {
    this._startDatePicker.dispose();
    this._endDatePicker.dispose();
  },
  data() {
    return {
      pickerStartDate: CURRENT_MONTH,
      pickerEndDate: CURRENT_MONTH,
      tableData: []
    };
  },
  computed: {
    paramStartDate() {
      return `${this.pickerStartDate}-01 00:00:00`;
    },
    paramEndDate() {
      return `${this.pickerEndDate}-31 23:59:59`;
    }
  },
  methods: {
    refreshTableData() {
      this.$showLoading()
      apiMonitor
        .GetDMAStatistics(this.paramStartDate, this.paramEndDate)
        .then(res => {
          console.log("RES ", res);
          this.tableData = res.data;
          this.$hideLoading()
        })
        .catch(err => {
          console.log("ERR ", err);
          this.$hideLoading()
        });
    },
    onStartDatePickerClick() {
      // 开始日期选择
      this._startDatePicker.show(result => {
        console.log(result);
        let pickedDate = result.value;
        this.pickerStartDate = pickedDate;
      });
    },
    onEndDatePickerClick() {
      // 结束日期选择
      this._endDatePicker.show(result => {
        let pickedDate = result.value;
        this.pickerEndDate = pickedDate;
      });
    },
    onSearchButtonClick() {
      this.refreshTableData()
    }
  }
};
</script>

<style lang="less" scoped>
.DMA-statistic-root {
  margin-top: calc(~"1.5vh + 44px");
  .date_picker_container {
    margin: 0.5vh 0;
    width: 99%;
    text-align: center;
    display: flex;
    flex-flow: row nowrap;
    button {
      margin-left: 5px;
    }
  }
  .tree_table_container {
    height: calc(~"98.5vh - 88px");
    width: 100%;
  }
}
</style>


