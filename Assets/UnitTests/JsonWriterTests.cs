using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Halak.Tests
{
    public class JsonWriterTests
    {
        public class FormatterTests
        {
            private static string Generate(JsonWriter.Formatter formatter)
            {
                TextWriter writer = new StringWriter();

                formatter.WriteStartObjectToken(writer);

                writer.Write("\"int1\"");
                formatter.WritePropertySeparator(writer);
                writer.Write(1);
                formatter.WriteValueSeparator(writer);

                writer.Write("\"array1\"");
                formatter.WritePropertySeparator(writer);
                formatter.WriteStartArrayToken(writer);
                {
                    writer.Write(11);
                    formatter.WriteValueSeparator(writer);
                    writer.Write(12);
                    formatter.WriteValueSeparator(writer);
                    writer.Write(13);
                }
                formatter.WriteEndArrayToken(writer);
                formatter.WriteValueSeparator(writer);

                writer.Write("\"object1\"");
                formatter.WritePropertySeparator(writer);
                formatter.WriteStartObjectToken(writer);
                {
                    writer.Write("\"o_int1\"");
                    formatter.WritePropertySeparator(writer);
                    writer.Write(100);
                    formatter.WriteValueSeparator(writer);

                    writer.Write("\"o_array1\"");
                    formatter.WritePropertySeparator(writer);
                    formatter.WriteStartArrayToken(writer);
                    {
                        writer.Write(111);
                        formatter.WriteValueSeparator(writer);
                        writer.Write(112);
                        formatter.WriteValueSeparator(writer);
                        writer.Write(113);
                    }
                    formatter.WriteEndArrayToken(writer);
                }
                formatter.WriteEndObjectToken(writer);

                formatter.WriteEndObjectToken(writer);

                return writer.ToString();
            }

            private static readonly string prettyExpected = @"{
  ""int1"": 1,
  ""array1"": [
    11,
    12,
    13
  ],
  ""object1"": {
    ""o_int1"": 100,
    ""o_array1"": [
      111,
      112,
      113
    ]
  }
}";

            [Test]
            public void PrettyFormatter()
            {
                var text = Generate(JsonWriter.Formatter.pretty);
                Assert.AreEqual(prettyExpected, text);
            }

            [Test]
            public void CompactFormatter()
            {
                var text = Generate(JsonWriter.Formatter.compact);
                var expected = prettyExpected.Replace(" ", "").Replace("\n", "");
                Assert.AreEqual(expected, text);
            }

            [Test]
            public void CompactLineByLineFormatter()
            {
                var text = Generate(JsonWriter.Formatter.compactLineByLine);
                var expected = prettyExpected.Replace(" ", "");
                Assert.AreEqual(expected, text);
            }
        }


        private string Write(Action<JsonWriter> action)
        {
            var sb = new StringBuilder();
            var writer = new JsonWriter(sb);
            writer.SetFormatter(JsonWriter.Formatter.compact);

            action(writer);

            return sb.ToString();
        }

        [Test]
        public void JsonWriter_WriteArray()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("array");
                writer.WriteStartArray();
                writer.WriteValue(10);
                writer.WriteValue(20);
                writer.WriteValue(30);
                writer.WriteEndArray();
            });

            var expected = "\"array\":[10,20,30]";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteObject()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("object");
                writer.WriteStartObject();

                writer.WritePropertyName("int1");
                writer.WriteValue(10);

                writer.WritePropertyName("array");
                writer.WriteStartArray();
                writer.WriteValue(10);
                writer.WriteValue(20);
                writer.WriteValue(30);
                writer.WriteEndArray();
                writer.WriteEndObject();
            });

            var expected = "\"object\":{\"int1\":10,\"array\":[10,20,30]}";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteBool()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("t");
                writer.WriteValue(true);

                writer.WritePropertyName("f");
                writer.WriteValue(false);

                writer.WritePropertyName("n");
                writer.WriteValue((bool?) null);
            });

            var expected = "\"t\":true,\"f\":false,\"n\":null";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteInt()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("v");
                writer.WriteValue(10);

                writer.WritePropertyName("nv");
                writer.WriteValue(-10);

                writer.WritePropertyName("n");
                writer.WriteValue((int?) null);
            });

            var expected = "\"v\":10,\"nv\":-10,\"n\":null";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteFloat()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("v");
                writer.WriteValue(10.0f);

                writer.WritePropertyName("v");
                writer.WriteValue(10.1f);

                writer.WritePropertyName("n");
                writer.WriteValue((float?) null);
            });

            var expected = "\"v\":10,\"v\":10.1,\"n\":null";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteString()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("v1");
                writer.WriteValue("youpi");

                writer.WritePropertyName("v2");
                writer.WriteValue("");

                writer.WritePropertyName("n");
                writer.WriteValue((string) null);
            });

            var expected = "\"v1\":\"youpi\",\"v2\":\"\",\"n\":null";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_WriteJValue()
        {
            var result = Write(writer =>
            {
                writer.WritePropertyName("v1");
                writer.WriteValue(new JValue("toto"));

                writer.WritePropertyName("v2");
                writer.WriteValue(new JValue(10));
            });

            var expected = "\"v1\":\"toto\",\"v2\":10";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JsonWriter_BuildJValue()
        {
            var sb = new StringBuilder();
            var writer = new JsonWriter(sb);
            writer.SetFormatter(JsonWriter.Formatter.compact);

            writer.WritePropertyName("object");
            writer.WriteStartObject();

            writer.WritePropertyName("int1");
            writer.WriteValue(10);

            writer.WritePropertyName("array");
            writer.WriteStartArray();
            writer.WriteValue(10);
            writer.WriteValue(20);
            writer.WriteValue(30);
            writer.WriteEndArray();
            writer.WriteEndObject();

            var jvalue = writer.BuildJson();

            var result = Write(w =>
            {
                w.WritePropertyName("v1");
                w.WriteStartObject();
                w.WriteValue(jvalue);
                w.WriteEndObject();
            });

            var expected = "\"v1\":{\"object\":{\"int1\":10,\"array\":[10,20,30]}}";
            Assert.AreEqual(expected, result);
        }
    }
}