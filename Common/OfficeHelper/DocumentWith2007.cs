using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.XWPF.UserModel;
using System.IO;

namespace Common
{
    public class DocumentWith2007 : XWPFDocument, IWordDocument
    {
        private IList<IParagraph> _paragraphs = new List<IParagraph>();

        public IList<IParagraph> IParagraphs
        {
            get
            {
                this._paragraphs.Clear();
                foreach (var item in base.Paragraphs)
                    this._paragraphs.Add(new ParagraphWith2007(item));
                return this._paragraphs;
            }
        }

        public string DocumentText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                var paragraphs = this.IParagraphs;
                int paragraphCount = paragraphs.Count;
                for (int i = 0; i < paragraphCount; i++)
                {
                    sb.AppendFormat("{0}\r", paragraphs[i].GetText());
                }
                return sb.ToString();
            }
        }

        public DocumentWith2007(Stream stream)
            : base(stream)
        {
        }
    }
}
