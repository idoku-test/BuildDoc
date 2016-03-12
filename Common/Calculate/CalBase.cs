using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    public abstract class CalBase
    {
        public List<CalculateTable> lstCalculateTable = new List<CalculateTable>();

        public CalBase(decimal balance, int months, decimal rates)
        {
             Calculate(balance, months, rates);
        }

        public CalBase(decimal balanceSY, decimal balanceGJJ, int months, decimal ratesGJJ,
            decimal ratesSY)
        {
             Calculate(balanceSY, balanceGJJ, months, ratesGJJ , ratesSY);
        }

        protected virtual void Calculate(decimal balance, int months, decimal rates)
        {
        }

        protected virtual void Calculate(decimal balanceGJJ, decimal balanceSY, int months,
            decimal ratesGJJ, decimal ratesSY)
        {
        }

        protected decimal GetMonthRate(decimal rates)
        {
            return (decimal)(rates / 12 /100);
        }

        protected decimal Round(decimal dec)
        {
            return decimal.Round(dec, 2);
        }
    }
}
