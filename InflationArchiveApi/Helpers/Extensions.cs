﻿namespace InflationArchive.Helpers
{
    public static class Extensions
    {
        public static string OnlyFirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.ToLowerInvariant().AsSpan(1))
            };
        }
    }
}
