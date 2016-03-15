namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 构件接口
    /// </summary>
    public interface IDocStructure
    {
        /// <summary>
        /// 文档块
        /// </summary>
        BlockType BlockType
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 生成文档
        /// </summary>
        /// <returns>文档</returns>
        Stream BuildDoc();

        /// <summary>
        /// 获取ID
        /// </summary>
        decimal GetID 
        { 
            get; 
        }

        /// <summary>
        /// 是否新节
        /// </summary>
        bool NewSection 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// 标签列表
        /// </summary>
        List<BaseLabel> LabelList
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化标签列表
        /// </summary>
        void InitLabelList();
    }
}
