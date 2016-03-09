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
    /// 条件标签类
    /// </summary>
    public class ConditionLabel : BaseLabel
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        public List<ConditionItem> LabelList
        {
            get;
            set;
        }

        /// <summary>
        /// 基础标签
        /// </summary>
        private BaseLabel baseLabel;

        /// <summary>
        /// 构件ID
        /// </summary>
        private decimal structureID;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelConfig">配置信息</param>
        /// <param name="docMaster">文档主类</param>
        /// <param name="structureID">构件ID</param>
        public ConditionLabel(string labelName, JToken labelConfig, DocMaster docMaster, decimal structureID)
        {
            this.LabelList = new List<ConditionItem>();
            this.LabelName = labelName;
            this.structureID = structureID;
            JArray configAry = (JArray)labelConfig["Config"];

            foreach (var label in configAry)
            {
                ConditionItem item = new ConditionItem();
                item.Condition = label["Condition"].Value<string>();
                if (label["LabelType"] != null && !string.IsNullOrEmpty(label["LabelType"].ToString()))
                {
                    LabelType labelType = (LabelType)Enum.Parse(typeof(LabelType), label["LabelType"].Value<string>());
                    switch (labelType)
                    {
                        case LabelType.DocLabel:
                            item.BaseLabel = new DocLabel(labelName, label, docMaster);
                            break;
                        case LabelType.ImageLabel:
                            item.BaseLabel = new ImageLabel(labelName, label, docMaster.DocTemplateType);
                            break;
                        case LabelType.TableLabel:
                            item.BaseLabel = new TableLabel(labelName, label, docMaster.DocTemplateType);
                            break;
                        case LabelType.TextLabel:
                            item.BaseLabel = new TextLabel(labelName, label, docMaster, this.structureID);
                            break;
                    }
                }

                this.LabelList.Add(item);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="labelList">条件列表</param>
        public ConditionLabel(string labelName, List<ConditionItem> labelList)
        {
            this.LabelName = labelName;
            this.LabelList = labelList;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="buildWord">文档操作类</param>
        public override void Execute(IBuildWord buildWord)
        {
            if (this.baseLabel == null)
            {
                this.baseLabel = this.ConditionJudgment();
            }

            if (this.baseLabel != null)
            {
                this.baseLabel.Execute(buildWord);
            }
            else
            {
                buildWord.InsertText(this.LabelName, string.Empty);
            }
        }

        /// <summary>
        /// 替换引用值
        /// </summary>
        /// <param name="labelName">标签名称</param>
        /// <param name="value">替换从值</param>
        /// <returns>是否成功</returns>
        public override bool Replace(string labelName, string value)
        {
            bool result = false;
            if (this.LabelList != null)
            {
                this.LabelList.ForEach(label =>
                {
                    //if (label.Condition.Contains("@") && label.Condition.Contains("@" + labelName + " "))                    
                    if ((label.Condition.Contains("@") && label.Condition.Contains("@" + labelName)) || label.BaseLabel.RelateValue.Contains("@" + labelName))
                    {
                        result = true;
                        //label.Condition = label.Condition.Replace("@" + labelName + " ", value);
                        label.Condition = label.Condition.Replace("@" + labelName , value);
                        label.BaseLabel.Replace(labelName, value);
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// 根据条件选择标签
        /// </summary>
        /// <returns>返回符合条件的标签</returns>
        public BaseLabel ConditionJudgment()
        {
            BaseLabel baseLabel = null;

            foreach (var v in this.LabelList)
            {
                // 根据Condition判断
                if (DocHelper.CalcByJs(v.Condition))
                {
                    baseLabel = v.BaseLabel;
                    break;
                }
                else
                {
                    continue;
                }
            }

            return baseLabel;
        }

        /// <summary>
        /// 引用值
        /// </summary>
        public override string RelateValue
        {
            get
            {
                if (this.LabelList != null)
                {
                    var rst = this.LabelList.DefaultIfEmpty().FirstOrDefault(l => l.Condition.Contains("@"));
                    if (rst != null)
                    {
                        return rst.Condition;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// 条件项
    /// </summary>
    public class ConditionItem
    {
        /// <summary>
        /// 条件
        /// </summary>
        public string Condition
        {
            get;
            set;
        }

        /// <summary>
        /// 基础标签
        /// </summary>
        public BaseLabel BaseLabel
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 文档帮助类
    /// </summary>
    public class DocHelper
    {
        /// <summary>
        /// JS表达式执行
        /// </summary>
        /// <param name="expression">JS表达式</param>
        /// <returns>返回JS执行结果</returns>
        public static bool CalcByJs(string expression)
        {
            try
            {
                //string expressionTrim = System.Text.RegularExpressions.Regex.Replace(expression, @"\s", "");
                var myEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                string result = Microsoft.JScript.Eval.JScriptEvaluate(expression, myEngine).ToString();
                return bool.Parse(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 条件正则表达式
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>结果</returns>
        public static string PatternString(string condition)
        {
            return System.Text.RegularExpressions.Regex.Match(condition, @"(?<=@)[\w\W]+?(?=[\W])").Value.Trim();
        }
    }
}
