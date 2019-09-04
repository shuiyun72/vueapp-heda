// 深拷贝一个对象或数组
export function deepCopy(obj) {
    return JSON.parse(JSON.stringify(obj))
}

// 判断当前是否是移动端
export function isMobile() {
    let rect = window.document.body.getBoundingClientRect()
    let isOnMobile = rect.width - 3 < 768
    return isOnMobile
}

//获取屏幕宽高
export function getClientSize() {
    let h = document.documentElement.clientHeight || document.body.clientHeight;
    let w = document.documentElement.clientWidth || document.body.clientWidth;
    return {
        width: w,
        height: h
    }
}

//获取滚动条宽度
export function getScrollWidth() {
    let noScroll, scroll, oDiv = document.createElement("DIV");
    oDiv.style.cssText = "position:absolute; top:-1000px; width:100px; height:100px; overflow:hidden;";
    noScroll = document.body.appendChild(oDiv).clientWidth;
    oDiv.style.overflowY = "scroll";
    scroll = oDiv.clientWidth;
    document.body.removeChild(oDiv);
    return noScroll - scroll;
}

//回到顶部
export function backToTop() {
    let scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
    if (scrollTop > 0) {
        window.requestAnimationFrame(backToTop);
        window.scrollTo(0, scrollTop - (scrollTop / 5));
    }
}

//取本地储存数据
export function getLocalItem(key) {
    let value;
    try {
        value = localStorage.getItem(key);
    } catch (ex) {
        // 开发环境下提示error
        if (__DEV__) {
            console.error('localStorage.getItem报错, ', ex.message);
        }
    } finally {
        return value;
    }
}

//设置本地储存数据
export function setLocalItem(key, value) {
    try {
        // ios safari 无痕模式下，直接使用 localStorage.setItem 会报错
        localStorage.setItem(key, value);
    } catch (ex) {
        // 开发环境下提示 error
        if (__DEV__) {
            console.error('localStorage.setItem报错, ', ex.message);
        }
    }
}

//取会话储存数据
export function getSessionItem(key) {
    let value;
    try {
        value = sessionStorage.getItem(key);
    } catch (ex) {
        // 开发环境下提示error
        if (__DEV__) {
            console.error('sessionStorage.getItem报错, ', ex.message);
        }
    } finally {
        return value;
    }
}

//设置会话储存数据
export function setSessionItem(key, value) {
    try {
        // ios safari 无痕模式下，直接使用 sessionStorage.setItem 会报错
        sessionStorage.setItem(key, value);
    } catch (ex) {
        // 开发环境下提示 error
        if (__DEV__) {
            console.error('sessionStorage.setItem报错, ', ex.message);
        }
    }
}

//Unicode转中文汉字
export function decode(str) {
    str = str.replace(/(\\u)(\w{1,4})/gi, function ($0) {
        return (String.fromCharCode(parseInt((escape($0).replace(/(%5Cu)(\w{1,4})/g, "$2")), 16)));
    });
    str = str.replace(/(&#x)(\w{1,4});/gi, function ($0) {
        return String.fromCharCode(parseInt(escape($0).replace(/(%26%23x)(\w{1,4})(%3B)/g, "$2"), 16));
    });
    str = str.replace(/(&#)(\d{1,6});/gi, function ($0) {
        return String.fromCharCode(parseInt(escape($0).replace(/(%26%23)(\d{1,6})(%3B)/g, "$2")));
    });
    return str;
}

// 解析unicode编码的string
export function parseUnicode(str) {
    return window.unescape(str.replace(/u/gi, "%u")).replace(/\%/g, '');
}

//转化为00:00时间格式
export function convertTime(seconds) {
    return [
        parseInt(seconds / 60 % 60),
        parseInt(seconds % 60)
    ].join(":").replace(/\b(\d)\b/g, "0$1");
}

export function shuffle(arr) {
    for (let i = arr.length - 1; i >= 0; i--) {
        let randomIndex = Math.floor(Math.random() * (i + 1));
        let itemAtIndex = arr[randomIndex];
        arr[randomIndex] = arr[i];
        arr[i] = itemAtIndex;
    }
    return arr;
}

// 根据经纬度计算两点距离，输出为公里
export function calcDistance(lng1, lat1, lng2, lat2, demical = 2) {
    var radLat1 = lat1 * Math.PI / 180.0;
    var radLat2 = lat2 * Math.PI / 180.0;
    var a = radLat1 - radLat2;
    var b = lng1 * Math.PI / 180.0 - lng2 * Math.PI / 180.0;
    var s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
    s = s * 6378.137;
    s = Math.round(s * Math.pow(10, demical)) / Math.pow(10, demical);
    return s
}

function getBase64FromImgEl(img) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, img.width, img.height);
    var dataURL = canvas.toDataURL("image/png");  // 可选其他值 image/jpeg
    return dataURL;
}

// 将网络中指定url的图片转换成前端base64字符串
export function imgUrlToBase64(src, cb) {
    var image = new Image();
    image.src = src + '?v=' + Math.random(); // 处理缓存
    image.crossOrigin = "*";  // 支持跨域图片
    image.onload = function(){
        var base64 = getBase64FromImgEl(image);
        cb && cb(base64);
    }
}

export function reverseObject(obj) {
    let result = {}
    let keys = Object.keys(obj)
    keys.forEach(key=> {
        result[obj[key]] = key
    })
    return result
}

// 给定对象，指定转换前后的key的对应关系（json表示），实现键的转换，值不变
export function mapObjectData (data, map) {
    let result = {};
    _.each(_.keys(map), originField => {
      result[map[originField]] = data[originField];
    });
    return result;
  }
