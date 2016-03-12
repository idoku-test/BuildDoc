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
    public class CalDBJ : CalBase
    {
        public CalDBJ(decimal balance, int months, decimal rates)
            : base(balance, months, rates)
        {
        }

        protected override void Calculate(decimal balance, int months, decimal rates)
        {
            decimal baseAmount = Round(balance / months), totalAmount = 0, totalInterest = 0;
            for (int month = months; month >= 1; month--)
            {
                var monthRate = GetMonthRate(rates);
                var interest = Round(balance * monthRate);
                var monthAmount = baseAmount + interest;
                balance -= baseAmount;
                if (month == 1 && balance != 0)
                {
                    baseAmount += balance;
                    monthAmount += balance;
                    balance = 0;
                }
                totalAmount += monthAmount;
                totalInterest += interest;
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
