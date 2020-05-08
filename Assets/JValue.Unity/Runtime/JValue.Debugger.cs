using System.Collections.Generic;
using System.Diagnostics;

namespace Halak
{
    [DebuggerDisplay("{ToDebuggerDisplay(),nq}", Type = "{ToDebuggerType(),nq}")]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    readonly partial struct JValue
    {
        private const int EllipsisCount = 64;

        private string ToDebuggerType()
        {
            switch (typeCode)
            {
                case TypeCode.Null:    return "JValue.Null";
                case TypeCode.Boolean: return "JValue.Boolean";
                case TypeCode.Number:  return "JValue.Number";
                case TypeCode.String:  return "JValue.String";
                case TypeCode.Array:   return "JValue.Array";
                case TypeCode.Object:  return "JValue.Object";
                default:               return "JValue.Null";
            }
        }

        private string ToDebuggerDisplay()
        {
            switch (typeCode)
            {
                case TypeCode.Array:
                case TypeCode.Object:
                    var serialized = Serialize(JsonWriter.Formatter.compact);
                    if (serialized.Length > EllipsisCount)
                        serialized = serialized.Substring(0, EllipsisCount - 3) + "...";
                    var elementCount = GetElementCount();
                    return $"{serialized} ({elementCount.ToString()} {(elementCount != 1 ? "items" : "item")})";

                default: return ToString();
            }
        }

        [DebuggerDisplay("{valueString,nq}", Type = "{valueTypeString,nq}")]
        private readonly struct ArrayElement
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly JValue value;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public readonly string valueString;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public readonly string valueTypeString;

            public ArrayElement(JValue value)
            {
                this.value = value;
                this.valueString = value.ToDebuggerDisplay();
                this.valueTypeString = value.ToDebuggerType();
            }
        }

        [DebuggerDisplay("{valueString,nq}", Name = "[{key,nq}]", Type = "{valueTypeString,nq}")]
        private readonly struct ObjectMember
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public readonly string key;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public readonly JValue value;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public readonly string valueString;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public readonly string valueTypeString;

            public ObjectMember(string key, JValue value)
            {
                this.key = key;
                this.value = value;
                this.valueString = value.ToDebuggerDisplay();
                this.valueTypeString = value.ToDebuggerType();
            }
        }

        private sealed class DebuggerProxy
        {
            private readonly JValue value;
            public DebuggerProxy(JValue value)
            {
                this.value = value;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object Items
            {
                get
                {
                    switch (value.typeCode)
                    {
                        case TypeCode.Null:
                        case TypeCode.Boolean:
                        case TypeCode.Number:
                        case TypeCode.String:
                            return null;

                        case TypeCode.Array:
                            return GetArrayElements(value);

                        case TypeCode.Object:
                            return GetObjectMembers(value);

                        default:
                            return null;
                    }
                }
            }

            private static ArrayElement[] GetArrayElements(JValue value)
            {
                var list = new List<ArrayElement>();
                foreach (var item in value.GetArrayItems())
                {
                    list.Add(new ArrayElement(item));
                }
                return list.ToArray();
            }

            private static ObjectMember[] GetObjectMembers(JValue value)
            {
                var list = new List<ObjectMember>();
                foreach (var item in value.GetObjectKeyValues())
                {
                    list.Add(new ObjectMember(item.Key.ToString(), item.Value));
                }
                return list.ToArray();
            }
        }
    }
}
