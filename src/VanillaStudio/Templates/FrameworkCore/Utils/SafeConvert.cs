namespace {{ProjectName}}.Framework.Utils
{
    public static class SafeConvert
    {
        public static int ToInt32(string? value, int defaultValue = 0)
        {
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public static long ToInt64(string? value, long defaultValue = 0)
        {
            return long.TryParse(value, out var result) ? result : defaultValue;
        }

        public static decimal ToDecimal(string? value, decimal defaultValue = 0)
        {
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }

        public static bool ToBoolean(string? value, bool defaultValue = false)
        {
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        public static DateTime ToDateTime(string? value, DateTime? defaultValue = null)
        {
            return DateTime.TryParse(value, out var result) ? result : defaultValue ?? DateTime.MinValue;
        }
    }
}
