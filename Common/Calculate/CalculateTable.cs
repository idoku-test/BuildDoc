using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    /// <summary>
    /// 贷款列表实体
    /// </summary>
    public class CalculateTable
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 偿还本息
        /// </summary>
        public string monthAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 偿还本金
        /// </summary>
        public string baseAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 偿还利息
        /// </summary>
        public string interest
        {
            get;
            set;
        }

        /// <summary>
        /// 剩余本金
        /// </summary>
        public string balance
        {
            get;
            set;
        }
    }
}
