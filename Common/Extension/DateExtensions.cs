using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;

namespace Common.Extensions
{
    public static class DateExtensions
    {
        public static string ToDateTime(this DateTime dateTime)
        {
            return (dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
