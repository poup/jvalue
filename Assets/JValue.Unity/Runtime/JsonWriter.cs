using JetBrains.Annotations;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Halak
{
    // TODO add Assert
    [PublicAPI]
    public sealed partial class JsonWriter : IDisposable
    {
        private TextWriter underlyingWriter;
        private Formatter m_formatter = Formatter.compact;
        private bool needComma;

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
            this.underlyingWriter = writer;
        }

        public void SetFormatter(Formatter formatter)
        {
            m_formatter = formatter ?? Formatter.compact;
        }

        public void Dispose()
        {
            underlyingWriter?.Dispose();
            underlyingWriter = null;
        }

        public void WritePropertyName(string key)
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
            }

            underlyingWriter.WriteEscapedString(key);

            m_formatter.WritePropertySeparator(underlyingWriter);
            needComma = false;
        }


        public void WriteStartArray()
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            m_formatter.WriteStartArrayToken(underlyingWriter);
        }

        public void WriteEndArray()
        {
            m_formatter.WriteEndArrayToken(underlyingWriter);
            needComma = true;
        }


        public void WriteStartObject()
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            m_formatter.WriteStartObjectToken(underlyingWriter);
        }

        public void WriteEndObject()
        {
            m_formatter.WriteEndObjectToken(underlyingWriter);
            needComma = true;
        }


        public void WriteNull()
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }

            underlyingWriter.Write(JValue.nullArray);
            needComma = true;
        }


        public void WriteValue(bool value)
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value ? JValue.trueArray : JValue.falseArray);
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value);
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value);
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.WriteInt32(value);
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.WriteInt64(value);
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
            needComma = true;
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
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            underlyingWriter.WriteEscapedString(value);
            needComma = true;
        }

        public void WriteValue(JValue value)
        {
            if (needComma)
            {
                m_formatter.WriteValueSeparator(underlyingWriter);
                needComma = false;
            }
            value.WriteTo(this);
            needComma = true;
        }


        public JValue BuildJson()
        {
            if (underlyingWriter is StringWriter stringWriter)
            {
                var stringBuilder = stringWriter.GetStringBuilder();
                return new JValue(stringBuilder.ToString(), 0, stringBuilder.Length);
            }
            throw new InvalidOperationException();
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
            writer.Write((char)('0' + ((value / 1000000000) % 10)));
            E8:
            writer.Write((char)('0' + ((value / 100000000) % 10)));
            E7:
            writer.Write((char)('0' + ((value / 10000000) % 10)));
            E6:
            writer.Write((char)('0' + ((value / 1000000) % 10)));
            E5:
            writer.Write((char)('0' + ((value / 100000) % 10)));
            E4:
            writer.Write((char)('0' + ((value / 10000) % 10)));
            E3:
            writer.Write((char)('0' + ((value / 1000) % 10)));
            E2:
            writer.Write((char)('0' + ((value / 100) % 10)));
            E1:
            writer.Write((char)('0' + ((value / 10) % 10)));
            E0:
            writer.Write((char)('0' + ((value / 1) % 10)));
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
            writer.Write((char)('0' + ((value / 1000000000000000000L) % 10)));
            E17:
            writer.Write((char)('0' + ((value / 100000000000000000L) % 10)));
            E16:
            writer.Write((char)('0' + ((value / 10000000000000000L) % 10)));
            E15:
            writer.Write((char)('0' + ((value / 1000000000000000L) % 10)));
            E14:
            writer.Write((char)('0' + ((value / 100000000000000L) % 10)));
            E13:
            writer.Write((char)('0' + ((value / 10000000000000L) % 10)));
            E12:
            writer.Write((char)('0' + ((value / 1000000000000L) % 10)));
            E11:
            writer.Write((char)('0' + ((value / 100000000000L) % 10)));
            E10:
            writer.Write((char)('0' + ((value / 10000000000L) % 10)));
            E9:
            writer.Write((char)('0' + ((value / 1000000000L) % 10)));
            E8:
            writer.Write((char)('0' + ((value / 100000000L) % 10)));
            E7:
            writer.Write((char)('0' + ((value / 10000000L) % 10)));
            E6:
            writer.Write((char)('0' + ((value / 1000000L) % 10)));
            E5:
            writer.Write((char)('0' + ((value / 100000L) % 10)));
            E4:
            writer.Write((char)('0' + ((value / 10000L) % 10)));
            E3:
            writer.Write((char)('0' + ((value / 1000L) % 10)));
            E2:
            writer.Write((char)('0' + ((value / 100L) % 10)));
            E1:
            writer.Write((char)('0' + ((value / 10L) % 10)));
            E0:
            writer.Write((char)('0' + ((value / 1L) % 10)));
        }

        public static void WriteEscapedString(this TextWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write(JValue.nullArray);
                return;
            }
            writer.Write('"');

            for (var i = 0; i < value.Length; i++)
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
