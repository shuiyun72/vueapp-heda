<!-- 签到记录的详情页面 -->
<template>
   <div class="sign_in_detail_container">
       <el-card>
           <div class="clearfix" slot="header">
                <span>{{detailInfo.title || '签到详情'}}</span>
            </div>
            <ul class="info_list">
                <li><span class="label">签到日期：</span><span class="content">{{detailInfo.date || '暂无'}}</span></li>
                <li><span class="label">签到时间：</span><span class="content">{{detailInfo.comTime || '暂无'}}</span></li>
                <li><span class="label">签到地点：</span><span class="content">{{detailInfo.comAddr || '暂无'}}</span></li>
                <li><span class="label">签退时间：</span><span class="content">{{detailInfo.leaveTime || '暂无'}}</span></li>
                <li><span class="label">签退地点：</span><span class="content">{{detailInfo.leaveAddr || '暂无'}}</span></li>
              
                <!-- <li><span class="label">创建时间：</span><span class="content">{{detailInfo.orderCreateTime || '暂无'}}</span></li> -->
            </ul>
       </el-card>     
    </div>
</template>

<script>
export default {
  props: {
    detailInfo: {
      require: true,
      type: Object
    }
  }
};
</script>

<style lang="less" scoped>
.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
.clearfix:after {
  clear: both;
}

.sign_in_detail_container {
  .el-card {
    margin-bottom: 10px;
  }
  .info_list {
    list-style-type: none;
    padding: 0px;
    li {
      border-bottom: 1px solid #0070c0;
      margin-bottom: 20px;
    }
    .content {
      display: inline-block;
      margin: 10px;
    }
  }
}
</style>


