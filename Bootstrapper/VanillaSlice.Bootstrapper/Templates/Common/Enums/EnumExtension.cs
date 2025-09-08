using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{ProjectName}}.Common.Enums
{
    public static class EnumExtension
    {
        public static TEnum? ToEnum<TEnum>(this string? value) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
