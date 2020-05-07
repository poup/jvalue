﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Assertions;

namespace Halak
{
    /// <summary>
    /// Super lightweight JSON Reader
    /// </summary>
    /// <seealso cref="http://www.json.org/"/>
    /// <seealso cref="https://github.com/halak/jvalue/"/>
    [PublicAPI]
    public readonly partial struct JValue : IComparable<JValue>, IEquatable<JValue>
    {
        #region TypeCode

        public enum TypeCode
        {
            Null,
            Boolean,
            Number,
            String,
            Array,
            Object,
        }


        public static TypeCode ComputeTypeCode(string source, int index, int length)
        {
            if (source == null)
            {
                return TypeCode.Null;
            }

            var end = BackwardSkipWhitespaces(source, length - 1) + 1;
            length = Math.Min(source.Length, end - index);

            if (length <= 0)
            {
                return TypeCode.Null;
            }

            switch (source[index])
            {
                case '"':
                    if (length < 2 || source[length - 1] != '"')
                        throw new JsonException("string not closed", source, index, length);

                    return TypeCode.String;

                case '[':
                    if (length < 2 || source[length - 1] != ']')
                        throw new JsonException("array not closed", source, index, length);

                    return TypeCode.Array;

                case '{':
                    if (length < 2 || source[length - 1] != '}')
                        throw new JsonException("object not closed", source, index, length);

                    return TypeCode.Object;

                case 't':
                    if (length != 4 || source[index + 1] != 'r' || source[index + 2] != 'u' || source[index + 3] != 'e')
                        throw new JsonException("Illegal boolean: expect true", source, index, length);

                    return TypeCode.Boolean;

                case 'f':
                    if (length != 5 || source[index + 1] != 'a' || source[index + 2] != 'l' || source[index + 3] != 's' || source[index + 4] != 'e')
                        throw new JsonException("Illegal boolean: expect false", source, index, length);

                    return TypeCode.Boolean;

                case 'n':
                    if (length != 4 || source[index + 1] != 'u' || source[index + 2] != 'l' || source[index + 3] != 'l')
                        throw new JsonException("Illegal null: expect null", source, index, length);

                    return TypeCode.Null;

                default:
                    // TODO checker is a number ?
                    return TypeCode.Number;
            }
        }

        #endregion


        #region Static Fields

        internal const string NullLiteral = "null";
        internal const string TrueLiteral = "true";
        internal const string FalseLiteral = "false";

        public static readonly JValue Null = new JValue(NullLiteral, TypeCode.Null);
        public static readonly JValue True = new JValue(TrueLiteral, TypeCode.Boolean);
        public static readonly JValue False = new JValue(FalseLiteral, TypeCode.Boolean);
        public static readonly JValue EmptyString = new JValue("\"\"", TypeCode.String);
        public static readonly JValue EmptyArray = new JValue("[]", TypeCode.Array);
        public static readonly JValue EmptyObject = new JValue("{}", TypeCode.Object);

        #endregion


        #region Fields

        public readonly string source;
        public readonly int startIndex;
        public readonly int length;
        public readonly TypeCode typeCode;

        #endregion


        #region Properties

        public int endIndex => startIndex + length - 1;
        public JValue this[string key] => Get(key);

        #endregion


        #region Indexer

        #endregion


        #region Constructors

        public static JValue Parse(string source)
        {
            if (source != null)
            {
                var index = SkipWhitespaces(source);
                var end = BackwardSkipWhitespaces(source, source.Length - 1) + 1;
                return new JValue(source, index, end - index);
            }
            return Null;
        }

        public JValue(bool value) : this(value ? TrueLiteral : FalseLiteral, TypeCode.Boolean)
        {
        }

        public JValue(int value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(long value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(ulong value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(float value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(double value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(decimal value) : this(value.ToString(NumberFormatInfo.InvariantInfo), TypeCode.Number)
        {
        }

        public JValue(string value)
        {
            if (value != null)
            {
                using (var writer = new StringWriter(new StringBuilder(value.Length + 2), CultureInfo.InvariantCulture))
                {
                    writer.WriteEscapedString(value);
                    source = writer.GetStringBuilder().ToString();
                    startIndex = 0;
                    length = source.Length;
                    typeCode = TypeCode.String;
                }
            }
            else
            {
                source = null;
                startIndex = 0;
                length = 0;
                typeCode = TypeCode.Null;
            }
        }


        internal JValue(string source, int startIndex, int length)
        {
            this.source = source;
            this.startIndex = startIndex;
            this.length = length;
            this.typeCode = ComputeTypeCode(source, startIndex, length);
        }

        private JValue(string source, TypeCode typeCode) : this(source, 0, source.Length, typeCode)
        {
        }

        private JValue(JValue original) : this(original.source, original.startIndex, original.length, original.typeCode)
        {
        }

        private JValue(string source, int startIndex, int length, TypeCode typeCode)
        {
            this.source = source;
            this.startIndex = startIndex;
            this.length = length;
            this.typeCode = typeCode;

            Assert.IsTrue(typeCode == ComputeTypeCode(source, startIndex, length));
        }

        #endregion


        #region Methods

        #region Convert

        public bool ToBoolean(bool defaultValue = false)
        {
            switch (typeCode)
            {
                case TypeCode.Null:    return defaultValue;
                case TypeCode.Boolean: return ToBooleanCore();
                case TypeCode.Number:  return ToDoubleCore(0.0) != 0.0;
                case TypeCode.String:  return length != 2; // two quotation marks
                case TypeCode.Array:   return true;
                case TypeCode.Object:  return true;
                default:               return defaultValue;
            }
        }

        public int ToInt32(int defaultValue = 0)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? 1 : 0;
                case TypeCode.Number:  return ToInt32Core(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToInt32Core(defaultValue);
                default:               return defaultValue;
            }
        }

        public long ToInt64(long defaultValue = 0)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? 1 : 0;
                case TypeCode.Number:  return ToInt64Core(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToInt64Core(defaultValue);
                default:               return defaultValue;
            }
        }

        public float ToSingle(float defaultValue = 0.0f)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? 1 : 0;
                case TypeCode.Number:  return ToSingleCore(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToSingleCore(defaultValue);
                default:               return defaultValue;
            }
        }

        public double ToDouble(double defaultValue = 0.0)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? 1.0 : 0.0;
                case TypeCode.Number:  return ToDoubleCore(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToDoubleCore(defaultValue);
                default:               return defaultValue;
            }
        }

        public decimal ToDecimal(decimal defaultValue = 0.0m)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? 1.0m : 0.0m;
                case TypeCode.Number:  return ToDecimalCore(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToDecimalCore(defaultValue);
                default:               return defaultValue;
            }
        }

        public JNumber ToNumber()
        {
            return ToNumber(JNumber.Zero);
        }

        public JNumber ToNumber(JNumber defaultValue)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? JNumber.One : JNumber.Zero;
                case TypeCode.Number:  return ToNumberCore(defaultValue);
                case TypeCode.String:  return ConvertForNumberParsing().ToNumberCore(defaultValue);
                default:               return JNumber.NaN;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ToBooleanCore()
            => source[startIndex] == 't';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ToInt32Core(int defaultValue)
            => JNumber.ParseInt32(source, startIndex, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long ToInt64Core(long defaultValue)
            => JNumber.ParseInt64(source, startIndex, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ToSingleCore(float defaultValue)
            => JNumber.ParseSingle(source, startIndex, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ToDoubleCore(double defaultValue)
            => JNumber.ParseDouble(source, startIndex, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private decimal ToDecimalCore(decimal defaultValue)
            => JNumber.ParseDecimal(source, startIndex, length, defaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JNumber ToNumberCore(JNumber defaultValue)
            => JNumber.TryParse(source, startIndex, out var value) ? value : defaultValue;

        public string ToUnescapedString(string defaultValue = "")
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return ToBooleanCore() ? TrueLiteral : FalseLiteral;
                case TypeCode.Number:  return source.Substring(startIndex, length);
                case TypeCode.String:  return ToUnescapedStringCore();
                default:               return defaultValue;
            }
        }

        private string ToUnescapedStringCore()
        {
            var sb = new StringBuilder(length);
            var enumerator = GetCharEnumerator();
            while (enumerator.MoveNext())
            {
                sb.Append(enumerator.Current);
            }
            return sb.ToString();
        }

        private JValue ConvertForNumberParsing()
        {
            var end = startIndex + length - 1;
            for (var i = startIndex + 1; i < end; i++)
            {
                if (source[i] == '\\')
                    return new JValue(ToUnescapedStringCore(), TypeCode.Number);
            }

            return new JValue(source, startIndex + 1, length - 2, TypeCode.Number);
        }

        #endregion


        #region Get

        private JValue Get(string key)
        {
            if (typeCode == TypeCode.Object)
            {
                foreach (var pair in GetObjectKeyValues())
                {
                    if (pair.Key.EqualsPropertyName(key))
                    {
                        return pair.Value;
                    }
                }
            }

            return Null;
        }


        private int GetElementCount()
        {
            Assert.IsTrue(typeCode == TypeCode.Array || typeCode == TypeCode.Object);

            var count = 0;
            var depth = 0;
            var end = startIndex + length - 1; // ignore } or ]
            for (var i = startIndex + 1; i < end; i++) // ignore { or [
            {
                switch (source[i])
                {
                    case ',':
                        if (depth == 0)
                            count++;
                        break;

                    case '[':
                    case '{':
                        depth++;
                        break;

                    case ']':
                    case '}':
                        depth--;
                        break;

                    case '"':
                        i = SkipString(i) - 1;
                        break;

                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    default:
                        if (count == 0)
                            count = 1;
                        break;
                }
            }

            if (depth != 0)
            {
                throw new JsonException($"{typeCode} is not closed", this);
            }

            return count;
        }

        #endregion


        #region Enumeration

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArrayEnumerator GetArrayItems() => new ArrayEnumerator(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectKeyValueEnumerator GetObjectKeyValues() => new ObjectKeyValueEnumerator(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CharEnumerator GetCharEnumerator() => new CharEnumerator(this);

        private int SkipValue(int index)
        {
            var end = startIndex + length;
            if (end <= index)
                return end;

            switch (source[index])
            {
                case '"': return SkipString(index);

                case '[':
                case '{':
                    return SkipBracket(index);

                default: return SkipLetterOrDigit(index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhitespaces(int index)
        {
            return SkipWhitespaces(source, index, startIndex + length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SkipWhitespaces(string source)
        {
            return SkipWhitespaces(source, 0, source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SkipWhitespaces(string source, int index, int end)
        {
            for (; index < end; index++)
            {
                switch (source[index])
                {
                    case ' ':
                    case ':':
                    case ',':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    default: return index;
                }
            }

            return end;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BackwardSkipWhitespaces(string source, int index)
        {
            for (; index >= 0; index--)
            {
                switch (source[index])
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    default: return index;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipLetterOrDigit(int index)
        {
            var end = startIndex + length;
            for (; index < end; index++)
            {
                switch (source[index])
                {
                    case ' ':
                    case ':':
                    case ',':
                    case ']':
                    case '}':
                    case '"':
                    case '\t':
                    case '\r':
                    case '\n':
                        return index;
                }
            }

            return end;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipString(int index)
        {
            var end = startIndex + length;
            index++;
            for (; index < end; index++)
            {
                switch (source[index])
                {
                    case '"': return index + 1;

                    case '\\':
                        index++;
                        break;
                }
            }
            throw new JsonException($"string not closed", this);

            return end;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipBracket(int index)
        {
            var end = startIndex + length;
            var depth = 0;
            for (; index < end; index++)
            {
                switch (source[index])
                {
                    case '[':
                    case '{':
                        depth++;
                        break;

                    case ']':
                    case '}':
                        depth--;

                        if (depth == 0)
                            return index + 1;

                        break;

                    case '"':
                        index = SkipString(index) - 1;
                        break;
                }
            }

            if (depth != 0)
            {
                throw new JsonException($"{typeCode} not closed", this);
            }
            return end;
        }

        #endregion


        #region Serialization

        public string Serialize(JsonWriter.Formatter formatter = null)
        {
            var builder = new StringBuilder(length);
            Serialize(builder, formatter);
            return builder.ToString();
        }

        public void Serialize(StringBuilder builder, JsonWriter.Formatter formatter = null)
        {
            using (var sw = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                var writer = new JsonWriter(sw);
                writer.SetFormatter(formatter);

                Serialize(writer, this);
            }
        }

        public void Serialize(JsonWriter writer) => Serialize(writer, this);

        internal void WriteTo(JsonWriter writer)
        {
            if (typeCode != TypeCode.Null)
            {
                var end = startIndex + length;
                for (var i = startIndex; i < end; i++)
                {
                    writer.WriteValue(source[i]);
                }
            }
            else
            {
                writer.WriteNull();
            }
        }

        private static void Serialize(JsonWriter writer, JValue value)
        {
            switch (value.typeCode)
            {
                case TypeCode.Array:
                    Serialize(writer, value.GetArrayItems());
                    break;

                case TypeCode.Object:
                    Serialize(writer, value.GetObjectKeyValues());
                    break;

                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        private static void Serialize(JsonWriter writer, ArrayEnumerator value)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                Serialize(writer, item);
            }

            writer.WriteEndArray();
        }

        private static void Serialize(JsonWriter writer, ObjectKeyValueEnumerator value)
        {
            writer.WriteStartObject();

            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key);
                Serialize(writer, pair.Value);
            }

            writer.WriteEndObject();
        }

        #endregion


        public override int GetHashCode()
        {
            switch (typeCode)
            {
                case TypeCode.Null:    return 0;
                case TypeCode.Boolean: return ToBoolean() ? 0x392307A6 : 0x63D95114;
                case TypeCode.Number:  return ToNumber().GetHashCode();
                case TypeCode.String:  return GetStringHashCode();
                case TypeCode.Array:   return GetArrayHashCode();
                case TypeCode.Object:  return GetObjectHashCode();
                default:               return 0;
            }
        }

        public bool Equals(string other)
        {
            if (other.Length != length)
            {
                return false;
            }

            for (int i = 0; i < length; ++i)
            {
                var j = i + startIndex;
                if (source[j] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool EqualsPropertyName(string name)
        {
            Assert.IsTrue(typeCode == TypeCode.String);

            var nameLength = name.Length;
            if (nameLength > length - 2)
            {
                return false;
            }

            int i = 0;
            var keyWithoutQuotes = new JValue(source, startIndex + 1, length - 2);
            foreach (var c in keyWithoutQuotes.GetCharEnumerator())
            {
                if (i >= nameLength)
                {
                    return false;
                }
                if (c != name[i])
                {
                    return false;
                }
                ++i;
            }

            return true;
        }

        public bool Equals(JValue other) => Equals(this, other);
        public int CompareTo(JValue other) => Compare(this, other);
        public override bool Equals(object obj) => obj is JValue other && Equals(other);

        public override string ToString()
        {
            if (typeCode != TypeCode.Null)
                return (startIndex == 0 && length == source.Length) ? source : source.Substring(startIndex, length);

            return NullLiteral;
        }


        #region HashCode

        private int GetStringHashCode()
        {
            Assert.IsTrue(typeCode == TypeCode.String);

            var enumerator = GetCharEnumerator();
            var hashCode = 0x219FFA9C;
            while (enumerator.MoveNext())
            {
                hashCode = HashCode.Combine(hashCode, enumerator.Current);
            }
            return hashCode;
        }

        private int GetArrayHashCode()
        {
            Assert.IsTrue(typeCode == TypeCode.Array);

            var hashCode = 0x12D398BA;
            foreach (var element in GetArrayItems())
            {
                hashCode = HashCode.Combine(hashCode, element.GetHashCode());
            }
            return hashCode;
        }

        private int GetObjectHashCode()
        {
            Assert.IsTrue(typeCode == TypeCode.Object);

            var hashCode = 0x50638734;
            foreach (var member in GetObjectKeyValues())
            {
                hashCode = HashCode.Combine(hashCode, member.Key.GetHashCode());
                hashCode = HashCode.Combine(hashCode, member.Value.GetHashCode());
            }
            return hashCode;
        }

        #endregion


        public static bool Equals(JValue left, JValue right)
        {
            var leftType = left.typeCode;
            var rightType = right.typeCode;
            if (leftType == rightType)
            {
                switch (leftType)
                {
                    case TypeCode.Null:    return true;
                    case TypeCode.Boolean: return left.ToBooleanCore() == right.ToBooleanCore();
                    case TypeCode.Number:  return JNumber.Equals(left.ToNumberCore(JNumber.NaN), right.ToNumberCore(JNumber.NaN));
                    case TypeCode.String:  return EqualsString(left, right);
                    case TypeCode.Array:   return SequenceEqual<JValue, ArrayEnumerator>(left.GetArrayItems(), right.GetArrayItems(), Equals);
                    case TypeCode.Object:  return SequenceEqual<KeyValuePair<JValue, JValue>, ObjectKeyValueEnumerator>(left.GetObjectKeyValues(), right.GetObjectKeyValues(), EqualsMember);
                }
            }

            return false;
        }

        public static int Compare(JValue left, JValue right)
        {
            var leftType = left.typeCode;
            var rightType = right.typeCode;
            if (leftType == rightType)
            {
                switch (leftType)
                {
                    case TypeCode.Null:    return 0;
                    case TypeCode.Boolean: return left.ToBooleanCore().CompareTo(right.ToBooleanCore());
                    case TypeCode.Number:  return JNumber.Compare(left.ToNumberCore(JNumber.NaN), right.ToNumberCore(JNumber.NaN));
                    case TypeCode.String:  return SequenceCompare<char, CharEnumerator>(left.GetCharEnumerator(), right.GetCharEnumerator(), (x, y) => x.CompareTo(y));
                    case TypeCode.Array:   return SequenceCompare(left.GetArrayItems(), right.GetArrayItems(), Compare);
                    case TypeCode.Object:  return SequenceCompare(left.GetObjectKeyValues().GetEnumerator(), right.GetObjectKeyValues(), CompareMember);
                    default:               return 0;
                }
            }
            return ((int)leftType).CompareTo((int)rightType);
        }

        private static int CompareMember(KeyValuePair<JValue, JValue> x, KeyValuePair<JValue, JValue> y)
        {
            var k = Compare(x.Key, y.Key);
            if (k != 0)
            {
                return k;
            }

            return Compare(x.Value, y.Value);
        }

        private static int SequenceCompare<T>(IEnumerator<T> a, IEnumerator<T> b, Func<T, T, int> compare) => SequenceCompare<T, IEnumerator<T>>(a, b, compare);

        private static int SequenceCompare<T, TEnumerator>(TEnumerator a, TEnumerator b, Func<T, T, int> compare) where TEnumerator : IEnumerator<T>
        {
            for (;;)
            {
                var aStep = a.MoveNext();
                var bStep = b.MoveNext();
                if (aStep && bStep)
                {
                    var result = compare(a.Current, b.Current);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        private static bool EqualsString(JValue a, JValue b) => SequenceEqual<char, CharEnumerator>(a.GetCharEnumerator(), b.GetCharEnumerator(), (x, y) => x == y);

        private static bool EqualsMember(KeyValuePair<JValue, JValue> x, KeyValuePair<JValue, JValue> y) => Equals(x.Key, y.Key) && Equals(x.Value, y.Value);

        private static bool SequenceEqual<T, TEnumerator>(TEnumerator a, TEnumerator b, Func<T, T, bool> equals) where TEnumerator : IEnumerator<T>
        {
            for (;;)
            {
                var aStep = a.MoveNext();
                var bStep = b.MoveNext();
                if (aStep && bStep)
                {
                    if (equals(a.Current, b.Current) == false)
                        return false;
                }
                else
                    return aStep == bStep;
            }
        }

        #endregion


        #region Implicit Conversion

        public static implicit operator bool(JValue value) => value.ToBoolean();
        public static implicit operator int(JValue value) => value.ToInt32();
        public static implicit operator long(JValue value) => value.ToInt64();
        public static implicit operator float(JValue value) => value.ToSingle();
        public static implicit operator double(JValue value) => value.ToDouble();
        public static implicit operator decimal(JValue value) => value.ToDecimal();
        public static implicit operator string(JValue value) => value.ToUnescapedString();
        public static implicit operator JValue(bool value) => new JValue(value);
        public static implicit operator JValue(int value) => new JValue(value);
        public static implicit operator JValue(long value) => new JValue(value);
        public static implicit operator JValue(float value) => new JValue(value);
        public static implicit operator JValue(double value) => new JValue(value);
        public static implicit operator JValue(decimal value) => new JValue(value);
        public static implicit operator JValue(string value) => new JValue(value);

        #endregion


        #region Operators

        public static bool operator==(JValue left, JValue right) => left.Equals(right);
        public static bool operator!=(JValue left, JValue right) => !left.Equals(right);
        public static bool operator<(JValue left, JValue right) => left.CompareTo(right) < 0;
        public static bool operator<=(JValue left, JValue right) => left.CompareTo(right) <= 0;
        public static bool operator>(JValue left, JValue right) => left.CompareTo(right) > 0;
        public static bool operator>=(JValue left, JValue right) => left.CompareTo(right) >= 0;

        #endregion
    }
}
