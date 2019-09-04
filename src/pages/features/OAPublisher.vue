<template>
  <div class="OA_manager_container">
    <div class="picker_container" v-if="operType === 1">
      <el-input
        :value="pickedFormName"
        suffix-icon="el-icon-arrow-down"
        readonly
        placeholder="请选择一个流程"
        @tap.native="onFormPickerClick"
      >
        <div slot="prepend" style="color: #001d26">当前流程：</div>
      </el-input>
    </div>
    <div class="form_container" v-if="currentShowForm !== ''">
      <component :is="currentShowForm" ref="form"></component>
      <div class="bottom_action_bar">
        <el-button type="success" :disabled="!buttonEnabled" @click="onSaveClick">保 存</el-button>
        <el-button type="primary" :disabled="!buttonEnabled" @click="onSubmitClick">发 送</el-button>
      </div>
    </div>
    <!-- 人员选择弹出框 -->
    <el-dialog
      title="选择发送人"
      width="90%"
      center
      :modal="false"
      :close-on-click-modal="false"
      :visible.sync="assignDialogVisiable"
      @close="onAssignDialogClose"
    >
      <AssignForm ref="assignForm" :nextStepList="assignFormProps.nextStepList"></AssignForm>
      <div slot="footer">
        <el-button type="primary" @click="onConfirmClick">确 定</el-button>
        <el-button @click="onCancelClick">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import _ from "lodash";
