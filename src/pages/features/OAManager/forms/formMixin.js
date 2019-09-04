import dateHelper from "@common/dateHelper";
import _ from 'lodash'
export default {
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    // 由外部传进来的初始表单值
    initialFormData: {
      type: Object,
      default () {
        return {};
      }
    }
  },
  mounted() {
    // 如果当前表单含有维护日期或者加油日期，为其赋值当前时间（精确到天）
    if ('gasDate' in this.formData) {
      this.formData.gasDate = dateHelper.format(new Date(), "yyyy-MM-dd")
    }
    if ('maintainDate' in this.formData) {
      this.formData.maintainDate = dateHelper.format(new Date(), "yyyy-MM-dd")
    }
    // 实例化date picker组件
    this._datePicker = new window.mui.DtPicker({
      type: "date",
      labels: ["年", "月", "日"]
    });
  },
  beforeDestroy() {
    this._datePicker.dispose();
  },
  data() {
    return {
      formData: {},
      formDataValidation: {},
      // f2bMap: {}
    };
  },
  methods: {
    onDatePickerClick() {
      if (this.disabled) {
        return
      }
      // 日期选择
      this._datePicker.show(result => {
        let pickedDate = result.value;
        if ('gasDate' in this.formData) {
          this.formData.gasDate = pickedDate;
        }
        if ('maintainDate' in this.formData) {
          this.formData.maintainDate = pickedDate;
        }
      });
    },
    //  将父组件传进来的初始值赋给内部的formData，在mounted要执行
    fillInitialFormData() {
      if (_.isEmpty(this.initialFormData)) {
        //pass
      } else {
        console.log("开始填充表单数据！！！！！！", this.initialFormData);
        _.each(_.keys(this.formData), fieldName => {
          this.formData[fieldName] =
            this.initialFormData[fieldName] === undefined ?
            "" :
            this.initialFormData[fieldName];
        });
      }
    },
    getValue() {
      if ('createDate' in this.formData) {
        this.formData.createDate = dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss")
      }
      return Object.assign({}, this.formData);
    },
    // getFormattedValue() {
    //   console.log('getFormattedValue ', this.mapFormData(this.formData, this.f2bMap))
    //   return this.mapFormData(this.formData, this.f2bMap)
    // },
    validate(callback) {
      this.$refs.form.validate(callback);
    },
    clearValidate() {
      this.$refs.form.clearValidate();
    },
    resetFields() {
      this.$refs.form.resetFields();
    },
    mapFormData(data, map) {
      let result = {};
      _.each(_.keys(map), originField => {
        result[map[originField]] = data[originField];
      });
      return result;
    },
  },
  watch: {
    initialFormData() {
      console.log("inital变化！！！");
      this.fillInitialFormData()
    }
  }
}
