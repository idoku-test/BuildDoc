using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace Common
{
    public interface IExcelHelper
    {
        DataSet ExcelToDataSet();
    }
}
