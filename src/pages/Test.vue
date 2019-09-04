<template>
<div>
    <div style="margin:10px 0; border: 1px solid #888; width:100vw;" v-for="item in list" :key="item.time">
        <span>{{item}}</span>
    </div>
</div>
</template>



<script>
import apiInspection from "@api/inspection";
import dateHelper from "@common/dateHelper";

export default {
  name: "Test",
  created() {},
  mounted() {
    // api.UploadLocation(
    //   0,
    //   0,
    //   dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
    //   215,
    //   1
    // );
    this.fun();
  },
  data() {
    return {
      list: []
    };
  },
  methods: {
    fun() {
      apiInspection
        .UploadLocation(
          0,
          0,
          dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
          215,
          1
        )
        .then(res => {
          this.list.push(dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"));
          this.fun();
        });
    }
  }
};
</script>

