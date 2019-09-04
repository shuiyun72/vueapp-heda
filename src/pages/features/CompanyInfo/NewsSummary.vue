<template>
  <div class="news_summary_container" ref="container_dom">
    <div class="left_part">
      <div class="left_part_ball"></div>
    </div>
    <div class="main_part">
      <div class="header">{{detailInfo.newsTitle}}</div>
      <!-- <div class="body" v-html="detailInfo.newsContent"></div> -->
      <span class="body"></span>
      <div class="footer">
        <div class="type_tag" v-show="enableTypeTag">{{detailInfo.newsType}}</div>
        <div class="date_tag">发布日期： {{detailInfo.newsPublishTime | dateFormatter}}</div>
      </div>
    </div>
  </div>
</template>

<script>
import dateHelper from "@common/dateHelper";

function setStrToDom(s, el) {
  let n = el.offsetHeight;
  for (let i = 0; i < s.length; i++) {
    el.innerHTML = s.substr(0, i);
    if (n < el.scrollHeight) {
      el.style.overflow = "hidden";
      el.innerHTML = s.substr(0, i - 3) + "...";
      break;
    }
  }
}
export default {
  props: {
    detailInfo: {
      type: Object,
      required: true,
      default() {
        return {};
      }
    },
    enableTypeTag: {
      type: Boolean,
      default: false
    }
  },
  mounted() {
    
    console.log('mounted')
    let content =
      `哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈
      哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈
      嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎
      嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎
      哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈`;
    setStrToDom(this.detailInfo.newsContent || content, this.$refs.container_dom);
  },
  undated() {
    console.log('update')
    let content =
      `哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈
      哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈
      嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎
      嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎嘎
      哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈`;
    setStrToDom(content, this.$refs.container_dom);
  },
  data() {
    return {};
  },
  filters: {
    dateFormatter(date) {
      return dateHelper.format(new Date(date), "yyyy-MM-dd hh:mm:ss");
    }
  }
};
</script>

<style lang="less">
.news_summary_container {
  width: 100%;
  height: 18vh;
  padding-top: 2%;
  display: flex;
  flex-flow: row nowrap;
  .left_part {
    margin-right: 2%;
    margin-left: 2%;
    .left_part_ball {
      width: 15px;
      height: 15px;
      border-radius: 100px;
      background: #495058;
    }
  }
  .main_part {
    // display: flex;
    // flex-flow: column nowrap;
    flex-grow: 1;
    max-width: 90vw;
    height: 100%;
    & > div {
      margin-top: 1%;
    }
    .header {
      width: 90%;
      margin-left: auto;
      margin-right: auto;
      text-align: center;
      overflow: hidden;
      text-overflow: ellipsis;
      height: 1.3em;
      line-height: 1.3em;
      white-space: nowrap;
      font-size: 15px;
      font-weight: bold;
    }
    .body {
      height: calc(~"92% - 44px");
      overflow: hidden;
      text-overflow: ellipsis;
      padding: 0 2%;
      display: block;
    }
    .footer {
      display: flex;
      flex-flow: row nowrap;
      justify-content: space-between;
      .type_tag {
        background-color: #0070c0;
        color: #fff;
        padding: 1px 5px;
        width: fit-content;
      }
      .date_tag {
        color: #666;
        padding: 1px 5px;
        width: fit-content;
      }
    }
  }
}
</style>


