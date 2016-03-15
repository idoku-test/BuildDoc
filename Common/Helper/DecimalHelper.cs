
using System.Xml;
using System;

namespace Common
{
    public class DecimalHelper
    {
        /// <summary>
        /// 验证数字的精度是否在运行范围内
        /// </summary>
        /// <param name="dInput">被验证的数字</param>
        /// <param name="nMaxPrecisionLength">允许的最大精度</param>
        /// <param name="nMaxDecimalPlaceLength">允许的小数位数</param>
        /// <returns>true表示通过验证</returns>
        public static bool CheckPrecision(decimal dInput, int nMaxPrecisionLength, int nMaxDecimalPlaceLength)
        {
            if (nMaxPrecisionLength < 1 || nMaxDecimalPlaceLength < 0 || nMaxPrecisionLength < nMaxDecimalPlaceLength)
            {
                return true;
            }
            string strFormat = String.Format("###0.{0}", new string('#', nMaxDecimalPlaceLength + 1));
            string strInput = Math.Abs(dInput).ToString(strFormat);
            string[] ary = strInput.Split('.');
            if (ary[0].Length > nMaxPrecisionLength - nMaxDecimalPlaceLength)//类似Oracle的整数位精度规则
            {
                return false;
            }
            if (ary.Length > 1 && ary[1].Length > nMaxDecimalPlaceLength)
            {
                return false;
            }           
            return true;
        }

        public static bool CheckDecimalPlace(decimal dInput, decimal? nMaxDecimalPlaceLength)
        {
            if (!nMaxDecimalPlaceLength.HasValue || nMaxDecimalPlaceLength.Value<0)
            {
                return true;
            }
            string strFormat = String.Format("###0.{0}", new string('#', (int)Math.Truncate(nMaxDecimalPlaceLength.Value) + 1));
            string strInput = Math.Abs(dInput).ToString(strFormat);
            string[] ary = strInput.Split('.');
            if (ary.Length > 1 && ary[1].Length > nMaxDecimalPlaceLength)
            {
                return false;
            }
            return true;
        }

        public static decimal Truncate(decimal dInput, int nMaxDecimalPlaceLength)
        {
            if (nMaxDecimalPlaceLength < 0 )
            {
                return dInput;
            }
            decimal dTmp = (decimal)Math.Pow(10, nMaxDecimalPlaceLength);
            return Math.Truncate(dInput * dTmp) / dTmp;
        }

    }
}