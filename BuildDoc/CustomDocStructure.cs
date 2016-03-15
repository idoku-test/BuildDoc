namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 自定义构件
    /// </summary>
    public class CustomDocStructure : IDocStructure
    {
        /// <summary>
        /// 文档生成类
        /// </summary>
        private IBuildWord buildWord;

        /// <summary>
        /// 文档主类
        /// </summary>
        private DocMaster docMaster;

        /// <summary>
        /// 构件ID
        /// </summary>
        private decimal structureID;

        /// <summary>
        /// 客户ID
        /// </summary>
        //private decimal customerID;

        /// <summary>
        /// 文件ID
        /// </summary>
        public decimal FileID
        {
            get;
            set;
        }

        /// <summary>
        /// 文档块
        /// </summary>
        public BlockType BlockType
        { 
            get; 
            set;
        }

        /// <summary>
        /// 标签列表
        /// </summary>
        public List<BaseLabel> LabelList
        {
            get;
            set;
        }

        /// <summary>
        /// 构件名称
        /// </summary>
        public string StructureName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否新节
        /// </summary>
        public bool NewSection
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="blockType">文档块</param>
        /// <param name="fileID">文件ID</param>
        /// <param name="docMaster">文档主类</param>
        /// <param name="structureID">构件ID</param>
        /// <param name="structureName">构件名称</param>
        /// <param name="customerID">客户ID</param>
        public CustomDocStructure(BlockType blockType, decimal fileID, DocMaster docMaster, decimal structureID, string structureName)
        {
            try
            {
                this.BlockType = blockType;
                this.FileID = fileID;
                this.docMaster = docMaster;
                this.StructureName = structureName;
                this.structureID = structureID;
                //this.customerID = customerID;
                this.NewSection = false;
                //this.buildWord = new BuildWord(FileHelper.GetFileStream(this.FileID));
                //this.LabelList = this.GetLabelList();
            }
            catch {
                throw new Exception("自定义构件'" + structureName + "'出错");
            }
        }

        /// <summary>
        /// 生成文档
        /// </summary>
        /// <param name="inputJson">输入值</param>
        /// <returns>文档流</returns>
        public Stream BuildDoc()
        {
            /*
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

                    if (this.FileID == id && type == StructureType.Custom)
                    {
                        inputValue.Add(v["LabelName"].Value<string>(), v["Value"].Value<string>());
                    }
                }
            }
            */

            // 替换书签内容
            foreach (var label in this.LabelList)
            {
                label.Execute(this.buildWord);
            }

            return this.buildWord.GetStream();
        }

        /// <summary>
        /// 初始化标签列表
        /// </summary>
        public void InitLabelList()
        {
            this.LabelList = this.GetLabelList();
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns>标签列表</returns>
        private List<BaseLabel> GetLabelList()
        {

            string error = "";
            try
            {
                List<BaseLabel> labelList = new List<BaseLabel>();
                List<string> labels = this.buildWord.GetAllMarks(@"《[^》]+》");
                foreach (var name in labels)
                {
                    string labelName = name.TrimStart('《').TrimEnd('》');
                    error = "标签'" + labelName + "'出错";
                    BaseLabel label = this.docMaster.LabelList.Find(it => it.LabelName == labelName);
                    if (label == null)
                    {
                        JObject config = JObject.Parse(@"
                    {
                      ""LabelName"": """ + labelName + @""",
                      ""LabelType"": ""TextLabel"",
                      ""Relate"": [],
                      ""Config"": {
                        ""GetDataMethod"": ""Const"",
                        ""Value"": """"
                      },
                      ""Control"": {
                        ""ControlType"": ""Text"",
                        ""Required"": ""false"",
                        ""ValidateString"": """"
                      }
                    }");
                        label = new TextLabel(labelName, config, this.docMaster, this.structureID);
                        this.docMaster.LabelList.Add(label);
                    }
                    labelList.Add(label);
                }

                return labelList;
            }

            catch
            {
                throw new Exception(error);
            }
        }
        /*
        private List<DataLabel> GetLabelList()
        {
            List<DataLabel> labelList = new List<DataLabel>();
            List<string> labels = this.buildWord.GetAllMarks(@"《[^》]+》");

            foreach (var name in labels)
            {
                string labelName = name.TrimStart('《').TrimEnd('》');
                if (!labelList.Any(it => it.LabelName == labelName))
                {
                    var label = new DataLabel(labelName, this.docMaster, this.structureID, this.customerID);
                    if (!string.IsNullOrEmpty(label.LabelName))
                    {
                        labelList.Add(label);
                    }
                }
            }

            return labelList;
        }
        */

        /// <summary>
        /// 文件ID
        /// </summary>
        public decimal GetID
        {
            get
            {
                return this.FileID;
            }
        }
    }
}
