namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 标签抽象类
    /// </summary>
    public abstract class BaseLabel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        private bool isPass = false;

        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName
        {
            get;
            set;
        }

        /// <summary>
        /// 标签类型名
        /// </summary>
        public string LabelTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 关联值
        /// </summary>
        public abstract string RelateValue
        { 
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public BaseLabel()
        {
            // 抽象类方法
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            // 抽象类方法
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public abstract void Execute(IBuildWord buildWord);

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">值</param>
        /// <returns>返回替换结果</returns>
        public abstract bool Replace(string labelName, string value);
    } // end BaseLabel
} // end namespace