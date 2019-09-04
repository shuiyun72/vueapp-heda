<template>
    <div class="wq_point_data_card">
        <el-card class="card">
            <div class="content">
                <div class="left">
                  <div  style="position: relative;">
                    <div class="value_text">{{yulv}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unitYulv}}</span>
                        <span class="data_type">{{dataTypeYulv}}</span>
                    </div>
                  </div>
                  <div style="position: relative;border-top: 2px solid #999">
                    <div class="value_text">{{zhuodu}}</div>
                    <div class="right_bottom">
                        <span class="unit">{{unitZhuodu}}</span>
                        <span class="data_type">{{dataTypeZhuodu}}</span>
                    </div>
                  </div>
                </div>
                <div class="right">
                    <div class="top">
                      <div class="row">
                        <span class="label">PH值：</span>
                        <span class="value">{{ph}}</span>
                      </div>
                      <div class="row">
                        <span class="label">电导率：</span>
                        <span class="value">{{diandaolv}}</span>
                      </div>
                      <div class="row">
                        <span class="label">温度：</span>
                        <span class="value">{{wendu}}</span>
                      </div>
                      <div class="row">
                        <span class="label">更新时间：</span><br>
                        <span class="value">{{readTime}}</span>
                      </div>
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
  props: [
    "yulv",
    "unitYulv",
    "dataTypeYulv",
    "zhuodu",
    "unitZhuodu",
    "dataTypeZhuodu",
    "ph",
    "diandaolv",
    "wendu",
    "readTime",
    "pointAddress"
  ]
};
</script>


<style lang="less">
.wq_point_data_card {
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
          line-height: 5rem;
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
        font-size: 1.1rem;
        .top {
          .label {
            color: #999;
            display: inline-block;
            min-width: 30%;
          }
          .value {
            display: inline-block;
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

