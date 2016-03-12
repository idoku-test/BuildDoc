using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    public class TaxType
    {
        /// <summary>
        /// 个人所得税
        /// </summary>
        public string income
        {
            get;
            set;
        }
        
        /// <summary>
        /// 契税
        /// </summary>
        public string contract
        {
            get;
            set;
        }

        /// <summary>
        /// 营业税
        /// </summary>
        public string business
        {
            get;
            set;
        }

        /// <summary>
        /// 服务税
        /// </summary>
        public string service
        {
            get;
            set;
        }

        /// <summary>
        /// 买方税款总额
        /// </summary>
        public string buy
        {
            set;
            get;
        }

        /// <summary>
        /// 卖方税款总额
        /// </summary>
        public string sell
        {
            get;
            set;
        }
    }
}
