﻿using System;
using System.Collections.Generic;

namespace Halak.JValueComparison
{
    static class NewtonsoftJsonSide
    {
        static Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        public static void EnumerateArray(string source)
        {
            var data = (object[])serializer.Deserialize(new System.IO.StringReader(source), typeof(object[]));
            foreach (var item in data)
            {
                Program.Noop(item);
            }
        }

        public static void QueryObject(string source, IEnumerable<string> keys)
        {
            var data = (Dictionary<string, object>)serializer.Deserialize(new System.IO.StringReader(source), typeof(Dictionary<string, object>));
            foreach (var item in keys)
            {
                Program.Noop(data[item]);
            }
        }
    }
}
