<template>
  <div class="OA_todo_detail_container">
    <div class="dynamic_form_container common_form_container">
      <component
        v-if="currentShowForm !== ''"
        :is="currentShowForm"
        :disabled="formDisabled"
        :initialFormData="initialFormData"
        ref="form"
      ></component>
    </div>
    <div class="common_form_container">
      <div class="approval_bar_container" v-if="isCurrentApprovalMode">
        <ApprovalBar ref="approvalBar"></ApprovalBar>
      </div>
      <div class="bottom_action_bar">
        <el-button
          type="success"
          v-if="buttonEnabledList.save"
          :disabled="!buttonEnabled"
          @click="onSaveClick"
        >保 存</el-button>
        <el-button
          type="primary"
          v-if="buttonEnabledList.send"
          :disabled="!buttonEnabled"
          @click="onSubmitClick"
        >发 送</el-button>
        <el-button
          type="primary"
          v-if="buttonEnabledList.complete"
          :disabled="!buttonEnabled"
          @click="onCompleteClick"
        >完 成</el-button>
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
  STEP_STATUS_MAPPING,
  FLOW_ID,
  BUTTON_NAME_MAPPING,
  FORM_DATA_MAPPING
} from "@api/OA";
import { setSessionItem, getSessionItem, reverseObject } from "@common/util";
import dateHelper from "@common/dateHelper";
import uuid from "@common/uuid";
import ApprovalBar from "./ApprovalBar/index";
import QJForm from "./forms/QJ";
import BYForm from "./forms/BY";
import JYForm from "./forms/JY";
import AssignForm from "@comp/OA-publisher/AssignForm.vue";
// 当前操作模式的枚举值
const ACTION_MODE = {
  approval: 1,
  edit: 2
};
export default {
  name: "OATodoDetail",
  props: {
    instanceId: {
      type: [Number, String],
      required: true
    },
    flowId: {
      type: [Number, String],
      required: true
    },
    stepInfo: {
      type: Object,
      default() {
        return {};
      }
    }
  },
  beforeRouteEnter(to, from, next) {
    next(instance => {
      // 如果没有instanceId和flowId传进来， 则视为没有本页面的权限
      if (!instance.instanceId || !instance.flowId) {
        instance.$router.go(-1);
      } else {
        // 如果有，则刷新（获取）表单数据
        instance.refreshRecordFormData();
        // 获取当前流程定义信息
        instance.getFlowDefinition();
        console.log(
          `
        =====
        当前FlowId: ${instance.flowId + " | " + instance.currentFlowNameAbbr}
        当前步骤数据: 
        `,
          instance.stepInfo
        );
      }
    });
  },
  data() {
    return {
      buttonEnabled: false,
      // 后端传来的当前步骤的FormData
      backendFormData: {},
      initialFormData: {},
      flowDefinition: {},
      // 人员选择弹出框是否可见
      assignDialogVisiable: false,
      // 传递给assignForm组件的props
      assignFormProps: {
        nextStepList: []
      },
      operType: 2
    };
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
    // 当前记录的流程缩写，返回QJ/BY/JY
    currentFlowNameAbbr() {
      return _.findKey(FLOW_ID, value => value == this.flowId);
    },
    // 当前显示哪个表单
    currentShowForm() {
      let currentFlowNameAbbr = this.currentFlowNameAbbr;
      let formComponent = "";
      switch (currentFlowNameAbbr) {
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
    },
    currentStepId() {
      return this.stepInfo.StepID;
    },
    // 整型表示的当前步骤的senderId
    currentStepSenderIdForInt() {
      let arr = this.stepInfo.SenderID.split("-");
      return Number(arr[arr.length - 1]);
    },
    // 当前是在编辑之前保存的还是在审批
    currentActionMode() {
      if (
        this.stepInfo.Status === 1 &&
        this.currentStepSenderIdForInt == this.currentUserId
      ) {
        return ACTION_MODE.edit;
      } else {
        return ACTION_MODE.approval;
      }
    },
    formDisabled() {
      return this.currentActionMode === ACTION_MODE.approval;
    },
    isCurrentApprovalMode() {
      return this.currentActionMode === ACTION_MODE.approval;
    },
    isCurrentEditMode() {
      return this.currentActionMode === ACTION_MODE.edit;
    },
    buttonEnabledList() {
      // 默认三个按钮都不显示
      let defaultList = {
        send: false,
        save: false,
        complete: false
      };
      if (_.isEmpty(this.initialFormData)) {
        return defaultList;
      } else if (_.isEmpty(this.flowDefinition)) {
        return defaultList;
      } else {
        // 从流程定义信息中找到当前step的定义信息
        let currentStepDefinition = this.flowDefinition.DesignJSON.steps.find(
          step => step.id === this.currentStepId
        );
        let currentStepButtonsDefinition = currentStepDefinition.buttons;
        if (
          Array.isArray(currentStepButtonsDefinition) &&
          !_.isEmpty(currentStepButtonsDefinition)
        ) {
          console.log(
            "=====当前步骤的button定义",
            currentStepButtonsDefinition
          );
          let sendEnabled = currentStepButtonsDefinition.some(
            button => button.showTitle === BUTTON_NAME_MAPPING.send
          );
          let saveEnabled = currentStepButtonsDefinition.some(
            button => button.showTitle === BUTTON_NAME_MAPPING.save
          );
          let completeEnabled = currentStepButtonsDefinition.some(
            button => button.showTitle === BUTTON_NAME_MAPPING.complete
          );
          return {
            send: sendEnabled,
            save: saveEnabled,
            complete: completeEnabled
          };
        } else {
          return defaultList;
        }
      }
    }
  },
  methods: {
    refreshRecordFormData() {
      apiOA.form[`Get${this.currentFlowNameAbbr}DetailById`](
        this.instanceId
      ).then(res => {
        console.log(`===== ${this.currentFlowNameAbbr}表单数据res`, res);
        let rawFormData = (this.backendFormData = res.data.Data[0]);
        // 根据当前的流程，选择对应的字段map函数，将formData转换成前端接受的格式
        let initialFormData = this.mapCurrentFormData(rawFormData);
        let timeFieldList = [
          "startTime",
          "endTime",
          "maintainDate",
          "createDate",
          "gasDate"
        ];
        _.each(timeFieldList, field => {
          if (field in initialFormData) {
            initialFormData[field] = dateHelper.format(
              new Date(initialFormData[field]),
              "yyyy-MM-dd hh:mm:ss"
            );
          }
        });
        this.initialFormData = initialFormData;
        console.log("=====map后的表单数据", this.initialFormData);
      });
    },
    getFlowDefinition() {
      apiOA.flow
        .GetFlowInfo(this.flowId)
        .then(res => {
          if (Array.isArray(res.data) && !_.isEmpty(res.data)) {
            let rawData = res.data[0];
            rawData.DesignJSON = JSON.parse(rawData.DesignJSON);
            rawData.RunJSON = JSON.parse(rawData.RunJSON);
            this.flowDefinition = Object.assign({}, rawData);
            console.log("=====流程定义res", this.flowDefinition);
            this.buttonEnabled = true;
          } else {
            mui.toast("获取流程数据失败，请检查网络");
          }
        })
        .catch(err => {
          console.log(err, err.response);
          mui.toast("获取流程数据失败，请检查网络");
        });
    },
    onSaveClick() {
      let reqSaveData = this.getSaveData();
      // 发送请求
      apiOA.form[`Submit${this.currentFlowNameAbbr}Form`](
        JSON.stringify([reqSaveData.taskJson]),
        JSON.stringify(reqSaveData.dataJson),
        this.operType
      ).then(res => {
        console.log("提交保存res", res);
        if (res.data.ErrCode == 0) {
          mui.toast("操作成功！");
          this.$router.go(-1);
        } else {
          mui.toast("操作失败，请稍后重试！");
        }
        err => {
          console.log(err, err.response);
          mui.toast("操作失败，请稍后重试！");
        };
      });
    },
    getSaveData() {
      let frontendFormData = this.$refs.form.getValue();
      let map = reverseObject(FORM_DATA_MAPPING[this.currentFlowNameAbbr]);
      let formDataForBackend = this.mapFormData(frontendFormData, map);
      let dataJson = formDataForBackend;
      dataJson.ID = this.instanceId;
      console.log("=====dataJson", dataJson);

      let taskJson = Object.assign({}, this.stepInfo, {
        // ID: uuid(),
        // PrevID: Default_ID,
        // PrevStepID: Default_ID,
        // FlowID: FLOW_ID.QJ,
        // StepID: this.getFirstStepId(),
        // StepName: this.flowDefinition.DesignJSON.steps[0].name,
        // GroupID: uuid(),
        // Title: frontendFormData.title,
        // SenderID: this.currentUserId,
        // SenderName: this.currentUser.PersonName,
        SenderTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        // ReceiveID: this.currentUserId,
        // ReceiveName: this.currentUser.PersonName,
        ReceiveTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        OpenTime: dateHelper.format(new Date(), "yyyy-MM-dd hh:mm:ss"),
        Status: 1
      });
      //   区分当前的操作模式，插入审批结果
      if (this.isCurrentApprovalMode) {
        let refApprovalBar = this.$refs.approvalBar;
        let approvalResult = refApprovalBar.getValue();
        taskJson.Comment = approvalResult;
      }
      console.log("=====taskJson", taskJson);
      return { taskJson, dataJson };
    },
    onSubmitClick() {
      // 获得第一步之后的所有可选步骤
      let allNextStep = this.getAllNextStep(this.currentStepId);
      // if (!_.isEmpty(allNextStep)) {
      this.assignFormProps.nextStepList = allNextStep;
      this.assignDialogVisiable = true;
      // } else {}
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
          let refApprovalBar = this.$refs.approvalBar;
          let approvalResult = refApprovalBar.getValue();
          if (!approvalResult) {
            mui.toast("请选择审批意见！");
            return;
          }
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
          console.log("====================================", [
            saveData.taskJson,
            sendData.taskJson
          ]);
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
    onAssignDialogClose() {
      this.$refs.assignForm.reset();
    },
    // 人员弹出框中点击取消
    onCancelClick() {
      this.assignDialogVisiable = false;
    },
    onCompleteClick() {
      let refApprovalBar = this.$refs.approvalBar;
      let approvalResult = refApprovalBar.getValue();
      if (!approvalResult) {
        mui.toast("请选择审批意见！");
        return;
      }
      let completeData = this.getSaveData();
      completeData.taskJson.Status = 2;
      completeData.taskJson.CompletedTime1 = dateHelper.format(
        new Date(),
        "yyyy-MM-dd hh:mm:ss"
      );
      // 发送请求
      apiOA.form[`Submit${this.currentFlowNameAbbr}Form`](
        JSON.stringify([completeData.taskJson]),
        JSON.stringify(completeData.dataJson),
        this.operType
      ).then(
        res => {
          console.log("最后一步完成res", res);
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
    },
    getAllNextStep(currentStepId) {
      let lineList = this.flowDefinition.DesignJSON.lines;
      let stepList = this.flowDefinition.DesignJSON.steps;
      let currentLineList = _.filter(lineList, line => {
        return line.from == currentStepId;
      });
      console.log(
        "getAllNextStep | currentStepId",
        currentStepId,
        currentLineList
      );
      return currentLineList.map(line => {
        let nextStepId = line.to;
        let nextStepName = _.find(stepList, step => {
          return nextStepId == step.id;
        }).name;
        console.log("next name", nextStepName);
        return { nextStepId, nextStepName };
      });
    },
    // 将后端数据转换成前端表单中接受的数据格式
    mapFormData(data, map) {
      let result = {};
      _.each(_.keys(map), originField => {
        result[map[originField]] = data[originField];
      });
      return result;
    },
    mapCurrentFormData(backendFormData) {
      return this.mapFormData(
        backendFormData,
        FORM_DATA_MAPPING[this.currentFlowNameAbbr]
      );
    }
  },
  components: {
    QJForm,
    BYForm,
    JYForm,
    ApprovalBar,
    AssignForm
  }
};
</script>

<style lang="less" scoped>
.OA_todo_detail_container {
  margin-top: calc(~"1vh + 54px");
  .approval_bar_container {
    margin-bottom: 15vh;
  }
  .bottom_action_bar {
    display: flex;
    justify-content: space-evenly;
  }
}
</style>
