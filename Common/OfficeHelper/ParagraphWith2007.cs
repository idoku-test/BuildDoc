using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HWPF.UserModel;
using NPOI.XWPF.UserModel;

namespace Common
{
    public class ParagraphWith2007 : IParagraph
    {
        XWPFParagraph _paragraph;

        public XWPFParagraph BaseParagraph
        {
            get { return this._paragraph; }
        }

        public ParagraphWith2007(XWPFParagraph paragraph)
        {
            this._paragraph = paragraph;
        }

        public string GetText()
        {
            return this._paragraph.GetText();
        }
    }
}
