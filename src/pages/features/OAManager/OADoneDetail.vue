<template>
  <div class="OA_done_detail_container">
    <div class="dynamic_form_container common_form_container">
      <component
        v-if="currentShowForm !== ''"
        :is="currentShowForm"
        :disabled="formDisabled"
        :initialFormData="initialFormData"
        ref="form"
      ></component>
    </div>
    <div class="common_form_container"></div>
  </div>
</template>

<script>
import _ from "lodash";
import apiOA, {
  STEP_STATUS_MAPPING,
  FLOW_ID,
  FORM_DATA_MAPPING
} from "@api/OA";
import { getSessionItem } from "@common/util";
import dateHelper from "@common/dateHelper";
import QJForm from "./forms/QJ";
import BYForm from "./forms/BY";
import JYForm from "./forms/JY";
export default {
  name: "OADoneDetail",
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
    formDisabled() {
      return true;
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
    JYForm
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