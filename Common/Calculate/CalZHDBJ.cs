using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    public class CalZHDBJ:CalBase
    {
        public CalZHDBJ(decimal balanceGJJ, decimal balanceSY, int months,
            decimal ratesGJJ, decimal ratesSY)
            : base(balanceGJJ, balanceSY, months, ratesGJJ, ratesSY)
        {
        }

        protected override void Calculate(decimal balanceGJJ, decimal balanceSY, int months,
            decimal ratesGJJ, decimal ratesSY)
        {
            decimal baseAmountSY = Round(balanceSY / months), totalAmountSY = 0, totalInterestSY = 0;
            decimal baseAmountGJJ = Round(balanceGJJ / months), totalAmountGJJ = 0, totalInterestGJJ = 0;
            decimal totalAmount = 0, totalInterest = 0;
            for (int month = months; month >= 1; month--)
            {
                var monthRateSY = GetMonthRate(ratesSY);
                var interestSY = Round(balanceSY * monthRateSY);
                var monthAmountSY = baseAmountSY + interestSY;
                balanceSY -= baseAmountSY;
                var monthRateGJJ = GetMonthRate(ratesGJJ);
                var interestGJJ = Round(balanceGJJ * monthRateGJJ);
                var monthAmountGJJ = baseAmountGJJ + interestGJJ;
                balanceGJJ -= baseAmountGJJ;

                var monthAmount = monthAmountSY + monthAmountGJJ;
                var baseAmount = baseAmountSY + baseAmountGJJ;
                var interest = interestSY + interestGJJ;
                var balance = balanceSY + balanceGJJ;

                if (month == 1 && balanceSY != 0)
                {
                    baseAmountSY += balanceSY;
                    monthAmountSY += balanceSY;
                    balanceSY = 0;
                    baseAmountGJJ += balanceGJJ;
                    monthAmountGJJ += balanceGJJ;
                    balanceSY = 0;
                }
                totalAmountSY += monthAmountSY;
                totalInterestSY += interestSY;
                totalAmountGJJ += monthAmountGJJ;
                totalInterestGJJ += interestGJJ;
                totalAmount = totalAmountSY + totalAmountGJJ;
                totalInterest = totalInterestSY + totalInterestGJJ;


                CalculateTable calTable = new CalculateTable();
                calTable.ID = months - month + 1;
                calTable.monthAmount = monthAmount.ToString("0.00");
                calTable.baseAmount = baseAmount.ToString("0.00");
                calTable.interest = interest.ToString("0.00");
                calTable.balance = balance.ToString("0.00");
                lstCalculateTable.Add(calTable);
            }
        }
    }
}
