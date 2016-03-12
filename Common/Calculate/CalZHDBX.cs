using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    public class CalZHDBX : CalBase
    {
        public CalZHDBX(decimal balanceGJJ, decimal balanceSY, int months,
            decimal ratesGJJ, decimal ratesSY)
            : base(balanceGJJ, balanceSY, months, ratesGJJ, ratesSY)
        {
        }

        protected override void Calculate(decimal balanceGJJ, decimal balanceSY, int months
            , decimal ratesGJJ, decimal ratesSY)
        {
            decimal baseAmountGJJ = 0, baseAmountSY = 0;
            decimal monthRate0GJJ = decimal.MinValue, monthRate0SY = decimal.MinValue, monthAmountGJJ = decimal.MinValue, monthAmountSY = decimal.MinValue;
            for (int month = months; month >= 1; month--)
            {
                var monthRateGJJ = GetMonthRate(ratesGJJ);
                var interestGJJ = Round(balanceGJJ * monthRateGJJ);
                if (monthRate0GJJ != monthRateGJJ) monthAmountGJJ = GetMonthAmount(balanceGJJ, monthRate0GJJ = monthRateGJJ, month);
                baseAmountGJJ = monthAmountGJJ - interestGJJ;
                balanceGJJ -= baseAmountGJJ;
                if (month == 1 && balanceGJJ != 0)
                {
                    baseAmountGJJ += balanceGJJ;
                    interestGJJ -= balanceGJJ;
                    balanceGJJ = 0;
                }

                var monthRateSY = GetMonthRate(ratesSY);
                var interestSY = Round(balanceSY * monthRateSY);
                if (monthRate0SY != monthRateSY) monthAmountSY = GetMonthAmount(balanceSY, monthRate0SY = monthRateSY, month);
                baseAmountSY = monthAmountSY - interestSY;
                balanceSY -= baseAmountSY;
                if (month == 1 && balanceSY != 0)
                {
                    baseAmountSY += balanceSY;
                    interestSY -= balanceSY;
                    balanceSY = 0;
                }
                var monthAmount = monthAmountGJJ + monthAmountSY;
                var interest = interestGJJ + interestSY;
                var balance = balanceGJJ + balanceSY;

                CalculateTable calTable = new CalculateTable();
                calTable.ID = months - month + 1;
                calTable.monthAmount = monthAmount.ToString("0.00");
                calTable.baseAmount = (baseAmountGJJ + baseAmountSY).ToString("0.00"); //baseAmount.ToString("0.00");
                calTable.interest = interest.ToString("0.00");
                calTable.balance = balance.ToString("0.00");
                lstCalculateTable.Add(calTable);
            }
        }

        decimal GetMonthAmount(decimal balance, decimal monthRate, int months)
        {
            double tmp = Math.Pow(1 + (double)monthRate, months);
            return Round((decimal)((double)balance * (double)monthRate * tmp / (tmp - 1)));
        }
    }
}
