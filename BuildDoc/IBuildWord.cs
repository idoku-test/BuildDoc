namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Aspose.Words;

    /// <summary>
    /// 生成文档接口
    /// </summary>
    public interface IBuildWord
    {
        BuildWord Load(Stream stream);

        BuildWord Load(string fullPath);

        /// <summary>
        /// 插入文档
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="docStream">文档流</param>
        /// <param name="newSection">是否使用新节插入</param>
        void InsertDoc(string bookmark, Stream docStream, bool newSection);

        /// <summary>
        /// 插入图像
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="fileIds">文件ID列表</param>
        void InsertImage(string bookmark, string[,] fileIds);

        /// <summary>
        /// 插入表格
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="rows">数据</param>
        /// <param name="tableFillType">填充方式</param>
        void InsertTable(string bookmark, string[,] rows, TableFillType tableFillType);

        /// <summary>
        /// 插入文本
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="text">文本</param>
        void InsertText(string bookmark, string text);

        /// <summary>
        /// 替换文本
        /// </summary>
        /// <param name="mark">书签名</param>
        /// <param name="text">值</param>
        void ReplaceText(string mark, string text);

        /// <summary>
        /// 获取书签列表
        /// </summary>
        /// <returns>书签列表</returns>
        List<string> GetBookmarks();

        /// <summary>
        /// 《[^《]+?_[^》]+?》匹配：《xxx_xxx》，《[\w\W]+?》匹配：《xxx》
        /// </summary>
        /// <param name="regexString">正则表达式</param>
        /// <returns>匹配的项</returns>
        List<string> GetAllMarks(string regexString = null);

        /// <summary>
        /// 删除书签
        /// </summary>
        void DelBookmarks();

        /// <summary>
        /// 更新目录树
        /// </summary>
        void UpdateFields();

        /// <summary>
        /// 创建一个空白文档
        /// </summary>
        void CreateBlankDoc();

        /// <summary>
        /// 获取文档流
        /// </summary>
        /// <returns>文档流</returns>
        Stream GetStream();

        /// <summary>
        /// 设置文档流
        /// </summary>
        /// <param name="docStream">文档流</param>
        void SetStream(Stream docStream);

        /// <summary>
        /// 创建书签
        /// </summary>
        /// <param name="bookmarkName">书签名称</param>
        void CreateBookmark(string bookmarkName);

        /// <summary>
        /// 设置页眉页脚
        /// </summary>
        /// <param name="dict">字典</param>
        void SetPageHeaderFooter(Dictionary<string, string> dict);
    }
}
