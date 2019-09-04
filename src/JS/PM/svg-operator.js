import $ from 'jquery'

export function setDataToSVG(columnData) {
  if (columnData) {
    if (document.getElementById('show') == null) {
      return;
    }

    //初始化当前监测点基础数据信息
    let svg = window.document.getElementById('show').getSVGDocument();
    $.each(columnData, function (index, item) {
      /*非正常标签赋值，例如：依靠电流判断开关的操作*/
      if (item.IDKID == 10 || item.IDKID == 11 || item.IDKID == 12) {
        if (svg.getElementById(item.DataField + "_1")) {
          if (item.IValue > 0) {
            svg.getElementById(item.DataField + "_1").setAttribute("xlink:href", "./Images/on.png");
          } else {
            svg.getElementById(item.DataField + "_1").setAttribute("xlink:href", "./Images/off.png");
          }
        }
        if (svg.getElementById(item.DataField + "_2")) {
          if (item.IValue > 0) {
            svg.getElementById(item.DataField + "_2").setAttribute("xlink:href", "./Images/fengyezhuandong.gif");
          } else {
            svg.getElementById(item.DataField + "_2").setAttribute("xlink:href", "./Images/fengyejingzhi.gif");
          }
        }
      }

      //不能通用
      if (item.DataField == "GZM_L_1_77_8") {
        if (svg.getElementById(item.DataField + "_1")) {
          if (item.IValue && item.IDataType == "2") {
            svg.getElementById(item.DataField + "_1").textContent = parseFloat(item.IValue).toFixed(2) + "  " + item.IDKUnit;
          }
        }
      }

      //正常标签赋值
      if (svg.getElementById(item.DataField)) {
        if (item.IDataType == "1") { //开关类型
          if (item.IValue == 1) {
            svg.getElementById(item.DataField).setAttribute("xlink:href", "./Images/on.png");

            svg.getElementById(item.DataField).setAttribute("IValue", item.IValue);
          } else if (item.IValue == 0) {
            svg.getElementById(item.DataField).setAttribute("xlink:href", "./Images/off.png");

            svg.getElementById(item.DataField).setAttribute("IValue", item.IValue);
          }
        } else if (item.IDataType == "2") { //浮点数类型
          var DesValue = "";
          if (item.IValue) {
            DesValue = item.IValue.toString().indexOf(".") == -1 ? item.IValue : Number(item.IValue).toFixed(2);
          } else {
            DesValue = item.IValue;
          }
          svg.getElementById(item.DataField).textContent = DesValue + "  " + item.IDKUnit;
        } else if (item.IDataType == "3") { //整数类型
          var DesValue = "";
          if (item.IValue) {
            DesValue == item.IValue;
          }
          svg.getElementById(item.DataField).textContent = item.IValue + "  " + item.IDKUnit;
        } else if (item.IDataType == "4") { //报警类型
          if (item.IValue == 1) {
            svg.getElementById(item.DataField).textContent = "故障";
          } else if (item.IValue == 0) {
            svg.getElementById(item.DataField).textContent = "正常";
          }
        } else { //整数类型
          var DesValue = "";
          if (item.IValue) {
            DesValue == item.IValue;
          }
          svg.getElementById(item.DataField).textContent = DesValue + "  " + item.IDKUnit;
        }
      }
    });
  };
}
