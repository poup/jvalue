using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Halak
{
    public readonly partial struct JValue
    {
        public readonly struct KeyValuePair
        {
            public readonly JValue Key;
            public readonly JValue Value;

            public KeyValuePair(JValue key, JValue value)
            {
                this.Key = key;
                this.Value = value;
            }

            public override string ToString()
            {
                return $"[{Key.ToString()}, {Value.ToString()}]";
            }
        }

        public struct ObjectKeyValueEnumerator : IEnumerator<KeyValuePair>
        {
            private readonly JValue m_source;
            private readonly int m_endIndex;

            private int m_nextIndex;
            private KeyValuePair m_current;

            public KeyValuePair Current => m_current;
            object IEnumerator.Current => m_current;

            internal ObjectKeyValueEnumerator(JValue value)
            {
                Assert.IsTrue(value.typeCode == TypeCode.Object);

                m_source = value;
                m_endIndex = value.startIndex + value.length - 1;

                m_nextIndex = value.SkipWhitespaces(value.startIndex+1);
                m_current = default;
            }


            [UsedImplicitly]
            public ObjectKeyValueEnumerator GetEnumerator()
            {
                return this;
            }

            [UsedImplicitly]
            public bool MoveNext()
            {
                if (m_nextIndex >= m_endIndex)
                {
                    return false;
                }

                var source = m_source;
                var keyStart = m_nextIndex;
                var keyEnd = source.SkipString(keyStart);

                var sourceString = source.source;

                if (sourceString[keyStart] != '"' || sourceString[keyEnd-1] != '"')
                {
                    throw new JsonException("expected property name (quoted string)", sourceString, keyStart, keyEnd-keyStart);
                }

                var valueStart = source.SkipWhitespaces(keyEnd + 1);
                var valueEnd = source.SkipValue(valueStart);

                m_current = new KeyValuePair(
                    new JValue(sourceString, keyStart, keyEnd - keyStart),
                    new JValue(sourceString, valueStart, valueEnd - valueStart)
                );

                m_nextIndex = source.SkipWhitespaces(valueEnd + 1);
                return true;
            }

            public void Reset()
            {
                m_nextIndex = m_source.SkipWhitespaces(m_source.startIndex+1);
                m_current = default;
            }

            public void Dispose()
            {
            }

            public Dictionary<string, JValue> ToDictionary()
            {
                var dictionary = new Dictionary<string, JValue>();
                foreach (var pair in this)
                {
                    dictionary[pair.Key.ToString()] = pair.Value;
                }
                return dictionary;
            }
        }
    }
}
