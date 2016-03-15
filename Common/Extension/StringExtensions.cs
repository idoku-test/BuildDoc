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
    /// <summary>
    /// Encapsulates a series of time saving extension methods to the <see cref="T:System.String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        #region Cryptography
        /// <summary>
        /// Creates an MD5 fingerprint of the String.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>An MD5 fingerprint of the String.</returns>
        public static string ToMD5Fingerprint(this string expression)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(expression.ToCharArray());

            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(bytes);

                // Concatenate the hash bytes into one long String.
                return hash.Aggregate(
                    new StringBuilder(32),
                    (sb, b) => sb.Append(b.ToString("X2", CultureInfo.InvariantCulture)))
                    .ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates an SHA1 fingerprint of the String.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>An SHA1 fingerprint of the String.</returns>
        public static string ToSHA1Fingerprint(this string expression)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(expression.ToCharArray());

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] hash = sha1.ComputeHash(bytes);

                // Concatenate the hash bytes into one long String.
                return hash.Aggregate(
                    new StringBuilder(40),
                    (sb, b) => sb.Append(b.ToString("X2", CultureInfo.InvariantCulture)))
                    .ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates an SHA256 fingerprint of the String.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>An SHA256 fingerprint of the String.</returns>
        public static string ToSHA256Fingerprint(this string expression)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(expression.ToCharArray());

            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                byte[] hash = sha256.ComputeHash(bytes);

                // Concatenate the hash bytes into one long String.
                return hash.Aggregate(
                    new StringBuilder(64),
                    (sb, b) => sb.Append(b.ToString("X2", CultureInfo.InvariantCulture)))
                    .ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Creates an SHA512 fingerprint of the String.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>An SHA256 fingerprint of the String.</returns>
        public static string ToSHA512Fingerprint(this string expression)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(expression.ToCharArray());

            using (SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider())
            {
                byte[] hash = sha512.ComputeHash(bytes);

                // Concatenate the hash bytes into one long String.
                return hash.Aggregate(
                    new StringBuilder(70),
                    (sb, b) => sb.Append(b.ToString("X2", CultureInfo.InvariantCulture)))
                    .ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string DESEncode(this string encryptString, string encryptKey)
        {
            if (encryptKey == null)
            {
                encryptKey = "";
            }
            if (encryptKey.Length >= 8)
            {
                encryptKey = encryptKey.Substring(0, 8);
            }
            else
            {
                encryptKey = encryptKey.PadRight(8, ' ');
            }

            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            using (DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider())
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string DESDecode(this string decryptString, string decryptKey)
        {

            if (decryptKey == null)
            {
                decryptKey = "";
            }
            if (decryptKey.Length >= 8)
            {
                decryptKey = decryptKey.Substring(0, 8);
            }
            else
            {
                decryptKey = decryptKey.PadRight(8, ' ');
            }
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            using (DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider())
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    try
                    {
                        cStream.FlushFinalBlock();
                        cStream.Dispose();
                    }
                    catch
                    {
                        return "";
                    }
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
            }
        }
        #endregion

        #region Numbers
        /// <summary>
        /// Creates an array of integers scraped from the String.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>An array of integers scraped from the String.</returns>
        public static int[] ToPositiveIntegerArray(this string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException("expression");
            }

            Regex regex = new Regex(@"\d+", RegexOptions.Compiled);

            MatchCollection matchCollection = regex.Matches(expression);

            // Get the collections.
            int count = matchCollection.Count;
            int[] matches = new int[count];

            // Loop and parse the int values.
            for (int i = 0; i < count; i++)
            {
                matches[i] = int.Parse(matchCollection[i].Value);
            }

            return matches;
        }
        #endregion

        #region Files and Paths
        /// <summary>
        /// Checks the string to see whether the value is a valid virtual path name.
        /// </summary>
        /// <param name="expression">The <see cref="T:System.String">String</see> instance that this method extends.</param>
        /// <returns>True if the given string is a valid virtual path name</returns>
        public static bool IsValidVirtualPathName(this string expression)
        {
            Uri uri;

            return Uri.TryCreate(expression, UriKind.Relative, out uri) && uri.IsWellFormedOriginalString();
        }
        #endregion

        #region Valid
        /// <summary>
        /// Determines whether [is valid number] [the specified expression].
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static bool IsNumber(this string expression)
        {
            Regex reg = new Regex(@"^\d+$");
            if (reg.IsMatch(expression))
                return true;
            return false;
        }
        #endregion

        #region Match（比较、相似度）

        /// <summary>
        /// Levenshtein 距离，又称编辑距离，指的是两个字符串之间，由一个转换成另一个所需的最少编辑操作次数。
        /// 许可的编辑操作包括将一个字符替换成另一个字符，插入一个字符，删除一个字符。
        /// 编辑距离的算法是首先由俄国科学家Levenshtein提出的，故又叫Levenshtein Distance。
        /// </summary>   
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="similarity">返回相似度</param>
        /// <param name="isCaseSensitive">指定是否区分大小写</param>
        /// <returns>返回编辑距离</returns>
        public static Int32 LevenshteinDistance(this string source, String target,
            out Double similarity, Boolean isCaseSensitive = false)
        {
            //比较的两个字符串有一方为空时
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                else
                {
                    similarity = 0;
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }


            String strFrom, strTo;
            if (isCaseSensitive)
            {   // 大小写敏感  
                strFrom = source;
                strTo = target;
            }
            else
            {   // 大小写无关  
                strFrom = source.ToUpper();
                strTo = target.ToUpper();
            }

            // 初始化  
            Int32 nFrom = strFrom.Length;
            Int32 nTo = strTo.Length;
            Int32[,] matrix = new Int32[nFrom + 1, nTo + 1];
            for (Int32 i = 0; i <= nFrom; i++) matrix[i, 0] = i;  // 注意：初始化[0,0]  
            for (Int32 j = 1; j <= nTo; j++) matrix[0, j] = j;

            // 迭代  
            for (Int32 i = 1; i <= nFrom; i++)
            {
                for (Int32 j = 1; j <= nTo; j++)
                {
                    int n1 = (strFrom[i - 1] == strTo[j - 1]) ? matrix[i - 1, j - 1] : matrix[i - 1, j - 1] + 1;
                    int n2 = matrix[i - 1, j] + 1;
                    int n3 = matrix[i, j - 1] + 1;
                    matrix[i, j] = Math.Min(n1, Math.Min(n2, n3));
                }
            }

            // 计算相似度  
            Int32 MaxLength = Math.Max(nFrom, nTo);   // 两字符串的最大长度  
            similarity = ((Double)(MaxLength - matrix[nFrom, nTo])) / MaxLength;

            return matrix[nFrom, nTo];    // 编辑距离  
        }
        #endregion

        #region cut and paste
        public static string Truncate(this string value, int len)
        {
            if (string.IsNullOrEmpty(value) || value.Length < len)
            {
                return value;
            }
            return value.Substring(0, len) + "...";
        }

        public static string ExtCityLetter(this string value,string letter)
        {
            if (string.IsNullOrEmpty(value))
            {
                return letter;
            }
            return value.Substring(0,1).ToUpper();
        }
        #endregion

        /// <summary> 
        /// 由Microsoft.Eval对象计算表达式，需要引用Microsoft.JScript和Microsoft.Vsa名字空间。 
        /// </summary> 
        /// <param name="expression">表达式</param> 
        /// <returns></returns> 
        public static string Eval(this string expression)
        {
            try
            {
                Microsoft.JScript.Vsa.VsaEngine ve = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                object returnValue = Microsoft.JScript.Eval.JScriptEvaluate((object)expression, ve);
                return returnValue.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
