<!-- 基于 h5 plus的图片上传组件 -->
<template>
  <div class="picture_uploader_plus_container">
    <div
      class="preview_container"
      :key="index"
      v-for="(pictureInfo, index) in pictureList"
      @tap="onPreviewClick(index)"
      @longtap="onPreviewLongtap(index)"
    >
      <img :src="pictureInfo.fileName" />
    </div>
    <div class="picture_uploader_rel" :style="!enableButtonForm?'position: relative;':''">
      <el-button v-if="enableButton" @click.native="onAddPictrueClick">添加图片</el-button>
      <input
        type="file"
        accept="image/*"
        capture="camera"
        id="imgPhoto"
        :class="!enableButtonForm?'imgPhoto':'imgPhotoForm'"
        v-if="!isPlus"
        @click="onAddPictrueInput"
      /> 
    </div>
    <div class="flex_pic" @tap="onPicHide" v-show="isPicShow">
      <img src="" class="flex_pic_img">
    </div>
  </div>
</template>

<script>
import { deepCopy } from "@common/util";
import dateHelper from "@common/dateHelper";
import _ from "lodash";

export default {
  props: {
    enableButton: {
      type: Boolean,
      default: false
    },
    enableButtonForm:{
      type: Boolean,
      default: false
    },
    uploadLimit: {
      type: Number,
      default: 2
    }
  },
  data() {
    return {
      pictureList: [],
      isPicShow:false
    };
  },
  computed:{
    isPlus(){
      if(window.plus){
        return true
      }else{
        return false
      }
    }
  },
  methods: {
    onAddPictrueInput(el) {
      this.openCamera(this.parsePictureFile);
    },
    onAddPictrueClick() {
      if (this.isPlus) {
        window.mui.plusReady(() => {
          window.plus.nativeUI.actionSheet(
            {
              cancel: "取消",
              buttons: [{ title: "拍照" }, { title: "从相册中选择" }]
            },
            event => {
              switch (event.index) {
                case 1:
                  // 如果已选取的图片达到上限，无法继续拍照
                  if (this.pictureList.length == this.uploadLimit) {
                    window.plus.nativeUI.alert(
                      `您最多可以选择${this.uploadLimit}张图片`
                    );
                  } else {
                    // 打开拍照并返回拍摄的图片对象
                    this.openCamera(this.parsePictureFile);
                  }
                  break;
                case 2:
                  // 打开相册并返回选取的图片对象
                  this.openGallery(this.parsePictureFile);
                  break;
              }
            }
          );
        });
      } else {
        this.openCamera(this.parsePictureFile);
      }
    },
    // 打开手机拍照，可通过callback拿到拍摄的图片对象
    openCamera(callback) {
      if (this.pictureList.length >= this.uploadLimit) {
        this.$emit("excceed");
        window.mui.toast(`您最多可以上传${this.uploadLimit}张图片`, {
          duration: "2000",
          type: "div"
        });
        return;
      }
      this.$emit("camera-open");
      if (this.isPlus) {
        window.plus.camera.getCamera().captureImage(
          picturePath => {
            this.$emit("captured");
            window.plus.io.resolveLocalFileSystemURL(
              picturePath,
              entry => {
                console.log("entry");
                entry.file(fileObj => {
                  console.log("fileObj", fileObj);
                  // this.pictureList.push({
                  //   fileName: fileObj.fullPath,
                  //   base64: ""
                  // });
                  if (!(callback instanceof Function)) {
                    callback = this.parsePictureFile;
                  }
                  callback(fileObj);
                });
              },
              err => {
                window.plus.nativeUI.alert("读取图片数据失败");
              }
            );
          },
          err => {
            window.plus.nativeUI.alert("拍摄已取消");
          },
          {
            // 拍照保存路径
            filename: "_doc/camera/"
          }
        );
      } else {
        console.log("else window.plus");
        var _this = this;
        // document.getElementById("imgPhoto").click();
        document.getElementById("imgPhoto").onchange = function() {
          var reader = new FileReader();
          console.log(reader);
          reader.onload = function(e) {
            //调用图片压缩方法：compress();
          };
          reader.readAsDataURL(this.files[0]);
          var fileSize = Math.round(this.files[0].size / 1024 / 1024); //以M为单位
          //this.files[0] 该信息包含：图片的大小，以byte计算 获取size的方法如下：this.files[0].size;
          if (!(callback instanceof Function)) {
            callback = _this.parsePictureFile;
          }
          callback(this.files[0]);
        };
      }
    },
    // 点击预览区的任一张预览图片时
    onPreviewClick(index = 0) {
      let picImg = document.getElementsByClassName("flex_pic_img")[0];
      picImg.src = this.pictureList[index].fileName;
      this.isPicShow = true;
    },
    onPicHide(){
      let _this = this;
      setTimeout(function(){
        _this.isPicShow = false;
      },100) 
    },
    // 长按预览图片
    onPreviewLongtap(tapIndex) {
      if (this.isPlus) {
        window.mui.plusReady(() => {
          window.plus.nativeUI.actionSheet(
            {
              cancel: "取消",
              buttons: [{ title: "删除", color: "red" }]
            },
            event => {
              switch (event.index) {
                case 1:
                  // 删除图片
                  this.pictureList = this.pictureList.filter(
                    (picture, index) => {
                      return index !== tapIndex;
                    }
                  );
                  this.$emit("change", deepCopy(this.pictureList));
                  break;
              }
            }
          );
        });
      } else {
        var _this = this;
        window.mui.confirm("确定删除图片？", function(e) {
          if (e.index == 1) {
            _this.pictureList.splice(tapIndex, 1);
            window.mui.toast("删除成功");
          } else {
            window.mui.toast("取消删除");
          }
        });
      }
    },
    // 打开手机相册，可通过callback拿到选取的的图片对象
    openGallery(callback) {
      let _this = this;
      window.plus.gallery.pick(
        event => {
          _this.pictureList = [];
          let picturesPathList = event.files;
          _.each(picturesPathList, picturePath => {
            window.plus.io.resolveLocalFileSystemURL(
              picturePath,
              entry => {
                entry.file(fileObj => {
                  if (!(callback instanceof Function)) {
                    callback = _this.parsePictureFile;
                  }
                  callback(fileObj);
                });
              },
              err => {
                window.plus.nativeUI.alert("读取图片数据失败");
              }
            );
          });
        },
        err => {
          window.plus.nativeUI.alert("选取图片操作失败");
        },
        {
          filter: "image",
          system: false,
          selected: _this.pictureList.map(pictureInfo => {
            return pictureInfo.fileName;
          }),
          multiple: true,
          maximum: _this.uploadLimit,
          onmaxed() {
            window.plus.nativeUI.alert(
              `您最多可以选择${_this.uploadLimit}张图片`
            );
          }
        }
      );
    },
    // 通过File Reader读取图片的fileName和base64编码，并保存到data.pictureList
    parsePictureFile(fileObj) {
      console.log("begin");
      var reader; 
      if(this.isPlus){
        reader = new window.plus.io.FileReader();
      }else{
        reader = new FileReader();
      }
      var image = new Image();
      var canvas = this.createCanvas();
      var ctx = canvas.getContext("2d");
      reader.readAsDataURL(fileObj, "utf-8");
      var _this = this;
      console.log(fileObj);
      reader.onloadend = event => {
        var result = event.target.result;
        var resultLast = "";

        image.onload = function() {
          // 图片加载完处理
          var imgScale = _this.imgScaleW(1000, image.width, image.height);
          canvas.width = imgScale.width;
          canvas.height = imgScale.height;
          ctx.drawImage(image, 0, 0, imgScale.width, imgScale.height);

          ctx.font = "26px Arial";
          ctx.textAlign = "right";
          ctx.textBaseline = "middle"; //更改字号后，必须重置对齐方式，否则居中麻烦。设置文本的垂直对齐方式
          // 设置文字阴影的颜色为黑色，透明度为90%
          ctx.shadowColor = "rgba(0, 0, 0, 0.9)";
          // 将阴影向右移动15px，向上移动10px
          ctx.shadowOffsetX = 2;
          ctx.shadowOffsetY = 2;
          // 轻微模糊阴影
          ctx.shadowBlur = 2;
          // ctx.textAlign = 'center';
          ctx.fillStyle = "#00ffff";
          var fleft = canvas.width - 20;
          var ftop = canvas.height - 30;
          let date = dateHelper.format(new Date());
          ctx.fillText(date, fleft, ftop);

          var dataURL = canvas.toDataURL("image/jpeg"); // 图片base64
          ctx.clearRect(0, 0, imgScale.width, imgScale.height); // 清除画布
          resultLast = dataURL; //dataURL:处理成功返回的图片base64
          // console.log("end", event, this.pictureList.length);
          _this.pictureList.push({
            fileName: resultLast,
            base64: resultLast
          });
          console.log("push end", _this.pictureList.length);
          // _.find(this.pictureList, { fileName: event.target.fileName }).base64 =
          //   event.target.result;
          // 向父级组件发送change事件以及最新的list数据
          _this.$emit("change", deepCopy(_this.pictureList));
        };
        image.src = result;
      };
      reader.onerror = err => {
        window.plus.nativeUI.alert("拍摄已取消");
      };
    },
    reset() {
      // 删除图片
      this.pictureList = [];
      this.$emit("change", deepCopy(this.pictureList));
      this.isPicShow = false;
    },
    //压缩图片
    createCanvas() {
      // 创建画布
      var canvas = document.getElementById("canvas");
      if (!canvas) {
        var canvasTag = document.createElement("canvas");
        canvasTag.setAttribute("id", "canvas");
        canvasTag.setAttribute("style", "display:none;"); //隐藏画布
        document.body.appendChild(canvasTag);
        canvas = document.getElementById("canvas");
      }
      return canvas;
    },
    imgScaleW(maxWidth, width, height) {
      /* maxWidth:宽度或者高度最大值
       * width：宽度
       * height：高度
       * */
      var imgScale = {};
      var w = 0;
      var h = 0;
      if (width <= maxWidth && height <= maxWidth) {
        // 如果图片宽高都小于限制的最大值,不用缩放
        imgScale = {
          width: width,
          height: height
        };
      } else {
        if (width >= height) {
          // 如果图片宽大于高
          w = maxWidth;
          h = Math.ceil((maxWidth * height) / width);
        } else {
          // 如果图片高大于宽
          h = maxWidth;
          w = Math.ceil((maxWidth * width) / height);
        }
        imgScale = {
          width: w,
          height: h
        };
      }
      return imgScale;
    }
  }
};
</script>

