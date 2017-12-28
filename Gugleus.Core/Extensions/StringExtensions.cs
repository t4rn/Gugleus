using System;

namespace Gugleus.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidUri(this string str)
        {
            Uri validatedUri;
            return Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out validatedUri);
        }
    }
}
