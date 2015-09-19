﻿using System;
using System.Collections.Generic;

namespace Halak.JValueComparison
{
    static class JValueSide
    {
        public static void EnumerateArray(string source)
        {
            var data = JValue.Parse(source);
            foreach (var item in data.Array())
            {
                Program.Noop(item.AsInt());
            }
        }

        public static void QueryObject(string source, IEnumerable<string> keys)
        {
            var data = JValue.Parse(source);
            foreach (var item in keys)
            {
                Program.Noop(data[item].AsInt());
            }
        }
    }
}
