using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebApplication1
{
    public static class Helper
    {
        public static string EmptyValue
        {
            get
            {
                return "undefined";
            }
        }

        public static string ExcelLimitMessage =
            "The number of {0} in your report ({0}:{1}) exceeds maximum number of {0} allowed in MS Excel Sheet ({0}:{2}). Please create report by region or reduce time period.";

        public static bool IsHasTags(this string value)
        {
            return Regex.IsMatch(value, @"<(.|\n)*>");
        }

       
        public static bool ToBoolOrDefault(this object value, bool defaultValue)
        {
            try
            {
                return value == null ? defaultValue : bool.Parse(value.ToString());
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static int ToIntOrDefault(this object value, int defaultValue)
        {
            try
            {
                return value == null ? defaultValue : int.Parse(value.ToString());
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static Guid ToGuid(this object value)
        {
            try
            {
                return new Guid(value.ToString());
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public static Guid? ToGuidNullable(this object value)
        {
            try
            {
                return new Guid(value.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string EscapeBraces(this string source)
        {
            var result = source.Replace("<", "&lt;");
            result = result.Replace(">", "&gt;");
            return result;
        }

        public static bool IsInteger(this string source)
        {
            try
            {
                int.Parse(source);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int ToInteger(this string source)
        {
            return int.Parse(source);
        }

        public static int? ToIntNullable(this string source)
        {
            try
            {
                return int.Parse(source);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int ToIntegerSafe(this string source)
        {
            try
            {
                return int.Parse(source);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal? StringToDecimal(string fieldName, string value)
        {
            if (value.Trim() == string.Empty)
            {
                return null;
            }
            try
            {
                return decimal.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception(string.Format("{0} has invalid floating point format.", fieldName));
            }
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || string.IsNullOrEmpty(value.Trim());
        }

        public static string NewPassword()
        {
            var rnd = new Random();
            var result = "";
            for(var i = 0; i < 6; i++)
            {
                result += rnd.Next(0, 10);
            }
            return result;
        }

        public static DateTime? ParseAsDateTime(this string date)
        {
            var dateTime = DateTime.UtcNow;
            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return null;
            }
            return dateTime;
        }

        public static string ToDateTimeString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", "/");
        }

        public static int? ToIntOrNullable(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
