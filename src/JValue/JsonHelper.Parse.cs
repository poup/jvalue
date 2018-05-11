﻿using System;

namespace Halak
{
    public static partial class JsonHelper
    {
        public static int Parse(string s, int startIndex, int length, int defaultValue)
        {
            if (length <= 0)
                return defaultValue;

            length += startIndex;

            var result = 0;
            if (s[startIndex] != '-')
            {
                if (s[startIndex] == '+')
                    startIndex++;

                for (var i = startIndex; i < length; i++)
                {
                    if ('0' <= s[i] && s[i] <= '9')
                    {
                        result = (result * 10) + (s[i] - '0');
                        if (result < 0) // overflow
                            return defaultValue;
                    }
                    else
                        return defaultValue;
                }
            }
            else
            {
                for (var i = startIndex + 1; i < length; i++)
                {
                    if ('0' <= s[i] && s[i] <= '9')
                    {
                        result = (result * 10) - (s[i] - '0');
                        if (result > 0)  // underflow
                            return defaultValue;
                    }
                    else
                        return defaultValue;
                }
            }

            return result;
        }

        public static long Parse(string s, int startIndex, int length, long defaultValue)
        {
            if (length <= 0)
                return defaultValue;

            length += startIndex;

            var result = 0L;
            if (s[startIndex] != '-')
            {
                if (s[startIndex] == '+')
                    startIndex++;

                for (var i = startIndex; i < length; i++)
                {
                    if ('0' <= s[i] && s[i] <= '9')
                    {
                        result = (result * 10L) + (s[i] - '0');
                        if (result < 0L) // overflow
                            return defaultValue;
                    }
                    else
                        return defaultValue;
                }
            }
            else
            {
                for (var i = startIndex + 1; i < length; i++)
                {
                    if ('0' <= s[i] && s[i] <= '9')
                    {
                        result = (result * 10L) - (s[i] - '0');
                        if (result > 0L)  // underflow
                            return defaultValue;
                    }
                    else
                        return defaultValue;
                }
            }

            return result;
        }

        public static float Parse(string s, int startIndex, int length, float defaultValue)
        {
            return (float)Parse(s, startIndex, length, (double)defaultValue);
        }

        public static double Parse(string s, int startIndex, int length, double defaultValue)
        {
            if (length <= 0)
                return defaultValue;

            var i = startIndex;
            if (s[startIndex] == '-' || s[startIndex] == '+')
                i++;

            length += startIndex;  // length => end
            var mantissa = 0L;
            if (i < length && '0' <= s[i] && s[i] <= '9')
            {
                mantissa = (mantissa * 10) + (s[i] - '0');
                i++;
            }
            else
                return defaultValue;

            for (; i < length; i++)
            {
                if ('0' <= s[i] && s[i] <= '9')
                    mantissa = (mantissa * 10) + (s[i] - '0');
                else if (s[i] == '.' || s[i] == 'e' || s[i] == 'E')
                    break;
                else
                    return defaultValue;
            }

            var exponent = 0;
            if (i < length && s[i] == '.')
            {
                i++;
                for (; i < length; i++, exponent++)
                {
                    if ('0' <= s[i] && s[i] <= '9')
                        mantissa = (mantissa * 10) + (s[i] - '0');
                    else if (s[i] == 'e' || s[i] == 'E')
                        break;
                    else
                        return defaultValue;
                }
            }

            if (i < length)
                exponent -= Parse(s, i + 1, length - (i + 1), 0);

            // defaultValue => result
            if (exponent != 0)
                defaultValue = mantissa / Pow10(exponent);
            else
                defaultValue = mantissa;

            if (s[startIndex] == '-')
                defaultValue = -defaultValue;

            return defaultValue;
        }

        public static double Pow10(int d) => (((d + Power10Bias) & int.MaxValue) < Power10Count ? Power10[d + Power10Bias] : Pow10Actually(d));
        private static double Pow10Actually(int d) => Math.Pow(10.0, d);

        private const int Power10Bias = 12;
        private const int Power10Count = 24;
        private static readonly double[] Power10 = new[]
        {
            1E-12, 1E-11, 1E-10, 1E-9, 1E-8, 1E-7, 1E-6, 1E-5, 1E-4, 1E-3, 1E-2, 1E-1,
            1E+0, 1E+1, 1E+2, 1E+3, 1E+4, 1E+5, 1E+6, 1E+7, 1E+8, 1E+9, 1E+10, 1E+11,
        };
    }
}