import apiOA, {
  FLOW_ID,
  getTaskJson,
  Default_ID,
  FORM_DATA_MAPPING
} from "@api/OA";
import uuid from "@common/uuid";
import NativePopPicker from "@comp/native/PopPicker";
import dateHelper from "@common/dateHelper";
import {
  getSessionItem,
  getLocalItem,
  parseUnicode,
  reverseObject,
  mapObjectData
} from "@common/util";
import AssignForm from "@comp/OA-publisher/AssignForm.vue";
import { QJForm, JYForm, BYForm } from "@pages/features/OAManager/forms";
export default {
  props: {
    // 1新建2修改
    operType: {
      type: Number,
      default: 1
    }
  },
  data() {
    return {
      // 选择器select的配置对象
      pickerDataOptions: [
        {
          text: "请销假",
          value: "QJ"
        },
        {
          text: "车辆保养申请",
          value: "BY"
        },
        {
          text: "车辆加油申请",
          value: "JY"
        }
      ],
      pickedFormName: "",
      pickedFormId: "",
      buttonEnabled: false,
      flowDefinition: {},
      // 人员选择弹出框是否可见
      assignDialogVisiable: false,
      assignFormProps: {
        nextStepList: []
      }
    };
  },
  mounted() {
    this.formPicker = new NativePopPicker().setData(this.pickerDataOptions);
  },
  computed: {
    // 当前的用户信息
    currentUser() {
      return JSON.parse(getSessionItem("currentUser"));
    },
    // 当前用户id
    currentUserId() {
      return this.currentUser.PersonId;
    },
    // 当前打开的流程缩写，返回QJ/BY/JY
    currentFlowNameAbbr() {
      return this.pickedFormId;
    },
    // 当前显示哪个表单
    currentShowForm() {
      let formComponent = "";
      switch (this.pickedFormId) {
        case "QJ":
          formComponent = QJForm;
          break;
        case "JY":
          formComponent = JYForm;
          break;
        case "BY":
          formComponent = BYForm;
          break;
        default:
          break;
      }
      return formComponent;
    }
  },
  methods: {
    onFormPickerClick() {
      this.formPicker.show().then(pickedItems => {
        // this.$showLoading();
        this.pickedFormName = pickedItems[0].text;
        this.pickedFormId = pickedItems[0].value;
      });
    },
    onSaveClick() {
      console.log(`保存表单 ${this.currentFlowNameAbbr}`);
      let reqSaveData = this.getSaveData();
      // 发送请求
      apiOA.form[`Submit${this.currentFlowNameAbbr}Form`](
        JSON.stringify([reqSaveData.taskJson]),
        JSON.stringify(reqSaveData.dataJson),
        this.operType
      ).then(
        res => {
          console.log(`保存表单 ${this.currentFlowNameAbbr} 成功, res`, res);
          if (res.data.ErrCode == 0) {
            mui.toast("操作成功！");
            this.$router.go(-1);
          } else {
            mui.toast("操作失败，请稍后重试！");
          }
        },
        err => {
          mui.toast("操作失败，请稍后重试！");
        }
      );
    },
    // 在拼装TaskJson时生成默认的Title存放在TaskJson中
    generateDefaultTitle() {
      // [当前用户名]的[当前流程名]申请
      return `${this.currentUser.PersonName}的${this.flowDefinition.Name}申请`;
    },
    getSaveData() {
      let frontendFormData = this.$refs.form.getValue();
      let map = reverseObject(FORM_DATA_MAPPING[this.currentFlowNameAbbr]);
      let formDataForBackend = mapObjectData(frontendFormData, map);
      let dataJson = formDataForBackend;
      console.log("=====dataJson", dataJson);
      let taskJson = Object.assign({}, getTaskJson(), {
        ID: uuid(),
        PrevID: Default_ID,
        PrevStepID: Default_ID,
        FlowID: FLOW_ID[this.currentFlowNameAbbr],
        StepID: this.getFirstStepId(),
        StepName: this.flowDefinition.DesignJSON.steps[0].name,
        GroupID: uuid(),
        Title: frontendFormData.title || this.generateDefaultTitle(),
        SenderID: this.currentUserId,
        SenderName: this.currentUser.PersonName,
        SenderTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        ReceiveID: this.currentUserId,
        ReceiveName: this.currentUser.PersonName,
        ReceiveTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        OpenTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        Status: 1
      });
      console.log("taskJson", taskJson);
      return { taskJson, dataJson };
    },
    onSubmitClick() {
      // 获得第一步之后的所有可选步骤
      let allNextStep = this.getAllNextStep(this.getFirstStepId());
      // if (!_.isEmpty(allNextStep)) {
      this.assignFormProps.nextStepList = allNextStep;
      this.assignDialogVisiable = true;
      // } else {}
    },
    onAssignDialogClose() {
      this.$refs.assignForm.reset();
    },
    // 人员弹出框中点击取消
    onCancelClick() {
      this.assignDialogVisiable = false;
    },
    // 人员弹出框中点击确定
    onConfirmClick() {
      this.$refs.assignForm.getValue((isValid, assignData) => {
        console.log("人员选择点击确定", isValid, assignData);
        if (isValid) {
          // let reqData = {
          //   DepId: Number(formData.departmentId),
          //   PersonId: Number(formData.assigneeId),
          //   PreEndTime: formData.deadlineTime,
          //   OrderTime: dateHelper.format(new Date(), "yyyy-MM-dd"),
          //   EventId: Number(this.orderId),
          //   UserName: this.currentUserName
          // };
          let saveData = this.getSaveData();
          saveData.taskJson.Status = 2;
          saveData.taskJson.CompletedTime1 = dateHelper.format(
            new Date(),
            "yyyy-MM-dd hh:mm:ss"
          );
          let sendData = this.getSaveData();
          // 整合taskJson、dataJson数据和部门人员选择数据
          sendData.taskJson.ReceiveID = assignData.assigneeId;
          sendData.taskJson.ReceiveName = assignData.assigneeName;
          sendData.taskJson.StepID = assignData.nextStepId;
          sendData.taskJson.StepName = assignData.nextStepName;
          sendData.taskJson.ID = uuid();
          sendData.taskJson.PrevID = saveData.taskJson.ID;
          sendData.taskJson.PrevStepID = saveData.taskJson.StepID;
          sendData.taskJson.Sort = saveData.taskJson.Sort + 1;
          sendData.taskJson.Status = 0;
          sendData.taskJson.GroupID = saveData.taskJson.GroupID;
          console.log("人员选择完毕, 最终组合的reqdata", saveData, sendData);
          // 发送请求
          apiOA.form[`Submit${this.currentFlowNameAbbr}Form`](
            JSON.stringify([saveData.taskJson, sendData.taskJson]),
            JSON.stringify(saveData.dataJson),
            this.operType
          ).then(
            res => {
              this.assignDialogVisiable = false;
              console.log("指定人员发送res", res);
              if (res.data.ErrCode == 0) {
                mui.toast("操作成功！");
                this.$router.go(-1);
              } else {
                mui.toast("操作失败，请稍后重试！");
              }
            },
            err => {
              console.log(err, err.response);
              mui.toast("操作失败，请稍后重试！");
            }
          );
        } else {
          // 表单数据有值为空
          mui.toast("请输入完整信息");
        }
      });
    },
    getFirstStepId() {
      let lineList = this.flowDefinition.DesignJSON.lines;
      let stepList = this.flowDefinition.DesignJSON.steps;
      let firstStep = _.find(stepList, step => {
        let lineToList = _.map(lineList, line => {
          return line.to;
        });
        return lineToList.includes(step.id) === false;
      });
      return firstStep.id;
    },
    getAllNextStep(currentStepId) {
      let lineList = this.flowDefinition.DesignJSON.lines;
      let stepList = this.flowDefinition.DesignJSON.steps;
      let currentLineList = _.filter(lineList, line => {
        return line.from == currentStepId;
      });
      console.log("currentStepId", currentStepId, currentLineList);
      return currentLineList.map(line => {
        let nextStepId = line.to;
        let nextStepName = _.find(stepList, step => {
          return nextStepId == step.id;
        }).name;
        console.log("next name", nextStepName);
        return { nextStepId, nextStepName };
      });
    }
    // getNextStepId(currentStepId) {
    //   let lineList = this.flowDefinition.DesignJSON.lines;
    //   let stepList = this.flowDefinition.DesignJSON.steps;
    //   let theLineList = _.find(lineList, line => {
    //     line.from == currentStepId;
    //   });
    // }
  },
  watch: {
    // 当选择的表单变化时，获取该flow的定义数据
    pickedFormId(newFormId) {
      let flowId = FLOW_ID[newFormId];
      this.buttonEnabled = false;
      apiOA.flow
        .GetFlowInfo(flowId)
        .then(res => {
          if (Array.isArray(res.data) && !_.isEmpty(res.data)) {
            let rawData = res.data[0];
            rawData.DesignJSON = JSON.parse(rawData.DesignJSON);
            rawData.RunJSON = JSON.parse(rawData.RunJSON);
            this.flowDefinition = Object.assign({}, rawData);
            this.buttonEnabled = true;
          } else {
            mui.toast("获取表单信息失败，请检查网络");
          }
        })
        .catch(err => {
          console.log(err, err.response);
          mui.toast("获取表单信息失败，请检查网络");
        });
    }
  },
  components: { AssignForm, QJForm, BYForm, JYForm }
};
</script>

<style lang="less">
.OA_manager_container {
  .center_content {
    text-align: center;
  }
  .form_container {
    width: 95%;
    margin: 0 auto;
    background-color: #fff;
    padding: 10px;
    margin-top: 2%;
    margin-bottom: 2%;
    border-radius: 8px;
    .bottom_action_bar {
      display: flex;
      justify-content: space-evenly;
    }
  }
  .el-form-item {
    button {
      margin: 0 8%;
    }
  }
}
</style>


