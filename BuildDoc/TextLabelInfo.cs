using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BuildDoc
{
    public class TextLabelInfo
    {
        public decimal ID;
        public StructureType StructureType;
        public string LabelName;
        public string FieldName;
        public string DefaultValue;
        public List<RelateItem> Relate;
        public FormatItem Format;
        public BaseControl Control;
        public string GetDataMethod_Name;
        public string DataSourceName;
        public string ConifgValue;
        public string FilterFieldName;
        public string FilterOperation;
        public string FilterValue;
        public string StructureName;
        public GetDataMethod GetDataMethod;
        public string OrginalValue;

        public TextLabelInfo(decimal id, StructureType structureType, string structureName, string labelName, string fieldName, string defaultValue, List<RelateItem> relate, FormatInfo format, BaseControl contorl, GetDataMethod getDataMethod
            , string dataSourceName, string configValue, string filterFieldName, string filterOperation, string filterValue)
        {
            this.ID = id;
            this.StructureType = structureType;
            this.LabelName = labelName;
            this.FieldName = fieldName;
            this.DefaultValue = defaultValue;
            this.Relate = relate;
            if (format != null)//format.ValueField, format.TextField,
                this.Format = new FormatItem( format.DecimalCount, format.Dividend, format.DateFormatString, format.IsUpperDate);
            this.Control = contorl;
            GetDataMethod_Name = EnumHelper.GetEnumDescription(getDataMethod);
            this.DataSourceName = dataSourceName;
            this.ConifgValue = configValue;
            this.FilterFieldName = filterFieldName;
            this.FilterOperation = filterOperation;
            this.FilterValue = filterValue;
            this.GetDataMethod = getDataMethod;
            this.StructureName = structureName;
        }
    }

    public class RelateItem
    {
        public string LabelName;
        public string FieldName;
    }

    public class FormatItem
    {
        public string ValueField;
        public string TextField;
        public int DecimalCount;
        public decimal Dividend;
        public string DateFormatString;
        public bool IsUpperDate;
        public FormatItem(int DecimalCount, decimal Dividend, string DateFormatString, bool IsUpperDate)
        {
            //this.ValueField = ValueField;
            //this.TextField = TextField;
            this.DecimalCount = DecimalCount;
            this.Dividend = Dividend;
            this.DateFormatString = DateFormatString;
            this.IsUpperDate = IsUpperDate;

        }
    }

    //public class ContorlItem
    //{
    //    public string LabelName;
    //    public bool Required;
    //    public string ControlTypeName;
    //    public ContorlItem(string labelName, bool required, ControlType controlType)
    //    {
    //        this.LabelName = labelName;
    //        this.Required = required;
    //        this.ControlTypeName = controlType.ToString();
    //    }
    //}
}
