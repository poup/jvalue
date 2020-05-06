using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Halak
{
    // TODO add Assert
    public sealed class JsonWriter : IDisposable
    {
        private TextWriter underlyingWriter;
        private int offset;

        public int Offset => offset;

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
            this.offset = 0;
        }

        public void Dispose()
        {
            var disposingWriter = underlyingWriter;
            underlyingWriter = null;
            disposingWriter?.Dispose();
        }

        public void WriteStartArray() => underlyingWriter.Write('[');
        public void WriteEndArray() => underlyingWriter.Write(']');
        public void WriteStartObject() => underlyingWriter.Write('{');
        public void WriteEndObject() => underlyingWriter.Write('}');

        public void WriteNull() => underlyingWriter.Write(JValue.NullLiteral);
        public void WriteValue(bool value) => underlyingWriter.Write(value ? JValue.TrueLiteral : JValue.FalseLiteral);
        public void WriteValue(byte value) => underlyingWriter.Write(value);
        public void WriteValue(int value) => underlyingWriter.WriteInt32(value);
        public void WriteValue(long value) => underlyingWriter.WriteInt64(value);
        public void WriteValue(float value) => underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
        public void WriteValue(double value) => underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
        public void WriteValue(decimal value) => underlyingWriter.Write(value.ToString(NumberFormatInfo.InvariantInfo));
        public void WriteValue(string value) => underlyingWriter.WriteEscapedString(value);
        public void WriteValue(JValue value) => value.WriteTo(underlyingWriter);

        public void WriteCommaIf(int offset)
        {
            if (this.offset != offset)
            {
                underlyingWriter.Write(',');
            }

            this.offset++;
        }

        public void WritePropertyName(string key)
        {
            underlyingWriter.WriteEscapedString(key);
            underlyingWriter.Write(':');
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
                writer.Write(JValue.NullLiteral);
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
