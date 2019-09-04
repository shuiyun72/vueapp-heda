<template>
    <div class="features_container">
        <header class="mui-bar mui-bar-nav dodger_blue_style">
            <a class="mui-action-back mui-icon mui-icon-left-nav mui-pull-left"></a>
            <h1 class="mui-title">{{headerText}}</h1>
        </header>
        <div class="feature_container">
            <router-view ref="currentPage"></router-view>
        </div>
    </div>
</template>

<script>
// 单个功能的页面
export default {
  name: "FeaturePage",
  // 路由切换到此页面前一刻执行的钩子函数
  beforeRouteEnter(to, from, next) {
    next(instance => {
      console.log('$route', instance.$route);
      // 定义在router中的meta属性可以在组件中通过this.$router访问到
      instance.headerText = instance.$route.meta.title;
    });
  },
  beforeRouteUpdate(to, from, next) {
    this.headerText = to.meta.title;
    next();
  },
  created() {
    this.$eventbus.$on("set-title", title => {
      this.changeTitle(title);
    });
  },
  mounted() {},
  beforeDestroy() {
    this.$eventbus.$off("set-title");
  },
  data() {
    return {
      // 顶部标题栏文字
      headerText: ""
    };
  },
  methods: {
    changeTitle(title) {
      this.headerText = title;
    }
  }
};
</script>

<style lang="less" scoped>
.features_container {
  /* 由于顶部标题栏是fixed定位，所以此处要定义margin-top */
  margin-top: calc(~"1vh + 44px");
  /* 这里定制了一个新类，如果不给header添加dodger_blue_style类，将使用mui默认的灰色导航条 */
  header.dodger_blue_style {
    height: calc(~"1vh + 44px");
    background-color: #001d26;
    .mui-action-back,
    .mui-title {
      color: white;
    }
  }
}
</style>


