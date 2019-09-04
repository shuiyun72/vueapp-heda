<template>
  <div class="assign_form_container">
    <div v-if="isCurrentLastStep">当前是审核流程的最后一步，确定给予审核通过？</div>
    <MuiList v-else :items="listItems" @row-click="onListRowClick"></MuiList>
  </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import MuiList from "@comp/common/MuiList.vue";
import dateHelper from "@common/dateHelper";
import apiOA from "@api/OA";
export default {
  props: {
    nextStepList: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  mounted() {
    // 实例化picker组件
    this._popPicker = new window.mui.PopPicker({ layer: 1 });
    // 实例化date picker组件
    // this._datePicker = new window.mui.DtPicker({
    //   type: "datetime",
    //   // beginDate: new Date(2015, 4, 25),
    //   // endDate: new Date(2018, 11, 25),
    //   labels: ["年", "月", "日", "时", "分"]
    // });
    // 调用部门人员接口，填充部门选择器和人员选择器
    this.getDeptAndStaffList();
  },
  data() {
    return {
      // 值与listItems一样，但用于该组件还原默认值操作
      defaultListItems: [
        {
          id: "next",
          label: "下一步",
          labelClass: "gray",
          placeholder: "请选择下一步流程",
          content: "",
          saperate: true,
          withDefaultIcon: true
        },
        {
          id: "department",
          label: "部门",
          labelClass: "gray",
          placeholder: "点击选择",
          content: "",
          saperate: true,
          withDefaultIcon: true
        },
        {
          id: "staff",
          label: "人员",
          labelClass: "gray",
          placeholder: "点击选择",
          content: "",
          saperate: true,
          withDefaultIcon: true
        }
        // {
        //   id: "date",
        //   label: "时间",
        //   labelClass: "gray",
        //   placeholder: "请选择时间",
        //   content: dateHelper.format(this.defaultTime, "yy-MM-dd hh:mm:ss"),
        //   saperate: true,
        //   withDefaultIcon: true
        // }
      ],
      listItems: [
        {
          id: "next",
          label: "下一步",
          labelClass: "gray",
          placeholder: "请选择下一步流程",
          content: "",
          saperate: true,
          withDefaultIcon: true
        },
        {
          id: "department",
          label: "部门",
          labelClass: "gray",
          placeholder: "点击选择",
          content: "",
          saperate: true,
          withDefaultIcon: true
        },
        {
          id: "staff",
          label: "人员",
          labelClass: "gray",
          placeholder: "点击选择",
          content: "",
          saperate: true,
          withDefaultIcon: true
        }
        // {
        //   id: "date",
        //   label: "时间",
        //   labelClass: "gray",
        //   placeholder: "请选择时间",
        //   content: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm"),
        //   saperate: true,
        //   withDefaultIcon: true
        // }
      ],
      //   从接口拿到的部门列表
      departmentList: [],
      pickerValue: {
        next: {},
        department: {},
        staff: {}
        // date: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm") || ""
      },
      //   当前选择部门下的人员列表
      staffPickerItems: []
    };
  },
  computed: {
    departmentPickerItems() {
      return this.departmentList.map(department => {
        return {
          value: department.IDeptID,
          text: department.CDepName
        };
      });
    },
    nextStepPickerItems() {
      return this.nextStepList.map(step => {
        return {
          value: step.nextStepId,
          text: step.nextStepName
        };
      });
    },
    currentPickedDepartmentId() {
      return this.pickerValue.department.value;
    },
    // 当前是否是最后一步
    isCurrentLastStep() {
      return this.nextStepList.length === 0;
    }
  },
  methods: {
    getDeptAndStaffList() {
      apiOA.flow.GetDepartmentAndPersonList().then(res => {
        console.log("人员", res);
        if (res.data && res.data.Data) {
          let departmentList = res.data.Data;
          this.departmentList = departmentList;
        } else {
          mui.toast("获取部门信息失败");
        }
      });
    },
    onListRowClick(row, rowIndex) {
      console.log("rowClick!", row, rowIndex);
      if (rowIndex === 0) {
        // 下一步选择
        this.openPicker(this.nextStepPickerItems, row, rowIndex);
      } else if (rowIndex === 1) {
        // 部门选择
        this.openPicker(this.departmentPickerItems, row, rowIndex);
      } else if (rowIndex === 2) {
        // 人员选择
        if (this.pickerValue["department"] === "") {
          window.mui.toast("请先选择部门");
        } else {
          this.openPicker(this.staffPickerItems, row, rowIndex);
        }
      } else if (rowIndex === 3) {
        // 日期选择
        this._datePicker.show(result => {
          let pickedDate = result.value;
          this.pickerValue[row.id] = pickedDate;
          this.listItems[rowIndex].content = pickedDate;
        });
      }
    },
    // 拿到列表数据后打开picker，并处理后续选择逻辑
    openPicker(pickerItems, row, rowIndex) {
      this._popPicker.setData(pickerItems);
      this._popPicker.show(pickedItems => {
        let pickedItem = pickedItems[0];
        console.log(pickedItem);
        /* 
          如果当前打开的是部门的picker
          则每次选择后判断本次是否与之前选择的事件类型一致
          若发生改变，则应清空下方人员项的内容
        */
        if (row.id === "department") {
          if (pickedItem.value !== this.pickerValue.department.value) {
            _.find(this.listItems, item => {
              return item.id === "staff";
            }).content = "";
          }
        }
        this.pickerValue[row.id] = pickedItem;
        this.listItems[rowIndex].content = pickedItem.text;
      });
    },
    getValue(callback) {
      let data = {
        departmentId: Number(this.currentPickedDepartmentId),
        assigneeId: this.pickerValue.staff.value,
        assigneeName: this.pickerValue.staff.text,
        nextStepId: this.pickerValue.next.value,
        nextStepName: this.pickerValue.next.text
        // deadlineTime: this.pickerValue.date
      };
      console.log(data);
      let isValid = !_.values(data).some(v => {
        return v === "" || v == undefined;
      });
      callback(isValid, data);
    },
    // 还原组件默认值
    reset() {
      this.listItems = deepCopy(this.defaultListItems);
      this.pickerValue = {
        department: {},
        staff: {},
        next: {}
        // date: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm") || ""
      };
    }
  },
  watch: {
    currentPickedDepartmentId(newDepartmentId) {
      let department = _.find(this.departmentList, dept => {
        return dept.IDeptID == newDepartmentId;
      });
      let staffList = department ? department.ListUser : [];

      // 将员工列表转换为mui-picker接受的参数形式
      this.staffPickerItems = staffList.map(staff => {
        return { text: staff.CAdminName, value: staff.IAdminID };
      });
    }
  },
  components: {
    MuiList
  }
};
</script>