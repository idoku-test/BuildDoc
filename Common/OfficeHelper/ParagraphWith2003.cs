using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HWPF.UserModel;

namespace Common
{
    public class ParagraphWith2003 : IParagraph
    {
        CharacterRun _characterRun;

        public ParagraphWith2003(CharacterRun characterRun)
        {
            this._characterRun = characterRun;
        }

        public string GetText()
        {
            return this._characterRun.Text;
        }
    }
}
