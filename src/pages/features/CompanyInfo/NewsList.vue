<template>
  <div class="news_list_container">
    <NewsSummary
      class="news_summary_container"
      v-for="news in newsList"
      :detailInfo="news"
      :key="news.newsId"
      :enableTypeTag="news.enableTypeTag"
    ></NewsSummary>
  </div>
</template>

<script>
import NewsSummary from "./NewsSummary";
export default {
  props: {
    newsList: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  components: { NewsSummary }
};
</script>

<style lang="less">
.news_list_container {
  background-color: #fff;
  .news_summary_container {
    &:nth-child(2n-1) {
      .main_part {
        background-color: #fff;
      }
    }
    &:nth-child(2n) {
      .main_part {
        background-color: #cdcfd0;
        border-radius: 10px;
        div.type_tag {
          border-bottom-left-radius: 10px;
        }
      }
    }
  }
}
</style>

