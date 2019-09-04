<template>
    <div class="point_data_card">
        <el-card class="card">
            <div class="content">
                <div class="left">
                    <div class="value_text">{{value}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unit}}</span>
                        <span class="data_type">{{dataType}}</span>
                    </div>
                </div>
                <div class="right">
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
    value: {
      type: [String, Number],
      default: ""
    },
    unit: {
      type: String,
      default: ""
    },
    dataType: {
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
.point_data_card {
  .card {
    &.el-card {
      border-left: 4px solid #00afa9;
      margin-bottom: 5px;
      .el-card__body {
          padding: 0
      }
    }
    .content {
      .left {
        display: inline-block;
        width: 39%;
        min-height: 100px;
        background-color: #E8E7E7;
        position: relative;
        vertical-align: bottom;
        .value_text {
          font-size: 2.2rem;
          font-weight: bold;
          text-align: center;
          line-height: 100px;
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

