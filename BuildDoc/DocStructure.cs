namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Data;
    using Newtonsoft.Json.Linq;
    using BuildDoc.Logic;


    public class DocStructure : IDocStructure
    {
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }

        public BlockType BlockType { get; set; }
        public string DocName;
        public decimal FileID;
        public string Json;
        /// <summary>
        /// 标签列表
        /// </summary>
        public List<BaseLabel> LabelList
        {
            get;
            set;
        }
        public bool NewSection { get; set; }

        protected IBuildWord buildWord;

        private DocMaster docMaster;
        private decimal structureID;

        public DocStructure(BlockType blockType, decimal structureID, DocMaster docMaster)
        {
            //根据structureID从数据库取出相关数据
            //BlockType blockType, string docName, decimal fileID, string json
            this.BlockType = blockType;
            this.structureID = structureID;
            //this.DocName = docName;
            //this.FileID = fileID;
            //this.Json = json;
            this.docMaster = docMaster;
            string error = "";
            try
            {
                var structure = BuildWordInstance.GetStructure((int)structureID);
                    if (structure!= null)
                    {
                        error = "构件'" + structure.STRUCTURE_NAME + "'文件不存在";
                        this.DocName = structure.STRUCTURE_NAME;
                        this.FileID = structure.FILE_ID.Value;
                        this.Json = structure.SET_CONTENT;
                        this.NewSection = (int)structure.IS_NEW_SECTION == 1;
                        this.buildWord = new BuildWord(FileServerHelper.GetFileStream(this.FileID));
                        //this.InitLabel(null);
                    }
              
            }
            catch
            {
                throw new Exception(error);
            }
            /*
            if (isInitLabel && this.LabelList != null && this.LabelList.Count > 0)
                this.InitLabel(new Dictionary<string, string>());
            */
        }

        /// <summary>
        /// 生成Word文件
        /// </summary>
        /// <returns></returns>
        public Stream BuildDoc()
        {
            if (this.LabelList != null)
            {
                /*
                //inputJson传入的值覆盖配置生成的值
                Dictionary<string, string> inputValue = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(inputJson))
                {
                    JArray ary = JArray.Parse(inputJson);
                    decimal id;
                    StructureType type;
                    foreach (var v in ary)
                    {
                        id = v["ID"].Value<decimal>();
                        type = (StructureType)Enum.Parse(typeof(StructureType), v["StructureType"].Value<string>());

                        if (this.structureID == id && type == StructureType.Config)
                            inputValue.Add(v["LabelName"].Value<string>(), v["Value"].Value<string>());
                    }
                }

                this.InitLabel(inputValue);
                */
                //替换书签内容
                foreach (var label in this.LabelList)
                {
                    label.Execute(this.buildWord);
                }
            }

            return this.buildWord.GetStream();
        }

        /// <summary>
        /// 初始化占位符或公式引用
        /// </summary>
        /// <param name="inputValue"></param>
        //private void InitLabel(Dictionary<string, string> inputValue)
        //{
        //    //应用替换值
        //    if (inputValue != null && inputValue.Count > 0)
        //    {
        //        this.LabelList.ForEach(label =>
        //        {
        //            if (label is TextLabel)
        //            {
        //                var textLabel = label as TextLabel;
        //                var input = inputValue.FirstOrDefault(t => t.Key == label.LabelName);
        //                if (!string.IsNullOrEmpty(input.Value))
        //                {
        //                    textLabel.IsInput = true;
        //                    textLabel.Value = input.Value;
        //                }
        //            }
        //        });
        //    }

            
        //    var inside = this.LabelList.Where(t => !t.RelateValue.Contains('@')).ToList();
        //    var outside = this.LabelList.Where(t => t.RelateValue.Contains('@')).ToList();

        //    var tmpList = new List<BaseLabel>();
        //    while (true)
        //    {
        //        bool isBreak = true;
        //        foreach (var oItem in outside)
        //        {
        //            foreach (var iItem in inside)
        //            {
        //                if (iItem is TextLabel)
        //                {
        //                    var textLabel = iItem as TextLabel;
        //                    //var value = string.IsNullOrEmpty(textLabel.RelateValue) ? textLabel.GetValue() : textLabel.RelateValue;

        //                    var value = textLabel.GetValue();
        //                    if (!textLabel.IsAfterCompute)
        //                        value = textLabel.InnerValue;

        //                    bool pass = oItem.Replace(iItem.LabelName, value);
        //                    if (!tmpList.Contains(oItem) && !oItem.RelateValue.Contains("@"))
        //                        tmpList.Add(oItem);
        //                    if (isBreak && pass)
        //                        isBreak = false;

        //                }
        //            }
        //        }
        //        foreach (var item in tmpList)
        //        {
        //            inside.Add(item);
        //            outside.Remove(item);
        //        }
        //        tmpList.Clear();
        //        if (isBreak)
        //            break;
        //    }

        //    var allInside = this.docMaster.LabelList.Where(t => !t.RelateValue.Contains('@')).ToList();
        //    //处理构建里无匹配的标签
        //    this.LabelList.ForEach(label =>
        //    {
        //        if (label is ConditionLabel)
        //        {
        //            var cl = label as ConditionLabel;
        //            cl.LabelList.ForEach(l =>
        //            {
        //                string key = DocHelper.PatternString(l.Condition);
        //                var baseLable = allInside.FirstOrDefault(i => i.LabelName == key);
        //                if (baseLable != null && baseLable is TextLabel)
        //                {
        //                    try
        //                    {
        //                        var textLabel = baseLable as TextLabel;
        //                        var value = string.IsNullOrEmpty(textLabel.RelateValue) ? textLabel.GetValue() : textLabel.RelateValue;
        //                        l.Condition = l.Condition.Replace("@" + key, value);
        //                    }
        //                    catch { }
        //                }
        //            });
        //        }
        //    });
        //}

        /// <summary>
        /// 初始化标签列表
        /// </summary>
        public void InitLabelList()
        {
            this.LabelList = this.GetLabelList();
        }

        /// <summary>
        /// 构建标签列表
        /// </summary>
        /// <returns></returns>
        private List<BaseLabel> GetLabelList()
        {
            #region--json
            //            json = @"[
            //            {
            //	            LabelName:'表标签',
            //	            LabelType:'TableLabel',
            //	            Config:{
            //	                DataSourceName:'dsn',
            //	                FilterFieldName:'id',
            //	                FilterOperation:'=',
            //	                FilterValue:'11',
            //	                FillType:'OnlyFillByRow',
            //	                Top:4,
            //	                ColumnInfo:[
            //	                    {
            //		                    FieldName:'title',
            //		                    ColumnIndex:0,
            //		                    FormatInfo:{
            //                                FormatType:'ValueToText',
            //	                            FormatString:'{0}傻X',
            //	                            DataSourceName:'sourceName',
            //	                            ValueField:'Value',
            //	                            TextField:'Text'
            //                            }
            //	                    },
            //                        {
            //		                    FieldName:'title2',
            //		                    ColumnIndex:1,
            //		                    FormatInfo:{
            //                                FormatType:'Number',
            //	                            FormatString:'{0}万元',
            //	                            DecimalCount:2
            //                            }
            //	                    }
            //                    ]
            //                },
            //	            Control:{},
            //	            Relate:[]
            //            }]";

            //            json = @"[
            //            {
            //	            LabelName:'图片标签',
            //	            LabelType:'ImageLabel',
            //	            Config:{
            //	                ImageName:'科兴科学园.jpg',
            //	                FileID:'10001'
            //                },
            //	            Control:{},
            //	            Relate:[]
            //            }]";

            //            json = @"[
            //            {
            //	            LabelName:'文档标签',
            //	            LabelType:'DocLabel',
            //	            Config:{
            //	                GetDataMethod:'Const',
            //		            DocName:'文档名',
            //		            FileID:'10002'
            //                },
            //	            Control:{},
            //	            Relate:[]
            //            }]";

            //            json = @"[
            //            {
            //	            LabelName:'文本标签',
            //	            LabelType:'TextLabel',
            //	            Config:{
            //	                GetDataMethod:'Source',
            //		            DataSourceName:'11',
            //		            FieldName:'23',
            //		            FilterFieldName:'33',
            //		            FilterOperation:'44',
            //		            FilterValue:'55',
            //		            FormatInfo:{
            //                        FormatType:'Number',
            //	                    FormatString:'{0}万元',
            //	                    DecimalCount:2,
            //                        Dividend:0
            //                    }
            //                },
            //	            Control:{},
            //	            Relate:[]
            //            }]";
            #endregion

            string error = "";
            try
            {
                if (string.IsNullOrEmpty(this.Json)) return null;

                List<BaseLabel> labelList = new List<BaseLabel>();
                JArray ary = JArray.Parse(this.Json);
                string labelName = string.Empty;
                LabelType labelType;

                foreach (var label in ary)
                {
                    if (label["LabelName"] != null)
                    {
                        labelName = label["LabelName"].Value<string>();
                    }
                    error = "标签'" + labelName + "'出错";
                    BaseLabel baseLabel = this.docMaster.LabelList.Find(it => it.LabelName == labelName);
                    if (baseLabel != null)
                    {
                        labelList.Add(baseLabel);
                    }
                    else
                    {
                        string rs="";
                        List<string> s = new List<string>();
                        s.ForEach(r => rs += r);

                        if (label["LabelType"] != null && !string.IsNullOrEmpty(label["LabelType"].ToString()))
                        {
                            labelType = (LabelType)Enum.Parse(typeof(LabelType), label["LabelType"].Value<string>());
                            switch (labelType)
                            {
                                case LabelType.TextLabel:
                                    baseLabel = new TextLabel(labelName, label, this.docMaster, this.structureID);
                                    break;
                                case LabelType.TableLabel:
                                    baseLabel = new TableLabel(labelName, label, this.docMaster.DocTemplateType);
                                    break;
                                case LabelType.ConditionLabel:
                                    baseLabel = new ConditionLabel(labelName, label, this.docMaster, this.structureID);
                                    break;
                                case LabelType.DocLabel:
                                    baseLabel = new DocLabel(labelName, label, this.docMaster);
                                    break;
                                case LabelType.ImageLabel:
                                    baseLabel = new ImageLabel(labelName, label, this.docMaster.DocTemplateType);
                                    break;
                            }
                            if (baseLabel != null)
                            {
                                baseLabel.LabelTypeName = labelType.ToString();
                                labelList.Add(baseLabel);
                                this.docMaster.LabelList.Add(baseLabel);
                            }
                        }
                    }
                }
                return labelList;
            }
            catch {
                throw new Exception(error);
            }
        }

        /// <summary>
        /// 所有当前文档的书签
        /// </summary>
        /// <returns></returns>
        public List<string> GetBookmarks()
        {
            return this.buildWord.GetBookmarks();
        }

        public decimal GetID
        {
            get { return this.structureID; }
        }
    }
}
