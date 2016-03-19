namespace BuildDoc
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel;

    /// <summary>
    /// 输出信息格式化
    /// </summary>
    public class FormatInfo
    {
        /// <summary>
        /// 输出格式化字符
        /// </summary>
        private string formatString = string.Empty;

        /// <summary>
        /// 格式化类型
        /// </summary>
        private FormatType formatType;

        /// <summary>
        /// 数据源
        /// </summary>
        private DataSource dataSource;

        /// <summary>
        /// 值字段名称
        /// </summary>
        public string ValueField
        {
            get;
            set;
        }

        /// <summary>
        /// 文本字段名称
        /// </summary>
        public string TextField
        {
            get;
            set;
        }

        /// <summary>
        /// 小数点位数
        /// </summary>
        private int decimalCount = 0;

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int DecimalCount
        {
            get
            {
                return this.decimalCount;
            }
            set
            {
                this.decimalCount = value;
            }
        }

        /// <summary>
        /// 除数
        /// </summary>
        private decimal dividend = 0;

        /// <summary>
        /// 除数
        /// </summary>
        public decimal Dividend
        {
            get
            {
                return this.dividend;
            }
            set
            {
                this.dividend = value;
            }
        }

        /// <summary>
        /// 日期格式化字符
        /// </summary>
        public string DateFormatString
        {
            get;
            set;
        }

        /// <summary>
        /// 是否转化为中文年月日
        /// </summary>
        private bool isUpperDate = false;

        /// <summary>
        /// 是否转化为中文年月日
        /// </summary>
        public bool IsUpperDate
        {
            get
            {
                return this.isUpperDate;
            }
            set
            {
                this.isUpperDate = value;
            }
        }

        /// <summary>
        /// 公式引用值是否使用格式化后
        /// </summary>
        private bool isAfterCompute = false;

        /// <summary>
        /// 公式引用值是否使用格式化后
        /// </summary>
        public bool IsAfterCompute
        {
            get
            {
                return this.isAfterCompute;
            }
            set
            {
                this.isAfterCompute = value;
            }
        }

        /// <summary>
        /// 是否使用千分位显示
        /// </summary>
        private bool isThousand = false;

        /// <summary>
        /// 是否使用千分位显示
        /// </summary>
        public bool IsThousand
        {
            get
            {
                return this.isThousand;
            }
            set
            {
                this.isThousand = value;
            }
        }

        /// <summary>
        /// 是否格式化信息配置
        /// </summary>
        [DefaultValue(false)]
        public bool HasValues
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">格式化信息</param>
        /// <param name="docTemplateType">模板类型</param>
        public FormatInfo(JToken config, DocTemplateType docTemplateType)
        {
            this.HasValues = config.HasValues;
            string formatType = string.Empty;
            if (!this.HasValues)
            {
                return;
            }

            if (config["FormatString"] != null)
            {
                this.formatString = config["FormatString"].Value<string>();
            }

            if (config["FormatType"] != null)
            {
                formatType = config["FormatType"].Value<string>();
            }

            if (!string.IsNullOrEmpty(formatType) && Enum.IsDefined(typeof(FormatType), formatType))
            {
                this.formatType = (FormatType)Enum.Parse(typeof(FormatType), formatType);
            }

            if (config["ValueField"] != null)
            {
                this.ValueField = config["ValueField"].Value<string>();
            }

            if (config["TextField"] != null)
            {
                this.TextField = config["TextField"].Value<string>();
            }

            if (config["DecimalCount"] != null)
            {
                int.TryParse(config["DecimalCount"].Value<string>(), out this.decimalCount);
            }

            if (config["Dividend"] != null)
            {
                decimal.TryParse(config["Dividend"].Value<string>(), out this.dividend);
            }

            if (config["DataSourceName"] != null)
            {
                string sourceName = config["DataSourceName"].Value<string>();
                this.dataSource = docTemplateType.DataSourceList.FirstOrDefault(t => t.DataSourceName == sourceName);
            }

            if (config["DateFormatString"] != null)
            {
                this.DateFormatString = config["DateFormatString"].Value<string>();
            }

            if (config["IsUpperDate"] != null)
            {
                bool.TryParse(config["IsUpperDate"].Value<string>(), out this.isUpperDate);
            }

            if (config["IsAfterCompute"] != null)
            {
                bool.TryParse(config["IsAfterCompute"].Value<string>(), out this.isAfterCompute);
            }

            if (config["IsThousand"] != null)
            {
                bool.TryParse(config["IsThousand"].Value<string>(), out this.isThousand);
            }
        }

        /// <summary>
        /// 返回格式化后的值
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>格式化输出值</returns>
        public string ToFormatString(string value)
        {
            string tmpValue = string.Empty;
            switch (this.formatType)
            {
                case FormatType.Number:
                    tmpValue = this.Number(value);
                    break;
                case FormatType.Date:
                    tmpValue = this.FormatDate(value);
                    break;
                case FormatType.Upper:
                    tmpValue = this.UpperNumber1(this.Fixed(value));
                    break;
                case FormatType.Ammount:
                    tmpValue = this.UpperNumber2(this.Fixed(value));
                    break;
                case FormatType.ValueToText:
                    tmpValue = this.ValueToText(value);
                    break;
            }

            if (!string.IsNullOrEmpty(this.formatString))
            {
                return string.Format(this.formatString, tmpValue);
            }
            else
            {
                return tmpValue;
            }
        }

        /// <summary>
        /// 字典值转换为文本
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string ValueToText(string value)
        {
            return this.dataSource.GetDictionaryText(this.TextField, this.ValueField, value);
        }

        /// <summary>
        /// 格式化日期
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string FormatDate(string value)
        {
            string result = string.Empty;
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(value, out date))
            {
                result = date.ToString(this.DateFormatString);
                if (this.IsUpperDate)
                {
                    result = date.ToString("yyyy-MM-dd");
                    result = DateConvert.Instance.Baodate2Chinese(result);
                }
            }

            return result;
        }

        /// <summary>
        /// 取小数点几位
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string Number(string value)
        {
            decimal result, divisor;
            if (decimal.TryParse(value, out result))
            {
                divisor = result;
                if (this.Dividend > 0)
                {
                    result = divisor / this.Dividend;
                }

                string formatChar = "F";
                if (this.IsThousand)
                {
                    formatChar = "N";
                }

                if (this.DecimalCount >= 0)
                {
                    return result.ToString(formatChar + this.DecimalCount);
                }
                else
                {
                    return result.ToString();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 把整数的后几位归0
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string Fixed(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = this.Number(value);
            if (this.Dividend > 0)
            {
                value = (decimal.Parse(value) * this.Dividend).ToString();
            }

            decimal result;
            if (decimal.TryParse(value, out result))
            {
                if (this.DecimalCount >= 0)
                {
                    decimal tmp = (decimal)Math.Pow(10, this.DecimalCount);
                    return Convert.ToInt64((result / tmp) * tmp).ToString();
                }
                else
                {
                    return result.ToString();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 数字转换为一二三...九
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string UpperNumber1(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                DigitToChineseText obj = new DigitToChineseText();
                return obj.Convert(value, false);
            }

            return string.Empty;
        }

        /// <summary>
        /// 数字转换为零、壹、贰、叁、肆、伍、陆、柒、捌、玖
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>转换后值</returns>
        private string UpperNumber2(string value)
        {
            if (this.IsThousand)
            {
                return this.Number(value);
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DigitToChineseText obj = new DigitToChineseText();
                    return obj.Convert(value, true);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 日期转换类
        /// </summary>
        private class DateConvert
        {
            /// <summary>
            /// 单例
            /// </summary>
            private static DateConvert dateConvert = null;

            /// <summary>
            /// 中文数组
            /// </summary>
            private char[] strChinese;

            /// <summary>
            /// 初始化
            /// </summary>
            private DateConvert()
            {
                this.strChinese = new char[] 
                {
                    '〇', '一', '二', '三', '四', '五', '六', '七', '八', '九', '十'
                };
            }

            /// <summary>
            /// 实例化
            /// </summary>
            public static DateConvert Instance
            {
                get
                {
                    if (dateConvert == null)
                    {
                        dateConvert = new DateConvert();
                    }

                    return dateConvert;
                }
            }

            /// <summary>
            /// 日期转换为中文日期
            /// </summary>
            /// <param name="strDate">原始日期</param>
            /// <returns>转换后值</returns>
            public string Baodate2Chinese(string strDate)
            {
                StringBuilder result = new StringBuilder();

                // 依据正则表达式判断参数是否正确
                Regex theReg = new Regex(@"(\d{2}|\d{4})(/|-)(\d{1,2})(/|-)(\d{1,2})");
                if (theReg.Match(strDate).Length != 0)
                {
                    // 将数字日期的年月日存到字符数组str中
                    string[] str = null;
                    if (strDate.Contains("-"))
                    {
                        str = strDate.Split('-');
                    }
                    else if (strDate.Contains("/"))
                    {
                        str = strDate.Split('/');
                    }

                    // str[0]中为年，将其各个字符转换为相应的汉字
                    for (int i = 0; i < str[0].Length; i++)
                    {
                        result.Append(this.strChinese[int.Parse(str[0][i].ToString())]);
                    }

                    result.Append("年");

                    // 转换月
                    int month = int.Parse(str[1]);
                    int mn1 = month / 10;
                    int mn2 = month % 10;
                    if (mn1 > 1)
                    {
                        result.Append(this.strChinese[mn1]);
                    }

                    if (mn1 > 0)
                    {
                        result.Append(this.strChinese[10]);
                    }

                    if (mn2 != 0)
                    {
                        result.Append(this.strChinese[mn2]);
                    }

                    result.Append("月");

                    // 转换日
                    int day = int.Parse(str[2]);
                    int dn1 = day / 10;
                    int dn2 = day % 10;
                    if (dn1 > 1)
                    {
                        result.Append(this.strChinese[dn1]);
                    }

                    if (dn1 > 0)
                    {
                        result.Append(this.strChinese[10]);
                    }

                    if (dn2 != 0)
                    {
                        result.Append(this.strChinese[dn2]);
                    }

                    result.Append("日");
                }
                else
                {
                    throw new ArgumentException();
                }

                return result.ToString();
            }
        }
    } // end FormatInfo
} // end namespace 文档模板配置