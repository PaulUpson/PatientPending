using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PatientPending.Core
{
    public static class Extensions
    {
        public static string ToTitleCase(this string s)
        {
            return Regex.Replace(s, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static string ToTitleCase(this object e)
        {
            var value = e.ToString();
            return value.ToTitleCase();
        }

        public static string StripWhitespace(this string text)
        {
            return text.Replace(" ", "");
        }

        public static int[] ToIntArray(this long number) {
            return number.ToString(CultureInfo.InvariantCulture).ToIntArray();
        }

        public static int[] ToIntArray(this string number) {
            var result = number.ToIntEnumerable();
            return result.ToArray();
        }

        public static IEnumerable<int> ToIntEnumerable(this string number) {
            var charArray = number.ToCharArray();
            foreach (var digit in charArray) {
                int result;
                if (int.TryParse(digit.ToString(CultureInfo.InvariantCulture), out result))
                    yield return result;
            }
        }

        public static long ToLong(this IEnumerable<int> numberGroup) {
            return Int64.Parse(numberGroup.Select(x => x.ToString(CultureInfo.InvariantCulture))
                                   .Aggregate((complete, next) => complete + next));
        }

		public static void AppendWithIndent(this StringBuilder sb, string text, params object[] args) {
			sb.AppendFormat("\n\t\t{0}", String.Format(text, args));
		}

		public static void AppendWithDoubleIndent(this StringBuilder sb, string text, params object[] args)
		{
			sb.AppendFormat("\n\t\t\t\t{0}", String.Format(text, args));
		}

        public static TEnum AsEnum<TEnum> (this string value) where TEnum : struct
        {
            if (String.IsNullOrEmpty(value))
                throw new InvalidOperationException("Cannot cast null or empty string to Enum of type " +
                                                    typeof (TEnum).Name);
            TEnum result;
            if(!Enum.TryParse(value, true, out result))
            {
                throw new InvalidOperationException(String.Format("Cannot cast string value {0} as Enum of type {1}",
                                                                  value, typeof (TEnum).Name));
            }
            return result;
        }

    	public static T As<T>(this object target)
    	{
    		return (T)target;
    	}
    }
}