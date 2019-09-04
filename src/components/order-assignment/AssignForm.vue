<template>
  <div class="assign_form_container">
    <MuiList :items="listItems" @row-click="onListRowClick"></MuiList>
  </div>
</template>

<script>
import _ from "lodash";
import { deepCopy } from "@common/util";
import MuiList from "@comp/common/MuiList.vue";
import dateHelper from "@common/dateHelper";
import apiMaintain from "@api/maintain";
export default {
  props: {
    departmentList: {
      type: Array,
      default() {
        return [
          {
            id: "d1",
            departmentName: "管网部",
            staff: [{ id: "s1", name: "张三" }, { id: "s2", name: "李四" }]
          },
          {
            id: "d2",
            departmentName: "行政部",
            staff: [{ id: "s3", name: "哈哈酱" }, { id: "s4", name: "王五" }]
          }
        ];
      }
    },
    defaultTime: {
      type: Date,
      default() {
        return new Date();
      }
    }
  },
  mounted() {
    // 实例化picker组件
    this._popPicker = new window.mui.PopPicker({ layer: 1 });
    // 实例化date picker组件
    this._datePicker = new window.mui.DtPicker({
      type: "datetime",
      // beginDate: new Date(2015, 4, 25),
      // endDate: new Date(2018, 11, 25),
      labels: ["年", "月", "日", "时", "分"]
    });
  },
  data() {
    return {
      // 值与listItems一样，但用于该组件还原默认值操作
      defaultListItems: [
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
        },
        {
          id: "date",
          label: "时间",
          labelClass: "gray",
          placeholder: "请选择时间",
          content: dateHelper.format(this.defaultTime, "yy-MM-dd hh:mm:ss"),
          saperate: true,
          withDefaultIcon: true
        }
      ],
      listItems: [
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
        },
        {
          id: "date",
          label: "时间",
          labelClass: "gray",
          placeholder: "请选择时间",
          content: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm"),
          saperate: true,
          withDefaultIcon: true
        }
      ],
      pickerValue: {
        department: "",
        staff: "",
        date: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm") || ""
      },
      staffPickerItems: []
    };
  },
  computed: {
    departmentPickerItems() {
      return this.departmentList.map(department => {
        return {
          value: department.id,
          text: department.departmentName
        };
      });
    },
    currentPickedDepartmentId() {
      return this.pickerValue.department.value;
    }
  },
  methods: {
    onListRowClick(row, rowIndex) {
      console.log("rowClick!", row, rowIndex);
      if (rowIndex === 0) {
        // 部门选择
        this.openPicker(this.departmentPickerItems, row, rowIndex);
      } else if (rowIndex === 1) {
        // 人员选择
        if (this.pickerValue["department"] === "") {
          window.mui.toast("请先选择部门");
        } else {
          this.openPicker(this.staffPickerItems, row, rowIndex);
        }
      } else if (rowIndex === 2) {
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
        departmentId: Number(this.pickerValue.department.value),
        assigneeId: this.pickerValue.staff.value,
        deadlineTime: this.pickerValue.date
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
        department: "",
        staff: "",
        date: dateHelper.format(this.defaultTime, "yyyy-MM-dd hh:mm") || ""
      };
    }
  },
  watch: {
    currentPickedDepartmentId(newDepartmentId) {
      let staffList = [];
      apiMaintain.GetStaffList(newDepartmentId).then(res => {
        if (res.data.result === true) {
          staffList = res.data.data.map(person => {
            return { id: person.personId, name: person.personName };
          });
        } else {
          mui.toast("该部门下暂无人员");
        }
        // 将员工列表转换为mui-picker接受的参数形式
        this.staffPickerItems = staffList.map(staff => {
          return { text: staff.name, value: staff.id };
        });
      });
    }
  },
  components: {
    MuiList
  }
};
</script>
