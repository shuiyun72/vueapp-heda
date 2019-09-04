using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
public class WebEnum
{
    /// <summary>
    /// 统一管理操作枚举
    /// </summary>
    public enum ActionEnum
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 显示
        /// </summary>
        Show,
        /// <summary>
        /// 查看
        /// </summary>
        View,
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 审核
        /// </summary>
        Audit,
        /// <summary>
        /// 回复
        /// </summary>
        Reply,
        /// <summary>
        /// 确认
        /// </summary>
        Confirm,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 作废
        /// </summary>
        Invalid,
        /// <summary>
        /// 生成
        /// </summary>
        Build,
        /// <summary>
        /// 安装
        /// </summary>
        Instal,
        /// <summary>
        /// 卸载
        /// </summary>
        UnLoad,
        /// <summary>
        /// 登录
        /// </summary>
        Login,
        /// <summary>
        /// 备份
        /// </summary>
        Back,
        /// <summary>
        /// 还原
        /// </summary>
        Restore,
        /// <summary>
        /// 替换
        /// </summary>
        Replace,
        /// <summary>
        /// 添加文件夹
        /// </summary>
        AddFolder,
        /// <summary>
        /// 修改文件夹
        /// </summary>
        EditFolder,
        /// <summary>
        /// 删除文件夹
        /// </summary>
        DeleteFolder,
        /// <summary>
        /// 添加文件
        /// </summary>
        AddDoc,
        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteDoc,
        /// <summary>
        /// 保存
        /// </summary>
        Save
    }
}
