<template>
  <!-- 请假表单 -->
  <div class="jiayou_form_container">
    <el-form
      label-position="left"
      id="jiayouForm"
      ref="form"
      :model="formData"
      :rules="formDataValidation"
      :disabled="disabled"
      @submit.native.prevent
    >
      <el-form-item label="车牌号" prop="plateNumber">
        <el-select
          name="plateNumber"
          v-model="formData.plateNumber"
          placeholder="请选择车牌号"
          style="width: 100%"
        >
          <el-option
            v-for="item in jiayouSelectOption1"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="加油型号" prop="gasType">
        <el-select
          name="gasType"
          v-model="formData.gasType"
          placeholder="请选择加油型号"
          style="width: 100%"
        >
          <el-option
            v-for="item in jiayouSelectOption2"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="加油人" prop="gasPerson">
        <el-input name="gasPerson" v-model.trim="formData.gasPerson" placeholder="请输入加油人姓名"></el-input>
      </el-form-item>
      <el-form-item label="加油日期" prop="gasDate">
        <el-input
          name="gasDate"
          v-model="formData.gasDate"
          :editable="false"
          readonly
          placeholder="请输入加油日期"
          @click.native="onDatePickerClick"
        ></el-input>
      </el-form-item>
      <el-form-item label="加油量（升）" prop="litre">
        <el-input name="litre" v-model.trim="formData.litre" placeholder="请输入加油量"></el-input>
      </el-form-item>
      <el-form-item label="单价（元）" prop="price">
        <el-input name="price" v-model.trim="formData.price" placeholder="请输入单价"></el-input>
      </el-form-item>
      <el-form-item label="花费金额" prop="cost">
        <el-input name="cost" v-model.trim="formData.cost" placeholder="请输入花费金额"></el-input>
      </el-form-item>
      <el-form-item label="公里总数" prop="distance">
        <el-input name="distance" v-model.trim="formData.distance" placeholder="请输入公里总数"></el-input>
      </el-form-item>
      <!-- <el-form-item label="附件" prop="attachment">
        <el-input name="attachment" v-model.trim="formData.attachment" placeholder="请上传附件"></el-input>
      </el-form-item>-->
      <el-form-item label="创建人" prop="createPerson">
        <el-input name="createPerson" v-model.trim="formData.createPerson" placeholder="请输入创建人"></el-input>
      </el-form-item>
      <!-- <el-form-item label="创建日期" prop="createDate">
        <el-input name="createDate" v-model.trim="formData.createDate" placeholder="请输入创建日期"></el-input>
      </el-form-item>-->
      <el-form-item label="备注" prop="comments">
        <el-input name="comments" type="textarea" v-model="formData.comments" placeholder="请输入备注内容"></el-input>
      </el-form-item>
      <el-form-item class="center_content">
        <slot name="bottom"></slot>
        <!-- <el-button type="success" :disabled="!buttonEnabled.qingjia" @click="onQJSaveClick">保存信息</el-button>
        <el-button type="primary" :disabled="!buttonEnabled.qingjia" @click="onQJSubmitClick">发送信息</el-button>-->
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import _ from "lodash";
import formMixin from "./formMixin";
export default {
  mixins: [formMixin],
  data() {
    return {
      formData: {
        plateNumber: "",
        gasType: "",
        gasPerson: "",
        gasDate: "",
        litre: "",
        price: "",
        cost: "",
        distance: "",
        attachment: "",
        createPerson: "",
        createDate: "",
        comments: ""
      },
      formDataValidation: {
        plateNumber: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        gasType: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        gasPerson: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        gasDate: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        litre: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        price: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        cost: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        distance: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        attachment: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        createPerson: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        createDate: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        comments: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ]
      },
      // 车牌号
      jiayouSelectOption1: [
        { label: "A12345", value: "A12345" },
        { label: "A12348", value: "A12348" }
      ],
      // 类型
      jiayouSelectOption2: [
        { label: "92号", value: "92号" },
        { label: "95号", value: "95号" },
        { label: "98号", value: "98号" }
      ]
    };
  }
};
</script>

