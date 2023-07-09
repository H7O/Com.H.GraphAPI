using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.H.GraphAPI.Mail
{
    internal static class EmbeddedExt
    {
        internal static bool EqualsIgnoreCase(
            this string originalString,
            string stringToCompare)
        {
            var isNullEqual = originalString.IsNullEqual(stringToCompare);
            if (isNullEqual != null) return isNullEqual.Value;
            return
                originalString
                .ToUpper(CultureInfo.InvariantCulture)
                .Equals(stringToCompare.ToUpper(CultureInfo.InvariantCulture));

        }

        internal static bool? IsNullEqual(
            this string originalString,
            string stringToCompare)
        {
            if (originalString == null && stringToCompare == null) return true;
            if ((originalString != null && stringToCompare == null)
                ||
                (originalString == null && stringToCompare != null)
                ) return false;
            return null;
        }
        internal static string EnsureParentDirectory(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException($"{nameof(path)} contains invalid characters.");
            if (Directory.Exists(Directory.GetParent(path).FullName))
                return path;
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            return path;
        }
        internal static byte[] ReadFully(this Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        internal static bool ContainsInvalidFileNameChars(this string str)
        {
            return str.Any(Path.GetInvalidFileNameChars().Contains);
        }

    }
}
