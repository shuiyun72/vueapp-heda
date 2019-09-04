import _ from 'lodash'
export default class PopPicker {
    constructor(pickerOptions) {
        if (window.mui && window.mui.PopPicker) {
            this.pickerOptions = {}
            this.dataOptions = []
            if (pickerOptions instanceof Object && !_.isEmpty(pickerOptions)) {

                this.pickerInstance = new window.mui.PopPicker(pickerOptions)
            } else {
                this.pickerInstance = new window.mui.PopPicker()
            }
        } else {
            console.error('没有引入mui或缺少mui PopPicker插件！')
        }
    }
    // picker.setData([{value:'程序中的值',text:'UI中显示的值'}])
    setData(dataArray) {
        if (Array.isArray(dataArray) && !_.isEmpty(dataArray)) {
            this.dataOptions = dataArray
            this.pickerInstance.setData(dataArray)
        } else {
            console.error(`使用setData时，参数是一个非空对象数组，且数组中每个对象必需text与value字段`)
        }
        return this
    }
    getSelectedItem() {
        return this.pickerInstance.getSelectedItem()
    }
    setSelectedIndex(index = 0, layer = 0, duration = 0) {
        if (typeof index === 'number' && typeof layer === 'number') {
            return new Promise(resolve => {
                this.pickerInstance.pickers[layer].setSelectedIndex(index, duration, () => {
                    resolve()
                })
            })
        } else {
            console.err(`使用Picker.setSelectedIndex方法时，参数为Number类型，指定具体项的索引；第二个参数为Number类型，指定具体层级`)
        }
    }
    setSelectedValue(value, layer = 0, duration = 0) {
        if (typeof value === 'string' && typeof layer === 'number') {
            return new Promise(resolve => {
                this.pickerInstance.pickers[layer].setSelectedValue(value, duration, () => {
                    resolve()
                })
            })
        } else {
            console.err(`使用Picker.setSelectedValue方法时，第一个参数为String类型，指定具体项的值；第二个参数为Number类型，指定具体层级`)
        }
    }
    getLayer(layer = 0) {
        return this.pickerInstance.pickers[layer]
    }
    show() {
        return new Promise((resolve) => {
            this.pickerInstance.show((pickedItems) => {
                resolve(pickedItems)
            })
        })
    }
    hide() {
        this.pickerInstance.hide()
        return this
    }
    destroy() {
        this.pickerInstance.dispose()
        this.pickerInstance = null
    }
}
