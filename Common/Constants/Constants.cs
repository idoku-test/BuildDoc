using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Common.Constants
{
    /// <summary>
    /// 编码规则Key
    /// </summary>
    public enum EnumCodeKey
    {
        [Description("楼盘")]
        Construction = 0,
        [Description("楼栋")]
        Building = 1,
        [Description("户")]
        House = 2,
        [Description("单元")]
        Unit = 3,
        [Description("楼层")]
        Floor = 4,
        [Description("纵列")]
        Column = 5
    }
}
