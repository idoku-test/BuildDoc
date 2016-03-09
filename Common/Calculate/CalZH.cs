using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    /// <summary>
    /// 综合贷款
    /// </summary>
    public class CalZH : CalBase
    {
        public CalZH(decimal balanceGJJ, decimal balanceSY, int months, DateTime date, 
            KeyValuePair<DateTime, PointF>[] ratesGJJ, KeyValuePair<DateTime, PointF>[] ratesSY)
            : base(balanceGJJ, balanceSY, months, date, ratesGJJ, ratesSY)
        {
        }
        protected override void Calculate(decimal balanceGJJ, decimal balanceSY, int months, DateTime date
            , KeyValuePair<DateTime, PointF>[] ratesGJJ, KeyValuePair<DateTime, PointF>[] ratesSY)
        {
            decimal baseAmount = 0;
            decimal monthRate0 = decimal.MinValue, monthAmountGJJ = decimal.MinValue, monthAmountSY = decimal.MinValue;
            for (int day = date.Day, month = months; month >= 1; month--, date = NextMonth(date, day))
            {
                var monthRateGJJ = GetMonthRate(date, months, ratesGJJ);
                var interestGJJ = Round(balanceGJJ * monthRateGJJ);
                if (monthRate0 != monthRateGJJ) monthAmountGJJ = GetMonthAmount(balanceGJJ, monthRate0 = monthRateGJJ, month);
                baseAmount = monthAmountGJJ - interestGJJ;
                balanceGJJ -= baseAmount;
                if (month == 1 && balanceGJJ != 0)
                {
                    baseAmount += balanceGJJ;
                    interestGJJ -= balanceGJJ;
                    balanceGJJ = 0;
                }

                var monthRateSY = GetMonthRate(date, months, ratesSY);
                var interestSY = Round(balanceSY * monthRateSY);
                if (monthRate0 != monthRateSY) monthAmountSY = GetMonthAmount(balanceSY, monthRate0 = monthRateSY, month);
                baseAmount = monthAmountSY - interestSY;
                balanceSY -= baseAmount;
                if (month == 1 && balanceSY != 0)
                {
                    baseAmount += balanceSY;
                    interestSY -= balanceSY;
                    balanceSY = 0;
                }
                var monthAmount = monthAmountGJJ + monthAmountSY;
                var interest = interestGJJ + interestSY;
                var balance = balanceGJJ + balanceSY;
                Table.Rows.Add(new object[] { months - month + 1, monthAmount, baseAmount, interest, balance });
            }
        }

        decimal GetMonthAmount(decimal balance, decimal monthRate, int months)
        {
            double tmp = Math.Pow(1 + (double)monthRate, months);
            return Round((decimal)((double)balance * (double)monthRate * tmp / (tmp - 1)));
        }
    }
}
