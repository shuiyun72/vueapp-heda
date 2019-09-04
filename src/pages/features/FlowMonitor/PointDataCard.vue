<template>
    <div class="flow_point_data_card">
        <el-card class="card">
            <div class="content">
                <div class="left" v-if="dataType2">
                  <div  style="position: relative;">
                    <div class="value_text">{{value1}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unit1}}</span>
                        <span class="data_type">{{dataType1}}</span>
                    </div>
                  </div>
                  <div style="position: relative;border-top: 2px solid #999">
                    <div class="value_text">{{value2}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unit2}}</span>
                        <span class="data_type">{{dataType2}}</span>
                    </div>
                  </div>
                </div>
                <div class="left" v-else>
                  <div  style="position: relative;">
                    <div class="value_text" style="line-height: 100px;">{{value1}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unit1}}</span>
                        <span class="data_type">{{dataType1}}</span>
                    </div>
                  </div>
                </div>
                <div class="right" >
                    <div class="top">
                        <span class="time_label">更新时间：</span><br>
                        <span class="time_value">{{readTime}}</span>
                    </div>
                    <div class="bottom">
                        <span class="point_address">{{pointAddress}}</span>
                    </div>
                </div>
            </div>
        </el-card>
    </div>   
</template>

<script>
export default {
  props: {
    // 主要数据的值，比如当前压力数据，正瞬时流量等等，对应卡片左边部分的主要内容
    value1: {
      type: [String, Number],
      default: ""
    },
    unit1: {
      type: String,
      default: ""
    },
    dataType1: {
      type: String,
      default: ""
    },
    value2: {
      type: [String, Number],
      default: ""
    },
    unit2: {
      type: String,
      default: ""
    },
    dataType2: {
      type: String,
      default: ""
    },
    readTime: {
      type: String,
      default: ""
    },
    pointAddress: {
      type: String,
      default: ""
    }
  }
};
</script>


<style lang="less">
.flow_point_data_card {
  .card {
    &.el-card {
      border-left: 4px solid #00afa9;
      margin-bottom: 5px;
      .el-card__body {
        padding: 0;
      }
    }
    .content {
      .left {
        display: inline-block;
        width: 39%;
        min-height: 100px;
        background-color: #e8e7e7;
        position: relative;
        vertical-align: bottom;
        .value_text {
          font-size: 2.2rem;
          font-weight: bold;
          text-align: center;
          line-height: 4.6rem;
        }
        .right_bottom {
          position: absolute;
          right: 2%;
          bottom: 2%;
          .unit {
            color: #00afa9;
          }
        }
      }
      .right {
        display: inline-block;
        width: 59%;
        vertical-align: bottom;
        .top {
          .time_label {
            color: #999;
          }
        }
        .bottom {
          text-align: right;
          padding-top: 2%;
          border-top: 1px solid #ddd;
          .point_address {
            font-size: 1.2rem;
            color: #00afa9;
          }
        }
      }
    }
  }
}
</style>

