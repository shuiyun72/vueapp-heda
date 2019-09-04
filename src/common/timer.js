let Tasks = {}
// 设置自定义键值对，用于在引用该模块的组件间共享
let Props = {

}
export default class Timer {
    static clear(uid) {
        if (uid in Tasks) {
            Tasks[uid].clear()
        }
    }

    static getAll() {
        return Tasks
    }

    static setProp(key, val) {
        if (typeof key === "string") {
            Props[key] = val
        } else {
            console.error('setProp方法的第一个参数 key 必须是String类型')
        }
    }

    static getProp(key) {
        if (typeof key === 'string' && key in Props) {
            return Props[key]
        } else {
            return undefined
        }
    }

    constructor(uid) {
        if (!(typeof uid === 'string')) {
            console.error('uid is required when you use Timer.prototype.set()')
        }
        this.uid = uid
        this.task = undefined
    }

    set(func, interval, context) {
        let id = window.setInterval(context ? func.bind(context) : func, interval)
        this.task = id
        Tasks[this.uid] = this
        return id
    }

    get() {
        return this.task
    }

    unset() {
        this.clear()
    }

    clear() {
        window.clearInterval(this.task)
        this.task = undefined
        if (this.uid in Tasks) {
            delete Tasks[this.uid]
        }
        return true
    }

    resetProp() {
        Props = {}
    }
}
