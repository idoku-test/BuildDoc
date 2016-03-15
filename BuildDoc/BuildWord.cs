namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Aspose.Words;
    using Aspose.Words.Saving;
    using Aspose.Words.Tables;
    using Common;

    /// <summary>
    /// Word操作类
    /// </summary>
    public class BuildWord : IBuildWord
    {
        /// <summary>
        /// 内部文档地址
        /// </summary>
        private string docPath;

        /// <summary>
        /// 内部文档对象
        /// </summary>
        private Document doc = null;

        /// <summary>
        /// 内部文档对象流
        /// </summary>
        private Stream docStream = null;

        /// <summary>
        /// 是否清除文档最后面的分节线
        /// </summary>
        private bool clearBlankLineWithDocumentEnd = false;

        /// <summary>
        /// 空
        /// </summary>
        public BuildWord() 
        { 
        }

        /// <summary>
        /// 带word文档流参数
        /// </summary>
        /// <param name="docStream">Word文档流</param>
        public BuildWord(Stream docStream)
        {
            this.doc = new Document(docStream);
            this.docStream = new MemoryTributary();
            // 清除下划线开头的书签
            this.DelBookmarks(this.doc, "_");
        }

        /// <summary>
        /// 读取文档
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public BuildWord Load(string fullPath)
        {
           
            string docName = Path.GetFileName(fullPath);
            
            FileInfo fi = new FileInfo(fullPath);
            
            if (fi.Exists)
            {
                this.docPath = fullPath;
                this.doc = new Document(fullPath);    
              
            }

            return this;
        }

        /// <summary>
        /// 读取文档
        /// </summary>
        /// <param name="docStream"></param>
        /// <returns></returns>
        public BuildWord Load(Stream docStream)
        {
            this.doc = new Document(docStream);
            this.docStream = new MemoryTributary();
            // 清除下划线开头的书签
            this.DelBookmarks(this.doc, "_");
            return this;
        }


        #region--InsertDoc
        /// <summary>
        /// 插入word文档
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="docStream">Word文档流</param>
        /// <param name="newSection">是否以新节插入</param>
        public void InsertDoc(string bookmark, Stream docStream, bool newSection)
        {
            if (this.doc == null || docStream == null)
            {
                return;
            }

            Document subDoc = new Document(docStream);

            // 清除下划线开头的书签
            this.DelBookmarks(subDoc, "_");
            var lastParagraph = subDoc.LastSection.Body.LastParagraph;
            if (lastParagraph.PreviousSibling != null && lastParagraph.PreviousSibling.NodeType == NodeType.Table && lastParagraph.Runs.Count == 0)
            {
                lastParagraph.Remove();
            }

            Bookmark mark = this.doc.Range.Bookmarks[bookmark];
            if (mark != null)
            {
                mark.Text = string.Empty;

                // 插入到另一文档
                this.InsertDocument(mark.BookmarkStart.ParentNode.PreviousSibling, subDoc, newSection);
                docStream.Close();
                docStream.Dispose();

                // 文件另存到桌面
                // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\InsertDoc.doc";
                // _doc.Save(path);
            }
        }
        #endregion

        #region--InsertImage
        /// <summary>
        /// 插入图像
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="imageStream">图像流名称</param>
        public void InsertImage(string bookmark, Stream imageStream)
        {
            if (this.doc == null || imageStream == null)
            {
                return;
            }

            Bookmark mark = this.doc.Range.Bookmarks[bookmark];
            if (mark != null)
            {
                try
                {
                    mark.Text = string.Empty;
                    var builder = new DocumentBuilder(this.doc);
                    builder.MoveTo(mark.BookmarkStart);
                    Image image = Image.FromStream(imageStream);
                    double width = image.Width;
                    double height = image.Height;
                    if (width > 400)
                    {
                        height = 400 / width * height;
                        width = 400;
                    }

                    builder.InsertImage(imageStream, width, height);
                }
                catch 
                { 
                }

                // 文件另存到桌面
                // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\InsertImage.doc";
                // _doc.Save(path);
            }
        }

        /// <summary>
        /// 插入图像
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="fileName">服务器文件路径</param>
        public void InsertImage(string bookmark, string fileName)
        {
            Bookmark mark = this.doc.Range.Bookmarks[bookmark];
            if (mark != null)
            {
                mark.Text = string.Empty;
                var builder = new DocumentBuilder(this.doc);
                builder.MoveTo(mark.BookmarkStart);
                builder.InsertImage(fileName);

                // 文件另存到桌面
                // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\InsertImage.doc";
                // _doc.Save(path);
            }
        }

        /// <summary>
        /// 插入图像
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="fileIds">文件服务器文件ID数组</param>
        public void InsertImage(string bookmark, string[,] fileIds)
        {
            if (this.doc == null || fileIds.GetLength(0) == 0)
            {
                return;
            }

            Bookmark mark = this.doc.Range.Bookmarks[bookmark];
            decimal fileId = 0;
            if (mark != null)
            {
                mark.Text = string.Empty;
                var builder = new DocumentBuilder(this.doc);
                builder.MoveTo(mark.BookmarkStart);
                var tableNode = this.FindParentNode(builder.CurrentNode, NodeType.Table) as Table;
                var cellNode = this.FindParentNode(builder.CurrentNode, NodeType.Cell) as Cell;
                double fixedWidth = 420, rowHeight = 0;
                int startCellIndex = 0;
                int tableIndex = -1;
                int rowCount = 0, colCount = 0;
                if (tableNode != null && cellNode != null)
                {
                    var table = tableNode as Table;
                    tableIndex = this.GetTableIndex(table);
                    rowCount = table.Rows.Count;
                    colCount = table.FirstRow.Cells.Count;
                    bool isBreak = false;

                    // 查找书签所在单元格的序号
                    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < colCount; colIndex++)
                        {
                            var cell = table.Rows[rowIndex].Cells[colIndex];
                            if (cellNode.Equals(cell))
                            {
                                isBreak = true;
                                break;
                            }

                            startCellIndex++;
                        }

                        if (isBreak)
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < fileIds.GetLength(0); i++)
                {
                    if (decimal.TryParse(fileIds[i, 0], out fileId))
                    {
                        if (tableIndex != -1 && rowCount > 0 && colCount > 0)
                        {
                            if (i + 1 + startCellIndex > rowCount * colCount)
                            {
                                break;
                            }

                            int rowIndex = (i + startCellIndex) / colCount;
                            int colIndex = (i + startCellIndex) % colCount;
                            builder.MoveToCell(tableIndex, rowIndex, colIndex, 0);
                            var cell = this.FindParentNode(builder.CurrentParagraph, NodeType.Cell) as Cell;
                            fixedWidth = cell.CellFormat.Width;
                            rowHeight = cell.ParentRow.RowFormat.Height;
                        }

                        //var imageStream = FileHelper.GetFileStream(fileId);
                        //if (imageStream.Length > 0)
                        //    this.InsertImage(builder, imageStream, fixedWidth, rowHeight);
                    }
                }
            }
        }

        /// <summary>
        /// 插入图像
        /// </summary>
        /// <param name="builder">文档构造类</param>
        /// <param name="imageStream">图像流</param>
        /// <param name="fixedWidth">宽度</param>
        /// <param name="rowHeight">高度</param>
        private void InsertImage(DocumentBuilder builder, Stream imageStream, double fixedWidth, double rowHeight)
        {
            Image image = Image.FromStream(imageStream);
            double width = image.Width;
            double height = image.Height;
            fixedWidth = fixedWidth - 10;
            if (rowHeight > 0)
            {
                double scaleWidth = rowHeight / height * width;
                fixedWidth = scaleWidth < fixedWidth ? scaleWidth : fixedWidth;
            }

            if (width > fixedWidth)
            {
                height = fixedWidth / width * height;
                width = fixedWidth;
            }

            builder.InsertImage(imageStream, width, height);
        }
        #endregion

        #region--InsertTable
        /// <summary>
        /// 插入表格
        /// </summary>
        /// <param name="bookmark">标签名称</param>
        /// <param name="tableData">表格数据</param>
        /// <param name="tableFillType">填充方式</param>
        public void InsertTable(string bookmark, string[,] tableData, TableFillType tableFillType)
        {
            if (this.doc == null || tableData == null)
            {
                if (this.doc != null)
                {
                    this.ReplaceText(bookmark, string.Empty);
                }

                return;
            }

            Bookmark mark = this.doc.Range.Bookmarks[bookmark];
            if (mark != null && tableData != null && tableData.Length > 0)
            {
                mark.Text = string.Empty;
                var builder = new DocumentBuilder(this.doc);

                switch (tableFillType)
                {
                    case TableFillType.AutoColumn:
                        this.AutoColumn(mark, tableData, builder);
                        break;
                    case TableFillType.AutoRow:
                        this.AutoRow(mark, tableData, builder);
                        break;
                    case TableFillType.OnlyFillByColumn:
                        this.OnlyFillByColumn(mark, tableData, builder);
                        break;
                    case TableFillType.OnlyFillByRow:
                        this.OnlyFillByRow(mark, tableData, builder);
                        break;
                }

                // 文件另存到桌面
                // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\InsertTable.doc";
                // _doc.Save(path);
            }
        }

        /// <summary>
        /// 自动填充行
        /// </summary>
        /// <param name="mark">书签名称</param>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void AutoRow(Bookmark mark, string[,] tableData, DocumentBuilder builder)
        {
            try
            {
                Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)mark.BookmarkStart.ParentNode.PreviousSibling;
                Aspose.Words.Tables.Cell cellStyle;
                NodeCollection allTables = this.doc.GetChildNodes(NodeType.Table, true);
                int tableIndex = allTables.IndexOf(table);

                List<CellFormat> widthList = new List<CellFormat>();
                for (int i = 0; i < tableData.GetLength(1); i++)
                {
                    // 移动单元格
                    builder.MoveToCell(tableIndex, 0, i, 0);

                    // 获取单元格样式
                    cellStyle = (Aspose.Words.Tables.Cell)builder.CurrentNode.GetAncestor(NodeType.Cell);
                    widthList.Add(cellStyle.CellFormat);
                }

                // 定位到书签
                builder.MoveTo(mark.BookmarkStart);

                for (var i = 0; i < tableData.GetLength(0); i++)
                {
                    for (var j = 0; j < tableData.GetLength(1); j++)
                    {
                        builder.InsertCell();

                        // 设置样式
                        builder.CellFormat.Borders.LineWidth = widthList[j].Borders.LineWidth;
                        builder.CellFormat.Borders.LineStyle = widthList[j].Borders.LineStyle;
                        builder.CellFormat.Borders.Color = widthList[j].Borders.Color;
                        builder.CellFormat.Width = widthList[j].Width;
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;

                        // 垂直居中对齐
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                        // 水平居中对齐
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.Write(tableData[i, j]);
                    }

                    builder.EndRow();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 自动扩充列
        /// </summary>
        /// <param name="mark">书签名称</param>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void AutoColumn(Bookmark mark, string[,] tableData, DocumentBuilder builder)
        {
            try
            {
                Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)mark.BookmarkStart.ParentNode.PreviousSibling;
                Aspose.Words.Tables.Cell cellStyle;
                NodeCollection allTables = this.doc.GetChildNodes(NodeType.Table, true);
                int tableIndex = allTables.IndexOf(table);

                // 记录标题列的样式
                List<CellFormat> widthList = new List<CellFormat>();
                for (int i = 0; i < tableData.GetLength(1); i++)
                {
                    // 移动单元格
                    builder.MoveToCell(tableIndex, 0, i, 0);

                    // 获取单元格样式
                    cellStyle = (Aspose.Words.Tables.Cell)builder.CurrentNode.GetAncestor(NodeType.Cell);
                    widthList.Add(cellStyle.CellFormat);
                }

                // 定位到书签
                builder.MoveTo(mark.BookmarkStart);

                string[,] ary = new string[tableData.GetLength(1), tableData.GetLength(0)];
                for (int i = 0; i < tableData.GetLength(0); i++)
                {
                    for (int j = 0; j < tableData.GetLength(1); j++)
                    {
                        ary[j, i] = tableData[i, j];
                    }
                }

                for (var i = 0; i < ary.GetLength(0); i++)
                {
                    for (var j = 0; j < ary.GetLength(1); j++)
                    {
                        // 添加一个单元格
                        builder.InsertCell();

                        // 设置样式
                        builder.CellFormat.Borders.LineWidth = widthList[j].Borders.LineWidth;
                        builder.CellFormat.Borders.LineStyle = widthList[j].Borders.LineStyle;
                        builder.CellFormat.Borders.Color = widthList[j].Borders.Color;
                        builder.CellFormat.Width = widthList[j].Width;
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;

                        // 垂直居中对齐
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                        // 水平居中对齐
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.Write(ary[i, j]);
                    }

                    builder.EndRow();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 只填充列
        /// </summary>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void OnlyFillByColumn(string[,] tableData, DocumentBuilder builder)
        {
            string[,] ary = new string[tableData.GetLength(1), tableData.GetLength(0)];
            for (int row = 0; row < tableData.GetLength(0); row++)
            {
                for (int col = 0; col < tableData.GetLength(1); col++)
                {
                    ary[col, row] = tableData[row, col];
                }
            }

            for (int row = 0; row < ary.GetLength(0); row++)
            {
                for (int col = 0; col < ary.GetLength(1); col++)
                {
                    builder.InsertCell();

                    // 垂直居中对齐
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                    // 水平居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.Write(ary[row, col]);
                }

                builder.EndRow();
            }
        }

        /// <summary>
        /// 只填充行
        /// </summary>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void OnlyFillByRow(string[,] tableData, DocumentBuilder builder)
        {
            for (int row = 0; row < tableData.GetLength(0); row++)
            {
                for (int col = 0; col < tableData.GetLength(1); col++)
                {
                    builder.InsertCell();

                    // 垂直居中对齐
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                    // 水平居中对齐
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.Write(tableData[row, col]);
                }

                builder.EndRow();
            }
        }

        #region--OnlyFillByColumn
        /// <summary>
        /// 在固定表格里，按列填充数据
        /// </summary>
        /// <param name="mark">书签名称</param>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void OnlyFillByColumn(Bookmark mark, string[,] tableData, DocumentBuilder builder)
        {
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)mark.BookmarkStart.GetAncestor(NodeType.Table);
            Aspose.Words.Tables.Row row = (Aspose.Words.Tables.Row)mark.BookmarkStart.GetAncestor(NodeType.Row);
            Cell cell = (Cell)mark.BookmarkStart.GetAncestor(NodeType.Cell);
            NodeCollection allTables = this.doc.GetChildNodes(NodeType.Table, true);
            int tableIndex = allTables.IndexOf(table);
            int rowIndex = table.IndexOf(row);
            int cellIndex = row.IndexOf(cell);
            int columnIndex = cellIndex;

            for (int i = 0; i < tableData.GetLength(0) && rowIndex < table.Rows.Count; i++)
            {
                columnIndex = cellIndex;
                for (int j = 0; j < tableData.GetLength(1); j++)
                {
                    if (columnIndex < row.Cells.Count)
                    {
                        builder.MoveToCell(tableIndex, rowIndex, columnIndex++, 0);

                        // 垂直居中对齐
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                        // 水平居中对齐
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.Write(tableData[i, j]);
                    }
                }

                rowIndex++;
            }
        }
        #endregion

        #region--OnlyFillByRow
        /// <summary>
        /// 在固定表格里，按行填充数据
        /// </summary>
        /// <param name="mark">书签名称</param>
        /// <param name="tableData">表格数据</param>
        /// <param name="builder">文档构造类</param>
        private void OnlyFillByRow(Bookmark mark, string[,] tableData, DocumentBuilder builder)
        {
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)mark.BookmarkStart.GetAncestor(NodeType.Table);
            Aspose.Words.Tables.Row row = (Aspose.Words.Tables.Row)mark.BookmarkStart.GetAncestor(NodeType.Row);
            Cell cell = (Cell)mark.BookmarkStart.GetAncestor(NodeType.Cell);
            NodeCollection allTables = this.doc.GetChildNodes(NodeType.Table, true);
            int tableIndex = allTables.IndexOf(table);
            int rowIndex = table.IndexOf(row);
            int cellIndex = row.IndexOf(cell);
            int columnIndex = cellIndex;

            string[,] ary = new string[tableData.GetLength(1), tableData.GetLength(0)];
            for (int i = 0; i < tableData.GetLength(0); i++)
            {
                for (int j = 0; j < tableData.GetLength(1); j++)
                {
                    ary[j, i] = tableData[i, j];
                }
            }

            for (int i = 0; i < ary.GetLength(0) && rowIndex < table.Rows.Count; i++)
            {
                columnIndex = cellIndex;
                for (int j = 0; j < ary.GetLength(1); j++)
                {
                    if (columnIndex < row.Cells.Count)
                    {
                        builder.MoveToCell(tableIndex, rowIndex, columnIndex++, 0);

                        // 垂直居中对齐
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;

                        // 水平居中对齐
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.Write(ary[i, j]);
                    }
                }

                rowIndex++;
            }
        }
        #endregion
        #endregion

        #region--InsertText
        /// <summary>
        /// 插入文本
        /// </summary>
        /// <param name="bookmark">书签名称</param>
        /// <param name="text">替换内容</param>
        public void InsertText(string bookmark, string text)
        {
            this.ReplaceText(bookmark, text);
        }
        #endregion

        #region--ReplaceText
        /// <summary>
        /// 指定内容替换书签
        /// </summary>
        /// <param name="mark">书签名称</param>
        /// <param name="text">替换值</param>
        public void ReplaceText(string mark, string text)
        {
            if (this.doc == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                // 没有值就删除书签
                this.doc.Range.Replace(new System.Text.RegularExpressions.Regex("《\\s*" + mark + "\\s*》"), new ReplaceBookmark(), false);
            }
            else
            {
                this.doc.Range.Replace(new System.Text.RegularExpressions.Regex("《\\s*" + mark + "\\s*》"), text);
            }
        }

        /// <summary>
        /// 书签替换实现类
        /// </summary>
        private class ReplaceBookmark : IReplacingCallback
        {
            /// <summary>
            /// 替换方法
            /// </summary>
            /// <param name="e">替换位置</param>
            /// <returns>替换结果</returns>
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                Node currentNode = e.MatchNode;
                Node currentStartNode = e.MatchNode;
                Node currentEndNode = e.MatchNode;
                Node curParentNode = e.MatchNode.ParentNode;

                try
                {
                    List<string> bookmarkStartNameList = new List<string>();
                    List<string> bookmarkEndNameList = new List<string>();
                    while (true)
                    {
                        if (currentStartNode != null)
                        {
                            currentStartNode = currentStartNode.PreviousPreOrder(currentNode.Document);
                        }

                        if (currentStartNode != null)
                        {
                            if (currentStartNode.NodeType == NodeType.BookmarkStart)
                            {
                                Aspose.Words.BookmarkStart start = currentStartNode as Aspose.Words.BookmarkStart;
                                if (!start.Name.Contains("_Toc"))
                                {
                                    bookmarkStartNameList.Add(start.Name);
                                }
                            }
                        }

                        if (currentEndNode != null)
                        {
                            currentEndNode = currentEndNode.NextPreOrder(currentNode.Document);
                        }

                        if (currentEndNode != null)
                        {
                            if (currentEndNode.NodeType == NodeType.BookmarkEnd)
                            {
                                Aspose.Words.BookmarkEnd end = currentEndNode as Aspose.Words.BookmarkEnd;
                                if (!end.Name.Contains("_Toc"))
                                {
                                    bookmarkEndNameList.Add(end.Name);
                                }
                            }
                        }

                        var intersectList = bookmarkStartNameList.Intersect(bookmarkEndNameList);
                        if (intersectList != null && intersectList.Count() > 0)
                        {
                            Bookmark bookmark = e.MatchNode.Document.Range.Bookmarks[intersectList.First()];
                            bookmark.Text = string.Empty;
                            bookmark.Remove();
                            break;
                        }

                        if (currentStartNode == null && currentEndNode == null)
                        {
                            break;
                        }
                    }

                    return ReplaceAction.Skip;
                }
                catch
                {
                    e.Replacement = string.Empty;
                    return ReplaceAction.Replace;
                }
            }
        }
        #endregion

        #region--GetBookmarks，GetAllMarks，DelBookmarks
        /// <summary>
        /// 获取所有书签
        /// </summary>
        /// <returns>书签列表</returns>
        public List<string> GetBookmarks()
        {
            List<string> list = new List<string>();
            BookmarkCollection bc = this.doc.Range.Bookmarks;

            foreach (Bookmark mark in bc)
            {
                if (mark.Text.Contains("《") || mark.Text.Contains("》"))
                {
                    continue;
                }

                list.Add(mark.Name);
            }

            var newlist = this.GetAllMarks();
            if (newlist != null)
            {
                list.AddRange(newlist);
            }

            return list;
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="regexString">标签正则表达式</param>
        /// <returns>标签列表</returns>
        public List<string> GetAllMarks(string regexString = null)
        {
            List<string> list = new List<string>();
            this.doc.Range.Bookmarks.Clear();
            var str = this.doc.GetText();

            if (string.IsNullOrEmpty(regexString))
            {
                regexString = @"(?<=《)[\w\W]+?(?=》)";
            }

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regexString);
            System.Text.RegularExpressions.MatchCollection mc = regex.Matches(str);
            foreach (System.Text.RegularExpressions.Match c in mc)
            {
                list.Add(c.Value.Trim());
            }

            return list;
        }

        /// <summary>
        /// 删除书签
        /// </summary>
        public void DelBookmarks()
        {
            if (this.doc == null)
            {
                return;
            }

            this.doc.Range.Bookmarks.Clear();
        }

        /// <summary>
        /// 删除书签
        /// </summary>
        /// <param name="doc">文档对象</param>
        /// <param name="startChar">前缀</param>
        private void DelBookmarks(Document doc, string startChar)
        {
            var marks = doc.Range.Bookmarks;
            var markList = new List<Bookmark>();
            foreach (Bookmark bm in marks)
            {
                if (bm.Name.StartsWith(startChar))
                {
                    markList.Add(bm);
                }
            }

            markList.ForEach(it => it.Remove());
        }
        #endregion

        #region--InsertDocument
        /// <summary>
        /// 插入文档
        /// </summary>
        /// <param name="insertAfterNode">插入节点</param>
        /// <param name="srcDoc">目标文档</param>
        /// <param name="newSection">是否新节</param>
        private void InsertDocument(Node insertAfterNode, Document srcDoc, bool newSection)
        {
            // Make sure that the node is either a paragraph or table.
            if ((!insertAfterNode.NodeType.Equals(NodeType.Paragraph)) &
              (!insertAfterNode.NodeType.Equals(NodeType.Table)))
            {
                throw new ArgumentException("目标节点应该是一个段落或表"); // The destination node should be either a paragraph or table.
            }

            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;

            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.Document, ImportFormatMode.UseDestinationStyles);

            DocumentBuilder builder = new DocumentBuilder(this.doc);

            List<TopicStartPara> topicStartParas = new List<TopicStartPara>();

            // Loop through all sections in the source document.
            foreach (Section srcSection in srcDoc.Sections)
            {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body)
                {
                    // Let's skip the node if it is a last empty paragraph in a section.
                    if (srcNode.NodeType.Equals(NodeType.Paragraph))
                    {
                        Paragraph para = (Paragraph)srcNode;
                        if (para.IsEndOfSection && !para.HasChildNodes)
                        {
                            continue;
                        }
                    }

                    // This creates a clone of the node, suitable for insertion into the destination document.
                    Node newNode = importer.ImportNode(srcNode, true);

                    // Insert new node after the reference node.
                    dstStory.InsertAfter(newNode, insertAfterNode);
                    insertAfterNode = newNode;

                    if ((newSection || !srcSection.Equals(srcDoc.FirstSection)) && srcNode.Equals(srcSection.Body.FirstChild) && insertAfterNode.PreviousSibling != null)
                    {
                        bool isRemove = false;
                        Node firstChildWithSection = insertAfterNode.PreviousSibling;
                        if (firstChildWithSection.NodeType != NodeType.Paragraph)
                        {
                            firstChildWithSection = new Paragraph(insertAfterNode.Document);
                            dstStory.InsertBefore(firstChildWithSection, insertAfterNode);
                            isRemove = true;
                        }

                        BreakType bt = BreakType.SectionBreakNewPage;
                        if (!srcSection.Equals(srcDoc.FirstSection))
                        {
                            switch (srcSection.PageSetup.SectionStart)
                            {
                                case SectionStart.Continuous:
                                    {
                                        bt = BreakType.SectionBreakContinuous;
                                        break;
                                    }
                            }
                        }

                        topicStartParas.Add(new TopicStartPara()
                        {
                            TopicStartNode = firstChildWithSection,
                            SourceSection = srcSection,
                            TargerBreakType = bt,
                            IsRemove = isRemove
                        });
                    }
                }
            }

            // 插入分节符
            foreach (var item in topicStartParas)
            {
                if (item.TopicStartNode != null)
                {
                    var moveNode = item.TopicStartNode;
                    if (!moveNode.NodeType.Equals(NodeType.Paragraph))
                    {
                        throw new ArgumentException("目标节点应该是一个段落");
                    }

                    var nextNode = moveNode.NextSibling;
                    builder.MoveTo(moveNode);
                    builder.InsertBreak(item.TargerBreakType);
                    builder.CurrentSection.HeadersFooters.LinkToPrevious(false);
                    PageSetup ps = builder.PageSetup;
                    ps.DifferentFirstPageHeaderFooter = false;
                    ps.OddAndEvenPagesHeaderFooter = false;
                    this.CopyHeadersFooters(importer, item.SourceSection, builder.CurrentSection);

                    // 删除添加分隔符后多加的空行
                    if (nextNode != null && nextNode.PreviousSibling != null &&
                        nextNode.PreviousSibling.NodeType == NodeType.Paragraph && ((Paragraph)nextNode.PreviousSibling).Runs.Count == 0)
                    {
                        nextNode.PreviousSibling.Remove();
                    }

                    if (item.IsRemove)
                    {
                        moveNode.Remove();
                    }
                }
            }
        }

        /// <summary>
        /// 复制页眉页脚
        /// </summary>
        /// <param name="importer">导入节点</param>
        /// <param name="srcSection">源分节</param>
        /// <param name="desSection">目标分节</param>
        private void CopyHeadersFooters(NodeImporter importer, Section srcSection, Section desSection)
        {
            desSection.HeadersFooters.Clear();
            foreach (HeaderFooter headerFooter in srcSection.HeadersFooters)
            {
                desSection.HeadersFooters.Add(importer.ImportNode(headerFooter, true));
            }
        }
        #endregion

        #region--UpdateFields
        /// <summary>
        /// 更新目录
        /// </summary>
        public void UpdateFields()
        {
            var nodes = this.doc.GetChildNodes(NodeType.Paragraph, true);
            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Paragraph)
                {
                    var p = node as Paragraph;
                    if (p.IsListItem && p.Runs.Count == 0)
                    {
                        p.ParagraphFormat.ClearFormatting();
                    }
                }
            }

            this.doc.UpdateFields();
        }
        #endregion

        #region--GetStream
        /// <summary>
        /// 获取文档流
        /// </summary>
        /// <returns>文档流</returns>
        public Stream GetStream()
        {
            if (this.clearBlankLineWithDocumentEnd)
            {
                var lastParagraph = this.doc.LastSection.Body.LastParagraph;
                if (lastParagraph.Runs.Count == 0)
                {
                    lastParagraph.Remove();
                }
            }

            // 220ppi Print - said to be excellent on most printers and screens.
            // 150ppi Screen - said to be good for web pages and projectors.
            // 96ppi Email - said to be good for minimal document size and sharing.
            int desiredPpi = 150;

            // In .NET this seems to be a good compression / quality setting.
            int jpegQuality = 90;
            Resampler.Resample(this.doc, desiredPpi, jpegQuality);
            this.doc.AcceptAllRevisions();
            this.doc.Save(this.docStream, SaveOptions.CreateSaveOptions(SaveFormat.Doc));
            return this.docStream;
        }
        #endregion

        #region--SetStream
        /// <summary>
        /// 设置文档流
        /// </summary>
        /// <param name="docStream">文档流</param>
        public void SetStream(Stream docStream)
        {
            this.docStream = docStream;
            this.doc = new Document(docStream);

            // 清除下划线开头的书签
            this.DelBookmarks(this.doc, "_");
        }
        #endregion

        #region--CreateBlankDoc
        /// <summary>
        /// 创建空的合并文档
        /// </summary>
        public void CreateBlankDoc()
        {
            this.doc = new Document();
            this.doc.RemoveAllChildren();
            this.CreateBookmark("MergeDoc");
            this.docStream = new MemoryTributary();
        }
        #endregion

        #region--CreateBookmark
        /// <summary>
        /// 创建书签
        /// </summary>
        /// <param name="bookmarkName">书签名称</param>
        public void CreateBookmark(string bookmarkName)
        {
            DocumentBuilder builder = new DocumentBuilder(this.doc);
            builder.MoveToDocumentEnd();
            if (builder.CurrentParagraph != null && builder.CurrentParagraph.Runs.Count > 0)
            {
                builder.Writeln();
                this.clearBlankLineWithDocumentEnd = true;
            }

            builder.StartBookmark(bookmarkName);
            builder.EndBookmark(bookmarkName);
        }
        #endregion

        /// <summary>
        /// 设置页眉页脚
        /// </summary>
        /// <param name="dict">键值对</param>
        public void SetPageHeaderFooter(Dictionary<string, string> dict)
        {
            Regex r = new Regex(@"《\s*(?<labelName>[\u4e00-\u9fa5\w]+).?\s*》");
            DocumentBuilder builder = new DocumentBuilder(this.doc);
            for (int i = 0; i < this.doc.Sections.Count; i++)
            {
                builder.MoveToSection(i);
                builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
                this.SetLabelValue(dict, r, builder);
                builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
                this.SetLabelValue(dict, r, builder);
                builder.MoveToHeaderFooter(HeaderFooterType.HeaderEven);
                this.SetLabelValue(dict, r, builder);
                builder.MoveToHeaderFooter(HeaderFooterType.FooterFirst);
                this.SetLabelValue(dict, r, builder);
                builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
                this.SetLabelValue(dict, r, builder);
                builder.MoveToHeaderFooter(HeaderFooterType.FooterEven);
                this.SetLabelValue(dict, r, builder);
            }
        }

        /// <summary>
        /// 设置标签值
        /// </summary>
        /// <param name="dict">键值对</param>
        /// <param name="r">正则表达式</param>
        /// <param name="builder">文档构造类</param>
        private void SetLabelValue(Dictionary<string, string> dict, Regex r, DocumentBuilder builder)
        {
            if (builder.CurrentParagraph != null && builder.CurrentParagraph.Range != null)
            {
                var range = builder.CurrentParagraph.Range;
                if (r.IsMatch(range.Text))
                {
                    MatchCollection ms = r.Matches(range.Text);
                    foreach (Match m in ms)
                    {
                        var labelName = m.Groups["labelName"].Value;
                        if (dict.Any(it => it.Key == labelName))
                        {
                            range.Replace(new Regex("《\\s*" + labelName + "\\s*》"), dict[labelName]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取表格序号
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <returns>序号</returns>
        private int GetTableIndex(Table table)
        {
            var parentNode = table.ParentNode;
            if (parentNode == null)
            {
                return -1;
            }

            var tables = parentNode.GetChildNodes(NodeType.Table, true);
            return tables.IndexOf(table);
        }

        /// <summary>
        /// 查找指定类型的父类
        /// </summary>
        /// <param name="currentNode">当前节点</param>
        /// <param name="nodeType">节点类型</param>
        /// <returns>节点</returns>
        private Node FindParentNode(Node currentNode, NodeType nodeType)
        {
            Node parentNode = currentNode;
            while (true)
            {
                parentNode = parentNode.ParentNode;
                if (parentNode == null || (parentNode != null && parentNode.NodeType == nodeType))
                {
                    return parentNode;
                }
            }
            //return null;
        }

        /// <summary>
        /// 分节信息类
        /// </summary>
        public class TopicStartPara
        {
            /// <summary>
            /// 开始节点
            /// </summary>
             public Node TopicStartNode { get; set; }

            /// <summary>
            /// 源分节
            /// </summary>
             public Section SourceSection { get; set; }

            /// <summary>
            /// 分节类型
            /// </summary>
            public BreakType TargerBreakType { get; set; }

            /// <summary>
            /// 是否删除
            /// </summary>
            public bool IsRemove { get; set; }
        }
    }
}
