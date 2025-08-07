using System;
using System.Globalization;

namespace WebApplication1
{
    public class Validator
    {
        public class ValidationException : Exception
        {
            public string ValidationMessage { get; set; }

            public ValidationException(string message)
            {
                ValidationMessage = message;
            }
        }
        public static int ValidateTime(string fieldName, string value)
        {
            try
            {
                if (value.Length != 5 || value[2] != ':' ||
                    !Char.IsNumber(value[0]) || !Char.IsNumber(value[0]) ||
                    !Char.IsNumber(value[3]) || !Char.IsNumber(value[4]))
                {
                    throw new Exception();
                }
                var hours = int.Parse(value[0].ToString() + value[1]);
                var minutes = int.Parse(value[3].ToString()+ value[4]);
                if ((hours > 23) || (minutes > 59))
                {
                    throw new Exception();
                }
                return hours*60 + minutes;
            }
            catch
            {
                throw new Exception("Invalid time format of " + fieldName);
            }
        }

        public static string ValidateEmptyString(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("Validation: " + fieldName + " can't be empty.");
            }
            return value;
        }

        public static int ValidateId(string fieldName, string value)
        {
            int id;
            try
            {
                id = int.Parse(value);
            }
            catch
            {
                throw new Exception("Validation: " + string.Format("{0} can't be converted to int (value = '{1}').", fieldName, value));
            }
            if (id < 1)
            {
                throw new Exception("Validation: " + string.Format("{0} should be more than zero.", fieldName));
            }
            return id;
        }

        public static int ValidateInteger(string fieldName, string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                throw new Exception("Validation: " + string.Format("{0} can't be converted to int (value = '{1}').", fieldName, value));
            }
        }

        public static DateTime ValidateDateTime(string fieldName, string value)
        {
            try
            {
                DateTime date;
                if (!DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    throw new Exception("throw");
                }
                return date;
            }
            catch
            {
                throw new Exception("Validation: " + string.Format("{0} can't be converted to DateTime. Expected 'dd/MM/yyyy HH:mm:ss'. Value '{1}'.", fieldName, value));
            }
        }

        public static DateTime ValidateDate(string fieldName, string value)
        {
            try
            {
                DateTime date;
                if (!DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                {
                    throw new Exception("throw");
                }
                return date;
            }
            catch
            {
                throw new Exception("Validation: " + string.Format("{0} can't be converted to DateTime. Expected 'dd/MM/yyyy'. Value '{1}'.", fieldName, value));                
            }
        }

        public static bool ValidateBoolean(string fieldName, string value)
        {
            value = value.ToLower();
            if (value == "true" || value == "1" || value == "yes")
                return true;
            if (value == "false" || value == "0" || value == "no")
                return false;
            throw new Exception("Validation: " + string.Format("{0} can't be converted to Boolean. Expected true/false/1/0/yes/no", fieldName));
        }

        public static decimal? ValidateDecimal(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return decimal.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("Validation: " + string.Format("{0} has invalid floating point format.", fieldName));
            }
        }
    }
}
