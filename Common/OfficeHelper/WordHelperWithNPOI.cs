using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.XWPF.UserModel;

namespace Common
{
    public class WordHelperWithNPOI : IWordHelper
    {
        IWordDocument _wordDocument;

        public WordHelperWithNPOI(string _docFilePath)
        {
            this.InitDocument(_docFilePath);
        }

        public WordHelperWithNPOI(string _docFilePath, bool _autoColumnHeder)
        {
            this.InitDocument(_docFilePath);
        }

        public string GetText()
        {
            return this._wordDocument.DocumentText;
        }

        private void InitDocument(string _docFilePath)
        {
            if (string.IsNullOrEmpty(_docFilePath))
            {
                throw new Exception("Word文件路径不能为空！");
            }
            if (!File.Exists(_docFilePath))
            {
                throw new Exception("指定路径的Word文件不存在！");
            }
            using (Stream fileStream = new FileStream(_docFilePath, FileMode.Open, FileAccess.Read))
            {
                this._wordDocument = WordDocuemntFactory.Create(fileStream);
            }
        }
    }
}
