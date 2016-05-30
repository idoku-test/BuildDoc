namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 表格填充类型
    /// </summary>
    public enum TableFillType
    {
        /// <summary>
        /// 扩充行
        /// </summary>
        AutoRow,

        /// <summary>
        /// 扩充列
        /// </summary>
        AutoColumn,

        /// <summary>
        /// 仅填充行
        /// </summary>
        OnlyFillByRow,

        /// <summary>
        /// 仅填充列
        /// </summary>
        OnlyFillByColumn
    }

    /// <summary>
    /// 块类型
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// 头
        /// </summary>
        Header,

        /// <summary>
        /// 详细
        /// </summary>
        Detail,

        /// <summary>
        /// 尾
        /// </summary>
        Footer
    }

    /// <summary>
    /// 标签类型
    /// </summary>
    public enum LabelType
    {
        /// <summary>
        /// 条件标签
        /// </summary>
        [Description("条件标签")]
        ConditionLabel,

        /// <summary>
        /// 文档标签
        /// </summary>
        [Description("文档标签")]
        DocLabel,

        /// <summary>
        /// 图像标签
        /// </summary>
        [Description("图像标签")]
        ImageLabel,

        /// <summary>
        /// 表格标签
        /// </summary>
        [Description("表格标签")]
        TableLabel,

        /// <summary>
        /// 文本标签
        /// </summary>
        [Description("文本标签")]
        TextLabel
    }

    /// <summary>
    /// 获取数据方法
    /// </summary>
    public enum GetDataMethod
    {
        /// <summary>
        /// 常量
        /// </summary>
        [Description("常量")]
        Const,

        /// <summary>
        /// 公式
        /// </summary>
        [Description("公式")]
        Formula,

        /// <summary>
        /// 数据源
        /// </summary>
        [Description("数据源")]
        Source,

        /// <summary>
        /// 动态表单
        /// </summary>
        [Description("动态表单")]
        Dynamic,

        /// <summary>
        /// 多数据源
        /// </summary>
        [Description("多数据源")]
        MultiSource
    }

    /// <summary>
    /// 格式化类型
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        Number,

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        Date,

        /// <summary>
        /// 值转文本
        /// </summary>
        [Description("数据库值转换")]
        ValueToText,

        /// <summary>
        /// 转大写
        /// </summary>
        [Description("大写转换")]
        Upper,

        /// <summary>
        /// 大写金额
        /// </summary>
        [Description("金额转换")]
        Ammount
    }

    /// <summary>
    /// 控件类型
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// 文本
        /// </summary>
        [Description("文本")]
        Text,

        /// <summary>
        /// 日期
        /// </summary>
        [Description("日期")]
        Date,

        /// <summary>
        /// 下拉框
        /// </summary>
        [Description("下拉选择")]
        Dropdown
    }

    public enum ControlFormatType
    {
        /// <summary>
        /// 整数
        /// </summary>
        [Description("整数")]
        Integer,

        /// <summary>
        /// 小数及整数
        /// </summary>
        [Description("小数及整数")]
        Number,
    }

    /// <summary>
    /// 控件填充类型
    /// </summary>
    public enum FillType
    {
        /// <summary>
        /// 数据源获取
        /// </summary>
          [Description("数据源")]
        DataSource,

        /// <summary>
        /// 自定义输入
        /// </summary>
          [Description("自定义")]
          Custom
    }

    /// <summary>
    /// 构件类型
    /// </summary>
    public enum StructureType
    {
        /// <summary>
        /// 后台预先配置构件
        /// </summary>
        Config = 1,

        /// <summary>
        /// 自定义构件
        /// </summary>
        Custom = 2
    }
}
