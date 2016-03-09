using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common.Calculate
{
    /// <summary>
    /// 等本息法
    /// </summary>
    public class CalDBX : CalBase
    {
        public CalDBX(decimal balance, int months, decimal rates)
            : base(balance, months, rates)
        {
        }

        protected override void Calculate(decimal balance, int months, decimal rates)
        {
            decimal baseAmount = 0;
            decimal monthRate0 = decimal.MinValue, monthAmount = decimal.MinValue;
            for (int month = months; month >= 1; month--)
            {
                var monthRate = GetMonthRate(rates);
                var interest = Round(balance * monthRate);
                if (monthRate0 != monthRate) monthAmount = GetMonthAmount(balance, monthRate0 = monthRate, month);
                baseAmount = monthAmount - interest;
                balance -= baseAmount;
                if (month == 1 && balance != 0)
                {
                    baseAmount += balance;
                    interest -= balance;
                    balance = 0;
                }
                //Table.Rows.Add(new object[] { months - month + 1, monthAmount, baseAmount, interest, balance });
                CalculateTable calTable = new CalculateTable();
                calTable.ID = months - month + 1;
                calTable.monthAmount = monthAmount.ToString("0.00");
                calTable.baseAmount = baseAmount.ToString("0.00");
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
