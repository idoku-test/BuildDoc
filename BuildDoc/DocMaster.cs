namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using System.Text.RegularExpressions;
    using BuildDoc.Entities;
    using BuildDoc.Logic;

    /// <summary>
    /// �ĵ���������
    /// </summary>
    public class DocMaster
    {
        private IBuildDocLogic BuildWordInstance
        {
            get { return BuildDocLogic.CreateInstance(); }
        }
             
        /// <summary>
        /// �ĵ����ɽӿ�
        /// </summary>
        private IBuildWord buildWord;

        /// <summary>
        /// ��ID
        /// </summary>
        private decimal masterID;

        /// <summary>
        /// ��������
        /// </summary>
        private string resultJson;

        /// <summary>
        /// inputJson�����ֵ�����������ɵ�ֵ
        /// </summary>
        public Dictionary<string, string> InputValue = new Dictionary<string, string>();

        /// <summary>
        /// ģ������
        /// </summary>
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// ������Ϣ�б�
        /// </summary>
        public List<IDocStructure> StructureInfoList
        {
            get;
            set;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string ResultJson
        {
            get
            {
                return this.resultJson;
            }
        }

        /// <summary>
        /// �ļ�ID
        /// </summary>
        public decimal FileID
        {
            get;
            set;
        }

        /// <summary>
        /// ��ǩ�б�
        /// </summary>
        public List<BaseLabel> LabelList
        {
            get;
            set;
        }
        /// <summary>
        /// �Ƿ�ϳɱ����ĵ�
        /// </summary>
        public bool IsBuildDoc
        { get; set; }

        /// <summary>
        /// ģ������
        /// </summary>
        internal DocTemplateType DocTemplateType
        {
            get;
            set;
        }

        /// <summary>
        /// ��̬������
        /// </summary>
        internal Dictionary<int, JArray> DynamicFormData
        {
            get;
            set;
        }
        /// <summary>
        /// ������Դ��ֵ�������ظ���ѯ
        /// </summary>
        internal Dictionary<string, string> MultiSourceValue
        { get; set; }

        /// <summary>
        /// ί�л�ȡ����
        /// </summary>
        internal Func<decimal, string, string, decimal, string, Dictionary<string, string>, string> GetValueDelegate
        {
            get;
            set;
        }

        /// <summary>
        /// ʵ��ID
        /// </summary>
        internal decimal InstanceID
        {
            get;
            set;
        }

        /// <summary>
        /// ��ʼ����ر���
        /// </summary>
        /// <param name="instanceDocumentID">ʵ���ĵ�ID</param>
        /// <param name="objID">����ID</param>
        //public DocMaster(decimal instanceDocumentID, decimal objID, bool isBuildDoc)
        //{
        //    this.LabelList = new List<BaseLabel>();

        //    // ����instanceDocumentID�����ݿ�ȡ��������� masterID instanceID resultJson jsonStructure
        //    using (BaseDB dbHelper = new OmpdDBHelper())
        //    {
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("I_INSTANCE_DOCUMENT_ID", instanceDocumentID);
        //        DataTable dt = dbHelper.ExecuteDataTableProc("pkg_redas_build_doc.sp_instance_document_get", dic);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            this.IsBuildDoc = isBuildDoc;
        //            this.masterID = Convert.ToDecimal(dt.Rows[0]["MOTHER_SET_ID"]);
        //            this.InstanceID = objID;
        //            this.resultJson = dt.Rows[0]["MANUAL_EDITING_RETURN"].ToString();
        //            this.InitData(dt.Rows[0]["DOCUMENT_STRUCTURE"].ToString(), null);
        //        }
        //    }
        //}

        /// <summary>
        /// ��ʼ����ر���
        /// </summary>
        /// <param name="masterID">��ID</param>
        /// <param name="instanceID">ʵ��ID</param>
        /// <param name="resultJson">��������</param>
        public DocMaster(decimal masterID, decimal instanceID, string resultJson, bool isBuildDoc)
            : this(masterID, instanceID, resultJson, null, isBuildDoc)
        {
        }

        /// <summary>
        /// ��ʼ����ر���
        /// </summary>
        /// <param name="masterID">��ID</param>
        /// <param name="instanceID">ʵ��ID</param>
        /// <param name="resultJson">��������</param>
        /// <param name="jsonStructure">��������</param>
        public DocMaster(decimal masterID, decimal instanceID, string resultJson, string jsonStructure, bool isBuildDoc) :
            this(masterID, instanceID, resultJson, jsonStructure, null, null, isBuildDoc)
        {
        }

        /// <summary>
        /// ��ʼ����ر���
        /// </summary>
        /// <param name="masterID">��ID</param>
        /// <param name="instanceID">ʵ��ID</param>
        /// <param name="resultJson">��������</param>
        /// <param name="jsonStructure">��������</param>
        /// <param name="getValueDelegate">��ȡֵ����</param>
        /// <param name="inputParams">�������</param>
        /// <param name="IsBuildDoc">�Ƿ��Ǻϳɱ���</param>
        public DocMaster(
            decimal masterID, 
            decimal instanceID, 
            string resultJson, 
            string jsonStructure,
            Func<decimal, string, string, decimal, string, Dictionary<string, string>, string> getValueDelegate, 
            Dictionary<string, string> inputParams,
            bool isBuildDoc
           )
        {
            this.IsBuildDoc = isBuildDoc;
            this.LabelList = new List<BaseLabel>();
            this.masterID = masterID;
            this.InstanceID = instanceID;
            this.resultJson = resultJson;
            this.GetValueDelegate = getValueDelegate;
            this.InitData(jsonStructure, inputParams);
         
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="jsonStructure">�����ṹ</param>
        /// <param name="inputParams">�������</param>
        private void InitData(string jsonStructure, Dictionary<string, string> inputParams)
        {
            MotherSetDTO motherSet = BuildWordInstance.GetMotherSet((int)this.masterID);
            if (motherSet != null)
            {
                this.FileID = motherSet.FILE_ID.Value;
                this.DocTemplateType = new DocTemplateType(motherSet.TEMPLATE_TYPE.Value, this.InstanceID, inputParams);
                var fileStream = FileServerHelper.GetFileStream(motherSet.FILE_ID.Value);
                if (fileStream != null)
                    this.buildWord = new BuildWord(fileStream);
                if(jsonStructure==null)
                    
            }
        }

        /// <summary>
        /// ����MasterIDȡ�������ṹ
        /// </summary>
        /// <param name="structureConfig">����������Ϣ</param>
        /// <returns>������Ϣ�б�</returns>
        private List<IDocStructure> GetStructureInfoList(Dictionary<BlockType, List<Structure>> structureConfig)
        {
            //decimal customerID = 0;
            List<IDocStructure> list = new List<IDocStructure>();
            //if (this.DocTemplateType.ParamsCache.ContainsKey("CustomerID"))
            //{
            //    decimal.TryParse(this.DocTemplateType.ParamsCache["CustomerID"], out customerID);
            //}


            foreach (var blockItem in structureConfig)
            {
                foreach (var structureItem in blockItem.Value)
                {
                    switch (structureItem.StructureType)
                    {
                        case StructureType.Config:
                            //����wordʱ����Ҫ���жϹ������Ƿ���Ч                               
                            if (this.IsBuildDoc || this.ConditionValid(structureItem.Condition, structureItem.Key))
                            {
                                var docStructure = new DocStructure(blockItem.Key, structureItem.Key, this);
                                docStructure.InitLabelList();
                                list.Add(docStructure);
                            }

                            break;
                        case StructureType.Custom:

                            list.Add(new CustomDocStructure(blockItem.Key, structureItem.Key, this, structureItem.Key, structureItem.StructureName));
                            break;
                    }
                }
            }

            foreach (IDocStructure item in list)
            {
                if (item is CustomDocStructure)
                {
                    item.InitLabelList();
                }
            }

            return list;

        }

        /// <summary>
        /// �����ж�
        /// </summary>
        /// <param name="condition">����</param>
        /// <param name="structureID">����ID</param>
        /// <returns>�Ƿ�ͨ��</returns>
        private bool ConditionValid(string condition, decimal structureID)
        {
            try
            {
                var result = false;
                if (!string.IsNullOrEmpty(condition))
                {

                    if (condition.Contains("@"))
                    {
                        string key = DocHelper.PatternString(condition);
                        if (this.DocTemplateType.ParamsCache.ContainsKey(key))
                        {
                            condition = condition.Replace("@" + key, this.DocTemplateType.ParamsCache[key]);
                        }
                    }

                    if (DocHelper.CalcByJs(condition))
                    {
                        result = true;
                    }

                }
                else
                {
                    result = true;
                }

                return result;
            }
            catch
            {

                throw new Exception("��֤ģ�幹��'" + structureID + "'������ʱ����");
            }
        }

        /// <summary>
        /// ��ȡ�����ṹ��Ϣ
        /// </summary>
        /// <param name="json">JSON�����ṹ��Ϣ</param>
        /// <returns>�����ṹ��Ϣ</returns>
        //private Dictionary<BlockType, List<Structure>> GetStructureDictionary(string json)
        //{
        //    var document_structure = new Redas.Logic.Document_structureLogic();
        //    Dictionary<BlockType, List<Structure>> structureDictionary = new Dictionary<BlockType, List<Structure>>();
        //    JObject j_structureConfig = JObject.Parse(json);
        //    string ids = string.Empty;
        //    foreach (var blockItem in j_structureConfig)
        //    {
        //        List<Structure> structureList = new List<Structure>();
        //        structureDictionary.Add((BlockType)Enum.Parse(typeof(BlockType), blockItem.Key), structureList);
        //        JArray j_structureList = blockItem.Value as JArray;

        //        foreach (var j_structure in j_structureList)
        //        {
        //            Structure structure = new Structure();
        //            structure.StructureType = (StructureType)Enum.Parse(typeof(StructureType), j_structure["Type"].Value<string>());
        //            structure.Key = j_structure["Key"].Value<decimal>();
        //            //structure.StructureName = j_structure["Name"].Value<string>();//�������ƻ��޸�

        //            structure.StructureName = document_structure.GetInfo(structure.Key, "", 0).STRUCTURE_NAME;
        //            structure.Condition = j_structure["Condition"].Value<string>();

        //            /*
        //            JObject j_Condition = j_structure["Condition"].Value<JObject>();
        //            if (j_Condition["DataLabelID"] != null)
        //            {

        //                structure.Condition.DataSourceName = j_Condition["DataSourceName"].Value<string>();
        //                structure.Condition.DataLabelName = j_Condition["DataLabelName"].Value<string>();
        //                structure.Condition.Operation = j_Condition["Operation"].Value<string>();
        //                structure.Condition.Value = j_Condition["Value"].Value<string>();
        //            }
        //            */
        //            structureList.Add(structure);
        //        }
        //    }

        //    return structureDictionary;
        //}

        /// <summary>
        /// û�е��õ�����ʱ���� ���й�������ǩ
        /// </summary>
        /// <returns></returns>
        /*
        public List<string> GetBookmarks()
        {
            List<string> bookmarks = new List<string>();
            foreach (var structureInfoItem in this.StructureInfoList)
            {
                var bookmarksWithStructure = structureInfoItem.GetBookmarks();
                foreach (var bookmark in bookmarksWithStructure)
                {
                    if (!bookmarks.Contains(bookmark))
                    {
                        bookmarks.Add(bookmark);
                    }
                }
            }
            return bookmarks;
        }
        */

        /// <summary>
        /// ��ȡ��ǰ�ĵ����е�����Դ
        /// </summary>
        /// <returns>����Դ�б�</returns>
        public List<DataSource> GetSourceList()
        {
            return this.DocTemplateType.DataSourceList;
        }

        /// <summary>
        /// �ϳ�ĸ��͹���
        /// </summary>
        /// <param name="structureInfoList">�����б�</param>
        private void UnionDoc(List<IDocStructure> structureInfoList)
        {
            if (this.buildWord == null)
            {
                bool firstStructure = true;
                this.buildWord = new BuildWord();
                foreach (var structureInfoItem in structureInfoList)
                {
                    var stream = structureInfoItem.BuildDoc();
                    if (stream != null)
                    {
                        if (firstStructure)
                        {
                            this.buildWord.SetStream(stream);
                            this.buildWord.CreateBookmark("MergeDoc");
                            firstStructure = false;
                        }
                        else
                        {
                            this.buildWord.InsertDoc("MergeDoc", stream, structureInfoItem.NewSection);
                        }

                        //this.GetTextLabelValue(dict, structureInfoItem);
                    }
                }
            }
            else
            {
                this.buildWord.CreateBookmark("MergeDoc");
                foreach (var structureInfoItem in structureInfoList)
                {
                    this.buildWord.InsertDoc("MergeDoc", structureInfoItem.BuildDoc(), structureInfoItem.NewSection);
                    //this.GetTextLabelValue(dict, structureInfoItem);
                }
            }

            // �滻ҳüҳ��
            var dict = new Dictionary<string, string>();
            var inside = this.LabelList.Where(t => !t.RelateValue.Contains('@')).ToList();
            inside.ForEach(item =>
            {
                if (item is TextLabel && item.LabelName != null && !dict.ContainsKey(item.LabelName))
                {
                    var label = item as TextLabel;
                    dict.Add(label.LabelName, label.GetValue());
                }
            });
            this.buildWord.SetPageHeaderFooter(dict);
        }

        /// <summary>
        /// ��ȡ�����ı���ǩֵ��ӵ�����Dict��
        /// </summary>
        /// <param name="dict">�ֵ�</param>
        /// <param name="structureInfoItem">������Ϣ��</param>
        private void GetTextLabelValue(Dictionary<string, string> dict, IDocStructure structureInfoItem)
        {
            if (structureInfoItem.LabelList != null)
            {
                foreach (var item in structureInfoItem.LabelList)
                {
                    if (item is TextLabel && item.LabelName != null && !dict.ContainsKey(item.LabelName))
                    {
                        var label = item as TextLabel;
                        dict.Add(label.LabelName, label.GetValue());
                    }
                }
            }
        }

        /// <summary>
        /// �����ĵ�
        /// </summary>
        /// <returns>�ĵ���</returns>
        public Stream BuildDoc()
        {
            this.UnionDoc(this.StructureInfoList);
            this.buildWord.DelBookmarks();
            this.buildWord.UpdateFields();
            var docStream = this.buildWord.GetStream();
            docStream.Position = 0;
            return docStream;
        }

        /// <summary>
        /// ��ȡ�����б�TextLabel����
        /// </summary>
        /// <returns>�����б�TextLabel����</returns>
        public List<TextLabelInfo> GetTextLabelList()
        {
            List<TextLabelInfo> labelList = new List<TextLabelInfo>();
            foreach (var structureInfoItem in this.StructureInfoList)
            {
                if (structureInfoItem is DocStructure)
                {
                    var tmpItem = structureInfoItem as DocStructure;
                    if (tmpItem.LabelList == null)
                    {
                        continue;
                    }

                    tmpItem.LabelList.ForEach(label =>
                    {
                        if (label is TextLabel)
                        {
                            var textLabel = label as TextLabel;
                            labelList.Add(new TextLabelInfo(
                                tmpItem.GetID, 
                                StructureType.Config, 
                                tmpItem.DocName, 
                                textLabel.LabelName, 
                                textLabel.FieldName,
                                textLabel.GetValue(),
                                textLabel.Relate, 
                                textLabel.FormatInfo, 
                                textLabel.DocControl,
                                textLabel.GetDataMethod, 
                                textLabel.DataSourceName, 
                                textLabel.ConfigValue, 
                                textLabel.FilterFieldName,
                                textLabel.FilterOperation, 
                                textLabel.FilterValue));
                        }
                        else if (label is ConditionLabel)
                        {
                            // var condLabel = label as ConditionLabel;
                            // var textLabel = condLabel.ConditionJudgment() as TextLabel;
                            // if (textLabel != null)
                            //    labelList.Add(new TextLabelInfo(tmpItem.GetID, StructureType.Config, tmpItem.DocName, textLabel.LabelName, textLabel.m_FieldName,
                            //        textLabel.GetValue(),
                            //        textLabel.Relate, textLabel.m_FormatInfo, textLabel.DocControl,
                            //        textLabel.m_GetDataMethod, textLabel.m_DataSourceName, textLabel.m_ConfigValue, textLabel.m_FilterFieldName,
                            //        textLabel.m_FilterOperation, textLabel.m_FilterValue));
                        }
                    });
                }
                else if (structureInfoItem is CustomDocStructure)
                {
                    var tmpItem = structureInfoItem as CustomDocStructure;
                    if (tmpItem.LabelList == null)
                    {
                        continue;
                    }

                    tmpItem.LabelList.ForEach(label =>
                    {
                        if (label is TextLabel)
                        {
                            var textLabel = label as TextLabel;
                            if (!string.IsNullOrEmpty(textLabel.LabelName))
                            {
                                labelList.Add(
                                    new TextLabelInfo(
                                        tmpItem.GetID, 
                                        StructureType.Custom,
                                        tmpItem.StructureName,
                                        textLabel.LabelName,
                                        textLabel.FieldName,
                                        textLabel.GetValue(),
                                        textLabel.Relate, 
                                        textLabel.FormatInfo, 
                                        textLabel.DocControl,
                                        textLabel.GetDataMethod, 
                                        textLabel.DataSourceName, 
                                        textLabel.ConfigValue,
                                        textLabel.FilterFieldName,
                                        textLabel.FilterOperation, 
                                        textLabel.FilterValue));
                            }
                        }
                    });
                }
            }

            return labelList;
        }

        /// <summary>
        /// ��ȡ�����б�ؼ�����Դ����
        /// </summary>
        /// <returns>����Դ�б�</returns>
        public Dictionary<string, JArray> GetDataSourceList()
        {
            Dictionary<string, JArray> dataSourceList = new Dictionary<string, JArray>();
            Dictionary<string, List<string>> tmpDS = new Dictionary<string, List<string>>();
            foreach (var structureInfoItem in this.StructureInfoList)
            {
                if (structureInfoItem is DocStructure)
                {
                    var tmpItem = structureInfoItem as DocStructure;
                    if (tmpItem.LabelList == null)
                    {
                        continue;
                    }

                    tmpItem.LabelList.ForEach(label =>
                    {
                        if (label is TextLabel)
                        {
                            TextLabel textLabel = label as TextLabel;
                            if (textLabel.GetDataMethod == GetDataMethod.Source && textLabel.DataSourceName.Trim() != string.Empty)
                            {
                                List<string> tmpFields = null;
                                if (tmpDS.ContainsKey(textLabel.DataSourceName))
                                {
                                    tmpFields = tmpDS[textLabel.DataSourceName];
                                }
                                else
                                {
                                    tmpFields = new List<string>();
                                    tmpDS.Add(textLabel.DataSourceName, tmpFields);
                                }

                                if (!tmpFields.Contains(textLabel.FilterFieldName))
                                {
                                    tmpFields.Add(textLabel.FilterFieldName);
                                }

                                if (!tmpFields.Contains(textLabel.FieldName))
                                {
                                    tmpFields.Add(textLabel.FieldName);
                                }
                            }

                            if (textLabel.DocControl is DropdownWithDataSourceControl)
                            {
                                var dropdownWithDataSourceControl = textLabel.DocControl as DropdownWithDataSourceControl;
                                List<string> tmpFields = null;
                                if (tmpDS.ContainsKey(dropdownWithDataSourceControl.DataSource))
                                {
                                    tmpFields = tmpDS[dropdownWithDataSourceControl.DataSource];
                                }
                                else
                                {
                                    tmpFields = new List<string>();
                                    tmpDS.Add(dropdownWithDataSourceControl.DataSource, tmpFields);
                                }

                                if (!tmpFields.Contains(dropdownWithDataSourceControl.TextField))
                                {
                                    tmpFields.Add(dropdownWithDataSourceControl.TextField);
                                }

                                if (!tmpFields.Contains(dropdownWithDataSourceControl.ValueField))
                                {
                                    tmpFields.Add(dropdownWithDataSourceControl.ValueField);
                                }

                                if (textLabel.Relate != null)
                                {
                                    foreach (var relateItem in textLabel.Relate)
                                    {
                                        if (!tmpFields.Contains(relateItem.FieldName))
                                        {
                                            tmpFields.Add(relateItem.FieldName);
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
            }

            foreach (var item in tmpDS)
            {
                var dataSource = this.DocTemplateType.DataSourceList.Find(it => it.DataSourceName.Equals(item.Key));
                JArray newJArray = new JArray();
                if (dataSource != null)
                {
                    DataTable dt = dataSource.GetDataTable();
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            JObject newJObject = new JObject();
                            foreach (var field in item.Value)
                            {
                                string newField = field.Replace("@", string.Empty);
                                if (field.Trim() == string.Empty || newJObject.Property(newField) != null)
                                {
                                    continue;
                                }

                                // if (field.Trim() == "" || newJObject.ToString().IndexOf("\"" + newField + "\":")>=0)
                                newJObject.Add(new JProperty(newField, dr[newField].ToString()));
                            }

                            newJArray.Add(newJObject);
                        }

                        dataSourceList.Add(item.Key, newJArray);
                    }
                }
            }

            return dataSourceList;
        }
    } // end CompanyTemplate

    /// <summary>
    /// ������
    /// </summary>
    public class Structure
    {
        /// <summary>
        /// ��������
        /// </summary>
        public StructureType StructureType
        {
            get;
            set;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string StructureName
        {
            get;
            set;
        }

        /// <summary>
        /// ����
        /// </summary>
        public decimal Key
        {
            get;
            set;
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Condition
        {
            get;
            set;
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// ����Դ
        /// </summary>
        public string DataSourceName
        {
            get;
            set;
        }

        /// <summary>
        /// ���ݱ�ǩ����
        /// </summary>
        public string DataLabelName
        {
            get;
            set;
        }

        /// <summary>
        /// ������
        /// </summary>
        public string Operation
        {
            get;
            set;
        }

        /// <summary>
        /// ֵ
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
} // end namespace