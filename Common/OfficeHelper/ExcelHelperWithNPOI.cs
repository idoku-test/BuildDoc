using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using System.Xml;
using System.Collections;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace Common
{
    public class ExcelHelperWithNPOI : IExcelHelper
    {
        private IWorkbook _workbook;
        public bool AutoColumnHeder;

        public ExcelHelperWithNPOI(string _excelFilePath)
        {
            this.AutoColumnHeder = false;
            this.InitWorkbook(_excelFilePath);
        }

        public ExcelHelperWithNPOI(string _excelFilePath, bool _autoColumnHeder)
        {
            this.AutoColumnHeder = _autoColumnHeder;
            this.InitWorkbook(_excelFilePath);
        }

        public ExcelHelperWithNPOI(Stream fileStream)
        {
            this.AutoColumnHeder = false;
            this._workbook = WorkbookFactory.Create(fileStream);
        }

        public ExcelHelperWithNPOI(Stream fileStream, bool _autoColumnHeder)
        {
            this.AutoColumnHeder = _autoColumnHeder;
            this._workbook = WorkbookFactory.Create(fileStream);
        }

        public DataSet ExcelToDataSet()
        {
            DataSet set = new DataSet();
            List<string> excelTablesName = this.GetExcelTablesName();
            foreach (string str in excelTablesName)
            {
                DataTable dt = this.ExcelToDataTable(str);
                dt.TableName = str;
                set.Tables.Add(dt);
            }
            return set;
        }

        public DataTable ExcelToDataTable(int index)
        {
            ISheet sheetAt = this._workbook.GetSheetAt(index);
            return this.ExcelToDataTable(sheetAt, 0);
        }

        public DataTable ExcelToDataTable(string tName)
        {
            ISheet sheet = this._workbook.GetSheet(tName);
            return this.ExcelToDataTable(sheet, 0);
        }

        private DataTable ExcelToDataTable(ISheet sheet, int headerIndex)
        {
            if (sheet.LastRowNum < headerIndex)
            {
                throw new Exception("Excel模板格式不对，读取下标值错误");
            }

            DataTable table = new DataTable();
            IRow row = sheet.GetRow(headerIndex);
            if (row == null)
            {
                return table;
            }
            int lastCellNum = row.LastCellNum;

            for (int i = 0; i < lastCellNum; i++)
            {
                string columnName = string.Empty;
                if (this.AutoColumnHeder)
                {
                    columnName = Convert.ToChar(0x41) + i.ToString();
                }
                else
                {
                    var cell = row.GetCell(i);
                    if (cell == null || String.IsNullOrEmpty(cell.ToString()))
                    {
                        lastCellNum = i;
                        break;
                    }
                    columnName = cell.ToString();
                }
                table.Columns.Add(columnName);
            }

            IEnumerator rowEnumerator = sheet.GetRowEnumerator();
            if (!this.AutoColumnHeder)
            {
                rowEnumerator.MoveNext();
            }

            while (rowEnumerator.MoveNext())
            {
                IRow current = (IRow)rowEnumerator.Current;
                DataRow row3 = table.NewRow();
                for (int j = 0; j < lastCellNum; j++)
                {
                    ICell cell = current.GetCell(j);
                    if (cell == null || cell.ToString().Trim() == "")
                    {
                        row3[j] = System.DBNull.Value;
                    }
                    else
                    {
                        if (cell.ToString().Contains("月") || cell.ToString().Contains("10000"))
                        {
                            string y = "";
                        }
                        //row3[j] = cell.ToString();

                        switch (cell.CellType)
                        {
                            case CellType.BLANK:
                                row3[j] = null;
                                break;
                            case CellType.BOOLEAN:
                                row3[j] = cell.BooleanCellValue;
                                break;
                            case CellType.ERROR:
                                row3[j] = cell.ErrorCellValue;
                                break;
                            case CellType.STRING:
                                row3[j] = cell.StringCellValue;
                                break;

                            case CellType.NUMERIC:
                                bool isDate = NPOI.HSSF.UserModel.HSSFDateUtil.IsCellDateFormatted(cell);
                                if (isDate)
                                {
                                    row3[j] = cell.DateCellValue;
                                }
                                else
                                {
                                    row3[j] = cell.NumericCellValue;
                                }

                                break;
                            case CellType.FORMULA:
                            default:
                                row3[j] = "=" + cell.CellFormula;
                                //row3[j] = cell.StringCellValue;
                                break;
                        }



                        //switch (cell.CellType)
                        //{
                        //    case CellType.NUMERIC:
                        //        row3[j] = cell.DateCellValue;
                        //        break;
                        //    default:

                        //        cell.SetCellType(CellType.STRING);
                        //        //row3[j].setCellType(Cell.CELL_TYPE_STRING);


                        //        row3[j] = cell.StringCellValue;
                        //        break;
                        //}
                    }
                }
                table.Rows.Add(row3);
            }
            return table;
        }

        public DataTable ExcelToDataTable(string tName, int headerIndex)
        {
            ISheet sheet = this._workbook.GetSheet(tName);
            return this.ExcelToDataTable(sheet, headerIndex);
        }


        private static ICellStyle GetDateFormat(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-mm-dd");
            return style;
        }

        public List<string> GetExcelTablesName()
        {
            List<string> list = new List<string>();
            foreach (ISheet sheet in this._workbook)
            {
                list.Add(sheet.SheetName);
            }
            return list;
        }

        private void InitWorkbook(string _excelFilePath)
        {
            if (string.IsNullOrEmpty(_excelFilePath))
            {
                throw new Exception("Excel文件路径不能为空！");
            }
            if (!File.Exists(_excelFilePath))
            {
                throw new Exception("指定路径的Excel文件不存在！");
            }
            string str = Path.GetExtension(_excelFilePath).ToLower();
            using (Stream fileStream = new FileStream(_excelFilePath, FileMode.Open, FileAccess.Read))
            {
                this._workbook = WorkbookFactory.Create(fileStream);
            }
        }

        public bool IsExistExcelTableName(string tName)
        {
            return this.GetExcelTablesName().Contains(tName);
        }

        private static void SetCellValue(string drValue, ICellStyle dateStyle, IFont font, HorizontalAlignment ha, Type columnType, ICell newCell)
        {
            switch (columnType.ToString())
            {
                case "System.String":
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime":
                    DateTime time;
                    DateTime.TryParse(drValue, out time);
                    newCell.SetCellValue(time);
                    newCell.CellStyle = dateStyle;
                    break;
                case "System.Boolean":
                    {
                        bool result = false;
                        bool.TryParse(drValue, out result);
                        newCell.SetCellValue(result);
                        break;
                    }
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    {
                        int num = 0;
                        int.TryParse(drValue, out num);
                        newCell.SetCellValue((double)num);
                        break;
                    }
                case "System.Decimal":
                case "System.Double":
                    {
                        double num2 = 0.0;
                        double.TryParse(drValue, out num2);
                        newCell.SetCellValue(num2);
                        break;
                    }
                case "System.DBNull":
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
            newCell.CellStyle.Alignment = ha;//水平对齐
            newCell.CellStyle.SetFont(font);//设置字体
        }

        public static void SaveFile(DataSet ds, string directory)
        {
            foreach (DataTable tab in ds.Tables)
            {
                string excelFilePath = Path.Combine(directory, string.Format("{0}{1}", tab.TableName + DateTime.Now.ToString("yyyy-MM-dd HHmmss"), ".xlsx"));
                using (Stream fileStream = new FileStream(excelFilePath, FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet(tab.TableName);
                    IRow row = sheet.CreateRow(0);
                    int colPosition = 0;
                    ICellStyle headCellStyle = workbook.CreateCellStyle();
                    headCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ORANGE.index;
                    headCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    headCellStyle.Alignment = HorizontalAlignment.CENTER;
                    headCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headCellStyle.BorderTop = headCellStyle.BorderBottom = headCellStyle.BorderLeft = headCellStyle.BorderRight = BorderStyle.THIN;
                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.BorderTop = dataCellStyle.BorderBottom = dataCellStyle.BorderLeft = dataCellStyle.BorderRight = BorderStyle.THIN;
                    //自动换行
                    //dataCellStyle.WrapText = true;
                    foreach (DataColumn col in tab.Columns)
                    {
                        ICell cell = row.CreateCell(colPosition);
                        cell.CellStyle = headCellStyle;
                        cell.SetCellValue(col.ColumnName);
                        colPosition++;
                    }


                    for (int rowPosition = 0; rowPosition < tab.Rows.Count; rowPosition++)
                    {
                        DataRow dr = tab.Rows[rowPosition];
                        row = sheet.CreateRow(rowPosition + 1);
                        colPosition = 0;
                        foreach (DataColumn col in tab.Columns)
                        {
                            ICell cell = row.CreateCell(colPosition);
                            cell.CellStyle = dataCellStyle;
                            string strValue = dr[col.ColumnName].ToString();
                            strValue = Regex.Replace(strValue, @"[\x00|\x01]", "");
                            cell.SetCellValue(strValue);
                            colPosition++;
                        }
                    }
                    workbook.Write(fileStream);
                }
            }
        }

        /// <summary>
        /// 把ds中的多个表保存成一个Excel文件
        /// add by luyj 2015-5-15 
        /// </summary>
        /// <param name="excelName">excel的文件名称，不含后缀</param>
        /// <param name="ds"></param>
        /// <param name="directory"></param>
        public static void SaveOneFile(string excelName, DataSet ds, string directory)
        {
            string excelFilePath = Path.Combine(directory, string.Format("{0}{1}", excelName + DateTime.Now.ToString("yyyy-MM-dd HHmmss"), ".xlsx"));
            using (Stream fileStream = new FileStream(excelFilePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                foreach (DataTable tab in ds.Tables)
                {
                    ISheet sheet = workbook.CreateSheet(tab.TableName);
                    IRow row = sheet.CreateRow(0);
                    int colPosition = 0;
                    ICellStyle headCellStyle = workbook.CreateCellStyle();
                    headCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ORANGE.index;
                    headCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    headCellStyle.Alignment = HorizontalAlignment.CENTER;
                    headCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    headCellStyle.BorderTop =
                        headCellStyle.BorderBottom =
                            headCellStyle.BorderLeft = headCellStyle.BorderRight = BorderStyle.THIN;
                    ICellStyle dataCellStyle = workbook.CreateCellStyle();
                    dataCellStyle.BorderTop =
                        dataCellStyle.BorderBottom =
                            dataCellStyle.BorderLeft = dataCellStyle.BorderRight = BorderStyle.THIN;
                    //自动换行
                    //dataCellStyle.WrapText = true;
                    foreach (DataColumn col in tab.Columns)
                    {
                        ICell cell = row.CreateCell(colPosition);
                        cell.CellStyle = headCellStyle;
                        cell.SetCellValue(col.ColumnName);
                        colPosition++;
                    }
                    for (int rowPosition = 0; rowPosition < tab.Rows.Count; rowPosition++)
                    {
                        DataRow dr = tab.Rows[rowPosition];
                        row = sheet.CreateRow(rowPosition + 1);
                        colPosition = 0;
                        foreach (DataColumn col in tab.Columns)
                        {
                            ICell cell = row.CreateCell(colPosition);
                            cell.CellStyle = dataCellStyle;
                            string strValue = dr[col.ColumnName].ToString();
                            strValue = Regex.Replace(strValue, @"[\x00|\x01]", "");
                            cell.SetCellValue(strValue);
                            colPosition++;
                        }
                    }
                }
                workbook.Write(fileStream);
            }
        }
    }
}
