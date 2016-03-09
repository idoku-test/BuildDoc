namespace BuildDoc
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// ��ǩ������
    /// </summary>
    public abstract class BaseLabel
    {
        /// <summary>
        /// �Ƿ�ɹ�
        /// </summary>
        private bool isPass = false;

        /// <summary>
        /// ��ǩ����
        /// </summary>
        public string LabelName
        {
            get;
            set;
        }

        /// <summary>
        /// ��ǩ������
        /// </summary>
        public string LabelTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// ����ֵ
        /// </summary>
        public abstract string RelateValue
        { 
            get;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public BaseLabel()
        {
            // �����෽��
        }

        /// <summary>
        /// ��Դ�ͷ�
        /// </summary>
        public virtual void Dispose()
        {
            // �����෽��
        }

        /// <summary>
        /// ִ��
        /// </summary>
        /// <param name="buildWord">�ĵ�������</param>
        public abstract void Execute(IBuildWord buildWord);

        /// <summary>
        /// �滻
        /// </summary>
        /// <param name="labelName">��ǩ����</param>
        /// <param name="value">ֵ</param>
        /// <returns>�����滻���</returns>
        public abstract bool Replace(string labelName, string value);
    } // end BaseLabel
} // end namespace