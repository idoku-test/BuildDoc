namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using BuildDoc.Entities;
    using BuildDoc.Logic;

    /// <summary>
    /// 多构件合并类
    /// </summary>
    public class DocMerger
    {
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }

        /// <summary>
        /// 文档操作类
        /// </summary>
        private IBuildWord buildWord;

        /// <summary>
        /// 实例文档ID列表
        /// </summary>
        private List<decimal> instanceDocumentIDList = null;

        /// <summary>
        /// 对象ID列表
        /// </summary>
        private List<decimal> objectIDList = null;

        /// <summary>
        /// 排序列表
        /// </summary>
        private List<MergerItem> orderList = new List<MergerItem>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mergerInstanceDocumentID">实例文档ID</param>
        /// <param name="instanceDocumentIDList">实例文档ID列表</param>
        /// <param name="objectIDList">对象ID列表</param>
        public DocMerger(decimal mergerInstanceDocumentID, List<decimal> instanceDocumentIDList, List<decimal> objectIDList)
        {
            this.buildWord = new BuildWord();

            this.instanceDocumentIDList = instanceDocumentIDList;
            this.objectIDList = objectIDList;

            InstanceDocumentDTO docInstance = BuildWordInstance.GetInstanceDocument(mergerInstanceDocumentID);
            if (docInstance != null) {
                JArray ary = JArray.Parse(docInstance.DOCUMENT_STRUCTURE);
                foreach (var ja in ary)
                {
                    this.orderList.Add(new MergerItem()
                    {
                        InstanceDocumentID = ja["InstanceId"].Value<decimal>(),
                        Key = ja["Key"].Value<decimal>()
                    });
                }
            }
        }

        /// <summary>
        /// 构建文档
        /// </summary>
        /// <returns>文档流</returns>
        public Stream BuildDoc()
        {
            for (var i = 0; i < this.instanceDocumentIDList.Count; i++)
            {
                DocMaster docMaster = new DocMaster(this.instanceDocumentIDList[i], this.objectIDList[i], true);
                foreach (var structureItem in docMaster.StructureInfoList)
                {
                    var findItem = this.orderList.Find(it => it.InstanceDocumentID == this.instanceDocumentIDList[i] && it.Key == structureItem.GetID);
                    if (findItem != null)
                    {
                        findItem.NewSection = structureItem.NewSection;
                        findItem.StructureStream = structureItem.BuildDoc();
                    }
                }
            }

            bool firstStructure = true;

            // 合并文档
            foreach (var mergerItem in this.orderList)
            {
                if (mergerItem.StructureStream != null)
                {
                    if (firstStructure)
                    {
                        this.buildWord.SetStream(mergerItem.StructureStream);
                        this.buildWord.CreateBookmark("MergeDoc");
                        firstStructure = false;
                    }
                    else
                    {
                        this.buildWord.InsertDoc("MergeDoc", mergerItem.StructureStream, mergerItem.NewSection);
                    }
                }
            }

            this.buildWord.DelBookmarks();
            this.buildWord.UpdateFields();
            var docStream = this.buildWord.GetStream();
            docStream.Position = 0;
            return docStream;
        }
    }

    /// <summary>
    /// 合并项信息
    /// </summary>
    public class MergerItem
    {
        /// <summary>
        /// 实例文档ID
        /// </summary>
        public decimal InstanceDocumentID
        {
            get;
            set;
        }

        /// <summary>
        /// 主键
        /// </summary>
        public decimal Key
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
        /// 构件文档流
        /// </summary>
        public Stream StructureStream
        {
            get;
            set;
        }
    }
}
