using JetBrains.Annotations;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Halak
{
    [PublicAPI]
    public sealed partial class JsonWriter
    {
        private TextWriter m_underlyingWriter;
        private Formatter m_formatter = Formatter.compact;
        private bool m_needComma;

        public JsonWriter(int capacity)
            : this(new StringBuilder(capacity))
        {
        }

        public JsonWriter(StringBuilder builder)
            : this(new StringWriter(builder, CultureInfo.InvariantCulture))
        {
        }

        public JsonWriter(TextWriter writer)
        {
            this.m_underlyingWriter = writer;
        }

        public void SetFormatter(Formatter formatter)
        {
            m_formatter = formatter ?? Formatter.compact;
        }

        public void WritePropertyName(string key)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
            }

            m_underlyingWriter.WriteEscapedString(key);

            m_formatter.WritePropertySeparator(m_underlyingWriter);
            m_needComma = false;
        }

        public void WritePropertyName(JValue key)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
            }

            m_underlyingWriter.WriteSubString(key.source, key.startIndex, key.endIndex);

            m_formatter.WritePropertySeparator(m_underlyingWriter);
            m_needComma = false;
        }


        public void WriteStartArray()
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_formatter.WriteStartArrayToken(m_underlyingWriter);
        }

        public void WriteEndArray()
        {
            m_formatter.WriteEndArrayToken(m_underlyingWriter);
            m_needComma = true;
        }


        public void WriteStartObject()
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_formatter.WriteStartObjectToken(m_underlyingWriter);
        }

        public void WriteEndObject()
        {
            m_formatter.WriteEndObjectToken(m_underlyingWriter);
            m_needComma = true;
        }


        public void WriteNull()
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(JValue.nullArray);
            m_needComma = true;
        }


        public void WriteValue(bool value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value ? JValue.trueArray : JValue.falseArray);
            m_needComma = true;
        }

        public void WriteValue(bool? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(char value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value);
            m_needComma = true;
        }

        public void WriteValue(char? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(byte value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value);
            m_needComma = true;
        }

        public void WriteValue(byte? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(int value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.WriteInt32(value);
            m_needComma = true;
        }

        public void WriteValue(int? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(long value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.WriteInt64(value);
            m_needComma = true;
        }

        public void WriteValue(long? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(float value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            m_needComma = true;
        }

        public void WriteValue(float? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(double value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            m_needComma = true;
        }

        public void WriteValue(double? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(decimal value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            m_needComma = true;
        }

        public void WriteValue(decimal? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public void WriteValue(string value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value, 0, value.Length);
            }
        }


        public void WriteValue(string source, int startIndex, int length)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.WriteEscapedString(source, startIndex, length);
            m_needComma = true;
        }

        public void WriteValue(JValue value)
        {
            if (m_needComma)
            {
                m_formatter.WriteValueSeparator(m_underlyingWriter);
                m_needComma = false;
            }

            m_underlyingWriter.WriteSubString(value.source, value.startIndex, value.length);
            m_needComma = true;
        }


        public JValue BuildJson()
        {
            var txt = m_underlyingWriter.ToString();
            return new JValue(txt, 0, txt.Length);
        }
    }

    internal static class JsonTextWriterExtensions
    {
        public static void WriteInt32(this TextWriter writer, int value)
        {
            if (value == 0)
            {
                writer.Write('0');
                return;
            }

            if (value < 0)
            {
                if (value == int.MinValue)
                {
                    writer.Write(int.MinValue);
                    return;
                }

                value = -value;
                writer.Write('-');
            }

            if (value < 10)
                goto E0;
            if (value < 100)
                goto E1;
            if (value < 1000)
                goto E2;
            if (value < 10000)
                goto E3;
            if (value < 100000)
                goto E4;
            if (value < 1000000)
                goto E5;
            if (value < 10000000)
                goto E6;
            if (value < 100000000)
                goto E7;
            if (value < 1000000000)
                goto E8;

            goto E9;

            E9:
            writer.Write((char) ('0' + ((value / 1000000000) % 10)));
            E8:
            writer.Write((char) ('0' + ((value / 100000000) % 10)));
            E7:
            writer.Write((char) ('0' + ((value / 10000000) % 10)));
            E6:
            writer.Write((char) ('0' + ((value / 1000000) % 10)));
            E5:
            writer.Write((char) ('0' + ((value / 100000) % 10)));
            E4:
            writer.Write((char) ('0' + ((value / 10000) % 10)));
            E3:
            writer.Write((char) ('0' + ((value / 1000) % 10)));
            E2:
            writer.Write((char) ('0' + ((value / 100) % 10)));
            E1:
            writer.Write((char) ('0' + ((value / 10) % 10)));
            E0:
            writer.Write((char) ('0' + ((value / 1) % 10)));
        }

        public static void WriteInt64(this TextWriter writer, long value)
        {
            if (value == 0)
            {
                writer.Write('0');
                return;
            }

            if (value < 0)
            {
                if (value == long.MinValue)
                {
                    writer.Write(long.MinValue);
                    return;
                }

                value = -value;
                writer.Write('-');
            }

            if (value < 10L)
                goto E0;
            if (value < 100L)
                goto E1;
            if (value < 1000L)
                goto E2;
            if (value < 10000L)
                goto E3;
            if (value < 100000L)
                goto E4;
            if (value < 1000000L)
                goto E5;
            if (value < 10000000L)
                goto E6;
            if (value < 100000000L)
                goto E7;
            if (value < 1000000000L)
                goto E8;
            if (value < 10000000000L)
                goto E9;
            if (value < 100000000000L)
                goto E10;
            if (value < 1000000000000L)
                goto E11;
            if (value < 10000000000000L)
                goto E12;
            if (value < 100000000000000L)
                goto E13;
            if (value < 1000000000000000L)
                goto E14;
            if (value < 10000000000000000L)
                goto E15;
            if (value < 100000000000000000L)
                goto E16;
            if (value < 1000000000000000000L)
                goto E17;

            goto E18;

            E18:
            writer.Write((char) ('0' + ((value / 1000000000000000000L) % 10)));
            E17:
            writer.Write((char) ('0' + ((value / 100000000000000000L) % 10)));
            E16:
            writer.Write((char) ('0' + ((value / 10000000000000000L) % 10)));
            E15:
            writer.Write((char) ('0' + ((value / 1000000000000000L) % 10)));
            E14:
            writer.Write((char) ('0' + ((value / 100000000000000L) % 10)));
            E13:
            writer.Write((char) ('0' + ((value / 10000000000000L) % 10)));
            E12:
            writer.Write((char) ('0' + ((value / 1000000000000L) % 10)));
            E11:
            writer.Write((char) ('0' + ((value / 100000000000L) % 10)));
            E10:
            writer.Write((char) ('0' + ((value / 10000000000L) % 10)));
            E9:
            writer.Write((char) ('0' + ((value / 1000000000L) % 10)));
            E8:
            writer.Write((char) ('0' + ((value / 100000000L) % 10)));
            E7:
            writer.Write((char) ('0' + ((value / 10000000L) % 10)));
            E6:
            writer.Write((char) ('0' + ((value / 1000000L) % 10)));
            E5:
            writer.Write((char) ('0' + ((value / 100000L) % 10)));
            E4:
            writer.Write((char) ('0' + ((value / 10000L) % 10)));
            E3:
            writer.Write((char) ('0' + ((value / 1000L) % 10)));
            E2:
            writer.Write((char) ('0' + ((value / 100L) % 10)));
            E1:
            writer.Write((char) ('0' + ((value / 10L) % 10)));
            E0:
            writer.Write((char) ('0' + ((value / 1L) % 10)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteEscapedString(this TextWriter writer, string value)
        {
            writer.WriteEscapedString(value, 0, value.Length);
        }

        public static void WriteEscapedString(this TextWriter writer, string value, int startIndex, int length)
        {
            if (value == null)
            {
                writer.Write(JValue.nullArray);
                return;
            }

            writer.Write('"');

            for (int i = startIndex, end = startIndex + length; i < end; i++)
            {
                var c = value[i];

                switch (c)
                {
                    case '"':
                        WriteEscapedChar(writer, '"');
                        break;

                    case '\\':
                        WriteEscapedChar(writer, '\\');
                        break;

                    case '\n':
                        WriteEscapedChar(writer, 'n');
                        break;

                    case '\t':
                        WriteEscapedChar(writer, 't');
                        break;

                    case '\r':
                        WriteEscapedChar(writer, 'r');
                        break;

                    case '\b':
                        WriteEscapedChar(writer, 'b');
                        break;

                    case '\f':
                        WriteEscapedChar(writer, 'f');
                        break;

                    default:
                        if (c > '\u001F')
                        {
                            writer.Write(c);
                        }
                        else
                        {
                            WriteHexChar(writer, c);
                        }

                        break;
                }
            }

            writer.Write('"');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSubString(this TextWriter writer, string value, int startIndex, int length)
        {
            for (int i = startIndex, end = startIndex + length; i < end; ++i)
            {
                writer.Write(value[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEscapedChar(TextWriter writer, char value)
        {
            writer.Write('\\');
            writer.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteHexChar(TextWriter writer, char value)
        {
            const string HexChars = "0123456789ABCDEF";

            writer.Write('\\');
            writer.Write('u');
            writer.Write(HexChars[(value & 0xF000) >> 12]);
            writer.Write(HexChars[(value & 0x0F00) >> 8]);
            writer.Write(HexChars[(value & 0x00F0) >> 4]);
            writer.Write(HexChars[(value & 0x000F) >> 0]);
        }
    }
}