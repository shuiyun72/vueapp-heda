<template>
  <!-- 请假表单 -->
  <div class="qingjia_form_container">
    <el-form
      label-position="left"
      id="qingjiaForm"
      ref="form"
      :model="formData"
      :rules="formDataValidation"
      :disabled="disabled"
      @submit.native.prevent
    >
      <el-form-item label="标题" prop="title">
        <el-input name="title" v-model.trim="formData.title" placeholder="请输入请假标题"></el-input>
      </el-form-item>
      <el-form-item label="请假人" prop="personName">
        <el-input name="name" v-model.trim="formData.personName" placeholder="请输入姓名"></el-input>
      </el-form-item>
      <el-form-item label="所在部门" prop="department">
        <el-input name="department" v-model.trim="formData.department" placeholder="请输入部门"></el-input>
      </el-form-item>
      <el-form-item label="开始日期" prop="startTime">
        <el-input
          name="startTime"
          v-model="formData.startTime"
          :editable="false"
          readonly
          placeholder="请输入开始日期"
          @click.native="onStartDatePickerClick"
        ></el-input>
      </el-form-item>
      <el-form-item label="结束日期" prop="endTime">
        <el-input
          name="endTime"
          v-model="formData.endTime"
          :editable="false"
          readonly
          placeholder="请输入结束日期"
          @click.native="onEndDatePickerClick"
        ></el-input>
      </el-form-item>
      <el-form-item label="请假天数" prop="day">
        <el-input name="day" v-model.trim="formData.day" placeholder="请输入请假天数"></el-input>
      </el-form-item>
      <el-form-item label="请假类型" prop="type">
        <el-select name="type" v-model="formData.type" placeholder="请选择类型" style="width: 100%">
          <el-option
            v-for="item in selectOption"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="请假事由" prop="reason">
        <el-input name="reason" type="textarea" v-model="formData.reason" placeholder="请输入请假事由"></el-input>
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
        title: "",
        personName: "",
        department: "",
        startTime: "",
        endTime: "",
        day: "",
        type: "",
        reason: ""
      },
      formDataValidation: {
        title: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        personName: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        department: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        startTime: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        endTime: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        day: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        type: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ],
        reason: [
          {
            required: true,
            message: "该字段不能为空",
            trigger: "blur"
          }
        ]
      },
      selectOption: [
        { label: "事假", value: "事假" },
        { label: "病假", value: "病假" },
        { label: "婚假", value: "婚假" },
        { label: "年假", value: "年假" },
        { label: "陪产假", value: "陪产假" }
      ]
    };
  },
  methods: {
    onStartDatePickerClick() {
      if (this.disabled) {
        return;
      }
      // 开始日期选择
      this._datePicker.show(result => {
        let pickedDate = result.value;
        this.formData.startTime = pickedDate;
      });
    },
    onEndDatePickerClick() {
      if (this.disabled) {
        return;
      }
      // 结束日期选择
      this._datePicker.show(result => {
        let pickedDate = result.value;
        this.formData.endTime = pickedDate;
      });
    }
  }
};
</script>

