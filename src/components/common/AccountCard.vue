<template>
    <div class="account_card">
        <div class="user_avatar"><img :src="avatarPath" alt=""></div>
        <div class="user_info">
            <span class="name">{{userName}}</span><br><span class="role">{{roleName}}</span>
        </div>
        <slot name="action"></slot>
    </div>
</template>

<script>
export default {
  props: {
    userName: String,
    roleName: String,
    avatarPath: String
  }
};
</script>

<style lang="less" scoped>
div.account_card {
  & > * {
    display: inline-block;
    vertical-align: bottom;
  }
  div.user_avatar {
    img {
      vertical-align: bottom;
      /* 头像图片为圆形 */
      border-radius: 50%;
      width: 60px;
      height: 60px;
    }
  }
  div.user_info {
    margin-right: 3%;
    /* 用户名文本span */
    span.name {
      color: white;
    }
    /* 角色名文本span */
    span.role {
      color: #009688;
      font-size: 0.8em;
    }
  }
}
</style>


