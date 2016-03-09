using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public interface IWordDocument
    {
        IList<IParagraph> IParagraphs { get; }
        string DocumentText { get; }
    }
}
