namespace BuildDoc
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel;

    /// <summary>
    /// �����Ϣ��ʽ��
    /// </summary>
    public class FormatInfo
    {
        /// <summary>
        /// �����ʽ���ַ�
        /// </summary>
        private string formatString = string.Empty;

        /// <summary>
        /// ��ʽ������
        /// </summary>
        private FormatType formatType;

        /// <summary>
        /// ����Դ
        /// </summary>
        private DataSource dataSource;

        /// <summary>
        /// ֵ�ֶ�����
        /// </summary>
        public string ValueField
        {
            get;
            set;
        }

        /// <summary>
        /// �ı��ֶ�����
        /// </summary>
        public string TextField
        {
            get;
            set;
        }

        /// <summary>
        /// С����λ��
        /// </summary>
        private int decimalCount = 0;

        /// <summary>
        /// С����λ��
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
        /// ����
        /// </summary>
        private decimal dividend = 0;

        /// <summary>
        /// ����
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
        /// ���ڸ�ʽ���ַ�
        /// </summary>
        public string DateFormatString
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�ת��Ϊ����������
        /// </summary>
        private bool isUpperDate = false;

        /// <summary>
        /// �Ƿ�ת��Ϊ����������
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
        /// ��ʽ����ֵ�Ƿ�ʹ�ø�ʽ����
        /// </summary>
        private bool isAfterCompute = false;

        /// <summary>
        /// ��ʽ����ֵ�Ƿ�ʹ�ø�ʽ����
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
        /// �Ƿ�ʹ��ǧ��λ��ʾ
        /// </summary>
        private bool isThousand = false;

        /// <summary>
        /// �Ƿ�ʹ��ǧ��λ��ʾ
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
        /// �Ƿ��ʽ����Ϣ����
        /// </summary>
        [DefaultValue(false)]
        public bool HasValues
        {
            get;
            set;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="config">��ʽ����Ϣ</param>
        /// <param name="docTemplateType">ģ������</param>
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
        /// ���ظ�ʽ�����ֵ
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>��ʽ�����ֵ</returns>
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
        /// �ֵ�ֵת��Ϊ�ı�
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
        private string ValueToText(string value)
        {
            return this.dataSource.GetDictionaryText(this.TextField, this.ValueField, value);
        }

        /// <summary>
        /// ��ʽ������
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
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
        /// ȡС���㼸λ
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
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
        /// �������ĺ�λ��0
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
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
        /// ����ת��Ϊһ����...��
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
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
        /// ����ת��Ϊ�㡢Ҽ���������������顢½���⡢�ơ���
        /// </summary>
        /// <param name="value">ԭʼֵ</param>
        /// <returns>ת����ֵ</returns>
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
        /// ����ת����
        /// </summary>
        private class DateConvert
        {
            /// <summary>
            /// ����
            /// </summary>
            private static DateConvert dateConvert = null;

            /// <summary>
            /// ��������
            /// </summary>
            private char[] strChinese;

            /// <summary>
            /// ��ʼ��
            /// </summary>
            private DateConvert()
            {
                this.strChinese = new char[] 
                {
                    '��', 'һ', '��', '��', '��', '��', '��', '��', '��', '��', 'ʮ'
                };
            }

            /// <summary>
            /// ʵ����
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
            /// ����ת��Ϊ��������
            /// </summary>
            /// <param name="strDate">ԭʼ����</param>
            /// <returns>ת����ֵ</returns>
            public string Baodate2Chinese(string strDate)
            {
                StringBuilder result = new StringBuilder();

                // ����������ʽ�жϲ����Ƿ���ȷ
                Regex theReg = new Regex(@"(\d{2}|\d{4})(/|-)(\d{1,2})(/|-)(\d{1,2})");
                if (theReg.Match(strDate).Length != 0)
                {
                    // ���������ڵ������մ浽�ַ�����str��
                    string[] str = null;
                    if (strDate.Contains("-"))
                    {
                        str = strDate.Split('-');
                    }
                    else if (strDate.Contains("/"))
                    {
                        str = strDate.Split('/');
                    }

                    // str[0]��Ϊ�꣬��������ַ�ת��Ϊ��Ӧ�ĺ���
                    for (int i = 0; i < str[0].Length; i++)
                    {
                        result.Append(this.strChinese[int.Parse(str[0][i].ToString())]);
                    }

                    result.Append("��");

                    // ת����
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

                    result.Append("��");

                    // ת����
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

                    result.Append("��");
                }
                else
                {
                    throw new ArgumentException();
                }

                return result.ToString();
            }
        }
    } // end FormatInfo
} // end namespace �ĵ�ģ������