<style lang="less">
.picture_uploader_plus_container {
  margin: 0 auto;
  .preview_container {
    margin: 1%;
    // width: 45vw;
    // height: 45vw;
    width: 30%;
    // height: ;
    display: inline-block;
    img {
      width: 100%;
      border: 1px solid royalblue;
      border-radius: 10px;
      // height: inherit;
    }
  }
}
.picture_uploader_rel {
  display: inline-block;
  vertical-align: top;
  .camerat_button_form{
    display: inline-block;
    position: absolute;
    opacity: 1;
    top: 0;
    left: 0;
    width: 98px;
    height: 40px;
  }
  .imgPhoto {
    position: absolute;
    opacity: 0;
    top: 0;
    left: 0;
    width: 98px;
    height: 40px;
  }
  .imgPhotoForm {
    position: absolute;
    opacity: 0;
    bottom: 0;
    left: 50%;
    width: 40%;
    margin-left:-42%;
    height: 40px;
    z-index:2;
  }
}
.flex_pic{
  width: 100%;
  height: calc(100vh);
  background: #eee;
  z-index: 1000;
  position:fixed;
  top: 0;
  left: 0;
  display: flex;
  justify-content:center;
  align-items: center
}
.flex_pic_img{
  width: 100%;
  position: absolute;
}
</style>


