using System;
using System.Linq;

namespace Gugleus.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidUri(this string str)
        {
            Uri validatedUri;
            return Uri.TryCreate(str, UriKind.Absolute, out validatedUri);
        }

        public static string Shorten(this string str, int chars)
        {
            if (!string.IsNullOrWhiteSpace(str) && str.Length > chars)
                return str.Substring(0, chars);

            return str;
        }

        public static bool In(this string instance, params string[] strings)
        {
            return strings.Contains(instance);
        }
    }
}
