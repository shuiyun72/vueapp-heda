<template>
  <div class="company_info_home">
    <div class="home_navbar">
      <div class="navbar_header">
        <div class="color_box"></div>
        <span>信息导航</span>
      </div>
      <div class="navbar_body">
        <div class="mui-row">
          <!-- 功能项 -->
          <ModuleItem
            v-for="item in navItems"
            :key="item.id"
            :title="item.title"
            :desc="item.desc"
            :mode="item.mode"
            :withBorder="item.withBorder"
            :picture="item.picture"
            :pictureContainerStyle="item.pictureContainerStyle"
            :class="item.class"
            @click="switchToListPage(item)"
            enabled
          ></ModuleItem>
        </div>
      </div>
    </div>
    <!-- <div class="latest_container">
      <NewsList :newsList="newsList"></NewsList>
    </div> -->
  </div>
</template>

<script>
import apiCompany from "@api/company";
import ModuleItem from "@comp/common/ModuleItem";
import NewsList from "./NewsList";
import $ from "jquery";
import _ from "lodash";

const STATIC_URL = "./static/images/company-info/";
const PNG = ".png";
const NAV_ITEM_BASIC_CONFIG = {
  index: 0,
  title: "",
  mode: "vertical",
  destination: "StateSummary",
  picture: "./static/images/company-info/gongsitongzhi.png",
  class: "mui-col-sm-3 mui-col-xs-3"
};
function parseUnicode(str) {
  return unescape(str.replace(/u/gi, "%u"));
}
export default {
  beforeRouteEnter(to, from, next) {
    next(instance => {
      // 获取导航栏目数据，再获取最新信息
      instance.getNavbarData().then(instance.getLatestNewList);
    });
  },
  updated() {
    // 计算下方列表的高度
    this.calcNewsListHeight();
  },
  data() {
    return {
      navItems: [],
      newsList: []
    };
  },
  methods: {
    getNavbarData() {
      return apiCompany.GetCompanyInfoType().then(res => {
        let rawList = res.data;
        let list = _.map(rawList, (rawItem, index) => {
          return Object.assign({}, NAV_ITEM_BASIC_CONFIG, {
            index: index + 1,
            typeId: rawItem.RI_TypeID,
            title: rawItem.RI_TypeName,
            // picture: `${STATIC_URL}${rawItem.RI_TypeName}${PNG}`
            picture: `${STATIC_URL}gongsitongzhi${PNG}`
          });
        });
        console.log("type", list);
        this.navItems = list;
      });
    },
    getLatestNewList() {
      apiCompany.GetCompanyInfoList().then(res => {
        console.log("list", res);
        let newsList = _.map(res.data.rows, rawNews => {
            // console.log(rawNews.RI_TypeID)
          return {
            newsId: rawNews.RI_InfoID,
            newsTitle: rawNews.RI_Heading,
            newsContent: parseUnicode(rawNews.RI_Content),
            newsType: _.find(this.navItems, { typeId: rawNews.RI_TypeID }) ? _.find(this.navItems, { typeId: rawNews.RI_TypeID })
              .title : '无',
            newsPublishTime: rawNews.RI_UpdateTime,
            enableTypeTag: true
          };
        });
        console.log("news", newsList);
        this.newsList = newsList
      });
    },
    switchToListPage(navItem) {},
    calcNewsListHeight() {
      let newListHeight =
        $(window).height() -
        $(".home_navbar").height() -
        parseFloat($(".home_navbar").css("margin-top")) -
        4;
      $(".latest_container").css("height", newListHeight + "px");
    }
  },
  components: {
    ModuleItem,
    NewsList
  }
};
</script>

<style lang="less">
.company_info_home {
  .home_navbar {
    margin-top: calc(~"1vh + 48px");
    background-color: #fff;
    .navbar_header {
      padding: 6px 2px;
      .color_box {
        background-color: #01273a;
        width: 10px;
        height: 17px;
        display: inline-block;
        vertical-align: middle;
        & + span {
          vertical-align: middle;
        }
      }
    }
  }
  .latest_container {
    margin-top: 4px;
    overflow: scroll;
  }
}
</style>