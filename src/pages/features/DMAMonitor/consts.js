const OUTER_ICON_STYLE = {
    fontSize: '30px',
    color: '#11A4DB'
}

const INNER_ICON_STYLE = {
    fontSize: '30px',
    color: 'teal',
    // color: 'sienna'
}


const FA_ICON_PATCH = {
    lineHeight: '30px'
}

function combine(...objList) {
    return Object.assign({}, ...objList)
}


export const ActionbarConfig = [{
        id: "select",
        text: "区域点选",
        iconClass: 'fas fa-map',
        iconStyle: combine(OUTER_ICON_STYLE, FA_ICON_PATCH)
    },
    {
        id: "rank",
        text: "分区统计",
        iconClass: "fas fa-table",
        iconStyle: OUTER_ICON_STYLE
    }
]
