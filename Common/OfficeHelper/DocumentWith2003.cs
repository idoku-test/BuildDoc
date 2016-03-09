using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HWPF.UserModel;
using NPOI.HWPF;
using System.IO;
using NPOI.HWPF.Model;
using NPOI.HWPF.Extractor;
using System.Text.RegularExpressions;

namespace Common
{
    public class DocumentWith2003 : HWPFDocument, IWordDocument
    {
        private IList<IParagraph> _paragraphs = new List<IParagraph>();

        public IList<IParagraph> IParagraphs
        {
            get
            {
                this._paragraphs.Clear();
                Range rang= base.GetRange();
                int paragraphCount = rang.NumParagraphs;
                for (int i = 0; i < paragraphCount; i++)
                {
                    CharacterRun pph = rang.GetCharacterRun(i);
                    this._paragraphs.Add(new ParagraphWith2003(pph));
                } 
                return this._paragraphs;
            }
        }

        public string DocumentText
        {
            get
            {
                //WordExtractor extractor = new WordExtractor(this);
                //string[] strArray = extractor.ParagraphText;
                
                Range rang = base.GetRange();
                string documentText = rang.Text.Replace("\a", "").Replace("\f", "");
                documentText = Regex.Replace(documentText, @"(?<=\x13).*?(?=\x14)", "");
                documentText = Regex.Replace(documentText, @"[\x13|\x14|\x15|\x0C|\x08]", "");
                return documentText;
            }
        }

        public DocumentWith2003(Stream stream)
            : base(stream)
        {
        }
    }
}
