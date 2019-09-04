<template>
  <!-- 请假表单 -->
  <div class="baoyang_form_container">
    <el-form
      label-position="left"
      id="baoyangForm"
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
            v-for="item in baoyangSelectOption1"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="维护类型" prop="maintainType">
        <el-select
          name="maintainType"
          v-model="formData.maintainType"
          placeholder="请选择维护类型"
          style="width: 100%"
        >
          <el-option
            v-for="item in baoyangSelectOption2"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="维护人" prop="maintainPerson">
        <el-input
          name="maintainPerson"
          v-model.trim="formData.maintainPerson"
          placeholder="请输入维护人姓名"
        ></el-input>
      </el-form-item>
      <el-form-item label="维护日期" prop="maintainDate">
        <el-input
          name="maintainDate"
          v-model="formData.maintainDate"
          :editable="false"
          readonly
          placeholder="请输入维护日期"
          @click.native="onDatePickerClick"
        ></el-input>
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
      <el-form-item label="保养内容" prop="maintainContent">
        <el-input
          name="maintainContent"
          type="textarea"
          v-model="formData.maintainContent"
          placeholder="请输入保养内容"
        ></el-input>
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
        maintainType: "",
        maintainPerson: "",
        maintainDate: "",
        cost: "",
        distance: "",
        attachment: "",
        createPerson: "",
        createDate: "",
        maintainContent: ""
      },
      formDataValidation: {
        plateNumber: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        maintainType: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        maintainPerson: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        maintainDate: [
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
        maintainContent: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ]
      },
      // 车牌号
      baoyangSelectOption1: [
        { label: "A12345", value: "A12345" },
        { label: "A12348", value: "A12348" }
      ],
      // 类型
      baoyangSelectOption2: [
        { label: "日常", value: "日常" },
        { label: "定期", value: "定期" },
        { label: "发动机维修", value: "发动机维修" },
        { label: "底盘维修", value: "底盘维修" }
      ]
    };
  }
};
</script>